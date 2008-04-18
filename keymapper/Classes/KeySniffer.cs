using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace KeyMapper
{

	class KeySniffer : IDisposable
	{

		#region Fields, properties, constants

		private const int WH_KEYBOARD_LL = 13;
		private const int WM_KEYDOWN = 0x100;
		private const int WM_SYSKEYDOWN = 0x104;

		internal LowLevelKeyboardProc _proc;

		// Implemented a subclass of CriticalHandleZeroOrMinusOneIsInvalid
		// to make sure handle is released, but it meant giving up too much control
		// of when the hook is deactivated.
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")]
		private IntPtr _hookID;

		private bool _suppress;
		private bool disposed = false;

		public event EventHandler<KeyMapperKeyPressedEventArgs> KeyPressed;
		internal delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

		#endregion

		#region Public methods

		public KeySniffer(bool suppress)
		{
			_suppress = suppress;
		}

		// Default to not suppressing keypresses
		public KeySniffer()
			: this(false)
		{ }

		~KeySniffer()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					if (_hookID != IntPtr.Zero)
					{
						Unhook();
					}
				}
				disposed = true;
			}
		}


		public void ActivateHook()
		{
			if (_hookID != IntPtr.Zero)
			{
				// Already hooked..
				return;
			}

			if (_proc == null)
			{
				_proc = HookCallback;
				GC.KeepAlive(_proc);
			}

			Hook();

		}

		public void DeactivateHook()
		{

			if (_hookID == IntPtr.Zero)
			{
				//  there is no hook..
				return;
			}

			Unhook();
		}

		#endregion

		#region Private methods

		[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
		private void Hook()
		{

			using (Process curProcess = Process.GetCurrentProcess())
			using (ProcessModule curModule = curProcess.MainModule)
			{
				_hookID = NativeMethods.SetWindowsHookEx(WH_KEYBOARD_LL, _proc,
													NativeMethods.GetModuleHandle(curModule.ModuleName), 0);

				if (_hookID == IntPtr.Zero)
				{
					int errorCode = Marshal.GetLastWin32Error();
					throw new System.ComponentModel.Win32Exception(errorCode);
				}
				else
				{
					// Console.WriteLine("Successfully hooked: {0}", _hookID);
				}
			}
		}

		[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
		private void Unhook()
		{

			// Documentation on UnhookWindows Ex states:
			// "If the function succeeds, the return value is nonzero.
			// If the function fails, the return value is zero. To get extended error information, call GetLastError."

			// This generates FXCop warning "Method called GetLastWin32Error but the immediately 
			// preceding call to IntPtr.op_Explicit(IntPtr):Int32 is not a platform invoke statement. Move the call to 
			// GetLastWin32Error so that it immediately follows the relevant platform invoke call."

			// Sure looks to me like that's wrong, probable because the method is in a different class, which
			// fxcop told me to do in the first place. 

			if (_hookID == IntPtr.Zero)
				return;

			int result = (int)NativeMethods.UnhookWindowsHookEx(_hookID);
			int error = Marshal.GetLastWin32Error();

			if (result == 0)
			{
				if (error != 1404) // 1404 is 'Invalid hook handle.'
				{
					// Well, this is bad. An unhooked keyboard hook could paralyse the system.
					// Throwing a hissy fit isn't going to achieve anything though.
					Console.WriteLine("UnhookWindowsEx failed with error code {0}", error);
				}
			}
	
			_hookID = IntPtr.Zero;
			_proc = null;

		}

		private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
		{
			if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
			{
				// Cast lParam into our structure
				KBHookStruct keypress = (KBHookStruct)Marshal.PtrToStructure(lParam, typeof(KBHookStruct));

				// Console.WriteLine("ScanCode: {0}, Extended: {1}, KeyCode: {2}, Name: ",
				//   keypress.Scancode, keypress.Extended, keypress.VirtualKeyCode, AppController.GetKeyName(keypress.Scancode, keypress.Extended));

				if (keypress.Scancode == 541)
				{
					// Right Alt, at least on my Dell SK-8115 keyboard
					// Console.WriteLine("Fixing Dell's keyboard bug");

					keypress.Scancode = 56;
					keypress.KeyFlags = 1;

				}

				if (keypress.VirtualKeyCode == 19)
				{
					// Pause button reports itself with the wrong scancode. 
					// and isn't supported by scancode mechanism anyway..
		
					// Make sure we pass on the correct scan code 
					// so callers can discard this key.
					keypress.Scancode = 69;
					keypress.KeyFlags = 1;

				}

				// Raise the event:
				if (this.KeyPressed != null)
				{
					KeyMapperKeyPressedEventArgs e = new KeyMapperKeyPressedEventArgs(keypress);
					KeyPressed(new object(), e);
				}

				if (_suppress)
				{
					// Return 1 to suppress the keypress.
					return (IntPtr)1;
				}
			}
			return NativeMethods.CallNextHookEx(_hookID, nCode, wParam, lParam);
		}

		#endregion

		#region Internal class for DLLImports

		internal class NativeMethods
		{
            private NativeMethods() { }

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

		}

		#endregion

	}

	#region KeyPress struct

	[StructLayout(LayoutKind.Sequential)]
	public struct KBHookStruct
	{
		private int _vkcode;
		private int _scancode;
		private int _flags;
		private int _time;
		private int _extrainfo;

		private const int LLKHF_EXTENDED = 0x1;

		public int VirtualKeyCode
		{
			get { return _vkcode; }
			set { _vkcode = value; }
		}

		public int Scancode
		{
			get { return _scancode; }
			set { _scancode = value; }
		}

		public int Extended
		{
			get { return (LLKHF_EXTENDED & _flags) == 1 ? 224 : 0; }
		}

		public int KeyFlags
		{
			get { return _flags; }
			set { _flags = value; }
		}

		public static bool operator ==(KBHookStruct key1, KBHookStruct key2)
		{
			// If Scancode and Extended are the same, it's the same key.
			return (key1.Scancode == key2.Scancode && key1.Extended == key2.Extended);
		}

		public override bool Equals(object obj)
		{
			return (obj is KBHookStruct && this == (KBHookStruct)obj);
		}

		// override object.GetHashCode
		public override int GetHashCode()
		{
			return AppController.GetHashFromKeyData(Scancode, Extended);
		}

		// The C# compiler and rule OperatorsShouldHaveSymmetricalOverloads require this.
		public static bool operator !=(KBHookStruct key1, KBHookStruct key2)
		{
			return !(key1 == key2);
		}

	}

	#endregion

	#region Event class

	public class KeyMapperKeyPressedEventArgs : EventArgs
	{
		KBHookStruct _key;

		public KBHookStruct Key
		{
			get { return _key; }
		}

		// Constructor 
		public KeyMapperKeyPressedEventArgs(KBHookStruct key)
		{
			_key = key;
		}

	}

	#endregion

}

