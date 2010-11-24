using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Diagnostics;
using KeyMapper.Classes;

namespace KeyMapper
{
	public partial class AddEditMapping : KMBaseForm
	{

		#region Fields, Enum

		// Direction of fade: 
		// 0 is from group list to button (ie unmapped to mapped)
		// 1 is from button to group list (ie mapped to unmapped)
		// 2 is from blank to button (capturing from key)

		private enum FadeDirection
		{
			FromUnmappedToMapped = 0, FromMappedToUnmapped, FromBlankToUnmapped
		}

		// Form states

		FadeDirection _direction = 0;
		bool _mapped;
		bool _disabled;
		bool _capturingToKey;
		bool _capturingFromKey;
		bool _selectingFromKeyFromLists;
		Point _savedPanelLocation;

		// Is this a new mapping, or are we editing an existing one..
		bool _newMapping;

		// Which set of keys to show in the lists
		int _keyThreshold = 1;

		// Current map
		KeyMapping _map;

		// For looking up the hash from the name
		Dictionary<string, int> _currentgroupmembers;

		// For capturing
		KeyMapper.KeySniffer _sniffer;

		ToolTip _tooltip = new ToolTip();


		#endregion

		#region Form methods

		public AddEditMapping(KeyMapping map, bool useCapture)
		{
			InitializeComponent();

			// There are four startup states for this form.

			// 1) Choose a From key by capturing it
			// 2) Choose a mapping for a specific From key
			// 3) Display a given mapping From and To. Includes disabled.
			// 4) Select a From key from the lists

			// Map is a struct so it can't be null.
			if (map.IsEmpty())
			{
				if (useCapture)
				{
					// We are capturing the 'from' key.
					_capturingFromKey = true;
				}
				else
				{
					_selectingFromKeyFromLists = true;
				}
			}
			else
			{
				_mapped = (map.To.Scancode > 0);
				_disabled = (map.To.Scancode == 0);
			}

			_newMapping = !map.IsValid();

			_map = map;

			// Default has the KeyLists panel in the frame. 
			if (_mapped | _disabled)
			{
				SwopPanelPositions(KeyListsPanel, MappingPanel);
			}
			else if (_capturingFromKey)
			{
				SwopPanelPositions(EmptyPanel, KeyListsPanel);
			}

			if (_selectingFromKeyFromLists)
			{
				// Need to move the lists to the left where the box while remembering where it was
				_savedPanelLocation = KeyListsPanel.Location;
				KeyListsPanel.Left = FromKeyPictureBox.Left;
				_keyThreshold = -1; // Show all keys as possible map-ees
			}
			else
				_keyThreshold = 1;

			SetListOptionsComboIndex();

			PopulateKeyLists();

			// Add event handlers now values have been assigned
			this.GroupsListbox.SelectedIndexChanged += GroupsListboxSelectedIndexChanged;
			this.KeysByGroupListbox.SelectedIndexChanged += KeysByGroupListboxSelectedIndexChanged;
			this.ListOptionsCombo.SelectedIndexChanged += ListOptionsComboSelectedIndexChanged;
			KeysByGroupListbox.DoubleClick += KeysByGroupListboxDoubleClick;

			SetupForm();
		}

		private void SetListOptionsComboIndex()
		{
			ListOptionsCombo.SelectedIndex = 1 - _keyThreshold;
		}

		private void SaveSettings()
		{
			Properties.Settings userSettings = new Properties.Settings();
			userSettings.EditMappingFormLocation = this.Location;
			userSettings.Save();
		}

		void PopulateKeyLists()
		{
			GroupsListbox.DataSource = new KeyDataXml().GetSortedGroupList(_keyThreshold);
			UpdateGroupMembers();
		}

		private void SetMapToBlankMapping()
		{
			_map = MappingsManager.GetEmptyMapping(_map.From);
		}

		private void SetMapToBlankMapping(int scancode, int extended)
		{
			_map = MappingsManager.GetEmptyMapping(new Key(scancode, extended));
		}

		private void SetupForm()
		{
			SetToolTips();
			SetButtonStates();
			SetButtonCaptions();
			SetButtonImages();
			SetCaption();
		}

