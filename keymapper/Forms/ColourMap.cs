using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using KeyMapper.Classes;
using Microsoft.Win32;

namespace KeyMapper.Forms
{


	public partial class ColourMap : KMBaseForm
	{

		#region fields

		int _buttonCount;
		int _currentButton;
		int _padding = 2;
		int _buttonHeight;
		int _buttonWidth;
		int _buttonsPerLine;
		int _numberOfLines;
		float _buttonScaleFactor;

		bool _showAllButtons;

		// Are any keys actually mapped?
		bool _mappedkeys;
		bool _disabledkeys;

		// What about keys that will be mapped after next reboot/logon?
		bool _pendingdisabled;
		bool _pendingmapped;

		// Are there keys which were mapped but have now been cleared?
		bool _pendingenabled;
		bool _pendingunmapped;

		ContextMenu _contextMenu;

		ToolTip _toolTip = new ToolTip();

		string _toolTipText;

		#endregion

		public ColourMap()
		{
			if (AppController.UserCannotWriteToApplicationRegistryKey)
				this._toolTipText = "Right-click to change which buttons are shown";
			else
				this._toolTipText = "Double-click a button to edit the colour: right click for more options";

			InitializeComponent();

			Properties.Settings userSettings = new Properties.Settings();
			this._showAllButtons = userSettings.ColourMapShowAllButtons;

			CreateContextMenu();
			Redraw();

			MappingsManager.MappingsChanged += delegate(object sender, EventArgs e) { Redraw(); };
			UserColourSettingManager.ColoursChanged += delegate(object sender, EventArgs e) { Redraw(false); };

		}

		private void CreateContextMenu()
		{
			this._contextMenu = new ContextMenu();
			int newItemIndex;

			newItemIndex = this._contextMenu.MenuItems.Add(new MenuItem("Show All Buttons", ShowAllButtons));
			if (this._showAllButtons)
				this._contextMenu.MenuItems[newItemIndex].Checked = true;

			newItemIndex = this._contextMenu.MenuItems.Add(new MenuItem("Show Currently Active Buttons Only", ShowCurrentButtons));
			if (this._showAllButtons == false)
				this._contextMenu.MenuItems[newItemIndex].Checked = true;

			if (AppController.UserCannotWriteToApplicationRegistryKey == false)
			{
				this._contextMenu.MenuItems.Add(new MenuItem("Reset All Colours", ResetAllColours));
				this._contextMenu.MenuItems.Add(new MenuItem("Close All Editor Forms", CloseAllEditorForms));
			}

			this.ContextMenu = this._contextMenu;


		}

		private void CloseAllEditorForms(object sender, EventArgs e)
		{
			FormsManager.CloseAllEditorForms();
		}



		private void ResetAllColours(object sender, EventArgs e)
		{
			// Two ways of doing this:
			// 1) Go through all effects, get default values, and save 
			// (Like Reset button in Editor form.)

			// Or, 2) - delete the UserColours registry subkey!

			string subkey = AppController.ApplicationRegistryKeyName + @"\UserColours\";
			RegistryKey k = Registry.CurrentUser.OpenSubKey(subkey, true);
			if (k != null)
			{
				k.Close();
				Registry.CurrentUser.DeleteSubKeyTree(subkey);
			}

			UserColourSettingManager.RaiseColoursChangedEvent();

		}


		private void ShowAllButtons(object sender, EventArgs e)
		{
			this._showAllButtons = true;
			CreateContextMenu();
			Redraw();
		}

		private void ShowCurrentButtons(object sender, EventArgs e)
		{
			this._showAllButtons = false;
			CreateContextMenu();
			Redraw();
		}


		void ResetFields()
		{
			this._mappedkeys = false;
			this._disabledkeys = false;

			this._pendingdisabled = false;
			this._pendingmapped = false;

			this._pendingenabled = false;
			this._pendingunmapped = false;
		}

