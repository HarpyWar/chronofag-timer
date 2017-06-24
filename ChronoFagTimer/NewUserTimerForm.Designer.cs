namespace ChronoFagTimer
{
    partial class NewUserTimerForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.txtHours = new System.Windows.Forms.NumericUpDown();
            this.txtMinutes = new System.Windows.Forms.NumericUpDown();
            this.txtSeconds = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.txtHours)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMinutes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSeconds)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Caption";
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(9, 139);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(196, 45);
            this.btnStart.TabIndex = 4;
            this.btnStart.Text = "Start Timer";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // txtTitle
            // 
            this.txtTitle.Location = new System.Drawing.Point(10, 36);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(195, 22);
            this.txtTitle.TabIndex = 0;
            this.txtTitle.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTitle_KeyDown);
            // 
            // txtHours
            // 
            this.txtHours.Location = new System.Drawing.Point(11, 91);
            this.txtHours.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.txtHours.Name = "txtHours";
            this.txtHours.Size = new System.Drawing.Size(53, 22);
            this.txtHours.TabIndex = 1;
            this.txtHours.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTitle_KeyDown);
            // 
            // txtMinutes
            // 
            this.txtMinutes.Location = new System.Drawing.Point(82, 91);
            this.txtMinutes.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.txtMinutes.Name = "txtMinutes";
            this.txtMinutes.Size = new System.Drawing.Size(53, 22);
            this.txtMinutes.TabIndex = 2;
            this.txtMinutes.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.txtMinutes.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTitle_KeyDown);
            // 
            // txtSeconds
            // 
            this.txtSeconds.Location = new System.Drawing.Point(153, 91);
            this.txtSeconds.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.txtSeconds.Name = "txtSeconds";
            this.txtSeconds.Size = new System.Drawing.Size(53, 22);
            this.txtSeconds.TabIndex = 3;
            this.txtSeconds.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTitle_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(68, 91);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(12, 17);
            this.label2.TabIndex = 7;
            this.label2.Text = ":";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(138, 91);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(12, 17);
            this.label3.TabIndex = 8;
            this.label3.Text = ":";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 71);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 17);
            this.label4.TabIndex = 9;
            this.label4.Text = "Hours";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(78, 71);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 17);
            this.label5.TabIndex = 10;
            this.label5.Text = "Minutes";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(150, 71);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(63, 17);
            this.label6.TabIndex = 11;
            this.label6.Text = "Seconds";
            // 
            // NewUserTimerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(218, 196);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtSeconds);
            this.Controls.Add(this.txtMinutes);
            this.Controls.Add(this.txtHours);
            this.Controls.Add(this.txtTitle);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewUserTimerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "New Timer";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.NewUserTimerForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTitle_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.txtHours)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMinutes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSeconds)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.NumericUpDown txtHours;
        private System.Windows.Forms.NumericUpDown txtMinutes;
        private System.Windows.Forms.NumericUpDown txtSeconds;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
    }
}