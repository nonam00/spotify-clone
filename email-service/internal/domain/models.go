package domain

const (
	SendConfirmEmailQueue = "email-service.send-confirm-email"
	SendRestoreEmailQueue = "email-service.send-restore-email"
)

type SendEmailMessage struct {
	Email string `json:"Email"`
	Code  string `json:"Code"`
}
