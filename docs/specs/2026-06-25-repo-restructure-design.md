# Design — Restructuration du monorepo LeaveManagement

**Date :** 2026-06-25
**Scope :** Restructuration du repository uniquement — pas de développement BFF ou Angular

---

## Contexte

Le repository contient actuellement uniquement le backend .NET 10 (Clean Architecture + CQRS). L'objectif est de le restructurer pour accueillir à terme un frontend Angular et un BFF d'authentification Keycloak, sans encore développer ces composants.

## Structure cible

```
LeaveManagement/
├── .github/                          # inchangé
├── Directory.Packages.props          # NEW — versions centralisées de tous les packages NuGet
├── Directory.Build.props             # NEW — props partagées (CPM activé, TreatWarningsAsErrors)
├── global.json                       # inchangé — contrainte SDK .NET 10
├── backend/
│   ├── src/
│   │   ├── LeaveManagement.API/
│   │   ├── LeaveManagement.Application/
│   │   ├── LeaveManagement.Domain/
│   │   ├── LeaveManagement.Persistence/
│   │   └── LeaveManagement.ExternalServices/
│   ├── tests/
│   │   ├── LeaveManagement.API.IntegrationTests/
│   │   ├── LeaveManagement.Application.UnitTests/
│   │   └── LeaveManagement.Tests.Common/
│   ├── LeaveManagement.slnx          # déplacé depuis la racine
│   └── Dockerfile                    # déplacé depuis la racine
├── bff/
│   └── .gitkeep                      # placeholder — BFF Keycloak à développer plus tard
├── frontend/
│   └── .gitkeep                      # placeholder — Angular à développer plus tard
├── _aspire/
│   ├── LeaveManagement.AppHost/      # inchangé — références projets mises à jour
│   └── LeaveManagement.ServiceDefaults/
└── infra/                            # inchangé
```

## Gestion centralisée des packages (Central Package Management)

### `Directory.Build.props` (racine)
```xml
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  </PropertyGroup>
</Project>
```

### `Directory.Packages.props` (racine)
Toutes les versions extraites des `.csproj` existants et centralisées ici. Les `.csproj` ne gardent que `<PackageReference Include="PackageName" />` sans attribut `Version`.

Packages à centraliser (versions actuelles) :
- `MediatR` 14.1.0
- `FluentValidation` 12.1.1
- `AutoMapper` 16.1.1
- `Microsoft.EntityFrameworkCore` 10.0.9
- `Npgsql.EntityFrameworkCore.PostgreSQL` 10.0.2
- `Microsoft.Extensions.DependencyInjection` 10.0.9
- `Swashbuckle.AspNetCore` (version à extraire du csproj)
- `xunit` 2.9.3
- `Moq` (version à extraire)
- `Shouldly` (version à extraire)
- `FluentAssertions` (version à extraire)
- `Reqnroll` 3.3.4
- `Testcontainers.PostgreSql` 4.12.0
- Packages Aspire (version à extraire)

## Fichiers à modifier

| Fichier | Changement |
|---|---|
| `backend/LeaveManagement.slnx` | Chemins relatifs de tous les projets mis à jour après déplacement |
| `_aspire/LeaveManagement.AppHost/*.csproj` | `ProjectReference` vers `../../backend/src/...` et `../../backend/tests/...` |
| `backend/Dockerfile` | Contexte Docker et chemins `COPY` ajustés pour nouvelle position |
| `.github/workflows/ci.yml` | Chemins vers `backend/src`, `backend/tests`, `backend/LeaveManagement.slnx` |
| `.github/workflows/cd.yml` | Chemin du Dockerfile → `backend/Dockerfile` |
| Tous les `.csproj` backend | Suppression des attributs `Version=` sur `PackageReference` |

## Ce qui ne change pas

- Logic applicative, domaine, persistence — aucune modification fonctionnelle
- `_aspire/` reste à la racine (orchestre tous les services à terme)
- `infra/` reste à la racine (IaC Azure partagée)
- `global.json` reste à la racine (s'applique à tous les projets .NET du monorepo)

## Décisions clés

- **`.slnx` dans `backend/`** : la solution ne concerne que les projets .NET backend ; l'AppHost Aspire référence les projets via chemins relatifs, pas via la solution
- **`Directory.Packages.props` à la racine** : couvre backend, Aspire, et couvrira automatiquement le BFF (aussi .NET) quand il sera créé
- **Keycloak** choisi comme IdP pour le futur BFF — aucun impact sur cette phase

## Hors scope

- Développement du BFF (Keycloak, ASP.NET Core)
- Développement du frontend Angular
- Mise à jour de l'AppHost Aspire pour orchestrer BFF et frontend
- Mise à jour de l'infrastructure Azure pour les nouveaux composants
