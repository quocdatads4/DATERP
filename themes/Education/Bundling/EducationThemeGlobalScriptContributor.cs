using System.Collections.Generic;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Bundling;

namespace Education.Bundling;

public class EducationThemeGlobalScriptContributor : SharedThemeGlobalScriptContributor
{
    public override void ConfigureBundle(BundleConfigurationContext context)
    {
        // Include ABP core scripts (jQuery, abp.js, etc.) from base class
        base.ConfigureBundle(context);

        // Helpers
        context.Files.AddIfNotContains("/_content/Education/vendor/js/helpers.js");

        // Menu JS
        context.Files.AddIfNotContains("/_content/Education/vendor/js/menu.js");

        // Config JS
        context.Files.AddIfNotContains("/_content/Education/js/config.js");

        // Main JS
        context.Files.AddIfNotContains("/_content/Education/js/main.js");

        // Site-specific JS
        context.Files.AddIfNotContains("/_content/Education/js/site.js");
    }
}

