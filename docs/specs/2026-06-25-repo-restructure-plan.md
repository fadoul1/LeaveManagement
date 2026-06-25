# Repo Restructure Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Restructurer le monorepo en domaines `backend/`, `bff/`, `frontend/` et activer le Central Package Management NuGet à la racine.

**Architecture:** Le code backend existant (`src/`, `tests/`) se déplace sous `backend/`. Des dossiers placeholder `bff/` et `frontend/` préparent les futurs composants. Un `Directory.Packages.props` à la racine centralise toutes les versions NuGet pour l'ensemble du monorepo.

**Tech Stack:** .NET 10, .NET Aspire 13.4.6, NuGet Central Package Management, Docker, GitHub Actions

---

## Fichiers concernés

| Fichier | Action |
|---|---|
| `bff/.gitkeep` | Créer |
| `frontend/.gitkeep` | Créer |
| `Directory.Build.props` | Créer (racine) |
| `Directory.Packages.props` | Créer (racine) |
| `src/` → `backend/src/` | Déplacer (git mv) |
| `tests/` → `backend/tests/` | Déplacer (git mv) |
| `LeaveManagement.slnx` → `backend/LeaveManagement.slnx` | Déplacer + modifier |
| `backend/src/LeaveManagement.API/Dockerfile` → `backend/Dockerfile` | Déplacer + modifier |
| `backend/src/LeaveManagement.API/LeaveManagement.API.csproj` | Modifier (ref ServiceDefaults + supprimer versions) |
| `backend/src/LeaveManagement.Application/LeaveManagement.Application.csproj` | Modifier (supprimer versions) |
| `backend/src/LeaveManagement.Persistence/LeaveManagement.Persistence.csproj` | Modifier (supprimer versions) |
| `backend/src/LeaveManagement.ExternalServices/LeaveManagement.ExternalServices.csproj` | Modifier (supprimer versions) |
| `backend/tests/LeaveManagement.API.IntegrationTests/LeaveManagement.API.IntegrationTests.csproj` | Modifier (supprimer versions) |
| `backend/tests/LeaveManagement.Tests.Common/LeaveManagement.Tests.Common.csproj` | Inchangé (pas de packages) |
| `_aspire/LeaveManagement.AppHost/LeaveManagement.AppHost.csproj` | Modifier (ref API + supprimer versions) |
| `_aspire/LeaveManagement.ServiceDefaults/LeaveManagement.ServiceDefaults.csproj` | Modifier (supprimer versions) |
| `.github/workflows/ci.yml` | Modifier (chemins solution + Dockerfile) |
| `.github/workflows/cd.yml` | Modifier (chemin Dockerfile) |

---

## Task 1: Créer les placeholders et vérifier le build initial

**Files:**
- Create: `bff/.gitkeep`
- Create: `frontend/.gitkeep`

- [ ] **Step 1: Vérifier que le build fonctionne avant toute modification**

```bash
cd c:/Users/fad.ouro.agoro/source/repos/LeaveManagement
dotnet build --configuration Release
```
Expected: Build succeeded, 0 erreurs. Si des erreurs existent, les corriger avant de continuer.

- [ ] **Step 2: Créer les dossiers placeholder**

```bash
mkdir bff
mkdir frontend
echo "" > bff/.gitkeep
echo "" > frontend/.gitkeep
```

- [ ] **Step 3: Commit**

```bash
git add bff/.gitkeep frontend/.gitkeep
git commit -m "chore: add bff and frontend placeholder directories"
```

---

## Task 2: Déplacer le code backend sous `backend/`

**Files:**
- Move: `src/` → `backend/src/`
- Move: `tests/` → `backend/tests/`
- Move: `LeaveManagement.slnx` → `backend/LeaveManagement.slnx`

> Note: Après ce commit, le build est temporairement cassé — les chemins de la solution et les références cross-dossiers ne sont pas encore à jour. C'est résolu dans les Tasks 3 et 4.

