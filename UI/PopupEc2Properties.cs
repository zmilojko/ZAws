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
    partial class PopupEc2Properties : ZAwsPopupForm
    {
        private ZAwsEc2 MyEC2
        {
            get
            {
                return (ZAwsEc2)base.MyObj;
            }
        }
        public PopupEc2Properties(ZAwsEc2 ec2)
            : base(ec2)
        {
            InitializeComponent();
            buttonStart.Click += Do.HandleInZawsUi(buttonStart_Click, "Successfully sent Start EC2 command.", "Error while sending EC2 start command, reason: {0}");
            buttonStop.Click += Do.HandleInZawsUi(buttonStop_Click, "Successfully sent Stop EC2 command.", "Error while sending EC2 stop command, reason: {0}");
            buttonTerminal.Click += Do.HandleInZawsUi(buttonTerminal_Click, "Successfully started SSH terminal application.", "Error while trying to start terminal application, reason: {0}");
            buttonFileBrowser.Click += Do.HandleInZawsUi(buttonFileBrowser_Click, "Successfully started SFTP file browser.", "Error while starting SFTP file browser, reason: {0}");
            buttonWWW.Click += Do.HandleInZawsUi(buttonWWW_Click, "Showing object in a Web browser", "Error while trying to show an object in a web browser, reason: {0}");
            buttonWWWip.Click += Do.HandleInZawsUi(buttonWWWip_Click, "Showing object in a Web browser", "Error while trying to show an object in a web browser, reason: {0}");

            buttonReboot.Click += Do.HandleInZawsUi(buttonReboot_Click, "Successfully sent Reboot EC2 command.", "Error while sending EC2 reboot command, reason: {0}");
            buttonInstallApp.Click += Do.HandleInZawsUi(buttonInstallApp_Click, "Successfully installed App on EC2.", "Error while installing app, reason: {0}");
            buttonRunScript.Click += Do.HandleInZawsUi(buttonRunScript_Click, "Successfully executed a script on EC2.", "Error while exeutnig script, reason: {0}");
            buttonChangeName.Click += Do.HandleInZawsUi(buttonChangeName_Click, "Successfully changed EC2 name.", "Error while changing EC2 name, reason: {0}");


            MyEC2.StatusChanged += new EventHandler(MyEC2_StatusChanged);
            MyEC2.ObjectDeleted += new EventHandler(MyEC2_ObjectDeleted);
            buttonChangeName.Enabled = false;
        }

        void MyEC2_ObjectDeleted(object sender, EventArgs e)
        {
            foreach (Control c in this.Controls)
            {
                if (c.GetType() == typeof(Button))
                {
                    c.Enabled = false;
                }
            }
            textBoxName.ReadOnly = true;
            this.Text = "DELECTED EC2";
        }

        void MyEC2_StatusChanged(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new EventHandler(MyEC2_StatusChanged), sender, e);
                return;
            }
            PopupEc2Properties_Load(sender, e);
        }

        private void PopupEc2Properties_Load(object sender, EventArgs e)
        {
            SettingInfo = true;
            this.Text = string.Format("EC2 {0}, id={1}, {2}", MyEC2.Name, MyEC2.InstanceId, MyEC2.Status.ToString());
            if (buttonChangeName.Enabled == false)
            {
                textBoxName.Text = MyEC2.Name;
            }
            textBoxType.Text = MyEC2.Reservation.RunningInstance[0].InstanceType;
            textBoxZone.Text = MyEC2.Reservation.RunningInstance[0].Placement.AvailabilityZone;
            textBoxSecGroups.Text = MyEC2.Reservation.RunningInstance[0].GroupName[0];
            textBoxKeyPair.Text = MyEC2.Reservation.RunningInstance[0].KeyName;
            textBoxPublicDns.Text = MyEC2.Reservation.RunningInstance[0].PublicDnsName;
            textBoxPrivateDns.Text = MyEC2.Reservation.RunningInstance[0].PrivateDnsName;
            textBoxElasticIp.Text = MyEC2.Reservation.RunningInstance[0].IpAddress;
            textBoxPrivateIp.Text = MyEC2.Reservation.RunningInstance[0].PrivateIpAddress;

            buttonStart.Enabled = MyEC2.Status == ZAwsEc2.Ec2Status.Stopped;
            buttonStop.Enabled = buttonReboot.Enabled = MyEC2.Status == ZAwsEc2.Ec2Status.Running;
            buttonTerminal.Enabled = buttonFileBrowser.Enabled = buttonInstallApp.Enabled =
                buttonRunScript.Enabled = MyEC2.Status == ZAwsEc2.Ec2Status.Running;
            SettingInfo = false;
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (MyEC2.Status == ZAwsEc2.Ec2Status.Stopped)
            {
                MyEC2.Start();
            }
        }
        private void buttonStop_Click(object sender, EventArgs e)
        {
            if (MyEC2.Status == ZAwsEc2.Ec2Status.Running)
            {
                MyEC2.Stop();
            }
        }
        private void buttonTerminal_Click(object sender, EventArgs e)
        {
            if (MyEC2.Status == ZAwsEc2.Ec2Status.Running)
            {
                MyEC2.StartTerminal();
            }
        }
        private void buttonFileBrowser_Click(object sender, EventArgs e)
        {
            if (MyEC2.Status == ZAwsEc2.Ec2Status.Running)
            {
                MyEC2.StartSshFileBrowser();
            }
        }
        private void buttonWWW_Click(object sender, EventArgs e)
        {
            Program.OpenWebBrowser("http://" + MyEC2.Reservation.RunningInstance[0].PublicDnsName);
        }
        private void buttonWWWip_Click(object sender, EventArgs e)
        {
            Program.OpenWebBrowser("http://" + MyEC2.Reservation.RunningInstance[0].IpAddress);
        }

        bool SettingInfo = false;
        private void textBoxName_TextChanged(object sender, EventArgs e)
        {
            if (!SettingInfo && textBoxName.Text != MyEC2.Name)
            {
                buttonChangeName.Enabled = true;
                textBoxName.BackColor = Color.Orange;
            }
            else
            {
                buttonChangeName.Enabled = false;
                textBoxName.BackColor = Color.White;
            }
        }

        private void PopupEc2Properties_FormClosed(object sender, FormClosedEventArgs e)
        {
            MyEC2.StatusChanged -= new EventHandler(MyEC2_StatusChanged);
        }

        private void buttonConsoleOutput_Click(object sender, EventArgs e)
        {
            Program.LaunchPopupForm<PopupEc2Output>(MyEC2);
        }

        private void buttonReboot_Click(object sender, EventArgs e)
        {
            if (MyEC2.Status == ZAwsEc2.Ec2Status.Running)
            {
                MyEC2.Reboot();
            }
        }
        private void buttonInstallApp_Click(object sender, EventArgs e)
        {

        }
        private void buttonChangeName_Click(object sender, EventArgs e)
        {

        }
        private void buttonRunScript_Click(object sender, EventArgs e)
        {

        }
        private void buttonMonitor_Click(object sender, EventArgs e)
        {

        }
    }
}
