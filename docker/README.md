# Docker Infrastructure for PosX

This directory contains Docker Compose configuration for local development infrastructure.

## Services

### SQL Server 2022
- **Port**: 1433
- **SA Password**: `PosX@2026!SecurePass`
- **Connection String**: `Server=localhost,1433;User Id=sa;Password=PosX@2026!SecurePass;TrustServerCertificate=True`

### RabbitMQ with Management UI
- **AMQP Port**: 5672
- **Management UI**: http://localhost:15672
- **Username**: `posx`
- **Password**: `posx123`

### Redis
- **Port**: 6379
- **Password**: `posx123`
- **Connection String**: `localhost:6379,password=posx123`

### Seq (Logging Dashboard)
- **UI**: http://localhost:5341
- **No authentication required for local development**

## Usage

### Start all services
```bash
docker-compose up -d
```

### Stop all services
```bash
docker-compose down
```

### Stop and remove volumes (clean slate)
```bash
docker-compose down -v
```

### View logs
```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f sqlserver
docker-compose logs -f rabbitmq
docker-compose logs -f redis
docker-compose logs -f seq
```

### Check service health
```bash
docker-compose ps
```

## Notes

- All data is persisted in Docker volumes
- Services are connected via a custom bridge network `posx-network`
- Health checks are configured for all services
- Override file (`docker-compose.override.yml`) contains development-specific settings
