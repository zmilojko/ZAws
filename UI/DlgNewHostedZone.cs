using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ZAws.Console
{
    partial class DlgNewHostedZone : Form
    {
        public DlgNewHostedZone(ZAwsEc2Controller controller)
        {
            InitializeComponent();
            Controller = controller;
        }

        ZAwsEc2Controller Controller;

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Controller.CreatedHostedZone(textBoxDomain.Text);
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
