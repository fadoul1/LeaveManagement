#!/bin/bash
set -e

# Configuration
RESOURCE_GROUP="rg-leavemanagement"
LOCATION="westeurope"
ENVIRONMENT=${1:-staging}

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
DEPLOYMENT_OUTPUT=$(az deployment group create \
  --resource-group $RESOURCE_GROUP \
  --template-file main.bicep \
  --parameters environment=$ENVIRONMENT \
  --parameters containerImage="mcr.microsoft.com/azuredocs/containerapps-helloworld:latest" \
  --parameters postgresPassword="$POSTGRES_PASSWORD" \
  --query properties.outputs \
  --output json)

# Extract outputs
ACR_NAME=$(echo $DEPLOYMENT_OUTPUT | jq -r '.acrName.value')
ACR_LOGIN_SERVER=$(echo $DEPLOYMENT_OUTPUT | jq -r '.acrLoginServer.value')
CONTAINER_APP_NAME=$(echo $DEPLOYMENT_OUTPUT | jq -r '.containerAppName.value')
CONTAINER_APP_URL=$(echo $DEPLOYMENT_OUTPUT | jq -r '.containerAppUrl.value')

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
ACR_CREDS=$(az acr credential show --name $ACR_NAME --output json)
ACR_USERNAME=$(echo $ACR_CREDS | jq -r '.username')
ACR_PASSWORD=$(echo $ACR_CREDS | jq -r '.passwords[0].value')

# Create service principal for GitHub Actions
echo "Creating service principal for GitHub Actions..."
SP_OUTPUT=$(az ad sp create-for-rbac \
  --name "sp-leavemanagement-github" \
  --role contributor \
  --scopes /subscriptions/$(az account show --query id -o tsv)/resourceGroups/$RESOURCE_GROUP \
  --sdk-auth)

echo ""
echo "Add these secrets to your GitHub repository:"
echo ""
echo "1. ACR_USERNAME: $ACR_USERNAME"
echo "2. ACR_PASSWORD: $ACR_PASSWORD"
echo "3. AZURE_CREDENTIALS:"
echo "$SP_OUTPUT"
echo ""
echo "Add these variables to your GitHub repository:"
echo ""
echo "1. AZURE_CONTAINER_REGISTRY: $ACR_LOGIN_SERVER"
echo "2. CONTAINER_APP_NAME: $CONTAINER_APP_NAME"
echo "3. RESOURCE_GROUP: $RESOURCE_GROUP"
echo ""
echo "=== PostgreSQL Password (save securely!) ==="
echo "Password: $POSTGRES_PASSWORD"
