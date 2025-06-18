using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace deploytool
{
    public class CollapsiblePanel : UserControl
    {
        private Panel headerPanel;
        private Label titleLabel;
        private Label collapseLabel;
        private Panel contentPanel;
        private bool isCollapsed = false;
        private int expandedHeight;

        public CollapsiblePanel()
        {
            InitializeComponent();
            this.expandedHeight = this.Height;
        }

        private void InitializeComponent()
        {
            // Header Panel
            headerPanel = new Panel();
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 30;
            headerPanel.BackColor = Color.LightSteelBlue;
            headerPanel.Cursor = Cursors.Hand;

            // Title Label
            titleLabel = new Label();
            titleLabel.AutoSize = true;
            titleLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            titleLabel.Location = new Point(10, 7);
            titleLabel.Text = "折叠面板";

            // Collapse Indicator (▼/►)
            collapseLabel = new Label();
            collapseLabel.Text = "▼";
            collapseLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            collapseLabel.AutoSize = true;
            collapseLabel.Location = new Point(150, 7);

            // Content Panel
            contentPanel = new Panel();
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.BackColor = SystemColors.Control;

            // Add controls to header
            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(collapseLabel);

            // Add to main control
            this.Controls.Add(contentPanel);
            this.Controls.Add(headerPanel);

            // Event handlers
            headerPanel.Click += HeaderPanel_Click;
            collapseLabel.Click += HeaderPanel_Click;
            titleLabel.Click += HeaderPanel_Click;

            // Set default size
            this.Size = new Size(200, 200);
        }

        private void HeaderPanel_Click(object sender, EventArgs e)
        {
            ToggleCollapse();
        }

        public void ToggleCollapse()
        {
            isCollapsed = !isCollapsed;

            if (isCollapsed)
            {
                // 折叠：保存当前高度并设置为标题高度
                expandedHeight = this.Height;
                this.Height = headerPanel.Height;
                collapseLabel.Text = "►";
            }
            else
            {
                // 展开：恢复高度
                this.Height = expandedHeight;
                collapseLabel.Text = "▼";
            }
        }

        // 公共属性
        public string Title
        {
            get => titleLabel.Text;
            set => titleLabel.Text = value;
        }

        public Panel ContentPanel => contentPanel;
    }
}