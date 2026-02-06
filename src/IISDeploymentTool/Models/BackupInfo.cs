namespace IISDeploymentTool.Models;

/// <summary>
/// 備份資訊模型
/// </summary>
public class BackupInfo
{
    /// <summary>
    /// 備份檔案完整路徑
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// 備份檔案名稱（例如：2026-02-05-01.zip）
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// 備份建立時間
    /// </summary>
    public DateTime CreatedTime { get; set; }

    /// <summary>
    /// 備份的檔案數量
    /// </summary>
    public int FileCount { get; set; }

    /// <summary>
    /// 備份檔案大小（bytes）
    /// </summary>
    public long FileSize { get; set; }
}
