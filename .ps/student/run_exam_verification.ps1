# Student Exam Verification
Write-Host "Cleaning up old processes..." -ForegroundColor Yellow
taskkill /F /IM dotnet.exe /T 2>$null
taskkill /F /IM chromedriver.exe /T 2>$null
Start-Sleep -Seconds 2

Write-Host "Building DATERP.Web..." -ForegroundColor Yellow
dotnet build C:\Users\QuocDat-PC\Documents\GitHub\DATERP\src\DATERP.Web\DATERP.Web.csproj --verbosity quiet

Write-Host "Starting Server..." -ForegroundColor Yellow
Start-Process dotnet -ArgumentList "run --project C:\Users\QuocDat-PC\Documents\GitHub\DATERP\src\DATERP.Web\DATERP.Web.csproj --no-build" -WindowStyle Hidden
Write-Host "Waiting 15s for server..."
Start-Sleep -Seconds 15

Set-Location C:\Users\QuocDat-PC\Documents\GitHub\DATERP\.agent\automation\student
Write-Host "Running Verification JS..." -ForegroundColor Yellow
node verify_student_exam_page.js

Write-Host "Done." -ForegroundColor Green
