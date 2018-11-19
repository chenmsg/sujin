namespace WindowsFormsApplication1
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnWrite = new System.Windows.Forms.Button();
            this.pgbWrite = new System.Windows.Forms.ProgressBar();
            this.lblWriteStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnWrite
            // 
            this.btnWrite.Location = new System.Drawing.Point(166, 60);
            this.btnWrite.Name = "btnWrite";
            this.btnWrite.Size = new System.Drawing.Size(75, 23);
            this.btnWrite.TabIndex = 0;
            this.btnWrite.Text = "button1";
            this.btnWrite.UseVisualStyleBackColor = true;
            this.btnWrite.Click += new System.EventHandler(this.btnWrite_Click);
            // 
            // pgbWrite
            // 
            this.pgbWrite.Location = new System.Drawing.Point(51, 98);
            this.pgbWrite.Name = "pgbWrite";
            this.pgbWrite.Size = new System.Drawing.Size(258, 23);
            this.pgbWrite.TabIndex = 1;
            // 
            // lblWriteStatus
            // 
            this.lblWriteStatus.AutoSize = true;
            this.lblWriteStatus.Location = new System.Drawing.Point(327, 109);
            this.lblWriteStatus.Name = "lblWriteStatus";
            this.lblWriteStatus.Size = new System.Drawing.Size(41, 12);
            this.lblWriteStatus.TabIndex = 2;
            this.lblWriteStatus.Text = "label1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(468, 217);
            this.Controls.Add(this.lblWriteStatus);
            this.Controls.Add(this.pgbWrite);
            this.Controls.Add(this.btnWrite);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnWrite;
        private System.Windows.Forms.ProgressBar pgbWrite;
        private System.Windows.Forms.Label lblWriteStatus;
    }
}

