package handler

import (
	"net/http"

	"file-service/internal/domain"
	"file-service/internal/service"
	"file-service/pkg/logger"

	"github.com/gin-gonic/gin"
)

type FileHandler struct {
	service service.FileService
	logger  *logger.Logger
}

func NewFileHandler(service service.FileService, logger *logger.Logger) *FileHandler {
	return &FileHandler{
		service: service,
		logger:  logger.WithComponent("file_handler"),
	}
}

func (h *FileHandler) GenerateUploadURL(c *gin.Context) {
	ctx := c.Request.Context()

	var req domain.UploadRequest
	if err := c.ShouldBindJSON(&req); err != nil {
		h.logger.Warn().Err(err).Msg("Invalid upload request")
		c.JSON(http.StatusBadRequest, domain.ErrorResponse{
			Error: "Invalid request payload",
		})
		return
	}

	response, err := h.service.GenerateUploadURL(ctx, req)
	if err != nil {
		h.logger.Error().Err(err).Msg("Failed to generate upload URL")
		c.JSON(http.StatusInternalServerError, domain.ErrorResponse{
			Error: "Failed to generate upload URL",
		})
		return
	}

	c.JSON(http.StatusOK, response)
}

func (h *FileHandler) GenerateDownloadURL(c *gin.Context) {
	ctx := c.Request.Context()

	fileType := domain.FileType(c.Query("type"))
	fileID := c.Query("file_id")

	if fileType == "" || fileID == "" {
		c.JSON(http.StatusBadRequest, domain.ErrorResponse{
			Error: "Missing required parameters: type, file_id, file_name",
		})
		return
	}

	if fileType != domain.FileTypeImage && fileType != domain.FileTypeAudio {
		c.JSON(http.StatusBadRequest, domain.ErrorResponse{
			Error: "Invalid file type. Must be 'image' or 'audio'",
		})
		return
	}

	response, err := h.service.GenerateDownloadURL(ctx, fileType, fileID)

	if err != nil {
		h.logger.Error().Err(err).
			Str("file_id", fileID).
			Str("file_type", string(fileType)).
			Msg("Failed to generate download URL")
		c.JSON(http.StatusInternalServerError, domain.ErrorResponse{
			Error: "Failed to generate download URL",
		})
		return
	}

	c.Redirect(http.StatusFound, response.URL)
}

func (h *FileHandler) DeleteFile(c *gin.Context) {
	ctx := c.Request.Context()

	fileType := domain.FileType(c.Query("type"))
	fileID := c.Query("file_id")

	if fileType == "" || fileID == "" {
		c.JSON(http.StatusBadRequest, domain.ErrorResponse{
			Error: "Missing required parameters: type, file_id, file_name",
		})
		return
	}

	if fileType != domain.FileTypeImage && fileType != domain.FileTypeAudio {
		c.JSON(http.StatusBadRequest, domain.ErrorResponse{
			Error: "Invalid file type. Must be 'image' or 'audio'",
		})
		return
	}

	if err := h.service.DeleteFile(ctx, fileType, fileID); err != nil {
		h.logger.Error().Err(err).
			Str("file_id", fileID).
			Str("file_type", string(fileType)).
			Msg("Failed to delete file")
		c.JSON(http.StatusInternalServerError, domain.ErrorResponse{
			Error: "Failed to delete file",
		})
		return
	}

	c.Status(http.StatusNoContent)
}

func (h *FileHandler) HealthCheck(c *gin.Context) {
	c.JSON(http.StatusOK, gin.H{
		"status":  "healthy",
		"service": "file-service",
	})
}
