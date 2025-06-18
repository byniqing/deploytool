using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace deploytool.core
{
    /// <summary>
    /// 安装net运行时环境
    /// </summary>
    internal class RuntimeDeploymentStep : DeploymentStep
    {
        private string dotnetDir { get; set; }
        public RuntimeDeploymentStep(DeploymentConfig config) : base(config)
        {
            config.DeployType = DeployType.Sit;
            StepName = "安装和配置net运行时环境";

            string projectDirectory = Environment.CurrentDirectory;
            // 获取项目根目录
            //string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).FullName;

            dotnetDir = Path.Combine(projectDirectory, $"{Constant.RUNTIME}");
        }

        public override async Task<bool> Execute(Action<DeploymentStep, string> progressCallback, CancellationToken ct)
        {
            try
            {
                //string dotnetDir = Path.Combine(RUNTIME_DIR, "dotnet");

                if (!Directory.Exists(dotnetDir))
                {
                    throw new DirectoryNotFoundException($"指定的目录不存在: {dotnetDir}");
                }

                if (Directory.Exists(dotnetDir))
                {
                    ////Log("安装.NET运行时...");
                    ///


                    foreach (string exe in Directory.GetFiles(dotnetDir, "aspnetcore-runtime-*.exe"))
                    {
                        try
                        {
                            string fileName = Path.GetFileName(exe);
                            string extension = Path.GetExtension(fileName).ToLower();

                            // 只处理exe和msi文件
                            if (extension != ".exe" && extension != ".msi")
                            {
                                Console.WriteLine($"跳过文件: {fileName} (不是exe或msi文件)");
                                continue;
                            }

                            Console.WriteLine($"准备执行文件: {fileName}");

                            // 根据文件类型设置不同的参数和执行方式
                            string arguments = "";
                            string processName = "";

                            if (extension == ".exe")
                            {
                                // 对于exe文件，使用/S静默安装并指定安装目录
                                arguments = $"/S";
                                processName = exe; // 直接执行exe文件
                            }
                            else // .msi
                            {
                                // 对于msi文件，使用msiexec命令行工具
                                //arguments = $"/i \"{filePath}\" /qn INSTALLDIR=\"{ERLANG_INSTALL_DIR}\"";
                                // 构建msiexec命令
                                arguments = $"/i \"{exe}\" /qn ";

                                processName = "msiexec.exe"; // 使用msiexec执行msi文件
                            }

                            // 执行文件
                            await RunProcessAsync(processName, arguments);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"执行文件 {Path.GetFileName(exe)} 时出错: {ex.Message}");
                        }
                    }
                }

                //Log("步骤4完成");
                return true;
            }
            catch (Exception ex)
            {
                //Log($"步骤4出错: {ex.Message}");
                throw;
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
