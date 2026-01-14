using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace DATERP.WinForms;

/// <summary>
/// Service chấm điểm bài thi Word sử dụng OpenXML.
/// </summary>
public class WinFormsGradingService
{
    /// <summary>
    /// Chấm điểm một Project dựa trên file bài làm và danh sách cấu hình Task.
    /// </summary>
    /// <param name="filePath">Đường dẫn file .docx của thí sinh</param>
    /// <param name="taskConfigs">Danh sách cấu hình GradingConfig (JSON) cho từng Task</param>
    /// <returns>Danh sách kết quả từng Task (Đạt/Không đạt)</returns>
    public List<TaskGradingResult> GradeProject(string filePath, List<string?> taskConfigs)
    {
        var results = new List<TaskGradingResult>();

        if (!File.Exists(filePath))
        {
            // Trả về tất cả các Task đều Fail nếu file không tồn tại
            for (int i = 0; i < taskConfigs.Count; i++)
            {
                results.Add(new TaskGradingResult { TaskOrder = i + 1, Passed = false, Reason = "File không tồn tại" });
            }
            return results;
        }

        try
        {
            using var doc = WordprocessingDocument.Open(filePath, false);
            if (doc.MainDocumentPart?.Document?.Body == null)
            {
                for (int i = 0; i < taskConfigs.Count; i++)
                {
                    results.Add(new TaskGradingResult { TaskOrder = i + 1, Passed = false, Reason = "File Word không hợp lệ" });
                }
                return results;
            }

            var body = doc.MainDocumentPart.Document.Body;

            int order = 1;
            foreach (var config in taskConfigs)
            {
                var result = GradeTask(doc, body, config, order);
                results.Add(result);
                order++;
            }
        }
        catch (Exception ex)
        {
            for (int i = 0; i < taskConfigs.Count; i++)
            {
                results.Add(new TaskGradingResult { TaskOrder = i + 1, Passed = false, Reason = $"Lỗi: {ex.Message}" });
            }
        }

        return results;
    }

    /// <summary>
    /// Chấm điểm một Task dựa trên cấu hình JSON.
    /// </summary>
    private TaskGradingResult GradeTask(WordprocessingDocument doc, Body body, string? configJson, int order)
    {
        var result = new TaskGradingResult { TaskOrder = order, Passed = false };

        if (string.IsNullOrWhiteSpace(configJson))
        {
            result.Reason = "Không có cấu hình chấm điểm";
            return result;
        }

        try
        {
            using var jsonDoc = JsonDocument.Parse(configJson);
            var root = jsonDoc.RootElement;

            if (!root.TryGetProperty("Type", out var typeElement))
            {
                result.Reason = "Cấu hình thiếu Type";
                return result;
            }

            string taskType = typeElement.GetString() ?? "";

            switch (taskType)
            {
                case "Property":
                    result = GradePropertyTask(doc, root, order);
                    break;
                case "ParagraphFormat":
                    result = GradeParagraphFormatTask(body, root, order);
                    break;
                case "TableSort":
                    result = GradeTableSortTask(body, root, order);
                    break;
                case "ListLevel":
                    result = GradeListLevelTask(body, root, order);
                    break;
                case "3DModel":
                    result = Grade3DModelTask(body, root, order);
                    break;
                case "ArtisticEffect":
                    result = GradeArtisticEffectTask(body, root, order);
                    break;
                default:
                    result.Reason = $"Loại Task không hỗ trợ: {taskType}";
                    break;
            }
        }
        catch (JsonException ex)
        {
            result.Reason = $"Lỗi parse JSON: {ex.Message}";
        }

        return result;
    }

    #region Task Grading Logic

