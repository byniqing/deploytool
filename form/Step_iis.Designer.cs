namespace deploytool.form
{
    partial class Step_iis
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnStart = new System.Windows.Forms.Button();
            this.gbProgress = new System.Windows.Forms.GroupBox();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.btnNext = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.gbProgress.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(317, 344);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(150, 23);
            this.btnStart.TabIndex = 3;
            this.btnStart.Text = "安装";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // gbProgress
            // 
            this.gbProgress.Controls.Add(this.txtLog);
            this.gbProgress.Location = new System.Drawing.Point(36, 84);
            this.gbProgress.Name = "gbProgress";
            this.gbProgress.Size = new System.Drawing.Size(609, 166);
            this.gbProgress.TabIndex = 4;
            this.gbProgress.TabStop = false;
            this.gbProgress.Text = "部署进度";
            // 
            // txtLog
            // 
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Location = new System.Drawing.Point(3, 17);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(603, 146);
            this.txtLog.TabIndex = 0;
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(491, 344);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(124, 23);
            this.btnNext.TabIndex = 5;
            this.btnNext.Text = "下一步";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("宋体", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(187, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(228, 60);
            this.label1.TabIndex = 6;
            this.label1.Text = "安装IIS";
            // 
            // Step_1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.gbProgress);
            this.Controls.Add(this.btnStart);
            this.Name = "Step_1";
            this.Text = "Step_1";
            this.gbProgress.ResumeLayout(false);
            this.gbProgress.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.GroupBox gbProgress;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Label label1;
    }
}