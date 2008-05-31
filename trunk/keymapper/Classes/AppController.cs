using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Win32;
using System.Globalization;
using System.IO;
using System.Configuration;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing.Text;


namespace KeyMapper
{

    public static class AppController
    {

        #region Fields, Properties

        // Always use the provided method to get this
        // as substitutions must be made for some cultures unless Arial Unicode MS in installed
        private static string _defaultKeyFont = "Lucida Sans Unicode";

        // Keyboard layout and keys
        private static string _currentLocale;

        private static CultureInfo _currentCultureInfo;
        private static LocalizedKeySet _currentLayout;
        private static KeyboardLayoutType _keyboardLayout;

        // Base font size for drawing text on keys
        private static float _baseFontSize;

        // KeyMapper's own reg key in HKCU
        private const string _appKeyName = @"Software\KeyMapper";

        // Whether current user can write to HKLM
        private static bool _canWriteBootMappings;

        // User mappings don't work on W2k
        private static bool _isWindows2000;

        // Trickery needed to write to HKLM on Vista
        private static bool _isVista;

        private static AppMutex _appMutex;

        private static StreamWriter _consoleWriterStream;
        private static string _consoleOutputFilename = "keymapper.log";

        private static bool _userCannotWriteToApplicationRegistryKey;
        private static bool? _dotNetFrameworkSPInstalled;

        private static bool? _arialUnicodeMSInstalled;

        private static Hashtable _customKeyboardLayouts = new Hashtable();

        private static List<string> _tempfiles = new List<string>();

        // Properties

        public static Hashtable CustomKeyboardLayouts
        {
            get { return AppController._customKeyboardLayouts; }
        }

        public static string CurrentLocale
        {
            get { return _currentLocale; }
        }

        public static bool UserCannotWriteToApplicationRegistryKey
        {
            get { return _userCannotWriteToApplicationRegistryKey; }
        }

        public static bool UserCannotWriteMappings
        {
            get
            {
                return (
                    MappingsManager.Filter == MappingFilter.Boot
                    && !_canWriteBootMappings
                    && !_isVista);
            }
        }

        public static bool UserCanWriteBootMappings
        {
            get { return _canWriteBootMappings; }
        }

        public static bool OperatingSystemIsWindows2000
        {
            get { return _isWindows2000; }
        }

        public static bool OperatingSystemIsVista
        {
            get { return _isVista; }
        }

        public static string ApplicationRegistryKeyName
        {
            get { return _appKeyName; }
        }

        public static KeyboardLayoutType KeyboardLayout
        {
            get { return _keyboardLayout; }
        }

        public static float BaseFontSize
        {
            get { return _baseFontSize; }
        }

        public static CultureInfo CurrentCultureInfo
        {
            get { return _currentCultureInfo; }
        }

        public static string LogFileName
        {
            get
            {
                string path;
                try
                {
                    path = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\KeyMapper";
                    if (Directory.Exists(path) == false)
                        Directory.CreateDirectory(path);

                }
                catch (IOException exc)
                {
                    Console.WriteLine("Can't get console filename: " + exc.ToString());
                    return string.Empty;
                }

                return path + @"\" + _consoleOutputFilename;
            }
        }

        #endregion

        #region Controller methods

        public static void Start()
        {
            // Need to have everything complete 
            // before keyboard form is shown, so no background tasks.
            KeyboardHelper.GetInstalledKeyboardList();
            LoadCustomKeyboardLayouts();
            SetLocale();
            EstablishSituation();


        }

        private static void LoadCustomKeyboardLayouts()
        {
            string path = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                    + @"\KeyMapper\customlayouts.txt";

            if (File.Exists(path) == false)
                return;

            string customLayouts;

            using (StreamReader sr = new StreamReader(path))
            {
                customLayouts = sr.ReadToEnd();
            }

            // Beware bad data..
            try
            {
                string[] layouts = customLayouts.Split(new char[] { (char)13 }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string nameValuePair in layouts)
                {
                    if (String.IsNullOrEmpty(nameValuePair))
                        continue;

                    int index = nameValuePair.IndexOf("=", StringComparison.InvariantCulture);
                    if (index < 0)
                        continue;

                    string locale = nameValuePair.Substring(0, index);
                    int value;
                    if (System.Int32.TryParse(nameValuePair.Substring(index + 1), out value) == false)
                        continue;

                    KeyboardLayoutType keyboardType = (KeyboardLayoutType)value;

                    _customKeyboardLayouts.Add(locale, keyboardType);

                }
            }
            catch { }


        }
        
