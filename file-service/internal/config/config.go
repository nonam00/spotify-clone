package config

import (
	"time"

	"github.com/spf13/viper"
)

type Config struct {
	Server   ServerConfig
	Minio    MinioConfig
	Cache    CacheConfig
	Security SecurityConfig
}

type ServerConfig struct {
	Port            string
	AccessToken     string
	ShutdownTimeout time.Duration
}

type MinioConfig struct {
	Endpoint        string
	AccessKeyID     string
	SecretAccessKey string
	UseSSL          bool
	ImageBucket     string
	AudioBucket     string
	Region          string
	PresignExpiry   time.Duration
}

type CacheConfig struct {
	Type  string // "redis" or "memory"
	Redis RedisConfig
}

type RedisConfig struct {
	Addr      string
	Password  string
	DB        int
	KeyPrefix string
}

type SecurityConfig struct {
	CORSAllowedOrigins []string
	APIKey             string
}

// Load function loads config from environment or sets default values
func Load() (*Config, error) {
	viper.SetDefault("MINIO_ACCESS_KEY_ID", "minioadmin")
	viper.SetDefault("MINIO_SECRET_ACCESS_KEY", "minioadmin")
	viper.SetDefault("SERVER_PORT", "8005")
	viper.SetDefault("SERVER_SHUTDOWN_TIMEOUT", "30s")
	viper.SetDefault("MINIO_ENDPOINT", "localhost:9000")
	viper.SetDefault("MINIO_USE_SSL", false)
	viper.SetDefault("MINIO_IMAGE_BUCKET", "image")
	viper.SetDefault("MINIO_AUDIO_BUCKET", "audio")
	viper.SetDefault("MINIO_PRESIGN_EXPIRY", "15m")
	viper.SetDefault("MINIO_REGION", "us-east-1")
	viper.SetDefault("CACHE_TYPE", "redis") // "redis" or "memory"
	viper.SetDefault("REDIS_ENDPOINT", "redis:6379")
	viper.SetDefault("REDIS_PASSWORD", "redispassword")
	viper.SetDefault("REDIS_DB", 1)
	viper.SetDefault("REDIS_KEY_PREFIX", "file-service:presigned-url:")
	viper.SetDefault("SECURITY_CORS_ALLOWED_ORIGINS", []string{"*"})
	viper.SetDefault("SECURITY_API_KEY", "")

	viper.AutomaticEnv()

	config := &Config{
		Server: ServerConfig{
			Port:            viper.GetString("SERVER_PORT"),
			ShutdownTimeout: viper.GetDuration("SERVER_SHUTDOWN_TIMEOUT"),
		},
		Minio: MinioConfig{
			Endpoint:        viper.GetString("MINIO_ENDPOINT"),
			AccessKeyID:     viper.GetString("MINIO_ACCESS_KEY_ID"),
			SecretAccessKey: viper.GetString("MINIO_SECRET_ACCESS_KEY"),
			UseSSL:          viper.GetBool("MINIO_USE_SSL"),
			ImageBucket:     viper.GetString("MINIO_IMAGE_BUCKET"),
			AudioBucket:     viper.GetString("MINIO_AUDIO_BUCKET"),
			PresignExpiry:   viper.GetDuration("MINIO_PRESIGN_EXPIRY"),
			Region:          viper.GetString("MINIO_REGION"),
		},
		Cache: CacheConfig{
			Type: viper.GetString("CACHE_TYPE"),
			Redis: RedisConfig{
				Addr:      viper.GetString("REDIS_ENDPOINT"),
				Password:  viper.GetString("REDIS_PASSWORD"),
				DB:        viper.GetInt("REDIS_DB"),
				KeyPrefix: viper.GetString("REDIS_KEY_PREFIX"),
			},
		},
		Security: SecurityConfig{
			CORSAllowedOrigins: viper.GetStringSlice("SECURITY_CORS_ALLOWED_ORIGINS"),
			APIKey:             viper.GetString("SECURITY_API_KEY"),
		},
	}

	return config, nil
}
