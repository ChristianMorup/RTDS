using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTDS.Configuration.Data
{
   internal class ESAPISettings
    {
        public ESAPISettings(string dcmtkBinPath, string aet, string aec, string aem, string ipPort, string tempStorage)
        {
            DCMTK_BIN_PATH = dcmtkBinPath;
            AET = aet;
            AEC = aec;
            AEM = aem;
            IP_PORT = ipPort;
            TempStorage = tempStorage;
        }

        public string DCMTK_BIN_PATH { get; }
        public string AET { get; }
        public string AEC { get; }
        public string AEM { get; }
        public string IP_PORT { get; }
        public string TempStorage { get; }
    }
}
