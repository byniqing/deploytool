using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace deploytool.form
{
    public partial class Step_nginx : Form
    {
        public Step_nginx()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            /*
             安装nginx，有配置文件替换
             */
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            // 隐藏当前窗口
            this.Hide();

            // 创建并显示窗口
            Step_srv step = new Step_srv();

            // 当窗口关闭时，关闭当前应用程序
            step.FormClosed += (s, args) => this.Close();
            step.Show();
        }
    }
}
