using RTDS.DTO;

namespace RTDS.CTDataProvider.Callbacks
{
    internal interface ICTScanRetrievedCallback
    {
        void OnCTScanRetrieved(CTScanInfo ctScanInfo);
    }
}
