namespace ChronoFagTimer
{
    partial class TomatoForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TomatoForm));
            this.lblPomodoroTime = new System.Windows.Forms.Label();
            this.lblBreakTime = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.lblDownTitle = new System.Windows.Forms.Label();
            this.btnExtraTime = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // lblPomodoroTime
            // 
            this.lblPomodoroTime.AutoSize = true;
            this.lblPomodoroTime.Location = new System.Drawing.Point(56, 71);
            this.lblPomodoroTime.Name = "lblPomodoroTime";
            this.lblPomodoroTime.Size = new System.Drawing.Size(118, 17);
            this.lblPomodoroTime.TabIndex = 0;
            this.lblPomodoroTime.Text = "lblPomodoroTime";
            // 
            // lblBreakTime
            // 
            this.lblBreakTime.AutoSize = true;
            this.lblBreakTime.Location = new System.Drawing.Point(56, 103);
            this.lblBreakTime.Name = "lblBreakTime";
            this.lblBreakTime.Size = new System.Drawing.Size(90, 17);
            this.lblBreakTime.TabIndex = 1;
            this.lblBreakTime.Text = "lblBreakTime";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(56, 34);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(49, 17);
            this.lblTitle.TabIndex = 4;
            this.lblTitle.Text = "lblTitle";
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            // 
            // lblDownTitle
            // 
            this.lblDownTitle.AutoSize = true;
            this.lblDownTitle.Location = new System.Drawing.Point(56, 141);
            this.lblDownTitle.Name = "lblDownTitle";
            this.lblDownTitle.Size = new System.Drawing.Size(84, 17);
            this.lblDownTitle.TabIndex = 4;
            this.lblDownTitle.Text = "lblDownTitle";
            // 
            // btnExtraTime
            // 
            this.btnExtraTime.BackgroundImage = global::ChronoFagTimer.Properties.Resources.extra_time;
            this.btnExtraTime.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnExtraTime.FlatAppearance.BorderColor = System.Drawing.Color.Snow;
            this.btnExtraTime.FlatAppearance.BorderSize = 2;
            this.btnExtraTime.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExtraTime.Location = new System.Drawing.Point(316, 166);
            this.btnExtraTime.Name = "btnExtraTime";
            this.btnExtraTime.Size = new System.Drawing.Size(52, 53);
            this.btnExtraTime.TabIndex = 5;
            this.btnExtraTime.TabStop = false;
            this.btnExtraTime.UseVisualStyleBackColor = true;
            this.btnExtraTime.Click += new System.EventHandler(this.btnExtraTime_Click_1);
            this.btnExtraTime.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnExtraTime_Click);
            // 
            // TomatoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(389, 231);
            this.Controls.Add(this.btnExtraTime);
            this.Controls.Add(this.lblDownTitle);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblBreakTime);
            this.Controls.Add(this.lblPomodoroTime);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TomatoForm";
            this.ShowInTaskbar = false;
            this.Text = "Form1";
            this.TopMost = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblPomodoroTime;
        private System.Windows.Forms.Label lblBreakTime;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.Label lblDownTitle;
        private System.Windows.Forms.Button btnExtraTime;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}

