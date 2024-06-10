using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.Globalization;
using KeyMapper.Classes;
using KeyMapper.Controls;
using KeyMapper.Providers;
using Microsoft.Win32;
using System.Drawing.Imaging;
using KeyMapper.Classes.Interop;

namespace KeyMapper.Forms
{
    public partial class KeyboardForm : KMBaseForm
    {
        private float buttonScale;
        private float keySize;
        private int paddingWidth;
        private bool hasNumberPad;
        private bool keysOnly;
        private KeySniffer sniffer;
        private bool cancelSlideShow;

        // Because we are intercepting a keypress before it is processed, can't ask
        // what state the keyboard is in using Form.IsKeySet or WIN32API funcs like
        // GetKeyState. So, using fields for the state of each key.

        private bool isCapsLockOn;
        private bool isNumLockOn;
        private bool isScrollLockOn;

        // Different arrangement of ALT and CTRL keys.
        private bool isMacKeyboard;

        private Size lastSize;

        private int[] rowTerminators;

        private readonly ToolTip FormToolTip;

        public KeyboardForm()
        {
            FormToolTip = new ToolTip();

            InitializeComponent();
            FormsManager.RegisterMainForm(this);

            if (DpiInfo.Dpi < 96)
            {
                menu.Height = (int)(menu.Height * (96F / DpiInfo.Dpi));
                // Menu will always show even is fonts are set to less than 100%
            }
            else if (DpiInfo.Dpi > 96)
            {
                menu.Height = (int)(menu.Height * (DpiInfo.Dpi / 96F));
            }

            LoadUserSettings();

            ResizeToAspect();

            if (DpiInfo.Dpi != 96)
            {
                PositionKeyboardCombo();
            }

            // This needs to be done after location and size of this form are fully determined.
            FormsManager.OpenChildForms();

            lastSize = Size;

            GetKeyboardData();

            // Create event handlers 
            ResizeEnd += KeyboardFormResizeEnd;
            KeyboardListCombo.SelectedIndexChanged += KeyboardListSelectedIndexChanged;

            MappingsManager.MappingsChanged += OnMappingsChanged;
            UserColourSettingManager.ColoursChanged += OnColoursChanged;

            // Sniff for Caps/Num/Scroll lock keys being pressed while app doesn't have focus
            sniffer = new KeySniffer();
            sniffer.KeyPressed += ReceiveKeyPress;
            sniffer.ActivateHook();

            Redraw();
        }


        private void PositionKeyboardCombo()
        {
            // Combo needs to go in the middle of the status bar..
            KeyboardListCombo.Top = StatusBar.Top + (StatusBar.Height - KeyboardListCombo.Height) / 2;

        }
        private void LoadUserSettings()
        {
            var userSettings = new Properties.Settings();

            // As user.config is writeable (if you can find it!)
            // don't want to trust the settings.

            var firstRun = userSettings.UserHasSavedSettings == false;
            var savedPosition = userSettings.KeyboardFormLocation;
            var savedWidth = userSettings.KeyboardFormWidth;

            hasNumberPad = userSettings.KeyboardFormHasNumberPad;
            isMacKeyboard = userSettings.KeyboardFormHasMacKeyboard;

            if (firstRun || savedPosition.IsEmpty || savedPosition.X == -32000)
            {
                FormsManager.PositionMainForm();
            }
            else
            {
                Location = savedPosition;
            }

            if (firstRun || savedWidth < MinimumSize.Width)
            {
                FormsManager.SizeMainForm();
            }
            else
            {
                Width = savedWidth;
            }
        }

        private void CalculateDimensions()
        {
            float keyWidth;

            if (keysOnly)
            {
                keyWidth = 15.7F;
            }
            else
            {
                // These two numbers correspond more-or less to the number of keys wide 
                // a keyboard is, with or without number pad, plus a bit extra for the padding between keys.
                keyWidth = hasNumberPad ? 23.75F : 20F;
            }

            // Calculate total width and key size
            const int buttonWidth = 128; // Starting width

            buttonScale = (float)ClientSize.Width / (int)(buttonWidth * keyWidth); // How much buttons have to be scaled to fit
            keySize = buttonWidth * buttonScale; // Actual size of buttons
            paddingWidth = (int)(keySize / 16); // Gap between rows and columns.
        }

