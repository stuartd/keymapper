using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Globalization;

namespace KeyMapper
{


    public partial class KeyboardForm : KMBaseForm
    {


        #region Fields and properties

        float _buttonScale;
        float _keySize;
        int _paddingWidth;
        bool _hasNumberPad;
        bool _keysOnly;
        KeyMapper.KeySniffer _sniffer;

        // Because we are intercepting a keypress before it is processed, can't ask
        // what state the keyboard is in using Form.IsKeySet or WIN32API funcs like
        // GetKeyState. So, using fields for the state of each key.

        bool _isCapsLockOn;
        bool _isNumLockOn;
        bool _isScrollLockOn;

        // Different arrangement of ALT and CTRL keys.
        bool _isMacKeyboard;

        ToolTip _tooltip = new ToolTip();

        // ContextMenu _contextMenu = new ContextMenu();
        // KeyPictureBox _contextBox;

        Size _lastSize;

        int[] _rowTerminators;

        public ToolTip FormToolTip
        {
            get { return _tooltip; }
            set { _tooltip = value; }
        }

        #endregion

        #region Form Methods

        public KeyboardForm()
        {

            InitializeComponent();
            FormsManager.RegisterMainForm(this);

            LoadUserSettings();

            ResizeToAspect();

            // This needs to be done after location and size of this form are fully determined.

            FormsManager.OpenChildForms();

            _lastSize = Size;

            GetKeyboardData();

            // Create event handlers 
            this.ResizeEnd += new System.EventHandler(this.KeyboardFormResizeEnd);
            this.KeyboardListCombo.SelectedIndexChanged += KeyboardListSelectedIndexChanged;

            MappingsManager.MappingsChanged += OnMappingsChanged;
            UserColourSettingManager.ColoursChanged += OnColoursChanged;

            // Sniff for Caps/Num/Scroll lock keys being pressed while app doesn't have focus
            _sniffer = new KeySniffer();
            _sniffer.KeyPressed += ReceiveKeyPress;
            _sniffer.ActivateHook();

            this.Redraw();

            // If control key is held down or app is in debug mode, show debug menu.
#if DEBUG
            debugToolStripMenuItem.Visible = true;
#endif

        }

        private void LoadUserSettings()
        {

            Properties.Settings userSettings = new Properties.Settings();

            // As user.config is writeable (if you can find it!)
            // don't want to trust the settings.

            bool firstrun = true;
            Point savedPosition = Point.Empty;
            int savedWidth = 0;
            MappingFilter oldFilter = MappingFilter.All;

            firstrun = (userSettings.UserHasSavedSettings == false);
            savedPosition = userSettings.KeyboardFormLocation;
            savedWidth = userSettings.KeyboardFormWidth;

            _hasNumberPad = userSettings.KeyboardFormHasNumberPad;
            _isMacKeyboard = userSettings.KeyboardFormHasMacKeyboard;
            oldFilter = (MappingFilter)userSettings.LastMappingsFilter;

            if (firstrun == false)
            {
                // AppController.SwitchKeyboardLayout((KeyboardLayoutType)userSettings.KeyboardLayout);
            }

            if (firstrun || savedPosition.IsEmpty)
                FormsManager.PositionMainForm();
            else
                this.Location = savedPosition;

            if (firstrun || savedWidth < this.MinimumSize.Width)
            {
                FormsManager.SizeMainForm();
            }
            else
            {
                this.Width = savedWidth;
            }


            // If there are boot mappings and no user mappings and the last view mode was boot, then
            // start in boot mode - as long as user has the rights to change them..

            if (oldFilter == MappingFilter.Boot
                && MappingsManager.GetMappingCount(MappingFilter.Boot) > 0
                && MappingsManager.GetMappingCount(MappingFilter.User) == 0
                && AppController.UserCanWriteBootMappings)
            {
                MappingsManager.SetFilter(MappingFilter.Boot);
            }
        }

