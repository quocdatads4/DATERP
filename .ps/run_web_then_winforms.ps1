$ErrorActionPreference = "Stop"

function Wait-ForUrl {
    param (
        [string]$Url,
        [int]$TimeoutSeconds = 120
    )

    Write-Host "Waiting for $Url to be ready..." -ForegroundColor Cyan
    $startTime = Get-Date

    while ((Get-Date) - $startTime -lt (New-TimeSpan -Seconds $TimeoutSeconds)) {
        try {
            $response = Invoke-WebRequest -Uri $Url -UseBasicParsing -Method Head -ErrorAction SilentlyContinue
            if ($response.StatusCode -eq 200) {
                Write-Host "Successfully connected to $Url" -ForegroundColor Green
                return $true
            }
        }
        catch {
            # Ignore and retry
        }
        Start-Sleep -Seconds 2
    }

    Write-Error "Timeout waiting for $Url"
    return $false
}

$webProject = Join-Path $PSScriptRoot "../src/DATERP.Web/DATERP.Web.csproj"
$winFormsProject = Join-Path $PSScriptRoot "../src/DATERP.WinForms/DATERP.WinForms.csproj"

# 1. Start Web App in background
Write-Host "Starting DATERP.Web..." -ForegroundColor Yellow
$webProcess = Start-Process -FilePath "dotnet" -ArgumentList "run --project `"$webProject`" --launch-profile http" -PassThru -NoNewWindow

# 2. Wait for Web App to be ready
$webUrl = "http://localhost:5223"
if (Wait-ForUrl -Url $webUrl) {
    # Extra delay to ensure Web server is fully initialized
    Write-Host "Web server responded. Waiting 5 seconds for full initialization..." -ForegroundColor Cyan
    Start-Sleep -Seconds 5
    Write-Host "Web server is FULLY READY!" -ForegroundColor Green
    
    # 3. Start WinForms App
    Write-Host "Starting DATERP.WinForms..." -ForegroundColor Yellow
    Start-Process -FilePath "dotnet" -ArgumentList "run --project `"$winFormsProject`" -- --auto-test" -Wait
}
else {
    Write-Error "Failed to start Web Application."
}

# Cleanup works if WinForms keeps the script alive with -Wait, 
# but if user closes Web console manually, we might want to kill Web process if this script exits?
# For now, let's leave Web running as intended often in dev scenarios.
# If strict cleanup is needed:
# try { ... } finally { Stop-Process -Id $webProcess.Id -ErrorAction SilentlyContinue }
