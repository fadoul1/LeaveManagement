# LeaveManagement System - AI Coding Instructions

## Architecture Overview

**Clean Architecture (4 Layers):**
- **Domain** (`LeaveManagement.Domain`): Entities, contracts, enumerations - no dependencies
- **Application** (`LeaveManagement.Application`): CQRS with MediatR, validators, mappers, responses
- **Infrastructure** (`LeaveManagement.Persistence`): EF Core, PostgreSQL, repositories
- **Presentation** (`LeaveManagement.API`): ASP.NET Core Web API controllers

**Data Flow:** Controller → MediatR Command/Query → Handler → Validator → Repository → Entity

## Critical Development Patterns

### CQRS + MediatR Pattern
All business logic uses MediatR handlers in `Application/Features/{Entity}/{Commands|Queries}/{Action}/`:
```
Features/Employees/Commands/CreateEmployee/
  ├── CreateEmployeeCommand.cs      # IRequest<EmployeeResponse?>
  ├── CreateEmployeeHandler.cs      # IRequestHandler<TCommand, TResponse>
  └── CreateEmployeeValidator.cs    # FluentValidation AbstractValidator<T>
```

**Handler Structure (Consistent Across All):**
1. Validate using FluentValidation (async)
2. Return `BaseResponse` with `Success=false` + `ValidationErrors` string on failure
3. Map command to entity using static mapper extension (`command.ToEmployee()`)
4. Call repository method
5. Map entity to response (`entity.ToEmployeeResponse()`)
6. Wrap in try-catch returning failed response on exception

### Logging Convention
Use **compile-time logging** with `LoggerMessage` attribute (not runtime string interpolation):
```csharp
public partial class CreateEmployeeHandler : IRequestHandler<...>
{
    private const string ClassName = nameof(CreateEmployeeHandler);
    
    public async Task<EmployeeResponse?> Handle(...)
    {
        const string methodeName = nameof(Handle);
        LogValidationOfTheEntries(logger, ClassName, methodeName);
        // ...
    }
    
    [LoggerMessage(LogLevel.Information, "[{className}][{methodeName}] Validation of the entries")]
    static partial void LogValidationOfTheEntries(ILogger<CreateEmployeeHandler> logger, string className, string methodeName);
}
```

### Dependency Registration
Each layer has its own service registration extension:
- `ApplicationServiceRegistration.cs`: Registers MediatR + all validators
- `PersistenceServiceRegistration.cs`: Registers DbContext + repositories
- `StartupExtensions.cs`: Orchestrates all layers in API project

### Testing Strategy

**Unit Tests** (`LeaveManagement.Application.UnitTests`):
- Test handlers with Moq for repositories and validators
- Use Shouldly assertions (`result.Success.ShouldBeTrue()`)
- Structure: `Features/{Entity}/{Commands|Queries}/{Action}/{Action}HandlerTests.cs`
- Follow AAA pattern strictly

**Integration Tests** (`LeaveManagement.API.IntegrationTests`):
- SpecFlow (BDD) with Gherkin `.feature` files
- `CustomWebApplicationFactory<Program>` for in-memory testing
- Database seeding via `DatabaseHook.cs`
- Routes defined in `LeaveManagement.Tests.Common/Routes.cs`

### Validation Error Handling
`ValidationErrors` is a **string** (not collection) containing formatted errors:
```csharp
var errors = string.Empty;
foreach (var error in resultErrors)
{
    errors += $"Property {error.PropertyName} failed Validation. Error was: {error.ErrorMessage} \n";
}
return new EmployeeResponse { Success = false, ValidationErrors = errors };
```

## Developer Workflows

**Build & Test:**
```bash
dotnet build                                        # Full solution
dotnet test                                         # All tests
dotnet test --filter "FullyQualifiedName~{Class}"   # Specific test class
```

**Database:**
- Auto-created on startup via `app.CreateDatabaseAsync()` (skipped in "Test" environment)
- Connection string: `TicketManagementConnectionString` (PostgreSQL)
- Migrations: Not currently used (EnsureCreated approach)

**Run API:**
```bash
dotnet run --project LeaveManagement.API
# Swagger: https://localhost:{port}/swagger
```

## Code Quality Enforcement

**Active Clean Code Instructions** (`.github/instructions/`):
- All C# code must follow Clean Code principles (Uncle Bob)
- Specific rules for: naming, functions, comments, error handling, testing, formatting
- **Instruction Files Apply Automatically** via `applyTo` glob patterns in workspace config
- AI agents MUST read relevant `.instructions.md` files before code changes

**Key Conventions from Instructions:**
- Functions: Single responsibility, max 20 lines, descriptive names
- Tests: One assertion per test concept, test data builders for complex objects
- Naming: Intention-revealing, searchable, no Hungarian notation
- Comments: Explain "why" not "what", prefer self-documenting code

## Technology Stack
- **.NET 10.0** (`global.json` specifies SDK)
- **MediatR** for CQRS
- **FluentValidation** for validation
- **EF Core + PostgreSQL** for data access
- **xUnit + Shouldly + Moq** for unit tests
- **SpecFlow** for BDD integration tests
- **Aspire** for orchestration (`LeaveManagement.AppHost`)

## Important Notes
- Primary constructor syntax used extensively (`CreateEmployeeHandler(params...)`)
- Extension methods for mapping (`ToEmployee()`, `ToEmployeeResponse()`)
- All entities inherit from `BaseEntity` (has `Id` property)
- Controllers are thin - delegate to MediatR immediately
- CORS configured with "Open" policy for development
