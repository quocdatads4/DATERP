using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace DATERP.Web;

[Dependency(ReplaceServices = true)]
public class DATAcademyBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "DATAcademy";

    public override string LogoUrl => "/_content/Education/img/logo-datacacademy.svg";

    public override string LogoReverseUrl => "/_content/Education/img/logo-datacacademy-white.svg";
}
