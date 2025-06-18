using deploytool.core;
using deploytool.iis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace deploytool.form
{
    public partial class Step_iis : Form
    {
        private DeploymentEngine _deploymentEngine;
        private CancellationTokenSource _cts;
        public Step_iis()
        {
            InitializeComponent();
        }
        private async void btnStart_Click(object sender, EventArgs e)
        {
            _cts = new CancellationTokenSource();
            _deploymentEngine = new DeploymentEngine(GetDeploymentConfig());

            var y = new IISDeploymentStep(GetDeploymentConfig());
            await _deploymentEngine.StartDeployment1(UpdateProgress, y, _cts.Token);
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
        private void btnStart_Click1(object sender, EventArgs e)
        {

            //判断是否是管理员权限运行的

            //UpdateProgress("IIS已经安装，跳过安装");
            //显示下一步
            btnNext.Enabled = true;

            btnStart.Enabled = false; //安装按钮隐藏

            //Invoke(new Action(() =>
            //{
            //    try
            //    {
            //        var iis = new IISRuntime();
            //        //要管理员权限才行
            //        if (iis.IsExsitIIS())
            //        {
            //            UpdateProgress("IIS已经安装，跳过安装");
            //            //显示下一步
            //            btnNext.Visible = true;
            //            btnStart.Visible = false; //安装按钮隐藏
            //        }
            //        else
            //        {
            //            UpdateProgress("正在安装iis...");
            //            //安装iis
            //            //iis.InstallIISRun();
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        UpdateProgress(ex.Message);
            //    }
            //}));
        }

        private void UpdateProgress1(string message, bool isComplete = false)
        {
            //if (InvokeRequired)
            //{
            //    Invoke(new Action(() => UpdateProgress(step, message)));
            //    return;
            //}
            //if (step != null)
            //{
            //    //txtLog.AppendText($"[{DateTime.Now}] {step.StepName}: {message}{Environment.NewLine}");
            //    //txtLog.AppendText($"---------------{Environment.NewLine}");
            //}
            txtLog.AppendText($"[{DateTime.Now}] {(isComplete ? "IIS安装完成" : "IIS安装")}: {message}{Environment.NewLine}");
            // 更新步骤状态显示
            txtLog.ScrollToCaret();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            // 隐藏当前窗口
            this.Hide();

            // 创建并显示窗口
            Step_mq step = new Step_mq();

            // 当窗口关闭时，关闭当前应用程序
            step.FormClosed += (s, args) => this.Close();
            step.Show();
        }
    }
}
