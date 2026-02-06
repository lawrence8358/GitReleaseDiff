using IISDeploymentTool.Models;

namespace IISDeploymentTool.Services;

/// <summary>
/// 部署服務，負責協調整個部署流程
/// </summary>
public class DeploymentService
{
    private readonly IISMaintenanceService _maintenanceService;
    private readonly BackupService _backupService;
    private readonly FileSyncService _fileSyncService;
    private readonly RollbackService _rollbackService;

    /// <summary>
    /// 初始化部署服務
    /// </summary>
    public DeploymentService()
    {
        _maintenanceService = new IISMaintenanceService();
        _backupService = new BackupService();
        _fileSyncService = new FileSyncService();
        _rollbackService = new RollbackService();
    }

    /// <summary>
    /// 執行部署
    /// </summary>
    /// <param name="settings">部署設定</param>
    /// <param name="progress">進度回報</param>
    /// <param name="cancellationToken">取消標記</param>
    /// <returns>部署結果</returns>
    public async Task<DeploymentResult> DeployAsync(
        DeploymentSettings settings,
        IProgress<string>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var result = new DeploymentResult
        {
            StartTime = DateTime.Now
        };

        try
        {
            // 步驟 0：驗證路徑
            progress?.Report("步驟 1/5：驗證路徑...");
            result.FailedAtStep = DeploymentStep.ValidatePaths;
            ValidatePaths(settings);
            cancellationToken.ThrowIfCancellationRequested();

            // 步驟 1：啟用維護模式
            progress?.Report("步驟 2/5：啟用維護模式（創建 app_offline.htm）...");
            result.FailedAtStep = DeploymentStep.EnableMaintenanceMode;
            await Task.Run(() =>
                _maintenanceService.Enable(settings.IISFolder, settings.ApplicationPoolName),
                cancellationToken);

            if (!string.IsNullOrWhiteSpace(settings.ApplicationPoolName))
            {
                progress?.Report($"已停止應用程式池：{settings.ApplicationPoolName}");
            }

            cancellationToken.ThrowIfCancellationRequested();

            // 步驟 2：備份檔案
            progress?.Report("步驟 3/5：備份現有檔案...");
            result.FailedAtStep = DeploymentStep.BackupFiles;
            var backupInfo = await Task.Run(() =>
                _backupService.CreateBackup(
                    settings.SourceFolder,
                    settings.IISFolder,
                    settings.BackupFolder,
                    progress),
                cancellationToken);
            result.BackupFilePath = backupInfo.FilePath;
            progress?.Report($"備份完成：{backupInfo.FileName}（{backupInfo.FileCount} 個檔案，{FormatFileSize(backupInfo.FileSize)}）");
            cancellationToken.ThrowIfCancellationRequested();

            // 步驟 3：同步檔案
            progress?.Report("步驟 4/5：同步檔案到 IIS 站台...");
            result.FailedAtStep = DeploymentStep.SyncFiles;
            var syncResult = await Task.Run(() =>
                _fileSyncService.SyncFiles(
                    settings.SourceFolder,
                    settings.IISFolder,
                    progress),
                cancellationToken);
            result.SyncedFileCount = syncResult.CopiedCount;

            if (syncResult.ErrorFiles.Count > 0)
            {
                progress?.Report($"警告：{syncResult.ErrorFiles.Count} 個檔案同步失敗");
                foreach (var error in syncResult.ErrorFiles.Take(5))
                {
                    progress?.Report($"  - {error}");
                }
                if (syncResult.ErrorFiles.Count > 5)
                {
                    progress?.Report($"  - 還有 {syncResult.ErrorFiles.Count - 5} 個錯誤...");
                }
            }

            cancellationToken.ThrowIfCancellationRequested();

            // 步驟 4：停用維護模式
            progress?.Report("步驟 5/5：停用維護模式（移除 app_offline.htm）...");
            result.FailedAtStep = DeploymentStep.DisableMaintenanceMode;
            await Task.Run(() =>
                _maintenanceService.Disable(settings.IISFolder, settings.ApplicationPoolName),
                cancellationToken);

            if (!string.IsNullOrWhiteSpace(settings.ApplicationPoolName))
            {
                progress?.Report($"已啟動應用程式池：{settings.ApplicationPoolName}");
            }

            // 完成
            result.IsSuccess = true;
            result.FailedAtStep = null;
            result.EndTime = DateTime.Now;
            progress?.Report($"✓ 部署完成！共同步 {result.SyncedFileCount} 個檔案，耗時 {result.Duration.TotalSeconds:F1} 秒");

            return result;
        }
        catch (OperationCanceledException)
        {
            result.IsSuccess = false;
            result.ErrorMessage = "操作已取消";
            result.EndTime = DateTime.Now;

            // 嘗試清理維護模式
            try
            {
                if (_maintenanceService.IsInMaintenanceMode(settings.IISFolder))
                {
                    progress?.Report("正在清理維護模式...");
                    _maintenanceService.Disable(settings.IISFolder, settings.ApplicationPoolName);
                }
            }
            catch
            {
                // 忽略清理錯誤
            }

            return result;
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.ErrorMessage = ex.Message;
            result.EndTime = DateTime.Now;

            progress?.Report($"✗ 部署失敗於步驟 {GetStepName(result.FailedAtStep)}：{ex.Message}");

            // 嘗試清理維護模式（如果可能）
            try
            {
                if (result.FailedAtStep >= DeploymentStep.EnableMaintenanceMode &&
                    _maintenanceService.IsInMaintenanceMode(settings.IISFolder))
                {
                    progress?.Report("正在清理維護模式...");
                    _maintenanceService.Disable(settings.IISFolder, settings.ApplicationPoolName);
                }
            }
            catch
            {
                // 忽略清理錯誤
            }

            return result;
        }
    }

