namespace ZAws.Console
{
    partial class DlgEditDnsRecord
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DlgEditDnsRecord));
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.comboBoxRecordType = new System.Windows.Forms.ComboBox();
            this.textBoxValue = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxTTL = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.listBoxOptions = new System.Windows.Forms.ListBox();
            this.buttonInsertSelectedOption = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonClose
            // 
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Location = new System.Drawing.Point(549, 209);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 1;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(468, 209);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(51, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Name";
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(92, 12);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(100, 20);
            this.textBoxName.TabIndex = 3;
            // 
            // comboBoxRecordType
            // 
            this.comboBoxRecordType.BackColor = System.Drawing.Color.White;
            this.comboBoxRecordType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxRecordType.FormattingEnabled = true;
            this.comboBoxRecordType.Items.AddRange(new object[] {
            "A",
            "AAAA",
            "CNAME",
            "MX",
            "TXT",
            "PTR",
            "SRV",
            "SPF",
            "NS",
            "SOA"});
            this.comboBoxRecordType.Location = new System.Drawing.Point(92, 38);
            this.comboBoxRecordType.Name = "comboBoxRecordType";
            this.comboBoxRecordType.Size = new System.Drawing.Size(72, 21);
            this.comboBoxRecordType.TabIndex = 4;
            this.comboBoxRecordType.SelectedIndexChanged += new System.EventHandler(this.comboBoxRecordType_SelectedIndexChanged);
            // 
            // textBoxValue
            // 
            this.textBoxValue.Location = new System.Drawing.Point(92, 65);
            this.textBoxValue.Multiline = true;
            this.textBoxValue.Name = "textBoxValue";
            this.textBoxValue.Size = new System.Drawing.Size(248, 95);
            this.textBoxValue.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(55, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Type";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(52, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Value";
            // 
            // textBoxTTL
            // 
            this.textBoxTTL.Location = new System.Drawing.Point(92, 166);
            this.textBoxTTL.Name = "textBoxTTL";
            this.textBoxTTL.Size = new System.Drawing.Size(100, 20);
            this.textBoxTTL.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(59, 169);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(27, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "TTL";
            // 
            // listBoxOptions
            // 
            this.listBoxOptions.FormattingEnabled = true;
            this.listBoxOptions.Location = new System.Drawing.Point(390, 65);
            this.listBoxOptions.Name = "listBoxOptions";
            this.listBoxOptions.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxOptions.Size = new System.Drawing.Size(234, 95);
            this.listBoxOptions.TabIndex = 5;
            this.listBoxOptions.SelectedIndexChanged += new System.EventHandler(this.listBoxOptions_SelectedIndexChanged);
            // 
            // buttonInsertSelectedOption
            // 
            this.buttonInsertSelectedOption.Enabled = false;
            this.buttonInsertSelectedOption.Location = new System.Drawing.Point(346, 92);
            this.buttonInsertSelectedOption.Name = "buttonInsertSelectedOption";
            this.buttonInsertSelectedOption.Size = new System.Drawing.Size(38, 40);
            this.buttonInsertSelectedOption.TabIndex = 6;
            this.buttonInsertSelectedOption.Text = "<";
            this.buttonInsertSelectedOption.UseVisualStyleBackColor = true;
            this.buttonInsertSelectedOption.Click += new System.EventHandler(this.buttonInsertSelectedOption_Click);
            // 
            // DlgEditDnsRecord
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.buttonClose;
            this.ClientSize = new System.Drawing.Size(636, 244);
            this.Controls.Add(this.buttonInsertSelectedOption);
            this.Controls.Add(this.listBoxOptions);
            this.Controls.Add(this.comboBoxRecordType);
            this.Controls.Add(this.textBoxValue);
            this.Controls.Add(this.textBoxTTL);
            this.Controls.Add(this.textBoxName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DlgEditDnsRecord";
            this.Text = "Edit DNS record";
            this.Load += new System.EventHandler(this.DlgEditDnsRecord_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.ComboBox comboBoxRecordType;
        private System.Windows.Forms.TextBox textBoxValue;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxTTL;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox listBoxOptions;
        private System.Windows.Forms.Button buttonInsertSelectedOption;
    }
}