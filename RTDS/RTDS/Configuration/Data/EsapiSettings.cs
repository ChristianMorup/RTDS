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
            DcmtkBinPath = dcmtkBinPath;
            AET = aet;
            AEC = aec;
            AEM = aem;
            IpPort = ipPort;
            TempStorage = tempStorage;
        }

        public string DcmtkBinPath { get; }
        public string AET { get; }
        public string AEC { get; }
        public string AEM { get; }
        public string IpPort { get; }
        public string TempStorage { get; }
    }
}
