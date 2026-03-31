using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BaiduAutoDownloader
{
    public class BaiduApiService
    {
        private readonly HttpClient _httpClient;

        public BaiduApiService()
        {
            _httpClient = new HttpClient();
            // Crucial user-agent to bypass some limits
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "pan.baidu.com");
        }

        /// <summary>
        /// Create a directory on the user's personal drive
        /// </summary>
        public async Task<bool> CreateCloudDirectoryAsync(string path, string bduss, string stoken, string bdstoken)
        {
            var url = $"https://pan.baidu.com/api/create?a=commit&channel=chunlei&clienttype=0&web=1&bdstoken={bdstoken}";
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("Cookie", $"BDUSS={bduss}; STOKEN={stoken}");
            request.Headers.Add("Referer", "https://pan.baidu.com/disk/home");

            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("path", path),
                new KeyValuePair<string, string>("isdir", "1"),
                new KeyValuePair<string, string>("size", "0"),
                new KeyValuePair<string, string>("block_list", "[]"),
                new KeyValuePair<string, string>("method", "post")
            });

            request.Content = formContent;
            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            var json = JsonConvert.DeserializeObject<dynamic>(content);
            return json?.errno == 0;
        }

        /// <summary>
        /// Get user's BDSTOKEN (CSRF token) needed for web API endpoints since share pages have it empty.
        /// </summary>
        public async Task<string> GetBdsTokenAsync(string bduss, string stoken)
        {
            var url = "https://pan.baidu.com/api/gettemplatevariable?fields=[%22bdstoken%22]";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Cookie", $"BDUSS={bduss}; STOKEN={stoken}");
            request.Headers.Add("Referer", "https://pan.baidu.com/disk/home");

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            var json = JsonConvert.DeserializeObject<dynamic>(content);
            return json?.result?.bdstoken?.ToString() ?? "";
        }

        /// <summary>
        /// Transfer (Save to Netdisk) using Web Cookies
        /// </summary>
        public async Task<BaiduWebTransferResponse> TransferShareToDriveAsync(string shareId, string uk, string bdstoken, string bduss, string stoken, string bdclnd, string[] fsids, string targetPath)
        {
            // The user must own a basic path in their netdisk for this to work.
            var url = $"https://pan.baidu.com/share/transfer?shareid={shareId}&from={uk}&ondup=newcopy&async=0&bdstoken={bdstoken}&channel=chunlei&web=1&app_id=250528&clienttype=0";

            var cookies = new List<string>
            {
                $"BDUSS={bduss}",
                $"STOKEN={stoken}"
            };
            if (!string.IsNullOrEmpty(bdclnd)) cookies.Add($"BDCLND={bdclnd}");

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("Cookie", string.Join("; ", cookies));
            request.Headers.Add("Referer", "https://pan.baidu.com/");

            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("fsidlist", JsonConvert.SerializeObject(fsids)),
                new KeyValuePair<string, string>("path", targetPath)
            });
            request.Content = formContent;

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<BaiduWebTransferResponse>(content);
        }

        /// <summary>
        /// Obtain the file list of a share via the web API. Vital if DOM extraction failed to grab fs_ids.
        /// </summary>
        public async Task<BaiduWebWxListResponse> GetShareListAsync(string shortUrl, string pwd, string bduss, string stoken, string bdclnd)
        {
            var url = $"https://pan.baidu.com/share/wxlist?channel=chunlei&clienttype=0&web=1&page=1&num=1000&dir=%2F&shorturl={shortUrl}&pwd={pwd}&root=1";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            
            var cookies = new List<string>
            {
                $"BDUSS={bduss}",
                $"STOKEN={stoken}",
                $"BDCLND={bdclnd}"
            };
            request.Headers.Add("Cookie", string.Join("; ", cookies));
            request.Headers.Add("Referer", "https://pan.baidu.com/disk/home");

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<BaiduWebWxListResponse>(content);
        }

        /// <summary>
        /// List files in the user's personal drive
        /// </summary>
        public async Task<BaiduBasicFileListResponse> ListFilesAsync(string accessToken, string dirPath)
        {
            var url = $"https://pan.baidu.com/rest/2.0/xpan/file?method=list&dir={Uri.EscapeDataString(dirPath)}&order=time&desc=1&folder=0&access_token={accessToken}";
            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<BaiduBasicFileListResponse>(content);
        }

        /// <summary>
        /// Query original dlink for files in the user's personal drive
        /// </summary>
        public async Task<BaiduBasicFileMetasResponse> GetFileDlinksAsync(string accessToken, long[] fsids)
        {
            var url = $"https://pan.baidu.com/rest/2.0/xpan/multimedia?method=filemetas&dlink=1&fsids={Uri.EscapeDataString(JsonConvert.SerializeObject(fsids))}&access_token={accessToken}";
            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<BaiduBasicFileMetasResponse>(content);
        }

        /// <summary>
        /// Delete the temporary folder
        /// </summary>
        public async Task<bool> DeleteFileAsync(string accessToken, string path)
        {
            var url = $"https://pan.baidu.com/rest/2.0/xpan/file?method=filemanager&opera=delete&access_token={accessToken}";
            
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            
            var payload = new[] { path };
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("filelist", JsonConvert.SerializeObject(payload)),
                new KeyValuePair<string, string>("async", "0")
            });
            
            request.Content = formContent;
            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            var json = JsonConvert.DeserializeObject<dynamic>(content);
            return json?.errno == 0;
        }
    }
}
