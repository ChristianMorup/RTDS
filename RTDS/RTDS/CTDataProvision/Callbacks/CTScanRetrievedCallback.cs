using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTDS.DTO;

namespace RTDS.CTDataProvision
{
    internal interface ICTScanRetrievedCallback
    {
        void OnCTScanRetrieved(CTScanInfo CTScanInfo);
    }
}
