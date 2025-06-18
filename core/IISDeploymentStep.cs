using Microsoft.Web.Administration;
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
                try
                {
                    //安装MIME类型数组
                    using (ServerManager serverManager = new ServerManager())
                    {
                        Configuration config = serverManager.GetApplicationHostConfiguration();
                        ConfigurationSection staticContentSection = config.GetSection("system.webServer/staticContent");
                        ConfigurationElementCollection mimeMapCollection = staticContentSection.GetCollection();

                        // 定义要添加的MIME类型数组
                        string[][] mimeTypesToAdd = new string[][]
                        {
                        new string[] { ".atlas", "application/octet-stream" },
                        new string[] { ".gltf", "model/gltf-binary" },
                        new string[] { ".webp", "image/webp" }
                        };

                        foreach (string[] mimeTypeInfo in mimeTypesToAdd)
                        {
                            string fileExtension = mimeTypeInfo[0];
                            string mimeType = mimeTypeInfo[1];

                            // 检查MIME类型是否已存在
                            if (!MimeTypeExists(mimeMapCollection, fileExtension))
                            {
                                ConfigurationElement newMimeMapElement = mimeMapCollection.CreateElement("mimeMap");
                                newMimeMapElement["fileExtension"] = fileExtension;
                                newMimeMapElement["mimeType"] = mimeType;
                                mimeMapCollection.Add(newMimeMapElement);
                                Console.WriteLine($"已添加MIME类型: {fileExtension} -> {mimeType}");
                            }
                            else
                            {
                                Console.WriteLine($"MIME类型已存在: {fileExtension} -> {mimeType}");
                            }
                        }

                        serverManager.CommitChanges();
                        Console.WriteLine("所有更改已提交到applicationHost.config");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"操作过程中发生错误: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"内部错误: {ex.InnerException.Message}");
                    }
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

        // 检查指定的文件扩展名是否已存在于MIME映射中
        private static bool MimeTypeExists(ConfigurationElementCollection collection, string fileExtension)
        {
            foreach (ConfigurationElement element in collection)
            {
                if (element["fileExtension"].ToString().Equals(fileExtension, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public override Task Rollback(Action<DeploymentStep, string> progressCallback)
        {
            throw new NotImplementedException();
        }
    }
}