    /// <summary>
    /// Task 1: Kiểm tra thuộc tính file (Category, Keywords, etc.)
    /// </summary>
    private TaskGradingResult GradePropertyTask(WordprocessingDocument doc, JsonElement config, int order)
    {
        var result = new TaskGradingResult { TaskOrder = order, Passed = false };

        string key = config.GetProperty("Key").GetString() ?? "";
        string expectedValue = config.GetProperty("Value").GetString() ?? "";
        string matchType = config.TryGetProperty("Match", out var m) ? m.GetString() ?? "Equals" : "Equals";

        var props = doc.PackageProperties;
        string? actualValue = key switch
        {
            "Category" => props.Category,
            "Keywords" => props.Keywords,
            "Subject" => props.Subject,
            "Title" => props.Title,
            _ => null
        };

        if (actualValue == null)
        {
            result.Reason = $"Thuộc tính {key} không tồn tại";
            return result;
        }

        bool match = matchType switch
        {
            "Contains" => actualValue.Contains(expectedValue, StringComparison.OrdinalIgnoreCase),
            "Equals" => actualValue.Equals(expectedValue, StringComparison.OrdinalIgnoreCase),
            _ => false
        };

        result.Passed = match;
        result.Reason = match ? "Đạt" : $"{key} = '{actualValue}' không khớp '{expectedValue}'";
        return result;
    }

    /// <summary>
    /// Task 2: Kiểm tra định dạng đoạn văn (Format Painter)
    /// </summary>
    private TaskGradingResult GradeParagraphFormatTask(Body body, JsonElement config, int order)
    {
        var result = new TaskGradingResult { TaskOrder = order, Passed = false };

        string headingText = config.GetProperty("HeadingText").GetString() ?? "";
        int refOrder = config.GetProperty("ReferenceParagraphOrder").GetInt32();
        int targetOrder = config.GetProperty("TargetParagraphOrder").GetInt32();

        // Tìm các đoạn văn sau Heading
        var paragraphs = body.Descendants<Paragraph>().ToList();
        int headingIndex = paragraphs.FindIndex(p => p.InnerText.Contains(headingText, StringComparison.OrdinalIgnoreCase));

        if (headingIndex == -1)
        {
            result.Reason = $"Không tìm thấy Heading: {headingText}";
            return result;
        }

        // Lấy đoạn tham chiếu và đoạn mục tiêu (bỏ qua các heading/empty)
        var contentParagraphs = paragraphs.Skip(headingIndex + 1)
            .Where(p => !string.IsNullOrWhiteSpace(p.InnerText))
            .ToList();

        if (contentParagraphs.Count < targetOrder)
        {
            result.Reason = "Không đủ đoạn văn để so sánh";
            return result;
        }

        var refPara = contentParagraphs[refOrder - 1];
        var targetPara = contentParagraphs[targetOrder - 1];

        // So sánh ParagraphProperties
        var refProps = refPara.ParagraphProperties?.OuterXml ?? "";
        var targetProps = targetPara.ParagraphProperties?.OuterXml ?? "";

        // So sánh Run Properties của run đầu tiên
        var refRunProps = refPara.Descendants<Run>().FirstOrDefault()?.RunProperties?.OuterXml ?? "";
        var targetRunProps = targetPara.Descendants<Run>().FirstOrDefault()?.RunProperties?.OuterXml ?? "";

        bool propsMatch = refProps == targetProps && refRunProps == targetRunProps;

        result.Passed = propsMatch;
        result.Reason = propsMatch ? "Đạt" : "Định dạng đoạn 2 không khớp đoạn 1";
        return result;
    }

