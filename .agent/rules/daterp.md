---
trigger: always_on
---

# DATERP - Architectural & Behavioral Rules for Antigravity

You are working on **DATERP**, an **ABP-based Modular ERP Platform** designed for Education (Language Centers, IT Centers, Schools) and Enterprise resource planning.

## 1. Architectural Principles (Non-negotiable)
- **Architecture**: Strict **ABP Modular Monolith**.
- **Core System (`src/`)**: Contains ONLY shared infrastructure and host applications. **NO** business logic specific to a module (like specific exam rules or course management) goes here.
- **Business Modules (`modules/`)**: ALL business logic must be encapsulated in standalone ABP Modules (e.g., `modules/Academic`, `modules/LMS`).
    - Modules must be designed to be potentially separable (microservice-ready) in the future.
    - Modules interact via **Contracts** (Interfaces/DTOs) and Events, never direct database joining across module distinct contexts unless explicitly architected via Shared Kernel.
- **Themes (`themes/`)**: UI styling and layout logic belong in Theme Modules. `src/DATERP.Web` is just the host to mount the theme.

## 2. Directory Structure Rules
- `src/`: **Infrastructure Layer**.
    - `DATERP.Domain.Shared`: Enums, Consts shared globally (e.g., Roles).
    - `DATERP.HttpApi.Host`: The main API entry point.
    - `DATERP.Web`: This is the **Application Shell**. It hosts modules but contains little UI logic itself.
- `modules/`: **Business Layer**.
    - `Academic`: Management of Classes, Schedules, Students, Centers.
    - `LMS`: Courses, Lessons, Enrollments, Progress tracking.
    - `Examination`: Tests, Questions, Results, Certificates.
    - `Finance`, `HR`, `CRM`, `Inventory`, `Reporting`: Respective domains.
- `themes/`: **Presentation Layer**.
    - `DATERP.Theme.Education`: Main theme for the education platform.

## 3. Technology Stack & Standards
- **Framework**: .NET 9.0 + ABP Framework (v9.x).
- **Frontend**: ASP.NET Core MVC (Razor Views) + Standard ABP UI libraries.
- **Database**: SQL Server (Entity Framework Core).
- **Identity**: ABP Identity Module (Roles: Admin, Teacher, Student, Staff, Accountant).

## 4. Coding & Implementation Guidelines
- **Inheritance**: Always inherit from ABP base classes:
    - `ApplicationService` (App Layer)
    - `DomainService` (Domain Logic)
    - `AggregateRoot<Guid>` (Entities)
    - `AbpModule` (Module Configuration)
- **Dependency Injection**:
    - Use `[DependsOn]` attribute for module dependencies.
    - Use Constructor Injection.
- **Data Transfer**: Always use DTOs for Application Service input/output. Never expose Entities directly to the API.
- **Localization**: All user-facing text must be localizable.
- **Versioning**: Maintain package consistency (currently v9.0.0 due to .NET 9 environment).

## 5. Workflow Trigger
- If asked to "Add a feature" (e.g., "Add student management"):
    1.  Identify the correct module (e.g., `modules/Academic`).
    2.  If the module doesn't exist structurally, initialize it first.
    3.  Implement implementing Domain -> EF Core -> Application -> Web layers within that module folder.
    4.  **DO NOT** add it to `src/DATERP.Application`.

## 6. Agent Behavior
- **Proactive Check**: Before implementing code, verify which module directory defines the context.
- **Refactoring**: If you see business logic in `src/`, propose moving it to `modules/`.