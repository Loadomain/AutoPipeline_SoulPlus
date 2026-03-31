namespace AutoPipeline
{
    partial class Form1
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
            btnOneClick = new System.Windows.Forms.Button();
            groupBox1 = new System.Windows.Forms.GroupBox();
            btnAuthSP = new System.Windows.Forms.Button();
            btnSyncBoards = new System.Windows.Forms.Button();
            cmbBoards = new System.Windows.Forms.ComboBox();
            txtCustomBoard = new System.Windows.Forms.TextBox();
            lblBoard = new System.Windows.Forms.Label();
            lblRange = new System.Windows.Forms.Label();
            txtStartItem = new System.Windows.Forms.TextBox();
            txtEndItem = new System.Windows.Forms.TextBox();
            lblPrice = new System.Windows.Forms.Label();
            nudMaxPrice = new System.Windows.Forms.NumericUpDown();
            lblKeywords = new System.Windows.Forms.Label();
            txtKeywords = new System.Windows.Forms.TextBox();
            lblExtCode = new System.Windows.Forms.Label();
            txtExtractKeywords = new System.Windows.Forms.TextBox();
            lblPwdCode = new System.Windows.Forms.Label();
            txtPasswordKeywords = new System.Windows.Forms.TextBox();
            groupBox2 = new System.Windows.Forms.GroupBox();
            btnAuthBaidu = new System.Windows.Forms.Button();
            rbPublicApi = new System.Windows.Forms.RadioButton();
            rbPrivateApi = new System.Windows.Forms.RadioButton();
            lblAppKey = new System.Windows.Forms.Label();
            txtAppKey = new System.Windows.Forms.TextBox();
            lblSecKey = new System.Windows.Forms.Label();
            txtSecretKey = new System.Windows.Forms.TextBox();
            groupBox3 = new System.Windows.Forms.GroupBox();
            lblTargetDir = new System.Windows.Forms.Label();
            txtTargetDir = new System.Windows.Forms.TextBox();
            btnBrowseTarget = new System.Windows.Forms.Button();
            lblFakeExt = new System.Windows.Forms.Label();
            txtFakeExt = new System.Windows.Forms.TextBox();
            lblRealExt = new System.Windows.Forms.Label();
            txtRealExt = new System.Windows.Forms.TextBox();
            tabControl1 = new System.Windows.Forms.TabControl();
            tabLog = new System.Windows.Forms.TabPage();
            rtbLog = new System.Windows.Forms.RichTextBox();
            tabDownloads = new System.Windows.Forms.TabPage();
            dgvDownloads = new System.Windows.Forms.DataGridView();
            colFile = new System.Windows.Forms.DataGridViewTextBoxColumn();
            colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            colSpeed = new System.Windows.Forms.DataGridViewTextBoxColumn();
            colProgress = new System.Windows.Forms.DataGridViewTextBoxColumn();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(nudMaxPrice)).BeginInit();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            tabControl1.SuspendLayout();
            tabLog.SuspendLayout();
            tabDownloads.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(dgvDownloads)).BeginInit();
            SuspendLayout();
            // 
            // btnOneClick
            // 
            btnOneClick.BackColor = System.Drawing.Color.MediumSeaGreen;
            btnOneClick.Font = new System.Drawing.Font("Microsoft YaHei UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            btnOneClick.ForeColor = System.Drawing.Color.White;
            btnOneClick.Location = new System.Drawing.Point(12, 12);
            btnOneClick.Name = "btnOneClick";
            btnOneClick.Size = new System.Drawing.Size(950, 60);
            btnOneClick.TabIndex = 0;
            btnOneClick.Text = "🚀 全 自 动 流 水 线 开 启 (SP爬取 -> 百度回收 -> 本地提纯)";
            btnOneClick.UseVisualStyleBackColor = false;
            btnOneClick.Click += new System.EventHandler(this.btnOneClick_Click);
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(btnAuthSP);
            groupBox1.Controls.Add(btnSyncBoards);
            groupBox1.Controls.Add(lblBoard);
            groupBox1.Controls.Add(cmbBoards);
            groupBox1.Controls.Add(txtCustomBoard);
            groupBox1.Controls.Add(lblRange);
            groupBox1.Controls.Add(txtStartItem);
            groupBox1.Controls.Add(txtEndItem);
            groupBox1.Controls.Add(lblPrice);
            groupBox1.Controls.Add(nudMaxPrice);
            groupBox1.Controls.Add(lblKeywords);
            groupBox1.Controls.Add(txtKeywords);
            groupBox1.Controls.Add(lblExtCode);
            groupBox1.Controls.Add(txtExtractKeywords);
            groupBox1.Controls.Add(lblPwdCode);
            groupBox1.Controls.Add(txtPasswordKeywords);
            groupBox1.Location = new System.Drawing.Point(12, 85);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(300, 270);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "① 爬虫引擎 (SouthPlus)";
            // 
            // btnAuthSP
            // 
            btnAuthSP.Location = new System.Drawing.Point(15, 25);
            btnAuthSP.Name = "btnAuthSP";
            btnAuthSP.Size = new System.Drawing.Size(130, 25);
            btnAuthSP.TabIndex = 0;
            btnAuthSP.Text = "登录获取授权凭证";
            btnAuthSP.UseVisualStyleBackColor = true;
            btnAuthSP.Click += new System.EventHandler(this.btnAuthSP_Click);
            // 
            // btnSyncBoards
            // 
            btnSyncBoards.Location = new System.Drawing.Point(155, 25);
            btnSyncBoards.Name = "btnSyncBoards";
            btnSyncBoards.Size = new System.Drawing.Size(130, 25);
            btnSyncBoards.TabIndex = 1;
            btnSyncBoards.Text = "云端同步板块";
            btnSyncBoards.UseVisualStyleBackColor = true;
            btnSyncBoards.Click += new System.EventHandler(this.btnSyncBoards_Click);
            // 
            // lblBoard
            // 
            lblBoard.AutoSize = true;
            lblBoard.Location = new System.Drawing.Point(15, 65);
            lblBoard.Name = "lblBoard";
            lblBoard.Size = new System.Drawing.Size(59, 17);
            lblBoard.TabIndex = 2;
            lblBoard.Text = "目标板块:";
            // 
            // cmbBoards
            // 
            cmbBoards.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbBoards.FormattingEnabled = true;
            cmbBoards.Location = new System.Drawing.Point(80, 62);
            cmbBoards.Name = "cmbBoards";
            cmbBoards.Size = new System.Drawing.Size(130, 25);
            cmbBoards.TabIndex = 3;
            // 
            // txtCustomBoard
            // 
            txtCustomBoard.Location = new System.Drawing.Point(220, 62);
            txtCustomBoard.Name = "txtCustomBoard";
            txtCustomBoard.PlaceholderText = "自填FID";
            txtCustomBoard.Size = new System.Drawing.Size(65, 23);
            txtCustomBoard.TabIndex = 4;
            // 
            // lblRange
            // 
            lblRange.AutoSize = true;
            lblRange.Location = new System.Drawing.Point(15, 100);
            lblRange.Name = "lblRange";
            lblRange.Size = new System.Drawing.Size(59, 17);
            lblRange.TabIndex = 5;
            lblRange.Text = "范围条目:";
            // 
            // txtStartItem
            // 
            txtStartItem.Location = new System.Drawing.Point(80, 97);
            txtStartItem.Name = "txtStartItem";
            txtStartItem.Size = new System.Drawing.Size(50, 23);
            txtStartItem.TabIndex = 6;
            txtStartItem.Text = "1";
            // 
            // txtEndItem
            // 
            txtEndItem.Location = new System.Drawing.Point(140, 97);
            txtEndItem.Name = "txtEndItem";
            txtEndItem.Size = new System.Drawing.Size(50, 23);
            txtEndItem.TabIndex = 7;
            txtEndItem.Text = "10";
            // 
            // lblPrice
            // 
            lblPrice.AutoSize = true;
            lblPrice.Location = new System.Drawing.Point(15, 135);
            lblPrice.Name = "lblPrice";
            lblPrice.Size = new System.Drawing.Size(59, 17);
            lblPrice.TabIndex = 8;
            lblPrice.Text = "单价上限:";
            // 
            // nudMaxPrice
            // 
            nudMaxPrice.Location = new System.Drawing.Point(80, 133);
            nudMaxPrice.Name = "nudMaxPrice";
            nudMaxPrice.Size = new System.Drawing.Size(50, 23);
            nudMaxPrice.TabIndex = 9;
            nudMaxPrice.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // lblKeywords
            // 
            lblKeywords.AutoSize = true;
            lblKeywords.Location = new System.Drawing.Point(15, 170);
            lblKeywords.Name = "lblKeywords";
            lblKeywords.Size = new System.Drawing.Size(59, 17);
            lblKeywords.TabIndex = 10;
            lblKeywords.Text = "指定平台:";
            // 
            // txtKeywords
            // 
            txtKeywords.Location = new System.Drawing.Point(80, 167);
            txtKeywords.Name = "txtKeywords";
            txtKeywords.Size = new System.Drawing.Size(205, 23);
            txtKeywords.TabIndex = 11;
            txtKeywords.Text = "百度,网盘,baidu";
            // 
            // lblExtCode
            // 
            lblExtCode.AutoSize = true;
            lblExtCode.Location = new System.Drawing.Point(15, 205);
            lblExtCode.Name = "lblExtCode";
            lblExtCode.Size = new System.Drawing.Size(71, 17);
            lblExtCode.TabIndex = 12;
            lblExtCode.Text = "补提取码词:";
            // 
            // txtExtractKeywords
            // 
            txtExtractKeywords.Location = new System.Drawing.Point(90, 202);
            txtExtractKeywords.Name = "txtExtractKeywords";
            txtExtractKeywords.Size = new System.Drawing.Size(195, 23);
            txtExtractKeywords.TabIndex = 13;
            // 
            // lblPwdCode
            // 
            lblPwdCode.AutoSize = true;
            lblPwdCode.Location = new System.Drawing.Point(15, 240);
            lblPwdCode.Name = "lblPwdCode";
            lblPwdCode.Size = new System.Drawing.Size(71, 17);
            lblPwdCode.TabIndex = 14;
            lblPwdCode.Text = "补解压码词:";
            // 
            // txtPasswordKeywords
            // 
            txtPasswordKeywords.Location = new System.Drawing.Point(90, 237);
            txtPasswordKeywords.Name = "txtPasswordKeywords";
            txtPasswordKeywords.Size = new System.Drawing.Size(195, 23);
            txtPasswordKeywords.TabIndex = 15;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(btnAuthBaidu);
            groupBox2.Controls.Add(rbPublicApi);
            groupBox2.Controls.Add(rbPrivateApi);
            groupBox2.Controls.Add(lblAppKey);
            groupBox2.Controls.Add(txtAppKey);
            groupBox2.Controls.Add(lblSecKey);
            groupBox2.Controls.Add(txtSecretKey);
            groupBox2.Location = new System.Drawing.Point(330, 85);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new System.Drawing.Size(300, 270);
            groupBox2.TabIndex = 2;
            groupBox2.TabStop = false;
            groupBox2.Text = "② 网盘通道 (BaiduAPI)";
            // 
            // btnAuthBaidu
            // 
            btnAuthBaidu.Location = new System.Drawing.Point(15, 25);
            btnAuthBaidu.Name = "btnAuthBaidu";
            btnAuthBaidu.Size = new System.Drawing.Size(270, 25);
            btnAuthBaidu.TabIndex = 0;
            btnAuthBaidu.Text = "激活内嵌网页进行网盘登录并验证Cookie";
            btnAuthBaidu.UseVisualStyleBackColor = true;
            btnAuthBaidu.Click += new System.EventHandler(this.btnAuthBaidu_Click);
            // 
            // rbPublicApi
            // 
            rbPublicApi.AutoSize = true;
            rbPublicApi.Checked = true;
            rbPublicApi.Location = new System.Drawing.Point(15, 65);
            rbPublicApi.Name = "rbPublicApi";
            rbPublicApi.Size = new System.Drawing.Size(176, 21);
            rbPublicApi.TabIndex = 1;
            rbPublicApi.TabStop = true;
            rbPublicApi.Text = "使用默认公用接口 (易限流)";
            rbPublicApi.UseVisualStyleBackColor = true;
            rbPublicApi.CheckedChanged += new System.EventHandler(this.rbApiMode_CheckedChanged);
            // 
            // rbPrivateApi
            // 
            rbPrivateApi.AutoSize = true;
            rbPrivateApi.Location = new System.Drawing.Point(15, 95);
            rbPrivateApi.Name = "rbPrivateApi";
            rbPrivateApi.Size = new System.Drawing.Size(176, 21);
            rbPrivateApi.TabIndex = 2;
            rbPrivateApi.Text = "使用私有 AppKey (全速专属)";
            rbPrivateApi.UseVisualStyleBackColor = true;
            rbPrivateApi.CheckedChanged += new System.EventHandler(this.rbApiMode_CheckedChanged);
            // 
            // lblAppKey
            // 
            lblAppKey.AutoSize = true;
            lblAppKey.Location = new System.Drawing.Point(30, 130);
            lblAppKey.Name = "lblAppKey";
            lblAppKey.Size = new System.Drawing.Size(56, 17);
            lblAppKey.TabIndex = 3;
            lblAppKey.Text = "AppKey:";
            // 
            // txtAppKey
            // 
            txtAppKey.Enabled = false;
            txtAppKey.Location = new System.Drawing.Point(90, 127);
            txtAppKey.Name = "txtAppKey";
            txtAppKey.Size = new System.Drawing.Size(195, 23);
            txtAppKey.TabIndex = 4;
            // 
            // lblSecKey
            // 
            lblSecKey.AutoSize = true;
            lblSecKey.Location = new System.Drawing.Point(23, 165);
            lblSecKey.Name = "lblSecKey";
            lblSecKey.Size = new System.Drawing.Size(63, 17);
            lblSecKey.TabIndex = 5;
            lblSecKey.Text = "SecretKey:";
            // 
            // txtSecretKey
            // 
            txtSecretKey.Enabled = false;
            txtSecretKey.Location = new System.Drawing.Point(90, 162);
            txtSecretKey.Name = "txtSecretKey";
            txtSecretKey.Size = new System.Drawing.Size(195, 23);
            txtSecretKey.TabIndex = 6;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(lblTargetDir);
            groupBox3.Controls.Add(txtTargetDir);
            groupBox3.Controls.Add(btnBrowseTarget);
            groupBox3.Controls.Add(lblFakeExt);
            groupBox3.Controls.Add(txtFakeExt);
            groupBox3.Controls.Add(lblRealExt);
            groupBox3.Controls.Add(txtRealExt);
            groupBox3.Location = new System.Drawing.Point(650, 85);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new System.Drawing.Size(310, 270);
            groupBox3.TabIndex = 3;
            groupBox3.TabStop = false;
            groupBox3.Text = "③ 全局下载与自动解压池设定";
            // 
            // lblTargetDir
            // 
            lblTargetDir.AutoSize = true;
            lblTargetDir.Location = new System.Drawing.Point(15, 30);
            lblTargetDir.Name = "lblTargetDir";
            lblTargetDir.Size = new System.Drawing.Size(83, 17);
            lblTargetDir.TabIndex = 0;
            lblTargetDir.Text = "全局目标目录:";
            // 
            // txtTargetDir
            // 
            txtTargetDir.Location = new System.Drawing.Point(15, 55);
            txtTargetDir.Name = "txtTargetDir";
            txtTargetDir.Size = new System.Drawing.Size(280, 23);
            txtTargetDir.TabIndex = 1;
            txtTargetDir.Text = "D:\\AutoPipeline_Library";
            // 
            // btnBrowseTarget
            // 
            btnBrowseTarget.Location = new System.Drawing.Point(15, 85);
            btnBrowseTarget.Name = "btnBrowseTarget";
            btnBrowseTarget.Size = new System.Drawing.Size(83, 25);
            btnBrowseTarget.TabIndex = 2;
            btnBrowseTarget.Text = "选择目录...";
            btnBrowseTarget.UseVisualStyleBackColor = true;
            btnBrowseTarget.Click += new System.EventHandler(this.btnBrowseTarget_Click);
            // 
            // lblFakeExt
            // 
            lblFakeExt.AutoSize = true;
            lblFakeExt.Location = new System.Drawing.Point(15, 140);
            lblFakeExt.Name = "lblFakeExt";
            lblFakeExt.Size = new System.Drawing.Size(95, 17);
            lblFakeExt.TabIndex = 3;
            lblFakeExt.Text = "解压源虚假后缀:";
            // 
            // txtFakeExt
            // 
            txtFakeExt.Location = new System.Drawing.Point(115, 137);
            txtFakeExt.Name = "txtFakeExt";
            txtFakeExt.PlaceholderText = "(留空则默认扫描无后缀文件)";
            txtFakeExt.Size = new System.Drawing.Size(180, 23);
            txtFakeExt.TabIndex = 4;
            txtFakeExt.Text = "";
            // 
            // lblRealExt
            // 
            lblRealExt.AutoSize = true;
            lblRealExt.Location = new System.Drawing.Point(15, 175);
            lblRealExt.Name = "lblRealExt";
            lblRealExt.Size = new System.Drawing.Size(95, 17);
            lblRealExt.TabIndex = 5;
            lblRealExt.Text = "解压为目标分类:";
            // 
            // txtRealExt
            // 
            txtRealExt.Location = new System.Drawing.Point(115, 172);
            txtRealExt.Name = "txtRealExt";
            txtRealExt.Size = new System.Drawing.Size(100, 23);
            txtRealExt.TabIndex = 6;
            txtRealExt.Text = ".zip";
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabLog);
            tabControl1.Controls.Add(tabDownloads);
            tabControl1.Location = new System.Drawing.Point(12, 365);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(950, 400);
            tabControl1.TabIndex = 4;
            // 
            // tabLog
            // 
            tabLog.Controls.Add(rtbLog);
            tabLog.Location = new System.Drawing.Point(4, 26);
            tabLog.Name = "tabLog";
            tabLog.Padding = new System.Windows.Forms.Padding(3);
            tabLog.Size = new System.Drawing.Size(942, 370);
            tabLog.TabIndex = 0;
            tabLog.Text = "主干作业日志";
            tabLog.UseVisualStyleBackColor = true;
            // 
            // rtbLog
            // 
            rtbLog.BackColor = System.Drawing.SystemColors.WindowText;
            rtbLog.Dock = System.Windows.Forms.DockStyle.Fill;
            rtbLog.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            rtbLog.ForeColor = System.Drawing.SystemColors.Info;
            rtbLog.Location = new System.Drawing.Point(3, 3);
            rtbLog.Name = "rtbLog";
            rtbLog.ReadOnly = true;
            rtbLog.Size = new System.Drawing.Size(936, 364);
            rtbLog.TabIndex = 0;
            rtbLog.Text = "[系统启动完成] 随时准备开始...";
            // 
            // tabDownloads
            // 
            tabDownloads.Controls.Add(dgvDownloads);
            tabDownloads.Location = new System.Drawing.Point(4, 24);
            tabDownloads.Name = "tabDownloads";
            tabDownloads.Padding = new System.Windows.Forms.Padding(3);
            tabDownloads.Size = new System.Drawing.Size(942, 372);
            tabDownloads.TabIndex = 1;
            tabDownloads.Text = "实时下载看板";
            tabDownloads.UseVisualStyleBackColor = true;
            // 
            // dgvDownloads
            // 
            dgvDownloads.AllowUserToAddRows = false;
            dgvDownloads.AllowUserToDeleteRows = false;
            dgvDownloads.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dgvDownloads.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvDownloads.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            colFile,
            colStatus,
            colSpeed,
            colProgress});
            dgvDownloads.Dock = System.Windows.Forms.DockStyle.Fill;
            dgvDownloads.Location = new System.Drawing.Point(3, 3);
            dgvDownloads.Name = "dgvDownloads";
            dgvDownloads.ReadOnly = true;
            dgvDownloads.RowHeadersVisible = false;
            dgvDownloads.RowTemplate.Height = 25;
            dgvDownloads.Size = new System.Drawing.Size(936, 366);
            dgvDownloads.TabIndex = 0;
            // 
            // colFile
            // 
            colFile.FillWeight = 40F;
            colFile.HeaderText = "文件名";
            colFile.Name = "colFile";
            colFile.ReadOnly = true;
            // 
            // colStatus
            // 
            colStatus.FillWeight = 20F;
            colStatus.HeaderText = "状态";
            colStatus.Name = "colStatus";
            colStatus.ReadOnly = true;
            // 
            // colSpeed
            // 
            colSpeed.FillWeight = 20F;
            colSpeed.HeaderText = "速度";
            colSpeed.Name = "colSpeed";
            colSpeed.ReadOnly = true;
            // 
            // colProgress
            // 
            colProgress.FillWeight = 30F;
            colProgress.HeaderText = "网络缓冲进度 (100%)";
            colProgress.Name = "colProgress";
            colProgress.ReadOnly = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(974, 781);
            Controls.Add(tabControl1);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(btnOneClick);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Form1";
            Text = "Auto Pipeline - 集成中控旗舰版 (V2)";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(nudMaxPrice)).EndInit();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            tabControl1.ResumeLayout(false);
            tabLog.ResumeLayout(false);
            tabDownloads.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(dgvDownloads)).EndInit();
            ResumeLayout(false);
        }

        private System.Windows.Forms.Button btnOneClick;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnAuthSP;
        private System.Windows.Forms.Button btnSyncBoards;
        private System.Windows.Forms.Label lblBoard;
        private System.Windows.Forms.ComboBox cmbBoards;
        private System.Windows.Forms.TextBox txtCustomBoard;
        private System.Windows.Forms.Label lblRange;
        private System.Windows.Forms.TextBox txtStartItem;
        private System.Windows.Forms.TextBox txtEndItem;
        private System.Windows.Forms.Label lblPrice;
        private System.Windows.Forms.NumericUpDown nudMaxPrice;
        private System.Windows.Forms.Label lblKeywords;
        private System.Windows.Forms.TextBox txtKeywords;
        private System.Windows.Forms.Label lblExtCode;
        private System.Windows.Forms.TextBox txtExtractKeywords;
        private System.Windows.Forms.Label lblPwdCode;
        private System.Windows.Forms.TextBox txtPasswordKeywords;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnAuthBaidu;
        private System.Windows.Forms.RadioButton rbPublicApi;
        private System.Windows.Forms.RadioButton rbPrivateApi;
        private System.Windows.Forms.Label lblAppKey;
        private System.Windows.Forms.TextBox txtAppKey;
        private System.Windows.Forms.Label lblSecKey;
        private System.Windows.Forms.TextBox txtSecretKey;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label lblTargetDir;
        private System.Windows.Forms.TextBox txtTargetDir;
        private System.Windows.Forms.Button btnBrowseTarget;
        private System.Windows.Forms.Label lblFakeExt;
        private System.Windows.Forms.TextBox txtFakeExt;
        private System.Windows.Forms.Label lblRealExt;
        private System.Windows.Forms.TextBox txtRealExt;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabLog;
        private System.Windows.Forms.RichTextBox rtbLog;
        private System.Windows.Forms.TabPage tabDownloads;
        private System.Windows.Forms.DataGridView dgvDownloads;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFile;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSpeed;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProgress;
    }
}