        private void Redraw()
        {
            // Can be errors during slideshow, so if we get one then cancel it.
            try
            {
                NativeMethods.LockWindowUpdate(Handle);
            }
            catch (ObjectDisposedException)
            {
                cancelSlideShow = true;
                return;
            }

            FormToolTip.RemoveAll();
            FormToolTip.SetToolTip(KeyboardListCombo, "Change the displayed keyboard");

            // Need to make sure these dispose as they have bitmap resources, so not using Controls.Clear()
            // as it didn't release them properly..
            for (int i = KeyboardPanel.Controls.Count - 1; i >= 0; i--)
            {
                KeyboardPanel.Controls[i].Dispose();
            }

            // Start in the top left corner..
            int left = paddingWidth;
            int top = paddingWidth + menu.Height;

            int numPadLeft = 0;
            int navLeft = 0;

            float mainKeyWidth = 14.5F * (keySize + paddingWidth) + paddingWidth * 2;
            float navWidth = (keySize + paddingWidth) * 3;

            // Work out how far back the number pad extends 
            if (hasNumberPad)
            {
                numPadLeft = (int)Math.Round(ClientSize.Width - (keySize + paddingWidth) * 4.2, 0);
                // Nav controls are three wide and they have to fit midway in the
                // gap between the end of the main body and the numberPad.
                navLeft = (int)Math.Round(mainKeyWidth + (numPadLeft - mainKeyWidth - navWidth) / 2, 0);
            }
            else
            {
                // Work out how far back the navkeys extend instead:
                navLeft = (int)Math.Round(ClientSize.Width - (keySize + paddingWidth) * 3.2, 0);
            }

            var keyboardLayout = AppController.KeyboardLayout;

            var kl = PhysicalKeyboardLayout.GetPhysicalLayout(keyboardLayout, isMacKeyboard);

            rowTerminators = PhysicalKeyboardLayout.GetRowTerminators(keyboardLayout);

            if (keysOnly == false)
            {
                // Function keys.
                DrawRow(kl.FunctionKeys, left, top);

                // Utility Keys
                if (hasNumberPad == false || keyboardLayout == KeyboardLayoutType.US)
                {
                    DrawRow(kl.UtilityKeys, navLeft, top);
                }
                else
                {
                    // Shunt keys along one key-width (plus padding) for UK layout so they right-justify
                    DrawRow(kl.UtilityKeys, numPadLeft + (int)Math.Round(keySize, 0) + paddingWidth, top);
                }

                // To get a spacer row between the F keys: add double padding
                top += (int)Math.Round(keySize + paddingWidth * 2, 0);

            }

            // TypewriterKeys keys
            DrawRow(kl.TypewriterKeys, left, top);

            if (keysOnly == false)
            {
                // Navigation - Insert, Home, End etc
                DrawRow(kl.NavigationKeys, navLeft, top);

                // Number pad
                if (hasNumberPad)
                {
                    DrawRow(kl.NumberPadKeys, numPadLeft, top);
                }

                // Skip down and back for arrow keys
                top += (int)Math.Round((keySize + paddingWidth) * 3, 0);
                DrawRow(kl.ArrowKeys, navLeft, top);

            }

            // Need to establish what the keys really are.. should know but it doesn't hurt to check,
            // especially as it went out of kilter at least once in development. 
            isCapsLockOn = IsKeyLocked(Keys.CapsLock); //  KeyboardHelper.IsKeySet(KeyboardHelper.ToggleKey.CapsLock);
            isNumLockOn = IsKeyLocked(Keys.NumLock); // .IsKKeyboardHelper.IsKeySet(KeyboardHelper.ToggleKey.NumLock);
            isScrollLockOn = IsKeyLocked(Keys.Scroll); //  KeyboardHelper.IsKeySet(KeyboardHelper.ToggleKey.ScrollLock);

            SetStatusLabelsText();
            SetMenuButtonStates();

            NativeMethods.LockWindowUpdate(IntPtr.Zero);

        }

