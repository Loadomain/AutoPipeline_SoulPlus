using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoExtractor;

public partial class Form1 : Form
{
    private bool _isProcessing = false;

    public Form1()
    {
        InitializeComponent();
    }

    private void Log(string message)
    {
        if (txtLog.InvokeRequired)
        {
            txtLog.Invoke(new Action(() => Log(message)));
        }
        else
        {
            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.ScrollToCaret();
        }
    }

    private void SetUiState(bool working)
    {
        if (InvokeRequired)
        {
            Invoke(new Action(() => SetUiState(working)));
            return;
        }

        _isProcessing = working;
        btnBrowse.Enabled = !working;
        txtDirectory.Enabled = !working;
        txtFakeExt.Enabled = !working;
        cmbTargetExt.Enabled = !working;
        btnStart.Enabled = !working;
    }

    private void BtnBrowse_Click(object sender, EventArgs e)
    {
        using (var fbd = new FolderBrowserDialog())
        {
            fbd.Description = "Select the root directory to process";
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                txtDirectory.Text = fbd.SelectedPath;
            }
        }
    }

    private async void BtnStart_Click(object sender, EventArgs e)
    {
        var dir = txtDirectory.Text;
        if (string.IsNullOrWhiteSpace(dir) || !Directory.Exists(dir))
        {
            MessageBox.Show("Please select a valid directory.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        var fakeExt = txtFakeExt.Text.Trim();
        var targetExt = cmbTargetExt.Text;
        if (string.IsNullOrEmpty(targetExt))
        {
            MessageBox.Show("Please select a target extension.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        SetUiState(true);
        txtLog.Clear();

        var options = new ExtractorCoreOptions
        {
            RootDirectory = dir,
            FakeExtension = fakeExt,
            TargetExtension = targetExt,
            SilentMode = false
        };

        var core = new ExtractorCore(options);
        core.OnLog += Log;
        core.OnPasswordRequired += filePath =>
        {
            string? inputPw = null;
            bool remember = false;
            Invoke(new Action(() =>
            {
                using (var pf = new PasswordForm(filePath))
                {
                    if (pf.ShowDialog(this) == DialogResult.OK)
                    {
                        inputPw = pf.Password;
                        remember = pf.RememberPassword;
                    }
                }
            }));
            return (inputPw, remember);
        };

        try
        {
            await Task.Run(() => core.Run());
        }
        catch (Exception ex)
        {
            Log($"Fatal Error: {ex.Message}");
        }
        finally
        {
            SetUiState(false);
        }
    }
}
