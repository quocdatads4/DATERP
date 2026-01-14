using System.Collections.Generic;

namespace Education.Models;

public class EntityListViewModel
{
    public string? Title { get; set; }
    public string? CreateButtonText { get; set; }
    public string? CreateButtonId { get; set; }
    public string CreateButtonIcon { get; set; } = "ti ti-plus";
    public string? TableId { get; set; }
    public List<EntityStatsItem> Stats { get; set; } = new List<EntityStatsItem>();
}

public class EntityStatsItem
{
    public string? Title { get; set; }
    public string? Value { get; set; }
    public string? Icon { get; set; }
    public string ColorClass { get; set; } = "primary"; // primary, success, warning, info, danger
}