        private void DrawRow(IEnumerable<KeyboardRow> Rows, int leftStart, int top)
        {
            int left = leftStart;

            // Need to set the exact width of the row.
            int width = (int)(14.7F * (keySize + paddingWidth));
            if (width % 2 != 0)
            {
                width += 1;
            }

            foreach (var row in Rows)
            {
                foreach (var key in row.Keys)
                {
                    if (key == null)
                    {
                        // Spacer - eg to centre the up arrow key 
                        left += (int)Math.Round((decimal)keySize, 0) + paddingWidth;
                    }
                    else
                    {
                        // The last key in the row must be stretched appropriately.
                        // Which one's that, then?

                        int index = Array.IndexOf(rowTerminators, KeyHasher.GetHashFromKeyData(key.ScanCode, key.Extended));

                        if (index < 0)
                        {
                            DrawKey(key.ScanCode, key.Extended, ref left, top, key.Button,
                                key.HorizontalStretch * paddingWidth, key.VerticalStretch * paddingWidth);
                        }
                        else
                        {

                            // Ok. How much space is left to fill?
                            // left is where we are to start drawing, width is the line width.
                            int keyWidth = width - left;

                            // Calculate the horizontal stretch value based on the sizes:

                            int buttonWidth = 0;

                            switch (key.Button)
                            {
                                case BlankButton.Blank:
                                case BlankButton.TallBlank:
                                    buttonWidth = 128;
                                    break;
                                case BlankButton.MediumWideBlank:
                                    buttonWidth = 192;
                                    break;
                                case BlankButton.DoubleWideBlank:
                                    buttonWidth = 256;
                                    break;
                                case BlankButton.TripleWideBlank:
                                    buttonWidth = 384;
                                    break;
                                case BlankButton.QuadrupleWideBlank:
                                    buttonWidth = 512;
                                    break;
                                default:
                                    break;
                            }

                            int stretch = (int)(keyWidth - buttonWidth * buttonScale);

                            DrawKey(key.ScanCode, key.Extended, ref left, top, key.Button,
                                stretch, key.VerticalStretch * paddingWidth);
                        }

                        left += key.RightPadding * paddingWidth;
                    }

                }

                top += paddingWidth + (int)Math.Round((decimal)keySize, 0);
                left = leftStart;
            }

        }

        private void DrawKey(int scanCode, int extended, ref int left, int top, BlankButton button, int horizontalStretch, int verticalStretch)
        {

            var box = new KeyPictureBox(scanCode, extended, button, buttonScale, horizontalStretch, verticalStretch);

            box.Left = left;
            box.Top = top;

            KeyboardPanel.Controls.Add(box);

            box.DoubleClick += KeyDoubleClick;

            string toolTipText = box.Map.MappingDescription;

            if (string.IsNullOrEmpty(toolTipText) == false)
            {
                FormToolTip.SetToolTip(box, toolTipText);
            }

            // left is a ref parameter.
            left += box.Image.Width + paddingWidth; // Width varies eg for double-width blanks
        }

        private void ResizeToAspect()
        {
            // Need to keep the aspect ratio in the shape of a keyboard. 

            if (WindowState == FormWindowState.Minimized)
            {
                return;
            }

            float factor;
            if (keysOnly)
            {
                factor = 34.5F;
            }
            else
            {
                factor = hasNumberPad ? 43F : 36F;
            }

            SetClientSizeCore(ClientSize.Width, StatusBar.Height + menu.Height +
                (int)(ClientSize.Width * (12F / factor)));

            menu.Width = ClientSize.Width;

            KeyboardPanel.Height = ClientSize.Height - StatusBar.Height + menu.Height;
            KeyboardPanel.Width = ClientSize.Width;

            CalculateDimensions();

        }

        private void SetMappingStatusLabelText()
        {
            int keyMappings = MappingsManager.GetMappingCount(MappingFilter.Set);

            string mapStatusText;

            if (keyMappings != 0)
            {
                mapStatusText = $"{keyMappings} mapping{(keyMappings != 1 ? "s" : "")}";
            }
            else
            {
                // Need to have *something* on the status bar otherwise it doesn't work
                // properly in W2K (it doesn't show on startup)
                mapStatusText = "No mappings";
            }

            StatusLabelMappings.Text = mapStatusText;
        }

        private void SetReadonlyStatusLabelText()
        {
            if (MappingsManager.IsRestartRequired())
            {
                StatusLabelRestartLogoff.Text = "Restart Windows to complete the mappings";
                StatusLabelRestartLogoff.Visible = true;
            }
            else
            {
                StatusLabelRestartLogoff.Text = string.Empty;
                StatusLabelRestartLogoff.Visible = false;
            }
        }

