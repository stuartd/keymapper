using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using KeyMapper.Classes;

namespace KeyMapper.Forms
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
		KeySniffer _sniffer;

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
					this._capturingFromKey = true;
				}
				else
				{
					this._selectingFromKeyFromLists = true;
				}
			}
			else
			{
				this._mapped = (map.To.Scancode > 0);
				this._disabled = (map.To.Scancode == 0);
			}

			this._newMapping = !map.IsValid();

			this._map = map;

			// Default has the KeyLists panel in the frame. 
			if (this._mapped | this._disabled)
			{
				SwopPanelPositions(this.KeyListsPanel, this.MappingPanel);
			}
			else if (this._capturingFromKey)
			{
				SwopPanelPositions(this.EmptyPanel, this.KeyListsPanel);
			}

			if (this._selectingFromKeyFromLists)
			{
				// Need to move the lists to the left where the box while remembering where it was
				this._savedPanelLocation = this.KeyListsPanel.Location;
				this.KeyListsPanel.Left = this.FromKeyPictureBox.Left;
				this._keyThreshold = -1; // Show all keys as possible map-ees
			}
			else
				this._keyThreshold = 1;

			SetListOptionsComboIndex();

			PopulateKeyLists();

			// Add event handlers now values have been assigned
			this.GroupsListbox.SelectedIndexChanged += GroupsListboxSelectedIndexChanged;
			this.KeysByGroupListbox.SelectedIndexChanged += KeysByGroupListboxSelectedIndexChanged;
			this.ListOptionsCombo.SelectedIndexChanged += ListOptionsComboSelectedIndexChanged;
			this.KeysByGroupListbox.DoubleClick += KeysByGroupListboxDoubleClick;

			SetupForm();
		}

		private void SetListOptionsComboIndex()
		{
			this.ListOptionsCombo.SelectedIndex = 1 - this._keyThreshold;
		}

		private void SaveSettings()
		{
			Properties.Settings userSettings = new Properties.Settings();
			userSettings.EditMappingFormLocation = this.Location;
			userSettings.Save();
		}

		void PopulateKeyLists()
		{
			this.GroupsListbox.DataSource = new KeyDataXml().GetSortedGroupList(this._keyThreshold);
			UpdateGroupMembers();
		}

		private void SetMapToBlankMapping()
		{
			this._map = MappingsManager.GetEmptyMapping(this._map.From);
		}

		private void SetMapToBlankMapping(int scancode, int extended)
		{
			this._map = MappingsManager.GetEmptyMapping(new Key(scancode, extended));
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

			this._tooltip.RemoveAll();

			if (this._mapped)
				this._tooltip.SetToolTip(this.MapButton, "Delete this key mapping");
			else
			{
				if (this._capturingFromKey)
					this._tooltip.SetToolTip(this.MapButton, "Choose this key to map");
				else
					this._tooltip.SetToolTip(this.MapButton, "Add this key mapping");
			}
			if (this._capturingToKey)
				this._tooltip.SetToolTip(this.CaptureAndCancelButton, "Stop capturing");
			else
				this._tooltip.SetToolTip(this.CaptureAndCancelButton, "Capture a target key by pressing it");

			if (this._disabled)
				this._tooltip.SetToolTip(this.DisableButton, "Enable this key");
			else
				this._tooltip.SetToolTip(this.DisableButton, "Disable this key");

			this._tooltip.SetToolTip(this.KeysByGroupListbox, "Keys are collected into groups");


		}

		private void SetCaption(string caption)
		{
			if (this._newMapping)
				this.Text = "Create a mapping" + (String.IsNullOrEmpty(caption) == false ? ": " + caption : "");
			else
				this.Text = "Edit mapping" + (String.IsNullOrEmpty(caption) == false ? ": " + caption : "");
		}

		private void SetButtonStates()
		{
			// Which buttons should be enabled:

			// Map button (aka UnMap, aka Set (for capture))

			this.MapButton.Enabled =
				(this._capturingFromKey && !this._map.IsEmpty())
				|| this._mapped
				|| (this._capturingFromKey && this._map.IsValid())
				|| (this._selectingFromKeyFromLists && this.KeysByGroupListbox.SelectedIndex >= 0)
				|| (this._capturingToKey && this._map.IsValid())
				|| (!this._disabled && !this._capturingToKey && this.KeysByGroupListbox.SelectedIndex >= 0);

			// Capture buttons should only be enabled when form is not in mapped mode and not disabled
			// and not capturing..
			this.CaptureAndCancelButton.Enabled = (!this._mapped && !this._disabled && !this._capturingFromKey && !this._selectingFromKeyFromLists);

			// Disabled button enabled when not mapped and not capturing.
			this.DisableButton.Enabled = (!this._mapped && !this._capturingToKey && !this._capturingFromKey && !this._selectingFromKeyFromLists);

		}

		private void SetButtonCaptions()
		{

			if (this._capturingFromKey == false && this._selectingFromKeyFromLists == false)
			{
				// Don't assign shortcut keys when they can't be used (ie capturing)
				if (this._mapped)
					this.MapButton.Text = "Un&map";
				else if (this._capturingToKey)
					this.MapButton.Text = "Map";
				else
					this.MapButton.Text = "&Map";

				this.DisableButton.Visible = true;
				this.DisableButton.Text = this._disabled ? "Ena&ble" : "Disa&ble";
				this.CaptureAndCancelButton.Visible = true;
				this.CaptureAndCancelButton.Text = this._capturingToKey ? "Stop" : "Cap&ture";

			}
			else
			{
				this.MapButton.Text = "Set";
				this.CaptureAndCancelButton.Visible = false;
				this.DisableButton.Visible = false;
			}


		}

		private void SetCaption()
		{
			string formCaption = String.Empty;

			if (!this._mapped && !this._disabled)
			{
				if (this._capturingToKey)
				{
					formCaption = "Press what you want the key to do";
				}
				else if (this._capturingFromKey)
				{
					// if (_map.IsEmpty())
					{
						formCaption = "Press the key you want to map";
					}
				}
				else if (this._selectingFromKeyFromLists)
				{
					if (this._map.IsEmpty())
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

			float scale = (DpiInfo.Dpi / 96F);

			if (this.FromKeyPictureBox.Image == null && this._map.IsEmpty())
			{
				this.FromKeyPictureBox.SetImage(ButtonImages.GetButtonImage
					(-1, -1, BlankButton.Blank, 0, 0, scale, ButtonEffect.None));
			}
			else
			{
				this.FromKeyPictureBox.SetImage(ButtonImages.GetButtonImage
					(this._map.From.Scancode, this._map.From.Extended, BlankButton.Blank, 0, 0, scale, ButtonEffect.None));
			}

			// To Key depends more on state
			int scancode = 0;
			int extended = 0;
			ButtonEffect effect = ButtonEffect.None;

			//  'Disabled' is a special case of 'Mapped'
			if (this._disabled)
			{
				effect = MappingsManager.IsMappingPending(this._map) ? ButtonEffect.DisabledPending : ButtonEffect.Disabled;
			}
			else
			{
				if (!this._mapped)
				{
					// Not mapped. What are we doing then??
					if (this._capturingToKey)
					{
						scancode = this._map.To.Scancode;
						extended = this._map.To.Extended;

						if (this._map.To.Scancode == 0)
						{
							// Can't map to a disabled key - show button as disabled..
							effect = ButtonEffect.Disabled;
						}
						else
						{
							effect = ButtonEffect.MappedPending;
						}
					}
					else if (this._capturingFromKey)
					{
						if (this._map.IsEmpty())
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
					scancode = this._map.To.Scancode;
					extended = this._map.To.Extended;
					effect = MappingsManager.IsMappingPending(this._map) ? ButtonEffect.MappedPending : ButtonEffect.Mapped;

				}
			}

			this.ToKeyPictureBox.SetImage(ButtonImages.GetButtonImage(scancode, extended, BlankButton.Blank, 0, 0, scale, effect));

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

			switch (this._direction)
			{
				case FadeDirection.FromUnmappedToMapped:
					panelFrom = this.KeyListsPanel;
					panelTo = this.MappingPanel;
					break;
				case FadeDirection.FromMappedToUnmapped:
					panelFrom = this.MappingPanel;
					panelTo = this.KeyListsPanel;
					break;
				case FadeDirection.FromBlankToUnmapped:
					panelFrom = this.EmptyPanel;
					panelTo = this.KeyListsPanel;
					break;
				default:
					return;
			}

			this.PanelFader.FadeComplete += PanelFaderFadeComplete;
			this.PanelFader.BringToFront();
			this.PanelFader.DoFade(panelFrom, panelTo);
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
			this.PanelFader.SendToBack();
			this.PanelFader.FadeComplete -= PanelFaderFadeComplete;
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
				this._keyThreshold = 1 - cb.SelectedIndex; // 0 -> 1, 1 -> 0, 2 -> -1
				PopulateKeyLists();
			}

		}

		private void GroupsListboxSelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateGroupMembers();
			this.KeysByGroupListbox.SelectedItem = 0;
			SetupForm();
		}

		private void KeysByGroupListboxSelectedIndexChanged(object sender, EventArgs e)
		{
			SetupForm();
		}

		private bool CreateMappingFromListboxValue()
		{
			this._map = new KeyMapping(this._map.From, GetKeyFromListboxValue());
			return true;
		}

		private Key GetKeyFromListboxValue()
		{
			if (this.KeysByGroupListbox.SelectedIndex < 0)
			{
				return new Key();
			}

			// Need to know the scancode and extended of the chosen key.
			string keyname = this.KeysByGroupListbox.Text;

			int hash = 0, scancode = 0, extended = 0;

			if (this._currentgroupmembers.ContainsKey(keyname))
			{
				hash = this._currentgroupmembers[keyname];
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
			this._currentgroupmembers = new KeyDataXml().GetGroupMembers(this.GroupsListbox.Text, this._keyThreshold);

			this.KeysByGroupListbox.Items.Clear();

			foreach (KeyValuePair<string, int> entry in this._currentgroupmembers)
			{
				this.KeysByGroupListbox.Items.Add(entry.Key);
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

			if (this._disabled)
				// Nono - Can't map while disabled. Shouldn't be here anyway!
				return;

			if (this._mapped)
			{
				// Unmap.
				MappingsManager.DeleteMapping(this._map);
				SetMapToBlankMapping();
				this.Close();
				return;
			}

			if (this._capturingToKey)
			{
				// Ah, but have we caught a "to" key yet?
				if (!this._map.IsValid())
					return;

				this._capturingToKey = false;
				StopCapture();
				MappingsManager.AddMapping(this._map);
				this.Close();
				return;
			}

			if (this._selectingFromKeyFromLists)
			{

				Key selectedKey = GetKeyFromListboxValue();

				// Have we been sent a dud??
				if (selectedKey.Scancode == 0)
				{
					// Something went wrong. 
					this._map = new KeyMapping();
				}
				else
				{
					SetMapToBlankMapping(selectedKey.Scancode, selectedKey.Extended);
					// Need to move panel back to where it was and set the image in the picturebox
					this.KeyListsPanel.Location = this._savedPanelLocation;

					this.FromKeyPictureBox.SetImage(ButtonImages.GetButtonImage(this._map.From.Scancode, this._map.From.Extended));
					this._selectingFromKeyFromLists = false;
					this._keyThreshold = 1;
					SetListOptionsComboIndex();
					SetupForm();
					return;

				}


			}

			if (this._capturingFromKey == false)
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
				this._capturingFromKey = false;
				StopCapture();
				this._direction = FadeDirection.FromBlankToUnmapped;
				SetupForm();
				Transition();
			}
		}

		private void CaptureOrCancelButtonClick(object sender, EventArgs e)
		{

			// If we are capturing, stop. If we're not, start. If capturing the From key, this is the cancel button.

			if (this._disabled || this._mapped)
				return;

			if (this._capturingFromKey)
			{
				// Cancel
				this._capturingFromKey = false;
				StopCapture();
				this.Close();
				return;
			}

			if (this._capturingToKey)
			{
				// Cancelling capture - return display to lists.
				this._capturingToKey = false;
				this._direction = FadeDirection.FromMappedToUnmapped;
				StopCapture();
			}
			else
			{
				// OK, start capturing the To key.
				// New capture each time:
				SetMapToBlankMapping();
				StartCapture();
				this._capturingToKey = true;
				this._direction = FadeDirection.FromUnmappedToMapped;
			}

			SetupForm();
			Transition();
		}

		private void DisableButtonClick(object sender, EventArgs e)
		{
			// Disable or enable, close form anyway.
			if (this._disabled)
			{
				// Enable
				MappingsManager.DeleteMapping(this._map);
			}
			else
			{
				// Disable
				this._map = new KeyMapping(this._map.From, new Key(0, 0));
				MappingsManager.AddMapping(this._map);
			}

			this.Close();

		}

		#endregion

		#region Keysniffer methods

		private void OnKeyPress(object sender, KeyMapperKeyPressedEventArgs e)
		{

            int scancode = e.Key.Scancode;
            int extended = e.Key.Extended;
            
  			if (this._capturingFromKey)
			{
				// Have we been sent a dud??
				if (scancode == 0)
				{
					// Can't use a disabled key as From
					this._map = new KeyMapping();
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

				this._map = new KeyMapping(this._map.From, new Key(scancode, extended));
			}

			this.SetupForm();
		}

		private void FormActivated(object sender, EventArgs e)
		{
			if (this._capturingToKey | this._capturingFromKey)
				StartCapture();
		}

		private void FormDeactivate(object sender, EventArgs e)
		{
			if (this._capturingToKey | this._capturingFromKey)
				StopCapture();
		}

		private void StartCapture()
		{

			if (this._sniffer == null)
			{
				this._sniffer = new KeySniffer(true);
				this._sniffer.KeyPressed += OnKeyPress;
			}

			this._sniffer.ActivateHook();

		}

		private void StopCapture()
		{
			this._sniffer.DeactivateHook();
		}

		#endregion

	}
}