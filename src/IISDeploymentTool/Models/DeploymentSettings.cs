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
    /// 應用程式池名稱（選填，若有提供則會在部署時停止/啟動應用程式池）
    /// </summary>
    public string ApplicationPoolName { get; set; } = string.Empty;

    /// <summary>
    /// 是否在部署前進行完整路徑驗證
    /// </summary>
    public bool ValidatePathsBeforeDeploy { get; set; } = true;

    /// <summary>
    /// 上次部署時間
    /// </summary>
    public DateTime? LastDeploymentTime { get; set; }

    /// <summary>
    /// 視窗寬度
    /// </summary>
    public int WindowWidth { get; set; } = 800;

    /// <summary>
    /// 視窗高度
    /// </summary>
    public int WindowHeight { get; set; } = 600;

    /// <summary>
    /// 是否最大化
    /// </summary>
    public bool IsMaximized { get; set; }
}