        void CalculateDimensions()
        {
            float keywidth;
            if (_keysOnly)
            {
                keywidth = 15.7F;
            }
            else
            {
                // These two numbers correspond more-or less to the number of keys wide 
                // a keyboard is, with or without number pad, plus a bit extra for the padding between keys.
                keywidth = _hasNumberPad ? 23.75F : 20F;
            }

            // Calculate total width and key size
            int buttonwidth = 128; // Starting width

            _buttonScale = ((float)this.ClientSize.Width / (int)(buttonwidth * keywidth)); // How much buttons have to be scaled to fit
            _keySize = (buttonwidth * _buttonScale); // Actual size of buttons
            _paddingWidth = (int)(_keySize / 16); // Gap between rows and columns.

        }

        void Redraw()
        {

            NativeMethods.LockWindowUpdate(this.Handle);

            FormToolTip.RemoveAll();
            FormToolTip.SetToolTip(KeyboardListCombo, "Change the displayed keyboard");

            // TODO: Fix that nasty border on the enter key.

            // Need to make sure these dispose as they have bitmap resources, so not using Controls.Clear()
            // as it didn't release them properly..
            for (int i = this.KeyboardPanel.Controls.Count - 1; i >= 0; i--)
                this.KeyboardPanel.Controls[i].Dispose();

            // Start in the top left corner..
            int left = _paddingWidth;
            int top = _paddingWidth + this.menu.Height;

            int numpadleft = 0;
            int navleft = 0;

            float mainkeywidth = (14.5F * (_keySize + _paddingWidth)) + (_paddingWidth * 2);
            float navwidth = ((_keySize + _paddingWidth) * 3);

            // Work out how far back the number pad extends 
            if (_hasNumberPad)
            {
                numpadleft = (int)Math.Round(this.ClientSize.Width - (((_keySize + _paddingWidth) * 4.2)), 0);
                // Nav controls are three wide and they have to fit midway in the
                // gap between the end of the main body and the numberpad.
                navleft = (int)Math.Round((mainkeywidth + ((numpadleft - mainkeywidth - navwidth) / 2)), 0);
            }
            else
            {
                // Work out how far back the navkeys extend instead:
                // navleft = (int)Math.Round(((14.8F * (KeySize + PaddingWidth)) + PaddingWidth * 2), 0);
                navleft = (int)Math.Round(this.ClientSize.Width - (((_keySize + _paddingWidth) * 3.2)), 0);
            }

            KeyboardLayoutType desiredlayout = AppController.KeyboardLayout;

            PhysicalKeyboardLayout kl = PhysicalKeyboardLayout.GetPhysicalLayout(desiredlayout, _isMacKeyboard);

            _rowTerminators = PhysicalKeyboardLayout.GetRowTerminators(desiredlayout);

            if (_keysOnly == false)
            {
                // Function keys.
                DrawRow(kl.Functions, left, top);

                // Utility Keys
                if (_hasNumberPad == false || desiredlayout == KeyboardLayoutType.US)
                {
                    DrawRow(kl.UtilityKeys, navleft, top);
                }
                else
                {
                    // Shunt keys along one key-width (plus padding) for UK layout so they right-justify
                    DrawRow(kl.UtilityKeys, numpadleft + (int)Math.Round(_keySize, 0) + _paddingWidth, top);
                }

                // To get a spacer row between the F keys: add double padding
                top += (int)Math.Round(_keySize + (_paddingWidth * 2), 0);

            }

            // Typewriter keys
            DrawRow(kl.Typewriter, left, top);

            if (_keysOnly == false)
            {
                // Navigation - Insert, Home, End etc
                DrawRow(kl.Navigation, navleft, top);

                // Number pad
                if (_hasNumberPad)
                {
                    DrawRow(kl.NumberPad, numpadleft, top);
                }

                // Skip down and back for arrow keys
                top += (int)Math.Round((_keySize + _paddingWidth) * 3, 0);
                DrawRow(kl.Arrows, navleft, top);

            }

            // Need to establish what the keys really are.. should know but it doesn't hurt to check,
            // especially as it went out of kilter at least once in development. 
            _isCapsLockOn = KeyboardHelper.IsKeySet(KeyboardHelper.ToggleKey.CapsLock);
            _isNumLockOn = KeyboardHelper.IsKeySet(KeyboardHelper.ToggleKey.NumLock);
            _isScrollLockOn = KeyboardHelper.IsKeySet(KeyboardHelper.ToggleKey.ScrollLock);

            SetStatusLabelsText();
            SetMenuButtonStates();

            NativeMethods.LockWindowUpdate(IntPtr.Zero);

        }