    /// <summary>
    /// Task 3: Kiểm tra bảng đã được sắp xếp
    /// </summary>
    private TaskGradingResult GradeTableSortTask(Body body, JsonElement config, int order)
    {
        var result = new TaskGradingResult { TaskOrder = order, Passed = false };

        string headingText = config.GetProperty("HeadingText").GetString() ?? "";

        // Tìm bảng gần Heading nhất
        var elements = body.ChildElements.ToList();
        int headingIndex = -1;
        for (int i = 0; i < elements.Count; i++)
        {
            if (elements[i] is Paragraph p && p.InnerText.Contains(headingText, StringComparison.OrdinalIgnoreCase))
            {
                headingIndex = i;
                break;
            }
        }

        if (headingIndex == -1)
        {
            result.Reason = $"Không tìm thấy Heading: {headingText}";
            return result;
        }

        // Tìm Table đầu tiên sau Heading
        Table? table = null;
        for (int i = headingIndex + 1; i < elements.Count; i++)
        {
            if (elements[i] is Table t)
            {
                table = t;
                break;
            }
        }

        if (table == null)
        {
            result.Reason = "Không tìm thấy bảng";
            return result;
        }

        // Đọc dữ liệu bảng (bỏ header row)
        var rows = table.Descendants<TableRow>().Skip(1).ToList();
        if (rows.Count < 2)
        {
            result.Passed = true; // Chỉ có 1 hàng thì mặc định đã sắp xếp
            result.Reason = "Đạt (ít hàng)";
            return result;
        }

        // Kiểm tra sắp xếp theo cột đầu tiên (Geologic period)
        var sortColumns = config.GetProperty("SortColumns").EnumerateArray().ToList();
        int primaryColIndex = sortColumns[0].GetProperty("ColIndex").GetInt32() - 1;

        bool sorted = true;
        for (int i = 0; i < rows.Count - 1 && sorted; i++)
        {
            var cells1 = rows[i].Descendants<TableCell>().ToList();
            var cells2 = rows[i + 1].Descendants<TableCell>().ToList();

            if (cells1.Count <= primaryColIndex || cells2.Count <= primaryColIndex) continue;

            string val1 = cells1[primaryColIndex].InnerText.Trim();
            string val2 = cells2[primaryColIndex].InnerText.Trim();

            if (string.Compare(val1, val2, StringComparison.OrdinalIgnoreCase) > 0)
            {
                sorted = false;
            }
        }

        result.Passed = sorted;
        result.Reason = sorted ? "Đạt" : "Bảng chưa được sắp xếp đúng";
        return result;
    }

    /// <summary>
    /// Task 4: Kiểm tra cấp độ danh sách
    /// </summary>
    private TaskGradingResult GradeListLevelTask(Body body, JsonElement config, int order)
    {
        var result = new TaskGradingResult { TaskOrder = order, Passed = false };

        string targetText = config.GetProperty("TargetText").GetString() ?? "";
        int expectedLevel = config.GetProperty("Level").GetInt32();

        var paragraph = body.Descendants<Paragraph>()
            .FirstOrDefault(p => p.InnerText.Contains(targetText, StringComparison.OrdinalIgnoreCase));

        if (paragraph == null)
        {
            result.Reason = $"Không tìm thấy đoạn văn: {targetText}";
            return result;
        }

        var numProps = paragraph.ParagraphProperties?.NumberingProperties;
        if (numProps == null)
        {
            result.Reason = "Đoạn văn không có định dạng danh sách";
            return result;
        }

        int? actualLevel = numProps.NumberingLevelReference?.Val?.Value;
        if (actualLevel == null)
        {
            result.Reason = "Không xác định được Level";
            return result;
        }

        result.Passed = actualLevel == expectedLevel;
        result.Reason = result.Passed ? "Đạt" : $"Level = {actualLevel + 1}, cần Level {expectedLevel + 1}";
        return result;
    }

