# Terraform configuration for deploying ScrapeWise to Azure App Service (Linux)
# Uses existing resource group and deploys containerized ASP.NET Core app

terraform {
  required_version = ">= 1.0"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }
  }
}

# Configure the Microsoft Azure Provider
provider "azurerm" {
  features {}
}

# Data source to reference existing resource group
data "azurerm_resource_group" "existing" {
  name = "scrapewise_resource"
}

# Azure Container Registry (Basic tier)
resource "azurerm_container_registry" "scrapewise_acr" {
  name                = "scrapewiseacr${random_string.suffix.result}"
  resource_group_name = data.azurerm_resource_group.existing.name
  location            = data.azurerm_resource_group.existing.location
  sku                 = "Basic"
  admin_enabled       = true

  tags = {
    Environment = "Production"
    Project     = "ScrapeWise"
    ManagedBy   = "Terraform"
  }
}

# Random string for unique naming
resource "random_string" "suffix" {
  length  = 8
  special = false
  upper   = false
}

# App Service Plan (Linux, F1 Free tier)
resource "azurerm_service_plan" "scrapewise_plan" {
  name                = "scrapewise-app-service-plan"
  resource_group_name = data.azurerm_resource_group.existing.name
  location            = data.azurerm_resource_group.existing.location
  os_type             = "Linux"
  sku_name            = "F1"

  tags = {
    Environment = "Production"
    Project     = "ScrapeWise"
    ManagedBy   = "Terraform"
  }
}

# Azure Web App (Linux Container)
resource "azurerm_linux_web_app" "scrapewise_app" {
  name                = "scrapewise-app-${random_string.suffix.result}"
  resource_group_name = data.azurerm_resource_group.existing.name
  location            = data.azurerm_resource_group.existing.location
  service_plan_id     = azurerm_service_plan.scrapewise_plan.id

  site_config {
    always_on = false # Required for F1 tier
    
    application_stack {
      docker_image     = "${azurerm_container_registry.scrapewise_acr.login_server}/scrapewise"
      docker_image_tag = "latest"
    }

    # Configure container port
    app_command_line = ""
  }

  # App settings for container configuration
  app_settings = {
    "DOCKER_REGISTRY_SERVER_URL"      = "https://${azurerm_container_registry.scrapewise_acr.login_server}"
    "DOCKER_REGISTRY_SERVER_USERNAME" = azurerm_container_registry.scrapewise_acr.admin_username
    "DOCKER_REGISTRY_SERVER_PASSWORD" = azurerm_container_registry.scrapewise_acr.admin_password
    "WEBSITES_ENABLE_APP_SERVICE_STORAGE" = "false"
    "WEBSITES_PORT" = "80"
    
    # Environment variables for your app
    "ASPNETCORE_ENVIRONMENT" = "Production"
    "ASPNETCORE_URLS" = "http://+:80"
    
    # Database connection (set this after deployment)
    # "DATABASE_CONNECTION_STRING" = "your_azure_database_connection_string"
  }

  # Configure container logs
  logs {
    detailed_error_messages = true
    failed_request_tracing  = true
    
    http_logs {
      file_system {
        retention_in_days = 7
        retention_in_mb   = 35
      }
    }
  }

  # Identity for accessing ACR (if needed)
  identity {
    type = "SystemAssigned"
  }

  tags = {
    Environment = "Production"
    Project     = "ScrapeWise"
    ManagedBy   = "Terraform"
  }

  depends_on = [azurerm_container_registry.scrapewise_acr]
}

# Role assignment to allow App Service to pull from ACR
resource "azurerm_role_assignment" "acr_pull" {
  scope                = azurerm_container_registry.scrapewise_acr.id
  role_definition_name = "AcrPull"
  principal_id         = azurerm_linux_web_app.scrapewise_app.identity[0].principal_id

  depends_on = [azurerm_linux_web_app.scrapewise_app]
}
