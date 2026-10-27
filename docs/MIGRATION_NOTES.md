# Migration Notes

## What Was Migrated

### From ASP.NET Web Forms 4.5.2 to .NET 8

| Old Component | New Component |
|---------------|---------------|
| `.aspx` pages | Razor Pages (`.cshtml`) |
| Code-behind (`.aspx.cs`) | Page Models (`.cshtml.cs`) |
| Master pages (`.master`) | Layout pages (`_Layout.cshtml`) |
| `Global.asax` | `Program.cs` |
| `Web.config` | `appsettings.json` |
| `packages.config` | `<PackageReference>` in `.csproj` |
| ADO.NET + Stored Procedures | Entity Framework Core 8 |
| `System.Web.UI.Page` | `PageModel` (Razor Pages) |
| `Session["key"]` | `HttpContext.Session.GetInt32("key")` |
| `Response.Redirect()` | `RedirectToPage()` |
| `System.Configuration.ConfigurationManager` | `IConfiguration` |

## Architecture Changes

### Old Architecture (3-Tier)
- Presentation: ASPX pages + Code-behind
- Business Logic: DAL class (myDAL.cs)
- Data Access: ADO.NET with stored procedures

### New Architecture (Clean Architecture)
- **Domain Layer**: Entities, Interfaces, Enums, Exceptions
- **Application Layer**: Services, DTOs, Business Logic
- **Infrastructure Layer**: EF Core DbContext, Repositories
- **Web Layer**: Razor Pages, ViewModels, DI Configuration

## Key Differences

### Authentication
- Old: Session-based with manual validation
- New: Session-based with service layer abstraction (ready for ASP.NET Core Identity upgrade)

### Data Access
- Old: ADO.NET with SqlCommand and stored procedures
- New: Entity Framework Core with LINQ queries and repository pattern

### Configuration
- Old: `Web.config` with `<connectionStrings>` and `<appSettings>`
- New: `appsettings.json` with `IConfiguration` and Options pattern

### Dependency Injection
- Old: Manual instantiation (`new myDAL()`)
- New: Constructor injection with built-in DI container

## Breaking Changes
1. Stored procedures replaced with EF Core LINQ queries
2. Session handling changed to use typed extension methods
3. ViewState removed (not needed with Razor Pages)
4. HTTP modules replaced with middleware

## Known Issues
1. The original application used stored procedures extensively. These have been replaced with EF Core queries that provide equivalent functionality.
2. The `ReputeIndex` calculation logic from stored procedures needs to be verified against business requirements.

## Future Improvements
1. Implement ASP.NET Core Identity for proper authentication
2. Add JWT authentication for API endpoints
3. Implement caching for frequently accessed data
4. Add pagination for large data sets
5. Implement real-time notifications with SignalR
