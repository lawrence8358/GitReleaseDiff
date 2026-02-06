namespace IISDeploymentTool;

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
        lblSourceFolder = new Label();
        txtSourceFolder = new TextBox();
        btnBrowseSource = new Button();
        lblIISFolder = new Label();
        txtIISFolder = new TextBox();
        btnBrowseIIS = new Button();
        lblBackupFolder = new Label();
        txtBackupFolder = new TextBox();
        btnBrowseBackup = new Button();
        lblAppPool = new Label();
        txtAppPool = new TextBox();
        buttonPanel = new FlowLayoutPanel();
        btnDeploy = new Button();
        btnRollback = new Button();
        btnCancel = new Button();
        lblLastDeploy = new Label();
        lblLastDeployTime = new Label();
        logPanel = new Panel();
        txtLog = new TextBox();
        lblLogTitle = new Label();
        statusPanel = new Panel();
        progressBar = new ProgressBar();
        lblStatus = new Label();
        mainPanel.SuspendLayout();
        inputPanel.SuspendLayout();
        buttonPanel.SuspendLayout();
        logPanel.SuspendLayout();
        statusPanel.SuspendLayout();
        SuspendLayout();
        //
        // mainPanel
        //
        mainPanel.ColumnCount = 1;
        mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        mainPanel.Controls.Add(inputPanel, 0, 0);
        mainPanel.Controls.Add(logPanel, 0, 1);
        mainPanel.Controls.Add(statusPanel, 0, 2);
        mainPanel.Dock = DockStyle.Fill;
        mainPanel.Location = new Point(0, 0);
        mainPanel.Name = "mainPanel";
        mainPanel.Padding = new Padding(10);
        mainPanel.RowCount = 3;
        mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainPanel.Size = new Size(800, 600);
        mainPanel.TabIndex = 0;
        //
        // inputPanel
        //
        inputPanel.AutoSize = true;
        inputPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        inputPanel.ColumnCount = 3;
        inputPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        inputPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        inputPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        inputPanel.Controls.Add(lblSourceFolder, 0, 0);
        inputPanel.Controls.Add(txtSourceFolder, 1, 0);
        inputPanel.Controls.Add(btnBrowseSource, 2, 0);
        inputPanel.Controls.Add(lblIISFolder, 0, 1);
        inputPanel.Controls.Add(txtIISFolder, 1, 1);
        inputPanel.Controls.Add(btnBrowseIIS, 2, 1);
        inputPanel.Controls.Add(lblBackupFolder, 0, 2);
        inputPanel.Controls.Add(txtBackupFolder, 1, 2);
        inputPanel.Controls.Add(btnBrowseBackup, 2, 2);
        inputPanel.Controls.Add(lblAppPool, 0, 3);
        inputPanel.Controls.Add(txtAppPool, 1, 3);
        inputPanel.Controls.Add(buttonPanel, 1, 4);
        inputPanel.Controls.Add(lblLastDeploy, 0, 5);
        inputPanel.Controls.Add(lblLastDeployTime, 1, 5);
        inputPanel.Dock = DockStyle.Top;
        inputPanel.Location = new Point(13, 13);
        inputPanel.Name = "inputPanel";
        inputPanel.RowCount = 6;
        inputPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        inputPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        inputPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        inputPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        inputPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        inputPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        inputPanel.Size = new Size(774, 180);
        inputPanel.TabIndex = 0;
        //
        // lblSourceFolder
        //
        lblSourceFolder.Anchor = AnchorStyles.Left;
        lblSourceFolder.AutoSize = true;
        lblSourceFolder.Location = new Point(3, 8);
        lblSourceFolder.Name = "lblSourceFolder";
        lblSourceFolder.Size = new Size(116, 15);
        lblSourceFolder.TabIndex = 0;
        lblSourceFolder.Text = "上版程式路徑：";
        //
        // txtSourceFolder
        //
        txtSourceFolder.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        txtSourceFolder.Location = new Point(125, 5);
        txtSourceFolder.Name = "txtSourceFolder";
        txtSourceFolder.PlaceholderText = "例如：D:\\Deploy\\Release";
        txtSourceFolder.Size = new Size(550, 23);
        txtSourceFolder.TabIndex = 1;
        //
        // btnBrowseSource
        //
        btnBrowseSource.Anchor = AnchorStyles.Left;
        btnBrowseSource.Location = new Point(681, 3);
        btnBrowseSource.Name = "btnBrowseSource";
        btnBrowseSource.Size = new Size(90, 27);
        btnBrowseSource.TabIndex = 2;
        btnBrowseSource.Text = "瀏覽...";
        btnBrowseSource.UseVisualStyleBackColor = true;
        //
        // lblIISFolder
        //
        lblIISFolder.Anchor = AnchorStyles.Left;
        lblIISFolder.AutoSize = true;
        lblIISFolder.Location = new Point(3, 41);
        lblIISFolder.Name = "lblIISFolder";
        lblIISFolder.Size = new Size(116, 15);
        lblIISFolder.TabIndex = 3;
        lblIISFolder.Text = "IIS 站台資料夾：";
        //
        // txtIISFolder
        //
        txtIISFolder.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        txtIISFolder.Location = new Point(125, 38);
        txtIISFolder.Name = "txtIISFolder";
        txtIISFolder.PlaceholderText = "例如：C:\\inetpub\\wwwroot\\MyApp";
        txtIISFolder.Size = new Size(550, 23);
        txtIISFolder.TabIndex = 4;
        //
        // btnBrowseIIS
        //
        btnBrowseIIS.Anchor = AnchorStyles.Left;
        btnBrowseIIS.Location = new Point(681, 36);
        btnBrowseIIS.Name = "btnBrowseIIS";
        btnBrowseIIS.Size = new Size(90, 27);
        btnBrowseIIS.TabIndex = 5;
        btnBrowseIIS.Text = "瀏覽...";
        btnBrowseIIS.UseVisualStyleBackColor = true;
        //
        // lblBackupFolder
        //
        lblBackupFolder.Anchor = AnchorStyles.Left;
        lblBackupFolder.AutoSize = true;
        lblBackupFolder.Location = new Point(3, 74);
        lblBackupFolder.Name = "lblBackupFolder";
        lblBackupFolder.Size = new Size(80, 15);
        lblBackupFolder.TabIndex = 6;
        lblBackupFolder.Text = "備份路徑：";
        //
        // txtBackupFolder
        //
        txtBackupFolder.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        txtBackupFolder.Location = new Point(125, 71);
        txtBackupFolder.Name = "txtBackupFolder";
        txtBackupFolder.PlaceholderText = "例如：D:\\Backups\\MyApp";
        txtBackupFolder.Size = new Size(550, 23);
        txtBackupFolder.TabIndex = 7;
        //
        // btnBrowseBackup
        //
        btnBrowseBackup.Anchor = AnchorStyles.Left;
        btnBrowseBackup.Location = new Point(681, 69);
        btnBrowseBackup.Name = "btnBrowseBackup";
        btnBrowseBackup.Size = new Size(90, 27);
        btnBrowseBackup.TabIndex = 8;
        btnBrowseBackup.Text = "瀏覽...";
        btnBrowseBackup.UseVisualStyleBackColor = true;
        //
        // lblAppPool
        //
        lblAppPool.Anchor = AnchorStyles.Left;
        lblAppPool.AutoSize = true;
        lblAppPool.Location = new Point(3, 107);
        lblAppPool.Name = "lblAppPool";
        lblAppPool.Size = new Size(116, 15);
        lblAppPool.TabIndex = 9;
        lblAppPool.Text = "應用程式池（選填）：";
        //
        // txtAppPool
        //
        txtAppPool.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        txtAppPool.Location = new Point(125, 104);
        txtAppPool.Name = "txtAppPool";
        txtAppPool.PlaceholderText = "例如：MyAppPool（若不填寫則僅使用 app_offline.htm）";
        txtAppPool.Size = new Size(550, 23);
        txtAppPool.TabIndex = 10;
        //
        // buttonPanel
        //
        buttonPanel.AutoSize = true;
        buttonPanel.Controls.Add(btnDeploy);
        buttonPanel.Controls.Add(btnRollback);
        buttonPanel.Controls.Add(btnCancel);
        buttonPanel.Dock = DockStyle.Fill;
        buttonPanel.Location = new Point(125, 135);
        buttonPanel.Name = "buttonPanel";
        buttonPanel.Size = new Size(550, 35);
        buttonPanel.TabIndex = 11;
        //
        // btnDeploy
        //
        btnDeploy.BackColor = Color.FromArgb(0, 122, 204);
        btnDeploy.FlatStyle = FlatStyle.Flat;
        btnDeploy.ForeColor = Color.White;
        btnDeploy.Location = new Point(3, 3);
        btnDeploy.Name = "btnDeploy";
        btnDeploy.Size = new Size(120, 32);
        btnDeploy.TabIndex = 0;
        btnDeploy.Text = "執行上版";
        btnDeploy.UseVisualStyleBackColor = false;
        //
        // btnRollback
        //
        btnRollback.Location = new Point(129, 3);
        btnRollback.Name = "btnRollback";
        btnRollback.Size = new Size(90, 32);
        btnRollback.TabIndex = 1;
        btnRollback.Text = "回滾...";
        btnRollback.UseVisualStyleBackColor = true;
        //
        // btnCancel
        //
        btnCancel.Enabled = false;
        btnCancel.Location = new Point(225, 3);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(90, 32);
        btnCancel.TabIndex = 2;
        btnCancel.Text = "取消";
        btnCancel.UseVisualStyleBackColor = true;
        //
        // lblLastDeploy
        //
        lblLastDeploy.Anchor = AnchorStyles.Left;
        lblLastDeploy.AutoSize = true;
        lblLastDeploy.ForeColor = Color.Gray;
        lblLastDeploy.Location = new Point(3, 180);
        lblLastDeploy.Name = "lblLastDeploy";
        lblLastDeploy.Size = new Size(80, 15);
        lblLastDeploy.TabIndex = 12;
        lblLastDeploy.Text = "上次部署：";
        //
        // lblLastDeployTime
        //
        lblLastDeployTime.Anchor = AnchorStyles.Left;
        lblLastDeployTime.AutoSize = true;
        lblLastDeployTime.ForeColor = Color.Gray;
        lblLastDeployTime.Location = new Point(125, 180);
        lblLastDeployTime.Name = "lblLastDeployTime";
        lblLastDeployTime.Size = new Size(56, 15);
        lblLastDeployTime.TabIndex = 13;
        lblLastDeployTime.Text = "（無）";
        //
        // logPanel
        //
        logPanel.Controls.Add(txtLog);
        logPanel.Controls.Add(lblLogTitle);
        logPanel.Dock = DockStyle.Fill;
        logPanel.Location = new Point(13, 173);
        logPanel.Name = "logPanel";
        logPanel.Size = new Size(774, 350);
        logPanel.TabIndex = 1;
        //
        // txtLog
        //
        txtLog.BackColor = Color.FromArgb(30, 30, 30);
        txtLog.Dock = DockStyle.Fill;
        txtLog.Font = new Font("Consolas", 9F);
        txtLog.ForeColor = Color.LightGray;
        txtLog.Location = new Point(0, 20);
        txtLog.Multiline = true;
        txtLog.Name = "txtLog";
        txtLog.ReadOnly = true;
        txtLog.ScrollBars = ScrollBars.Vertical;
        txtLog.Size = new Size(774, 330);
        txtLog.TabIndex = 1;
        //
        // lblLogTitle
        //
        lblLogTitle.BackColor = Color.FromArgb(45, 45, 48);
        lblLogTitle.Dock = DockStyle.Top;
        lblLogTitle.Font = new Font("Microsoft JhengHei UI", 9F, FontStyle.Bold);
        lblLogTitle.ForeColor = Color.White;
        lblLogTitle.Location = new Point(0, 0);
        lblLogTitle.Name = "lblLogTitle";
        lblLogTitle.Padding = new Padding(5, 0, 0, 0);
        lblLogTitle.Size = new Size(774, 20);
        lblLogTitle.TabIndex = 0;
        lblLogTitle.Text = "執行日誌";
        lblLogTitle.TextAlign = ContentAlignment.MiddleLeft;
        //
        // statusPanel
        //
        statusPanel.AutoSize = true;
        statusPanel.Controls.Add(progressBar);
        statusPanel.Controls.Add(lblStatus);
        statusPanel.Dock = DockStyle.Bottom;
        statusPanel.Location = new Point(13, 529);
        statusPanel.Name = "statusPanel";
        statusPanel.Size = new Size(774, 58);
        statusPanel.TabIndex = 2;
        //
        // progressBar
        //
        progressBar.Dock = DockStyle.Top;
        progressBar.Location = new Point(0, 20);
        progressBar.MarqueeAnimationSpeed = 30;
        progressBar.Name = "progressBar";
        progressBar.Size = new Size(774, 23);
        progressBar.Style = ProgressBarStyle.Marquee;
        progressBar.TabIndex = 1;
        progressBar.Visible = false;
        //
        // lblStatus
        //
        lblStatus.Dock = DockStyle.Top;
        lblStatus.Location = new Point(0, 0);
        lblStatus.Name = "lblStatus";
        lblStatus.Padding = new Padding(0, 5, 0, 0);
        lblStatus.Size = new Size(774, 20);
        lblStatus.TabIndex = 0;
        lblStatus.Text = "就緒";
        //
        // MainForm
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(800, 600);
        Controls.Add(mainPanel);
        MinimumSize = new Size(650, 450);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "IIS 部署工具";
        mainPanel.ResumeLayout(false);
        mainPanel.PerformLayout();
        inputPanel.ResumeLayout(false);
        inputPanel.PerformLayout();
        buttonPanel.ResumeLayout(false);
        logPanel.ResumeLayout(false);
        logPanel.PerformLayout();
        statusPanel.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion

    private TableLayoutPanel mainPanel;
    private TableLayoutPanel inputPanel;
    private Label lblSourceFolder;
    private TextBox txtSourceFolder;
    private Button btnBrowseSource;
    private Label lblIISFolder;
    private TextBox txtIISFolder;
    private Button btnBrowseIIS;
    private Label lblBackupFolder;
    private TextBox txtBackupFolder;
    private Button btnBrowseBackup;
    private Label lblAppPool;
    private TextBox txtAppPool;
    private FlowLayoutPanel buttonPanel;
    private Button btnDeploy;
    private Button btnRollback;
    private Button btnCancel;
    private Label lblLastDeploy;
    private Label lblLastDeployTime;
    private Panel logPanel;
    private TextBox txtLog;
    private Label lblLogTitle;
    private Panel statusPanel;
    private ProgressBar progressBar;
    private Label lblStatus;
}
