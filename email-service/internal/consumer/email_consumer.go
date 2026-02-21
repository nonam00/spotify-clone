package consumer

import (
	"context"
	"encoding/json"
	"fmt"
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
	queueName    string
	handlerType  string // "confirm" or "restore"
}

func NewEmailConsumer(
	cfg *config.RabbitMQConfig,
	svc service.EmailService,
	l *logger.Logger,
	queue string,
	handlerType string,
) *EmailConsumer {
	return &EmailConsumer{
		cfg:          cfg,
		emailService: svc,
		logger:       l.WithComponent("email_consumer_" + handlerType),
		queueName:    queue,
		handlerType:  handlerType,
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

	_, err = ch.QueueDeclare(
		c.queueName,
		true,
		false,
		false,
		false,
		nil,
	)
	if err != nil {
		c.logger.Fatal().
			Err(err).
			Str("queue", c.queueName).
			Msg("failed to declare queue")
	}

	prefetchCount := 50
	_ = ch.Qos(prefetchCount, 0, false)

	deliveries, err := ch.Consume(c.queueName, "", false, false, false, false, nil)
	if err != nil {
		return err
	}

	var wg sync.WaitGroup
	sem := make(chan struct{}, prefetchCount)

	for {
		select {
		case <-ctx.Done():
			wg.Wait()
			return nil
		case d, ok := <-deliveries:
			if !ok {
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
	var msg domain.SendEmailMessage
	if err := json.Unmarshal(d.Body, &msg); err != nil {
		c.logger.Error().Err(err).Msg("Failed to unmarshal message")
		_ = d.Nack(false, false)
		return
	}

	var err error
	if c.handlerType == "confirm" {
		err = c.emailService.SendConfirmationCode(msg.Email, msg.Code)
	} else {
		err = c.emailService.SendRestoreToken(msg.Email, msg.Code)
	}

	if err != nil {
		c.logger.Error().Err(err).Str("email", msg.Email).Msg("Failed to send email")
		_ = d.Nack(false, true) // Requeue on SMTP failure
		return
	}

	_ = d.Ack(false)
}
