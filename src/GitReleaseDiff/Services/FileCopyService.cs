using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GitReleaseDiff.Models;

namespace GitReleaseDiff.Services;

/// <summary>
/// 檔案複製服務，負責將變更的檔案從建置目錄複製到部署目錄
/// </summary>
public class FileCopyService
{
    /// <summary>
    /// 複製結果資訊
    /// </summary>
    public class CopyResult
    {
        public int CopiedCount { get; set; }
        public List<string> CopiedFiles { get; set; } = new();
        public List<string> NotFoundFiles { get; set; } = new();
        public List<string> SkippedExtensions { get; set; } = new();
    }

    /// <summary>
    /// 執行檔案複製
    /// </summary>
    /// <param name="fileDiffs">檔案差異清單</param>
    /// <param name="buildOutputFolder">建置結果資料夾</param>
    /// <param name="deploymentFolder">預計上版資料夾</param>
    /// <param name="projectPathPrefix">專案路徑前綴（可選，用於多專案方案）</param>
    /// <returns>複製結果</returns>
    public CopyResult CopyMatchedFiles(
        IEnumerable<FileDiffInfo> fileDiffs,
        string buildOutputFolder,
        string deploymentFolder,
        string projectPathPrefix = "")
    {
        var result = new CopyResult();

        if (string.IsNullOrWhiteSpace(buildOutputFolder) || string.IsNullOrWhiteSpace(deploymentFolder))
        {
            throw new ArgumentException("建置資料夾與部署資料夾路徑不得為空");
        }

        if (!Directory.Exists(buildOutputFolder))
        {
            throw new DirectoryNotFoundException($"找不到建置資料夾: {buildOutputFolder}");
        }

        // 確保部署資料夾存在
        if (!Directory.Exists(deploymentFolder))
        {
            Directory.CreateDirectory(deploymentFolder);
        }

        foreach (var diff in fileDiffs)
        {
            // 忽略刪除的檔案
            if (diff.Status == FileChangeStatus.Deleted)
            {
                continue;
            }

            // 處理路徑前綴
            var relativePath = diff.FilePath;

            if (!string.IsNullOrWhiteSpace(projectPathPrefix))
            {
                // 標準化前綴（確保以 / 結尾）
                var normalizedPrefix = projectPathPrefix.Trim().Replace('\\', '/');
                if (!normalizedPrefix.EndsWith('/'))
                {
                    normalizedPrefix += '/';
                }

                // 移除前綴
                if (relativePath.StartsWith(normalizedPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    relativePath = relativePath.Substring(normalizedPrefix.Length);
                }
            }

            // 修正路徑分隔符號以匹配當前作業系統
            relativePath = relativePath.Replace('/', Path.DirectorySeparatorChar);
            var sourceFile = Path.Combine(buildOutputFolder, relativePath);
            var destFile = Path.Combine(deploymentFolder, relativePath);

            if (File.Exists(sourceFile))
            {
                try
                {
                    // 建立目標資料夾結構
                    var destDir = Path.GetDirectoryName(destFile);
                    if (!string.IsNullOrEmpty(destDir) && !Directory.Exists(destDir))
                    {
                        Directory.CreateDirectory(destDir);
                    }

                    // 複製檔案 (覆寫)
                    File.Copy(sourceFile, destFile, true);
                    result.CopiedFiles.Add(relativePath);
                    result.CopiedCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"複製檔案失敗 {relativePath}: {ex.Message}");
                    // 可以在這裡記錄錯誤，或者視為失敗
                }
            }
            else
            {
                result.NotFoundFiles.Add(relativePath);
            }
        }

        return result;
    }
}
