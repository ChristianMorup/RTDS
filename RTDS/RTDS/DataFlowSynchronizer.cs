using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using RTDS.Configuration;
using RTDS.CTDataProvision;
using RTDS.DTO;
using RTDS.Monitoring.Args;

namespace RTDS
{
    internal class DataFlowSynchronizer : ICTScanRetrievedCallback, ICorrectedCTScanRetrievedCallback
    {
        private readonly Queue<PermStorageFolderStructure> _createdFolderStructures;
        private readonly Queue<CTScanInfo> _ctScanInfos;

        public DataFlowSynchronizer()
        {
            _createdFolderStructures = new Queue<PermStorageFolderStructure>();
            _ctScanInfos = new Queue<CTScanInfo>();
        }

        public void OnFolderCreated(object sender, PermFolderCreatedArgs args)
        {
            TaskWatcher.AddTask(Task.Run(() =>
            {
                _createdFolderStructures.Enqueue(args.PermStorageFolderStructure);
                SynchronizeDataFlows();
            }));
        }

        public void OnCTScanRetrieved(CTScanInfo dicomObject)
        {
            _ctScanInfos.Enqueue(dicomObject);
        }

        private void SynchronizeDataFlows()
        {
            if (_createdFolderStructures.Count > 0 && _ctScanInfos.Count > 0)
            {
                StoreDicomFilesInPermStorage(_ctScanInfos.Dequeue(), _createdFolderStructures.Dequeue().CtPath);
            }
        }

        private void StoreDicomFilesInPermStorage(CTScanInfo ctInfo, string permStorage)
        {
            foreach (var file in ctInfo.DcmFiles)
            {
                Directory.Move(file, permStorage);
            }
        }

        public void OnCorrectedCTScanRetrieved(CTScanInfo info, string id)
        {
            _ctScanInfos.Clear();

            var baseDirectory = Directory.GetDirectories(ConfigurationManager.GetConfigurationPaths().BaseTargetPath, "*" + id + "")[0];

            var ctDirectory = Directory.GetDirectories(baseDirectory, "*" + "CT" + "*")[0];

            DirectoryInfo directoryInfo = new DirectoryInfo(ctDirectory);

            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                file.Delete();
            }

            StoreDicomFilesInPermStorage(info, ctDirectory);
        }
    }
}