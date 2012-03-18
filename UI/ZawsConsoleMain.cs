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
        }

        void ZAwsObject_StatusChanged(object sender, EventArgs e)
        {
            awsListView.Invalidate();
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
            buttonStart.Left = 
                buttonStop.Left =
                buttonOpen.Left = 
                buttonCloseAll.Left =
                    this.ClientSize.Width - 12 - buttonStart.Width;

            awsListView.Left = 12;
            awsListView.Top = 12;
            awsListView.Height = this.ClientSize.Height;
            awsListView.Width = buttonStart.Left - 12;

            //Size of tiles - this should amke ti minimum 100, while filling the area.
            int c = awsListView.Width / 100;
            awsListView.TileSize = new Size(awsListView.Width / c, 120);
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
                e.Graphics.DrawString("IP", IconFont, Brushes.Blue, IconSpace);
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
            else
            {
                //Unknown ZAWS object
                Debug.Assert(false, "Unknown ZAWS object");
                throw new ArgumentException("Unknown ZAws object: " + e.Item.Tag.GetType().ToString());
            }
        }


    }
}
