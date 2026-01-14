---
description: Tạo CRUD pages cho entity mới trong Examination module (DTOs, AppService, Razor Pages, Localization, Automation)
---

# Tạo CRUD Pages cho Entity mới trong Examination Module

Workflow này hướng dẫn tạo đầy đủ các file cho một entity mới trong module Examination, bao gồm Application layer, Web layer, Localization và Automation.

## Thông tin cần chuẩn bị

Trước khi bắt đầu, xác định:
- **EntityName**: Tên entity (ví dụ: `ExamProject`)
- **EntityDisplayName**: Tên hiển thị tiếng Việt (ví dụ: "Đề thi")
- **ParentEntity**: Entity cha nếu có (ví dụ: `ExamList`)
- **Properties**: Các thuộc tính của entity

---

## Các bước thực hiện

### 1. Application.Contracts Layer

Tạo DTOs trong `modules/Examination/src/DATERP.Examination.Application.Contracts/Examination/`:

```csharp
// {EntityName}Dto.cs
public class {EntityName}Dto : FullAuditedEntityDto<Guid>
{
    public Guid {ParentEntity}Id { get; set; }
    public string {ParentEntity}Title { get; set; } = default!;
    // Các properties khác...
}

public class CreateUpdate{EntityName}Dto
{
    public Guid {ParentEntity}Id { get; set; }
    // Các properties khác...
}
```

Tạo Interface trong cùng thư mục:
```csharp
// I{EntityName}AppService.cs
public interface I{EntityName}AppService :
    ICrudAppService<{EntityName}Dto, Guid, PagedAndSortedResultRequestDto, CreateUpdate{EntityName}Dto>
{
    Task<ListResultDto<{EntityName}Dto>> GetListBy{ParentEntity}IdAsync(Guid {parentEntity}Id);
}
```

### 2. Application Layer

Tạo AppService trong `modules/Examination/src/DATERP.Examination.Application/Examination/`:

```csharp
// {EntityName}AppService.cs
[RemoteService(Name = "Examination")]
public class {EntityName}AppService :
    CrudAppService<{EntityName}, {EntityName}Dto, Guid, PagedAndSortedResultRequestDto, CreateUpdate{EntityName}Dto>,
    I{EntityName}AppService
{
    // Override GetAsync và GetListAsync để load thông tin parent entity
}
```

Cập nhật AutoMapper trong `ExaminationApplicationAutoMapperProfile.cs`:
```csharp
CreateMap<{EntityName}, {EntityName}Dto>(MemberList.None)
    .ForMember(dest => dest.{ParentEntity}Title, opt => opt.Ignore());
CreateMap<CreateUpdate{EntityName}Dto, {EntityName}>(MemberList.None);
CreateMap<{EntityName}Dto, CreateUpdate{EntityName}Dto>(MemberList.None);
```

### 3. Web Layer - Razor Pages

Tạo thư mục và files trong `modules/Examination/src/DATERP.Examination.Web/Pages/Examination/{EntityName}s/`:

| File | Mô tả |
|------|-------|
| `Index.cshtml` | Sử dụng `_AdminTable.cshtml` partial |
| `Index.cshtml.cs` | Handler với `OnGetGetListAsync` cho DataTable |
| `Index.js` | Config DataTable với `initAdminTable()` |
| `CreateModal.cshtml` | ABP dynamic form với dropdown parent |
| `CreateModal.cshtml.cs` | ViewModel với `[SelectItems]` attribute |
| `EditModal.cshtml` | ABP dynamic form |
| `EditModal.cshtml.cs` | Load entity và parent list |

> [!IMPORTANT]
> Trong cshtml, sử dụng **fully qualified model name** để tránh lỗi ambiguous reference:
> `@model DATERP.Examination.Web.Pages.Examination.{EntityName}s.IndexModel`

### 4. Localization

Cập nhật `vi.json` và `en.json` trong `modules/Examination/src/DATERP.Examination.Domain.Shared/Localization/Examination/`:

```json
{
  "{EntityName}s": "{EntityDisplayName}",
  "New{EntityName}": "Thêm {EntityDisplayName} mới",
  "{EntityName}DeletionConfirmationMessage": "Bạn có chắc chắn muốn xóa {EntityDisplayName} {0}?"
}
```

### 5. PowerShell Script

Tạo file `.ps/admin/run_{entityname}s_automation.ps1`:

```powershell
# Script to build and run DATERP then verify {EntityName}s page
# Copy từ template run_examlists_automation.ps1
# Thay đổi: verify_{entityname}s_page.js
```

### 6. Workflow File

Tạo `.agent/workflows/admin/run_{entityname}s_automation.md`:

```markdown
---
description: Chạy automation để truy cập trang {EntityName}s
---
# Run {EntityName}s Automation
// turbo
1. Chạy script:
\`\`\`powershell
.\.ps\admin\run_{entityname}s_automation.ps1
\`\`\`
```

### 7. Selenium Automation

Tạo `.agent/automation/admin/verify_{entityname}s_page.js`:
- Copy từ template `verify_examlists_page.js` (trong folder admin)
- Thay đổi URL: `/Examination/{EntityName}s`
- Thay đổi TableId: `{EntityName}sTable`
- Thay đổi CreateButtonId: `New{EntityName}Button`

---

## Verification

// turbo
1. Build project:
```powershell
dotnet build .\modules\Examination\src\DATERP.Examination.Web\DATERP.Examination.Web.csproj
```

2. Chạy automation để verify:
```powershell
powershell -ExecutionPolicy Bypass -File .\.ps\admin\run_{entityname}s_automation.ps1
```

---

## Checklist tổng hợp

- [ ] `{EntityName}Dto.cs` - DTOs
- [ ] `I{EntityName}AppService.cs` - Interface
- [ ] `{EntityName}AppService.cs` - Implementation
- [ ] `ExaminationApplicationAutoMapperProfile.cs` - Mapping
- [ ] `{EntityName}s/Index.cshtml` - Index page
- [ ] `{EntityName}s/Index.cshtml.cs` - Index handler
- [ ] `{EntityName}s/Index.js` - DataTable config
- [ ] `{EntityName}s/CreateModal.cshtml` - Create modal
- [ ] `{EntityName}s/CreateModal.cshtml.cs` - Create handler
- [ ] `{EntityName}s/EditModal.cshtml` - Edit modal
- [ ] `{EntityName}s/EditModal.cshtml.cs` - Edit handler
- [ ] `vi.json` - Vietnamese localization
- [ ] `en.json` - English localization
- [ ] `.ps/admin/run_{entityname}s_automation.ps1` - PowerShell script
- [ ] `.agent/workflows/admin/run_{entityname}s_automation.md` - Workflow
- [ ] `.agent/automation/admin/verify_{entityname}s_page.js` - Selenium
- [ ] Build thành công
- [ ] Automation pass
