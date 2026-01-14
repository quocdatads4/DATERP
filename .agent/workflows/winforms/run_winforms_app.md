---
description: Build and Run DATERP WinForms Application (connects to running Web server)
---

# Run DATERP WinForms App

This workflow starts DATERP.Web server first, then launches the WinForms WebView2 wrapper.

## Prerequisites
The DATERP.Web server must be running on `http://localhost:5223`.

## Steps

// turbo
1. Start DATERP.Web server (if not already running):
```bash
Start-Process -NoNewWindow -FilePath "dotnet" -ArgumentList "run --project src/DATERP.Web/DATERP.Web.csproj"
```

// turbo
2. Wait for Web server to initialize:
```bash
Start-Sleep -Seconds 5
```

// turbo
3. Build and run WinForms application:
```bash
dotnet run --project src/DATERP.WinForms/DATERP.WinForms.csproj
```
