using IISDeploymentTool.Models;
using IISDeploymentTool.Services;

namespace IISDeploymentTool;

/// <summary>
/// 主視窗
/// </summary>
public partial class MainForm : Form
{
    private readonly DeploymentService _deploymentService;
    private readonly SettingsService _settingsService;
    private CancellationTokenSource? _cancellationTokenSource;

    /// <summary>
    /// 初始化主視窗
    /// </summary>
    public MainForm()
    {
        InitializeComponent();

        // 設定視窗圖示
        try
        {
            var iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "icon.ico");
            if (File.Exists(iconPath))
            {
                this.Icon = new Icon(iconPath);
            }
        }
        catch { /* 忽略圖示載入錯誤 */ }

        _deploymentService = new DeploymentService();
        _settingsService = new SettingsService();

        // 載入設定
        LoadSettings();

        // 綁定事件
        btnDeploy.Click += BtnDeploy_Click;
        btnRollback.Click += BtnRollback_Click;
        btnCancel.Click += BtnCancel_Click;
        btnBrowseSource.Click += BtnBrowseSource_Click;
        btnBrowseIIS.Click += BtnBrowseIIS_Click;
        btnBrowseBackup.Click += BtnBrowseBackup_Click;
        this.FormClosing += MainForm_FormClosing;
    }

    /// <summary>
    /// 載入設定
    /// </summary>
    private void LoadSettings()
    {
        var settings = _settingsService.Load();
        txtSourceFolder.Text = settings.SourceFolder;
        txtIISFolder.Text = settings.IISFolder;
        txtBackupFolder.Text = settings.BackupFolder;

        if (settings.LastDeploymentTime.HasValue)
        {
            lblLastDeployTime.Text = settings.LastDeploymentTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }

    /// <summary>
    /// 儲存設定
    /// </summary>
    private void SaveSettings()
    {
        var settings = new DeploymentSettings
        {
            SourceFolder = txtSourceFolder.Text.Trim(),
            IISFolder = txtIISFolder.Text.Trim(),
            BackupFolder = txtBackupFolder.Text.Trim(),
            LastDeploymentTime = DateTime.Now
        };
        _settingsService.Save(settings);
    }

    /// <summary>
    /// 執行上版按鈕點擊事件
    /// </summary>
    private async void BtnDeploy_Click(object? sender, EventArgs e)
    {
        // 驗證輸入
        if (!ValidateInputs()) return;

        // 顯示確認對話框
        var confirmMsg = $"請確認以下路徑：\n\n" +
            $"上版程式路徑：\n{txtSourceFolder.Text}\n\n" +
            $"IIS 站台路徑：\n{txtIISFolder.Text}\n\n" +
            $"備份路徑：\n{txtBackupFolder.Text}\n\n" +
            $"確認無誤後將開始部署。";

        var confirmResult = MessageBox.Show(
            confirmMsg,
            "確認部署",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question,
            MessageBoxDefaultButton.Button2);

        if (confirmResult != DialogResult.Yes) return;

        // 設定 UI 狀態
        SetUiProcessingState(true);
        txtLog.Clear();
        AppendLog("========================================\r\n");
        AppendLog("開始部署流程...\r\n");
        AppendLog("========================================\r\n\r\n");

        _cancellationTokenSource = new CancellationTokenSource();

        try
        {
            var progress = new Progress<string>(message =>
            {
                AppendLog($"[{DateTime.Now:HH:mm:ss}] {message}\r\n");
            });

            var settings = new DeploymentSettings
            {
                SourceFolder = txtSourceFolder.Text.Trim(),
                IISFolder = txtIISFolder.Text.Trim(),
                BackupFolder = txtBackupFolder.Text.Trim()
            };

            var result = await _deploymentService.DeployAsync(
                settings,
                progress,
                _cancellationTokenSource.Token);

            if (result.IsSuccess)
            {
                AppendLog("\r\n========================================\r\n");
                AppendLog("✓ 部署成功完成！\r\n");
                AppendLog("========================================\r\n");
                AppendLog($"同步檔案數：{result.SyncedFileCount}\r\n");
                AppendLog($"執行時間：{result.Duration.TotalSeconds:F1} 秒\r\n");
                AppendLog($"備份檔案：{result.BackupFilePath}\r\n");
                AppendLog("========================================\r\n");

                MessageBox.Show(
                    $"部署成功！\n\n" +
                    $"同步檔案數：{result.SyncedFileCount}\n" +
                    $"執行時間：{result.Duration.TotalSeconds:F1} 秒\n\n" +
                    $"備份檔案：\n{result.BackupFilePath}",
                    "部署完成",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // 儲存設定
                SaveSettings();

                // 更新上次部署時間
                lblLastDeployTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
            else
            {
                AppendLog("\r\n========================================\r\n");
                AppendLog("✗ 部署失敗\r\n");
                AppendLog("========================================\r\n");
                AppendLog($"錯誤訊息：{result.ErrorMessage}\r\n");
                AppendLog("========================================\r\n");

                // 詢問是否回滾
                if (result.FailedAtStep >= DeploymentStep.BackupFiles &&
                    !string.IsNullOrEmpty(result.BackupFilePath))
                {
                    var rollbackResult = MessageBox.Show(
                        $"部署失敗：{result.ErrorMessage}\n\n" +
                        $"是否要回滾到之前的版本？",
                        "部署失敗",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Error);

                    if (rollbackResult == DialogResult.Yes)
                    {
                        await PerformRollback(result.BackupFilePath, settings.IISFolder);
                    }
                }
                else
                {
                    MessageBox.Show(
                        $"部署失敗：{result.ErrorMessage}",
                        "錯誤",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }
        catch (Exception ex)
        {
            AppendLog($"\r\n✗ 發生未預期的錯誤：{ex.Message}\r\n");
            MessageBox.Show(
                $"發生錯誤：{ex.Message}",
                "錯誤",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            SetUiProcessingState(false);
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }
    }

    /// <summary>
    /// 執行回滾
    /// </summary>
    private async Task PerformRollback(string backupFilePath, string iisFolder)
    {
        AppendLog("\r\n========================================\r\n");
        AppendLog("開始回滾操作...\r\n");
        AppendLog("========================================\r\n\r\n");

        var progress = new Progress<string>(message =>
        {
            AppendLog($"[{DateTime.Now:HH:mm:ss}] {message}\r\n");
        });

        try
        {
            await _deploymentService.RollbackAsync(backupFilePath, iisFolder, progress);

            AppendLog("\r\n========================================\r\n");
            AppendLog("✓ 回滾完成\r\n");
            AppendLog("========================================\r\n");

            MessageBox.Show(
                "已成功回滾到之前的版本",
                "回滾完成",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            AppendLog($"\r\n✗ 回滾失敗：{ex.Message}\r\n");
            MessageBox.Show(
                $"回滾失敗：{ex.Message}",
                "錯誤",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// 追加日誌訊息
    /// </summary>
    private void AppendLog(string message)
    {
        txtLog.AppendText(message);
        txtLog.SelectionStart = txtLog.Text.Length;
        txtLog.ScrollToCaret();
    }

    /// <summary>
    /// 驗證輸入
    /// </summary>
    private bool ValidateInputs()
    {
        if (string.IsNullOrWhiteSpace(txtSourceFolder.Text))
        {
            MessageBox.Show("請選擇上版程式路徑", "驗證",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtSourceFolder.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtIISFolder.Text))
        {
            MessageBox.Show("請選擇 IIS 站台資料夾路徑", "驗證",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtIISFolder.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtBackupFolder.Text))
        {
            MessageBox.Show("請選擇備份路徑", "驗證",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtBackupFolder.Focus();
            return false;
        }

        return true;
    }

    /// <summary>
    /// 設定 UI 處理狀態
    /// </summary>
    private void SetUiProcessingState(bool isProcessing)
    {
        txtSourceFolder.Enabled = !isProcessing;
        btnBrowseSource.Enabled = !isProcessing;
        txtIISFolder.Enabled = !isProcessing;
        btnBrowseIIS.Enabled = !isProcessing;
        txtBackupFolder.Enabled = !isProcessing;
        btnBrowseBackup.Enabled = !isProcessing;

        btnDeploy.Enabled = !isProcessing;
        btnRollback.Enabled = !isProcessing;
        btnCancel.Enabled = isProcessing;

        progressBar.Visible = isProcessing;
        lblStatus.Text = isProcessing ? "處理中..." : "就緒";
    }

    /// <summary>
    /// 瀏覽上版程式路徑按鈕點擊事件
    /// </summary>
    private void BtnBrowseSource_Click(object? sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog();
        dialog.Description = "請選擇上版程式資料夾";
        if (Directory.Exists(txtSourceFolder.Text))
        {
            dialog.InitialDirectory = txtSourceFolder.Text;
        }

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            txtSourceFolder.Text = dialog.SelectedPath;
        }
    }

    /// <summary>
    /// 瀏覽 IIS 站台路徑按鈕點擊事件
    /// </summary>
    private void BtnBrowseIIS_Click(object? sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog();
        dialog.Description = "請選擇 IIS 站台資料夾";
        if (Directory.Exists(txtIISFolder.Text))
        {
            dialog.InitialDirectory = txtIISFolder.Text;
        }

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            txtIISFolder.Text = dialog.SelectedPath;
        }
    }

    /// <summary>
    /// 瀏覽備份路徑按鈕點擊事件
    /// </summary>
    private void BtnBrowseBackup_Click(object? sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog();
        dialog.Description = "請選擇備份資料夾";
        if (Directory.Exists(txtBackupFolder.Text))
        {
            dialog.InitialDirectory = txtBackupFolder.Text;
        }

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            txtBackupFolder.Text = dialog.SelectedPath;
        }
    }

    /// <summary>
    /// 回滾按鈕點擊事件
    /// </summary>
    private async void BtnRollback_Click(object? sender, EventArgs e)
    {
        // 驗證備份路徑
        var backupFolder = txtBackupFolder.Text.Trim();
        if (string.IsNullOrWhiteSpace(backupFolder))
        {
            MessageBox.Show("請先設定備份路徑", "提示",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtBackupFolder.Focus();
            return;
        }

        if (!Directory.Exists(backupFolder))
        {
            MessageBox.Show("備份路徑不存在", "錯誤",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        // 驗證 IIS 路徑
        var iisFolder = txtIISFolder.Text.Trim();
        if (string.IsNullOrWhiteSpace(iisFolder))
        {
            MessageBox.Show("請先設定 IIS 站台資料夾路徑", "提示",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtIISFolder.Focus();
            return;
        }

        if (!Directory.Exists(iisFolder))
        {
            MessageBox.Show("IIS 站台資料夾不存在", "錯誤",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        // 掃描備份清單
        var backupService = new BackupService();
        var backups = backupService.GetBackupList(backupFolder);

        if (backups.Count == 0)
        {
            MessageBox.Show($"在備份路徑中找不到任何備份檔案\n\n路徑：{backupFolder}",
                "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        // 顯示備份清單對話框
        using var backupListForm = new BackupListForm(backups);
        if (backupListForm.ShowDialog(this) == DialogResult.OK)
        {
            var selectedBackup = backupListForm.SelectedBackup;
            if (selectedBackup != null)
            {
                // 確認回滾
                var confirmMsg = $"確定要從以下備份還原嗎？\n\n" +
                    $"備份檔案：{selectedBackup.FileName}\n" +
                    $"建立時間：{selectedBackup.CreatedTime:yyyy-MM-dd HH:mm:ss}\n" +
                    $"檔案數量：{selectedBackup.FileCount}\n\n" +
                    $"警告：此操作將覆蓋目前 IIS 站台中的檔案！";

                var confirmResult = MessageBox.Show(
                    confirmMsg,
                    "確認回滾",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);

                if (confirmResult == DialogResult.Yes)
                {
                    await PerformManualRollback(selectedBackup.FilePath, iisFolder);
                }
            }
        }
    }

    /// <summary>
    /// 執行手動回滾
    /// </summary>
    private async Task PerformManualRollback(string backupFilePath, string iisFolder)
    {
        SetUiProcessingState(true);
        txtLog.Clear();
        AppendLog("========================================\r\n");
        AppendLog("開始手動回滾操作...\r\n");
        AppendLog("========================================\r\n\r\n");

        var progress = new Progress<string>(message =>
        {
            AppendLog($"[{DateTime.Now:HH:mm:ss}] {message}\r\n");
        });

        try
        {
            await _deploymentService.RollbackAsync(backupFilePath, iisFolder, progress);

            AppendLog("\r\n========================================\r\n");
            AppendLog("✓ 回滾完成\r\n");
            AppendLog("========================================\r\n");

            MessageBox.Show(
                "已成功從備份還原",
                "回滾完成",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            AppendLog($"\r\n✗ 回滾失敗：{ex.Message}\r\n");
            MessageBox.Show(
                $"回滾失敗：{ex.Message}",
                "錯誤",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            SetUiProcessingState(false);
        }
    }

    /// <summary>
    /// 取消按鈕點擊事件
    /// </summary>
    private void BtnCancel_Click(object? sender, EventArgs e)
    {
        _cancellationTokenSource?.Cancel();
        AppendLog("\r\n使用者取消操作\r\n");
    }

    /// <summary>
    /// 表單關閉事件
    /// </summary>
    private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
        {
            var result = MessageBox.Show(
                "部署正在進行中，確定要關閉嗎？",
                "確認",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.No)
            {
                e.Cancel = true;
                return;
            }

            _cancellationTokenSource.Cancel();
        }
    }
}
