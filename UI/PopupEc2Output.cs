using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.Threading;

namespace ZAws.Console
{
    partial class PopupEc2Output : Form
    {
        public PopupEc2Output(ZAwsEc2Controller controller, ZAwsEc2 ec2)
        {
            Ec2Instance = ec2;
            Controller = controller;

            InitializeComponent();
        }

        ZAwsEc2 Ec2Instance;
        ZAwsEc2Controller Controller;

        private void PopupEc2Output_Load(object sender, EventArgs e)
        {
            this.Text = "Boot console output for "+Ec2Instance.Name+" - waiting for output";
            textBox1.Text = Ec2Instance.ConsoleOutput;

            Ec2Instance.ConsoleUpdate += new EventHandler<ZAwsEc2.NewConceolOutputEventArgs>(Ec2Instance_ConsoleUpdate);
        }

        void Ec2Instance_ConsoleUpdate(object sender, ZAwsEc2.NewConceolOutputEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new EventHandler<ZAwsEc2.NewConceolOutputEventArgs>(Ec2Instance_ConsoleUpdate), sender, e);
                return;
            }

            this.Text = "Boot console output for " + Ec2Instance.Name + " - " + e.Timestamp;
            textBox1.Text = e.Output;
            textBox1.SelectionStart = textBox1.Text.Length;
            textBox1.ScrollToCaret();

            if(!string.IsNullOrWhiteSpace(textBox1.Text))
            {
                SystemSounds.Beep.Play();
                Thread.Sleep(400);
                SystemSounds.Beep.Play();
                Thread.Sleep(400);
                SystemSounds.Beep.Play();
                Thread.Sleep(400);
                SystemSounds.Beep.Play();
            }
        }

        private void PopupEc2Output_SizeChanged(object sender, EventArgs e)
        {
            this.textBox1.Size = new Size(ClientSize.Width - 24, ClientSize.Height - 24);
        }

    }
}