- [ ] **Step 1: Créer le dossier `backend/` et déplacer src, tests, slnx**

```bash
mkdir backend
git mv src backend/src
git mv tests backend/tests
git mv LeaveManagement.slnx backend/LeaveManagement.slnx
```

- [ ] **Step 2: Commit**

```bash
git add -A
git commit -m "chore: move src/ and tests/ under backend/"
```

---

## Task 3: Mettre à jour le fichier solution

**Files:**
- Modify: `backend/LeaveManagement.slnx`

Les chemins des projets `src/` et `tests/` restent valides (relatifs à `backend/`). Seuls les chemins vers `_aspire/` et `.github/` (qui sont à la racine) doivent être préfixés par `../`.

- [ ] **Step 1: Réécrire `backend/LeaveManagement.slnx`**

Remplacer l'intégralité du fichier par :

```xml
<Solution>
  <Folder Name="/.github/">
    <File Path="../.github/copilot-instructions.md" />
  </Folder>
  <Folder Name="/.github/dotnet-instructions/">
    <File Path="../.github/dotnet-instructions/chapter-10_classes.instructions.md" />
    <File Path="../.github/dotnet-instructions/chapter-12_emergence.instructions.md" />
    <File Path="../.github/dotnet-instructions/chapter-15_xUnit_Internals.instructions.md" />
    <File Path="../.github/dotnet-instructions/chapter-2_meaningful_names.instructions.md" />
    <File Path="../.github/dotnet-instructions/chapter-3_functions.instructions.md" />
    <File Path="../.github/dotnet-instructions/chapter-4_comments.instructions.md" />
    <File Path="../.github/dotnet-instructions/chapter-5_formatting.instructions.md" />
    <File Path="../.github/dotnet-instructions/chapter-6_objects_and_data_structures.instructions.md" />
    <File Path="../.github/dotnet-instructions/chapter-7_error_handling.instructions.md" />
    <File Path="../.github/dotnet-instructions/chapter-8_boundaries.instructions.md" />
    <File Path="../.github/dotnet-instructions/chapter-9_unit_tests.instructions.md" />
  </Folder>
  <Folder Name="/.github/prompts/">
    <File Path="../.github/prompts/software-craftsmanship.prompt.md" />
  </Folder>
  <Folder Name="/src/" />
  <Folder Name="/src/Application/">
    <Project Path="src/LeaveManagement.Application/LeaveManagement.Application.csproj" />
  </Folder>
  <Folder Name="/src/Domain/">
    <Project Path="src/LeaveManagement.Domain/LeaveManagement.Domain.csproj" />
  </Folder>
  <Folder Name="/src/Infrastructure/">
    <Project Path="src/LeaveManagement.ExternalServices/LeaveManagement.ExternalServices.csproj" />
    <Project Path="src/LeaveManagement.Persistence/LeaveManagement.Persistence.csproj" />
  </Folder>
  <Folder Name="/src/Presentation/">
    <Project Path="src/LeaveManagement.API/LeaveManagement.API.csproj" />
  </Folder>
  <Folder Name="/tests/">
    <Project Path="tests/LeaveManagement.API.IntegrationTests/LeaveManagement.API.IntegrationTests.csproj" />
    <Project Path="tests/LeaveManagement.Tests.Common/LeaveManagement.Tests.Common.csproj" />
  </Folder>
  <Folder Name="/_aspire/">
    <Project Path="../_aspire/LeaveManagement.AppHost/LeaveManagement.AppHost.csproj" />
    <Project Path="../_aspire/LeaveManagement.ServiceDefaults/LeaveManagement.ServiceDefaults.csproj" />
  </Folder>
</Solution>
```

- [ ] **Step 2: Commit**

```bash
git add backend/LeaveManagement.slnx
git commit -m "chore: update solution file paths for new backend/ layout"
```

---

## Task 4: Corriger les références de projets cross-dossiers

