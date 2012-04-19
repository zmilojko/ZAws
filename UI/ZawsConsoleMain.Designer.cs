namespace ZAws.Console
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
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.textBoxTrace = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.buttonWWW = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.awsListView = new ZAws.Console.ZawsListView();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonStart
            // 
            this.buttonStart.Enabled = false;
            this.buttonStart.Location = new System.Drawing.Point(556, 99);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 23);
            this.buttonStart.TabIndex = 1;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            // 
            // buttonStop
            // 
            this.buttonStop.Enabled = false;
            this.buttonStop.Location = new System.Drawing.Point(556, 128);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(75, 23);
            this.buttonStop.TabIndex = 1;
            this.buttonStop.Text = "Stop";
            this.buttonStop.UseVisualStyleBackColor = true;
            // 
            // buttonTerminal
            // 
            this.buttonTerminal.Enabled = false;
            this.buttonTerminal.Location = new System.Drawing.Point(556, 157);
            this.buttonTerminal.Name = "buttonTerminal";
            this.buttonTerminal.Size = new System.Drawing.Size(75, 23);
            this.buttonTerminal.TabIndex = 1;
            this.buttonTerminal.Text = "Terminal";
            this.buttonTerminal.UseVisualStyleBackColor = true;
            // 
            // buttonDelete
            // 
            this.buttonDelete.Location = new System.Drawing.Point(557, 444);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(75, 23);
            this.buttonDelete.TabIndex = 1;
            this.buttonDelete.Text = "Delete";
            this.buttonDelete.UseVisualStyleBackColor = true;
            // 
            // buttonLaunchEc2Instance
            // 
            this.buttonLaunchEc2Instance.Enabled = false;
            this.buttonLaunchEc2Instance.Location = new System.Drawing.Point(556, 215);
            this.buttonLaunchEc2Instance.Name = "buttonLaunchEc2Instance";
            this.buttonLaunchEc2Instance.Size = new System.Drawing.Size(75, 23);
            this.buttonLaunchEc2Instance.TabIndex = 1;
            this.buttonLaunchEc2Instance.Text = "Launch";
            this.buttonLaunchEc2Instance.UseVisualStyleBackColor = true;
            // 
            // buttonFileBrowser
            // 
            this.buttonFileBrowser.Enabled = false;
            this.buttonFileBrowser.Location = new System.Drawing.Point(556, 186);
            this.buttonFileBrowser.Name = "buttonFileBrowser";
            this.buttonFileBrowser.Size = new System.Drawing.Size(75, 23);
            this.buttonFileBrowser.TabIndex = 1;
            this.buttonFileBrowser.Text = "File browser";
            this.buttonFileBrowser.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(553, 80);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "EC2";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(553, 315);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Elastic IPs";
            // 
            // buttonIpNew
            // 
            this.buttonIpNew.Location = new System.Drawing.Point(556, 331);
            this.buttonIpNew.Name = "buttonIpNew";
            this.buttonIpNew.Size = new System.Drawing.Size(75, 23);
            this.buttonIpNew.TabIndex = 4;
            this.buttonIpNew.Text = "New IP";
            this.buttonIpNew.UseVisualStyleBackColor = true;
            // 
            // buttonIpAssociate
            // 
            this.buttonIpAssociate.Location = new System.Drawing.Point(556, 360);
            this.buttonIpAssociate.Name = "buttonIpAssociate";
            this.buttonIpAssociate.Size = new System.Drawing.Size(75, 23);
            this.buttonIpAssociate.TabIndex = 4;
            this.buttonIpAssociate.Text = "Associate";
            this.buttonIpAssociate.UseVisualStyleBackColor = true;
            // 
            // DNS
            // 
            this.DNS.AutoSize = true;
            this.DNS.Location = new System.Drawing.Point(553, 386);
            this.DNS.Name = "DNS";
            this.DNS.Size = new System.Drawing.Size(30, 13);
            this.DNS.TabIndex = 5;
            this.DNS.Text = "DNS";
            // 
            // buttonDnsNew
            // 
            this.buttonDnsNew.Location = new System.Drawing.Point(556, 402);
            this.buttonDnsNew.Name = "buttonDnsNew";
            this.buttonDnsNew.Size = new System.Drawing.Size(75, 23);
            this.buttonDnsNew.TabIndex = 6;
            this.buttonDnsNew.Text = "New Domain";
            this.buttonDnsNew.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(553, 241);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "S3";
            // 
            // buttonNewBucket
            // 
            this.buttonNewBucket.Enabled = false;
            this.buttonNewBucket.Location = new System.Drawing.Point(556, 257);
            this.buttonNewBucket.Name = "buttonNewBucket";
            this.buttonNewBucket.Size = new System.Drawing.Size(75, 23);
            this.buttonNewBucket.TabIndex = 6;
            this.buttonNewBucket.Text = "New bucket";
            this.buttonNewBucket.UseVisualStyleBackColor = true;
            // 
            // buttonBucketFileBrowser
            // 
            this.buttonBucketFileBrowser.Enabled = false;
            this.buttonBucketFileBrowser.Location = new System.Drawing.Point(556, 286);
            this.buttonBucketFileBrowser.Name = "buttonBucketFileBrowser";
            this.buttonBucketFileBrowser.Size = new System.Drawing.Size(75, 23);
            this.buttonBucketFileBrowser.TabIndex = 6;
            this.buttonBucketFileBrowser.Text = "File browser";
            this.buttonBucketFileBrowser.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(553, 428);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "General";
            // 
            // splitContainer
            // 
            this.splitContainer.Location = new System.Drawing.Point(12, 12);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.awsListView);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.textBoxTrace);
            this.splitContainer.Size = new System.Drawing.Size(371, 345);
            this.splitContainer.SplitterDistance = 258;
            this.splitContainer.TabIndex = 7;
            // 
            // textBoxTrace
            // 
            this.textBoxTrace.BackColor = System.Drawing.Color.White;
            this.textBoxTrace.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxTrace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxTrace.Location = new System.Drawing.Point(0, 0);
            this.textBoxTrace.Multiline = true;
            this.textBoxTrace.Name = "textBoxTrace";
            this.textBoxTrace.ReadOnly = true;
            this.textBoxTrace.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxTrace.Size = new System.Drawing.Size(371, 83);
            this.textBoxTrace.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(556, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(75, 65);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 8;
            this.pictureBox1.TabStop = false;
            // 
            // buttonWWW
            // 
            this.buttonWWW.Location = new System.Drawing.Point(556, 473);
            this.buttonWWW.Name = "buttonWWW";
            this.buttonWWW.Size = new System.Drawing.Size(75, 23);
            this.buttonWWW.TabIndex = 9;
            this.buttonWWW.Text = "WWW";
            this.buttonWWW.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.ForeColor = System.Drawing.Color.Gray;
            this.textBox1.Location = new System.Drawing.Point(557, 502);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(74, 37);
            this.textBox1.TabIndex = 10;
            this.textBox1.Text = "Monitoring not started";
            // 
            // awsListView
            // 
            this.awsListView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.awsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.awsListView.Location = new System.Drawing.Point(0, 0);
            this.awsListView.Name = "awsListView";
            this.awsListView.OwnerDraw = true;
            this.awsListView.Size = new System.Drawing.Size(371, 258);
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
            this.ClientSize = new System.Drawing.Size(719, 566);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.buttonWWW);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.splitContainer);
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
            this.MinimumSize = new System.Drawing.Size(567, 600);
            this.Name = "MainView";
            this.Text = "M";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainView_FormClosed);
            this.Load += new System.EventHandler(this.MainView_Load);
            this.Resize += new System.EventHandler(this.MainView_Resize);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
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
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.TextBox textBoxTrace;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button buttonWWW;
        private System.Windows.Forms.TextBox textBox1;
    }
}