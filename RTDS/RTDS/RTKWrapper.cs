
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RTDS
{
    public class RTKWrapper
    {
        public static void convert(string fileIn, string fileIn2, string fileIn3, string fileOut)
        {
            ontheflyrecon(fileIn, fileIn2, fileIn3, fileOut);
        }


        [DllImport("RTK_Wrap.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ontheflyrecon(string file_in1, string file_in2, string file_in3, string file_out);

    }
}
