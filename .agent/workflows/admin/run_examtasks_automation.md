---
description: Chạy automation để truy cập trang ExamTasks (run_examtasks_automation.ps1)
---

# Run ExamTasks Automation

Workflow này tự động khởi động hệ thống DATERP và mở trang **Nhiệm vụ (ExamTasks)**.

## Các bước thực hiện

// turbo
1. Chạy script automation:
```powershell
.\.ps\admin\run_examtasks_automation.ps1
```

## Nội dung thực hiện
Script sẽ thực hiện:
- Build code mới nhất của DATERP.Web.
- Khởi động server.
- Đăng nhập Admin.
- Truy cập `/Examination/ExamTasks`.
- Kiểm tra DataTable với các cột: Nội dung, Tên đề thi, Điểm, Thứ tự.

> [!NOTE]
> Kết quả sẽ hiển thị trong terminal và log file.
