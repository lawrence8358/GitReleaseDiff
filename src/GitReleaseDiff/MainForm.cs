using GitReleaseDiff.Models;
using GitReleaseDiff.Services;

namespace GitReleaseDiff;

/// <summary>
/// 主視窗表單，提供 Git 版本比對的使用者介面
/// </summary>
public partial class MainForm : Form
{
    private readonly GitService _gitService;
    private readonly SettingsService _settingsService;
    private readonly CsvExportService _csvExportService;
    private readonly FileCopyService _fileCopyService;
    private List<FileDiffInfo> _currentResults = new();
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

        _gitService = new GitService();
        _settingsService = new SettingsService();
        _csvExportService = new CsvExportService();
        _fileCopyService = new FileCopyService();

        // 載入上次的設定
        LoadSettings();

        // 綁定事件
        btnCompare.Click += BtnCompare_Click;
        btnExport.Click += BtnExport_Click;
        btnCancel.Click += BtnCancel_Click;
        btnBrowseBuildOutput.Click += BtnBrowseBuildOutput_Click;
        btnBrowseDeployment.Click += BtnBrowseDeployment_Click;
        btnCopyFiles.Click += BtnCopyFiles_Click;
        this.FormClosing += MainForm_FormClosing;
    }

    /// <summary>
    /// 載入應用程式設定
    /// </summary>
    private void LoadSettings()
    {
        var settings = _settingsService.Load();
        txtGitUrl.Text = settings.GitUrl;
        txtPat.Text = settings.PersonalAccessToken;
        txtBaseCommit.Text = settings.BaseCommitId;
        txtCompareCommit.Text = settings.CompareCommitId;
        txtBuildOutput.Text = settings.BuildOutputFolder;
        txtDeployment.Text = settings.DeploymentFolder;
        txtProjectPrefix.Text = settings.ProjectPathPrefix;
        txtForceCopyFiles.Text = settings.ForceCopyFileList;
    }

    /// <summary>
    /// 儲存應用程式設定
    /// </summary>
    private void SaveSettings()
    {
        var settings = new AppSettings
        {
            GitUrl = txtGitUrl.Text.Trim(),
            PersonalAccessToken = txtPat.Text.Trim(),
            BaseCommitId = txtBaseCommit.Text.Trim(),
            CompareCommitId = txtCompareCommit.Text.Trim(),
            BuildOutputFolder = txtBuildOutput.Text.Trim(),
            DeploymentFolder = txtDeployment.Text.Trim(),
            ProjectPathPrefix = txtProjectPrefix.Text.Trim(),
            ForceCopyFileList = txtForceCopyFiles.Text.Trim()
        };
        _settingsService.Save(settings);
    }

    /// <summary>
    /// 執行比對按鈕點擊事件
    /// </summary>
    private async void BtnCompare_Click(object? sender, EventArgs e)
    {
        // 驗證輸入
        if (!ValidateInputs())
        {
            return;
        }

        // 儲存設定
        SaveSettings();

        // 設定 UI 為處理中狀態
        SetUiProcessingState(true);

        _cancellationTokenSource = new CancellationTokenSource();

        try
        {
            var progress = new Progress<string>(message =>
            {
                lblStatus.Text = message;
            });

            _currentResults = await _gitService.GetDiffAsync(
                txtGitUrl.Text.Trim(),
                txtBaseCommit.Text.Trim(),
                txtCompareCommit.Text.Trim(),
                txtPat.Text.Trim(),
                progress,
                _cancellationTokenSource.Token
            );

            // 顯示結果
            DisplayResults(_currentResults);

            lblStatus.Text = $"比對完成，共找到 {_currentResults.Count} 個檔案差異";
            btnExport.Enabled = _currentResults.Count > 0;
        }
        catch (OperationCanceledException)
        {
            lblStatus.Text = "操作已取消";
            dgvResults.Rows.Clear();
            lblResultCount.Text = "比對結果：";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"比對失敗：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            lblStatus.Text = "比對失敗";
        }
        finally
        {
            SetUiProcessingState(false);
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }
    }

    /// <summary>
    /// 匯出 CSV 按鈕點擊事件
    /// </summary>
    private void BtnExport_Click(object? sender, EventArgs e)
    {
        if (_currentResults.Count == 0)
        {
            MessageBox.Show("沒有可匯出的資料", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var saveDialog = new SaveFileDialog
        {
            Filter = "CSV 檔案 (*.csv)|*.csv",
            DefaultExt = "csv",
            FileName = $"GitDiff_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
        };

        if (saveDialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                _csvExportService.Export(_currentResults, saveDialog.FileName);
                MessageBox.Show($"已成功匯出至：\n{saveDialog.FileName}", "匯出成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"匯出失敗：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    /// <summary>
    /// 取消按鈕點擊事件
    /// </summary>
    private void BtnCancel_Click(object? sender, EventArgs e)
    {
        _cancellationTokenSource?.Cancel();
    }

    /// <summary>
    /// 視窗關閉事件
    /// </summary>
    private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        _cancellationTokenSource?.Cancel();
    }

    /// <summary>
    /// 驗證輸入欄位
    /// </summary>
    /// <returns>是否通過驗證</returns>
    private bool ValidateInputs()
    {
        if (string.IsNullOrWhiteSpace(txtGitUrl.Text))
        {
            ShowValidationError("請輸入 Git 網址");
            txtGitUrl.Focus();
            return false;
        }

        if (!txtGitUrl.Text.Trim().StartsWith("http://") && !txtGitUrl.Text.Trim().StartsWith("https://"))
        {
            ShowValidationError("Git 網址必須以 http:// 或 https:// 開頭");
            txtGitUrl.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtPat.Text))
        {
            ShowValidationError("請輸入 Personal Access Token (PAT)");
            txtPat.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtBaseCommit.Text))
        {
            ShowValidationError("請輸入基準 Commit ID");
            txtBaseCommit.Focus();
            return false;
        }

        if (!GitService.IsValidCommitId(txtBaseCommit.Text.Trim()))
        {
            ShowValidationError("基準 Commit ID 格式不正確，應為 4-40 位的十六進位字元");
            txtBaseCommit.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtCompareCommit.Text))
        {
            ShowValidationError("請輸入比對 Commit ID");
            txtCompareCommit.Focus();
            return false;
        }

        if (!GitService.IsValidCommitId(txtCompareCommit.Text.Trim()))
        {
            ShowValidationError("比對 Commit ID 格式不正確，應為 4-40 位的十六進位字元");
            txtCompareCommit.Focus();
            return false;
        }

        return true;
    }

    /// <summary>
    /// 顯示驗證錯誤訊息
    /// </summary>
    /// <param name="message">錯誤訊息</param>
    private void ShowValidationError(string message)
    {
        MessageBox.Show(message, "輸入驗證", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    /// <summary>
    /// 設定 UI 處理中狀態
    /// </summary>
    /// <param name="isProcessing">是否處理中</param>
    private void SetUiProcessingState(bool isProcessing)
    {
        txtGitUrl.Enabled = !isProcessing;
        txtPat.Enabled = !isProcessing;
        txtBaseCommit.Enabled = !isProcessing;
        txtCompareCommit.Enabled = !isProcessing;
        btnCompare.Enabled = !isProcessing;
        btnExport.Enabled = !isProcessing && _currentResults.Count > 0;
        btnCancel.Enabled = isProcessing;
        progressBar.Visible = isProcessing;

        // 新增控制項狀態控制
        bool hasResults = !isProcessing && _currentResults.Count > 0;
        txtBuildOutput.Enabled = hasResults;
        btnBrowseBuildOutput.Enabled = hasResults;
        txtProjectPrefix.Enabled = hasResults;
        txtDeployment.Enabled = hasResults;
        btnBrowseDeployment.Enabled = hasResults;
        txtForceCopyFiles.Enabled = hasResults;
        btnCopyFiles.Enabled = hasResults;

        if (isProcessing)
        {
            lblStatus.Text = "正在處理中...";
            dgvResults.Rows.Clear();
            lblResultCount.Text = "比對結果：處理中...";
        }
    }

    /// <summary>
    /// 顯示比對結果
    /// </summary>
    /// <param name="results">檔案差異清單</param>
    private void DisplayResults(List<FileDiffInfo> results)
    {
        dgvResults.Rows.Clear();

        foreach (var diff in results)
        {
            var rowIndex = dgvResults.Rows.Add(
                diff.FolderPath,
                diff.FileName,
                diff.FileExtension,
                diff.StatusDescription,
                diff.FilePath
            );

            // 根據狀態設定行的背景顏色
            var row = dgvResults.Rows[rowIndex];
            row.DefaultCellStyle.BackColor = diff.Status switch
            {
                FileChangeStatus.Added => Color.FromArgb(220, 255, 220),      // 淺綠色
                FileChangeStatus.Deleted => Color.FromArgb(255, 220, 220),    // 淺紅色
                FileChangeStatus.Modified => Color.FromArgb(255, 255, 220),   // 淺黃色
                FileChangeStatus.Renamed => Color.FromArgb(220, 240, 255),    // 淺藍色
                _ => Color.White
            };
        }

        lblResultCount.Text = $"比對結果：共 {results.Count} 個檔案";
    }

    /// <summary>
    /// 瀏覽建置結果資料夾
    /// </summary>
    private void BtnBrowseBuildOutput_Click(object? sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog();
        dialog.Description = "請選擇 CI 建置結果資料夾";
        if (Directory.Exists(txtBuildOutput.Text))
        {
            dialog.InitialDirectory = txtBuildOutput.Text;
        }

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            txtBuildOutput.Text = dialog.SelectedPath;
        }
    }

    /// <summary>
    /// 瀏覽預計上版資料夾
    /// </summary>
    private void BtnBrowseDeployment_Click(object? sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog();
        dialog.Description = "請選擇預計上版資料夾";
        if (Directory.Exists(txtDeployment.Text))
        {
            dialog.InitialDirectory = txtDeployment.Text;
        }

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            txtDeployment.Text = dialog.SelectedPath;
        }
    }

    /// <summary>
    /// 執行檔案複製
    /// </summary>
    private void BtnCopyFiles_Click(object? sender, EventArgs e)
    {
        var buildPath = txtBuildOutput.Text.Trim();
        var deployPath = txtDeployment.Text.Trim();

        // 驗證輸入
        if (string.IsNullOrWhiteSpace(buildPath))
        {
            MessageBox.Show("請選擇建置結果資料夾", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtBuildOutput.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(deployPath))
        {
            MessageBox.Show("請選擇預計上版資料夾", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtDeployment.Focus();
            return;
        }

        if (!Directory.Exists(buildPath))
        {
            MessageBox.Show($"找不到建置結果資料夾：\n{buildPath}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        // 檢查部署資料夾是否已有檔案
        if (Directory.Exists(deployPath) && Directory.GetFileSystemEntries(deployPath).Length > 0)
        {
            var result = MessageBox.Show(
                $"預計上版資料夾此路徑：\n{deployPath}\n\n該資料夾內已有檔案，是否刪除所有內容並繼續？\n(注意：此操作將清空該資料夾)",
                "確認刪除",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (result != DialogResult.Yes)
            {
                return;
            }

            try
            {
                // 清空資料夾
                var dirInfo = new DirectoryInfo(deployPath);
                foreach (var file in dirInfo.GetFiles()) file.Delete();
                foreach (var dir in dirInfo.GetDirectories()) dir.Delete(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"清空資料夾失敗：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        try
        {
            // 儲存設定
            SaveSettings();

            // 1. 複製 Git 差異檔案
            var projectPrefix = txtProjectPrefix.Text.Trim();
            var gitResult = _fileCopyService.CopyMatchedFiles(_currentResults, buildPath, deployPath, projectPrefix);

            // 2. 複製強制指定的檔案（支援萬用字元）
            var forceCopyList = txtForceCopyFiles.Text.Trim();
            var forcedResult = _fileCopyService.CopyForcedFiles(forceCopyList, buildPath, deployPath);

            // 3. 合併結果（使用 Distinct 去重）
            var totalResult = new FileCopyService.CopyResult
            {
                CopiedCount = gitResult.CopiedCount + forcedResult.CopiedCount,
                CopiedFiles = gitResult.CopiedFiles.Concat(forcedResult.CopiedFiles).ToList(),
                NotFoundFiles = gitResult.NotFoundFiles
                    .Concat(forcedResult.NotFoundFiles)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList()
            };

            // 4. 顯示詳細結果
            var message = $"複製完成！\n\n" +
                          $"【Git 差異檔案】\n" +
                          $"  成功: {gitResult.CopiedCount} 個\n" +
                          $"  未找到: {gitResult.NotFoundFiles.Count} 個\n\n" +
                          $"【強制複製檔案】\n" +
                          $"  成功: {forcedResult.CopiedCount} 個\n" +
                          $"  未找到: {forcedResult.NotFoundFiles.Count} 個\n\n" +
                          $"【總計】\n" +
                          $"  成功: {totalResult.CopiedCount} 個\n" +
                          $"  未找到: {totalResult.NotFoundFiles.Count} 個";

            if (totalResult.NotFoundFiles.Count > 0)
            {
                message += $"\n\n未找到的檔案範例（前 5 個）：\n" +
                           string.Join("\n", totalResult.NotFoundFiles.Take(5).Select(f => $"  • {f}"));

                if (totalResult.NotFoundFiles.Count > 5)
                {
                    message += $"\n  ... 還有 {totalResult.NotFoundFiles.Count - 5} 個";
                }
            }

            MessageBox.Show(message, "處理完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"檔案複製過程發生錯誤：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
