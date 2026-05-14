package consumer

import (
	"context"
	"encoding/json"
	"errors"
	"fmt"
	"sync"

	"file-service/internal/config"
	"file-service/internal/domain"
	"file-service/internal/service"
	"file-service/pkg/logger"

	amqp "github.com/rabbitmq/amqp091-go"
)

type DeleteFileConsumer struct {
	cfg         *config.RabbitMQConfig
	fileService service.FileService
	logger      *logger.Logger
}

func NewDeleteFileConsumer(
	cfg *config.RabbitMQConfig,
	fileService service.FileService,
	l *logger.Logger,
) *DeleteFileConsumer {
	return &DeleteFileConsumer{
		cfg:         cfg,
		fileService: fileService,
		logger:      l.WithComponent("delete_file_consumer"),
	}
}

func (c *DeleteFileConsumer) Run(ctx context.Context) error {
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

	// Setting QoS (Prefetch Count) to prevent OOM
	prefetchCount := 100

	if err := ch.Qos(prefetchCount, 0, false); err != nil {
		return fmt.Errorf("failed to set QoS: %w", err)
	}

	args := amqp.Table{
		"x-dead-letter-exchange":    "system.dlx",
		"x-dead-letter-routing-key": domain.DeleteFileQueue + ".dlq",
	}

	_, err = ch.QueueDeclare(
		domain.DeleteFileQueue,
		true,
		false,
		false,
		false,
		args,
	)
	if err != nil {
		return fmt.Errorf("failed to declare queue %s: %w", domain.DeleteFileQueue, err)
	}

	deliveries, err := ch.Consume(
		domain.DeleteFileQueue,
		"file-service-delete-consumer",
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
		Str("queue", domain.DeleteFileQueue).
		Msg("Delete file consumer started")

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
				wg.Wait()
				return nil
			}

			sem <- struct{}{} // Acquire token
			wg.Add(1)

			go func(delivery amqp.Delivery) {
				defer wg.Done()
				defer func() { <-sem }() // Release token
				c.handleMessage(ctx, delivery)
			}(d)
		}
	}
}

func (c *DeleteFileConsumer) handleMessage(ctx context.Context, d amqp.Delivery) {
	nack := func(requeue bool) {
		if err := d.Nack(false, requeue); err != nil {
			c.logger.Error().Err(err).Msg("Failed to nack message")
		}
	}

	var msg domain.DeleteFileMessage
	if err := json.Unmarshal(d.Body, &msg); err != nil {
		c.logger.Error().Err(err).
			Str("body", string(d.Body)).
			Msg("Failed to unmarshal delete file message")
		nack(false) // Don't requeue malformed JSON
		return
	}

	if msg.FileType == "" || msg.FileID == "" {
		c.logger.Warn().
			Str("file_id", msg.FileID).
			Str("file_type", string(msg.FileType)).
			Msg("Missing file_type or file_id")
		nack(false) // Drop invalid messages
		return
	}

	switch msg.FileType {
	case domain.FileTypeImage, domain.FileTypeAudio:
	default:
		c.logger.Warn().
			Str("file_type", string(msg.FileType)).
			Msg("Invalid file type")
		nack(false) // Drop unsupported file types
		return
	}

	if err := c.fileService.DeleteFile(ctx, msg.FileType, msg.FileID); err != nil {
		if errors.Is(err, domain.ErrFileNotFound) {
			c.logger.Debug().
				Str("file_id", msg.FileID).
				Str("file_type", string(msg.FileType)).
				Msg("File not found, acking (idempotent)")
			_ = d.Ack(false)
			return
		}
		c.logger.Error().Err(err).
			Str("file_id", msg.FileID).
			Str("file_type", string(msg.FileType)).
			Msg("Failed to delete file")

		// TODO: Create retry mechanism
		nack(false)
		return
	}

	if err := d.Ack(false); err != nil {
		c.logger.Error().Err(err).
			Str("file_id", msg.FileID).
			Msg("Failed to ack message")
	}
}
