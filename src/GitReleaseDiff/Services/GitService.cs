using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using GitReleaseDiff.Models;

namespace GitReleaseDiff.Services;

/// <summary>
/// Git 服務類別，負責處理所有與 Git 相關的操作
/// </summary>
public class GitService
{
    private readonly string _workingDirectory;

    /// <summary>
    /// 初始化 Git 服務
    /// </summary>
    /// <param name="workingDirectory">工作目錄路徑</param>
    public GitService(string workingDirectory)
    {
        _workingDirectory = workingDirectory;
    }

    /// <summary>
    /// 預設建構子，使用暫存目錄作為工作目錄
    /// </summary>
    public GitService() : this(Path.GetTempPath())
    {
    }

    /// <summary>
    /// 透過 Azure DevOps / TFS REST API 取得兩個 Commit 之間的檔案差異
    /// </summary>
    /// <param name="gitUrl">Git 儲存庫網址</param>
    /// <param name="baseCommitId">基準 Commit ID</param>
    /// <param name="compareCommitId">比對 Commit ID</param>
    /// <param name="personalAccessToken">Personal Access Token (PAT)</param>
    /// <param name="progress">進度回報委派</param>
    /// <param name="cancellationToken">取消權杖</param>
    /// <returns>檔案差異清單</returns>
    public async Task<List<FileDiffInfo>> GetDiffAsync(
        string gitUrl,
        string baseCommitId,
        string compareCommitId,
        string personalAccessToken,
        IProgress<string>? progress = null,
        CancellationToken cancellationToken = default)
    {
        progress?.Report("正在解析 Git 網址...");

        // 解析 Git URL 取得 API 基底 URL
        var apiInfo = ParseGitUrl(gitUrl);
        if (apiInfo == null)
        {
            throw new ArgumentException("無法解析 Git 網址，請確認網址格式正確");
        }

        progress?.Report("正在連接 Azure DevOps / TFS 伺服器...");

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

        // 設定 PAT 授權 (使用 Basic Authentication)
        var authToken = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($":{personalAccessToken}"));
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {authToken}");

        // 設定超時時間
        httpClient.Timeout = TimeSpan.FromMinutes(5);

        // 建構 API URL - 使用 diffs/commits API
        var apiUrl = $"{apiInfo.BaseUrl}/_apis/git/repositories/{apiInfo.RepositoryName}/diffs/commits?" +
                     $"baseVersion={baseCommitId}&baseVersionType=commit" +
                     $"&targetVersion={compareCommitId}&targetVersionType=commit" +
                     $"&$top=2000" +
                     $"&api-version=6.0";

        progress?.Report($"正在查詢 Commit 差異: {baseCommitId.Substring(0, Math.Min(7, baseCommitId.Length))} -> {compareCommitId.Substring(0, Math.Min(7, compareCommitId.Length))}...");

