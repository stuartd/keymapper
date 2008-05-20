using System;
using System.Windows.Forms;
using System.Security.Permissions;

[assembly: CLSCompliant(true)]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, Execution = true, UnmanagedCode = true)]

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
			{
				AppController.CloseConsoleOutput();
				return;
			}

			// Redirect console next. Only one instance of the app can have the log file open

#if DEBUG
#else
			Console.Write("Redirecting console output");
			AppController.RedirectConsoleOutput();
#endif

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
			Application.ThreadException += ApplicationThreadException;
			AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;

			AppController.ValidateUserConfigFile();

			Properties.Settings userSettings = new Properties.Settings();
			if (userSettings.UpgradeRequired)
			{
				userSettings.Upgrade();
				userSettings.UpgradeRequired = false;
				userSettings.Save();
			}

			AppController.StartAppController();
			Application.Run(new KeyboardForm());

			AppController.CloseAppController();
			// Release static events or else leak.
			Application.ThreadException -= ApplicationThreadException;
			AppDomain.CurrentDomain.UnhandledException -= UnhandledExceptionHandler;

		}

		static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
		{
			Exception ex = e.ExceptionObject as Exception;
			if (ex != null)
			{
				Console.WriteLine("Unhandled exception (1): {0}", ex);
			}
		}

		static void ApplicationThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			ArgumentException aex = e.Exception as ArgumentException;
			if (aex != null)
			{
				if (aex.ParamName != null && aex.ParamName.ToUpperInvariant() == "CULTURE")
				{
					// This is a bug in the .NET framework where some cultures won't load on Windows Server 2003
					// and throw "Culture ID x (0xX) is not a supported culture"

					Console.WriteLine("Handled culture info error");
					return;
				}
			}

			Console.WriteLine("Unhandled exception (1): {0}", e.Exception);
		}
	}
}