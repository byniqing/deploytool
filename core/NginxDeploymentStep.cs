using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace deploytool.core
{
    /// <summary>
    /// 安装nginx
    /// </summary>
    internal class NginxDeploymentStep : DeploymentStep
    {
        private string dotnetDir { get; set; }
        private const string NSSM_DOWNLOAD_URL = "https://nssm.cc/release/nssm-2.24.zip";
        private const string NSSM_EXE_PATH = @"C:\nssm-2.24\win64\nssm.exe"; // 64位系统路径
        public NginxDeploymentStep(DeploymentConfig config) : base(config)
        {
            config.DeployType = DeployType.Sit;
            StepName = "安装nginx";

            string projectDirectory = Environment.CurrentDirectory;
            // 获取项目根目录
            //string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).FullName;

            dotnetDir = Path.Combine(projectDirectory, $"{Constant.NGINX}");
        }

        public override async Task<bool> Execute(Action<DeploymentStep, string> progressCallback, CancellationToken ct)
        {
            //string sourcePath = @"C:\source";  // 替换成实际的源目录路径（A路径）
            string targetPath = @"D:\destination";  // 替换成实际的目标目录路径（B路径）

            try
            {
                // 检查源目录是否存在
                if (!Directory.Exists(dotnetDir))
                {
                    throw new DirectoryNotFoundException($"源目录不存在: {dotnetDir}");
                }

                DirectoryCopy(new DirectoryInfo(dotnetDir), new DirectoryInfo(targetPath));
                Console.WriteLine("目录复制成功！");

                /*
                 nginx.conf是提前赋值好吗？
                如果动态配置了域名，那这里也要动态修改
                是先配置好。在开始安装的，所以配置好后。这里要修改
                 */

                //用nssm 安装服务的方式
                //https://nssm.cc/download
                //https://nginx.org/en/download.html

                // 2. 确保NSSM工具可用
                EnsureNssmInstalled();

                // 3. 使用NSSM安装Nginx服务
                string nginxExePath = Path.Combine(targetPath, "nginx.exe");
                string nginxStopExePath = Path.Combine(targetPath, "nginx.exe");
                string serviceName = "NginxService";

                InstallNginxService(NSSM_EXE_PATH, serviceName, nginxExePath, nginxStopExePath);

                Console.WriteLine("Nginx服务安装成功");

                // 4. 启动Nginx服务
                StartService(serviceName);

                Console.WriteLine("Nginx服务已启动");


            }
            catch (Exception ex)
            {
                Console.WriteLine($"复制过程中出现错误: {ex.Message}");
            }
            return true;
        }

        private static void DirectoryCopy(DirectoryInfo source, DirectoryInfo target)
        {
            foreach (DirectoryInfo dir in source.GetDirectories())
            {
                DirectoryCopy(dir, target.CreateSubdirectory(dir.Name));
            }

            foreach (FileInfo file in source.GetFiles())
            {
                file.CopyTo(Path.Combine(target.FullName, file.Name), true);
            }
        }
        // 确保NSSM工具存在
        private static void EnsureNssmInstalled()
        {
            if (!File.Exists(NSSM_EXE_PATH))
            {
                Console.WriteLine("NSSM工具不存在，正在下载...");

                // 创建临时目录
                string tempDir = Path.Combine(Path.GetTempPath(), "nssm_temp");
                Directory.CreateDirectory(tempDir);

                try
                {
                    // 下载NSSM压缩包
                    string zipPath = Path.Combine(tempDir, "nssm.zip");
                    using (WebClient client = new WebClient())
                    {
                        client.DownloadFile(NSSM_DOWNLOAD_URL, zipPath);
                    }

                    // 解压（需要引用System.IO.Compression.FileSystem）
                    System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, tempDir);

                    // 复制nssm.exe到目标位置
                    string extractedNssmPath = Path.Combine(tempDir, "nssm-2.24", "win64", "nssm.exe");
                    string targetDir = Path.GetDirectoryName(NSSM_EXE_PATH);

                    if (!Directory.Exists(targetDir))
                        Directory.CreateDirectory(targetDir);

                    File.Copy(extractedNssmPath, NSSM_EXE_PATH, true);

                    Console.WriteLine("NSSM工具安装完成");
                }
                catch (Exception ex)
                {
                    throw new Exception("无法安装NSSM工具，请手动下载: https://nssm.cc/download", ex);
                }
                finally
                {
                    // 清理临时文件
                    Directory.Delete(tempDir, true);
                }
            }
        }

        // 使用NSSM安装服务
        private static void InstallNginxService(string nssmPath, string serviceName, string startExePath, string stopExePath)
        {
            // 检查服务是否已存在
            if (IsServiceExists(serviceName))
            {
                Console.WriteLine($"服务 {serviceName} 已存在，正在卸载...");
                ExecuteCommand(nssmPath, $"remove {serviceName} confirm");
            }

            // 安装服务
            ExecuteCommand(nssmPath, $"install {serviceName} \"{startExePath}\"");

            // 设置停止命令（nginx -s stop）
            ExecuteCommand(nssmPath, $"set {serviceName} AppStopMethodSkip 6");
            ExecuteCommand(nssmPath, $"set {serviceName} AppStopMethodConsole 1");
            ExecuteCommand(nssmPath, $"set {serviceName} AppStopMethodWindow 2");
            ExecuteCommand(nssmPath, $"set {serviceName} AppStopMethodThreads 3");
            ExecuteCommand(nssmPath, $"set {serviceName} AppStopCommandLine \"-s stop\"");

            // 设置自动启动
            ExecuteCommand(nssmPath, $"set {serviceName} Start SERVICE_AUTO_START");

            // 设置重启策略
            ExecuteCommand(nssmPath, $"set {serviceName} AppRestartDelay 1000");
            ExecuteCommand(nssmPath, $"set {serviceName} AppThrottle 1000");

            Console.WriteLine($"Nginx服务 {serviceName} 配置完成");
        }

        // 检查服务是否存在
        private static bool IsServiceExists(string serviceName)
        {
            try
            {
                using (ServiceController controller = new ServiceController(serviceName))
                {
                    // 尝试获取服务状态，若不存在会抛出异常
                    ServiceControllerStatus status = controller.Status;
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        // 启动服务
        private static void StartService(string serviceName)
        {
            try
            {
                using (ServiceController controller = new ServiceController(serviceName))
                {
                    if (controller.Status != ServiceControllerStatus.Running)
                    {
                        controller.Start();
                        controller.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"无法启动服务 {serviceName}: {ex.Message}", ex);
            }
        }

        // 执行命令行命令
        private static void ExecuteCommand(string fileName, string arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (Process process = new Process { StartInfo = startInfo })
            {
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    throw new Exception($"命令执行失败: {fileName} {arguments}\n错误: {error}");
                }
            }
        }
        static async Task<int> RunProcessAsync(string fileName, string arguments)
        {
            //Log($"异步运行: {fileName} {arguments}");

            var tcs = new TaskCompletionSource<int>();

            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    UseShellExecute = true,
                    Verb = "runas"
                },
                EnableRaisingEvents = true
            };

            process.Exited += (sender, args) =>
            {
                //Log($"进程 {fileName} 已退出，退出码: {process.ExitCode}");
                tcs.SetResult(process.ExitCode);
                process.Dispose();
            };

            try
            {
                process.Start();
                //Log($"进程 {fileName} 已启动，ID: {process.Id}");
            }
            catch (Exception ex)
            {
                //Log($"启动进程时出错: {ex.Message}");
                tcs.SetException(ex);
            }

            return await tcs.Task;
        }
        public override Task Rollback(Action<DeploymentStep, string> progressCallback)
        {
            throw new NotImplementedException();
        }
    }
}
