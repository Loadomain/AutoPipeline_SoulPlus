using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SouthPlusDownloader
{
    public partial class Form1 : Form
    {
        private CancellationTokenSource _cancellationTokenSource;
        private CrawlerService _crawlerService;

        public Form1()
        {
            InitializeComponent();
            InitializeBoards();
            LoadSavedCredentials();
        }

        private void LoadSavedCredentials()
        {
            var config = TaskConfig.LoadCredentials();
            if (!string.IsNullOrEmpty(config.Cookie)) txtCookie.Text = config.Cookie;
            if (!string.IsNullOrEmpty(config.UserAgent)) txtUserAgent.Text = config.UserAgent;
            if (!string.IsNullOrEmpty(config.CustomCodeKeywords)) txtCustomCode.Text = config.CustomCodeKeywords;
            if (!string.IsNullOrEmpty(config.CustomPasswordKeywords)) txtCustomPassword.Text = config.CustomPasswordKeywords;
        }

        private void InitializeBoards()
        {
            var config = TaskConfig.LoadCredentials();
            var manager = new BoardManager(config, new HttpClient(), Log);
            var boards = manager.LoadLocalBoards();
            
            if (boards.Count == 0)
            {
                // 默认占位符，提示用户更新
                boards = new Dictionary<string, int>
                {
                    { "自定义", 0 },
                    { "【请点击右下角“同步”获取】", 0 }
                };
            }

            UpdateBoardUI(boards);
        }

        private void UpdateBoardUI(Dictionary<string, int> boards)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateBoardUI(boards)));
                return;
            }
            cmbBoard.DataSource = new BindingSource(boards, null);
            cmbBoard.DisplayMember = "Key";
            cmbBoard.ValueMember = "Value";
            if (cmbBoard.Items.Count > 0) cmbBoard.SelectedIndex = 0;
        }

        private void cmbBoard_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbBoard.SelectedValue is int fid)
            {
                txtCustomFid.Enabled = (fid == 0);
            }
        }

        private void Log(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => Log(message)));
                return;
            }
            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\r\n");
            txtLog.ScrollToCaret();
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCookie.Text))
            {
                MessageBox.Show("请先填入有效的 Cookie。");
                return;
            }

            int targetFid = 0;
            if (cmbBoard.SelectedValue is int fid && fid != 0)
            {
                targetFid = fid;
            }
            else
            {
                if (!int.TryParse(txtCustomFid.Text, out targetFid))
                {
                    MessageBox.Show("自定义 FID 必须为数字。");
                    return;
                }
            }

            var config = new TaskConfig
            {
                BoardId = targetFid,
                StartItem = (int)numStart.Value,
                EndItem = (int)numEnd.Value,
                MaxPrice = (int)numMaxPrice.Value,
                Cookie = txtCookie.Text,
                UserAgent = txtUserAgent.Text,
                Platforms = txtPlatforms.Text.Split(new[] { ',', '，' }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries),
                CustomCodeKeywords = txtCustomCode.Text,
                CustomPasswordKeywords = txtCustomPassword.Text
            };
            config.SaveCredentials(); // 保存到本地文件

            _cancellationTokenSource = new CancellationTokenSource();
            _crawlerService = new CrawlerService(config, Log);
            
            btnStart.Enabled = false;
            btnStop.Enabled = true;

            try
            {
                Log("任务开始...");
                await _crawlerService.StartAsync(_cancellationTokenSource.Token);
                Log("任务完成！");
            }
            catch (OperationCanceledException)
            {
                Log("任务已取消。");
            }
            catch (Exception ex)
            {
                Log($"任务出现严重错误: {ex.Message}");
            }
            finally
            {
                btnStart.Enabled = true;
                btnStop.Enabled = false;
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _cancellationTokenSource?.Cancel();
            btnStop.Enabled = false;
        }

        private async void btnSyncBoards_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCookie.Text))
            {
                MessageBox.Show("请先完成身份验证获取 Cookie。");
                return;
            }

            var config = new TaskConfig { Cookie = txtCookie.Text, UserAgent = txtUserAgent.Text };
            var handler = new HttpClientHandler
            {
                UseCookies = false,
                AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
            };
            using var httpClient = new HttpClient(handler);
            httpClient.DefaultRequestHeaders.Add("User-Agent", config.UserAgent);
            httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8");
            httpClient.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9,en;q=0.8");
            
            var manager = new BoardManager(config, httpClient, Log);

            btnSyncBoards.Enabled = false;
            btnStart.Enabled = false;
            
            _cancellationTokenSource = new CancellationTokenSource();
            try
            {
                var boards = await manager.FetchLatestBoardsAsync(_cancellationTokenSource.Token);
                if (boards != null && boards.Count > 0)
                {
                    UpdateBoardUI(boards);
                    MessageBox.Show("版块列表同步成功！", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (OperationCanceledException)
            {
                Log("同步被取消。");
            }
            catch (Exception ex)
            {
                Log($"同步失败: {ex.Message}");
            }
            finally
            {
                btnSyncBoards.Enabled = true;
                btnStart.Enabled = true;
            }
        }

        private void btnAutoLogin_Click(object sender, EventArgs e)
        {
            using (var browserForm = new BrowserForm())
            {
                if (browserForm.ShowDialog() == DialogResult.OK && browserForm.Success)
                {
                    txtCookie.Text = browserForm.ExtractedCookie;
                    txtUserAgent.Text = browserForm.ExtractedUserAgent;
                    Log("内嵌浏览器认证成功，已自动回填 Cookie 和 UserAgent。");
                    
                    // 同步立刻保存一下
                    var cfg = TaskConfig.LoadCredentials();
                    cfg.Cookie = txtCookie.Text;
                    cfg.UserAgent = txtUserAgent.Text;
                    cfg.SaveCredentials();
                }
                else
                {
                    Log("内嵌浏览器认证取消或获取失败。");
                }
            }
        }
    }
}
