using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using KeyMapper.Providers;
using Microsoft.Win32;
using System.IO;
using KeyMapper.Classes.Interop;

namespace KeyMapper.Classes
{
	/// <summary>
    ///  Static class providing Keyboard helper methods
    /// </summary>
	internal static class KeyboardHelper
    {
        // The don't need to be IntPtrs as they aren't actually system resources 
		private static List<int> systemInputLocaleIdentifiers;

        // It's just easier to have this as in IntPtr
		private static IntPtr currentInputLocaleIdentifier;

        public static Hashtable InstalledKeyboards { get; }

        private static readonly IOperatingSystemCapability operatingSystemCapability = new OperatingSystemCapabilityProvider();

        static KeyboardHelper()
        {
            InstalledKeyboards = new Hashtable();
        }

        private static IEnumerable<int> SystemInputLocaleIdentifiers
        {
            get
            {
                if (systemInputLocaleIdentifiers == null)
                {
                    int keyboards = NativeMethods.GetKeyboardLayoutList(0, null);
                    var temp = new int[keyboards];
                    NativeMethods.GetKeyboardLayoutList(keyboards, temp);

                    systemInputLocaleIdentifiers = new List<int>(keyboards);
                    systemInputLocaleIdentifiers.AddRange(temp);
                }

                return systemInputLocaleIdentifiers;
            }
        }
        
        public static int SetLocale(string locale)
        {
            UnloadLayout();
            currentInputLocaleIdentifier = NativeMethods.LoadKeyboardLayout(
                locale, NativeMethods.KLF_ACTIVATE | NativeMethods.KLF_SUBSTITUTE_OK);

            // While we have it, get it's HKL and return the low word of it:
            // (this allows the appropriate culture to be loaded)
            int hkl = (int)NativeMethods.GetKeyboardLayout(0);
            return hkl & 0x0000FFFF;
        }

        public static void UnloadLayout()
        {
            // If the current layout isn't in the list of system layouts, unload it.
            foreach (int i in SystemInputLocaleIdentifiers)
            {
                if (currentInputLocaleIdentifier == (IntPtr)i)
                {
                    currentInputLocaleIdentifier = IntPtr.Zero;
                    break;
                }
            }

            if (currentInputLocaleIdentifier != IntPtr.Zero)
            {
                NativeMethods.UnloadKeyboardLayout(currentInputLocaleIdentifier);
                // Console.WriteLine("Unloading {0}", CurrentLayout);
            }
        }

        public static string GetKeyName(int scancode, ref bool overlong)
        {
            var keyState = new byte[256];

            // Set all the IME bits on (only works for Japanese = Korean & Chinese still don't work..?)
            keyState[(int)Keys.KanaMode] = 0x80;
            keyState[(int)Keys.HanguelMode] = 0x80;
            keyState[(int)Keys.JunjaMode] = 0x80;
            keyState[(int)Keys.FinalMode] = 0x80;
            keyState[(int)Keys.HanjaMode] = 0x80;
            keyState[(int)Keys.KanjiMode] = 0x80;

            var result = new StringBuilder();

            const int bufferLength = 10;

            // Get the key itself:
            var sbUnshifted = new StringBuilder(bufferLength);

            uint vk = NativeMethods.MapVirtualKeyEx((uint)scancode, 1, currentInputLocaleIdentifier);
            // 	Console.WriteLine((Keys)vk + " - " + vk.ToString() + " - " + "Scancode: " + scancode.ToString());

            int rc = NativeMethods.ToUnicodeEx(vk, (uint)scancode, keyState, sbUnshifted, sbUnshifted.Capacity, 0, currentInputLocaleIdentifier);

            if (rc > 1)
            {
                // this is an out parameter: many unicode glyphs are more than one "character"
                overlong = true;
            }

            if (rc < 0)
            {
                // This is a dead key - a key which only combines with the next pressed to form an accent etc.
                // In order to stop it combining with the shifted state, we need to flush out what's stored in the keyboard state
                // by calling the function again now.
                // ref: http://blogs.msdn.com/michkap/archive/2006/03/24/559169.aspx

                NativeMethods.ToUnicodeEx(
                    (uint)Keys.Space,
                    NativeMethods.MapVirtualKeyEx((uint)Keys.Space, 0, currentInputLocaleIdentifier),
                    keyState,
                    sbUnshifted,
                    sbUnshifted.Capacity,
                    0,
                    currentInputLocaleIdentifier);

                // There is one character stored in our buffer though:
                rc = 1;
            }

            if (rc < sbUnshifted.Length)
            {
                sbUnshifted.Remove(rc, sbUnshifted.Length - rc);
            }

            if (rc > 0)
            {
                result.Append(sbUnshifted.ToString().ToUpper(AppController.CurrentCultureInfo));
            }

            // Set SHIFT on..
            keyState[(int)Keys.ShiftKey] = 0x80;

            var sbShifted = new StringBuilder(bufferLength);

            rc = NativeMethods.ToUnicodeEx(
                vk,
                (uint)scancode,
                keyState,
                sbShifted,
                sbShifted.Capacity,
                0,
                currentInputLocaleIdentifier);

            // If unshifter was a dead key, so will be shifted.
            if (rc < 0)
            {
                int dummy = NativeMethods.ToUnicodeEx(
                    (uint)Keys.Space,
                    NativeMethods.MapVirtualKeyEx((uint)Keys.Space, 0, currentInputLocaleIdentifier),
                    keyState,
                    sbUnshifted,
                    sbUnshifted.Capacity,
                    0,
                    currentInputLocaleIdentifier);

                // There will be one character stored in our buffer though:
                // (well, at least one, but we have no way of knowing if more)
                rc = 1;
            }

            if (rc > 1)
            {
                overlong = true;
            }

            if (rc < sbShifted.Length)
            {
                sbShifted.Remove(rc, sbShifted.Length - rc);
            }

            // If this shifted state the same as the unshifted.ToUpper
            // (e.g. e and E) then don't add it.
            if ((rc > 0) & (string.Compare(sbShifted.ToString(), sbUnshifted.ToString(), true, AppController.CurrentCultureInfo) != 0))
            {
                // Not wanting to do this for letters and the like..
                result.Append(" " + sbShifted);
            }

            return result.ToString();

        }

