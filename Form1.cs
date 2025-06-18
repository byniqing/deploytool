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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CreateCollapsiblePanel();
        }

        private void CreateCollapsiblePanel()
        {
            var collapsiblePanel = new CollapsiblePanel();
            collapsiblePanel.Title = "高级选项";
            collapsiblePanel.Size = new Size(250, 200);
            collapsiblePanel.Location = new Point(20, 20);

            // 添加内容到内容面板
            var content = collapsiblePanel.ContentPanel;
            content.Padding = new Padding(10);

            // 示例：添加一些控件
            var checkBox = new CheckBox { Text = "启用特性", Location = new Point(10, 10) };
            var textBox = new TextBox { Location = new Point(10, 40), Width = 150 };
            var button = new Button { Text = "保存", Location = new Point(10, 70) };

            content.Controls.Add(checkBox);
            content.Controls.Add(textBox);
            content.Controls.Add(button);

            this.Controls.Add(collapsiblePanel);
        }
    }
}
