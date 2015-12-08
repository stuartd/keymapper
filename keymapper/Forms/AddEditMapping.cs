using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using KeyMapper.Classes;

namespace KeyMapper.Forms
{
	public partial class AddEditMapping : KMBaseForm
	{
	    // Direction of fade: 
		// 0 is from group list to button (ie unmapped to mapped)
		// 1 is from button to group list (ie mapped to unmapped)
		// 2 is from blank to button (capturing from key)

		private enum FadeDirection
		{
			FromUnmappedToMapped = 0, FromMappedToUnmapped, FromBlankToUnmapped
		}

		// Form states

	    private FadeDirection direction = 0;
	    private readonly bool mapped;
	    private readonly bool disabled;
	    private bool capturingToKey;
	    private bool capturingFromKey;
	    private bool selectingFromKeyFromLists;
	    private readonly Point savedPanelLocation;

		// Is this a new mapping, or are we editing an existing one..
	    private readonly bool newMapping;

		// Which set of keys to show in the lists
	    private int keyThreshold = 1;

		// Current map
	    private KeyMapping map;

		// For looking up the hash from the name
	    private Dictionary<string, int> currentgroupmembers;

		// For capturing
	    private KeySniffer sniffer;

	    private readonly ToolTip tooltip = new ToolTip();

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
                    capturingFromKey = true;
				}
				else
				{
                    selectingFromKeyFromLists = true;
				}
			}
			else
			{
                mapped = (map.To.Scancode > 0);
                disabled = (map.To.Scancode == 0);
			}

            newMapping = !map.IsValid();

            this.map = map;

			// Default has the KeyLists panel in the frame. 
			if (mapped | disabled)
			{
				SwopPanelPositions(KeyListsPanel, MappingPanel);
			}
			else if (capturingFromKey)
			{
				SwopPanelPositions(EmptyPanel, KeyListsPanel);
			}

			if (selectingFromKeyFromLists)
			{
                // Need to move the lists to the left where the box while remembering where it was
                savedPanelLocation = KeyListsPanel.Location;
                KeyListsPanel.Left = FromKeyPictureBox.Left;
                keyThreshold = -1; // Show all keys as possible map-ees
			}
			else
                keyThreshold = 1;

			SetListOptionsComboIndex();

			PopulateKeyLists();

            // Add event handlers now values have been assigned
            GroupsListbox.SelectedIndexChanged += GroupsListboxSelectedIndexChanged;
            KeysByGroupListbox.SelectedIndexChanged += KeysByGroupListboxSelectedIndexChanged;
            ListOptionsCombo.SelectedIndexChanged += ListOptionsComboSelectedIndexChanged;
            KeysByGroupListbox.DoubleClick += KeysByGroupListboxDoubleClick;

			SetupForm();
		}

		private void SetListOptionsComboIndex()
		{
            ListOptionsCombo.SelectedIndex = 1 - keyThreshold;
		}

		private void SaveSettings()
		{
			Properties.Settings userSettings = new Properties.Settings();
			userSettings.EditMappingFormLocation = Location;
			userSettings.Save();
		}

	    private void PopulateKeyLists()
		{
            GroupsListbox.DataSource = new KeyDataXml().GetSortedGroupList(keyThreshold);
			UpdateGroupMembers();
		}

		private void SetMapToBlankMapping()
		{
            map = MappingsManager.GetEmptyMapping(map.From);
		}

		private void SetMapToBlankMapping(int scancode, int extended)
		{
            map = MappingsManager.GetEmptyMapping(new Key(scancode, extended));
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

            tooltip.RemoveAll();

			if (mapped)
                tooltip.SetToolTip(MapButton, "Delete this key mapping");
			else
			{
				if (capturingFromKey)
                    tooltip.SetToolTip(MapButton, "Choose this key to map");
				else
                    tooltip.SetToolTip(MapButton, "Add this key mapping");
			}
			if (capturingToKey)
                tooltip.SetToolTip(CaptureAndCancelButton, "Stop capturing");
			else
                tooltip.SetToolTip(CaptureAndCancelButton, "Capture a target key by pressing it");

			if (disabled)
                tooltip.SetToolTip(DisableButton, "Enable this key");
			else
                tooltip.SetToolTip(DisableButton, "Disable this key");

            tooltip.SetToolTip(KeysByGroupListbox, "Keys are collected into groups");


		}

		private void SetCaption(string caption)
		{
			if (newMapping)
                Text = "Create a mapping" + (string.IsNullOrEmpty(caption) == false ? ": " + caption : "");
			else
                Text = "Edit mapping" + (string.IsNullOrEmpty(caption) == false ? ": " + caption : "");
		}

		private void SetButtonStates()
		{
            // Which buttons should be enabled:

            // Map button (aka UnMap, aka Set (for capture))

            MapButton.Enabled =
				(capturingFromKey && !map.IsEmpty())
				|| mapped
                || (capturingFromKey && map.IsValid())
				|| (selectingFromKeyFromLists && KeysByGroupListbox.SelectedIndex >= 0)
				|| (capturingToKey && map.IsValid())
				|| (!disabled && !capturingToKey && KeysByGroupListbox.SelectedIndex >= 0);

            // Capture buttons should only be enabled when form is not in mapped mode and not disabled
            // and not capturing..
            CaptureAndCancelButton.Enabled = (!mapped && !disabled && !capturingFromKey && !selectingFromKeyFromLists);

            // Disabled button enabled when not mapped and not capturing.
            DisableButton.Enabled = (!mapped && !capturingToKey && !capturingFromKey && !selectingFromKeyFromLists);

		}

		private void SetButtonCaptions()
		{

			if (capturingFromKey == false && selectingFromKeyFromLists == false)
			{
				// Don't assign shortcut keys when they can't be used (ie capturing)
				if (mapped)
                    MapButton.Text = "Un&map";
				else if (capturingToKey)
                    MapButton.Text = "Map";
				else
                    MapButton.Text = "&Map";

                DisableButton.Visible = true;
                DisableButton.Text = disabled ? "Ena&ble" : "Disa&ble";
                CaptureAndCancelButton.Visible = true;
                CaptureAndCancelButton.Text = capturingToKey ? "Stop" : "Cap&ture";

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
			string formCaption = string.Empty;

			if (!mapped && !disabled)
			{
				if (capturingToKey)
				{
					formCaption = "Press what you want the key to do";
				}
				else if (capturingFromKey)
				{
					// if (map.IsEmpty())
					{
						formCaption = "Press the key you want to map";
					}
				}
				else if (selectingFromKeyFromLists)
				{
					if (map.IsEmpty())
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

			if (FromKeyPictureBox.Image == null && map.IsEmpty())
			{
                FromKeyPictureBox.SetImage(ButtonImages.GetButtonImage
					(-1, -1, BlankButton.Blank, 0, 0, scale, ButtonEffect.None));
			}
			else
			{
                FromKeyPictureBox.SetImage(ButtonImages.GetButtonImage
					(map.From.Scancode, map.From.Extended, BlankButton.Blank, 0, 0, scale, ButtonEffect.None));
			}

			// To Key depends more on state
			int scancode = 0;
			int extended = 0;
			ButtonEffect effect = ButtonEffect.None;

			//  'Disabled' is a special case of 'Mapped'
			if (disabled)
			{
				effect = MappingsManager.IsMappingPending(map) ? ButtonEffect.DisabledPending : ButtonEffect.Disabled;
			}
			else
			{
				if (!mapped)
				{
					// Not mapped. What are we doing then??
					if (capturingToKey)
					{
						scancode = map.To.Scancode;
						extended = map.To.Extended;

						if (map.To.Scancode == 0)
						{
							// Can't map to a disabled key - show button as disabled..
							effect = ButtonEffect.Disabled;
						}
						else
						{
							effect = ButtonEffect.MappedPending;
						}
					}
					else if (capturingFromKey)
					{
						if (map.IsEmpty())
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
					scancode = map.To.Scancode;
					extended = map.To.Extended;
					effect = MappingsManager.IsMappingPending(map) ? ButtonEffect.MappedPending : ButtonEffect.Mapped;

				}
			}

            ToKeyPictureBox.SetImage(ButtonImages.GetButtonImage(scancode, extended, BlankButton.Blank, 0, 0, scale, effect));

		}

		private void EditMappingFormClosing(object sender, FormClosingEventArgs e)
		{
			SaveSettings();
		}

	    private void Transition()
		{

			Panel panelFrom, panelTo;

			switch (direction)
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

	    private static void SwopPanelPositions(Panel p1, Panel p2)
		{
			Point pt = p2.Location;
			p2.Location = p1.Location;
			p1.Location = pt;
		}

	    private void PanelFaderFadeComplete(object sender, EventArgs e)
		{
            PanelFader.SendToBack();
            PanelFader.FadeComplete -= PanelFaderFadeComplete;
		}

	    private void KeysByGroupListboxDoubleClick(object sender, EventArgs e)
		{
			MapSelected();
		}

	    private void ListOptionsComboSelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBox cb = sender as ComboBox;
			if (cb != null)
			{
                keyThreshold = 1 - cb.SelectedIndex; // 0 -> 1, 1 -> 0, 2 -> -1
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
            map = new KeyMapping(map.From, GetKeyFromListboxValue());
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

			int hash;

			if (currentgroupmembers.ContainsKey(keyname))
			{
				hash = currentgroupmembers[keyname];
			}
			else
			{
				// As this is a lookup on an internal list we really should know what they are.
				return new Key();
			}

			return new Key(KeyHasher.GetScancodeFromHash(hash), KeyHasher.GetExtendedFromHash(hash));
		}


		private void UpdateGroupMembers()
		{
            currentgroupmembers = new KeyDataXml().GetGroupMembers(GroupsListbox.Text, keyThreshold);

            KeysByGroupListbox.Items.Clear();

			foreach (KeyValuePair<string, int> entry in currentgroupmembers)
			{
                KeysByGroupListbox.Items.Add(entry.Key);
			}
		}

	    private void MapButtonClick(object sender, EventArgs e)
		{
			MapSelected();
		}

		private void MapSelected()
		{

			if (disabled)
				// Nono - Can't map while disabled. Shouldn't be here anyway!
				return;

			if (mapped)
			{
				// Unmap.
				MappingsManager.DeleteMapping(map);
				SetMapToBlankMapping();
                Close();
				return;
			}

			if (capturingToKey)
			{
				// Ah, but have we caught a "to" key yet?
				if (!map.IsValid())
					return;

                capturingToKey = false;
				StopCapture();
				MappingsManager.AddMapping(map);
                Close();
				return;
			}

			if (selectingFromKeyFromLists)
			{

				Key selectedKey = GetKeyFromListboxValue();

				// Have we been sent a dud??
				if (selectedKey.Scancode == 0)
				{
                    // Something went wrong. 
                    map = new KeyMapping();
				}
				else
				{
					SetMapToBlankMapping(selectedKey.Scancode, selectedKey.Extended);
                    // Need to move panel back to where it was and set the image in the picturebox
                    KeyListsPanel.Location = savedPanelLocation;

                    FromKeyPictureBox.SetImage(ButtonImages.GetButtonImage(map.From.Scancode, map.From.Extended));
                    selectingFromKeyFromLists = false;
                    keyThreshold = 1;
					SetListOptionsComboIndex();
					SetupForm();
					return;

				}


			}

			if (capturingFromKey == false)
			{
				// Not mapped, not capturing From or To keys, so this is mapping from list.
				// Need to call method to create map from name.
				if (CreateMappingFromListboxValue())
				{
					MappingsManager.AddMapping(map);
                    Close();
				}
				return;
			}
			else
			{
                // Setting the From key. Map has already been created from keypress
                capturingFromKey = false;
				StopCapture();
                direction = FadeDirection.FromBlankToUnmapped;
				SetupForm();
				Transition();
			}
		}

		private void CaptureOrCancelButtonClick(object sender, EventArgs e)
		{

			// If we are capturing, stop. If we're not, start. If capturing the From key, this is the cancel button.

			if (disabled || mapped)
				return;

			if (capturingFromKey)
			{
                // Cancel
                capturingFromKey = false;
				StopCapture();
                Close();
				return;
			}

			if (capturingToKey)
			{
                // Cancelling capture - return display to lists.
                capturingToKey = false;
                direction = FadeDirection.FromMappedToUnmapped;
				StopCapture();
			}
			else
			{
				// OK, start capturing the To key.
				// New capture each time:
				SetMapToBlankMapping();
				StartCapture();
                capturingToKey = true;
                direction = FadeDirection.FromUnmappedToMapped;
			}

			SetupForm();
			Transition();
		}

		private void DisableButtonClick(object sender, EventArgs e)
		{
			// Disable or enable, close form anyway.
			if (disabled)
			{
				// Enable
				MappingsManager.DeleteMapping(map);
			}
			else
			{
                // Disable
                map = new KeyMapping(map.From, new Key(0, 0));
				MappingsManager.AddMapping(map);
			}

            Close();
		}

	    private void OnKeyPress(object sender, KeyMapperKeyPressedEventArgs e)
		{

            int scancode = e.Key.Scancode;
            int extended = e.Key.Extended;
            
  			if (capturingFromKey)
			{
				// Have we been sent a dud??
				if (scancode == 0)
				{
                    // Can't use a disabled key as From
                    map = new KeyMapping();
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

                map = new KeyMapping(map.From, new Key(scancode, extended));
			}

            SetupForm();
		}

		private void FormActivated(object sender, EventArgs e)
		{
			if (capturingToKey | capturingFromKey)
				StartCapture();
		}

		private void FormDeactivate(object sender, EventArgs e)
		{
			if (capturingToKey | capturingFromKey)
				StopCapture();
		}

		private void StartCapture()
		{

			if (sniffer == null)
			{
                sniffer = new KeySniffer(true);
                sniffer.KeyPressed += OnKeyPress;
			}

            sniffer.ActivateHook();

		}

		private void StopCapture()
		{
            sniffer.DeactivateHook();
		}
	}
}