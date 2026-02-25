package domain

const (
	SendEmailQueue = "email-service.send-email"
)

type EmailTopic = int

const (
	Confirm EmailTopic = 1
	Restore EmailTopic = 2
)

type SendEmailMessage struct {
	Email      string     `json:"email"`
	Code       string     `json:"code"`
	EmailTopic EmailTopic `json:"topic"`
}
