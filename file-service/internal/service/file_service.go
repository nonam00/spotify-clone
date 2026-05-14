package service

import (
	"context"
	"file-service/internal/storage/cache"
	"fmt"
	"time"

	"file-service/internal/domain"
	"file-service/internal/storage/minio"
	"file-service/pkg/logger"

	"github.com/google/uuid"
)

type FileService interface {
	GenerateUploadURL(ctx context.Context, req domain.UploadRequest) (*domain.PresignedURLResponse, error)
	GenerateDownloadURL(ctx context.Context, fileType domain.FileType, fileID string, isInternalRequest bool) (*domain.PresignedURLResponse, error)
	CheckFileExists(ctx context.Context, fileType domain.FileType, fileID string) (bool, error)
	DeleteFile(ctx context.Context, fileType domain.FileType, fileID string) error
}

type fileService struct {
	storage *minio.MinioClient
	cache   cache.URLCache
	logger  *logger.Logger
}

func NewFileService(storage *minio.MinioClient, cache cache.URLCache, logger *logger.Logger) FileService {
	return &fileService{
		storage: storage,
		cache:   cache,
		logger:  logger.WithComponent("file_service"),
	}
}

func (s *fileService) GenerateUploadURL(ctx context.Context, req domain.UploadRequest) (*domain.PresignedURLResponse, error) {
	fileID := uuid.New().String()

	presignedURL, err := s.storage.GeneratePresignedPutURL(ctx, req, fileID)
	if err != nil {
		s.logger.Error().Err(err).
			Str("file_type", string(req.FileType)).
			Msg("Failed to generate upload URL")
		return nil, fmt.Errorf("failed to generate upload URL: %w", err)
	}

	s.logger.Info().
		Str("file_id", fileID).
		Str("file_type", string(req.FileType)).
		Msg("Upload URL generated successfully")

	return presignedURL, nil
}

func (s *fileService) GenerateDownloadURL(
	ctx context.Context,
	fileType domain.FileType,
	fileID string,
	isInternalRequest bool,
) (*domain.PresignedURLResponse, error) {
	// Check cache first for external requests
	if !isInternalRequest {
		cacheKey := s.cache.GenerateKey(fileType, fileID)
		if cached, found := s.cache.Get(ctx, cacheKey); found {
			s.logger.Info().
				Str("file_id", fileID).
				Str("file_type", string(fileType)).
				Msg("Presigned GET URL retrieved from cache")
			return cached, nil
		}
	}

	exists, err := s.storage.CheckFileExists(ctx, fileType, fileID)

	if err != nil {
		s.logger.Error().Err(err).
			Str("file_id", fileID).
			Str("file_type", string(fileType)).
			Msg("Failed to check file existence")
		return nil, fmt.Errorf("failed to check file existence: %w", err)
	}

	if !exists {
		s.logger.Warn().
			Str("file_id", fileID).
			Str("file_type", string(fileType)).
			Msg("File not found")
		return nil, fmt.Errorf("file not found")
	}

	presignedURL, err := s.storage.GeneratePresignedGetURL(ctx, fileType, fileID, isInternalRequest)

	if err != nil {
		s.logger.Error().Err(err).
			Str("file_id", fileID).
			Str("file_type", string(fileType)).
			Msg("Failed to generate download URL")
		return nil, fmt.Errorf("failed to generate download URL: %w", err)
	}

	// Cache the response with TTL matching the presign expiry for external requests
	if !isInternalRequest {
		cacheKey := s.cache.GenerateKey(fileType, fileID)
		if err := s.cache.Set(ctx, cacheKey, presignedURL, time.Until(presignedURL.ExpiresAt)); err != nil {
			s.logger.Warn().Err(err).Str("file_id", fileID).Msg("Failed to cache presigned URL")
		}
	}

	s.logger.Info().
		Str("file_id", fileID).
		Str("file_type", string(fileType)).
		Msg("Download URL generated successfully")

	return presignedURL, nil
}

func (s *fileService) CheckFileExists(ctx context.Context, fileType domain.FileType, fileID string) (bool, error) {
	exists, err := s.storage.CheckFileExists(ctx, fileType, fileID)
	if err != nil {
		s.logger.Error().Err(err).
			Str("file_id", fileID).
			Str("file_type", string(fileType)).
			Msg("Failed to check file existence")
		return false, fmt.Errorf("failed to check file existence: %w", err)
	}
	return exists, nil
}

func (s *fileService) DeleteFile(ctx context.Context, fileType domain.FileType, fileID string) error {
	exists, err := s.storage.CheckFileExists(ctx, fileType, fileID)

	if err != nil {
		s.logger.Error().Err(err).
			Str("file_id", fileID).
			Str("file_type", string(fileType)).
			Msg("Failed to check file existence")
		return fmt.Errorf("failed to check file existence: %w", err)
	}

	if !exists {
		s.logger.Warn().
			Str("file_id", fileID).
			Str("file_type", string(fileType)).
			Msg("File not found")
		return domain.ErrFileNotFound
	}

	if err := s.storage.DeleteFile(ctx, fileType, fileID); err != nil {
		s.logger.Error().Err(err).
			Str("file_id", fileID).
			Str("file_type", string(fileType)).
			Msg("Failed to delete file")
		return fmt.Errorf("failed to delete file: %w", err)
	}

	// Invalidate cache for this file
	cacheKey := s.cache.GenerateKey(fileType, fileID)
	if err := s.cache.Delete(ctx, cacheKey); err != nil {
		s.logger.Warn().Err(err).Str("file_id", fileID).Msg("Failed to invalidate cache")
	}

	s.logger.Info().
		Str("file_id", fileID).
		Str("file_type", string(fileType)).
		Msg("File deleted successfully")

	return nil
}
