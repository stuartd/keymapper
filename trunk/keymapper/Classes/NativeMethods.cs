using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace KeyMapper
{
	class NativeMethods
	{
		private NativeMethods() { }

		#region Forms

		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		internal extern static void LockWindowUpdate(IntPtr hWnd);

		#endregion

		#region AppMutex

		public const int SW_RESTORE = 9;

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		[DllImport("user32.dll")]
		internal static extern int IsIconic(IntPtr hWnd);

		#endregion

		#region Keyboard
        
		[DllImport("user32.dll", CharSet = CharSet.Unicode, 
            EntryPoint = "MapVirtualKeyExW", ExactSpelling = true)]
		internal static extern uint MapVirtualKeyEx(
			uint uCode,
			uint uMapType,
			IntPtr dwhkl);

		[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern IntPtr GetKeyboardLayout(int idThread);

		[DllImport("user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
		internal static extern int GetKeyboardLayoutList(int nBuff, [Out, MarshalAs(UnmanagedType.LPArray)] int[] lpList);

		[DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "LoadKeyboardLayoutW", ExactSpelling = true)]
		internal static extern IntPtr LoadKeyboardLayout(string pwszKLID, uint Flags);

		[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool UnloadKeyboardLayout(IntPtr hkl);

		[DllImport("user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, ThrowOnUnmappableChar = true)]
		internal static extern int ToUnicodeEx(
			uint wVirtKey,
			uint wScanCode,
			byte[] lpKeyState,
			StringBuilder pwszBuff,
			int cchBuff,
			uint wFlags,
			IntPtr hkl);

		[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern int GetKeyboardLayoutName([Out] StringBuilder pwszKLID);

		[DllImport("shlwapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern uint SHLoadIndirectString(string pszSource, StringBuilder pszOutBuf, uint cchOutBuf, IntPtr ppvReserved);

		[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

		[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern short GetKeyState(int nKey);

		[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern int GetKeyNameText(IntPtr lParam, [Out] StringBuilder lpString, int nSize);
		
		public const int KEYEVENTF_EXTENDEDKEY = 0x1;
		public const int KEYEVENTF_KEYUP = 0x2;
		public const int KL_NAMELENGTH = 9;
		public const int KLF_ACTIVATE = 0x00000001;
		public const uint KLF_NOTELLSHELL = 0x00000080;
		public const uint KLF_SUBSTITUTE_OK = 0x00000002;

		#endregion

		#region KeySniffer


		// Marshal the delegate otherwise it get's GCd.
		[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern IntPtr SetWindowsHookEx(int idHook, [MarshalAs(UnmanagedType.FunctionPtr)] LowLevelKeyboardProc lpfn, IntPtr hMod, int dwThreadId);

		[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.SysInt)]
		internal static extern IntPtr UnhookWindowsHookEx(IntPtr hhk);

		[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
				IntPtr wParam, IntPtr lParam);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern IntPtr GetModuleHandle(string lpModuleName);

		//[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		//internal static extern uint MapVirtualKeyEx(uint uCode, uint uMapType, IntPtr dwhkl);

		#endregion

		#region Registry

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		public static extern int RegOpenKeyEx
			(UIntPtr hKey,
			string lpSubKey,
			uint ulOptions,
			int samDesired,
			out UIntPtr phkResult);

		[DllImport("advapi32.dll")]
		public extern static uint RegQueryInfoKey
			(UIntPtr hkey,
			IntPtr lpClass,
			IntPtr lpcbClass,
			IntPtr lpReserved,
			IntPtr lpcSubKeys,
			IntPtr lpcbMaxSubKeyLen,
			IntPtr lpcbMaxClassLen,
			IntPtr lpcValues,
			IntPtr lpcbMaxValueNameLen,
			IntPtr lpcbMaxValueLen,
			IntPtr lpcbSecurityDescriptor,
			out Int64 lpftLastWriteTime);

		[DllImport("Advapi32.dll")]
		public static extern uint RegCloseKey(UIntPtr hKey);

#endregion

		#region KeyPictureBox

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DestroyIcon(IntPtr hIcon);

		#endregion


	}
}
