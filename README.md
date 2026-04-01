# digiEasy Backend

ASP.NET Core 2.0 Web API with SQLite (EF Core). The database file is created on first run.

## Prerequisites

.NET Core 2.0 SDK

## Run

```bash
dotnet restore
dotnet run
```

Open the URL shown in the console. `GET /` lists the available routes.

## Endpoints

- `GET /` — status and route list
- `POST /api/users/register` — register a user (JSON)
- `PATCH /api/users/password` — update password (JSON)

## Configuration

Set `ConnectionStrings:DefaultConnection` in `appsettings.json` to change the SQLite file (default: `digi_easy.db`).

Sample request bodies: `_create_example.txt`, `_update_example.txt`.
