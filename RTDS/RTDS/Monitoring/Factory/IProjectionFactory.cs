using System.Threading.Tasks;
using RTDS.DTO;

namespace RTDS.Monitoring.Factory
{
    internal interface IProjectionFactory
    {
        ProjectionInfo CreateProjectionInfo(string baseTargetPath, string sourcePath, int index);
    }
}