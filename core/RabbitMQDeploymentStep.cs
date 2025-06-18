using Microsoft.Web.Administration;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace deploytool.core
{
    /// <summary>
    /// 安装rabbitmq
    /// </summary>
    internal class RabbitMQDeploymentStep : DeploymentStep
    {
        //private const string RABBITMQ_INSTALL_DIR = @"D:\Program Files\RabbitMQ";
        //private const string ERLANG_INSTALL_DIR = @"D:\Program Files\erl-25.3";

        private const string RABBITMQ_INSTALL_DIR = @"D:\temp1\RabbitMQ";
        private const string ERLANG_INSTALL_DIR = @"D:\temp1\erl";


        private string folderPath { get; set; }
        public RabbitMQDeploymentStep(DeploymentConfig config) : base(config)
        {
            config.DeployType = DeployType.Sit;
            StepName = "安装和配置RabbitMQ";

            string projectDirectory = Environment.CurrentDirectory;
            // 获取项目根目录
            //string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).FullName;

            folderPath = Path.Combine(projectDirectory, $"{Constant.RABBITMQ}");
        }

        public override async Task<bool> Execute(Action<DeploymentStep, string> progressCallback, CancellationToken ct)
        {
            /*
             关键修改说明
MSI 安装程序：使用 /quiet 和 /norestart 参数实现静默安装，避免弹出安装界面和重启提示。
SQL Server 安装：
使用 /ConfigurationFile 参数指定配置文件路径，实现完整静默安装
配置文件示例（需创建 ConfigurationFile.ini）：
ini
[OPTIONS]
IACCEPTSQLSERVERLICENSETERMS=1
ACTION=Install
FEATURES=SQLENGINE,SSMS
INSTANCENAME=MSSQLSERVER
SECURITYMODE=SQL
SAPWD=YourStrongPassword
SQLSYSADMINACCOUNTS=Administrator
TCPENABLED=1
NPENABLED=1


Erlang 和 RabbitMQ：使用 /S 参数进行静默安装，并指定安装路径。
PowerShell 命令：添加 -Quiet 或 -Confirm:$false 参数，避免交互式提示。
注意事项
对于复杂的安装程序（如 SQL Server），建议预先创建配置文件以确保所有选项正确设置。
某些安装程序可能需要额外的参数（如接受许可协议），请根据具体软件调整。
静默安装可能会隐藏错误信息，建议通过日志监控安装过程。
对于依赖用户输入的安装程序（如需要选择安装组件），可能需要更复杂的处理方式。
             */
            try
            {

                //e94a2cff-f4ef-44c3-8c83-52d14fe032a2
                var kk = Guid.NewGuid().ToString();
                bool erlExists = Directory.Exists(ERLANG_INSTALL_DIR);
                if (!erlExists)
                {
                    SetEnvironmentVariable("ERLANG_HOME", ERLANG_INSTALL_DIR);

                    string otpFile = Directory.GetFiles(folderPath, "otp_win64_*.exe").FirstOrDefault();

                    if (otpFile != null)
                    {
                        //RunProcess(otpFile, "/quiet", folderPath, true); //这样会打开软件提示安装
                        // 关键修改：使用/S参数静默安装并指定/D=路径
                        //RunProcess(otpFile, $"/S /D={ERLANG_INSTALL_DIR}", folderPath, true);
                        await RunProcessAsync(otpFile, $"/S /D={ERLANG_INSTALL_DIR}", folderPath);
                    }

                }
                //Log("安装RabbitMQ...");
                //progressCallback(this, $"创建文件夹路径 {Config.WebAppPath}...");

                bool mqExists = Directory.Exists(RABBITMQ_INSTALL_DIR);
                if (!mqExists)
                {
                    // 获取项目根目录
                    // 设置Erlang和RabbitMQ的安装路径环境变量
                    SetEnvironmentVariable("RABBITMQ_BASE", RABBITMQ_INSTALL_DIR);

                    string rabbitmqFile = Directory.GetFiles(folderPath, "rabbitmq-server-*.exe").FirstOrDefault();
                    if (rabbitmqFile != null)
                    {
                        //RunProcess(rabbitmqFile, "/quiet", folderPath, true);
                        // 使用/S参数进行静默安装
                        //RunProcess(rabbitmqFile, $"/S /D={RABBITMQ_INSTALL_DIR}", folderPath, true);
                        await RunProcessAsync(rabbitmqFile, $"/S /D={RABBITMQ_INSTALL_DIR}", folderPath);
                    }

                    // 新增: 启动RabbitMQ服务
                    // Log("启动RabbitMQ服务...");
                    //StartService("RabbitMQ");

                    //启动rabbitmq
                    await StartServiceAsync("RabbitMQ");
                    //Log("等待RabbitMQ服务启动...");
                    Thread.Sleep(5000); // 等待5秒让服务完全启动

                    // 新增: 验证服务状态
                    if (IsServiceRunning("RabbitMQ"))
                    {
                        // Log("RabbitMQ服务已成功启动");
                    }
                    else
                    {
                        //Log("警告: RabbitMQ服务未能成功启动");
                    }

                    //Log("重启服务器实例...");
                    // 实际应用中可能需要确认是否需要重启
                    // RestartComputer();

                    var y = Environment.GetEnvironmentVariable("ERLANG_HOME");

                    var root = Path.Combine(RABBITMQ_INSTALL_DIR, "rabbitmq_server-4.1.1", "sbin", "rabbitmq-plugins.bat");

                    // Log("创建RabbitMQ用户...");
                    //RunPowerShellCommand($"& $'{RABBITMQ_INSTALL_DIR}\\RabbitMQ Server\\rabbitmq_server-4.1.1\\sbin\\rabbitmqctl.bat' add_user core FwXuEZBpWaLNdVgatkrHAGqiS");
                    //RunPowerShellCommand($"& $'{RABBITMQ_INSTALL_DIR}\\Program Files\\RabbitMQ Server\\rabbitmq_server-4.1.1\\sbin\\rabbitmqctl.bat' set_permissions core \".*\" \".*\" \".*\"");

                    var res = ExecuteCmd($@"{RABBITMQ_INSTALL_DIR}\rabbitmq_server-4.1.1\sbin\rabbitmqctl.bat add_user core1 FwXuEZBpWaLNdVgatkrHAGqiS");
                    progressCallback(this, $"创建密码 {res}...");

                    res = ExecuteCmd($"{RABBITMQ_INSTALL_DIR}\\rabbitmq_server-4.1.1\\sbin\\rabbitmqctl.bat set_permissions core1 \".*\" \".*\" \".*\"");
                    progressCallback(this, $"创建权限 {res}...");
                    //enable:启用，disable:禁用
                    //Log("启用RabbitMQ管理插件...");

                    //启用管理界面插件
                    var rabbitMqCommand = $"{Path.Combine(RABBITMQ_INSTALL_DIR, "rabbitmq_server-4.1.1", "sbin", "rabbitmq-plugins.bat")} enable rabbitmq_management";
                    res = ExecuteCmd(rabbitMqCommand);
                    progressCallback(this, $"启用插件 {res}...");


                    //await RunPowerShellCommandAsync($"& '{root}' add_user core FwXuEZBpWaLNdVgatkrHAGqiS");
                    //await RunPowerShellCommandAsync($"& '{root}' set_permissions core \".*\" \".*\" \".*\"");

                    ////enable:启用，disable:禁用
                    ////Log("启用RabbitMQ管理插件...");
                    //await RunPowerShellCommandAsync($"& '{Path.Combine(RABBITMQ_INSTALL_DIR, "rabbitmq_server-4.1.1", "sbin", "rabbitmq-plugins.bat")}' enable rabbitmq_management");
                }

                //Log("步骤5完成");
                return true;
            }
            catch (Exception ex)
            {
                // Log($"步骤5出错: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 执行命令行
        /// </summary>
        /// <param name="optionalFilePath">命令</param>
        /// <returns>optionalFilePaths</returns>
        protected string ExecuteCmd(string optionalFilePath)
        {
            string[] optionalFilePaths = new string[] { optionalFilePath };
            return ExecuteCmd(optionalFilePaths);
        }
        /// <summary>
        /// 执行命令行
        /// </summary>
        /// <param name="optionalFilePaths">命令</param>
        /// <returns>返回结果</returns>
        protected string ExecuteCmd(string[] optionalFilePaths)
        {
            string strRst = "";
            try
            {
                //运行命令行
                Process p = new Process();
                // 设定程序名
                p.StartInfo.FileName = "cmd.exe";
                // 关闭Shell的使用
                p.StartInfo.UseShellExecute = false;
                // 重定向标准输入
                p.StartInfo.RedirectStandardInput = true;
                // 重定向标准输出
                p.StartInfo.RedirectStandardOutput = true;
                //重定向错误输出
                p.StartInfo.RedirectStandardError = true;
                // 设置不显示窗口
                p.StartInfo.CreateNoWindow = true;
                // 启动进程
                p.Start();
                for (int i = 0; i < optionalFilePaths.Length; i++)
                {
                    p.StandardInput.WriteLine(optionalFilePaths[i]);
                }
                p.StandardInput.WriteLine("exit");

                // 从输出流获取命令执行结果
                strRst = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                p.Close();
            }
            catch (IOException io)
            {
                Console.Write(io.Message.ToString());
            }
            return strRst;
        }

        // 异步设置环境变量
        static async Task SetEnvironmentVariableAsync(string variableName, string value)
        {
            //Log($"异步设置环境变量: {variableName} = {value}");

            await Task.Run(() =>
            {
                string currentValue = Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.Machine);

                if (currentValue != value)
                {
                    Environment.SetEnvironmentVariable(variableName, value, EnvironmentVariableTarget.Machine);

                    // 验证环境变量是否设置成功
                    string actualValue = Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.Machine);
                    if (actualValue != value)
                    {
                        //Log($"警告: 环境变量 {variableName} 设置失败，预期值: {value}，实际值: {actualValue ?? "null"}");
                    }
                    else
                    {
                        //Log($"环境变量 {variableName} 已成功设置为 {value}");
                    }
                }
                else
                {
                    //Log($"环境变量 {variableName} 已设置为 {value}，跳过设置");
                }
            });
        }

        // 异步运行PowerShell命令
        static async Task<int> RunPowerShellCommandAsync(string command)
        {
            //Log($"异步运行PowerShell: {command}");

            var tcs = new TaskCompletionSource<int>();

            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    //Arguments = $"-Command {command}",
                    Arguments = $"-Command \"{command}\"",
                    //UseShellExecute = true,

                    // 关闭Shell的使用
                    UseShellExecute = false,
                    // 重定向标准输入
                    RedirectStandardInput = true,
                    // 重定向标准输出
                    //重定向错误输出
                    RedirectStandardError = true,
                    // 设置不显示窗口
                    CreateNoWindow = true,
                    //Verb = "runas"
                },
                EnableRaisingEvents = true
            };

            process.Exited += (sender, args) =>
            {
                //Log($"PowerShell进程已退出，退出码: {process.ExitCode}");
                tcs.SetResult(process.ExitCode);
                process.Dispose();
            };

            try
            {
                process.Start();
                // 从输出流获取命令执行结果
                //var strRst = process.StandardOutput.ReadToEnd();
                //process.WaitForExit();
                //process.Close();
                //Log($"PowerShell进程已启动，ID: {process.Id}");
            }
            catch (Exception ex)
            {
                //Log($"启动PowerShell进程时出错: {ex.Message}");
                tcs.SetException(ex);
            }

            return await tcs.Task;
        }
        static async Task<(int exitCode, string standardOutput, string standardError)> RunPowerShellCommandAsync1(string command)
        {
            // 对命令中的双引号进行转义
            string escapedCommand = command.Replace("\"", "\"\"");

            var tcs = new TaskCompletionSource<(int, string, string)>();
            var outputBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-NoExit -Command \"& {{ {escapedCommand} }}\"",
                    UseShellExecute = false, // 必须为false才能重定向输出
                    CreateNoWindow = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    Verb = "" // 移除runas，仅在必要时使用
                },
                EnableRaisingEvents = true
            };

            process.OutputDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                    outputBuilder.AppendLine(args.Data);
            };

            process.ErrorDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                    errorBuilder.AppendLine(args.Data);
            };

            process.Exited += (sender, args) =>
            {
                tcs.SetResult((process.ExitCode, outputBuilder.ToString(), errorBuilder.ToString()));
            };

            try
            {
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
            process.WaitForExit();
            //process.Dispose();
            return await tcs.Task;
        }
        // 异步运行进程
        static async Task<int> RunProcessAsync(string fileName, string arguments, string workingDirectory)
        {
            //Log($"异步运行: {fileName} {arguments}");

            var tcs = new TaskCompletionSource<int>();

            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    WorkingDirectory = workingDirectory,
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

        // 异步启动服务
        static async Task StartServiceAsync(string serviceName)
        {
            try
            {
                using (ServiceController controller = new ServiceController(serviceName))
                {
                    if (controller.Status != ServiceControllerStatus.Running)
                    {
                        //Log($"正在启动服务 {serviceName}...");

                        // 使用Task.Run避免阻塞UI线程
                        await Task.Run(() =>
                        {
                            controller.Start();
                            controller.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
                        });

                        //Log($"服务 {serviceName} 已启动");
                    }
                    else
                    {
                        //Log($"服务 {serviceName} 已经在运行");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log($"启动服务时出错: {ex.Message}");
                throw;
            }
        }
        // 新增: 检查服务是否正在运行
        static bool IsServiceRunning(string serviceName)
        {
            try
            {
                using (ServiceController controller = new ServiceController(serviceName))
                {
                    return controller.Status == ServiceControllerStatus.Running;
                }
            }
            catch (Exception ex)
            {
                //Log($"检查服务状态时出错: {ex.Message}");
                return false;
            }
        }

        // 新增: 启动Windows服务
        static void StartService(string serviceName)
        {
            try
            {
                using (ServiceController controller = new ServiceController(serviceName))
                {
                    if (controller.Status != ServiceControllerStatus.Running)
                    {
                        controller.Start();
                        controller.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
                        //Log($"服务 {serviceName} 已启动");
                    }
                    else
                    {
                        //Log($"服务 {serviceName} 已经在运行");
                    }
                }
            }
            catch (Exception ex)
            {
                //Log($"启动服务时出错: {ex.Message}");
                throw;
            }
        }

        // 添加设置环境变量的辅助方法
        static void SetEnvironmentVariable(string variableName, string value)
        {
            //Log($"设置环境变量: {variableName} = {value}");
            Environment.SetEnvironmentVariable(variableName, value, EnvironmentVariableTarget.Machine);

            // 为当前进程设置环境变量，以便后续命令能立即使用
            Environment.SetEnvironmentVariable(variableName, value);
        }
        // 检查RabbitMQ是否已安装
        static bool IsRabbitMQInstalled()
        {
            //Log("检查RabbitMQ是否已安装...");

            try
            {
                // 检查RabbitMQ服务是否存在
                bool serviceExists = CheckServiceExists("RabbitMQ");
                if (serviceExists)
                {
                    // Log("发现RabbitMQ服务，已安装");
                    return true;
                }

                // 检查RabbitMQ安装目录是否存在
                bool directoryExists = Directory.Exists(RABBITMQ_INSTALL_DIR);
                if (directoryExists)
                {
                    // Log("发现RabbitMQ安装目录，可能已安装");

                    // 进一步检查目录中是否有可执行文件
                    string rabbitmqctlPath = Path.Combine(RABBITMQ_INSTALL_DIR, "sbin", "rabbitmqctl.bat");
                    if (File.Exists(rabbitmqctlPath))
                    {
                        //Log("发现RabbitMQ控制脚本，已安装");
                        return true;
                    }
                }

                // 检查Erlang是否安装
                bool erlangInstalled = CheckErlangInstalled();
                if (erlangInstalled)
                {
                    // Log("发现Erlang安装，但未找到RabbitMQ服务，可能未完整安装");
                }

                //Log("未发现RabbitMQ安装");
                return false;
            }
            catch (Exception ex)
            {
                //Log($"检查RabbitMQ安装状态时出错: {ex.Message}");
                return false;
            }
        }
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

        // 检查Erlang是否已安装
        static bool CheckErlangInstalled()
        {
            try
            {
                // 检查Erlang安装目录
                bool directoryExists = Directory.Exists(ERLANG_INSTALL_DIR);
                if (directoryExists)
                {
                    //Log("发现Erlang安装目录");

                    // 检查bin目录中的erl.exe
                    string erlExePath = Path.Combine(ERLANG_INSTALL_DIR, "bin", "erl.exe");
                    if (File.Exists(erlExePath))
                    {
                        // Log("发现Erlang执行文件，已安装");
                        return true;
                    }
                }

                // 检查注册表
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Ericsson\Erlang"))
                {
                    if (key != null)
                    {
                        //Log("在注册表中发现Erlang条目，已安装");
                        return true;
                    }
                }

                //Log("未发现Erlang安装");
                return false;
            }
            catch (Exception ex)
            {
                //Log($"检查Erlang安装状态时出错: {ex.Message}");
                return false;
            }
        }

        // 辅助方法: 运行PowerShell命令
        static void RunPowerShellCommand(string command)
        {
            //Log($"运行PowerShell: {command}");
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-Command {command}",
                UseShellExecute = true,
                Verb = "runas"
            };

            using (Process process = Process.Start(startInfo))
            {
                process.WaitForExit();
                // Log($"PowerShell进程退出码: {process.ExitCode}");
            }
        }
        // 辅助方法: 运行进程
        static void RunProcess(string fileName, string arguments, string workingDirectory, bool waitForExit = false)
        {
            //Log($"运行: {fileName} {arguments}");
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                WorkingDirectory = workingDirectory,
                UseShellExecute = true,
                Verb = "runas"
            };

            using (Process process = Process.Start(startInfo))
            {
                if (waitForExit)
                {
                    process.WaitForExit();
                    //Log($"进程退出码: {process.ExitCode}");
                }
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
