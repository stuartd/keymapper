using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using KeyMapper.Classes.Interop;

namespace KeyMapper.Classes
{
	
	public delegate IntPtr LowLevelKeyboardProc(int code, IntPtr wParam, IntPtr lParam);

	internal sealed class KeySniffer : IDisposable
	{
		private const int WH_KEYBOARD_LL = 13;
		private const int WM_KEYDOWN = 0x100;
		private const int WM_SYSKEYDOWN = 0x104;

		private LowLevelKeyboardProc proc;

		// Implemented a subclass of CriticalHandleZeroOrMinusOneIsInvalid
		// to make sure handle is released, but it meant giving up too much control
		// of when the hook is deactivated.
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")]
		private IntPtr hookId;

		private readonly bool suppress;
		private bool disposed = false;

		public event EventHandler<KeyMapperKeyPressedEventArgs> KeyPressed;

		public KeySniffer(bool suppress)
		{
			this.suppress = suppress;
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

		private void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					if (hookId != IntPtr.Zero)
					{
						Unhook();
					}
				}
				disposed = true;
			}
		}


		public void ActivateHook()
		{
			if (hookId != IntPtr.Zero)
			{
				// Already hooked..
				return;
			}

			if (proc == null)
			{
				proc = HookCallback;
				GC.KeepAlive(proc);
			}

			Hook();

		}

		public void DeactivateHook()
		{

			if (hookId == IntPtr.Zero)
			{
				//  there is no hook..
				return;
			}

			Unhook();
		}

		[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
		private void Hook()
		{

			using (var curProcess = Process.GetCurrentProcess())
			using (var curModule = curProcess.MainModule)
			{
				hookId = NativeMethods.SetWindowsHookEx(WH_KEYBOARD_LL, proc,
													NativeMethods.GetModuleHandle(curModule.ModuleName), 0);

				if (hookId == IntPtr.Zero)
				{
					int errorCode = Marshal.GetLastWin32Error();
					throw new System.ComponentModel.Win32Exception(errorCode);
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

			if (hookId == IntPtr.Zero) {
				return;
			}

			int result = (int)NativeMethods.UnhookWindowsHookEx(hookId);
			int error = Marshal.GetLastWin32Error();

			if (result == 0)
			{
				if (error != 1404) // 1404 is 'Invalid hook handle.'
				{
					// Well, this is bad. A key-suppressing keyboard hook that fails to unhook could paralyse the system.
					// Throwing a hissy fit isn't going to achieve anything though.
					Console.WriteLine("UnhookWindowsEx failed with error code {0}", error);
				}
			}
	
			hookId = IntPtr.Zero;
			proc = null;

		}

		private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
		{
			if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
			{
				// Cast lParam into our structure
				var keypress = (KbHookStruct)Marshal.PtrToStructure(lParam, typeof(KbHookStruct));
                
				//  Console.WriteLine("ScanCode: {0}, Extended: {1}, KeyCode: {2}, Name: {3}",
				 //  keypress.Scancode, keypress.Extended, keypress.VirtualKeyCode, AppController.GetKeyName(keypress.Scancode, keypress.Extended));

				if (keypress.Scancode == 541)
				{
					// Right Alt, at least on my Dell SK-8115 keyboard
					// Console.WriteLine("Fixing Dell's Right Alt keyboard bug");

					keypress.Scancode = 56;
					keypress.KeyFlags = 1;

				}

				if (keypress.VirtualKeyCode == 19)
				{
					// Pause. This doesn't capture well - it's extended value is 225
					// rather than 224, so 
	
					keypress.Scancode = 29;
					keypress.KeyFlags = 2;

				}

                // Some keyboards report Num Lock as having the extended bit set
                // on keypress, but that doesn't work in a mapping.
                if (keypress.Scancode == 69 && keypress.Extended == 224)
                {
                    // The Keyboard lies.
                    keypress.Extended = 0;
                }

				// Raise the event:
				if (KeyPressed != null)
				{
					var e = new KeyMapperKeyPressedEventArgs(keypress);
					KeyPressed(new object(), e);
				}

				if (suppress)
				{
					// Return 1 to suppress the keypress.
					return (IntPtr)1;
				}
			}
			return NativeMethods.CallNextHookEx(hookId, nCode, wParam, lParam);
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct KbHookStruct
	{
		private readonly int _time;
		private readonly int _extrainfo;

		private const int LLKHF_EXTENDED = 0x1;
		private const int LLKHF_EXTENDED_PAUSE = 0x2;

		public int VirtualKeyCode { get; set; }

		public int Scancode { get; set; }

		public int Extended
		{
			get
			{

				if ((LLKHF_EXTENDED & KeyFlags) == LLKHF_EXTENDED)
				{
					return 224;
				}
				else if ((LLKHF_EXTENDED_PAUSE & KeyFlags) == LLKHF_EXTENDED_PAUSE)
				{
					return 225;
				}
				else
				{
					return 0;
				}
			}
            set => KeyFlags = value == 224 ? LLKHF_EXTENDED : 0;
		}

		// They *are* flags.
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags")]
		public int KeyFlags { get; set; }

		public static bool operator ==(KbHookStruct key1, KbHookStruct key2)
		{
			// If Scancode and Extended are the same, it's the same key.
			return key1.Scancode == key2.Scancode && key1.Extended == key2.Extended;
		}

		public override bool Equals(object obj)
		{
			return obj is KbHookStruct && this == (KbHookStruct)obj;
		}

		// override object.GetHashCode
		public override int GetHashCode()
		{
			return KeyHasher.GetHashFromKeyData(Scancode, Extended);
		}

		// The C# compiler and rule OperatorsShouldHaveSymmetricalOverloads require this.
		public static bool operator !=(KbHookStruct key1, KbHookStruct key2)
		{
			return !(key1 == key2);
		}

	}

	public class KeyMapperKeyPressedEventArgs : EventArgs
	{
		public KbHookStruct Key { get; }

		// Constructor 
		public KeyMapperKeyPressedEventArgs(KbHookStruct key)
		{
			Key = key;
		}

	}
}

