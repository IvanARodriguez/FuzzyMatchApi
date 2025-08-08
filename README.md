# Address Fuzzy Match API - Project Architecture

## Project Structure

```
AddressFuzzyMatchApi/
├── src/
│ ├── AddressFuzzyMatchApi/
│ │ ├── Controllers/ # API Controllers (if using controllers)
│ │ ├── Endpoints/ # Minimal API endpoints
│ │ ├── Extensions/ # Extension methods
│ │ ├── Middleware/ # Custom middleware
│ │ ├── Program.cs # Application entry point
│ │ ├── appsettings.json # Configuration
│ │ └── AddressFuzzyMatchApi.csproj
│ │
│ ├── AddressFuzzyMatchApi.Core/
│ │ ├── Models/ # Domain models and DTOs
│ │ ├── Interfaces/ # Service interfaces
│ │ ├── Services/ # Business logic services
│ │ ├── Exceptions/ # Custom exceptions
│ │ └── AddressFuzzyMatchApi.Core.csproj
│ │
│ ├── AddressFuzzyMatchApi.Infrastructure/
│ │ ├── Data/ # Data access layer
│ │ ├── Repositories/ # Data repositories
│ │ ├── External/ # External service integrations
│ │ └── AddressFuzzyMatchApi.Infrastructure.csproj
│ │
│ └── AddressFuzzyMatchApi.Tests/
│ ├── Unit/ # Unit tests
│ ├── Integration/ # Integration tests
│ ├── TestData/ # Test data files
│ └── AddressFuzzyMatchApi.Tests.csproj
│
├── data/
│ └── addresses_10000.csv # CSV data file
│
├── docker/
│ ├── Dockerfile
│ └── docker-compose.yml
│
├── docs/
│ ├── api-documentation.md
│ └── deployment-guide.md
│
├── scripts/
│ ├── setup.sh
│ └── deploy.sh
│
└── AddressFuzzyMatchApi.sln # Solution file
```

## Layer Responsibilities

1. API Layer (AddressFuzzyMatchApi)

- Purpose: HTTP endpoints, request/response handling, validation
- Dependencies: Core layer only
- Contains:
  - Minimal API endpoints
  - Request/Response models
  - Validation attributes
  - Middleware registration
  - Dependency injection configuration

2. Core Layer (AddressFuzzyMatchApi.Core)

- Purpose: Business logic, domain models, interfaces
- Dependencies: None (pure business logic)
- Contains:
  - Domain models
  - Service interfaces
  - Business logic services
  - Custom exceptions
  - DTOs and mapping profiles

3. Infrastructure Layer (AddressFuzzyMatchApi.Infrastructure)

- Purpose: Data access, external services, file I/O
- Dependencies: Core layer only
- Contains:
  - CSV data access
  - Repository implementations
  - External API clients
  - File system operations
  - Caching implementations

4. Tests Layer (AddressFuzzyMatchApi.Tests)

- Purpose: Unit and integration tests
- Dependencies: All layers
- Contains:
  - Unit tests for services
  - Integration tests for endpoints
  - Test fixtures and mock data
  - Test utilities
