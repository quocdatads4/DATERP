---
description: Quy trình tiêu chuẩn để triển khai các tính năng mới trong DATERP
---

# Quy Trình Phát Triển Tính Năng Mới

Quy trình này thực thi vòng lặp "Suy Nghĩ-Hành Động-Xác Minh" (Think-Act-Verify) và các nguyên tắc OpenSpec.

## Giai Đoạn 1: Lập Kế Hoạch & Đề Xuất (Suy Nghĩ)
1.  **Đọc Quy Tắc**: Xem lại `.antigravity/rules.md`.
2.  **Soạn Thảo Đề Xuất**: Copy `.agent/templates/feature_proposal.md` vào `implementation_plan.md` (Artifact).
3.  **Định Nghĩa Docs**: Điền vào phần `Requirements` sử dụng các kịch bản `KHI/THÌ` (WHEN/THEN).
4.  **Thống Nhất**: Sử dụng `notify_user` để trình bày kế hoạch và nhận sự chấp thuận. **Chưa được viết code vào lúc này.**

## Giai Đoạn 2: Triển Khai (Hành Động)
1.  **Thực Thi Task**: Làm theo danh sách kiểm tra (checklist) trong kế hoạch của bạn.
2.  **Phân Lớp Nghiêm Ngặt**:
    *   **Domain Trước**: Định nghĩa Entities và Business Logic.
    *   **App Sau**: Phơi bày qua DTOs và AppServices.
    *   **UI Cuối Cùng**: Kết nối UI với AppServices.
3.  **Không Rò Rỉ Logic**: Nếu bạn thấy mình đang viết các quy tắc nghiệp vụ `if/else` trong Controller hoặc View, **DỪNG LẠI** và chuyển nó vào Domain Manager.

## Giai Đoạn 3: Xác Minh (Phản Chiếu)
1.  **Build**: Chạy `dotnet build`.
2.  **Tự Động Hóa**: Kiểm tra xem script tự động hóa có tồn tại trong `.agent/automation` không.
    *   *Nếu có*: Chạy nó (ví dụ: `/run_examination_automation`).
    *   *Nếu không*: Tạo một script kiểm thử cơ bản nếu tính năng quan trọng.
3.  **Log**: Ghi lại kết quả.

## Giai Đoạn 4: Bàn Giao
1.  **Walkthrough**: Tạo `walkthrough.md` với ảnh chụp màn hình/log.
2.  **Thông Báo**: Báo cho người dùng biết tính năng đã sẵn sàng.
