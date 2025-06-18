using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace deploytool.core
{
    public class WebSitCheck
    {
        public static (bool, string) CheckApp(WebSitConfig config)
        {
            if (CheckSiteNameExists(config.SiteName))
            {
                return (false, $"网站名称 {config.SiteName} 已存在");
            }
            else if (CheckAppPoolExists(config.SiteName))
            {
                return (false, $"程序池 {config.SiteName} 已存在");
            }
            else if (CheckPortInUse(config.Port))
            {
                return (false, $"端口号 {config.Port} 已被使用");
            }
            else if (CheckHostNameExists(config.SiteName))
            {
                return (false, $"域名绑定 {config.SiteName} 已存在");
            }
            return (true, "");
        }

        /// <summary>
        /// 检查指定名称的网站是否存在
        /// </summary>
        static bool CheckSiteNameExists(string siteName)
        {
            using (ServerManager serverManager = new ServerManager())
            {
                return serverManager.Sites.Any(s =>
                    s.Name.Equals(siteName, StringComparison.OrdinalIgnoreCase));
            }
        }

        /// <summary>
        /// 检查指定名称的程序池是否存在
        /// </summary>
        static bool CheckAppPoolExists(string appPoolName)
        {
            using (ServerManager serverManager = new ServerManager())
            {
                return serverManager.ApplicationPools.Any(p =>
                    p.Name.Equals(appPoolName, StringComparison.OrdinalIgnoreCase));
            }
        }

        /// <summary>
        /// 检查指定端口号是否被使用
        /// </summary>
        static bool CheckPortInUse(int port)
        {
            using (ServerManager serverManager = new ServerManager())
            {
                return serverManager.Sites.Any(s =>
                    s.Bindings.Any(b => GetPortFromBinding(b) == port));
            }
        }

        /// <summary>
        /// 从绑定信息中提取端口号
        /// </summary>
        static int GetPortFromBinding(Binding binding)
        {
            // 格式通常为 "IP:端口:主机名" 或 ":端口:主机名" 或 "端口"
            string[] parts = binding.BindingInformation.Split(':');
            if (parts.Length >= 2 && int.TryParse(parts[1], out int port))
            {
                return port;
            }
            return -1;
        }

        /// <summary>
        /// 检查指定域名绑定是否存在
        /// </summary>
        static bool CheckHostNameExists(string hostName)
        {
            using (ServerManager serverManager = new ServerManager())
            {
                return serverManager.Sites.Any(s =>
                    s.Bindings.Any(b => GetHostNameFromBinding(b).Equals(
                        hostName, StringComparison.OrdinalIgnoreCase)));
            }
        }

        /// <summary>
        /// 从绑定信息中提取主机名
        /// </summary>
        static string GetHostNameFromBinding(Binding binding)
        {
            // 格式通常为 "IP:端口:主机名" 或 ":端口:主机名"
            string[] parts = binding.BindingInformation.Split(':');
            if (parts.Length >= 3)
            {
                return parts[2];
            }
            return string.Empty;
        }


        //static void DeployWebsite(string sourcePath, string siteName, string physicalPath, int port, string hostName, string appPoolName)
        //{
        //    using (ServerManager serverManager = new ServerManager())
        //    {
        //        // 1. 检查并创建应用程序池
        //        bool appPoolExists = serverManager.ApplicationPools.Any(p =>
        //            p.Name.Equals(appPoolName, StringComparison.OrdinalIgnoreCase));

        //        if (!appPoolExists)
        //        {
        //            Console.WriteLine($"创建应用程序池: {appPoolName}");
        //            ApplicationPool appPool = serverManager.ApplicationPools.Add(appPoolName);
        //            appPool.ManagedRuntimeVersion = "v4.0"; // 设置为你的.NET版本
        //            appPool.AutoStart = true;
        //        }
        //        else
        //        {
        //            Console.WriteLine($"应用程序池已存在: {appPoolName}");
        //        }

        //        // 2. 检查并创建/更新网站
        //        Site existingSite = serverManager.Sites.FirstOrDefault(s =>
        //            s.Name.Equals(siteName, StringComparison.OrdinalIgnoreCase));

        //        if (existingSite == null)
        //        {
        //            // 检查端口是否被其他网站使用
        //            bool portInUse = serverManager.Sites.Any(s =>
        //                s.Bindings.Any(b => GetPortFromBinding(b) == port));

        //            if (portInUse)
        //            {
        //                throw new InvalidOperationException($"端口 {port} 已被其他网站使用!");
        //            }

        //            // 创建新网站
        //            Console.WriteLine($"创建新网站: {siteName}");
        //            string bindingInfo = string.IsNullOrEmpty(hostName)
        //                ? $"*:{port}:"
        //                : $"*:{port}:{hostName}";

        //            Site newSite = serverManager.Sites.Add(siteName, "http", bindingInfo, physicalPath);
        //            newSite.ApplicationDefaults.ApplicationPoolName = appPoolName;

        //            // 确保物理路径存在
        //            if (!Directory.Exists(physicalPath))
        //            {
        //                Console.WriteLine($"创建物理路径: {physicalPath}");
        //                Directory.CreateDirectory(physicalPath);
        //            }
        //        }
        //    }
        //}
    }
}