        void DrawRow(List<KeyboardRow> Rows, int leftstart, int top)
        {
            int left = leftstart;

            // Need to set the exact width of the row.
            int width = (int)(14.7F * (_keySize + _paddingWidth));
            if (width % 2 != 0)
                width += 1;

            foreach (KeyboardRow row in Rows)
            {
                foreach (KeyboardLayoutElement key in row.Keys)
                {
                    if (key == null)
                    {
                        // Spacer - eg to centre the up arrow key 
                        left += (int)Math.Round((decimal)_keySize, 0) + _paddingWidth;
                    }
                    else
                    {
                        // The last key in the row must be stretched appropriately.
                        // Which one's that, then?

                        int index = Array.IndexOf(_rowTerminators, AppController.GetHashFromKeyData(key.Scancode, key.Extended));

                        if (index < 0)
                        {
                            DrawKey(key.Scancode, key.Extended, ref left, top, key.Button,
                                key.HorizontalStretch * _paddingWidth, key.VerticalStretch * _paddingWidth);
                        }
                        else
                        {

                            // Ok. How much space is left to fill?
                            // left is where we are to start drawing, width is the line width.
                            int keywidth = (int)((width - left));

                            // Calculate the horizontal stretch value based on the sizes:

                            int buttonwidth = 0;

                            switch (key.Button)
                            {
                                case BlankButton.Blank:
                                case BlankButton.TallBlank:
                                    buttonwidth = 128;
                                    break;
                                case BlankButton.MediumWideBlank:
                                    buttonwidth = 192;
                                    break;
                                case BlankButton.DoubleWideBlank:
                                    buttonwidth = 256;
                                    break;
                                case BlankButton.TripleWideBlank:
                                    buttonwidth = 384;
                                    break;
                                case BlankButton.QuadrupleWideBlank:
                                    buttonwidth = 512;
                                    break;
                                default:
                                    break;
                            }

                            int stretch = (int)(keywidth - (buttonwidth * _buttonScale));

                            DrawKey(key.Scancode, key.Extended, ref left, top, key.Button,
                               stretch, key.VerticalStretch * _paddingWidth);
                        }

                        left += key.RightPadding * _paddingWidth;
                    }

                }

                top += _paddingWidth + (int)Math.Round((decimal)_keySize, 0);
                left = leftstart;
            }

        }

        void DrawKey(int scancode, int extended, ref int left, int top, BlankButton button, int horizontalStretch, int verticalStretch)
        {

            bool isPauseButton = false;

            if (scancode == 69 && extended == 224)
                isPauseButton = true;

            KeyPictureBox box = new KeyPictureBox(scancode, extended, button, this._buttonScale, horizontalStretch, verticalStretch, isPauseButton);

            box.Left = left;
            box.Top = top;

            this.KeyboardPanel.Controls.Add(box);

            if (isPauseButton == false &&
                ((MappingsManager.Filter == MappingFilter.Boot && !AppController.UserCanWriteBootMappings) == false))
            {
                box.DoubleClick += new EventHandler(KeyDoubleClick);
                // box.MouseDown += new MouseEventHandler(KeyPictureBoxMouseDown);
                // box.ContextMenu = _contextMenu;
            }

            string toolTipText;
            if (isPauseButton == false)
                toolTipText = box.Map.MappingDescription();
            else
                toolTipText = "The Pause key cannot be used in a mapping";

            if (String.IsNullOrEmpty(toolTipText) == false)
                FormToolTip.SetToolTip(box, toolTipText);

            // left is a ref parameter.
            left += box.Image.Width + _paddingWidth; // Width varies eg for double-width blanks

        }

