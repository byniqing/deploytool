using deploytool.core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace deploytool.form
{
    public partial class ConfigForm : Form
    {
        private int controlPadding = 10;

        private string folderPath = @"C:\A"; // 替换为实际的文件夹路径


        public ConfigForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 获取项目根目录
            string projectDirectory = Environment.CurrentDirectory;
            // 获取项目根目录
            //string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).FullName;

            folderPath = Path.Combine(projectDirectory, $"config/{Constant.WebSit}");
            DisplayFolders();
        }

        private void DisplayFolders()
        {
            try
            {
                // 清空现有控件
                panelWarp.Controls.Clear();

                // 获取指定文件夹下的所有子文件夹
                string[] subfolders = Directory.GetDirectories(folderPath);

                // 设置面板的自动滚动
                panelWarp.AutoScroll = true;

                // 计算每个GroupBox的位置和大小
                int yPosition = 10;
                int groupBoxWidth = panelWarp.Width - 30;
                int groupBoxHeight = 100; // 增加高度以容纳三个控件对

                // 定义每个控件对的宽度比例
                float controlWidthRatio = 0.3f; // 每个控件对占GroupBox宽度的30%
                //float controlWidthRatio = 0.25f; // 调整为25%以容纳按钮
                int controlWidth = (int)(groupBoxWidth * controlWidthRatio) - 20;
                int xPosition = 15;

                foreach (string subfolder in subfolders)
                {
                    // 创建GroupBox
                    GroupBox groupBox = new GroupBox
                    {
                        Location = new System.Drawing.Point(10, yPosition),
                        Size = new System.Drawing.Size(groupBoxWidth, groupBoxHeight),
                        Text = "站点信息"
                    };

                    // 获取文件夹名称
                    var folderName = new DirectoryInfo(subfolder).Name;

                    var folderName1 = new DirectoryInfo(subfolder).FullName;

                    // 创建并添加"站点"标签和文本框
                    CreateLabelTextBoxPair(groupBox, "站点:", folderName, ref xPosition, controlWidth);

                    // 创建并添加"程序池"标签和文本框
                    CreateLabelTextBoxPair(groupBox, "程序池:", folderName, ref xPosition, controlWidth);

                    var siteConfig = GetSitConfig();

                    var port = siteConfig?.FirstOrDefault(w => w.SiteName == folderName)?.Port;

                    // 创建并添加"端口"标签和文本框
                    TextBox portTextBox = CreateLabelTextBoxPair(groupBox, "端口:", port?.ToString(), ref xPosition, controlWidth);

                    // 创建并添加按钮
                    //CreateButton(groupBox, "操作", ref xPosition, controlWidth, portTextBox);

                    // 将GroupBox添加到面板
                    panelWarp.Controls.Add(groupBox);

                    // 更新下一个GroupBox的位置
                    yPosition += groupBoxHeight + 10;
                    xPosition = 15; // 重置x位置
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"发生错误: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private TextBox CreateLabelTextBoxPair(GroupBox container, string labelText, string initialValue, ref int xPosition, int controlWidth)
        {
            int labelHeight = 20;
            int textBoxHeight = 25;
            int verticalOffset = 30; // 控件对的垂直偏移

            // 创建标签
            Label label = new Label
            {
                Location = new System.Drawing.Point(xPosition, 20),
                Size = new System.Drawing.Size(controlWidth, labelHeight),
                Text = labelText
            };

            // 创建文本框
            TextBox textBox = new TextBox
            {
                Location = new System.Drawing.Point(xPosition, 45),
                Size = new System.Drawing.Size(controlWidth, textBoxHeight),
                ReadOnly = true, //只读的
                Text = initialValue
            };

            // 将控件添加到GroupBox
            container.Controls.Add(label);
            container.Controls.Add(textBox);

            // 更新x位置
            xPosition += controlWidth + controlPadding;
            return textBox; // 返回文本框引用，以便按钮可以访问
        }

        private void CreateButton(GroupBox container, string buttonText, ref int xPosition, int controlWidth, TextBox associatedTextBox)
        {
            int buttonHeight = 25;
            int verticalOffset = 45; // 按钮的垂直位置与文本框对齐

            // 创建按钮
            Button button = new Button
            {
                Location = new System.Drawing.Point(xPosition, verticalOffset),
                Size = new System.Drawing.Size(controlWidth, buttonHeight),
                Text = buttonText
            };

            // 为按钮添加点击事件处理程序
            button.Click += (sender, e) =>
            {
                // 获取关联的文本框值
                string portValue = associatedTextBox.Text;

                // 执行相应操作，例如：
                MessageBox.Show($"端口号 {portValue} 的操作已触发", "操作",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 这里可以添加你需要的处理逻辑
            };

            // 将按钮添加到GroupBox
            container.Controls.Add(button);

            // 更新x位置
            xPosition += controlWidth + controlPadding;

        }
        /// <summary>
        /// 重新加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            DisplayFolders();
        }

        /// <summary>
        /// 校验
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCheck_Click(object sender, EventArgs e)
        {
            var isCheck = true;
            var siteConfig = GetSitConfig();
            foreach (var sit in siteConfig)
            {
                var (check, messge) = WebSitCheck.CheckApp(sit);
                if (!check)
                {
                    isCheck = check;
                    MessageBox.Show(messge, "添加网站", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if(isCheck)
            {
                MessageBox.Show("校验通过", "添加网站", MessageBoxButtons.OK);
            }
        }

        private List<WebSitConfig> GetSitConfig()
        {
            return Main.SiteConfig;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            /*
             重置，就是删除已经存在的项目
             */
        }
    }
}
