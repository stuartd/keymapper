using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Windows.Forms;
using KeyMapper.Classes.Interop;
using KeyMapper.Providers;
using Microsoft.Win32;

namespace KeyMapper.Classes
{
    public static class AppController
    {
        // Always use the provided method to get this
        // as substitutions must be made for some cultures unless Arial Unicode MS in installed
        private static string _defaultKeyFont = "Lucida Sans Unicode";

        // Keyboard layout and keys

        private static LocalizedKeySet _currentLayout;

        // Single instance handle
        private static AppMutex appMutex;

        private static bool? arialUnicodeMSInstalled;

        private static readonly List<string> tempfiles = new List<string>();

        // Ready for extraction (home grown ioc of some kind? as long as they don't have dependencies)

        private static readonly IOperatingSystemCapability operatingSystemCapability = new OperatingSystemCapabilityProvider();

        private static readonly IRegistryTimestampService registryTimestampService = new RegistryTimestampService();

        private static Hashtable customKeyboardLayouts { get; set; }

        private static string currentLocale { get; set; }

        public static bool UserCannotWriteToApplicationRegistryKey { get; private set; }

        public static bool UserCannotWriteMappings
        {
            get
            {
                // Can't write mappings if *all* of these are true:
                // a) Currently looking at Boot Mappings
                // b) Earlier than Vista (ie doesn't implement UAC)
                // c) User can't write boot mappings.

                // (XP doesn't allow process elevation, so if you can't then you can't)
                return (MappingsManager.Filter == MappingFilter.Boot
                        && !UserCanWriteBootMappings
                        && !operatingSystemCapability.ImplementsUAC);
            }
        }

        public static bool UserCanWriteBootMappings { get; private set; }

        public static string ApplicationRegistryKeyName { get; private set; }

        public static KeyboardLayoutType KeyboardLayout { get; private set; }

        public static CultureInfo CurrentCultureInfo { get; private set; }

