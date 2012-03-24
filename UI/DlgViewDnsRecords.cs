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
    partial class DlgViewDnsRecords : Form
    {
        ZAwsEc2Controller MyController;
        ZAwsHostedZone MyZone;

        public DlgViewDnsRecords(ZAwsEc2Controller Controller, ZAwsHostedZone Zone)
        {
            InitializeComponent();

            MyController = Controller;
            MyZone = Zone;

            this.Text = string.Format("Hosted zone {0} resource records", MyZone.Name);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DlgViewDnsRecords_Load(object sender, EventArgs e)
        {
            foreach (var r in MyZone.currentRecordSet)
            {
                ListViewItem item = new ListViewItem(r.Name);
                item.SubItems.Add(r.Type);
                string s = "";
                foreach(var rr in r.ResourceRecords)
                {
                    if(!string.IsNullOrWhiteSpace(s)) { s+= "\r"; }
                    s += rr.Value;
                }

                item.SubItems.Add(s);
                item.SubItems.Add("");
                item.SubItems.Add(r.TTL.ToString());
                item.Tag = r;
                listViewRecrods.Items.Add(item);
            }
        }
    }
}
