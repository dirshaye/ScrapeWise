# GitHub Actions CI/CD for Azure Deployment

## Overview

This repository uses GitHub Actions for automated CI/CD deployment to Azure App Service. The workflow provides:

- ✅ **Automated testing** before deployment
- 🐳 **Docker containerization** with Azure Container Registry
- 🔄 **Automatic rollback** if deployment fails
- 🧹 **Cleanup** of old container images
- 📊 **Health checks** to ensure deployment success

## Workflow Details

### Triggers
- **Push to main branch**: Automatically triggers build, test, and deploy
- **Pull requests**: Runs build and test (no deployment)

### Jobs

#### 1. Build and Test (`build-and-test`)
- Sets up .NET 9.0 environment
- Restores dependencies
- Builds the application
- Runs all tests
- Publishes the application
- Uploads build artifacts

#### 2. Deploy to Azure (`deploy-to-azure`)
- Only runs on main branch pushes after successful build/test
- Downloads build artifacts
- Logs into Azure using service principal
- Captures current deployment for potential rollback
- Builds and pushes Docker image to Azure Container Registry
- Deploys new image to Azure App Service
- Performs health checks on the deployed application
- **Automatically rolls back** if health checks fail

#### 3. Cleanup (`cleanup-old-images`)
- Removes old Docker images from ACR (keeps last 5 versions)
- Runs only after successful deployment

## Rollback Strategy

The workflow implements automatic rollback:

1. **Before deployment**: Captures the current running image
2. **After deployment**: Performs HTTP health checks for 5 minutes
3. **On failure**: Automatically reverts to the previous working image
4. **Notification**: Reports success or failure with rollback details

## Health Checks

The deployment performs comprehensive health checks:
- Tests the application URL for HTTP 200 responses
- Retries up to 10 times with 30-second intervals
- Allows up to 5 minutes for the application to start

## Azure Resources

The workflow deploys to:
- **Resource Group**: `scrapewise_resource`
- **App Service**: `scrapewise-app-x2w3nky2`
- **Container Registry**: `scrapewiseacrx2w3nky2`
- **Image**: `scrapewise:latest` (with timestamped tags)

## Setup Requirements

### 1. Azure Service Principal
A service principal with Contributor access to the resource group is required.

**To set up:**
```bash
# Run the setup script (already done)
./setup-github-actions.sh
```

### 2. GitHub Secret
Add the following secret to your GitHub repository:

**Secret Name**: `AZURE_CREDENTIALS`
**Secret Value**: JSON output from the setup script

**Steps:**
1. Go to GitHub repository → Settings → Secrets and variables → Actions
2. Click "New repository secret"
3. Name: `AZURE_CREDENTIALS`
4. Value: Copy the JSON from setup script output
5. Click "Add secret"

## Environment Variables

The workflow automatically sets these Azure App Service settings:
- `ConnectionStrings__DefaultConnection`: Database connection string
- `ASPNETCORE_ENVIRONMENT`: Production
- `ASPNETCORE_URLS`: http://+:80
- `WEBSITES_PORT`: 80

## Monitoring and Logs

To view deployment logs:
```bash
# View live application logs
az webapp log tail --name scrapewise-app-x2w3nky2 --resource-group scrapewise_resource

# View deployment history
az webapp deployment list --name scrapewise-app-x2w3nky2 --resource-group scrapewise_resource

# Check current app status
az webapp show --name scrapewise-app-x2w3nky2 --resource-group scrapewise_resource --query "state"
```

## Manual Deployment

If you need to manually trigger a deployment:
1. Go to GitHub → Actions
2. Select "Build and Deploy to Azure" workflow
3. Click "Run workflow" button
4. Choose the branch and click "Run workflow"

## Troubleshooting

### Common Issues

1. **Build Failures**: Check the build logs in GitHub Actions
2. **Test Failures**: Review test output and fix failing tests
3. **Deployment Failures**: Check Azure App Service logs
4. **Health Check Failures**: Verify application starts correctly

### Emergency Rollback

If you need to manually rollback:
```bash
# List recent deployments
az webapp deployment list --name scrapewise-app-x2w3nky2 --resource-group scrapewise_resource

# Get previous image tag
az acr repository show-tags --name scrapewiseacrx2w3nky2 --repository scrapewise --orderby time_desc

# Rollback to specific image
az webapp config container set \
  --name scrapewise-app-x2w3nky2 \
  --resource-group scrapewise_resource \
  --docker-custom-image-name scrapewiseacrx2w3nky2.azurecr.io/scrapewise:PREVIOUS_TAG
```

## Security Considerations

- Azure credentials are stored as GitHub secrets
- Service principal has minimal required permissions (Contributor on resource group only)
- Old container images are automatically cleaned up
- Database credentials are managed via Azure App Service settings

## Cost Optimization

- **ACR Cleanup**: Automatically removes old images to reduce storage costs
- **Image Tagging**: Uses timestamp + commit SHA for unique, traceable images
- **Resource Efficiency**: Reuses existing Azure resources

This setup provides enterprise-grade CI/CD with automatic rollback capabilities, ensuring your application stays available even if a deployment fails.
