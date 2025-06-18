using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace deploytool.core
{
    /// <summary>
    /// 具体的项目和服务部署
    /// </summary>
    internal class WebSitDeploymentStep : DeploymentStep
    {
        public WebSitDeploymentStep(DeploymentConfig config) : base(config)
        {
            config.DeployType = DeployType.Sit;
            StepName = "项目 部署";
        }

        ///// <summary>
        ///// 判断iis是否已经安装
        ///// </summary>
        ///// <returns></returns>
        //public static bool IsIisInstalled()
        //{
        //    try
        //    {
        //        using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(
        //            "SELECT * FROM Win32_Service WHERE Name='W3SVC'"))
        //        {
        //            var y = searcher.Get().Count;
        //            return searcher.Get().Count > 0;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

        public override async Task<bool> Execute(Action<DeploymentStep, string> progressCallback, CancellationToken ct)
        {
            try
            {
                var host = $"{Config.SiteName}:{Config.Port}";
                progressCallback(this, $"正在部署网站 {host}...");

                //// 确保源路径存在
                //if (!Directory.Exists(Config.WebAppPath))
                //{
                //    Console.WriteLine($"源路径不存在: {sourcePath}");
                //    return false;
                //}



                //// 配置参数
                string siteName = Config.SiteName;// "YourWebApp";
                string physicalPath = Path.Combine(Config.WebAppPath, Config.SiteName);// @"C:\inetpub\wwwroot\YourWebApp";
                //int port = 8080;
                //string dllPath = @"C:\Path\To\YourDll.dll"; // DLL源路径
                string appPoolName = siteName;// + "AppPool";
                //var f = Path.Combine(physicalPath, Config.SiteName);

                // 创建网站目录
                if (!Directory.Exists(physicalPath))
                {
                    progressCallback(this, $"创建文件夹路径 {Config.WebAppPath}...");
                    Directory.CreateDirectory(physicalPath);
                }

                // 创建应用程序池
                CreateAppPool(appPoolName);

                // 创建网站目录
                //CreateWebsiteDirectory(physicalPath);

                // 复制DLL到网站目录
                //DeployDll(Config.ServiceAppPath, Path.Combine(physicalPath, "bin"));

                //DeployDll(Config.ServiceAppPath, physicalPath);
                CopyFilesRecursively(new DirectoryInfo(Config.ServiceAppPath), new DirectoryInfo(physicalPath));

                // 创建网站
                CreateWebsite();

                //Console.WriteLine($"IIS网站 '{siteName}' 已成功创建在端口 {port}");
                //Console.WriteLine("按任意键退出...");
                //Console.ReadKey();


                //var name = WebSitCheck.CheckSiteNameExists(Config.SiteName);
                //if(name)
                //{

                //}

                //发布成功，还要写host，host最好是提前准备好，直接复制替换最好

                //progressCallback(this, $"正在部署项目{host}...");
                //if (IsIisInstalled())
                //{
                //    progressCallback(this, "服务器已经配置了 IIS，跳过配置...");
                //}
                //else
                //{

                //}
                //progressCallback(this, "正在安装 IIS...");
                //// 使用静默安装参数
                //var process = Process.Start("setup.exe", $"/ConfigurationFile=Configuration.ini /QS");
                //process.WaitForExit();
                ////await process.WaitForExitAsync(ct);

                //if (process.ExitCode != 0)
                //    throw new Exception($"安装失败，退出代码: {process.ExitCode}");

                //progressCallback(this, "正在还原数据库...");
                //await RestoreDatabase(progressCallback);

                IsExecuted = true;
                return true;
            }
            catch (Exception ex)
            {
                progressCallback(this, $"错误: {ex.Message}");
                return false;
            }
        }


        static void CreateAppPool(string appPoolName)
        {
            using (ServerManager serverManager = new ServerManager())
            {
                // 检查应用程序池是否已存在
                // if (serverManager.ApplicationPools[appPoolName] == null)
                //{
                ApplicationPool newAppPool = serverManager.ApplicationPools.Add(appPoolName);
                newAppPool.ManagedRuntimeVersion = "v4.0";
                newAppPool.ManagedPipelineMode = ManagedPipelineMode.Integrated;
                newAppPool.AutoStart = true;
                serverManager.CommitChanges();
                //Console.WriteLine($"应用程序池 '{appPoolName}' 已创建");
                //}
                //else
                //{
                //    Console.WriteLine($"应用程序池 '{appPoolName}' 已存在");
                //}
            }
        }

        //static void CreateWebsiteDirectory(string physicalPath)
        //{
        //    if (!Directory.Exists(physicalPath))
        //    {
        //        Directory.CreateDirectory(physicalPath);
        //        Console.WriteLine($"网站目录 '{physicalPath}' 已创建");

        //        // 创建bin目录
        //        string binPath = Path.Combine(physicalPath, "bin");
        //        Directory.CreateDirectory(binPath);
        //        Console.WriteLine($"bin目录 '{binPath}' 已创建");
        //    }
        //    else
        //    {
        //        Console.WriteLine($"网站目录 '{physicalPath}' 已存在");
        //    }
        //}

        private static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
        {
            foreach (DirectoryInfo dir in source.GetDirectories())
            {
                CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
            }

            foreach (FileInfo file in source.GetFiles())
            {
                file.CopyTo(Path.Combine(target.FullName, file.Name), true);
            }
        }

        static void DeployDll(string sourceDllPath, string targetBinPath)
        {
            if (!Directory.Exists(targetBinPath))
            {
                Directory.CreateDirectory(targetBinPath);
            }

            string dllFileName = Path.GetFileName(sourceDllPath);
            string targetPath = Path.Combine(targetBinPath, dllFileName);

            File.Copy(sourceDllPath, targetPath, true); //true 说明可以覆盖同名文件
            //Console.WriteLine($"DLL已部署到: {targetPath}");

            // 复制依赖项（如果有）
            // 这里可以添加代码来复制DLL的所有依赖项
        }

        void CreateWebsite()
        {
            using (ServerManager serverManager = new ServerManager())
            {
                //// 检查网站是否已存在
                //if (serverManager.Sites[siteName] == null)
                //{
                string physicalPath = Path.Combine(Config.WebAppPath, Config.SiteName);
                Site newSite = serverManager.Sites.Add(Config.SiteName, physicalPath, Config.Port);

                // 清除默认绑定
                newSite.Bindings.Clear();

                // 添加带域名的绑定
                string bindingInformation = $"*:{Config.Port}:{Config.SiteName}";
                newSite.Bindings.Add(bindingInformation, "http");

                //不带域名的绑定
                //string bindingInfo = $"*:{Config.Port}:"; // 不包含主机名部分
                //newSite.Bindings.Add(bindingInfo, "http");

                //添加证书

                newSite.ApplicationDefaults.ApplicationPoolName = Config.SiteName;
                serverManager.CommitChanges();
                //Console.WriteLine($"网站 '{siteName}' 已创建并绑定到域名 {Config.SiteName}:{port}");
            }
        }

        public override Task Rollback(Action<DeploymentStep, string> progressCallback)
        {
            throw new NotImplementedException();
        }
    }
}
