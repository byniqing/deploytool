using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deploytool.core
{
    public enum DeployType
    {
        /// <summary>
        /// iis
        /// </summary>
        IIS,

        /// <summary>
        /// 软件
        /// </summary>
        Soft,

        /// <summary>
        /// 站点
        /// </summary>
        Sit,

        /// <summary>
        /// windows服务
        /// </summary>
        Srv
    }

    /// <summary>
    /// 配置管理类
    /// </summary>
    public class DeploymentConfig
    {
        // IIS 配置
        public string SiteName { get; set; } = "MyAppSite";
        public int Port { get; set; } = 80;

        public DeployType DeployType { get; set; }

        // SQL Server 配置
        public string SqlServerPath { get; set; }
        public string DbBackupPath { get; set; }
        public string DbConnectionString { get; set; }
        public string DbName { get; set; } = "AppDatabase";

        // RabbitMQ 配置
        public string RabbitMQPath { get; set; }
        public string RabbitMQServiceName { get; set; } = "RabbitMQ";

        // Nginx 配置
        public string NginxConfigPath { get; set; }

        // 应用部署到服务器的路径
        public string WebAppPath { get; set; } = @"D:\iis\s";

        /// <summary>
        /// 
        /// </summary>
        public string ServiceAppPath { get; set; }

        // 保存/加载配置的方法
        public void Save(string filePath) => File.WriteAllText(filePath, JsonConvert.SerializeObject(this));
        public static DeploymentConfig Load(string filePath) =>
            JsonConvert.DeserializeObject<DeploymentConfig>(File.ReadAllText(filePath));
    }
}
