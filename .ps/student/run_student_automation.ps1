# Student Dashboard Automation Script
# Tự động khởi động server và chạy verification cho Student Dashboard

# 1. Dọn dẹp các tiến trình cũ (server, Chrome, ChromeDriver)
Write-Host "Cleaning up old processes..." -ForegroundColor Yellow
taskkill /F /IM dotnet.exe /T 2>$null
taskkill /F /IM chromedriver.exe /T 2>$null
Start-Sleep -Seconds 2
Write-Host "Cleanup done." -ForegroundColor Green

# 2. Build DATERP.Web project
Write-Host "Building DATERP.Web..." -ForegroundColor Yellow
dotnet build C:\Users\QuocDat-PC\Documents\GitHub\DATERP\src\DATERP.Web\DATERP.Web.csproj --verbosity quiet
Write-Host "Build completed." -ForegroundColor Green

# 3. Khởi động DATERP.Web server (background)
Write-Host "Starting DATERP.Web server..." -ForegroundColor Yellow
Start-Process dotnet -ArgumentList "run --project C:\Users\QuocDat-PC\Documents\GitHub\DATERP\src\DATERP.Web\DATERP.Web.csproj --no-build" -WindowStyle Hidden
Write-Host "Waiting for server to start (15 seconds)..." -ForegroundColor Cyan
Start-Sleep -Seconds 15
Write-Host "Server should be running at http://localhost:5223" -ForegroundColor Green

# 4. Di chuyển đến thư mục automation
Set-Location C:\Users\QuocDat-PC\Documents\GitHub\DATERP\.agent\automation\student

# 5. Chạy automation script cho Student Dashboard
Write-Host "Running Student Dashboard verification script..." -ForegroundColor Yellow
node verify_student_dashboard.js

# Wait for user input before closing
Write-Host "Automation script finished." -ForegroundColor Cyan
Read-Host -Prompt "Press Enter to exit"
