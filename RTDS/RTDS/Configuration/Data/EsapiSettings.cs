using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTDS.Configuration.Data
{
   public class ESAPISettings
    {
        public ESAPISettings()
        {

        }
        public ESAPISettings(string dcmtkBinPath, string aet, string aec, string aem, string ipPort, string tempStorage)
        {
            DcmtkBinPath = dcmtkBinPath;
            AET = aet;
            AEC = aec;
            AEM = aem;
            IpPort = ipPort;
            TempStorage = tempStorage;
        }

        public string DcmtkBinPath { get; set; }
        public string AET { get; set; }
        public string AEC { get; set; }
        public string AEM { get; set; }
        public string IpPort { get; set; }
        public string TempStorage { get; set; }
    }
}
