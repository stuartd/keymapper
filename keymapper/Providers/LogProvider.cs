using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using KeyMapper.Classes;

namespace KeyMapper.Providers
{
    public static class LogProvider
    {
        static LogProvider()
        {
            logFileName = Path.Combine(AppController.KeyMapperFilePath, ConsoleOutputFilename);
        }

        private const string ConsoleOutputFilename = "keymapper.log";

        // Redirect console output
        private static StreamWriter consoleWriterStream;
        private static readonly string logFileName;

        public static void ClearLogFile()
        {
            if (consoleWriterStream != null)
            {
                consoleWriterStream.BaseStream.SetLength(0);
                Console.WriteLine("Log file cleared: {0}", DateTime.Now);
            }
            else
            {
                Console.Write("Can't clear log in debug mode.");
            }
        }

        public static void RedirectConsoleOutput()
        {
            string path = logFileName;
            string existingLogEntries = string.Empty;

            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            if (File.Exists(path))
            {
                // In order to be able to clear the log, the StreamWriter must be opened in create mode.
                // so read the contents of the log first.

                using (var sr = new StreamReader(path))
                {
                    existingLogEntries = sr.ReadToEnd();
                }
            }

            consoleWriterStream = new StreamWriter(path, false, Encoding.UTF8)
            {
                AutoFlush = true
            };
            consoleWriterStream.Write(existingLogEntries);

            // Direct standard output to the log file.
            Console.SetOut(consoleWriterStream);

            Console.WriteLine("Logging started: {0}", DateTime.Now);
        }

        public static void CloseConsoleOutput()
        {
            consoleWriterStream?.Close();
        }

        public static void ViewLogFile()
        {
            string logfile = logFileName;
            if (string.IsNullOrEmpty(logfile))
            {
                return;
            }

            Process.Start(logfile);
        }
    }
}