**Files:**
- Modify: `backend/src/LeaveManagement.API/LeaveManagement.API.csproj`
- Modify: `_aspire/LeaveManagement.AppHost/LeaveManagement.AppHost.csproj`

Deux références traversent la frontière `backend/` ↔ racine :
1. `LeaveManagement.API` → `LeaveManagement.ServiceDefaults` : était `..\..\` (2 niveaux → racine), doit être `..\..\..\_aspire\` (3 niveaux → racine)
2. `LeaveManagement.AppHost` → `LeaveManagement.API` : était `..\..\src\` (2 niveaux → racine), doit être `..\..\backend\src\`

- [ ] **Step 1: Corriger la référence ServiceDefaults dans `LeaveManagement.API.csproj`**

Dans `backend/src/LeaveManagement.API/LeaveManagement.API.csproj`, remplacer :
```xml
<ProjectReference Include="..\..\_aspire\LeaveManagement.ServiceDefaults\LeaveManagement.ServiceDefaults.csproj" />
```
Par :
```xml
<ProjectReference Include="..\..\..\_aspire\LeaveManagement.ServiceDefaults\LeaveManagement.ServiceDefaults.csproj" />
```

- [ ] **Step 2: Corriger la référence API dans `LeaveManagement.AppHost.csproj`**

Dans `_aspire/LeaveManagement.AppHost/LeaveManagement.AppHost.csproj`, remplacer :
```xml
<ProjectReference Include="..\..\src\LeaveManagement.API\LeaveManagement.API.csproj" />
```
Par :
```xml
<ProjectReference Include="..\..\backend\src\LeaveManagement.API\LeaveManagement.API.csproj" />
```

- [ ] **Step 3: Vérifier que le build fonctionne**

```bash
dotnet build backend/LeaveManagement.slnx --configuration Release
```
Expected: Build succeeded, 0 erreurs.

- [ ] **Step 4: Commit**

```bash
git add backend/src/LeaveManagement.API/LeaveManagement.API.csproj
git add _aspire/LeaveManagement.AppHost/LeaveManagement.AppHost.csproj
git commit -m "chore: fix cross-boundary project references after backend/ restructure"
```

---

## Task 5: Déplacer et mettre à jour le Dockerfile

**Files:**
- Move: `backend/src/LeaveManagement.API/Dockerfile` → `backend/Dockerfile`

Le Dockerfile est maintenant dans `backend/` mais le contexte Docker dans la CI reste `.` (racine du repo), ce qui permet de COPY les fichiers `_aspire/` et les futurs fichiers racine.

- [ ] **Step 1: Déplacer le Dockerfile**

```bash
git mv backend/src/LeaveManagement.API/Dockerfile backend/Dockerfile
```

- [ ] **Step 2: Réécrire `backend/Dockerfile`**

Remplacer l'intégralité du fichier par :

```dockerfile
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /repo

# Copy root build configuration (required for Central Package Management)
COPY Directory.Build.props .
COPY Directory.Packages.props .
COPY global.json .

# Copy solution and project files for restore layer cache
COPY backend/LeaveManagement.slnx backend/
COPY backend/src/LeaveManagement.Domain/LeaveManagement.Domain.csproj backend/src/LeaveManagement.Domain/
COPY backend/src/LeaveManagement.Application/LeaveManagement.Application.csproj backend/src/LeaveManagement.Application/
COPY backend/src/LeaveManagement.Persistence/LeaveManagement.Persistence.csproj backend/src/LeaveManagement.Persistence/
COPY backend/src/LeaveManagement.ExternalServices/LeaveManagement.ExternalServices.csproj backend/src/LeaveManagement.ExternalServices/
COPY _aspire/LeaveManagement.ServiceDefaults/LeaveManagement.ServiceDefaults.csproj _aspire/LeaveManagement.ServiceDefaults/
COPY backend/src/LeaveManagement.API/LeaveManagement.API.csproj backend/src/LeaveManagement.API/

