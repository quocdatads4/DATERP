# 🛡️ Quy Tắc Agent DATERP (v1.1 - ABP Strict)
Được chuyển thể từ Tiêu chuẩn Google Antigravity & ABP Framework Best Practices.

## 1. Triết Lý Cốt Lõi: Artifact-First (Ưu Tiên Hiện Vật)
**KHÔNG ĐƯỢC chỉ viết code.**
Đối với mọi tác vụ phức tạp, bạn PHẢI tạo một **Hiện Vật (Artifact)** trước.

### Giao thức:
1.  **Lập Kế Hoạch**: Tạo hoặc cập nhật `implementation_plan.md` trong thư mục artifact TRƯỚC KHI chạm vào `src/`.
2.  **Đề Xuất (OpenSpec)**: Đối với các Module MỚI hoặc Tính Năng Lớn, hãy tạo một Đề xuất ngắn gọn.
3.  **Bằng Chứng**: Khi xác minh, hãy thực thi kịch bản tự động hóa liên quan và kiểm tra log.

## 2. Tiêu Chuẩn ABP Framework (BẮT BUỘC TUÂN THỦ)

### 🧩 Domain Layer (Trái tim của hệ thống)
*   **Entities**:
    *   Sử dụng `AuditedAggregateRoot<Guid>` cho Root Entity.
    *   Sử dụng `Entity<Guid>` cho các bảng con.
    *   Hàm tạo (Constructor) phải `protected` hoặc `internal` để ép buộc dùng Manager.
*   **Domain Managers**: Chứa TẤT CẢ logic nghiệp vụ phức tạp.
    *   Ví dụ: Kiểm tra trùng lặp, tính toán điểm số.
    *   Code mẫu: `public async Task<Exam> CreateAsync(...)`
*   **Repository**:
    *   Chỉ tiêm (inject) `IRepository<TEntity, Guid>` vào Manager hoặc AppService.
    *   **TUYỆT ĐỐI CẤM** dùng `IQueryable` (Linq) trong UI/Controller.

### 🚀 Application Layer (Orchestration)
*   **DTOs**:
    *   Không được tái sử dụng Entity làm DTO.
    *   Input DTO và Output DTO phải tách biệt.
    *   Sử dụng `ObjectMapper` để chuyển đổi dữ liệu, không gán tay từng trường.
*   **AppServices**:
    *   Chỉ làm nhiệm vụ điều phối (gọi Manager, gọi Repository, map DTO).
    *   Không được chứa logic nghiệp vụ cốt lõi (Business Rules).
    *   Tất cả method phải là `virtual public async Task...`.

### 💾 Infrastructure (EF Core)
*   **Migrations**: Luôn chạy `dotnet ef migrations add [Name]` trong thư mục chứa `DbContextFactory` (thường là `EntityFrameworkCore`).
*   **Query Performance**: Chú ý vấn đề N+1, sử dụng `.Include()` khi cần thiết.

### 🌐 Frontend (MVC / Razor Pages)
*   **JavaScript Proxies**: Sử dụng Dynamic JavaScript Proxies của ABP (`daterp.controllers.examination...`). Tránh gọi `$.ajax` thủ công vào các URL cứng.
*   **Localization**: Mọi text hiển thị phải qua `L["Key"]`. Không hardcode chuỗi tiếng Việt/Anh trực tiếp trong file `.cshtml`.

## 3. Quy Trình Làm Việc: Suy Nghĩ-Hành Động-Xác Minh
1.  **Suy Nghĩ (Think)**: Phân tích yêu cầu. Kiểm tra `task.md` và `implementation_plan.md`.
2.  **Hành Động (Act)**: Triển khai thay đổi theo từng module (Domain -> App -> UI).
3.  **Xác Minh (Verify)**:
    *   **Build**: PHẢI chạy kiểm tra build.
    *   **Tự Động Hóa**: CHẠY quy trình tự động hóa liên quan.
    *   **Hình Ảnh**: Cung cấp bằng chứng hình ảnh nếu sửa UI.

## 4. Workflows Đã Định Nghĩa
*   Sử dụng các workflow trong `.agent/workflows` thay vì gõ lệnh thủ công.
