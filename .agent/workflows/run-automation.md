---
description: Run Selenium automation script for login and UI verification
---

# Run Automation Script

This workflow runs the Selenium automation script to test login and verify the Identity Users page.

## Prerequisites
- Node.js installed
- Chrome browser installed
- DATERP.Web server running on http://localhost:5223

## Steps

// turbo
1. Kill any existing Chrome instances to avoid conflicts:
```powershell
taskkill /F /IM chrome.exe /T 2>$null
taskkill /F /IM chromedriver.exe /T 2>$null
```

// turbo
2. Wait 2 seconds for processes to terminate:
```powershell
Start-Sleep -Seconds 2
```

// turbo
3. Run the automation script:
```powershell
cd C:\Users\QuocDat-PC\Documents\GitHub\DATERP\.agent\automation
node auto_login.js
```

## Expected Output
- Login successful message
- Users Details Table found
- index.js reference found in HTML
- Browser console logs showing JS initialization

## Notes
- Browser will remain open after automation completes for manual inspection
- Check the console output for any errors or warnings