		private void ConstrainForm()
		{

			// One button, two buttons, three buttons - show in one line
			// Four buttons: Show in tow rows of two
			// Four buttons, five buttons, six buttons - show in two rows of three
			// Seven buttons, eight buttons - show in two rows of four.

			switch (this._buttonCount)
			{
				case 1:
				case 2:
				case 3:
					this._buttonsPerLine = this._buttonCount;
					this._numberOfLines = 1;
					break;
				case 4:
					this._buttonsPerLine = 2;
					this._numberOfLines = 2;
					break;
				case 5:
				case 6:
					this._buttonsPerLine = 3;
					this._numberOfLines = 2;
					break;
				case 7:
				case 8:
					this._buttonsPerLine = 4;
					this._numberOfLines = 2;
					break;
			}

			// Scale the button size down depending on screen resolution width..
			// Form hasn't been positioned yet, so look at primary monitor resolution.

			int screenWidth = SystemInformation.PrimaryMonitorSize.Width;

			if (screenWidth < 801)
				this._buttonScaleFactor = 0.35F;
			else if (screenWidth < 1025)
				this._buttonScaleFactor = 0.4F;
			else if (screenWidth < 1281)
				this._buttonScaleFactor = 0.5F;
			else
				this._buttonScaleFactor = 0.5F;

			this._buttonWidth = (int)(192 * this._buttonScaleFactor);
			this._buttonHeight = (int)(128 * this._buttonScaleFactor);

			// Now work out how big the form should be.
			// Width: Number of buttons per line * buttonsize + (buttons + 1 * padding)
			int width = (this._buttonsPerLine * this._buttonWidth) + ((this._buttonsPerLine + 1) * this._padding);
			// Height: Number of lines * buttonhight + (number of lines +1 * padding)
			int height = (this._numberOfLines * this._buttonHeight) + ((this._numberOfLines + 1) * this._padding);

			this.ClientSize = new Size(width, height);

		}

		private void Redraw()
		{
			Redraw(true);
		}

		private void Redraw(bool reloadMappings)
		{
			this.SuspendLayout();

			this._toolTip.RemoveAll();
			this._toolTip.SetToolTip(this, this._toolTipText);

			if (reloadMappings)
			{
				ResetFields();
				GetButtons();
			}

			for (int i = this.Controls.Count - 1; i >= 0; i--)
				this.Controls[i].Dispose();

			// In order to fix a problem where the form is resized to smaller then the enforced minimum
			// size where the bits of the form which aren then still visible aren't repainted, if the button count is one then
			// constrain twice - once with a count of two then a refresh to clear the background, and then with the count reset to one.

			if (this._buttonCount == 1)
			{
				this._buttonCount = 2;
				ConstrainForm();
				this.Refresh();
				this._buttonCount = 1;
			}

			ConstrainForm();

			AddButtons();

			if (this._buttonCount > 1)
				this.Text = "KeyMapper Colour Map";
			else
				this.Text = "Colour Map";

			this.ResumeLayout();
		}

		private void AddButtons()
		{

			this._currentButton = 1;

			AddButton("Normal", ButtonEffect.None);

			if (this._showAllButtons || this._mappedkeys)
				AddButton("Mapped", ButtonEffect.Mapped);

			if (this._showAllButtons || this._pendingmapped)
				AddButton("Pending Mapped", ButtonEffect.MappedPending);

			if (this._showAllButtons || this._pendingunmapped)
				AddButton("Pending Unmapped", ButtonEffect.UnmappedPending);

			if (this._showAllButtons || this._disabledkeys)
				AddButton("Disabled", ButtonEffect.Disabled);

			if (this._showAllButtons || this._pendingdisabled)
				AddButton("Pending Disabled", ButtonEffect.DisabledPending);

			if (this._showAllButtons || this._pendingenabled)
				AddButton("Pending Enabled", ButtonEffect.EnabledPending);

		}

