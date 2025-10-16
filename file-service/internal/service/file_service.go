package service

import (
	"context"
	"file-service/internal/domain"
	"file-service/internal/storage"
	"file-service/pkg/logger"
	"fmt"

	"github.com/google/uuid"
)

type FileService interface {
	GenerateUploadURL(ctx context.Context, req domain.UploadRequest) (*domain.UploadResponse, error)
	GenerateDownloadURL(ctx context.Context, fileType domain.FileType, fileID string) (*domain.PresignedURLResponse, error)
	CheckFileExists(ctx context.Context, fileType domain.FileType, fileID string) (bool, error)
	DeleteFile(ctx context.Context, fileType domain.FileType, fileID string) error
}

type fileService struct {
	storage storage.MinioClient
	logger  *logger.Logger
}

func NewFileService(storage *storage.MinioClient, logger *logger.Logger) FileService {
	return &fileService{
		storage: *storage,
		logger:  logger.WithComponent("file_service"),
	}
}

func (s *fileService) GenerateUploadURL(ctx context.Context, req domain.UploadRequest) (*domain.UploadResponse, error) {
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

	return &domain.UploadResponse{
		UploadURL: *presignedURL,
		FileID:    fileID,
	}, nil
}

func (s *fileService) GenerateDownloadURL(ctx context.Context, fileType domain.FileType, fileID string) (*domain.PresignedURLResponse, error) {
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

	presignedURL, err := s.storage.GeneratePresignedGetURL(ctx, fileType, fileID)

	if err != nil {
		s.logger.Error().Err(err).
			Str("file_id", fileID).
			Str("file_type", string(fileType)).
			Msg("Failed to generate download URL")
		return nil, fmt.Errorf("failed to generate download URL: %w", err)
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
		return fmt.Errorf("file not found")
	}

	if err := s.storage.DeleteFile(ctx, fileType, fileID); err != nil {
		s.logger.Error().Err(err).
			Str("file_id", fileID).
			Str("file_type", string(fileType)).
			Msg("Failed to delete file")
		return fmt.Errorf("failed to delete file: %w", err)
	}

	s.logger.Info().
		Str("file_id", fileID).
		Str("file_type", string(fileType)).
		Msg("File deleted successfully")

	return nil
}
