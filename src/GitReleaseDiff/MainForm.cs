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
    private List<FileDiffInfo> _currentResults = new();
    private CancellationTokenSource? _cancellationTokenSource;

    /// <summary>
    /// 初始化主視窗
    /// </summary>
    public MainForm()
    {
        InitializeComponent();

        _gitService = new GitService();
        _settingsService = new SettingsService();
        _csvExportService = new CsvExportService();

        // 載入上次的設定
        LoadSettings();

        // 綁定事件
        btnCompare.Click += BtnCompare_Click;
        btnExport.Click += BtnExport_Click;
        btnCancel.Click += BtnCancel_Click;
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
            CompareCommitId = txtCompareCommit.Text.Trim()
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
}
