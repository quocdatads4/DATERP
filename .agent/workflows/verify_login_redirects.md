---
description: Verify login and role-based redirects for Admin, Student, and Teacher accounts.
---

1. **Prerequisite**: Ensure the `DATERP.Web` application is running on `http://localhost:5223`.

2. **Verify Administrator Access**:
   - Navigate to `http://localhost:5223/Account/Login`
   - Login with:
     - Username: `admin@datacademy.edu.vn`
     - Password: `Admin@123`
   - **Expectation**: Redirected to `/admin/dashboard`.
   - Action: Logout.

3. **Verify Student Access**:
   - Navigate to `http://localhost:5223/Account/Login`
   - Login with:
     - Username: `student@datacademy.edu.vn`
     - Password: `Student@123`
   - **Expectation**: Redirected to `/student/dashboard`.
   - Action: Logout.

4. **Verify Teacher Access**:
   - Navigate to `http://localhost:5223/Account/Login`
   - Login with:
     - Username: `teacher@datacademy.edu.vn`
     - Password: `Teacher@123`
   - **Expectation**: Redirected to `/teacher/dashboard`.
   - Action: Logout.