    /// <summary>
    /// 執行回滾
    /// </summary>
    /// <param name="backupFilePath">備份檔案路徑</param>
    /// <param name="iisFolder">IIS 站台資料夾</param>
    /// <param name="progress">進度回報</param>
    /// <param name="appPoolName">應用程式池名稱（選填）</param>
    public async Task RollbackAsync(
        string backupFilePath,
        string iisFolder,
        IProgress<string>? progress = null,
        string? appPoolName = null)
    {
        progress?.Report("開始回滾操作...");

        // 啟用維護模式
        progress?.Report("啟用維護模式...");
        await Task.Run(() => _maintenanceService.Enable(iisFolder, appPoolName));

        if (!string.IsNullOrWhiteSpace(appPoolName))
        {
            progress?.Report($"已停止應用程式池：{appPoolName}");
        }

        // 從備份還原
        await Task.Run(() => _rollbackService.RestoreFromBackup(
            backupFilePath,
            iisFolder,
            progress));

        // 停用維護模式
        progress?.Report("停用維護模式...");
        await Task.Run(() => _maintenanceService.Disable(iisFolder, appPoolName));

        if (!string.IsNullOrWhiteSpace(appPoolName))
        {
            progress?.Report($"已啟動應用程式池：{appPoolName}");
        }

        progress?.Report("✓ 回滾完成");
    }

    /// <summary>
    /// 驗證路徑
    /// </summary>
    /// <param name="settings">部署設定</param>
    private void ValidatePaths(DeploymentSettings settings)
    {
        if (!Directory.Exists(settings.SourceFolder))
        {
            throw new DirectoryNotFoundException(
                $"找不到上版程式資料夾：{settings.SourceFolder}");
        }

        if (!Directory.Exists(settings.IISFolder))
        {
            throw new DirectoryNotFoundException(
                $"找不到 IIS 站台資料夾：{settings.IISFolder}");
        }

        if (!Directory.Exists(settings.BackupFolder))
        {
            // 嘗試創建備份資料夾
            try
            {
                Directory.CreateDirectory(settings.BackupFolder);
            }
            catch (Exception ex)
            {
                throw new IOException(
                    $"無法創建備份資料夾：{settings.BackupFolder}，錯誤：{ex.Message}");
            }
        }
    }

    /// <summary>
    /// 取得步驟名稱
    /// </summary>
    private string GetStepName(DeploymentStep? step)
    {
        return step switch
        {
            DeploymentStep.ValidatePaths => "驗證路徑",
            DeploymentStep.EnableMaintenanceMode => "啟用維護模式",
            DeploymentStep.BackupFiles => "備份檔案",
            DeploymentStep.SyncFiles => "同步檔案",
            DeploymentStep.DisableMaintenanceMode => "停用維護模式",
            _ => "未知步驟"
        };
    }

    /// <summary>
    /// 格式化檔案大小
    /// </summary>
    private string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
}
