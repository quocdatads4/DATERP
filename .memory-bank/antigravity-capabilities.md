# Khả năng của Antigravity Agent đối với dự án DATERP

Với cấu hình hiện tại (đã nạp "bộ não" DATERP vào Antigravity Workspace), Agent này có thể hoạt động như một **"Lập trình viên Senior ABP"** ảo để hỗ trợ dự án.

Dưới đây là 5 nhóm công việc cụ thể mà Agent này có thể thực hiện:

## 1. 🏗️ Tự Động Hóa Code ("Scaffolding")
Thay vì copy-paste code mẫu thủ công, bạn có thể ra lệnh:
> *"Tạo một Module mới tên là 'Inventory' với Entity 'Product' (Name, SKU, Price). Đảm bảo tuân thủ cấu trúc ABP Modular Monolith."*

Agent sẽ:
*   Tự động tạo cây thư mục `modules/Inventory`.
*   Tạo các file chuẩn DDD: `Product.cs`, `InventoryDbContext`, `ProductAppService`, `ProductDto`.
*   Đăng ký module vào hệ thống.

## 2. 🧹 Refactoring & Kiến Trúc
DATERP có quy tắc rất nghiêm ngặt (không business logic trong `src/`). Agent có thể giúp:
> *"Kiểm tra thư mục src/DATERP.Application xem có logic nghiệp vụ nào bị đặt nhầm chỗ không. Nếu có, hãy đề xuất kế hoạch di chuyển sang module tương ứng."*

Hoặc:
> *"Review Entity 'Student' trong module Academic và đảm bảo nó kế thừa đúng AggregateRoot."*

## 3. 📝 Viết Tài Liệu & Swagger
Bạn có thể yêu cầu Agent đọc code và sinh tài liệu:
> *"Viết file README.md cho module Examination, giải thích luồng dữ liệu của việc tạo bài thi."*
> *"Tạo XML comments cho toàn bộ ExamTaskController để hiển thị đẹp trên Swagger."*

## 4. 🧪 Tạo Unit Test
Nếu bạn lười viết test:
> *"Viết Unit Test cho hàm CalculateGPA trong AcademicAppService. Bao gồm các case điểm biên."*

## 5. 🔍 Phân Tích & Gỡ Lỗi (Debugging)
Khi gặp lỗi khó hiểu của ABP Framework:
> *"Tôi gặp lỗi 'Repository not registered' khi gọi ExamRepository. Hãy kiểm tra xem tôi đã thêm module dependency trong ExaminationModule.cs chưa?"*

---

## 💡 Cách thực hiện ngay
Bạn chỉ cần mở Terminal tại folder `antigravity-workspace-template` và chạy:

```powershell
python src/agent.py "Quét qua folder C:\Users\QuocDat-PC\Documents\GitHub\DATERP\modules\Examination và kiểm tra xem các Controller đã kế thừa đúng chuẩn ABP chưa"
```
