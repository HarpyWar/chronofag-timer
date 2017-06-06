namespace PCTomatoTime
{
    partial class Form1
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
            this.lblPomodoroTime = new System.Windows.Forms.Label();
            this.lblBreakTime = new System.Windows.Forms.Label();
            this.lblPomodoroCounter = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblPomodoroTime
            // 
            this.lblPomodoroTime.AutoSize = true;
            this.lblPomodoroTime.Location = new System.Drawing.Point(56, 71);
            this.lblPomodoroTime.Name = "lblPomodoroTime";
            this.lblPomodoroTime.Size = new System.Drawing.Size(46, 17);
            this.lblPomodoroTime.TabIndex = 0;
            this.lblPomodoroTime.Text = "label1";
            // 
            // lblBreakTime
            // 
            this.lblBreakTime.AutoSize = true;
            this.lblBreakTime.Location = new System.Drawing.Point(236, 71);
            this.lblBreakTime.Name = "lblBreakTime";
            this.lblBreakTime.Size = new System.Drawing.Size(46, 17);
            this.lblBreakTime.TabIndex = 1;
            this.lblBreakTime.Text = "label2";
            // 
            // lblPomodoroCounter
            // 
            this.lblPomodoroCounter.AutoSize = true;
            this.lblPomodoroCounter.Location = new System.Drawing.Point(372, 159);
            this.lblPomodoroCounter.Name = "lblPomodoroCounter";
            this.lblPomodoroCounter.Size = new System.Drawing.Size(46, 17);
            this.lblPomodoroCounter.TabIndex = 3;
            this.lblPomodoroCounter.Text = "label2";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(477, 303);
            this.Controls.Add(this.lblPomodoroCounter);
            this.Controls.Add(this.lblBreakTime);
            this.Controls.Add(this.lblPomodoroTime);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.ShowInTaskbar = false;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblPomodoroTime;
        private System.Windows.Forms.Label lblBreakTime;
        private System.Windows.Forms.Label lblPomodoroCounter;
    }
}

