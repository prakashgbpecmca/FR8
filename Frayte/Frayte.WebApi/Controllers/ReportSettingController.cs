using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Web.Http;
using Frayte.Services.Business;
using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using System.Data.Entity.Validation;
using RazorEngine;
using RazorEngine.Templating;
using Frayte.WebApi.Utility;
using System.IO;

namespace Frayte.WebApi.Controllers
{
    [Authorize]
    public class ReportSettingController : ApiController
    {
        string filename = string.Empty;

        [HttpGet]
        public FryatePODResult ShipmentPDF_Report()
        {
            FrayteOperationZone foz = GetBusinessZoneID();
            GeneratePDF_ShipmentReportDetail(foz.OperationZoneId);
            string pdfFileName = HttpContext.Current.Server.MapPath("~/UploadFiles/PDFGenerator/") + filename;
            FryatePODResult result = new FryatePODResult();
            result.PDFPath = pdfFileName;
            return result;
        }

        public FrayteOperationZone GetBusinessZoneID()
        {
            FrayteOperationZone OperationZone = UtilityRepository.GetOperationZone();
            return OperationZone;
        }

        #region -- Private Methods --

        private string GeneratePDF_ShipmentReportDetail(int OperationZoneId)
        {
            string pageStyleSheet = HttpContext.Current.Server.MapPath("~/Content/") + "confirm.css";
            string bootStrapStyleSheet = HttpContext.Current.Server.MapPath("~/Content/") + "bootstrap.css";

            string pdfLogo = HttpContext.Current.Server.MapPath("~/Content/") + "FrayteLogo.png";
            var detail = new ReportSettingRepository().GetShipmentId(OperationZoneId);

            FrayteShipmentReportPDF obj = new FrayteShipmentReportPDF();
            obj.ShipmentReportDetail = new List<FrayteShipmentReport>();
            obj.ShipmentReportDetail = detail;
            obj.PageStyleSheet = pageStyleSheet;
            obj.BootStrapStyleSheet = bootStrapStyleSheet;
            obj.pdfLogo = pdfLogo;

            string template = File.ReadAllText(HttpContext.Current.Server.MapPath("~/Templates/") + "ReportPdf.cshtml");
            var EmailBody = Engine.Razor.RunCompile(template, "ShipmentReportPDF", null, obj);
            string pdfFileName = "ShipmentPODReport_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss") + ".html";
            string pdfFilePath = HttpContext.Current.Server.MapPath("~/UploadFiles/PDFGenerator/HTMLFile");
            string pdfFullPath = pdfFilePath + @"\" + pdfFileName;

            using (FileStream fs = new FileStream(pdfFullPath, FileMode.Create))
            {
                using (StreamWriter w = new StreamWriter(fs, System.Text.Encoding.UTF8))
                {
                    w.WriteLine(EmailBody);
                }
            }

            List<string> lstHtmlFiles = new List<string>();
            lstHtmlFiles.Add(pdfFullPath);

            //Before creating new PDF file, remove the earlier one.
            filename = "ShipmentPODReport_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss") + ".PDF";
            string pdfPath = HttpContext.Current.Server.MapPath("~/UploadFiles/PDFGenerator/") + "ShipmentPODReport_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss") + ".PDF";
            if (File.Exists(pdfPath))
            {
                File.Delete(pdfPath);
            }

            string pdfFile = PDFGenerator.HtmlToPdf("~/UploadFiles/PDFGenerator", "ShipmentPODReport_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss"), lstHtmlFiles.ToArray(), null);

            if (System.IO.File.Exists(pdfFullPath))
            {
                System.IO.File.Delete(pdfFullPath);
            }

            return pdfFile;
        }

        #endregion
    }
}
