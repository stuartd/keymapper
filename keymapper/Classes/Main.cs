using System;
using System.Windows.Forms;
using KeyMapper.Forms;
using KeyMapper.Providers;

[assembly: CLSCompliant(true)]
namespace KeyMapper.Classes
{
    internal class main
    {
        [STAThread]
        private static void Main()
        {
            if (AppController.IsOnlyAppInstance() == false)
            {
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += ApplicationThreadException;
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;

            AppController.CreateAppDirectory();

#if DEBUG
#else
			Console.Write("Redirecting console output");
			LogProvider.RedirectConsoleOutput();
#endif

            ConfigFileProvider.ValidateUserConfigFile();

            var userSettings = new Properties.Settings();
            if (userSettings.UpgradeRequired)
            {
                Console.WriteLine("Upgrading settings to new version");
                userSettings.Upgrade();
                userSettings.UpgradeRequired = false;
                userSettings.Save();
            }

            AppController.Start();

            Application.Run(new KeyboardForm());

            AppController.Close();
            // Release static events or else leak.
            Application.ThreadException -= ApplicationThreadException;
            AppDomain.CurrentDomain.UnhandledException -= UnhandledExceptionHandler;
        }

        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                Console.WriteLine("Unhandled exception (1): {0}", ex);
            }
        }

        private static void ApplicationThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            if (e.Exception is ArgumentException aex)
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
