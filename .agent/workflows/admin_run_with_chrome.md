---
description: Build & Run DATERP.Web with Selenium Automation (XPath)
---
1. [Setup] Install Node.js Dependencies (One-time)
// turbo
cd .agent/automation
npm install

2. [Build] Build DATERP.Web
// turbo
dotnet build src/DATERP.Web/DATERP.Web.csproj

3. [Run] Start Web Server
// turbo
Start-Process dotnet -ArgumentList "run --project src/DATERP.Web/DATERP.Web.csproj --property:WarningLevel=0" -WindowStyle Hidden
Write-Host "Waiting for server..."
Start-Sleep -Seconds 10

4. [Auto-Login] Run Selenium Script
// turbo
cd .agent/automation
node auto_login.js
