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

        /// <summary>
        /// 合併另一個複製結果
        /// </summary>
        public void Merge(CopyResult other)
        {
            CopiedCount += other.CopiedCount;
            CopiedFiles.AddRange(other.CopiedFiles);
            NotFoundFiles.AddRange(other.NotFoundFiles);
            SkippedExtensions.AddRange(other.SkippedExtensions);
        }
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

    /// <summary>
    /// 複製強制指定的檔案清單（支援萬用字元 * 和 ?）
    /// </summary>
    /// <param name="forceCopyFileList">強制複製的檔案清單（多行文字，換行分隔，支援萬用字元）</param>
    /// <param name="buildOutputFolder">建置結果資料夾</param>
    /// <param name="deploymentFolder">預計上版資料夾</param>
    /// <returns>複製結果</returns>
    public CopyResult CopyForcedFiles(
        string forceCopyFileList,
        string buildOutputFolder,
        string deploymentFolder)
    {
        var result = new CopyResult();

        if (string.IsNullOrWhiteSpace(forceCopyFileList))
        {
            return result; // 空清單直接返回
        }

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

        // 解析多行文字
        var patterns = forceCopyFileList
            .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Trim())
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToList();

        foreach (var pattern in patterns)
        {
            try
            {
                // 標準化路徑分隔符號
                var normalizedPattern = pattern.Replace('/', Path.DirectorySeparatorChar);

                // 檢查是否包含萬用字元
                if (normalizedPattern.Contains('*') || normalizedPattern.Contains('?'))
                {
                    // 萬用字元匹配模式
                    CopyWildcardFiles(normalizedPattern, buildOutputFolder, deploymentFolder, result);
                }
                else
                {
                    // 單一檔案複製
                    CopySingleFile(normalizedPattern, buildOutputFolder, deploymentFolder, result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"處理檔案模式失敗 {pattern}: {ex.Message}");
                result.NotFoundFiles.Add($"{pattern} (錯誤: {ex.Message})");
            }
        }

        return result;
    }

    /// <summary>
    /// 複製單一檔案
    /// </summary>
    private void CopySingleFile(string relativePath, string sourceRoot, string destRoot, CopyResult result)
    {
        var sourceFile = Path.Combine(sourceRoot, relativePath);
        var destFile = Path.Combine(destRoot, relativePath);

        if (File.Exists(sourceFile))
        {
            // 建立目標資料夾結構
            var destDir = Path.GetDirectoryName(destFile);
            if (!string.IsNullOrEmpty(destDir) && !Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }

            // 複製檔案（覆寫）
            File.Copy(sourceFile, destFile, true);
            result.CopiedFiles.Add(relativePath);
            result.CopiedCount++;
        }
        else
        {
            result.NotFoundFiles.Add(relativePath);
        }
    }

    /// <summary>
    /// 使用萬用字元匹配並複製檔案
    /// </summary>
    private void CopyWildcardFiles(string pattern, string sourceRoot, string destRoot, CopyResult result)
    {
        // 分離目錄部分和檔案名稱模式
        var patternDir = Path.GetDirectoryName(pattern) ?? string.Empty;
        var filePattern = Path.GetFileName(pattern);

        // 處理 ** 遞迴搜尋（例如：lib\**\*.dll）
        var searchOption = patternDir.Contains("**")
            ? SearchOption.AllDirectories
            : SearchOption.TopDirectoryOnly;

        // 移除 ** 並取得實際的搜尋起點
        var searchDir = patternDir.Replace("**", "").TrimEnd(Path.DirectorySeparatorChar);
        if (string.IsNullOrEmpty(searchDir))
        {
            searchDir = ".";
        }

        var fullSearchPath = Path.Combine(sourceRoot, searchDir);

        if (!Directory.Exists(fullSearchPath))
        {
            result.NotFoundFiles.Add(pattern + " (資料夾不存在)");
            return;
        }

        // 搜尋匹配的檔案
        var matchedFiles = Directory.GetFiles(fullSearchPath, filePattern, searchOption);

        if (matchedFiles.Length == 0)
        {
            result.NotFoundFiles.Add(pattern + " (未匹配任何檔案)");
            return;
        }

        // 複製所有匹配的檔案
        foreach (var sourceFile in matchedFiles)
        {
            try
            {
                // 計算相對路徑
                var relativePath = Path.GetRelativePath(sourceRoot, sourceFile);
                var destFile = Path.Combine(destRoot, relativePath);

                // 建立目標資料夾結構
                var destDir = Path.GetDirectoryName(destFile);
                if (!string.IsNullOrEmpty(destDir) && !Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                }

                // 複製檔案（覆寫）
                File.Copy(sourceFile, destFile, true);
                result.CopiedFiles.Add(relativePath);
                result.CopiedCount++;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"複製檔案失敗 {sourceFile}: {ex.Message}");
                result.NotFoundFiles.Add(Path.GetRelativePath(sourceRoot, sourceFile));
            }
        }
    }
}
