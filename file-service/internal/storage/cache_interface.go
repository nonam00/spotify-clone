package storage

import (
	"file-service/internal/domain"
	"time"
)

// URLCache defines the interface for caching presigned URLs
type URLCache interface {
	Get(key string) (*domain.PresignedURLResponse, bool)
	Set(key string, value *domain.PresignedURLResponse, ttl time.Duration) error
	Delete(key string) error
	GenerateKey(fileType domain.FileType, fileID string) string
}
