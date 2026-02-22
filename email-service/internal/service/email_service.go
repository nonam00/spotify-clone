package service

import (
	"fmt"

	"email-service/internal/config"
	"email-service/pkg/logger"

	"gopkg.in/mail.v2"
)

type EmailService interface {
	SendConfirmationCode(email, code string) error
	SendRestoreToken(email, code string) error
}

type emailService struct {
	cfg    *config.EmailConfig
	logger *logger.Logger
	dialer *mail.Dialer
}

func NewEmailService(cfg *config.EmailConfig, l *logger.Logger) EmailService {
	d := mail.NewDialer(cfg.Host, cfg.Port, cfg.User, cfg.Password)

	d.SSL = false
	d.TLSConfig = nil
	d.StartTLSPolicy = mail.NoStartTLS

	return &emailService{
		cfg:    cfg,
		logger: l.WithComponent("email_service"),
		dialer: d,
	}
}

func (m *emailService) SendConfirmationCode(email, code string) error {
	link := fmt.Sprintf("http://localhost/api/1/auth/activate?email=%s&code=%s", email, code)
	body := fmt.Sprintf("Please confirm your account by clicking <a href='%s'>here</a>", link)
	return m.send(email, "Account confirmation", body)
}

func (m *emailService) SendRestoreToken(email, code string) error {
	link := fmt.Sprintf("http://localhost/api/1/auth/restore-access?email=%s&code=%s", email, code)
	body := fmt.Sprintf("Please restore access to your account by clicking <a href='%s'>here</a>. "+
		"We've changed your password to <b>12345678</b>.", link)
	return m.send(email, "Account restore", body)
}

func (m *emailService) send(to, subject, body string) error {
	msg := mail.NewMessage()
	msg.SetHeader("From", "sender@example.com")
	msg.SetHeader("To", to)
	msg.SetHeader("Subject", subject)
	msg.SetBody("text/html", body)
	return m.dialer.DialAndSend(msg)
}
