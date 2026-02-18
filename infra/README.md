# Infrastructure Azure - LeaveManagement

## Architecture

```
                    ┌─────────────────────────────────────────────────────┐
                    │                    Azure                            │
                    │  ┌─────────────┐   ┌──────────────────────────────┐ │
  GitHub Actions ───┼──│     ACR     │──►│   Container Apps Environment │ │
   (push image)     │  │  (Registry) │   │  ┌────────────────────────┐  │ │
                    │  └──────┬──────┘   │  │    Container App       │  │ │
                    │         │ webhook  │  │    (API .NET 10)       │  │ │
                    │         └────────► │  └───────────┬────────────┘  │ │
                    │                    └──────────────┼───────────────┘ │
                    │                                   │                 │
                    │  ┌────────────────────────────────▼───────────────┐ │
                    │  │         PostgreSQL Flexible Server             │ │
                    │  └───────────────────────────────────────────────┘ │
                    └─────────────────────────────────────────────────────┘
```

## Stratégie de déploiement

Le pipeline CD utilise uniquement les **identifiants admin de l'ACR** pour pousser les images
Docker. **Aucun Service Principal ni App Registration Entra ID n'est nécessaire.**

Le déploiement automatique est assuré par le **Continuous Deployment** d'Azure Container Apps,
qui crée un webhook ACR pour détecter les nouvelles images poussées.

### Flux

1. **CI** : Build, tests, qualité du code
2. **CD - Push** : Build et push de l'image avec tag `staging` + SHA du commit
3. **ACR webhook** → Container App staging crée une nouvelle révision automatiquement
4. **Smoke test** : Vérification du endpoint `/health`
5. **Promotion** : Re-tag de l'image `staging` → `production` (sans rebuild)
6. **ACR webhook** → Container App production crée une nouvelle révision automatiquement

## Prérequis

- Azure CLI installé et connecté (`az login`)
- Compte GitHub avec accès au repository

## Déploiement Initial

### 1. Déployer l'infrastructure Azure

```bash
cd infra
chmod +x deploy.sh
./deploy.sh staging
```

Le script :
- Crée le Resource Group, ACR, Container Apps Environment, PostgreSQL
- Pousse une image placeholder avec le tag `staging`
- Met à jour la Container App pour référencer l'image taguée
- Affiche les credentials et variables à configurer dans GitHub

Pour la production :
```bash
./deploy.sh production
```

### 2. Activer le Continuous Deployment (une seule fois)

Dans le **portail Azure**, pour chaque Container App :

1. Ouvrir la ressource Container App
2. Menu gauche → **Continuous deployment**
3. Activer le déploiement continu
4. Container registry : votre ACR
5. Image : `leavemanagement-api`
6. Tag : `staging` (ou `production` selon l'environnement)

Ceci crée un webhook ACR qui déclenche automatiquement une nouvelle révision
quand une image est poussée avec le tag correspondant.

> **Aucun Service Principal ni Entra ID requis** pour cette approche.

### 3. Configurer les secrets GitHub

**Settings > Secrets and variables > Actions > Secrets**

| Secret | Description |
|--------|-------------|
| `ACR_USERNAME` | Username admin du Container Registry |
| `ACR_PASSWORD` | Password admin du Container Registry |

### 4. Configurer les variables GitHub

**Settings > Secrets and variables > Actions > Variables**

| Variable | Description | Exemple |
|----------|-------------|---------|
| `AZURE_CONTAINER_REGISTRY` | Login server de l'ACR | `leavemanagementacr123.azurecr.io` |
| `STAGING_APP_URL` | URL complète de la Container App staging | `https://leavemanagement-api-staging.xxx.azurecontainerapps.io` |

### 5. Configurer les environnements GitHub

Créez deux environnements dans GitHub (**Settings > Environments**) :

1. **staging** - Déploiement automatique
2. **production** - Avec protection (reviewers requis recommandé)

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
1. Gate de qualité CI (réutilise `ci.yml`)
2. Build et push de l'image Docker vers ACR (tags `staging` + SHA)
3. Attente de la propagation du déploiement continu (~60s)
4. Smoke test sur staging (`/health`)
5. Promotion vers production : re-tag `staging` → `production`

> Le workflow CD **n'utilise aucune commande `az`**.
> Il communique uniquement avec l'ACR via `docker/login-action` et `docker/build-push-action`.

## Commandes Azure Utiles

```bash
# Voir les logs de l'application
az containerapp logs show \
  --name leavemanagement-api-staging \
  --resource-group rg-leavemanagement \
  --follow

# Redémarrer l'application
az containerapp revision restart \
  --name leavemanagement-api-staging \
  --resource-group rg-leavemanagement

# Voir l'état de l'application
az containerapp show \
  --name leavemanagement-api-staging \
  --resource-group rg-leavemanagement \
  --query properties.latestRevisionFqdn

# Mise à jour manuelle de l'image (si continuous deployment non activé)
az containerapp update \
  --name leavemanagement-api-staging \
  --resource-group rg-leavemanagement \
  --image <acr-login-server>/leavemanagement-api:staging
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
