using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace deploytool.core
{
    internal class IISDeploymentStep : DeploymentStep
    {
        public IISDeploymentStep(DeploymentConfig config) : base(config)
        {
            config.SiteName = "IIS";
            config.DeployType = DeployType.IIS;
            StepName = "IIS 部署";
        }

        /// <summary>
        /// 判断iis是否已经安装
        /// </summary>
        /// <returns></returns>
        public static bool IsIisInstalled()
        {
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                    "SELECT * FROM Win32_Service WHERE Name='W3SVC'"))
                {
                    var y = searcher.Get().Count;
                    return searcher.Get().Count > 0;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public override async Task<bool> Execute(Action<DeploymentStep, string> progressCallback, CancellationToken ct)
        {
            try
            {
                progressCallback(this, "正在安装 IIS...");
                if (IsIisInstalled())
                {
                    progressCallback(this, "服务器已经配置了 IIS，跳过配置...");
                }
                else
                {

                }

                IsExecuted = true;
                return true;
            }
            catch (Exception ex)
            {
                progressCallback(this, $"错误: {ex.Message}");
                return false;
            }
        }

        public override Task Rollback(Action<DeploymentStep, string> progressCallback)
        {
            throw new NotImplementedException();
        }
    }
}
