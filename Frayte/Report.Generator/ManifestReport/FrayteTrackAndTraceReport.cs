using Frayte.Services.Business;
using Frayte.Services.Utility;
using Frayte.Services.Models;
using Report.Generator.ReportTemplate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using System.Net.Http;
using System.IO;
using Frayte.Services.Models.Express;

namespace Report.Generator.ManifestReport
{
    public class FrayteTrackAndTraceReport
    {
        public FrayteManifestName GenerateTrackAndTraceExcel(FrayteTrackDirectBooking trackdetail)
        {
            FrayteManifestName result = new FrayteManifestName();
            string Name = "";
            if (trackdetail.CustomerName == "ALL")
            {
                var OperationZone = UtilityRepository.GetOperationZone();
                if (OperationZone.OperationZoneId == 1)
                {
                    Name = "All_HK";
                }
                else if (OperationZone.OperationZoneId == 2)
                {
                    Name = "All_UK";
                }
            }
            else
            {
                Name = trackdetail.CustomerName;
            }
            try
            {
                var track = new DirectShipmentRepository().GetTrackAndTraceDetail(trackdetail);
                if (track != null && track.Count > 0)
                {
                    ReportTemplate.Other.TrackAndTraceReport rp = new ReportTemplate.Other.TrackAndTraceReport();
                    rp.DataSource = track;
                    if (File.Exists(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "/" + "Track&Trace" + "/" + Name + "_" + "Track_Trace_Detail" + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".xlsx"))
                    {
                        File.Delete(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "/" + "Track&Trace" + "/" + Name + "_" + "Track_Trace_Detail" + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".xlsx");
                        rp.ExportToXlsx(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "/" + "Track&Trace" + "/" + Name + "_" + "Track_Trace_Detail" + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".xlsx");
                        result.FileName = Name + "_" + "Track_Trace_Detail" + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".xlsx";
                        result.FilePath = AppSettings.WebApiPath + "ReportFiles/Track&Trace/" + Name + "_" + "Track_Trace_Detail" + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".xlsx";
                    }
                    else
                    {
                        System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "/" + "Track&Trace" + "/"));
                        rp.ExportToXlsx(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "/" + "Track&Trace" + "/" + Name + "_" + "Track_Trace_Detail" + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".xlsx");
                        result.FileName = Name + "_" + "Track_Trace_Detail" + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".xlsx";
                        result.FilePath = AppSettings.WebApiPath + "ReportFiles/Track&Trace/" + Name + "_" + "Track_Trace_Detail" + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".xlsx";
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }

        public FrayteManifestName GenerateManifestExcel(int ManifestId)
        {
            FrayteManifestName result = new FrayteManifestName();
            try
            {
                var track = new ManifestRepository().GetManifestShipments(ManifestId);

                if (track != null && track.Count > 0)
                {
                    ReportTemplate.Other.TrackAndTraceReport rp = new ReportTemplate.Other.TrackAndTraceReport();
                    rp.DataSource = track;
                    if (File.Exists(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "/" + "Manifest" + "/" + track[0].CustomerCompany + " - " + track[0].Courier + " - " + track[0].ManifestNumber + ".xlsx"))
                    {
                        File.Delete(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "/" + "Manifest" + "/" + track[0].CustomerCompany + " - " + track[0].Courier + " - " + track[0].ManifestNumber + ".xlsx");
                        rp.ExportToXlsx(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "/" + "Manifest" + "/" + track[0].CustomerCompany + " - " + track[0].Courier + " - " + track[0].ManifestNumber + ".xlsx");
                        result.FileName = track[0].CustomerCompany + " - " + track[0].Courier + " - " + track[0].ManifestNumber + ".xlsx";
                        result.FilePath = AppSettings.WebApiPath + "ReportFiles/Manifest/" + track[0].CustomerCompany + " - " + track[0].Courier + " - " + track[0].ManifestNumber + ".xlsx";
                    }
                    else
                    {
                        System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "/" + "Manifest" + "/"));
                        rp.ExportToXlsx(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "/" + "Manifest" + "/" + track[0].CustomerCompany + " - " + track[0].Courier + " - " + track[0].ManifestNumber + ".xlsx");
                        result.FileName = track[0].CustomerCompany + " - " + track[0].Courier + " - " + track[0].ManifestNumber + ".xlsx";
                        result.FilePath = AppSettings.WebApiPath + "ReportFiles/Manifest/" + track[0].CustomerCompany + " - " + track[0].Courier + " - " + track[0].ManifestNumber + ".xlsx";
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }

        #region AvinashCode

        #region ExpressTrackandTrace

        public FrayteManifestName GenerateExpressTrackandTraceDetail(ExpressTrackandTrace trackdetail)
        {
            FrayteManifestName result = new FrayteManifestName();

            string Name = "";
            if (trackdetail.CustomerName == "ALL")
            {
                var OperationZone = UtilityRepository.GetOperationZone();
                if (OperationZone.OperationZoneId == 1)
                {
                    Name = "AllHK";
                }
                else if (OperationZone.OperationZoneId == 2)
                {
                    Name = "AllUK";
                }
            }
            else
            {
                Name = trackdetail.CustomerName;
            }

            try
            {
                var track = new ExpressRepository().GetExpressTrackAndTraceDetail(trackdetail);
                if (track != null && track.Count > 0)
                {
                    ReportTemplate.Other.AllUKReport rp = new ReportTemplate.Other.AllUKReport();
                    rp.DataSource = track;
                    if (File.Exists(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "/" + "ExpressTrack&Trace" + "/" + Name + "_" + "Express_Track_Trace_Detail" + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".xlsx"))
                    {
                        File.Delete(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "/" + "ExpressTrack&Trace" + "/" + Name + "_" + "Express_Track_Trace_Detail" + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".xlsx");
                        rp.ExportToXlsx(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "/" + "ExpressTrack&Trace" + "/" + Name + "_" + "Express_Track_Trace_Detail" + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".xlsx");
                        result.FileName = Name + "_" + "Express_Track_Trace_Detail" + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".xlsx";
                        result.FilePath = AppSettings.WebApiPath + "ReportFiles/ExpressTrack&Trace/" + Name + "_" + "Express_Track_Trace_Detail" + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".xlsx";
                        result.FileStatus = true;
                    }
                    else
                    {
                        System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder + "/" + "ExpressTrack&Trace" + "/"));
                        rp.ExportToXlsx(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "/" + "ExpressTrack&Trace" + "/" + Name + "_" + "Express_Track_Trace_Detail" + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".xlsx");
                        result.FileName = Name + "_" + "Express_Track_Trace_Detail" + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".xlsx";
                        result.FilePath = AppSettings.WebApiPath + "ReportFiles/ExpressTrack&Trace/" + Name + "_" + "Express_Track_Trace_Detail" + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".xlsx";
                        result.FileStatus = true;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }

        #endregion

        #endregion

    }
}
