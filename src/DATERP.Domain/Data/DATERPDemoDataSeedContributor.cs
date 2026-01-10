using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
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
    private readonly IIdentityUserRepository _userRepository;

    public DATERPDemoDataSeedContributor(
        IGuidGenerator guidGenerator,
        ICurrentTenant currentTenant,
        IdentityUserManager userManager,
        IdentityRoleManager roleManager,
        IIdentityUserRepository userRepository)
    {
        _guidGenerator = guidGenerator;
        _currentTenant = currentTenant;
        _userManager = userManager;
        _roleManager = roleManager;
        _userRepository = userRepository;
    }

    [UnitOfWork]
    public async Task SeedAsync(DataSeedContext context)
    {
        await SeedRolesAsync();
        await SeedUsersAsync();
    }

    private async Task SeedRolesAsync()
    {
        string[] roleNames = { "Teacher", "Student" };

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
        // 1. Define Target Users
        var targetUsers = new List<(string Username, string Email, string Password, string Role)>
        {
            ("student", "student@datacademy.edu.vn", "Student@123", "Student"),
            ("teacher", "teacher@datacademy.edu.vn", "Teacher@123", "Teacher")
        };

        // 2. Create or Update Target Users
        foreach (var userInfo in targetUsers)
        {
            var user = await _userManager.FindByNameAsync(userInfo.Username);
            if (user == null)
            {
                user = new IdentityUser(_guidGenerator.Create(), userInfo.Username, userInfo.Email, _currentTenant.Id);
                (await _userManager.CreateAsync(user, userInfo.Password)).CheckErrors();
            }
            else
            {
                if (!string.Equals(user.Email, userInfo.Email, StringComparison.OrdinalIgnoreCase))
                {
                    (await _userManager.SetEmailAsync(user, userInfo.Email)).CheckErrors();
                    await _userManager.UpdateAsync(user);
                }

                // Force Reset Password
                if (await _userManager.HasPasswordAsync(user))
                {
                    (await _userManager.RemovePasswordAsync(user)).CheckErrors();
                }
                (await _userManager.AddPasswordAsync(user, userInfo.Password)).CheckErrors();
            }

            // Assign Role
            if (!await _userManager.IsInRoleAsync(user, userInfo.Role))
            {
                (await _userManager.AddToRoleAsync(user, userInfo.Role)).CheckErrors();
            }
        }

        // 3. Cleanup: Delete users NOT in the allowed list
        // Allow list includes 'admin' (from main seeder) and our target users
        var safeUsernames = new[] { "admin", "student", "teacher", "Administrator" };

        // Fetch all users using Repository
        var allUsers = await _userRepository.GetListAsync();

        foreach (var user in allUsers)
        {
            if (!safeUsernames.Contains(user.UserName))
            {
                await _userRepository.DeleteAsync(user);
            }
        }
    }
}
