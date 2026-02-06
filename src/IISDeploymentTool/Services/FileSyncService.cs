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

                // 複製檔案（覆寫）
                File.Copy(sourceFile, destFile, overwrite: true);

                result.CopiedFiles.Add(relPath);
                result.CopiedCount++;
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
}
