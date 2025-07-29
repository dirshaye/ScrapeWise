# Terraform state configuration
# Uncomment and configure for remote state storage

# terraform {
#   backend "azurerm" {
#     resource_group_name  = "scrapewise_resource"
#     storage_account_name = "scrapewiseterraformstate"
#     container_name       = "tfstate"
#     key                  = "scrapewise.terraform.tfstate"
#   }
# }

# Alternative: Use local state (default)
# The state file will be stored locally as terraform.tfstate
# For production, consider using remote state storage
