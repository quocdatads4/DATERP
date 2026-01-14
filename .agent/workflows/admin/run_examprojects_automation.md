---
description: Chạy automation để truy cập trang ExamProjects (run_examprojects_automation.ps1)
---

# Run ExamProjects Automation

Workflow này tự động khởi động hệ thống DATERP và mở trang **Đề thi (ExamProjects)**.

## Các bước thực hiện

// turbo
1. Chạy script automation:
```powershell
.\.ps\admin\run_examprojects_automation.ps1
```

## Nội dung thực hiện
Script sẽ thực hiện:
- Build code mới nhất của DATERP.Web.
- Khởi động server.
- Đăng nhập Admin.
- Truy cập `/Examination/ExamProjects`.
- Kiểm tra DataTable với các cột: Tên đề thi, Bài thi, Hướng dẫn, Thứ tự.

> [!NOTE]
> Kết quả sẽ hiển thị trong terminal và log file.
