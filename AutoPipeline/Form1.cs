using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoPipeline
{
    public partial class Form1 : Form
    {
        private readonly string baseDir = @"e:\loadfield\workforce\software\自动化三合一";
        private string SPPath => Path.Combine(baseDir, "SouthPlusDownloader", "publish", "SouthPlusDownloader.exe");
        private string BaiduPath => Path.Combine(baseDir, "BaiduAutoDownloader", "publish", "BaiduAutoDownloader.exe");
        private string ExtractorPath => Path.Combine(baseDir, "AutoExtractor", "publish", "AutoExtractor.exe");
        
        private string SpJsonPath => Path.Combine(baseDir, "SouthPlusDownloader", "publish", "southplus_results.json");
        private string SpBoardsPath => Path.Combine(baseDir, "SouthPlusDownloader", "publish", "boards.json");
        private string BaiduConfigPath => Path.Combine(baseDir, "BaiduAutoDownloader", "publish", "config.json");

        public Form1()
        {
            InitializeComponent();
            LoadBoards();
            LoadBaiduConfig();
        }

        private void AppendLog(string text)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendLog), text);
                return;
            }
            rtbLog.AppendText($"{DateTime.Now:HH:mm:ss} | {text}\n");
            rtbLog.SelectionStart = rtbLog.Text.Length;
            rtbLog.ScrollToCaret();
        }

        private void UpdateProgress(string filename, string status, string speed, string progressStr)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string, string, string, string>(UpdateProgress), filename, status, speed, progressStr);
                return;
            }

            double.TryParse(progressStr, out double progressValue);
            string bar = MakeTextBar(progressValue);

            foreach (DataGridViewRow row in dgvDownloads.Rows)
            {
                if (row.Cells[0].Value?.ToString() == filename)
                {
                    row.Cells[1].Value = status;
                    row.Cells[2].Value = speed;
                    row.Cells[3].Value = bar;
                    return;
                }
            }

            // Not found, add new row
            dgvDownloads.Rows.Add(filename, status, speed, bar);
        }

        private string MakeTextBar(double progress)
        {
            int max = 20;
            int filled = (int)(progress / 100.0 * max);
            if(filled > max) filled = max;
            if(filled < 0) filled = 0;
            string bar = new string('█', filled) + new string('░', max - filled);
            return $"[{bar}] {progress:F1}%";
        }

        private void LoadBoards()
        {
            try
            {
                if (File.Exists(SpBoardsPath))
                {
                    string json = File.ReadAllText(SpBoardsPath);
                    var dict = JsonSerializer.Deserialize<Dictionary<string, int>>(json);
                    cmbBoards.Items.Clear();
                    foreach(var k in dict.Keys) cmbBoards.Items.Add(k);
                    if(cmbBoards.Items.Count > 0) cmbBoards.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                AppendLog("加载板块失败: " + ex.Message);
            }
        }

        private void LoadBaiduConfig()
        {
            try
            {
                if (File.Exists(BaiduConfigPath))
                {
                    string json = File.ReadAllText(BaiduConfigPath);
                    var doc = JsonDocument.Parse(json);
                    
                    if (doc.RootElement.TryGetProperty("ApiMode", out var apiModeNode))
                    {
                        var mode = apiModeNode.GetString();
                        rbPrivateApi.Checked = (mode == "Custom");
                        rbPublicApi.Checked = (mode != "Custom");
                    }
                    
                    if (doc.RootElement.TryGetProperty("CustomAppKey", out var appKeyNode))
                        txtAppKey.Text = appKeyNode.GetString();
                        
                    if (doc.RootElement.TryGetProperty("CustomSecretKey", out var secKeyNode))
                        txtSecretKey.Text = secKeyNode.GetString();
                }
            }
            catch { }
        }

        private void SaveBaiduConfig()
        {
            try
            {
                var dict = new Dictionary<string, object>();
                if (File.Exists(BaiduConfigPath))
                {
                    string oldJson = File.ReadAllText(BaiduConfigPath);
                    dict = JsonSerializer.Deserialize<Dictionary<string, object>>(oldJson);
                }

                dict["ApiMode"] = rbPrivateApi.Checked ? "Custom" : "PublicPool";
                dict["CustomAppKey"] = txtAppKey.Text;
                dict["CustomSecretKey"] = txtSecretKey.Text;
                dict["TargetDirectory"] = txtTargetDir.Text;

                string newJson = JsonSerializer.Serialize(dict, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(BaiduConfigPath, newJson);
            }
            catch (Exception ex)
            {
                AppendLog("保存 Baidu 配置失败: " + ex.Message);
            }
        }

        private void rbApiMode_CheckedChanged(object sender, EventArgs e)
        {
            txtAppKey.Enabled = rbPrivateApi.Checked;
            txtSecretKey.Enabled = rbPrivateApi.Checked;
        }

        private void btnAuthSP_Click(object sender, EventArgs e)
        {
            using (var sform = new SilentAuthForm())
            {
                sform.ShowDialog();
                if (sform.Success)
                    AppendLog("南+授权凭证已经成功捕获并保存完毕 (Cookie, UA 隐藏保护中).");
                else
                    AppendLog("未成功获取南+授权。");
            }
        }

        private async void btnSyncBoards_Click(object sender, EventArgs e)
        {
            btnSyncBoards.Enabled = false;
            AppendLog("正在后台静默同步板块...");
            try
            {
                await RunProcessAndLogAsync(SPPath, "--sync-boards");
                LoadBoards();
                AppendLog("板块同步完成，下拉列表已更新。");
            }
            catch (Exception ex)
            {
                AppendLog("同步出错: " + ex.Message);
            }
            finally
            {
                btnSyncBoards.Enabled = true;
            }
        }

        private void btnAuthBaidu_Click(object sender, EventArgs e)
        {
            string appKey = rbPrivateApi.Checked ? txtAppKey.Text.Trim() : "iYCeC9g08h5vuP9UqvPHKKSVrKFXGa1v";
            
            if (string.IsNullOrEmpty(appKey))
            {
                MessageBox.Show("应用 AppKey 为空，如果是私有通道请确保已输入！");
                return;
            }
            
            AppendLog("跳过原网盘客户端界面... 极速激活内嵌网页进行网盘登录...");
            using (var bauth = new BaiduAuthForm(appKey))
            {
                bauth.ShowDialog();
                if (bauth.Success && !string.IsNullOrEmpty(bauth.AccessToken))
                {
                    AppendLog("[授权通过] 网盘凭证已在 UI 内网拦截成功.");
                    
                    // 保存 AccessToken 到 config.json 去给 BaiduAutoDownloader 用！
                    SaveBaiduConfig(); // 先刷基础配置
                    
                    try
                    {
                        string oldJson = File.Exists(BaiduConfigPath) ? File.ReadAllText(BaiduConfigPath) : "{}";
                        var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(oldJson);
                        dict["AccessToken"] = bauth.AccessToken;
                        dict["ExpiryTime"] = DateTime.Now.AddDays(29); // 简化的 token 30 天有效期保存
                        File.WriteAllText(BaiduConfigPath, JsonSerializer.Serialize(dict, new JsonSerializerOptions { WriteIndented = true }));
                        
                        AppendLog("[网盘配置刷新] 已将内嵌拦截的秘钥无缝转移到内核。");
                    }
                    catch { }
                }
                else
                {
                    AppendLog("用户关闭了网盘登入卡片...");
                }
            }
        }

        private void btnBrowseTarget_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    txtTargetDir.Text = fbd.SelectedPath;
                }
            }
        }

        private async Task<int> RunProcessAndLogAsync(string filename, string arguments)
        {
            AppendLog($"\n>>> 执行系统指令: {Path.GetFileName(filename)} {arguments}");
            var tcs = new TaskCompletionSource<int>();
            
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = filename,
                    Arguments = arguments,
                    WorkingDirectory = Path.GetDirectoryName(filename),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    // 取消强制指定编码，允许它使用操作系统默认的 GBK 回显防止中文乱码变成  和 框
                },
                EnableRaisingEvents = true
            };

            process.OutputDataReceived += (s, e) => 
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    if (e.Data.StartsWith("[DL_PROG]|"))
                    {
                        var parts = e.Data.Split('|');
                        if (parts.Length >= 5)
                            UpdateProgress(parts[1], parts[2], parts[3], parts[4]);
                    }
                    else
                    {
                        AppendLog(e.Data);
                    }
                }
            };
            
            process.ErrorDataReceived += (s, e) => { if (!string.IsNullOrEmpty(e.Data)) AppendLog($"[错误] {e.Data}"); };

            process.Exited += (s, e) =>
            {
                tcs.SetResult(process.ExitCode);
                process.Dispose();
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return await tcs.Task;
        }

        private async void btnOneClick_Click(object sender, EventArgs e)
        {
            btnOneClick.Enabled = false;
            try
            {
                AppendLog("========== 【一键流水线开启】 ==========");

                // === 阶段 1：SP 爬取
                AppendLog("\n>> [阶段 1/3] 南+数据采集...");
                
                string boardParam = "";
                if (cmbBoards.SelectedItem != null && cmbBoards.SelectedItem.ToString() == "自定义")
                    boardParam = txtCustomBoard.Text;
                else if (cmbBoards.SelectedItem != null)
                {
                    try
                    {
                        string json = File.ReadAllText(SpBoardsPath);
                        var dict = JsonSerializer.Deserialize<Dictionary<string, int>>(json);
                        boardParam = dict[cmbBoards.SelectedItem.ToString()].ToString();
                    } catch {}
                }
                if(string.IsNullOrEmpty(boardParam)) boardParam = "216";

                string spArgs = $"-b {boardParam} -s {txtStartItem.Text} -e {txtEndItem.Text} -p {nudMaxPrice.Value} -k \"{txtKeywords.Text}\"";
                if(!string.IsNullOrEmpty(txtExtractKeywords.Text)) spArgs += $" -c \"{txtExtractKeywords.Text}\"";
                if(!string.IsNullOrEmpty(txtPasswordKeywords.Text)) spArgs += $" -pw \"{txtPasswordKeywords.Text}\"";
                spArgs += " -l"; // enable log

                await RunProcessAndLogAsync(SPPath, spArgs);

                // === 阶段 2：百度云下载
                tabControl1.SelectedTab = tabDownloads;
                dgvDownloads.Rows.Clear();
                AppendLog("\n>> [阶段 2/3] 百度网盘自动化下载...");
                
                SaveBaiduConfig();
                
                if (!File.Exists(SpJsonPath))
                {
                    AppendLog($"[警告] 未找到数据源 {SpJsonPath}，提取结果为空。操作终止。");
                    return;
                }

                // Call Baidu cli
                string bArgs = $"--test \"{SpJsonPath}\" \"{txtTargetDir.Text}\"";
                await RunProcessAndLogAsync(BaiduPath, bArgs);

                // === 阶段 3：解压修复
                tabControl1.SelectedTab = tabLog;
                AppendLog("\n>> [阶段 3/3] 全自动密码分析与解压...");

                List<string> passwords = new List<string>();
                try
                {
                    string json = File.ReadAllText(SpJsonPath);
                    var data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
                    foreach (var kvp in data.Values)
                    {
                        if (kvp.TryGetProperty("解压密码", out var pwdNode))
                        {
                            string pwd = pwdNode.GetString();
                            if (!string.IsNullOrWhiteSpace(pwd) && pwd != "未获取") passwords.Add(pwd);
                        }
                    }
                }
                catch (Exception ex) { AppendLog($"密码收集异常: {ex.Message}"); }

                var uniquePasswords = passwords.Distinct().ToList();
                AppendLog($"收集到 {uniquePasswords.Count} 个可能的论坛专属解压密码进行爆破。");

                string extArgs = $"--dir \"{txtTargetDir.Text}\"";
                if (!string.IsNullOrWhiteSpace(txtFakeExt.Text)) extArgs += $" --fake-ext \"{txtFakeExt.Text}\"";
                if (!string.IsNullOrWhiteSpace(txtRealExt.Text)) extArgs += $" --target-ext \"{txtRealExt.Text}\"";

                foreach (var p in uniquePasswords) extArgs += $" --password \"{p}\"";

                await RunProcessAndLogAsync(ExtractorPath, extArgs);

                AppendLog("\n========== 【一键流水线运转完美结束】 ==========");
                MessageBox.Show("全自动化流水线所有阶段执行完毕！", "回收成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                AppendLog($"[流水线总线错误] 异常崩溃: {ex}");
            }
            finally
            {
                btnOneClick.Enabled = true;
            }
        }
    }
}
