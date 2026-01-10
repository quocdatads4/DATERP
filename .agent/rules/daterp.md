---
trigger: always_on
---

📘 DATERP – ABP INTERNAL GUIDELINES

Architecture · Module Design · Educational Agent Behavior

1. Purpose & Scope

This document defines the mandatory internal standards for building, extending, and operating DATERP, an ABP-based Modular ERP Platform focused on:

Education systems (Schools, Language Centers, IT Training Centers – IC3, MOS)

Enterprise Resource Planning (ERP)

AI-powered Educational Agents (Google Antigravity)

These guidelines apply to:

Backend Developers

Frontend Developers

Business Analysts (BA)

Product Owners

AI / Prompt Engineers

2. Core Architectural Philosophy
2.1 Architecture Model

ABP Modular Monolith (STRICT)

Module-first design, microservice-ready by default

No accidental coupling between domains is allowed

❗ Any violation of module boundaries is considered a design defect, not a temporary workaround.

3. Layer Responsibilities (Mandatory)
3.1 src/ – Infrastructure Layer

Purpose: Hosting, shared abstractions, and cross-cutting concerns.

Allowed:

Hosting applications

Shared enums, constants, permissions

Authentication / Authorization

Technical and infrastructure configuration

Forbidden:

Business rules

Educational logic

Examination rules

Course-related logic

Domain-specific workflows

3.2 modules/ – Business Layer (Core Value)

Purpose: All domain logic must reside here.

Each module:

Owns its Domain

Owns its Database Context

Owns its Application Services

Must be independently evolvable

Example modules:

Academic

LMS

Examination

Finance

HR

CRM

Inventory

Reporting

Modules must communicate only via:

DTOs / Interfaces (Contracts)

Domain Events

Shared Kernel (only when explicitly justified)

❌ Direct database joins across modules are strictly forbidden, except for explicitly designed reporting or read-model purposes.

3.3 themes/ – Presentation Layer

Purpose: UI, layout, and branding.

Rules:

UI logic belongs to Theme modules

DATERP.Web acts only as the application shell

No business decisions are allowed in Razor Views

4. Technology & Coding Standards
4.1 Technology Stack

.NET 9.0

ABP Framework v9.x

ASP.NET Core MVC (Razor)

SQL Server + Entity Framework Core

ABP Identity Module

4.2 Mandatory Base Classes

Always inherit from:

AbpModule

AggregateRoot<Guid>

DomainService

ApplicationService

4.3 Data Exposure Rules

DTOs are mandatory

Entities must NEVER be exposed via APIs

Mapping must be done via AutoMapper or explicit, well-defined manual mapping

4.4 Dependency Injection

Constructor Injection only

Use [DependsOn] to declare module dependencies

5. Feature Development Workflow (SOP)

When implementing any feature:

Identify the correct Business Module

If the module does not exist → create the module structure first

Implement strictly in the following order:

Domain layer

EF Core layer

Application layer

Web/UI layer (inside the module)

Validate that:

No business logic leaks into src/

No cross-module coupling exists

If business logic is found in src/, refactoring must be proposed immediately.

6. Educational Domain Principles (IC3 / MOS)
6.1 Target Learners

Vietnamese learners:

Grade 6–9

Grade 10–12

College / vocational students

Skill level: Beginner → Intermediate

6.2 Educational ABP Framework

ABP = Adaptive – Beginner-friendly – Practice-first

Adaptive: Difficulty adjusts based on learner interaction

Beginner-friendly: Always explain from fundamentals when necessary

Practice-first: Real-world practice takes precedence over theory

6.3 Examination Orientation

Prioritize real IC3 / MOS exam scenarios

Focus on:

Task-based questions

Common mistakes

Time-saving techniques

7. AI / Agent Behavioral Guidelines (Antigravity)
7.1 Language & Tone

Vietnamese language only

Simple, clear, and supportive

Avoid academic or intimidating language

Never use judgmental or discouraging expressions

7.2 Teaching Behavior

Step-by-step explanations

Use:

Bullet points

Numbered steps

Office software examples (Word, Excel, PowerPoint)

7.3 Adaptation Rules

If the learner is confused → simplify

If the learner understands → summarize and extend

Light clarification questions are allowed (grade level, learning module)

8. Restrictions & Quality Control

The system MUST NOT:

Assume advanced technical knowledge

Overload learners with excessive theory

Use harsh or discouraging language

Mix unrelated domains within a single module

All PRs and features must be reviewed against:

Module boundary compliance

ABP principles

Educational suitability

9. Final Objective

DATERP exists to:

Deliver a clean, scalable ABP architecture

Enable modular enterprise growth

Support effective education for Vietnamese learners

Improve IC3 / MOS learning outcomes through practice-first design

📌 Internal Rule of Thumb

If it smells like business logic, it belongs in modules/.
If it smells like UI logic, it belongs in themes/.
If it smells confusing to students, simplify it.