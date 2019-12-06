using System;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Text;
using RTDS.Monitoring;
using RTDS.Monitoring.Factory;
using RTDS.Utility;

namespace RTDS
{
    public class RTDSFacade
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public bool StartMonitoring()
        {
            if (TaskWatcher.HasSubscriber())
            {
                var fileController = CreateFileController();
                var baseFolderController = CreateBaseFolderController(fileController);

                baseFolderController.StartFolderMonitor();
                return true;
            }

            Logger.Fatal("No errorhandler is subscribed.");
            return false;
        }

        private IFileController CreateFileController()
        {
            var monitorFactory = new MonitorFactory();
            var folderCreator = new DefaultProjectionFolderCreator(new FolderCreator(new FileUtil()));
            return new FileController(folderCreator, monitorFactory);
        }

        private BaseFolderController CreateBaseFolderController(IFileController fileController)
        {
            return new BaseFolderController(new MonitorFactory(), fileController);
        }

        public void SubscribeErrorHandler(AbstractErrorHandler errorHandler)
        {
            TaskWatcher.AddErrorListener(errorHandler);
        }

        public void UnsubscribeErrorHandler(AbstractErrorHandler errorHandler)
        {
            TaskWatcher.RemoveErrorListener(errorHandler);
        }
    }

    public class Tests
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct args_info_onthefly_recon
        {
            public bool verbose_flag;
            public StringBuilder hardware_arg;
            
        }



        [DllImport("RTK_Wrap.dll", CharSet = CharSet.Unicode)]
        public static extern int ontheflyrecon(IntPtr hWnd, String text, String caption, uint type);


    }
}