        void ResizeToAspect()
        {
            // Need to keep the aspect ratio in the shape of a keyboard. 

            if (WindowState == FormWindowState.Minimized)
                return;

            float factor;
            if (_keysOnly)
                factor = 34.5F;
            else
                factor = _hasNumberPad ? 43F : 36F;

            this.SetClientSizeCore(this.ClientSize.Width, StatusBar.Height + this.menu.Height +
                (int)(this.ClientSize.Width * (12F / (factor))));

            menu.Width = this.ClientSize.Width;

            KeyboardPanel.Height = this.ClientSize.Height - StatusBar.Height + this.menu.Height;
            KeyboardPanel.Width = this.ClientSize.Width;

            CalculateDimensions();

        }

        void SetMappingStatusLabelText()
        {

            int allmaps = MappingsManager.GetMappingCount(MappingFilter.All);
            int bootmaps = MappingsManager.GetMappingCount(MappingFilter.Boot);
            int usermaps = MappingsManager.GetMappingCount(MappingFilter.User);
            int currentmaps = MappingsManager.GetMappingCount(MappingsManager.Filter);

            // TODO: Localizing issue. How to do plurals in other cultures???

            IFormatProvider fp = System.Globalization.CultureInfo.InvariantCulture.NumberFormat;

            string mapstatustext = string.Empty;

            if (allmaps > 0)
            {
                string bootmaptext = string.Empty;
                string usermaptext = string.Empty;
                if (bootmaps != 0)
                {
                    if (AppController.LocalUserMappingsAllowed)
                        bootmaptext = bootmaps.ToString(fp) + " boot mapping" + (bootmaps != 1 ? "s" : "");
                    else
                        bootmaptext = bootmaps.ToString(fp) + " mapping" + (bootmaps != 1 ? "s" : "");
                }

                if (usermaps != 0)
                {
                    usermaptext = usermaps.ToString(fp) + " user mapping" + (usermaps != 1 ? "s" : "");

                    mapstatustext =
                        bootmaptext +
                        (String.IsNullOrEmpty(bootmaptext) ? "" : ", ") +
                        usermaptext;
                }
                else
                    mapstatustext = bootmaptext;
            }
            else
            {
                // Need to have *something* on the status bar otherwise it doesn't work
                // properly in W2K (it doesn't show on startup)
                mapstatustext = "No mappings";
            }

            StatusLabelMappings.Text = mapstatustext;

        }

        void SetReadonlyStatusLabelText()
        {
            if (MappingsManager.IsRestartRequired())
            {
                StatusLabelRestartLogoff.Text = "Restart to complete the mappings";
                StatusLabelRestartLogoff.Visible = true;
            }
            else if (MappingsManager.IsLogOnRequired())
            {
                StatusLabelRestartLogoff.Text = "Log on again to complete the mappings";
                StatusLabelRestartLogoff.Visible = true;
            }
            else
            {
                StatusLabelRestartLogoff.Text = String.Empty;
                StatusLabelRestartLogoff.Visible = false;
            }

            StatusLabelReadOnly.Visible = AppController.UserCannotWriteMappings;
        }

        void SetFilterStatusLabelText()
        {

            if (AppController.LocalUserMappingsAllowed == false)
                StatusLabelMappingDisplayType.Visible = false;
            else
            {
                switch (MappingsManager.Filter)
                {
                    case MappingFilter.All:
                        StatusLabelMappingDisplayType.Visible = false;
                        break;

                    case MappingFilter.Boot:
                        StatusLabelMappingDisplayType.Text = "Editing Boot Mappings";
                        StatusLabelMappingDisplayType.Visible = true;
                        break;

                    case MappingFilter.User:
                        StatusLabelMappingDisplayType.Text = "Editing User Mappings";
                        StatusLabelMappingDisplayType.Visible = true;
                        break;

                    default:
                        break;
                }
            }

        }

