# LeaveManagement

A cloud-native, production-grade REST API for managing employee leave requests. Built with **.NET 10**, **Clean Architecture**, and **CQRS** pattern, deployed on **Azure Container Apps** via automated **GitHub Actions** pipelines.

---

## Table of Contents

- [Goal](#goal)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Component Map](#component-map)
- [Domain Model](#domain-model)
- [API Endpoints](#api-endpoints)
- [Infrastructure](#infrastructure)
- [CI/CD Pipeline](#cicd-pipeline)
- [Tech Stack](#tech-stack)
- [Getting Started](#getting-started)
- [Testing](#testing)
- [Configuration](#configuration)

---

## Goal

> **This project is built for learning purposes.** It serves as a hands-on reference for applying modern .NET backend practices — Clean Architecture, CQRS, BDD testing, cloud-native deployment, and CI/CD — in a realistic, end-to-end context.

LeaveManagement provides a RESTful backend API that allows organizations to:

- **Manage employees** — create, update, retrieve, and soft-delete employee records
- **Manage leave requests** — submit and query leave requests (sick leave, annual leave, or other) per employee
- **Expose health probes** — liveness and readiness endpoints for container orchestration

The system is intentionally designed as a **microservice-ready backend** with observability, resilience, and multi-environment deployment built in, so each layer of the stack can be studied and understood independently.

---

## Architecture

The project follows **Clean Architecture** (also known as Onion Architecture) with a strict dependency rule: outer layers depend on inner layers, never the reverse.

```
┌─────────────────────────────────────────────────────────────────────┐
│                        CLIENTS / CONSUMERS                          │
│              (Swagger UI, Mobile Apps, Frontend SPAs)               │
└─────────────────────────────┬───────────────────────────────────────┘
                              │ HTTP/REST
                              ▼
┌─────────────────────────────────────────────────────────────────────┐
│                     PRESENTATION LAYER                              │
│                  LeaveManagement.API (net10.0)                      │
│                                                                     │
│   ┌──────────────────────┐     ┌───────────────────────────────┐   │
│   │  EmployeesController │     │       LeavesController        │   │
│   │  GET /employees      │     │  GET  /leaves                 │   │
│   │  GET /employees/{id} │     │  GET  /leaves/{employeeId}    │   │
│   │  POST /employees     │     │  POST /leaves                 │   │
│   │  PUT  /employees     │     └───────────────────────────────┘   │
│   │  DELETE /employees   │                                         │
│   └──────────────────────┘     ┌──────────────────┐               │
│                                │  GET /health      │               │
│   Middleware: CORS, Swagger,   └──────────────────┘               │
│   FluentValidation, Logging                                         │
└─────────────────────────────┬───────────────────────────────────────┘
                              │ MediatR (IRequest / IRequestHandler)
                              ▼
┌─────────────────────────────────────────────────────────────────────┐
│                     APPLICATION LAYER                               │
│              LeaveManagement.Application (net10.0)                  │
│                                                                     │
│   COMMANDS (Write)                   QUERIES (Read)                │
│   ┌─────────────────────────┐        ┌────────────────────────┐    │
│   │ CreateEmployeeCommand   │        │ GetEmployeesListQuery  │    │
│   │ UpdateEmployeeCommand   │        │ GetEmployeeByIdQuery   │    │
│   │ DeleteEmployeeCommand   │        │ GetLeavesListQuery     │    │
│   │ CreateLeaveCommand      │        │ GetLeavesByEmployee    │    │
│   └────────────┬────────────┘        └──────────┬─────────────┘    │
│                │                                │                   │
│   ┌────────────▼────────────────────────────────▼─────────────┐    │
│   │              MediatR Handlers + FluentValidation           │    │
│   │                    AutoMapper (DTOs ↔ Entities)            │    │
│   └────────────────────────────────────────────────────────────┘    │
└─────────────────────────────┬───────────────────────────────────────┘
                              │ Repository Interfaces (Dependency Inversion)
                              ▼
┌─────────────────────────────────────────────────────────────────────┐
│                        DOMAIN LAYER                                 │
│                LeaveManagement.Domain (net10.0)                     │
│                 (No external dependencies)                          │
│                                                                     │
│   ┌──────────────────┐    ┌──────────────────┐                     │
│   │  Employee        │    │  Leave           │                     │
│   │  - FirstName     │    │  - Type          │                     │
│   │  - LastName      │    │  - Status        │                     │
│   │  - Email         │◄───│  - StartDate     │                     │
│   │  - PhoneNumber   │    │  - EndDate       │                     │
│   │  - Leaves (nav)  │    │  - Reason        │                     │
│   └──────────────────┘    │  - EmployeeId    │                     │
│                           └──────────────────┘                     │
│   ┌──────────────────────────────────────────────────────────┐     │
│   │  BaseEntity: Id, CreatedAt, UpdatedAt, DeletedAt         │     │
│   │  IBaseRepository<T>, ILeaveRepository                    │     │
│   │  LeaveTypeEnum, LeaveStatusEnum                          │     │
│   └──────────────────────────────────────────────────────────┘     │
└─────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────┐
│                    INFRASTRUCTURE LAYERS                            │
│                                                                     │
│  ┌──────────────────────────────────┐  ┌────────────────────────┐  │
│  │  LeaveManagement.Persistence     │  │ LeaveManagement        │  │
│  │  (EF Core + PostgreSQL)          │  │ .ExternalServices      │  │
│  │                                  │  │                        │  │
│  │  ApplicationContext (DbContext)  │  │  ITimeProvider         │  │
│  │  BaseRepository<T>               │  │  SystemTimeProvider    │  │
│  │  LeaveRepository                 │  │  (UTC time abstraction │  │
│  │  Migrations                      │  │   for testability)     │  │
│  └──────────────┬───────────────────┘  └────────────────────────┘  │
│                 │ Npgsql                                            │
│                 ▼                                                   │
│         ┌──────────────────┐                                        │
│         │   PostgreSQL 16  │                                        │
│         │  (Docker local / │                                        │
│         │   Azure Flexible │                                        │
│         │   Server cloud)  │                                        │
│         └──────────────────┘                                        │
└─────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────┐
│                     ORCHESTRATION (Dev Only)                        │
│                                                                     │
│  ┌──────────────────────────────────┐  ┌────────────────────────┐  │
│  │  LeaveManagement.AppHost         │  │  ServiceDefaults       │  │
│  │  (.NET Aspire Orchestrator)      │  │  - OpenTelemetry       │  │
│  │  Manages services, PostgreSQL,   │  │  - Service Discovery   │  │
│  │  dashboards in local dev         │  │  - HTTP Resilience     │  │
│  └──────────────────────────────────┘  └────────────────────────┘  │
└─────────────────────────────────────────────────────────────────────┘
```

### Architecture Principles

| Principle | Implementation |
|-----------|---------------|
| **Clean Architecture** | Strict inward dependency rule across 4 layers |
| **CQRS** | MediatR separates Commands (write) from Queries (read) |
| **Repository Pattern** | Domain defines interfaces; Persistence implements them |
| **Dependency Inversion** | All abstractions owned by inner layers |
| **Soft Delete** | `DeletedAt` timestamp on `BaseEntity` — data is never physically removed |
| **Structured Logging** | `LoggerMessage` source generators for zero-allocation logging |

---

## Project Structure

```
LeaveManagement/
│
├── src/
│   ├── LeaveManagement.API/                  # HTTP entry point
│   │   ├── Controllers/
│   │   │   ├── EmployeesController.cs
│   │   │   └── LeavesController.cs
│   │   ├── StartupExtensions.cs              # DI registrations & middleware
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   ├── appsettings.Development.json      # Local PostgreSQL connection
│   │   └── Dockerfile                        # Multi-stage production build
│   │
│   ├── LeaveManagement.Application/          # Business logic (CQRS)
│   │   ├── Commands/
│   │   │   ├── CreateEmployee/
│   │   │   ├── UpdateEmployee/
│   │   │   ├── DeleteEmployee/
│   │   │   └── CreateLeave/
│   │   ├── Queries/
│   │   │   ├── GetEmployeesList/
│   │   │   ├── GetEmployeeById/
│   │   │   ├── GetLeavesList/
│   │   │   └── GetLeavesByEmployeeId/
│   │   ├── Mappers/                          # AutoMapper profiles
│   │   ├── Validators/                       # FluentValidation rules
│   │   └── ServiceRegistration.cs
│   │
│   ├── LeaveManagement.Domain/               # Core — no dependencies
│   │   ├── Entities/
│   │   │   ├── BaseEntity.cs
│   │   │   ├── Employee.cs
│   │   │   └── Leave.cs
│   │   ├── Enumerations/
│   │   │   ├── LeaveTypeEnum.cs
│   │   │   └── LeaveStatusEnum.cs
│   │   └── Repositories/
│   │       ├── IBaseRepository.cs
│   │       └── ILeaveRepository.cs
│   │
│   ├── LeaveManagement.Persistence/          # EF Core + PostgreSQL
│   │   ├── ApplicationContext.cs
│   │   ├── Repositories/
│   │   │   ├── BaseRepository.cs
│   │   │   └── LeaveRepository.cs
│   │   └── ServiceRegistration.cs
│   │
│   ├── LeaveManagement.ExternalServices/     # External integrations
│   │   ├── TimeProvider/
│   │   │   ├── ITimeProvider.cs
│   │   │   └── SystemTimeProvider.cs
│   │   └── ServiceRegistration.cs
│   │
│   └── LeaveManagement.McpServer/            # MCP (AI tool integration)
│
├── _aspire/
│   ├── LeaveManagement.AppHost/              # .NET Aspire orchestration
│   └── LeaveManagement.ServiceDefaults/      # Shared observability config
│
├── tests/
│   ├── LeaveManagement.API.IntegrationTests/ # BDD integration tests
│   │   ├── Features/
│   │   │   ├── EmployeeAPI.feature           # Gherkin scenarios
│   │   │   └── LeaveAPI.feature
│   │   ├── StepDefinitions/
│   │   │   ├── EmployeeApiStepDefinitions.cs
│   │   │   └── LeaveApiStepDefinitions.cs
│   │   └── Support/
│   │       ├── CustomWebApplicationFactory.cs # Testcontainers + DI overrides
│   │       ├── DatabaseHook.cs               # Seed & clean between tests
│   │       ├── FakeTimeProvider.cs
│   │       └── Utilities.cs
│   │
│   └── LeaveManagement.Tests.Common/         # Shared test utilities
│
├── infra/
│   ├── main.bicep                            # Azure IaC (full cloud stack)
│   ├── deploy.sh                             # Bootstrap deployment script
│   └── README.md                             # Deployment documentation
│
├── .github/
│   ├── workflows/
│   │   ├── ci.yml                            # Build, test, code quality
│   │   ├── cd.yml                            # Deploy to Azure
│   │   └── dotnet.yml                        # Legacy workflow
│   ├── dotnet-instructions/                  # Clean code guides (Copilot)
│   ├── prompts/                              # Software craftsmanship prompts
│   └── copilot-instructions.md
│
└── LeaveManagement.slnx                      # Solution file (modern format)
```

---

## Component Map

The diagram below shows how every component connects at runtime:

```
  GitHub Push
      │
      ▼
  GitHub Actions CI ──────────────────────────────────────────────────┐
  (ci.yml)                                                            │
  ├── dotnet build                                                    │
  ├── dotnet test (Reqnroll BDD)                                      │
  │     ├── Testcontainers.PostgreSql (ephemeral DB)                 │
  │     └── CustomWebApplicationFactory (in-process API)             │
  └── dotnet format verify                                            │
                                                                      │
  GitHub Actions CD                                                   │
  (cd.yml)                                                            │
  ├── docker build → push → ACR (tag: staging + sha)                 │
  ├── ACR Webhook → Azure Container Apps (Staging) ◄──────deploys────┘
  ├── Smoke test: GET /health (15 retries × 15s)
  └── Promote: re-tag staging → production
        └── ACR Webhook → Azure Container Apps (Production)

─────────────────────────────────────────────────────────────────────

  AZURE CLOUD (per environment: staging / production)

  ┌─────────────────────────────────────────────────────────────┐
  │              Azure Container Apps Environment               │
  │                                                             │
  │  ┌──────────────────────────────────────────────────────┐  │
  │  │           LeaveManagement Container App              │  │
  │  │           Port 8080 | External HTTP Ingress           │  │
  │  │                                                       │  │
  │  │  ┌──────────────────────────────────────────────┐   │  │
  │  │  │  ASP.NET Core 10 API                         │   │  │
  │  │  │  ├── /api/employees                          │   │  │
  │  │  │  ├── /api/leaves                             │   │  │
  │  │  │  └── /health (liveness + readiness probes)   │   │  │
  │  │  └──────────────────────────────────────────────┘   │  │
  │  │                                                       │  │
  │  │  Scaling:                                             │  │
  │  │  - Staging:    0–3 replicas (scale-to-zero)          │  │
  │  │  - Production: 1–10 replicas                         │  │
  │  └──────────────────────────────────────────────────────┘  │
  │                          │                                  │
  │          Npgsql connection string (env var)                 │
  │                          ▼                                  │
  │  ┌──────────────────────────────────────────────────────┐  │
  │  │         Azure PostgreSQL Flexible Server 16          │  │
  │  │         Standard_B1ms | 32GB | 7-day backup          │  │
  │  │         Database: leavemanagement                    │  │
  │  └──────────────────────────────────────────────────────┘  │
  │                                                             │
  │  ┌───────────────────────┐  ┌──────────────────────────┐  │
  │  │  Log Analytics        │  │  Container Registry (ACR)│  │
  │  │  Workspace            │  │  Basic tier              │  │
  │  │  30-day retention     │  │  Image repository        │  │
  │  └───────────────────────┘  └──────────────────────────┘  │
  └─────────────────────────────────────────────────────────────┘

─────────────────────────────────────────────────────────────────────

  LOCAL DEVELOPMENT (with .NET Aspire)

  ┌──────────────────────────────────────────────────────┐
  │  LeaveManagement.AppHost (Aspire Orchestrator)       │
  │  ├── Launches API project                            │
  │  ├── Provisions PostgreSQL (Docker container)        │
  │  ├── Aspire Dashboard (traces, logs, metrics)        │
  │  └── Service Discovery wires service URLs            │
  └──────────────────────────────────────────────────────┘
```

---

## Domain Model

```
┌─────────────────────────────────────────────────────────┐
│                     BaseEntity                          │
│─────────────────────────────────────────────────────────│
│ + Id         : long                                     │
│ + CreatedAt  : DateTimeOffset                           │
│ + UpdatedAt  : DateTimeOffset                           │
│ + DeletedAt  : DateTimeOffset?   (null = not deleted)   │
└─────────────────────────────────────────────────────────┘
                          ▲
           ┌──────────────┴──────────────┐
           │                             │
┌──────────┴──────────┐       ┌──────────┴──────────────┐
│      Employee       │       │          Leave           │
│─────────────────────│       │──────────────────────────│
│ + FirstName  string │       │ + Type       LeaveType   │
│ + LastName   string │       │   (SickLeave |           │
│ + Email      string │       │    AnnualLeave | Other)  │
│ + PhoneNumber string│       │ + Status     LeaveStatus │
│ + Leaves     ICol.. │◄──────│   (InProgress | Finish)  │
└─────────────────────┘       │ + StartDate  DateTime    │
                              │ + EndDate    DateTime    │
                              │ + Reason     string      │
                              │ + EmployeeId long  (FK)  │
                              └──────────────────────────┘
```

**Enumerations**

| `LeaveTypeEnum` | `LeaveStatusEnum` |
|-----------------|-------------------|
| `SickLeave`     | `InProgress`      |
| `AnnualLeave`   | `Finish`          |
| `Other`         |                   |

---

## API Endpoints

### Employees — `api/employees`

| Method   | Path                  | Description              | Body              |
|----------|-----------------------|--------------------------|-------------------|
| `GET`    | `/api/employees`      | List all employees       | —                 |
| `GET`    | `/api/employees/{id}` | Get employee by ID       | —                 |
| `POST`   | `/api/employees`      | Create employee          | `CreateEmployeeCommand` |
| `PUT`    | `/api/employees`      | Update employee          | `UpdateEmployeeCommand` |
| `DELETE` | `/api/employees/{id}` | Soft-delete employee     | —                 |

### Leaves — `api/leaves`

| Method | Path                        | Description                | Body                |
|--------|-----------------------------|----------------------------|---------------------|
| `GET`  | `/api/leaves`               | List all leave requests    | —                   |
| `GET`  | `/api/leaves/{employeeId}`  | Get leaves for an employee | —                   |
| `POST` | `/api/leaves`               | Submit a leave request     | `CreateLeaveCommand` |

### Health

| Method | Path      | Description                   |
|--------|-----------|-------------------------------|
| `GET`  | `/health` | Liveness / readiness probe    |

> Swagger UI is available at `/swagger` in `Development` mode.

---

## Infrastructure

All cloud resources are defined as **Infrastructure as Code** using Azure Bicep (`infra/main.bicep`).

```
Azure Resource Group
│
├── Azure Container Registry (ACR)        — stores Docker images
│   └── Basic tier, admin user enabled
│
├── Log Analytics Workspace               — centralized logging
│   └── 30-day retention, Pay-as-you-go
│
├── Container Apps Environment            — managed PaaS runtime
│   └── Integrated with Log Analytics
│
├── PostgreSQL Flexible Server 16         — managed relational DB
│   ├── SKU: Standard_B1ms (burstable)
│   ├── Storage: 32 GB (auto-grow enabled)
│   ├── Backup: 7-day retention
│   └── Firewall: allows Azure-internal services
│
└── Container App                         — runs the API
    ├── Image: pulled from ACR
    ├── CPU: 0.5 vCPU | Memory: 1 Gi
    ├── Port: 8080, HTTP external ingress
    ├── Health probes: liveness, readiness, startup
    ├── Env vars: ASPNETCORE_ENVIRONMENT, ConnectionStrings__DefaultConnection
    └── Scaling:
        ├── staging    → min: 0  max: 3  (scale-to-zero, cost-saving)
        └── production → min: 1  max: 10
```

### Environments

| Parameter     | Staging                    | Production                  |
|---------------|----------------------------|-----------------------------|
| Replicas      | 0–3 (scale-to-zero)        | 1–10                        |
| ASPNETCORE_ENVIRONMENT | Staging          | Production                  |
| Purpose       | QA / smoke testing         | Live traffic                |

---

## CI/CD Pipeline

```
Developer pushes to master
          │
          ▼
┌─────────────────────────────────────────────────────────────┐
│                    ci.yml (Build & Test)                    │
│                                                             │
│  Job 1: build-and-test                                      │
│  ├── Checkout + Setup .NET 10                               │
│  ├── Restore NuGet (cached)                                 │
│  ├── dotnet build --configuration Release                   │
│  ├── dotnet test (Reqnroll BDD + xUnit)                     │
│  └── Upload test results artifact                           │
│                                                             │
│  Job 2: code-quality                                        │
│  └── dotnet format --verify-no-changes                      │
│                                                             │
│  Job 3: docker-build                                        │
│  └── docker buildx build (validate, no push)               │
└─────────────────────────────────────────────────────────────┘
          │ on success
          ▼
┌─────────────────────────────────────────────────────────────┐
│                    cd.yml (Deploy)                          │
│                                                             │
│  Job: build-and-push-staging                                │
│  ├── Login to ACR (admin credentials via GitHub Secrets)    │
│  ├── docker buildx build + push                             │
│  │   Tags: :staging  :sha-{commit}                          │
│  ├── Wait 60s (webhook propagation)                         │
│  └── Smoke test: GET /health (15 attempts × 15s)           │
│                        │                                    │
│                on smoke test pass                           │
│                        ▼                                    │
│  Job: promote-to-production                                 │
│  ├── Re-tag: staging → production  (no rebuild!)           │
│  └── ACR webhook → Production Container App auto-deploys   │
└─────────────────────────────────────────────────────────────┘
```

**GitHub Secrets required:**

| Secret              | Description                    |
|---------------------|--------------------------------|
| `ACR_USERNAME`      | Azure Container Registry login |
| `ACR_PASSWORD`      | Azure Container Registry token |

**GitHub Variables required:**

| Variable            | Description                    |
|---------------------|--------------------------------|
| `ACR_LOGIN_SERVER`  | e.g. `myregistry.azurecr.io`  |
| `STAGING_HEALTH_URL`| Full URL to staging /health    |

---

## Tech Stack

| Layer              | Technology                          | Version  |
|--------------------|-------------------------------------|----------|
| Runtime            | .NET                                | 10.0     |
| API Framework      | ASP.NET Core Web API                | 10.0     |
| Architecture       | CQRS via MediatR                    | 12.5.0   |
| Validation         | FluentValidation                    | 12.1.0   |
| Mapping            | AutoMapper                          | 15.1.0   |
| ORM                | Entity Framework Core               | 10.0.0   |
| Database           | PostgreSQL                          | 16       |
| DB Driver          | Npgsql EF Core Provider             | 10.0.0   |
| API Docs           | Swashbuckle / Swagger               | 10.0.1   |
| Observability      | OpenTelemetry                       | 1.14.0   |
| Resilience         | Microsoft.Extensions.Http.Resilience| 10.0.0   |
| Service Discovery  | Microsoft.Extensions.ServiceDiscovery | 10.0.0 |
| Dev Orchestration  | .NET Aspire                         | 13.0.0   |
| BDD Testing        | Reqnroll (SpecFlow successor)       | 3.2.1    |
| Test Assertions    | FluentAssertions                    | 8.8.0    |
| DB in Tests        | Testcontainers.PostgreSql           | 4.1.0    |
| DB Reset in Tests  | Respawn                             | 6.2.1    |
| Unit Test Runner   | xUnit                               | 2.9.3    |
| Containers         | Docker (multi-stage Dockerfile)     | —        |
| Cloud Platform     | Azure Container Apps                | —        |
| IaC                | Azure Bicep                         | —        |
| CI/CD              | GitHub Actions                      | —        |

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- PostgreSQL 16 **or** Docker (for local DB via Aspire)

### Option A — Run with .NET Aspire (recommended)

Aspire automatically provisions PostgreSQL via Docker and wires all services:

```bash
# Restore workloads first (once)
dotnet workload restore

# Run the Aspire AppHost
dotnet run --project _aspire/LeaveManagement.AppHost
```

The Aspire dashboard opens at `https://localhost:15888` with live logs, traces, and metrics. The API is accessible via the URL shown in the dashboard.

### Option B — Run API directly

1. Start a local PostgreSQL instance (or use Docker):
   ```bash
   docker run -d \
     --name postgres-leave \
     -e POSTGRES_PASSWORD=admin \
     -p 5432:5432 \
     postgres:16
   ```

2. The connection string in `src/LeaveManagement.API/appsettings.Development.json` defaults to:
   ```
   Host=localhost;Port=5432;Database=LeaveManagementDbDev;Username=postgres;Password=admin
   ```

3. Run the API:
   ```bash
   dotnet run --project src/LeaveManagement.API
   ```

4. Open Swagger UI: `https://localhost:{port}/swagger`

> The application automatically runs EF Core migrations on startup — no manual `dotnet ef database update` required.

---

## Testing

Integration tests use **Reqnroll** (BDD with Gherkin) backed by a real PostgreSQL database spun up via **Testcontainers**.

```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run only integration tests
dotnet test tests/LeaveManagement.API.IntegrationTests
```

**How tests work:**

```
Reqnroll Feature File (.feature)
         │  Gherkin steps
         ▼
Step Definitions (.cs)
         │  HTTP calls via HttpClient
         ▼
CustomWebApplicationFactory
  ├── Spins up real ASP.NET Core pipeline (in-process)
  ├── Replaces PostgreSQL with Testcontainers instance
  └── Injects FakeTimeProvider (fixed date: 2025-12-01)
         │
         ▼
DatabaseHook
  ├── Seeds initial data before each scenario
  └── Uses Respawn to reset DB state between scenarios
```

---

## Configuration

| Setting                                      | Default (Development)                              |
|----------------------------------------------|----------------------------------------------------|
| `ConnectionStrings__DefaultConnection`       | `Host=localhost;Port=5432;Database=LeaveManagementDbDev;Username=postgres;Password=admin` |
| `ASPNETCORE_ENVIRONMENT`                     | `Development`                                      |
| Health endpoint                              | `/health`                                          |
| Swagger UI                                   | `/swagger` (Development only)                      |
| API base port                                | Defined by launchSettings or Aspire                |

In **production/staging** on Azure, the connection string is injected via Container App environment variables defined in `main.bicep`.

---

## Deployment

See [infra/README.md](infra/README.md) for full deployment documentation including:

- Azure prerequisites and initial setup
- How to configure GitHub Secrets and Variables
- ACR webhook configuration for automated deployments
- Useful Azure CLI commands
- Cost estimation

---

## Project Philosophy

This project follows **Software Craftsmanship** and **Clean Code** principles documented under `.github/`:

- **Single Responsibility** — each class has one reason to change
- **CQRS** — write paths never pollute read paths
- **Testability** — every external dependency is abstracted (`ITimeProvider`, `IBaseRepository`)
- **No magic strings** — enums, constants, and strongly-typed models throughout
- **Soft deletes** — data integrity is preserved; nothing is permanently removed
- **Structured logging** — `LoggerMessage` source generators for high-performance, zero-allocation logs
- **Observability by default** — OpenTelemetry traces and metrics included from the start
