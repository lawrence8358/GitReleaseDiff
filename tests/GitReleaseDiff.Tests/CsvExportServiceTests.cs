using GitReleaseDiff.Models;
using GitReleaseDiff.Services;
using Xunit;

namespace GitReleaseDiff.Tests;

/// <summary>
/// CsvExportService 的單元測試
/// </summary>
public class CsvExportServiceTests
{
    private readonly CsvExportService _service;

    /// <summary>
    /// 初始化測試
    /// </summary>
    public CsvExportServiceTests()
    {
        _service = new CsvExportService();
    }

    /// <summary>
    /// 測試產生 CSV 內容包含標題列
    /// </summary>
    [Fact]
    public void GenerateCsvContent_WithHeader_ShouldIncludeHeaderRow()
    {
        // Arrange
        var diffs = new List<FileDiffInfo>
        {
            FileDiffInfo.Create("src/test.cs", FileChangeStatus.Added)
        };

        // Act
        var result = _service.GenerateCsvContent(diffs, includeHeader: true);

        // Assert
        Assert.StartsWith("資料夾路徑,檔案名稱,附檔名,狀態,完整路徑,舊檔案路徑", result);
    }

    /// <summary>
    /// 測試產生 CSV 內容不包含標題列
    /// </summary>
    [Fact]
    public void GenerateCsvContent_WithoutHeader_ShouldNotIncludeHeaderRow()
    {
        // Arrange
        var diffs = new List<FileDiffInfo>
        {
            FileDiffInfo.Create("src/test.cs", FileChangeStatus.Added)
        };

        // Act
        var result = _service.GenerateCsvContent(diffs, includeHeader: false);

        // Assert
        Assert.StartsWith("src,test.cs,.cs,新增", result);
    }

    /// <summary>
    /// 測試空清單應產生只有標題的 CSV
    /// </summary>
    [Fact]
    public void GenerateCsvContent_EmptyList_ShouldReturnOnlyHeader()
    {
        // Arrange
        var diffs = new List<FileDiffInfo>();

        // Act
        var result = _service.GenerateCsvContent(diffs, includeHeader: true);

        // Assert
        var lines = result.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        Assert.Single(lines);
        Assert.Contains("資料夾路徑", lines[0]);
    }

    /// <summary>
    /// 測試 CSV 欄位中包含逗號時的轉義
    /// </summary>
    [Fact]
    public void GenerateCsvContent_FieldWithComma_ShouldBeQuoted()
    {
        // Arrange
        var diffs = new List<FileDiffInfo>
        {
            FileDiffInfo.Create("src/folder,name/test.cs", FileChangeStatus.Modified)
        };

        // Act
        var result = _service.GenerateCsvContent(diffs, includeHeader: false);

        // Assert
        Assert.Contains("\"src/folder,name\"", result);
    }

    /// <summary>
    /// 測試 CSV 欄位中包含引號時的轉義
    /// </summary>
    [Fact]
    public void GenerateCsvContent_FieldWithQuotes_ShouldBeEscaped()
    {
        // Arrange
        var diffs = new List<FileDiffInfo>
        {
            FileDiffInfo.Create("src/test\"file.cs", FileChangeStatus.Modified)
        };

        // Act
        var result = _service.GenerateCsvContent(diffs, includeHeader: false);

        // Assert
        Assert.Contains("\"\"", result); // 引號應被轉義為兩個引號
    }

    /// <summary>
    /// 測試匯出到檔案功能
    /// </summary>
    [Fact]
    public void Export_ToFile_ShouldCreateFile()
    {
        // Arrange
        var diffs = new List<FileDiffInfo>
        {
            FileDiffInfo.Create("src/test.cs", FileChangeStatus.Added),
            FileDiffInfo.Create("src/model.cs", FileChangeStatus.Modified)
        };
        var tempFile = Path.GetTempFileName();

        try
        {
            // Act
            _service.Export(diffs, tempFile);

            // Assert
            Assert.True(File.Exists(tempFile));
            var content = File.ReadAllText(tempFile);
            Assert.Contains("資料夾路徑", content);
            Assert.Contains("test.cs", content);
            Assert.Contains("model.cs", content);
        }
        finally
        {
            // Cleanup
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }
}
