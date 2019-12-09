using RTDS.DTO;

namespace RTDS.CBCTDataProvider.Monitoring.Factory
{
    internal interface IProjectionInfoFactory
    {
        PermProjectionInfo CreatePermProjectionInfo(PermStorageFolderStructure folderStructure, TempProjectionInfo tempProjectionInfo);

        TempProjectionInfo CreateTempProjectionInfo(string path, int id);
    }
}