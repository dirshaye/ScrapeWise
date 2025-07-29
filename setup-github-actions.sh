#!/bin/bash

# Setup script for GitHub Actions Azure deployment
# This script creates a service principal for GitHub Actions to deploy to Azure

set -e

echo "🚀 Setting up Azure credentials for GitHub Actions..."

# Check if Azure CLI is installed and user is logged in
if ! az account show &> /dev/null; then
    echo "❌ Please login to Azure CLI first: az login"
    exit 1
fi

# Get current subscription info
SUBSCRIPTION_ID=$(az account show --query id --output tsv)
SUBSCRIPTION_NAME=$(az account show --query name --output tsv)

echo "📋 Current subscription: $SUBSCRIPTION_NAME ($SUBSCRIPTION_ID)"

# Variables
RESOURCE_GROUP="scrapewise_resource"
SP_NAME="scrapewise-github-actions"
ROLE="Contributor"

echo "🔐 Creating service principal for GitHub Actions..."

# Create service principal and capture the output
SP_OUTPUT=$(az ad sp create-for-rbac \
    --name "$SP_NAME" \
    --role "$ROLE" \
    --scopes "/subscriptions/$SUBSCRIPTION_ID/resourceGroups/$RESOURCE_GROUP" \
    --json-auth)

echo ""
echo "✅ Service principal created successfully!"
echo ""
echo "🔑 Add the following secret to your GitHub repository:"
echo "   Secret name: AZURE_CREDENTIALS"
echo "   Secret value:"
echo ""
echo "$SP_OUTPUT"
echo ""
echo "📝 Steps to add this secret to GitHub:"
echo "   1. Go to your GitHub repository"
echo "   2. Click Settings > Secrets and variables > Actions"
echo "   3. Click 'New repository secret'"
echo "   4. Name: AZURE_CREDENTIALS"
echo "   5. Value: Copy the JSON output above"
echo "   6. Click 'Add secret'"
echo ""
echo "🎯 After adding the secret, your GitHub Actions will be able to deploy to Azure automatically!"
echo ""
echo "⚠️  Important: Keep this credential secure and don't share it!"
