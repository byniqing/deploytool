using deploytool.form;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace deploytool
{
    public partial class Run : Form
    {
        public Run()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            // 隐藏当前窗口
            this.Hide();

            // 创建并显示窗口
            Step_iis step = new Step_iis();

            // 当窗口B关闭时，关闭当前应用程序
            step.FormClosed += (s, args) => this.Close();
            step.Show();
        }
    }
}
