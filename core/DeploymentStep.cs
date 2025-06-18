using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace deploytool.core
{
    /// <summary>
    /// 部署步骤基类
    /// </summary>
    public abstract class DeploymentStep
    {
        /// <summary>
        /// 是否安装了
        /// </summary>
        public bool IsUpdate { get; set; } = true;
        public string StepName { get; protected set; }
        public bool IsExecuted { get; protected set; }
        public DeploymentConfig Config { get; }

        protected DeploymentStep(DeploymentConfig config)
        {
            Config = config;
        }

        public abstract Task<bool> Execute(Action<DeploymentStep, string> progressCallback, CancellationToken ct);
        public abstract Task Rollback(Action<DeploymentStep, string> progressCallback);
    }
}