        public enum ToggleKey
        {
            NumLock = Keys.NumLock,
            CapsLock = Keys.CapsLock,
            ScrollLock = Keys.Scroll
        } ;


        public static void PressKey(ToggleKey keycode)
        {
            // All the ToggleKeys are extended. Press once for down, once for up.
            NativeMethods.keybd_event((byte)keycode, 0x45,
                NativeMethods.KEYEVENTF_EXTENDEDKEY | 0, UIntPtr.Zero);
            NativeMethods.keybd_event((byte)keycode, 0x45,
                NativeMethods.KEYEVENTF_EXTENDEDKEY | NativeMethods.KEYEVENTF_KEYUP, UIntPtr.Zero);
        }

        //public static void PressKey(int scancode)
        //{
        //    uint vk = NativeMethods.MapVirtualKeyEx((uint)scancode, 3, _currentInputLocaleIdentifier);
        //    NativeMethods.keybd_event((byte)vk, 0x45, (uint)(0), UIntPtr.Zero);
        //    NativeMethods.keybd_event((byte)vk, 0x45, (uint)(KEYEVENTF_KEYUP), UIntPtr.Zero);
        //}

        public static string GetCurrentKeyboardLocale()
        {
            var buffer = new StringBuilder(new string(' ', NativeMethods.KL_NAMELENGTH));
            // GetKeyboardLayoutName puts the current locale into the passed buffer
            int result = NativeMethods.GetKeyboardLayoutName(buffer);
            if (result == 0) {
				return null;
			}

			return buffer.ToString();
        }

        public static string GetKeyboardName()
        {
            string locale = GetCurrentKeyboardLocale();
            return locale == null 
                ? "Keyboard name cannot be determined" 
                : GetKeyboardName(locale);
        }

        public static void GetInstalledKeyboardList()
        {
            InstalledKeyboards.Clear();

            var registry =
                Registry.LocalMachine.OpenSubKey
                (@"SYSTEM\CurrentControlSet\Control\Keyboard Layouts");

            if (registry == null)
            {
                return;
            }

            foreach (string name in registry.GetSubKeyNames())
            {
                // Add the name as the key and the value as the.. value.
                InstalledKeyboards.Add(GetKeyboardName(name), name);
            }
        }

        private static IEnumerable<string> GetInstalledKeyboardListInNameOrder()
        {
            if (InstalledKeyboards == null)
            {
                GetInstalledKeyboardList();
            }

            var results = new string[InstalledKeyboards.Count];
            InstalledKeyboards.Keys.CopyTo(results, 0);

            Array.Sort(results);
            return results;
        }

        public static void ShowKeyboardList()
        {
            var kblist = GetInstalledKeyboardListInNameOrder();
            var keyboards = new StringBuilder();
            foreach (string keyboard in kblist)
            {
                keyboards.Append(keyboard + (char) 13 + (char) 10);
            }

            string keyboardListFile = Path.Combine(Path.GetTempPath(), "installed keyboards.txt");
            AppController.RegisterTempFile(keyboardListFile); // Reluctantly register for deletion

            using (var fs = new FileStream(keyboardListFile, FileMode.Create))
            using (var sw = new StreamWriter(fs))
            {
                sw.Write(keyboards.ToString());
                sw.Flush();
            }
            System.Diagnostics.Process.Start(keyboardListFile);
        }


		private static string GetKeyboardName(string locale)
        {

            string keyboardname = "Unknown";

            var key =
                Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Keyboard Layouts\" + locale);

            // This is the result for Windows 2000 and earlier and a fallback for later
            // as some keyboards (eg the Apple ones installed by Parallels) don't have the
            // Layout Display Name value.

            // It's possible the value doesn't exist locally (when using remote desktop, for example)
            // In this case, won't be able to revert to this keyboard after the layout is changed.

            if (key == null)
            {
                return keyboardname;
            }

            keyboardname = key.GetValue("Layout Text").ToString();

            if (operatingSystemCapability.SupportsLocalizedKeyboardNames) 
            {
                // XP or later - can get localised name for keyboard:
                // (if it exists - pass empty string so that's the return if it doesn't)

                string keyboardShellName = key.GetValue("Layout Display Name", "").ToString();
                string localName = string.Empty;

                if (string.IsNullOrEmpty(keyboardShellName) == false)
                {
                    var sbName = new StringBuilder(260);

                    if (NativeMethods.SHLoadIndirectString
                        (keyboardShellName, sbName, (uint)sbName.Capacity, IntPtr.Zero) == 0)
                    {
                        localName = sbName.ToString();
                    }
                }

                if (string.IsNullOrEmpty(localName) == false)
                {
                    keyboardname = localName;
                }
            }

            return keyboardname;
        }
    }
}