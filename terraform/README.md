# ScrapeWise Azure Deployment with Terraform

This directory contains Terraform configuration files to deploy the ScrapeWise ASP.NET Core application to Azure App Service using Docker containers.

## Architecture

- **Azure Container Registry (ACR)**: Stores your Docker images (Basic tier)
- **Azure App Service Plan**: Linux-based hosting plan (F1 Free tier)
- **Azure Web App**: Containerized web application with managed identity
- **Role Assignment**: Allows App Service to pull images from ACR

## Prerequisites

1. **Azure CLI**: Install and login to Azure
   ```bash
   az login
   ```

2. **Terraform**: Install Terraform (>= 1.0)
   ```bash
   # On Ubuntu/Debian
   wget -O- https://apt.releases.hashicorp.com/gpg | sudo gpg --dearmor -o /usr/share/keyrings/hashicorp-archive-keyring.gpg
   echo "deb [signed-by=/usr/share/keyrings/hashicorp-archive-keyring.gpg] https://apt.releases.hashicorp.com $(lsb_release -cs) main" | sudo tee /etc/apt/sources.list.d/hashicorp.list
   sudo apt update && sudo apt install terraform
   ```

3. **Docker**: Ensure Docker is installed for building images
   ```bash
   sudo apt install docker.io
   sudo usermod -aG docker $USER
   ```

4. **Existing Resource Group**: The resource group `scrapewise_resource` must exist in `West Europe`

## Deployment Steps

### 1. Initialize Terraform
```bash
cd terraform
terraform init
```

### 2. Plan the Deployment
```bash
terraform plan
```

### 3. Apply the Configuration
```bash
terraform apply
```

### 4. Build and Push Docker Image
After Terraform completes, get the ACR login server from the output:
```bash
# Get ACR details
ACR_NAME=$(terraform output -raw container_registry_name)
ACR_SERVER=$(terraform output -raw container_registry_login_server)

# Login to ACR
az acr login --name $ACR_NAME

# Build and push your image
cd ..  # Go back to project root
docker build -t $ACR_SERVER/scrapewise:latest .
docker push $ACR_SERVER/scrapewise:latest
```

### 5. Configure Environment Variables
Set your database connection string and other environment variables:
```bash
WEB_APP_NAME=$(terraform output -raw web_app_name)
RESOURCE_GROUP=$(terraform output -raw resource_group_name)

az webapp config appsettings set \
  --name $WEB_APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --settings DATABASE_CONNECTION_STRING="your_azure_database_connection_string"
```

### 6. Access Your Application
```bash
APP_URL=$(terraform output -raw web_app_url)
echo "Your app is available at: $APP_URL"
```

## Configuration Files

- **`main.tf`**: Core infrastructure resources
- **`variables.tf`**: Input variables and their defaults
- **`outputs.tf`**: Output values after deployment
- **`backend.tf`**: Terraform state configuration

## Key Features

- **Free Tier**: Uses F1 App Service Plan (free)
- **Container Registry**: Basic tier ACR for image storage
- **Managed Identity**: Secure access between App Service and ACR
- **Auto-scaling**: Disabled for F1 tier (always_on = false)
- **Logging**: Configured for troubleshooting
- **Security**: Proper role assignments and secure image pulling

## Environment Variables

The following environment variables are automatically configured:
- `ASPNETCORE_ENVIRONMENT=Production`
- `ASPNETCORE_URLS=http://+:80`
- `WEBSITES_PORT=80`
- `DOCKER_REGISTRY_SERVER_*` (for ACR access)

You need to manually set:
- `DATABASE_CONNECTION_STRING`: Your PostgreSQL or Azure SQL connection string

## Monitoring and Troubleshooting

### View Application Logs
```bash
az webapp log tail --name $WEB_APP_NAME --resource-group $RESOURCE_GROUP
```

### Check Container Logs
```bash
az webapp log download --name $WEB_APP_NAME --resource-group $RESOURCE_GROUP
```

### Restart the Application
```bash
az webapp restart --name $WEB_APP_NAME --resource-group $RESOURCE_GROUP
```

## Scaling (Upgrade from F1)

To upgrade to a paid tier for better performance:
```bash
# Update the SKU in variables.tf or override
terraform apply -var="app_service_sku=B1"
```

## Cleanup

To destroy all resources (except the existing resource group):
```bash
terraform destroy
```

## Cost Estimation

- **F1 App Service Plan**: Free (limited to 60 minutes/day)
- **Basic ACR**: ~$5/month
- **Storage**: Minimal costs for logs and images

## Security Best Practices Applied

- ✅ Managed identity for secure ACR access
- ✅ No hardcoded credentials
- ✅ Environment variables for sensitive data
- ✅ HTTPS enforced
- ✅ Container isolation

## Next Steps

1. Set up Azure Database for PostgreSQL
2. Configure custom domain (optional)
3. Set up monitoring and alerts
4. Configure CI/CD pipeline
5. Consider upgrading to paid tier for production workloads
