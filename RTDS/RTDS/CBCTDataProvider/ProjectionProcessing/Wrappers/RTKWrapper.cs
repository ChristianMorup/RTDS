using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace RTDS.CBCTDataProvider.ProjectionProcessing.Wrappers
{
    public class RTKWrapper : IRTKWrapper
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        [HandleProcessCorruptedStateExceptions] 
        public void PerformReconstruction(string file1, string file2, string file3, string fileOut)
        {
            try
            {
                ontheflyrecon(file1, file2, file3, fileOut);
            }
            catch (Exception e)
            {
                Logger.Fatal(e);
                throw;
            }

        }

        //TODO Rename
        [DllImport("RTK_Wrap.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int ontheflyrecon(string file_in1, string file_in2, string file_in3, string file_out);

    }
}