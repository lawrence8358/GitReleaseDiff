using GitReleaseDiff.Services;
using Xunit;

namespace GitReleaseDiff.Tests;

/// <summary>
/// GitService 的單元測試
/// </summary>
public class GitServiceTests
{
    /// <summary>
    /// 測試有效的 Commit ID 格式驗證
    /// </summary>
    [Theory]
    [InlineData("6d7b", true)]                                          // 最短有效短碼
    [InlineData("6d7bdd0e", true)]                                      // 8位短碼
    [InlineData("6d7bdd0e040be168463fa6f92d82c41a83465612", true)]      // 40位完整碼
    [InlineData("ABC123", true)]                                        // 大寫字母
    [InlineData("abc123", true)]                                        // 小寫字母
    public void IsValidCommitId_WithValidId_ShouldReturnTrue(string commitId, bool expected)
    {
        // Act
        var result = GitService.IsValidCommitId(commitId);

        // Assert
        Assert.Equal(expected, result);
    }

    /// <summary>
    /// 測試無效的 Commit ID 格式驗證
    /// </summary>
    [Theory]
    [InlineData("", false)]                                              // 空字串
    [InlineData("   ", false)]                                           // 空白
    [InlineData("abc", false)]                                           // 太短
    [InlineData("ghijkl", false)]                                        // 非十六進位字元
    [InlineData("6d7bdd0e040be168463fa6f92d82c41a834656123", false)]     // 超過40位
    [InlineData("6d7b-dd0e", false)]                                     // 包含特殊字元
    public void IsValidCommitId_WithInvalidId_ShouldReturnFalse(string commitId, bool expected)
    {
        // Act
        var result = GitService.IsValidCommitId(commitId);

        // Assert
        Assert.Equal(expected, result);
    }

    /// <summary>
    /// 測試 null 的 Commit ID 應回傳 false
    /// </summary>
    [Fact]
    public void IsValidCommitId_WithNull_ShouldReturnFalse()
    {
        // Act
        var result = GitService.IsValidCommitId(null!);

        // Assert
        Assert.False(result);
    }
}
