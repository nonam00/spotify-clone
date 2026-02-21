package config

import (
	"github.com/spf13/viper"
)

type Config struct {
	Server   ServerConfig
	Email    EmailConfig
	RabbitMQ RabbitMQConfig
}

type ServerConfig struct {
	Port string
}

type RabbitMQConfig struct {
	URL string
}

type EmailConfig struct {
	Host     string
	Port     int
	User     string
	Password string
}

// Load function loads config from environment or sets default values
func Load() (*Config, error) {
	viper.SetDefault("SERVER_PORT", "7855")
	viper.SetDefault("RABBITMQ_URL", "amqp://myuser:mypassword@localhost:5672/")
	viper.SetDefault("EMAIL_HOST", "localhost")
	viper.SetDefault("EMAIL_PORT", 1025)
	viper.SetDefault("EMAIL_USER", "")
	viper.SetDefault("EMAIL_PASSWORD", "")

	viper.AutomaticEnv()

	config := &Config{
		Server: ServerConfig{
			Port: viper.GetString("SERVER_PORT"),
		},
		RabbitMQ: RabbitMQConfig{
			URL: viper.GetString("RABBITMQ_URL"),
		},
		Email: EmailConfig{
			Host:     viper.GetString("EMAIL_HOST"),
			Port:     viper.GetInt("EMAIL_PORT"),
			User:     viper.GetString("EMAIL_USER"),
			Password: viper.GetString("EMAIL_PASSWORD"),
		},
	}

	return config, nil
}
