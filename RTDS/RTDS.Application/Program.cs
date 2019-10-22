using System;
using System.IO;
using System.Threading.Tasks;
using RTDS.Configuration;
using RTDS.Configuration.Data;
using RTDS.Monitoring;
using RTDS.Monitoring.Factory;
using RTDS.Monitoring.Monitors;
using RTDS.Monitoring.Wrappers;
using RTDS.Utility;

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