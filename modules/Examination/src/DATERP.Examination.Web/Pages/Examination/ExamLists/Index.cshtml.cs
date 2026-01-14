using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Volo.Abp.Application.Dtos;
using DATERP.Examination.Examination;

namespace DATERP.Examination.Web.Pages.Examination.ExamLists;

public class IndexModel : PageModel
{
    private readonly IExamListAppService _examListAppService;
    private readonly IExamSubjectAppService _examSubjectAppService;
    private readonly Volo.Abp.Users.ICurrentUser _currentUser;

    public int TotalCount { get; set; }
    public bool IsStudent { get; set; }

    [BindProperty(SupportsGet = true)]
    public Guid? SubjectId { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SubjectCode { get; set; }

    public ExamSubjectDto Subject { get; set; }
    public List<ExamListDto> ExamLists { get; set; }

    public IndexModel(
        IExamListAppService examListAppService,
        IExamSubjectAppService examSubjectAppService,
        Volo.Abp.Users.ICurrentUser currentUser)
    {
        _examListAppService = examListAppService;
        _examSubjectAppService = examSubjectAppService;
        _currentUser = currentUser;
    }

    public async Task OnGetAsync()
    {
        IsStudent = _currentUser.IsInRole("student") || _currentUser.IsInRole("Student");

        if (IsStudent)
        {
            if (!string.IsNullOrEmpty(SubjectCode))
            {
                var subject = await _examSubjectAppService.GetByCodeAsync(SubjectCode);
                if (subject != null)
                {
                    SubjectId = subject.Id;
                }
            }

            if (SubjectId.HasValue)
            {
                Subject = await _examSubjectAppService.GetAsync(SubjectId.Value);
                var result = await _examListAppService.GetListBySubjectIdAsync(SubjectId.Value);
                ExamLists = result.Items.ToList();
            }
        }

        if (!IsStudent || !SubjectId.HasValue)
        {
            var result = await _examListAppService.GetListAsync(new PagedAndSortedResultRequestDto
            {
                MaxResultCount = 1,
                SkipCount = 0
            });
            TotalCount = (int)result.TotalCount;
        }
    }

    public async Task<JsonResult> OnGetGetListAsync(DataTableRequest input)
    {
        var result = await _examListAppService.GetListAsync(new PagedAndSortedResultRequestDto
        {
            MaxResultCount = input.Length <= 0 ? 10 : input.Length,
            SkipCount = input.Start,
            Sorting = "Order asc"
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
