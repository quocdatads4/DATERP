using System;
using System.Threading.Tasks;
using DATERP.Examination.Examination;
using Microsoft.AspNetCore.Mvc;
using DATERP.Examination.Web.Pages;

namespace DATERP.Examination.Web.Pages.Examination.ExamSubjects;

public class EditModalModel : ExaminationPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public CreateUpdateExamSubjectDto ExamSubject { get; set; } = default!;

    private readonly IExamSubjectAppService _examSubjectAppService;

    public EditModalModel(IExamSubjectAppService examSubjectAppService)
    {
        _examSubjectAppService = examSubjectAppService;
    }

    public async Task OnGetAsync()
    {
        var examSubjectDto = await _examSubjectAppService.GetAsync(Id);
        ExamSubject = ObjectMapper.Map<ExamSubjectDto, CreateUpdateExamSubjectDto>(examSubjectDto);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _examSubjectAppService.UpdateAsync(Id, ExamSubject);
        return NoContent();
    }
}
