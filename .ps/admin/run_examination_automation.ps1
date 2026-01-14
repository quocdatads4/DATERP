# Script to build and run DATERP then verify Examination Module

Write-Host "=== RUNNING EXAMINATION MODULE AUTOMATION ===" -ForegroundColor Cyan

# 1. Cleanup
Write-Host "Step 1: Cleaning up old processes..." -ForegroundColor Yellow
taskkill /F /IM dotnet.exe /T 2>$null
taskkill /F /IM chromedriver.exe /T 2>$null
Start-Sleep -Seconds 2

# 2. Build
Write-Host "Step 2: Building DATERP.Web..." -ForegroundColor Yellow
dotnet build C:\Users\QuocDat-PC\Documents\GitHub\DATERP\src\DATERP.Web\DATERP.Web.csproj --verbosity quiet
if ($LASTEXITCODE -ne 0) {
    Write-Host "Build FAILED!" -ForegroundColor Red
    exit $LASTEXITCODE
}
Write-Host "Build successful." -ForegroundColor Green

# 3. Start Server
Write-Host "Step 3: Starting server..." -ForegroundColor Yellow
Start-Process dotnet -ArgumentList "run --project C:\Users\QuocDat-PC\Documents\GitHub\DATERP\src\DATERP.Web\DATERP.Web.csproj --no-build" -WindowStyle Hidden
$maxRetries = 60
$retryCount = 0
$serverReady = $false
$url = "http://localhost:5223"

Write-Host "Waiting for server to be ready at $url ..." -ForegroundColor Cyan

while ($retryCount -lt $maxRetries) {
    try {
        $response = Invoke-WebRequest -Uri $url -Method Head -UseBasicParsing -TimeoutSec 2 -ErrorAction Stop
        if ($response.StatusCode -eq 200) {
            $serverReady = $true
            Write-Host "Server is UP and READY!" -ForegroundColor Green
            break
        }
    }
    catch {
        Write-Host "." -NoNewline -ForegroundColor DarkGray
        Start-Sleep -Seconds 2
        $retryCount++
    }
}

if (-not $serverReady) {
    Write-Host "`nServer failed to start within timeout." -ForegroundColor Red
    exit 1
}

# 4. Run Automation
Write-Host "Step 4: Running Selenium automation..." -ForegroundColor Yellow
Set-Location C:\Users\QuocDat-PC\Documents\GitHub\DATERP\.agent\automation\admin
node verify_examination_module.js

Write-Host "=== AUTOMATION FINISHED ===" -ForegroundColor Green
Read-Host -Prompt "Press Enter to exit"
