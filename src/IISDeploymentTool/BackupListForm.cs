using IISDeploymentTool.Models;

namespace IISDeploymentTool;

/// <summary>
/// 備份清單對話框
/// </summary>
public partial class BackupListForm : Form
{
    /// <summary>
    /// 選擇的備份資訊
    /// </summary>
    public BackupInfo? SelectedBackup { get; private set; }

    private readonly List<BackupInfo> _backups;

    /// <summary>
    /// 初始化備份清單對話框
    /// </summary>
    /// <param name="backups">備份清單</param>
    public BackupListForm(List<BackupInfo> backups)
    {
        _backups = backups;
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

        LoadBackups();
    }

    /// <summary>
    /// 載入備份清單
    /// </summary>
    private void LoadBackups()
    {
        dgvBackups.Rows.Clear();

        foreach (var backup in _backups.OrderByDescending(b => b.CreatedTime))
        {
            var row = new DataGridViewRow();
            row.CreateCells(dgvBackups);
            row.Cells[0].Value = backup.FileName;
            row.Cells[1].Value = backup.CreatedTime.ToString("yyyy-MM-dd HH:mm:ss");
            row.Cells[2].Value = FormatFileSize(backup.FileSize);
            row.Cells[3].Value = backup.FileCount;
            row.Tag = backup;

            dgvBackups.Rows.Add(row);
        }

        if (dgvBackups.Rows.Count > 0)
        {
            dgvBackups.Rows[0].Selected = true;
        }
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

    /// <summary>
    /// 確定按鈕點擊事件
    /// </summary>
    private void BtnOk_Click(object? sender, EventArgs e)
    {
        if (dgvBackups.SelectedRows.Count > 0)
        {
            SelectedBackup = dgvBackups.SelectedRows[0].Tag as BackupInfo;
            DialogResult = DialogResult.OK;
            Close();
        }
        else
        {
            MessageBox.Show("請選擇要回滾的備份", "提示",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    /// <summary>
    /// 取消按鈕點擊事件
    /// </summary>
    private void BtnCancel_Click(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }

    /// <summary>
    /// 雙擊列表項時直接確定
    /// </summary>
    private void DgvBackups_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0)
        {
            BtnOk_Click(sender, e);
        }
    }
}
