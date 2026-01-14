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

public class EditModalModel : ExaminationPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public EditExamProjectViewModel ExamProject { get; set; } = default!;

    public List<SelectListItem> ExamLists { get; set; } = new();

    private readonly IExamProjectAppService _examProjectAppService;
    private readonly IExamListAppService _examListAppService;

    public EditModalModel(
        IExamProjectAppService examProjectAppService,
        IExamListAppService examListAppService)
    {
        _examProjectAppService = examProjectAppService;
        _examListAppService = examListAppService;
    }

    public async Task OnGetAsync()
    {
        var projectDto = await _examProjectAppService.GetAsync(Id);
        ExamProject = new EditExamProjectViewModel
        {
            ExamListId = projectDto.ExamListId,
            Name = projectDto.Name,
            Instruction = projectDto.Instruction,
            Order = projectDto.Order
        };
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
        await _examProjectAppService.UpdateAsync(Id, dto);
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
            Text = x.Title,
            Selected = x.Id == ExamProject.ExamListId
        }).ToList();
    }

    public class EditExamProjectViewModel
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
