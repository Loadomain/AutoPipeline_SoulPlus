using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace BaiduAutoDownloader
{
    public static class CliRunner
    {
        public static void Run(string jsonPath = "test.json", string targetDir = @"C:\tmp\test_download")
        {
            try
            {
                var startConfig = Configuration.Load();
                File.AppendAllText(@"C:\tmp\cli_log.txt", $"[CLI START] Reading {jsonPath} | API Mode: {startConfig.ApiMode}\n");
                string json = File.ReadAllText(jsonPath);
                var rawDict = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(json);
                var resources = new List<ResourceItem>();

                foreach (var kvp in rawDict)
                {
                    string urlsStr = kvp.Value.ContainsKey("地址") ? kvp.Value["地址"] : "";
                    string pwd = kvp.Value.ContainsKey("提取码") ? kvp.Value["提取码"] : "";
                    var urlArray = urlsStr.Split(new[] { ',', ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                    resources.Add(new ResourceItem { Title = kvp.Key, Urls = urlArray, Pwd = pwd });
                }

                File.AppendAllText(@"C:\tmp\cli_log.txt", $"Parsed {resources.Count} tasks.\n");

                var config = Configuration.Load();
                config.TargetDirectory = targetDir;
                
                if (!Directory.Exists(targetDir))
                    Directory.CreateDirectory(targetDir);

                // Run UI loop for WebView2 async ops
                Form dummyForm = new Form { Opacity = 0, ShowInTaskbar = false };
                dummyForm.Load += async (s, e) =>
                {
                    try
                    {
                        var scraper = new WebScraperForm();
                        scraper.Show(); // Opacity 0

                        var api = new BaiduApiService();
                        System.Net.Http.HttpClient downloadClient = new System.Net.Http.HttpClient();
                        downloadClient.DefaultRequestHeaders.Add("User-Agent", "pan.baidu.com");

                        foreach (var res in resources)
                        {
                            foreach (var url in res.Urls)
                            {
                                File.AppendAllText(@"C:\tmp\cli_log.txt", $"[SCRAPE] {url}\n");
                                var scrapeRes = await scraper.InitializeAndScrapeAsync(url, res.Pwd);

                                if (!scrapeRes.Success)
                                {
                                    File.AppendAllText(@"C:\tmp\cli_log.txt", $"[ERROR] Scrape failed: {scrapeRes.ErrorMessage}\n");
                                    continue;
                                }

                                File.AppendAllText(@"C:\tmp\cli_log.txt", $"[COOKIES] BDUSS={!string.IsNullOrEmpty(scrapeRes.Bduss)}, STOKEN={!string.IsNullOrEmpty(scrapeRes.Stoken)}, BDCLND={!string.IsNullOrEmpty(scrapeRes.Bdclnd)}\n");

                                if (scrapeRes.RootFsids == null || scrapeRes.RootFsids.Count == 0)
                                {
                                    File.AppendAllText(@"C:\tmp\cli_log.txt", $"[WXLIST_FALLBACK] RootFsids empty. Attempting api fallback.\n");
                                    try
                                    {
                                        var uri = new Uri(url);
                                        var shortUrl = uri.AbsolutePath.TrimStart('/').Split('/')[1]; // handles /s/xxxx
                                        var wxRes = await api.GetShareListAsync(shortUrl, res.Pwd, scrapeRes.Bduss, scrapeRes.Stoken, scrapeRes.Bdclnd);
                                        File.AppendAllText(@"C:\tmp\cli_log.txt", $"[WXLIST_RAW] {JsonConvert.SerializeObject(wxRes)}\n");
                                        if (wxRes != null && wxRes.data?.list != null)
                                        {
                                            scrapeRes.RootFsids = new Newtonsoft.Json.Linq.JArray();
                                            foreach (var item in wxRes.data.list) scrapeRes.RootFsids.Add(item.fs_id);
                                        }
                                    }
                                    catch (Exception wxEx)
                                    {
                                        File.AppendAllText(@"C:\tmp\cli_log.txt", $"[WXLIST_ERROR] {wxEx.Message}\n");
                                    }
                                }

                                File.AppendAllText(@"C:\tmp\cli_log.txt", $"[SUCCESS] Found {scrapeRes.RootFsids?.Count ?? 0} root files.\n");

                                if (scrapeRes.RootFsids == null || scrapeRes.RootFsids.Count == 0)
                                    continue;

                                string tempCloudPath = $"/BaiduAutoDownloaderTemp/CLITest_{Guid.NewGuid()}";
                                File.AppendAllText(@"C:\tmp\cli_log.txt", $"[TRANSFER] to {tempCloudPath}\n");

                                var fsidArray = new List<string>();
                                foreach(var fs in scrapeRes.RootFsids) fsidArray.Add(fs.ToString());

                                scrapeRes.Bdstoken = await api.GetBdsTokenAsync(scrapeRes.Bduss, scrapeRes.Stoken);
                                File.AppendAllText(@"C:\tmp\cli_log.txt", $"[BDSTOKEN_FORCE_FETCHED] {scrapeRes.Bdstoken}\n");

                                // Try create dir
                                bool created = await api.CreateCloudDirectoryAsync(tempCloudPath, scrapeRes.Bduss, scrapeRes.Stoken, scrapeRes.Bdstoken);
                                File.AppendAllText(@"C:\tmp\cli_log.txt", $"[DIR_CREATE] HTTP success: {created}\n");

                                var transferRes = await api.TransferShareToDriveAsync(
                                    scrapeRes.ShareId, scrapeRes.Uk, scrapeRes.Bdstoken,
                                    scrapeRes.Bduss, scrapeRes.Stoken, scrapeRes.Bdclnd,
                                    fsidArray.ToArray(), tempCloudPath);

                                File.AppendAllText(@"C:\tmp\cli_log.txt", $"[TRANSFER_RES] errno: {transferRes.errno}, taskid: {transferRes.taskid}\n");
                                
                                await Task.Delay(3000);

                                File.AppendAllText(@"C:\tmp\cli_log.txt", $"[LISTING_DRIVE] {tempCloudPath}\n");
                                try
                                {
                                    var listres = await api.ListFilesAsync(config.AccessToken, tempCloudPath);
                                    if (listres != null && (listres.errno == -6 || listres.errno == 111 || listres.errno == 4 || listres.errno == 31034))
                                    {
                                        throw new OpenApiAuthException(listres.errno, "Token invalid or rate limited");
                                    }

                                    File.AppendAllText(@"C:\tmp\cli_log.txt", $"[OPEN_API_ROOT] {JsonConvert.SerializeObject(listres)}\n");
                                    if (listres?.list != null && listres.list.Count > 0)
                                    {
                                        foreach (var f in listres.list)
                                        {
                                            var metas = await api.GetFileDlinksAsync(config.AccessToken, new[] { f.fs_id });
                                            if (metas != null && (metas.errno == -6 || metas.errno == 111 || metas.errno == 4 || metas.errno == 31034))
                                            {
                                                throw new OpenApiAuthException(metas.errno, "Token invalid or rate limited");
                                            }

                                            var dlink = metas?.list?[0]?.dlink;
                                            File.AppendAllText(@"C:\tmp\cli_log.txt", $"[FILE_FOUND] {f.server_filename} Dlink Status: {!string.IsNullOrEmpty(dlink)}\n");
                                            
                                            if (!string.IsNullOrEmpty(dlink))
                                            {
                                                using (var resp = await downloadClient.GetAsync(dlink + $"&access_token={config.AccessToken}", System.Net.Http.HttpCompletionOption.ResponseHeadersRead))
                                                {
                                                    File.AppendAllText(@"C:\tmp\cli_log.txt", $"[DOWNLOAD START] Length: {resp.Content.Headers.ContentLength}\n");
                                                    var ds = await resp.Content.ReadAsStreamAsync();
                                                    
                                                    string relativePath = f.path.Substring(tempCloudPath.Length).TrimStart('/');
                                                    string cleanTitle = string.Join("_", res.Title.Split(Path.GetInvalidFileNameChars()));
                                                    string localTargetDir = Path.Combine(config.TargetDirectory, cleanTitle);
                                                    string localFilePath = Path.Combine(localTargetDir, relativePath);
                                                    string localFileDir = Path.GetDirectoryName(localFilePath);
                                                    
                                                    if (!Directory.Exists(localFileDir)) Directory.CreateDirectory(localFileDir);
                                                    
                                                    using (var fileStream = new FileStream(localFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                                                    {
                                                        var buf = new byte[8192];
                                                        int bytesRead;
                                                        long totalRead = 0;
                                                        
                                                        var sw = System.Diagnostics.Stopwatch.StartNew();
                                                        long lastRead = 0;
                                                        long lastReportTime = sw.ElapsedMilliseconds;
                                                        Console.WriteLine($"[DL_PROG]|{f.server_filename}|Queue|0 MB/s|0");

                                                        while ((bytesRead = await ds.ReadAsync(buf, 0, buf.Length)) > 0)
                                                        {
                                                            await fileStream.WriteAsync(buf, 0, bytesRead);
                                                            totalRead += bytesRead;

                                                            long now = sw.ElapsedMilliseconds;
                                                            if (now - lastReportTime > 500)
                                                            {
                                                                double diffSec = (now - lastReportTime) / 1000.0;
                                                                double speedMB = diffSec > 0 ? ((totalRead - lastRead) / 1048576.0) / diffSec : 0;
                                                                double progress = f.size > 0 ? (totalRead * 100.0 / f.size) : 0;
                                                                
                                                                Console.WriteLine($"[DL_PROG]|{f.server_filename}|Downloading|{speedMB:F2} MB/s|{progress:F1}");
                                                                
                                                                lastRead = totalRead;
                                                                lastReportTime = now;
                                                            }
                                                        }
                                                        Console.WriteLine($"[DL_PROG]|{f.server_filename}|Finished|0 MB/s|100");
                                                        File.AppendAllText(@"C:\tmp\cli_log.txt", $"[DOWNLOAD SUCCESS] Fully read {totalRead} bytes into {localFilePath}.\n");
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        File.AppendAllText(@"C:\tmp\cli_log.txt", $"[LIST_ERROR] Could not list transferred files. Is Open API token valid?\n");
                                    }
                                }
                                catch (OpenApiAuthException ex)
                                {
                                    File.AppendAllText(@"C:\tmp\cli_log.txt", $"[FATAL] 限流或授权失效 (Err:{ex.Errno})。由于在CLI无头模式中，无法自动轮换和拉起二维码。请打开GUI模式重新授权新通道。\n");
                                    Console.WriteLine($"FATAL: API Rate Limited (Err: {ex.Errno}). Please run GUI to rotate AppKey and re-authenticate!");
                                }

                                await api.DeleteFileAsync(config.AccessToken, tempCloudPath);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        File.AppendAllText(@"C:\tmp\cli_log.txt", $"[FATAL] {ex}\n");
                    }
                    finally
                    {
                        Application.Exit();
                    }
                };
                
                Application.Run(dummyForm);
            }
            catch (Exception ex)
            {
                File.AppendAllText(@"C:\tmp\cli_log.txt", $"Startup error: {ex.Message}\n");
            }
        }
    }
}
