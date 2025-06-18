using deploytool.core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace deploytool
{
    public partial class MainForm : Form
    {
        private DeploymentEngine _deploymentEngine;
        private CancellationTokenSource _cts;

        public MainForm()
        {
            InitializeComponent();
            //btnPause.Enabled = false;
           // btnRollback.Enabled = false;
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            _cts = new CancellationTokenSource();
            _deploymentEngine = new DeploymentEngine(GetDeploymentConfig());

            //btnStart.Enabled = false;
            //btnPause.Enabled = true;
            //btnCancel.Enabled = true;

            await _deploymentEngine.StartDeployment(UpdateProgress, _cts.Token);
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            _deploymentEngine?.TogglePause();
           // btnPause.Text = _deploymentEngine?.IsPaused == true ? "继续" : "暂停";
        }

        private async void btnRollback_Click(object sender, EventArgs e)
        {
            await _deploymentEngine?.Rollback(UpdateProgress);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _cts?.Cancel();
        }

        private void UpdateProgress(DeploymentStep step, string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateProgress(step, message)));
                return;
            }

            //txtLog.AppendText($"[{DateTime.Now}] {step.StepName}: {message}{Environment.NewLine}");
            // 更新步骤状态显示
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

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(837, 503);
            this.Name = "MainForm";
            this.ResumeLayout(false);

        }
    }
}