    /// <summary>
    /// Task 5: Kiểm tra 3D Model và Wrapping
    /// </summary>
    private TaskGradingResult Grade3DModelTask(Body body, JsonElement config, int order)
    {
        var result = new TaskGradingResult { TaskOrder = order, Passed = false };

        string expectedWrapping = config.GetProperty("Wrapping").GetString() ?? "";

        // OpenXML: 3D Models được lưu dưới dạng Drawing > Inline/Anchor > Model3D (hoặc AlternateContent)
        // Kiểm tra có Drawing nào chứa 3D không
        var drawings = body.Descendants<Drawing>().ToList();

        bool found3D = false;
        bool correctWrapping = false;

        foreach (var drawing in drawings)
        {
            var outerXml = drawing.OuterXml;

            // 3D Model thường có namespace model3d hoặc mc:AlternateContent với model3d
            if (outerXml.Contains("model3d", StringComparison.OrdinalIgnoreCase) ||
                outerXml.Contains("3dmodel", StringComparison.OrdinalIgnoreCase))
            {
                found3D = true;

                // Kiểm tra Wrapping (Anchor = có wrap, Inline = không)
                if (drawing.FirstChild is DocumentFormat.OpenXml.Drawing.Wordprocessing.Anchor anchor)
                {
                    // Anchor = có text wrapping
                    var wrapSquare = anchor.Descendants<DocumentFormat.OpenXml.Drawing.Wordprocessing.WrapSquare>().Any();
                    if (expectedWrapping.Equals("Square", StringComparison.OrdinalIgnoreCase) && wrapSquare)
                    {
                        correctWrapping = true;
                    }
                }
            }
        }

        if (!found3D)
        {
            result.Reason = "Không tìm thấy 3D Model";
            return result;
        }

        result.Passed = correctWrapping;
        result.Reason = correctWrapping ? "Đạt" : $"Text Wrapping không phải {expectedWrapping}";
        return result;
    }

    /// <summary>
    /// Task 6: Kiểm tra Artistic Effect trên hình ảnh
    /// </summary>
    private TaskGradingResult GradeArtisticEffectTask(Body body, JsonElement config, int order)
    {
        var result = new TaskGradingResult { TaskOrder = order, Passed = false };

        string effectName = config.GetProperty("EffectName").GetString() ?? "";
        string contextText = config.TryGetProperty("ContextText", out var ct) ? ct.GetString() ?? "" : "";

        // Tìm vùng chứa contextText
        var paragraphs = body.Descendants<Paragraph>().ToList();
        int contextIndex = paragraphs.FindIndex(p => p.InnerText.Contains(contextText, StringComparison.OrdinalIgnoreCase));

        // Tìm Drawing (hình ảnh) trong hoặc gần vùng context
        var drawings = body.Descendants<Drawing>().ToList();

        bool foundEffect = false;

        foreach (var drawing in drawings)
        {
            var outerXml = drawing.OuterXml;

            // Artistic Effects trong OpenXML thường nằm trong a:extLst hoặc a14:artisticPencilSketch
            if (outerXml.Contains("artisticPencilSketch", StringComparison.OrdinalIgnoreCase) ||
                (effectName.Equals("PencilSketch", StringComparison.OrdinalIgnoreCase) &&
                 outerXml.Contains("a14:imgEffect", StringComparison.OrdinalIgnoreCase)))
            {
                foundEffect = true;
                break;
            }
        }

        result.Passed = foundEffect;
        result.Reason = foundEffect ? "Đạt" : $"Không tìm thấy hiệu ứng {effectName}";
        return result;
    }

    #endregion

    #region Legacy Support

    /// <summary>
    /// Chấm điểm đơn giản (Legacy - dùng cho Mock)
    /// </summary>
    public int GradeSession(List<string> filePaths)
    {
        int totalScore = 0;
        if (filePaths == null || filePaths.Count == 0) return 0;

        foreach (var path in filePaths)
        {
            totalScore += GradeFileLegacy(path);
        }

        return totalScore / filePaths.Count;
    }

    private int GradeFileLegacy(string filePath)
    {
        if (!File.Exists(filePath)) return 0;

        try
        {
            var info = new FileInfo(filePath);
            if (info.Length < 1000) return 100;
            return 850;
        }
        catch
        {
            return 0;
        }
    }

    #endregion
}

/// <summary>
/// Kết quả chấm điểm của một Task
/// </summary>
public class TaskGradingResult
{
    public int TaskOrder { get; set; }
    public bool Passed { get; set; }
    public string Reason { get; set; } = "";
}
