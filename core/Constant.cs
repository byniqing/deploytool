using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deploytool.core
{
    public class Constant
    {
        public static string WebSit = "4-website";



        /// <summary>
        /// 根目录
        /// </summary>
        public const string INSTALL_ROOT = @"config";
        /// <summary>
        /// NET环境运行时
        /// </summary>
        public const string RUNTIME = @"config\1-runtime";

        /// <summary>
        /// sql server 安装包
        /// </summary>
        public const string MSSQL = @"config\2-mssql";

        /// <summary>
        /// 要还原的db
        /// </summary>
        public const string DB = @"config\3-db";

        /// <summary>
        /// 要执行的sql
        /// </summary>
        public const string SQL = @"config\4-sql";

        /// <summary>
        /// 要按照的rabbitmq
        /// </summary>
        public const string RABBITMQ = @"config\5-rabbitmq";

        /// <summary>
        /// 要安装的nginx
        /// </summary>
        public const string NGINX = @"config\6-nginx";

        /// <summary>
        /// 要安装的windows服务
        /// </summary>
        public const string SRV = @"config\7-srv";

        /// <summary>
        /// 证书
        /// </summary>
        public const string CERT = @"config\8-cert";

        /// <summary>
        /// 要发布的网站
        /// </summary>
        public const string SITE = @"config\9-site";

        /// <summary>
        /// 要发布的网站的配置
        /// </summary>
        public const string SITE_CONFIG = @"config\9-site\site.json";

        /// <summary>
        /// 其他设置文件
        /// </summary>
        public const string SETTING = @"config\10-setting";





        public const string RUNTIME_DIR = @"C:\_INS\02-Runtime";
        public const string RABBITMQ_DIR = @"C:\_INS\03-RabbitMQ";
        public const string NGINX_DIR = @"C:\_INS\04-NGINX";
        public const string SETTINGS_DIR = @"C:\_INS\05-Settings";
        public const string PNG_DIR = @"C:\png";
        public const string SQL_BACKUP_DIR = @"C:\Program Files\Microsoft SQL Server\MSSQL13.MSSQLSERVER\MSSQL\Backup";
        public const string HOSTS_FILE = @"C:\Windows\System32\drivers\etc\hosts";
        public const string APPLICATION_HOST_CONFIG = @"C:\Windows\System32\inetsrv\Config\applicationHost.config";

    }
}
