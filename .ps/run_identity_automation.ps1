# Auto-generated from .agent/workflows/start-automation.md

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
Set-Location C:\Users\QuocDat-PC\Documents\GitHub\DATERP\.agent\automation
# (Dependencies đã được xác nhận cài đặt)


# 5. Chạy automation script chính (verify_identity_page.js)
Write-Host "Running identity verification script..." -ForegroundColor Yellow
node verify_identity_page.js

# Wait for user input before closing
Write-Host "Automation script finished." -ForegroundColor Cyan
Read-Host -Prompt "Press Enter to exit"