        private void SetStatusLabelsText()
        {
            SetMappingStatusLabelText();
            SetReadonlyStatusLabelText();
        }

        private void SaveUserSettings()
        {
            var userSettings = new Properties.Settings
            {
                KeyboardFormLocation = Location,
                KeyboardFormWidth = Width,
                KeyboardFormHasNumberPad = hasNumberPad,
                KeyboardFormHasMacKeyboard = isMacKeyboard,
                ColourMapFormOpen = FormsManager.IsColourMapFormOpen(),
                MappingListFormOpen = FormsManager.IsMappingListFormOpen(),
                UserHasSavedSettings = true
            };

            // userSettings.KeyboardLayout = (int)AppController.KeyboardLayout;
            userSettings.Save();
        }

        private void GetKeyboardData()
        {

            // InstalledKeyboards contains the list of keyboard names and locales
            // Combo just gets the names at that's what combos like.

            ArrayList keyboardComboData;
            ToolStripItem[] keyboardMenuData;

            var tempArr = new ArrayList(KeyboardHelper.InstalledKeyboards.Keys);
            tempArr.Sort();
            keyboardComboData = new ArrayList(KeyboardHelper.InstalledKeyboards.Count + 1);
            // Add the current keyboard and a separator:
            keyboardComboData.Add(new ComboItemSeparator.SeparatorItem(KeyboardHelper.GetKeyboardName()));
            keyboardComboData.AddRange(tempArr);

            KeyboardListCombo.DataSource = keyboardComboData;

            keyboardMenuData = new ToolStripMenuItem[KeyboardHelper.InstalledKeyboards.Count];

            int count = 0;
            foreach (string name in tempArr)
            {
                keyboardMenuData[count] = new ToolStripMenuItem(name, null, selectLayoutToolStripItemClick);
                count++;
            }

            selectLayoutToolStripMenuItem.DropDownItems.AddRange(keyboardMenuData);
        }


        private void ToggleNumberPad()
        {
            hasNumberPad = !hasNumberPad;
            ResizeToAspect();
            Redraw();
        }

        private void ChangeKeyboard(string name)
        {
            ChangeKeyboard(name, false);
        }


        private void ChangeKeyboard(string name, bool calledFromCombo)
        {

            if (KeyboardHelper.InstalledKeyboards.Contains(name))
            {
                AppController.SetLocale(KeyboardHelper.InstalledKeyboards[name].ToString());
                if (calledFromCombo == false)
                {
                    KeyboardListCombo.SelectedIndexChanged -= KeyboardListSelectedIndexChanged;
                    KeyboardListCombo.SelectedItem = name;
                    KeyboardListCombo.SelectedIndexChanged += KeyboardListSelectedIndexChanged;
                }
                Redraw();
            }
        }

        private void SimulateToggleKeyKeypress(KeyboardHelper.ToggleKey key)
        {
            sniffer.DeactivateHook();
            KeyboardHelper.PressKey(key);

            switch (key)
            {
                case KeyboardHelper.ToggleKey.NumLock:
                    isNumLockOn = !isNumLockOn;
                    break;
                case KeyboardHelper.ToggleKey.CapsLock:
                    isCapsLockOn = !isCapsLockOn;
                    break;
                case KeyboardHelper.ToggleKey.ScrollLock:
                    isScrollLockOn = !isScrollLockOn;
                    break;
                default:
                    break;
            }

            sniffer.ActivateHook();
            SetToggleMenuButtonStates();
        }

        private void ChangeKeyOrientation()
        {
            switch (AppController.KeyboardLayout)
            {
                case KeyboardLayoutType.US:
                    AppController.SwitchKeyboardLayout(KeyboardLayoutType.European);
                    Redraw();
                    break;
                case KeyboardLayoutType.European:
                    AppController.SwitchKeyboardLayout(KeyboardLayoutType.US);
                    Redraw();
                    break;
                default:
                    AppController.SwitchKeyboardLayout(KeyboardLayoutType.US);
                    Redraw();
                    break;
            }
            // In order to remember the choice..
            AppController.AddCustomLayout();
        }

