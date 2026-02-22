package main

import (
	"context"
	"errors"
	"log"
	"net/http"
	"os"
	"os/signal"
	"syscall"
	"time"

	"email-service/internal/config"
	"email-service/internal/consumer"
	"email-service/internal/handler"
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
	emailService := service.NewEmailService(&cfg.Email, l)

	ctx, stopConsumer := context.WithCancel(context.Background())
	confirmConsumer := consumer.NewEmailConsumer(&cfg.RabbitMQ, emailService, l)
	go func() {
		if err := confirmConsumer.Run(ctx); err != nil {
			l.Error().Err(err).Msg("Send email consumer exited with error")
		}
	}()

	// HTTP Server for Health/Metrics
	srv := &http.Server{
		Addr:    ":" + cfg.Server.Port,
		Handler: setupRouter(),
	}

	go func() {
		l.Info().Str("port", cfg.Server.Port).Msg("Starting server")
		if err := srv.ListenAndServe(); err != nil && !errors.Is(err, http.ErrServerClosed) {
			l.Fatal().Err(err).Msg("Server failed")
		}
	}()

	// Wait for interrupt signal
	quit := make(chan os.Signal, 1)
	signal.Notify(quit, syscall.SIGINT, syscall.SIGTERM)
	<-quit

	l.Info().Msg("Shutting down...")

	// Stop consumers first to stop processing new messages
	stopConsumer()

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
