using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace SouthPlusDownloader
{
    public class CrawlerService
    {
        private readonly TaskConfig _config;
        private readonly Action<string> _logger;
        private readonly HttpClientHandler _handler;
        private readonly HttpClient _httpClient;
        private readonly PostAnalyzer _analyzer;

        public CrawlerService(TaskConfig config, Action<string> logger)
        {
            _config = config;
            _logger = logger;

            _handler = new HttpClientHandler
            {
                UseCookies = false,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            _httpClient = new HttpClient(_handler);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", _config.UserAgent);
            _httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8");
            _httpClient.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9,en;q=0.8");
            
            _analyzer = new PostAnalyzer(_config, _httpClient, _logger);
        }

        public async Task StartAsync(CancellationToken token)
        {
            _logger($"开始抓取板块 ID: {_config.BoardId}, 从第 {_config.StartItem} 条到第 {_config.EndItem} 条。");

            // 南+默认每页帖子数量约 35 (剔除置顶贴后可能在 25-35 之间)
            // 为了准确度，我们干脆一页一页遍历，自己计数。
            int currentItemIndex = 1; // 绝对计数器，用于对比 Config 的 StartItem 和 EndItem
            int currentPage = 1;
            
            // 为了快速跳过前面不需要的页
            // 如果 startItem 很大，我们粗略计算起始页
            if (_config.StartItem > 35)
            {
                currentPage = ((_config.StartItem - 1) / 35) + 1;
                // 重置当前索引，跳过的页保守按每页30个非置顶计算（这只是个估算！）
                currentItemIndex = (currentPage - 1) * 35 + 1;
                _logger($"计算起始页为 {currentPage} (估算起点索引: {currentItemIndex})");
            }

            bool completed = false;
            Random rand = new Random();

            while (!completed)
            {
                token.ThrowIfCancellationRequested();

                // 恢复为正规动态 URL
                string pageUrl = $"https://south-plus.net/thread.php?fid={_config.BoardId}&page={currentPage}";
                _logger($"正在获取列表页: {pageUrl}");

                string html = "";
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, pageUrl);
                    if (!string.IsNullOrEmpty(_config.Cookie))
                    {
                        request.Headers.Add("Cookie", _config.Cookie);
                    }
                    var response = await _httpClient.SendAsync(request, token);
                    response.EnsureSuccessStatusCode();
                    html = await response.Content.ReadAsStringAsync();
                    System.IO.File.WriteAllText("debug_html.txt", html); // 保存日志文件以便排查
                }
                catch (Exception ex)
                {
                    _logger($"获取列表页失败: {ex.Message}");
                    break;
                }

                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                // 获取帖子列表 <tr class="tr3 t_one">
                var rows = doc.DocumentNode.SelectNodes("//tr[@class='tr3 t_one']");
                if (rows == null || rows.Count == 0)
                {
                    _logger("列表页没有帖子，抓取结束或 Cookie 失效(遇到认证板块提问)。");
                    if (html.Contains("没有权限查看") || html.Contains("认证版块") || html.Contains("只有注册会员"))
                    {
                        _logger("【严重错误】您的 Cookie 已经失效或未填写正确！请确保：");
                        _logger("1. Cookie 为最新。");
                        _logger("2. 您的浏览器 User-Agent 必须和爬虫一致，南+的会话强绑定浏览器UA！");
                    }
                    else if (html.Contains("cf-browser-verification") || html.ToLower().Contains("cloudflare"))
                    {
                        _logger("【严重错误】被 Cloudflare 盾拦截！请在浏览器无痕模式通过验证后重新提取 Cookie 和 User-Agent。");
                    }
                    break;
                }

                int processedInPage = 0;

                foreach (var row in rows)
                {
                    token.ThrowIfCancellationRequested();

                    // 南+ 的帖子标题链接格式通常是 <a href="read.php?tid-..." id="a_ajax_...">标题</a>
                    var aTag = row.SelectSingleNode(".//a[starts-with(@id, 'a_ajax_')]");
                    if (aTag == null) continue;

                    string title = aTag.InnerText.Trim();
                    string tidHref = aTag.GetAttributeValue("href", "");
                    
                    if (string.IsNullOrEmpty(tidHref)) continue;

                    // 判断是否达到起始条目
                    if (currentItemIndex < _config.StartItem)
                    {
                        currentItemIndex++;
                        continue;
                    }

                    // 判断是否超过结束条目
                    if (currentItemIndex > _config.EndItem)
                    {
                        _logger($"已达到结束条目设定 ({_config.EndItem})，停止抓取。");
                        completed = true;
                        break;
                    }

                    string threadUrl = $"https://south-plus.net/{tidHref}";
                    
                    // 进行处理
                    await _analyzer.ProcessPostAsync(threadUrl, title, token);
                    
                    processedInPage++;
                    currentItemIndex++;

                    // 延迟防封禁
                    int delay = rand.Next(_config.MinDelayMs, _config.MaxDelayMs);
                    await Task.Delay(delay, token);
                }

                if (processedInPage == 0) 
                {
                    if (currentItemIndex > _config.StartItem || currentPage > 30)
                    {
                        _logger("当前页面没有有效帖子，或者连续翻页到达安全上限，抓取结束。");
                        break;
                    }
                }

                currentPage++;
            }
        }
    }
}
