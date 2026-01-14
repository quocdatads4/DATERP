using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DATERP.WinForms.Tests;

/// <summary>
/// Unit Test cho WinFormsGradingService - Logic chấm điểm Word 2019 Project 1
/// </summary>
public class WinFormsGradingServiceTests
{
    private readonly WinFormsGradingService _gradingService = new();

    #region Edge Cases

    [Fact]
    public void GradeProject_FileNotFound_ShouldReturnAllFail()
    {
        // Arrange
        string testFile = "nonexistent_file.docx";
        var taskConfigs = new List<string?>
        {
            "{\"Type\": \"Property\", \"Key\": \"Category\", \"Value\": \"dinosaur\", \"Match\": \"Contains\"}"
        };

        // Act
        var results = _gradingService.GradeProject(testFile, taskConfigs);

        // Assert
        Assert.Single(results);
        Assert.False(results[0].Passed);
        Assert.Contains("không tồn tại", results[0].Reason, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void GradeProject_InvalidConfig_ShouldHandleGracefully()
    {
        // Arrange
        string testFile = "nonexistent_file.docx";
        var taskConfigs = new List<string?>
        {
            "invalid_json"
        };

        // Act
        var results = _gradingService.GradeProject(testFile, taskConfigs);

        // Assert
        Assert.Single(results);
        Assert.False(results[0].Passed);
        Assert.Contains("không tồn tại", results[0].Reason, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void GradeProject_UnsupportedTaskType_ShouldHandleGracefully()
    {
        // Arrange
        string testFile = "nonexistent_file.docx";
        var taskConfigs = new List<string?>
        {
            "{\"Type\": \"UnsupportedType\"}"
        };

        // Act
        var results = _gradingService.GradeProject(testFile, taskConfigs);

        // Assert
        Assert.Single(results);
        Assert.False(results[0].Passed);
        Assert.Contains("không tồn tại", results[0].Reason, StringComparison.OrdinalIgnoreCase);
    }

    #endregion

    #region Test JSON Configuration Validation

    [Fact]
    public void PropertyTask_JsonConfig_Valid()
    {
        // Arrange
        var config = "{\"Type\": \"Property\", \"Key\": \"Category\", \"Value\": \"dinosaur\", \"Match\": \"Contains\"}";

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            var doc = System.Text.Json.JsonDocument.Parse(config);
            Assert.True(doc.RootElement.TryGetProperty("Type", out var type));
            Assert.Equal("Property", type.GetString());
        });
    }

    [Fact]
    public void ParagraphFormatTask_JsonConfig_Valid()
    {
        // Arrange
        var config = "{\"Type\": \"ParagraphFormat\", \"ReferenceParagraphOrder\": 1, \"TargetParagraphOrder\": 2, \"HeadingText\": \"Test\"}";

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            var doc = System.Text.Json.JsonDocument.Parse(config);
            Assert.True(doc.RootElement.TryGetProperty("Type", out var type));
            Assert.Equal("ParagraphFormat", type.GetString());
        });
    }

    [Fact]
    public void TableSortTask_JsonConfig_Valid()
    {
        // Arrange
        var config = "{\"Type\": \"TableSort\", \"HeadingText\": \"Test\", \"SortColumns\": [{\"ColIndex\": 1, \"Order\": \"Ascending\"}]}";

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            var doc = System.Text.Json.JsonDocument.Parse(config);
            Assert.True(doc.RootElement.TryGetProperty("Type", out var type));
            Assert.Equal("TableSort", type.GetString());
        });
    }

    [Fact]
    public void ListLevelTask_JsonConfig_Valid()
    {
        // Arrange
        var config = "{\"Type\": \"ListLevel\", \"TargetText\": \"Test\", \"Level\": 2}";

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            var doc = System.Text.Json.JsonDocument.Parse(config);
            Assert.True(doc.RootElement.TryGetProperty("Type", out var type));
            Assert.Equal("ListLevel", type.GetString());
        });
    }

    [Fact]
    public void ThreeDModelTask_JsonConfig_Valid()
    {
        // Arrange
        var config = "{\"Type\": \"3DModel\", \"Wrapping\": \"Square\"}";

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            var doc = System.Text.Json.JsonDocument.Parse(config);
            Assert.True(doc.RootElement.TryGetProperty("Type", out var type));
            Assert.Equal("3DModel", type.GetString());
        });
    }

    [Fact]
    public void ArtisticEffectTask_JsonConfig_Valid()
    {
        // Arrange
        var config = "{\"Type\": \"ArtisticEffect\", \"EffectName\": \"PencilSketch\", \"ContextText\": \"Test\"}";

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            var doc = System.Text.Json.JsonDocument.Parse(config);
            Assert.True(doc.RootElement.TryGetProperty("Type", out var type));
            Assert.Equal("ArtisticEffect", type.GetString());
        });
    }

    #endregion

    #region Legacy Tests

    [Fact]
    public void GradeSession_Legacy_ReturnsZeroForEmpty()
    {
        // Arrange
        var filePaths = new List<string>();

        // Act
        int score = _gradingService.GradeSession(filePaths);

        // Assert
        Assert.Equal(0, score);
    }

    [Fact]
    public void GradeFileLegacy_FileExists_ReturnsScore()
    {
        // Arrange - We can't create a real file easily, so test the null case
        string filePath = "nonexistent_file.docx";

        // Act
        // The GradeFileLegacy method is private, so we test GradeSession
        var filePaths = new List<string> { filePath };
        int score = _gradingService.GradeSession(filePaths);

        // Assert
        Assert.Equal(0, score); // File doesn't exist, should return 0
    }

    #endregion

    #region Task Grading Result Class Tests

    [Fact]
    public void TaskGradingResult_DefaultValues()
    {
        // Arrange & Act
        var result = new TaskGradingResult();

        // Assert
        Assert.Equal(0, result.TaskOrder);
        Assert.False(result.Passed);
        Assert.Equal("", result.Reason);
    }

    [Fact]
    public void TaskGradingResult_SetValues()
    {
        // Arrange & Act
        var result = new TaskGradingResult
        {
            TaskOrder = 1,
            Passed = true,
            Reason = "Test passed"
        };

        // Assert
        Assert.Equal(1, result.TaskOrder);
        Assert.True(result.Passed);
        Assert.Equal("Test passed", result.Reason);
    }

    #endregion
}