		private void SetToolTips()
		{

			_tooltip.RemoveAll();

			if (_mapped)
				_tooltip.SetToolTip(MapButton, "Delete this key mapping");
			else
			{
				if (_capturingFromKey)
					_tooltip.SetToolTip(MapButton, "Choose this key to map");
				else
					_tooltip.SetToolTip(MapButton, "Add this key mapping");
			}
			if (_capturingToKey)
				_tooltip.SetToolTip(CaptureAndCancelButton, "Stop capturing");
			else
				_tooltip.SetToolTip(CaptureAndCancelButton, "Capture a target key by pressing it");

			if (_disabled)
				_tooltip.SetToolTip(DisableButton, "Enable this key");
			else
				_tooltip.SetToolTip(DisableButton, "Disable this key");

			_tooltip.SetToolTip(KeysByGroupListbox, "Keys are collected into groups");


		}

		private void SetCaption(string caption)
		{
			if (_newMapping)
				this.Text = "Create a mapping" + (String.IsNullOrEmpty(caption) == false ? ": " + caption : "");
			else
				this.Text = "Edit mapping" + (String.IsNullOrEmpty(caption) == false ? ": " + caption : "");
		}

		private void SetButtonStates()
		{
			// Which buttons should be enabled:

			// Map button (aka UnMap, aka Set (for capture))

			MapButton.Enabled =
				(_capturingFromKey && !_map.IsEmpty())
				|| _mapped
				|| (_capturingFromKey && _map.IsValid())
				|| (_selectingFromKeyFromLists && KeysByGroupListbox.SelectedIndex >= 0)
				|| (_capturingToKey && _map.IsValid())
				|| (!_disabled && !_capturingToKey && KeysByGroupListbox.SelectedIndex >= 0);

			// Capture buttons should only be enabled when form is not in mapped mode and not disabled
			// and not capturing..
			CaptureAndCancelButton.Enabled = (!_mapped && !_disabled && !_capturingFromKey && !_selectingFromKeyFromLists);

			// Disabled button enabled when not mapped and not capturing.
			DisableButton.Enabled = (!_mapped && !_capturingToKey && !_capturingFromKey && !_selectingFromKeyFromLists);

		}

		private void SetButtonCaptions()
		{

			if (_capturingFromKey == false && _selectingFromKeyFromLists == false)
			{
				// Don't assign shortcut keys when they can't be used (ie capturing)
				if (_mapped)
					MapButton.Text = "Un&map";
				else if (_capturingToKey)
					MapButton.Text = "Map";
				else
					MapButton.Text = "&Map";

				DisableButton.Visible = true;
				DisableButton.Text = _disabled ? "Ena&ble" : "Disa&ble";
				CaptureAndCancelButton.Visible = true;
				CaptureAndCancelButton.Text = _capturingToKey ? "Stop" : "Cap&ture";

			}
			else
			{
				MapButton.Text = "Set";
				CaptureAndCancelButton.Visible = false;
				DisableButton.Visible = false;
			}


		}

		private void SetCaption()
		{
			string formCaption = String.Empty;

			if (!_mapped && !_disabled)
			{
				if (_capturingToKey)
				{
					formCaption = "Press what you want the key to do";
				}
				else if (_capturingFromKey)
				{
					// if (_map.IsEmpty())
					{
						formCaption = "Press the key you want to map";
					}
				}
				else if (_selectingFromKeyFromLists)
				{
					if (_map.IsEmpty())
					{
						formCaption = "Choose the key you want to map";
					}
				}
				else
				{
					formCaption = "Choose a key from a group or use capture";
				}
			}
			else
			{
				// Mapped.
			}

			SetCaption(formCaption);
		}


		private void SetButtonImages()
		{

			// Set the buttons' bitmap as required. Always call SetImage as that 
			// handles releasing the existing bitmap if any..

			// From key is straightforward.

			float scale = ((float)AppController.DpiY / 96F);

			if (FromKeyPictureBox.Image == null && _map.IsEmpty())
			{
				FromKeyPictureBox.SetImage(ButtonImages.GetButtonImage
					(-1, -1, BlankButton.Blank, 0, 0, scale, ButtonEffect.None));
			}
			else
			{
				FromKeyPictureBox.SetImage(ButtonImages.GetButtonImage
					(_map.From.Scancode, _map.From.Extended, BlankButton.Blank, 0, 0, scale, ButtonEffect.None));
			}

			// To Key depends more on state
			int scancode = 0;
			int extended = 0;
			ButtonEffect effect = ButtonEffect.None;

			//  'Disabled' is a special case of 'Mapped'
			if (_disabled)
			{
				effect = MappingsManager.IsMappingPending(_map) ? ButtonEffect.DisabledPending : ButtonEffect.Disabled;
			}
			else
			{
				if (!_mapped)
				{
					// Not mapped. What are we doing then??
					if (_capturingToKey)
					{
						scancode = _map.To.Scancode;
						extended = _map.To.Extended;

						if (_map.To.Scancode == 0)
						{
							// Can't map to a disabled key - show button as disabled..
							effect = ButtonEffect.Disabled;
						}
						else
						{
							effect = ButtonEffect.MappedPending;
						}
					}
					else if (_capturingFromKey)
					{
						if (_map.IsEmpty())
						{
							// Show a blank key.
							scancode = -1;
							extended = -1;
						}
					}
				}
				else
				{
					// Mapped to a specific key
					scancode = _map.To.Scancode;
					extended = _map.To.Extended;
					effect = MappingsManager.IsMappingPending(_map) ? ButtonEffect.MappedPending : ButtonEffect.Mapped;

				}
			}

			ToKeyPictureBox.SetImage(ButtonImages.GetButtonImage(scancode, extended, BlankButton.Blank, 0, 0, scale, effect));

		}

