using System.Threading.Tasks;
using DATERP.Examination.Examination;
using Microsoft.AspNetCore.Mvc;
using DATERP.Examination.Web.Pages;

namespace DATERP.Examination.Web.Pages.Examination.ExamSubjects;

public class CreateModalModel : ExaminationPageModel
{
    [BindProperty]
    public CreateUpdateExamSubjectDto ExamSubject { get; set; } = default!;

    private readonly IExamSubjectAppService _examSubjectAppService;

    public CreateModalModel(IExamSubjectAppService examSubjectAppService)
    {
        _examSubjectAppService = examSubjectAppService;
    }

    public void OnGet()
    {
        ExamSubject = new CreateUpdateExamSubjectDto();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _examSubjectAppService.CreateAsync(ExamSubject);
        return NoContent();
    }
}
