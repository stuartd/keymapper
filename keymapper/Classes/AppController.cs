using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Security.Permissions;
using System.Threading;
using System.IO;
using System.IO.IsolatedStorage;
using System.Configuration;


namespace KeyMapper
{

    public static class AppController
    {

        #region Fields, Properties

        // Always use the provided method to get this
        // as substitutions must be made for some cultures..
        private static string _defaultKeyFont = "Lucida Sans Unicode";

        // Caches
        private static Hashtable _fontcache = new Hashtable();
        private static List<Bitmap> _buttoncache = new List<Bitmap>();

        // Keyboard keys & layout
        private static string _currentlocale;
        private static CultureInfo _currentCultureInfo;
        private static LocalizedKeySet _currentlayout;

        // Base font size for drawing text on keys
        private static float _basefontsize;

        // Keyboard layouts, physical and localized.
        private static KeyboardLayoutType _keyboardlayout;

        // KeyMapper's own reg key in HKCU
        private const string _appkeyname = @"Software\KeyMapper";

        // Whether current user can write to HKLM
        private static bool _canWriteBootMappings;

        // Whether user mappings work (they don't on W2k)
        private static bool _canHaveLocalUserMappings;

        private static AppMutex _appmutex;

        private static System.Windows.Forms.IWin32Window _keyboardFormHandle;

        private static bool _isConsoleRedirected;
        private static StreamWriter _consoleWriterStream;
        private static string _consoleOutputFilename = "keymapper.log";

        // Properties

        public static System.Windows.Forms.IWin32Window KeyboardFormHandle
        {
            get { return _keyboardFormHandle; }
            set { _keyboardFormHandle = value; }
        }

        public static bool UserCannotWriteMappings
        {
            get
            {
                return (MappingsManager.Filter == MappingFilter.Boot && !_canWriteBootMappings);
            }
        }

        public static bool UserCanWriteBootMappings
        {
            get { return _canWriteBootMappings; }
        }

        public static bool LocalUserMappingsAllowed
        {
            get { return _canHaveLocalUserMappings; }
        }

        public static string ApplicationRegistryKeyName
        {
            get { return _appkeyname; }
        }

        public static KeyboardLayoutType KeyboardLayout
        {
            get { return _keyboardlayout; }
        }

        public static float BaseFontSize
        {
            get { return _basefontsize; }
        }

        public static CultureInfo CurrentCultureInfo
        {
            get { return _currentCultureInfo; }
        }

        #endregion

        #region Controller methods

        public static void StartAppController()
        {
            StartBackgroundTasks();
            SetLocale();
            EstablishSituation();
        }

        public static void CloseAppController()
        {
            ClearFontCache();

            foreach (Bitmap bmp in _buttoncache)
            {
                if (bmp != null)
                    bmp.Dispose();
            }
            _buttoncache.Clear();

            KeyboardHelper.UnloadLayout();
            CloseConsoleOutput();

        }

        private static void StartBackgroundTasks()
        {
            ThreadStart job1 = new ThreadStart(KeyboardHelper.GetInstalledKeyboardList);
            Thread thread1 = new Thread(job1);
            thread1.Start();
        }

