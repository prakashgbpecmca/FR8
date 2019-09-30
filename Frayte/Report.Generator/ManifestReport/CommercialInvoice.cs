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
using DevExpress.XtraReports.Parameters;
using System.IO;

namespace Report.Generator.ManifestReport
{
    public class CommercialInvoice
    {
        public FrayteManifestName CreateCommercialInvoce(int DirectShipmnetId, string CustomerName)
        {
            FrayteManifestName result = new FrayteManifestName();

            var detail = new DirectShipmentRepository().GetCommercilaInvoiceFileName(DirectShipmnetId);

            var invoice = new DirectShipmentRepository().CreateDirectBookingCommercialInvoice(DirectShipmnetId);
            if (invoice != null)
            {
                ReportTemplate.Other.CommercialInvoice cm = new ReportTemplate.Other.CommercialInvoice();
                cm.DataSource = invoice;
                cm.ExportToPdf(HttpContext.Current.Server.MapPath(AppSettings.ReportFolder) + detail.CompanyName + "-Commercial Invoice-" + detail.CreationDate.ToString("yyyy-dd-MM") + "-" + detail.LogisticType + " " + detail.RateType + "-AWB-" + detail.FrayetNo + ".pdf");
                result.FileName = detail.CompanyName + "-Commercial Invoice-" + detail.CreationDate.ToString("yyyy-dd-MM") + "-" + detail.LogisticType + " " + detail.RateType + "-AWB-" + detail.FrayetNo + ".pdf";
                result.FilePath = AppSettings.WebApiPath + "ReportFiles/" + detail.CompanyName + "-Commercial Invoice-" + detail.CreationDate.ToString("yyyy-dd-MM") + "-" + detail.LogisticType + " " + detail.RateType + "-AWB-" + detail.FrayetNo + ".pdf";
                result.FileStatus = true;
            }
            else
            {
                result.FileStatus = false;
            }
            return result;
        }
    }
}
