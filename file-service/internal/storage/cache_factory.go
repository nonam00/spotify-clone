package storage

import (
	"context"
	"file-service/internal/config"
	"file-service/pkg/logger"

	"github.com/redis/go-redis/v9"
)

// NewURLCache creates a cache implementation based on configuration
func NewURLCache(cfg *config.CacheConfig, log *logger.Logger) (URLCache, error) {
	if cfg.Type == "redis" {
		redisClient := redis.NewClient(&redis.Options{
			Addr:     cfg.Redis.Addr,
			Password: cfg.Redis.Password,
			DB:       cfg.Redis.DB,
		})

		// Test connection
		ctx := context.Background()
		if err := redisClient.Ping(ctx).Err(); err != nil {
			return nil, err
		}

		log.Info().Str("type", "redis").Str("addr", cfg.Redis.Addr).Msg("Redis cache initialized")
		return newRedisCache(redisClient, cfg.Redis.KeyPrefix), nil
	}

	// Default to in-memory cache
	log.Info().Str("type", "memory").Msg("In-memory cache initialized")
	return newMemoryCache(), nil
}
