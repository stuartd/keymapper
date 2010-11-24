using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using KeyMapper.Classes;

namespace KeyMapper
{
    public partial class HelpForm : KMBaseForm
    {
        public HelpForm()
        {
            InitializeComponent();
            LoadUserSettings();
            this.FormClosed += FormsManager.ChildFormClosed;
            this.labelFAQ.Links[0].LinkData = "http://justkeepswimming.net/keymapper/faq.aspx";
        }

        private void LoadUserSettings()
        {
            Properties.Settings userSettings = new KeyMapper.Properties.Settings();
            chkShowHelpAtStartup.Checked = userSettings.ShowHelpFormAtStartup;
        }

   


        private void HelpFormFormClosed(object sender, FormClosedEventArgs e)
        {
            SaveUserSettings();
        }

        private void SaveUserSettings()
        {
            Properties.Settings userSettings = new KeyMapper.Properties.Settings();
            userSettings.ShowHelpFormAtStartup = chkShowHelpAtStartup.Checked;
            userSettings.HelpFormLocation = this.Location;
            userSettings.Save();
        }

        private void labelFAQClick(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url = e.Link.LinkData as string;
            if (string.IsNullOrEmpty(url) == false)
                System.Diagnostics.Process.Start(url);
        }



    }


}
