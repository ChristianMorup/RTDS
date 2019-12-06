using System;
using VMS.TPS.Common.Model.API;


namespace RTDS.Application
{
    class Program
    {
        static void Main(string[] args)
        {  
            RTDSFacade facade = new RTDSFacade();

            facade.StartMonitoring();
            Console.ReadKey();
        }
    }
}