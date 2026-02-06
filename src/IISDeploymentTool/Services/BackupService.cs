using System.IO.Compression;
using IISDeploymentTool.Models;

namespace IISDeploymentTool.Services;

/// <summary>
/// 備份服務，負責創建和管理備份檔案
/// </summary>
public class BackupService
{
    /// <summary>
    /// 創建備份
    /// </summary>
    /// <param name="sourceFolder">上版程式資料夾</param>
    /// <param name="iisFolder">IIS 站台資料夾</param>
    /// <param name="backupFolder">備份存放位置</param>
    /// <param name="progress">進度回報</param>
    /// <returns>備份資訊</returns>
    public BackupInfo CreateBackup(
        string sourceFolder,
        string iisFolder,
        string backupFolder,
        IProgress<string>? progress = null)
    {
        // 確保備份資料夾存在
        if (!Directory.Exists(backupFolder))
        {
            Directory.CreateDirectory(backupFolder);
        }

        // 生成備份檔名
        var backupFileName = GenerateBackupFileName(backupFolder);
        var backupFilePath = Path.Combine(backupFolder, backupFileName);

        progress?.Report($"正在分析需要備份的檔案...");

        // 取得上版程式資料夾中的所有檔案（相對路徑）
        var sourceFiles = GetRelativeFilePaths(sourceFolder);

        // 找出 IIS 站台中對應存在的檔案
        var filesToBackup = new List<string>();
        foreach (var relPath in sourceFiles)
        {
            var iisFilePath = Path.Combine(iisFolder, relPath);
            if (File.Exists(iisFilePath))
            {
                filesToBackup.Add(relPath);
            }
        }

        progress?.Report($"找到 {filesToBackup.Count} 個檔案需要備份");

        if (filesToBackup.Count == 0)
        {
            progress?.Report("沒有需要備份的檔案（IIS 站台中沒有對應的檔案）");
        }

        // 使用 System.IO.Compression 創建 ZIP
        using (var archive = ZipFile.Open(backupFilePath, ZipArchiveMode.Create))
        {
            for (int i = 0; i < filesToBackup.Count; i++)
            {
                var relPath = filesToBackup[i];
                var iisFilePath = Path.Combine(iisFolder, relPath);

                try
                {
                    // 保持資料夾結構
                    archive.CreateEntryFromFile(iisFilePath, relPath, CompressionLevel.Optimal);
                }
                catch (Exception ex)
                {
                    progress?.Report($"備份檔案失敗 {relPath}: {ex.Message}");
                }

                if ((i + 1) % 10 == 0 || i == filesToBackup.Count - 1)
                {
                    progress?.Report($"備份中... ({i + 1}/{filesToBackup.Count})");
                }
            }
        }

        var fileInfo = new FileInfo(backupFilePath);

        return new BackupInfo
        {
            FilePath = backupFilePath,
            FileName = backupFileName,
            CreatedTime = DateTime.Now,
            FileCount = filesToBackup.Count,
            FileSize = fileInfo.Length
        };
    }

    /// <summary>
    /// 生成備份檔名（yyyy-MM-dd-NN.zip）
    /// </summary>
    /// <param name="backupFolder">備份資料夾路徑</param>
    /// <returns>備份檔名</returns>
    public string GenerateBackupFileName(string backupFolder)
    {
        var today = DateTime.Now.ToString("yyyy-MM-dd");
        var pattern = $"{today}-*.zip";

        // 找出今天的所有備份檔案
        var existingBackups = new List<string>();
        if (Directory.Exists(backupFolder))
        {
            existingBackups = Directory.GetFiles(backupFolder, pattern)
                .Select(Path.GetFileName)
                .Where(f => f != null)
                .Select(f => f!)
                .ToList();
        }

        // 找出最大的流水號
        int maxSequence = 0;
        foreach (var fileName in existingBackups)
        {
            // 解析檔名：yyyy-MM-dd-NN.zip
            var nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
            var parts = nameWithoutExt.Split('-');
            if (parts.Length == 4 && int.TryParse(parts[3], out int seq))
            {
                maxSequence = Math.Max(maxSequence, seq);
            }
        }

        // 新的流水號
        var newSequence = maxSequence + 1;
        return $"{today}-{newSequence:D2}.zip";
    }

    /// <summary>
    /// 掃描備份資料夾，取得所有備份檔案清單
    /// </summary>
    /// <param name="backupFolder">備份資料夾路徑</param>
    /// <returns>備份清單</returns>
    public List<BackupInfo> GetBackupList(string backupFolder)
    {
        var backups = new List<BackupInfo>();

        if (!Directory.Exists(backupFolder))
        {
            return backups;
        }

        // 搜尋所有 .zip 檔案
        var zipFiles = Directory.GetFiles(backupFolder, "*.zip", SearchOption.TopDirectoryOnly);

        foreach (var zipFile in zipFiles)
        {
            try
            {
                var fileInfo = new FileInfo(zipFile);
                var fileName = fileInfo.Name;

                // 嘗試從 ZIP 檔案中取得檔案數量
                int fileCount = 0;
                try
                {
                    using (var archive = ZipFile.OpenRead(zipFile))
                    {
                        fileCount = archive.Entries.Count;
                    }
                }
                catch
                {
                    // 如果無法讀取 ZIP，跳過
                    continue;
                }

                backups.Add(new BackupInfo
                {
                    FilePath = zipFile,
                    FileName = fileName,
                    CreatedTime = fileInfo.CreationTime,
                    FileCount = fileCount,
                    FileSize = fileInfo.Length
                });
            }
            catch
            {
                // 忽略無法讀取的檔案
            }
        }

        return backups;
    }

    /// <summary>
    /// 取得資料夾中所有檔案的相對路徑
    /// </summary>
    /// <param name="folder">資料夾路徑</param>
    /// <returns>相對路徑清單</returns>
    private List<string> GetRelativeFilePaths(string folder)
    {
        var result = new List<string>();
        var files = Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            var relPath = Path.GetRelativePath(folder, file);
            result.Add(relPath);
        }

        return result;
    }
}
