using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frayte.Services.Business;
using Frayte.Services.Utility;
using XStreamline.Log;

namespace ManifestTrackingBatchProcess
{
    class Program
    {
        static void Main(string[] args)
        {
            string xsUserFolder = @"C:\FMS\" + "FrayteSchedularlog.txt";
            BaseLog.Instance.SetLogFile(xsUserFolder);
            Logger _log = Get_Log();
            var ManifestId = args[0];
            try
            {
               
                _log.Error("This is Vikshit");
                new ManifestTrackingRepository().GetManifestTracking(ManifestId);
            }
            catch(Exception e)
            {
                _log.Error(e.Message);
            }
        }
        public static Logger Get_Log()
        {
            Logger log = BaseLog.Instance.GetLogger(null);
            return log;
        }
    }
}
