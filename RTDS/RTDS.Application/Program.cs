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
            //facade.GetCTScan(VMS.TPS.Common.Model.API.Application.CreateApplication(null,null),"aedc");
            Console.ReadKey();
        }
    }
}