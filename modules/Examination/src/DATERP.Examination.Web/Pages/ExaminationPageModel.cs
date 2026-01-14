using DATERP.Examination.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace DATERP.Examination.Web.Pages;

/* Inherit your PageModel classes from this class.
 */
public abstract class ExaminationPageModel : AbpPageModel
{
    protected ExaminationPageModel()
    {
        LocalizationResourceType = typeof(ExaminationResource);
        ObjectMapperContext = typeof(ExaminationWebModule);
    }
}