# Restore dependencies
RUN dotnet restore backend/src/LeaveManagement.API/LeaveManagement.API.csproj

# Copy source code
COPY backend/src/LeaveManagement.Domain/ backend/src/LeaveManagement.Domain/
COPY backend/src/LeaveManagement.Application/ backend/src/LeaveManagement.Application/
COPY backend/src/LeaveManagement.Persistence/ backend/src/LeaveManagement.Persistence/
COPY backend/src/LeaveManagement.ExternalServices/ backend/src/LeaveManagement.ExternalServices/
COPY _aspire/LeaveManagement.ServiceDefaults/ _aspire/LeaveManagement.ServiceDefaults/
COPY backend/src/LeaveManagement.API/ backend/src/LeaveManagement.API/

# Build and publish
WORKDIR /repo/backend/src/LeaveManagement.API
RUN dotnet publish -c Release -o /app/publish --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

USER app

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "LeaveManagement.API.dll"]
```

- [ ] **Step 3: Commit**

```bash
git add backend/Dockerfile
git commit -m "chore: move Dockerfile to backend/ and update COPY paths"
```

---

## Task 6: Activer le Central Package Management

**Files:**
- Create: `Directory.Build.props`
- Create: `Directory.Packages.props`
- Modify: `backend/src/LeaveManagement.API/LeaveManagement.API.csproj`
- Modify: `backend/src/LeaveManagement.Application/LeaveManagement.Application.csproj`
- Modify: `backend/src/LeaveManagement.Persistence/LeaveManagement.Persistence.csproj`
- Modify: `backend/src/LeaveManagement.ExternalServices/LeaveManagement.ExternalServices.csproj`
- Modify: `backend/tests/LeaveManagement.API.IntegrationTests/LeaveManagement.API.IntegrationTests.csproj`
- Modify: `_aspire/LeaveManagement.AppHost/LeaveManagement.AppHost.csproj`
- Modify: `_aspire/LeaveManagement.ServiceDefaults/LeaveManagement.ServiceDefaults.csproj`

> Important: Les Steps 1, 2 et 3 doivent être faits ensemble avant le build — dès que `ManagePackageVersionsCentrally=true` est activé, les `.csproj` ne peuvent plus avoir d'attributs `Version=` sur leurs `PackageReference`.

- [ ] **Step 1: Créer `Directory.Build.props` à la racine**

```xml
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  </PropertyGroup>
</Project>
```

- [ ] **Step 2: Créer `Directory.Packages.props` à la racine**

```xml
<Project>
  <PropertyGroup>
    <CentralPackageTransitivePinningEnabled>true</CentralPackageTransitivePinningEnabled>
  </PropertyGroup>
  <ItemGroup Label="Application">
    <PackageVersion Include="AutoMapper" Version="16.1.1" />
    <PackageVersion Include="FluentValidation" Version="12.1.1" />
    <PackageVersion Include="FluentValidation.DependencyInjectionExtensions" Version="12.1.1" />
    <PackageVersion Include="MediatR" Version="14.1.0" />
    <PackageVersion Include="Swashbuckle.AspNetCore" Version="10.2.3" />
  </ItemGroup>
  <ItemGroup Label="Infrastructure">
    <PackageVersion Include="Microsoft.AspNetCore.Http.Abstractions" Version="2.3.11" />
    <PackageVersion Include="Microsoft.EntityFrameworkCore" Version="10.0.9" />
    <PackageVersion Include="Microsoft.Extensions.DependencyInjection" Version="10.0.9" />
    <PackageVersion Include="Microsoft.Extensions.Logging.Abstractions" Version="10.0.9" />
    <PackageVersion Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="10.0.2" />
  </ItemGroup>
  <ItemGroup Label="Aspire">
    <PackageVersion Include="Aspire.Hosting.AppHost" Version="13.4.6" />
    <PackageVersion Include="Microsoft.Extensions.Http.Resilience" Version="10.7.0" />
    <PackageVersion Include="Microsoft.Extensions.ServiceDiscovery" Version="10.7.0" />
    <PackageVersion Include="OpenTelemetry" Version="1.16.0" />
    <PackageVersion Include="OpenTelemetry.Exporter.OpenTelemetryProtocol" Version="1.16.0" />
    <PackageVersion Include="OpenTelemetry.Extensions.Hosting" Version="1.16.0" />
    <PackageVersion Include="OpenTelemetry.Instrumentation.AspNetCore" Version="1.15.2" />
    <PackageVersion Include="OpenTelemetry.Instrumentation.Http" Version="1.15.1" />
    <PackageVersion Include="OpenTelemetry.Instrumentation.Runtime" Version="1.15.1" />
  </ItemGroup>
  <ItemGroup Label="Tests">
    <PackageVersion Include="coverlet.collector" Version="10.0.1" />
    <PackageVersion Include="FluentAssertions" Version="8.10.0" />
    <PackageVersion Include="Microsoft.AspNetCore.Mvc.Testing" Version="10.0.9" />
    <PackageVersion Include="Microsoft.Data.SqlClient" Version="7.0.1" />
    <PackageVersion Include="Microsoft.Data.Sqlite" Version="10.0.9" />
    <PackageVersion Include="Microsoft.NET.Test.Sdk" Version="18.7.0" />
    <PackageVersion Include="Reqnroll.SpecFlowCompatibility" Version="3.3.3" />
    <PackageVersion Include="Reqnroll.xUnit" Version="3.3.4" />
    <PackageVersion Include="Respawn" Version="7.0.0" />
    <PackageVersion Include="Testcontainers.PostgreSql" Version="4.12.0" />
    <PackageVersion Include="xunit" Version="2.9.3" />
    <PackageVersion Include="xunit.runner.visualstudio" Version="3.1.5" />
  </ItemGroup>
