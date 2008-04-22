using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace KeyMapper
{
	public partial class ColourList : KMBaseForm
	{

		#region fields

		int _buttons;
		float _factor;
		int _currentButton;
		int _buttonsize;
		int _padding = 2;

		bool _showAllButtons ;

		// Are any keys actually mapped?
		bool _mappedkeys;
		bool _disabledkeys;

		// What about keys that will be mapped after next reboot/logon?
		bool _pendingdisabled;
		bool _pendingmapped;

		// Are there keys which were mapped but have now been cleared?
		bool _pendingenabled;
		bool _pendingunmapped;

		Size _oldSize;

		#endregion

		public ColourList(Point callingFormLocation, Size callingFormSize)
		{
			InitializeComponent();

			this.MinimumSize = new Size(96, 64);

			LoadSettings(callingFormLocation, callingFormSize);

			Redraw();

			MappingsManager.MappingsChanged += delegate(object sender, EventArgs e) { Redraw(); };
			UserColourSettingManager.ColoursChanged += delegate(object sender, EventArgs e) { Redraw(); };

			this.ResizeEnd += ColourMapResizeEnd;

		}
		
		void ResetFields()
		{
			_mappedkeys = false;
			_disabledkeys = false;

			_pendingdisabled = false;
			_pendingmapped = false;

			_pendingenabled = false;
			_pendingunmapped = false;

			_currentButton = 1;

		}

		private void ConstrainForm()
		{
			// Set size. We need the button count for this.

			// There are up to four buttons per line (MediumWidth = 192 px) and five sets of padding:

			int buttonsPerLine = _buttons / 2 ;

			int totalWidth = (int)((192F * buttonsPerLine) + (_padding * (buttonsPerLine + 1))) + 10;
			_factor = (float)(this.ClientSize.Width) / totalWidth;
			if (_factor > 1)
				_factor = 1;
			_buttonsize = (int)(192 * _factor);

			this.ClientSize = new Size(10 + (_buttonsize * _buttons) + (_buttons * _padding), this.ClientSize.Height);

			Console.WriteLine("Factor: {0} Buttonsize: {1}", _factor, _buttonsize);


		}

		private void Redraw()
		{

			ResetFields();

			for (int i = this.Controls.Count - 1; i >= 0; i--)
				this.Controls[i].Dispose();

			GetButtons();

			ConstrainForm();

			AddButtons();

			_oldSize = Size;

			this.Refresh();

		}

		private void AddButtons()
		{

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
				_buttons = 8;
				return;
			}

			// Assume there are some normal unmapped keys!
			_buttons = 1;

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
							_buttons++;
						}
					}
					else
					{
						if (!_pendingmapped)
						{
							_pendingmapped = true;
							_buttons++;
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
							_buttons++;
						}
					}
					else
					{
						if (!_mappedkeys)
						{
							_mappedkeys = true;
							_buttons++;
						}
					}
				}
			}

			// Now look at the cleared keys.

			Collection<KeyMapping> maps = MappingsManager.GetClearedMappings();

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
						_buttons++;
					}
				}
				else
				{
					if (!_pendingunmapped)
					{
						_pendingunmapped = true;
						_buttons++;
					}
				}
			}
		}


		void AddButton(string text, ButtonEffect effect)
		{

			PictureBox pb = new PictureBox();
			pb.Image = ButtonImages.GetButtonImage(BlankButton.MediumWideBlank, _factor, text, effect);
			pb.Height = pb.Image.Height;
			pb.Width = pb.Image.Width;

			// Placement: Padding * (buttons + 1)
			if (_currentButton < 4)
			{
				pb.Left = ((_currentButton + 1) * _padding) + (_currentButton * pb.Width);
				pb.Top = (_padding);
			}
			else
			{
				pb.Left = ((_currentButton - 3) * _padding) + ((_currentButton - 4) * pb.Width);
				pb.Top = (_padding * 2) + pb.Height;
			}

			this.Controls.Add(pb);
			_currentButton++;


		}

		void PictureBoxDoubleClick(object sender, EventArgs e)
		{

			PictureBox pb = sender as PictureBox;
			if (pb != null)
			{
				string message = pb.Tag.ToString();

				string effectname = message.Substring(0, message.IndexOf(" ", StringComparison.Ordinal));
				string text = message.Substring(message.IndexOf(" ", StringComparison.Ordinal) + 1);

				ButtonEffect effect = (ButtonEffect)System.Enum.Parse(typeof(ButtonEffect), effectname);

				ColourEditor ce = new ColourEditor(effect, text);
				ce.Show(AppController.KeyboardFormHandle);
			}
		}

		private void ColourMapResizeEnd(object sender, EventArgs e)
		{
			if (Size != _oldSize)
			{
				ConstrainForm();
				Redraw();
				_oldSize = Size;
			}
		}

		private void ColourMapFormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason == CloseReason.UserClosing)
			{
				e.Cancel = true;
				this.Hide();
			}
			SaveSettings();
		}

		private void SaveSettings()
		{
			Properties.Settings userSettings = new Properties.Settings();
			userSettings.ColourListFormLocation = this.Location;
			userSettings.ColourListFormHeight = this.Height;
			userSettings.ColourMapFormWidth = this.Width;
			userSettings.Save();
		}

		private void LoadSettings(Point callingFormLocation, Size callingFormSize)
		{

			Properties.Settings userSettings = new Properties.Settings();

			Point savedLocation = userSettings.ColourListFormLocation;

			if (savedLocation.IsEmpty)
				this.Location = new Point(callingFormLocation.X, callingFormLocation.Y + callingFormSize.Height + 1);
			else
				this.Location = savedLocation;

			int savedHeight = userSettings.ColourListFormHeight;
			if (savedHeight != 0)
				this.Height = savedHeight;

			int savedWidth = userSettings.ColourMapFormWidth;
			if (savedWidth != 0)
				this.Width = savedWidth;


		}

	}

}



