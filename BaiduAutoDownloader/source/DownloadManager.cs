using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace BaiduAutoDownloader
{
    public class DownloadManager
    {
        private BaiduApiService _api;
        private WebScraperForm _scraper;
        private HttpClient _downloadClient;

        public DownloadManager()
        {
            _api = new BaiduApiService();
            _downloadClient = new HttpClient();
            _downloadClient.DefaultRequestHeaders.Add("User-Agent", "pan.baidu.com");
            _downloadClient.Timeout = TimeSpan.FromHours(12); // Long timeout for large files
        }

        public async void StartDownloads(List<ResourceItem> resources, Configuration config, MainForm mainForm, ListView lvTasks)
        {
            try
            {
                if (_scraper == null || _scraper.IsDisposed)
                {
                    _scraper = new WebScraperForm();
                    _scraper.Show(); // Keep it hidden via opacity
                }

                foreach (ListViewItem item in lvTasks.Items)
                {
                    var resource = item.Tag as ResourceItem;
                    if (resource == null) continue;

                    mainForm.ReportProgress(item, "拉取分享信息...", "", "");
                    
                    bool success = false;
                    bool needsRetry = false;
                    
                    do 
                    {
                        needsRetry = false;
                        try
                        {
                            success = await ProcessResourceAsync(resource, config, mainForm, item);
                        }
                        catch (OpenApiAuthException ex)
                        {
                            mainForm.ReportProgress(item, $"API限流/授权失效(Err:{ex.Errno})", "-", "-");
                            
                            string msg = $"下载接口被限流或授权失效（错误码：{ex.Errno}）。\n\n是否立即尝试切换 API 通道重新授权并继续下载？";
                            
                            if (mainForm.InvokeRequired)
                            {
                                var result = (DialogResult)mainForm.Invoke(new Func<DialogResult>(() => 
                                    MessageBox.Show(mainForm, msg, "API 通道受限", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                                ));
                                
                                if (result == DialogResult.Yes)
                                {
                                    config.RotateToNextPublicKey();
                                    var authResult = (DialogResult)mainForm.Invoke(new Func<DialogResult>(() => 
                                    {
                                        using (var authForm = new AuthForm(config))
                                        {
                                            var res = authForm.ShowDialog();
                                            if (res == DialogResult.OK) 
                                                config.AccessToken = authForm.AccessToken;
                                            return res;
                                        }
                                    }));
                                    
                                    if (authResult == DialogResult.OK)
                                    {
                                        needsRetry = true; // Loop back and try this file again!
                                    }
                                }
                            }
                        }
                    } while (needsRetry);

                    if (success)
                    {
                        mainForm.ReportProgress(item, "已完成", "-", "100%");
                    }
                    else
                    {
                        mainForm.ReportProgress(item, "失败，请看日志", "-", "Error");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("发生严重异常: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _scraper?.Close();
                mainForm.TaskCompleted();
            }
        }

        private async Task<bool> ProcessResourceAsync(ResourceItem resource, Configuration config, MainForm mainForm, ListViewItem uiItem)
        {
            string cleanTitle = string.Join("_", resource.Title.Split(Path.GetInvalidFileNameChars()));
            string tempCloudPath = $"/BaiduAutoDownloaderTemp/{cleanTitle}";
            string localTargetDir = Path.Combine(config.TargetDirectory, cleanTitle);

            // Make sure local directory exists
            if (!Directory.Exists(localTargetDir))
                Directory.CreateDirectory(localTargetDir);

            foreach (var url in resource.Urls)
            {
                try
                {
                    mainForm.ReportProgress(uiItem, $"解析链接中...", "", "10%");
                    var result = await _scraper.InitializeAndScrapeAsync(url, resource.Pwd);

                    if (!result.Success)
                    {
                        MessageBox.Show($"解析失败: {result.ErrorMessage}\n链接: {url}", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        continue;
                    }

                    if (result.RootFsids == null || result.RootFsids.Count == 0)
                    {
                        mainForm.ReportProgress(uiItem, $"拉取文件列表...", "", "20%");
                        try
                        {
                            var uri = new Uri(url);
                            var shortUrl = uri.AbsolutePath.TrimStart('/').Split('/').Last();
                            var wxRes = await _api.GetShareListAsync(shortUrl, resource.Pwd, result.Bduss, result.Stoken, result.Bdclnd);
                            if (wxRes != null && wxRes.data?.list != null && wxRes.data.list.Count > 0)
                            {
                                result.RootFsids = new JArray();
                                foreach (var fitem in wxRes.data.list) result.RootFsids.Add(fitem.fs_id);
                            }
                        }
                        catch (Exception wxEx) { }
                    }

                    if (result.RootFsids == null || result.RootFsids.Count == 0)
                    {
                        MessageBox.Show($"获取文件列表失败，分享链接可能已失效：\n{url}", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        continue;
                    }

                    mainForm.ReportProgress(uiItem, $"正在转存到网盘...", "", "30%");
                    
                    // Always fetch a fresh BDSTOKEN for the home directory. The share page's ones are invalid for Transfer.
                    result.Bdstoken = await _api.GetBdsTokenAsync(result.Bduss, result.Stoken);
                    
                    // Crucial: Create the target directory first. Transfer API will fail with errno 2 if it doesn't exist.
                    await _api.CreateCloudDirectoryAsync(tempCloudPath, result.Bduss, result.Stoken, result.Bdstoken);

                    var fsidArray = result.RootFsids.Select(t => t.ToString()).ToArray();
                    var transferRes = await _api.TransferShareToDriveAsync(
                        result.ShareId, result.Uk, result.Bdstoken,
                        result.Bduss, result.Stoken, result.Bdclnd,
                        fsidArray, tempCloudPath);

                    if (transferRes.errno != 0)
                    {
                        // 12 is usually path exists, 120 is folder tree too large (async task)
                        if (transferRes.errno != 12 && transferRes.errno != 120)
                        {
                            MessageBox.Show($"转存失败，错误码 {transferRes.errno}");
                            continue;
                        }
                    }

                    // Await potentially async transfer if taskid exists
                    if (transferRes.taskid > 0 || transferRes.errno == 120)
                    {
                        mainForm.ReportProgress(uiItem, "等待异步大文件转存...", "", "35%");
                        await Task.Delay(5000); 
                    }

                    mainForm.ReportProgress(uiItem, $"正在获取文件列表...", "", "40%");
                    
                    // Recursive list download files
                    var downloadQueue = new List<BasicFileMeta>();
                    await TraverseCloudDirectoryAsync(config.AccessToken, tempCloudPath, downloadQueue);

                    if (downloadQueue.Count == 0)
                    {
                        continue; // No files found somehow, proceed to next URL
                    }

                    mainForm.ReportProgress(uiItem, $"开始下载 {downloadQueue.Count} 个文件...", "", "50%");
                    
                    long totalBytes = downloadQueue.Sum(f => f.size);
                    long downloadedBytesTotal = 0;

                    foreach (var f in downloadQueue)
                    {
                        // Request dlink
                        var metas = await _api.GetFileDlinksAsync(config.AccessToken, new[] { f.fs_id });
                        if (metas != null && (metas.errno == -6 || metas.errno == 111 || metas.errno == 4 || metas.errno == 31034))
                        {
                            throw new OpenApiAuthException(metas.errno, "Open API GetDlink failed with Auth/Limit error.");
                        }

                        var metaDetails = metas?.list?.FirstOrDefault();
                        if (metaDetails == null || string.IsNullOrEmpty(metaDetails.dlink))
                        {
                            continue; // Failed to get dlink
                        }

                        // Adjust path for local reconstruction
                        string relativePath = f.path.Substring(tempCloudPath.Length).TrimStart('/');
                        string localFilePath = Path.Combine(localTargetDir, relativePath);
                        string localFileDir = Path.GetDirectoryName(localFilePath);

                        if (!Directory.Exists(localFileDir))
                            Directory.CreateDirectory(localFileDir);

                        string dlinkAuth = metaDetails.dlink + $"&access_token={config.AccessToken}";
                        int maxRetries = 5;
                        bool fileCompleted = false;

                        for (int attempt = 1; attempt <= maxRetries && !fileCompleted; attempt++)
                        {
                            try
                            {
                                long existingLength = 0;
                                if (File.Exists(localFilePath))
                                {
                                    existingLength = new FileInfo(localFilePath).Length;
                                }

                                if (existingLength >= metaDetails.size && metaDetails.size > 0)
                                {
                                    // Already fully downloaded
                                    downloadedBytesTotal += metaDetails.size;
                                    fileCompleted = true;
                                    break;
                                }

                                var request = new HttpRequestMessage(HttpMethod.Get, dlinkAuth);
                                if (existingLength > 0)
                                {
                                    request.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(existingLength, null);
                                }

                                using (var response = await _downloadClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                                {
                                    response.EnsureSuccessStatusCode();

                                    using (var contentStream = await response.Content.ReadAsStreamAsync())
                                    using (var fileStream = new FileStream(localFilePath, FileMode.Append, FileAccess.Write, FileShare.None, 8192, true))
                                    {
                                        var buffer = new byte[8192];
                                        int bytesRead;
                                        long currentFileRead = existingLength;
                                        var startTime = DateTime.Now;
                                        var lastReportTime = DateTime.MinValue;

                                        while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                                        {
                                            await fileStream.WriteAsync(buffer, 0, bytesRead);
                                            currentFileRead += bytesRead;
                                            
                                            // Ensure we don't double count during retries
                                            downloadedBytesTotal += bytesRead;

                                            if ((DateTime.Now - lastReportTime).TotalMilliseconds > 500)
                                            {
                                                lastReportTime = DateTime.Now;
                                                // calculate speed based on bytes downloaded this session vs time
                                                double speedInMB = ((currentFileRead - existingLength) / 1024.0 / 1024.0) / Math.Max(0.1, (DateTime.Now - startTime).TotalSeconds);
                                                double progressPct = (double)downloadedBytesTotal / totalBytes * 100.0;
                                                
                                                mainForm.ReportProgress(uiItem, 
                                                    $"下载: {metaDetails.server_filename}",
                                                    $"{speedInMB:F2} MB/s", 
                                                    $"{progressPct:F1}%"
                                                );
                                            }
                                        }
                                        
                                        // Reached EOF normally
                                        fileCompleted = true;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                if (attempt == maxRetries)
                                {
                                    throw new Exception($"文件 {metaDetails.server_filename} 下载失败 (重试 {maxRetries} 次后仍报错): {ex.Message}");
                                }
                                mainForm.ReportProgress(uiItem, $"连接断开，即将重试({attempt}/{maxRetries})...", "-", "-");
                                await Task.Delay(2000); // Wait before retrying
                            }
                        }
                    }

                    // Done with this URL, cleanup cloud directory
                    mainForm.ReportProgress(uiItem, $"清理云端临时文件...", "-", "99%");
                    await _api.DeleteFileAsync(config.AccessToken, tempCloudPath);

                    return true; // Successfully processed
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"处理链接 {url} 发生异常: " + ex.Message);
                }
            }

            return false;
        }

        private async Task TraverseCloudDirectoryAsync(string accessToken, string currentDirPath, List<BasicFileMeta> files)
        {
            var res = await _api.ListFilesAsync(accessToken, currentDirPath);
            if (res == null || res.list == null || res.errno != 0)
            {
                if (res != null && (res.errno == -6 || res.errno == 111 || res.errno == 4 || res.errno == 31034))
                {
                    throw new OpenApiAuthException(res.errno, "Open API Listing failed with Auth/Limit error.");
                }
                return;
            }

            foreach (var item in res.list)
            {
                if (item.isdir == 1)
                {
                    await TraverseCloudDirectoryAsync(accessToken, item.path, files);
                }
                else
                {
                    files.Add(item);
                }
            }
        }
    }
}
