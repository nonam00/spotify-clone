package cache

import (
	"context"
	"sync"
	"time"

	"file-service/internal/domain"
)

type memoryCacheEntry struct {
	value     *domain.PresignedURLResponse
	expiresAt time.Time
}

type memoryCache struct {
	mu    sync.RWMutex
	items map[string]*memoryCacheEntry
}

func newMemoryCache() *memoryCache {
	return &memoryCache{
		items: make(map[string]*memoryCacheEntry),
	}
}

func (c *memoryCache) Get(_ context.Context, key string) (*domain.PresignedURLResponse, bool) {
	c.mu.RLock()
	entry, exists := c.items[key]
	isExpired := exists && time.Now().After(entry.expiresAt)
	c.mu.RUnlock()

	if !exists {
		return nil, false
	}

	// If expired, remove it and return not found
	if isExpired {
		c.mu.Lock()
		// Double-check after acquiring write lock (another goroutine might have removed it)
		if entry, stillExists := c.items[key]; stillExists && time.Now().After(entry.expiresAt) {
			delete(c.items, key)
		}
		c.mu.Unlock()
		return nil, false
	}

	return entry.value, true
}

func (c *memoryCache) Set(_ context.Context, key string, value *domain.PresignedURLResponse, ttl time.Duration) error {
	c.mu.Lock()
	defer c.mu.Unlock()

	c.items[key] = &memoryCacheEntry{
		value:     value,
		expiresAt: time.Now().Add(ttl),
	}
	return nil
}

func (c *memoryCache) Delete(_ context.Context, key string) error {
	c.mu.Lock()
	defer c.mu.Unlock()

	delete(c.items, key)
	return nil
}

func (c *memoryCache) GenerateKey(fileType domain.FileType, fileID string) string {
	return string(fileType) + ":" + fileID
}
