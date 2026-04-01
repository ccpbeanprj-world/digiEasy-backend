# digiEasy Backend

ASP.NET Core 2.0 Web API for user registration and password updates. Data is stored in SQLite via Entity Framework Core; the database file is created automatically on first run.

## Prerequisites

- [.NET Core 2.0 SDK](https://dotnet.microsoft.com/download/dotnet/2.0) (or a compatible runtime if you only publish/run a built app)

## Run locally

```bash
dotnet restore
dotnet run
```

Open the URL printed in the console (default is often `http://localhost:5000`). A `GET` on the root returns a short status message and lists the API routes.

## API

| Method | Path | Description |
|--------|------|-------------|
| `GET` | `/` | Health/info and endpoint list |
| `POST` | `/api/users/register` | Register a new user |
| `PATCH` | `/api/users/password` | Change password for an existing user |

Request/response bodies are JSON. Registration validates email, password strength, age (18+), gender (`M` / `F`), and an 8-digit mobile number.

## Configuration

Connection string in `appsettings.json` points at a SQLite file (`digi_easy.db` by default) next to the running app. Change `ConnectionStrings:DefaultConnection` if you want a different path.

## Example requests

The repo includes sample payloads you can adapt:

- `_create_example.txt` — PowerShell hashtable for registration
- `_update_example.txt` — password update example
# digiEasy-backend
