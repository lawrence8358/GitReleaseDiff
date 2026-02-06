namespace IISDeploymentTool.Models;

/// <summary>
/// 部署步驟列舉
/// </summary>
public enum DeploymentStep
{
    /// <summary>
    /// 步驟 0：準備階段 - 驗證路徑
    /// </summary>
    ValidatePaths,

    /// <summary>
    /// 步驟 1：創建 app_offline.htm
    /// </summary>
    EnableMaintenanceMode,

    /// <summary>
    /// 步驟 2：備份現有檔案
    /// </summary>
    BackupFiles,

    /// <summary>
    /// 步驟 3：同步檔案到 IIS 站台
    /// </summary>
    SyncFiles,

    /// <summary>
    /// 步驟 4：移除 app_offline.htm
    /// </summary>
    DisableMaintenanceMode,

    /// <summary>
    /// 完成
    /// </summary>
    Completed
}
