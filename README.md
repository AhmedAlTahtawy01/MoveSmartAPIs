# MoveSmartAPIs

A comprehensive fleet management system backend API for hospital vehicle operations, built with ASP.NET Core 8.0.

## 🚀 Live Demo

**API Documentation**: [https://movesmartapi.runasp.net/index.html](https://movesmartapi.runasp.net/index.html)

## 📋 Overview

MoveSmartAPIs is a graduation project that provides a complete backend solution for managing hospital fleet operations including:

- **Vehicle Management**: Track vehicles, buses, and their status
- **Driver Management**: Manage drivers and their assignments
- **Maintenance System**: Handle maintenance applications and records
- **Inventory Management**: Track spare parts and consumables
- **Mission Management**: Coordinate missions and job orders
- **Patrol System**: Manage patrols and subscriptions
- **User Management**: Role-based access control with JWT authentication

## 🏗️ Architecture

The project follows a clean 3-layer architecture:

```
Move_Smart/
├── Move_Smart/           # Presentation Layer (Controllers)
├── BusinessLogicLayer/   # Business Logic Layer (Services)
└── DataAccessLayer/      # Data Access Layer (Repositories)
```

### Technology Stack

- **Framework**: ASP.NET Core 8.0
- **Database**: MySQL
- **Authentication**: JWT Bearer Tokens
- **Real-time**: SignalR for notifications
- **Documentation**: Swagger/OpenAPI
- **Architecture**: Repository Pattern with 3-Layer Architecture

## 🚀 Quick Start

### Prerequisites

- .NET 8.0 SDK
- MySQL Server
- Visual Studio 2022

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd MoveSmartAPIs
   ```

2. **Database Setup**
   ```sql
   -- Run the database script
   source Database/Move_Smart.sql
   ```

3. **Environment Configuration**
   ```bash
   # Set connection string
   setx ConnectionStrings__DefaultConnection "Server=localhost;Database=move_smart;Uid=your_username;Pwd=your_password;"
   
   # Set JWT key
   setx JWT__Key "YourSuperSecretKeyHere"
   ```

4. **Build and Run**
   ```bash
   cd Move_Smart/Move_Smart
   dotnet restore
   dotnet build
   dotnet run
   ```

5. **Access API Documentation**
   - Swagger UI: `https://localhost:7000/swagger`
   - API Base URL: `https://localhost:7000/api`

## 📚 API Endpoints

### Core Modules

- **Users** (`/api/users`) - User management and authentication
- **Vehicles** (`/api/vehicles`) - Vehicle fleet management
- **Drivers** (`/api/drivers`) - Driver management
- **Buses** (`/api/buses`) - Bus capacity management
- **Applications** (`/api/applications`) - Application workflow
- **Maintenance** (`/api/maintenance`) - Maintenance tracking
- **Missions** (`/api/missions`) - Mission coordination
- **Patrols** (`/api/patrols`) - Patrol management
- **Spare Parts** (`/api/spareparts`) - Inventory management
- **Consumables** (`/api/consumables`) - Consumable tracking

### Authentication

All endpoints require JWT authentication except for login:
- **POST** `/api/users/login` - User authentication
- **POST** `/api/users/register` - User registration

## 🔐 User Roles

The system supports role-based access control:

- **SuperUser** - Full system access
- **HospitalManager** - Hospital-level management
- **GeneralManager** - General management operations
- **GeneralSupervisor** - Supervision and approval
- **PatrolsSupervisor** - Patrol management
- **WorkshopSupervisor** - Workshop operations
- **AdministrativeSupervisor** - Administrative tasks

## 📊 Database Schema

The database includes 20+ tables covering:

- **Core Entities**: Users, Vehicles, Drivers, Employees
- **Applications**: Job orders, maintenance, purchase orders
- **Operations**: Missions, patrols, vacations
- **Inventory**: Spare parts, consumables, replacements
- **Relationships**: Mission-vehicle assignments, patrol subscriptions

## 🛠️ Development

### Project Structure

```
Move_Smart/
├── Controllers/          # API Controllers
├── Models/              # Request/Response Models
├── Program.cs           # Application entry point
└── appsettings.json     # Configuration
```

### Naming Conventions

- **Variables**: `camelCase`
- **Functions**: `PascalCase`
- **Classes**: `PascalCase`
- **Files**: `PascalCase` with appropriate suffixes
  - Controllers: `PascalCaseController.cs`
  - Services: `PascalCaseService.cs`
  - Repositories: `PascalCaseRepo.cs`

### Adding New Features

1. Create entity in DataAccessLayer
2. Implement repository pattern
3. Add business logic in BusinessLogicLayer
4. Create API controller
5. Update documentation

## 📅 Project Timeline

See [BACKEND_DEADLINES.md](BACKEND_DEADLINES.md) for detailed project milestones and team assignments.

## 🤝 Contributing

1. Follow the naming conventions in [PROJECT_GUIDELINES.md](PROJECT_GUIDELINES.md)
2. Ensure all tests pass
3. Update documentation
4. Submit pull request for review

## 📄 License

This project is part of a graduation project for academic purposes.

## 📞 Support

For questions or issues, please refer to the project documentation or contact the development team.
