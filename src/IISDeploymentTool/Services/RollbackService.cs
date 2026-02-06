using System.IO.Compression;

namespace IISDeploymentTool.Services;

/// <summary>
/// 回滾服務，負責從備份還原檔案
/// </summary>
public class RollbackService
{
    /// <summary>
    /// 從備份還原
    /// </summary>
    /// <param name="backupFilePath">備份檔案路徑</param>
    /// <param name="iisFolder">IIS 站台資料夾</param>
    /// <param name="progress">進度回報</param>
    public void RestoreFromBackup(
        string backupFilePath,
        string iisFolder,
        IProgress<string>? progress = null)
    {
        if (!File.Exists(backupFilePath))
        {
            throw new FileNotFoundException($"找不到備份檔案：{backupFilePath}");
        }

        progress?.Report("正在從備份還原檔案...");

        using (var archive = ZipFile.OpenRead(backupFilePath))
        {
            int total = archive.Entries.Count;
            int current = 0;

            foreach (var entry in archive.Entries)
            {
                current++;

                // 跳過目錄項目
                if (string.IsNullOrEmpty(entry.Name))
                    continue;

                var destPath = Path.Combine(iisFolder, entry.FullName);

                try
                {
                    // 確保目標資料夾存在
                    var destDir = Path.GetDirectoryName(destPath);
                    if (!string.IsNullOrEmpty(destDir) && !Directory.Exists(destDir))
                    {
                        Directory.CreateDirectory(destDir);
                    }

                    // 解壓縮並覆寫
                    entry.ExtractToFile(destPath, overwrite: true);
                }
                catch (Exception ex)
                {
                    progress?.Report($"還原檔案失敗 {entry.FullName}: {ex.Message}");
                }

                if (current % 10 == 0 || current == total)
                {
                    progress?.Report($"還原中... ({current}/{total})");
                }
            }
        }

        progress?.Report("還原完成");
    }
}
