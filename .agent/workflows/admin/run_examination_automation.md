---
description: Chạy automation testing và truy cập nhanh vào module Examination (run_examination_automation.ps1)
---

# Run Examination Automation

Workflow này tự động thực hiện các bước để khởi động hệ thống DATERP và kiểm tra khả năng truy cập vào module **Examination** (API và Swagger).

## Các bước thực hiện

// turbo
1. Chạy script automation:
```powershell
.\.ps\admin\run_examination_automation.ps1
```

## Nội dung kiểm tra
Script sẽ thực hiện:
- Build code mới nhất của module Examination.
- Khởi động server DATERP.Web.
- Sử dụng Selenium để mở Chrome.
- Kiểm tra trang **Swagger UI** để xác nhận các API của Examination đã được đăng ký thành công:
    - `ExamSubject`
    - `ExamList`
    - `ExamProject`
    - `ExamTask`
- Đăng nhập vào hệ thống để kiểm tra trạng thái hoạt động.

> [!NOTE]
> Kết quả sẽ hiển thị trực tiếp trong cửa sổ terminal. Trình duyệt Chrome sẽ được giữ nguyên sau khi chạy xong để bạn có thể kiểm tra thủ công các endpoint.