        public static string GetKeyFontName(bool localizable)
        {

            if (localizable == false)
                return _defaultKeyFont; // Don't want the static keys to change.

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

        public static void RedirectConsoleOutput()
        {

            try
            {
                string path = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\KeyMapper";
                if (Directory.Exists(path) == false)
                    Directory.CreateDirectory(path);

                _consoleWriterStream = new StreamWriter(path + @"\" + _consoleOutputFilename, true, System.Text.Encoding.UTF8);

            }
            catch (IOException exc)
            {
                // Tree falling in the woods...
                Console.WriteLine(exc.Message + "Can't create log file.");
                return;
            }

            _isConsoleRedirected = true;

            // Direct standard output to the log file.
            Console.SetOut(_consoleWriterStream);
        }

        public static void CloseConsoleOutput()
        {
            if (_isConsoleRedirected)
            {
                _consoleWriterStream.Close();

            }
        }


        private static void EstablishSituation()
        {

            // The current mappings in effect are composed of:
            // 1) HKLM mappings which aren't overridden in HKCU
            // 2) Mappings in HKCU

            // In order to notify user when a restart or logoff is required, we need to track 
            // what mappings were in effect the first time the program was run after reboot/logoff.

            // To do that we need a registry key of our very own, so first up:

            RegistryKey kmregkey = Registry.CurrentUser.OpenSubKey(_appkeyname, true);
            bool savedMappingsExist = true;

            if (kmregkey == null)
            {
                // Key does not exist, or no permissions to write:
                // Create it. Or at least try..
                kmregkey = Registry.CurrentUser.CreateSubKey(_appkeyname);

                // At this point, we know that we have no saved record of what the mappings were
                // previously, as the reg key doesn't exist. In this case, we want to be able to 
                // show the mappings as "existing" as they almost certainly are.

                // (It's also possible that this key exists but the values don't if they've been manually deleted)
                savedMappingsExist = false;

            }

            if (kmregkey != null)
            {
                kmregkey.Close();
                // Really should have access to this key as it's in the user hive. But it isn't a requirement, as such.
                // TODO: Test what happens when user has a) read only access and b) no access
                // As later we try to save to this key anyway..
            }

            // Mappings in HKCU override mappings in HKLM

            // If user uses Fast User Switching to switch
            // to an account which is already logged in, the HKCU mappings disappear.

            // So.

            // Is the current user able to write to the Keyboard Layout key in HKLM??
            // (This key always exists, Windows recreates it if it's deleted)

            _canWriteBootMappings = RegistryHelper.CanUserWriteToKey
                    (RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Keyboard Layout");

            // Are we using an OS later than Windows 2000 where user mappings are allowed?
            _canHaveLocalUserMappings =
                System.Environment.OSVersion.Version.Major > 5
                | (System.Environment.OSVersion.Version.Major == 5 & System.Environment.OSVersion.Version.Minor > 0);

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

            // It can also happen I think - when awakening a VM from sleep - that is boottime later than logontime.

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

            Console.WriteLine("Booted: {0}, Logged On: {1}, HKLM {2}, HKCU {3}", boottime, logontime, HKLMWrite, HKCUWrite);

            // Get the current scancode maps
            MappingsManager.GetMappingsFromRegistry();

            // If user mappings are inappropriate (win2k) default to boot.
            if (_canHaveLocalUserMappings == false)
                MappingsManager.SetFilter(MappingFilter.Boot);

            // If HLKM or HKCU Mappings have not been changed since boot/login 
            // (ie their timestamp is earlier than the boot/login time)
            // then save them to our own reg key. This means we can determine whether a 
            // restart or logoff is required because the current mappings are different from the saved mappings.

            if (HKLMWrite < boottime || savedMappingsExist == false)
            {
                MappingsManager.SaveMappings(Mappings.CurrentBootMappings,
                    MapLocation.KeyMapperLocalMachineKeyboardLayout);
                // As have overwritten our stored value with a new one, reload it ..
                MappingsManager.GetMappingsFromRegistry(MapLocation.KeyMapperLocalMachineKeyboardLayout);
                // .. and recalculate mappings.
                MappingsManager.PopulateMappingLists();

            }

            if (HKCUWrite < logontime || savedMappingsExist == false)
            {
                MappingsManager.SaveMappings(Mappings.CurrentUserMappings,
                    MapLocation.KeyMapperCurrentUserKeyboardLayout);
                MappingsManager.GetMappingsFromRegistry(MapLocation.KeyMapperCurrentUserKeyboardLayout);
                MappingsManager.PopulateMappingLists();
            }


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
                // If the file is invalid
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            }

            catch (ConfigurationErrorsException ex)
            {
                DeleteInvalidUserFileFromException(ex);
            }

            try
            {
                // Access a property to find any other error types
                Properties.Settings u = new KeyMapper.Properties.Settings();
                Point p = u.ColourEditorLocation;
            }


            catch (ConfigurationErrorsException ex)
            {
                DeleteInvalidUserFileFromException(ex);
            }

        }






        public static bool IsLaptop()
        {
            // Going to be good enough for w2k + - no need for impersonation.
            // Just in case WMI is hosed, and this isn't a critical piece of info, wrap/try

            int chassisType = 0;

            try
            {
                System.Management.ManagementScope wmi = new System.Management.ManagementScope();
                System.Management.SelectQuery query = new System.Management.SelectQuery(@"select ChassisTypes from Win32_SystemEnclosure");
                System.Management.ManagementObjectSearcher searcher = new System.Management.ManagementObjectSearcher(wmi, query);
                System.Management.ManagementObjectCollection coll = searcher.Get();
                foreach (System.Management.ManagementObject m in coll)
                {
                    short[] chtype = m.Properties["ChassisTypes"].Value as short[];
                    chassisType = chtype[0];
                }
            }

            catch (InvalidOperationException)
            {
                // No need to do anything.
            }

            //  Look for  Portable, Laptop, Notebook, Handheld, Sub-Notebook
            if (chassisType == 8 ||
                chassisType == 9 ||
                chassisType == 10 ||
                chassisType == 11 ||
                chassisType == 14)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private static void SetLocale()
        {
            SetLocale(null);
        }

        public static void SetLocale(string locale)
        {
            SetLocale(locale, false);
        }

        public static void SetLocale(string locale, bool force)
        {
            // In case we are switching between locales and loading a different font for each
            // without changing size..
            ClearFontCache();

            // Only want to reset locale temporarily so save current value
            string currentkeyboardlocale = KeyboardHelper.GetCurrentKeyboardLocale();

            if (String.IsNullOrEmpty(locale))
            {
                // At startup we need to load the current locale.
                locale = currentkeyboardlocale;
            }

            if ((locale != _currentlocale) || force)
            {

                // Ask the keydata interface what kind of layout this locale has - US, Euro etc.
                _keyboardlayout = new KeyDataXml().GetKeyboardLayoutType(locale);

                // Load the keyboard layout for the minimum possible time and keep the results:
                // This can error with some cultures, problems with framework, unhandled thread exception occurs.
                try
                {
                    if (_currentCultureInfo != null)
                        Console.WriteLine("Setting culture to: LCID: {0}", _currentCultureInfo.LCID);

                    int culture = KeyboardHelper.SetLocale(locale);
                    _currentCultureInfo = new CultureInfo(culture);

                    _currentlayout = new LocalizedKeySet();
                    _currentlocale = locale;
                }

                catch (Exception ex)
                {
                    // TODO: Remove before production.
                    System.Windows.Forms.MessageBox.Show(ex.ToString());
                }

                finally
                {
                    // Set it back (if different)
                    if (_currentlocale != currentkeyboardlocale)
                        KeyboardHelper.SetLocale(currentkeyboardlocale);

                }
            }

        }

        public static void SwitchKeyboardLayout(KeyboardLayoutType layout)
        {
            _keyboardlayout = layout;
        }

        public static bool CheckForExistingInstances()
        {
            _appmutex = new AppMutex();
            return (!_appmutex.GetMutex());
        }

        #endregion

        #region Cache methods

        public static Font GetFontFromCache(float size, bool localizable)
        {

            string hash = GetKeyFontName(localizable) + size.ToString(CultureInfo.InvariantCulture.NumberFormat);

            if (_fontcache.Contains(hash))
            {
                return (Font)_fontcache[hash];
            }

            Font font = new Font(GetKeyFontName(localizable), size);
            _fontcache.Add(hash, font);
            return font;
        }

        private static void ClearFontCache()
        {

            foreach (Font f in _fontcache.Values)
            {
                if (f != null)
                    f.Dispose();
            }
            _fontcache.Clear();

        }

        #endregion

        #region Key methods

        public static void SetFontSizes(float scale)
        {
            // When the scale changes, the fonts all change so may as well release 
            // what we have as they won't get used again.

            AppController.ClearFontCache();

            // See what font size fits the scaled-down button 
            float basefontsize = 36F;

            Font font = AppController.GetFontFromCache(basefontsize, false);

            using (Bitmap bmp = ButtonImages.ResizeBitmap(AppController.GetBitmap(BlankButton.Blank), scale, false))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                // Helps MeasureString. Can also pass StringFormat.GenericTypographic apparently ??

                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                int CharacterWidth = (int)g.MeasureString("@", font).Width;
                // Only use 90% of the bitmap's size to allow for the edges (especially at small sizes)
                float ratio = ((float)((0.9F * bmp.Height) / 2)) / (float)CharacterWidth;
                basefontsize = (basefontsize * ratio);
            }

            _basefontsize = basefontsize;

            // Console.WriteLine("Base: {0} Double: {1} Multi: {2}", FontSizeSingle, FontSizeDouble, FontSizeMulti);

        }

        public static string GetKeyName(int scancode, int extended)
        {
            // Look up the values in our current layout.
            if (scancode == 0 && extended == 0)
                return "Disabled";

            if (scancode == -1 && extended == -1)
                return "";

            int hash = AppController.GetHashFromKeyData(scancode, extended);
            if (_currentlayout.ContainsKey(hash))
            {
                return _currentlayout.GetKeyName(hash);
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

            foreach (Bitmap loadedbutton in _buttoncache)
            {
                if (loadedbutton != null)
                {
                    // if (StringComparison.OrdinalIgnoreCase

                    if (String.Compare(loadedbutton.Tag.ToString().ToLowerInvariant(), buttonname, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        return (Bitmap)loadedbutton.Clone();
                    }
                }
            }

            Bitmap bmp = ButtonImages.GetImage(buttonname);

            bmp.Tag = buttonname.ToString();
            _buttoncache.Add(bmp);

            return (Bitmap)bmp.Clone();
        }

        public static bool IsOverlongKey(int hash)
        {
            return _currentlayout.IsKeyNameOverlong(hash);
        }

        public static bool IsLocalizableKey(int hash)
        {
            return _currentlayout.IsKeyLocalizable(hash);
        }

        #endregion

        #region Miscellaneous

        internal static int GetHighestCommonDenominator(int a, int b)
        {
            // Euclidean algorithm
            if (b == 0) return a;
            return GetHighestCommonDenominator(b, a % b);
        }

        //[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        //public static void InitiateLogOff(bool restart)
        //{

        //    // Windows XP or later use shutdown.exe (especially with /g for Vista to restart open apps!)

        //    if (Environment.OSVersion.Version.Major >= 5 & Environment.OSVersion.Version.Minor >= 1)
        //    {
        //        // Won't work on WIn2k (shouldn't be here anyway)
        //        System.Diagnostics.Process shutdown = new System.Diagnostics.Process();
        //        shutdown.StartInfo.FileName = "shutdown.exe";
        //        // /g for Vista

        //        if (restart)
        //        {
        //            if (Environment.OSVersion.Version.Major > 5)
        //                shutdown.StartInfo.Arguments =
        //                    " /g /c \"Restart initiated on your behalf by KeyMapper\" /d p:2:4";
        //            else
        //                shutdown.StartInfo.Arguments =
        //                    "/r /c \"Restart initiated on your behalf by KeyMapper. Type shutdown /a in a command prompt to cancel\" /d p:2:4";
        //        }
        //        else
        //        {
        //            // Log off.
        //            shutdown.StartInfo.Arguments = " /l";
        //        }
        //        shutdown.Start();
        //        shutdown.Close();

        //    }
        //	}

        #endregion

        #region Key codings

        public static int GetHashFromKeyData(int scancode, int extended)
        {
            // Slide scancode one bit to the left and add one if extended is nonzero.
            return ((scancode << 1) + (extended != 0 ? 1 : 0));
        }

        public static int GetScancodeFromHash(int hash)
        {
            return (hash >> 1);
        }

        public static int GetExtendedFromHash(int hash)
        {
            // Extended value is 224 when set.
            return ((hash % 2) != 0 ? 224 : 0);
        }

        #endregion

    }




}

