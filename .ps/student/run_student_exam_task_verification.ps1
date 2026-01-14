# Stop any existing DATERP process to free up ports/files
Write-Host "=== Step 1: Cleaning up old processes ===" -ForegroundColor Cyan
Get-Process -Name "DATERP.Web" -ErrorAction SilentlyContinue | Stop-Process -Force
Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Where-Object { $_.CommandLine -like "*DATERP.Web*" } | Stop-Process -Force -ErrorAction SilentlyContinue
Get-Process -Name "chrome" -ErrorAction SilentlyContinue | Stop-Process -Force
Get-Process -Name "chromedriver" -ErrorAction SilentlyContinue | Stop-Process -Force
Get-Process -Name "node" -ErrorAction SilentlyContinue | Stop-Process -Force
Start-Sleep -Seconds 2
Write-Host "Cleanup completed." -ForegroundColor Green

# Define paths
$projectPath = "C:\Users\QuocDat-PC\Documents\GitHub\DATERP\src\DATERP.Web"
$verificationScript = "C:\Users\QuocDat-PC\Documents\GitHub\DATERP\.agent\automation\student\verify_student_exam_task.js"
$serverUrl = "http://localhost:5223"

# Build the project
Write-Host "`n=== Step 2: Building DATERP.Web ===" -ForegroundColor Cyan
dotnet build $projectPath

if ($LASTEXITCODE -ne 0) {
    Write-Error "Build FAILED. Exiting."
    exit 1
}
Write-Host "Build completed successfully." -ForegroundColor Green

# Start the server in background
Write-Host "`n=== Step 3: Starting Server ===" -ForegroundColor Cyan
$serverProcess = Start-Process -FilePath "dotnet" -ArgumentList "run --project $projectPath" -PassThru -NoNewWindow
Write-Host "Server process started (PID: $($serverProcess.Id))"

# Wait for server to be ready with health check
Write-Host "`n=== Step 4: Waiting for server to be ready ===" -ForegroundColor Cyan
$maxWaitSeconds = 120
$waitInterval = 5
$elapsed = 0
$serverReady = $false

while ($elapsed -lt $maxWaitSeconds) {
    try {
        $response = Invoke-WebRequest -Uri $serverUrl -TimeoutSec 5 -UseBasicParsing -ErrorAction Stop
        if ($response.StatusCode -eq 200 -or $response.StatusCode -eq 302) {
            $serverReady = $true
            break
        }
    }
    catch {
        Write-Host "  Waiting... ($elapsed s elapsed)"
    }
    Start-Sleep -Seconds $waitInterval
    $elapsed += $waitInterval
}

if (-not $serverReady) {
    Write-Error "Server did not start within $maxWaitSeconds seconds. Exiting."
    Stop-Process -Id $serverProcess.Id -Force -ErrorAction SilentlyContinue
    exit 1
}
Write-Host "Server is READY at $serverUrl (took $elapsed seconds)" -ForegroundColor Green

# Run verification
Write-Host "`n=== Step 5: Running Selenium Verification for Exam Tasks ===" -ForegroundColor Cyan
node $verificationScript

if ($LASTEXITCODE -ne 0) {
    Write-Host "Verification script completed with errors." -ForegroundColor Yellow
}
else {
    Write-Host "Verification script completed successfully." -ForegroundColor Green
}

# Keep server running for inspection
Write-Host "`n=== Done ===" -ForegroundColor Cyan
Write-Host "Server is still running. Browser should be open for inspection."
