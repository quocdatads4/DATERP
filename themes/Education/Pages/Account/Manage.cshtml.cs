using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Account;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.Identity;

namespace Education.Pages.Account;

public class ManageModel : AbpPageModel
{
    [BindProperty]
    public PersonalInfoViewModel PersonalInfo { get; set; } = new();

    [BindProperty]
    public ChangePasswordViewModel ChangePassword { get; set; } = new();

    private readonly IProfileAppService _profileAppService;

    public ManageModel(IProfileAppService profileAppService)
    {
        _profileAppService = profileAppService;
    }

    public async Task OnGetAsync()
    {
        var profile = await _profileAppService.GetAsync();
        PersonalInfo = new PersonalInfoViewModel
        {
            UserName = profile.UserName,
            Email = profile.Email,
            Name = profile.Name,
            PhoneNumber = profile.PhoneNumber
        };
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var input = new UpdateProfileDto
        {
            UserName = PersonalInfo.UserName,
            Email = PersonalInfo.Email,
            Name = PersonalInfo.Name,
            PhoneNumber = PersonalInfo.PhoneNumber
        };

        await _profileAppService.UpdateAsync(input);

        Alerts.Success("Cập nhật thông tin thành công!");
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostChangePasswordAsync()
    {
        var input = new ChangePasswordInput
        {
            CurrentPassword = ChangePassword.CurrentPassword,
            NewPassword = ChangePassword.NewPassword
        };

        await _profileAppService.ChangePasswordAsync(input);

        Alerts.Success("Đổi mật khẩu thành công!");
        return RedirectToPage();
    }

    public class PersonalInfoViewModel
    {
        public string UserName { get; set; } = default!;
        public string Email { get; set; } = default!;

        [Display(Name = "Họ và tên")]
        public string Name { get; set; } = default!;

        [Display(Name = "Số điện thoại")]
        [Phone]
        public string PhoneNumber { get; set; } = default!;
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu hiện tại")]
        [Display(Name = "Mật khẩu hiện tại")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; } = default!;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
        [Display(Name = "Mật khẩu mới")]
        [DataType(DataType.Password)]
        [StringLength(128, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất {2} ký tự")]
        public string NewPassword { get; set; } = default!;

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu mới")]
        [Display(Name = "Xác nhận mật khẩu")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; } = default!;
    }
}
