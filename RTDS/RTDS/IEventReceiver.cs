namespace RTDS
{
    public interface IEventReceiver
    {
        void OnFolderCreated(string id);
        void OnReconstructedImageFinished(object sender, string path);
    }
}