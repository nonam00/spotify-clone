package middleware

import (
	"errors"
	"file-service/pkg/logger"
	"fmt"
	"time"

	"github.com/gin-gonic/gin"
	"github.com/prometheus/client_golang/prometheus"
	"github.com/prometheus/client_golang/prometheus/promauto"
	"github.com/prometheus/client_golang/prometheus/promhttp"
)

var (
	httpRequestsTotal = promauto.NewCounterVec(
		prometheus.CounterOpts{
			Name: "http_requests_total",
			Help: "Total number of HTTP requests",
		},
		[]string{"method", "endpoint", "status"},
	)

	httpRequestDuration = promauto.NewHistogramVec(
		prometheus.HistogramOpts{
			Name:    "http_request_duration_seconds",
			Help:    "HTTP request duration in seconds",
			Buckets: prometheus.DefBuckets,
		},
		[]string{"method", "endpoint"},
	)

	httpRequestSize = promauto.NewHistogramVec(
		prometheus.HistogramOpts{
			Name:    "http_request_size_bytes",
			Help:    "HTTP request size in bytes",
			Buckets: prometheus.ExponentialBuckets(100, 10, 7),
		},
		[]string{"method", "endpoint"},
	)

	httpResponseSize = promauto.NewHistogramVec(
		prometheus.HistogramOpts{
			Name:    "http_response_size_bytes",
			Help:    "HTTP response size in bytes",
			Buckets: prometheus.ExponentialBuckets(100, 10, 7),
		},
		[]string{"method", "endpoint"},
	)
)

func LoggerMiddleware(log *logger.Logger) gin.HandlerFunc {
	return func(c *gin.Context) {
		start := time.Now()
		path := c.Request.URL.Path
		query := c.Request.URL.RawQuery

		c.Next()

		end := time.Now()
		latency := end.Sub(start)

		fields := map[string]interface{}{
			"status":     c.Writer.Status(),
			"method":     c.Request.Method,
			"path":       path,
			"query":      query,
			"ip":         c.ClientIP(),
			"user_agent": c.Request.UserAgent(),
			"latency":    latency,
			"time":       end.Format(time.RFC3339),
		}

		if len(c.Errors) > 0 {
			var errorSlice []error
			for _, s := range c.Errors.Errors() {
				errorSlice = append(errorSlice, errors.New(s))
			}
			log.Error().Str("path", path).Errs(
				"errors",
				errorSlice,
			).Fields(fields).Msg("Request completed with errors")
		} else {
			log.Info().Fields(fields).Msg("Request completed")
		}
	}
}

func CORSMiddleware(allowedOrigins []string) gin.HandlerFunc {
	return func(c *gin.Context) {
		origin := c.Request.Header.Get("Origin")

		// Check if origin is allowed
		for _, allowedOrigin := range allowedOrigins {
			if allowedOrigin == "*" || allowedOrigin == origin {
				c.Writer.Header().Set("Access-Control-Allow-Origin", origin)
				break
			}
		}

		c.Writer.Header().Set("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS")
		c.Writer.Header().Set("Access-Control-Allow-Headers", "Content-Type, Authorization")
		c.Writer.Header().Set("Access-Control-Allow-Credentials", "true")

		if c.Request.Method == "OPTIONS" {
			c.AbortWithStatus(204)
			return
		}

		c.Next()
	}
}

func PrometheusMiddleware() gin.HandlerFunc {
	return func(c *gin.Context) {
		start := time.Now()
		method := c.Request.Method
		endpoint := c.FullPath()
		if endpoint == "" {
			endpoint = c.Request.URL.Path
		}

		// Record request size
		requestSize := float64(c.Request.ContentLength)
		if requestSize > 0 {
			httpRequestSize.WithLabelValues(method, endpoint).Observe(requestSize)
		}

		// Wrap response writer to capture response size
		writer := &responseWriter{ResponseWriter: c.Writer, size: 0}
		c.Writer = writer

		c.Next()

		// Record metrics
		status := c.Writer.Status()
		duration := time.Since(start).Seconds()
		responseSize := float64(writer.size)

		httpRequestsTotal.WithLabelValues(method, endpoint, fmt.Sprintf("%d", status)).Inc()
		httpRequestDuration.WithLabelValues(method, endpoint).Observe(duration)
		if responseSize > 0 {
			httpResponseSize.WithLabelValues(method, endpoint).Observe(responseSize)
		}
	}
}

func PrometheusHandler() gin.HandlerFunc {
	return gin.WrapH(promhttp.Handler())
}

type responseWriter struct {
	gin.ResponseWriter
	size int
}

func (w *responseWriter) Write(b []byte) (int, error) {
	size, err := w.ResponseWriter.Write(b)
	w.size += size
	return size, err
}

func (w *responseWriter) WriteString(s string) (int, error) {
	size, err := w.ResponseWriter.WriteString(s)
	w.size += size
	return size, err
}
