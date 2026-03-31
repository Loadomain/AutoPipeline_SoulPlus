using System;
using System.Windows.Forms;

namespace AutoExtractor;

public partial class PasswordForm : Form
{
    public string Password { get; private set; } = string.Empty;
    public bool RememberPassword { get; private set; } = false;

    public PasswordForm(string filePath)
    {
        InitializeComponent();
        lblPrompt.Text = $"Please enter password for:\n{System.IO.Path.GetFileName(filePath)}";
    }

    private void BtnOk_Click(object sender, EventArgs e)
    {
        Password = txtPassword.Text;
        RememberPassword = chkRememberPassword.Checked;
        this.DialogResult = DialogResult.OK;
        this.Close();
    }

    private void BtnCancel_Click(object sender, EventArgs e)
    {
        this.DialogResult = DialogResult.Cancel;
        this.Close();
    }
}
