using RTDS.CTDataProvider.Callbacks;

namespace RTDS
{
    public interface IEventReceiver : ICTScanRetrievedCallback, ICorrectedCTScanRetrievedCallback
    {
        void OnFolderCreated(string id);
        void OnReconstructedImageFinished(object sender, string path);
    }
}