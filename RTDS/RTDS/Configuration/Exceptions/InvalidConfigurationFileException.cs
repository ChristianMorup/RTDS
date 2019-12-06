using System;

namespace RTDS.Configuration.Exceptions
{
    public class InvalidConfigurationFileException : Exception
    {
        private readonly Exception _e;

        public InvalidConfigurationFileException(string path)
        {
            Path = path;
        }

        public string Path { get; set; }
    }
}