using System.Collections.Generic;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;

namespace Education.Bundling;

public class EducationThemeGlobalStyleContributor : BundleContributor
{
    public override void ConfigureBundle(BundleConfigurationContext context)
    {
        // Icons
        context.Files.AddIfNotContains("/_content/Education/vendor/fonts/tabler-icons.css");

        // Core CSS
        context.Files.AddIfNotContains("/_content/Education/vendor/css/core.css");

        // Theme CSS (default theme)
        context.Files.AddIfNotContains("/_content/Education/vendor/css/theme-default.css");

        // Demo/Custom CSS
        context.Files.AddIfNotContains("/_content/Education/css/demo.css");

        // Site-specific CSS
        context.Files.AddIfNotContains("/_content/Education/css/site.css");
    }
}

