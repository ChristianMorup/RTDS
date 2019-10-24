﻿using System;
using System.IO;
using System.Xml.Serialization;
using RTDS.Configuration.Data;
using RTDS.Configuration.Exceptions;

namespace RTDS.Configuration
{
    internal class ConfigurationManager
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private const int DefaultTimerValue = 30000;
        private static RTDSConfiguration _configuration = null;

        /// <summary>
        /// Returns the current configuration for RTDS. 
        /// Preferable this is not used since no validation is made.
        /// Use instead specific methods for the configuration details, that is needed. 
        /// </summary>
        /// <returns>RTDSConfiguration</returns>
        public static RTDSConfiguration GetConfiguration()
        {
            if (_configuration == null)
            {
                _configuration = GetConfigurationFromFile();

                if (_configuration == null)
                {
                    _configuration = CreateDefaultConfiguration();
                    OverrideConfiguration(_configuration, true);
                }
            }

            return _configuration;
        }

        private static RTDSConfiguration CreateDefaultConfiguration()
        {
            return new RTDSConfiguration
            {
                Paths = new RTDSPaths
                {
                    BaseTargetPath = "",
                    BaseSourcePath = ""
                },
                MonitorSettings = new RTDSMonitorSettings
                {
                    Timer = DefaultTimerValue
                }
            };
        }

        /// <summary>
        /// Returns the current configuration for paths needed in RTDS. 
        /// If no path is specified in the configuration file, then exception is thrown. 
        /// </summary>
        /// <returns>RTDSPaths</returns>
        public static RTDSPaths GetConfigurationPaths()
        {
            if (_configuration == null)
            {
                _configuration = GetConfiguration();
            }

            //Throws exception if paths are invalid:
            ConfigurationValidator.ValidatePaths(_configuration.Paths);

            return _configuration.Paths;
        }

        public static RTDSMonitorSettings GetMonitorSettings()
        {
            if (_configuration == null || !ConfigurationValidator.IsMonitorSettingsValid(_configuration.MonitorSettings))
            {
                _configuration = GetConfiguration();

                if (!ConfigurationValidator.IsMonitorSettingsValid(_configuration.MonitorSettings))
                {

                    _configuration.MonitorSettings = new RTDSMonitorSettings()
                    {
                        Timer = DefaultTimerValue
                    };

                    Logger.Info("Invalid timer value in configuration has been overwritten");
                    OverrideConfiguration(_configuration, true);

                }
            }

            return _configuration.MonitorSettings;
        }

        public static void OverrideConfiguration(RTDSConfiguration configuration, bool overrideConfigFile)
        {
            if (overrideConfigFile)
            {
                string exeConfigPath = typeof(ConfigurationManager).Assembly.Location + ".config";

                XmlSerializer serializer = new XmlSerializer(typeof(RTDSConfiguration));

                using (TextWriter writer = new StreamWriter(exeConfigPath))
                {
                    serializer.Serialize(writer, configuration);
                }
            }

            _configuration = configuration;
        }


        private static RTDSConfiguration GetConfigurationFromFile()
        {
            RTDSConfiguration configuration = null;
            string exeConfigPath = typeof(ConfigurationManager).Assembly.Location + ".config";

            if (File.Exists(exeConfigPath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(RTDSConfiguration));
                using (TextReader reader = new StreamReader(exeConfigPath))
                {
                    configuration = (RTDSConfiguration)serializer.Deserialize(reader);
                }

                if (configuration == null)
                {
                    Logger.Fatal("Could not parse configuration information");
                    throw new InvalidConfigurationFileException(exeConfigPath);
                }
            }

            return configuration;
        }
    }
}