using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace KeyMapper
{
	public partial class HelpForm : KMBaseForm
	{
		public HelpForm()
		{
			InitializeComponent();
			LoadUserSettings();
			this.FormClosed += FormsManager.ChildFormClosed;
			this.MouseDown += HelpFormMouseDown;
			
		}

		private void LoadUserSettings()
		{
			Properties.Settings userSettings = new KeyMapper.Properties.Settings();
			chkShowHelpAtStartup.Checked = userSettings.ShowHelpFormAtAtStartup;
		}

		private void HelpFormMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			const int WM_NCLBUTTONDOWN = 0xA1;
			const int HT_CAPTION = 0x2;

			if (e.Button == MouseButtons.Left)
			{
				NativeMethods.ReleaseCapture();
				NativeMethods.SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
			}
		}


		private void HelpFormFormClosed(object sender, FormClosedEventArgs e)
		{
			SaveUserSettings();
		}

		private void SaveUserSettings()
		{
			Properties.Settings userSettings = new KeyMapper.Properties.Settings();
			userSettings.ShowHelpFormAtAtStartup = chkShowHelpAtStartup.Checked;
			userSettings.HelpFormOpen = false;
			userSettings.HelpFormLocation = this.Location;
			userSettings.Save();
		}

	}


}
