# WorkshopFlow — Manufacturing ERP REST API

A lightweight Manufacturing ERP (Enterprise Resource Planning) backend built with **ASP.NET Core (.NET 10)**, featuring a full production management workflow: Items with Bills of Materials and Routing, Work Orders with sequential Operations, Inventory tracking, and Role-Based Access Control.

---

## Table of Contents

- [Features](#features)
- [Architecture](#architecture)
- [Tech Stack](#tech-stack)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Environment Variables](#environment-variables)
- [Default Users](#default-users)
- [API Overview](#api-overview)
- [Role & Capability Matrix](#role--capability-matrix)
- [Running Tests](#running-tests)
- [Database Migrations](#database-migrations)

---

## Features

- **Items Management** — Full CRUD with item types (RawMaterial, SemiFinished, FinalProduct, Consumable), automatic weight calculation from BOM
- **Bill of Materials (BOM)** — Multi-level BOM support, component quantity tracking
- **Routing Steps** — Production routing per item with workstation/machine assignment
- **Work Orders** — Full lifecycle: Draft → Released → InProgress → Completed/Cancelled, with stock validation on release
- **Operations** — Sequential operation execution (assign operator → start → complete), automatic stock production on completion
- **Inventory Transactions** — Purchase, Adjustment, Production, and Consumption tracking with stock level management
- **Workstations & Machines** — Equipment management with nested machine assignment
- **Users & Roles** — JWT authentication with 4 roles and granular capability-based authorization
- **Unit Tests** — xUnit + Moq test suite covering core business logic

---

## Architecture

```
WorkshopFlow/
├── Controllers/          # API endpoints (REST)
├── Services/             # Business logic layer
│   ├── IXxxService.cs    # Service interfaces
│   └── XxxService.cs     # Service implementations
├── Repositories/         # Data access layer
│   ├── IUnitOfWork.cs    # Unit of Work pattern
│   └── XxxRepository.cs  # Repository implementations
├── Models/               # EF Core entity models
├── DTO/                  # Data Transfer Objects (Insert/Update/ReadOnly)
├── Exceptions/           # Custom exception types
├── Configuration/        # AutoMapper, JWT, CORS setup
├── Migrations/           # EF Core database migrations
└── Resources/db/         # Flyway-style seed SQL scripts

WorkshopFlow.Tests/
├── Services/
│   ├── ItemServiceTests.cs
│   ├── WorkOrderServiceTests.cs
│   └── InventoryTransactionServiceTests.cs
```

**Design Patterns:** Repository Pattern, Unit of Work, Service Layer, DTO mapping (AutoMapper)

---

## Tech Stack

| Component | Technology |
|---|---|
| Framework | ASP.NET Core (.NET 10) |
| ORM | Entity Framework Core 10 |
| Database | SQL Server 2022 |
| Authentication | JWT Bearer Tokens |
| Object Mapping | AutoMapper |
| Containerization | Docker + Docker Compose |
| Testing | xUnit + Moq |
| API Documentation | Swagger / OpenAPI |

---

## Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (recommended)
- OR: .NET 10 SDK + SQL Server 2022

---

## Getting Started

### Option A — Docker (Recommended)

1. Clone the repository:
```bash
git clone https://github.com/dimigeo6595/WorkshopFlowWebModelFirstRestDev.git
cd WorkshopFlowWebModelFirstRestDev
```

2. Create a `.env` file in the root directory (see [Environment Variables](#environment-variables)):
```bash
cp .env.example .env
# Edit .env with your values
```

3. Start the application:
```bash
docker compose up --build
```

4. The API will be available at `http://localhost:8081`
5. Swagger UI: `http://localhost:8081/swagger`

### Option B — Local Development

1. Install .NET 10 SDK and SQL Server 2022
2. Update `appsettings.Development.json` with your connection string
3. Run migrations:
```bash
cd WorkshopFlow
dotnet ef database update
```
4. Run seed scripts from `Resources/db/` in order (V001 → V007)
5. Start the application:
```bash
dotnet run
```

---

## Environment Variables

Create a `.env` file in the project root:

```env
# Database
SA_PASSWORD=YourStrong!Passw0rd
DB_PORT=1437
DB_HOST=sqlserver
DB_NAME=WorkshopFlow
DB_USER=sa
DB_USER_PASSWORD=YourStrong!Passw0rd

# Application
APP_PORT=8081
ASPNETCORE_ENVIRONMENT=Production

# JWT
JWT_SECRET=your-super-secret-jwt-key-at-least-32-characters
JWT_ISSUER=https://localhost:8081
JWT_AUDIENCE=https://localhost:8081

# CORS
CORS_ORIGIN=http://localhost:5173
ALLOWED_HOSTS=*
```

---

## Default Users

Seeded automatically on first run:

| Username | Password | Role |
|---|---|---|
| `admin` | `C0d1ngF@!` | ADMIN |
| `engineer1` | `C0d1ngF@!` | PRODUCTION_ENGINEER |
| `operator1` | `C0d1ngF@!` | OPERATOR |
| `warehouse1` | `C0d1ngF@!` | WAREHOUSE_MANAGER |

> ⚠️ Change all passwords before deploying to production.

---

## API Overview

Base URL: `http://localhost:8081/api/v1`

All endpoints (except `/auth/login`) require a `Bearer` token in the `Authorization` header.

| Resource | Endpoints |
|---|---|
| Auth | `POST /auth/login` |
| Items | `GET/POST /items`, `GET/PUT/DELETE /items/{id}` |
| BOM | `GET/POST /items/{id}/bom`, `PUT/DELETE /items/{id}/bom/{lineId}` |
| Routing | `GET/POST /items/{id}/routing`, `PUT/DELETE /items/{id}/routing/{stepId}` |
| Work Orders | `GET/POST /workorders`, `GET/PUT/DELETE /workorders/{id}` |
| WO Status | `POST /workorders/{id}/release`, `POST /workorders/{id}/cancel` |
| Operations | `GET /workorders/{id}/operations`, `PATCH .../assign`, `PATCH .../start`, `PATCH .../complete` |
| Inventory | `GET /inventory/items/{itemId}`, `POST /inventory` |
| Workstations | `GET/POST /workstations`, `GET/PUT/DELETE /workstations/{id}` |
| Machines | `GET/POST /workstations/{id}/machines`, `PUT/DELETE /workstations/{id}/machines/{machineId}` |
| Users | `GET/POST /users`, `GET/PUT/DELETE /users/{id}` |
| Roles | `GET /roles` |
| UoM | `GET /uom` |

Full interactive documentation available at `/swagger` when running.

---

## Role & Capability Matrix

| Capability | ADMIN | PROD_ENGINEER | OPERATOR | WAREHOUSE |
|---|:---:|:---:|:---:|:---:|
| VIEW_ITEMS | ✅ | ✅ | ✅ | ✅ |
| INSERT_ITEM | ✅ | ✅ | ❌ | ❌ |
| EDIT_ITEM | ✅ | ✅ | ❌ | ❌ |
| DELETE_ITEM | ✅ | ❌ | ❌ | ❌ |
| VIEW_BOM / EDIT_BOM | ✅ | ✅ | ❌ | ❌ |
| VIEW_ROUTING / EDIT_ROUTING | ✅ | ✅ | ✅ / ❌ | ❌ |
| VIEW_WORK_ORDERS | ✅ | ✅ | ✅ | ❌ |
| INSERT/EDIT_WORK_ORDER | ✅ | ✅ | ❌ | ❌ |
| START/COMPLETE_WORK_ORDER | ✅ | ✅ | ✅ | ❌ |
| ASSIGN_WORK_ORDER | ✅ | ✅ | ❌ | ❌ |
| VIEW_INVENTORY | ✅ | ✅ | ❌ | ✅ |
| ADJUST_INVENTORY | ✅ | ❌ | ❌ | ✅ |
| VIEW_USERS | ✅ | ✅ | ❌ | ❌ |
| INSERT/EDIT/DELETE_USER | ✅ | ❌ | ❌ | ❌ |
| VIEW/EDIT_MACHINES | ✅ | ✅ / ❌ | ❌ | ❌ |

---

## Running Tests

```bash
cd WorkshopFlow.Tests
dotnet test
```

Expected output:
```
Test summary: total: 32, failed: 0, succeeded: 32, skipped: 0
```

The test suite covers:
- **ItemService** (9 tests) — CRUD, duplicate code detection, soft delete, weight calculation
- **WorkOrderService** (9 tests) — Creation validations, status transitions, operation sequencing
- **InventoryTransactionService** (7 tests) — Transaction type restrictions, negative stock prevention, stock updates

---

## Database Migrations

The project uses EF Core migrations for schema management and SQL scripts for seed data.

**Apply migrations:**
```bash
dotnet ef database update
```

**Create a new migration:**
```bash
dotnet ef migrations add MigrationName
```

**Seed scripts** (run in order after migrations):
```
Resources/db/
├── V001__InitialSeed.sql          # Roles, capabilities, initial users
├── V002__AddViewUserCapabilities.sql
├── V003__SeedUnitOfMeasures.sql
├── V004__SeedWorkstations.sql
├── V005__SeedMachines.sql
├── V006__SeedElbowDuctHierarchy.sql   # Sample items with BOM/Routing
└── V007__SeedWorkOrdersQuarter.sql    # Sample work orders (Q3 2026)
```

---

## Frontend

The companion React frontend is available at: [workshopflow-frontend](https://github.com/dimigeo6595/workshopflow-frontend)

Built with React 19, TypeScript, Vite, Tailwind CSS v4, and shadcn/ui.

---

## License

This project is developed for educational purposes.
