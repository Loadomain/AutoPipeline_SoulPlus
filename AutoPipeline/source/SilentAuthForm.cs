using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System.IO;
using System.Text.Json;

namespace AutoPipeline
{
    public class SilentAuthForm : Form
    {
        private WebView2 webView;
        public string ExtractedCookie { get; private set; } = "";
        public string ExtractedUserAgent { get; private set; } = "";
        public bool Success { get; private set; } = false;

        public SilentAuthForm()
        {
            this.Text = "自动获取鉴权信息 (首次需经过 Cloudflare 5秒盾，请耐心等待...)";
            this.Width = 1000;
            this.Height = 700;
            this.StartPosition = FormStartPosition.CenterParent;

            webView = new WebView2();
            webView.Dock = DockStyle.Fill;
            this.Controls.Add(webView);

            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            try
            {
                await webView.EnsureCoreWebView2Async(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("初始化内置浏览器失败，请确保安装了 WebView2 Runtime。\r\n" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            webView.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
            webView.CoreWebView2.Navigate("https://south-plus.net/thread.php?fid=216&page=1");
        }

        private async void CoreWebView2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                ExtractedUserAgent = webView.CoreWebView2.Settings.UserAgent;
                if (string.IsNullOrEmpty(ExtractedUserAgent))
                {
                    string jsResult = await webView.CoreWebView2.ExecuteScriptAsync("navigator.userAgent");
                    ExtractedUserAgent = jsResult.Trim('"');
                }

                var cookies = await webView.CoreWebView2.CookieManager.GetCookiesAsync("https://south-plus.net");
                bool hasWinduser = cookies.Any(c => c.Name == "eb9e6_winduser");

                string html = await webView.CoreWebView2.ExecuteScriptAsync("document.body.innerText");
                html = html != "null" ? System.Text.RegularExpressions.Regex.Unescape(html).Trim('"') : "";

                bool passedShield = html.Contains("发布新帖") || html.Contains("本版块为正规版块") == false || html.Contains("退出");

                if (hasWinduser && passedShield)
                {
                    this.Text = "获取成功！检测到有效的论坛凭证，正在自动回填并保存...";

                    ExtractedCookie = string.Join("; ", cookies.Select(c => $"{c.Name}={c.Value}"));
                    Success = true;

                    // Write to SP's credentials.json
                    try
                    {
                        string spCredPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..", "SouthPlusDownloader", "publish", "credentials.json");
                        var data = new { Cookie = ExtractedCookie, UserAgent = ExtractedUserAgent };
                        File.WriteAllText(spCredPath, JsonSerializer.Serialize(data));
                    }
                    catch { }

                    await Task.Delay(1500);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    this.Text = "需要您在页面输入账号登录或等待 Cloudflare 验证完成...";
                }
            }
        }
    }
}
