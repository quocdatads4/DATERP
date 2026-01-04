using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace DATERP.Data;

public class DATERPDemoDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IGuidGenerator _guidGenerator;
    private readonly ICurrentTenant _currentTenant;
    private readonly IdentityUserManager _userManager;
    private readonly IdentityRoleManager _roleManager;

    public DATERPDemoDataSeedContributor(
        IGuidGenerator guidGenerator,
        ICurrentTenant currentTenant,
        IdentityUserManager userManager,
        IdentityRoleManager roleManager)
    {
        _guidGenerator = guidGenerator;
        _currentTenant = currentTenant;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [UnitOfWork]
    public async Task SeedAsync(DataSeedContext context)
    {
        await SeedRolesAsync();
        await SeedUsersAsync();
    }

    private async Task SeedRolesAsync()
    {
        string[] roleNames = { "Teacher", "Student", "Staff" };

        foreach (var roleName in roleNames)
        {
            if (await _roleManager.FindByNameAsync(roleName) == null)
            {
                var role = new IdentityRole(_guidGenerator.Create(), roleName, _currentTenant.Id)
                {
                    IsStatic = true,
                    IsPublic = true
                };
                (await _roleManager.CreateAsync(role)).CheckErrors();
            }
        }
    }

    private async Task SeedUsersAsync()
    {
        var users = new[]
        {
            new { Name = "teacher", Email = "teacher@daterp.com", Role = "Teacher" },
            new { Name = "student", Email = "student@daterp.com", Role = "Student" },
            new { Name = "staff", Email = "staff@daterp.com", Role = "Staff" }
        };

        const string defaultPassword = "1q2w3E*";

        foreach (var userInfo in users)
        {
            var user = await _userManager.FindByNameAsync(userInfo.Name);
            if (user == null)
            {
                user = new IdentityUser(_guidGenerator.Create(), userInfo.Name, userInfo.Email, _currentTenant.Id);
                (await _userManager.CreateAsync(user, defaultPassword)).CheckErrors();
            }

            if (!await _userManager.IsInRoleAsync(user, userInfo.Role))
            {
                (await _userManager.AddToRoleAsync(user, userInfo.Role)).CheckErrors();
            }
        }
    }
}
