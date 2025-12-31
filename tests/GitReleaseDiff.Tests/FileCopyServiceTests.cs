using GitReleaseDiff.Models;
using GitReleaseDiff.Services;
using Xunit;

namespace GitReleaseDiff.Tests;

/// <summary>
/// FileCopyService 的單元測試
/// </summary>
public class FileCopyServiceTests : IDisposable
{
    private readonly FileCopyService _service;
    private readonly string _tempBuildFolder;
    private readonly string _tempDeployFolder;

    public FileCopyServiceTests()
    {
        _service = new FileCopyService();
        _tempBuildFolder = Path.Combine(Path.GetTempPath(), "GitReleaseDiff_Build_" + Guid.NewGuid());
        _tempDeployFolder = Path.Combine(Path.GetTempPath(), "GitReleaseDiff_Deploy_" + Guid.NewGuid());

        Directory.CreateDirectory(_tempBuildFolder);
        Directory.CreateDirectory(_tempDeployFolder);
    }

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(_tempBuildFolder)) Directory.Delete(_tempBuildFolder, true);
            if (Directory.Exists(_tempDeployFolder)) Directory.Delete(_tempDeployFolder, true);
        }
        catch
        {
            // 忽略清理錯誤
        }
    }

    [Fact]
    public void CopyMatchedFiles_ShouldCopyFiles_WhenFilesExist()
    {
        // Arrange
        // 建立測試檔案
        var fileName = "test.txt";
        var content = "Hello World";
        File.WriteAllText(Path.Combine(_tempBuildFolder, fileName), content);

        var diffs = new List<FileDiffInfo>
        {
            FileDiffInfo.Create(fileName, FileChangeStatus.Modified)
        };

        // Act
        var result = _service.CopyMatchedFiles(diffs, _tempBuildFolder, _tempDeployFolder);

        // Assert
        Assert.Equal(1, result.CopiedCount);
        Assert.Single(result.CopiedFiles);
        Assert.Empty(result.NotFoundFiles);
        Assert.True(File.Exists(Path.Combine(_tempDeployFolder, fileName)));
        Assert.Equal(content, File.ReadAllText(Path.Combine(_tempDeployFolder, fileName)));
    }

    [Fact]
    public void CopyMatchedFiles_ShouldMaintainDirectoryStructure()
    {
        // Arrange
        var relativePath = @"folder\subfolder\test.js";
        var fullSourcePath = Path.Combine(_tempBuildFolder, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(fullSourcePath)!);
        File.WriteAllText(fullSourcePath, "var a = 1;");

        var diffs = new List<FileDiffInfo>
        {
            FileDiffInfo.Create(relativePath.Replace('\\', '/'), FileChangeStatus.Added)
        };

        // Act
        var result = _service.CopyMatchedFiles(diffs, _tempBuildFolder, _tempDeployFolder);

        // Assert
        Assert.Equal(1, result.CopiedCount);
        var expectedDestPath = Path.Combine(_tempDeployFolder, relativePath);
        Assert.True(File.Exists(expectedDestPath));
    }

    [Fact]
    public void CopyMatchedFiles_ShouldHandleNotFoundFiles()
    {
        // Arrange
        var diffs = new List<FileDiffInfo>
        {
            FileDiffInfo.Create("missing.css", FileChangeStatus.Modified)
        };

        // Act
        var result = _service.CopyMatchedFiles(diffs, _tempBuildFolder, _tempDeployFolder);

        // Assert
        Assert.Equal(0, result.CopiedCount);
        Assert.Single(result.NotFoundFiles);
        Assert.Contains("missing.css", result.NotFoundFiles[0].Replace('\\', '/'));
    }

    [Fact]
    public void CopyMatchedFiles_ShouldSkipDeletedFiles()
    {
        // Arrange
        var diffs = new List<FileDiffInfo>
        {
            FileDiffInfo.Create("deleted.txt", FileChangeStatus.Deleted)
        };

        // Act
        var result = _service.CopyMatchedFiles(diffs, _tempBuildFolder, _tempDeployFolder);

        // Assert
        Assert.Equal(0, result.CopiedCount);
        Assert.Empty(result.NotFoundFiles);
    }

    [Theory]
    [InlineData("", "path")]
    [InlineData("path", "")]
    [InlineData(null, "path")]
    public void CopyMatchedFiles_ShouldThrowArgumentException_WhenPathsInvalid(string? buildPath, string? deployPath)
    {
        // Arrange
        var diffs = new List<FileDiffInfo>();

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            _service.CopyMatchedFiles(diffs, buildPath!, deployPath!));
    }

    [Fact]
    public void CopyMatchedFiles_ShouldThrowDirectoryNotFound_WhenBuildFolderDoesNotExist()
    {
        // Arrange
        var diffs = new List<FileDiffInfo>();
        var nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        // Act & Assert
        Assert.Throws<DirectoryNotFoundException>(() =>
            _service.CopyMatchedFiles(diffs, nonExistentPath, _tempDeployFolder));
    }

    [Fact]
    public void CopyMatchedFiles_ShouldStripProjectPrefix_WhenPrefixProvided()
    {
        // Arrange
        var fileName = "test.css";
        var content = "body { color: red; }";

        // 建置資料夾中的檔案路徑（不含前綴）
        File.WriteAllText(Path.Combine(_tempBuildFolder, fileName), content);

        // Git 差異中的檔案路徑（含專案前綴）
        var diffs = new List<FileDiffInfo>
        {
            FileDiffInfo.Create("ProjectName/" + fileName, FileChangeStatus.Modified)
        };

        // Act
        var result = _service.CopyMatchedFiles(diffs, _tempBuildFolder, _tempDeployFolder, "ProjectName");

        // Assert
        Assert.Equal(1, result.CopiedCount);
        Assert.Single(result.CopiedFiles);
        Assert.Empty(result.NotFoundFiles);
        Assert.True(File.Exists(Path.Combine(_tempDeployFolder, fileName)));
    }

    [Fact]
    public void CopyMatchedFiles_ShouldStripProjectPrefixWithTrailingSlash()
    {
        // Arrange
        var fileName = "config.json";
        File.WriteAllText(Path.Combine(_tempBuildFolder, fileName), "{}");

        var diffs = new List<FileDiffInfo>
        {
            FileDiffInfo.Create("MyProject/" + fileName, FileChangeStatus.Added)
        };

        // Act - 前綴帶有斜線
        var result = _service.CopyMatchedFiles(diffs, _tempBuildFolder, _tempDeployFolder, "MyProject/");

        // Assert
        Assert.Equal(1, result.CopiedCount);
    }

    [Fact]
    public void CopyMatchedFiles_ShouldKeepOriginalPath_WhenPrefixNotMatch()
    {
        // Arrange
        var fileName = "file.js";
        File.WriteAllText(Path.Combine(_tempBuildFolder, fileName), "console.log('test');");

        // Git 差異中的路徑與前綴不符
        var diffs = new List<FileDiffInfo>
        {
            FileDiffInfo.Create("OtherProject/" + fileName, FileChangeStatus.Modified)
        };

        // Act - 前綴不匹配，使用原始路徑搜尋
        var result = _service.CopyMatchedFiles(diffs, _tempBuildFolder, _tempDeployFolder, "ProjectName");

        // Assert - 找不到檔案（因為前綴不匹配，路徑仍保留 OtherProject/）
        Assert.Equal(0, result.CopiedCount);
        Assert.Single(result.NotFoundFiles);
    }

    [Fact]
    public void CopyMatchedFiles_ShouldWorkNormally_WhenPrefixIsEmpty()
    {
        // Arrange
        var fileName = "app.js";
        File.WriteAllText(Path.Combine(_tempBuildFolder, fileName), "var x = 1;");

        var diffs = new List<FileDiffInfo>
        {
            FileDiffInfo.Create(fileName, FileChangeStatus.Modified)
        };

        // Act - 空前綴
        var result = _service.CopyMatchedFiles(diffs, _tempBuildFolder, _tempDeployFolder, "");

        // Assert
        Assert.Equal(1, result.CopiedCount);
    }

    [Fact]
    public void CopyMatchedFiles_ShouldHandleNestedPathWithPrefix()
    {
        // Arrange
        var relativePath = @"Controllers\HomeController.cs";
        var fullSourcePath = Path.Combine(_tempBuildFolder, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(fullSourcePath)!);
        File.WriteAllText(fullSourcePath, "public class HomeController {}");

        // Git 路徑包含專案前綴
        var gitPath = "AFA_EmployerQualification/Controllers/HomeController.cs";
        var diffs = new List<FileDiffInfo>
        {
            FileDiffInfo.Create(gitPath, FileChangeStatus.Modified)
        };

        // Act
        var result = _service.CopyMatchedFiles(diffs, _tempBuildFolder, _tempDeployFolder, "AFA_EmployerQualification");

        // Assert
        Assert.Equal(1, result.CopiedCount);
        var expectedPath = Path.Combine(_tempDeployFolder, relativePath);
        Assert.True(File.Exists(expectedPath));
    }
}
