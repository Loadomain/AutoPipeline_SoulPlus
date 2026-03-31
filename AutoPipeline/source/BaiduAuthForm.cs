using System;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace AutoPipeline
{
    public class BaiduAuthForm : Form
    {
        private WebView2 webView;
        private string _appKey;

        public bool Success { get; private set; } = false;
        public string AccessToken { get; private set; }

        public BaiduAuthForm(string appKey)
        {
            _appKey = appKey;
            
            this.Text = "网盘极速授权通道";
            this.Width = 600;
            this.Height = 800;
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
                string globalProfilePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..", "BaiduWebViewData"));
                var env = await CoreWebView2Environment.CreateAsync(null, globalProfilePath);
                await webView.EnsureCoreWebView2Async(env);
            }
            catch (Exception ex)
            {
                MessageBox.Show("初始化内置浏览器失败，请确保安装了 WebView2 Runtime。\r\n" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            webView.SourceChanged += WebView_SourceChanged;
            webView.CoreWebView2.NavigationStarting += WebView_NavigationStarting;

            LoadAuthPage();
        }

        private void LoadAuthPage()
        {
            if (webView?.CoreWebView2 != null && !string.IsNullOrEmpty(_appKey))
            {
                string authUrl = $"https://openapi.baidu.com/oauth/2.0/authorize?response_type=token&client_id={_appKey}&redirect_uri=oob&scope=basic,netdisk";
                webView.CoreWebView2.Navigate(authUrl);
            }
            else
            {
                MessageBox.Show("无法加载验证页面，AppKey 缺失或 WebView2 未就绪。");
                this.Close();
            }
        }

        private void WebView_SourceChanged(object? sender, CoreWebView2SourceChangedEventArgs e)
        {
            if (webView.Source != null)
            {
                CheckUrlForToken(webView.Source.ToString());
            }
        }

        private void WebView_NavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
        {
            CheckUrlForToken(e.Uri);
        }

        private void CheckUrlForToken(string url)
        {
            if (string.IsNullOrEmpty(url)) return;
            
            if (url.Contains("access_token="))
            {
                try
                {
                    string fragment = "";
                    int hashIdx = url.IndexOf("#");
                    if (hashIdx >= 0)
                    {
                        fragment = url.Substring(hashIdx + 1);
                    }
                    else
                    {
                        int qmIdx = url.IndexOf("?");
                        if (qmIdx >= 0) fragment = url.Substring(qmIdx + 1);
                        else fragment = url;
                    }
                    
                    var parts = fragment.Split('&');
                    foreach (var part in parts)
                    {
                        if (part.StartsWith("access_token="))
                        {
                            AccessToken = part.Substring("access_token=".Length);
                            this.Success = true;
                            
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                            return;
                        }
                    }
                }
                catch { }
            }
        }
    }
}
