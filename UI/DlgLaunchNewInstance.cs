///////////////////////////////////////////////////////////////////////////////
//   Copyright 2012 Z-Ware Ltd.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

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

        public string NewInstanceId = "";

        private void buttonLaunch_Click(object sender, EventArgs e)
        {
            decimal Price = 0;
            if (!comboBoxPrice.Text.Contains("Full price"))
            {
                Price = Convert.ToDecimal(comboBoxPrice.Text.Split(' ')[0]);
            }

            //Collect the apps
            ZAws.ZAwsAmi.NewApp[] apps = new ZAwsAmi.NewApp[listView1.Items.Count];
            for (int i = 0; i < listView1.Items.Count; i++)
            {
                apps[i] = (ZAws.ZAwsAmi.NewApp)listView1.Items[i].Tag;
            }

            string instanceId = Ami.Launch(comboBoxInstanceType.Text, SecGroup, KeyPair, textBoxInstanceName.Text, textBoxStrtupScript.Text, apps, Price);
            NewInstanceId = instanceId;
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

            //load the file "ec2_initscript" from the working directory
            try
            {
                StreamReader streamReader = new StreamReader("ec2_initscript");
                textBoxStrtupScript.Text = streamReader.ReadToEnd();
                streamReader.Close();
            }
            catch { }

            //name
            int n = (int)Program.AppRegKey.GetValue("NextInstanceDefaultNameNumber", 0);
            n = (n +1) % 1000;
            this.textBoxInstanceName.Text = string.Format("ZAws{0:D3}", n);
            Program.AppRegKey.SetValue("NextInstanceDefaultNameNumber", n);

            comboBoxInstanceType_SelectedIndexChanged(sender, e);
        }

        bool directoryWasNotChangedManually = true;
        bool nowChangingDirectoryAutomatically = false;
        private void textBoxGitAppLocation_TextChanged(object sender, EventArgs e)
        {
            if (directoryWasNotChangedManually)
            {
                nowChangingDirectoryAutomatically = true;
                try
                {
                    textBoxGitApp.Text = textBoxGitAppLocation.Text.Substring(textBoxGitAppLocation.Text.LastIndexOf("/") + 1);
                    if(textBoxGitApp.Text.Contains(".git"))
                    {
                        textBoxGitApp.Text = textBoxGitApp.Text.Substring(0, textBoxGitApp.Text.IndexOf(".git"));
                    }
                }
                catch { }
                nowChangingDirectoryAutomatically = false;
            }
            textBoxGitAppUrl_TextChanged(sender, e);
        }

        private void textBoxGitApp_TextChanged(object sender, EventArgs e)
        {
            if (nowChangingDirectoryAutomatically)
            {
                return;
            }
            directoryWasNotChangedManually = string.IsNullOrWhiteSpace(textBoxGitApp.Text);
            textBoxGitAppUrl_TextChanged(sender, e);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ZAws.ZAwsAmi.NewApp newApp = new ZAws.ZAwsAmi.NewApp()
                            {
                                AppName = textBoxGitApp.Text,
                                AppLocation = textBoxGitAppLocation.Text,
                                AppUrl = textBoxGitAppUrl.Text,
                                CreateUrlRecords = checkBoxGitAppDnsRecord.Checked,
                                DefaultServerApp = checkBoxGitAppDefaultServerApp.Checked,
                                TypeIsRails = comboBoxGitAppType.Text.Contains("Rails")
                            };
            ListViewItem newItem = new ListViewItem(newApp.AppLocation);
            newItem.SubItems.Add(newApp.AppUrl);
            newItem.SubItems.Add(newApp.TypeIsRails?"Rails":"Generic");
            newItem.Tag = newApp;
            listView1.Items.Add(newItem);
        }

        private void textBoxGitAppUrl_TextChanged(object sender, EventArgs e)
        {
            button2.Enabled = !(string.IsNullOrWhiteSpace(textBoxGitApp.Text)
                                || string.IsNullOrWhiteSpace(textBoxGitAppLocation.Text)
                                || string.IsNullOrWhiteSpace(textBoxGitAppUrl.Text));
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonRemoveAppFomList.Enabled = listView1.SelectedItems.Count == 1;
        }

        private void buttonRemoveAppFomList_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                listView1.Items.Remove(listView1.SelectedItems[0]);
            }
        }

        private void comboBoxInstanceType_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBoxPrice.Items.Clear();
            comboBoxPrice.Items.Add("Full price - on demand instance");
            switch (comboBoxInstanceType.Text)
            {
                case "t1.micro":
                    comboBoxPrice.Items.Add("0,01 - Minimum, probably available");
                    comboBoxPrice.Items.Add("0,02 - Most likely available");
                    comboBoxPrice.Items.Add("0,025 - Full price (on demand)");
                    break;
                case "m1.small":
                    comboBoxPrice.Items.Add("0,036 - Minimum, probably available");
                    comboBoxPrice.Items.Add("0,06 - Most likely available");
                    comboBoxPrice.Items.Add("0,08 - Most likely available");
                    comboBoxPrice.Items.Add("0,09 - Full price (on demand)");
                    break;
                case "m1.medium":
                    comboBoxPrice.Items.Add("0,05 - Minimum, probably available");
                    comboBoxPrice.Items.Add("0,07 - Likely available");
                    comboBoxPrice.Items.Add("0,09 - Likely available");
                    comboBoxPrice.Items.Add("0,11 - Most likely available");
                    comboBoxPrice.Items.Add("0,15 - Most likely available");
                    comboBoxPrice.Items.Add("0,18 - Full price (on demand)");
                    break;
                case "m1.large":
                    comboBoxPrice.Items.Add("0,15 - Minimum, probably available");
                    comboBoxPrice.Items.Add("0,18 - Likely available");
                    comboBoxPrice.Items.Add("0,21 - Likely available");
                    comboBoxPrice.Items.Add("0,25 - Most likely available");
                    comboBoxPrice.Items.Add("0,30 - Most likely available");
                    comboBoxPrice.Items.Add("0,36 - Full price (on demand)");
                    break;                
                case "m1.xlarge":
                    comboBoxPrice.Items.Add("0,29 - Minimum, probably available");
                    comboBoxPrice.Items.Add("0,33 - Likely available");
                    comboBoxPrice.Items.Add("0,38 - Likely available");
                    comboBoxPrice.Items.Add("0,50 - Most likely available");
                    comboBoxPrice.Items.Add("0,60 - Most likely available");
                    comboBoxPrice.Items.Add("0,72 - Full price (on demand)");
                    break;                
                case "m2.xlarge":
                    comboBoxPrice.Items.Add("0,22 - Minimum, probably available");
                    comboBoxPrice.Items.Add("0,25 - Likely available");
                    comboBoxPrice.Items.Add("0,29 - Likely available");
                    comboBoxPrice.Items.Add("0,35 - Most likely available");
                    comboBoxPrice.Items.Add("0,42 - Most likely available");
                    comboBoxPrice.Items.Add("0,506 - Full price (on demand)");
                    break;
                case "m2.2xlarge":
                    comboBoxPrice.Items.Add("0,51 - Minimum, probably available");
                    comboBoxPrice.Items.Add("0,60 - Likely available");
                    comboBoxPrice.Items.Add("0,70 - Likely available");
                    comboBoxPrice.Items.Add("0,80 - Most likely available");
                    comboBoxPrice.Items.Add("0,90 - Most likely available");
                    comboBoxPrice.Items.Add("1,012  - Full price (on demand)");
                    break;
                case "m2.4xlarge":
                    comboBoxPrice.Items.Add("1,008 - Minimum, probably available");
                    comboBoxPrice.Items.Add("1,10 - Likely available");
                    comboBoxPrice.Items.Add("1,25 - Likely available");
                    comboBoxPrice.Items.Add("1,40 - Most likely available");
                    comboBoxPrice.Items.Add("1,65 - Most likely available");
                    comboBoxPrice.Items.Add("2,024 - Full price (on demand)");
                    break;
                case "c1.medium":
                    comboBoxPrice.Items.Add("0,08 - Minimum, probably available");
                    comboBoxPrice.Items.Add("0,10 - Likely available");
                    comboBoxPrice.Items.Add("0,12 - Likely available");
                    comboBoxPrice.Items.Add("0,14 - Most likely available");
                    comboBoxPrice.Items.Add("0,16 - Most likely available");
                    comboBoxPrice.Items.Add("0,186 - Full price (on demand)");
                    break;
                case "c1.xlarge":
                    comboBoxPrice.Items.Add("0,29 - Minimum, probably available");
                    comboBoxPrice.Items.Add("0,35 - Likely available");
                    comboBoxPrice.Items.Add("0,42 - Likely available");
                    comboBoxPrice.Items.Add("0,50 - Most likely available");
                    comboBoxPrice.Items.Add("0,60 - Most likely available");
                    comboBoxPrice.Items.Add("0,744 - Full price (on demand)");
                    break;
            }
            comboBoxPrice.SelectedIndex = 0;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
    }
}
