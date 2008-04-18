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
		bool _showAllColours = false;
		int _buttons;
		float _factor;
		int _currentbutton;
		int _buttonsize;
		int _padding = 2;

		bool _vertical;

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

			LoadSettings(callingFormLocation, callingFormSize);
			Redraw();

			MappingsManager.MappingsChanged += MappingsChanged;
			this.ResizeEnd += ColourMapResizeEnd;
			this.ResizeBegin += ColourMapResizeBegin;
			this.Resize += ColourMapResizeBegin;

		}


		void ColourMapResizeBegin(object sender, EventArgs e)
		{
			this.MinimumSize = new Size(0, 0);
			this.MaximumSize = new Size(0, 0);
		}

		void MappingsChanged(object sender, EventArgs e)
		{
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

			_currentbutton = 1;
			_factor = 0F;
			_buttonsize = 0;

		}

		private void ConstrainForm()
		{

			_vertical = (this.ClientSize.Height > this.ClientSize.Width);

			if (_vertical)
			{
				this.MinimumSize = new Size(128, 64);
				this.MaximumSize = new Size(192, 9999);
			}
			else
			{
				this.MinimumSize = new Size(64, 64);
				this.MaximumSize = new Size(9999, 160);
			}

		}


		private void Redraw()
		{
			ConstrainForm();

			for (int i = this.Controls.Count - 1; i >= 0; i--)
				this.Controls[i].Dispose(); // Hopefully this clears out bitmaps and events etc?

			ResetFields();
			GetButtons();

			// Set our orientation and size.

			if (_vertical)
			{
				_factor = (this.ClientSize.Width - 10) / 192F;
				_buttonsize = (int)(128 * _factor);
				this.ClientSize = new Size(this.ClientSize.Width, 10 + (_buttonsize * _buttons) + (_buttons * _padding));
			}
			else
			{
				// Bitmap size is determined by height.
				_factor = (this.ClientSize.Height - 10) / 128F;

				// Using medium wide blanks which are 192 wide.
				_buttonsize = (int)(192 * _factor);

				this.ClientSize = new Size(10 + (_buttonsize * _buttons) + (_buttons * _padding), this.ClientSize.Height);
			}

			AddButtons();

			_oldSize = Size;

			this.Refresh();

		}

		private void AddButtons()
		{

			AddButton("Normal", ButtonEffect.None);

			if (_showAllColours || _mappedkeys)
				AddButton("Mapped", ButtonEffect.Mapped);

			if (_showAllColours || _pendingmapped)
				AddButton("Pending Mapped", ButtonEffect.MappedPending);

			if (_showAllColours || _pendingunmapped)
				AddButton("Pending Unmapped", ButtonEffect.UnmappedPending);

			if (_showAllColours || _disabledkeys)
				AddButton("Disabled", ButtonEffect.Disabled);

			if (_showAllColours || _pendingdisabled)
				AddButton("Pending Disabled", ButtonEffect.DisabledPending);

			if (_showAllColours || _pendingenabled)
				AddButton("Pending Enabled", ButtonEffect.EnabledPending);

		}

		private void GetButtons()
		{

			// By default, only show the buttons which are in use on the keyboard form. 

			if (_showAllColours)
			{
				_buttons = 7;
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

		private void AddButton(string text, ButtonEffect effect)
		{

			PictureBox pb = new PictureBox();
			pb.Image = ButtonImages.GetButtonImage(BlankButton.MediumWideBlank, _factor, text, effect);
			pb.Height = pb.Image.Height;
			pb.Width = pb.Image.Width;

			// Cheesy but effective way of passing info to doubleclick
			pb.Tag = effect.ToString() + " " + text;

			if (_vertical)
			{
				pb.Left = 5;
				pb.Top = 5 + (_buttonsize * (_currentbutton - 1)) + _currentbutton * _padding;
			}
			else
			{
				pb.Top = 5;
				pb.Left = ((_currentbutton - 1) * _buttonsize) + _currentbutton * _padding;

			}


			pb.DoubleClick += new EventHandler(PictureBoxDoubleClick);
			this.Controls.Add(pb);
			_currentbutton++;

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

