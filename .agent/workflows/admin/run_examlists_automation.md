---
description: Chạy automation để truy cập trang ExamLists (run_examlists_automation.ps1)
---

# Run ExamLists Automation

Workflow này tự động khởi động hệ thống DATERP và mở trang **Danh sách bài thi (ExamLists)**.

## Các bước thực hiện

// turbo
1. Chạy script automation:
```powershell
.\.ps\admin\run_examlists_automation.ps1
```

## Nội dung thực hiện
Script sẽ thực hiện:
- Build code mới nhất của DATERP.Web.
- Khởi động server.
- Mở Chrome Incognito và truy cập `/Examination/ExamLists`.
- Chờ bạn nhấn Enter để dừng server.

> [!NOTE]
> Trang ExamLists sẽ hiển thị danh sách các bài thi với các cột: Tiêu đề, Môn thi, Thời gian, Thứ tự.
