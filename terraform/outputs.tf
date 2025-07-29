# Output values for important resources

output "resource_group_name" {
  description = "Name of the existing resource group"
  value       = data.azurerm_resource_group.existing.name
}

output "resource_group_location" {
  description = "Location of the resource group"
  value       = data.azurerm_resource_group.existing.location
}

output "container_registry_name" {
  description = "Name of the Azure Container Registry"
  value       = azurerm_container_registry.scrapewise_acr.name
}

output "container_registry_login_server" {
  description = "Login server URL for the ACR"
  value       = azurerm_container_registry.scrapewise_acr.login_server
}

output "container_registry_admin_username" {
  description = "Admin username for ACR"
  value       = azurerm_container_registry.scrapewise_acr.admin_username
  sensitive   = true
}

output "container_registry_admin_password" {
  description = "Admin password for ACR"
  value       = azurerm_container_registry.scrapewise_acr.admin_password
  sensitive   = true
}

output "app_service_plan_name" {
  description = "Name of the App Service Plan"
  value       = azurerm_service_plan.scrapewise_plan.name
}

output "app_service_plan_tier" {
  description = "Pricing tier of the App Service Plan"
  value       = azurerm_service_plan.scrapewise_plan.sku_name
}

output "web_app_name" {
  description = "Name of the Azure Web App"
  value       = azurerm_linux_web_app.scrapewise_app.name
}

output "web_app_url" {
  description = "Default URL of the Azure Web App"
  value       = "https://${azurerm_linux_web_app.scrapewise_app.default_hostname}"
}

output "web_app_default_hostname" {
  description = "Default hostname of the Azure Web App"
  value       = azurerm_linux_web_app.scrapewise_app.default_hostname
}

output "web_app_principal_id" {
  description = "Principal ID of the Web App's managed identity"
  value       = azurerm_linux_web_app.scrapewise_app.identity[0].principal_id
}

# Deployment instructions
output "deployment_instructions" {
  description = "Next steps for deployment"
  value = <<-EOT
    
    🚀 DEPLOYMENT INSTRUCTIONS:
    
    1. Build and push your Docker image:
       docker build -t ${azurerm_container_registry.scrapewise_acr.login_server}/scrapewise:latest .
       docker login ${azurerm_container_registry.scrapewise_acr.login_server}
       docker push ${azurerm_container_registry.scrapewise_acr.login_server}/scrapewise:latest
    
    2. Set your database connection string in App Service:
       az webapp config appsettings set --name ${azurerm_linux_web_app.scrapewise_app.name} \
         --resource-group ${data.azurerm_resource_group.existing.name} \
         --settings DATABASE_CONNECTION_STRING="your_connection_string"
    
    3. Access your app at: https://${azurerm_linux_web_app.scrapewise_app.default_hostname}
    
    4. Monitor logs: 
       az webapp log tail --name ${azurerm_linux_web_app.scrapewise_app.name} \
         --resource-group ${data.azurerm_resource_group.existing.name}
  EOT
}
