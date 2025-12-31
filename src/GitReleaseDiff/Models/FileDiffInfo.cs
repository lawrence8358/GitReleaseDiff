namespace GitReleaseDiff.Models;

/// <summary>
/// 檔案變更狀態列舉
/// </summary>
public enum FileChangeStatus
{
    /// <summary>
    /// 新增
    /// </summary>
    Added,

    /// <summary>
    /// 修改
    /// </summary>
    Modified,

    /// <summary>
    /// 刪除
    /// </summary>
    Deleted,

    /// <summary>
    /// 重新命名
    /// </summary>
    Renamed,

    /// <summary>
    /// 複製
    /// </summary>
    Copied
}

/// <summary>
/// 檔案差異資訊模型
/// </summary>
public class FileDiffInfo
{
    /// <summary>
    /// 檔案完整路徑
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// 資料夾路徑
    /// </summary>
    public string FolderPath { get; set; } = string.Empty;

    /// <summary>
    /// 檔案名稱
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// 附檔名
    /// </summary>
    public string FileExtension { get; set; } = string.Empty;

    /// <summary>
    /// 變更狀態
    /// </summary>
    public FileChangeStatus Status { get; set; }

    /// <summary>
    /// 狀態的中文描述
    /// </summary>
    public string StatusDescription => Status switch
    {
        FileChangeStatus.Added => "新增",
        FileChangeStatus.Modified => "修改",
        FileChangeStatus.Deleted => "刪除",
        FileChangeStatus.Renamed => "重新命名",
        FileChangeStatus.Copied => "複製",
        _ => "未知"
    };

    /// <summary>
    /// 舊檔案路徑（用於重新命名或複製的情況）
    /// </summary>
    public string? OldFilePath { get; set; }

    /// <summary>
    /// 建立檔案差異資訊
    /// </summary>
    /// <param name="filePath">檔案完整路徑</param>
    /// <param name="status">變更狀態</param>
    /// <param name="oldFilePath">舊檔案路徑（可選）</param>
    /// <returns>檔案差異資訊實例</returns>
    public static FileDiffInfo Create(string filePath, FileChangeStatus status, string? oldFilePath = null)
    {
        var normalizedPath = filePath.Replace("\\", "/");
        var lastSlashIndex = normalizedPath.LastIndexOf('/');
        var fileName = lastSlashIndex >= 0 ? normalizedPath.Substring(lastSlashIndex + 1) : normalizedPath;
        
        // 取得附檔名
        var lastDotIndex = fileName.LastIndexOf('.');
        var fileExtension = lastDotIndex >= 0 ? fileName.Substring(lastDotIndex) : string.Empty;

        return new FileDiffInfo
        {
            FilePath = normalizedPath,
            FolderPath = lastSlashIndex >= 0 ? normalizedPath.Substring(0, lastSlashIndex) : string.Empty,
            FileName = fileName,
            FileExtension = fileExtension,
            Status = status,
            OldFilePath = oldFilePath
        };
    }
}