        // Called by FormsManager when a form closes.
        public void RegenerateMenuExternal()
        {
            SetWindowMenuButtonStates();
        }

        private void SetToggleMenuButtonStates()
        {
            // Toggle Keys
            capsLockToolStripMenuItem.Checked = isCapsLockOn;
            numLockToolStripMenuItem.Checked = isNumLockOn;
            scrollLockToolStripMenuItem.Checked = isScrollLockOn;
            setCurrentToggleKeysAtBootToolStripMenuItem.Enabled = true;
        }

        private void SetMappingsMenuButtonStates()
        {
            clearAllToolStripMenuItem.Text = "C&lear All Mappings";
        }

        private void SetWindowMenuButtonStates()
        {
            colourMapFormToolStripMenuItem.Checked = FormsManager.IsColourMapFormOpen();
            mappingListFormToolStripMenuItem.Checked = FormsManager.IsMappingListFormOpen();
        }

        private void SetEditMenuButtonStates()
        {
            undoToolStripMenuItem.Enabled = MappingsManager.UndoStackCount > 0;
            redoToolStripMenuItem.Enabled = MappingsManager.RedoStackCount > 0;
        }

        private void SetKeyboardLayoutMenuButtonStates()
        {
            toggleNumberPadToolStripMenuItem.Enabled = !keysOnly;
            showMainKeysOnlyToolStripMenuItem.Checked = keysOnly;

            toggleNumberPadToolStripMenuItem.Checked = hasNumberPad;
            useMacKeyboardToolStripMenuItem.Checked = isMacKeyboard;
        }

        private void SetMenuButtonStates()
        {
            SetToggleMenuButtonStates();
            SetMappingsMenuButtonStates();
            SetWindowMenuButtonStates();
            SetEditMenuButtonStates();
            SetKeyboardLayoutMenuButtonStates();
            // SetAdvancedMenuButtonStates();
        }

		private void KeyboardListSelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeKeyboard(KeyboardListCombo.Text, true);
        }

        private void KeyDoubleClick(object sender, EventArgs e)
        {
            if (sender is not KeyPictureBox box)
            {
                return;
            }

            FormsManager.ShowEditMappingForm(box.Map, false);
        }

        private void KeyboardFormClosed(object sender, FormClosedEventArgs e)
        {
            if (sniffer != null)
            {
                sniffer.KeyPressed -= ReceiveKeyPress;
                sniffer.DeactivateHook();
                sniffer = null;
            }
            KeyboardHelper.UnloadLayout();
        }

        private void KeyboardFormClosing(object sender, FormClosingEventArgs e)
        {
            // Save settings before we close, so references to subforms are still live.
            SaveUserSettings();
        }

