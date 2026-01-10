---
description: Verify login for different account types (Administrator, Teacher, Student) using Selenium
---

// turbo-all
1. Ensure the web server is running (`dotnet run` in `src/DATERP.Web`).
2. Run the automation script for each role:
    - **Verify Administrator:** `node .agent/automation/auto_login.js admin`
    - **Verify Teacher:** `node .agent/automation/auto_login.js teacher`
    - **Verify Student:** `node .agent/automation/auto_login.js student`

> [!NOTE]
> The script will wait for a redirect or output specific validation/alert errors if login fails.
