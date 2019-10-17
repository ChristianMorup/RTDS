using System;
using System.Configuration;
using System.IO;

namespace RTDS.Configuration
{
    internal class ConfigurationManager
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private static string GetAppSetting(System.Configuration.Configuration config, string key)
        {
            KeyValueConfigurationElement element = config.AppSettings.Settings[key];
            if (element != null)
            {
                string value = element.Value;
                if (!string.IsNullOrEmpty(value))
                    return value;
            }

            return string.Empty;
        }

        public static string GetConfiguration(string key)
        {
            System.Configuration.Configuration config = null;
            string exeConfigPath = typeof(ConfigurationManager).Assembly.Location;
            try
            {
                config = System.Configuration.ConfigurationManager.OpenExeConfiguration(exeConfigPath);
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex, "Cannot load configuration file at: " + exeConfigPath);
                throw new NoConfigurationFileException(exeConfigPath, ex );
            }

            if (config != null)
            {
                return GetAppSetting(config, key);
            }
            else
            {
                Logger.Fatal("Cannot find the following key in configuration file: " + key);
                throw new InvalidConfigurationKeyException(key);
            }
        }
    }
}