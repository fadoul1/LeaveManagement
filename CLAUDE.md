# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build and Run Commands

```bash
# Build the solution
dotnet build LeaveManagement.sln

# Run the API
dotnet run --project src/LeaveManagement.API/LeaveManagement.API.csproj

# Run via Aspire (recommended for development - includes orchestration)
dotnet run --project src/LeaveManagement.AppHost/LeaveManagement.AppHost.csproj

# Run all tests
dotnet test LeaveManagement.sln

# Run specific test project
dotnet test tests/LeaveManagement.API.IntegrationTests/LeaveManagement.API.IntegrationTests.csproj
dotnet test tests/LeaveManagement.Persistence.IntegrationTests/LeaveManagement.Persistence.IntegrationTests.csproj

# Run a single test by name
dotnet test --filter "FullyQualifiedName~TestMethodName"
```

Swagger UI available at `https://localhost:7000/swagger/ui` in Development mode.

## Architecture

This is a .NET 8 Clean Architecture solution with CQRS pattern:

```
src/
├── LeaveManagement.API          → ASP.NET Core Web API (Controllers, Startup)
├── LeaveManagement.Application  → CQRS handlers (MediatR), validation (FluentValidation), mapping (AutoMapper)
├── LeaveManagement.Domain       → Entities, repository contracts, enums (no external dependencies)
├── LeaveManagement.Persistence  → EF Core DbContext, repository implementations, SQL Server
├── LeaveManagement.AppHost      → Aspire orchestration host
└── LeaveManagement.ServiceDefaults → Shared Aspire defaults (OpenTelemetry, resilience)
```

## Key Patterns

**CQRS with MediatR**: Commands and Queries in `Application/Features/{Entity}/Commands|Queries/`. Each operation has a handler class.

**Repository Pattern**: Generic `IBaseRepository<T>` with specialized repositories (e.g., `ILeaveRepository`). Implementations in `Persistence/Repositories/`.

**Soft Delete**: Entities inherit from `BaseEntity` with `DeletedAt` field. Delete operations set this timestamp instead of removing records. Repository `GetAllAsync()` filters out soft-deleted items.

**Validation**: FluentValidation validators per command. Validation runs in handlers before business logic. Failed validation returns response with `Success=false`.

## Domain Model

- **Employee**: FirstName, LastName, Email, PhoneNumber, has many Leaves
- **Leave**: LeaveType (enum), Status (enum), StartDate, EndDate, belongs to Employee
- **BaseEntity**: Id, CreatedAt, UpdatedAt, DeletedAt (all entities inherit this)

## Service Registration

Order matters in `StartupExtensions.cs`:
1. ServiceDefaults (Aspire)
2. Application services (MediatR, AutoMapper, validators)
3. Persistence services (DbContext, repositories)

Database auto-creates on startup via `CreateDatabaseAsync()`.

## Testing

BDD-style tests using SpecFlow + xUnit. Feature files in `tests/*/Features/`, step definitions in `StepDefinitions/`. Uses in-memory database and `WebApplicationFactory` for API tests.

## MCP Server Integration

The solution includes an MCP (Model Context Protocol) Server that exposes the LeaveManagement business logic as AI-accessible tools.

### MCP Server Commands

```bash
# Run the MCP Server (stdio transport for Claude Code integration)
dotnet run --project src/LeaveManagement.McpServer/LeaveManagement.McpServer.csproj

# Run MCP Server tests
dotnet test tests/LeaveManagement.McpServer.Tests/LeaveManagement.McpServer.Tests.csproj
```

### MCP Architecture

```
src/
└── LeaveManagement.McpServer    → MCP Server (stdio transport, tool definitions, resource exposure)
    ├── Tools/                   → MCP Tool implementations (one per command/query)
    ├── Resources/               → MCP Resource implementations (schema, enums, health)
    ├── Services/                → Tool registry, schema generator, response serializer
    └── Handlers/                → MCP protocol handlers (ListTools, CallTool, ReadResource)
```

### Available MCP Tools

| Tool | Description | Input |
|------|-------------|-------|
| CreateEmployee | Create a new employee record | FirstName, LastName, Email, PhoneNumber |
| UpdateEmployee | Update an existing employee | EmployeeId, FirstName, LastName, Email, PhoneNumber |
| DeleteEmployee | Soft-delete an employee | EmployeeId |
| GetAllEmployees | List all active employees | (none) |
| GetEmployeeById | Get employee by ID | EmployeeId |
| CreateLeave | Create a leave request | Type, Status, StartDate, EndDate, Reason, EmployeeId |
| GetAllLeaves | List all leaves | (none) |
| GetLeavesByEmployeeId | Get leaves for an employee | EmployeeId |

### Available MCP Resources

| URI | Description |
|-----|-------------|
| `schema://database` | Database schema (tables, columns, relationships) |
| `config://enums` | Enum definitions (LeaveType, LeaveStatus) |
| `status://health` | System health status |

### Claude Code Integration

To use with Claude Code, configure the MCP server in your Claude Code settings:

```json
{
  "mcpServers": {
    "leave-management": {
      "command": "dotnet",
      "args": ["run", "--project", "src/LeaveManagement.McpServer/LeaveManagement.McpServer.csproj"],
      "cwd": "/path/to/LeaveManagement"
    }
  }
}
```

## Spec-Driven Development Workflow

This project uses spec-driven development with planning artifacts in the `docs/` directory.

### Planning Documents

- **[docs/requirements.md](docs/requirements.md)** - User stories and acceptance criteria
- **[docs/plan.md](docs/plan.md)** - Implementation plan with priorities
- **[docs/tasks.md](docs/tasks.md)** - Technical task checklist

### Working with the Task List

When implementing features from `docs/tasks.md`:

1. **Mark tasks as complete**: Change `[ ]` to `[x]` when a task is finished
2. **Keep phases intact**: Do not remove or rename phase headers
3. **Add tasks if needed**: Insert new tasks under the appropriate phase with the next sequential number (e.g., T1.1.8)
4. **Maintain links**: Every task must reference its plan item and requirement(s)
5. **Follow the format**:
   ```markdown
   - [ ] **T{phase}.{section}.{number}** Task description
   - [x] **T1.1.1** Completed task example
   ```
6. **Work sequentially within phases**: Complete Phase 1 before starting Phase 2 (dependencies exist)
7. **Update requirements**: If a task reveals new acceptance criteria, update `docs/requirements.md`

### Task Naming Convention

Tasks use the pattern `T{phase}.{section}.{task}`:
- **T1.2.3** = Phase 1, Section 2, Task 3
- Phases align with `plan.md` phases (P1, P2, etc.)
- Sections align with plan subsections (P1.1, P1.2, etc.)
