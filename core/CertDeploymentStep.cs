using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace deploytool.core
{
    /// <summary>
    /// 安装证书
    /// </summary>
    internal class CertDeploymentStep : DeploymentStep
    {
        private string certPath { get; set; }
        public CertDeploymentStep(DeploymentConfig config) : base(config)
        {
            config.DeployType = DeployType.Sit;
            StepName = "安装证书";

            string projectDirectory = Environment.CurrentDirectory;
            // 获取项目根目录
            //string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).FullName;

            certPath = Path.Combine(projectDirectory, $"{Constant.CERT}");
        }

        public override async Task<bool> Execute(Action<DeploymentStep, string> progressCallback, CancellationToken ct)
        {
            try
            {
                // 证书和密码文件路径
                //string pfxFilePath = Path.Combine(dotnetDir, "certificate.pfx");// @"C:\path\to\your\certificate.pfx";
                //string passwordFilePath = @"C:\path\to\your\pwd.txt";

                //string certificateDirectory = @"C:\path\to\your\";
                string passwordFilePath = Path.Combine(certPath, "pwd.txt");

                // 查找目录中的第一个.pfx文件
                string[] pfxFiles = Directory.GetFiles(certPath, "*.pfx");
                if (pfxFiles.Length == 0)
                {
                    Console.WriteLine("错误：在指定目录中未找到PFX证书文件。");
                    return false;
                }

                if (pfxFiles.Length > 1)
                {
                    Console.WriteLine("警告：在指定目录中找到多个PFX证书文件，将使用第一个文件。");
                }

                string pfxFilePath = pfxFiles[0];
                Console.WriteLine($"找到证书文件：{Path.GetFileName(pfxFilePath)}");

                // 从文件读取密码
                string pfxPassword = File.ReadAllText(passwordFilePath).Trim();

                // 加载PFX证书
                X509Certificate2 certificate = new X509Certificate2(
                    pfxFilePath,
                    pfxPassword,
                    X509KeyStorageFlags.MachineKeySet |
                    X509KeyStorageFlags.PersistKeySet |
                    X509KeyStorageFlags.Exportable);

                // 打开受信任的根证书存储区（需要管理员权限）
                //using (X509Store store = new X509Store(StoreName.Root, StoreLocation.LocalMachine))
                // 打开WebHosting证书存储区（需要管理员权限）
                using (X509Store store = new X509Store("WebHosting", StoreLocation.LocalMachine))
                {
                    store.Open(OpenFlags.ReadWrite);

                    // 检查证书是否已存在
                    bool certificateExists = false;
                    foreach (var cert in store.Certificates)
                    {
                        if (cert.Thumbprint == certificate.Thumbprint)
                        {
                            certificateExists = true;
                            //Console.WriteLine("证书已存在于受信任的根存储区中。");
                            Console.WriteLine("证书已存在于WebHosting存储区中。");
                            break;
                        }
                    }

                    // 如果证书不存在，则添加它
                    if (!certificateExists)
                    {
                        store.Add(certificate);
                        //Console.WriteLine("证书已成功添加到受信任的根存储区。");
                        Console.WriteLine("证书已成功添加到WebHosting存储区。");
                    }

                    store.Close();
                }

                //Log("步骤4完成");
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"文件未找到: {ex.FileName}");
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("访问被拒绝。请确保你有读取密码文件和修改证书存储的权限。");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"操作失败: {ex.Message}");
                Console.WriteLine("请确保你有管理员权限，证书和密码文件路径正确，并且密码有效。");
                return false;
            }
            return true;
        }

        public override Task Rollback(Action<DeploymentStep, string> progressCallback)
        {
            throw new NotImplementedException();
        }
    }
}
