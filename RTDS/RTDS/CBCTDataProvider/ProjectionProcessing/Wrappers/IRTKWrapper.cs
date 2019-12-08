namespace RTDS.CBCTDataProvider.ProjectionProcessing.Wrappers
{
    internal interface IRTKWrapper
    {
        void PerformReconstruction(string file1, string file2, string file3, string fileOut);
    }
}