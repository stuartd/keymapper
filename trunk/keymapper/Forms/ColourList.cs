using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace KeyMapper
{


	public partial class ColourList : KMBaseForm
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
		string _toolTipText = "Double-click a button to edit the colour: right click for more options";
		
		#endregion

		public ColourList()
		{
			InitializeComponent();
			CreateContextMenu();

			Redraw();

			MappingsManager.MappingsChanged += delegate(object sender, EventArgs e) { Redraw(); };
			UserColourSettingManager.ColoursChanged += delegate(object sender, EventArgs e) { Redraw(false); };

		}

		private void CreateContextMenu()
		{
			_contextMenu = new ContextMenu();
			int newItemIndex;

			newItemIndex = _contextMenu.MenuItems.Add(new MenuItem("Show All Buttons", ShowAllButtons));
			if (_showAllButtons)
				_contextMenu.MenuItems[newItemIndex].Checked = true;

			newItemIndex = _contextMenu.MenuItems.Add(new MenuItem("Show Current Buttons Only", ShowCurrentButtons));
			if (_showAllButtons == false)
				_contextMenu.MenuItems[newItemIndex].Checked = true;

			_contextMenu.MenuItems.Add(new MenuItem("Reset All Colours", ResetAllColours));
			this.ContextMenu = _contextMenu;

		}

		private void ResetAllColours(object sender, EventArgs e)
		{
			// Two ways of doing this:
			// 1) Go through all effects, get default values, and save 
			// (Like Reset button in Editor form.)

			// Or, 2) - delete the UserColours registry subkey!

			string subkey = AppController.ApplicationRegistryKeyName + @"\UserColours\";
			RegistryKey k = Registry.CurrentUser.OpenSubKey(subkey, true) ;
			if (k != null)
			{
				k.Close() ;
				Registry.CurrentUser.DeleteSubKeyTree(subkey);
			}

			UserColourSettingManager.RaiseColoursChangedEvent();

		}


		private void ShowAllButtons(object sender, EventArgs e)
		{
			_showAllButtons = true;
			CreateContextMenu();
			Redraw();
		}

		private void ShowCurrentButtons(object sender, EventArgs e)
		{
			_showAllButtons = false;
			CreateContextMenu();
			Redraw();
		}


		void ResetFields()
		{
			_mappedkeys = false;
			_disabledkeys = false;

			_pendingdisabled = false;
			_pendingmapped = false;

			_pendingenabled = false;
			_pendingunmapped = false;
		}

		private void ConstrainForm()
		{

			// One button, two buttons, three buttons - show in one line
			// Four buttons: Show in tow rows of two
			// Four buttons, five buttons, six buttons - show in two rows of three
			// Seven buttons, eight buttons - show in two rows of four.

			switch (_buttonCount)
			{
				case 1:
				case 2:
				case 3:
					_buttonsPerLine = _buttonCount;
					_numberOfLines = 1;
					break;
				case 4:
					_buttonsPerLine = 2;
					_numberOfLines = 2;
					break;
				case 5:
				case 6:
					_buttonsPerLine = 3;
					_numberOfLines = 2;
					break;
				case 7:
				case 8:
					_buttonsPerLine = 4;
					_numberOfLines = 2;
					break;
			}

			_buttonScaleFactor = 0.75F;

			_buttonWidth = (int)(192 * _buttonScaleFactor);
			_buttonHeight = (int)(128 * _buttonScaleFactor);

			// Now work out how big the form should be.
			// Width: Number of buttons per line * buttonsize + (buttons + 1 * padding)
			int width = (_buttonsPerLine * _buttonWidth) + ((_buttonsPerLine + 1) * _padding);
			// Height: Number of lines * buttonehight + (number of lines +1 * padding)
			int height = (_numberOfLines * _buttonHeight) + ((_numberOfLines + 1) * _padding);

			this.ClientSize = new Size(width, height);

		}

		private void Redraw()
		{
			Redraw(true);
		}

		private void Redraw(bool reloadMappings)
		{
			_toolTip.RemoveAll();
			_toolTip.SetToolTip(this, _toolTipText);

			if (reloadMappings)
			{
				ResetFields();
				GetButtons();
			}

			for (int i = this.Controls.Count - 1; i >= 0; i--)
				this.Controls[i].Dispose();

			this.SuspendLayout();
			this.Refresh();
			this.ResumeLayout();

			ConstrainForm();

			AddButtons();
		}

		private void AddButtons()
		{

			_currentButton = 1;

			AddButton("Normal", ButtonEffect.None);

			if (_showAllButtons || _mappedkeys)
				AddButton("Mapped", ButtonEffect.Mapped);

			if (_showAllButtons || _pendingmapped)
				AddButton("Pending Mapped", ButtonEffect.MappedPending);

			if (_showAllButtons || _pendingunmapped)
				AddButton("Pending Unmapped", ButtonEffect.UnmappedPending);

			if (_showAllButtons || _disabledkeys)
				AddButton("Disabled", ButtonEffect.Disabled);

			if (_showAllButtons || _pendingdisabled)
				AddButton("Pending Disabled", ButtonEffect.DisabledPending);

			if (_showAllButtons || _pendingenabled)
				AddButton("Pending Enabled", ButtonEffect.EnabledPending);

			if (_showAllButtons)
				AddButton("Mapping Disallowed", ButtonEffect.NoMappingAllowed);

		}

		private void GetButtons()
		{

			if (_showAllButtons)
			{
				_buttonCount = 8;
				return;
			}

			// Assume there are some normal unmapped keys!
			_buttonCount = 1;

			// See what's currently mapped by looking at the current mapping list
			Collection<KeyMapping> currentMaps = MappingsManager.GetMappings(MappingFilter.Current);

			foreach (KeyMapping map in currentMaps)
			{

				if (MappingsManager.IsMappingPending(map, MappingFilter.All))
				{
					// Pending
					if (MappingsManager.IsDisabledMapping(map))
					{
						if (!_pendingdisabled)
						{
							_pendingdisabled = true;
							_buttonCount++;
						}
					}
					else
					{
						if (!_pendingmapped)
						{
							_pendingmapped = true;
							_buttonCount++;
						}
					}
				}
				else
				{
					// Actual 
					if (MappingsManager.IsDisabledMapping(map))
					{
						if (!_disabledkeys)
						{
							_disabledkeys = true;
							_buttonCount++;
						}
					}
					else
					{
						if (!_mappedkeys)
						{
							_mappedkeys = true;
							_buttonCount++;
						}
					}
				}
			}

			// Now look at the cleared keys.

			Collection<KeyMapping> maps = MappingsManager.ClearedMappings;

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
					if (!_pendingenabled)
					{
						_pendingenabled = true;
						_buttonCount++;
					}
				}
				else
				{
					if (!_pendingunmapped)
					{
						_pendingunmapped = true;
						_buttonCount++;
					}
				}
			}
		}


		void AddButton(string text, ButtonEffect effect)
		{

			PictureBox pb = new PictureBox();
			pb.Image = ButtonImages.GetButtonImage(BlankButton.MediumWideBlank, _buttonScaleFactor, text, effect);
			pb.Height = pb.Image.Height;
			pb.Width = pb.Image.Width;

			int position;
			int line;

			// First, see which line the button is in and what position in the line it occupies.
			if (_numberOfLines == 1 || _currentButton <= _buttonsPerLine)
			{
				position = _currentButton;
				line = 1;
			}
			else
			{
				position = _currentButton - _buttonsPerLine;
				line = 2;
			}

			pb.Left = ((position - 1) * _buttonWidth) + (position * _padding);
			pb.Top = ((line - 1) * _buttonHeight) + (line * _padding);

			pb.Tag = effect.ToString() + " " + text;

			pb.DoubleClick += PictureBoxDoubleClick;

			this.Controls.Add(pb);
			_toolTip.SetToolTip(pb, _toolTipText);

			_currentButton++;


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
			userSettings.Save();
		}


	}

}



