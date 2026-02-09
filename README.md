# PosX - Desktop Point of Sale Application

A production-ready desktop POS application built with .NET MAUI Blazor Hybrid for the frontend and .NET 8 microservices with SQL Server for the backend.

## Architecture Overview

### Frontend
- **.NET MAUI 8.0** - Cross-platform desktop framework
- **Blazor Hybrid** - Web UI in native desktop app
- **Offline Support** - SQLite for local caching
- **Barcode Scanning** - ZXing.Net.Maui integration
- **Receipt Printing** - Platform-specific APIs

### Backend - Microservices
1. **Identity.API** - Authentication & Authorization (JWT)
2. **Sales.API** - Sales transactions, payments, receipts
3. **Products.API** - Product catalog, pricing, categories
4. **Inventory.API** - Stock management, movements, alerts
5. **Customers.API** - Customer management, loyalty programs
6. **Employees.API** - Employee management, roles, permissions
7. **Reports.API** - Analytics, reporting, dashboard data

### Infrastructure
- **API Gateway** - YARP for routing and authentication
- **Message Broker** - RabbitMQ for event-driven architecture
- **Caching** - Redis for performance
- **Logging** - Serilog + Seq for centralized logging
- **Database** - SQL Server 2022 (database-per-service pattern)

## Clean Architecture

Each microservice follows Clean Architecture with 4 layers:

```
ServiceName/
├── ServiceName.Domain/         # Enterprise business rules
├── ServiceName.Application/    # Application business rules (CQRS with MediatR)
├── ServiceName.Infrastructure/ # External interfaces (EF Core, RabbitMQ)
└── ServiceName.API/            # Presentation layer (Controllers, Middleware)
```

## Technology Stack

