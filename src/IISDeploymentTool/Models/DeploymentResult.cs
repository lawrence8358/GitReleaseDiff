namespace IISDeploymentTool.Models;

/// <summary>
/// 部署結果模型
/// </summary>
public class DeploymentResult
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// 失敗的步驟（如果有）
    /// </summary>
    public DeploymentStep? FailedAtStep { get; set; }

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 備份檔案路徑（用於回滾）
    /// </summary>
    public string? BackupFilePath { get; set; }

    /// <summary>
    /// 同步的檔案數量
    /// </summary>
    public int SyncedFileCount { get; set; }

    /// <summary>
    /// 開始時間
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// 結束時間
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 執行時長
    /// </summary>
    public TimeSpan Duration => EndTime.HasValue
        ? EndTime.Value - StartTime
        : TimeSpan.Zero;
}
