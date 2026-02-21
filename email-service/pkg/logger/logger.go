package logger

import (
	"os"
	"time"

	"github.com/rs/zerolog"
)

type Logger struct {
	zerolog.Logger
}

func New(serviceName string) *Logger {
	zerolog.TimeFieldFormat = time.RFC3339
	zerolog.SetGlobalLevel(zerolog.InfoLevel)

	logger := zerolog.New(os.Stdout).
		With().
		Timestamp().
		Str("service", serviceName).
		Logger()

	return &Logger{logger}
}

func (l *Logger) WithComponent(component string) *Logger {
	logger := l.With().Str("component", component).Logger()
	return &Logger{logger}
}
