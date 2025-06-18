using deploytool.core;
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
    public partial class Step_mq : Form
    {
        private DeploymentEngine _deploymentEngine;
        private CancellationTokenSource _cts;
        public Step_mq()
        {
            InitializeComponent();
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            /*
             安装rabbitmq，以服务的方式安装
             */
            _cts = new CancellationTokenSource();
            _deploymentEngine = new DeploymentEngine(GetDeploymentConfig());

            var y = new RabbitMQDeploymentStep(GetDeploymentConfig());
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
        private void btnNext_Click(object sender, EventArgs e)
        {
            // 隐藏当前窗口
            this.Hide();

            // 创建并显示窗口
            Step_nginx step = new Step_nginx();

            step.FormClosed += (s, args) => this.Close();
            step.Show();
        }
    }
}
