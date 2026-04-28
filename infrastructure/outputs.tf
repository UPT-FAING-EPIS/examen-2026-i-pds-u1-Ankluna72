# =====================================================================
# Terraform Outputs
# =====================================================================

output "backend_url" {
  description = "URL of the backend API App Service"
  value       = "https://${azurerm_linux_web_app.backend.default_hostname}"
}

output "frontend_url" {
  description = "URL of the frontend Static Web App"
  value       = "https://${azurerm_static_web_app.frontend.default_host_name}"
}

output "acr_login_server" {
  description = "Container Registry login server"
  value       = azurerm_container_registry.acr.login_server
}

output "postgres_fqdn" {
  description = "PostgreSQL server FQDN"
  value       = azurerm_postgresql_flexible_server.main.fqdn
}

output "resource_group_name" {
  description = "Resource group name"
  value       = azurerm_resource_group.main.name
}
