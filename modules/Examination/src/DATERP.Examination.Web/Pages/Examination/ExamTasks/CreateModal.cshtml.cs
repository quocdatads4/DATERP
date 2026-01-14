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

namespace DATERP.Examination.Web.Pages.Examination.ExamTasks;

public class CreateModalModel : ExaminationPageModel
{
    [BindProperty]
    public CreateExamTaskViewModel ExamTask { get; set; } = default!;

    public List<SelectListItem> Projects { get; set; } = new();

    private readonly IExamTaskAppService _examTaskAppService;
    private readonly IExamProjectAppService _examProjectAppService;

    public CreateModalModel(
        IExamTaskAppService examTaskAppService,
        IExamProjectAppService examProjectAppService)
    {
        _examTaskAppService = examTaskAppService;
        _examProjectAppService = examProjectAppService;
    }

    public async Task OnGetAsync()
    {
        ExamTask = new CreateExamTaskViewModel();
        await LoadProjectsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var dto = new CreateUpdateExamTaskDto
        {
            ProjectId = ExamTask.ProjectId,
            Content = ExamTask.Content,
            Point = ExamTask.Point,
            Order = ExamTask.Order
        };
        await _examTaskAppService.CreateAsync(dto);
        return NoContent();
    }

    private async Task LoadProjectsAsync()
    {
        var projects = await _examProjectAppService.GetListAsync(new Volo.Abp.Application.Dtos.PagedAndSortedResultRequestDto
        {
            MaxResultCount = 100,
            SkipCount = 0
        });

        Projects = projects.Items.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = x.Name
        }).ToList();
    }

    public class CreateExamTaskViewModel
    {
        [SelectItems(nameof(Projects))]
        [DisplayName("ProjectName")]
        public Guid ProjectId { get; set; }

        [Required]
        [DisplayName("Content")]
        [TextArea(Rows = 4)]
        public string Content { get; set; } = default!;

        [DisplayName("Point")]
        public double Point { get; set; }

        [DisplayName("Order")]
        public int Order { get; set; }
    }
}
