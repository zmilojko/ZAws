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
    partial class DlgLaunchNewInstance : Form
    {
        public DlgLaunchNewInstance(ZAwsEc2Controller controller, ZAwsAmi ami, ZAwsSecGroup secGroup, ZAwsKeyPair keyPair)
        {
            Ami = ami;
            SecGroup = secGroup;
            KeyPair = keyPair;
            Controller = controller;

            InitializeComponent();
        }

        ZAwsAmi Ami;
        ZAwsSecGroup SecGroup;
        ZAwsKeyPair KeyPair;
        ZAwsEc2Controller Controller;

        private void buttonLaunch_Click(object sender, EventArgs e)
        {
            Ami.Launch(SecGroup, KeyPair, textBoxInstanceName.Text, textBoxStrtupScript.Text);
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void DlgLaunchNewInstance_Load(object sender, EventArgs e)
        {
            foreach (var a in Controller.CurrentAmis)
            {
                comboBoxAmi.Items.Add(a.Name);
            }
            comboBoxAmi.SelectedItem = Ami.Name;

            foreach (var s in Controller.CurrentSecGroups)
            {
                comboBoxSecGroup.Items.Add(s.Name);
            }
            comboBoxSecGroup.SelectedItem = SecGroup.Name;

            foreach (var k in Controller.CurrentKeyPairs)
            {
                comboBoxKeyPair.Items.Add(k.Name);
            }
            comboBoxKeyPair.SelectedItem = KeyPair.Name;

            comboBoxRegion.SelectedIndex = 3;
            comboBoxInstanceType.SelectedIndex = 0;
            comboBoxGitAppType.SelectedIndex = 0;

        }
    }
}