</Project>
```

- [ ] **Step 3: Supprimer les attributs `Version=` de tous les `.csproj` backend**

**`backend/src/LeaveManagement.API/LeaveManagement.API.csproj`** — remplacer le bloc `<ItemGroup>` packages par :
```xml
  <ItemGroup>
    <PackageReference Include="FluentValidation" />
    <PackageReference Include="Swashbuckle.AspNetCore" />
    <PackageReference Include="MediatR" />
  </ItemGroup>
```

**`backend/src/LeaveManagement.Application/LeaveManagement.Application.csproj`** — remplacer le bloc `<ItemGroup>` packages par :
```xml
  <ItemGroup>
    <PackageReference Include="AutoMapper" />
    <PackageReference Include="FluentValidation" />
    <PackageReference Include="FluentValidation.DependencyInjectionExtensions" />
    <PackageReference Include="MediatR" />
    <PackageReference Include="Microsoft.Extensions.Logging.Abstractions" />
  </ItemGroup>
```

**`backend/src/LeaveManagement.Persistence/LeaveManagement.Persistence.csproj`** — remplacer le bloc `<ItemGroup>` packages par :
```xml
  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Http.Abstractions" />
    <PackageReference Include="Microsoft.EntityFrameworkCore" />
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" />
    <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" />
  </ItemGroup>
```

**`backend/src/LeaveManagement.ExternalServices/LeaveManagement.ExternalServices.csproj`** — remplacer le bloc `<ItemGroup>` packages par :
```xml
  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" />
  </ItemGroup>
```

**`backend/tests/LeaveManagement.API.IntegrationTests/LeaveManagement.API.IntegrationTests.csproj`** — remplacer le premier `<ItemGroup>` (packages) par :
```xml
  <ItemGroup>
    <PackageReference Include="coverlet.collector">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" />
    <PackageReference Include="Microsoft.Data.SqlClient" />
    <PackageReference Include="Microsoft.Data.Sqlite" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" />
    <PackageReference Include="Respawn" />
    <PackageReference Include="Testcontainers.PostgreSql" />
    <PackageReference Include="Reqnroll.SpecFlowCompatibility" />
    <PackageReference Include="Reqnroll.xUnit" />
    <PackageReference Include="xunit" />
    <PackageReference Include="xunit.runner.visualstudio">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="FluentAssertions" />
  </ItemGroup>