        void SetStatusLabelsText()
        {
            SetMappingStatusLabelText();
            SetReadonlyStatusLabelText();
            SetFilterStatusLabelText();
        }

        void SaveUserSettings()
        {

            Properties.Settings userSettings = new Properties.Settings();
            userSettings.KeyboardFormLocation = this.Location;
            userSettings.KeyboardFormWidth = this.Width;

            userSettings.KeyboardFormHasNumberPad = this._hasNumberPad;
            userSettings.KeyboardFormHasMacKeyboard = this._isMacKeyboard;

            userSettings.ColourMapFormOpen = FormsManager.IsColourMapFormOpen();
            userSettings.MappingListFormOpen = FormsManager.IsMappingListFormOpen();

            userSettings.LastMappingsFilter = (int)MappingsManager.Filter;

            userSettings.UserHasSavedSettings = true;
            // userSettings.KeyboardLayout = (int)AppController.KeyboardLayout;
            userSettings.Save();

        }

        #endregion

        #region Keyboard methods

        private void GetKeyboardData()
        {

            // InstalledKeyboards contains the list of keyboard names and locales
            // Combo just gets the names at that's what combos like.

            ArrayList keyboardComboData;
            ToolStripItem[] keyboardMenuData;

            ArrayList tempArr = new ArrayList(KeyboardHelper.InstalledKeyboards.Keys);
            tempArr.Sort();
            keyboardComboData = new ArrayList(KeyboardHelper.InstalledKeyboards.Count + 1);
            // Add the current keyboard and a separator:
            keyboardComboData.Add(new KeyMapper.ComboItemSeparator.SeparatorItem(KeyMapper.KeyboardHelper.GetKeyboardName()));
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


        void ToggleNumberpad()
        {
            _hasNumberPad = !_hasNumberPad;
            ResizeToAspect();
            Redraw();
        }

        void ChangeKeyboard(string name)
        {
            ChangeKeyboard(name, false);
        }


        void ChangeKeyboard(string name, bool calledFromCombo)
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
                this.Redraw();
            }
        }

        void SimulateToggleKeyKeypress(KeyboardHelper.ToggleKey key)
        {
            _sniffer.DeactivateHook();
            KeyboardHelper.PressKey(key);

            switch (key)
            {
                case KeyboardHelper.ToggleKey.NumLock:
                    _isNumLockOn = !_isNumLockOn;
                    break;
                case KeyboardHelper.ToggleKey.CapsLock:
                    _isCapsLockOn = !_isCapsLockOn;
                    break;
                case KeyboardHelper.ToggleKey.ScrollLock:
                    _isScrollLockOn = !_isScrollLockOn;
                    break;
                default:
                    break;
            }

            _sniffer.ActivateHook();
            SetToggleMenuButtonStates();
        }

