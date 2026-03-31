using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace BaiduAutoDownloader
{
    public partial class MainForm : Form
    {
        private Configuration _config;
        private DownloadManager _downloadManager;

        public MainForm()
        {
            InitializeComponent();
            _downloadManager = new DownloadManager();
            // Bind DownloadManager events later if needed
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _config = Configuration.Load();
            txtTargetDir.Text = _config.TargetDirectory;
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            if (string.IsNullOrEmpty(_config.AccessToken))
            {
                lblStatus.Text = "当前未授权 / 未登录";
                btnStart.Enabled = false;
            }
            else
            {
                lblStatus.Text = "已授权，可以开始任务";
                btnStart.Enabled = true;
            }
        }

        private void btnSelectTargetDir_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "请选择目标下载文件夹";
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    txtTargetDir.Text = fbd.SelectedPath;
                    _config.TargetDirectory = fbd.SelectedPath;
                    _config.Save();
                }
            }
        }

        private void btnAuth_Click(object sender, EventArgs e)
        {
            var authForm = new AuthForm(_config);
            if (authForm.ShowDialog() == DialogResult.OK)
            {
                _config.AccessToken = authForm.AccessToken;
                _config.Save();
                UpdateStatus();
                MessageBox.Show("授权成功！现在可以开始下载任务。", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_config.TargetDirectory))
            {
                MessageBox.Show("请先选择目标下载路径！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtJsonInput.Text))
            {
                MessageBox.Show("请先粘贴包含资源信息的 JSON 数据！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var inputJson = txtJsonInput.Text;
                var rawDict = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(inputJson);
                var resources = new List<ResourceItem>();

                listViewTasks.Items.Clear();

                foreach (var kvp in rawDict)
                {
                    string urlsStr = kvp.Value.ContainsKey("地址") ? kvp.Value["地址"] : "";
                    string pwd = kvp.Value.ContainsKey("提取码") ? kvp.Value["提取码"] : "";
                    string zipPwd = kvp.Value.ContainsKey("解压密码") ? kvp.Value["解压密码"] : "";

                    // Sometimes urls are separated by comma or newlines
                    var urlArray = urlsStr.Split(new[] { ',', ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                    var item = new ResourceItem
                    {
                        Title = kvp.Key,
                        Urls = urlArray,
                        Pwd = pwd,
                        ZipPassword = zipPwd
                    };
                    resources.Add(item);

                    // Add to UI
                    var lvItem = new ListViewItem(item.Title);
                    lvItem.SubItems.Add("等待中...");
                    lvItem.SubItems.Add("-");
                    lvItem.SubItems.Add("0%");
                    lvItem.Tag = item;
                    listViewTasks.Items.Add(lvItem);
                }

                btnStart.Enabled = false;
                lblStatus.Text = "任务正在初始化...";

                // Pass control to DownloadManager asynchronously
                _downloadManager.StartDownloads(resources, _config, this, listViewTasks);
            }
            catch (Exception ex)
            {
                MessageBox.Show("解析 JSON 失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void ReportProgress(ListViewItem item, string status, string speed, string progress)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => ReportProgress(item, status, speed, progress)));
                return;
            }

            if (!string.IsNullOrEmpty(status)) item.SubItems[1].Text = status;
            if (!string.IsNullOrEmpty(speed)) item.SubItems[2].Text = speed;
            if (!string.IsNullOrEmpty(progress)) item.SubItems[3].Text = progress;
        }

        public void TaskCompleted()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(TaskCompleted));
                return;
            }
            btnStart.Enabled = true;
            lblStatus.Text = "所有任务处理完毕";
        }
    }
}
