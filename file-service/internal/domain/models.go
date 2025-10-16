package domain

import "time"

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

type UploadResponse struct {
	UploadURL PresignedURLResponse `json:"upload_url"`
	FileID    string               `json:"file_id"`
}

type ErrorResponse struct {
	Error   string `json:"error"`
	Details string `json:"details,omitempty"`
}
