package main

import (
	"context"
	"email-service/internal/consumer"
	"email-service/internal/domain"
	"email-service/internal/handler"
	"errors"
	"log"
	"net/http"
	"os"
	"os/signal"
	"syscall"
	"time"

	"email-service/internal/config"
	"email-service/internal/service"
	"email-service/pkg/logger"

	"github.com/gin-gonic/gin"
)

func main() {
	cfg, err := config.Load()
	if err != nil {
		log.Fatalf("Failed to load config: %v", err)
	}

	// Initialize logger
	l := logger.New("email-service")

	// Initialize dependencies
	emailSvc := service.NewEmailService(&cfg.Email, l)

	ctx, stopConsumers := context.WithCancel(context.Background())

	// Initialize two consumers for the two different queues
	confirmConsumer := consumer.NewEmailConsumer(&cfg.RabbitMQ, emailSvc, l, domain.SendConfirmEmailQueue, "confirm")
	restoreConsumer := consumer.NewEmailConsumer(&cfg.RabbitMQ, emailSvc, l, domain.SendRestoreEmailQueue, "restore")

	// Run Consumers
	go func() {
		if err := confirmConsumer.Run(ctx); err != nil {
			l.Error().Err(err).Msg("Confirm consumer failed")
		}
	}()
	go func() {
		if err := restoreConsumer.Run(ctx); err != nil {
			l.Error().Err(err).Msg("Restore consumer failed")
		}
	}()

	// HTTP Server for Health/Metrics
	srv := &http.Server{
		Addr:    ":" + cfg.Server.Port,
		Handler: setupRouter(),
	}

	go func() {
		if err := srv.ListenAndServe(); err != nil && !errors.Is(err, http.ErrServerClosed) {
			l.Fatal().Err(err).Msg("Server failed")
		}
	}()

	// Graceful Shutdown
	quit := make(chan os.Signal, 1)
	signal.Notify(quit, syscall.SIGINT, syscall.SIGTERM)
	<-quit

	l.Info().Msg("Shutting down...")
	stopConsumers() // Signal consumers to finish current work

	shutdownCtx, cancel := context.WithTimeout(context.Background(), 10*time.Second)
	defer cancel()

	if err := srv.Shutdown(shutdownCtx); err != nil {
		l.Error().Err(err).Msg("Forced shutdown")
	}
}

func setupRouter() *gin.Engine {
	router := gin.New()

	router.Use(gin.Recovery())

	// System routes
	router.GET("/health", handler.HealthCheck)
	router.GET("/metrics", handler.PrometheusHandler())
	return router
}
