# TaskManagerAPI
RESTful ASP.NET Core Web API for managing tasks with JWT authentication, role-based authorization, and layered architecture.

## Features
- User authentication using JWT
- Role-based authorization (Admin, TeamLeader, Developer)
- User registration (Admin only)
- Secure password handling via ASP.NET Core Identity
- Pre-seeded users and roles
- Swagger UI with JWT authentication support
- Task management domain (TaskItem, TaskItemStatus)
- Repository pattern implementation for data access
- TaskItemStatus management (GetAll, GetById, Create, Update, Delete)
- TaskItems management (GetAll, GetById, Create, Update)
- DTO mapping using AutoMapper

## Tech Stack
- ASP.NET Core Web API (.NET 9)
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity
- JWT Authentication
- Swagger (Swashbuckle)
- Repository Pattern
- AutoMapper

## Architecture
The project follows a layered architecture:

- Controllers – handle HTTP requests and responses
- Services – contain business logic
- Repositories – handle data access using Entity Framework Core
- Data layer – DbContext and entity configurations

## API Endpoints

### Authentication
- POST /api/auth/login

### Accounts (Admin only)
- POST /api/accounts

### TaskItemStatuses
Requires authentication (JWT Bearer token)

- GET /api/taskitemstatuses – get all statuses
- GET /api/taskitemstatuses/{id} – get status by id
- POST /api/taskitemstatuses – create new status
- PUT /api/taskitemstatuses/{id} – update status by id
- DELETE /api/taskitemstatuses/{id} – delete status by id

### TaskItems
Requires authentication (JWT Bearer token)

- GET /api/taskitems – get user tasks (supports paging & filtering via query params)
- GET /api/taskitems/{id} – get user task by id
- POST /api/taskitems – create new task
- PUT /api/taskitems/{id} – update task by id

## Authentication
This API uses JWT Bearer authentication.

To access protected endpoints:
1. Call `/api/auth/login`
2. Copy the returned JWT token
3. Use it in requests:

Authorization: Bearer {token}

## Getting Started
1. Clone repository
```bash
git clone https://github.com/your-username/taskmanager-api.git
cd taskmanager-api
```

2. Configure environment

Update appsettings.Development.json:
- Connection string
- JWT settings
- Seeded users

3. Apply migrations

Package Manager Console:
```powershell
Update-Database
```

CLI: 
```bash
dotnet ef database update
```

4. Run the application

Swagger UI: 

`https://localhost:7168/swagger`
