[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-22041afd0340ce965d47ae6ef1cefeee28c7c493a6346c4f15d667ab976d596c.svg)](https://classroom.github.com/a/4lPbAfsF)
[![Open in Codespaces](https://classroom.github.com/assets/launch-codespace-2972f46106e565e64193e422d61a12cf1da4916b45550586e14ef0a7c637dd04.svg)](https://classroom.github.com/open-in-codespaces?assignment_repo_id=23742646)

# Alumno: Jefferson Rosas Chambilla
# 🏋️ Sistema de Gestión de Gimnasio
Una plataforma web completa para la gestión de membresías de clientes y sesiones de entrenamiento en un gimnasio.

## 📋 Tabla de Contenidos

- [Descripción](#descripción)
- [Arquitectura](#arquitectura)
- [Principios SOLID](#principios-solid)
- [Tecnologías](#tecnologías)
- [Instalación y Ejecución](#instalación-y-ejecución)
- [API Endpoints](#api-endpoints)
- [Infraestructura](#infraestructura)
- [CI/CD Pipelines](#cicd-pipelines)

## Descripción

El Sistema de Gestión de Gimnasio es una plataforma web que permite:

- ✅ Registrar y administrar clientes del gimnasio
- ✅ Gestionar planes de membresía (activa, vencida, suspendida, cancelada)
- ✅ Programar y registrar sesiones de entrenamiento
- ✅ Registrar rutinas por sesión (ejercicios, series, repeticiones, cargas)
- ✅ Visualizar historial de membresías y sesiones por cliente

## Arquitectura

```
examen-2026-i-pds-u1-Ankluna72/
├── backend/                    # API .NET Core 8
│   ├── src/
│   │   ├── GymManagement.API/          # Capa de presentación (Controllers)
│   │   ├── GymManagement.Application/  # Casos de uso, servicios
│   │   ├── GymManagement.Domain/       # Entidades, interfaces, Value Objects
│   │   └── GymManagement.Infrastructure/ # Repos, DbContext, migraciones
│   └── tests/
│       ├── GymManagement.UnitTests/
│       └── GymManagement.IntegrationTests/
├── frontend/                   # SPA React + Vite
│   ├── src/
│   │   ├── components/
│   │   ├── pages/
│   │   ├── services/
│   │   └── hooks/
├── infrastructure/             # Terraform IaC
│   ├── main.tf
│   ├── variables.tf
│   └── outputs.tf
└── .github/workflows/          # GitHub Actions CI/CD
    ├── infra.yml
    ├── infra_diagram.yml
    ├── class_diagram.yml
    ├── publish_doc.yml
    ├── deploy_app.yml
    └── release.yml
```

## Principios SOLID

### S — Single Responsibility Principle (SRP)

> *"Una clase debe tener una sola razón para cambiar."*

**Aplicación en el proyecto:**

| Clase/Componente | Responsabilidad Única |
|---|---|
| `ClientController` | Solo maneja las peticiones HTTP relacionadas con clientes |
| `ClientService` | Solo contiene la lógica de negocio de clientes |
| `ClientRepository` | Solo gestiona la persistencia de datos de clientes |
| `MembershipExpiryChecker` | Solo verifica y actualiza el estado de vencimiento |
| `WorkoutSessionService` | Solo gestiona la lógica de sesiones de entrenamiento |

**Ejemplo en código:**
```csharp
// ✅ CORRECTO - ClientService tiene una sola responsabilidad
public class ClientService : IClientService
{
    private readonly IClientRepository _repository;
    // Solo lógica de negocio de clientes
    public async Task<ClientDto> GetByIdAsync(int id) { ... }
    public async Task<ClientDto> CreateAsync(CreateClientDto dto) { ... }
}

// ❌ INCORRECTO - Violaría SRP mezclando responsabilidades
public class ClientManager
{
    public void CreateClient() { }      // negocio
    public void SendEmail() { }         // notificaciones
    public void SaveToDatabase() { }    // persistencia
}
```

---

### O — Open/Closed Principle (OCP)

> *"Las entidades de software deben estar abiertas para extensión, pero cerradas para modificación."*

**Aplicación en el proyecto:**

El sistema usa **interfaces** e **inyección de dependencias** para extender comportamiento sin modificar código existente.

| Interfaz | Propósito | Implementaciones |
|---|---|---|
| `IClientRepository` | Abstracción del repositorio | `ClientRepository` (EF Core) |
| `IMembershipRepository` | Abstracción del repo de membresías | `MembershipRepository` |
| `IWorkoutSessionRepository` | Abstracción del repo de sesiones | `WorkoutSessionRepository` |

**Ejemplo en código:**
```csharp
// Interfaz cerrada para modificación
public interface IMembershipStatusStrategy
{
    MembershipStatus GetStatus(Membership membership);
}

// Abierta para extensión — nuevas estrategias sin modificar código existente
public class ActiveStatusStrategy : IMembershipStatusStrategy { ... }
public class ExpiredStatusStrategy : IMembershipStatusStrategy { ... }
public class SuspendedStatusStrategy : IMembershipStatusStrategy { ... }
```

---

### L — Liskov Substitution Principle (LSP)

> *"Las subclases deben ser sustituibles por sus clases base."*

**Aplicación en el proyecto:**

Todas las implementaciones de repositorios e interfaces pueden sustituirse entre sí sin romper el comportamiento esperado.

```csharp
// Base contract
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}

// Cualquier implementación puede sustituir a IRepository<Client>
// sin alterar el comportamiento esperado por la capa de aplicación
public class ClientRepository : IRepository<Client>, IClientRepository { ... }
```

---

### I — Interface Segregation Principle (ISP)

> *"Los clientes no deben ser forzados a depender de interfaces que no utilizan."*

**Aplicación en el proyecto:**

Las interfaces están segregadas por dominio funcional específico.

```csharp
// ✅ Interfaces segregadas
public interface IClientRepository
{
    Task<Client?> GetByIdAsync(int id);
    Task<IEnumerable<Client>> GetAllAsync();
    Task AddAsync(Client client);
}

public interface IMembershipRepository
{
    Task<Membership?> GetByIdAsync(int id);
    Task<IEnumerable<Membership>> GetByClientIdAsync(int clientId);
    Task RenewAsync(int membershipId, DateTime newEndDate);
}

// ❌ Violaría ISP — una sola interfaz gigante que obliga a implementar todo
public interface IEverythingRepository
{
    Task<Client?> GetClientById(int id);
    Task<Membership?> GetMembershipById(int id);
    Task<WorkoutSession?> GetSessionById(int id);
    // ...30 más métodos...
}
```

---

### D — Dependency Inversion Principle (DIP)

> *"Los módulos de alto nivel no deben depender de módulos de bajo nivel. Ambos deben depender de abstracciones."*

**Aplicación en el proyecto:**

Los controladores y servicios dependen de **interfaces** (abstracciones), nunca de implementaciones concretas.

```csharp
// ✅ CORRECTO - ClientController depende de la abstracción IClientService
public class ClientController : ControllerBase
{
    private readonly IClientService _clientService; // abstracción

    public ClientController(IClientService clientService) // DI
    {
        _clientService = clientService;
    }
}

// Program.cs — El contenedor de DI conecta abstracciones con implementaciones
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IMembershipService, MembershipService>();
```

**Diagrama de dependencias:**
```
Controller → IService → IRepository → Database
     ↓            ↓           ↓
  (abstrac.)  (abstrac.)  (abstrac.)
     ↑            ↑           ↑
ServiceImpl  RepoImpl    EF Core
```

---

## Tecnologías

| Capa | Tecnología |
|---|---|
| Backend | .NET Core 8, C#, ASP.NET Web API |
| ORM | Entity Framework Core |
| Base de Datos | PostgreSQL |
| Frontend | React 18 + Vite + TypeScript |
| Estilos | CSS Modules + Custom Design System |
| IaC | Terraform |
| CI/CD | GitHub Actions |
| Documentación | Docfx |
| Contenedores | Docker |

## Instalación y Ejecución

### Prerequisitos

- .NET 8 SDK
- Node.js 20+
- PostgreSQL 15+
- Docker (opcional)

### Backend

```bash
cd backend
dotnet restore
dotnet ef database update --project src/GymManagement.Infrastructure --startup-project src/GymManagement.API
dotnet run --project src/GymManagement.API
```

La API estará disponible en: `http://localhost:5000`
Swagger UI: `http://localhost:5000/swagger`

### Frontend

```bash
cd frontend
npm install
npm run dev
```

El frontend estará disponible en: `http://localhost:5173`

### Docker Compose (recomendado)

```bash
docker-compose up -d
```

## API Endpoints

| Método | Endpoint | Descripción |
|---|---|---|
| `POST` | `/api/clients` | Registrar cliente |
| `GET` | `/api/clients/{id}` | Obtener detalle de cliente |
| `GET` | `/api/clients/{id}/memberships` | Ver historial de membresías |
| `GET` | `/api/clients/{id}/workout-sessions` | Ver sesiones de trabajo |
| `POST` | `/api/memberships` | Registrar membresía |
| `GET` | `/api/memberships/{id}` | Obtener detalle de membresía |
| `POST` | `/api/memberships/{id}/renew` | Renovar membresía |
| `POST` | `/api/workout-sessions` | Registrar sesión de entrenamiento |

## Infraestructura

La infraestructura se gestiona con Terraform y despliega en Azure:

- **Azure App Service** — Backend API
- **Azure Static Web Apps** — Frontend React
- **Azure Database for PostgreSQL** — Base de datos
- **Azure Container Registry** — Imágenes Docker

## CI/CD Pipelines

| Workflow | Descripción | Trigger |
|---|---|---|
| `infra.yml` | Despliegue de infraestructura Terraform | Push a `main` (infra/) |
| `infra_diagram.yml` | Genera diagrama de infraestructura | Push a `main` (infra/) |
| `class_diagram.yml` | Genera diagrama de clases UML | Push a `main` (backend/) |
| `publish_doc.yml` | Publica documentación en GitHub Pages | Push a `main` |
| `deploy_app.yml` | Despliega frontend y backend | Push a `main` |
| `release.yml` | Crea release con changelog | Push de tags `v*.*.*` |

## Autor

**Ankluna72** — Examen Unidad 1 — Patrones de Diseño de Software 2026-I

---

*Universidad Privada de Tacna — Facultad de Ingeniería — Escuela Profesional de Ingeniería de Sistemas*
