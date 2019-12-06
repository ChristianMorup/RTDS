using System;

namespace RTDS.Application
{
    class Program
    {
        static void Main(string[] args)
        {
            RTDSFacade facade = new RTDSFacade();

            // facade.StartMonitoring();

            //  RTKWrapper.ontheflyrecon("fwe", "few");

            RTKWrapper.ontheflyrecon("C:\\Users\\chrmo\\Downloads\\Projections (1)\\Projections\\proj0001.xim",
                "C:\\Users\\chrmo\\Downloads\\Projections (1)\\Projections\\proj0002.xim",
                "C:\\Users\\chrmo\\Downloads\\Projections (1)\\Projections\\proj0003.xim",
                "C:\\Users\\chrmo\\Desktop\\Projection\\proj0001.mha");

            Console.ReadKey();
        }
    }
}