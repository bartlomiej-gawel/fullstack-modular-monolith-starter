# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Repository Structure

- **backend/** - .NET 10 modular monolith API
- **docker/** - Docker Compose configuration

## Build and Test Commands

```bash
# Build the backend
dotnet build backend/Starter.slnx

# Run the backend application
dotnet run --project backend/src/Bootstrapper/Bootstrapper.csproj

# Run all tests
dotnet test backend/Starter.slnx

# Run a specific test project
dotnet test backend/tests/Common/Common.Abstractions.UnitTests/Common.Abstractions.UnitTests.csproj

# Run a single test by name
dotnet test --filter "FullyQualifiedName~EmailTests.Create_WithValidEmail_ShouldSucceed"

# Run with Docker Compose
docker compose -f docker/compose.yaml up --build
```

## Backend Architecture

The backend is a **modular monolith** with clean separation between modules that communicate via integration events.

### Project Structure

- **Bootstrapper** (`backend/src/Bootstrapper/`): ASP.NET Core host that dynamically loads and registers all modules at startup. Modules are discovered by scanning for DLLs prefixed with `Modules.`.

- **Common.Abstractions** (`backend/src/Common/Common.Abstractions/`): Shared contracts and base types:
  - `IModule` interface that all modules must implement
  - Domain primitives: `Entity<TId>`, `AggregateRoot<TId>`, `ValueObject`
  - `IDomainEvent` and `IIntegrationEvent` contracts

- **Common.Infrastructure** (`backend/src/Common/Common.Infrastructure/`): Shared infrastructure:
  - `ApplicationDbContext` base class with optional Outbox/Inbox support
  - PostgreSQL database setup with per-module schemas
  - Outbox pattern for reliable integration event publishing
  - Inbox pattern for idempotent event handling
  - Domain and integration event dispatchers with automatic handler discovery

### Module Structure

Each module (e.g., Identity, Communications) follows this pattern:
- `Modules.<Name>/` - Main module with `IModule` implementation, domain, and infrastructure
- `Modules.<Name>.IntegrationEvents/` - Public integration event contracts for cross-module communication

Modules register themselves via the `IModule` interface:
```csharp
public sealed class IdentityModule : IModule
{
    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabaseContext<IdentityModuleDbContext>(SchemaName);
        services.AddOutbox<IdentityModuleDbContext>();
        services.AddInbox<IdentityModuleDbContext>();
    }
    public void Use(WebApplication app) { }
}
```

### Database Architecture

- Each module has its own database schema (e.g., `identity`, `communications`)
- Module DbContexts inherit from `ApplicationDbContext` and specify their schema
- Outbox/Inbox tables are created per-module when enabled
- PostgreSQL is the database provider

### Event System

- **Domain Events**: Internal to a module, dispatched via `DomainEventInterceptor` when entities are saved
- **Integration Events**: Cross-module communication via Outbox pattern with background processor

### Testing

Uses TUnit framework. Tests follow the pattern `backend/tests/<Area>/<ProjectName>.UnitTests/`.

### Configuration

Key settings in `backend/src/Bootstrapper/appsettings.json`:
- `Database:ConnectionString` - PostgreSQL connection
- `Outbox:IntervalSeconds`, `BatchSize`, `MaxRetryCount` - Outbox processor settings
- `Inbox:CleanupIntervalSeconds`, `RetentionDays` - Inbox cleanup settings