		private void EditMappingFormClosing(object sender, FormClosingEventArgs e)
		{
			SaveSettings();
		}

		#endregion

		#region Panel moving and fading methods

		private void Transition()
		{

			Panel panelFrom, panelTo;

			switch (_direction)
			{
				case FadeDirection.FromUnmappedToMapped:
					panelFrom = KeyListsPanel;
					panelTo = MappingPanel;
					break;
				case FadeDirection.FromMappedToUnmapped:
					panelFrom = MappingPanel;
					panelTo = KeyListsPanel;
					break;
				case FadeDirection.FromBlankToUnmapped:
					panelFrom = EmptyPanel;
					panelTo = KeyListsPanel;
					break;
				default:
					return;
			}

			PanelFader.FadeComplete += PanelFaderFadeComplete;
			PanelFader.BringToFront();
			PanelFader.DoFade(panelFrom, panelTo);
			SwopPanelPositions(panelFrom, panelTo);

		}

		static void SwopPanelPositions(Panel p1, Panel p2)
		{
			Point pt = p2.Location;
			p2.Location = p1.Location;
			p1.Location = pt;
		}

		void PanelFaderFadeComplete(object sender, EventArgs e)
		{
			PanelFader.SendToBack();
			PanelFader.FadeComplete -= PanelFaderFadeComplete;
		}

		#endregion

		#region Listbox methods

		void KeysByGroupListboxDoubleClick(object sender, EventArgs e)
		{
			MapSelected();
		}

		void ListOptionsComboSelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBox cb = sender as ComboBox;
			if (cb != null)
			{
				_keyThreshold = 1 - cb.SelectedIndex; // 0 -> 1, 1 -> 0, 2 -> -1
				PopulateKeyLists();
			}

		}

