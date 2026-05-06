# SkyRoute — Flight Search & Booking (Angular + .NET)

## Requirements

- Node.js + npm (tested with Node `v22.x`)
- .NET SDK (tested with .NET SDK `10.0.202`)
- SQL Server (LocalDB or local/remote SQL Server)

## Run the project (frontend + backend)

From the repository root, open two PowerShell terminals.

### 1) Frontend (Angular)

```powershell
cd skyroute-frontend
npm install
npm start
```

App: `http://localhost:4800`

### 2) Backend (.NET API)

In another terminal:

```powershell
cd skyroute-backend
dotnet restore
```

**Apply EF Core migrations** (do this before the first run, or whenever you add migrations). Requires the EF CLI tool once: `dotnet tool install --global dotnet-ef`.

```powershell
dotnet ef database update --project SkyRoute.Api.csproj
```

Optional: use your Development settings when applying from the CLI:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet ef database update --project SkyRoute.Api.csproj
```

Then start the API:

```powershell
dotnet run
```

API (per `launchSettings.json`): `http://localhost:5000`

Main endpoints:

- `POST /api/flights/search`
- `POST /api/bookings`

## Database (EF Core + SQL Server)

### Connection string

By default the backend uses this connection string (in `skyroute-backend/appsettings.json`):

```text
Server=(localdb)\mssqllocaldb;Database=SkyRouteDev;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true
```

### Migrations

On startup the backend runs `db.Database.Migrate()` automatically (see `skyroute-backend/Program.cs`), so:

- If the DB does not exist, it is created
- If there are pending migrations, they are applied

### Change DB / credentials

You can override the connection string via environment variable:

```powershell
$env:ConnectionStrings__SkyRouteSql="Server=localhost;Database=SkyRouteDev;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=true"
cd skyroute-backend
dotnet run
```

## “Super user” / admin

This challenge **does not implement authentication, roles, or users** (no Identity, no superuser seeding).

- There are no admin credentials to configure.
- If you need auth/admin, that would be new scope.

## Tests

### Backend

```powershell
dotnet test skyroute-backend-tests/SkyRoute.Api.Tests/SkyRoute.Api.Tests.csproj
```

### Frontend

```powershell
cd skyroute-frontend
npm test
```

