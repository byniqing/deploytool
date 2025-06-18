using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deploytool.core
{
    public class WebSitConfig
    {
        // IIS 配置
        public string SiteName { get; set; } = "MyAppSite";

        /// <summary>
        /// 程序池名称
        /// </summary>
        public string AppPoolName { get; set; }

        /// <summary>
        /// 端口号
        /// </summary>
        public int Port { get; set; } = 80;
    }
}