		private void GroupsListboxSelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateGroupMembers();
			KeysByGroupListbox.SelectedItem = 0;
			SetupForm();
		}

		private void KeysByGroupListboxSelectedIndexChanged(object sender, EventArgs e)
		{
			SetupForm();
		}

		private bool CreateMappingFromListboxValue()
		{
			this._map = new KeyMapping(_map.From, GetKeyFromListboxValue());
			return true;
		}

		private Key GetKeyFromListboxValue()
		{
			if (KeysByGroupListbox.SelectedIndex < 0)
			{
				return new Key();
			}

			// Need to know the scancode and extended of the chosen key.
			string keyname = KeysByGroupListbox.Text;

			int hash = 0, scancode = 0, extended = 0;

			if (_currentgroupmembers.ContainsKey(keyname))
			{
				hash = _currentgroupmembers[keyname];
			}
			else
			{
				// As this is a lookup on an internal list we really should know what they are.
				return new Key();
			}

			scancode = AppController.GetScancodeFromHash(hash);
			extended = AppController.GetExtendedFromHash(hash);

			return new Key(scancode, extended);

		}


		private void UpdateGroupMembers()
		{
			this._currentgroupmembers = new KeyDataXml().GetGroupMembers(GroupsListbox.Text, _keyThreshold);

			KeysByGroupListbox.Items.Clear();

			foreach (KeyValuePair<string, int> entry in _currentgroupmembers)
			{
				KeysByGroupListbox.Items.Add(entry.Key);
			}
		}

		#endregion

		#region Button methods

		private void MapButtonClick(object sender, EventArgs e)
		{
			MapSelected();
		}

		private void MapSelected()
		{

			if (_disabled)
				// Nono - Can't map while disabled. Shouldn't be here anyway!
				return;

			if (_mapped)
			{
				// Unmap.
				MappingsManager.DeleteMapping(_map);
				SetMapToBlankMapping();
				this.Close();
				return;
			}

			if (_capturingToKey)
			{
				// Ah, but have we caught a "to" key yet?
				if (!_map.IsValid())
					return;

				_capturingToKey = false;
				StopCapture();
				MappingsManager.AddMapping(this._map);
				this.Close();
				return;
			}

			if (_selectingFromKeyFromLists)
			{

				Key selectedKey = GetKeyFromListboxValue();

				// Have we been sent a dud??
				if (selectedKey.Scancode == 0)
				{
					// Something went wrong. 
					_map = new KeyMapping();
				}
				else
				{
					SetMapToBlankMapping(selectedKey.Scancode, selectedKey.Extended);
					// Need to move panel back to where it was and set the image in the picturebox
					KeyListsPanel.Location = _savedPanelLocation;

					FromKeyPictureBox.SetImage(ButtonImages.GetButtonImage(_map.From.Scancode, _map.From.Extended));
					_selectingFromKeyFromLists = false;
					_keyThreshold = 1;
					SetListOptionsComboIndex();
					SetupForm();
					return;

				}


			}

			if (_capturingFromKey == false)
			{
				// Not mapped, not capturing From or To keys, so this is mapping from list.
				// Need to call method to create map from name.
				if (CreateMappingFromListboxValue())
				{
					MappingsManager.AddMapping(this._map);
					this.Close();
				}
				return;
			}
			else
			{
				// Setting the From key. Map has already been created from keypress
				_capturingFromKey = false;
				StopCapture();
				_direction = FadeDirection.FromBlankToUnmapped;
				SetupForm();
				Transition();
			}
		}

		private void CaptureOrCancelButtonClick(object sender, EventArgs e)
		{

			// If we are capturing, stop. If we're not, start. If capturing the From key, this is the cancel button.

			if (_disabled || _mapped)
				return;

			if (_capturingFromKey)
			{
				// Cancel
				_capturingFromKey = false;
				StopCapture();
				this.Close();
				return;
			}

			if (_capturingToKey)
			{
				// Cancelling capture - return display to lists.
				_capturingToKey = false;
				_direction = FadeDirection.FromMappedToUnmapped;
				StopCapture();
			}
			else
			{
				// OK, start capturing the To key.
				// New capture each time:
				SetMapToBlankMapping();
				StartCapture();
				_capturingToKey = true;
				_direction = FadeDirection.FromUnmappedToMapped;
			}

			SetupForm();
			Transition();
		}

		private void DisableButtonClick(object sender, EventArgs e)
		{
			// Disable or enable, close form anyway.
			if (_disabled)
			{
				// Enable
				MappingsManager.DeleteMapping(this._map);
			}
			else
			{
				// Disable
				_map = new KeyMapping(_map.From, new Key(0, 0));
				MappingsManager.AddMapping(_map);
			}

			this.Close();

		}

		#endregion

		#region Keysniffer methods

		private void OnKeyPress(object sender, KeyMapperKeyPressedEventArgs e)
		{

            int scancode = e.Key.Scancode;
            int extended = e.Key.Extended;
            
  			if (_capturingFromKey)
			{
				// Have we been sent a dud??
				if (scancode == 0)
				{
					// Can't use a disabled key as From
					_map = new KeyMapping();
				}
				else
				{
					SetMapToBlankMapping(scancode, extended);
				}
			}
			else
			{
				// Can't tell from the passed key whether it's mapped or not as 
				// if it is, we get the mapped scancode and have no way of telling 
				// what the original key was (it's possible 2 keys could be mapped to the 
				// same key, meaning can't just do a reverse lookup.)

				// So, mapping to a mapped key is de facto allowed.

				_map = new KeyMapping(_map.From, new Key(scancode, extended));
			}

			this.SetupForm();
		}

		private void FormActivated(object sender, EventArgs e)
		{
			if (_capturingToKey | _capturingFromKey)
				StartCapture();
		}

		private void FormDeactivate(object sender, EventArgs e)
		{
			if (_capturingToKey | _capturingFromKey)
				StopCapture();
		}

		private void StartCapture()
		{

			if (_sniffer == null)
			{
				_sniffer = new KeySniffer(true);
				_sniffer.KeyPressed += OnKeyPress;
			}

			_sniffer.ActivateHook();

		}

		private void StopCapture()
		{
			_sniffer.DeactivateHook();
		}

		#endregion

	}
}