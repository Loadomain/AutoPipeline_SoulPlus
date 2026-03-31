using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SouthPlusDownloader
{
    public class BoardManager
    {
        private readonly HttpClient _httpClient;
        private readonly TaskConfig _config;
        private readonly Action<string> _logger;
        public readonly string BoardsFile = "boards.json";

        public BoardManager(TaskConfig config, HttpClient httpClient, Action<string> logger)
        {
            _config = config;
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<Dictionary<string, int>> FetchLatestBoardsAsync(CancellationToken token)
        {
            _logger("开始同步南+全站 Cxxx 板块列表，这可能需要 1~2 分钟，请耐心等待...");
            var results = new Dictionary<string, int>();
            
            // 加入默认的一些固定板块
            results["自定义"] = 0;
            results["茶馆"] = 9;
            results["实用动画"] = 4;
            results["实用漫画"] = 5;
            results["同人语音"] = 128;
            results["CG资源"] = 14;
            results["同人志"] = 36;
            results["单行本"] = 37;
            results["3D综合"] = 127;

            try
            {
                var req = new HttpRequestMessage(HttpMethod.Get, "https://south-plus.net/index.php");
                if (!string.IsNullOrEmpty(_config.Cookie)) req.Headers.Add("Cookie", _config.Cookie);
                
                var res = await _httpClient.SendAsync(req, token);
                res.EnsureSuccessStatusCode();
                var html = await res.Content.ReadAsStringAsync();

                // 找主页里所有的 Comic Market / Cxxx 大分类
                var matches = Regex.Matches(html, @"<a[^>]*href=""thread\.php\?fid-(\d+)\.html""[^>]*>\s*(C(?:omic(?: Market)?)?\s*\d+[^<]*)\s*</a>", RegexOptions.IgnoreCase);
                
                var categories = new List<(int Fid, string Name)>();
                foreach (Match match in matches)
                {
                    if (int.TryParse(match.Groups[1].Value, out int fid))
                    {
                        // 统一名称规范为 "C104", "C103" 等
                        string rawName = match.Groups[2].Value.Trim();
                        var numMatch = Regex.Match(rawName, @"\d+");
                        string cleanName = numMatch.Success ? $"C{numMatch.Value} 同人志" : $"{rawName} 同人志";
                        categories.Add((fid, cleanName));
                    }
                }

                _logger($"已从首页识别到 {categories.Count} 个主漫展分类，正在逐个深入探测真正发贴子版块...");

                // 不要并发太激进，南+有 5 秒盾和频率限制，我们采用平缓的顺序爬取
                foreach (var cat in categories)
                {
                    token.ThrowIfCancellationRequested();
                    
                    try
                    {
                        var catReq = new HttpRequestMessage(HttpMethod.Get, $"https://south-plus.net/thread.php?fid={cat.Fid}");
                        if (!string.IsNullOrEmpty(_config.Cookie)) catReq.Headers.Add("Cookie", _config.Cookie);
                        
                        var catRes = await _httpClient.SendAsync(catReq, token);
                        catRes.EnsureSuccessStatusCode();
                        var catHtml = await catRes.Content.ReadAsStringAsync();

                        // 试图寻找 同人志&CG 的子版块 (必须包含 fnamecolor 防止匹配到网站全局导航栏如16号版块)
                        var links = Regex.Matches(catHtml, @"<a[^>]*href=""thread\.php\?fid-(\d+)\.html""[^>]*>(.*?)</a>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                        
                        int targetSubFid = 0;
                        foreach (Match sm in links)
                        {
                            string fullTag = sm.Value;
                            if (!fullTag.Contains("fnamecolor")) continue; // 只看真正的子版块名

                            string subName = sm.Groups[2].Value;
                            if (Regex.IsMatch(subName, @"同人志|[a-zA-Z&]*CG|同人", RegexOptions.IgnoreCase))
                            {
                                if (!subName.Contains("图墙") && !subName.Contains("音声"))
                                {
                                    targetSubFid = int.Parse(sm.Groups[1].Value);
                                    break;
                                }
                            }
                        }

                        if (targetSubFid > 0)
                        {
                            results[cat.Name] = targetSubFid;
                            _logger($"[成功] {cat.Name} -> {targetSubFid}");
                        }
                        else
                        {
                            // 如果他没有子版块，或者自己就是一个直接发帖的版块呢？
                            // 检查有没有帖子 read.php?tid-xxx
                            if (Regex.IsMatch(catHtml, @"read\.php\?tid-"))
                            {
                                results[cat.Name] = cat.Fid;
                                _logger($"[直接版块] {cat.Name} -> {cat.Fid}");
                            }
                            else
                            {
                                _logger($"[跳过] {cat.Name} -> 无法解析有效的发贴版块。");
                            }
                        }
                        
                        // 强制延迟防封
                        await Task.Delay(500, token);
                    }
                    catch (Exception ex)
                    {
                        _logger($"探测 {cat.Name} 时出错: {ex.Message}");
                    }
                }

                _logger("分类同步完毕，正在保存至本地 json...");
                var options = new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
                File.WriteAllText(BoardsFile, JsonSerializer.Serialize(results, options));
                _logger("同步成功！您可以直接在下拉选项中使用了！");
                return results;
            }
            catch (Exception ex)
            {
                _logger($"同步大分类时彻底失败: {ex.Message}");
                return results; // 返回部分获取的数据
            }
        }

        public Dictionary<string, int> LoadLocalBoards()
        {
            if (File.Exists(BoardsFile))
            {
                try
                {
                    var json = File.ReadAllText(BoardsFile);
                    var dict = JsonSerializer.Deserialize<Dictionary<string, int>>(json);
                    if (dict != null && dict.Count > 0)
                    {
                        return dict;
                    }
                }
                catch { }
            }
            return new Dictionary<string, int>(); // 找不到则返回空
        }
    }
}
