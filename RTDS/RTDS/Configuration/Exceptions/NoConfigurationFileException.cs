using System;

namespace RTDS.Configuration.Exceptions
{
    public class NoConfigurationFileException : Exception
    {
        private readonly Exception _e;

        public NoConfigurationFileException()
        {
            Path = "Unknown path";
        }

        public NoConfigurationFileException(string path)
        {
            Path = path;
        }

        public NoConfigurationFileException(string path, Exception e)
        {
            _e = e;
            Path = path;
        }

        public string Path { get; set; }
    }
}