```

- [ ] **Step 4: Supprimer les attributs `Version=` des `.csproj` Aspire**

**`_aspire/LeaveManagement.AppHost/LeaveManagement.AppHost.csproj`** — remplacer le bloc `<ItemGroup>` packages par :
```xml
  <ItemGroup>
    <PackageReference Include="Aspire.Hosting.AppHost" />
  </ItemGroup>
```
> Note: `<Sdk Name="Aspire.AppHost.Sdk" Version="9.4.0" />` conserve son `Version=` — les références SDK ne sont pas gérées par CPM.

**`_aspire/LeaveManagement.ServiceDefaults/LeaveManagement.ServiceDefaults.csproj`** — remplacer le bloc `<ItemGroup>` packages par :
```xml
  <ItemGroup>
    <FrameworkReference Include="Microsoft.AspNetCore.App" />
    <PackageReference Include="Microsoft.Extensions.Http.Resilience" />
    <PackageReference Include="Microsoft.Extensions.ServiceDiscovery" />
    <PackageReference Include="OpenTelemetry" />
    <PackageReference Include="OpenTelemetry.Exporter.OpenTelemetryProtocol" />
    <PackageReference Include="OpenTelemetry.Extensions.Hosting" />
    <PackageReference Include="OpenTelemetry.Instrumentation.AspNetCore" />
    <PackageReference Include="OpenTelemetry.Instrumentation.Http" />
    <PackageReference Include="OpenTelemetry.Instrumentation.Runtime" />
  </ItemGroup>
```

- [ ] **Step 5: Vérifier que le build fonctionne avec CPM**

```bash
dotnet restore backend/LeaveManagement.slnx
dotnet build backend/LeaveManagement.slnx --configuration Release
```
Expected: Build succeeded, 0 erreurs. Si une erreur `NU1009` apparaît ("package version defined both centrally and locally"), un `Version=` a été oublié dans un `.csproj` — le supprimer.

- [ ] **Step 6: Commit**

```bash
git add Directory.Build.props Directory.Packages.props
git add backend/src/LeaveManagement.API/LeaveManagement.API.csproj
git add backend/src/LeaveManagement.Application/LeaveManagement.Application.csproj
git add backend/src/LeaveManagement.Persistence/LeaveManagement.Persistence.csproj
git add backend/src/LeaveManagement.ExternalServices/LeaveManagement.ExternalServices.csproj
git add backend/tests/LeaveManagement.API.IntegrationTests/LeaveManagement.API.IntegrationTests.csproj
git add _aspire/LeaveManagement.AppHost/LeaveManagement.AppHost.csproj
git add _aspire/LeaveManagement.ServiceDefaults/LeaveManagement.ServiceDefaults.csproj
git commit -m "chore: enable NuGet Central Package Management"
```

---

## Task 7: Mettre à jour les workflows CI/CD

**Files:**
- Modify: `.github/workflows/ci.yml`
- Modify: `.github/workflows/cd.yml`

- [ ] **Step 1: Mettre à jour `ci.yml`**

Remplacer l'intégralité du fichier par :

```yaml
name: CI

on:
  push:
    branches: [develop, master]
  pull_request:
    branches: [develop, master]
  workflow_call:

env:
  DOTNET_VERSION: '10.0.x'
  DOTNET_SKIP_FIRST_TIME_EXPERIENCE: true
  DOTNET_CLI_TELEMETRY_OPTOUT: true
  SOLUTION: backend/LeaveManagement.slnx

