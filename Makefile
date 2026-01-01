.PHONY: up down build logs db-shell api-logs web-logs clean

# Start all services
up:
	docker-compose up -d

# Start with rebuild
up-build:
	docker-compose up -d --build

# Start with logs
dev:
	docker-compose up

# Stop all services
down:
	docker-compose down

# Stop and remove volumes
clean:
	docker-compose down -v

# Rebuild all images
build:
	docker-compose build

# View all logs
logs:
	docker-compose logs -f

# View specific service logs
api-logs:
	docker-compose logs -f api

web-logs:
	docker-compose logs -f web

db-logs:
	docker-compose logs -f db

seq-logs:
	docker-compose logs -f seq

# Database shell
db-shell:
	docker-compose exec db psql -U savedmind -d savedmind

# Restart a service
restart-api:
	docker-compose restart api

restart-web:
	docker-compose restart web

# Status
ps:
	docker-compose ps
