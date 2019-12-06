using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTDS.Configuration.Data;

namespace RTDS.Configuration.Exceptions
{
    public class InvalidESAPISettingsException : Exception
    {
        private readonly Exception _e;
        public override string Message { get; }

        public InvalidESAPISettingsException()
        {
            
        }
        public InvalidESAPISettingsException(string message)
        {
            Message = message;
        }

        public InvalidESAPISettingsException(Exception e)
        {
            _e = e;
        }
    }
}
