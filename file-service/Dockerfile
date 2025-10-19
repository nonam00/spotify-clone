FROM golang:1.25-alpine AS builder

WORKDIR /app
# Install deps
COPY go.mod go.sum ./
RUN go mod download
# Copy source code and build
COPY . .
RUN CGO_ENABLED=0 GOOS=linux go build -a -installsuffix cgo -o main ./cmd/server

FROM alpine:latest AS runner
WORKDIR /root/
COPY --from=builder /app/main .
EXPOSE 8005
CMD ["./main"]