        public static string KeyMapperFilePath
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath
                                        (Environment.SpecialFolder.LocalApplicationData), "KeyMapper");
            }
        }

        static AppController()
        {
            customKeyboardLayouts = new Hashtable();
            ApplicationRegistryKeyName = @"Software\KeyMapper";
        }

        public static void CreateAppDirectory()
        {
            if (Directory.Exists(KeyMapperFilePath) == false)
            {
                try
                {
                    Directory.CreateDirectory(KeyMapperFilePath);
                }
                catch
                {
                    // Can't log it. Going to cause errors (when app tries to read / write to the directory!)
                    // So.. ?
                    MessageBox.Show("Creating the Application Directory failed");
                }
            }
        }

        public static Font GetButtonFont(float size, bool localizable)
        {
            if (Math.Abs(size - 0) < float.Epsilon)
            {
                Console.WriteLine("ERROR: Zero sized font requested");
                size = 10;
            }

            var font = new Font(GetKeyFontName(localizable), size);
            return font;
        }

        public static string GetKeyName(int scancode, int extended)
        {
            // Look up the values in the current localized layout.
            if (scancode == 0 && extended == 0)
            {
                return "Disabled";
            }

            if (scancode == -1 && extended == -1)
            {
                return "";
            }

            int hash = KeyHasher.GetHashFromKeyData(scancode, extended);
            if (_currentLayout.ContainsKey(hash))
            {
                return _currentLayout.GetKeyName(hash);
            }

            Console.WriteLine("Unknown key: sc {0} ex {1}", scancode, extended);
            return "Unknown";
        }

       public static bool IsOverlongKey(int hash)
        {
            return _currentLayout.IsKeyNameOverlong(hash);
        }

        public static bool IsLocalizableKey(int hash)
        {
            return _currentLayout.IsKeyLocalizable(hash);
        }

        public static int GetHighestCommonDenominator(int value1, int value2)
        {
            // Euclidean algorithm
            if (value2 == 0) return value1;
            return GetHighestCommonDenominator(value2, value1 % value2);
        }

        public static bool ConfirmWriteToProtectedSectionOfRegistryOnVistaOrLater(string innerText)
        {
            string text = "In order to write " + innerText + ", Key Mapper needs to add to " +
                          "the protected section of your computer's registry. You may need to approve this action " +
                          "which will be performed by your Registry Editor.";

            TaskDialogResult result = FormsManager.ShowTaskDialog("Do you want to proceed?", text, "Key Mapper",
                                                                  TaskDialogButtons.Yes | TaskDialogButtons.No,
                                                                  TaskDialogIcon.SecurityShield);

            return result == TaskDialogResult.Yes;
        }

        public static void WriteRegistryFileVista(string filePath)
        {
            tempfiles.Add(filePath);

            string command = " /s " + (char)34 + filePath + (char)34;

            try
            {
                var process = new Process
                                  {
                                      StartInfo =
                                          {
                                              FileName = "regedit.exe",
                                              Arguments = command
                                          }
                                  };
                process.Start();
                process.WaitForExit();
            }
            catch (Win32Exception)
            {
                Console.WriteLine("Writing to protected section of registry was cancelled");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error writing to registry: {0}", ex);
            }
        }

        public static void WriteRegistryEntryVista(RegistryHive registryHive, string key, string valueName, string value)
        {
            string filename = Path.GetTempPath() + Path.GetRandomFileName() + ".reg";

            using (var sw = new StreamWriter(filename, false, Encoding.Unicode))
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
                        throw new InvalidOperationException(registryHive + " not supported");
                }

                sw.WriteLine(@"[" + hive + @"\" + key + "]");
                sw.Write("\"" + valueName + "\"=");

                if (string.IsNullOrEmpty(value))
                {
                    sw.Write("-");
                }
                else
                {
                    sw.WriteLine((char)34 + value + (char)34);
                }
            }

            WriteRegistryFileVista(filename);
        }

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
            string path = Path.Combine(KeyMapperFilePath, "customlayouts.txt");

            if (File.Exists(path) == false)
            {
                return;
            }

            string customLayouts;

            using (var sr = new StreamReader(path))
            {
                customLayouts = sr.ReadToEnd();
            }

            // Beware bad data..
            try
            {
                var terminators = new[] { "\r\n" };

                string[] layouts = customLayouts.Split(terminators, StringSplitOptions.RemoveEmptyEntries);
                foreach (string nameValuePair in layouts)
                {
                    if (string.IsNullOrEmpty(nameValuePair))
                        continue;

                    int index = nameValuePair.IndexOf("=", StringComparison.Ordinal);
                    if (index < 0)
                        continue;

                    string locale = nameValuePair.Substring(0, index);
                    int value;
                    if (int.TryParse(nameValuePair.Substring(index + 1), out value) == false)
                        continue;

                    var keyboardType = (KeyboardLayoutType)value;

                    customKeyboardLayouts.Add(locale, keyboardType);
                }
            }
            catch
            {
            }
        }

        public static void AddCustomLayout()
        {
            if (customKeyboardLayouts.Contains(currentLocale))
            {
                customKeyboardLayouts.Remove(currentLocale);
            }

            customKeyboardLayouts.Add(currentLocale, KeyboardLayout);
        }

        private static void SaveCustomLayouts()
        {
            // Always save, but only if layout actually is custom.
            // This means next time we only load layouts which are different.

            // Always overwrite, in case custom layouts have been cleared.

            string path = Path.Combine(KeyMapperFilePath, "customlayouts.txt");

            var kd = new KeyDataXml();

            using (var sw = new StreamWriter(path, false))
            {
                foreach (DictionaryEntry de in customKeyboardLayouts)
                    if ((int)de.Value != (int)kd.GetKeyboardLayoutType(de.Key.ToString()))
                        sw.WriteLine(de.Key + "=" + (int)de.Value);
            }
        }

        public static void Close()
        {
            SaveCustomLayouts();

            KeyboardHelper.UnloadLayout();

            if ((operatingSystemCapability.ImplementsUAC)
                && UserCanWriteBootMappings == false
                && (MappingsManager.VistaMappingsNeedSaving()))
                MappingsManager.SaveBootMappingsVista();

            LogProvider.CloseConsoleOutput();

            foreach (string filepath in tempfiles)
            {
                try
                {
                    File.Delete(filepath);
                }
                catch
                {
                }
            }
        }

        public static string GetKeyFontName(bool localizable)
        {
            if (arialUnicodeMSInstalled == null)
            {
                arialUnicodeMSInstalled = false;
                var installedFonts = new InstalledFontCollection();
                FontFamily[] fonts = installedFonts.Families;
                foreach (FontFamily ff in fonts)
                {
                    if (ff.Name == "Arial Unicode MS")
                    {
                        arialUnicodeMSInstalled = true;
                        _defaultKeyFont = "Arial Unicode MS";
                        break;
                    }
                }
            }


            if (localizable == false || (bool)arialUnicodeMSInstalled)
                return _defaultKeyFont; // Don't want the static keys to change font.

            // Default font for keys is Lucida Sans Unicode as it's on every version of Windows

            // Lucida Sans Unicode simply doesn't contain the characters for Bengali & Malayalam
            // Different versions of Windows have differernt cultures installed 
            // e.g. the two above were installed by XP SP2 ..

            // It's possible the culture has been installed but the font has been 
            // deleted. WIndows will hopefully then do some font substitution.

            switch (CurrentCultureInfo.LCID)
            {
                case 1081: // Devanagari
                    return "Mangal";
                case 1093:
                    // Bengali. Raavi would be a choice for Gurmukhi but it doesn't seem to be in the installed list..
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
                kmregkey = Registry.CurrentUser.OpenSubKey(ApplicationRegistryKeyName, true);
            }
            catch (SecurityException e)
            {
                Console.WriteLine("Can't access KeyMapper registry key: {0}", e);
                UserCannotWriteToApplicationRegistryKey = true;
            }

            bool savedMappingsExist = false;

            if (kmregkey == null && UserCannotWriteToApplicationRegistryKey == false)
            {
                // Key does not exist, or no permissions to write:
                // Create it. Or at least try..
                try
                {
                    kmregkey = Registry.CurrentUser.CreateSubKey(ApplicationRegistryKeyName);
                }
                catch (SecurityException e)
                {
                    Console.WriteLine("Cannot create KeyMapper registry key: {0}", e);
                    UserCannotWriteToApplicationRegistryKey = true;
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

            UserCanWriteBootMappings = registryTimestampService.CanUserWriteToKey
                (RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Keyboard Layout");


            // When was the system booted? (Milliseconds vs Ticks is correct..)
            DateTime boottime = DateTime.Now - TimeSpan.FromMilliseconds(Environment.TickCount);

            // When did the current user log in?

            DateTime logontime = registryTimestampService.GetRegistryKeyTimestamp(RegistryHive.CurrentUser, "Volatile Environment");

            // Now, the "Volatile Environment" key in RegistryHive.CurrentUser
            // >isn't< always unloaded on logoff.

            // Sometimes, as well, logontime returns the wrong time. I think this is because when 
            // the system writes the Volatile Environment subkey, it hasn't yet loaded the correct
            // time zone or isn't respecting Daylight Saving. Sometimes, on some computers..

            if (logontime == DateTime.MinValue)
            {
                // If using Run As to run KeyMapper, then logontime will be 01/01/0001 00:00:00
                // as the Volatile key won't exist.
                logontime = boottime.AddSeconds(1);
            }

            // It can also happen - e.g. when restoring a Virtual Machine - 
            // that the boottime is later than logontime.

            if (boottime > logontime)
            {
                //  Console.WriteLine("Boot time greater than logontime: Boot Time {0} Logon Time {1}", boottime, logontime);
                boottime = logontime.AddSeconds(-1);
            }

            // Just in case the timestamp bug ever works in reverse:
            if (logontime > DateTime.Now)
            {
                Console.WriteLine("Logontime greater than Now: Logon Time {0}, Now: {1}", logontime, DateTime.Now);
                logontime = DateTime.Now;
            }

            // When was HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Keyboard Layout written?
            DateTime HKLMWrite = registryTimestampService.GetRegistryKeyTimestamp
                (RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Keyboard Layout");

            // When was HKEY_CURRENT_USER\Keyboard Layout written?
            DateTime HKCUWrite = registryTimestampService.GetRegistryKeyTimestamp
                (RegistryHive.CurrentUser, @"Keyboard Layout");

            // Console.WriteLine("Booted: {0}, Logged On: {1}, HKLM {2}, HKCU {3}", 
            // boottime, logontime, HKLMWrite, HKCUWrite);

            // Get the current scancode maps
            MappingsManager.GetMappingsFromRegistry();

            // If user mappings are inappropriate (win2k, win 7) default to boot.
            if (operatingSystemCapability.SupportsUserMappings == false)
            {
                MappingsManager.SetFilter(MappingFilter.Boot);
            }

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
            {
                MappingsManager.StoreUnsavedMappings();
            }

            if (operatingSystemCapability.ImplementsUAC)
            {
                MappingsManager.SaveMappings(Mappings.CurrentBootMappings, MapLocation.KeyMapperVistaMappingsCache);
            }
        }

        public static void SetLocale(string locale = null)
        {
            // Only want to reset locale temporarily so save current value
            string currentkeyboardlocale = KeyboardHelper.GetCurrentKeyboardLocale();

            if (string.IsNullOrEmpty(locale))
            {
                // At startup we need to load the current locale.
                locale = currentkeyboardlocale;
            }

            if ((locale != currentLocale))
            {
                if (customKeyboardLayouts != null && customKeyboardLayouts.ContainsKey(locale))
                    KeyboardLayout = (KeyboardLayoutType)customKeyboardLayouts[locale];
                else
                {
                    // Ask the keydata interface what kind of layout this locale has - US, Euro etc.
                    KeyboardLayout = new KeyDataXml().GetKeyboardLayoutType(locale);
                }

                // Load the keyboard layout for the minimum possible time and keep the results:
                // This can error with some cultures, problems with framework, unhandled thread exception occurs.
                try
                {
                    // if (_currentCultureInfo != null)
                    // Console.WriteLine("Setting culture to: LCID: {0}", _currentCultureInfo.LCID);

                    int culture = KeyboardHelper.SetLocale(locale);
                    CurrentCultureInfo = new CultureInfo(culture);

                    _currentLayout = new LocalizedKeySet();
                    currentLocale = locale;
                }

                catch (Exception ex)
                {
                    Console.WriteLine("Set Locale Exception: {0}", ex);
                }

                finally
                {
                    // Set it back (if different)
                    if (currentLocale != currentkeyboardlocale)
                        KeyboardHelper.SetLocale(currentkeyboardlocale);
                }
            }
        }

        public static void SwitchKeyboardLayout(KeyboardLayoutType layout)
        {
            KeyboardLayout = layout;
        }

        public static bool IsOnlyAppInstance()
        {
            appMutex = new AppMutex();
            bool gotMutex = appMutex.GetMutex();
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

        public static void RegisterTempFile(string filePath)
        {
            tempfiles.Add(filePath);
        }
    }
}