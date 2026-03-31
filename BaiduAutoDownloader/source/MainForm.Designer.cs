namespace BaiduAutoDownloader;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        btnSelectTargetDir = new Button();
        txtTargetDir = new TextBox();
        label1 = new Label();
        btnAuth = new Button();
        txtJsonInput = new TextBox();
        btnStart = new Button();
        listViewTasks = new ListView();
        colResourceName = new ColumnHeader();
        colStatus = new ColumnHeader();
        colSpeed = new ColumnHeader();
        colProgress = new ColumnHeader();
        lblStatus = new Label();
        SuspendLayout();
        // 
        // btnSelectTargetDir
        // 
        btnSelectTargetDir.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnSelectTargetDir.Location = new Point(697, 12);
        btnSelectTargetDir.Name = "btnSelectTargetDir";
        btnSelectTargetDir.Size = new Size(75, 25);
        btnSelectTargetDir.TabIndex = 0;
        btnSelectTargetDir.Text = "选择...";
        btnSelectTargetDir.UseVisualStyleBackColor = true;
        btnSelectTargetDir.Click += btnSelectTargetDir_Click;
        // 
        // txtTargetDir
        // 
        txtTargetDir.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtTargetDir.Location = new Point(81, 13);
        txtTargetDir.Name = "txtTargetDir";
        txtTargetDir.ReadOnly = true;
        txtTargetDir.Size = new Size(610, 23);
        txtTargetDir.TabIndex = 1;
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(12, 16);
        label1.Name = "label1";
        label1.Size = new Size(68, 17);
        label1.TabIndex = 2;
        label1.Text = "目标路径：";
        // 
        // btnAuth
        // 
        btnAuth.Location = new Point(12, 45);
        btnAuth.Name = "btnAuth";
        btnAuth.Size = new Size(135, 30);
        btnAuth.TabIndex = 3;
        btnAuth.Text = "1. 登录百度网盘授权";
        btnAuth.UseVisualStyleBackColor = true;
        btnAuth.Click += btnAuth_Click;
        // 
        // txtJsonInput
        // 
        txtJsonInput.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
        txtJsonInput.Location = new Point(12, 85);
        txtJsonInput.Multiline = true;
        txtJsonInput.Name = "txtJsonInput";
        txtJsonInput.ScrollBars = ScrollBars.Vertical;
        txtJsonInput.Size = new Size(328, 307);
        txtJsonInput.TabIndex = 4;
        // 
        // btnStart
        // 
        btnStart.Location = new Point(153, 45);
        btnStart.Name = "btnStart";
        btnStart.Size = new Size(187, 30);
        btnStart.TabIndex = 5;
        btnStart.Text = "2. 解析并开始下载任务";
        btnStart.UseVisualStyleBackColor = true;
        btnStart.Click += btnStart_Click;
        // 
        // listViewTasks
        // 
        listViewTasks.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        listViewTasks.Columns.AddRange(new ColumnHeader[] { colResourceName, colStatus, colSpeed, colProgress });
        listViewTasks.FullRowSelect = true;
        listViewTasks.GridLines = true;
        listViewTasks.Location = new Point(346, 85);
        listViewTasks.Name = "listViewTasks";
        listViewTasks.Size = new Size(426, 307);
        listViewTasks.TabIndex = 6;
        listViewTasks.UseCompatibleStateImageBehavior = false;
        listViewTasks.View = View.Details;
        // 
        // colResourceName
        // 
        colResourceName.Text = "资源名称";
        colResourceName.Width = 200;
        // 
        // colStatus
        // 
        colStatus.Text = "状态";
        colStatus.Width = 80;
        // 
        // colSpeed
        // 
        colSpeed.Text = "速度";
        colSpeed.Width = 80;
        // 
        // colProgress
        // 
        colProgress.Text = "进度";
        colProgress.Width = 60;
        // 
        // lblStatus
        // 
        lblStatus.AutoSize = true;
        lblStatus.Location = new Point(346, 52);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(128, 17);
        lblStatus.TabIndex = 7;
        lblStatus.Text = "当前未授权 / 未登录";
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(784, 404);
        Controls.Add(lblStatus);
        Controls.Add(listViewTasks);
        Controls.Add(btnStart);
        Controls.Add(txtJsonInput);
        Controls.Add(btnAuth);
        Controls.Add(label1);
        Controls.Add(txtTargetDir);
        Controls.Add(btnSelectTargetDir);
        Name = "MainForm";
        Text = "Baidu Netdisk Auto Downloader";
        Load += MainForm_Load;
        ResumeLayout(false);
        PerformLayout();
    }

    private Button btnSelectTargetDir;
    private TextBox txtTargetDir;
    private Label label1;
    private Button btnAuth;
    private TextBox txtJsonInput;
    private Button btnStart;
    private ListView listViewTasks;
    private Label lblStatus;
    private ColumnHeader colResourceName;
    private ColumnHeader colStatus;
    private ColumnHeader colSpeed;
    private ColumnHeader colProgress;
}
