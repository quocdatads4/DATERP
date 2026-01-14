# System Patterns

## Tech Stack
-   **Framework**: .NET 9.0
-   **Platform**: ABP Framework 9.0.0
-   **UI**: ASP.NET Core MVC (Razor) with Custom `Education` Theme.
-   **Database**: Entity Framework Core 9.0.0 (PostgreSQL/SQL Server compatible).
-   **Logging**: Serilog (Console, File, Async).
-   **API Documentation**: Swagger / OpenApi.

## Module Structure
Each business module (e.g., `modules/Academic`) is currently a placeholder (`README.md` only) and awaits implementation following the DDD layered structure:
-   **Domain**: Entities, Aggregates, Domain Services, Repository Interfaces.
-   **Domain.Shared**: Enums, Constants, Localization, Error Codes.
-   **Application.Contracts**: DTOs, Application Service Interfaces.
-   **Application**: Application Service Implementations (Use Cases).
-   **EntityFrameworkCore**: DbContext, Repository Implementations, EF Mappings.
-   **HttpApi**: API Controllers (exposed via REST).
-   **Web**: UI implementation (Razor Pages/Views) for that specific module.

### Infrastructure (`src`)
-   **DATERP.Web**: Main Host. Configures:
    -   `EducationThemeModule` for UI.
    -   `RewriteOptions` for mapping `themes/education/*` to embedded resources.
    -   Standard ABP modules (`Identity`, `TenantManagement`, `Account`).
-   **DATERP.Domain.Shared**: Shared kernel.
-   **DATERP.HttpApi.Host**: Backend API host.

### Theme Engine (`themes`)
-   **Education Theme**: Located in `themes/Education`.
    -   **Module**: `EducationThemeModule` registers the theme and bundles.
    -   **Bundling**: Defines `Education.Global.Styles` and `Education.Global.Scripts`.
    -   **Virtual File System**: Embeds resources under namespace `Education`.
    -   **Inheritance**: Uses ABP's `LayoutHook` system. Default Layout: `Application`.

## Key Design Patterns
-   **Dependency Injection**: Constructor injection only.
-   **Event Bus**: Used for cross-module communication (Domain Events).
-   **Repository Pattern**: Standard ABP repositories.
-   **AutoMapper**: For Entity <-> DTO mapping.
-   **Localization**: JSON-based, per module.

## Naming Conventions
-   **Namespace**: `DATERP.{Module}.*`
-   **Services**: `I{Name}AppService` / `{Name}AppService`
-   **Repositories**: `I{Name}Repository` / `{Name}Repository`

## CI/CD & Workflows
-   Use `task_boundary` for structured progress.
-   Artifacts in `.gemini/antigravity/brain`.
