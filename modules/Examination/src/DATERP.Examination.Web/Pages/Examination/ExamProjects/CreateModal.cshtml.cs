using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DATERP.Examination.Examination;
using DATERP.Examination.Web.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;

namespace DATERP.Examination.Web.Pages.Examination.ExamProjects;

public class CreateModalModel : ExaminationPageModel
{
    [BindProperty]
    public CreateExamProjectViewModel ExamProject { get; set; } = default!;

    public List<SelectListItem> ExamLists { get; set; } = new();

    private readonly IExamProjectAppService _examProjectAppService;
    private readonly IExamListAppService _examListAppService;

    public CreateModalModel(
        IExamProjectAppService examProjectAppService,
        IExamListAppService examListAppService)
    {
        _examProjectAppService = examProjectAppService;
        _examListAppService = examListAppService;
    }

    public async Task OnGetAsync()
    {
        ExamProject = new CreateExamProjectViewModel();
        await LoadExamListsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var dto = new CreateUpdateExamProjectDto
        {
            ExamListId = ExamProject.ExamListId,
            Name = ExamProject.Name,
            Instruction = ExamProject.Instruction,
            Order = ExamProject.Order
        };
        await _examProjectAppService.CreateAsync(dto);
        return NoContent();
    }

    private async Task LoadExamListsAsync()
    {
        var examLists = await _examListAppService.GetListAsync(new Volo.Abp.Application.Dtos.PagedAndSortedResultRequestDto
        {
            MaxResultCount = 100,
            SkipCount = 0
        });

        ExamLists = examLists.Items.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = x.Title
        }).ToList();
    }

    public class CreateExamProjectViewModel
    {
        [SelectItems(nameof(ExamLists))]
        [DisplayName("ExamListTitle")]
        public Guid ExamListId { get; set; }

        [Required]
        [DisplayName("ProjectName")]
        public string Name { get; set; } = default!;

        [DisplayName("Instruction")]
        [TextArea(Rows = 4)]
        public string? Instruction { get; set; }

        [DisplayName("Order")]
        public int Order { get; set; }
    }
}
