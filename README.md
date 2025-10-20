# 🎵 Spotify Clone

is a modern music streaming web application built using a microservices architecture. It allows users to upload, listen, and organize music tracks into personalized playlists.

## 🏗️ Architecture

The project is built on a microservices architecture and includes:

### Backend (ASP.NET Core)
- **Domain Layer** - domain models and business logic
- **Application Layer** - application layer with CQRS pattern (MediatR)
- **Infrastructure Layer** - external services (JWT, Email, password hashing)
- **Persistence Layer** - database interaction (Entity Framework Core)
- **WebAPI Layer** - REST API controllers

### Frontend (Next.js 15)
- **React 19** with TypeScript
- **Tailwind CSS** for styling
- **Zustand** for state management
- **Radix UI** for interface components
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
- **MediatR** - CQRS pattern implementation
- **FluentValidation** - data validation
- **JWT Bearer** - authentication

### Frontend
- **Next.js 15** - React framework
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

### In Development
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
- .NET 8 SDK (for local development)
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
- `JWT_SECRET` - JWT token secret key
- `JWT_EXPIRY` - token expiration date

#### Email (SMTP)
- `SMTP_HOST` - SMTP server
- `SMTP_PORT` - port SMTP
- `SMTP_USER` - SMTP user
- `SMTP_PASSWORD` - SMTP password

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
