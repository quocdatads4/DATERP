---
description: Chạy automation để xác minh luồng Student truy cập Exam Tasks (run_student_exam_task_verification.ps1)
---

# Workflow: Student Exam Tasks Verification

Script này xây dựng DATERP.Web, khởi động server, và chạy Selenium để xác minh luồng Student truy cập trang Exam Tasks (Details bài thi).

## Các bước thực hiện

1. **Dọn dẹp tiến trình cũ**: Script tự động dừng các tiến trình `DATERP.Web`, `dotnet`, `chrome`, `chromedriver`, và `node` đang chạy.

2. **Build dự án**:
   ```powershell
   dotnet build C:\Users\QuocDat-PC\Documents\GitHub\DATERP\src\DATERP.Web
   ```

3. **Khởi động server**: Server sẽ chạy ở background trên `http://localhost:5223`.

4. **Chờ server khởi động**: Script chờ 120 giây (tối đa) để đảm bảo server sẵn sàng.

5. **Chạy Selenium Verification**: Script Node.js `verify_student_exam_task.js` sẽ:
   - Đăng nhập với tài khoản Student
   - Điều hướng đến trang Exam Subjects -> Exam Lists
   - Click vào một bài thi để vào trang chi tiết (Tasks/Projects)
   - Xác minh trang Exam Tasks hiển thị đúng

## Chạy Workflow

// turbo-all
```powershell
powershell -ExecutionPolicy Bypass -File C:\Users\QuocDat-PC\Documents\GitHub\DATERP\.ps\student\run_student_exam_task_verification.ps1
```

## Files liên quan
- **PowerShell Runner**: `.ps\student\run_student_exam_task_verification.ps1`
- **Selenium Script**: `.agent\automation\student\verify_student_exam_task.js`
