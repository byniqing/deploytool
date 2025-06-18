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
    public partial class Step_srv : Form
    {
        public Step_srv()
        {
            InitializeComponent();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            // 隐藏当前窗口
            this.Hide();

            // 创建并显示窗口
            Step_5 step = new Step_5();

            // 当窗口关闭时，关闭当前应用程序
            step.FormClosed += (s, args) => this.Close();
            step.Show();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            /*
             安装windows服务
             */
        }
    }
}
