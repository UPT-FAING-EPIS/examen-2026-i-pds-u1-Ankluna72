# 🚀 Guía de Despliegue GRATUITO

## Resumen de costos: **$0.00 — Todo gratis**

| Servicio | Uso | Costo |
|---|---|---|
| **GitHub Pages** | Frontend React | ✅ Gratis |
| **Render.com** | Backend .NET API | ✅ Gratis (750 hrs/mes) |
| **Render.com PostgreSQL** | Base de datos | ✅ Gratis (1 GB) |

---

## PASO 1 — Habilitar GitHub Pages (2 minutos)

> El frontend quedará en: `https://upt-faing-epis.github.io/examen-2026-i-pds-u1-Ankluna72/`

1. Ve a tu repositorio: `https://github.com/UPT-FAING-EPIS/examen-2026-i-pds-u1-Ankluna72`
2. Haz clic en **Settings** (pestaña)
3. En el menú lateral, haz clic en **Pages**
4. En **"Source"**, selecciona: **GitHub Actions**
5. Guarda. ¡Listo!

El workflow `deploy_app.yml` desplegará automáticamente el frontend cada vez que hagas push a `main`.

---

## PASO 2 — Crear cuenta en Render.com (5 minutos)

> El backend quedará en: `https://gym-management-api.onrender.com`

### 2.1 Registrarse (sin tarjeta de crédito)

1. Ve a **https://render.com**
2. Haz clic en **"Get Started for Free"**
3. Regístrate con tu **cuenta de GitHub** (más fácil)
4. Autoriza el acceso a GitHub

### 2.2 Crear el servicio desde el Blueprint

1. En el dashboard de Render, haz clic en **"New +"** → **"Blueprint"**
2. Conecta tu repositorio: `examen-2026-i-pds-u1-Ankluna72`
3. Render detectará automáticamente el archivo `render.yaml`
4. Haz clic en **"Apply"**
5. Render creará automáticamente:
   - ✅ Web Service: `gym-management-api` (.NET API)
   - ✅ PostgreSQL: `gym-postgres` (base de datos)
6. Espera ~5 minutos a que termine el deploy

### 2.3 Obtener la URL del backend

1. Ve a tu servicio `gym-management-api` en el dashboard
2. Copia la URL que aparece arriba, ej: `https://gym-management-api.onrender.com`

---

## PASO 3 — Conectar Frontend con Backend (2 minutos)

Necesitas decirle a GitHub Pages dónde está tu backend de Render.com.

### 3.1 Agregar Variable de Repositorio en GitHub

1. Ve a tu repositorio en GitHub
2. **Settings** → **Secrets and variables** → **Actions**
3. Haz clic en la pestaña **"Variables"** (no Secrets)
4. Haz clic en **"New repository variable"**
5. Nombre: `RENDER_BACKEND_URL`
6. Valor: `https://gym-management-api.onrender.com` ← (la URL que copiaste)
7. Clic en **"Add variable"**

### 3.2 Obtener el Deploy Hook de Render (opcional, para auto-deploy)

1. En Render.com, ve a tu servicio `gym-management-api`
2. Haz clic en **Settings**
3. Busca **"Deploy Hook"** y copia la URL
4. En GitHub → **Settings** → **Secrets and variables** → **Actions** → **Secrets**
5. Nombre: `RENDER_DEPLOY_HOOK_URL`
6. Valor: la URL del deploy hook

> ⚠️ Este secret es **opcional**. Sin él, el backend se despliega automáticamente desde Render.com cuando haces push, gracias al `render.yaml`.

---

## PASO 4 — Re-ejecutar el Workflow (1 minuto)

1. Ve a tu repositorio en GitHub
2. Haz clic en la pestaña **Actions**
3. Selecciona el workflow **"Deploy Application"**
4. Haz clic en **"Run workflow"** → **"Run workflow"**
5. Espera ~3 minutos

---

## ✅ URLs Finales

Una vez completados los pasos:

| Componente | URL |
|---|---|
| 🌐 **Frontend (React)** | `https://upt-faing-epis.github.io/examen-2026-i-pds-u1-Ankluna72/` |
| ⚙️ **Backend API** | `https://gym-management-api.onrender.com` |
| 📖 **Swagger UI** | `https://gym-management-api.onrender.com/swagger` |
| ❤️ **Health Check** | `https://gym-management-api.onrender.com/health` |
| 📚 **Documentación** | `https://upt-faing-epis.github.io/examen-2026-i-pds-u1-Ankluna72/` |

---

## ⚠️ Notas Importantes

### Render.com Free Tier
- El servicio **se "duerme"** después de 15 minutos sin tráfico
- La primera petición después del sueño tarda ~30 segundos en responder
- Esto es normal en el plan gratuito

### GitHub Pages
- El deploy tarda ~2-3 minutos después de cada push
- Puedes ver el progreso en la pestaña **Actions** del repositorio

### Resumen de Secrets/Variables necesarios

| Nombre | Tipo | Dónde obtenerlo | ¿Obligatorio? |
|---|---|---|---|
| `RENDER_BACKEND_URL` | Variable (no secret) | Dashboard Render.com → URL del servicio | ✅ Sí |
| `RENDER_DEPLOY_HOOK_URL` | Secret | Dashboard Render.com → Settings → Deploy Hook | ❌ Opcional |

**Total: 1 variable + 1 secret opcional. ¡Nada más!**
