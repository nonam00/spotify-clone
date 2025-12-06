package main

import (
	"context"
	"errors"
	"file-service/internal/config"
	"file-service/internal/handler"
	"file-service/internal/service"
	"file-service/internal/storage"
	"file-service/pkg/logger"
	"file-service/pkg/middleware"
	"log"
	"net/http"
	"os"
	"os/signal"
	"syscall"

	"github.com/gin-gonic/gin"
)

func main() {
	// Load configuration
	cfg, err := config.Load()
	if err != nil {
		log.Fatalf("Failed to load config: %v", err)
	}

	// Initialize logger
	l := logger.New("file-service")

	// Initialize cache
	urlCache, err := storage.NewURLCache(&cfg.Cache, l)
	if err != nil {
		l.Fatal().Err(err).Msg("Failed to initialize cache")
	}

	// Initialize Minio client
	minioClient, err := storage.NewMinioClient(&cfg.Minio, urlCache, l)
	if err != nil {
		l.Fatal().Err(err).Msg("Failed to initialize Minio client")
	}

	// Initialize services
	fileService := service.NewFileService(minioClient, l)

	// Initialize handlers
	fileHandler := handler.NewFileHandler(fileService, l)

	// Setup router
	router := setupRouter(fileHandler, cfg, l)

	// Create server
	srv := &http.Server{
		Addr:    ":" + cfg.Server.Port,
		Handler: router,
	}

	// Start server in a goroutine
	go func() {
		l.Info().Str("port", cfg.Server.Port).Msg("Starting server")
		if err := srv.ListenAndServe(); err != nil && !errors.Is(err, http.ErrServerClosed) {
			l.Fatal().Err(err).Msg("Failed to start server")
		}
	}()

	// Wait for interrupt signal to gracefully shutdown the server
	quit := make(chan os.Signal, 1)
	signal.Notify(quit, syscall.SIGINT, syscall.SIGTERM)
	<-quit

	l.Info().Msg("Shutting down server...")

	// The context is used to inform the server it has 30 seconds to finish
	// the request it is currently handling
	ctx, cancel := context.WithTimeout(context.Background(), cfg.Server.ShutdownTimeout)
	defer cancel()

	if err := srv.Shutdown(ctx); err != nil {
		l.Fatal().Err(err).Msg("Server forced to shutdown")
	}

	l.Info().Msg("Server exited")
}

func setupRouter(fileHandler *handler.FileHandler, cfg *config.Config, log *logger.Logger) *gin.Engine {
	router := gin.New()

	// Global middleware
	router.Use(gin.Recovery())
	router.Use(middleware.LoggerMiddleware(log))
	router.Use(middleware.CORSMiddleware(cfg.Security.CORSAllowedOrigins))

	// Health check
	router.GET("/health", fileHandler.HealthCheck)

	// API routes
	api := router.Group("/api/v1")
	{
		api.POST("/upload-url", fileHandler.GenerateUploadURL)
		api.GET("/download-url", fileHandler.GenerateDownloadURL)
		api.DELETE("", fileHandler.DeleteFile)
	}

	return router
}
