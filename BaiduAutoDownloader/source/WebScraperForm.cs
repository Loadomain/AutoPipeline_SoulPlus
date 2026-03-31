using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace BaiduAutoDownloader
{
    public class ScrapeResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string Uk { get; set; }
        public string ShareId { get; set; }
        public string Bdstoken { get; set; }
        public string Bduss { get; set; }
        public string Stoken { get; set; }
        public string Bdclnd { get; set; }
        public JArray RootFsids { get; set; }
    }

    public class WebScraperForm : Form
    {
        private WebView2 webView;
        private TaskCompletionSource<ScrapeResult> _tcs;
        private string _targetUrl;
        private bool _isParsing = false;

        public WebScraperForm()
        {
            this.ShowInTaskbar = false;
            this.Opacity = 0; // Completely invisible
            this.Width = 800;
            this.Height = 600;

            webView = new WebView2
            {
                Dock = DockStyle.Fill
            };
            this.Controls.Add(webView);
            this.Load += WebScraperForm_Load;
        }

        private async void WebScraperForm_Load(object sender, EventArgs e)
        {
            await webView.EnsureCoreWebView2Async(null);
        }

        public async Task<ScrapeResult> InitializeAndScrapeAsync(string url, string pwd)
        {
            _targetUrl = url;
            if (!string.IsNullOrEmpty(pwd) && !_targetUrl.Contains($"pwd={pwd}") && !_targetUrl.Contains($"pwd="))
            {
                _targetUrl += _targetUrl.Contains("?") ? $"&pwd={pwd}" : $"?pwd={pwd}";
            }

            // Wait until initialized if not already
            while (webView.CoreWebView2 == null)
            {
                await Task.Delay(50);
            }

            // Clear cache and navigate
            webView.CoreWebView2.Navigate(_targetUrl);

            // Poll for up to 60 seconds to allow manual captcha solving
            for (int i = 0; i < 60; i++)
            {
                await Task.Delay(1000);
                // If stuck for 6 seconds, heavily implies captcha, terms of service, or extraction code issue.
                if (i == 6)
                {
                    this.Invoke(new Action(() => 
                    {
                        this.Opacity = 1; // Make it visible!
                        this.Text = "请手动解决验证码或服务条款后再等待自动继续...";
                        this.BringToFront();
                        this.Activate();
                    }));
                }

                try
                {
                    string script = $@"
                        (function() {{
                            try {{
                                // Auto-fill extraction code if the box is present
                                var pwdInput = document.querySelector('.access-box input') || document.querySelector('.T09YQd form input') || document.querySelector('#accessCode');
                                var btn = document.querySelector('.access-box a.g-button') || document.querySelector('.T09YQd form a') || document.querySelector('#getfileBtn');
                                if (pwdInput && btn) {{
                                    if (!pwdInput.value || pwdInput.value.length < 4) {{
                                        pwdInput.value = '{pwd}';
                                        var ev = new Event('input', {{ bubbles: true}});
                                        pwdInput.dispatchEvent(ev);
                                    }}
                                    btn.click();
                                }}

                                var _uk = '';
                                var _share_id = '';
                                var _bdstoken = '';
                                var _fsids = [];
                                
                                if (typeof window.yunData !== 'undefined') {{
                                    try {{
                                        _uk = window.yunData.share_uk || window.yunData.uk;
                                        _share_id = window.yunData.shareid || window.yunData.share_id;
                                        _bdstoken = window.yunData.bdstoken;
                                        var flist = window.yunData.file_list;
                                        if (flist && flist.length > 0) _fsids = flist.map(f => f.fs_id);
                                    }} catch (e) {{}}
                                }}

                                if ((!_uk || !_share_id || _fsids.length === 0) && typeof window.locals !== 'undefined' && typeof window.locals.get === 'function') {{
                                    try {{
                                        _uk = window.locals.get('share_uk') || window.locals.get('uk') || _uk;
                                        _share_id = window.locals.get('shareid') || window.locals.get('share_id') || _share_id;
                                        _bdstoken = window.locals.get('bdstoken') || _bdstoken;
                                        var flist = window.locals.get('file_list');
                                        if (flist && flist.length > 0) _fsids = flist.map(f => f.fs_id);
                                    }} catch (e) {{}}
                                }}

                                return JSON.stringify({{
                                    uk: (_uk ? _uk.toString() : ''),
                                    share_id: (_share_id ? _share_id.toString() : ''),
                                    bdstoken: _bdstoken || '',
                                    fsids: _fsids
                                }});
                            }} catch(err) {{
                                return '{{}}';
                            }}
                        }})();
                    ";

                    string resultJson = await webView.ExecuteScriptAsync(script);
                    resultJson = resultJson.Trim('"').Replace("\\\"", "\"").Replace("\\\\", "\\");
                    
                    var payload = JsonConvert.DeserializeObject<JObject>(resultJson);
                    
                    string uk = payload?.Value<string>("uk");
                    string share_id = payload?.Value<string>("share_id");
                    string bdstoken = payload?.Value<string>("bdstoken");
                    JArray fsids = payload?.Value<JArray>("fsids");

                    if (!string.IsNullOrEmpty(uk) && !string.IsNullOrEmpty(share_id) && fsids != null && fsids.Count > 0)
                    {
                        var cookies = await webView.CoreWebView2.CookieManager.GetCookiesAsync("https://pan.baidu.com");
                        var bduss = cookies.FirstOrDefault(c => c.Name == "BDUSS")?.Value;
                        var stoken = cookies.FirstOrDefault(c => c.Name == "STOKEN")?.Value;
                        var bdclnd = cookies.FirstOrDefault(c => c.Name == "BDCLND")?.Value;

                        return new ScrapeResult
                        {
                            Success = true,
                            Uk = uk,
                            ShareId = share_id,
                            Bdstoken = bdstoken,
                            RootFsids = fsids,
                            Bduss = bduss,
                            Stoken = stoken,
                            Bdclnd = bdclnd
                        };
                    }
                }
                catch { } // Ignore JSON parse errors on intermediate states
            }

            try
            {
                string htmlSnippet = await webView.ExecuteScriptAsync("document.documentElement.outerHTML");
                System.IO.File.WriteAllText(@"C:\tmp\error_scraper.html", JsonConvert.DeserializeObject<string>(htmlSnippet));
            }
            catch { }

            webView.CoreWebView2.Stop();
            return new ScrapeResult { Success = false, ErrorMessage = "未能在页面中找到网盘分享变量，可能被拦截或页面结构已更新" };
        }
    }
}
