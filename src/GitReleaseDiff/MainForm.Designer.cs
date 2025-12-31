namespace GitReleaseDiff;

partial class MainForm
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        mainPanel = new TableLayoutPanel();
        inputPanel = new TableLayoutPanel();
        lblGitUrl = new Label();
        txtGitUrl = new TextBox();
        lblPat = new Label();
        txtPat = new TextBox();
        lblBaseCommit = new Label();
        txtBaseCommit = new TextBox();
        lblCompareCommit = new Label();
        txtCompareCommit = new TextBox();
        buttonPanel = new FlowLayoutPanel();
        btnCompare = new Button();
        btnExport = new Button();
        btnCancel = new Button();
        resultPanel = new Panel();
        dgvResults = new DataGridView();
        dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
        dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
        dataGridViewTextBoxColumn3 = new DataGridViewTextBoxColumn();
        dataGridViewTextBoxColumn4 = new DataGridViewTextBoxColumn();
        dataGridViewTextBoxColumn5 = new DataGridViewTextBoxColumn();
        lblResultCount = new Label();
        statusPanel = new Panel();
        lblStatus = new Label();
        progressBar = new ProgressBar();
        mainPanel.SuspendLayout();
        inputPanel.SuspendLayout();
        buttonPanel.SuspendLayout();
        resultPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvResults).BeginInit();
        statusPanel.SuspendLayout();
        SuspendLayout();
        // 
        // mainPanel
        // 
        mainPanel.ColumnCount = 1;
        mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        mainPanel.Controls.Add(inputPanel, 0, 0);
        mainPanel.Controls.Add(resultPanel, 0, 1);
        mainPanel.Controls.Add(statusPanel, 0, 2);
        mainPanel.Dock = DockStyle.Fill;
        mainPanel.Location = new Point(0, 0);
        mainPanel.Name = "mainPanel";
        mainPanel.Padding = new Padding(10);
        mainPanel.RowCount = 3;
        mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainPanel.Size = new Size(900, 650);
        mainPanel.TabIndex = 0;
        // 
        // inputPanel
        // 
        inputPanel.AutoSize = true;
        inputPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        inputPanel.ColumnCount = 2;
        inputPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        inputPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        inputPanel.Controls.Add(lblGitUrl, 0, 0);
        inputPanel.Controls.Add(txtGitUrl, 1, 0);
        inputPanel.Controls.Add(lblPat, 0, 1);
        inputPanel.Controls.Add(txtPat, 1, 1);
        inputPanel.Controls.Add(lblBaseCommit, 0, 2);
        inputPanel.Controls.Add(txtBaseCommit, 1, 2);
        inputPanel.Controls.Add(lblCompareCommit, 0, 3);
        inputPanel.Controls.Add(txtCompareCommit, 1, 3);
        inputPanel.Controls.Add(buttonPanel, 1, 4);
        inputPanel.Dock = DockStyle.Fill;
        inputPanel.Location = new Point(13, 13);
        inputPanel.Name = "inputPanel";
        inputPanel.RowCount = 5;
        inputPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        inputPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        inputPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        inputPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        inputPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        inputPanel.Size = new Size(874, 149);
        inputPanel.TabIndex = 0;
        // 
        // lblGitUrl
        // 
        lblGitUrl.Anchor = AnchorStyles.Left;
        lblGitUrl.AutoSize = true;
        lblGitUrl.Location = new Point(3, 8);
        lblGitUrl.Margin = new Padding(3, 8, 3, 3);
        lblGitUrl.Name = "lblGitUrl";
        lblGitUrl.Size = new Size(89, 18);
        lblGitUrl.TabIndex = 0;
        lblGitUrl.Text = "Git 網址：";
        // 
        // txtGitUrl
        // 
        txtGitUrl.Dock = DockStyle.Fill;
        txtGitUrl.Location = new Point(185, 5);
        txtGitUrl.Margin = new Padding(3, 5, 3, 5);
        txtGitUrl.Name = "txtGitUrl";
        txtGitUrl.PlaceholderText = "例如: https://dev.azure.com/organization/ProjectName/_git/RepoName";
        txtGitUrl.Size = new Size(686, 25);
        txtGitUrl.TabIndex = 1;
        // 
        // lblPat
        // 
        lblPat.Anchor = AnchorStyles.Left;
        lblPat.AutoSize = true;
        lblPat.Location = new Point(3, 43);
        lblPat.Margin = new Padding(3, 8, 3, 3);
        lblPat.Name = "lblPat";
        lblPat.Size = new Size(176, 18);
        lblPat.TabIndex = 2;
        lblPat.Text = "Personal Access Token：";
        // 
        // txtPat
        // 
        txtPat.Dock = DockStyle.Fill;
        txtPat.Location = new Point(185, 40);
        txtPat.Margin = new Padding(3, 5, 3, 5);
        txtPat.Name = "txtPat";
        txtPat.PasswordChar = '●';
        txtPat.PlaceholderText = "請輸入 Azure DevOps Personal Access Token";
        txtPat.Size = new Size(686, 25);
        txtPat.TabIndex = 3;
        // 
        // lblBaseCommit
        // 
        lblBaseCommit.Anchor = AnchorStyles.Left;
        lblBaseCommit.AutoSize = true;
        lblBaseCommit.Location = new Point(3, 78);
        lblBaseCommit.Margin = new Padding(3, 8, 3, 3);
        lblBaseCommit.Name = "lblBaseCommit";
        lblBaseCommit.Size = new Size(142, 18);
        lblBaseCommit.TabIndex = 4;
        lblBaseCommit.Text = "基準 Commit ID：";
        // 
        // txtBaseCommit
        // 
        txtBaseCommit.Dock = DockStyle.Fill;
        txtBaseCommit.Location = new Point(185, 75);
        txtBaseCommit.Margin = new Padding(3, 5, 3, 5);
        txtBaseCommit.Name = "txtBaseCommit";
        txtBaseCommit.PlaceholderText = "例如: 6d7bdd0e 或完整 SHA";
        txtBaseCommit.Size = new Size(686, 25);
        txtBaseCommit.TabIndex = 5;
        lblBaseCommit.Size = new Size(142, 18);
        lblBaseCommit.TabIndex = 6;
        lblBaseCommit.Text = "基準 Commit ID：";
        // 
        // txtBaseCommit
        // 
        txtBaseCommit.Dock = DockStyle.Fill;
        // 
        // lblCompareCommit
        // 
        lblCompareCommit.Anchor = AnchorStyles.Left;
        lblCompareCommit.AutoSize = true;
        lblCompareCommit.Location = new Point(3, 113);
        lblCompareCommit.Margin = new Padding(3, 8, 3, 3);
        lblCompareCommit.Name = "lblCompareCommit";
        lblCompareCommit.Size = new Size(142, 18);
        lblCompareCommit.TabIndex = 6;
        lblCompareCommit.Text = "比對 Commit ID：";
        // 
        // txtCompareCommit
        // 
        txtCompareCommit.Dock = DockStyle.Fill;
        txtCompareCommit.Location = new Point(185, 110);
        txtCompareCommit.Margin = new Padding(3, 5, 3, 5);
        txtCompareCommit.Name = "txtCompareCommit";
        txtCompareCommit.PlaceholderText = "例如: a1b2c3d4 或完整 SHA";
        txtCompareCommit.Size = new Size(686, 25);
        txtCompareCommit.TabIndex = 7;
        // 
        // buttonPanel
        // 
        buttonPanel.AutoSize = true;
        buttonPanel.Controls.Add(btnCompare);
        buttonPanel.Controls.Add(btnExport);
        buttonPanel.Controls.Add(btnCancel);
        buttonPanel.Dock = DockStyle.Fill;
        buttonPanel.FlowDirection = FlowDirection.LeftToRight;
        buttonPanel.Location = new Point(185, 145);
        buttonPanel.Margin = new Padding(3, 5, 3, 5);
        buttonPanel.Name = "buttonPanel";
        buttonPanel.Size = new Size(686, 36);
        buttonPanel.TabIndex = 8;
        // 
        // btnCompare
        // 
        btnCompare.Location = new Point(3, 3);
        btnCompare.Name = "btnCompare";
        btnCompare.Size = new Size(100, 30);
        btnCompare.TabIndex = 0;
        btnCompare.Text = "執行比對";
        btnCompare.UseVisualStyleBackColor = true;
        // 
        // btnExport
        // 
        btnExport.Enabled = false;
        btnExport.Location = new Point(119, 3);
        btnExport.Margin = new Padding(13, 3, 3, 3);
        btnExport.Name = "btnExport";
        btnExport.Size = new Size(100, 30);
        btnExport.TabIndex = 1;
        btnExport.Text = "匯出 CSV";
        btnExport.UseVisualStyleBackColor = true;
        // 
        // btnCancel
        // 
        btnCancel.Enabled = false;
        btnCancel.Location = new Point(235, 3);
        btnCancel.Margin = new Padding(13, 3, 3, 3);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(80, 30);
        btnCancel.TabIndex = 2;
        btnCancel.Text = "取消";
        btnCancel.UseVisualStyleBackColor = true;
        // 
        // resultPanel
        // 
        resultPanel.Controls.Add(dgvResults);
        resultPanel.Controls.Add(lblResultCount);
        resultPanel.Dock = DockStyle.Fill;
        resultPanel.Location = new Point(13, 175);
        resultPanel.Name = "resultPanel";
        resultPanel.Padding = new Padding(0, 10, 0, 0);
        resultPanel.Size = new Size(874, 442);
        resultPanel.TabIndex = 1;
        // 
        // dgvResults
        // 
        dgvResults.AllowUserToAddRows = false;
        dgvResults.AllowUserToDeleteRows = false;
        dgvResults.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvResults.BackgroundColor = SystemColors.Window;
        dgvResults.BorderStyle = BorderStyle.Fixed3D;
        dgvResults.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvResults.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn1, dataGridViewTextBoxColumn2, dataGridViewTextBoxColumn3, dataGridViewTextBoxColumn4, dataGridViewTextBoxColumn5 });
        dgvResults.Dock = DockStyle.Fill;
        dgvResults.Location = new Point(0, 28);
        dgvResults.MultiSelect = true;
        dgvResults.Name = "dgvResults";
        dgvResults.ReadOnly = true;
        dgvResults.RowHeadersVisible = false;
        dgvResults.RowHeadersWidth = 49;
        dgvResults.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvResults.Size = new Size(874, 414);
        dgvResults.TabIndex = 0;
        // 
        // dataGridViewTextBoxColumn1
        // 
        dataGridViewTextBoxColumn1.FillWeight = 40F;
        dataGridViewTextBoxColumn1.HeaderText = "資料夾路徑";
        dataGridViewTextBoxColumn1.MinimumWidth = 6;
        dataGridViewTextBoxColumn1.Name = "FolderPath";
        // 
        // dataGridViewTextBoxColumn2
        // 
        dataGridViewTextBoxColumn2.FillWeight = 25F;
        dataGridViewTextBoxColumn2.HeaderText = "檔案名稱";
        dataGridViewTextBoxColumn2.MinimumWidth = 6;
        dataGridViewTextBoxColumn2.Name = "FileName";
        // 
        // dataGridViewTextBoxColumn3
        // 
        dataGridViewTextBoxColumn3.FillWeight = 10F;
        dataGridViewTextBoxColumn3.HeaderText = "附檔名";
        dataGridViewTextBoxColumn3.MinimumWidth = 6;
        dataGridViewTextBoxColumn3.Name = "FileExtension";
        // 
        // dataGridViewTextBoxColumn4
        // 
        dataGridViewTextBoxColumn4.FillWeight = 10F;
        dataGridViewTextBoxColumn4.HeaderText = "狀態";
        dataGridViewTextBoxColumn4.MinimumWidth = 6;
        dataGridViewTextBoxColumn4.Name = "Status";
        // 
        // dataGridViewTextBoxColumn5
        // 
        dataGridViewTextBoxColumn5.FillWeight = 15F;
        dataGridViewTextBoxColumn5.HeaderText = "完整路徑";
        dataGridViewTextBoxColumn5.MinimumWidth = 6;
        dataGridViewTextBoxColumn5.Name = "FilePath";
        dataGridViewTextBoxColumn5.Visible = false;
        // 
        // lblResultCount
        // 
        lblResultCount.AutoSize = true;
        lblResultCount.Dock = DockStyle.Top;
        lblResultCount.Location = new Point(0, 10);
        lblResultCount.Margin = new Padding(0, 0, 0, 5);
        lblResultCount.Name = "lblResultCount";
        lblResultCount.Size = new Size(71, 18);
        lblResultCount.TabIndex = 1;
        lblResultCount.Text = "比對結果：";
        // 
        // statusPanel
        // 
        statusPanel.AutoSize = true;
        statusPanel.Controls.Add(lblStatus);
        statusPanel.Controls.Add(progressBar);
        statusPanel.Dock = DockStyle.Fill;
        statusPanel.Location = new Point(13, 623);
        statusPanel.Name = "statusPanel";
        statusPanel.Size = new Size(874, 14);
        statusPanel.TabIndex = 2;
        // 
        // lblStatus
        // 
        lblStatus.AutoSize = true;
        lblStatus.Dock = DockStyle.Top;
        lblStatus.Location = new Point(0, 20);
        lblStatus.Margin = new Padding(0, 5, 0, 0);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(35, 18);
        lblStatus.TabIndex = 0;
        lblStatus.Text = "就緒";
        // 
        // progressBar
        // 
        progressBar.Dock = DockStyle.Top;
        progressBar.Location = new Point(0, 0);
        progressBar.Name = "progressBar";
        progressBar.Size = new Size(874, 20);
        progressBar.Style = ProgressBarStyle.Marquee;
        progressBar.TabIndex = 1;
        progressBar.Visible = false;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(8F, 18F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(900, 650);
        Controls.Add(mainPanel);
        MinimumSize = new Size(800, 600);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Git 版本比對工具";
        mainPanel.ResumeLayout(false);
        inputPanel.ResumeLayout(false);
        inputPanel.PerformLayout();
        buttonPanel.ResumeLayout(false);
        resultPanel.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dgvResults).EndInit();
        statusPanel.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion

    private TextBox txtGitUrl;
    private TextBox txtPat;
    private TextBox txtBaseCommit;
    private TextBox txtCompareCommit;
    private Button btnCompare;
    private Button btnExport;
    private Button btnCancel;
    private DataGridView dgvResults;
    private Label lblResultCount;
    private Label lblStatus;
    private ProgressBar progressBar;
    private TableLayoutPanel mainPanel;
    private TableLayoutPanel inputPanel;
    private Label lblGitUrl;
    private Label lblPat;
    private Label lblBaseCommit;
    private Label lblCompareCommit;
    private FlowLayoutPanel buttonPanel;
    private Panel resultPanel;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
    private Panel statusPanel;
}
