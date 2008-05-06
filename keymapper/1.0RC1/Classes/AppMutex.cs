using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace KeyMapper
{
	class AppMutex : IDisposable
	{

		private bool _disposed = false;
		System.Threading.Mutex _appMutex;

		public bool GetMutex()
		{
			bool acquired;
			_appMutex = new System.Threading.Mutex(true, "KeyMapperAppMutex", out acquired);

			if (!acquired)
			{
				SwitchToExistingInstance();
				return false;
			}

			return true;
		}

		private static void SwitchToExistingInstance()
		{
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
		}


		#region Finalizer & IDisposable implementation

		~AppMutex()
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
			if (_disposed == false)
			{
				if (disposing)
				{
					_appMutex.ReleaseMutex();
					_appMutex.Close();
				}

				_disposed = true;
			}
		}

		#endregion

	}
}
