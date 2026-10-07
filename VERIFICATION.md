# Verification / Audit Notes

## Checks performed
- Inspected all C# source files, Razor views, project file, configuration and README.
- Reworked authentication to use ASP.NET Core Identity with explicit MVC login/register/logout views.
- Added password policy, lockout and unique-email configuration.
- Added server-side role checks and Sales Executive record scoping for MVC dashboards/pages and API GET/PUT access.
- Added DTOs to API responses instead of returning EF entities directly.
- Added anti-forgery attributes to all state-changing MVC actions.
- Added client-side unobtrusive validation libraries to the layout.
- Added required opportunity/follow-up business validation.
- Added Chart.js dashboard charts.
- Added duplicate email/phone protection for customers.
- Added audit logging to major MVC create/update/delete/authentication operations.
- Checked C# brace balance and project XML validity.
- Checked for accidental password-hash/security-field exposure in API projections.

## Environment limitation
The verification environment used to prepare this package does not contain the .NET 8 SDK (`dotnet` command is unavailable). Therefore a real `dotnet restore`, `dotnet build`, database startup, HTTP integration test, and browser test could not be executed here.

Run these commands on a machine with .NET 8 SDK installed before publishing:

```bash
dotnet restore
dotnet build
dotnet run
```

This is a source-level audit and correction, not a claim that a binary build was executed in this environment.
