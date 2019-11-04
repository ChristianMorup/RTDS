using System;

namespace RTDS.Configuration.Exceptions
{
    public class MissingSpecifiedPathException : Exception
    {
        private readonly Exception _e;
        public string MissingPath { get; set; }

        public MissingSpecifiedPathException()
        {
            MissingPath = "Unknown path";
        }

        public MissingSpecifiedPathException(string path)
        {
            MissingPath = path;
        }

        public MissingSpecifiedPathException(string path, Exception e)
        {
            _e = e;
            MissingPath = path;
        }
    }
}
