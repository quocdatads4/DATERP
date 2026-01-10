# QUY TẮC & TIÊU CHUẨN PHÁT TRIỂN DỰ ÁN DATERP (ABP FRAMEWORK)

Tài liệu này tổng hợp các quy tắc cốt lõi, tiêu chuẩn kiến trúc và hướng dẫn phát triển cho dự án DATERP, dựa trên nền tảng **ABP Framework v9.x** và các nguyên lý **Domain Driven Design (DDD)**.

Tham khảo chi tiết: [ABP Framework Best Practices](https://abp.io/docs/latest/best-practices/index)

---

## 1. Triết Lý Kiến Trúc (Core Philosophy)

### 1.1. Mô hình Kiến trúc
*   **Modular Monolith (Tuyệt đối tuân thủ):** Hệ thống được thiết kế theo dạng One-Projet nhưng bản chất là tập hợp các **Modules** độc lập.
*   **Module-First:** Mọi tính năng nghiệp vụ (Business Feature) đều phải thuộc về một Module cụ thể (ví dụ: `Academic`, `LMS`, `Examination`).
*   **Không Coupling Chéo:** Các module chỉ giao tiếp với nhau qua **Interface (Contract)** hoặc **Event (Data Integration)**. Tuyệt đối **KHÔNG** join bảng database giữa các module.

### 1.2. Phân Tầng Hệ Thống (Layering)

Tuân thủ nghiêm ngặt mô hình 4 tầng của DDD:

1.  **Domain Layer (`modules/X/src/X.Domain`)**:
    *   Chứa `Entities`, `Value Objects`, `Domain Services`, `Specifications`.
    *   Đây là **trái tim** của nghiệp vụ.
    *   **TUYỆT ĐỐI KHÔNG** phụ thuộc vào Infrastructure, Application hay Web.
    *   Không chứa logic liên quan đến HTTP, JSON, DTO.

2.  **Application Layer (`modules/X/src/X.Application`)**:
    *   Chứa `Application Services`, `DTOs`.
    *   Nhiệm vụ: Điều phối công việc, chuyển đổi dữ liệu (Mapping), xử lý transaction.
    *   **KHÔNG** chứa logic nghiệp vụ phức tạp (hãy đẩy xuống Domain Service).
    *   Input/Output của API **BẮT BUỘC** là DTO, không bao giờ trả về Entity.

3.  **Infrastructure Layer (`src/` hoặc `modules/X/src/X.EntityFrameworkCore`)**:
    *   Chứa cấu hình Database (`DbContext`), Repository Implementation, tích hợp hệ thống thứ 3 (Email, SMS, Payment).
    *   Nhiệm vụ: Hiện thực hóa các interface trừu tượng từ Domain.

4.  **Presentation/Web Layer (`themes/` & `src/DATERP.Web`)**:
    *   Chứa `Razor Pages`, `Controllers`, `JavaScript`, `CSS`.
    *   **TUYỆT ĐỐI KHÔNG** chứa logic nghiệp vụ. Chỉ làm nhiệm vụ hiển thị và gọi Application Service.

---

## 2. Quy Tắc Lập Trình (Coding Rules)

### 2.1. Entity & Aggregate Root
*   Luôn kế thừa `AggregateRoot<TKey>` cho thực thể chính.
*   Kế thừa `Entity<TKey>` cho các thực thể con.
*   Hạn chế setter `public` cho các property quan trọng. Hãy dùng method business (ví dụ: `ApproveOrder()` thay vì `order.Status = Approved`).

### 2.2. Repository & Querying
*   Ưu tiên sử dụng **`IRepository<TEntity, TKey>`** mặc định của ABP.
*   Khi cần query phức tạp (Join nhiều bảng, Statistic):
    *   Tạo `Custom Repository` hoặc `Query Service` riêng.
    *   **Luôn dùng `IQueryable`** và `await ToListAsync()` ở bước cuối cùng.
    *   **Không** return `IQueryable` ra khỏi Repository để tránh Leaky Abstraction.

### 2.3. Data Transfer Objects (DTO)
*   **Bắt buộc:** Mọi API Controller / AppService method đều phải nhận và trả về DTO.
*   **Naming:**
    *   User input: `Create...Dto`, `Update...Dto`.
    *   Output: `...Dto`.
*   Sử dụng `AutoMapper` cho các mapping đơn giản. Mapping phức tạp nên viết tay (Manual Mapping) trong `DomainService` hoặc `AppService` để dễ debug.

### 2.4. Dependency Injection (DI)
*   Sử dụng **Constructor Injection** cho mọi dependency.
*   Hạn chế `Property Injection` (trừ khi là Logger hoặc Optional Dep).
*   Không khởi tạo service bằng `new Class()`, hãy để Container quản lý.

### 2.5. Async/Await
*   **Luôn luôn dùng Async/Await** cho các tác vụ I/O (Database, File, HTTP Call).
*   Quy tắc đặt tên method: Kết thúc bằng `Async` (ví dụ: `GetListAsync`).

---

## 3. Quy Tắc Module Hóa (Modularity Rules)

### 3.1. Ranh giới Module (Boundaries)
*   Mỗi Module sở hữu `DbContext` riêng.
*   **Cấm:** Module A Query trực tiếp `DbSet` của Module B.
*   **Giải pháp:** Module A gọi `Contract Interace` của Module B, hoặc lắng nghe `Distributed Event` (Kafka/RabbitMQ/DbEvent) từ Module B.

### 3.2. Cấu trúc thư mục DATERP
*   `src/`: Chỉ chứa code hạ tầng (Hosting, Shared Shared, Auth).
*   `modules/`: Chứa code nghiệp vụ (Mỗi thư mục là một module độc lập).
*   `themes/`: Chứa giao diện (UI).

Nếu bạn thấy code nghiệp vụ (business logic) nằm trong `src/`, đó là sai phạm (Refactor ngay).

---

## 4. Hướng Dẫn Giáo dục & Đại Lý AI (Antigravity Scope)

### 4.1. Phong cách
*   Ngôn ngữ: **Tiếng Việt**.
*   Văn phong: Đơn giản, hỗ trợ, khích lệ (Phù hợp học sinh cấp 2-3, sinh viên nghề).
*   Không dùng thuật ngữ chuyên ngành quá sâu nếu không giải thích.

### 4.2. Trải nghiệm người dùng (UX)
*   Giao diện: **Premium, Dynamic, Wow-factor**.
*   Tránh dùng màu "chết" (plain red/blue), dùng Gradient/HSL hiện đại.

---

## 5. Quy Trình Work-flow (SOP)

1.  **Xác định Module:** Tính năng này thuộc về module nào? (Nếu chưa có, tạo mới).
2.  **Domain Layer trước:** Định nghĩa Entity, Enum, Rules.
3.  **Infrastructure:** Cấu hình EF Core, Migrations.
4.  **Application:** Viết DTO, AppService, Mapping.
5.  **UI/Web:** Viết Razor Page, JS gọi API.
6.  **Review:** Kiểm tra xem có vi phạm quy tắc "Cross-module" hoặc "Business in UI" không.

---

*Tài liệu này là "Luật" (Law) trong quá trình phát triển DATERP. Mọi PR (Pull Request) vi phạm sẽ bị reject.*
