# 📘 Phân tích Module Account (`Volo.Abp.Account`)

Tài liệu phân tích chi tiết về module quản lý tài khoản và danh tính trong hệ thống DATERP.

## 1. 🌐 Tổng quan
**Module Account** (`Volo.Abp.Account`) đóng vai trò cốt lõi trong việc quản lý danh tính, xác thực và đăng ký người dùng.
- **Vai trò**: Xử lý Login, Register, Forgot Password, và User Profile.
- **Loại tích hợp**: NuGet Package (không có source code gốc trong `modules/`).

## 2. 🧩 Tích hợp trong DATERP

### 📦 Sự phụ thuộc
- **Package**: `Volo.Abp.Account.Web`
- **Module Class**: `DATERPWebModule.cs` tải `AbpAccountWebModule`.

### 🎨 Giao diện & Ghi đè (UI Overrides)
Dự án sử dụng **Theme Education** để tùy biến toàn bộ giao diện mặc định.
- **Vị trí Theme**: `themes/Education`
- **Vị trí Override Account**: `themes/Education/Pages/Account/`
- **File quan trọng**:
  - 🔑 **Đăng nhập**: `themes/Education/Pages/Account/Login.cshtml`
  - 📝 **Đăng ký**: `themes/Education/Pages/Account/Register.cshtml` (nếu có)

### 🧭 Menu & Điều hướng
- **User Menu**: Dropdown góc phải trên cùng (Avatar) chứa các link Profile/Logout.
- **Admin Menu**:
  - 👥 **Users**: `/Identity/Users`
  - 🛡️ **Roles**: `/Identity/Roles`

## 3. 🔗 Danh sách Routes & Tính năng

### 🔐 Xác thực (Authentication)
| Chức năng | URL | Mô tả |
| :--- | :--- | :--- |
| **Đăng nhập** | `/Account/Login` | Trang đăng nhập tùy chỉnh (Education Theme). |
| **Đăng xuất** | `/Account/Logout` | Action đăng xuất hệ thống. |
| **Đăng ký** | `/Account/Register` | Đăng ký tài khoản mới (cần bật Setting). |

### 👤 Quản lý Tài khoản (My Account)
| Chức năng | URL | Mô tả |
| :--- | :--- | :--- |
| **Hồ sơ** | `/Account/Manage` | Dashboard thông tin cá nhân. |
| **Đổi mật khẩu** | `/Account/Manage/ChangePassword` | Thay đổi mật khẩu đăng nhập. |
| **Dữ liệu** | `/Account/Manage/PersonalData` | Tải xuống/Xóa dữ liệu cá nhân. |
| **Bảo mật 2 lớp** | `/Account/Manage/TwoFactorAuthentication` | Cấu hình 2FA (Email/SMS/Authenticator). |

### 🛡️ Quy trình Bảo mật
- ❓ **Quên mật khẩu**: `/Account/ForgotPassword`
- 🔄 **Đặt lại mật khẩu**: `/Account/ResetPassword`
- 📧 **Xác thực Email**: `/Account/EmailConfirmation`
- 📜 **Nhật ký bảo mật**: `/Account/SecurityLogs`

### 🔌 API Endpoints (Backend)
Các API sẵn có từ `Volo.Abp.Account.HttpApi`:
- `POST /api/account/register`
- `POST /api/account/send-password-reset-code`
- `POST /api/account/reset-password`
- `GET/PUT /api/account/my-profile`
- `POST /api/account/change-password`

## 4. ⚙️ Cấu hình & Mở rộng

### 🛠️ Cách tùy chỉnh (Customization)
1.  **Chỉnh sửa Giao diện**:
    - Truy cập thư mục: `themes/Education/Pages/Account/`.
    - Chỉnh sửa file `.cshtml` tương ứng (ví dụ: thêm logo, đổi màu sắc form login).
2.  **Cấu hình Logic**:
    - Sử dụng module `SettingManagement` để bật/tắt tính năng (ví dụ: `Abp.Account.IsSelfRegistrationEnabled`).
3.  **Điều hướng**:
    - Menu `DATERPMenuContributor.cs` chủ yếu quản lý menu chính (Main Menu). Menu tài khoản (User Menu) được render bởi ViewComponent của ABP.

---
## 5. 👥 Dữ liệu Mẫu & Tài khoản mặc định
Hệ thống được cấu hình (qua Data Seeder tại `src/DATERP.Domain/Data/`) với 3 tài khoản mẫu cố định. Mọi tài khoản khác sẽ bị **XÓA** khi chạy seed data để đảm bảo môi trường chuẩn.

### Danh sách tài khoản:
| Vai trò (Role) | Email | Mật khẩu | Quyền hạn |
| :--- | :--- | :--- | :--- |
| **Quản trị viên** | `admin@datacademy.edu.vn` | `Admin@123` | **Administrator** (Full quyền) |
| **Học viên** | `student@datacademy.edu.vn` | `Student@123` | **Student** (Truy cập khóa học, thi) |
| **Giáo viên** | `teacher@datacademy.edu.vn` | `Teacher@123` | **Teacher** (Quản lý lớp, chấm điểm) |

> [!NOTE]
> Vấn đề **Role**: Role quản trị viên mặc định `admin` đã được đổi tên hiển thị thành `Administrator`. `Student` và `Teacher` được tạo mới nếu chưa có.

### Chính sách Data Seeder:
- **Tự động tạo/sửa**: Khi chạy DbMigrator (`src/DATERP.DbMigrator`), hệ thống sẽ đảm bảo 3 tài khoản trên tồn tại với đúng thông tin (Email/Pass) và quyền hạn.
- **Cơ chế Clean-up**: Code seed data (`DATERPDemoDataSeedContributor.cs`) bao gồm logic `_userRepository.GetListAsync()` và `DeleteAsync` để xóa sạch các user thừa không nằm trong danh sách an toàn (Admin, Student, Teacher).

---
## 6. 🔀 Điều hướng theo Vai trò (Role Mapping)

Hệ thống đã triển khai logic điều hướng tự động ngay sau khi đăng nhập thông qua trang chủ `Index.cshtml`.

- **Vị trí file xử lý**: `themes/Education/Pages/Index.cshtml` và `Index.cshtml.cs`.
- **Cơ chế**: Khi người dùng truy cập trang chủ (`/`):
    1.  Kiểm tra đăng nhập. Nếu chưa -> Chuyển về `/Account/Login`.
    2.  Nếu đã đăng nhập, kiểm tra Role và điều hướng tương ứng:

| Role | Trang đích (Dashboard) | Đường dẫn |
| :--- | :--- | :--- |
| **Administrator** | Dashboard Quản trị | `/Admin/Dashboard` |
| **Student** | Dashboard Học viên | `/Student/Dashboard` |
| **Teacher** | Dashboard Giáo viên | `/Teacher/Dashboard` |
| *Khác* | Trang Welcome mặc định | `/` |

---
📅 *Cập nhật lần cuối: 2026-01-05*
