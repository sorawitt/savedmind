# Development Dockerfile for hot reload
FROM oven/bun:latest
WORKDIR /app

# Copy package files
COPY package.json bun.lock* ./

# Install dependencies
RUN bun install

# Copy source
COPY . .

# Expose port
EXPOSE 3000

# Run dev server with host binding
CMD ["bun", "run", "dev", "--host", "0.0.0.0", "--port", "3000"]