        void ChangeKeyOrientation()
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
                    break;
            }
        }

        #endregion

        #region Menu Buttons

        // Called by FormsManager when a form closes.
        public void RegenerateMenuExternal()
        {
            SetWindowMenuButtonStates();
            SetHelpMenuButtonStates();
        }

        void SetToggleMenuButtonStates()
        {
            // Toggle Keys
            capsLockToolStripMenuItem.Checked = _isCapsLockOn;
            numLockToolStripMenuItem.Checked = _isNumLockOn;
            scrollLockToolStripMenuItem.Checked = _isScrollLockOn;
        }

        void SetMappingsMenuButtonStates()
        {

            // Mappings - view all, user, boot.
            if (AppController.LocalUserMappingsAllowed)
                switch (MappingsManager.Filter)
                {
                    case MappingFilter.All:
                        clearAllToolStripMenuItem.Text = "C&lear All Mappings";
                        break;

                    case MappingFilter.Boot:
                        clearAllToolStripMenuItem.Text = "C&lear All Boot Mappings";
                        break;

                    case MappingFilter.User:
                        clearAllToolStripMenuItem.Text = "C&lear All User Mappings";
                        break;
                }
            else
                clearAllToolStripMenuItem.Text = "C&lear All Mappings";

            // Disable "Clear Mappings" and "Revert To Saved Mappings" if user can't write mappings at all
            // and the latter if there haven't been any changes

            clearAllToolStripMenuItem.Enabled = !AppController.UserCannotWriteMappings;

            revertToSavedToolStripMenuItem.Enabled = (
                AppController.UserCannotWriteMappings == false &&
                (MappingsManager.IsRestartRequired() != false ||
                MappingsManager.IsLogOnRequired() != false));

            onlyShowBootMappingsToolStripMenuItem.Text = "Boot Mappings" +
              (AppController.UserCanWriteBootMappings ? String.Empty : " (Read Only)");

            // Mappings - check current view
            showAllMappingsToolStripMenuItem.Checked = (MappingsManager.Filter == MappingFilter.All);
            onlyShowBootMappingsToolStripMenuItem.Checked = (MappingsManager.Filter == MappingFilter.Boot);
            onlyShowUserMappingsToolStripMenuItem.Checked = (MappingsManager.Filter == MappingFilter.User);

            // Whether to allow the option of viewing user mappings (ie not on W2K) 
            chooseMappingsToolStripMenuItem.Visible = (AppController.LocalUserMappingsAllowed);

            selectFromCaptureToolStripMenuItem.Enabled = !AppController.UserCannotWriteMappings;
        }

        void SetWindowMenuButtonStates()
        {
            colourMapFormToolStripMenuItem.Checked = FormsManager.IsColourMapFormOpen();
            mappingListFormToolStripMenuItem.Checked = FormsManager.IsMappingListFormOpen();

        }

        void SetEditMenuButtonStates()
        {
            undoToolStripMenuItem.Enabled = (MappingsManager.UndoStackCount > 0);
            redoToolStripMenuItem.Enabled = (MappingsManager.RedoStackCount > 0);
        }

        void SetKeyboardLayoutMenuButtonStates()
        {
            toggleNumberPadToolStripMenuItem.Enabled = !_keysOnly;
            showMainKeysOnlyToolStripMenuItem.Checked = _keysOnly;

            toggleNumberPadToolStripMenuItem.Checked = _hasNumberPad;
            useMacKeyboardToolStripMenuItem.Checked = _isMacKeyboard;
        }

        void SetMenuButtonStates()
        {
            SetToggleMenuButtonStates();
            SetMappingsMenuButtonStates();
            SetWindowMenuButtonStates();
            SetEditMenuButtonStates();
            SetKeyboardLayoutMenuButtonStates();
            SetHelpMenuButtonStates();
        }

        private void SetHelpMenuButtonStates()
        {
            if (FormsManager.IsHelpFormOpen())
                showHelpToolStripMenuItem.Text = "Hide &Help";
            else
                showHelpToolStripMenuItem.Text = "Show &Help";
        }

        #endregion

        #region Event Methods

        private void KeyboardListSelectedIndexChanged(object sender, EventArgs e)
        {
            this.ChangeKeyboard(KeyboardListCombo.Text, true);
        }

        private void KeyDoubleClick(object sender, EventArgs e)
        {
            KeyPictureBox box = sender as KeyPictureBox;
            if (box == null)
                return;
            FormsManager.ShowEditMappingForm(box.Map, false);
        }

        private void KeyboardFormClosed(object sender, FormClosedEventArgs e)
        {
            if (_sniffer != null)
            {
                _sniffer.KeyPressed -= ReceiveKeyPress;
                _sniffer.DeactivateHook();
                _sniffer = null;
            }
            KeyboardHelper.UnloadLayout();
        }

        private void KeyboardFormClosing(object sender, FormClosingEventArgs e)
        {
            // Save settings before we close, so references to subforms are still live.
            SaveUserSettings();
        }

        private void OnMappingsChanged(object sender, EventArgs e)
        {
            this.Redraw();
        }

        void OnColoursChanged(object sender, EventArgs e)
        {
            this.Redraw();
        }


        private void KeyboardFormResizeEnd(object sender, EventArgs e)
        {
            if (Size != _lastSize) // Not just a move (which fires this too)
            {
                _lastSize = Size;
                ResizeToAspect();
                Redraw();
            }
        }

        private void ReceiveKeyPress(object sender, KeyMapperKeyPressedEventArgs e)
        {
            if (e != null)
            {
                KBHookStruct key = e.Key;

                // Because we are intercepting a keypress before it is processed, can't ask
                // what state the keyboard is in using Form.IsKeySet or WIN32API funcs like
                // GetKeyState. So, using fields for the state of each key.

                switch (key.VirtualKeyCode)
                {
                    case (int)KeyboardHelper.ToggleKey.CapsLock:
                        _isCapsLockOn = !_isCapsLockOn;
                        SetToggleMenuButtonStates();
                        break;
                    case (int)KeyboardHelper.ToggleKey.NumLock:
                        _isNumLockOn = !_isNumLockOn;
                        SetToggleMenuButtonStates();
                        break;
                    case (int)KeyboardHelper.ToggleKey.ScrollLock:
                        _isScrollLockOn = !_isScrollLockOn;
                        SetToggleMenuButtonStates();
                        break;
                }

            }

        }

        //void KeyPictureBoxMouseDown(object sender, MouseEventArgs e)
        //{
        //    if (e.Button == MouseButtons.Right)
        //    {
        //        _contextBox = (sender as KeyPictureBox);
        //        if (_contextBox != null)
        //        {
        //            PopulateContextMenu();
        //        }
        //    }
        //}

        #endregion

        #region Main Menu click methods

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
            ToggleNumberpad();
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
            this.ChangeKeyboard(sender.ToString());
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
            _keysOnly = !_keysOnly;
            ResizeToAspect();
            Redraw();
        }

        private void onlyShowBootMappingsMenuItemClick(object sender, EventArgs e)
        {
            MappingsManager.SetFilter(MappingFilter.Boot);
            Redraw();
        }

        private void onlyShowUserMappingsMenuItemClick(object sender, EventArgs e)
        {
            MappingsManager.SetFilter(MappingFilter.User);
            Redraw();
        }

        private void showAllMappingsMenuItemClick(object sender, EventArgs e)
        {
            MappingsManager.SetFilter(MappingFilter.All);
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
            _isMacKeyboard = !_isMacKeyboard;
            Redraw();
        }

        private void revertToDefaultKeyboardLayoutMenuItemClick(object sender, EventArgs e)
        {
            // Revert to default keyboard layout 
            this.ChangeKeyboard(KeyboardHelper.GetKeyboardName());
            this.Redraw();
        }

        private void exportAsRegistryFileMenuItemClick(object sender, EventArgs e)
        {
            // This is the required format:

            //            Windows Registry Editor Version 5.00

            // [HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Keyboard Layout]
            // "Scancode Map"=hex:00,00,00,00,00,00,00,00,02,00,00,00,2a,00,3a,00,00,00,00,00

            // [HKEY_CURRENT_USER\Keyboard Layout]
            // "Scancode Map"=hex:00,00,00,00,00,00,00,00,04,00,00,00,5d,e0,1c,e0,1d,00,5b,e0,2a,00,3a,00,00,00,00,00

            // Where there are no mappings, delete the valueS:
            // "Scancode Map"=-

            SaveFileDialog fd = new SaveFileDialog();
            fd.AddExtension = true;
            fd.DefaultExt = "reg";
            fd.Filter = "Registry files (*.reg)|*.reg";
            fd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            fd.OverwritePrompt = true;
            fd.AutoUpgradeEnabled = true;
            fd.FileName = "Key Mappings";
            DialogResult dr = fd.ShowDialog();
            if (dr != DialogResult.OK)
                return;

            int bootMappingCount = MappingsManager.GetMappingCount(MappingFilter.Boot);

            StreamWriter sw = new StreamWriter(fd.FileName, false, Encoding.Unicode);
            sw.WriteLine("Windows Registry Editor Version 5.00");
            sw.WriteLine();

            sw.WriteLine(@"[HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Keyboard Layout]");
            sw.Write("\"Scancode Map\"=");
            if (bootMappingCount > 0)
            {
                sw.Write("hex:");
                WriteMappingsToStream(sw, MappingsManager.GetMappingsAsByteArray(MappingsManager.GetMappings(MappingFilter.Boot)));
            }
            else
                sw.Write("-");

            sw.WriteLine();


            if (bootMappingCount > 0)
                sw.WriteLine();

            sw.WriteLine(@"[HKEY_CURRENT_USER\Keyboard Layout]");

            sw.Write("\"Scancode Map\"=");

            if (MappingsManager.GetMappingCount(MappingFilter.User) > 0)
            {
                sw.Write("hex:");
                WriteMappingsToStream(sw, MappingsManager.GetMappingsAsByteArray(MappingsManager.GetMappings(MappingFilter.User)));
            }
            else
                sw.Write("-");

            sw.WriteLine();

            sw.Close();

        }

        private static void WriteMappingsToStream(StreamWriter sw, byte[] bytemappings)
        {
            for (int i = 0; i < bytemappings.GetLength(0); i++)
            {
                sw.Write(bytemappings[i].ToString("X", CultureInfo.InvariantCulture).PadLeft(2, (char)48));
                if (i < bytemappings.GetLength(0) - 1)
                    sw.Write(",");
            }


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
            Properties.Settings userSettings = new KeyMapper.Properties.Settings();
            userSettings.EditMappingFormLocation = Point.Empty;
            userSettings.Save();
        }

        private void showHelpMenuItemClick(object sender, EventArgs e)
        {
            FormsManager.ToggleHelpForm();
            SetHelpMenuButtonStates();
        }

        private void aboutKeyMapperMenuItemClick(object sender, EventArgs e)
        {
            FormsManager.ShowAboutForm();
        }

        private void showDebugMenuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            debugToolStripMenuItem.Visible = true;
        }

        private void putKeyboardListOnClipboardToolStripMenuItemClick(object sender, EventArgs e)
        {
            string[] kblist = KeyboardHelper.GetInstalledKeyboardListAsStringArray();
            StringBuilder keyboards = new StringBuilder();
            foreach (string keyboard in kblist)
                keyboards.Append(keyboard + (char)13 + (char)10);
            try
            {
                Clipboard.Clear();
                Clipboard.SetText(keyboards.ToString(), TextDataFormat.Text);
            }
            catch (ExternalException ex)
            {
                Console.WriteLine("Clipboard is in use: " + ex.ToString());
            }
            catch (System.Threading.ThreadStateException ex)
            {
                Console.WriteLine("Clipboard operation failed: " + ex.ToString());
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine("Nothing to put on clipboard " + ex.ToString());
            }
        }

        private void viewLogFileToolStripMenuItemClick(object sender, EventArgs e)
        {
            string logfile = AppController.GetLogFilename();
            if (string.IsNullOrEmpty(logfile))
                return;

            System.Diagnostics.Process.Start(logfile);

        }

        #endregion

        #region Tests

        //private void stressTestToolStripMenuItem_Click(object sender, EventArgs e)
        //{

        //    Random r = new Random();
        //    for (int i = 1; i < 1000; i++)
        //    {
        //        int val = r.Next(KeyboardListCombo.Items.Count);
        //        KeyboardListCombo.SelectedIndex = val;
        //        Application.DoEvents();

        //    }

        //}

        #endregion

    }

}

