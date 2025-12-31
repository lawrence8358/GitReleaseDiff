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

    /// <summary>
    /// 建置結果資料夾
    /// </summary>
    public string BuildOutputFolder { get; set; } = string.Empty;

    /// <summary>
    /// 預計上版的資料夾
    /// </summary>
    public string DeploymentFolder { get; set; } = string.Empty;

    /// <summary>
    /// 專案路徑前綴（用於多專案方案，可選）
    /// </summary>
    public string ProjectPathPrefix { get; set; } = string.Empty;
}
