using System;

namespace RTDS.CTDataProvider.Exceptions
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