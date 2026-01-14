using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DATERP.Examination.Examination;

namespace DATERP.Examination.Web.Pages.Examination.ExamTaking;

public class IndexModel : PageModel
{
    private readonly IExamListAppService _examListAppService;
    private readonly IExamProjectAppService _examProjectAppService;
    private readonly IExamTaskAppService _examTaskAppService;
    private readonly Volo.Abp.Domain.Repositories.IRepository<DATERP.Examination.Examination.ExamSubject, Guid> _examSubjectRepository;
    private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _webHostEnvironment;

    [BindProperty(SupportsGet = true)]
    public Guid ExamListId { get; set; }

    public ExamListDto? ExamList { get; set; }
    public List<ExamProjectDto> Projects { get; set; } = new();
    public List<ExamTaskDto> Tasks { get; set; } = new();
    public Guid? CurrentProjectId { get; set; }
    public ExamProjectDto? CurrentProject { get; set; }

    public IndexModel(
        IExamListAppService examListAppService,
        IExamProjectAppService examProjectAppService,
        IExamTaskAppService examTaskAppService,
        Volo.Abp.Domain.Repositories.IRepository<DATERP.Examination.Examination.ExamSubject, Guid> examSubjectRepository,
        Microsoft.AspNetCore.Hosting.IWebHostEnvironment webHostEnvironment)
    {
        _examListAppService = examListAppService;
        _examProjectAppService = examProjectAppService;
        _examTaskAppService = examTaskAppService;
        _examSubjectRepository = examSubjectRepository;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<IActionResult> OnGetAsync(Guid? projectId = null)
    {
        if (ExamListId == Guid.Empty)
        {
            return RedirectToPage("/Examination/ExamSubjects/Index");
        }

        // Load ExamList
        ExamList = await _examListAppService.GetAsync(ExamListId);
        if (ExamList == null)
        {
            return NotFound();
        }

        // Load Projects for this ExamList
        var projectResult = await _examProjectAppService.GetListAsync(new Volo.Abp.Application.Dtos.PagedAndSortedResultRequestDto
        {
            MaxResultCount = 100,
            SkipCount = 0,
            Sorting = "Order asc"
        });
        Projects = projectResult.Items
            .Where(p => p.ExamListId == ExamListId)
            .OrderBy(p => p.Order)
            .ToList();

        // Set current project - use projectId from URL if provided, otherwise use first
        if (Projects.Any())
        {
            if (projectId.HasValue && Projects.Any(p => p.Id == projectId.Value))
            {
                CurrentProject = Projects.First(p => p.Id == projectId.Value);
            }
            else
            {
                CurrentProject = Projects.First();
            }
            CurrentProjectId = CurrentProject.Id;


            // Load Tasks for current project
            var taskResult = await _examTaskAppService.GetListByProjectIdAsync(CurrentProjectId.Value);
            Tasks = taskResult.Items.ToList();
        }

        return Page();
    }

    public async Task<JsonResult> OnGetProjectTasksAsync(Guid projectId)
    {
        var taskResult = await _examTaskAppService.GetListByProjectIdAsync(projectId);
        var tasks = taskResult.Items.ToList();

        return new JsonResult(tasks);
    }

    public async Task<IActionResult> OnGetDownloadResourceAsync(Guid projectId)
    {
        // 1. Get Project
        var project = await _examProjectAppService.GetAsync(projectId);
        if (project == null) return NotFound();

        // 2. Get List
        var list = await _examListAppService.GetAsync(project.ExamListId);
        if (list == null) return NotFound();

        // 3. Get Subject
        var subject = await _examSubjectRepository.GetAsync(list.SubjectId);
        if (subject == null) return NotFound();

        // 4. Construct Path: wwwroot/exam-data/{SubjectCode}/List{ListOrder}/Project{ProjectOrder}.docx
        // Standardize path using Path.Combine for OS compatibility
        var webRootPath = _webHostEnvironment.WebRootPath;
        var relativePath = System.IO.Path.Combine(
            "exam-data",
            subject.Code,
            $"List{list.Order}",
            $"Project{project.Order}.docx");

        var filePath = System.IO.Path.Combine(webRootPath, relativePath);

        if (!System.IO.File.Exists(filePath))
        {
            return NotFound($"File not found at: {relativePath}");
        }

        // 5. Serve File
        var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
        return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"Project{project.Order}.docx");
    }
}
