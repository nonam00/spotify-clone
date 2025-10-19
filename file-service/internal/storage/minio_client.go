package storage

import (
	"context"
	"fmt"
	"net/url"
	"strings"
	"time"

	"file-service/internal/config"
	"file-service/internal/domain"
	"file-service/pkg/logger"

	"github.com/minio/minio-go/v7"
	"github.com/minio/minio-go/v7/pkg/credentials"
)

type MinioClient struct {
	client *minio.Client
	config *config.MinioConfig
	logger *logger.Logger
}

func NewMinioClient(cfg *config.MinioConfig, log *logger.Logger) (*MinioClient, error) {
	client, err := minio.New(cfg.Endpoint, &minio.Options{
		Creds:  credentials.NewStaticV4(cfg.AccessKeyID, cfg.SecretAccessKey, ""),
		Secure: cfg.UseSSL,
	})
	if err != nil {
		return nil, fmt.Errorf("failed to create minio client: %w", err)
	}

	mc := &MinioClient{
		client: client,
		config: cfg,
		logger: log,
	}

	if err := mc.initializeBuckets(context.Background()); err != nil {
		return nil, fmt.Errorf("failed to initialize buckets: %w", err)
	}

	return mc, nil
}

func (m *MinioClient) initializeBuckets(ctx context.Context) error {
	buckets := []string{m.config.ImageBucket, m.config.AudioBucket}

	for _, bucket := range buckets {
		exists, err := m.client.BucketExists(ctx, bucket)
		if err != nil {
			return fmt.Errorf("failed to check if bucket %s exists: %w", bucket, err)
		}

		if !exists {
			if err := m.client.MakeBucket(ctx, bucket, minio.MakeBucketOptions{}); err != nil {
				return fmt.Errorf("failed to create bucket %s: %w", bucket, err)
			}

			policy := fmt.Sprintf(`{
				"Version": "2012-10-17",
				"Statement": [
					{
						"Effect": "Allow",
						"Principal": {"AWS": ["*"]},
						"Action": ["s3:GetObject"],
						"Resource": ["arn:aws:s3:::%s/*"]
					}
				]
			}`, bucket)

			if err := m.client.SetBucketPolicy(ctx, bucket, policy); err != nil {
				m.logger.Warn().Err(err).Str("bucket", bucket).Msg("Failed to set bucket policy")
			}

			m.logger.Info().Str("bucket", bucket).Msg("Bucket created successfully")
		}
	}

	return nil
}

func (m *MinioClient) GeneratePresignedPutURL(ctx context.Context, req domain.UploadRequest, fileID string) (*domain.PresignedURLResponse, error) {
	bucket := m.getBucketByFileType(req.FileType)

	presignedUrl, err := m.client.PresignedPutObject(ctx, bucket, fileID, m.config.PresignExpiry)
	if err != nil {
		return nil, fmt.Errorf("failed to generate presigned put URL: %w", err)
	}

	return &domain.PresignedURLResponse{
		URL:       m.fixProxyDomain(presignedUrl.String()),
		ExpiresAt: time.Now().Add(m.config.PresignExpiry),
		FileID:    fileID,
	}, nil
}

func (m *MinioClient) GeneratePresignedGetURL(ctx context.Context, fileType domain.FileType, fileID string) (*domain.PresignedURLResponse, error) {
	bucket := m.getBucketByFileType(fileType)

	reqParams := make(url.Values)

	presignedUrl, err := m.client.PresignedGetObject(ctx, bucket, fileID, m.config.PresignExpiry, reqParams)
	if err != nil {
		return nil, fmt.Errorf("failed to generate presigned get URL: %w", err)
	}

	return &domain.PresignedURLResponse{
		URL:       m.fixProxyDomain(presignedUrl.String()),
		ExpiresAt: time.Now().Add(m.config.PresignExpiry),
		FileID:    fileID,
	}, nil
}

func (m *MinioClient) CheckFileExists(ctx context.Context, fileType domain.FileType, fileID string) (bool, error) {
	bucket := m.getBucketByFileType(fileType)

	_, err := m.client.StatObject(ctx, bucket, fileID, minio.StatObjectOptions{})
	if err != nil {
		if minio.ToErrorResponse(err).Code == "NoSuchKey" {
			return false, nil
		}
		return false, fmt.Errorf("failed to check file existence: %w", err)
	}

	return true, nil
}

func (m *MinioClient) DeleteFile(ctx context.Context, fileType domain.FileType, fileID string) error {
	bucket := m.getBucketByFileType(fileType)

	if err := m.client.RemoveObject(ctx, bucket, fileID, minio.RemoveObjectOptions{}); err != nil {
		return fmt.Errorf("failed to delete file: %w", err)
	}

	m.logger.Info().
		Str("bucket", bucket).
		Str("object", fileID).
		Str("file_id", fileID).
		Msg("File deleted successfully")

	return nil
}

func (m *MinioClient) getBucketByFileType(fileType domain.FileType) string {
	switch fileType {
	case domain.FileTypeImage:
		return m.config.ImageBucket
	case domain.FileTypeAudio:
		return m.config.AudioBucket
	default:
		return m.config.ImageBucket
	}
}

// Hardcoded: changing domain for localhost nginx proxy
func (m *MinioClient) fixProxyDomain(url string) string {
	return strings.Replace(url, m.config.Endpoint, "localhost/s3", 1)
}
