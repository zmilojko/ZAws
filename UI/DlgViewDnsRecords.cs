using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

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
            ShowItems();
        }

        private void ShowItems()
        {
            listViewRecrods.Items.Clear();
            foreach (var r in MyZone.currentRecordSet)
            {
                ListViewItem item = new ListViewItem(r.Name);
                item.SubItems.Add(r.Type);
                string s = "";
                foreach (var rr in r.ResourceRecords)
                {
                    string newEntry = rr.Value;
                    //try to find elastic IPs
                    foreach (var el in MyController.CurrentElasticIps)
                    {
                        if (el.Name == rr.Value)
                        {
                            if (el.Associated)
                            {
                                newEntry += " => " + el.AssociatedEc2.Name;
                            }
                            else
                            {
                                newEntry += " => Elastic IP => X";
                            }
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(s)) { s += "\r\n"; }
                    s += newEntry;
                }

                item.SubItems.Add(s);
                item.SubItems.Add("");
                item.SubItems.Add(r.TTL.ToString());
                item.Tag = r;
                listViewRecrods.Items.Add(item);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listViewRecrods.SelectedItems.Count > 0 &&
                MessageBox.Show(string.Format("Are you sure you want to delete this record?"), "Confirm deletion",
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2)
                == System.Windows.Forms.DialogResult.Yes)
            {
                foreach (ListViewItem item in listViewRecrods.SelectedItems)
                {
                    Amazon.Route53.Model.ResourceRecordSet s = (Amazon.Route53.Model.ResourceRecordSet)item.Tag;
                    MyZone.DeleteRecord(s);
                }
                ShowItems();
            }
            
        }

        private void listViewRecrods_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonDelete.Enabled = listViewRecrods.SelectedItems.Count > 0;
            buttonEdit.Enabled = listViewRecrods.SelectedItems.Count == 1;
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            if (listViewRecrods.SelectedItems.Count == 1)
            {
                Amazon.Route53.Model.ResourceRecordSet set = (Amazon.Route53.Model.ResourceRecordSet)listViewRecrods.SelectedItems[0].Tag;
                DlgEditDnsRecord dlg = new DlgEditDnsRecord(MyController, set);
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    Amazon.Route53.Model.ChangeResourceRecordSetsResponse resp = 
                    MyController.route53.ChangeResourceRecordSets(new Amazon.Route53.Model.ChangeResourceRecordSetsRequest()
                        .WithHostedZoneId(this.MyZone.ResponseData.Id)
                        .WithChangeBatch(new Amazon.Route53.Model.ChangeBatch()
                                .WithChanges(new Amazon.Route53.Model.Change()
                                                   .WithAction("DELETE")
                                                   .WithResourceRecordSet(set),
                                             new Amazon.Route53.Model.Change()
                                                   .WithAction("CREATE")
                                                   .WithResourceRecordSet(new Amazon.Route53.Model.ResourceRecordSet()
                                                      .WithName(dlg.textBoxName.Text)
                                                      .WithType(dlg.comboBoxRecordType.Text)
                                                      .WithTTL(Convert.ToInt32(dlg.textBoxTTL.Text))
                                                      .WithResourceRecords(dlg.CurrentResourceRecords)))));
                    Thread.Sleep(2000);
                    MyZone.UpdateInfo();
                    ShowItems();
                }
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            Amazon.Route53.Model.ResourceRecordSet set = new Amazon.Route53.Model.ResourceRecordSet();
            DlgEditDnsRecord dlg = new DlgEditDnsRecord(MyController, set);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                MyZone.AddRecord(new Amazon.Route53.Model.ResourceRecordSet()
                                                    .WithName(dlg.textBoxName.Text)
                                                    .WithType(dlg.comboBoxRecordType.Text)
                                                    .WithTTL(Convert.ToInt32(dlg.textBoxTTL.Text))
                                                    .WithResourceRecords(dlg.CurrentResourceRecords));
                ShowItems();
            }
            
        }
    }
}
