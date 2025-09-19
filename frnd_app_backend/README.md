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

## Quick start

- dotnet restore
- dotnet build
- dotnet run

Swagger UI: http://localhost:3001/docs  
OpenAPI JSON: http://localhost:3001/swagger/v1/swagger.json

## Configuration

appsettings.json contains:
- ConnectionStrings:Default (Sqlite)
- Jwt: Issuer, Audience, SigningKey, ExpiryMinutes

Set environment variables to override for production.

## Migrations

On startup the app runs `Database.Migrate()` to create/update the Sqlite database schema automatically.

## Auth

Add header `Authorization: Bearer <token>` to access protected endpoints.
