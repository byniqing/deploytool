using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace deploytool.core
{
    /// <summary>
    /// 安装windows服务
    /// </summary>
    internal class WindowsServicesDeploymentStep
    {
        // 检查特定服务是否存在
        static bool CheckServiceExists(string serviceName)
        {
            try
            {
                using (ServiceController controller = new ServiceController(serviceName))
                {
                    // 如果能获取到服务状态，则认为服务已存在
                    ServiceControllerStatus status = controller.Status;
                    //Log($"找到服务: {serviceName} (状态: {status})");
                    return true;
                }
            }
            catch (InvalidOperationException)
            {
                // 服务不存在
                return false;
            }
        }
    }
}
