using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using DATERP.Examination.Examination;
using DATERP.Examination.Web.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;

namespace DATERP.Examination.Web.Pages.Examination.ExamLists;

public class CreateModalModel : ExaminationPageModel
{
    [BindProperty]
    public CreateExamListViewModel ExamList { get; set; } = default!;

    public List<SelectListItem> Subjects { get; set; } = new();

    private readonly IExamListAppService _examListAppService;
    private readonly IExamSubjectAppService _examSubjectAppService;

    public CreateModalModel(
        IExamListAppService examListAppService,
        IExamSubjectAppService examSubjectAppService)
    {
        _examListAppService = examListAppService;
        _examSubjectAppService = examSubjectAppService;
    }

    public async Task OnGetAsync()
    {
        ExamList = new CreateExamListViewModel();
        await LoadSubjectsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var dto = new CreateUpdateExamListDto
        {
            SubjectId = ExamList.SubjectId,
            Title = ExamList.Title,
            TimeLimit = ExamList.TimeLimit,
            Order = ExamList.Order
        };
        await _examListAppService.CreateAsync(dto);
        return NoContent();
    }

    private async Task LoadSubjectsAsync()
    {
        var subjects = await _examSubjectAppService.GetListAsync(new Volo.Abp.Application.Dtos.PagedAndSortedResultRequestDto
        {
            MaxResultCount = 100,
            SkipCount = 0
        });

        Subjects = subjects.Items.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = x.Name
        }).ToList();
    }

    public class CreateExamListViewModel
    {
        [SelectItems(nameof(Subjects))]
        [DisplayName("SubjectName")]
        public Guid SubjectId { get; set; }

        [Required]
        [DisplayName("Title")]
        public string Title { get; set; } = default!;

        [DisplayName("TimeLimit")]
        public int TimeLimit { get; set; } = 50;

        [DisplayName("Order")]
        public int Order { get; set; }
    }
}
