using deploytool.core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.DirectoryServices;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace deploytool
{
    public partial class Main : Form
    {
        private DeploymentEngine _deploymentEngine;
        private CancellationTokenSource _cts;
        //private string folderPath = @"D:\iis\s"; // 替换为实际的文件夹路径

        /// <summary>
        /// 站点配置信息
        /// </summary>
        public static List<WebSitConfig> SiteConfig { get; set; }

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            // 获取项目根目录
            string projectDirectory = Environment.CurrentDirectory;
            //string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).FullName;

            string jsonPath = Path.Combine(projectDirectory, $"config/{Constant.WebSit}/sit.json");
            // 检查文件是否存在
            if (File.Exists(jsonPath))
            {
                // 读取文件内容
                string content = File.ReadAllText(jsonPath);
                SiteConfig = JsonConvert.DeserializeObject<List<WebSitConfig>>(File.ReadAllText(jsonPath));
                //Console.WriteLine("文件内容：");
                //Console.WriteLine(content);
            }
        }

        /// <summary>
        /// 开始发布
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnStart_Click(object sender, EventArgs e)
        {
            //throw new Exception($"安装失败，退出代码:");

            //string projectDirectory = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.FullName;

            //string parentDirectory = Directory.GetParent(Environment.CurrentDirectory).FullName;

            //要判断config配置跟实际文件夹要一一对应，否则会异常

            //安装证书

            //安装运行时 环境


            foreach (var sit in SiteConfig)
            {
                var (check, messge) = WebSitCheck.CheckApp(sit);
                if (!check)
                {
                    MessageBox.Show(messge, "添加网站", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            _cts = new CancellationTokenSource();
            _deploymentEngine = new DeploymentEngine(GetDeploymentConfig());

            btnCheck.Enabled = false;

            //btnStart.Enabled = false;
            //btnPause.Enabled = true;
            //btnCancel.Enabled = true;

            await _deploymentEngine.StartDeployment(UpdateProgress, _cts.Token);
        }

        private void UpdateProgress(DeploymentStep step, string message)
        {
            //if (InvokeRequired)
            //{
            //    Invoke(new Action(() => UpdateProgress(step, message)));
            //    return;
            //}
            if (step != null)
            {
                //txtLog.AppendText($"[{DateTime.Now}] {step.StepName}: {message}{Environment.NewLine}");
                //txtLog.AppendText($"---------------{Environment.NewLine}");
            }
            txtLog.AppendText($"[{DateTime.Now}] {step?.StepName ?? "完成"}: {message}{Environment.NewLine}");
            // 更新步骤状态显示
            txtLog.ScrollToCaret();
        }

        private DeploymentConfig GetDeploymentConfig()
        {
            // 从UI控件获取配置
            return new DeploymentConfig
            {
                //SqlServerPath = txtSqlPath.Text,
                //DbBackupPath = txtDbBackup.Text,
                //RabbitMQPath = txtRabbitMQPath.Text,
                // 其他配置项...
            };
        }

        /// <summary>
        /// 校验配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCheck_Click(object sender, EventArgs e)
        {
            var f = new form.ConfigForm();
            f.ShowDialog();
            //// 获取项目根目录
            //string projectDirectory = Environment.CurrentDirectory;
            //// 构建文件路径（假设txt文件在项目根目录下）
            //string filePath = Path.Combine(projectDirectory, "config/4-website/sit.json");
            //// 检查文件是否存在
            //if (File.Exists(filePath))
            //{
            //    // 读取文件内容
            //    string content = File.ReadAllText(filePath);

            //    var config = JsonConvert.DeserializeObject<List<DeploymentConfig>>(File.ReadAllText(filePath));

            //    Console.WriteLine("文件内容：");
            //    Console.WriteLine(content);
            //}
            //else
            //{
            //    Console.WriteLine($"文件不存在：{filePath}");
            //}

            //判断iis是否安装
            //DirectoryEntry rootEntry = new DirectoryEntry("IIS://localhost/w3svc");

            //MessageBox.Show(IISDeploymentStep.IsIisInstalled().ToString());
            //if (!IISDeploymentStep.IsIisInstalled())
            //{
            //    //安装iis
            //}

            ////1：校验sql是否安装，没安装，是否有安装包，安装了，提示用户

            //var t = SoftWareManager.IsSoftwareInstalled("SQL Server");

            //2：校验mq是否安装

            //3：校验nginx是否安装

            //4：项目文件是否有，最少要1个

            //5：服务是否有
        }

    }
}
