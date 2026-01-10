using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Users;
using Volo.Abp.Identity.Web.Navigation;
using Volo.Abp.SettingManagement.Web.Navigation;
using Volo.Abp.TenantManagement.Web.Navigation;
using Volo.Abp.UI.Navigation;

namespace DATERP.Web.Menus;

public class DATERPMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private async Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var currentUser = context.ServiceProvider.GetRequiredService<ICurrentUser>();

        // 1. ADMINISTRATOR
        // Shows: Dashboard, Education, Management, Settings
        if (currentUser.IsInRole("admin") || currentUser.IsInRole("Administrator"))
        {
            // Dashboard
            var adminDashboard = new ApplicationMenuItem(
                DATERPMenus.Home,
                "Bảng điều khiển", // Dashboard -> Bảng điều khiển
                "~/",
                icon: "ti ti-smart-home",
                order: 0
            );
            context.Menu.AddItem(adminDashboard);

            // Education -> Đào tạo
            var educationMenu = new ApplicationMenuItem(DATERPMenus.Education, "Đào tạo", icon: "ti ti-school", order: 1);
            educationMenu.AddItem(new ApplicationMenuItem(DATERPMenus.Academy, "Học viện", "/Academy", icon: "ti ti-book")); // Academy -> Học viện
            educationMenu.AddItem(new ApplicationMenuItem(DATERPMenus.Courses, "Khóa học", "/Courses", icon: "ti ti-notebook")); // Courses -> Khóa học
            educationMenu.AddItem(new ApplicationMenuItem(DATERPMenus.Students, "Học viên", "/Students", icon: "ti ti-users")); // Students -> Học viên
            educationMenu.AddItem(new ApplicationMenuItem(DATERPMenus.Teachers, "Giáo viên", "/Teachers", icon: "ti ti-user-check")); // Teachers -> Giáo viên
            context.Menu.AddItem(educationMenu);

            // Management -> Quản lý
            var managementMenu = new ApplicationMenuItem(DATERPMenus.Management, "Quản lý hệ thống", icon: "ti ti-settings", order: 2);
            managementMenu.AddItem(new ApplicationMenuItem(DATERPMenus.Users, "Người dùng", "/Identity/Users", icon: "ti ti-user")); // Users -> Người dùng
            managementMenu.AddItem(new ApplicationMenuItem(DATERPMenus.Roles, "Vai trò", "/Identity/Roles", icon: "ti ti-shield")); // Roles -> Vai trò
            managementMenu.AddItem(new ApplicationMenuItem(DATERPMenus.Tenants, "Thuê bao", "/TenantManagement/Tenants", icon: "ti ti-building")); // Tenants -> Thuê bao
            context.Menu.AddItem(managementMenu);

            // Settings -> Cấu hình
            var settingsMenu = new ApplicationMenuItem(DATERPMenus.Settings, "Cấu hình", icon: "ti ti-tool", order: 3);
            settingsMenu.AddItem(new ApplicationMenuItem(DATERPMenus.GeneralSettings, "Chung", "/SettingManagement", icon: "ti ti-adjustments")); // General Settings -> Chung
            settingsMenu.AddItem(new ApplicationMenuItem(DATERPMenus.Features, "Tính năng", "/FeatureManagement", icon: "ti ti-toggle-left")); // Features -> Tính năng
            context.Menu.AddItem(settingsMenu);
        }

        // 2. STUDENTS
        // Shows: Trang chủ, Khóa học, Luyện thi
        if (currentUser.IsInRole("student") || currentUser.IsInRole("Student"))
        {
            // Only add Home if not already added
            if (context.Menu.FindMenuItem(DATERPMenus.Home) == null)
            {
                context.Menu.AddItem(new ApplicationMenuItem(
                    "Student.Home",
                    "Trang chủ",
                    "~/",
                    icon: "ti ti-home-2",
                    order: 0
                ));
            }

            context.Menu.AddItem(new ApplicationMenuItem(
                DATERPMenus.MyLearning,
                "Khóa học",
                "/student/courses",
                icon: "ti ti-book-2",
                order: 10
            ));

            context.Menu.AddItem(new ApplicationMenuItem(
                DATERPMenus.ExamSimulation,
                "Luyện thi",
                "/student/exams",
                icon: "ti ti-test-pipe",
                order: 11
            ));
        }

        // 3. TEACHERS
        // Shows: Trang chủ, Khóa học, Bài học
        if (currentUser.IsInRole("teacher") || currentUser.IsInRole("Teacher"))
        {
            // Only add Home if not already added
            if (context.Menu.FindMenuItem(DATERPMenus.Home) == null && context.Menu.FindMenuItem("Student.Home") == null)
            {
                context.Menu.AddItem(new ApplicationMenuItem(
                    "Teacher.Home",
                    "Trang chủ",
                    "~/",
                    icon: "ti ti-home-2",
                    order: 0
                ));
            }

            context.Menu.AddItem(new ApplicationMenuItem(
                "Teacher.Courses",
                "Khóa học", // Teacher Courses
                "/teacher/courses",
                icon: "ti ti-notebook",
                order: 20
            ));

            context.Menu.AddItem(new ApplicationMenuItem(
                "Teacher.Lessons",
                "Bài học", // Teacher Lessons / Grading
                "/teacher/lessons",
                icon: "ti ti-chalkboard",
                order: 21
            ));
        }
    }
}
