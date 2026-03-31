namespace AutoExtractor;

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
        this.lblDirectory = new System.Windows.Forms.Label();
        this.txtDirectory = new System.Windows.Forms.TextBox();
        this.btnBrowse = new System.Windows.Forms.Button();
        this.lblFakeExt = new System.Windows.Forms.Label();
        this.txtFakeExt = new System.Windows.Forms.TextBox();
        this.lblTargetExt = new System.Windows.Forms.Label();
        this.cmbTargetExt = new System.Windows.Forms.ComboBox();
        this.btnStart = new System.Windows.Forms.Button();
        this.txtLog = new System.Windows.Forms.TextBox();
        this.SuspendLayout();

        // lblDirectory
        this.lblDirectory.AutoSize = true;
        this.lblDirectory.Location = new System.Drawing.Point(12, 16);
        this.lblDirectory.Name = "lblDirectory";
        this.lblDirectory.Size = new System.Drawing.Size(107, 17);
        this.lblDirectory.TabIndex = 0;
        this.lblDirectory.Text = "Source Directory:";

        // txtDirectory
        this.txtDirectory.Location = new System.Drawing.Point(125, 13);
        this.txtDirectory.Name = "txtDirectory";
        this.txtDirectory.Size = new System.Drawing.Size(398, 23);
        this.txtDirectory.TabIndex = 1;

        // btnBrowse
        this.btnBrowse.Location = new System.Drawing.Point(529, 12);
        this.btnBrowse.Name = "btnBrowse";
        this.btnBrowse.Size = new System.Drawing.Size(75, 25);
        this.btnBrowse.TabIndex = 2;
        this.btnBrowse.Text = "Browse...";
        this.btnBrowse.UseVisualStyleBackColor = true;
        this.btnBrowse.Click += new System.EventHandler(this.BtnBrowse_Click);

        // lblFakeExt
        this.lblFakeExt.AutoSize = true;
        this.lblFakeExt.Location = new System.Drawing.Point(12, 53);
        this.lblFakeExt.Name = "lblFakeExt";
        this.lblFakeExt.Size = new System.Drawing.Size(252, 17);
        this.lblFakeExt.TabIndex = 3;
        this.lblFakeExt.Text = "Fake Extension (e.g. .mp4, leave empty if none):";

        // txtFakeExt
        this.txtFakeExt.Location = new System.Drawing.Point(270, 50);
        this.txtFakeExt.Name = "txtFakeExt";
        this.txtFakeExt.Size = new System.Drawing.Size(100, 23);
        this.txtFakeExt.TabIndex = 4;

        // lblTargetExt
        this.lblTargetExt.AutoSize = true;
        this.lblTargetExt.Location = new System.Drawing.Point(386, 53);
        this.lblTargetExt.Name = "lblTargetExt";
        this.lblTargetExt.Size = new System.Drawing.Size(106, 17);
        this.lblTargetExt.TabIndex = 5;
        this.lblTargetExt.Text = "Target Extension:";

        // cmbTargetExt
        this.cmbTargetExt.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.cmbTargetExt.FormattingEnabled = true;
        this.cmbTargetExt.Items.AddRange(new object[] { ".zip", ".rar", ".7z" });
        this.cmbTargetExt.Location = new System.Drawing.Point(498, 50);
        this.cmbTargetExt.Name = "cmbTargetExt";
        this.cmbTargetExt.Size = new System.Drawing.Size(106, 25);
        this.cmbTargetExt.TabIndex = 6;
        this.cmbTargetExt.SelectedIndex = 0;

        // btnStart
        this.btnStart.Location = new System.Drawing.Point(620, 12);
        this.btnStart.Name = "btnStart";
        this.btnStart.Size = new System.Drawing.Size(125, 63);
        this.btnStart.TabIndex = 7;
        this.btnStart.Text = "Start Extractor";
        this.btnStart.UseVisualStyleBackColor = true;
        this.btnStart.Click += new System.EventHandler(this.BtnStart_Click);

        // txtLog
        this.txtLog.Location = new System.Drawing.Point(15, 96);
        this.txtLog.Multiline = true;
        this.txtLog.Name = "txtLog";
        this.txtLog.ReadOnly = true;
        this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        this.txtLog.Size = new System.Drawing.Size(730, 329);
        this.txtLog.TabIndex = 8;

        // Form1
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(763, 442);
        this.Controls.Add(this.txtLog);
        this.Controls.Add(this.btnStart);
        this.Controls.Add(this.cmbTargetExt);
        this.Controls.Add(this.lblTargetExt);
        this.Controls.Add(this.txtFakeExt);
        this.Controls.Add(this.lblFakeExt);
        this.Controls.Add(this.btnBrowse);
        this.Controls.Add(this.txtDirectory);
        this.Controls.Add(this.lblDirectory);
        this.Name = "Form1";
        this.Text = "AutoExtractor";
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private System.Windows.Forms.Label lblDirectory;
    private System.Windows.Forms.TextBox txtDirectory;
    private System.Windows.Forms.Button btnBrowse;
    private System.Windows.Forms.Label lblFakeExt;
    private System.Windows.Forms.TextBox txtFakeExt;
    private System.Windows.Forms.Label lblTargetExt;
    private System.Windows.Forms.ComboBox cmbTargetExt;
    private System.Windows.Forms.Button btnStart;
    private System.Windows.Forms.TextBox txtLog;
}
