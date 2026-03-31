namespace AutoExtractor;

partial class PasswordForm
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
        this.lblPrompt = new System.Windows.Forms.Label();
        this.txtPassword = new System.Windows.Forms.TextBox();
        this.chkRememberPassword = new System.Windows.Forms.CheckBox();
        this.btnOk = new System.Windows.Forms.Button();
        this.btnCancel = new System.Windows.Forms.Button();
        this.SuspendLayout();
        
        // lblPrompt
        this.lblPrompt.AutoSize = true;
        this.lblPrompt.Location = new System.Drawing.Point(12, 19);
        this.lblPrompt.Name = "lblPrompt";
        this.lblPrompt.Size = new System.Drawing.Size(126, 17);
        this.lblPrompt.TabIndex = 0;
        this.lblPrompt.Text = "Please enter password:";
        
        // txtPassword
        this.txtPassword.Location = new System.Drawing.Point(15, 39);
        this.txtPassword.Name = "txtPassword";
        this.txtPassword.PasswordChar = '*';
        this.txtPassword.Size = new System.Drawing.Size(326, 23);
        this.txtPassword.TabIndex = 1;
        
        // chkRememberPassword
        this.chkRememberPassword.AutoSize = true;
        this.chkRememberPassword.Location = new System.Drawing.Point(15, 68);
        this.chkRememberPassword.Name = "chkRememberPassword";
        this.chkRememberPassword.Size = new System.Drawing.Size(193, 21);
        this.chkRememberPassword.TabIndex = 2;
        this.chkRememberPassword.Text = "Remember this password";
        this.chkRememberPassword.UseVisualStyleBackColor = true;
        
        // btnOk
        this.btnOk.Location = new System.Drawing.Point(176, 107);
        this.btnOk.Name = "btnOk";
        this.btnOk.Size = new System.Drawing.Size(75, 29);
        this.btnOk.TabIndex = 3;
        this.btnOk.Text = "OK";
        this.btnOk.UseVisualStyleBackColor = true;
        this.btnOk.Click += new System.EventHandler(this.BtnOk_Click);
        
        // btnCancel
        this.btnCancel.Location = new System.Drawing.Point(266, 107);
        this.btnCancel.Name = "btnCancel";
        this.btnCancel.Size = new System.Drawing.Size(75, 29);
        this.btnCancel.TabIndex = 4;
        this.btnCancel.Text = "Cancel";
        this.btnCancel.UseVisualStyleBackColor = true;
        this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
        
        // PasswordForm
        this.AcceptButton = this.btnOk;
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.CancelButton = this.btnCancel;
        this.ClientSize = new System.Drawing.Size(353, 148);
        this.Controls.Add(this.btnCancel);
        this.Controls.Add(this.btnOk);
        this.Controls.Add(this.chkRememberPassword);
        this.Controls.Add(this.txtPassword);
        this.Controls.Add(this.lblPrompt);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "PasswordForm";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        this.Text = "Password Required";
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private System.Windows.Forms.Label lblPrompt;
    private System.Windows.Forms.TextBox txtPassword;
    private System.Windows.Forms.CheckBox chkRememberPassword;
    private System.Windows.Forms.Button btnOk;
    private System.Windows.Forms.Button btnCancel;
}
