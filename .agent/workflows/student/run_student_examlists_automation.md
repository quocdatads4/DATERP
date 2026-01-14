---
description: Chạy automation để xác minh luồng Student truy cập Exam Lists (run_exam_list_verification.ps1)
---

# Workflow: Student Exam Lists Verification

Script này xây dựng DATERP.Web, khởi động server, và chạy Selenium để xác minh luồng Student truy cập trang Exam Lists.

## Các bước thực hiện

1. **Dọn dẹp tiến trình cũ**: Script tự động dừng các tiến trình `DATERP.Web`, `dotnet`, `chrome`, `chromedriver`, và `node` đang chạy.

2. **Build dự án**:
   ```powershell
   dotnet build C:\Users\QuocDat-PC\Documents\GitHub\DATERP\src\DATERP.Web
   ```

3. **Khởi động server**: Server sẽ chạy ở background trên `http://localhost:5223`.

4. **Chờ server khởi động**: Script chờ 30 giây để đảm bảo server sẵn sàng.

5. **Chạy Selenium Verification**: Script Node.js `verify_student_exam_list.js` sẽ:
   - Đăng nhập với tài khoản Student
   - Điều hướng đến trang Exam Subjects
   - Click vào nút "Truy cập bài làm" (Access Exam)
   - Xác minh trang Exam Lists hiển thị đúng thông tin môn học và danh sách bài thi

## Chạy Workflow

// turbo-all
```powershell
powershell -ExecutionPolicy Bypass -File C:\Users\QuocDat-PC\Documents\GitHub\DATERP\.ps\student\run_exam_list_verification.ps1
```

## Files liên quan
- **PowerShell Runner**: `.ps\student\run_exam_list_verification.ps1`
- **Selenium Script**: `.agent\automation\student\verify_student_exam_list.js`
