# 🎵 Spotify Clone

is a modern music streaming web application built using a microservices architecture. It allows users to upload, listen, and organize music tracks into personalized playlists.

https://github.com/user-attachments/assets/83bd7c31-7573-4925-b783-11156097a753

## 🏗️ Architecture

The project is built on a microservices architecture and includes:

### Backend (ASP.NET Core 10)
- **Domain Layer** - Rich domain models with business logic, entities, value objects, domain events
- **Application Layer** - Implements use cases using CQRS pattern, domain event handlers and result pattern
- **Infrastructure Layer** - External services (JWT, Email, password hashing)
- **Persistence Layer** - Database interaction (ORM - Entity Framework Core) with repository pattern
- **WebAPI Layer** - REST API controllers, DTOs and middleware

### Frontend (Next.js 16)
- Using Feature-Sliced Design (FSD) methodology with clear separation of concerns
- Primary user-facing interface for music streaming and management
- User authorization with JWT tokens

### Moderation App (React + Vite)
- Administrative interface for content moderation and system management
- Using Feature-Sliced Design (FSD) methodology with clear separation of concerns
- Permissions-based access control with JWT tokens

### File Service (Go)
- Independent HTTP microservice for S3 interactions
- Integration with MinIO (S3-compatible storage)
- Using distributed cache based on Redis for better performance on getting files

### AI Transcription & Moderation Service
- Converts uploaded audio tracks to text using **Faster Whisper** for high-speed, accurate speech-to-text.
- Scans transcriptions for inappropriate content with **better-profanity**
- Integrates with RabbitMQ using **aio-pika** for event-driven processing of newly uploaded tracks.

### Infrastructure
- **RabbitMQ** - Message broker
- **PostgreSQL** - Main database for primary data
- **Redis** - Storage for tokens and distributed cache
- **MinIO** - Object storage for audio and image files
- **Nginx** - Reverse proxy and load balancer
- **Prometheus** - Metrics collection and storage
- **Grafana** - Metrics visualization and dashboards

## ✨ Main Features

### 🎵 Music Management
- **Track Upload** - users can upload their own audio files
- **Listen** - built-in audio player with playback controls
- **Search and Filter** - search by title, author, and other parameters
- **Favorite Tracks** - like system for saving favorite songs

### 📋 Playlists
- **Create Playlists** - users can create personal collections
- **Manage Tracks** - add and remove songs from playlists

### 👤 User System
- **Registration and Authorization** - JWT tokens for authentication
- **Email Confirmation** - account verification via email
- **User Profiles** - customize avatars and personal information
- **Refresh tokens** - secure session refresh

### 🔍 Content Moderation
- **Authorization with Permissions** - every moderator has an account with own permissions
- **Unpublished Songs Management** - view and manage songs pending publication
- **User Accounts Management** - view information and statistics about users, manage user accounts or songs published by users
- **Moderator Accounts Management** - view and manage moderator accounts, give or revoke permissions if you are super-moderator
- **Bulk Operations** - publish or delete multiple songs at once
- **Audio Preview** - listen to songs before moderation decisions
- **Automatic Explicit Moderation** – AI-powered content verification

### 📊 Monitoring & Observability
- **Real-time Metrics** - track application performance and health
- **Custom Dashboards** - visualize key metrics in Grafana
- **System Monitoring** - monitor server resources and infrastructure
- **HTTP Metrics** - request rates, response times, error rates
- **Database Metrics** - query performance and connection pools

## 🚀 Tech Stack

### Backend
- **.NET Core 10** - Modern, cross-platform runtime for high-performance applications
- **Entity Framework Core** - Modern ORM with LINQ support, migrations, and change tracking
- **Npgsql** - .NET data provider for PostgreSQL
- **FluentValidation** - Strongly-typed validation library with fluent API
- **JWT Bearer Authentication** - Secure token-based authentication with refresh token support
- **Prometheus .NET Client** - Metrics exposure for monitoring integration
- **BCrypt.Net** - Secure password hashing and verification
- **xUnit** - Unit test library
- **Test Containers** - Library for creating docker instances of real dependencies for Unit tests