        private void KeyboardFormKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                cancelSlideShow = true;
            }
        }

        private void OnMappingsChanged(object sender, EventArgs e)
        {
            Redraw();
        }

        private void OnColoursChanged(object sender, EventArgs e)
        {
            Redraw();
        }


        private void KeyboardFormResizeEnd(object sender, EventArgs e)
        {
            if (Size != lastSize) // Not just a move (which fires this too)
            {
                lastSize = Size;
                ResizeToAspect();
                Redraw();
            }
        }

        private void ReceiveKeyPress(object sender, KeyMapperKeyPressedEventArgs e)
        {
            if (e != null)
            {
                var key = e.Key;

                // Because we are intercepting a keypress before it is processed, can't ask
                // what state the keyboard is in using Form.IsKeySet or WIN32API funcs like
                // GetKeyState. So, using fields for the state of each key.

                // (In fact, the only thing this achieves is live updating of the Toggle Lock Menu if user presses
                // a lock key while menu is open. It's a small thing, but would be a shame to lose it)

                switch (key.VirtualKeyCode)
                {
                    case (int)KeyboardHelper.ToggleKey.CapsLock:
                        isCapsLockOn = !isCapsLockOn;
                        SetToggleMenuButtonStates();
                        break;
                    case (int)KeyboardHelper.ToggleKey.NumLock:
                        isNumLockOn = !isNumLockOn;
                        SetToggleMenuButtonStates();
                        break;
                    case (int)KeyboardHelper.ToggleKey.ScrollLock:
                        isScrollLockOn = !isScrollLockOn;
                        if (isScrollLockOn != IsKeyLocked(Keys.Scroll))
                        {
                            SetToggleMenuButtonStates();
                        }

                        break;
                }

            }

        }

        //void KeyPictureBoxMouseDown(object sender, MouseEventArgs e)
        //{
        //    if (e.Button == MouseButtons.Right)
        //    {
        //        contextBox = (sender as KeyPictureBox);
        //        if (contextBox != null)
        //        {
        //            PopulateContextMenu();
        //        }
        //    }
        //}

        private void exitMenuItemClick(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void undoMenuItemClick(object sender, EventArgs e)
        {
            MappingsManager.UndoMappingChange();
        }

        private void redoMenuItemClick(object sender, EventArgs e)
        {
            MappingsManager.RedoMappingChange();
        }

        private void changeOrientationMenuItemClick(object sender, EventArgs e)
        {
            ChangeKeyOrientation();
        }

        private void capsLockMenuItemClick(object sender, EventArgs e)
        {
            SimulateToggleKeyKeypress(KeyboardHelper.ToggleKey.CapsLock);
        }

        private void toggleNumberPadMenuItemClick(object sender, EventArgs e)
        {
            ToggleNumberPad();
        }

        private void numLockMenuItemClick(object sender, EventArgs e)
        {
            SimulateToggleKeyKeypress(KeyboardHelper.ToggleKey.NumLock);
        }

        private void scrollLockMenuItemClick(object sender, EventArgs e)
        {
            SimulateToggleKeyKeypress(KeyboardHelper.ToggleKey.ScrollLock);
        }

        private void selectLayoutToolStripItemClick(object sender, EventArgs e)
        {
            ChangeKeyboard(sender.ToString());
        }

        private void selectFromCaptureMenuItemClick(object sender, EventArgs e)
        {
            FormsManager.ShowEditMappingForm(new KeyMapping(), true);
        }

        private void revertToSavedMenuItemClick(object sender, EventArgs e)
        {
            MappingsManager.RevertToStartupMappings();
            OnMappingsChanged(null, null);
        }

        private void clearAllMenuItemClick(object sender, EventArgs e)
        {
            MappingsManager.ClearMappings();
            OnMappingsChanged(null, null);
        }

        private void showMainKeysOnlyMenuItemClick(object sender, EventArgs e)
        {
            keysOnly = !keysOnly;
            ResizeToAspect();
            Redraw();
        }

        private void viewListMenuItemClick(object sender, EventArgs e)
        {
            FormsManager.ToggleMappingListForm();
            SetWindowMenuButtonStates();
        }

        private void showColourMapFormMenuItemClick(object sender, EventArgs e)
        {
            FormsManager.ToggleColourMapForm();
            SetWindowMenuButtonStates();
        }


        private void useMacKeyboardMenuItemClick(object sender, EventArgs e)
        {
            isMacKeyboard = !isMacKeyboard;
            Redraw();
        }

        private void revertToDefaultKeyboardLayoutMenuItemClick(object sender, EventArgs e)
        {
            // Revert to default keyboard layout 
            ChangeKeyboard(KeyboardHelper.GetKeyboardName());
            Redraw();
        }

        private void exportAsRegistryFileMenuItemClick(object sender, EventArgs e)
        {
            MappingsManager.ExportMappingsAsRegistryFile(false);
        }

        private void selectFromListsMenuItemClick(object sender, EventArgs e)
        {
            FormsManager.ShowEditMappingForm(new KeyMapping(), false);
        }

        private void arrangeWindowsMenuItemClick(object sender, EventArgs e)
        {
            FormsManager.ArrangeAllOpenForms();
            // Also, delete the saved position for the Add/Edit mapping form so it's in it's default location
            // next time it's shown.
            var userSettings = new Properties.Settings();
            userSettings.EditMappingFormLocation = Point.Empty;
            userSettings.Save();
        }

        private void showHelpMenuItemClick(object sender, EventArgs e)
        {
            FormsManager.ShowHelpForm();
        }

        private void aboutKeyMapperMenuItemClick(object sender, EventArgs e)
        {
            FormsManager.ShowAboutForm();
        }

        private void putKeyboardListOnClipboardToolStripMenuItemClick(object sender, EventArgs e)
        {
            KeyboardHelper.ShowKeyboardList();
        }

        private void viewLogFileToolStripMenuItemClick(object sender, EventArgs e)
        {
            LogProvider.ViewLogFile();
        }

        private void setCurrentToggleKeysAtBootToolStripMenuItemClick(object sender, EventArgs e)
        {
            // The HKEYCURRENTUSER version of this setting is set at logoff.
            // This sets the value which applies before any user has logged on.

            string value =
                ((isCapsLockOn ? 1 : 0) + (isNumLockOn ? 2 : 0) + (isScrollLockOn ? 4 : 0)).ToString(CultureInfo.InvariantCulture);

            if (AppController.UserCanWriteMappings == false)
            {
                AppController.WriteRegistryEntryVista(
                    RegistryHive.Users, @".DEFAULT\Control Panel\Keyboard", "InitialKeyboardIndicators", value);
            }
            else
            {
                try
                {
                    var key = Registry.Users.OpenSubKey(@".DEFAULT\Control Panel\Keyboard", true);
                    key?.SetValue("InitialKeyboardIndicators", value);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error trying to set default toggle keys: {0}", ex.ToString());
                }
            }

        }

        private void printScreenToFileToolStripMenuItemClick(object sender, EventArgs e)
        {
            SaveKeyboardImageAsFile(false);
        }

        private void SaveKeyboardImageAsFile(bool autoSave)
        {
            using (var bmp = new Bitmap(Width, Height))
            {
                DrawToBitmap(bmp, new Rectangle(Point.Empty, Size));

                var actualSize = new Size(ClientSize.Width, ClientSize.Height - menu.Height - StatusBar.Height);

                using (var bmp2 = new Bitmap(actualSize.Width, actualSize.Height))
                {
                    var p = PointToScreen(Point.Empty);

                    int x = p.X - Left;
                    int y = p.Y - Top + menu.Height;

                    using (var g = Graphics.FromImage(bmp2))
                    {
                        g.DrawImage(bmp, 0, 0, new Rectangle(x, y, actualSize.Width, actualSize.Height), GraphicsUnit.Pixel);
                    }

                    string filename = KeyboardListCombo.Text;

                    if (autoSave)
                    {
                        // bmp2.Save(@"somefolder" + filename + ".png", ImageFormat.Png) ;
                    }
                    else
                    {
                        var fd = new SaveFileDialog
                        {
                            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                            OverwritePrompt = false,
                            AutoUpgradeEnabled = true,
                            FileName = filename + " keyboard layout",
                            Filter = "PNG Image (*.png)|*.png|JPEG Image (*.jpg,*.jpeg)|*.jpg;*.jpeg|Bitmap (*.bmp)|*.bmp"
                        };

                        if (fd.ShowDialog() == DialogResult.OK)
                        {
                            if (fd.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) ||
                                fd.FileName.EndsWith("jpeg", StringComparison.OrdinalIgnoreCase))
                            {
                                bmp2.Save(fd.FileName, ImageFormat.Jpeg);
                            }
                            else if (fd.FileName.EndsWith("bmp", StringComparison.OrdinalIgnoreCase))
                            {
                                bmp2.Save(fd.FileName, ImageFormat.Bmp);
                            }
                            else if (fd.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase))
                            {
                                bmp2.Save(fd.FileName, ImageFormat.Png);
                            }
                        }
                    }
                }
            }
        }

        private void clearLogFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LogProvider.ClearLogFile();
        }

        private void KeyboardSlideshowToolStripMenuItemClick(object sender, EventArgs e)
        {
            string caption = Text;
            int currentKeyboard = KeyboardListCombo.SelectedIndex;
            Text += " (press Escape to stop slideshow)";
            cancelSlideShow = false;
            KeyPress += KeyboardFormKeyPress;
            for (int i = 0; i < KeyboardListCombo.Items.Count; i++)
            {
                if (cancelSlideShow)
                {
                    KeyboardListCombo.SelectedIndex = currentKeyboard;
                    Application.DoEvents();
                    break;
                }

                KeyboardListCombo.SelectedIndex = i;
                Application.DoEvents();
                System.Threading.Thread.Sleep(250);
            }
            KeyPress -= KeyboardFormKeyPress;
            Text = caption;

        }

        private void resetUserSettingsToolStripMenuItemClick(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();
        }

    }

}
