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

	"file-service/internal/config"
	"file-service/internal/consumer"
	"file-service/internal/handler"
	"file-service/internal/service"
	"file-service/internal/storage/cache"
	"file-service/internal/storage/minio"
	"file-service/pkg/logger"
	"file-service/pkg/middleware"

	"github.com/gin-gonic/gin"
)

func main() {
	cfg, err := config.Load()
	if err != nil {
		log.Fatalf("Failed to load config: %v", err)
	}

	// Initialize logger
	l := logger.New("file-service")

	// Initialize dependencies
	urlCache, err := cache.NewURLCache(&cfg.Cache, l)
	if err != nil {
		l.Fatal().Err(err).Msg("Failed to initialize cache")
	}

	minioClient, err := minio.NewMinioClient(&cfg.Minio, urlCache, l)
	if err != nil {
		l.Fatal().Err(err).Msg("Failed to initialize Minio client")
	}

	fileService := service.NewFileService(minioClient, l)
	fileHandler := handler.NewFileHandler(fileService, l)

	// Initialize and start RabbitMQ delete file consumer
	ctx, stopConsumer := context.WithCancel(context.Background())
	deleteFileConsumer := consumer.NewDeleteFileConsumer(&cfg.RabbitMQ, fileService, l)
	go func() {
		if err := deleteFileConsumer.Run(ctx); err != nil {
			l.Error().Err(err).Msg("Delete file consumer exited with error")
		}
	}()

	// Create server
	srv := &http.Server{
		Addr:         ":" + cfg.Server.Port,
		Handler:      setupRouter(fileHandler, cfg, l),
		ReadTimeout:  10 * time.Second,
		WriteTimeout: 10 * time.Second,
	}

	// Start server in a goroutine
	go func() {
		l.Info().Str("port", cfg.Server.Port).Msg("Starting server")
		if err := srv.ListenAndServe(); err != nil && !errors.Is(err, http.ErrServerClosed) {
			l.Fatal().Err(err).Msg("Failed to start server")
		}
	}()

	// Wait for interrupt signal
	quit := make(chan os.Signal, 1)
	signal.Notify(quit, syscall.SIGINT, syscall.SIGTERM)
	<-quit

	l.Info().Msg("Initiating graceful shutdown...")

	// Stop consumers first to stop processing new messages
	stopConsumer()

	shutdownCtx, shutdownCancel := context.WithTimeout(context.Background(), cfg.Server.ShutdownTimeout)
	defer shutdownCancel()

	if err := srv.Shutdown(shutdownCtx); err != nil {
		l.Fatal().Err(err).Msg("Server forced to shutdown")
	}

	l.Info().Msg("Server exited")
}

func setupRouter(fileHandler *handler.FileHandler, cfg *config.Config, log *logger.Logger) *gin.Engine {
	router := gin.New()

	// Global middleware
	router.Use(
		gin.Recovery(),
		middleware.LoggerMiddleware(log),
		middleware.PrometheusMiddleware(),
		middleware.CORSMiddleware(cfg.Security.CORSAllowedOrigins),
	)

	// System routes
	router.GET("/health", healthCheck)
	router.GET("/metrics", handler.PrometheusHandler())

	// API routes
	api := router.Group("/api/v1")
	{
		api.POST("/upload-url", fileHandler.GenerateUploadURL)
		api.GET("/download-url", fileHandler.GenerateDownloadURL)
	}

	return router
}

func healthCheck(c *gin.Context) {
	c.JSON(http.StatusOK, gin.H{"status": "healthy", "service": "file-service"})
}
