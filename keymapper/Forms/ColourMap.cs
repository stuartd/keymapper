using System;
using System.Drawing;
using System.Windows.Forms;
using KeyMapper.Classes;
using Microsoft.Win32;

namespace KeyMapper.Forms
{


	public partial class ColourMap : KMBaseForm
	{
		private int buttonCount;
		private int currentButton;
		private readonly int padding = 2;
		private int buttonHeight;
		private int buttonWidth;
		private int buttonsPerLine;
		private int numberOfLines;
		private float buttonScaleFactor;

		private bool showAllButtons;

		// Are any keys actually mapped?
		private bool mappedkeys;
		private bool disabledkeys;

		// What about keys that will be mapped after next reboot/logon?
		private bool pendingdisabled;
		private bool pendingmapped;

		// Are there keys which were mapped but have now been cleared?
		private bool pendingenabled;
		private bool pendingunmapped;

		private ContextMenu contextMenu;

		private readonly ToolTip toolTip = new ToolTip();

		private readonly string toolTipText;

		public ColourMap()
		{
			if (AppController.UserCannotWriteToApplicationRegistryKey) {
				toolTipText = "Right-click to change which buttons are shown";
			}
			else {
				toolTipText = "Double-click a button to edit the colour: right click for more options";
			}

			InitializeComponent();

			var userSettings = new Properties.Settings();
			showAllButtons = userSettings.ColourMapShowAllButtons;

			CreateContextMenu();
			Redraw();

			MappingsManager.MappingsChanged += delegate(object sender, EventArgs e) { Redraw(); };
			UserColourSettingManager.ColoursChanged += delegate(object sender, EventArgs e) { Redraw(false); };

		}

		private void CreateContextMenu()
		{
			contextMenu = new ContextMenu();

			int newItemIndex = contextMenu.MenuItems.Add(new MenuItem("Show All Buttons", ShowAllButtons));
			if (showAllButtons) {
				contextMenu.MenuItems[newItemIndex].Checked = true;
			}

			newItemIndex = contextMenu.MenuItems.Add(new MenuItem("Show Currently Active Buttons Only", ShowCurrentButtons));
			if (showAllButtons == false) {
				contextMenu.MenuItems[newItemIndex].Checked = true;
			}

			if (AppController.UserCannotWriteToApplicationRegistryKey == false)
			{
				contextMenu.MenuItems.Add(new MenuItem("Reset All Colours", ResetAllColours));
				contextMenu.MenuItems.Add(new MenuItem("Close All Editor Forms", CloseAllEditorForms));
			}

			ContextMenu = contextMenu;


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
			var k = Registry.CurrentUser.OpenSubKey(subkey, true);
			if (k != null)
			{
				k.Close();
				Registry.CurrentUser.DeleteSubKeyTree(subkey);
			}

			UserColourSettingManager.RaiseColoursChangedEvent();

		}


		private void ShowAllButtons(object sender, EventArgs e)
		{
			showAllButtons = true;
			CreateContextMenu();
			Redraw();
		}

		private void ShowCurrentButtons(object sender, EventArgs e)
		{
			showAllButtons = false;
			CreateContextMenu();
			Redraw();
		}


		private void ResetFields()
		{
			mappedkeys = false;
			disabledkeys = false;

			pendingdisabled = false;
			pendingmapped = false;

			pendingenabled = false;
			pendingunmapped = false;
		}

		private void ConstrainForm()
		{

			// One button, two buttons, three buttons - show in one line
			// Four buttons: Show in tow rows of two
			// Four buttons, five buttons, six buttons - show in two rows of three
			// Seven buttons, eight buttons - show in two rows of four.

			switch (buttonCount)
			{
				case 1:
				case 2:
				case 3:
					buttonsPerLine = buttonCount;
					numberOfLines = 1;
					break;
				case 4:
					buttonsPerLine = 2;
					numberOfLines = 2;
					break;
				case 5:
				case 6:
					buttonsPerLine = 3;
					numberOfLines = 2;
					break;
				case 7:
				case 8:
					buttonsPerLine = 4;
					numberOfLines = 2;
					break;
			}

			// Scale the button size down depending on screen resolution width..
			// Form hasn't been positioned yet, so look at primary monitor resolution.

			int screenWidth = SystemInformation.PrimaryMonitorSize.Width;

			if (screenWidth < 801) {
				buttonScaleFactor = 0.35F;
			}
			else if (screenWidth < 1025) {
				buttonScaleFactor = 0.4F;
			}
			else if (screenWidth < 1281) {
				buttonScaleFactor = 0.5F;
			}
			else {
				buttonScaleFactor = 0.5F;
			}

			buttonWidth = (int)(192 * buttonScaleFactor);
			buttonHeight = (int)(128 * buttonScaleFactor);

			// Now work out how big the form should be.
			// Width: Number of buttons per line * buttonsize + (buttons + 1 * padding)
			int width = buttonsPerLine * buttonWidth + (buttonsPerLine + 1) * padding;
			// Height: Number of lines * buttonhight + (number of lines +1 * padding)
			int height = numberOfLines * buttonHeight + (numberOfLines + 1) * padding;

			ClientSize = new Size(width, height);

		}

