using RTDS.DTO;

namespace RTDS.CTDataProvider.Callbacks
{
    public interface ICTScanRetrievedCallback
    {
        void OnCTScanRetrieved(CTScanInfo ctScanInfo);
    }
}
