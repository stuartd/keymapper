using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace KeyMapper
{
	/// <summary>
	///  Static class providing Keyboard helper methods
	/// </summary>
	static class KeyboardHelper
	{

		#region Fields

		// The don't need to be IntPtrs as they aren't actually system resources 
		static List<int> _systemInputLocaleIdentifiers;
		// It's just easier to have this as in IntPtr
		static IntPtr _currentInputLocaleIdentifier;
		static Hashtable _installedKeyboards;

		#endregion

		#region Properties

		public static Hashtable InstalledKeyboards
		{
			get { return KeyboardHelper._installedKeyboards; }
		}

		private static List<int> SystemInputLocaleIdentifiers
		{
			get
			{
				if (_systemInputLocaleIdentifiers == null)
				{
					int keyboards = NativeMethods.GetKeyboardLayoutList(0, null);
					int[] temp = new int[keyboards];
					NativeMethods.GetKeyboardLayoutList(keyboards, temp);

					_systemInputLocaleIdentifiers = new List<int>(keyboards);
					_systemInputLocaleIdentifiers.AddRange(temp);
				}

				return _systemInputLocaleIdentifiers;
			}
		}

		#endregion


		public static int SetLocale(string locale)
		{
			UnloadLayout();
			_currentInputLocaleIdentifier = (IntPtr)NativeMethods.LoadKeyboardLayout(
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
				if (_currentInputLocaleIdentifier == (IntPtr)i)
				{
					_currentInputLocaleIdentifier = IntPtr.Zero;
					break;
				}
			}

			if (_currentInputLocaleIdentifier != IntPtr.Zero)
			{
				NativeMethods.UnloadKeyboardLayout(_currentInputLocaleIdentifier);
				// Console.WriteLine("Unloading {0}", CurrentLayout);
			}

		}

		/// <summary>
		///  Query the current keyboard layout for unshifted and shifted key values
		/// as these compose the "name" of the key - e.g. 1 and !, [ and {.
		/// </summary>

		public static string GetKeyName(int scancode, ref bool overlong)
		{
			byte[] KeyState = new byte[256];

			// Set all the IME bits on (only works for Japanese = Korean & Chinese still don't work..?)
			KeyState[(int)Keys.KanaMode] = 0x80;
			KeyState[(int)Keys.HanguelMode] = 0x80;
			KeyState[(int)Keys.JunjaMode] = 0x80;
			KeyState[(int)Keys.FinalMode] = 0x80;
			KeyState[(int)Keys.HanjaMode] = 0x80;
			KeyState[(int)Keys.KanjiMode] = 0x80;
	

			// Will put the unshifted and shifter values into this SB:
			StringBuilder result = new StringBuilder();

			int bufferLength = 10;	

			// Get the key itself:
			StringBuilder sbUnshifted = new StringBuilder(bufferLength);

			uint vk = NativeMethods.MapVirtualKeyEx((uint)scancode, 1, _currentInputLocaleIdentifier);
			// 	Console.WriteLine((Keys)vk + " - " + vk.ToString() + " - " + "Scancode: " + scancode.ToString());

			int rc = NativeMethods.ToUnicodeEx(vk, (uint)scancode, KeyState, sbUnshifted, sbUnshifted.Capacity, 0, _currentInputLocaleIdentifier);

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

				int dummy = NativeMethods.ToUnicodeEx(
					(uint)Keys.Space,
					NativeMethods.MapVirtualKeyEx((uint)Keys.Space, 0, _currentInputLocaleIdentifier),
					KeyState,
					sbUnshifted,
					sbUnshifted.Capacity,
					0,
					_currentInputLocaleIdentifier);

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
			KeyState[(int)Keys.ShiftKey] = 0x80;

			StringBuilder sbShifted = new StringBuilder(bufferLength);

			rc = NativeMethods.ToUnicodeEx(
				vk,
				(uint)scancode,
				KeyState,
				sbShifted,
				sbShifted.Capacity,
				0,
				_currentInputLocaleIdentifier);

			// If unshifter was a dead key, so will be shifted.
			if (rc < 0)
			{

				int dummy = NativeMethods.ToUnicodeEx(
					(uint)Keys.Space,
					NativeMethods.MapVirtualKeyEx((uint)Keys.Space, 0, _currentInputLocaleIdentifier),
					KeyState,
					sbUnshifted,
					sbUnshifted.Capacity,
					0,
					_currentInputLocaleIdentifier);

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
			if (rc > 0 & (String.Compare(sbShifted.ToString(), sbUnshifted.ToString(), true, AppController.CurrentCultureInfo) != 0))
			{
				// Not wanting to do this for letters and the like..
				result.Append(" " + sbShifted.ToString());
			}

			return result.ToString();

		}

		public enum ToggleKey
		{
			NumLock = Keys.NumLock,
			CapsLock = Keys.CapsLock,
			ScrollLock = Keys.Scroll
		} ;

		public static bool IsKeySet(ToggleKey keycode)
		{
			int result = NativeMethods.GetKeyState((int)keycode);
			// result is 0: Not pressed and not toggled on.
			// result is 1: Not pressed, but toggled on
			// otherwise: Pressed (and may be toggled on)

			return (result != 0);
		}

		public static void PressKey(ToggleKey keycode)
		{
			// All the ToggleKeys are extended. Press once for down, once for up.
			NativeMethods.keybd_event((byte)keycode, 0x45, 
				(uint)	(NativeMethods.KEYEVENTF_EXTENDEDKEY | 0), UIntPtr.Zero);
			NativeMethods.keybd_event((byte)keycode, 0x45, 
				(uint)(NativeMethods.KEYEVENTF_EXTENDEDKEY | NativeMethods.KEYEVENTF_KEYUP), UIntPtr.Zero);
		}

		//public static void PressKey(int scancode)
		//{
		//    uint vk = NativeMethods.MapVirtualKeyEx((uint)scancode, 3, _currentInputLocaleIdentifier);
		//    NativeMethods.keybd_event((byte)vk, 0x45, (uint)(0), UIntPtr.Zero);
		//    NativeMethods.keybd_event((byte)vk, 0x45, (uint)(KEYEVENTF_KEYUP), UIntPtr.Zero);
		//}

		public static string GetCurrentKeyboardLocale()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder(new string(' ', NativeMethods.KL_NAMELENGTH));
			int result;
			// GetKeyboardLayoutName puts the current locale into the passed buffer
			result = NativeMethods.GetKeyboardLayoutName(buffer);
			if (result == 0)
				return null;
			else
				return buffer.ToString();
		}

		public static string GetKeyboardName()
		{
			string locale = GetCurrentKeyboardLocale();
			if (locale == null)
			{
				return "Keyboard name cannot be determined";
			}
			else
			{
				return GetKeyboardName(locale);
			}
		}

		public static void GetInstalledKeyboardList()
		{
			RegistryKey registry =
				Registry.LocalMachine.OpenSubKey
				(@"SYSTEM\CurrentControlSet\Control\Keyboard Layouts");

			string[] keyboardlocales = registry.GetSubKeyNames();

			Hashtable result = new Hashtable();

			for (int i = 0; i < keyboardlocales.Length; i++)
			{
				// Add the name as the key and the value as the.. value.
				result.Add(GetKeyboardName(keyboardlocales[i]), keyboardlocales[i]);
			}

			_installedKeyboards = result;

		}


		public static string GetKeyboardName(string locale)
		{

			string keyboardname = "Unknown";

			RegistryKey registry =
			Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Keyboard Layouts\" + locale);

			// This is the result for Windows 2000 and earlier and a fallback for later
			// as some keyboards (eg the Apple ones installed by Parallels) don't have the
			// Layout Display Name value.
			keyboardname = registry.GetValue("Layout Text").ToString();

			// 5.1 and upwards (including 6.0) are valid
			if (
				(Environment.OSVersion.Version.Major == 5 & Environment.OSVersion.Version.Minor > 0)
				| Environment.OSVersion.Version.Major > 5)
			{
				// XP or later - can get localised name for keyboard:
				// (if it exists - pass empty string so that's the return if it doesn't)

				string keyboardshellname = registry.GetValue("Layout Display Name", "").ToString();
				string localname = "";

				if (keyboardshellname != null && keyboardshellname.Length > 0)
				{
					StringBuilder sbName = new StringBuilder(260);

					if (NativeMethods.SHLoadIndirectString(
						keyboardshellname,
						sbName,
						(uint)sbName.Capacity,
						IntPtr.Zero)
						== 0)
					{
						localname = sbName.ToString();
					}
				}

				if (localname.Length != 0)
				{
					keyboardname = localname;
				}
			}

			return keyboardname;
		}


	}



}
