using System;

namespace RTDS.CTDataProvision
{
    public class CTProviderException : Exception
    {
        public override string Message { get; }

        public CTProviderException(string message)
        {
            Message = message;
        }
    }
}