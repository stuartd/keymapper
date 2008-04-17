using System;
using System.Windows.Forms;
using System.Security.Permissions;

[assembly: CLSCompliant(true)]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, Execution = true, UnmanagedCode = true)]

// TODO: Figure out what needs to be done so exception doesn't occur calling registry access 
// [assembly: PermissionSet(SecurityAction.RequestOptional, Name = "Nothing")]

namespace KeyMapper
{
	class main
	{

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{

			// Look for a running copy and activate it if it exists
			if (AppController.CheckForExistingInstances() == true)
				return;

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
			Application.ThreadException += ApplicationThreadException;
			AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;

			AppController.StartAppController();

			Application.Run(new KeyboardForm());

			AppController.CloseAppController();
			Application.ThreadException -= ApplicationThreadException; // Release static event or else..
			AppDomain.CurrentDomain.UnhandledException -= UnhandledExceptionHandler;


		}

		static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
		{
			MessageBox.Show("There's been a murrrrrrrder (1)");
			Exception ex = e.ExceptionObject as Exception;
			if (ex != null)
				System.Diagnostics.EventLog.WriteEntry("KeyMapper", ex.Message, System.Diagnostics.EventLogEntryType.Error);
		}

		static void ApplicationThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			ArgumentException aex = e.Exception as ArgumentException;
			if (aex != null)
			{
				if (aex.ParamName.ToUpperInvariant() == "CULTURE")
				{
					// This is a bug in the .NET framework where some cultures won't load on Windows Server 2003
					// and throw "Culture ID x (0xX) is not a supported culture"

					// MessageBox.Show("Handled");
					return;
				}
			}

			MessageBox.Show("Unhandled exception: " + e.Exception.ToString());
		}
	}
}