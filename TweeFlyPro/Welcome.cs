using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
            if (this.mainForm.newConfig(textBox1.Text, false))
            {
                this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.mainForm.loadConf(comboBox2.Text, false))
            {
                this.mainForm.projectName = comboBox2.Text;
                this.mainForm.updateFromConf(this.mainForm.conf);
                this.Close();
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Welcome_Load(object sender, EventArgs e)
        {
            // Load templates
            string templateDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "templates");
            if (Directory.Exists(templateDir))
            {
                string[] templateDirs = Directory.GetDirectories(templateDir);
                comboBox1.Items.Clear();
                foreach (string td in templateDirs)
                {
                    comboBox1.Items.Add(new DirectoryInfo(td).Name);
                }
                if (comboBox1.Items.Count > 0)
                {
                    comboBox1.SelectedIndex = 0;
                }
            }

            // Load projects
            string projectsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "projects");
            if (Directory.Exists(projectsDir))
            {
                string[] projectDirs = Directory.GetDirectories(projectsDir);
                comboBox2.Items.Clear();
                foreach (string pd in projectDirs)
                {
                    if (File.Exists(Path.Combine(pd, "project.tfcx")) || File.Exists(Path.Combine(pd, "project.tfc")))
                    {
                        comboBox2.Items.Add(new DirectoryInfo(pd).Name);
                    }
                }
                if (comboBox2.Items.Count > 0)
                {
                    comboBox2.SelectedIndex = 0;
                }
            }

            // Disable create button if project already exists
            string startProjectXDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "projects", textBox1.Text, "project.tfcx");
            string startProjectDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "projects", textBox1.Text, "project.tfc");
            button1.Enabled = !File.Exists(startProjectDir) && !File.Exists(startProjectXDir);
            button4.Enabled = !File.Exists(startProjectDir) && !File.Exists(startProjectXDir);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string projectsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "projects", textBox1.Text, "project.tfcx");
            string projectXDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "projects", textBox1.Text, "project.tfc");
            button1.Enabled = !File.Exists(projectsDir) && !File.Exists(projectXDir);
            button4.Enabled = !File.Exists(projectsDir) && !File.Exists(projectXDir);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string templateDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "templates");

            if (this.mainForm.newConfigFromTemplate(textBox1.Text, Path.Combine(templateDir, comboBox1.Text), false))
            {
                this.Close();
            }
        }
    }
}
