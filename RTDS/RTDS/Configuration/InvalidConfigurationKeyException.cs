using System;

namespace RTDS.Configuration
{
    public class InvalidConfigurationKeyException : Exception
    {
        private readonly Exception _e;

        public InvalidConfigurationKeyException()
        {
            Key = "Unknown key";
        }

        public InvalidConfigurationKeyException(string key)
        {
            Key = key;
        }

        public InvalidConfigurationKeyException(string key, Exception e)
        {
            _e = e;
            Key = key;
        }

        public string Key { get; set; }
    }
}