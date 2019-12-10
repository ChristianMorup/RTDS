using System;
using System.Threading;
using System.Threading.Tasks;
using RTDS.ExceptionHandling;
using VMS.TPS.Common.Model.API;


namespace RTDS.Application
{
    class Program
    {
        static void Main(string[] args)
        {  
            RTDSImpl impl = new RTDSImpl();

            impl.SubscribeErrorHandler(new ErrorHandler());
            impl.StartMonitoring();
            

            Console.ReadKey();
        }
    }

    class ErrorHandler : IErrorHandler
    {
        public void OnFatalError(string errorMessage)
        {
            Console.WriteLine(errorMessage);
        }

        public void OnWarning(string warningMessage)
        {
            throw new NotImplementedException();
        }
    }
}