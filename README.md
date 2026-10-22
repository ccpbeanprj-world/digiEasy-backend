# digiEasy Backend

ASP.NET Core 8 Web API: user **register**, **password update**, **verify password**, **delete** (SQLite + EF Core). **Swagger UI** is enabled only in **Development**.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) **8.0.426** or any newer 8.0 SDK, including later feature bands such as 8.0.5xx (runtime **8.0.32**, October 2026 security update). [`global.json`](global.json) sets that minimum with `rollForward: latestFeature`. SDKs older than 8.0.426 are not used.

## Quick start

```bash
dotnet restore
dotnet build
dotnet test
dotnet run --project digiEasy-backend.csproj
```

- **API root:** `GET /` → `{ "message": "digiEasy API", "swagger": "/swagger" }`
- **Swagger:** `http://localhost:5000/swagger` (or your URL) — **only when `ASPNETCORE_ENVIRONMENT=Development`**

**Windows / file locks:** if build fails with MSB3027, stop the running app (**Ctrl+C**) or run `.\scripts\stop-locked-api.ps1`. Prefer `.\scripts\run.ps1` (stops old host, then runs). Faster restarts: `.\scripts\run.ps1 -NoBuild` or `dotnet run --no-build` after a successful build.

## API

| Method | Path | Body (JSON) |
|--------|------|-------------|
| `GET` | `/` | — |
| `POST` | `/api/users/register` | `RegisterRequest` |
| `PATCH` | `/api/users/password` | `UpdatePasswordRequest` |
| `POST` | `/api/users/verify-password` | `VerifyPasswordRequest` |
| `DELETE` | `/api/users` | `DeleteUserRequest` |

Sample JSON: [`examples/`](examples/). PowerShell helpers: [`scripts/`](scripts/) (`register.ps1`, `update-password.ps1`, `verify-password.ps1`, `run.ps1`, `stop-locked-api.ps1`).

## Configuration

`ConnectionStrings:DefaultConnection` in `appsettings.json` — default SQLite file `digi_easy.db` (created on first run, ignored by git).

---

## Security (application)

This repo is a **sample / dev-oriented** API. Before any real deployment:

| Topic | Current state | Hardening |
|--------|----------------|-----------|
| **Auth** | None — all endpoints are public | Add authentication (JWT, cookies, API keys) and authorization policies. |
| **Swagger** | **Off in Production** (`Program.cs`) | Keep it that way; do not expose docs on the public internet without auth. |
| **HTTPS** | Not enforced here | Use HTTPS in production (reverse proxy, Kestrel certs). |
| **Secrets** | No secrets in committed `appsettings` | Use **User Secrets** (dev), **environment variables**, or a vault in production — never commit passwords or connection strings with real credentials. |
| **Rate limiting** | None | Add rate limiting / WAF to slow abuse (register, verify-password). |
| **`verify-password`** | Reveals whether an email exists and if a password matches | **Dev/troubleshooting only** — remove or strictly protect in production (it enables account probing). |
| **CORS** | Not configured | Set explicit origins if you add a browser front end. |
| **Passwords** | Hashed with ASP.NET Core `PasswordHasher` | Good for storage; pair with transport encryption (HTTPS). |

## Git & pushing safely

- **Do not commit:** local SQLite (`*.db`), `bin/`, `obj/`, `.vs/`, user secrets, or `.last-register-email` (see [`.gitignore`](.gitignore)).
- **Before `git push`:** `git status` — ensure no `appsettings.Production.json` with real secrets (prefer env vars).
- **On GitHub/GitLab:** enable **branch protection** on `main`, **required reviews**, and (if private) **secret scanning**. Rotate any credential that was ever committed by mistake.

---

## Scripts (PowerShell, repo root)

| Script | Purpose |
|--------|---------|
| `scripts/run.ps1` | Stop lock holders + `dotnet run` (`-NoBuild` optional) |
| `scripts/stop-locked-api.ps1` | Stop `digiEasy-backend` / `dotnet` hosting this DLL |
| `scripts/register.ps1` | Register with random email; writes `.last-register-email` |
| `scripts/update-password.ps1` | Patch password (reads `.last-register-email` if `-Email` omitted) |
| `scripts/verify-password.ps1` | Call verify-password API |

If execution policy blocks: `powershell -ExecutionPolicy Bypass -File .\scripts\run.ps1`

**PowerShell tip:** `Invoke-RestMethod` must be one logical line (or use `` ` `` for continuation); broken lines drop `-Body` → HTTP 400.

## Troubleshooting

- **`dotnet run` slow:** it runs MSBuild each time — use `--no-build` or `dotnet watch run` after an initial build.
- **Alternate output when bin is locked:** `dotnet build -o .\artifacts\build` then `dotnet .\artifacts\build\digiEasy-backend.dll` from repo root (working directory matters for `digi_easy.db`).

## Project layout

- `Controllers/` — HTTP endpoints  
- `Models/` — request DTOs + `User`  
- `Data/` — EF Core `AppDbContext`  
- `Swagger/` — OpenAPI examples for Swagger UI  
- `digiEasy-backend.Tests/` — integration tests (`WebApplicationFactory`)
