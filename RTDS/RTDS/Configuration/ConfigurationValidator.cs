using System.IO;
using RTDS.Configuration.Data;
using RTDS.Configuration.Exceptions;

namespace RTDS.Configuration
{
    internal class ConfigurationValidator
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// If paths are invalid an exception is thrown otherwise returns.
        /// </summary>
        /// <param name="paths"></param>
        public static void ValidatePaths(RTDSPaths paths)
        {
            if (paths == null)
            {
                Logger.Fatal("The RTDSPaths object is null.");
                throw new MissingSpecifiedPathException("The RTDSPaths object is null.");
            }
            if (string.IsNullOrEmpty(paths.BaseSourcePath))
            {
                Logger.Fatal("Base source path is missing.");
                throw new MissingSpecifiedPathException("Base source path is missing.");
            }
            if (string.IsNullOrEmpty(paths.BaseTargetPath))
            {
                Logger.Fatal("Base target path is missing.");
                throw new MissingSpecifiedPathException("Base target path is missing.");
            }
            if (!Directory.Exists(paths.BaseSourcePath))
            {
                Logger.Fatal("The base source path does not exists");
                throw new InvalidPathException(paths.BaseSourcePath);
            }
            if (!Directory.Exists(paths.BaseTargetPath))
            {
                Logger.Fatal("The base target path does not exists");
                throw new InvalidPathException(paths.BaseTargetPath);
            }
        }

        public static bool IsMonitorSettingsValid(RTDSMonitorSettings settings)
        {
            return settings != null && settings.Timer != 0;
        }
    }
}
