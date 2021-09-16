using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TweeFly;

namespace TweeFlyPro
{
    public partial class Welcome : Form
    {
        private Form1 mainForm = null;

        public Welcome()
        {
            InitializeComponent();
            this.ControlBox = false;
        }

        public Welcome(Form1 callingForm)
        {
            mainForm = callingForm as Form1;
            InitializeComponent();
            this.ControlBox = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.mainForm.newConfig(false))
            {
                this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.mainForm.loadConf())
            {
                this.mainForm.updateFromConf(this.mainForm.conf);
                this.Close();
            }
        }
    }
}
