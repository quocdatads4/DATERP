# 🎓 Education Theme for DATERP

![Version](https://img.shields.io/badge/version-1.0.0-blue.svg)
![Framework](https://img.shields.io/badge/ABP%20Framework-9.0-purple.svg)
![Status](https://img.shields.io/badge/status-Active-success.svg)

## 📖 Overview

The **Education Theme** is a custom UI theme designed for the **DATERP** platform, specifically tailored for educational management contexts (Schools, Exam Centers, LMS). It extends the **ABP Framework** theming system to provide a clean, student-friendly interface.

### ✨ Key Features

-   **🎨 Custom Design**: Tailored "Education" look and feel for students and teachers.
-   **🧩 Modular Architecture**: Built as a standard ABP Module (`EducationThemeModule`).
-   **📱 Responsive Layouts**: Fully responsive layouts for Desktop, Tablet, and Mobile.
-   **🌍 Localization**: Built-in support for multiple languages (VN/EN).
-   **⚡ Bundling & Minification**: Optimized asset delivery via ABP Bundling.

---

## 🏗️ Architecture

### 📂 Folder Structure

```
themes/Education/
├── 📂 Bundling/          # CSS/JS Bundling Contributors
├── 📂 Localization/      # JSON Localization files (en, vn)
├── 📂 Pages/             # Custom Pages (overrides)
├── 📂 Themes/            # Core Theme Files
│   └── 📂 Education/
│       ├── 📂 Components/# Shared UI Components
│       └── 📂 Layouts/   # Master Layouts (Application, Account)
├── 📄 EducationTheme.cs  # Main Theme Implementation
└── 📄 EducationThemeModule.cs # Infrastructure Configuration
```

### 🔄 Theme Resolution Flow

```mermaid
graph TD
    A[User Request] -->|Http Request| B(ABP Framework)
    B -->|Resolve Theme| C{Theme Manager}
    C -->|Selects| D[EducationTheme]
    D -->|GetLayout()| E{Layout Type}
    E -->|Application| F[Layouts/Application.cshtml]
    E -->|Account| G[Layouts/Account.cshtml]
    E -->|Empty| H[Layouts/Empty.cshtml]
    F & G & H -->|Injects| I[Global Styles/Scripts]
    I --> J[Final HTML Response]
```

## 🛠️ Components

### 1. Theme Module (`EducationThemeModule.cs`)
Registers the theme with ABP's dependency injection and configures:
-   **Virtual File System**: Maps embedded resources.
-   **Theming Options**: Sets `EducationTheme` as default.
-   **Bundling**: Injects `Education.Global.Styles` and `Education.Global.Scripts`.

### 2. Layouts
-   **Application**: Main dashboard layout with sidebar and navbar.
-   **Account**: Minimal layout for Login/Register pages.
-   **Empty**: Blank canvas for special pages.

### 3. Bundling
Located in `Bundling/`, control which CSS/JS files are loaded globally.
-   `EducationThemeGlobalStyleContributor`: Bootstrap, FontAwesome, Custom CSS.
-   `EducationThemeGlobalScriptContributor`: jQuery, ABP Utils, Custom JS.

## 🚀 Usage

To use this theme in your ABP Web Project:

1.  **Add Reference**:
    Ensure your Web project references the `Education` project.

2.  **Add Module Dependency**:
    In your `WebModule.cs`:
    ```csharp
    [DependsOn(typeof(EducationThemeModule))]
    public class YourProjectWebModule : AbpModule { ... }
    ```

3.  **Run**:
    The system will automatically resolve `EducationTheme` as the default theme.

## 🎨 Customization

### Modifying Styles
Edit files in `wwwroot/css/` or add new SCSS files. Ensure they are added to the `BundleContributor`.

### Overriding Views
Place standard ABP views in `Pages/` or `Views/` to override default implementation (e.g., `Themes/Education/Layouts/Application.cshtml`).

---
Based on **ABP Framework** • Built for **DATERP**
