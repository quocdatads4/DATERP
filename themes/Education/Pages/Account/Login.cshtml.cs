using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Volo.Abp.Account.Settings;
using Volo.Abp.Account.Web;
using Volo.Abp.Identity;
using Volo.Abp.Settings;

namespace Education.Pages.Account;

/// <summary>
/// Custom Login PageModel for Education theme.
/// Inherits from ABP's LoginModel to leverage standard authentication logic.
/// </summary>
public class LoginModel : Volo.Abp.Account.Web.Pages.Account.LoginModel
{
    public LoginModel(
        IAuthenticationSchemeProvider schemeProvider,
        IOptions<AbpAccountOptions> accountOptions,
        IOptions<IdentityOptions> identityOptions,
        IdentityDynamicClaimsPrincipalContributorCache identityDynamicClaimsPrincipalContributorCache,
        IWebHostEnvironment webHostEnvironment)
        : base(schemeProvider, accountOptions, identityOptions, identityDynamicClaimsPrincipalContributorCache, webHostEnvironment)
    {
    }

    public override async Task<IActionResult> OnGetAsync()
    {
        // Initialize LoginInput to avoid null reference
        LoginInput ??= new LoginInputModel();

        // Get external providers
        ExternalProviders = await GetExternalProviders();

        // Check if local login is enabled
        EnableLocalLogin = await SettingProvider.IsTrueAsync(AccountSettingNames.EnableLocalLogin);

        // If only external login is available, redirect to that provider
        if (IsExternalLoginOnly)
        {
            return await OnPostExternalLogin(ExternalProviders.First().AuthenticationScheme);
        }

        return Page();
    }

    // Let base class handle OnPostAsync - it has proper error handling
}
