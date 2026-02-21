# ProQuote

ProQuote is a role-based RFQ (Request for Quotation) management platform built with .NET and Blazor.

## Tech Stack

- .NET 10
- ASP.NET Core Blazor (Server, interactive components)
- Entity Framework Core
- SQL Server (LocalDB by default)
- MudBlazor UI

## Solution Structure

- `src/ProQuote.Web` - Web UI and app host
- `src/ProQuote.Application` - Application layer (use cases, DTOs, interfaces)
- `src/ProQuote.Domain` - Domain entities and enums
- `src/ProQuote.Infrastructure` - Data access, identity, repositories, services

## Getting Started

1. Restore dependencies:
```powershell
dotnet restore ProQuote.slnx
```

2. Build:
```powershell
dotnet build ProQuote.slnx
```

3. Run the web app:
```powershell
dotnet run --project src/ProQuote.Web/ProQuote.Web.csproj
```

## Configuration

Main configuration file:

- `src/ProQuote.Web/appsettings.json`

Default local database connection uses LocalDB (`ProQuoteDb`).

## Status

The codebase has been renamed from `RFQApp` to `ProQuote` across solution and project structure.
