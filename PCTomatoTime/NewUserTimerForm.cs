using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PCTomatoTime
{
    public partial class NewUserTimerForm : Form
    {
        public NewUserTimerForm()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            var seconds = txtHours.Value * 60 * 60 + txtMinutes.Value * 60 + txtSeconds.Value;
            ((TomatoForm)Owner).AddCustomTimer(txtTitle.Text, (int)seconds);
            this.Close();
        }

        private void NewUserTimerForm_Load(object sender, EventArgs e)
        {

        }

        private void txtTitle_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnStart_Click(sender, e);
            }
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

    }
}
