using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Uow;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System;

namespace DATERP.Data;

public class DATERPDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IPermissionManager _permissionManager;
    private readonly IPermissionDefinitionManager _permissionDefinitionManager;
    private readonly IGuidGenerator _guidGenerator;
    private readonly ICurrentTenant _currentTenant;

    private readonly IdentityUserManager _userManager;
    private readonly IdentityRoleManager _roleManager;

    public DATERPDataSeedContributor(
        IPermissionManager permissionManager,
        IPermissionDefinitionManager permissionDefinitionManager,
        IGuidGenerator guidGenerator,
        ICurrentTenant currentTenant,
        IdentityUserManager userManager,
        IdentityRoleManager roleManager)
    {
        _permissionManager = permissionManager;
        _permissionDefinitionManager = permissionDefinitionManager;
        _guidGenerator = guidGenerator;
        _currentTenant = currentTenant;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [UnitOfWork]
    public async Task SeedAsync(DataSeedContext context)
    {
        await SeedAdminUserAsync();
        await SeedPermissionsAsync();
    }

    private async Task SeedAdminUserAsync()
    {
        const string adminUserName = "admin";
        const string adminEmail = "admin@datacademy.edu.vn";
        const string adminPassword = "Admin@123";
        const string adminRoleName = "Administrator";

        // Seed Role
        if (await _roleManager.FindByNameAsync(adminRoleName) == null)
        {
            var role = new IdentityRole(_guidGenerator.Create(), adminRoleName, _currentTenant.Id)
            {
                IsStatic = true
            };
            (await _roleManager.CreateAsync(role)).CheckErrors();
        }

        // Seed User
        var adminUser = await _userManager.FindByNameAsync(adminUserName);
        if (adminUser == null)
        {
            adminUser = new IdentityUser(_guidGenerator.Create(), adminUserName, adminEmail, _currentTenant.Id);
            adminUser.SetEmailConfirmed(true);
            (await _userManager.CreateAsync(adminUser, adminPassword)).CheckErrors();
        }
        else
        {
            // Update Email if changed
            if (!string.Equals(adminUser.Email, adminEmail, StringComparison.OrdinalIgnoreCase))
            {
                (await _userManager.SetEmailAsync(adminUser, adminEmail)).CheckErrors();
                (await _userManager.UpdateAsync(adminUser)).CheckErrors();
            }

            // Force Reset Password
            if (await _userManager.HasPasswordAsync(adminUser))
            {
                (await _userManager.RemovePasswordAsync(adminUser)).CheckErrors();
            }
            (await _userManager.AddPasswordAsync(adminUser, adminPassword)).CheckErrors();
        }

        // Assign Role
        if (!await _userManager.IsInRoleAsync(adminUser, adminRoleName))
        {
            (await _userManager.AddToRoleAsync(adminUser, adminRoleName)).CheckErrors();
        }
    }

    private async Task SeedPermissionsAsync()
    {
        var permissions = await _permissionDefinitionManager.GetPermissionsAsync();

        foreach (var permission in permissions)
        {
            if (permission.Providers.Any() && !permission.Providers.Contains(RolePermissionValueProvider.ProviderName))
            {
                continue;
            }

            await _permissionManager.SetForRoleAsync("Administrator", permission.Name, true);
        }
    }
}

public static class IdentityResultExtensions
{
    public static void CheckErrors(this IdentityResult result)
    {
        if (!result.Succeeded)
        {
            throw new Exception("Identity operation failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}