        try
        {
            var response = await httpClient.GetAsync(apiUrl, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new HttpRequestException($"API 請求失敗: {response.StatusCode}\n{errorContent}");
            }

            var jsonContent = await response.Content.ReadAsStringAsync(cancellationToken);

            progress?.Report("正在解析差異資料...");

            var diffResult = ParseDiffResponse(jsonContent);

            progress?.Report($"找到 {diffResult.Count} 個檔案差異");

            // 依資料夾和檔名排序
            return diffResult
                .OrderBy(f => f.FolderPath)
                .ThenBy(f => f.FileName)
                .ToList();
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"連接 Git 伺服器失敗: {ex.Message}", ex);
        }
        catch (TaskCanceledException)
        {
            throw new OperationCanceledException("操作已取消");
        }
    }

    /// <summary>
    /// 解析 Git URL 取得 API 資訊
    /// </summary>
    /// <param name="gitUrl">Git 網址</param>
    /// <returns>API 資訊</returns>
    private ApiInfo? ParseGitUrl(string gitUrl)
    {
        // 支援格式: https://tfs.xxx.com/tfs/DefaultCollection/ProjectName/_git/RepoName
        // 或: https://dev.azure.com/organization/ProjectName/_git/RepoName

        var match = Regex.Match(gitUrl, @"^(https?://[^/]+(?:/tfs)?/[^/]+/[^/]+)/_git/([^/]+)/?$", RegexOptions.IgnoreCase);

        if (match.Success)
        {
            return new ApiInfo
            {
                BaseUrl = match.Groups[1].Value,
                RepositoryName = match.Groups[2].Value
            };
        }

        // 嘗試另一種格式
        match = Regex.Match(gitUrl, @"^(https?://[^/]+/[^/]+/[^/]+)/_git/([^/]+)/?$", RegexOptions.IgnoreCase);

        if (match.Success)
        {
            return new ApiInfo
            {
                BaseUrl = match.Groups[1].Value,
                RepositoryName = match.Groups[2].Value
            };
        }

        return null;
    }

    /// <summary>
    /// 解析 API 回應的差異資料
    /// </summary>
    /// <param name="jsonContent">JSON 回應內容</param>
    /// <returns>檔案差異清單</returns>
    private List<FileDiffInfo> ParseDiffResponse(string jsonContent)
    {
        var result = new List<FileDiffInfo>();

        try
        {
            var jsonObj = Newtonsoft.Json.Linq.JObject.Parse(jsonContent);
            var changes = jsonObj["changes"] as Newtonsoft.Json.Linq.JArray;

            if (changes == null)
            {
                return result;
            }

            foreach (var change in changes)
            {
                var item = change["item"];
                if (item == null) continue;

                // 跳過資料夾類型
                var isFolder = (bool?)item["isFolder"] ?? false;
                if (isFolder) continue;

                var path = (string?)item["path"];
                if (string.IsNullOrEmpty(path)) continue;

                // 移除開頭的斜線
                if (path.StartsWith("/"))
                {
                    path = path.Substring(1);
                }

                var changeType = (string?)change["changeType"] ?? "edit";
                var status = MapChangeType(changeType);

                string? oldPath = null;
                if (status == FileChangeStatus.Renamed)
                {
                    var sourceServerItem = (string?)change["sourceServerItem"];
                    if (!string.IsNullOrEmpty(sourceServerItem))
                    {
                        oldPath = sourceServerItem.TrimStart('/');
                    }
                }

                result.Add(FileDiffInfo.Create(path, status, oldPath));
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"解析差異資料失敗: {ex.Message}", ex);
        }

        return result;
    }

    /// <summary>
    /// 對應變更類型字串到列舉值
    /// </summary>
    /// <param name="changeType">變更類型字串</param>
    /// <returns>檔案變更狀態</returns>
    private FileChangeStatus MapChangeType(string changeType)
    {
        // Azure DevOps API 可能回傳複合類型，如 "rename, edit"
        var lowerType = changeType.ToLower();

        if (lowerType.Contains("add"))
            return FileChangeStatus.Added;
        if (lowerType.Contains("delete"))
            return FileChangeStatus.Deleted;
        if (lowerType.Contains("rename"))
            return FileChangeStatus.Renamed;
        if (lowerType.Contains("copy"))
            return FileChangeStatus.Copied;

        // 預設為修改
        return FileChangeStatus.Modified;
    }

    /// <summary>
    /// 驗證 Commit ID 格式
    /// </summary>
    /// <param name="commitId">Commit ID</param>
    /// <returns>是否為有效格式</returns>
    public static bool IsValidCommitId(string commitId)
    {
        if (string.IsNullOrWhiteSpace(commitId))
            return false;

        // 支援短碼（至少4字元）到完整的40字元 SHA
        return Regex.IsMatch(commitId, @"^[0-9a-fA-F]{4,40}$");
    }

    /// <summary>
    /// API 資訊內部類別
    /// </summary>
    private class ApiInfo
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string RepositoryName { get; set; } = string.Empty;
    }
}
