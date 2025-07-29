# Variables for ScrapeWise Azure deployment

variable "project_name" {
  description = "Name of the project"
  type        = string
  default     = "scrapewise"
}

variable "environment" {
  description = "Environment name (dev, staging, prod)"
  type        = string
  default     = "prod"
}

variable "resource_group_name" {
  description = "Name of the existing Azure resource group"
  type        = string
  default     = "scrapewise_resource"
}

variable "location" {
  description = "Azure region for resources (should match existing RG)"
  type        = string
  default     = "UK South"
}

variable "app_service_sku" {
  description = "SKU for the App Service Plan"
  type        = string
  default     = "F1"
  
  validation {
    condition     = contains(["F1", "B1", "B2", "B3", "S1", "S2", "S3", "P1v2", "P2v2", "P3v2"], var.app_service_sku)
    error_message = "App Service SKU must be one of: F1, B1, B2, B3, S1, S2, S3, P1v2, P2v2, P3v2."
  }
}

variable "acr_sku" {
  description = "SKU for Azure Container Registry"
  type        = string
  default     = "Basic"
  
  validation {
    condition     = contains(["Basic", "Standard", "Premium"], var.acr_sku)
    error_message = "ACR SKU must be one of: Basic, Standard, Premium."
  }
}

variable "docker_image_name" {
  description = "Name of the Docker image"
  type        = string
  default     = "scrapewise"
}

variable "docker_image_tag" {
  description = "Tag of the Docker image"
  type        = string
  default     = "latest"
}

variable "enable_logging" {
  description = "Enable detailed logging for the web app"
  type        = bool
  default     = true
}

variable "log_retention_days" {
  description = "Number of days to retain logs"
  type        = number
  default     = 7
  
  validation {
    condition     = var.log_retention_days >= 1 && var.log_retention_days <= 30
    error_message = "Log retention days must be between 1 and 30."
  }
}

variable "custom_domain" {
  description = "Custom domain name for the web app (optional)"
  type        = string
  default     = ""
}

variable "tags" {
  description = "A map of tags to assign to the resources"
  type        = map(string)
  default = {
    Environment = "Production"
    Project     = "ScrapeWise"
    ManagedBy   = "Terraform"
  }
}
