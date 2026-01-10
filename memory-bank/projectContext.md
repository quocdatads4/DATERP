# Project Context

## Overview
DATERP is a Modular ERP Platform built on the ABP Framework, focused on Education Systems (Schools, Language Centers, IT Training) and Enterprise Resource Planning. It aims to integrate AI-powered educational agents.

## Core Goals
1.  **Education**: Adaptive learning, IC3/MOS examination focus, practice-first approach.
2.  **ERP**: Modular enterprise management (CRM, Finance, HR, Inventory).
3.  **AI Integration**: "Antigravity" agents for educational support (Vietnamese language, supportive tone).

## Architecture Philosophy
-   **ABP Modular Monolith**: Strict adherence to module boundaries.
-   **Layered Design**:
    -   `src/`: Infrastructure, Hosting, Cross-cutting concerns.
    -   `modules/`: Business Logic (Domain, Application, EF Core).
    -   `themes/`: Presentation/UI.
-   **Data Isolation**: Each module owns its data and context. No cross-module joins.

## Tech Stack
-   **Framework**: .NET 9.0, ABP Framework v9.x
-   **UI**: ASP.NET Core MVC (Razor), Vanilla CSS / Custom Themes.
-   **Database**: SQL Server / PostgreSQL (deduced from `NpgsqlException` in history), Entity Framework Core.
-   **Authentication**: ABP Identity / OpenIddict.

## User Rules & Constraints
-   **Language**: Vietnamese for educational content.
-   **Design**: Premium, dynamic, modern UI (Glassmorphism, rich aesthetics).
-   **Forbidden**: Business logic in `src`, UI logic in `modules` (unless in ViewModels/DTOs), direct cross-module DB access.

## Authentication & Authorization
-   **Standard Roles**: The system strictly uses only three roles:
    1.  `Administrator`: full system access.
    2.  `Teacher`: access to educational management.
    3.  `Student`: access to learning materials and exams.
-   **Forbidden Roles**: The generic `admin` role has been removed and must not be used.

### Standard Accounts (Development/Demo)
| Role | Username | Email | Password |
| :--- | :--- | :--- | :--- |
| **Administrator** | `admin` | `admin@datacademy.edu.vn` | `Admin@123` |
| **Teacher** | `teacher` | `teacher@datacademy.edu.vn` | `Teacher@123` |
| **Student** | `student` | `student@datacademy.edu.vn` | `Student@123` |

> [!NOTE]
> All users are manually seeded or managed via `DATERPDataSeedContributor` and `DATERPDemoDataSeedContributor`.
