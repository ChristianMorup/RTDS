using System;
using VMS.TPS.Common.Model.API;


namespace RTDS.Application
{
    class Program
    {
        static void Main(string[] args)
        {  
            RTDSImpl impl = new RTDSImpl();

            impl.StartMonitoring();
            Console.ReadKey();
        }
    }
}