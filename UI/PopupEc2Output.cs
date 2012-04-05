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
using System.Media;
using System.Threading;

namespace ZAws.Console
{
    partial class PopupEc2Output : ZAwsPopupForm
    {
        public PopupEc2Output(ZAwsEc2 ec2)
            : base(ec2)
        {
            InitializeComponent();
        }

        ZAwsEc2 Ec2Instance
        {
            get
            {
                return (ZAwsEc2)base.MyObj;
            }
        }
        private void PopupEc2Output_Load(object sender, EventArgs e)
        {
            this.Text = "Boot console output for "+Ec2Instance.Name+" - waiting for output";
            textBox1.Text = Ec2Instance.ConsoleOutput;

            Ec2Instance.ConsoleUpdate += new EventHandler<ZAwsEc2.NewConceolOutputEventArgs>(Ec2Instance_ConsoleUpdate);
        }

        void Ec2Instance_ConsoleUpdate(object sender, ZAwsEc2.NewConceolOutputEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new EventHandler<ZAwsEc2.NewConceolOutputEventArgs>(Ec2Instance_ConsoleUpdate), sender, e);
                return;
            }

            this.Text = "Boot console output for " + Ec2Instance.Name + " - " + e.Timestamp;
            textBox1.Text = e.Output;
            textBox1.SelectionStart = textBox1.Text.Length;
            textBox1.ScrollToCaret();
        }

        private void PopupEc2Output_SizeChanged(object sender, EventArgs e)
        {
            this.textBox1.Size = new Size(ClientSize.Width - 24, ClientSize.Height - 24);
        }

    }
}
