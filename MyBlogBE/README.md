# MyBlogBE

MyBlogBE is the backend API for the MyBlog application. It is built with .NET (targeting .NET 8.0) and exposes REST endpoints used by the frontend.

This README explains how to configure, run and develop the backend locally, and highlights important configuration keys found in `WebApi/appsettings.json`.

## Contents

- `WebApi/` - ASP.NET Core Web API project and application entry (contains `Program.cs` and `appsettings.json`).
- `Application/` - business logic, DTOs and services.
- `BusinessObject/` - entity models and enums.
- `DataAccess/` - EF DbContext, repositories, and seeds.

## Prerequisites

- .NET SDK 8.0+ (dotnet)
- A supported database (set via the connection string)
- (Optional) Docker

Verify .NET SDK:

```powershell
dotnet --version
```

## Configuration

Primary configuration file: `WebApi/appsettings.json`.
Important keys (examples from the project):

- `ConnectionStrings:DefaultConnection` — database connection string. Replace `YourConnectionStringHere` with your database connection.
- `BaseSettings:FrontendUrl` — the frontend URL used for CORS (default in the repository: `http://localhost:5173`).
- `BaseSettings:JwtSettings:Key` — JWT signing key. Replace with a secure secret in production.
- `BaseSettings:EmailSettings` — SMTP settings for outgoing emails (SmtpServer, Port, SenderEmail, Username, Password).
- `BaseSettings:UploadSettings` — keys and endpoint for image upload service.

For local development you can create `appsettings.Development.json` or use environment variables. Example environment variable name mapping for Docker/OS-level envs:

- `ConnectionStrings__DefaultConnection`
- `BaseSettings__JwtSettings__Key`

Never commit production secrets to source control.

## How it runs (what Program.cs does)

- Registers controllers, localization, and HTTP context accessor.
- Adds services for Swagger, Database, Settings, Security, Hosts, Loggers, Repositories, and Application services via extension methods.
- Configures middleware in this order: API logging, exception handling, request localization, forwarded headers, routing, Swagger (development only), CORS, authentication, user validation middleware, authorization, and then maps controllers.
- In Development environment the app automatically enables Swagger and runs `Seeder.Seed(db)` to populate initial data. Comment out the seeder call if you don't want auto-seed.

Key middleware and behavior:

- Swagger is enabled only in Development.
- Forwarded headers are configured to accept `X-Forwarded-For` and `X-Forwarded-Proto` (important when behind a proxy).
- CORS policy name used: `_myAllowSpecificOrigins` (configured in Web services extensions).

## Run locally

1. Update configuration:
   - Edit `WebApi/appsettings.json` or create `appsettings.Development.json` and set `ConnectionStrings:DefaultConnection` and other settings.

2. Build and run (PowerShell example):

```powershell
cd .\MyBlogBE\WebApi
dotnet build -c Debug
dotnet run
```

3. When running in Development:

- Swagger UI will be available (e.g. `http://localhost:<port>/swagger`). The exact port will be printed by `dotnet run`.
- The database seeder will run automatically; if you want to prevent automatic seeding, remove or comment out the seeding block in `Program.cs`.

## Database migrations / seeding

- The repository includes a `DataAccess` project with `MyBlogContext` and a Seeder. If you use EF migrations, create and apply migrations from the solution root or the `DataAccess` project depending on your setup.

Example (generic EF CLI commands):

```powershell
cd .\MyBlogBE\DataAccess
dotnet ef migrations add InitialCreate --startup-project ..\WebApi
dotnet ef database update --startup-project ..\WebApi
```

Adjust `--startup-project` paths as necessary.

## Swagger / API documentation

- Swagger is registered through `AddSwaggerServices()` and enabled automatically in Development. Open `/swagger` while the API is running to inspect endpoints and test them.

## Docker

There is a `dockerfile` in `MyBlogBE` (repository root).

```powershell
# Build image from repository root
docker build -t myblog-be .\MyBlogBE

# Run with connection string provided via environment variable
docker run -e "ConnectionStrings__DefaultConnection=Server=...;Database=...;User Id=...;Password=...;" -p 5000:80 myblog-be
```

Keep secrets out of Dockerfile and pass them as environment variables or use a secret manager.

## Development tips

- If you are debugging authentication or CORS issues, check:
  - `BaseSettings:FrontendUrl` matches the frontend origin.
  - Authentication configuration in the `AddSecurityServices` extension.
  - Forwarded headers when running behind proxies or reverse proxies.

- To avoid automatic seeding during development, comment out the seeder block in `Program.cs`.

---
