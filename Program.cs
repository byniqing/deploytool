using deploytool.form;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace deploytool
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //// 对是否具有管理员权限进行检查
            //bool isAdmin = IsAdministrator();

            //// 根据权限状态进行相应处理
            //if (isAdmin)
            //{
            //    MessageBox.Show("程序已获取管理员权限", "权限检查",
            //        MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}
            //else
            //{
            //    MessageBox.Show("请以管理员权限运行软件", "权限检查",
            //        MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Step_mq());
        }

        // 该方法用于判断当前用户是否为管理员
        private static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
