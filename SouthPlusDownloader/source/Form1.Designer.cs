namespace SouthPlusDownloader
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblCustomFid = new System.Windows.Forms.Label();
            this.txtCustomFid = new System.Windows.Forms.TextBox();
            this.lblBoard = new System.Windows.Forms.Label();
            this.cmbBoard = new System.Windows.Forms.ComboBox();
            this.lblRange = new System.Windows.Forms.Label();
            this.numStart = new System.Windows.Forms.NumericUpDown();
            this.lblTo = new System.Windows.Forms.Label();
            this.numEnd = new System.Windows.Forms.NumericUpDown();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblMaxPrice = new System.Windows.Forms.Label();
            this.numMaxPrice = new System.Windows.Forms.NumericUpDown();
            this.lblPlatforms = new System.Windows.Forms.Label();
            this.txtPlatforms = new System.Windows.Forms.TextBox();
            this.lblCustomCode = new System.Windows.Forms.Label();
            this.txtCustomCode = new System.Windows.Forms.TextBox();
            this.lblCustomPassword = new System.Windows.Forms.Label();
            this.txtCustomPassword = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lblCookie = new System.Windows.Forms.Label();
            this.txtCookie = new System.Windows.Forms.TextBox();
            this.lblUserAgent = new System.Windows.Forms.Label();
            this.txtUserAgent = new System.Windows.Forms.TextBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnSyncBoards = new System.Windows.Forms.Button();
            this.btnAutoLogin = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numStart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEnd)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxPrice)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            
            // groupBox1
            this.groupBox1.Controls.Add(this.lblCustomFid);
            this.groupBox1.Controls.Add(this.txtCustomFid);
            this.groupBox1.Controls.Add(this.lblBoard);
            this.groupBox1.Controls.Add(this.cmbBoard);
            this.groupBox1.Controls.Add(this.lblRange);
            this.groupBox1.Controls.Add(this.numStart);
            this.groupBox1.Controls.Add(this.lblTo);
            this.groupBox1.Controls.Add(this.numEnd);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(350, 110);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "目标设置";
            
            // lblBoard
            this.lblBoard.AutoSize = true;
            this.lblBoard.Location = new System.Drawing.Point(10, 25);
            this.lblBoard.Name = "lblBoard";
            this.lblBoard.Size = new System.Drawing.Size(65, 12);
            this.lblBoard.Text = "板块选择:";
            
            // cmbBoard
            this.cmbBoard.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBoard.FormattingEnabled = true;
            this.cmbBoard.Location = new System.Drawing.Point(70, 22);
            this.cmbBoard.Name = "cmbBoard";
            this.cmbBoard.Size = new System.Drawing.Size(120, 23);
            this.cmbBoard.TabIndex = 1;
            this.cmbBoard.SelectedIndexChanged += new System.EventHandler(this.cmbBoard_SelectedIndexChanged);
            
            // lblCustomFid
            this.lblCustomFid.AutoSize = true;
            this.lblCustomFid.Location = new System.Drawing.Point(200, 25);
            this.lblCustomFid.Name = "lblCustomFid";
            this.lblCustomFid.Text = "自定义FID:";
            
            // txtCustomFid
            this.txtCustomFid.Location = new System.Drawing.Point(270, 22);
            this.txtCustomFid.Name = "txtCustomFid";
            this.txtCustomFid.Size = new System.Drawing.Size(60, 23);
            this.txtCustomFid.TabIndex = 2;
            
            // lblRange
            this.lblRange.AutoSize = true;
            this.lblRange.Location = new System.Drawing.Point(10, 65);
            this.lblRange.Name = "lblRange";
            this.lblRange.Text = "采集范围 (条):";
            
            // numStart
            this.numStart.Location = new System.Drawing.Point(100, 63);
            this.numStart.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            this.numStart.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numStart.Name = "numStart";
            this.numStart.Size = new System.Drawing.Size(80, 23);
            this.numStart.TabIndex = 3;
            this.numStart.Value = new decimal(new int[] { 1, 0, 0, 0 });
            
            // lblTo
            this.lblTo.AutoSize = true;
            this.lblTo.Location = new System.Drawing.Point(190, 65);
            this.lblTo.Name = "lblTo";
            this.lblTo.Text = "到";
            
            // numEnd
            this.numEnd.Location = new System.Drawing.Point(215, 63);
            this.numEnd.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            this.numEnd.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numEnd.Name = "numEnd";
            this.numEnd.Size = new System.Drawing.Size(80, 23);
            this.numEnd.TabIndex = 4;
            this.numEnd.Value = new decimal(new int[] { 100, 0, 0, 0 });

            // groupBox2
            this.groupBox2.Controls.Add(this.lblMaxPrice);
            this.groupBox2.Controls.Add(this.numMaxPrice);
            this.groupBox2.Controls.Add(this.lblPlatforms);
            this.groupBox2.Controls.Add(this.txtPlatforms);
            this.groupBox2.Controls.Add(this.lblCustomCode);
            this.groupBox2.Controls.Add(this.txtCustomCode);
            this.groupBox2.Controls.Add(this.lblCustomPassword);
            this.groupBox2.Controls.Add(this.txtCustomPassword);
            this.groupBox2.Location = new System.Drawing.Point(380, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(350, 170);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "过滤条件";
            
            // lblMaxPrice
            this.lblMaxPrice.AutoSize = true;
            this.lblMaxPrice.Location = new System.Drawing.Point(10, 25);
            this.lblMaxPrice.Name = "lblMaxPrice";
            this.lblMaxPrice.Text = "单价上限 (SP币):";
            
            // numMaxPrice
            this.numMaxPrice.Location = new System.Drawing.Point(120, 23);
            this.numMaxPrice.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            this.numMaxPrice.Name = "numMaxPrice";
            this.numMaxPrice.Size = new System.Drawing.Size(80, 23);
            this.numMaxPrice.TabIndex = 5;
            this.numMaxPrice.Value = new decimal(new int[] { 5, 0, 0, 0 });
            
            // lblPlatforms
            this.lblPlatforms.AutoSize = true;
            this.lblPlatforms.Location = new System.Drawing.Point(10, 65);
            this.lblPlatforms.Name = "lblPlatforms";
            this.lblPlatforms.Text = "指定平台 (逗号分隔):";
            
            // txtPlatforms
            this.txtPlatforms.Location = new System.Drawing.Point(135, 62);
            this.txtPlatforms.Name = "txtPlatforms";
            this.txtPlatforms.Size = new System.Drawing.Size(200, 23);
            this.txtPlatforms.TabIndex = 6;
            this.txtPlatforms.Text = "百度,网盘,baidu";

            // lblCustomCode
            this.lblCustomCode.AutoSize = true;
            this.lblCustomCode.Location = new System.Drawing.Point(10, 100);
            this.lblCustomCode.Name = "lblCustomCode";
            this.lblCustomCode.Text = "补充提取码:";
            
            // txtCustomCode
            this.txtCustomCode.Location = new System.Drawing.Point(135, 97);
            this.txtCustomCode.Name = "txtCustomCode";
            this.txtCustomCode.Size = new System.Drawing.Size(200, 23);
            this.txtCustomCode.TabIndex = 7;
            this.txtCustomCode.PlaceholderText = "默认词库外自定义关键词";

            // lblCustomPassword
            this.lblCustomPassword.AutoSize = true;
            this.lblCustomPassword.Location = new System.Drawing.Point(10, 135);
            this.lblCustomPassword.Name = "lblCustomPassword";
            this.lblCustomPassword.Text = "补充解压码:";
            
            // txtCustomPassword
            this.txtCustomPassword.Location = new System.Drawing.Point(135, 132);
            this.txtCustomPassword.Name = "txtCustomPassword";
            this.txtCustomPassword.Size = new System.Drawing.Size(200, 23);
            this.txtCustomPassword.TabIndex = 8;
            this.txtCustomPassword.PlaceholderText = "默认词库外自定义关键词";

            // groupBox3
            this.groupBox3.Controls.Add(this.lblCookie);
            this.groupBox3.Controls.Add(this.txtCookie);
            this.groupBox3.Controls.Add(this.lblUserAgent);
            this.groupBox3.Controls.Add(this.txtUserAgent);
            this.groupBox3.Controls.Add(this.btnAutoLogin);
            this.groupBox3.Location = new System.Drawing.Point(12, 190);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(718, 90);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "身份验证";

            // lblCookie
            this.lblCookie.AutoSize = true;
            this.lblCookie.Location = new System.Drawing.Point(10, 25);
            this.lblCookie.Name = "lblCookie";
            this.lblCookie.Text = "Cookie:";
            
            // txtCookie
            this.txtCookie.Location = new System.Drawing.Point(80, 22);
            this.txtCookie.Name = "txtCookie";
            this.txtCookie.Size = new System.Drawing.Size(500, 23);
            this.txtCookie.TabIndex = 7;
            
            // lblUserAgent
            this.lblUserAgent.AutoSize = true;
            this.lblUserAgent.Location = new System.Drawing.Point(10, 55);
            this.lblUserAgent.Name = "lblUserAgent";
            this.lblUserAgent.Text = "UA (重要):";
            
            // txtUserAgent
            this.txtUserAgent.Location = new System.Drawing.Point(80, 52);
            this.txtUserAgent.Name = "txtUserAgent";
            this.txtUserAgent.Size = new System.Drawing.Size(500, 23);
            this.txtUserAgent.TabIndex = 8;
            this.txtUserAgent.Text = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36";
            
            // btnAutoLogin
            this.btnAutoLogin.Location = new System.Drawing.Point(590, 22);
            this.btnAutoLogin.Name = "btnAutoLogin";
            this.btnAutoLogin.Size = new System.Drawing.Size(110, 53);
            this.btnAutoLogin.TabIndex = 9;
            this.btnAutoLogin.Text = "自动获取\r\n(内嵌浏览器)";
            this.btnAutoLogin.UseVisualStyleBackColor = true;
            this.btnAutoLogin.Click += new System.EventHandler(this.btnAutoLogin_Click);
            
            // btnStart
            this.btnStart.Location = new System.Drawing.Point(12, 290);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(100, 35);
            this.btnStart.TabIndex = 11;
            this.btnStart.Text = "开始采集";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            
            // btnStop
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(130, 290);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(100, 35);
            this.btnStop.TabIndex = 4;
            this.btnStop.Text = "停止采集";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Enabled = false;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            
            // btnSyncBoards
            this.btnSyncBoards.Location = new System.Drawing.Point(248, 290);
            this.btnSyncBoards.Name = "btnSyncBoards";
            this.btnSyncBoards.Size = new System.Drawing.Size(160, 35);
            this.btnSyncBoards.TabIndex = 13;
            this.btnSyncBoards.Text = "同步最新版块列表";
            this.btnSyncBoards.UseVisualStyleBackColor = true;
            this.btnSyncBoards.Click += new System.EventHandler(this.btnSyncBoards_Click);
            
            // txtLog
            this.txtLog.Location = new System.Drawing.Point(12, 335);
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.Size = new System.Drawing.Size(718, 250);
            this.txtLog.TabIndex = 14;
            this.txtLog.Text = "";

            // Form1
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(744, 561);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.btnSyncBoards);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "SouthPlus 资源下载辅助器";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numStart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEnd)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxPrice)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblBoard;
        private System.Windows.Forms.ComboBox cmbBoard;
        private System.Windows.Forms.Label lblCustomFid;
        private System.Windows.Forms.TextBox txtCustomFid;
        private System.Windows.Forms.Label lblRange;
        private System.Windows.Forms.NumericUpDown numStart;
        private System.Windows.Forms.Label lblTo;
        private System.Windows.Forms.NumericUpDown numEnd;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lblMaxPrice;
        private System.Windows.Forms.NumericUpDown numMaxPrice;
        private System.Windows.Forms.Label lblPlatforms;
        private System.Windows.Forms.TextBox txtPlatforms;
        private System.Windows.Forms.Label lblCustomCode;
        private System.Windows.Forms.TextBox txtCustomCode;
        private System.Windows.Forms.Label lblCustomPassword;
        private System.Windows.Forms.TextBox txtCustomPassword;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label lblCookie;
        private System.Windows.Forms.TextBox txtCookie;
        private System.Windows.Forms.Label lblUserAgent;
        private System.Windows.Forms.TextBox txtUserAgent;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnSyncBoards;
        private System.Windows.Forms.Button btnAutoLogin;
        private System.Windows.Forms.RichTextBox txtLog;
    }
}
