# Infrastructure Azure - LeaveManagement

## Architecture

```
                    ┌─────────────────────────────────────────────────────┐
                    │                    Azure                            │
                    │  ┌─────────────┐   ┌──────────────────────────────┐ │
  GitHub Actions ───┼──│     ACR     │───│   Container Apps Environment │ │
                    │  │  (Registry) │   │  ┌────────────────────────┐  │ │
                    │  └─────────────┘   │  │    Container App       │  │ │
                    │                    │  │    (API .NET 10)       │  │ │
                    │                    │  └───────────┬────────────┘  │ │
                    │                    └──────────────┼───────────────┘ │
                    │                                   │                 │
                    │  ┌────────────────────────────────▼───────────────┐ │
                    │  │         PostgreSQL Flexible Server            │ │
                    │  └───────────────────────────────────────────────┘ │
                    └─────────────────────────────────────────────────────┘
```

## Prérequis

- Azure CLI installé et connecté (`az login`)
- jq installé (pour le parsing JSON)
- Compte GitHub avec accès au repository

## Déploiement Initial

### 1. Déployer l'infrastructure Azure

```bash
# Staging
cd infra
chmod +x deploy.sh
./deploy.sh staging

# Production
./deploy.sh production
```

### 2. Configurer les secrets GitHub

Après le déploiement, ajoutez les secrets suivants dans GitHub :
**Settings > Secrets and variables > Actions > Secrets**

| Secret | Description |
|--------|-------------|
| `ACR_USERNAME` | Username du Container Registry |
| `ACR_PASSWORD` | Password du Container Registry |
| `AZURE_CREDENTIALS` | JSON des credentials du Service Principal |

### 3. Configurer les variables GitHub

**Settings > Secrets and variables > Actions > Variables**

| Variable | Description | Exemple |
|----------|-------------|---------|
| `AZURE_CONTAINER_REGISTRY` | URL du registry | `leavemanagementacr123.azurecr.io` |
| `CONTAINER_APP_NAME` | Nom de la Container App | `leavemanagement-api` |
| `RESOURCE_GROUP` | Nom du Resource Group | `rg-leavemanagement` |

### 4. Configurer les environnements GitHub

Créez deux environnements dans GitHub (**Settings > Environments**) :

1. **staging** - Déploiement automatique
2. **production** - Avec protection (reviewers requis)

## Workflows CI/CD

### CI (`ci.yml`)

Déclenché sur :
- Push sur `develop` ou `master`
- Pull Request vers `develop` ou `master`

Actions :
- Build et tests unitaires
- Vérification de la qualité du code
- Build de l'image Docker (sans push)

### CD (`cd.yml`)

Déclenché sur :
- Push sur `master`
- Déclenchement manuel (workflow_dispatch)

Actions :
- Build et push de l'image Docker vers ACR
- Déploiement sur staging
- Déploiement sur production (après staging)

## Commandes Azure Utiles

```bash
# Voir les logs de l'application
az containerapp logs show \
  --name leavemanagement-api \
  --resource-group rg-leavemanagement \
  --follow

# Redémarrer l'application
az containerapp revision restart \
  --name leavemanagement-api \
  --resource-group rg-leavemanagement

# Voir l'état de l'application
az containerapp show \
  --name leavemanagement-api \
  --resource-group rg-leavemanagement \
  --query properties.latestRevisionFqdn

# Connexion à PostgreSQL
az postgres flexible-server connect \
  --name leavemanagement-db \
  --admin-user pgadmin \
  --admin-password <password>
```

## Variables d'environnement de l'application

| Variable | Description |
|----------|-------------|
| `ASPNETCORE_ENVIRONMENT` | Staging ou Production |
| `ConnectionStrings__TicketManagementConnectionString` | Chaîne de connexion PostgreSQL |

## Coûts estimés (par mois)

| Ressource | SKU | Coût estimé |
|-----------|-----|-------------|
| Container Apps | Consumption | ~$0-20 |
| PostgreSQL | B1ms | ~$15 |
| Container Registry | Basic | ~$5 |
| Log Analytics | Pay-as-you-go | ~$2-5 |

**Total estimé : ~$25-45/mois** (staging avec scale à 0 quand inactif)
