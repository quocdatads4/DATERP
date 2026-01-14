using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Volo.Abp.Application.Dtos;
using DATERP.Examination.Examination;

namespace DATERP.Examination.Web.Pages.Examination.ExamTasks;

public class IndexModel : PageModel
{
    private readonly IExamTaskAppService _examTaskAppService;

    public int TotalCount { get; set; }

    public IndexModel(IExamTaskAppService examTaskAppService)
    {
        _examTaskAppService = examTaskAppService;
    }

    public async Task OnGetAsync()
    {
        var result = await _examTaskAppService.GetListAsync(new PagedAndSortedResultRequestDto
        {
            MaxResultCount = 1,
            SkipCount = 0
        });
        TotalCount = (int)result.TotalCount;
    }

    public async Task<JsonResult> OnGetGetListAsync(DataTableRequest input)
    {
        var result = await _examTaskAppService.GetListAsync(new PagedAndSortedResultRequestDto
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