		private void GetButtons()
		{

			if (this._showAllButtons)
			{
				this._buttonCount = 8;
				return;
			}

			// Assume there are some normal unmapped keys!
			this._buttonCount = 1;

			// See what's currently mapped by looking at the current mapping list
			Collection<KeyMapping> currentMaps = MappingsManager.GetMappings(MappingFilter.Current);

			foreach (KeyMapping map in currentMaps)
			{

				if (MappingsManager.IsMappingPending(map, MappingFilter.All))
				{
					// Pending
					if (MappingsManager.IsDisabledMapping(map))
					{
						if (!this._pendingdisabled)
						{
							this._pendingdisabled = true;
							this._buttonCount++;
						}
					}
					else
					{
						if (!this._pendingmapped)
						{
							this._pendingmapped = true;
							this._buttonCount++;
						}
					}
				}
				else
				{
					// Actual 
					if (MappingsManager.IsDisabledMapping(map))
					{
						if (!this._disabledkeys)
						{
							this._disabledkeys = true;
							this._buttonCount++;
						}
					}
					else
					{
						if (!this._mappedkeys)
						{
							this._mappedkeys = true;
							this._buttonCount++;
						}
					}
				}
			}

			// Now look at the cleared keys.

			IEnumerable<KeyMapping> maps = MappingsManager.ClearedMappings;

			foreach (KeyMapping map in maps)
			{
				// Has this cleared key been remapped (in which case we ignore it)
				bool remapped = false;
				foreach (KeyMapping currentmap in MappingsManager.GetMappings(MappingFilter.Current))
				{
					if (currentmap.From == map.From)
					{
						remapped = true;
						break;
					}
				}

				if (remapped)
					continue;

				if (MappingsManager.IsDisabledMapping(map))
				{
					if (!this._pendingenabled)
					{
						this._pendingenabled = true;
						this._buttonCount++;
					}
				}
				else
				{
					if (!this._pendingunmapped)
					{
						this._pendingunmapped = true;
						this._buttonCount++;
					}
				}
			}
		}


		void AddButton(string text, ButtonEffect effect)
		{
            PictureBox pb = new PictureBox
                                {
                                    Image = ButtonImages.GetButtonImage
                                    (BlankButton.MediumWideBlank, this._buttonScaleFactor, text, effect)
                                };

		    pb.Height = pb.Image.Height;
			pb.Width = pb.Image.Width;

			// If there is only one button, contain it in the centre of the form (the minimum size for a form
			// is bigger than a button until button scale gets to 0.6 or so which is too big for small resolutions)

			if (this._buttonCount == 1)
			{
				// Forms have a minimum size of 123. Applying a slight kludge factor too.
				pb.Left = (((123 - SystemInformation.BorderSize.Width - this._buttonWidth) / 2) - 5);
				pb.Top = this._padding;
			}
			else
			{
				int position;
				int line;

				// First, see which line the button is in and what position in the line it occupies.
				if (this._numberOfLines == 1 || this._currentButton <= this._buttonsPerLine)
				{
					position = this._currentButton;
					line = 1;
				}
				else
				{
					position = this._currentButton - this._buttonsPerLine;
					line = 2;
				}

				pb.Left = ((position - 1) * this._buttonWidth) + (position * this._padding);
				pb.Top = ((line - 1) * this._buttonHeight) + (line * this._padding);

			}

			pb.Tag = effect.ToString() + " " + text;

			if (AppController.UserCannotWriteToApplicationRegistryKey == false)
				pb.DoubleClick += PictureBoxDoubleClick;

			this.Controls.Add(pb);
			this._toolTip.SetToolTip(pb, this._toolTipText);

			this._currentButton++;


		}


		void PictureBoxDoubleClick(object sender, EventArgs e)
		{
			PictureBox pb = sender as PictureBox;
			if (pb != null && pb.Tag != null)
				FormsManager.ShowColourEditorForm(pb.Tag.ToString());
		}

		private void ColourMapFormClosing(object sender, FormClosingEventArgs e)
		{
			SaveSettings();
		}

		private void SaveSettings()
		{
			Properties.Settings userSettings = new Properties.Settings();
			userSettings.ColourListFormLocation = this.Location;
			userSettings.ColourMapShowAllButtons = this._showAllButtons;
			userSettings.Save();
		}

	}

}



