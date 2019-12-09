using System;
using System.IO;
using RTDS.DTO;

namespace RTDS.CBCTDataProvider.Monitoring.Factory
{
    internal class ProjectionInfoInfoFactory : IProjectionInfoFactory
    {
        public PermProjectionInfo CreatePermProjectionInfo(PermStorageFolderStructure folderStructure,
            TempProjectionInfo tempProjectionInfo)
        {
            ValidateInputs(folderStructure.XimPath, tempProjectionInfo.Id);

            var fileName = "proj" + tempProjectionInfo.Id + ".xim";
            var fullTargetPath = Path.Combine(folderStructure.XimPath, fileName);
            return new PermProjectionInfo {FileName = fileName, FilePath = fullTargetPath, Id = tempProjectionInfo.Id};
        }

        public TempProjectionInfo CreateTempProjectionInfo(string path, int id)
        {
            ValidateInputs(path, id);
            return new TempProjectionInfo(id, path);
        }

        private void ValidateInputs(string path, int id)
        {
            if (string.IsNullOrEmpty(path) || id < 0)
            {
                throw new ArgumentException("Input parameters are invalid.");
            }
        }
    }
}