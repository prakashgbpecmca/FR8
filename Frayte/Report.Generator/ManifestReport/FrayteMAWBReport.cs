using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frayte.Services.Business;
using Frayte.Services.Models;
using Frayte.Services.Utility;
using System.IO;
using System.Web;
using Frayte.Services.Models.Express;

namespace Report.Generator.ManifestReport
{
    public class FrayteMAWBReport
    {
        public FrayteManifestName FrayteMAWB(ExpressShipmentModel shipment, string moduleType)
        {
            FrayteManifestName result = new FrayteManifestName();
            try
            {
                var data = new FRAYTEAWBRepository().GetFrayteMAWBPDFDataSource(shipment.ExpressId, moduleType);
                Report.Generator.ReportTemplate.Express.FrayteMAWB report = new Report.Generator.ReportTemplate.Express.FrayteMAWB();
                report.DataSource = data;

                if (System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~/PackageLabel/Express/" + shipment.ExpressId)))
                {
                    string fileName = shipment.Service.HubCarrier == FrayteShortName.FrayteDomesticJP ? FrayteShortName.FrayteDomesticJP +"_" + shipment.TrackingNo.Replace(" ", "") + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (All)" + ".pdf" : FrayteShortName.FrayteDomesticSG + "_" + shipment.TrackingNo.Replace(" ", "") + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (All)" + ".pdf";
                    string filePath = AppSettings.WebApiPath + "/PackageLabel/Express/" + shipment.ExpressId + "/" + fileName;
                    string filePhysicalPath = HttpContext.Current.Server.MapPath("~/PackageLabel/Express/" + shipment.ExpressId + "/" + fileName);

                    new ExpressRepository().UpdateExpressLogisticlabel(shipment.ExpressId, fileName);
                    new ExpressRepository().SaveExpressDetailPackagelabel(shipment.ExpressId);

                    if (File.Exists(filePhysicalPath))
                    {
                        File.Delete(filePhysicalPath);
                    }
                    report.ExportToPdf(filePhysicalPath);

                    result.FileName = fileName;
                    result.FilePath = filePath;
                }
                else
                {
                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/PackageLabel/Express/" + shipment.ExpressId));
                    string fileName = shipment.Service.HubCarrier == FrayteShortName.FrayteDomesticJP ? FrayteShortName.FrayteDomesticJP + "_" + shipment.TrackingNo.Replace(" ", "") + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (All)" + ".pdf" : FrayteShortName.FrayteDomesticSG + "_" + shipment.TrackingNo.Replace(" ", "") + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (All)" + ".pdf";
                    string filePath = AppSettings.WebApiPath + "/PackageLabel/Express/" + shipment.ExpressId + "/" + fileName;
                    string filePhysicalPath = HttpContext.Current.Server.MapPath("~/PackageLabel/Express/" + shipment.ExpressId + "/" + fileName);
                    new ExpressRepository().UpdateExpressLogisticlabel(shipment.ExpressId, fileName);
                    new ExpressRepository().SaveExpressDetailPackagelabel(shipment.ExpressId);

                    if (File.Exists(filePhysicalPath))
                    {
                        File.Delete(filePhysicalPath);
                    }
                    report.ExportToPdf(filePhysicalPath);

                    result.FileName = fileName;
                    result.FilePath = filePath;
                }
                return result;
            }
            catch (Exception e)
            {
                return result;
            }
        }
    }
}
