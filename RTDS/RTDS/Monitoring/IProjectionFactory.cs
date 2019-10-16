namespace RTDS.Monitoring
{
    internal interface IProjectionFactory
    {
        ProjectionInfo CreateProjectionInfo(string permanentStoragePath);
    }
}