# 🎵 Spotify Clone

is a modern music streaming web application built using a microservices architecture. It allows users to upload, listen, and organize music tracks into personalized playlists.

https://github.com/user-attachments/assets/e064eb61-1632-4a06-ac58-bdb705514cf3

## 🏗️ Architecture

The project is built on a microservices architecture and includes:

### Backend (ASP.NET Core)
- **Domain Layer** - rich domain models with business logic, entities, value objects, domain events
- **Application Layer** - implements use cases using CQRS pattern, domain event handlers and result pattern
- **Infrastructure Layer** - external services (JWT, Email, password hashing)
- **Persistence Layer** - database interaction (ORM - Entity Framework Core) with repository pattern
- **WebAPI Layer** - REST API controllers, DTOs and middleware

### Frontend (Next.js 16)
- **React 19** with TypeScript
- **Tailwind CSS** for styling
- **Zustand** for state management
- **React Hot Toast** for notifications

### File Service (Go)
- File management microservice
- Integration with MinIO (S3-compatible storage)
- Uploading and downloading audio files and images

### Infrastructure
- **PostgreSQL** - main database
- **Redis** - storage for confirmation codes (and cache in future)
- **MinIO** - object storage for files
- **Nginx** - reverse proxy and load balancer
- **MailHog** - test SMTP server for development

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

## 🚀 Tech Stack

### Backend
- **.NET Core** - the main technology
- **Entity Framework Core** - ORM for working with the database
- **FluentValidation** - data validation
- **JWT Bearer** - authentication

### Frontend
- **Next.js 16** - React framework
- **TypeScript** - for JavaScript type-safety
- **Tailwind CSS** - utility-first CSS framework
- **Zustand** - lightweight state management
- **React Icons** - icons

### DevOps & Infrastructure
- **Docker & Docker Compose** - containerization
- **PostgreSQL** - relational database
- **Redis** - in-memory storage
- **MinIO** - S3-compatible storage
- **Nginx** - web server and proxy

## 📋 Development Plans

### Plans
- [ ] **Moderation Service** - Automatic verification of uploaded content
- [ ] **Recommender System** - Algorithms for music suggestions
- [ ] **Social Features** - Subscriptions, comments, ratings
- [ ] **Mobile App** - React Native version

### Completed
- [x] **Email Confirmation** - Account Verification System
- [x] **Basic Functionality** - Uploading, Playback, Playlists
- [x] **Microservice Architecture** - Separation into Independent Services

## 🛠️ Installation and Launch

### Requirements
- Docker and Docker Compose
- .NET 9 SDK (for local development)
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

#### JWT
- `JwtOptions__SecretKey` - JWT token secret key
- `JwtOptions__ExpiresHours` - token ttl

#### Email (SMTP)
- `SmtpOptions__Server` - SMTP server domain
- `SmtpOptions__Port` - SMTP server port

#### Next SSR endpoints
- `SERVER_API_URL` - endpoint for web api (if running in container use host.docker.internal)
- `FILE_SERVER_URL` - endpoint for file service (if running in container use host.docker.internal)
*endpoints for client side request are hardcoded because of the way how next.js operates with env variables*

### 📝 Important Notes

- **EF Core migrations** must be run before the first run of the application
- All configuration settings must be specified in .env files
- For production, it is recommended to use external services instead of MailHog
- MinIO is used for file storage, make sure access is configured correctly

## 🤝 Contributing to the Project

I welcome contributions! Please:

1. Fork the repository
2. Create a branch for the new feature (`git checkout -b feature/amazing-feature`)
3. Commit the changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request
