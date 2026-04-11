# TaskManagerAPI
Simple task manager API project for managing tasks.

## Features
- User authentication using JWT
- Role-based authorization (Admin, TeamLeader, Developer)
- User registration (Admin only)
- Secure password handling via ASP.NET Core Identity
- Pre-seeded users and roles
- Swagger UI with JWT authentication support
- Task management domain (TaskItem, TaskItemStatus)
- Repository pattern implementation for data access
- TaskItemStatus management (CRUD - in progress)
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
