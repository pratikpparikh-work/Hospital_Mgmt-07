# Clinic Management System - .NET 8 Migration

## Overview
This is the migrated version of the Clinic Management System, originally built with ASP.NET Web Forms 4.5.2, now running on .NET 8 with clean architecture.

## Architecture
The solution follows Clean Architecture with four layers:

```
ClinicManagement.sln
├── src/
│   ├── ClinicManagement.Domain          # Entities, Interfaces, Enums, Exceptions
│   ├── ClinicManagement.Application     # Services, DTOs, Business Logic
│   ├── ClinicManagement.Infrastructure  # EF Core, Repositories, Data Access
│   └── ClinicManagement.Web             # Razor Pages, ViewModels, UI
└── tests/
    └── ClinicManagement.UnitTests       # xUnit unit tests
```

## Prerequisites
- .NET 8 SDK
- SQL Server (or SQL Server Express)
- Visual Studio 2022 or VS Code

## Setup Instructions

### 1. Database Setup
Run the SQL scripts in the `Database Files` folder to create the database:
```sql
-- Run in order:
1. Schema.sql
2. Admin.sql
3. Doctor.sql
4. Patient.sql
5. SignUp.sql
6. Insertions.sql
```

### 2. Configuration
Update the connection string in `src/ClinicManagement.Web/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=DBProject;Integrated Security=True;TrustServerCertificate=True;"
  }
}
```

### 3. Build and Run
```bash
cd src/ClinicManagement.Web
dotnet restore
dotnet build
dotnet run
```

## User Roles
- **Patient (Type=1)**: Register, book appointments, view history
- **Doctor (Type=2)**: Manage appointments, update prescriptions, billing
- **Admin (Type=3)**: Manage doctors, staff, view dashboard

## Key Features
- Patient registration and login
- Doctor appointment management
- Admin dashboard with statistics
- Bill management
- Treatment history tracking
- Patient notifications and feedback

## Technology Stack
- **Framework**: .NET 8
- **UI**: ASP.NET Core Razor Pages
- **ORM**: Entity Framework Core 8
- **Database**: SQL Server
- **Logging**: Serilog
- **Testing**: xUnit, Moq, FluentAssertions
- **CSS**: Bootstrap 5