### Frontend
**Shared**:
- **React 19** - Latest React version with concurrent features and improved performance
- **TypeScript** - Type-safe JavaScript with strict configuration
- **Tailwind CSS 4** - Utility-first CSS framework with JIT compilation
- **Zustand** - Minimalist state management with hooks and middleware support
- **Zod** - Typescript-first validation library
- **Radix UI** - UI kit for shared common components
- **React Icons** - Icon library with various icon sets

**Main Application**:
- **Next.js 16** - React framework with server-side rendering, static generation, and API routes
- **Dnd Kit** - Library for dragging UI elements 
- **React Hot Toast** - Elegant toast notifications with promise support

**Moderation Application**:
- **Vite** - Modern frontend build tool
- **React Router DOM** - Declarative routing for React single-page applications

### Go Microservices
**Shared**:
- **Go 1.25** - High-performance language
- **Gin Web Framework** - Lightweight and efficient HTTP web framework
- **AMQP 0.9.1** - Library for consuming RabbitMQ messages
- **Zerolog** - High-performance, structured logging
- **Viper** - Configuration management with support for multiple formats
- **Prometheus Go Client** - Metrics exposure for monitoring integration

**File Service**:
- **MinIO Go SDK** - S3-compatible object storage client for file management
- **Redis Go Client** - Redis integration for caching and temporary storage

**Email Service**
- **Gomail** - SMTP mail client

### AI Transcription & Moderation Service
- **FastAPI** – Modern, high-performance async web framework
- **Faster Whisper** – CTranslate2-based Whisper implementation for rapid audio transcription
- **better-profanity** – Profanity detection and content filtering
- **aio-pika** – Async RabbitMQ client for messaging

### DevOps & Infrastructure
- **Docker & Docker Compose** - Containerization and basic orchestration
- **PostgreSQL** - Advanced relational database
- **Redis** - In-memory data structure store with persistence
- **MinIO** - High-performance S3-compatible object storage
- **Nginx** - High-performance HTTP server and reverse proxy
- **Prometheus** - Monitoring toolkit
- **Grafana** - Multi-platform analytics and visualization

## 📋 Development Plans

### Plans
- [ ] **Recommender System** - Algorithms for music suggestions
- [ ] **Social Features** - Subscriptions, comments, ratings
- [ ] **Performance Testing** - Load testing with k6 or Locust

### Completed
- [x] **Monitoring Stack** - Prometheus and Grafana integration
- [x] **Moderation Service** - Manual content moderation interface
- [x] **Email Confirmation** - Account Verification System
- [x] **Automatic Moderation** - AI-powered content verification
- [x] **Live Lyrics** - Sing along with synced text  

## 🛠️ Installation and Launch

### Requirements
- Docker and Docker Compose
- .NET 10 SDK (for local development)
- Node.js 18+ (for local development)

### ⚙️ Configuration

The application requires the following environment variables to be configured:

#### Database
- `POSTGRES_USER` - PostgreSQL user
- `POSTGRES_PASSWORD` - PostgreSQL password
- `POSTGRES_DB` - database name

#### Redis
- `REDIS_PASSWORD` - Redis password
- `REDIS_USER` - Redis user

#### MinIO
- `MINIO_ROOT_USER` - MinIO user
- `MINIO_ROOT_PASSWORD` - MinIO password

#### Grafana 
- `GRAFANA_USER` - Grafana user
- `GRAFANA_PASSWORD` - Grafana password

#### JWT
- `JwtOptions__SecretKey` - JWT token secret key
- `JwtOptions__ExpiresHours` - token ttl

#### Email (SMTP)
- `SmtpOptions__Server` - SMTP server domain
- `SmtpOptions__Port` - SMTP server port

#### Next SSR endpoints
- `SERVER_API_URL` - endpoint for web api (if running in container use host.docker.internal)
*endpoints for client side request are hardcoded because of the way how next.js operates with env variables*

### 📝 Important Notes

- All configuration settings must be specified in .env files
- For production, it is recommended to use external services instead of MailHog
- MinIO is used for file storage, make sure access is configured correctly
- Initial moderator credentials: *admin@admin.com adminadmin*

## 🤝 Contributing to the Project

I welcome contributions! Please:

1. Fork the repository
2. Create a branch for the new feature (`git checkout -b feature/amazing-feature`)
3. Commit the changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request
