
Write-Host "Cleaning up old processes..."
Get-Process chrome, chromedriver, node -ErrorAction SilentlyContinue | Where-Object { $_.StartTime -lt (Get-Date).AddMinutes(-5) } | Stop-Process -Force

$projectPath = "src/DATERP.Web"
Write-Host "Building DATERP.Web..."
dotnet build $projectPath

Write-Host "Starting DATERP.Web server..."
$process = Start-Process "dotnet" -ArgumentList "run --project $projectPath" -PassThru -NoNewWindow
Start-Sleep -Seconds 45

Write-Host "Server should be running at http://localhost:5223"

Write-Host "Running roles verification script..."
node .agent/automation/admin/verify_identity_roles.js

Write-Host "Automation script finished."

