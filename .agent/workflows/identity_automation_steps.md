---
description: Khởi động và chạy automation scripts từ thư mục .agent/automation
---

# Start Automation

Workflow này khởi động DATERP.Web server và chạy các automation scripts.

## Prerequisites
- Node.js đã được cài đặt
- Chrome browser đã được cài đặt
- .NET SDK đã được cài đặt

## Steps

// turbo
1. Dọn dẹp các tiến trình cũ (server, Chrome, ChromeDriver):
```powershell
Write-Host "Cleaning up old processes..." -ForegroundColor Yellow
taskkill /F /IM dotnet.exe /T 2>$null
taskkill /F /IM chromedriver.exe /T 2>$null
Start-Sleep -Seconds 2
Write-Host "Cleanup done." -ForegroundColor Green
```

// turbo
2. Build DATERP.Web project:
```powershell
Write-Host "Building DATERP.Web..." -ForegroundColor Yellow
dotnet build C:\Users\QuocDat-PC\Documents\GitHub\DATERP\src\DATERP.Web\DATERP.Web.csproj --verbosity quiet
Write-Host "Build completed." -ForegroundColor Green
```

// turbo
3. Khởi động DATERP.Web server (background):
```powershell
Write-Host "Starting DATERP.Web server..." -ForegroundColor Yellow
Start-Process dotnet -ArgumentList "run --project C:\Users\QuocDat-PC\Documents\GitHub\DATERP\src\DATERP.Web\DATERP.Web.csproj --no-build" -WindowStyle Hidden
Write-Host "Waiting for server to start (15 seconds)..." -ForegroundColor Cyan
Start-Sleep -Seconds 15
Write-Host "Server should be running at http://localhost:5223" -ForegroundColor Green
```

// turbo
4. Kiểm tra và cài đặt Node.js dependencies nếu chưa có:
```powershell
cd C:\Users\QuocDat-PC\Documents\GitHub\DATERP\.agent\automation
if (-not (Test-Path "node_modules")) {
    Write-Host "Installing dependencies..." -ForegroundColor Yellow
    npm install
} else {
    Write-Host "Dependencies already installed." -ForegroundColor Green
}
```

// turbo
5. Chạy automation script chính (verify_identity_page.js):
```powershell
cd C:\Users\QuocDat-PC\Documents\GitHub\DATERP\.agent\automation
Write-Host "Running identity verification script..." -ForegroundColor Yellow
node verify_identity_page.js
```

## Available Scripts

| Script | Mô tả |
|--------|-------|
| `auto_login.js` | Tự động đăng nhập và kiểm tra UI |
| `verify_identity_page.js` | Xác minh trang Identity Users |

## Chạy script khác

Để chạy script khác, thay thế bước 5 bằng:
```powershell
node verify_identity_page.js
```

## Notes
- Workflow sẽ tự động build và khởi động server trước khi chạy automation
- Browser sẽ giữ mở sau khi automation hoàn tất để kiểm tra thủ công
- Kiểm tra console output để xem lỗi hoặc cảnh báo
- Server chạy ở chế độ background (Hidden Window)
