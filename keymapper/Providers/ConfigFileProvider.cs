using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using KeyMapper.Properties;

namespace KeyMapper.Providers
{
    public static class ConfigFileProvider
    {
        private static void DeleteInvalidUserFileFromException(ConfigurationException ex)
        {
            Console.WriteLine("User Config file is invalid - resetting to default");

            string fileName = "";

            if (!string.IsNullOrEmpty(ex.Filename))
            {
                fileName = ex.Filename;
            }
            else
            {
                if (ex.InnerException is ConfigurationErrorsException innerException && !string.IsNullOrEmpty(innerException.Filename))
                {
                    fileName = innerException.Filename;
                }
            }
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }

        public static void ValidateUserConfigFile()
        {
            // Even with these checks, occasionally get a "failed to load configuration system" 
            // exception.
            try
            {
                // If file is corrupt this will trigger an exception
                var config = ConfigurationManager.OpenExeConfiguration
                    (ConfigurationUserLevel.PerUserRoamingAndLocal);
            }
            catch (ConfigurationErrorsException ex)
            {
                DeleteInvalidUserFileFromException(ex);
                return;
            }
            try
            {
                // Access a property to find any other error types - invalid XML etc.
                var userSettings = new Settings();
                var p = userSettings.ColourEditorLocation;
            }
            catch (ConfigurationErrorsException ex)
            {
                DeleteInvalidUserFileFromException(ex);
            }
        }

    }
}
