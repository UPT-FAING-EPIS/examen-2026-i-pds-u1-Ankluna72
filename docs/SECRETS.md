# GitHub Secrets Configuration Guide

Para que los workflows de GitHub Actions funcionen correctamente, debes configurar los siguientes **Secrets** en tu repositorio GitHub.

## Cómo configurar Secrets

1. Ve a tu repositorio en GitHub
2. Haz clic en **Settings** → **Secrets and variables** → **Actions**
3. Haz clic en **New repository secret**

---

## Secrets Requeridos

### Azure Authentication

| Secret | Descripción | Cómo obtenerlo |
|---|---|---|
| `AZURE_CLIENT_ID` | ID de la aplicación de Azure AD | `az ad sp show --id <service-principal-name> --query appId` |
| `AZURE_CLIENT_SECRET` | Contraseña del Service Principal | Al crear el SP: `az ad sp create-for-rbac` |
| `AZURE_SUBSCRIPTION_ID` | ID de la suscripción Azure | `az account show --query id` |
| `AZURE_TENANT_ID` | ID del tenant Azure | `az account show --query tenantId` |
| `AZURE_CREDENTIALS` | JSON completo de credenciales | `az ad sp create-for-rbac --sdk-auth` |

### Database

| Secret | Descripción |
|---|---|
| `DB_ADMIN_PASSWORD` | Contraseña del administrador de PostgreSQL |

### Container Registry

| Secret | Descripción |
|---|---|
| `ACR_LOGIN_SERVER` | Login server del ACR (ej: gymmanagementXXXacr.azurecr.io) |
| `ACR_USERNAME` | Username del ACR |
| `ACR_PASSWORD` | Password del ACR |

### App Service

| Secret | Descripción |
|---|---|
| `AZURE_WEBAPP_NAME` | Nombre del App Service del backend |
| `AZURE_STATIC_WEB_APP_NAME` | Nombre del Static Web App |
| `AZURE_STATIC_WEB_APPS_API_TOKEN` | Token del Static Web App (desde Azure Portal) |

---

## Crear el Service Principal para Azure

```bash
# Login a Azure
az login

# Crear Service Principal con permisos de Contributor
az ad sp create-for-rbac \
  --name "gym-management-github-actions" \
  --role Contributor \
  --scopes /subscriptions/<SUBSCRIPTION_ID> \
  --sdk-auth
```

El output JSON es el valor de `AZURE_CREDENTIALS`.

---

## Configurar GitHub Pages

1. Ve a **Settings** → **Pages**
2. En **Source**, selecciona **GitHub Actions**
3. El workflow `publish_doc.yml` se encargará del despliegue

---

## Variables de Entorno para el Workflow

Puedes también crear **Environments** en GitHub:
- `dev` — Desarrollo
- `staging` — Pruebas
- `prod` — Producción

Cada environment puede tener sus propios secrets.