        public static void AddCustomLayout()
        {
            if (_customKeyboardLayouts.Contains(_currentLocale))
                _customKeyboardLayouts.Remove(_currentLocale);

            _customKeyboardLayouts.Add(_currentLocale, _keyboardLayout);
        }

        private static void SaveCustomLayouts()
        {
            if (_customKeyboardLayouts.Count == 0)
                return;

            string path = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
             + @"\KeyMapper\customlayouts.txt";

            using (StreamWriter sw = new StreamWriter(path))
            {
                foreach (DictionaryEntry de in _customKeyboardLayouts)
                    sw.Write(de.Key + "=" + (int)de.Value);

            }


        }

        public static void Close()
        {
            SaveCustomLayouts();

            KeyboardHelper.UnloadLayout();

            if (AppController.OperatingSystemIsVista
                && AppController.UserCanWriteBootMappings == false
                && (MappingsManager.IsRestartRequired() || MappingsManager.VistaMappingsNeedSaving()))
                MappingsManager.SaveBootMappingsVista();

            CloseConsoleOutput();

            foreach (string filepath in _tempfiles)
            {
                try
                {
                    System.IO.File.Delete(filepath);
                }
                catch { }
            }
        }

        public static string GetKeyFontName(bool localizable)
        {

            if (_arialUnicodeMSInstalled == null)
            {
                _arialUnicodeMSInstalled = false;
                InstalledFontCollection installedFonts = new InstalledFontCollection();
                FontFamily[] fonts = installedFonts.Families;
                foreach (FontFamily ff in fonts)
                {
                    if (ff.Name == "Arial Unicode MS")
                    {
                        _arialUnicodeMSInstalled = true;
                        _defaultKeyFont = "Arial Unicode MS";
                        break;
                    }

                }
            }



            if (localizable == false || (bool)_arialUnicodeMSInstalled)
                return _defaultKeyFont; // Don't want the static keys to change font.

            // Default font for keys is Lucida Sans Unicode as it's on every version of Windows
            // (Could look for Arial Unicode MS (which is installed by Office) I suppose as it has lots more in
            // Bit concerned as it's > 20MB in size though.)

            // Lucida Sans Unicode simply doesn't contain the characters for Bengali & Malayalam
            // Differnet versions of Windows have differernt cultures installed 
            // e.g. the two above were installed by XP SP2 ..

            // It's possible the culture has been installed but the font has been 
            // deleted. That _could_ be time to wheel out Arial Unicode, would need to test if font is installed each time.

            switch (_currentCultureInfo.LCID)
            {
                case 1081: // Devanagari
                    return "Mangal";
                case 1093: // Bengali. Raavi would be a choice for Gurmukhi but it doesn't seem to be in the installed list..
                    return "Vrinda";
                case 1100: // Malayam
                    return "Kartika";
                case 1095: // Gujurati
                    return "Shruti";
                case 1099: // Kannada
                    return "Tunga";
                case 1097: // Tamil
                    return "Latha";
                case 1098: // Telugu
                    return "Gautami";
                case 1037: // Hebrew
                    return "Miriam";
                case 1041: // Japanese
                    return "MS Mincho";
                case 1105:
                    return "Microsoft Himalaya";

                case 1125: // Divehi: "MV Boli" is exactly the same as LSU except one key - the F2D2 key 
                // which LSU gets wrong. However MV Boli but looks too 'cartoonish' on the number keys for my liking.

                case 1054: // Thai - for some reason all the Thai fonts come out far too small ..??

                default:
                    return _defaultKeyFont;
            }
        }

