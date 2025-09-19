# Connect Hub - Backend (frnd_app_backend)

A modern .NET 8 Web API implementing:
- User authentication (JWT), registration and login
- User profiles (CRUD for current user, public fetch, search)
- Friend requests (send, accept, reject, list)
- Messaging (send to friends, conversation history)
- EF Core (Sqlite default) with code-first migrations
- Layered architecture (Controllers, Services, Repositories)
- Global error handling with consistent JSON responses
- Swagger (OpenAPI) docs themed with "Ocean Professional" (blue & amber accents)

## Dependencies

Defined in dotnet.csproj and restored with dotnet restore:
- Microsoft.EntityFrameworkCore (8.x)
- Microsoft.EntityFrameworkCore.Sqlite (8.x)
- Microsoft.EntityFrameworkCore.Design (8.x) (PrivateAssets=all)
- Microsoft.AspNetCore.Authentication.JwtBearer (8.x)
- Swashbuckle.AspNetCore (6.x)
- BCrypt.Net-Next (4.x)
- FluentValidation.AspNetCore (11.x)
- NSwag.AspNetCore (14.x) (optional; remove if unused)

## Quick start

```
dotnet restore
dotnet build
dotnet run
```

Default server URLs are determined by ASPNETCORE_URLS or launch settings. Swagger is served at:
- Swagger UI: /docs
- OpenAPI JSON: /swagger/v1/swagger.json

## Configuration

appsettings.json contains:
- ConnectionStrings:Default (Sqlite, default `Data Source=frndapp.db`)
- Jwt: Issuer, Audience, SigningKey, ExpiryMinutes

Override via environment variables in production (do not hardcode secrets). Ensure SigningKey is at least 32 characters.

## Migrations

On startup, the app runs `Database.Migrate()` to create/update the Sqlite database schema automatically.

## Auth

Add header:
```
Authorization: Bearer <token>
```

to access protected endpoints.
