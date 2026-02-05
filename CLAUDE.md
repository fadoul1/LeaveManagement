# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Test Commands

```bash
# Build
dotnet build

# Run all tests
dotnet test

# Run specific test class
dotnet test --filter "FullyQualifiedName~{ClassName}"

# Run integration tests only
dotnet test tests/LeaveManagement.API.IntegrationTests

# Run API
dotnet run --project src/Presentation/LeaveManagement.API

# Run with Aspire orchestration
dotnet run --project _aspire/LeaveManagement.AppHost
```

## Architecture

**Clean Architecture with CQRS** - .NET 10.0, PostgreSQL, MediatR

```
src/
├── Domain/LeaveManagement.Domain           # Entities, contracts - no dependencies
├── Application/LeaveManagement.Application # MediatR handlers, validators, mappers
├── Infrastructure/LeaveManagement.Persistence # EF Core, repositories
└── Presentation/LeaveManagement.API        # Controllers (thin, delegate to MediatR)

tests/
├── LeaveManagement.API.IntegrationTests    # Reqnroll BDD tests with Testcontainers
├── LeaveManagement.Integration.Tests       # Persistence layer tests
└── LeaveManagement.Tests.Common            # Shared utilities, Routes.cs
```

**Data Flow:** Controller → MediatR Command/Query → Handler → Validator → Repository → Entity

## CQRS Handler Pattern

All business logic in `Application/Features/{Entity}/{Commands|Queries}/{Action}/`:
```
CreateEmployee/
├── CreateEmployeeCommand.cs      # IRequest<EmployeeResponse?>
├── CreateEmployeeHandler.cs      # IRequestHandler<TCommand, TResponse>
└── CreateEmployeeValidator.cs    # FluentValidation AbstractValidator<T>
```

**Handler structure:**
1. Validate using FluentValidation (async)
2. Return `BaseResponse` with `Success=false` + `ValidationErrors` string on failure
3. Map command to entity using static extension (`command.ToEmployee()`)
4. Call repository
5. Map entity to response (`entity.ToEmployeeResponse()`)
6. Wrap in try-catch returning failed response on exception

## Logging Convention

Use **compile-time logging** with `LoggerMessage` attribute:
```csharp
[LoggerMessage(LogLevel.Information, "[{className}][{methodeName}] Validation of the entries")]
static partial void LogValidationOfTheEntries(ILogger<CreateEmployeeHandler> logger, string className, string methodeName);
```

## Key Conventions

- **Primary constructors** used extensively for DI
- **Extension methods for mapping** instead of AutoMapper (`ToEmployee()`, `ToEmployeeResponse()`)
- **Soft deletes** via `DeletedAt` timestamp in `BaseEntity`
- **ValidationErrors** is a string, not a collection
- **Thin controllers** - delegate to MediatR immediately
- **Connection string key**: `TicketManagementConnectionString`
- **Database auto-created** on startup via `CreateDatabaseAsync()` (skipped in "Test" environment)

## Testing

- **Integration tests**: Reqnroll (BDD) with `.feature` files, `CustomWebApplicationFactory<Program>`, Testcontainers for PostgreSQL
- **Routes**: Centralized in `LeaveManagement.Tests.Common/Routes.cs`
- **Unit tests**: xUnit + Moq + Shouldly, AAA pattern

## Code Quality

Clean Code principles enforced via `.github/instructions/` files:
- Functions: Single responsibility, max 20 lines
- Tests: One assertion per test concept
- Naming: Intention-revealing, searchable
- Comments: Explain "why" not "what"
