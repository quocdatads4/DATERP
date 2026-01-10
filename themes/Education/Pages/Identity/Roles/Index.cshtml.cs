using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Volo.Abp.Identity;
using Volo.Abp.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Education.Pages.Identity.Roles
{
    public class IdentityRoleCustomIndexModel : PageModel
    {
        private readonly IIdentityRoleAppService _roleAppService;

        public List<RoleViewModel> Roles { get; set; } = new List<RoleViewModel>();

        // Statistics
        public long TotalRoles { get; set; }
        public long DefaultRoles { get; set; }
        public long PublicRoles { get; set; }
        public long StaticRoles { get; set; }

        public IdentityRoleCustomIndexModel(IIdentityRoleAppService roleAppService)
        {
            _roleAppService = roleAppService;
        }

        public async Task OnGetAsync()
        {
            // Fetch all roles
            var input = new GetIdentityRolesInput
            {
                MaxResultCount = 1000
            };

            var roleResult = await _roleAppService.GetListAsync(input);
            var roles = roleResult.Items;

            // Calculate Stats
            TotalRoles = roleResult.TotalCount;
            DefaultRoles = roles.Count(r => r.IsDefault);
            PublicRoles = roles.Count(r => r.IsPublic);
            StaticRoles = roles.Count(r => r.IsStatic);

            // Map to ViewModel
            Roles = roles.Select(r => new RoleViewModel
            {
                Id = r.Id,
                Name = r.Name,
                IsDefault = r.IsDefault,
                IsPublic = r.IsPublic,
                IsStatic = r.IsStatic,
                UserCount = 0, // Would need separate query to count users per role
                Initials = GetInitials(r.Name)
            }).ToList();
        }

        private string GetInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "R";
            var words = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (words.Length >= 2)
            {
                return $"{words[0][0]}{words[1][0]}".ToUpper();
            }
            return name.Substring(0, Math.Min(2, name.Length)).ToUpper();
        }
    }

    public class RoleViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public bool IsPublic { get; set; }
        public bool IsStatic { get; set; }
        public int UserCount { get; set; }
        public string Initials { get; set; } = string.Empty;
    }
}
