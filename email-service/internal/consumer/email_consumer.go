package consumer

import (
	"context"
	"encoding/json"
	"fmt"
	"strconv"
	"sync"

	"email-service/internal/config"
	"email-service/internal/domain"
	"email-service/internal/service"
	"email-service/pkg/logger"

	amqp "github.com/rabbitmq/amqp091-go"
)

type EmailConsumer struct {
	cfg          *config.RabbitMQConfig
	emailService service.EmailService
	logger       *logger.Logger
}

func NewEmailConsumer(
	cfg *config.RabbitMQConfig,
	svc service.EmailService,
	l *logger.Logger,
) *EmailConsumer {
	return &EmailConsumer{
		cfg:          cfg,
		emailService: svc,
		logger:       l.WithComponent("email_consumer"),
	}
}

func (c *EmailConsumer) Run(ctx context.Context) error {
	conn, err := amqp.Dial(c.cfg.URL)
	if err != nil {
		return fmt.Errorf("failed to connect to RabbitMQ: %w", err)
	}
	defer conn.Close()

	ch, err := conn.Channel()
	if err != nil {
		return fmt.Errorf("failed to open channel: %w", err)
	}
	defer ch.Close()

	prefetchCount := 100
	if err := ch.Qos(prefetchCount, 0, false); err != nil {
		return fmt.Errorf("failed to set QoS: %w", err)
	}

	args := amqp.Table{
		"x-dead-letter-exchange":    "system.dlx",
		"x-dead-letter-routing-key": domain.SendEmailQueue + ".dlq",
	}

	_, err = ch.QueueDeclare(
		domain.SendEmailQueue,
		true,
		false,
		false,
		false,
		args,
	)
	if err != nil {
		return fmt.Errorf("failed to declare queue %s: %w", domain.SendEmailQueue, err)
	}

	deliveries, err := ch.Consume(
		domain.SendEmailQueue,
		"email-service-send-email-consumer",
		false,
		false,
		false,
		false,
		nil,
	)
	if err != nil {
		return fmt.Errorf("failed to register consumer: %w", err)
	}

	c.logger.Info().
		Str("queue", domain.SendEmailQueue).
		Msg("Send email consumer started")

	var wg sync.WaitGroup
	sem := make(chan struct{}, prefetchCount)

	for {
		select {
		case <-ctx.Done():
			c.logger.Info().Msg("Delete file consumer shutting down")
			wg.Wait()
			return nil
		case d, ok := <-deliveries:
			if !ok {
				c.logger.Info().Msg("RabbitMQ channel closed, shutting down...")
				return nil
			}
			sem <- struct{}{}
			wg.Add(1)
			go func(msg amqp.Delivery) {
				defer wg.Done()
				defer func() { <-sem }()
				c.handleMessage(msg)
			}(d)
		}
	}
}

func (c *EmailConsumer) handleMessage(d amqp.Delivery) {
	nack := func(requeue bool) {
		if err := d.Nack(false, requeue); err != nil {
			c.logger.Error().Err(err).Msg("Failed to nack message")
		}
	}

	var msg domain.SendEmailMessage
	if err := json.Unmarshal(d.Body, &msg); err != nil {
		c.logger.Error().Err(err).
			Str("body", string(d.Body)).
			Msg("Failed to unmarshal send email message")
		nack(false) // Don't requeue malformed JSON
		return
	}

	if msg.Email == "" || msg.Code == "" {
		c.logger.Warn().
			Str("email", msg.Email).
			Str("code", msg.Code).
			Msg("Missing email or code")
		nack(false) // Drop invalid messages
		return
	}

	c.logger.Info().
		Str("email", msg.Email).
		Str("code", msg.Code).
		Str("topic", strconv.Itoa(msg.EmailTopic)).
		Msg("Sending email")

	var err error
	switch msg.EmailTopic {
	case domain.Confirm:
		err = c.emailService.SendConfirmationCode(msg.Email, msg.Code)
	case domain.Restore:
		err = c.emailService.SendRestoreToken(msg.Email, msg.Code)
	default:
		c.logger.Warn().
			Str("email_topic", strconv.Itoa(msg.EmailTopic)).
			Msg("Invalid email topic")
		nack(false) // Drop invalid messages
		return
	}

	if err != nil {
		c.logger.Error().Err(err).
			Str("email", msg.Email).
			Msg("Failed to send email")

		// TODO: Create retry mechanism
		nack(false)
		return
	}

	if err := d.Ack(false); err != nil {
		c.logger.Error().Err(err).
			Str("email", msg.Email).
			Msg("Failed to ack message")
	}

	c.logger.Info().
		Str("email", msg.Email).
		Msg("Successfully sent email")
}
