﻿namespace ZAws.Console
{
    partial class MainView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainView));
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonTerminal = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonLaunchEc2Instance = new System.Windows.Forms.Button();
            this.buttonFileBrowser = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonIpNew = new System.Windows.Forms.Button();
            this.buttonIpAssociate = new System.Windows.Forms.Button();
            this.DNS = new System.Windows.Forms.Label();
            this.buttonDnsNew = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonNewBucket = new System.Windows.Forms.Button();
            this.buttonBucketFileBrowser = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.awsListView = new ZAws.Console.ZawsListView();
            this.SuspendLayout();
            // 
            // buttonStart
            // 
            this.buttonStart.Enabled = false;
            this.buttonStart.Location = new System.Drawing.Point(464, 28);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 23);
            this.buttonStart.TabIndex = 1;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Enabled = false;
            this.buttonStop.Location = new System.Drawing.Point(464, 57);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(75, 23);
            this.buttonStop.TabIndex = 1;
            this.buttonStop.Text = "Stop";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // buttonTerminal
            // 
            this.buttonTerminal.Enabled = false;
            this.buttonTerminal.Location = new System.Drawing.Point(464, 86);
            this.buttonTerminal.Name = "buttonTerminal";
            this.buttonTerminal.Size = new System.Drawing.Size(75, 23);
            this.buttonTerminal.TabIndex = 1;
            this.buttonTerminal.Text = "Terminal";
            this.buttonTerminal.UseVisualStyleBackColor = true;
            this.buttonTerminal.Click += new System.EventHandler(this.buttonTerminal_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Location = new System.Drawing.Point(465, 373);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(75, 23);
            this.buttonDelete.TabIndex = 1;
            this.buttonDelete.Text = "Delete";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonLaunchEc2Instance
            // 
            this.buttonLaunchEc2Instance.Enabled = false;
            this.buttonLaunchEc2Instance.Location = new System.Drawing.Point(464, 144);
            this.buttonLaunchEc2Instance.Name = "buttonLaunchEc2Instance";
            this.buttonLaunchEc2Instance.Size = new System.Drawing.Size(75, 23);
            this.buttonLaunchEc2Instance.TabIndex = 1;
            this.buttonLaunchEc2Instance.Text = "Launch";
            this.buttonLaunchEc2Instance.UseVisualStyleBackColor = true;
            this.buttonLaunchEc2Instance.Click += new System.EventHandler(this.buttonLaunchEc2Instance_Click);
            // 
            // buttonFileBrowser
            // 
            this.buttonFileBrowser.Enabled = false;
            this.buttonFileBrowser.Location = new System.Drawing.Point(464, 115);
            this.buttonFileBrowser.Name = "buttonFileBrowser";
            this.buttonFileBrowser.Size = new System.Drawing.Size(75, 23);
            this.buttonFileBrowser.TabIndex = 1;
            this.buttonFileBrowser.Text = "File browser";
            this.buttonFileBrowser.UseVisualStyleBackColor = true;
            this.buttonFileBrowser.Click += new System.EventHandler(this.buttonFileBrowser_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(461, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "EC2";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(461, 244);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Elastic IPs";
            // 
            // buttonIpNew
            // 
            this.buttonIpNew.Location = new System.Drawing.Point(464, 260);
            this.buttonIpNew.Name = "buttonIpNew";
            this.buttonIpNew.Size = new System.Drawing.Size(75, 23);
            this.buttonIpNew.TabIndex = 4;
            this.buttonIpNew.Text = "New IP";
            this.buttonIpNew.UseVisualStyleBackColor = true;
            this.buttonIpNew.Click += new System.EventHandler(this.buttonIpNew_Click);
            // 
            // buttonIpAssociate
            // 
            this.buttonIpAssociate.Location = new System.Drawing.Point(464, 289);
            this.buttonIpAssociate.Name = "buttonIpAssociate";
            this.buttonIpAssociate.Size = new System.Drawing.Size(75, 23);
            this.buttonIpAssociate.TabIndex = 4;
            this.buttonIpAssociate.Text = "Associate";
            this.buttonIpAssociate.UseVisualStyleBackColor = true;
            this.buttonIpAssociate.Click += new System.EventHandler(this.buttonIpAssociate_Click);
            // 
            // DNS
            // 
            this.DNS.AutoSize = true;
            this.DNS.Location = new System.Drawing.Point(461, 315);
            this.DNS.Name = "DNS";
            this.DNS.Size = new System.Drawing.Size(30, 13);
            this.DNS.TabIndex = 5;
            this.DNS.Text = "DNS";
            // 
            // buttonDnsNew
            // 
            this.buttonDnsNew.Location = new System.Drawing.Point(464, 331);
            this.buttonDnsNew.Name = "buttonDnsNew";
            this.buttonDnsNew.Size = new System.Drawing.Size(75, 23);
            this.buttonDnsNew.TabIndex = 6;
            this.buttonDnsNew.Text = "New Domain";
            this.buttonDnsNew.UseVisualStyleBackColor = true;
            this.buttonDnsNew.Click += new System.EventHandler(this.buttonDnsNew_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(461, 170);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "S3";
            // 
            // buttonNewBucket
            // 
            this.buttonNewBucket.Enabled = false;
            this.buttonNewBucket.Location = new System.Drawing.Point(464, 186);
            this.buttonNewBucket.Name = "buttonNewBucket";
            this.buttonNewBucket.Size = new System.Drawing.Size(75, 23);
            this.buttonNewBucket.TabIndex = 6;
            this.buttonNewBucket.Text = "New bucket";
            this.buttonNewBucket.UseVisualStyleBackColor = true;
            // 
            // buttonBucketFileBrowser
            // 
            this.buttonBucketFileBrowser.Enabled = false;
            this.buttonBucketFileBrowser.Location = new System.Drawing.Point(464, 215);
            this.buttonBucketFileBrowser.Name = "buttonBucketFileBrowser";
            this.buttonBucketFileBrowser.Size = new System.Drawing.Size(75, 23);
            this.buttonBucketFileBrowser.TabIndex = 6;
            this.buttonBucketFileBrowser.Text = "File browser";
            this.buttonBucketFileBrowser.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(461, 357);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "General";
            // 
            // awsListView
            // 
            this.awsListView.Location = new System.Drawing.Point(3, 0);
            this.awsListView.Name = "awsListView";
            this.awsListView.OwnerDraw = true;
            this.awsListView.Size = new System.Drawing.Size(400, 300);
            this.awsListView.TabIndex = 0;
            this.awsListView.UseCompatibleStateImageBehavior = false;
            this.awsListView.View = System.Windows.Forms.View.Tile;
            this.awsListView.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.awsListView_DrawItem);
            this.awsListView.SelectedIndexChanged += new System.EventHandler(this.awsListView_SelectedIndexChanged);
            this.awsListView.DoubleClick += new System.EventHandler(this.awsListView_DoubleClick);
            // 
            // MainView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(719, 636);
            this.Controls.Add(this.awsListView);
            this.Controls.Add(this.buttonBucketFileBrowser);
            this.Controls.Add(this.buttonNewBucket);
            this.Controls.Add(this.buttonDnsNew);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.DNS);
            this.Controls.Add(this.buttonIpAssociate);
            this.Controls.Add(this.buttonIpNew);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonLaunchEc2Instance);
            this.Controls.Add(this.buttonFileBrowser);
            this.Controls.Add(this.buttonTerminal);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.buttonStart);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(567, 502);
            this.Name = "MainView";
            this.Text = "ZawsConsoleMain";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainView_FormClosed);
            this.Load += new System.EventHandler(this.MainView_Load);
            this.Resize += new System.EventHandler(this.MainView_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Button buttonTerminal;
        private System.Windows.Forms.Button buttonDelete;
        private ZawsListView awsListView;
        private System.Windows.Forms.Button buttonLaunchEc2Instance;
        private System.Windows.Forms.Button buttonFileBrowser;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonIpNew;
        private System.Windows.Forms.Button buttonIpAssociate;
        private System.Windows.Forms.Label DNS;
        private System.Windows.Forms.Button buttonDnsNew;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonNewBucket;
        private System.Windows.Forms.Button buttonBucketFileBrowser;
        private System.Windows.Forms.Label label4;
    }
}