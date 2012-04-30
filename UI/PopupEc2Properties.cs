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
            buttonChangeName.Click += Do.HandleInZawsUi(buttonChangeName_Click, "Successfully changed EC2 name.", "Error while changing EC2 name, reason: {0}");

            buttonMysqlAdmin.Click += Do.HandleInZawsUi(buttonMysqlAdmin_Click, "Successfully started MySQL Admin.", "Error starting MySQL Admin, reason: {0}");
            buttonMysqlBrowser.Click += Do.HandleInZawsUi(buttonMysqlBrowser_Click, "Successfully started MySQL Browser.", "Error starting MySQL Browser, reason: {0}");

            buttonAppsRefresh.Click += Do.HandleInZawsUi(buttonAppsRefresh_Click, "Apps info retrieved.", "Error while checking installed apps, reason: {0}");
            buttonAppsUpdate.Click += Do.HandleInZawsUi(buttonAppsUpdate_Click, "Selected apps updated from repositories. NOTE: You might have to perform additional steps and/or restart the web server for changes to take effect.", "Error updating selected apps, reason: {0}");
            buttonAppsRebootApache.Click += Do.HandleInZawsUi(buttonAppsRebootApache_Click, "Apache restarted.", "Problem restarting apache, reason: {0}");

            MyEC2.StatusChanged += new EventHandler(MyEC2_StatusChanged);
            MyEC2.ObjectDeleted += new EventHandler(MyEC2_ObjectDeleted);
            buttonChangeName.Enabled = false;
        }

        void buttonAppsRebootApache_Click(object sender, EventArgs e)
        {
            MyEC2.RebootWebServer();
        }

        void buttonAppsUpdate_Click(object sender, EventArgs e)
        {
            if (MyEC2.Status == ZAwsEc2.Ec2Status.Running && listViewApps.SelectedItems.Count > 0)
            {
                foreach (ListViewItem item in listViewApps.SelectedItems)
                {
                    ZAwsEc2.Application app = (ZAwsEc2.Application)item.Tag;
                    if (!string.IsNullOrWhiteSpace(app.Repo))
                    {
                        app.Update();
                        Program.TraceLine("Application {0} updated from repository {1} succesfully.", app.Name, app.Repo);
                    }
                }
            }
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
            RefreshInfo();
        }

        private void RefreshInfo()
        {
            //SettingInfo = true;
            this.Text = string.Format("EC2 {0}, id={1}, {2}", MyEC2.Name, MyEC2.InstanceId, MyEC2.Status.ToString());
            if (buttonChangeName.Enabled == false)
            {
                textBoxName.Text = MyEC2.Name;
            }
            else if (textBoxName.Text == MyEC2.Name)
            {
                textBoxName_TextChanged(null, null);
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
            buttonTerminal.Enabled = buttonFileBrowser.Enabled =
                buttonRunScript.Enabled = buttonAppsNew.Enabled = buttonAppsRebootApache.Enabled =
                buttonAppsRefresh.Enabled = (MyEC2.Status == ZAwsEc2.Ec2Status.Running);

            try
            {
                chartCPU.Series[0].Points.Clear();
                chartCPU.Series[1].Points.Clear();
                chartNet.Series[0].Points.Clear();

                chartCPU.ChartAreas[0].AxisX.LabelStyle.Format = "HH:mm";
                chartCPU.ChartAreas[0].AxisX.LabelAutoFitStyle = System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.LabelsAngleStep30;
                chartCPU.ChartAreas[0].AxisX.IsLabelAutoFit = true;
                chartNet.ChartAreas[0].AxisX.LabelStyle.Format = "HH:mm";
                chartNet.ChartAreas[0].AxisX.LabelAutoFitStyle = System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.LabelsAngleStep30;
                chartNet.ChartAreas[0].AxisX.IsLabelAutoFit = true;
                    foreach (var point in MyEC2.StatisticCPUUtiliationMax.Values.OrderBy(new Func<ZAwsEc2.DataSample, DateTime>(myExtractor)))
                    {
                        chartCPU.Series[0].Points.AddXY(point.Time, point.Value);
                    }
                    foreach (var point in MyEC2.StatisticCPUUtiliationAvg.Values.OrderBy(new Func<ZAwsEc2.DataSample, DateTime>(myExtractor)))
                    {
                        chartCPU.Series[1].Points.AddXY(point.Time, point.Value);
                    }
                    foreach (var point in MyEC2.StatisticNetworkOut.Values.OrderBy(new Func<ZAwsEc2.DataSample, DateTime>(myExtractor)))
                    {
                        chartNet.Series[0].Points.AddXY(point.Time, point.Value);
                    }
                    chartNet.Visible = true;
                    chartCPU.Visible = true;
                    labelChart1.Visible = labelChart2.Visible = true;
            }
            catch
            {
                chartNet.Visible = 
                chartCPU.Visible = 
                labelChart1.Visible = labelChart2.Visible = false;
            }
            //SettingInfo = false;
        }
        DateTime myExtractor(ZAwsEc2.DataSample arg)
        {
            return arg.Time;
        }
        private void PopupEc2Properties_Load(object sender, EventArgs e)
        {
            RefreshInfo();
            RefreshApps(false);
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
        private void buttonMysqlAdmin_Click(object sender, EventArgs e)
        {
            if (MyEC2.Status == ZAwsEc2.Ec2Status.Running)
            {
                MyEC2.ConnectApp(System.Configuration.ConfigurationManager.AppSettings["MySqlAdminApp"],
                    System.Configuration.ConfigurationManager.AppSettings["MySqlAdminArgs"],
                    System.Configuration.ConfigurationManager.AppSettings["MySqlUsername"],
                    System.Configuration.ConfigurationManager.AppSettings["MySqlPassword"]);

            }
        }
        private void buttonMysqlBrowser_Click(object sender, EventArgs e)
        {
            if (MyEC2.Status == ZAwsEc2.Ec2Status.Running)
            {
                MyEC2.ConnectApp(System.Configuration.ConfigurationManager.AppSettings["MySqlBrowserApp"],
                    System.Configuration.ConfigurationManager.AppSettings["MySqlBrowserArgs"],
                    System.Configuration.ConfigurationManager.AppSettings["MySqlUsername"],
                    System.Configuration.ConfigurationManager.AppSettings["MySqlPassword"]);

            }
        }
        #region App handling
        private void buttonAppsRefresh_Click(object sender, EventArgs e)
        {
            RefreshApps(true);
        }
        void RefreshApps(bool force)
        {
            if (MyEC2.Status == ZAwsEc2.Ec2Status.Running)
            {
                listViewApps.Items.Clear();
                listViewApps.SelectedItems.Clear();
                buttonAppsRefresh.Enabled = false;
                foreach (ZAwsEc2.Application app in MyEC2.GetInstalledApps(force))
                {
                    var i = listViewApps.Items.Add(new ListViewItem(app.Name));
                    i.SubItems.Add(app.Repo);
                    i.SubItems.Add(app.URL);
                    i.SubItems.Add(app.AppType.ToString());
                    i.Tag = app;
                }
            }
        }
        #endregion 
        
        private void buttonWWW_Click(object sender, EventArgs e)
        {
            Program.OpenWebBrowser("http://" + MyEC2.Reservation.RunningInstance[0].PublicDnsName);
        }
        private void buttonWWWip_Click(object sender, EventArgs e)
        {
            Program.OpenWebBrowser("http://" + MyEC2.Reservation.RunningInstance[0].IpAddress);
        }

        //bool SettingInfo = false;
        private void textBoxName_TextChanged(object sender, EventArgs e)
        {
            if (/*!SettingInfo &&*/ textBoxName.Text != MyEC2.Name)
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
            MyEC2.SetName(textBoxName.Text);
        }
        private void buttonRunScript_Click(object sender, EventArgs e)
        {

        }
        private void buttonMonitor_Click(object sender, EventArgs e)
        {

        }

        private void listViewApps_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (MyEC2.Status == ZAwsEc2.Ec2Status.Running && listViewApps.SelectedItems.Count > 0)
            {
                foreach (ListViewItem item in listViewApps.SelectedItems)
                {
                    ZAwsEc2.Application app = (ZAwsEc2.Application)item.Tag;
                    if (!string.IsNullOrWhiteSpace(app.Repo))
                    {
                        buttonAppsUpdate.Enabled = true;
                        return;
                    }
                }
                buttonAppsUpdate.Enabled = false;
            }
            else
            {
                buttonAppsUpdate.Enabled = false;
            }
        }

        private void buttonAppsNew_Click(object sender, EventArgs e)
        {
            if (MyEC2.Status == ZAwsEc2.Ec2Status.Running)
            {
                DlgNewApp dlg = new DlgNewApp();
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        MyEC2.InstallApp(dlg.textBoxGitApp.Text, dlg.textBoxGitAppUrl.Text, dlg.textBoxGitAppLocation.Text,
                            dlg.comboBoxGitAppType.SelectedIndex == 0 ? ZAwsEc2.ApplicationType.RAILS_APP : ZAwsEc2.ApplicationType.GENERIC,
                            dlg.checkBoxGitAppDnsRecord.Checked, dlg.checkBoxGitAppDefaultServerApp.Checked);
                        RefreshApps(false);
                        Program.TraceLine("Succesfuly installed app {0}. Note: you still need to restart the web server for the changes to take place.", dlg.textBoxGitApp.Text);
                    }
                    catch (Exception ex)
                    {
                        Program.TraceLine("Error installing new app, reason: {0}.", ex);
                    }
                }
            }
        }
    }
}
