using Frayte.Services.Business;
using Frayte.Services.Utility;
using Frayte.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using System.Net.Http;
using System.IO;

namespace Report.Generator.ManifestReport
{
    public class FrayteManifestReport
    {
        public FrayteManifestName GenerateManifestReport(int ManifestId, string moduleType, int UserId, int RoleId)
        {
            FrayteManifestName result = new FrayteManifestName();
            try
            {
                var manifestlist = new ManifestRepository().DownLoadManifest(ManifestId, moduleType, UserId, RoleId);
                if (manifestlist != null && manifestlist.Count > 0 && moduleType == FrayteShipmentServiceType.DirectBooking)
                {
                    if (manifestlist[0].Collection[0].LogisticCompany + " " + manifestlist[0].Collection[0].LogisticType == "DHL Export" || manifestlist[0].Collection[0].LogisticCompany + " " + manifestlist[0].Collection[0].LogisticType == "DHL UKShipment")
                    {
                        ReportTemplate.Other.ManifestDHLReport rp = new ReportTemplate.Other.ManifestDHLReport();
                        if (RoleId == (int)FrayteUserRole.UserCustomer)
                        {
                            rp.Parameters["lblNote"].Value = "MEX Logistics Ltd is not responsible for any parcel lost with expired labels.";
                        }
                        else
                        {
                            rp.Parameters["lblNote"].Value = "FRAYTE GLOBAL is not responsible for any parcel lost with expired labels.";
                        }
                        rp.DataSource = manifestlist;

                        if (File.Exists(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "/" + "DBK - " + manifestlist[0].CompanyName + " - " + manifestlist[0].Collection[0].LogisticCompany + " " + manifestlist[0].Collection[0].RateTypeDisplay + " - " + manifestlist[0].ManifestFileName + ".pdf"))
                        {
                            File.Delete(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "/" + "DBK - " + manifestlist[0].CompanyName + " - " + manifestlist[0].Collection[0].LogisticCompany + " " + manifestlist[0].Collection[0].RateTypeDisplay + " - " + manifestlist[0].ManifestFileName + ".pdf");
                            rp.ExportToPdf(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "/" + "DBK - " + manifestlist[0].CompanyName + " - " + manifestlist[0].Collection[0].LogisticCompany + " " + manifestlist[0].Collection[0].RateTypeDisplay + " - " + manifestlist[0].ManifestFileName + ".pdf");
                            result.FileName = "DBK - " + manifestlist[0].CompanyName + " - " + manifestlist[0].Collection[0].LogisticCompany + " " + manifestlist[0].Collection[0].RateTypeDisplay + " - " + manifestlist[0].ManifestFileName + ".pdf";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/" + "DBK - " + manifestlist[0].CompanyName + " - " + manifestlist[0].Collection[0].LogisticCompany + " " + manifestlist[0].Collection[0].RateTypeDisplay + " - " + manifestlist[0].ManifestFileName + ".pdf";
                        }
                        else
                        {
                            rp.ExportToPdf(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "/" + "DBK - " + manifestlist[0].CompanyName + " - " + manifestlist[0].Collection[0].LogisticCompany + " " + manifestlist[0].Collection[0].RateTypeDisplay + " - " + manifestlist[0].ManifestFileName + ".pdf");
                            result.FileName = "DBK - " + manifestlist[0].CompanyName + " - " + manifestlist[0].Collection[0].LogisticCompany + " " + manifestlist[0].Collection[0].RateTypeDisplay + " - " + manifestlist[0].ManifestFileName + ".pdf";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/" + "DBK - " + manifestlist[0].CompanyName + " - " + manifestlist[0].Collection[0].LogisticCompany + " " + manifestlist[0].Collection[0].RateTypeDisplay + " - " + manifestlist[0].ManifestFileName + ".pdf";
                        }
                    }
                    else
                    {
                        ReportTemplate.Other.ManifestReport rp = new ReportTemplate.Other.ManifestReport();
                        if (RoleId == (int)FrayteUserRole.UserCustomer)
                        {
                            rp.Parameters["lblNote"].Value = "MEX Logistics Ltd is not responsible for any parcel lost with expired labels.";
                        }
                        else
                        {
                            rp.Parameters["lblNote"].Value = "FRAYTE Logistics Ltd is not responsible for any parcel lost with expired labels.";
                        }
                        rp.DataSource = manifestlist;

                        if (File.Exists(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "/" + "DBK - " + manifestlist[0].CompanyName + " - " + manifestlist[0].Collection[0].LogisticCompany + " " + manifestlist[0].Collection[0].RateTypeDisplay + " - " + manifestlist[0].ManifestFileName + ".pdf"))
                        {
                            File.Delete(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "/" + "DBK - " + manifestlist[0].CompanyName + " - " + manifestlist[0].Collection[0].LogisticCompany + " " + manifestlist[0].Collection[0].RateTypeDisplay + " - " + manifestlist[0].ManifestFileName + ".pdf");
                            rp.ExportToPdf(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "/" + "DBK - " + manifestlist[0].CompanyName + " - " + manifestlist[0].Collection[0].LogisticCompany + " " + manifestlist[0].Collection[0].RateTypeDisplay + " - " + manifestlist[0].ManifestFileName + ".pdf");
                            result.FileName = "DBK - " + manifestlist[0].CompanyName + " - " + manifestlist[0].Collection[0].LogisticCompany + " " + manifestlist[0].Collection[0].RateTypeDisplay + " - " + manifestlist[0].ManifestFileName + ".pdf";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/" + "DBK - " + manifestlist[0].CompanyName + " - " + manifestlist[0].Collection[0].LogisticCompany + " " + manifestlist[0].Collection[0].RateTypeDisplay + " - " + manifestlist[0].ManifestFileName + ".pdf";
                        }
                        else
                        {
                            rp.ExportToPdf(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + "/" + "DBK - " + manifestlist[0].CompanyName + " - " + manifestlist[0].Collection[0].LogisticCompany + " " + manifestlist[0].Collection[0].RateTypeDisplay + " - " + manifestlist[0].ManifestFileName + ".pdf");
                            result.FileName = "DBK - " + manifestlist[0].CompanyName + " - " + manifestlist[0].Collection[0].LogisticCompany + " " + manifestlist[0].Collection[0].RateTypeDisplay + " - " + manifestlist[0].ManifestFileName + ".pdf";
                            result.FilePath = AppSettings.WebApiPath + "ReportFiles/" + "DBK - " + manifestlist[0].CompanyName + " - " + manifestlist[0].Collection[0].LogisticCompany + " " + manifestlist[0].Collection[0].RateTypeDisplay + " - " + manifestlist[0].ManifestFileName + ".pdf";
                        }
                    }
                }
                else if (manifestlist != null && manifestlist.Count > 0 && moduleType == FrayteShipmentServiceType.eCommerce)
                {
                    ReportTemplate.Other.eCommerceManifestReport rp = new ReportTemplate.Other.eCommerceManifestReport();
                    rp.DataSource = manifestlist;
                    if (File.Exists(HttpContext.Current.Server.MapPath(AppSettings.eCommerceReportFolder) + "/" + manifestlist[0].ManifestName + ".pdf"))
                    {
                        File.Delete(HttpContext.Current.Server.MapPath(AppSettings.eCommerceReportFolder) + "/" + manifestlist[0].ManifestName + ".pdf");
                        rp.ExportToPdf(HttpContext.Current.Server.MapPath(AppSettings.eCommerceReportFolder) + "/" + manifestlist[0].ManifestName + ".pdf");
                        result.FileName = manifestlist[0].ManifestName + ".pdf";
                        result.FilePath = AppSettings.WebApiPath + "ReportFiles/eCommerce/" + manifestlist[0].ManifestName + ".pdf";
                    }
                    else
                    {
                        if (!Directory.Exists(HttpContext.Current.Server.MapPath(AppSettings.eCommerceReportFolder)))
                            Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.eCommerceReportFolder));
                        rp.ExportToPdf(HttpContext.Current.Server.MapPath(AppSettings.eCommerceReportFolder) + "/" + manifestlist[0].ManifestName + ".pdf");
                        result.FileName = manifestlist[0].ManifestName + ".pdf";
                        result.FilePath = AppSettings.WebApiPath + "ReportFiles/eCommerce/" + manifestlist[0].ManifestName + ".pdf";
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }
    }
}