jobs:
  build:
    name: Build & Test
    runs-on: ubuntu-latest

    steps:
      - name: Checkout code
        uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: ${{ env.DOTNET_VERSION }}

      - name: Cache NuGet packages
        uses: actions/cache@v4
        with:
          path: ~/.nuget/packages
          key: ${{ runner.os }}-nuget-${{ hashFiles('**/*.csproj', '**/Directory.Packages.props') }}
          restore-keys: |
            ${{ runner.os }}-nuget-

      - name: Restore workloads
        run: dotnet workload restore ${{ env.SOLUTION }}

      - name: Restore dependencies
        run: dotnet restore ${{ env.SOLUTION }}

      - name: Build
        run: dotnet build ${{ env.SOLUTION }} --no-restore --configuration Release

      - name: Run Unit Tests
        run: |
          dotnet test ${{ env.SOLUTION }} --no-build \
            --configuration Release \
            --verbosity normal \
            --logger "trx;LogFileName=unit-tests.trx" \
            --results-directory ./TestResults

      - name: Upload Test Results
        uses: actions/upload-artifact@v4
        if: always()
        with:
          name: test-results
          path: ./TestResults/*.trx

  code-quality:
    name: Code Quality
    runs-on: ubuntu-latest

    steps:
      - name: Checkout code
        uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: ${{ env.DOTNET_VERSION }}

      - name: Restore workloads
        run: dotnet workload restore ${{ env.SOLUTION }}

      - name: Restore dependencies
        run: dotnet restore ${{ env.SOLUTION }}

      - name: Check formatting
        run: dotnet format ${{ env.SOLUTION }} --verify-no-changes --verbosity diagnostic || true

  docker-build:
    name: Docker Build
    runs-on: ubuntu-latest
    needs: build

    steps:
      - name: Checkout code
        uses: actions/checkout@v4

      - name: Set up Docker Buildx
        uses: docker/setup-buildx-action@v3

      - name: Build Docker image
        uses: docker/build-push-action@v6
        with:
          context: .
          file: ./backend/Dockerfile
          push: false
          tags: leavemanagement-api:${{ github.sha }}
          cache-from: type=gha
          cache-to: type=gha,mode=max
```

- [ ] **Step 2: Mettre à jour `cd.yml`**

Dans `.github/workflows/cd.yml`, remplacer les deux occurrences de :
```yaml
          file: ./src/LeaveManagement.API/Dockerfile
```
Par :
```yaml
          file: ./backend/Dockerfile
```
(Uniquement dans le job `build-and-push-staging` — le job `promote-to-production` utilise `docker buildx imagetools create` et n'a pas de `file:`.)

- [ ] **Step 3: Commit**

```bash
git add .github/workflows/ci.yml .github/workflows/cd.yml
git commit -m "chore: update CI/CD for backend/ restructure and Central Package Management"
```

---

## Task 8: Vérification finale

- [ ] **Step 1: Build complet**

```bash
dotnet build backend/LeaveManagement.slnx --configuration Release
```
Expected: `Build succeeded.` avec 0 erreurs.

- [ ] **Step 2: Tests**

```bash
dotnet test backend/LeaveManagement.slnx --configuration Release --no-build
```
Expected: Tous les tests existants passent.

- [ ] **Step 3: Build Docker (simulation locale)**

```bash
docker build -f backend/Dockerfile -t leavemanagement-api:local .
```
Expected: Image construite sans erreur. Si Docker n'est pas disponible localement, skiper et laisser la CI valider.

- [ ] **Step 4: Vérifier la structure finale**

```bash
ls -la
ls backend/
ls bff/
ls frontend/
```
Expected :
```
backend/
  src/
  tests/
  Dockerfile
  LeaveManagement.slnx
bff/
  .gitkeep
frontend/
  .gitkeep
Directory.Build.props
Directory.Packages.props
global.json
_aspire/
infra/
.github/
```

- [ ] **Step 5: Commit final si ajustements**

Si des corrections mineures ont été faites lors de la vérification :
```bash
git add -A
git commit -m "chore: fix post-restructure build issues"
```
