using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace SouthPlusDownloader
{
    public class PostAnalyzer
    {
        private readonly TaskConfig _config;
        private readonly HttpClient _httpClient;
        private readonly Action<string> _logger;
        private readonly object _fileLock = new object();
        private readonly string _outputFile = "southplus_results.json";

        public PostAnalyzer(TaskConfig config, HttpClient httpClient, Action<string> logger)
        {
            _config = config;
            _httpClient = httpClient;
            _logger = logger;
        }

        public class ResourceInfo
        {
            [JsonPropertyName("地址")]
            public string Address { get; set; }
            [JsonPropertyName("提取码")]
            public string ExtractionCode { get; set; }
            [JsonPropertyName("解压密码")]
            public string DecompressionPassword { get; set; }
            [JsonPropertyName("价格")]
            public string Price { get; set; }
            [JsonPropertyName("源网页")]
            public string SourceUrl { get; set; }
            [JsonPropertyName("是否下载了")]
            public string IsDownloaded { get; set; }
        }

        public async Task ProcessPostAsync(string threadUrl, string title, CancellationToken token)
        {
            try
            {
                _logger($"正在分析帖子: {title} ({threadUrl})");
                
                var request1 = new HttpRequestMessage(HttpMethod.Get, threadUrl);
                if (!string.IsNullOrEmpty(_config.Cookie)) request1.Headers.Add("Cookie", _config.Cookie);
                var response1 = await _httpClient.SendAsync(request1, token);
                response1.EnsureSuccessStatusCode();
                var html = await response1.Content.ReadAsStringAsync();
                
                // 保存调试文件
                File.WriteAllText("debug_post_html.txt", html);

                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                // 1. 获取主楼内容 (兼容带空格的 class='tpc_content ')
                var mainPostNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'tpc_content')]");
                if (mainPostNode == null)
                {
                    _logger("  未找到正文内容，已跳过。");
                    return;
                }
                
                string contentText = mainPostNode.InnerText;

                // 2. 检查全局 HTML 是否包含匹配平台 (应对标签嵌套错误或隐藏标签等情况)
                bool platformMatched = false;
                if (_config.Platforms == null || _config.Platforms.Length == 0)
                {
                    platformMatched = true; // 数组空白则为盲购模式
                }
                else
                {
                    foreach (var platform in _config.Platforms)
                    {
                        if (string.IsNullOrWhiteSpace(platform)) continue;
                        if (html.Contains(platform, StringComparison.OrdinalIgnoreCase))
                        {
                            platformMatched = true;
                            break;
                        }
                    }
                }

                if (!platformMatched)
                {
                    _logger($"  未发现指定盘符关键字，已跳过。");
                    return;
                }

                string itemPriceText = "免费/已购";
                string extractedLinks = "未获取";
                string extractedCode = "未获取";
                string extractedPassword = "未获取";
                string buyResult = "无需购买";

                // 3. 寻找隐藏的购买元素 (判断是否需要购买)
                // 兼容 1: <a href="job.php?action=buy...">
                // 兼容 2: <input onclick="location.href='job.php?action=buy...' ">
                var buyLinkNode = doc.DocumentNode.SelectSingleNode("//a[contains(@href, 'job.php?action=buy')] | //input[contains(@onclick, 'job.php?action=buy')]");
                
                if (buyLinkNode != null)
                {
                    // 需要购买
                    string buyUrlPath = "";
                    if (buyLinkNode.Name == "a")
                    {
                        buyUrlPath = buyLinkNode.GetAttributeValue("href", "");
                    }
                    else if (buyLinkNode.Name == "input")
                    {
                        var onclick = buyLinkNode.GetAttributeValue("onclick", "");
                        var matchUrl = Regex.Match(onclick, @"location\.href='([^']+)'");
                        if (matchUrl.Success) buyUrlPath = matchUrl.Groups[1].Value;
                    }

                    // 尝试匹配价格，往上找 span 或 div 包含 "售价 xxx SP币"
                    var parentContent = buyLinkNode.ParentNode?.ParentNode?.InnerText ?? buyLinkNode.ParentNode?.InnerText ?? "";
                    var match = Regex.Match(parentContent, @"售价\s*(\d+)\s*(SP币|G|金币)");
                    int price = 0;
                    if (match.Success)
                    {
                        price = int.Parse(match.Groups[1].Value);
                        itemPriceText = $"{price} {match.Groups[2].Value}";
                    }
                    else
                    {
                        var numMatch = Regex.Match(parentContent, @"(\d+)\s*SP币");
                        if (numMatch.Success) {
                            price = int.Parse(numMatch.Groups[1].Value);
                            itemPriceText = $"{price} SP币";
                        }
                    }

                    _logger($"  需要购买，价格探测为: {price} (内容:{itemPriceText})。 上限: {_config.MaxPrice}");

                    if (price <= _config.MaxPrice)
                    {
                        _logger($"  价格符合条件 ({price} <= {_config.MaxPrice})，提交购买请求...");
                        try
                        {
                            var buyUrl = "https://south-plus.net/" + buyUrlPath.Replace("&amp;", "&");
                            var buyReq = new HttpRequestMessage(HttpMethod.Get, buyUrl);
                            if (!string.IsNullOrEmpty(_config.Cookie)) buyReq.Headers.Add("Cookie", _config.Cookie);
                            // 必须加上 Referer 防止被拦截
                            buyReq.Headers.Add("Referer", threadUrl);
                            
                            var buyResponseInfo = await _httpClient.SendAsync(buyReq, token);
                            
                            // 延迟后重新抓取页面
                            await Task.Delay(1000, token);
                            
                            var request2 = new HttpRequestMessage(HttpMethod.Get, threadUrl);
                            if (!string.IsNullOrEmpty(_config.Cookie)) request2.Headers.Add("Cookie", _config.Cookie);
                            var response2 = await _httpClient.SendAsync(request2, token);
                            response2.EnsureSuccessStatusCode();
                            html = await response2.Content.ReadAsStringAsync();
                            doc.LoadHtml(html);
                            mainPostNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'tpc_content')]");
                            contentText = mainPostNode?.InnerText ?? html; // 保底用 html
                            buyResult = $"购买成功({itemPriceText})";
                        }
                        catch (Exception ex)
                        {
                            _logger($"  购买失败! 错误: {ex.Message}");
                            buyResult = "自动购买失败";
                        }
                    }
                    else
                    {
                        _logger($"  超过设定的最大价格 ({_config.MaxPrice})，已跳过购买。");
                        buyResult = "放弃(价格过高)";
                    }
                }

                // 4. 从 contentText 和 HTML 中提取链接和密码 (因为部分被隐藏的 DOM 可能提取不了 Text)
                // 正则捕获提取码和网盘链接
                var linkMatches = Regex.Matches(contentText + html, @"(https?://[^\s<""'<>]*(?:pan\.baidu\.com|quark\.cn|aliyundrive\.com|mega\.nz|drive\.google\.com|115\.com|lanzou|weiyun)[^\s<""'<>]*)", RegexOptions.IgnoreCase);
                var magnetMatches = Regex.Matches(contentText + html, @"magnet:\?xt=urn:btih:[A-Za-z0-9]+", RegexOptions.IgnoreCase);

                if (linkMatches.Count > 0)
                {
                    extractedLinks = string.Join(", ", linkMatches.Cast<Match>().Select(m => m.Value).Distinct());
                    // 尝试从链接里截取 pwd=xxx 作为提取码
                    foreach (Match m in linkMatches)
                    {
                        var pwdMatch = Regex.Match(m.Value, @"[?&]pwd=([a-zA-Z0-9]+)", RegexOptions.IgnoreCase);
                        if (pwdMatch.Success)
                        {
                            extractedCode = pwdMatch.Groups[1].Value;
                            break;
                        }
                    }
                }
                else if (magnetMatches.Count > 0)
                {
                    extractedLinks = string.Join(", ", magnetMatches.Cast<Match>().Select(m => m.Value).Distinct());
                }
                
                // 如果链接里没截到，从文本里提取“提取码/密码”
                if (extractedCode == "未获取")
                {
                    string codePrefix = string.IsNullOrWhiteSpace(_config.CustomCodeKeywords) ? "" : "|" + string.Join("|", _config.CustomCodeKeywords.Split(new[] { ',', '，' }, StringSplitOptions.RemoveEmptyEntries).Select(k => Regex.Escape(k.Trim())));
                    var codeMatch = Regex.Match(contentText, @"(?<!解压)(?:提取码|密码" + codePrefix + @")\s*[:：]?\s*([a-zA-Z0-9_\-]+)", RegexOptions.IgnoreCase);
                    if (codeMatch.Success)
                    {
                        extractedCode = codeMatch.Groups[1].Value;
                    }
                }

                // 解压密码: 匹配 pwd =, pwd=, pwd, 解压密码, 解压密码:, 解压密码 :
                string pwdPrefix = string.IsNullOrWhiteSpace(_config.CustomPasswordKeywords) ? "" : "|" + string.Join("|", _config.CustomPasswordKeywords.Split(new[] { ',', '，' }, StringSplitOptions.RemoveEmptyEntries).Select(k => Regex.Escape(k.Trim())));
                var passMatch = Regex.Match(contentText, @"(?<![?&])(?:pwd|解压密码|解压码" + pwdPrefix + @")\s*[:：=]?\s*([a-zA-Z0-9_\-\.\u4e00-\u9fa5]+)", RegexOptions.IgnoreCase);

                if (passMatch.Success)
                {
                    extractedPassword = passMatch.Groups[1].Value;
                }
                else
                {
                    if (html.Contains("4004")) extractedPassword = "4004"; // 特供应对 4004 独立一行
                }

                if (buyResult.Contains("购买成功") || buyResult == "无需购买") 
                {
                    if (extractedLinks == "未获取" && buyResult != "无需购买")
                    {
                        extractedLinks = "可能隐藏或非标准格式网盘链接，请人工检查原贴";
                    }
                }

                _logger($"  --> 数据: 状态:[{buyResult}] 提取链接:[{extractedLinks}] 提取码:[{extractedCode}] 解压密码:[{extractedPassword}]");
                
                if (extractedLinks != "未获取" || buyResult.Contains("成功") || buyResult == "无需购买")
                {
                    SaveResult(title, buyResult, extractedLinks, extractedCode, extractedPassword, itemPriceText, threadUrl);
                }
            }
            catch (Exception ex)
            {
                _logger($"  解析帖子时出错: {ex.Message}");
            }
        }

        private void SaveResult(string title, string buyResult, string links, string code, string password, string price, string url)
        {
            lock (_fileLock)
            {
                Dictionary<string, ResourceInfo> results;
                if (File.Exists(_outputFile))
                {
                    try
                    {
                        string json = File.ReadAllText(_outputFile);
                        results = JsonSerializer.Deserialize<Dictionary<string, ResourceInfo>>(json) ?? new Dictionary<string, ResourceInfo>();
                    }
                    catch
                    {
                        results = new Dictionary<string, ResourceInfo>();
                    }
                }
                else
                {
                    results = new Dictionary<string, ResourceInfo>();
                }
                
                // 将原始 title 简化，去除可能引起格式问题的非法字符
                string cleanTitle = title.Replace("\r", "").Replace("\n", "").Trim();

                results[cleanTitle] = new ResourceInfo
                {
                    Address = links == "未获取" || links.StartsWith("可能隐藏") ? links : links,
                    ExtractionCode = code == "未获取" ? "" : code,
                    DecompressionPassword = password == "未获取" ? "" : password,
                    Price = price,
                    SourceUrl = url,
                    IsDownloaded = "否"
                };

                var options = new JsonSerializerOptions 
                { 
                    WriteIndented = true, 
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping 
                };
                File.WriteAllText(_outputFile, JsonSerializer.Serialize(results, options));
            }
        }
    }
}
