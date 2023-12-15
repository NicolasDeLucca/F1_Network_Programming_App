using System;
using System.Configuration;

namespace Common.Managers
{
    public class SettingManager
    {
        public string ReadSettings(string key)
        {
            string value = string.Empty;
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                if (appSettings != null)
                    value = appSettings[key];
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading configuration");
            }
            return value;
        }
    }
}
