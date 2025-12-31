namespace GitReleaseDiff.Models;

/// <summary>
/// 應用程式設定模型
/// </summary>
public class AppSettings
{
    /// <summary>
    /// Git 網址
    /// </summary>
    public string GitUrl { get; set; } = string.Empty;

    /// <summary>
    /// 基準 Commit ID
    /// </summary>
    public string BaseCommitId { get; set; } = string.Empty;

    /// <summary>
    /// 比對 Commit ID
    /// </summary>
    public string CompareCommitId { get; set; } = string.Empty;

    /// <summary>
    /// Personal Access Token (PAT)
    /// </summary>
    public string PersonalAccessToken { get; set; } = string.Empty;
}
