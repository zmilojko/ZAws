namespace ZAws.Console
{
    partial class PopupEc2Properties
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PopupEc2Properties));
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxType = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.labelKeyPair = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.textBoxZone = new System.Windows.Forms.TextBox();
            this.textBoxSecGroups = new System.Windows.Forms.TextBox();
            this.textBoxKeyPair = new System.Windows.Forms.TextBox();
            this.textBoxPublicDns = new System.Windows.Forms.TextBox();
            this.textBoxPrivateDns = new System.Windows.Forms.TextBox();
            this.textBoxPrivateIp = new System.Windows.Forms.TextBox();
            this.buttonChangeName = new System.Windows.Forms.Button();
            this.textBoxElasticIp = new System.Windows.Forms.TextBox();
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonReboot = new System.Windows.Forms.Button();
            this.buttonTerminate = new System.Windows.Forms.Button();
            this.buttonTerminal = new System.Windows.Forms.Button();
            this.buttonFileBrowser = new System.Windows.Forms.Button();
            this.buttonInstallApp = new System.Windows.Forms.Button();
            this.buttonRunScript = new System.Windows.Forms.Button();
            this.buttonMonitor = new System.Windows.Forms.Button();
            this.buttonConsoleOutput = new System.Windows.Forms.Button();
            this.buttonWWW = new System.Windows.Forms.Button();
            this.buttonWWWip = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxName
            // 
            this.textBoxName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxName.Location = new System.Drawing.Point(95, 12);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(169, 20);
            this.textBoxName.TabIndex = 1;
            this.textBoxName.TextChanged += new System.EventHandler(this.textBoxName_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(54, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name";
            // 
            // textBoxType
            // 
            this.textBoxType.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxType.Location = new System.Drawing.Point(95, 38);
            this.textBoxType.Name = "textBoxType";
            this.textBoxType.ReadOnly = true;
            this.textBoxType.Size = new System.Drawing.Size(104, 13);
            this.textBoxType.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(58, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Type";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(57, 57);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Zone";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(25, 76);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Sec. groups";
            // 
            // labelKeyPair
            // 
            this.labelKeyPair.AutoSize = true;
            this.labelKeyPair.Location = new System.Drawing.Point(44, 95);
            this.labelKeyPair.Name = "labelKeyPair";
            this.labelKeyPair.Size = new System.Drawing.Size(45, 13);
            this.labelKeyPair.TabIndex = 9;
            this.labelKeyPair.Text = "Key pair";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(27, 114);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(62, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Public DNS";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(23, 133);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(66, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Private DNS";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(38, 152);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(51, 13);
            this.label8.TabIndex = 16;
            this.label8.Text = "Elastic IP";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(36, 171);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 13);
            this.label9.TabIndex = 19;
            this.label9.Text = "Private IP";
            // 
            // textBoxZone
            // 
            this.textBoxZone.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxZone.Location = new System.Drawing.Point(95, 57);
            this.textBoxZone.Name = "textBoxZone";
            this.textBoxZone.ReadOnly = true;
            this.textBoxZone.Size = new System.Drawing.Size(107, 13);
            this.textBoxZone.TabIndex = 6;
            // 
            // textBoxSecGroups
            // 
            this.textBoxSecGroups.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxSecGroups.Location = new System.Drawing.Point(95, 76);
            this.textBoxSecGroups.Name = "textBoxSecGroups";
            this.textBoxSecGroups.ReadOnly = true;
            this.textBoxSecGroups.Size = new System.Drawing.Size(107, 13);
            this.textBoxSecGroups.TabIndex = 8;
            // 
            // textBoxKeyPair
            // 
            this.textBoxKeyPair.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxKeyPair.Location = new System.Drawing.Point(95, 95);
            this.textBoxKeyPair.Name = "textBoxKeyPair";
            this.textBoxKeyPair.ReadOnly = true;
            this.textBoxKeyPair.Size = new System.Drawing.Size(107, 13);
            this.textBoxKeyPair.TabIndex = 10;
            // 
            // textBoxPublicDns
            // 
            this.textBoxPublicDns.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxPublicDns.Location = new System.Drawing.Point(95, 114);
            this.textBoxPublicDns.Name = "textBoxPublicDns";
            this.textBoxPublicDns.ReadOnly = true;
            this.textBoxPublicDns.Size = new System.Drawing.Size(311, 13);
            this.textBoxPublicDns.TabIndex = 12;
            // 
            // textBoxPrivateDns
            // 
            this.textBoxPrivateDns.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxPrivateDns.Location = new System.Drawing.Point(95, 133);
            this.textBoxPrivateDns.Name = "textBoxPrivateDns";
            this.textBoxPrivateDns.ReadOnly = true;
            this.textBoxPrivateDns.Size = new System.Drawing.Size(353, 13);
            this.textBoxPrivateDns.TabIndex = 15;
            // 
            // textBoxPrivateIp
            // 
            this.textBoxPrivateIp.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxPrivateIp.Location = new System.Drawing.Point(95, 171);
            this.textBoxPrivateIp.Name = "textBoxPrivateIp";
            this.textBoxPrivateIp.ReadOnly = true;
            this.textBoxPrivateIp.Size = new System.Drawing.Size(107, 13);
            this.textBoxPrivateIp.TabIndex = 20;
            // 
            // buttonChangeName
            // 
            this.buttonChangeName.Location = new System.Drawing.Point(268, 11);
            this.buttonChangeName.Name = "buttonChangeName";
            this.buttonChangeName.Size = new System.Drawing.Size(41, 22);
            this.buttonChangeName.TabIndex = 2;
            this.buttonChangeName.Text = "Set";
            this.buttonChangeName.UseVisualStyleBackColor = true;
            // 
            // textBoxElasticIp
            // 
            this.textBoxElasticIp.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxElasticIp.Location = new System.Drawing.Point(95, 152);
            this.textBoxElasticIp.Name = "textBoxElasticIp";
            this.textBoxElasticIp.Size = new System.Drawing.Size(107, 13);
            this.textBoxElasticIp.TabIndex = 17;
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(501, 12);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(91, 23);
            this.buttonStart.TabIndex = 21;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Location = new System.Drawing.Point(501, 41);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(91, 23);
            this.buttonStop.TabIndex = 22;
            this.buttonStop.Text = "Stop";
            this.buttonStop.UseVisualStyleBackColor = true;
            // 
            // buttonReboot
            // 
            this.buttonReboot.Location = new System.Drawing.Point(501, 70);
            this.buttonReboot.Name = "buttonReboot";
            this.buttonReboot.Size = new System.Drawing.Size(91, 23);
            this.buttonReboot.TabIndex = 23;
            this.buttonReboot.Text = "Reboot";
            this.buttonReboot.UseVisualStyleBackColor = true;
            // 
            // buttonTerminate
            // 
            this.buttonTerminate.Location = new System.Drawing.Point(501, 99);
            this.buttonTerminate.Name = "buttonTerminate";
            this.buttonTerminate.Size = new System.Drawing.Size(91, 23);
            this.buttonTerminate.TabIndex = 24;
            this.buttonTerminate.Text = "Terminate";
            this.buttonTerminate.UseVisualStyleBackColor = true;
            // 
            // buttonTerminal
            // 
            this.buttonTerminal.Location = new System.Drawing.Point(501, 142);
            this.buttonTerminal.Name = "buttonTerminal";
            this.buttonTerminal.Size = new System.Drawing.Size(91, 23);
            this.buttonTerminal.TabIndex = 25;
            this.buttonTerminal.Text = "Terminal";
            this.buttonTerminal.UseVisualStyleBackColor = true;
            // 
            // buttonFileBrowser
            // 
            this.buttonFileBrowser.Location = new System.Drawing.Point(501, 171);
            this.buttonFileBrowser.Name = "buttonFileBrowser";
            this.buttonFileBrowser.Size = new System.Drawing.Size(91, 23);
            this.buttonFileBrowser.TabIndex = 26;
            this.buttonFileBrowser.Text = "File browser";
            this.buttonFileBrowser.UseVisualStyleBackColor = true;
            // 
            // buttonInstallApp
            // 
            this.buttonInstallApp.Location = new System.Drawing.Point(501, 218);
            this.buttonInstallApp.Name = "buttonInstallApp";
            this.buttonInstallApp.Size = new System.Drawing.Size(91, 23);
            this.buttonInstallApp.TabIndex = 27;
            this.buttonInstallApp.Text = "Install app.";
            this.buttonInstallApp.UseVisualStyleBackColor = true;
            // 
            // buttonRunScript
            // 
            this.buttonRunScript.Location = new System.Drawing.Point(501, 247);
            this.buttonRunScript.Name = "buttonRunScript";
            this.buttonRunScript.Size = new System.Drawing.Size(91, 23);
            this.buttonRunScript.TabIndex = 28;
            this.buttonRunScript.Text = "Run script";
            this.buttonRunScript.UseVisualStyleBackColor = true;
            // 
            // buttonMonitor
            // 
            this.buttonMonitor.Location = new System.Drawing.Point(501, 288);
            this.buttonMonitor.Name = "buttonMonitor";
            this.buttonMonitor.Size = new System.Drawing.Size(91, 23);
            this.buttonMonitor.TabIndex = 29;
            this.buttonMonitor.Text = "Monitor";
            this.buttonMonitor.UseVisualStyleBackColor = true;
            this.buttonMonitor.Click += new System.EventHandler(this.buttonMonitor_Click);
            // 
            // buttonConsoleOutput
            // 
            this.buttonConsoleOutput.Location = new System.Drawing.Point(501, 317);
            this.buttonConsoleOutput.Name = "buttonConsoleOutput";
            this.buttonConsoleOutput.Size = new System.Drawing.Size(91, 23);
            this.buttonConsoleOutput.TabIndex = 30;
            this.buttonConsoleOutput.Text = "Console output";
            this.buttonConsoleOutput.UseVisualStyleBackColor = true;
            this.buttonConsoleOutput.Click += new System.EventHandler(this.buttonConsoleOutput_Click);
            // 
            // buttonWWW
            // 
            this.buttonWWW.Location = new System.Drawing.Point(412, 109);
            this.buttonWWW.Name = "buttonWWW";
            this.buttonWWW.Size = new System.Drawing.Size(41, 22);
            this.buttonWWW.TabIndex = 13;
            this.buttonWWW.Text = "www";
            this.buttonWWW.UseVisualStyleBackColor = true;
            // 
            // buttonWWWip
            // 
            this.buttonWWWip.Location = new System.Drawing.Point(208, 147);
            this.buttonWWWip.Name = "buttonWWWip";
            this.buttonWWWip.Size = new System.Drawing.Size(41, 22);
            this.buttonWWWip.TabIndex = 18;
            this.buttonWWWip.Text = "www";
            this.buttonWWWip.UseVisualStyleBackColor = true;
            // 
            // PopupEc2Properties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(604, 382);
            this.Controls.Add(this.buttonConsoleOutput);
            this.Controls.Add(this.buttonMonitor);
            this.Controls.Add(this.buttonRunScript);
            this.Controls.Add(this.buttonInstallApp);
            this.Controls.Add(this.buttonFileBrowser);
            this.Controls.Add(this.buttonTerminal);
            this.Controls.Add(this.buttonTerminate);
            this.Controls.Add(this.buttonReboot);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.buttonWWWip);
            this.Controls.Add(this.buttonWWW);
            this.Controls.Add(this.buttonChangeName);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.labelKeyPair);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxPrivateDns);
            this.Controls.Add(this.textBoxPublicDns);
            this.Controls.Add(this.textBoxElasticIp);
            this.Controls.Add(this.textBoxPrivateIp);
            this.Controls.Add(this.textBoxKeyPair);
            this.Controls.Add(this.textBoxSecGroups);
            this.Controls.Add(this.textBoxZone);
            this.Controls.Add(this.textBoxType);
            this.Controls.Add(this.textBoxName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PopupEc2Properties";
            this.Text = "PopupEc2Properties";
            this.Load += new System.EventHandler(this.PopupEc2Properties_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelKeyPair;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBoxZone;
        private System.Windows.Forms.TextBox textBoxSecGroups;
        private System.Windows.Forms.TextBox textBoxKeyPair;
        private System.Windows.Forms.TextBox textBoxPublicDns;
        private System.Windows.Forms.TextBox textBoxPrivateDns;
        private System.Windows.Forms.TextBox textBoxPrivateIp;
        private System.Windows.Forms.Button buttonChangeName;
        private System.Windows.Forms.TextBox textBoxElasticIp;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Button buttonReboot;
        private System.Windows.Forms.Button buttonTerminate;
        private System.Windows.Forms.Button buttonTerminal;
        private System.Windows.Forms.Button buttonFileBrowser;
        private System.Windows.Forms.Button buttonInstallApp;
        private System.Windows.Forms.Button buttonRunScript;
        private System.Windows.Forms.Button buttonMonitor;
        private System.Windows.Forms.Button buttonConsoleOutput;
        private System.Windows.Forms.Button buttonWWW;
        private System.Windows.Forms.Button buttonWWWip;
    }
}