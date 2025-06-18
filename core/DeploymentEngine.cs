using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace deploytool.core
{
    /// <summary>
    /// 部署引擎核心
    /// </summary>
    public class DeploymentEngine
    {
        private readonly List<DeploymentStep> _steps = new List<DeploymentStep>();
        private readonly DeploymentConfig _config;


        //private readonly DeploymentStep _stepsover = new DeploymentStep();

        /// <summary>
        /// 站点配置信息
        /// </summary>
        public static List<WebSitConfig> SiteConfig
        {
            get
            {
                return Main.SiteConfig;
            }
        }

        public bool IsPaused { get; private set; }
        private PauseTokenSource _pts = new PauseTokenSource();

        string projectDirectory = Path.Combine(Environment.CurrentDirectory, "config");

        public DeploymentEngine(DeploymentConfig config)
        {
            _config = config;
            //InitializeSteps();
        }

        public string[] GerAllWebSitFolder()
        {
            // 获取项目路径
            var webSit = Path.Combine(projectDirectory, Constant.WebSit);
            string[] folders = Directory.GetDirectories(webSit);
            return folders;
        }
        private void InitializeSteps()
        {
            _steps.Add(new IISDeploymentStep(_config)); //IIS
            _steps.Add(new SqlServerDeploymentStep(_config));

            /*
             如果文件夹项目数量。和sitconfig配置文件数量不一样，怎么处理？
             */
            string[] sitFolders = GerAllWebSitFolder();

            foreach (string subfolder in sitFolders)
            {
                var info = new DirectoryInfo(subfolder);
                var name = info.Name;
                // 获取文件夹名称路径
                var full = info.FullName;
                var c = new DeploymentConfig
                {
                    SiteName = name,
                    Port = SiteConfig.FirstOrDefault(f => f.SiteName == name).Port,
                    ServiceAppPath = full,
                    //DeployType = DeployType.Sit
                };
                _steps.Add(new WebSitDeploymentStep(c));
            }


            //SiteConfig.ForEach(f =>
            //{
            //    var c = new DeploymentConfig
            //    {
            //        SiteName = f.SiteName,
            //        Port = f.Port,
            //        // DeployType = DeployType.Sit
            //    };
            //    _steps.Add(new WebSitDeploymentStep(c));
            //});


            //_steps.Add(new RabbitMQInstallStep(_config));
            //_steps.Add(new NginxConfigurationStep(_config));
            //_steps.Add(new WindowsServiceInstallStep(_config));
            //_steps.Add(new AppDeploymentStep(_config));
            //_steps.Add(new VerificationStep(_config));
        }

        public void TogglePause()
        {
            IsPaused = !IsPaused;
            if (IsPaused) _pts.Pause();
            else _pts.Resume();
        }

        public async Task StartDeployment(Action<DeploymentStep, string> progressCallback, CancellationToken ct)
        {
            var executedSteps = new Stack<DeploymentStep>();

            try
            {
                var tip = false;
                foreach (var step in _steps)
                {
                    ct.ThrowIfCancellationRequested();
                    await _pts.Token.WaitWhilePausedAsync();
                    if (!tip)
                    {
                        tip = true;
                        progressCallback(step, "开始执行");
                    }
                    //progressCallback(step, step.Config.DeployType.ToString());
                    bool success = await step.Execute(progressCallback, ct);

                    if (success)
                    {
                        executedSteps.Push(step);
                        if (step.IsUpdate)
                        {
                            if (step.Config.DeployType == DeployType.Sit
                                || step.Config.DeployType == DeployType.Srv)
                            {
                                var host = $"{step.Config.SiteName}:{step.Config.Port}";
                                progressCallback(step, $"{host}部署成功...");
                            }
                            else if (step.Config.DeployType == DeployType.Soft)
                            {
                                progressCallback(step, $"{step.Config.SiteName}安装成功");
                            }
                            else
                            {
                                progressCallback(step, "执行成功");
                            }
                        }
                    }
                    else
                    {
                        progressCallback(step, "执行失败，开始回滚");
                        //await Rollback(executedSteps, progressCallback);
                        return;
                    }
                }
                //executedSteps.Push(new DeploymentStep());
                //progressCallback(step, "执行成功");
                progressCallback(null, "所有步骤执行完成！");

                //开始检查网站是否都启动了


            }
            catch (OperationCanceledException)
            {
                progressCallback(null, "部署已被取消");
                //await Rollback(executedSteps, progressCallback);
            }
        }

        public async Task Rollback(Action<DeploymentStep, string> progressCallback)
        {
            await Rollback(new Stack<DeploymentStep>(_steps.Where(s => s.IsExecuted)), progressCallback);
        }

        private async Task Rollback(Stack<DeploymentStep> executedSteps, Action<DeploymentStep, string> progressCallback)
        {
            while (executedSteps.Count > 0)
            {
                var step = executedSteps.Pop();
                progressCallback(step, "开始回滚");
                await step.Rollback(progressCallback);
                progressCallback(step, "回滚完成");
            }
        }

        public async Task StartDeployment1(Action<DeploymentStep, string> progressCallback, DeploymentStep step, CancellationToken ct)
        {
            var executedSteps = new Stack<DeploymentStep>();

            try
            {
                var tip = false;
                // foreach (var step in _steps)
                //{
                ct.ThrowIfCancellationRequested();
                //await _pts.Token.WaitWhilePausedAsync();
                if (!tip)
                {
                    tip = true;
                    progressCallback(step, "开始执行");
                }
                //progressCallback(step, step.Config.DeployType.ToString());
                bool success = await step.Execute(progressCallback, ct);

                if (success)
                {
                    executedSteps.Push(step);
                    if (step.IsUpdate)
                    {
                        if (step.Config.DeployType == DeployType.Sit
                            || step.Config.DeployType == DeployType.Srv)
                        {
                            var host = $"{step.Config.SiteName}:{step.Config.Port}";
                            progressCallback(step, $"{host}部署成功...");
                        }
                        else if (step.Config.DeployType == DeployType.Soft)
                        {
                            progressCallback(step, $"{step.Config.SiteName}安装成功");
                        }
                        else
                        {
                            progressCallback(step, "执行成功");
                        }
                    }
                }
                else
                {
                    progressCallback(step, "执行失败，开始回滚");
                    //await Rollback(executedSteps, progressCallback);
                    return;
                }
                //}
                //executedSteps.Push(new DeploymentStep());
                //progressCallback(step, "执行成功");
                progressCallback(null, "所有步骤执行完成！");

                //开始检查网站是否都启动了


            }
            catch (OperationCanceledException)
            {
                progressCallback(null, "部署已被取消");
                //await Rollback(executedSteps, progressCallback);
            }
        }
    }
}
