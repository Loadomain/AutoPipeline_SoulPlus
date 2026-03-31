using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;

namespace BaiduAutoDownloader
{
    public partial class AuthForm : Form
    {
        private WebView2 webView;
        private Configuration _config;

        public string AccessToken { get; private set; }

        private RadioButton rbPublic;
        private RadioButton rbCustom;
        private Label lblAppKey;
        private TextBox txtCustomKey;
        private Button btnLoad;
        private Panel topPanel;

        public AuthForm(Configuration config)
        {
            _config = config;
            InitializeComponent();
            SetupUI();
            InitializeAsync();
        }

        private void SetupUI()
        {
            this.Size = new Size(1000, 750);

            topPanel = new Panel { Dock = DockStyle.Top, Height = 70, Padding = new Padding(10) };
            
            rbPublic = new RadioButton { Text = "使用内置开源通道池(推荐,均衡负载防限流)", AutoSize = true, Location = new Point(10, 10) };
            rbCustom = new RadioButton { Text = "使用私人开发者通道(完全独享)", AutoSize = true, Location = new Point(10, 40) };
            
            lblAppKey = new Label { Text = "AppKey:", AutoSize = true, Location = new Point(230, 43) };
            txtCustomKey = new TextBox { Location = new Point(290, 39), Width = 250 };
            btnLoad = new Button { Text = "应用通道并进入授权页", Location = new Point(550, 15), Width = 180, Height = 45 };

            topPanel.Controls.Add(rbPublic);
            topPanel.Controls.Add(rbCustom);
            topPanel.Controls.Add(lblAppKey);
            topPanel.Controls.Add(txtCustomKey);
            topPanel.Controls.Add(btnLoad);
            this.Controls.Add(topPanel);

            // Sync with current config
            if (_config.ApiMode == ApiAuthMode.PublicPool) rbPublic.Checked = true;
            else rbCustom.Checked = true;
            
            txtCustomKey.Text = _config.CustomAppKey;

            btnLoad.Click += BtnLoad_Click;
        }

        private async void InitializeAsync()
        {
            webView = new WebView2
            {
                Dock = DockStyle.Fill
            };
            this.Controls.Add(webView);
            webView.BringToFront(); // Ensure webview is below top panel since it's Dock.Fill

            await webView.EnsureCoreWebView2Async(null);

            webView.NavigationStarting += WebView_NavigationStarting;
            webView.SourceChanged += WebView_SourceChanged;

            LoadAuthPage();
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            _config.ApiMode = rbPublic.Checked ? ApiAuthMode.PublicPool : ApiAuthMode.Custom;
            _config.CustomAppKey = txtCustomKey.Text.Trim();
            _config.Save();
            
            if (_config.ApiMode == ApiAuthMode.Custom && string.IsNullOrEmpty(_config.CustomAppKey))
            {
                MessageBox.Show("私有通道模式下必须填入您的 AppKey。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            LoadAuthPage();
        }

        private void LoadAuthPage()
        {
            if (webView?.CoreWebView2 != null && !string.IsNullOrEmpty(_config.AppKey))
            {
                string authUrl = $"https://openapi.baidu.com/oauth/2.0/authorize?response_type=token&client_id={_config.AppKey}&redirect_uri=oob&scope=basic,netdisk";
                webView.CoreWebView2.Navigate(authUrl);
            }
        }

        private void WebView_SourceChanged(object sender, CoreWebView2SourceChangedEventArgs e)
        {
            CheckUrlForToken(webView.Source.ToString());
        }

        private void WebView_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            CheckUrlForToken(e.Uri);
        }

        private void CheckUrlForToken(string url)
        {
            if (string.IsNullOrEmpty(url)) return;
            
            // Check if oob redirect hit with token
            if (url.Contains("access_token="))
            {
                var uri = new Uri(url);
                string fragment = uri.Fragment;
                if (fragment.StartsWith("#")) fragment = fragment.Substring(1);
                
                var parts = fragment.Split('&');
                foreach (var part in parts)
                {
                    if (part.StartsWith("access_token="))
                    {
                        AccessToken = part.Substring("access_token=".Length);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                        return;
                    }
                }
            }
        }
    }
}