		private void Redraw(bool reloadMappings = true)
		{
			SuspendLayout();

			toolTip.RemoveAll();
			toolTip.SetToolTip(this, toolTipText);

			if (reloadMappings)
			{
				ResetFields();
				GetButtons();
			}

			for (int i = Controls.Count - 1; i >= 0; i--) {
				Controls[i].Dispose();
			}

			// In order to fix a problem where the form is resized to smaller then the enforced minimum
			// size where the bits of the form which aren then still visible aren't repainted, if the button count is one then
			// constrain twice - once with a count of two then a refresh to clear the background, and then with the count reset to one.

			if (buttonCount == 1)
			{
				buttonCount = 2;
				ConstrainForm();
				Refresh();
				buttonCount = 1;
			}

			ConstrainForm();

			AddButtons();

			if (buttonCount > 1) {
				Text = "KeyMapper Colour Map";
			}
			else {
				Text = "Colour Map";
			}

			ResumeLayout();
		}

		private void AddButtons()
		{

			currentButton = 1;

			AddButton("Normal", ButtonEffect.None);

			if (showAllButtons || mappedkeys) {
				AddButton("Mapped", ButtonEffect.Mapped);
			}

			if (showAllButtons || pendingmapped) {
				AddButton("Pending Mapped", ButtonEffect.MappedPending);
			}

			if (showAllButtons || pendingunmapped) {
				AddButton("Pending Unmapped", ButtonEffect.UnmappedPending);
			}

			if (showAllButtons || disabledkeys) {
				AddButton("Disabled", ButtonEffect.Disabled);
			}

			if (showAllButtons || pendingdisabled) {
				AddButton("Pending Disabled", ButtonEffect.DisabledPending);
			}

			if (showAllButtons || pendingenabled) {
				AddButton("Pending Enabled", ButtonEffect.EnabledPending);
			}
		}

		private void GetButtons()
		{

			if (showAllButtons)
			{
				buttonCount = 8;
				return;
			}

			// Assume there are some normal unmapped keys!
			buttonCount = 1;

			// See what's currently mapped by looking at the current mapping list
			var currentMaps = MappingsManager.GetMappings(MappingFilter.Current);

			foreach (var map in currentMaps)
			{

				if (MappingsManager.IsMappingPending(map, MappingFilter.All))
				{
					// Pending
					if (MappingsManager.IsDisabledMapping(map))
					{
						if (!pendingdisabled)
						{
							pendingdisabled = true;
							buttonCount++;
						}
					}
					else
					{
						if (!pendingmapped)
						{
							pendingmapped = true;
							buttonCount++;
						}
					}
				}
				else
				{
					// Actual 
					if (MappingsManager.IsDisabledMapping(map))
					{
						if (!disabledkeys)
						{
							disabledkeys = true;
							buttonCount++;
						}
					}
					else
					{
						if (!mappedkeys)
						{
							mappedkeys = true;
							buttonCount++;
						}
					}
				}
			}

			// Now look at the cleared keys.

			var maps = MappingsManager.ClearedMappings;

			foreach (var map in maps)
			{
				// Has this cleared key been remapped (in which case we ignore it)
				bool remapped = false;
				foreach (var currentmap in MappingsManager.GetMappings(MappingFilter.Current))
				{
					if (currentmap.From == map.From)
					{
						remapped = true;
						break;
					}
				}

				if (remapped) {
					continue;
				}

				if (MappingsManager.IsDisabledMapping(map))
				{
					if (!pendingenabled)
					{
						pendingenabled = true;
						buttonCount++;
					}
				}
				else
				{
					if (!pendingunmapped)
					{
						pendingunmapped = true;
						buttonCount++;
					}
				}
			}
		}


		private void AddButton(string text, ButtonEffect effect)
		{
            var pb = new PictureBox
                                {
                                    Image = ButtonImages.GetButtonImage
                                    (BlankButton.MediumWideBlank, buttonScaleFactor, text, effect)
                                };

		    pb.Height = pb.Image.Height;
			pb.Width = pb.Image.Width;

			// If there is only one button, contain it in the centre of the form (the minimum size for a form
			// is bigger than a button until button scale gets to 0.6 or so which is too big for small resolutions)

			if (buttonCount == 1)
			{
				// Forms have a minimum size of 123. Applying a slight kludge factor too.
				pb.Left = (123 - SystemInformation.BorderSize.Width - buttonWidth) / 2 - 5;
				pb.Top = padding;
			}
			else
			{
				int position;
				int line;

				// First, see which line the button is in and what position in the line it occupies.
				if (numberOfLines == 1 || currentButton <= buttonsPerLine)
				{
					position = currentButton;
					line = 1;
				}
				else
				{
					position = currentButton - buttonsPerLine;
					line = 2;
				}

				pb.Left = (position - 1) * buttonWidth + position * padding;
				pb.Top = (line - 1) * buttonHeight + line * padding;

			}

			pb.Tag = effect + " " + text;

			if (AppController.UserCannotWriteToApplicationRegistryKey == false) {
				pb.DoubleClick += PictureBoxDoubleClick;
			}

			Controls.Add(pb);
			toolTip.SetToolTip(pb, toolTipText);

			currentButton++;


		}


		private void PictureBoxDoubleClick(object sender, EventArgs e)
		{
			if (sender is PictureBox pb && pb.Tag != null) {
				FormsManager.ShowColourEditorForm(pb.Tag.ToString());
			}
		}

		private void ColourMapFormClosing(object sender, FormClosingEventArgs e)
		{
			SaveSettings();
		}

		private void SaveSettings()
		{
			var userSettings = new Properties.Settings {
				ColourListFormLocation = Location,
				ColourMapShowAllButtons = showAllButtons
			};

			userSettings.Save();
		}

	}

}



