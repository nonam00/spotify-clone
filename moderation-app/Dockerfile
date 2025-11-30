FROM node:25-alpine AS base

FROM base AS deps
WORKDIR /app
COPY package*.json ./
RUN npm ci

FROM base AS builder
WORKDIR /app
COPY --from=deps /app/node_modules ./node_modules
COPY . .
ENV NODE_ENV=production
RUN npm run build

FROM base AS runner
WORKDIR /app
ENV NODE_ENV=production
RUN addgroup --system --gid 1001 nodejs && \
    adduser --system --uid 1001 vitejs

# Copy built application
COPY --from=builder --chown=vitejs:nodejs /app/dist ./dist
COPY --from=builder --chown=vitejs:nodejs /app/package*.json ./
COPY --from=deps --chown=vitejs:nodejs /app/node_modules ./node_modules

USER vitejs
EXPOSE 5173

# For production, use preview mode
CMD ["npm", "run", "preview", "--", "--host", "0.0.0.0", "--port", "5173"]