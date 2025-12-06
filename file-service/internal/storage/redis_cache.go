package storage

import (
	"context"
	"encoding/json"
	"errors"
	"file-service/internal/domain"
	"time"

	"github.com/redis/go-redis/v9"
)

type RedisCache struct {
	client    *redis.Client
	keyPrefix string
}

func newRedisCache(client *redis.Client, keyPrefix string) *RedisCache {
	return &RedisCache{
		client:    client,
		keyPrefix: keyPrefix,
	}
}

func (r *RedisCache) Get(key string) (*domain.PresignedURLResponse, bool) {
	ctx := context.Background()
	fullKey := r.keyPrefix + key

	val, err := r.client.Get(ctx, fullKey).Result()
	if errors.Is(err, redis.Nil) {
		return nil, false
	}
	if err != nil {
		return nil, false
	}

	var response domain.PresignedURLResponse
	if err := json.Unmarshal([]byte(val), &response); err != nil {
		return nil, false
	}

	// Check if expired (Redis should handle this, but double-check)
	if time.Now().After(response.ExpiresAt) {
		return nil, false
	}

	return &response, true
}

func (r *RedisCache) Set(key string, value *domain.PresignedURLResponse, ttl time.Duration) error {
	ctx := context.Background()
	fullKey := r.keyPrefix + key

	data, err := json.Marshal(value)
	if err != nil {
		return err
	}

	return r.client.Set(ctx, fullKey, data, ttl).Err()
}

func (r *RedisCache) Delete(key string) error {
	ctx := context.Background()
	fullKey := r.keyPrefix + key

	return r.client.Del(ctx, fullKey).Err()
}

func (r *RedisCache) GenerateKey(fileType domain.FileType, fileID string) string {
	return string(fileType) + ":" + fileID
}

// Ensure RedisCache implements URLCache interface
var _ URLCache = (*RedisCache)(nil)
