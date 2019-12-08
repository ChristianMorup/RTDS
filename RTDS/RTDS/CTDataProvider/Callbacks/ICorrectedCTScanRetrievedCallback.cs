using RTDS.DTO;

namespace RTDS.CTDataProvider.Callbacks
{
    internal interface ICorrectedCTScanRetrievedCallback
    {
        void OnCorrectedCTScanRetrieved(CTScanInfo info, string id);
    }
}