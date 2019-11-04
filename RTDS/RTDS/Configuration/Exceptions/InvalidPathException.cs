using System;

namespace RTDS.Configuration.Exceptions
{
    public class InvalidPathException : Exception
    {
        private readonly Exception _e;
        public string InvalidPath { get; set; }

        public InvalidPathException()
        {
            InvalidPath = "Unknown invalid path";
        }

        public InvalidPathException(string path)
        {
            InvalidPath = path;
        }

        public InvalidPathException(string path, Exception e)
        {
            _e = e;
            InvalidPath = path;
        }
    }
}