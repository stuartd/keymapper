namespace KeyMapper
{
	partial class KeyboardForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param _name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
				if (_sniffer != null)
					_sniffer.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KeyboardForm));
			this.KeyboardPanel = new System.Windows.Forms.Panel();
			this.menu = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mappingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.selectFromCaptureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.selectFromListsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.revertToSavedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.clearAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.showListToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
			this.chooseMappingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.showAllMappingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.onlyShowBootMappingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.onlyShowUserMappingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.keyboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.changeOrientationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toggleNumberPadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.showMainKeysOnlyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.useMacKeyboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.revertToDefaultToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.selectLayoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toggleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.numLockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.capsLockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.scrollLockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.windowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.mappingListFormToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.colourMapFormToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.arrangeWindowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.showHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutKeyMapperToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.StatusBar = new System.Windows.Forms.StatusStrip();
			this.StatusLabelMappings = new System.Windows.Forms.ToolStripStatusLabel();
			this.StatusLabelRestartLogoff = new System.Windows.Forms.ToolStripStatusLabel();
			this.StatusLabelReadOnly = new System.Windows.Forms.ToolStripStatusLabel();
			this.StatusLabelMappingDisplayType = new System.Windows.Forms.ToolStripStatusLabel();
			this.KeyboardListCombo = new KeyMapper.KeyboardListCombo();
			this.menu.SuspendLayout();
			this.StatusBar.SuspendLayout();
			this.SuspendLayout();
			// 
			// KeyboardPanel
			// 
			this.KeyboardPanel.AllowDrop = true;
			this.KeyboardPanel.Location = new System.Drawing.Point(0, 0);
			this.KeyboardPanel.Name = "KeyboardPanel";
			this.KeyboardPanel.Size = new System.Drawing.Size(924, 421);
			this.KeyboardPanel.TabIndex = 0;
			// 
			// menu
			// 
			this.menu.AutoSize = false;
			this.menu.Dock = System.Windows.Forms.DockStyle.None;
			this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.mappingsToolStripMenuItem,
            this.keyboardToolStripMenuItem,
            this.toggleToolStripMenuItem,
            this.windowsToolStripMenuItem,
            this.helpToolStripMenuItem});
			this.menu.Location = new System.Drawing.Point(0, 0);
			this.menu.Name = "menu";
			this.menu.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			this.menu.Size = new System.Drawing.Size(924, 24);
			this.menu.Stretch = false;
			this.menu.TabIndex = 6;
			this.menu.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(38, 20);
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.exitToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
			this.exitToolStripMenuItem.Text = "E&xit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitMenuItemClick);
			// 
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem});
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			this.editToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
			this.editToolStripMenuItem.Text = "&Edit";
			// 
			// undoToolStripMenuItem
			// 
			this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
			this.undoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
			this.undoToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
			this.undoToolStripMenuItem.Text = "&Undo";
			this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoMenuItemClick);
			// 
			// redoToolStripMenuItem
			// 
			this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
			this.redoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
			this.redoToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
			this.redoToolStripMenuItem.Text = "&Redo";
			this.redoToolStripMenuItem.Click += new System.EventHandler(this.redoMenuItemClick);
			// 
			// mappingsToolStripMenuItem
			// 
			this.mappingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripSeparator1,
            this.revertToSavedToolStripMenuItem,
            this.clearAllToolStripMenuItem,
            this.showListToolStripMenuItem,
            this.chooseMappingsToolStripMenuItem,
            this.exportToolStripMenuItem});
			this.mappingsToolStripMenuItem.Name = "mappingsToolStripMenuItem";
			this.mappingsToolStripMenuItem.Size = new System.Drawing.Size(72, 20);
			this.mappingsToolStripMenuItem.Text = "&Mappings";
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectFromCaptureToolStripMenuItem,
            this.selectFromListsToolStripMenuItem});
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(261, 22);
			this.toolStripMenuItem1.Text = "Create &New Mapping";
			// 
			// selectFromCaptureToolStripMenuItem
			// 
			this.selectFromCaptureToolStripMenuItem.Name = "selectFromCaptureToolStripMenuItem";
			this.selectFromCaptureToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.P)));
			this.selectFromCaptureToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
			this.selectFromCaptureToolStripMenuItem.Text = "&Use Key Capture";
			this.selectFromCaptureToolStripMenuItem.Click += new System.EventHandler(this.selectFromCaptureMenuItemClick);
			// 
			// selectFromListsToolStripMenuItem
			// 
			this.selectFromListsToolStripMenuItem.Name = "selectFromListsToolStripMenuItem";
			this.selectFromListsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.T)));
			this.selectFromListsToolStripMenuItem.Size = new System.Drawing.Size(250, 22);
			this.selectFromListsToolStripMenuItem.Text = "&Select From Lists";
			this.selectFromListsToolStripMenuItem.Click += new System.EventHandler(this.selectFromListsMenuItemClick);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(258, 6);
			// 
			// revertToSavedToolStripMenuItem
			// 
			this.revertToSavedToolStripMenuItem.Name = "revertToSavedToolStripMenuItem";
			this.revertToSavedToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.R)));
			this.revertToSavedToolStripMenuItem.Size = new System.Drawing.Size(261, 22);
			this.revertToSavedToolStripMenuItem.Text = "&Revert To Saved";
			this.revertToSavedToolStripMenuItem.Click += new System.EventHandler(this.revertToSavedMenuItemClick);
			// 
			// clearAllToolStripMenuItem
			// 
			this.clearAllToolStripMenuItem.Name = "clearAllToolStripMenuItem";
			this.clearAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.C)));
			this.clearAllToolStripMenuItem.Size = new System.Drawing.Size(261, 22);
			this.clearAllToolStripMenuItem.Text = "&Clear All Mappings";
			this.clearAllToolStripMenuItem.Click += new System.EventHandler(this.clearAllMenuItemClick);
			// 
			// showListToolStripMenuItem
			// 
			this.showListToolStripMenuItem.Name = "showListToolStripMenuItem";
			this.showListToolStripMenuItem.Size = new System.Drawing.Size(258, 6);
			// 
			// chooseMappingsToolStripMenuItem
			// 
			this.chooseMappingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showAllMappingsToolStripMenuItem,
            this.onlyShowBootMappingsToolStripMenuItem,
            this.onlyShowUserMappingsToolStripMenuItem});
			this.chooseMappingsToolStripMenuItem.Name = "chooseMappingsToolStripMenuItem";
			this.chooseMappingsToolStripMenuItem.Size = new System.Drawing.Size(261, 22);
			this.chooseMappingsToolStripMenuItem.Text = "&Show";
			this.chooseMappingsToolStripMenuItem.Click += new System.EventHandler(this.showAllMappingsMenuItemClick);
			// 
			// showAllMappingsToolStripMenuItem
			// 
			this.showAllMappingsToolStripMenuItem.Name = "showAllMappingsToolStripMenuItem";
			this.showAllMappingsToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
			this.showAllMappingsToolStripMenuItem.Text = "All Mappings";
			this.showAllMappingsToolStripMenuItem.Click += new System.EventHandler(this.showAllMappingsMenuItemClick);
			// 
			// onlyShowBootMappingsToolStripMenuItem
			// 
			this.onlyShowBootMappingsToolStripMenuItem.Name = "onlyShowBootMappingsToolStripMenuItem";
			this.onlyShowBootMappingsToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
			this.onlyShowBootMappingsToolStripMenuItem.Text = "Boot Mappings";
			this.onlyShowBootMappingsToolStripMenuItem.Click += new System.EventHandler(this.onlyShowBootMappingsMenuItemClick);
			// 
			// onlyShowUserMappingsToolStripMenuItem
			// 
			this.onlyShowUserMappingsToolStripMenuItem.Name = "onlyShowUserMappingsToolStripMenuItem";
			this.onlyShowUserMappingsToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
			this.onlyShowUserMappingsToolStripMenuItem.Text = "User Mappings";
			this.onlyShowUserMappingsToolStripMenuItem.Click += new System.EventHandler(this.onlyShowUserMappingsMenuItemClick);
			// 
			// exportToolStripMenuItem
			// 
			this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
			this.exportToolStripMenuItem.Size = new System.Drawing.Size(261, 22);
			this.exportToolStripMenuItem.Text = "&Export As Registry File";
			this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportAsRegistryFileMenuItemClick);
			// 
			// keyboardToolStripMenuItem
			// 
			this.keyboardToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeOrientationToolStripMenuItem,
            this.toggleNumberPadToolStripMenuItem,
            this.showMainKeysOnlyToolStripMenuItem,
            this.useMacKeyboardToolStripMenuItem,
            this.revertToDefaultToolStripMenuItem,
            this.selectLayoutToolStripMenuItem});
			this.keyboardToolStripMenuItem.Name = "keyboardToolStripMenuItem";
			this.keyboardToolStripMenuItem.Size = new System.Drawing.Size(116, 20);
			this.keyboardToolStripMenuItem.Text = "&Keyboard Layout";
			// 
			// changeOrientationToolStripMenuItem
			// 
			this.changeOrientationToolStripMenuItem.Name = "changeOrientationToolStripMenuItem";
			this.changeOrientationToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.E)));
			this.changeOrientationToolStripMenuItem.Size = new System.Drawing.Size(314, 22);
			this.changeOrientationToolStripMenuItem.Text = "&Switch Enter key orientation";
			this.changeOrientationToolStripMenuItem.Click += new System.EventHandler(this.changeOrientationMenuItemClick);
			// 
			// toggleNumberPadToolStripMenuItem
			// 
			this.toggleNumberPadToolStripMenuItem.Name = "toggleNumberPadToolStripMenuItem";
			this.toggleNumberPadToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.U)));
			this.toggleNumberPadToolStripMenuItem.Size = new System.Drawing.Size(314, 22);
			this.toggleNumberPadToolStripMenuItem.Text = "&Number Pad";
			this.toggleNumberPadToolStripMenuItem.Click += new System.EventHandler(this.toggleNumberPadMenuItemClick);
			// 
			// showMainKeysOnlyToolStripMenuItem
			// 
			this.showMainKeysOnlyToolStripMenuItem.Name = "showMainKeysOnlyToolStripMenuItem";
			this.showMainKeysOnlyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.K)));
			this.showMainKeysOnlyToolStripMenuItem.Size = new System.Drawing.Size(314, 22);
			this.showMainKeysOnlyToolStripMenuItem.Text = "Typewriter &Keys Only";
			this.showMainKeysOnlyToolStripMenuItem.Click += new System.EventHandler(this.showMainKeysOnlyMenuItemClick);
			// 
			// useMacKeyboardToolStripMenuItem
			// 
			this.useMacKeyboardToolStripMenuItem.Name = "useMacKeyboardToolStripMenuItem";
			this.useMacKeyboardToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.M)));
			this.useMacKeyboardToolStripMenuItem.Size = new System.Drawing.Size(314, 22);
			this.useMacKeyboardToolStripMenuItem.Text = "&Mac Keyboard";
			this.useMacKeyboardToolStripMenuItem.Click += new System.EventHandler(this.useMacKeyboardMenuItemClick);
			// 
			// revertToDefaultToolStripMenuItem
			// 
			this.revertToDefaultToolStripMenuItem.Name = "revertToDefaultToolStripMenuItem";
			this.revertToDefaultToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.D)));
			this.revertToDefaultToolStripMenuItem.Size = new System.Drawing.Size(314, 22);
			this.revertToDefaultToolStripMenuItem.Text = "&Reset To Default";
			this.revertToDefaultToolStripMenuItem.Click += new System.EventHandler(this.revertToDefaultKeyboardLayoutMenuItemClick);
			// 
			// selectLayoutToolStripMenuItem
			// 
			this.selectLayoutToolStripMenuItem.Name = "selectLayoutToolStripMenuItem";
			this.selectLayoutToolStripMenuItem.Size = new System.Drawing.Size(314, 22);
			this.selectLayoutToolStripMenuItem.Text = "Select &Language";
			// 
			// toggleToolStripMenuItem
			// 
			this.toggleToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.numLockToolStripMenuItem,
            this.capsLockToolStripMenuItem,
            this.scrollLockToolStripMenuItem});
			this.toggleToolStripMenuItem.Name = "toggleToolStripMenuItem";
			this.toggleToolStripMenuItem.Size = new System.Drawing.Size(83, 20);
			this.toggleToolStripMenuItem.Text = "&Toggle Key";
			// 
			// numLockToolStripMenuItem
			// 
			this.numLockToolStripMenuItem.Name = "numLockToolStripMenuItem";
			this.numLockToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.N)));
			this.numLockToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
			this.numLockToolStripMenuItem.Text = "&Num Lock";
			this.numLockToolStripMenuItem.Click += new System.EventHandler(this.numLockMenuItemClick);
			// 
			// capsLockToolStripMenuItem
			// 
			this.capsLockToolStripMenuItem.Name = "capsLockToolStripMenuItem";
			this.capsLockToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.P)));
			this.capsLockToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
			this.capsLockToolStripMenuItem.Text = "&Caps Lock";
			this.capsLockToolStripMenuItem.Click += new System.EventHandler(this.capsLockMenuItemClick);
			// 
			// scrollLockToolStripMenuItem
			// 
			this.scrollLockToolStripMenuItem.Name = "scrollLockToolStripMenuItem";
			this.scrollLockToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.S)));
			this.scrollLockToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
			this.scrollLockToolStripMenuItem.Text = "&Scroll Lock";
			this.scrollLockToolStripMenuItem.Click += new System.EventHandler(this.scrollLockMenuItemClick);
			// 
			// windowsToolStripMenuItem
			// 
			this.windowsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mappingListFormToolStripMenuItem,
            this.colourMapFormToolStripMenuItem,
            this.arrangeWindowsToolStripMenuItem});
			this.windowsToolStripMenuItem.Name = "windowsToolStripMenuItem";
			this.windowsToolStripMenuItem.Size = new System.Drawing.Size(69, 20);
			this.windowsToolStripMenuItem.Text = "&Windows";
			// 
			// mappingListFormToolStripMenuItem
			// 
			this.mappingListFormToolStripMenuItem.Name = "mappingListFormToolStripMenuItem";
			this.mappingListFormToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.I)));
			this.mappingListFormToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
			this.mappingListFormToolStripMenuItem.Text = "&Mapping List";
			this.mappingListFormToolStripMenuItem.Click += new System.EventHandler(this.viewListMenuItemClick);
			// 
			// colourMapFormToolStripMenuItem
			// 
			this.colourMapFormToolStripMenuItem.Name = "colourMapFormToolStripMenuItem";
			this.colourMapFormToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.L)));
			this.colourMapFormToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
			this.colourMapFormToolStripMenuItem.Text = "&Colour Map";
			this.colourMapFormToolStripMenuItem.Click += new System.EventHandler(this.showColourMapFormMenuItemClick);
			// 
			// arrangeWindowsToolStripMenuItem
			// 
			this.arrangeWindowsToolStripMenuItem.Name = "arrangeWindowsToolStripMenuItem";
			this.arrangeWindowsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.W)));
			this.arrangeWindowsToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
			this.arrangeWindowsToolStripMenuItem.Text = "&Arrange All";
			this.arrangeWindowsToolStripMenuItem.Click += new System.EventHandler(this.arrangeWindowsMenuItemClick);
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showHelpToolStripMenuItem,
            this.aboutKeyMapperToolStripMenuItem});
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.helpToolStripMenuItem.Text = "&Help";
			// 
			// showHelpToolStripMenuItem
			// 
			this.showHelpToolStripMenuItem.Name = "showHelpToolStripMenuItem";
			this.showHelpToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.H)));
			this.showHelpToolStripMenuItem.Size = new System.Drawing.Size(255, 22);
			this.showHelpToolStripMenuItem.Text = "&Show Help";
			this.showHelpToolStripMenuItem.Click += new System.EventHandler(this.showHelpMenuItemClick);
			// 
			// aboutKeyMapperToolStripMenuItem
			// 
			this.aboutKeyMapperToolStripMenuItem.Name = "aboutKeyMapperToolStripMenuItem";
			this.aboutKeyMapperToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.A)));
			this.aboutKeyMapperToolStripMenuItem.Size = new System.Drawing.Size(255, 22);
			this.aboutKeyMapperToolStripMenuItem.Text = "About KeyMapper";
			this.aboutKeyMapperToolStripMenuItem.Click += new System.EventHandler(this.aboutKeyMapperMenuItemClick);
			// 
			// StatusBar
			// 
			this.StatusBar.AllowItemReorder = true;
			this.StatusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabelMappings,
            this.StatusLabelRestartLogoff,
            this.StatusLabelReadOnly,
            this.StatusLabelMappingDisplayType});
			this.StatusBar.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
			this.StatusBar.Location = new System.Drawing.Point(0, 425);
			this.StatusBar.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
			this.StatusBar.Name = "StatusBar";
			this.StatusBar.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			this.StatusBar.Size = new System.Drawing.Size(922, 22);
			this.StatusBar.TabIndex = 11;
			this.StatusBar.Text = "statusbar";
			// 
			// StatusLabelMappings
			// 
			this.StatusLabelMappings.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.StatusLabelMappings.BorderStyle = System.Windows.Forms.Border3DStyle.Adjust;
			this.StatusLabelMappings.Name = "StatusLabelMappings";
			this.StatusLabelMappings.Padding = new System.Windows.Forms.Padding(0, 0, 25, 0);
			this.StatusLabelMappings.Size = new System.Drawing.Size(172, 17);
			this.StatusLabelMappings.Text = "Showing all X mappings";
			this.StatusLabelMappings.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// StatusLabelRestartLogoff
			// 
			this.StatusLabelRestartLogoff.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.StatusLabelRestartLogoff.BorderStyle = System.Windows.Forms.Border3DStyle.Adjust;
			this.StatusLabelRestartLogoff.Name = "StatusLabelRestartLogoff";
			this.StatusLabelRestartLogoff.Padding = new System.Windows.Forms.Padding(0, 0, 25, 0);
			this.StatusLabelRestartLogoff.Size = new System.Drawing.Size(127, 17);
			this.StatusLabelRestartLogoff.Text = "Restart / LogOff";
			this.StatusLabelRestartLogoff.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// StatusLabelReadOnly
			// 
			this.StatusLabelReadOnly.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.StatusLabelReadOnly.BorderStyle = System.Windows.Forms.Border3DStyle.Adjust;
			this.StatusLabelReadOnly.Name = "StatusLabelReadOnly";
			this.StatusLabelReadOnly.Padding = new System.Windows.Forms.Padding(0, 0, 25, 0);
			this.StatusLabelReadOnly.Size = new System.Drawing.Size(96, 17);
			this.StatusLabelReadOnly.Text = "Read-Only";
			this.StatusLabelReadOnly.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.StatusLabelReadOnly.Visible = false;
			// 
			// StatusLabelMappingDisplayType
			// 
			this.StatusLabelMappingDisplayType.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.StatusLabelMappingDisplayType.BorderStyle = System.Windows.Forms.Border3DStyle.Adjust;
			this.StatusLabelMappingDisplayType.Name = "StatusLabelMappingDisplayType";
			this.StatusLabelMappingDisplayType.Padding = new System.Windows.Forms.Padding(0, 0, 25, 0);
			this.StatusLabelMappingDisplayType.Size = new System.Drawing.Size(158, 17);
			this.StatusLabelMappingDisplayType.Text = "Boot / User Mappings";
			this.StatusLabelMappingDisplayType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// KeyboardListCombo
			// 
			this.KeyboardListCombo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.KeyboardListCombo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.KeyboardListCombo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.KeyboardListCombo.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			this.KeyboardListCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.KeyboardListCombo.Font = new System.Drawing.Font("Verdana", 8F);
			this.KeyboardListCombo.FormattingEnabled = true;
			this.KeyboardListCombo.Location = new System.Drawing.Point(661, 426);
			this.KeyboardListCombo.Name = "KeyboardListCombo";
			this.KeyboardListCombo.Size = new System.Drawing.Size(239, 21);
			this.KeyboardListCombo.TabIndex = 12;
			this.KeyboardListCombo.TabStop = false;
			// 
			// KeyboardForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(922, 447);
			this.Controls.Add(this.KeyboardListCombo);
			this.Controls.Add(this.StatusBar);
			this.Controls.Add(this.menu);
			this.Controls.Add(this.KeyboardPanel);
			this.DoubleBuffered = true;
			this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menu;
			this.MaximizeBox = false;
			this.Name = "KeyboardForm";
			this.Text = "KeyMapper";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.KeyboardFormClosed);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.KeyboardFormClosing);
			this.menu.ResumeLayout(false);
			this.menu.PerformLayout();
			this.StatusBar.ResumeLayout(false);
			this.StatusBar.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Panel KeyboardPanel;
		private System.Windows.Forms.MenuStrip menu;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem mappingsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem revertToSavedToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem clearAllToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem keyboardToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem changeOrientationToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem selectLayoutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toggleToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem numLockToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem capsLockToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem scrollLockToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toggleNumberPadToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem showMainKeysOnlyToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator showListToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem chooseMappingsToolStripMenuItem;
		private System.Windows.Forms.StatusStrip StatusBar;
		private System.Windows.Forms.ToolStripStatusLabel StatusLabelMappings;
		private System.Windows.Forms.ToolStripStatusLabel StatusLabelRestartLogoff;
		private System.Windows.Forms.ToolStripMenuItem showAllMappingsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem onlyShowBootMappingsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem onlyShowUserMappingsToolStripMenuItem;
		private System.Windows.Forms.ToolStripStatusLabel StatusLabelReadOnly;
		private System.Windows.Forms.ToolStripStatusLabel StatusLabelMappingDisplayType;
		private System.Windows.Forms.ToolStripMenuItem useMacKeyboardToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem windowsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem mappingListFormToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem colourMapFormToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem revertToDefaultToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem selectFromCaptureToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem selectFromListsToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private KeyboardListCombo KeyboardListCombo;
        private System.Windows.Forms.ToolStripMenuItem arrangeWindowsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem showHelpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem aboutKeyMapperToolStripMenuItem;


	}
}