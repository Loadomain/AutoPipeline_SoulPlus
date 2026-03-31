using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Linq;
using System.Threading;

namespace SouthPlusDownloader
{
    internal static class Program
    {
        [DllImport("kernel32.dll")]
        static extern bool AttachConsole(int dwProcessId);

        private const int ATTACH_PARENT_PROCESS = -1;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                // CLI 模式
                AttachConsole(ATTACH_PARENT_PROCESS);
                Console.WriteLine("\r\n[SouthPlus Downloader] 命令行模式启动...");

                RunCliMode(args);
            }
            else
            {
                // GUI 模式
                ApplicationConfiguration.Initialize();
                Application.Run(new Form1());
            }
        }

        static void RunCliMode(string[] args)
        {
            try
            {
                // 默认值
                int boardId = 216;
                int startItem = 1;
                int endItem = 10;
                int maxPrice = 5;
                string[] platforms = new[] { "百度", "网盘", "baidu" };
                string customCode = string.Empty;
                string customPassword = string.Empty;
                bool enableLog = false;
                string logFile = "southplus.log";

                // 手动解析 args (为了不引入其它第三方命令行库保持轻量)
                for (int i = 0; i < args.Length; i++)
                {
                    switch (args[i].ToLower())
                    {
                        case "--board":
                        case "-b":
                            boardId = int.Parse(args[++i]);
                            break;
                        case "--start":
                        case "-s":
                            startItem = int.Parse(args[++i]);
                            break;
                        case "--end":
                        case "-e":
                            endItem = int.Parse(args[++i]);
                            break;
                        case "--price":
                        case "-p":
                            maxPrice = int.Parse(args[++i]);
                            break;
                        case "--keywords":
                        case "-k":
                            platforms = args[++i].Split(new[] { ',', '，' }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                            break;
                        case "--code":
                        case "-c":
                            customCode = args[++i];
                            break;
                        case "--pwd":
                        case "-pw":
                            customPassword = args[++i];
                            break;
                        case "--log":
                        case "-l":
                            enableLog = true;
                            break;
                        case "--help":
                        case "-h":
                            PrintHelp();
                            return;
                        case "--sync-boards":
                            var syncCfg = TaskConfig.LoadCredentials();
                            if (string.IsNullOrEmpty(syncCfg.Cookie)) { Console.WriteLine("[错误] 请先配置Cookie以同步板块。"); return; }
                            var handler = new System.Net.Http.HttpClientHandler { UseCookies = false, AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate };
                            var httpClient = new System.Net.Http.HttpClient(handler);
                            if(!string.IsNullOrEmpty(syncCfg.UserAgent)) httpClient.DefaultRequestHeaders.Add("User-Agent", syncCfg.UserAgent);
                            var bm = new BoardManager(syncCfg, httpClient, Console.WriteLine);
                            bm.FetchLatestBoardsAsync(new System.Threading.CancellationToken()).GetAwaiter().GetResult();
                            return;
                    }
                }

                // 加载保存的 Cookie
                var config = TaskConfig.LoadCredentials();
                if (string.IsNullOrEmpty(config.Cookie) || string.IsNullOrEmpty(config.UserAgent))
                {
                    Console.WriteLine("[错误] 没有在 credentials.json 中找到您的身份认证信息。");
                    Console.WriteLine("请先双击打开本程序的图形界面，点击“自动获取”登录成功一次后关闭，然后再使用命令行模式。");
                    return;
                }

                config.BoardId = boardId;
                config.StartItem = startItem;
                config.EndItem = endItem;
                config.MaxPrice = maxPrice;
                config.Platforms = platforms;
                config.CustomCodeKeywords = customCode;
                config.CustomPasswordKeywords = customPassword;

                Console.WriteLine($"[参数] 板块: {boardId}, 范围: {startItem}-{endItem}, 价格上限: {maxPrice}");
                Console.WriteLine($"[参数] 分发平台: {(platforms.Length > 0 ? string.Join(", ", platforms) : "未指定 (将启用无条件盲购)")}");
                if (!string.IsNullOrEmpty(customCode)) Console.WriteLine($"[参数] 自定义提取码词库: {customCode}");
                if (!string.IsNullOrEmpty(customPassword)) Console.WriteLine($"[参数] 自定义解压码词库: {customPassword}");

                Action<string> logger = (msg) =>
                {
                    string formattedMsg = $"[{DateTime.Now:HH:mm:ss}] {msg}";
                    Console.WriteLine(formattedMsg);
                    if (enableLog)
                    {
                        lock (logFile) {
                            File.AppendAllText(logFile, formattedMsg + Environment.NewLine);
                        }
                    }
                };

                var cts = new CancellationTokenSource();
                var crawler = new CrawlerService(config, logger);

                crawler.StartAsync(cts.Token).GetAwaiter().GetResult();
                Console.WriteLine("\n[完成] 命令行抓取任务顺利结束。");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[严重错误] 命令行运行时遇到错误: {ex.Message}");
            }
        }

        static void PrintHelp()
        {
            Console.WriteLine(@"
SouthPlus Downloader 命令行使用说明

常规参数：
  -b, --board      板块 ID，默认为 216 (C104)
  -s, --start      采集起始条目 (默认 1)
  -e, --end        采集结束条目 (默认 10)
  -p, --price      允许自动购买的最大 SP 币价格 (默认 5)
  -k, --keywords   资源平台关键词过滤，用逗号或者中文逗号分隔 (留空或不填默认 百度,网盘,baidu)
  -c, --code       补充自定义提取码正则/关键字，逗号分隔 (例：密码,sec)
  -pw, --pwd       补充自定义解压码正则/关键字，逗号分隔 (例：秘钥,解压)
  -l, --log        开启日志记录，会将输出同步写入 southplus.log 文件中
  -h, --help       查看本帮助文件

示例用法：
  .\SouthPlusDownloader.exe -b 216 -s 1 -e 25 -p 0 -k ""百度,mega"" -c ""特殊密码"" --log
  .\SouthPlusDownloader.exe -b 216 -s 1 -e 25 -p 5 -k "" "" --log  (盲购模式)
");
        }
    }
}