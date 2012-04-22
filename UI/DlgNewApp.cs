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
    public partial class DlgNewApp : Form
    {
        public DlgNewApp()
        {
            InitializeComponent();
            comboBoxGitAppType.SelectedIndex = 0;
        }

        bool directoryWasNotChangedManually = true;
        bool nowChangingDirectoryAutomatically = false;
        private void textBoxGitAppLocation_TextChanged(object sender, EventArgs e)
        {
            if (directoryWasNotChangedManually)
            {
                nowChangingDirectoryAutomatically = true;
                try
                {
                    textBoxGitApp.Text = textBoxGitAppLocation.Text.Substring(textBoxGitAppLocation.Text.LastIndexOf("/") + 1);
                    if (textBoxGitApp.Text.Contains(".git"))
                    {
                        textBoxGitApp.Text = textBoxGitApp.Text.Substring(0, textBoxGitApp.Text.IndexOf(".git"));
                    }
                }
                catch { }
                nowChangingDirectoryAutomatically = false;
            }
            TextChanged();
        }

        private void textBoxGitApp_TextChanged(object sender, EventArgs e)
        {
            if (nowChangingDirectoryAutomatically)
            {
                return;
            }
            directoryWasNotChangedManually = string.IsNullOrWhiteSpace(textBoxGitApp.Text);
            TextChanged();
        }

        private void textBoxGitAppUrl_TextChanged(object sender, EventArgs e)
        {
            TextChanged();
        }

        new void TextChanged()
        {
            buttonOK.Enabled = !(string.IsNullOrWhiteSpace(textBoxGitApp.Text)
                                || string.IsNullOrWhiteSpace(textBoxGitAppLocation.Text)
                                || string.IsNullOrWhiteSpace(textBoxGitAppUrl.Text));
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void DlgNewApp_Load(object sender, EventArgs e)
        {
            TextChanged();
        }
    }
}