- **.NET 8.0** (LTS)
- **ASP.NET Core Web API**
- **Entity Framework Core 8.0**
- **MediatR** (CQRS pattern)
- **FluentValidation**
- **AutoMapper**
- **Polly** (Resilience)
- **Serilog** (Logging)
- **Docker & Docker Compose**

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [JetBrains Rider](https://www.jetbrains.com/rider/)
- [SQL Server Management Studio](https://learn.microsoft.com/sql/ssms/download-sql-server-management-studio-ssms) (optional)

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/ssengur01/PosX-Project.git
cd PosX-Project
```

### 2. Start Infrastructure Services

Start SQL Server, RabbitMQ, Redis, and Seq using Docker Compose:

```bash
cd docker
docker-compose up -d
```

Verify services are running:
```bash
docker-compose ps
```

Access management UIs:
- **RabbitMQ Management**: http://localhost:15672 (user: `posx`, pass: `posx123`)
- **Seq Logging**: http://localhost:5341
- **SQL Server**: localhost:1433 (user: `sa`, pass: `PosX@2026!SecurePass`)

### 3. Restore NuGet Packages

```bash
dotnet restore PosX.sln
```

### 4. Build the Solution

```bash
dotnet build PosX.sln
```

### 5. Run Database Migrations

```bash
# Run migrations for each service (will be added in Stage 2)
```

### 6. Run the Applications

```bash
# Run API Gateway
cd src/ApiGateway
dotnet run

# Run microservices (separate terminals)
cd src/Services/Identity/Identity.API
dotnet run

# Run desktop app
cd src/Desktop/PosX.Desktop
dotnet run
```

## Project Structure

```
PosX-Project/
├── .github/workflows/      # CI/CD pipelines
├── docker/                 # Docker Compose configuration
├── docs/                   # Documentation
├── src/
│   ├── Services/          # Backend microservices
│   │   ├── Identity/
│   │   ├── Sales/
│   │   ├── Products/
│   │   ├── Inventory/
│   │   ├── Customers/
│   │   ├── Employees/
│   │   └── Reports/
│   ├── Desktop/           # MAUI Blazor desktop app
│   ├── ApiGateway/        # API Gateway (YARP)
│   └── BuildingBlocks/    # Shared libraries
│       ├── BuildingBlocks.Common/
│       ├── BuildingBlocks.Logging/
│       ├── BuildingBlocks.EventBus/
│       ├── BuildingBlocks.Authentication/
│       └── BuildingBlocks.Resilience/
└── tests/                 # Test projects
```

## BuildingBlocks (Shared Libraries)

### BuildingBlocks.Common
- Base entity and aggregate root classes
- Value object pattern implementation
- Domain event abstractions
- Repository interfaces
- Result pattern for error handling
- Guard clauses for validation

### BuildingBlocks.Logging
- Serilog configuration
- Structured logging setup
- Seq integration
- Request/response logging middleware

### BuildingBlocks.EventBus
- RabbitMQ event bus implementation
- Integration event abstractions
- Event handler interfaces
- Publish/subscribe pattern

### BuildingBlocks.Authentication
- JWT authentication configuration
- Token validation
- Authentication extension methods

### BuildingBlocks.Resilience
- Polly retry policies
- Circuit breaker implementation
- Timeout policies
- HttpClient resilience extensions

## Development Workflow

This project follows a **stage-by-stage** development approach:

- **Stage 1** ✅ (CURRENT): Foundation setup, BuildingBlocks
- **Stage 2**: Identity & Authentication
- **Stage 3**: Products Microservice
- **Stage 4**: Desktop App Shell
- **Stage 5**: Sales Microservice
- **Stage 6**: Desktop App - Sales Module
- **Stage 7-18**: Additional features...

**Important**: Complete each stage fully before moving to the next.

## Docker Infrastructure

### Services Configuration

**SQL Server**
- Port: 1433
- SA Password: `PosX@2026!SecurePass`

**RabbitMQ**
- AMQP Port: 5672
- Management UI: http://localhost:15672
- Credentials: `posx` / `posx123`

**Redis**
- Port: 6379
- Password: `posx123`

**Seq**
- UI: http://localhost:5341

### Docker Commands

```bash
# Start all services
docker-compose up -d

# Stop all services
docker-compose down

# View logs
docker-compose logs -f

# Reset everything (including data)
docker-compose down -v
```

## Testing

Our testing strategy follows the **Test Pyramid** approach with comprehensive unit and integration tests.

### Quick Start

Run all tests:
```bash
dotnet test
```

Run specific test project:
```bash
dotnet test tests/UnitTests/Products.UnitTests/
dotnet test tests/IntegrationTests/Products.IntegrationTests/
```

Run with coverage:
```bash
dotnet test /p:CollectCoverage=true
```

### Test Structure

- **Unit Tests** (70%): Fast, isolated tests for business logic
  - Domain entities, value objects, handlers
  - Location: `tests/UnitTests/`

- **Integration Tests** (20%): API endpoint tests with database
  - Full HTTP request/response cycle
  - Location: `tests/IntegrationTests/`

- **E2E Tests** (10%): Full user workflows (planned for Stage 17)

### Current Coverage

| Component | Coverage |
|-----------|----------|
| Products.Domain | 85% |
| Products.Application | 75% |
| **Overall Target** | **80%** |

### Documentation

- 📖 [Complete Testing Strategy](./docs/TESTING_STRATEGY.md)
- 📋 [Quick Reference Guide](./docs/TESTING_QUICK_REFERENCE.md)

### Test Tools

- **xUnit** - Testing framework
- **Moq** - Mocking framework
- **FluentAssertions** - Readable assertions
- **WebApplicationFactory** - Integration testing

## CI/CD

GitHub Actions workflow automatically:
- Builds the solution on push/PR
- Runs all tests
- Publishes test results

## Contributing

1. Create a feature branch from `main`
2. Make your changes following clean architecture principles
3. Write unit and integration tests
4. Ensure all tests pass
5. Submit a pull request

## Code Style

- Follow C# coding conventions (`.editorconfig`)
- Use meaningful variable and method names
- Keep methods small and focused (single responsibility)
- Write XML documentation for public APIs
- Maintain test coverage > 80%

## License

Copyright © 2026 PosX Team

## Support

For issues and questions:
- GitHub Issues: https://github.com/ssengur01/PosX-Project/issues

---

**Built with ❤️ using .NET 8 and Clean Architecture**
