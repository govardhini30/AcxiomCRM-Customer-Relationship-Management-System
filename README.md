# AcxiomCRM — Verified Source Package

Role-based CRM implemented with ASP.NET Core 8 MVC, Entity Framework Core, SQLite and ASP.NET Core Identity.

## Implemented
- Login, registration, logout and access-denied flow
- Identity password hashing and account lockout
- Admin, Manager and SalesExecutive roles
- Customer, Lead, Opportunity and Follow-Up CRUD workflows
- Client-side unobtrusive validation through DataAnnotations + server-side validation
- Required business rules for customers, leads, opportunities and follow-ups
- Dashboard KPI cards and Chart.js lead/opportunity charts
- Append-oriented audit logging service
- DTO-based REST APIs for Customers, Leads and Opportunities
- Swagger in Development
- Bootstrap responsive UI
- Anti-forgery protection on MVC state-changing forms
- SQLite database created automatically on first run

## Requirements
- .NET 8 SDK

## Run
```bash
dotnet restore
dotnet build
dotnet run
```

Open the HTTPS URL printed by the application. Swagger is available at `/swagger` in Development.

### Development admin
- Email: `admin@acxiomcrm.local`
- Password: `Admin@123`

Change this credential before any real deployment.

## Notes
This repository intentionally uses `Database.EnsureCreated()` so a fresh clone can run without requiring a pre-existing migration database. For production deployment, replace this with reviewed EF Core migrations and secure secrets/configuration.

## GitHub
```bash
git init
git add .
git commit -m "Initial verified AcxiomCRM implementation"
git branch -M main
git remote add origin YOUR_GITHUB_REPOSITORY_URL
git push -u origin main
```
