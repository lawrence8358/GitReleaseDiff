using GitReleaseDiff.Models;
using Xunit;

namespace GitReleaseDiff.Tests;

/// <summary>
/// FileDiffInfo 模型的單元測試
/// </summary>
public class FileDiffInfoTests
{
    /// <summary>
    /// 測試 Create 方法能正確解析檔案路徑
    /// </summary>
    [Fact]
    public void Create_WithValidPath_ShouldParseCorrectly()
    {
        // Arrange
        var filePath = "src/Controllers/HomeController.cs";
        var status = FileChangeStatus.Modified;

        // Act
        var result = FileDiffInfo.Create(filePath, status);

        // Assert
        Assert.Equal("src/Controllers/HomeController.cs", result.FilePath);
        Assert.Equal("src/Controllers", result.FolderPath);
        Assert.Equal("HomeController.cs", result.FileName);
        Assert.Equal(FileChangeStatus.Modified, result.Status);
    }

    /// <summary>
    /// 測試 Create 方法能正確處理根目錄檔案
    /// </summary>
    [Fact]
    public void Create_WithRootFile_ShouldHaveEmptyFolderPath()
    {
        // Arrange
        var filePath = "README.md";
        var status = FileChangeStatus.Added;

        // Act
        var result = FileDiffInfo.Create(filePath, status);

        // Assert
        Assert.Equal("README.md", result.FilePath);
        Assert.Equal(string.Empty, result.FolderPath);
        Assert.Equal("README.md", result.FileName);
    }

    /// <summary>
    /// 測試 Create 方法能正確處理反斜線路徑
    /// </summary>
    [Fact]
    public void Create_WithBackslashPath_ShouldNormalize()
    {
        // Arrange
        var filePath = "src\\Models\\User.cs";
        var status = FileChangeStatus.Modified;

        // Act
        var result = FileDiffInfo.Create(filePath, status);

        // Assert
        Assert.Equal("src/Models/User.cs", result.FilePath);
        Assert.Equal("src/Models", result.FolderPath);
        Assert.Equal("User.cs", result.FileName);
    }

    /// <summary>
    /// 測試狀態描述是否正確
    /// </summary>
    [Theory]
    [InlineData(FileChangeStatus.Added, "新增")]
    [InlineData(FileChangeStatus.Modified, "修改")]
    [InlineData(FileChangeStatus.Deleted, "刪除")]
    [InlineData(FileChangeStatus.Renamed, "重新命名")]
    [InlineData(FileChangeStatus.Copied, "複製")]
    public void StatusDescription_ShouldReturnCorrectChineseText(FileChangeStatus status, string expectedDescription)
    {
        // Arrange
        var diff = FileDiffInfo.Create("test.cs", status);

        // Act
        var description = diff.StatusDescription;

        // Assert
        Assert.Equal(expectedDescription, description);
    }

    /// <summary>
    /// 測試重新命名時包含舊路徑
    /// </summary>
    [Fact]
    public void Create_WithRename_ShouldIncludeOldPath()
    {
        // Arrange
        var newPath = "src/NewFile.cs";
        var oldPath = "src/OldFile.cs";
        var status = FileChangeStatus.Renamed;

        // Act
        var result = FileDiffInfo.Create(newPath, status, oldPath);

        // Assert
        Assert.Equal(newPath, result.FilePath);
        Assert.Equal(oldPath, result.OldFilePath);
    }
}
