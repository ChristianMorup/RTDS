using RTDS.DTO;

namespace RTDS.CTDataProvider.Callbacks
{
    public interface ICorrectedCTScanRetrievedCallback
    {
        void OnCorrectedCTScanRetrieved(CTScanInfo info, string id);
    }
}