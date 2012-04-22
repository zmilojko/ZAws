namespace ZAws.Console
{
    partial class DlgNewApp
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DlgNewApp));
            this.checkBoxGitAppDefaultServerApp = new System.Windows.Forms.CheckBox();
            this.checkBoxGitAppDnsRecord = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxGitAppUrl = new System.Windows.Forms.TextBox();
            this.textBoxGitApp = new System.Windows.Forms.TextBox();
            this.textBoxGitAppLocation = new System.Windows.Forms.TextBox();
            this.comboBoxGitAppType = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // checkBoxGitAppDefaultServerApp
            // 
            this.checkBoxGitAppDefaultServerApp.AutoSize = true;
            this.checkBoxGitAppDefaultServerApp.Location = new System.Drawing.Point(323, 90);
            this.checkBoxGitAppDefaultServerApp.Name = "checkBoxGitAppDefaultServerApp";
            this.checkBoxGitAppDefaultServerApp.Size = new System.Drawing.Size(164, 17);
            this.checkBoxGitAppDefaultServerApp.TabIndex = 18;
            this.checkBoxGitAppDefaultServerApp.Text = "Default application on server.";
            this.checkBoxGitAppDefaultServerApp.UseVisualStyleBackColor = true;
            // 
            // checkBoxGitAppDnsRecord
            // 
            this.checkBoxGitAppDnsRecord.AutoSize = true;
            this.checkBoxGitAppDnsRecord.Checked = true;
            this.checkBoxGitAppDnsRecord.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxGitAppDnsRecord.Location = new System.Drawing.Point(98, 90);
            this.checkBoxGitAppDnsRecord.Name = "checkBoxGitAppDnsRecord";
            this.checkBoxGitAppDnsRecord.Size = new System.Drawing.Size(219, 17);
            this.checkBoxGitAppDnsRecord.TabIndex = 17;
            this.checkBoxGitAppDnsRecord.Text = "Check/create DNS record and Elastic IP";
            this.checkBoxGitAppDnsRecord.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(8, 67);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(84, 13);
            this.label10.TabIndex = 15;
            this.label10.Text = "Application URL";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(32, 41);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(60, 13);
            this.label9.TabIndex = 13;
            this.label9.Text = "Destination";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(23, 15);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(69, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "App. location";
            // 
            // textBoxGitAppUrl
            // 
            this.textBoxGitAppUrl.BackColor = System.Drawing.Color.LightCyan;
            this.textBoxGitAppUrl.Location = new System.Drawing.Point(98, 64);
            this.textBoxGitAppUrl.Name = "textBoxGitAppUrl";
            this.textBoxGitAppUrl.Size = new System.Drawing.Size(405, 20);
            this.textBoxGitAppUrl.TabIndex = 16;
            this.textBoxGitAppUrl.Text = "b1.zwr.fi";
            this.textBoxGitAppUrl.TextChanged += new System.EventHandler(this.textBoxGitAppUrl_TextChanged);
            // 
            // textBoxGitApp
            // 
            this.textBoxGitApp.BackColor = System.Drawing.Color.LightCyan;
            this.textBoxGitApp.Location = new System.Drawing.Point(98, 38);
            this.textBoxGitApp.Name = "textBoxGitApp";
            this.textBoxGitApp.Size = new System.Drawing.Size(405, 20);
            this.textBoxGitApp.TabIndex = 14;
            this.textBoxGitApp.Text = "b1";
            this.textBoxGitApp.TextChanged += new System.EventHandler(this.textBoxGitApp_TextChanged);
            // 
            // textBoxGitAppLocation
            // 
            this.textBoxGitAppLocation.BackColor = System.Drawing.Color.LightCyan;
            this.textBoxGitAppLocation.Location = new System.Drawing.Point(98, 12);
            this.textBoxGitAppLocation.Name = "textBoxGitAppLocation";
            this.textBoxGitAppLocation.Size = new System.Drawing.Size(405, 20);
            this.textBoxGitAppLocation.TabIndex = 12;
            this.textBoxGitAppLocation.Text = "git@github.com:zmilojko/b1.git";
            this.textBoxGitAppLocation.TextChanged += new System.EventHandler(this.textBoxGitAppLocation_TextChanged);
            // 
            // comboBoxGitAppType
            // 
            this.comboBoxGitAppType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxGitAppType.FormattingEnabled = true;
            this.comboBoxGitAppType.Items.AddRange(new object[] {
            "Rails application",
            "Generic application"});
            this.comboBoxGitAppType.Location = new System.Drawing.Point(98, 113);
            this.comboBoxGitAppType.Name = "comboBoxGitAppType";
            this.comboBoxGitAppType.Size = new System.Drawing.Size(121, 21);
            this.comboBoxGitAppType.TabIndex = 20;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(7, 116);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(85, 13);
            this.label12.TabIndex = 19;
            this.label12.Text = "Application type:";
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(428, 111);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 21;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonOK.Location = new System.Drawing.Point(347, 111);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 21;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // DlgNewApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(518, 145);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.comboBoxGitAppType);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.checkBoxGitAppDefaultServerApp);
            this.Controls.Add(this.checkBoxGitAppDnsRecord);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.textBoxGitAppUrl);
            this.Controls.Add(this.textBoxGitApp);
            this.Controls.Add(this.textBoxGitAppLocation);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DlgNewApp";
            this.Text = "New application for the EC2";
            this.Load += new System.EventHandler(this.DlgNewApp_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.CheckBox checkBoxGitAppDefaultServerApp;
        public System.Windows.Forms.CheckBox checkBoxGitAppDnsRecord;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label7;
        public System.Windows.Forms.TextBox textBoxGitAppUrl;
        public System.Windows.Forms.TextBox textBoxGitApp;
        public System.Windows.Forms.TextBox textBoxGitAppLocation;
        public System.Windows.Forms.ComboBox comboBoxGitAppType;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
    }
}