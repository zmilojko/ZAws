using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Amazon.Route53.Model;

namespace ZAws.Console
{
    partial class DlgEditDnsRecord : Form
    {
        ZAwsEc2Controller MyController;
        public ResourceRecordSet MyRecords { get; private set; }
        public DlgEditDnsRecord(ZAwsEc2Controller controller, ResourceRecordSet records)
        {
            InitializeComponent();
            MyController = controller;
            MyRecords = records; 
            comboBoxRecordType.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        const string newline = "\r\n";

        private void DlgEditDnsRecord_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(MyRecords.Name))
            {
                textBoxName.ReadOnly = true;
                comboBoxRecordType.Enabled = false;

                textBoxName.Text = MyRecords.Name;
                string s = "";
                foreach (var v in MyRecords.ResourceRecords)
                {
                    if (!string.IsNullOrWhiteSpace(s)) { s += newline; }
                    s += v.Value;
                }
                textBoxValue.Text = s;
                textBoxTTL.Text = MyRecords.TTL.ToString();
                comboBoxRecordType.Text = MyRecords.Type;
                comboBoxRecordType_SelectedIndexChanged(null, null);

                if (MyRecords.Type == "SOA" || MyRecords.Type == "NS")
                {
                    textBoxValue.ReadOnly = true;
                    textBoxTTL.ReadOnly = true;
                    buttonOK.Enabled = false;
                    listBoxOptions.Enabled = false;
                }
            }
            else
            {
                //These we do not support adding for, because we are not using them. Feel free to take some exceptions out.
                comboBoxRecordType.Items.Remove("PTR");
                comboBoxRecordType.Items.Remove("SRV");
                comboBoxRecordType.Items.Remove("SPF");
                comboBoxRecordType.Items.Remove("NS");
                comboBoxRecordType.Items.Remove("SOA");

                textBoxTTL.Text = "800";
            }
        }

        const string gooleAppsEmailServersMsg = "Google Apps MX servers";

        private void comboBoxRecordType_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBoxOptions.Items.Clear();
            switch (comboBoxRecordType.Text)
            {
                case "A":
                case "AAAA":
                    foreach (var ip in MyController.CurrentElasticIps)
                    {
                        if (ip.Associated)
                        {
                            InsertIntoTheSuggestionList(ip.Name + "(" + ip.AssociatedEc2.Name + " via elastic Ip)");
                        }
                    }                    
                    foreach (var ip in MyController.CurrentElasticIps)
                    {
                        if (!ip.Associated)
                        {
                            InsertIntoTheSuggestionList(ip.Name);
                        }
                    }
                    foreach (var zone in MyController.CurrentHostedZones)
                    {
                        if (zone.RecordsAvailable)
                        {
                            foreach (var r in zone.currentRecordSet)
                            {
                                if (r.Type == "A" || r.Type == "AAAA")
                                {
                                    foreach (var rr in r.ResourceRecords)
                                    {
                                        InsertIntoTheSuggestionList(rr.Value);
                                    }
                                }
                            }
                        }
                    }                  
                    break;
                case "CNAME":
                case "PTR":
                case "MX":
                    foreach (var zone in MyController.CurrentHostedZones)
                    {
                        if (comboBoxRecordType.Text == "MX")
                        {
                            InsertIntoTheSuggestionList(gooleAppsEmailServersMsg);
                        }
                        if (zone.RecordsAvailable)
                        {
                            foreach (var r in zone.currentRecordSet)
                            {
                                InsertIntoTheSuggestionList(r.Name);
                                if(r.Type == "CNAME" || r.Type == "PTR")
                                {
                                    foreach(var rr in r.ResourceRecords)
                                    {
                                        InsertIntoTheSuggestionList(rr.Value);
                                    }
                                }
                                if(r.Type == "MX")
                                {
                                    foreach (var rr in r.ResourceRecords)
                                    {
                                        rr.Value.Substring(rr.Value.IndexOf(' ') + 1);
                                        InsertIntoTheSuggestionList(rr.Value);
                                    }
                                }
                            }
                        }
                    }
                    break;
            }
        }

        private void InsertIntoTheSuggestionList(string s)
        {
            foreach (object k in listBoxOptions.Items)
            {
                if (s == k.ToString())
                {
                    return;
                }

            }
            listBoxOptions.Items.Add(s);
        }


        private void listBoxOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonInsertSelectedOption.Enabled = listBoxOptions.SelectedItems.Count > 0;
        }

        void NewLineIfNeeded()
        {
            if (!string.IsNullOrWhiteSpace(textBoxValue.Text))
            {
                textBoxValue.Text += newline;
            }
        }
        private void buttonInsertSelectedOption_Click(object sender, EventArgs e)
        {
            foreach (string i in listBoxOptions.SelectedItems)
            {
                NewLineIfNeeded();
                if (i == gooleAppsEmailServersMsg)
                {
                    
                    textBoxValue.Text += "1 ASPMX.L.GOOGLE.COM"
                                 + newline + "5 ALT1.ASPMX.L.GOOGLE.COM"
                                 + newline + "5 ALT2.ASPMX.L.GOOGLE.COM"
                                 + newline + "10 ASPMX2.GOOGLEMAIL.COM"
                                 + newline + "10 ASPMX3.GOOGLEMAIL.COM";
                    continue;
                }
                if (comboBoxRecordType.Text == "MX")
                {
                    textBoxValue.Text += "10 ";
                }
                textBoxValue.Text += i;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
        internal IEnumerable<Amazon.Route53.Model.ResourceRecord> CurrentResourceRecords
        {
            get
            {
                List<Amazon.Route53.Model.ResourceRecord> list = new List<ResourceRecord>();
                foreach (string s in textBoxValue.Text.Split(new string[] {newline}, StringSplitOptions.RemoveEmptyEntries))
                {
                    string ss = s;
                    if (s.Contains("via elastic"))
                    {
                        ss = s.Substring(0, s.IndexOf("("));
                    }

                    list.Add(new ResourceRecord().WithValue(ss));
                }
                return list;
            }
        }
    }
}
