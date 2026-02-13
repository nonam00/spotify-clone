package middleware

import (
	"errors"
	"time"

	"file-service/pkg/logger"

	"github.com/gin-gonic/gin"
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
