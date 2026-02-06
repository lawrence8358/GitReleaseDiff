namespace IISDeploymentTool;

partial class BackupListForm
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
        lblTitle = new Label();
        dgvBackups = new DataGridView();
        colFileName = new DataGridViewTextBoxColumn();
        colCreatedTime = new DataGridViewTextBoxColumn();
        colFileSize = new DataGridViewTextBoxColumn();
        colFileCount = new DataGridViewTextBoxColumn();
        buttonPanel = new FlowLayoutPanel();
        btnOk = new Button();
        btnCancel = new Button();
        mainPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvBackups).BeginInit();
        buttonPanel.SuspendLayout();
        SuspendLayout();
        //
        // mainPanel
        //
        mainPanel.ColumnCount = 1;
        mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        mainPanel.Controls.Add(lblTitle, 0, 0);
        mainPanel.Controls.Add(dgvBackups, 0, 1);
        mainPanel.Controls.Add(buttonPanel, 0, 2);
        mainPanel.Dock = DockStyle.Fill;
        mainPanel.Location = new Point(0, 0);
        mainPanel.Name = "mainPanel";
        mainPanel.Padding = new Padding(10);
        mainPanel.RowCount = 3;
        mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        mainPanel.Size = new Size(700, 450);
        mainPanel.TabIndex = 0;
        //
        // lblTitle
        //
        lblTitle.AutoSize = true;
        lblTitle.Font = new Font("Microsoft JhengHei UI", 10F, FontStyle.Bold);
        lblTitle.Location = new Point(13, 10);
        lblTitle.Name = "lblTitle";
        lblTitle.Padding = new Padding(0, 0, 0, 10);
        lblTitle.Size = new Size(394, 28);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "請選擇要回滾的備份（雙擊或點擊確定按鈕）：";
        //
        // dgvBackups
        //
        dgvBackups.AllowUserToAddRows = false;
        dgvBackups.AllowUserToDeleteRows = false;
        dgvBackups.AllowUserToResizeRows = false;
        dgvBackups.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvBackups.BackgroundColor = SystemColors.Window;
        dgvBackups.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvBackups.Columns.AddRange(new DataGridViewColumn[] { colFileName, colCreatedTime, colFileSize, colFileCount });
        dgvBackups.Dock = DockStyle.Fill;
        dgvBackups.Location = new Point(13, 41);
        dgvBackups.MultiSelect = false;
        dgvBackups.Name = "dgvBackups";
        dgvBackups.ReadOnly = true;
        dgvBackups.RowHeadersVisible = false;
        dgvBackups.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvBackups.Size = new Size(674, 346);
        dgvBackups.TabIndex = 1;
        dgvBackups.CellDoubleClick += DgvBackups_CellDoubleClick;
        //
        // colFileName
        //
        colFileName.FillWeight = 40F;
        colFileName.HeaderText = "檔案名稱";
        colFileName.Name = "colFileName";
        colFileName.ReadOnly = true;
        //
        // colCreatedTime
        //
        colCreatedTime.FillWeight = 30F;
        colCreatedTime.HeaderText = "建立時間";
        colCreatedTime.Name = "colCreatedTime";
        colCreatedTime.ReadOnly = true;
        //
        // colFileSize
        //
        colFileSize.FillWeight = 15F;
        colFileSize.HeaderText = "大小";
        colFileSize.Name = "colFileSize";
        colFileSize.ReadOnly = true;
        //
        // colFileCount
        //
        colFileCount.FillWeight = 15F;
        colFileCount.HeaderText = "檔案數";
        colFileCount.Name = "colFileCount";
        colFileCount.ReadOnly = true;
        //
        // buttonPanel
        //
        buttonPanel.AutoSize = true;
        buttonPanel.Controls.Add(btnOk);
        buttonPanel.Controls.Add(btnCancel);
        buttonPanel.Dock = DockStyle.Right;
        buttonPanel.FlowDirection = FlowDirection.RightToLeft;
        buttonPanel.Location = new Point(487, 393);
        buttonPanel.Name = "buttonPanel";
        buttonPanel.Padding = new Padding(0, 10, 0, 0);
        buttonPanel.Size = new Size(200, 44);
        buttonPanel.TabIndex = 2;
        //
        // btnOk
        //
        btnOk.BackColor = Color.FromArgb(0, 122, 204);
        btnOk.FlatStyle = FlatStyle.Flat;
        btnOk.ForeColor = Color.White;
        btnOk.Location = new Point(103, 13);
        btnOk.Name = "btnOk";
        btnOk.Size = new Size(94, 28);
        btnOk.TabIndex = 0;
        btnOk.Text = "確定";
        btnOk.UseVisualStyleBackColor = false;
        btnOk.Click += BtnOk_Click;
        //
        // btnCancel
        //
        btnCancel.Location = new Point(3, 13);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(94, 28);
        btnCancel.TabIndex = 1;
        btnCancel.Text = "取消";
        btnCancel.UseVisualStyleBackColor = true;
        btnCancel.Click += BtnCancel_Click;
        //
        // BackupListForm
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(700, 450);
        Controls.Add(mainPanel);
        MaximizeBox = false;
        MinimizeBox = false;
        MinimumSize = new Size(600, 400);
        Name = "BackupListForm";
        ShowIcon = false;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "選擇備份";
        mainPanel.ResumeLayout(false);
        mainPanel.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvBackups).EndInit();
        buttonPanel.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion

    private TableLayoutPanel mainPanel;
    private Label lblTitle;
    private DataGridView dgvBackups;
    private DataGridViewTextBoxColumn colFileName;
    private DataGridViewTextBoxColumn colCreatedTime;
    private DataGridViewTextBoxColumn colFileSize;
    private DataGridViewTextBoxColumn colFileCount;
    private FlowLayoutPanel buttonPanel;
    private Button btnOk;
    private Button btnCancel;
}
