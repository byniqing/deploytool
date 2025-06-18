using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace deploytool.core
{
    /// <summary>
    ///  具体步骤实现示例
    /// </summary>
    public class SqlServerDeploymentStep : DeploymentStep
    {
        public SqlServerDeploymentStep(DeploymentConfig config) : base(config)
        {
            config.DeployType = DeployType.Soft;
            StepName = "SQL Server 部署";
        }

        public override async Task<bool> Execute(Action<DeploymentStep, string> progressCallback, CancellationToken ct)
        {
            try
            {
                progressCallback(this, "正在安装 SQL Server...");

                //1：校验sql是否安装，没安装，是否有安装包，安装了，提示用户

                var isMssql = SoftWareManager.IsSoftwareInstalled("SQL Server");
                if (isMssql)
                {
                    progressCallback(this, "服务器已经安装了 SQL Server...，忽略");
                    IsUpdate = false;
                }

                //// 使用静默安装参数
                //var process = Process.Start("setup.exe", $"/ConfigurationFile=Configuration.ini /QS");
                //process.WaitForExit();
                ////await process.WaitForExitAsync(ct);

                //if (process.ExitCode != 0)
                //    throw new Exception($"安装失败，退出代码: {process.ExitCode}");

                //progressCallback(this, "正在还原数据库...");
                //await RestoreDatabase(progressCallback);

                IsExecuted = true;
                return true;
            }
            catch (Exception ex)
            {
                progressCallback(this, $"错误: {ex.Message}");
                return false;
            }
        }

        private async Task RestoreDatabase(Action<DeploymentStep, string> progressCallback)
        {
            using (var connection = new SqlConnection(Config.DbConnectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(
                    $"RESTORE DATABASE {Config.DbName} FROM DISK = '{Config.DbBackupPath}'",
                    connection);
                await command.ExecuteNonQueryAsync();
            }
        }

        public override async Task Rollback(Action<DeploymentStep, string> progressCallback)
        {
            progressCallback(this, "卸载 SQL Server...");
            // 执行卸载逻辑
            await Task.Delay(1000); // 模拟卸载

            progressCallback(this, "删除数据库...");
            using (var connection = new SqlConnection(Config.DbConnectionString))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(
                    $"DROP DATABASE IF EXISTS {Config.DbName}",
                    connection);
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
