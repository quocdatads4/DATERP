# 📘 Phân tích Module Identity (`Volo.Abp.Identity`)

Tài liệu phân tích chi tiết về module quản lý người dùng và phân quyền trong hệ thống DATERP.

## 1. 🌐 Tổng quan
**Module Identity** (`Volo.Abp.Identity`) chịu trách nhiệm quản lý tổ chức, người dùng (Users), vai trò (Roles) và quyền hạn (Permissions).
- **Vai trò**: Quản trị hệ thống, phân quyền truy cập.
- **Loại tích hợp**: NuGet Package (Sử dụng UI mặc định của ABP, chưa được ghi đè).

## 2. 🧩 Tích hợp trong DATERP

### 📦 Sự phụ thuộc
- **Package**: `Volo.Abp.Identity.Web`
- **Module Class**: `DATERPWebModule.cs` tải `AbpIdentityWebModule`.

### 🎨 Giao diện & Ghi đè (UI Overrides)
Hiện tại, module này sử dụng **giao diện mặc định** của ABP Framework (Razor Class Library), không có bản ghi đè trong project.
- **Trạng thái**: Default (Chưa Custom).
- **Vị trí mong đợi nếu Custom**: 
  - `themes/Education/Pages/Identity/` (Ưu tiên)
  - hoặc `DATERP.Web/Pages/Identity/`

### 🧭 Menu & Điều hướng
Module được tích hợp vào menu chính của dòng **Quản trị viên (Administrator)** thông qua `DATERPMenuContributor.cs`.

**Vị trí Menu:**
`Management` -> `Users` / `Roles`

## 3. 🔗 Danh sách Routes & Tính năng

### 👥 Quản lý Người dùng (Users)
| Chức năng | URL | Mô tả |
| :--- | :--- | :--- |
| **Danh sách User** | `/Identity/Users` | Trang grid quản lý (Tìm kiếm, Thêm, Sửa, Xóa). |
| **Tạo mới** | `/Identity/Users/CreateModal` | Modal tạo người dùng (Popup). |
| **Chỉnh sửa** | `/Identity/Users/EditModal` | Modal chỉnh sửa người dùng. |
| **Phân quyền** | `/Identity/Users/PermissionsModal` | Gán quyền trực tiếp cho User. |

### 🛡️ Quản lý Vai trò (Roles)
| Chức năng | URL | Mô tả |
| :--- | :--- | :--- |
| **Danh sách Role** | `/Identity/Roles` | Quản lý các vai trò (VD: admin, teacher, student). |
| **Phân quyền Role** | `/Identity/Roles/PermissionsModal` | Thiết lập ma trận quyền cho từng Role. |

### 🔌 API Endpoints (Backend)
Các API sẵn có từ `Volo.Abp.Identity.HttpApi` (thường dùng bởi UI hoặc client khác):
- `GET /api/identity/users`
- `POST /api/identity/users`
- `GET /api/identity/roles`
- `PUT /api/identity/users/{id}/change-password`

## 4. ⚙️ Cấu hình & Mở rộng

### 🛠️ Cách tùy chỉnh (Customization)
Module này chưa được tùy chỉnh giao diện. Để tùy chỉnh:
1.  **Giao diện**:
    - Tạo thư mục: `themes/Education/Pages/Identity/Users/`.
    - Tạo file `Index.cshtml` để ghi đè trang danh sách User.
2.  **Logic**:
    - Ghi đè `IdentityAppService` hoặc sử dụng `ObjectExtensionManager` để thêm trường dữ liệu tùy chỉnh (Extra Properties) cho User.
3.  **Quyền hạn**:
    - Định nghĩa thêm quyền trong `DATERPPermissionDefinitionProvider.cs` nếu cần mở rộng chức năng nghiệp vụ.


## 5. UI Customization

### Page Overrides (`src/DATERP.Web/Pages/Identity/Users`)

This module's default User Management page (`/Identity/Users`) has been overridden to provide a custom, statistics-driven dashboard design.

#### [Index.cshtml](file:///C:/Users/QuocDat-PC/Documents/GitHub/DATERP/src/DATERP.Web/Pages/Identity/Users/Index.cshtml) (Razor Page)
- **Features**:
  - **Statistics Cards**: Displays Total, Active, Inactive, and Pending user counts.
  - **Custom Grid**: Replaces standard DataTables with a custom HTML table and Card view for mobile.
  - **Quick Filters**: Buttons to filter by status (Visual only currently, pending JS implementation).
  - **Bulk Actions**: UI for bulk operations.
- **Data Binding**: Uses a custom `IndexModel` instead of the default generic list.

#### [Index.cshtml.cs](file:///C:/Users/QuocDat-PC/Documents/GitHub/DATERP/src/DATERP.Web/Pages/Identity/Users/Index.cshtml.cs) (PageModel)
- **Inheritance**: Inherits directly from `PageModel` to bypass default ABP UI logic.
- **Services**: Injects `IIdentityUserAppService` and `IdentityUserManager`.
- **Logic**:
  - Fetches users via `GetListAsync`.
  - Calculates statistics (`TotalUsers`, `ActiveUsers`, `InactiveUsers`, `PendingUsers`) in `OnGetAsync`.
  - Maps `IdentityUserDto` to a helper `UserViewModel` for easier display handling.
