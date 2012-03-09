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
            awsListView.Items.Add("Please wait until ZAws Console connects to the AWS servers...");
            controller.NewObject += new EventHandler<ZAwsEc2Controller.ZAwsNewObjectEventArgs>(controller_NewObject);
            controller.Connect();
        }

        void controller_NewObject(object sender, ZAwsEc2Controller.ZAwsNewObjectEventArgs e)
        {
            if (this.InvokeRequired)
            {
                //Following Invoknig must be asynchronous, so not to cause deadlock with the Disconnect handler.
                this.BeginInvoke(new EventHandler<ZAwsEc2Controller.ZAwsNewObjectEventArgs>(controller_NewObject), sender, e);
                return;
            }
            if (awsListView.Items.Count == 1 && awsListView.Items[0].Tag == null)
            {
                awsListView.Items.Clear();
            }
            awsListView.Items.Add(new ListViewItem(e.NewObject.Name))
                .Tag = e.NewObject;
        }

        private void MainView_Resize(object sender, EventArgs e)
        {
            awsListView.Left = 0;
            awsListView.Top = 0;
            awsListView.Height = this.ClientSize.Height;
            awsListView.Width = buttonStart.Left - 12;
        }


    }
}
