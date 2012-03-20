using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace ZAws.Console
{
    public partial class MainView : Form
    {
        public MainView()
        {
            InitializeComponent();
        }

        ZAwsEc2Controller controller = new ZAwsEc2Controller();

        private void MainView_Load(object sender, EventArgs e)
        {
            MainView_Resize(null, null);
            //awsListView.Items.Add("Please wait until ZAws Console connects to the AWS servers...");
            controller.NewObject += new EventHandler<ZAwsEc2Controller.ZAwsNewObjectEventArgs>(controller_NewObject);
            controller.Connect();

            //Create groups
            awsListView.Groups.Add(new ListViewGroup("EC2","EC2 Instances")); 
            awsListView.Groups.Add(new ListViewGroup("S3","S3 Buckets")); 
            awsListView.Groups.Add(new ListViewGroup("DNS","Route 53 Hosted Zones")); 
            awsListView.Groups.Add(new ListViewGroup("EC2x","Other EC2 Objects"));

            awsListView_SelectedIndexChanged(sender, e);
        }

        void controller_NewObject(object sender, ZAwsEc2Controller.ZAwsNewObjectEventArgs e)
        {
            if (this.InvokeRequired)
            {
                Debug.Assert(e.NewObject != null);
                //Following Invoknig must be asynchronous, so not to cause deadlock with the Disconnect handler.
                this.BeginInvoke(new EventHandler<ZAwsEc2Controller.ZAwsNewObjectEventArgs>(controller_NewObject), sender, e);
                return;
            }
            if (awsListView.Items.Count == 1 && awsListView.Items[0].Tag == null)
            {
                awsListView.Items.Clear();
            }

            ListViewGroup g;
            if (e.NewObject.GetType() == typeof(ZAwsEc2) || e.NewObject.GetType() == typeof(ZAwsElasticIp))
            {
                g = awsListView.Groups["EC2"];
                awsListView.ShowGroups = true;
            }
            else if (e.NewObject.GetType() == typeof(ZAwsS3))
            {
                g = awsListView.Groups["S3"];
                awsListView.ShowGroups = true;
            } 
            else if (e.NewObject.GetType() == typeof(ZAwsHostedZone))
            {
                g = awsListView.Groups["DNS"];
                awsListView.ShowGroups = true;
            }
            else
            {
                g = awsListView.Groups["EC2x"];
            }

            ListViewItem newItem = new ListViewItem(e.NewObject.Name);
            newItem.Tag = e.NewObject;
            awsListView.Items.Add(newItem);
            g.Items.Add(newItem);

            
            e.NewObject.StatusChanged += new EventHandler(ZAwsObject_StatusChanged);
            e.NewObject.ObjectDeleted += new EventHandler(ZAwsObject_ObjectDeleted);
            awsListView_SelectedIndexChanged(sender, e);
        }

        void ZAwsObject_ObjectDeleted(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                //Following Invoknig must be asynchronous, so not to cause deadlock with the Disconnect handler.
                this.BeginInvoke(new EventHandler(ZAwsObject_ObjectDeleted), sender, e);
                return;
            }
            ZAwsObject obj = (ZAwsObject)sender;
            awsListView.Items.Remove(ItemFromZAwsObject(obj));
            obj.StatusChanged -= new EventHandler(ZAwsObject_StatusChanged);
            obj.ObjectDeleted -= new EventHandler(ZAwsObject_ObjectDeleted);
            awsListView_SelectedIndexChanged(sender, e);
        }

        void ZAwsObject_StatusChanged(object sender, EventArgs e)
        {
            awsListView.Invalidate();
            awsListView_SelectedIndexChanged(sender, e);
        }

        ListViewItem ItemFromZAwsObject(ZAwsObject obj)
        {
            foreach (ListViewItem item in awsListView.Items)
            {
                if (((ZAwsObject)(item.Tag)) == obj)
                {
                    return item;
                }
            }
            throw new Exception("Cannot find object in list");
        }

        private void MainView_Resize(object sender, EventArgs e)
        {
            foreach (Control ctrl in Controls)
            {
                if(ctrl.GetType() == typeof(Button)) { ctrl.Left = this.ClientSize.Width - 12 - buttonStart.Width; }
                if(ctrl.GetType() == typeof(Label)) { ctrl.Left = this.ClientSize.Width - 12 - buttonStart.Width - 3; }

            }

            awsListView.Left = 12;
            awsListView.Top = 12;
            awsListView.Height = this.ClientSize.Height;
            awsListView.Width = buttonStart.Left - 12;

            //Size of tiles - this should amke ti minimum 100, while filling the area.
            int c = awsListView.Width / 100;
            awsListView.TileSize = new Size(105, 120);
        }

        private void MainView_FormClosed(object sender, FormClosedEventArgs e)
        {
            controller.Disconnect();
        }

        private void awsListView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            if(!((e.Item != null && e.Item.Tag != null && e.Item.Tag.GetType().IsSubclassOf(typeof(ZAwsObject)))))
            {
                e.DrawDefault = true;
                return;
            }

            ZAwsObject obj = (ZAwsObject)e.Item.Tag;

            e.DrawDefault = false;

            if((e.State & ListViewItemStates.Selected) != 0)
            {
                e.Graphics.FillRectangle(Brushes.LightGray, e.Bounds);
            }
            if ((e.State & ListViewItemStates.Focused) != 0)
            {
                e.Graphics.DrawRectangle(new Pen(Color.DarkGreen, 2), Rectangle.Inflate(e.Bounds, -2, -2));
            }
            else
            {
                e.Graphics.DrawRectangle(Pens.Gray, Rectangle.Inflate(e.Bounds, -2, -2));
            }


            Font IconFont = new Font(FontFamily.GenericSansSerif, 18, FontStyle.Bold);
            Font AdditionalStatusFont = new Font(FontFamily.GenericSansSerif, 6, FontStyle.Italic);
            Font NameFont = new Font(FontFamily.GenericSansSerif, 9, FontStyle.Regular);

            Rectangle IconSpace = new Rectangle(e.Bounds.X + 3, e.Bounds.Y + 3, 100, 25);
            Rectangle AdditionalIconSpace = new Rectangle(e.Bounds.X + 5, e.Bounds.Y + 25, 50, 10);
            Rectangle NameSpace = new Rectangle(e.Bounds.X + 3, e.Bounds.Y + 30, 100, 30);
            Rectangle DetailsSpace1 = new Rectangle(e.Bounds.X + 3, e.Bounds.Y + 42, 100, 30);
            Rectangle DetailsSpace2 = new Rectangle(e.Bounds.X + 3, e.Bounds.Y + 54, 100, 30);
            Rectangle DetailsSpace3 = new Rectangle(e.Bounds.X + 3, e.Bounds.Y + 66, 100, 30);

            e.Graphics.DrawString(obj.Name, NameFont, Brushes.DarkBlue, NameSpace);

            if(e.Item.Tag.GetType() == typeof(ZAwsEc2))
            {
                ZAwsEc2 ec2 = (ZAwsEc2)e.Item.Tag;

                switch (ec2.Status)
                {
                    case ZAwsEc2.Ec2Status.Running:
                        e.Graphics.DrawString("EC2", IconFont, Brushes.Green, IconSpace);
                        break;
                    case ZAwsEc2.Ec2Status.Stopped:
                        e.Graphics.DrawString("EC2", IconFont, Brushes.Red, IconSpace);
                        break;
                    case ZAwsEc2.Ec2Status.Stopping:
                        e.Graphics.DrawString("EC2", IconFont, Brushes.Red, IconSpace);
                        e.Graphics.DrawString("stopping", AdditionalStatusFont, Brushes.Red, AdditionalIconSpace);
                        break;
                    case ZAwsEc2.Ec2Status.Pending:
                        e.Graphics.DrawString("EC2", IconFont, Brushes.Green, IconSpace);
                        e.Graphics.DrawString("booting", AdditionalStatusFont, Brushes.Green, AdditionalIconSpace);
                        break;
                    case ZAwsEc2.Ec2Status.Terminated:
                        e.Graphics.DrawString("EC2", IconFont, Brushes.DarkRed, IconSpace);
                        e.Graphics.DrawString("terminated", AdditionalStatusFont, Brushes.DarkRed, AdditionalIconSpace);
                        e.Graphics.DrawLine(Pens.DarkRed, e.Bounds.Left, e.Bounds.Top, e.Bounds.Right, e.Bounds.Bottom);
                        e.Graphics.DrawLine(Pens.DarkRed, e.Bounds.Left, e.Bounds.Bottom, e.Bounds.Right, e.Bounds.Top);
                        break;
                    case ZAwsEc2.Ec2Status.ShuttingDown:
                        e.Graphics.DrawString("EC2", IconFont, Brushes.DarkRed, IconSpace);
                        e.Graphics.DrawString("shutting down", AdditionalStatusFont, Brushes.DarkRed, AdditionalIconSpace);
                        break;
                }
            }
            else if (e.Item.Tag.GetType() == typeof(ZAwsElasticIp))
            {
                ZAwsElasticIp ip = (ZAwsElasticIp)e.Item.Tag;
                e.Graphics.DrawString("IP", IconFont, Brushes.Blue, IconSpace);
                if (ip.Associated)
                {
                    e.Graphics.DrawString("=> " + ip.AssociatedEc2.Name, NameFont, Brushes.Black, DetailsSpace1);
                }
                else
                {
                    e.Graphics.DrawString("=> X", NameFont, Brushes.Black, DetailsSpace1);
                }

            }
            else if (e.Item.Tag.GetType() == typeof(ZAwsS3))
            {
                e.Graphics.DrawString("S3", IconFont, Brushes.Blue, IconSpace);
            }
            else if (e.Item.Tag.GetType() == typeof(ZAwsHostedZone))
            {
                e.Graphics.DrawString("DNS", IconFont, Brushes.Blue, IconSpace);
            }
            else if (e.Item.Tag.GetType() == typeof(ZAwsSnapshot))
            {
                e.Graphics.DrawString("IMG", IconFont, Brushes.Blue, IconSpace);
            }
            else if (e.Item.Tag.GetType() == typeof(ZAwsSecGroup))
            {
                e.Graphics.DrawString("Sec", IconFont, Brushes.Blue, IconSpace);
            }
            else if (e.Item.Tag.GetType() == typeof(ZAwsKeyPair))
            {
                e.Graphics.DrawString("Keys", IconFont, Brushes.Blue, IconSpace);
            }
            else if (e.Item.Tag.GetType() == typeof(ZAwsAmi))
            {
                e.Graphics.DrawString("AMI", IconFont, Brushes.Purple, IconSpace);
            }
            else if (e.Item.Tag.GetType() == typeof(ZAwsEbsVolume))
            {
                e.Graphics.DrawString("EBS", IconFont, Brushes.Blue, IconSpace);
            }
            else
            {
                //Unknown ZAWS object
                Debug.Assert(false, "Unknown ZAWS object");
                throw new ArgumentException("Unknown ZAws object: " + e.Item.Tag.GetType().ToString());
            }
        }

        private void awsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                //Following Invoknig must be asynchronous, so not to cause deadlock with the Disconnect handler.
                this.BeginInvoke(new EventHandler(awsListView_SelectedIndexChanged), sender, e);
                return;
            }
            foreach (Control ctrl in Controls)
            {
                if (ctrl.GetType() == typeof(Button)) { ctrl.Enabled = false; }
            }

            buttonDelete.Enabled = awsListView.SelectedItems.Count > 0;

            foreach (ListViewItem item in awsListView.SelectedItems)
            {
                if(item.Tag.GetType() == typeof(ZAwsEc2) && ((ZAwsEc2)item.Tag).Status == ZAwsEc2.Ec2Status.Stopped)
                {
                    buttonStart.Enabled = true;
                }
            }
            foreach (ListViewItem item in awsListView.SelectedItems)
            {
                if (item.Tag.GetType() == typeof(ZAwsEc2) && ((ZAwsEc2)item.Tag).Status == ZAwsEc2.Ec2Status.Running)
                {
                    buttonStop.Enabled = true;
                    buttonTerminal.Enabled = true;
                    buttonFileBrowser.Enabled = true;
                }
            }
            foreach (ListViewItem item in awsListView.SelectedItems)
            {
                if (item.Tag.GetType() == typeof(ZAwsAmi))
                {
                    buttonLaunchEc2Instance.Enabled = true;
                }
            }


            //Check for launch: must be exactly one AMI, and zero or one of key and sec.
            bool Amipresent = false;
            bool secGroupPresent = false;
            bool secKeyPresent = false;
            foreach (ListViewItem item in awsListView.SelectedItems)
            {
                if (item.Tag.GetType() == typeof(ZAwsAmi)) 
                {
                    if (Amipresent)
                    {
                        Amipresent = false;
                        break;
                    }
                    Amipresent = true;
                }
                if (item.Tag.GetType() == typeof(ZAwsSecGroup))
                {
                    if (secGroupPresent)
                    {
                        Amipresent = false;
                        break;
                    }
                    secGroupPresent = true;
                } 
                if (item.Tag.GetType() == typeof(ZAwsKeyPair))
                {
                    if (secKeyPresent)
                    {
                        Amipresent = false;
                        break;
                    }
                    secKeyPresent = true;
                }
            }
            buttonLaunchEc2Instance.Enabled = Amipresent && secGroupPresent && secKeyPresent;

        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in awsListView.SelectedItems)
            {
                if (item.Tag.GetType() == typeof(ZAwsEc2) && ((ZAwsEc2)item.Tag).Status == ZAwsEc2.Ec2Status.Stopped)
                {
                    ((ZAwsEc2)item.Tag).Start();
                }
            }
            buttonStart.Enabled = false;
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in awsListView.SelectedItems)
            {
                if (item.Tag.GetType() == typeof(ZAwsEc2) && ((ZAwsEc2)item.Tag).Status == ZAwsEc2.Ec2Status.Running)
                {
                    ((ZAwsEc2)item.Tag).Stop();
                }
            }
            buttonStop.Enabled = false;
        }

        private void buttonLaunchEc2Instance_Click(object sender, EventArgs e)
        {
            ZAwsAmi ami = null;
            ZAwsSecGroup secGroup = null;
            ZAwsKeyPair keyPair = null;
            foreach (ListViewItem item in awsListView.SelectedItems)
            {
                if (item.Tag.GetType() == typeof(ZAwsAmi))
                {
                    if (ami != null)
                    {
                        return;
                    }
                    ami = (ZAwsAmi)item.Tag;
                }
                if (item.Tag.GetType() == typeof(ZAwsSecGroup))
                {
                    if (secGroup != null)
                    {
                        return;
                    }
                    secGroup = (ZAwsSecGroup)item.Tag;
                }
                if (item.Tag.GetType() == typeof(ZAwsKeyPair))
                {
                    if (keyPair != null)
                    {
                        return;
                    }
                    keyPair = (ZAwsKeyPair)item.Tag;
                }
            }
            if (ami == null || secGroup == null || keyPair == null)
            {
                return;
            }
            ami.Launch(secGroup, keyPair);
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in awsListView.SelectedItems)
            {
                var response = MessageBox.Show(string.Format("Are yo usure you want to permanently delete object {1} of type {0}?",
                    ((ZAwsObject)item.Tag).Name, item.Tag.GetType()), "Confirm deletion - this cannot be undone!", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
                if (response == System.Windows.Forms.DialogResult.Yes)
                {
                    ((ZAwsObject)item.Tag).DeleteObject();
                }
            }
        }

        private void buttonTerminal_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in awsListView.SelectedItems)
            {
                if (item.Tag.GetType() == typeof(ZAwsEc2) && ((ZAwsEc2)item.Tag).Status == ZAwsEc2.Ec2Status.Running)
                {
                    ((ZAwsEc2)item.Tag).StartTerminal();
                }
            }
        }

        private void buttonFileBrowser_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in awsListView.SelectedItems)
            {
                if (item.Tag.GetType() == typeof(ZAwsEc2) && ((ZAwsEc2)item.Tag).Status == ZAwsEc2.Ec2Status.Running)
                {
                    ((ZAwsEc2)item.Tag).StartSshFileBrowser();
                }
            }
        }


    }
}
