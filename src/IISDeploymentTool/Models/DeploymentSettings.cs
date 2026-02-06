namespace IISDeploymentTool.Models;

/// <summary>
/// 部署設定模型
/// </summary>
public class DeploymentSettings
{
    /// <summary>
    /// 上版程式路徑（來源資料夾）
    /// </summary>
    public string SourceFolder { get; set; } = string.Empty;

    /// <summary>
    /// IIS 站台資料夾路徑（目標資料夾）
    /// </summary>
    public string IISFolder { get; set; } = string.Empty;

    /// <summary>
    /// 備份路徑
    /// </summary>
    public string BackupFolder { get; set; } = string.Empty;

    /// <summary>
    /// 是否在部署前進行完整路徑驗證
    /// </summary>
    public bool ValidatePathsBeforeDeploy { get; set; } = true;

    /// <summary>
    /// 上次部署時間
    /// </summary>
    public DateTime? LastDeploymentTime { get; set; }
}
