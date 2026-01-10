# Project Structure

## Root
-   `src/`: Main infrastructure and host application.
-   `modules/`: Domain-specific business logic modules.
-   `themes/`: Custom UI themes.
-   `memory-bank/`: Project documentation and active context.

## Shared Infrastructure (`src`)
-   `DATERP.Web`: Main ASP.NET Core MVC Host.
-   `DATERP.HttpApi.Host`: (Likely) Generic API Host.
-   `DATERP.Domain.Shared`: Shared enums/constants.

## Business Modules (`modules`)
(Currently largely scaffolding/empty, ready for implementation)
-   `Academic`: Student/Course management.
-   `Examination`: Testing and Assessment logic.
-   `LMS`: Learning Management System features.
-   `Finance`: Billing and payments.
-   `HR`: Staff management.
-   `CRM`: Customer relationships.
-   `Inventory`: Asset management.
-   `Reporting`: Cross-module analytics.

## Themes (`themes`)
-   `Education`: Custom implementation for "Adaptive - Beginner-friendly - Practice-first" UI.
    -   `Layouts/`: Razor layouts (Application, Account).
    -   `Components/`: UI bits (Footer, LoginIllustration).
    -   `Themes/`: Specific theme overrides (Education, Dark).

## Memory Bank (`memory-bank`)
-   `projectContext.md`: Goals and high-level context.
-   `systemPatterns.md`: Architecture and technical standards.
-   `structure.md`: This file.
-   `activeContext.md`: Current session goal.
