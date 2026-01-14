namespace Education.Models;

/// <summary>
/// ViewModel for the reusable _AdminTable.cshtml partial view
/// </summary>
public class AdminTableViewModel
{
    /// <summary>
    /// Title displayed in the card header
    /// </summary>
    public string Title { get; set; } = "Danh sách";

    /// <summary>
    /// Icon class for the title (e.g., "ti ti-category-2")
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// Unique ID for the DataTable element
    /// </summary>
    public string TableId { get; set; } = "DataTable";

    /// <summary>
    /// ID for the create button
    /// </summary>
    public string CreateButtonId { get; set; } = "CreateButton";

    /// <summary>
    /// Text for the create button
    /// </summary>
    public string CreateButtonText { get; set; } = "Thêm mới";

    /// <summary>
    /// Icon class for the create button
    /// </summary>
    public string CreateButtonIcon { get; set; } = "ti ti-plus";

    /// <summary>
    /// Whether to show the create button
    /// </summary>
    public bool ShowCreateButton { get; set; } = true;

    /// <summary>
    /// Whether to show the export button
    /// </summary>
    public bool ShowExportButton { get; set; } = false;

    /// <summary>
    /// Whether to show the search bar
    /// </summary>
    public bool ShowSearchBar { get; set; } = true;

    /// <summary>
    /// Placeholder text for the search input
    /// </summary>
    public string SearchPlaceholder { get; set; } = "Tìm kiếm...";

    /// <summary>
    /// Optional stats cards to display above the table
    /// </summary>
    public List<StatCardModel>? Stats { get; set; }

    /// <summary>
    /// Optional filter dropdowns
    /// </summary>
    public List<FilterOptionModel>? FilterOptions { get; set; }
}

/// <summary>
/// Model for stats card display
/// </summary>
public class StatCardModel
{
    public string Title { get; set; } = "";
    public string Value { get; set; } = "0";
    public string Icon { get; set; } = "ti ti-chart-bar";
    public string ColorClass { get; set; } = "primary";
}

/// <summary>
/// Model for filter dropdown options
/// </summary>
public class FilterOptionModel
{
    public string Key { get; set; } = "";
    public string Label { get; set; } = "";
    public List<SelectOption> Options { get; set; } = new();
}

/// <summary>
/// Simple select option model
/// </summary>
public class SelectOption
{
    public string Value { get; set; } = "";
    public string Text { get; set; } = "";
}
