using System;

namespace RTDS.Configuration.Exceptions
{
    public class InvalidConfigurationFileException : Exception
    {
        private readonly Exception _e;

        public InvalidConfigurationFileException()
        {
            Path = "Unknown path";
        }

        public InvalidConfigurationFileException(string path)
        {
            Path = path;
        }

        public InvalidConfigurationFileException(string path, Exception e)
        {
            _e = e;
            Path = path;
        }

        public string Path { get; set; }
    }
}