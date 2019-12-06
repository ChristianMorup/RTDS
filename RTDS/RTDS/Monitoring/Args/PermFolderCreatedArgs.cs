using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTDS.DTO;
using RTDS.Monitoring.Monitors;

namespace RTDS.Monitoring.Args
{
    internal class PermFolderCreatedArgs : EventArgs
    {
        public PermStorageFolderStructure PermStorageFolderStructure { get; }

        public string Id => PermStorageFolderStructure.BaseFolderName;

        public PermFolderCreatedArgs(PermStorageFolderStructure permStorageFolderStructure)
        {
            PermStorageFolderStructure = permStorageFolderStructure;
        }
    }
}
