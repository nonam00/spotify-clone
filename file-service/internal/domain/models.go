package domain

import (
	"errors"
	"time"
)

var ErrFileNotFound = errors.New("file not found")

type FileType string

const (
	FileTypeImage FileType = "image"
	FileTypeAudio FileType = "audio"
)

type PresignedURLResponse struct {
	URL       string    `json:"url"`
	ExpiresAt time.Time `json:"expires_at"`
	FileID    string    `json:"file_id"`
}

type UploadRequest struct {
	FileType FileType `json:"file_type" binding:"required,oneof=image audio"`
}

type ErrorResponse struct {
	Error   string `json:"error"`
	Details string `json:"details,omitempty"`
}

// DeleteFileMessage - сообщение для удаления файла через RabbitMQ
type DeleteFileMessage struct {
	FileType FileType `json:"file_type"`
	FileID   string   `json:"file_id"`
}
