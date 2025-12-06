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
	"sync"
	"syscall"

	"github.com/gin-gonic/gin"
	"github.com/prometheus/client_golang/prometheus"
	"github.com/prometheus/client_golang/prometheus/collectors"
)

var (
	metricsOnce sync.Once
)

func initMetrics() {
	metricsOnce.Do(func() {
		// Register default Go metrics only once
		// Use Register instead of MustRegister to handle AlreadyRegisteredError gracefully
		goCollector := collectors.NewGoCollector()
		processCollector := collectors.NewProcessCollector(collectors.ProcessCollectorOpts{})

		if err := prometheus.Register(goCollector); err != nil {
			var alreadyRegisteredError prometheus.AlreadyRegisteredError
			// Only panic if it's not an AlreadyRegisteredError
			if !errors.As(err, &alreadyRegisteredError) {
				panic(err)
			}
			// AlreadyRegisteredError is fine, collector is already registered
		}

		if err := prometheus.Register(processCollector); err != nil {
			var alreadyRegisteredError prometheus.AlreadyRegisteredError
			// Only panic if it's not an AlreadyRegisteredError
			if !errors.As(err, &alreadyRegisteredError) {
				panic(err)
			}
			// AlreadyRegisteredError is fine, collector is already registered
		}
	})
}

func main() {
	// Load configuration
	cfg, err := config.Load()
	if err != nil {
		log.Fatalf("Failed to load config: %v", err)
	}

	// Initialize logger
	l := logger.New("file-service")

	// Initialize metrics
	initMetrics()

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
	router.Use(middleware.PrometheusMiddleware())
	router.Use(middleware.CORSMiddleware(cfg.Security.CORSAllowedOrigins))

	// Health check
	router.GET("/health", fileHandler.HealthCheck)

	// Metrics endpoint
	router.GET("/metrics", middleware.PrometheusHandler())

	// API routes
	api := router.Group("/api/v1")
	{
		api.POST("/upload-url", fileHandler.GenerateUploadURL)
		api.GET("/download-url", fileHandler.GenerateDownloadURL)
		api.DELETE("", fileHandler.DeleteFile)
	}

	return router
}
