namespace IISDeploymentTool.Services;

/// <summary>
/// 檔案同步服務，負責將檔案從來源複製到目標
/// </summary>
public class FileSyncService
{
    /// <summary>
    /// 同步結果
    /// </summary>
    public class SyncResult
    {
        /// <summary>
        /// 成功複製的檔案數量
        /// </summary>
        public int CopiedCount { get; set; }

        /// <summary>
        /// 跳過的檔案數量
        /// </summary>
        public int SkippedCount { get; set; }

        /// <summary>
        /// 成功複製的檔案清單
        /// </summary>
        public List<string> CopiedFiles { get; set; } = new();

        /// <summary>
        /// 發生錯誤的檔案清單
        /// </summary>
        public List<string> ErrorFiles { get; set; } = new();
    }

    /// <summary>
    /// 同步檔案
    /// </summary>
    /// <param name="sourceFolder">來源資料夾</param>
    /// <param name="destFolder">目標資料夾</param>
    /// <param name="progress">進度回報</param>
    /// <returns>同步結果</returns>
    public SyncResult SyncFiles(
        string sourceFolder,
        string destFolder,
        IProgress<string>? progress = null)
    {
        var result = new SyncResult();

        progress?.Report("正在掃描來源檔案...");

        // 取得所有來源檔案
        var sourceFiles = Directory.GetFiles(sourceFolder, "*.*", SearchOption.AllDirectories);

        progress?.Report($"找到 {sourceFiles.Length} 個檔案需要同步");

        for (int i = 0; i < sourceFiles.Length; i++)
        {
            var sourceFile = sourceFiles[i];
            var relPath = Path.GetRelativePath(sourceFolder, sourceFile);
            var destFile = Path.Combine(destFolder, relPath);

            try
            {
                // 確保目標資料夾存在
                var destDir = Path.GetDirectoryName(destFile);
                if (!string.IsNullOrEmpty(destDir) && !Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                }

                // 複製檔案（覆寫），對於被鎖定的檔案進行重試
                if (CopyFileWithRetry(sourceFile, destFile))
                {
                    result.CopiedFiles.Add(relPath);
                    result.CopiedCount++;
                }
                else
                {
                    result.ErrorFiles.Add($"{relPath}: 檔案被鎖定，無法複製（已重試多次）");
                }
            }
            catch (Exception ex)
            {
                result.ErrorFiles.Add($"{relPath}: {ex.Message}");
            }

            if ((i + 1) % 10 == 0 || i == sourceFiles.Length - 1)
            {
                progress?.Report($"同步中... ({i + 1}/{sourceFiles.Length})");
            }
        }

        return result;
    }

    /// <summary>
    /// 帶重試機制的檔案複製
    /// </summary>
    /// <param name="sourceFile">來源檔案路徑</param>
    /// <param name="destFile">目標檔案路徑</param>
    /// <param name="maxRetries">最大重試次數</param>
    /// <param name="retryDelayMs">重試延遲（毫秒）</param>
    /// <returns>是否成功複製</returns>
    private bool CopyFileWithRetry(
        string sourceFile,
        string destFile,
        int maxRetries = 5,
        int retryDelayMs = 2000)
    {
        for (int retry = 0; retry < maxRetries; retry++)
        {
            try
            {
                // 如果目標檔案存在且被鎖定，嘗試先刪除
                if (File.Exists(destFile))
                {
                    // 嘗試移除唯讀屬性
                    try
                    {
                        var attributes = File.GetAttributes(destFile);
                        if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                        {
                            File.SetAttributes(destFile, attributes & ~FileAttributes.ReadOnly);
                        }
                    }
                    catch
                    {
                        // 忽略屬性變更錯誤
                    }
                }

                // 複製檔案
                File.Copy(sourceFile, destFile, overwrite: true);
                return true;
            }
            catch (IOException) when (retry < maxRetries - 1)
            {
                // 檔案被鎖定，等待後重試
                Thread.Sleep(retryDelayMs);
            }
            catch (UnauthorizedAccessException) when (retry < maxRetries - 1)
            {
                // 權限問題，等待後重試
                Thread.Sleep(retryDelayMs);
            }
        }

        return false;
    }
}
