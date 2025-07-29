#!/bin/bash

# ScrapeWise Azure Deployment Script
# This script automates the deployment of ScrapeWise to Azure using Terraform

set -e  # Exit on any error

echo "🚀 Starting ScrapeWise Azure Deployment"
echo "======================================"

# Check prerequisites
echo "📋 Checking prerequisites..."

# Check if Azure CLI is installed and logged in
if ! command -v az &> /dev/null; then
    echo "❌ Azure CLI is not installed. Please install it first."
    exit 1
fi

if ! az account show &> /dev/null; then
    echo "❌ Not logged in to Azure. Please run 'az login' first."
    exit 1
fi

# Check if Terraform is installed
if ! command -v terraform &> /dev/null; then
    echo "❌ Terraform is not installed. Please install it first."
    exit 1
fi

# Check if Docker is installed
if ! command -v docker &> /dev/null; then
    echo "❌ Docker is not installed. Please install it first."
    exit 1
fi

echo "✅ All prerequisites met!"

# Navigate to terraform directory
cd "$(dirname "$0")"
if [ ! -f "main.tf" ]; then
    echo "❌ main.tf not found. Make sure you're in the terraform directory."
    exit 1
fi

# Initialize Terraform
echo "🔧 Initializing Terraform..."
terraform init

# Validate configuration
echo "🔍 Validating Terraform configuration..."
terraform validate

# Plan deployment
echo "📋 Planning deployment..."
terraform plan -out=tfplan

# Ask for confirmation
read -p "🤔 Do you want to proceed with the deployment? (y/N): " -n 1 -r
echo
if [[ ! $REPLY =~ ^[Yy]$ ]]; then
    echo "❌ Deployment cancelled."
    exit 1
fi

# Apply the plan
echo "🚀 Deploying infrastructure..."
terraform apply tfplan

# Get outputs
echo "📊 Getting deployment outputs..."
ACR_NAME=$(terraform output -raw container_registry_name)
ACR_SERVER=$(terraform output -raw container_registry_login_server)
WEB_APP_NAME=$(terraform output -raw web_app_name)
RESOURCE_GROUP=$(terraform output -raw resource_group_name)
APP_URL=$(terraform output -raw web_app_url)

echo "✅ Infrastructure deployed successfully!"
echo ""
echo "📋 Deployment Summary:"
echo "====================="
echo "🏪 Resource Group: $RESOURCE_GROUP"
echo "📦 Container Registry: $ACR_NAME"
echo "🌐 Web App: $WEB_APP_NAME"
echo "🔗 App URL: $APP_URL"
echo ""

# Build and push Docker image
echo "🐳 Building and pushing Docker image..."

# Go back to project root
cd ..

# Check if Dockerfile exists
if [ ! -f "Dockerfile" ]; then
    echo "❌ Dockerfile not found in project root."
    exit 1
fi

# Login to ACR
echo "🔐 Logging in to Azure Container Registry..."
az acr login --name $ACR_NAME

# Build image
echo "🔨 Building Docker image..."
docker build -t $ACR_SERVER/scrapewise:latest .

# Push image
echo "📤 Pushing image to ACR..."
docker push $ACR_SERVER/scrapewise:latest

echo "✅ Docker image pushed successfully!"

# Wait for app to start
echo "⏳ Waiting for app to start..."
sleep 30

# Set environment variables
echo "⚙️  Setting environment variables..."
read -p "🔗 Enter your database connection string (or press Enter to skip): " DB_CONNECTION

if [ ! -z "$DB_CONNECTION" ]; then
    az webapp config appsettings set \
        --name $WEB_APP_NAME \
        --resource-group $RESOURCE_GROUP \
        --settings DATABASE_CONNECTION_STRING="$DB_CONNECTION" \
        --output none
    echo "✅ Database connection string set!"
fi

# Final success message
echo ""
echo "🎉 DEPLOYMENT COMPLETED SUCCESSFULLY!"
echo "====================================="
echo ""
echo "🌐 Your app is available at: $APP_URL"
echo "📋 Resource Group: $RESOURCE_GROUP"
echo "📦 Container Registry: $ACR_SERVER"
echo ""
echo "📝 Next steps:"
echo "   1. Visit your app: $APP_URL"
echo "   2. Check logs: az webapp log tail --name $WEB_APP_NAME --resource-group $RESOURCE_GROUP"
echo "   3. Monitor performance in Azure Portal"
echo ""
echo "🔧 To update your app:"
echo "   1. Make your changes"
echo "   2. docker build -t $ACR_SERVER/scrapewise:latest ."
echo "   3. docker push $ACR_SERVER/scrapewise:latest"
echo "   4. az webapp restart --name $WEB_APP_NAME --resource-group $RESOURCE_GROUP"
echo ""
echo "Happy coding! 🚀"
