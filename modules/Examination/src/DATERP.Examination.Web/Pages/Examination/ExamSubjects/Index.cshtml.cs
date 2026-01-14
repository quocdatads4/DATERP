using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Volo.Abp.Application.Dtos;
using DATERP.Examination.Examination;

namespace DATERP.Examination.Web.Pages.Examination.ExamSubjects;

public class IndexModel : PageModel
{
    private readonly IExamSubjectAppService _examSubjectAppService;
    private readonly Volo.Abp.Users.ICurrentUser _currentUser;

    public bool IsStudent { get; set; }
    public IReadOnlyList<ExamSubjectDto> ExamSubjects { get; set; } = new List<ExamSubjectDto>();

    public IndexModel(
        IExamSubjectAppService examSubjectAppService,
        Volo.Abp.Users.ICurrentUser currentUser)
    {
        _examSubjectAppService = examSubjectAppService;
        _currentUser = currentUser;
    }

    public async Task OnGetAsync()
    {
        IsStudent = _currentUser.IsInRole("Student");

        if (IsStudent)
        {
            var result = await _examSubjectAppService.GetListAsync(new PagedAndSortedResultRequestDto
            {
                MaxResultCount = 100, // Fetch all reasonable number of subjects for grid
                Sorting = "Name asc"
            });
            ExamSubjects = result.Items;
        }
    }

    public async Task<JsonResult> OnGetGetListAsync(DataTableRequest input)
    {
        var result = await _examSubjectAppService.GetListAsync(new PagedAndSortedResultRequestDto
        {
            MaxResultCount = input.Length <= 0 ? 10 : input.Length,
            SkipCount = input.Start,
            Sorting = "Name asc" // Default sorting
        });

        return new JsonResult(new
        {
            draw = input.Draw,
            recordsTotal = result.TotalCount,
            recordsFiltered = result.TotalCount,
            data = result.Items
        });
    }

    public class DataTableRequest
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
    }
}
