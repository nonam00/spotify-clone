package middleware

import "github.com/gin-gonic/gin"

// APIKeyMiddleware validates API key in request header X-API-Key
func APIKeyMiddleware(apiKey string) gin.HandlerFunc {
	return func(c *gin.Context) {
		// If API key is not configured, skip check
		if apiKey == "" {
			c.Next()
			return
		}

		providedKey := c.GetHeader("X-API-Key")
		if providedKey == "" {
			c.JSON(401, gin.H{
				"error": "Missing API key. Please provide X-API-Key header",
			})
			c.Abort()
			return
		}

		if providedKey != apiKey {
			c.JSON(403, gin.H{
				"error": "Invalid API key",
			})
			c.Abort()
			return
		}

		c.Next()
	}
}
