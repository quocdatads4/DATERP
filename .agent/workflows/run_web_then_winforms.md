---
description: Run DATERP Web App and then WinForms App in correct order
---

# Run DATERP Web then WinForms

This workflow launches the DATERP.Web server, waits for it to become available on port 5223, and then launches the DATERP.WinForms application.

## Steps

// turbo
1. Run the orchestration script:
```powershell
powershell -ExecutionPolicy Bypass -File ./.ps/run_web_then_winforms.ps1
```
