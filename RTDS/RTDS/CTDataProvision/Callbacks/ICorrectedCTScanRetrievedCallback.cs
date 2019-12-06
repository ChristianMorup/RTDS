using RTDS.DTO;

namespace RTDS.CTDataProvision
{
    internal interface ICorrectedCTScanRetrievedCallback
    {
        void OnCorrectedCTScanRetrieved(CTScanInfo info, string id);
    }
}