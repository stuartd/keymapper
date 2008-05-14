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
			uint msg = 0xA1 ; // WM_NCLBUTTONDOWN 
			IntPtr wparam = (IntPtr)0x2; // HT_CAPTION 

			if (e.Button == MouseButtons.Left)
			{
				NativeMethods.ReleaseCapture();
				NativeMethods.SendMessage(Handle, msg, wparam, IntPtr.Zero);
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
