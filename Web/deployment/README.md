# HiSubmit Production Deployment

This folder contains the compiled, framework-dependent Release publish output of
`Web/Web.csproj` targeting `net8.0`.

## Purpose

Plesk/IIS should deploy **this folder's contents** to the production httpdocs root, not
the raw source repository tree.

## Entry Point

IIS is configured via `web.config` to run:
```
dotnet .\Web.dll
```
using ASP.NET Core Module V2 (in-process hosting).

## Required Runtime

- .NET 8 ASP.NET Core Runtime (framework-dependent — must be installed on the server)
- SQL Server accessible at the configured connection string

## Configuration

`appsettings.json` contains non-secret configuration.

**The database password and other secrets must be provided via environment variables on
the production server — NOT committed to this repository.**

Set the following environment variable in Plesk / IIS:

| Name | Value |
|---|---|
| `ConnectionStrings__DefaultConnection` | Full SQL Server connection string including password |

The double underscore `__` is the ASP.NET Core nested-key separator and overrides
`appsettings.json` at runtime.

## Directories on the server (NOT in this folder)

These directories exist on the production server and must NOT be overwritten during deployment:

- `Files/` — user-uploaded submission files
- `private-files/` — private user content
- `Logs/` — application log files (Serilog)
- `.well-known/` — SSL certificate renewal

## Updating this folder

After source code changes, run from the repository root:

```powershell
dotnet clean Web\Web.csproj -c Release
dotnet restore Web\Web.csproj
dotnet build  Web\Web.csproj -c Release
dotnet publish Web\Web.csproj -c Release -f net8.0 -o D:\hisubmit-publish --self-contained false /p:UseAppHost=false
```

Then sync the publish output into this `deployment/` directory (excluding `.pdb`, `Logs/`, `Files/`, `.dylib`, `.so`).
