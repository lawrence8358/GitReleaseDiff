using GitReleaseDiff.Models;
using GitReleaseDiff.Services;
using Xunit;

namespace GitReleaseDiff.Tests;

/// <summary>
/// SettingsService 的單元測試
/// </summary>
public class SettingsServiceTests
{
    /// <summary>
    /// 測試儲存和載入設定
    /// </summary>
    [Fact]
    public void SaveAndLoad_ShouldPersistSettings()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        var service = new SettingsService(tempFile);
        var settings = new AppSettings
        {
            GitUrl = "https://example.com/git",
            BaseCommitId = "abc123",
            CompareCommitId = "def456"
        };

        try
        {
            // Act
            service.Save(settings);
            var loaded = service.Load();

            // Assert
            Assert.Equal(settings.GitUrl, loaded.GitUrl);
            Assert.Equal(settings.BaseCommitId, loaded.BaseCommitId);
            Assert.Equal(settings.CompareCommitId, loaded.CompareCommitId);
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

    /// <summary>
    /// 測試載入不存在的設定檔案應回傳預設設定
    /// </summary>
    [Fact]
    public void Load_NonExistentFile_ShouldReturnDefaultSettings()
    {
        // Arrange
        var tempFile = Path.Combine(Path.GetTempPath(), $"nonexistent_{Guid.NewGuid()}.json");
        var service = new SettingsService(tempFile);

        // Act
        var loaded = service.Load();

        // Assert
        Assert.NotNull(loaded);
        Assert.Equal(string.Empty, loaded.GitUrl);
        Assert.Equal(string.Empty, loaded.BaseCommitId);
        Assert.Equal(string.Empty, loaded.CompareCommitId);
    }

    /// <summary>
    /// 測試載入損壞的設定檔案應回傳預設設定
    /// </summary>
    [Fact]
    public void Load_CorruptedFile_ShouldReturnDefaultSettings()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, "{ invalid json }}}");
        var service = new SettingsService(tempFile);

        try
        {
            // Act
            var loaded = service.Load();

            // Assert
            Assert.NotNull(loaded);
            Assert.Equal(string.Empty, loaded.GitUrl);
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

    /// <summary>
    /// 測試儲存設定時應該將 PersonalAccessToken 加密
    /// </summary>
    [Fact]
    public void Save_ShouldEncryptPersonalAccessToken()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        var service = new SettingsService(tempFile);
        var plainToken = "TEST_TOKEN_12345";
        var settings = new AppSettings
        {
            PersonalAccessToken = plainToken
        };

        try
        {
            // Act
            service.Save(settings);

            // Assert
            var json = File.ReadAllText(tempFile);

            // 確保檔案內容不包含明文 Token
            Assert.DoesNotContain(plainToken, json);

            // 確保可以正確讀回
            var loaded = service.Load();
            Assert.Equal(plainToken, loaded.PersonalAccessToken);
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
