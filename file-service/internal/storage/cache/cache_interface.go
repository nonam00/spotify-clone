package cache

import (
	"context"
	"time"

	"file-service/internal/domain"
)

// URLCache defines the interface for caching presigned URLs
type URLCache interface {
	Get(ctx context.Context, key string) (*domain.PresignedURLResponse, bool)
	Set(ctx context.Context, key string, value *domain.PresignedURLResponse, ttl time.Duration) error
	Delete(ctx context.Context, key string) error
	GenerateKey(fileType domain.FileType, fileID string) string
}