        private static void EstablishSituation()
        {

            // The current mappings in effect are composed of:
            // 1) HKLM mappings which aren't overridden in HKCU
            // 2) Mappings in HKCU

            // In order to notify user when a restart or logoff is required, we need to track 
            // what mappings were in effect the first time the program was run after reboot/logoff.

            // To do that we need a registry key of our own.
            RegistryKey kmregkey = null;
            try
            {
                kmregkey = Registry.CurrentUser.OpenSubKey(_appKeyName, true);
            }
            catch (System.Security.SecurityException e)
            {
                Console.WriteLine("Can't access KeyMapper registry key: {0}", e);
                _userCannotWriteToApplicationRegistryKey = true;
            }

            bool savedMappingsExist = false;

            if (kmregkey == null && _userCannotWriteToApplicationRegistryKey == false)
            {
                // Key does not exist, or no permissions to write:
                // Create it. Or at least try..
                try
                {
                    kmregkey = Registry.CurrentUser.CreateSubKey(_appKeyName);
                }
                catch (System.Security.SecurityException e)
                {
                    Console.WriteLine("Cannot create KeyMapper registry key: {0}", e);
                    _userCannotWriteToApplicationRegistryKey = true;
                }

            }

            if (kmregkey != null)
            {

                string[] values = kmregkey.GetValueNames();
                for (int i = 0; i < values.Length; i++)
                {
                    if (values[i] == "UserMaps" || values[i] == "BootMaps")
                    {
                        savedMappingsExist = true;
                        break;
                    }
                }

                kmregkey.Close();
                // Really should have access to this key as it's in the user hive. 
                // But it isn't a requirement, as such: can't save custom colours without it.
            }

            // Mappings in HKCU override mappings in HKLM

            // If user uses Fast User Switching to switch
            // to an account which is already logged in, the HKCU mappings disappear.

            // Is the current user able to write to the Keyboard Layout key in HKLM??
            // (This key always exists, Windows recreates it if it's deleted)

            _canWriteBootMappings = RegistryHelper.CanUserWriteToKey
                    (RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Keyboard Layout");

            _isWindows2000 =
                (System.Environment.OSVersion.Version.Major < 5
                | (System.Environment.OSVersion.Version.Major == 5 & System.Environment.OSVersion.Version.Minor == 0));

            _isVista = System.Environment.OSVersion.Version.Major > 5;

            // When was the system booted? (Milliseconds vs Ticks is correct..)
            DateTime boottime = DateTime.Now - TimeSpan.FromMilliseconds(System.Environment.TickCount);

            // When did the current user log in?

            DateTime logontime = RegistryHelper.GetRegistryKeyTimestamp(RegistryHive.CurrentUser, "Volatile Environment");

            // Now, the "Volatile Environment" key in RegistryHive.CurrentUser
            // >isn't< always unloaded on logoff. I though there was a fallback though..
            //  querying the user's ADSI LastLogin property. Unfortunately 
            // this gets the last time logged in >including when unlocking Windows<
            // so this is as good as it gets.

            // Sometimes, as well, logontime returns the wrong time. I think this is because when 
            // the system writes the Volatile Environment subkey, it hasn't yet loaded the correct
            // time zone or isn't respecting Daylight Saving. Sometimes, on some computers..

            // It can also happen - e.g. when restoring a Parallels Virtual Machine - 
            // that the boottime is later than logontime.

            if (boottime > logontime)
            {
                Console.WriteLine("Boot time greater than logontime: Boot Time {0} Logon Time {1}", boottime, logontime);
                boottime = logontime.AddSeconds(-1);
            }

            // Just in case the timestamp bug ever works in reverse:
            if (logontime > DateTime.Now)
            {
                Console.WriteLine("Logontime greater than Now: Logon Time {0}, Now: {1}", logontime, DateTime.Now);
                logontime = DateTime.Now;
            }

            // When was HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Keyboard Layout written?
            DateTime HKLMWrite = RegistryHelper.GetRegistryKeyTimestamp
                (RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Keyboard Layout");

            // When was HKEY_CURRENT_USER\Keyboard Layout written?
            DateTime HKCUWrite = RegistryHelper.GetRegistryKeyTimestamp
                (RegistryHive.CurrentUser, @"Keyboard Layout");

            //  Console.WriteLine("Booted: {0}, Logged On: {1}, HKLM {2}, HKCU {3}", boottime, logontime, HKLMWrite, HKCUWrite);

            // Get the current scancode maps
            MappingsManager.GetMappingsFromRegistry();

            // If user mappings are inappropriate (win2k) default to boot.
            if (_isWindows2000)
                MappingsManager.SetFilter(MappingFilter.Boot);

            // If HLKM or HKCU Mappings have not been changed since boot/login 
            // (ie their timestamp is earlier than the boot/login time)
            // then save them to our own reg key. This means we can determine whether a 
            // restart or logoff is required because the current mappings are different from the saved mappings.

            if (HKLMWrite < boottime || savedMappingsExist == false)
            {
                MappingsManager.SaveBootMappingsToKeyMapperKey();

            }

            if (HKCUWrite < logontime || savedMappingsExist == false)
            {
                MappingsManager.SaveUserMappingsToKeyMapperKey();

            }

            if (savedMappingsExist == false)
                MappingsManager.StoreUnsavedMappings();

            if (_isVista)
                MappingsManager.SaveMappings(Mappings.CurrentBootMappings, MapLocation.KeyMapperVistaMappingsCache);



        }

        private static void DeleteInvalidUserFileFromException(ConfigurationException ex)
        {
            Console.WriteLine("User Config file is invalid - resetting to default");

            string fileName = "";

            if (!string.IsNullOrEmpty(ex.Filename))
            {
                fileName = ex.Filename;
            }
            else
            {
                System.Configuration.ConfigurationErrorsException innerException =
                ex.InnerException as System.Configuration.ConfigurationErrorsException;
                if (innerException != null && !string.IsNullOrEmpty(innerException.Filename))
                {
                    fileName = innerException.Filename;
                }
            }
            if (System.IO.File.Exists(fileName))
            {
                System.IO.File.Delete(fileName);
            }


        }

        public static void ValidateUserConfigFile()
        {
            try
            {
                // If file is corrupt this will trigger an exception
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            }

            catch (ConfigurationErrorsException ex)
            {
                DeleteInvalidUserFileFromException(ex);
                return;
            }

            try
            {
                // Access a property to find any other error types - invalid XML etc.
                Properties.Settings u = new KeyMapper.Properties.Settings();
                Point p = u.ColourEditorLocation;
            }


            catch (ConfigurationErrorsException ex)
            {
                DeleteInvalidUserFileFromException(ex);
            }

        }

        private static void SetLocale()
        {
            SetLocale(null);
        }

        public static void SetLocale(string locale)
        {

            // Only want to reset locale temporarily so save current value
            string currentkeyboardlocale = KeyboardHelper.GetCurrentKeyboardLocale();

            if (String.IsNullOrEmpty(locale))
            {
                // At startup we need to load the current locale.
                locale = currentkeyboardlocale;
            }

            if ((locale != _currentLocale))
            {
                if (_customKeyboardLayouts != null && _customKeyboardLayouts.ContainsKey(locale))
                    _keyboardLayout = (KeyboardLayoutType)_customKeyboardLayouts[locale];
                else
                {
                    // Ask the keydata interface what kind of layout this locale has - US, Euro etc.
                    _keyboardLayout = new KeyDataXml().GetKeyboardLayoutType(locale);
                }

                // Load the keyboard layout for the minimum possible time and keep the results:
                // This can error with some cultures, problems with framework, unhandled thread exception occurs.
                try
                {
                    // if (_currentCultureInfo != null)
                    // Console.WriteLine("Setting culture to: LCID: {0}", _currentCultureInfo.LCID);

                    int culture = KeyboardHelper.SetLocale(locale);
                    _currentCultureInfo = new CultureInfo(culture);

                    _currentLayout = new LocalizedKeySet();
                    _currentLocale = locale;
                }

                catch (Exception ex)
                {
                    Console.WriteLine("Set Locale Exception: {0}", ex);
                }

                finally
                {
                    // Set it back (if different)
                    if (_currentLocale != currentkeyboardlocale)
                        KeyboardHelper.SetLocale(currentkeyboardlocale);

                }
            }

        }

        public static void SwitchKeyboardLayout(KeyboardLayoutType layout)
        {
            _keyboardLayout = layout;
        }

        public static bool ActivateExistingInstance()
        {
            _appMutex = new AppMutex();
            bool gotMutex = _appMutex.GetMutex();
            if (gotMutex == false)
                SwitchToExistingInstance();

            return gotMutex;
        }

        private static void SwitchToExistingInstance()
        {
            IntPtr hWnd = IntPtr.Zero;
            Process process = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(process.ProcessName);
            foreach (Process _process in processes)
            {
                // Get the first instance that is not this instance, has the
                // same process name and was started from the same file name
                // and location. Also check that the process has a valid
                // window handle in this session to filter out other user's
                // processes.
                if (_process.Id != process.Id &&
                    _process.MainModule.FileName == process.MainModule.FileName &&
                    _process.MainWindowHandle != IntPtr.Zero)
                {
                    hWnd = _process.MainWindowHandle;
                    break;
                }
            }



            if (hWnd != IntPtr.Zero)
            {
                // Restore window if minimised. Do not restore if already in
                // normal or maximised window state, since we don't want to
                // change the current state of the window.
                if (NativeMethods.IsIconic(hWnd) != 0)
                {
                    NativeMethods.ShowWindow(hWnd, 9); // SW_RESTORE
                }

                // Set foreground window.
                NativeMethods.SetForegroundWindow(hWnd);
            }
        }

        public static bool DotNetFramework2ServicePackInstalled
        {
            get
            {
                if (_dotNetFrameworkSPInstalled == null)
                {
                    // The key exists in Vista and Windows 2008 Server
                    // but no need to check it.
                    if (System.Environment.OSVersion.Version.Major > 5)
                        _dotNetFrameworkSPInstalled = true;
                    else
                    {
                        int sp = 0;
                        RegistryKey regkey = Registry.LocalMachine.OpenSubKey(
                            @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v2.0.50727");

                        if (regkey != null)
                        {
                            sp = (int)regkey.GetValue("SP", 0);
                            regkey.Close();
                        }
                        _dotNetFrameworkSPInstalled = (sp > 0);
                    }
                    if (_dotNetFrameworkSPInstalled == false)
                        Console.WriteLine("There is a Service Pack available for the .NET framework 2 available from http://tinyurl.com/5a47nf");
                }

                return (bool)_dotNetFrameworkSPInstalled;
            }
        }

        public static void RegisterTempFile(string filepath)
        {
            _tempfiles.Add(filepath);
        }

        #endregion

        #region Log methods

        public static void ClearLogFile()
        {
            if (_consoleWriterStream != null)
            {
                _consoleWriterStream.BaseStream.SetLength(0);
                Console.WriteLine("Log file cleared: {0}", DateTime.Now);
            }
            else
                Console.Write("Can't clear log in debug mode.");
        }

        public static void RedirectConsoleOutput()
        {

            string path = LogFileName;
            string existingLogEntries = String.Empty;

            if (String.IsNullOrEmpty(path))
                return;

            if (File.Exists(path))
            {
                // In order to be able to clear the log, the streamwriter must be opened in create mode.
                // so read the contents of the log first.

                using (StreamReader sr = new StreamReader(path))
                {
                    existingLogEntries = sr.ReadToEnd();
                }
            }

            _consoleWriterStream = new StreamWriter(path, false, System.Text.Encoding.UTF8);
            _consoleWriterStream.AutoFlush = true;
            _consoleWriterStream.Write(existingLogEntries);

            // Direct standard output to the log file.
            Console.SetOut(_consoleWriterStream);

            Console.WriteLine("Logging started: {0}", DateTime.Now);
        }

        public static void CloseConsoleOutput()
        {
            if (_consoleWriterStream != null)
            {
                _consoleWriterStream.Close();
            }
        }


        #endregion

        #region Key methods

        public static Font GetButtonFont(float size, bool localizable)
        {

            if (size == 0)
            {
                Console.WriteLine("ERROR: Zero sized font requested");
                size = 10;
            }

            Font font = new Font(GetKeyFontName(localizable), size);
            return font;
        }


        public static void SetFontSizes(float scale)
        {


            // See what font size fits the scaled-down button 
            float basefontsize = 36F;

            Font font = AppController.GetButtonFont(basefontsize, false);

            // Not using ButtonImages.GetButtonImage as that is where we were called from..
            using (Bitmap bmp = ButtonImages.ResizeBitmap(AppController.GetBitmap(BlankButton.Blank), scale, false))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                // Helps MeasureString. Can also pass StringFormat.GenericTypographic apparently ??

                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                int CharacterWidth = (int)g.MeasureString(((char)77).ToString(), font).Width;
                // Only use 90% of the bitmap's size to allow for the edges (especially at small sizes)
                float ratio = ((float)((0.9F * bmp.Height) / 2)) / (float)CharacterWidth;
                basefontsize = (basefontsize * ratio);
            }

            _baseFontSize = basefontsize;

        }

        public static string GetKeyName(int scancode, int extended)
        {
            // Look up the values in the current localized layout.
            if (scancode == 0 && extended == 0)
                return "Disabled";

            if (scancode == -1 && extended == -1)
                return "";

            int hash = AppController.GetHashFromKeyData(scancode, extended);
            if (_currentLayout.ContainsKey(hash))
            {
                return _currentLayout.GetKeyName(hash);
            }
            else
            {
                Console.WriteLine("Unknown key: sc {0} ex {1}", scancode, extended);
                return "Unknown";
            }

        }

        public static Bitmap GetBitmap(BlankButton button)
        {
            // Have we already extracted this bmp?
            // Buttons are stored as lower case.
            string buttonname = button.ToString().ToLowerInvariant();

            //foreach (Bitmap loadedbutton in _buttonCache)
            //{
            //    if (loadedbutton != null)
            //    {
            //        if (String.Compare(loadedbutton.Tag.ToString().ToLowerInvariant(), buttonname, StringComparison.OrdinalIgnoreCase) == 0)
            //        {
            //            return (Bitmap)loadedbutton.Clone();
            //        }
            //    }
            //}

            Bitmap bmp = ButtonImages.GetImage(buttonname, "png");

            //bmp.Tag = buttonname.ToString();
            // _buttonCache.Add(bmp);

            return bmp;
            // (Bitmap)bmp.Clone();
        }

        public static bool IsOverlongKey(int hash)
        {
            return _currentLayout.IsKeyNameOverlong(hash);
        }

        public static bool IsLocalizableKey(int hash)
        {
            return _currentLayout.IsKeyLocalizable(hash);
        }

        #endregion

        #region Miscellaneous

        public static int GetHighestCommonDenominator(int value1, int value2)
        {
            // Euclidean algorithm
            if (value2 == 0) return value1;
            return GetHighestCommonDenominator(value2, value1 % value2);
        }

        // This needs to be in a separate method as this property was introduced in .Net Framework 2
        // Service Pack 1, so JITing a method containing the property raises an exception.
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        public static void EnableVisualUpgrade(FileDialog fd)
        {
            fd.AutoUpgradeEnabled = true;
        }

        #endregion

        #region Write to protected registry sections on Vista

        public static bool ConfirmWriteToProtectedSectionOfRegistryOnVista(string innertext)
        {
            string text = "In order to write " + innertext + ", Key Mapper needs to add to " +
                   "the protected section of your computer's registry. You may need to approve this action " +
                       "which will be performed by your Registry Editor.";

            TaskDialogResult result = FormsManager.ShowTaskDialog("Do you want to proceed?", text, "Key Mapper",
                       TaskDialogButtons.Yes | TaskDialogButtons.No, TaskDialogIcon.SecurityShield);

            if (result == TaskDialogResult.Yes)
                return true;
            else
                return false;

        }

        public static void WriteRegistryFileToProtectedSectionOfRegistryOnVista(string filepath)
        {

            string command = " /s " + (char)34 + filepath + (char)34;
            
            try
            {
                System.Diagnostics.Process.Start("regedit.exe", command);
            }
            catch (System.ComponentModel.Win32Exception)
            {
                Console.WriteLine("Writing to protected section of registry was cancelled");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error writing to registry: {0}", ex);
            }

            _tempfiles.Add(filepath);

        }

        public static void WriteToProtectedSectionOfRegistryOnVista(RegistryHive registryHive, string key, string valueName, string value)
        {

            string filename = System.IO.Path.GetTempPath() + Path.GetRandomFileName() + ".reg";

            using (StreamWriter sw = new StreamWriter(filename, false, System.Text.Encoding.Unicode))
            {
                sw.WriteLine("Windows Registry Editor Version 5.00");
                sw.WriteLine();

                string hive;
                switch (registryHive)
                {
                    case RegistryHive.LocalMachine:
                        hive = "HKEY_LOCAL_MACHINE";
                        break;
                    case RegistryHive.Users:
                        hive = "HKEY_USERS";
                        break;
                    default:
                        throw new InvalidOperationException(registryHive.ToString() + " not supported");
                }

                sw.WriteLine(@"[" + hive + @"\" + key + "]");
                sw.Write("\"" + valueName + "\"=");
                if (String.IsNullOrEmpty(value))
                    sw.Write("-");
                else
                    sw.WriteLine((char)34 + value + (char)34);
            }

            AppController.WriteRegistryFileToProtectedSectionOfRegistryOnVista(filename);
         

        }

        #endregion


        #region Key codings

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId = "scancode*1000")]
        public static int GetHashFromKeyData(int scancode, int extended)
        {
            // Need to preserve the actual extended value as they are all 224 except Pause
            // which is 225.
            return (scancode * 1000) + extended;
        }

        public static int GetScancodeFromHash(int hash)
        {
            return (hash / 1000);
        }

        public static int GetExtendedFromHash(int hash)
        {
            // Extended value is 224 when set.
            return (hash % 1000);
        }

        #endregion

    }




}

