using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Amazon.EC2.Model;

namespace ZAws.Console
{
    
    public class ZawsListView : ListView
    {
        public ZawsListView()
            : base()
        {
            DoubleBuffered = true;
        }
    }
    /*
    public partial class MainView : Form
    {
        public MainView()
        {
            InitializeComponent();

            controller.StatusUpdate += this.HandleStatusUpdate;
        }

        ZAwsEc2Controller controller = new ZAwsEc2Controller();

        private void MainView_Load(object sender, EventArgs e)
        {
            controller.Connect();

            /*
            var resp = controller.GetRunningInstances();

            foreach (Amazon.EC2.Model.Reservation res in resp.DescribeInstancesResult.Reservation)
            {
                ZAwsEc2Instance newInstance = new ZAwsEc2Instance(res);
                var newItem = this.listView1.Items.Add(newInstance.Name);
                newItem.Tag = newInstance;
                newItem.ImageIndex = newInstance.StatusCode;
            }
             * *//*

            listView1.OwnerDraw = true;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonStart.Enabled = false;
            buttonStop.Enabled = false;
            buttonRestart.Enabled = false;
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                if (((ZAwsEc2)item.Tag).StatusCode == 1)
                {
                    buttonStart.Enabled = true;
                }
            } 
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                if (((ZAwsEc2)item.Tag).StatusCode == 0)
                {
                    buttonStop.Enabled = true;
                    //buttonRestart.Enabled = true;
                }
            }
        }

        

        private void buttonStart_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                if (((ZAwsEc2)item.Tag).StatusCode == 1)
                {
                    controller.StartInstance(((ZAwsEc2)item.Tag));
                }
            } 
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                if (((ZAwsEc2)item.Tag).StatusCode == 0)
                {
                    controller.StopInstance(((ZAwsEc2)item.Tag));
                }
            } 
        }

        private void buttonRestart_Click(object sender, EventArgs e)
        {
        }

        private void HandleStatusUpdate(object sender, ZAwsEc2Controller.ZAwsMonitorEventArgs e)
        {
            if (this.InvokeRequired)
            {
                //Following Invoknig must be asynchronous, so not to cause deadlock with the Disconnect handler.
                this.BeginInvoke(new EventHandler<ZAwsEc2Controller.ZAwsMonitorEventArgs>(HandleStatusUpdate), sender, e);
                return;
            }

            //var resp = controller.GetRunningInstances();

            //foreach (Amazon.EC2.Model.Reservation res in resp.DescribeInstancesResult.Reservation)
            foreach (ZAwsEc2 newInstance in e.CurrentInstances)
            {
                //ZAwsEc2Instance newInstance = new ZAwsEc2Instance(res);
                bool updated = false;

                foreach (ListViewItem item in listView1.Items)
                {
                    ZAwsEc2 oldInstance = ((ZAwsEc2)item.Tag);
                    if (oldInstance.InstanceId == newInstance.InstanceId)
                    {
                        item.Text = newInstance.Name;
                        item.Tag = newInstance;
                        item.ImageIndex = newInstance.StatusCode;
                        updated = true;
                        break;
                    }
                }
                if (!updated)
                {
                    var newItem = this.listView1.Items.Add(newInstance.Name);
                    newItem.Tag = newInstance;
                    newItem.ImageIndex = newInstance.StatusCode;
                }
            }
            List<ListViewItem> itemsToRemove = new List<ListViewItem>();
            foreach (ListViewItem item in listView1.Items)
            {
                bool found = false;
                foreach (ZAwsEc2 newInstance in e.CurrentInstances)
                {
                    ZAwsEc2 oldInstance = ((ZAwsEc2)item.Tag);
                    if (oldInstance.InstanceId == newInstance.InstanceId)
                    {
                        found = true;
                    }
                }
                if (!found)
                {
                    itemsToRemove.Add(item);
                }
            }
            foreach (ListViewItem item in itemsToRemove)
            {
                listView1.Items.Remove(item);
            }

            listView1_SelectedIndexChanged(null, null);
        }

        private void MainView_FormClosed(object sender, FormClosedEventArgs e)
        {
            controller.Disconnect();
        }

        private void listView1_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.Graphics.DrawRectangle(new Pen(Color.Aqua), e.Bounds);
        }


    }
*/
}
