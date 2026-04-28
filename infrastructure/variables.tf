# =====================================================================
# Terraform Variables
# =====================================================================

variable "project_name" {
  description = "Project name prefix for all resources"
  type        = string
  default     = "gymmanagement"
}

variable "environment" {
  description = "Deployment environment (dev, staging, prod)"
  type        = string
  default     = "dev"
  validation {
    condition     = contains(["dev", "staging", "prod"], var.environment)
    error_message = "Environment must be dev, staging, or prod."
  }
}

variable "location" {
  description = "Azure region"
  type        = string
  default     = "eastus"
}

variable "db_admin_user" {
  description = "PostgreSQL administrator login"
  type        = string
  default     = "gymadmin"
  sensitive   = true
}

variable "db_admin_password" {
  description = "PostgreSQL administrator password"
  type        = string
  sensitive   = true
}
