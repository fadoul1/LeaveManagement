#!/bin/bash
set -e

# Configuration
RESOURCE_GROUP="rg-leavemanagement"
LOCATION="francecentral"
ENVIRONMENT=${1:-staging}
IMAGE_NAME="leavemanagement-api"

echo "=== Deploying LeaveManagement Infrastructure ==="
echo "Environment: $ENVIRONMENT"
echo "Resource Group: $RESOURCE_GROUP"
echo "Location: $LOCATION"

# Generate a secure password for PostgreSQL
POSTGRES_PASSWORD=$(openssl rand -base64 24)

# Create Resource Group if it doesn't exist
echo "Creating resource group..."
az group create \
  --name $RESOURCE_GROUP \
  --location $LOCATION \
  --output none

# Deploy infrastructure using Bicep
echo "Deploying infrastructure..."
az deployment group create \
  --resource-group $RESOURCE_GROUP \
  --template-file main.bicep \
  --parameters environment=$ENVIRONMENT \
  --parameters containerImage="mcr.microsoft.com/dotnet/samples:aspnetapp" \
  --parameters postgresPassword="$POSTGRES_PASSWORD" \
  --output none

# Extract outputs directly via Azure CLI queries
ACR_NAME=$(az deployment group show --resource-group $RESOURCE_GROUP --name main --query properties.outputs.acrName.value -o tsv)
ACR_LOGIN_SERVER=$(az deployment group show --resource-group $RESOURCE_GROUP --name main --query properties.outputs.acrLoginServer.value -o tsv)
CONTAINER_APP_NAME=$(az deployment group show --resource-group $RESOURCE_GROUP --name main --query properties.outputs.containerAppName.value -o tsv)
CONTAINER_APP_URL=$(az deployment group show --resource-group $RESOURCE_GROUP --name main --query properties.outputs.containerAppUrl.value -o tsv)

echo ""
echo "=== Deployment Complete ==="
echo ""
echo "ACR Name: $ACR_NAME"
echo "ACR Login Server: $ACR_LOGIN_SERVER"
echo "Container App Name: $CONTAINER_APP_NAME"
echo "Container App URL: $CONTAINER_APP_URL"
echo ""
echo "=== GitHub Secrets to Configure ==="
echo ""

# Get ACR credentials
ACR_USERNAME=$(az acr credential show --name $ACR_NAME --query username -o tsv)
ACR_PASSWORD=$(az acr credential show --name $ACR_NAME --query "passwords[0].value" -o tsv)

# Prepare Container App for tagged image deployments
echo ""
echo "Configuring Container App for tagged deployments..."

# Import a placeholder image into ACR with the environment tag (no Docker required)
echo "Importing placeholder image into ACR..."
az acr import \
  --name $ACR_NAME \
  --source mcr.microsoft.com/dotnet/samples:aspnetapp \
  --image "${IMAGE_NAME}:${ENVIRONMENT}" \
  --force \
  --output none

# Update Container App to reference the tagged image from ACR
az containerapp update \
  --name "$CONTAINER_APP_NAME" \
  --resource-group $RESOURCE_GROUP \
  --image "${ACR_LOGIN_SERVER}/${IMAGE_NAME}:${ENVIRONMENT}" \
  --output none

echo ""
echo "=== Deployment Complete ==="
echo ""
echo "ACR Name:          $ACR_NAME"
echo "ACR Login Server:  $ACR_LOGIN_SERVER"
echo "Container App:     $CONTAINER_APP_NAME"
echo "Container App URL: $CONTAINER_APP_URL"
echo ""
echo "=== GitHub Configuration ==="
echo ""
echo "Add these SECRETS (Settings > Secrets and variables > Actions > Secrets):"
echo ""
echo "  ACR_USERNAME:  $ACR_USERNAME"
echo "  ACR_PASSWORD:  $ACR_PASSWORD"
echo ""
echo "Add these VARIABLES (Settings > Secrets and variables > Actions > Variables):"
echo ""
echo "  AZURE_CONTAINER_REGISTRY:  $ACR_LOGIN_SERVER"
echo "  STAGING_APP_URL:           $CONTAINER_APP_URL"
echo ""
echo "=== Enable Continuous Deployment (one-time, per Container App) ==="
echo ""
echo "In Azure Portal, for each Container App:"
echo "  1. Open the Container App resource"
echo "  2. Left menu > 'Continuous deployment'"
echo "  3. Enable continuous deployment"
echo "  4. Container registry: $ACR_NAME"
echo "  5. Image: $IMAGE_NAME"
echo "  6. Tag: $ENVIRONMENT"
echo ""
echo "This creates an ACR webhook that auto-deploys on image push."
echo "No Service Principal or Entra ID App Registration required."
echo ""
echo "=== Environments GitHub (one-time) ==="
echo ""
echo "Create two environments in GitHub (Settings > Environments):"
echo "  1. staging  - automatic deployment"
echo "  2. production - with protection rules (reviewers recommended)"
echo ""
echo "=== PostgreSQL Password (save securely!) ==="
echo "Password: $POSTGRES_PASSWORD"
