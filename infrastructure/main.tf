# =====================================================================
# Terraform — Gym Management System Infrastructure
# Provider: Azure
# =====================================================================

terraform {
  required_version = ">= 1.7.0"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.100"
    }
    random = {
      source  = "hashicorp/random"
      version = "~> 3.6"
    }
  }

  backend "azurerm" {
    resource_group_name  = "tf-state-rg"
    storage_account_name = "gymtfstate"
    container_name       = "tfstate"
    key                  = "gym.tfstate"
  }
}

provider "azurerm" {
  features {}
}

# ── Random suffix for globally unique names ─────────────────────────
resource "random_string" "suffix" {
  length  = 6
  upper   = false
  special = false
}

# ── Resource Group ──────────────────────────────────────────────────
resource "azurerm_resource_group" "main" {
  name     = "${var.project_name}-${var.environment}-rg"
  location = var.location
  tags     = local.common_tags
}

# ── Container Registry ──────────────────────────────────────────────
resource "azurerm_container_registry" "acr" {
  name                = "${var.project_name}${random_string.suffix.result}acr"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  sku                 = "Basic"
  admin_enabled       = true
  tags                = local.common_tags
}

# ── App Service Plan ────────────────────────────────────────────────
resource "azurerm_service_plan" "main" {
  name                = "${var.project_name}-${var.environment}-asp"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  os_type             = "Linux"
  sku_name            = "B1"
  tags                = local.common_tags
}

# ── PostgreSQL Flexible Server ──────────────────────────────────────
resource "azurerm_postgresql_flexible_server" "main" {
  name                   = "${var.project_name}-${var.environment}-pg-${random_string.suffix.result}"
  resource_group_name    = azurerm_resource_group.main.name
  location               = azurerm_resource_group.main.location
  version                = "15"
  administrator_login    = var.db_admin_user
  administrator_password = var.db_admin_password
  storage_mb             = 32768
  sku_name               = "B_Standard_B1ms"
  backup_retention_days  = 7

  tags = local.common_tags
}

resource "azurerm_postgresql_flexible_server_database" "gymdb" {
  name      = "gymdb"
  server_id = azurerm_postgresql_flexible_server.main.id
  collation = "en_US.utf8"
  charset   = "utf8"
}

resource "azurerm_postgresql_flexible_server_firewall_rule" "allow_azure" {
  name             = "allow-azure-services"
  server_id        = azurerm_postgresql_flexible_server.main.id
  start_ip_address = "0.0.0.0"
  end_ip_address   = "0.0.0.0"
}

# ── Backend App Service ─────────────────────────────────────────────
resource "azurerm_linux_web_app" "backend" {
  name                = "${var.project_name}-${var.environment}-api-${random_string.suffix.result}"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  service_plan_id     = azurerm_service_plan.main.id

  site_config {
    application_stack {
      docker_image_name        = "${azurerm_container_registry.acr.login_server}/gym-api:latest"
      docker_registry_url      = "https://${azurerm_container_registry.acr.login_server}"
      docker_registry_username = azurerm_container_registry.acr.admin_username
      docker_registry_password = azurerm_container_registry.acr.admin_password
    }
    always_on = true
    cors {
      allowed_origins = ["*"]
    }
  }

  app_settings = {
    "ASPNETCORE_ENVIRONMENT"                    = var.environment == "prod" ? "Production" : "Development"
    "ConnectionStrings__DefaultConnection"      = "Host=${azurerm_postgresql_flexible_server.main.fqdn};Database=gymdb;Username=${var.db_admin_user};Password=${var.db_admin_password};SslMode=Require"
    "WEBSITES_ENABLE_APP_SERVICE_STORAGE"       = "false"
  }

  tags = local.common_tags
}

# ── Frontend Static Web App ─────────────────────────────────────────
resource "azurerm_static_web_app" "frontend" {
  name                = "${var.project_name}-${var.environment}-spa"
  resource_group_name = azurerm_resource_group.main.name
  location            = "eastus2"
  sku_tier            = "Free"
  sku_size            = "Free"
  tags                = local.common_tags
}

# ── Local Values ────────────────────────────────────────────────────
locals {
  common_tags = {
    Project     = var.project_name
    Environment = var.environment
    ManagedBy   = "Terraform"
    Owner       = "Ankluna72"
  }
}
