using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace SouthPlusDownloader
{
    public class BrowserForm : Form
    {
        private WebView2 webView;
        public string ExtractedCookie { get; private set; } = "";
        public string ExtractedUserAgent { get; private set; } = "";
        public bool Success { get; private set; } = false;

        public BrowserForm()
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
            // 初始化 WebView2 环境
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
            
            // 直接访问 216 版块列表页，如果有盾就会过盾，要求登录则要求登录
            webView.CoreWebView2.Navigate("https://south-plus.net/thread.php?fid=216&page=1");
        }

        private async void CoreWebView2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                // 获取当前真实 UA
                ExtractedUserAgent = webView.CoreWebView2.Settings.UserAgent;
                if (string.IsNullOrEmpty(ExtractedUserAgent))
                {
                    string jsResult = await webView.CoreWebView2.ExecuteScriptAsync("navigator.userAgent");
                    ExtractedUserAgent = jsResult.Trim('"');
                }

                // 获取目标站的全部 Cookies
                var cookies = await webView.CoreWebView2.CookieManager.GetCookiesAsync("https://south-plus.net");
                
                // 判断是否具备核心 Cookie: eb9e6_winduser (代表用户态标识，是否登录)
                // cf_clearance (这可能不是必须的如果 IP 足够好，但如果有必定需要)
                bool hasWinduser = cookies.Any(c => c.Name == "eb9e6_winduser");

                // 执行 JS 提取页面上的文字特征判断是不是仍被拦在门外
                string html = await webView.CoreWebView2.ExecuteScriptAsync("document.body.innerText");
                html = html != "null" ? System.Text.RegularExpressions.Regex.Unescape(html).Trim('"') : "";

                // 规则：只要不是“只有注册会员才能进入”，并且有 eb9e6_winduser (其实也可以不要winduser只要有帖子内容就行)
                // 稳妥一点，检测页面上是否有 tpc_content 或者 id=read_ 的元素，或者 "发布新帖" 的文字
                bool passedShield = html.Contains("发布新帖") || html.Contains("本版块为正规版块") == false || html.Contains("退出");

                if (hasWinduser && passedShield)
                {
                    this.Text = "获取成功！检测到有效的论坛凭证，正在自动回填...";
                    
                    // 拼接 Cookie
                    ExtractedCookie = string.Join("; ", cookies.Select(c => $"{c.Name}={c.Value}"));
                    Success = true;

                    // 延迟 1.5 秒后自动关闭，让用户看清楚成功提示
                    await Task.Delay(1500);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    this.Text = "请手动完成登录或等待 Cloudflare 验证完成...";
                }
            }
        }
    }
}
