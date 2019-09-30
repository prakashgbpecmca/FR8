using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frayte.Services.Utility;
using Frayte.Services.Business;
using Frayte.Services.Models;
using System.Web;
using System.Drawing.Imaging;

namespace Report.Generator.ManifestReport
{
    public class eCommerceShipmentLabelReport
    {

        public FrayteResult GeteCommerceShipmentLabelReportDetail(int eCommerceShipmentId, FrayteeCommerceShipmentLabelReport obj, string filename)
        {
            FrayteResult result = new FrayteResult();

            FrayteeCommerceShipmentLabelReport frayteeCommerceLabelReport = new eCommerceShipmentRepository().GeteCommerceShipmentLabelReportDetail(obj);
            List<FrayteeCommerceShipmentLabelReport> reportDataSource = new List<FrayteeCommerceShipmentLabelReport>();
            if (frayteeCommerceLabelReport != null)
            {
                ReportTemplate.Other.eCommerceLabelReport re = new ReportTemplate.Other.eCommerceLabelReport();

                // Set ShipFrom Address In Label
                re.xrRichText1.Text = frayteeCommerceLabelReport.eCommerceShipment.ShipFrom.FirstName.ToUpper() + " " + frayteeCommerceLabelReport.eCommerceShipment.ShipFrom.LastName.ToUpper() +
                                      System.Environment.NewLine +
                                      frayteeCommerceLabelReport.eCommerceShipment.ShipFrom.CompanyName.ToUpper() + System.Environment.NewLine + frayteeCommerceLabelReport.eCommerceShipment.ShipFrom.Address.ToUpper();

                if (!string.IsNullOrEmpty(frayteeCommerceLabelReport.eCommerceShipment.ShipFrom.Address2))
                {
                    re.xrRichText1.Text += System.Environment.NewLine + frayteeCommerceLabelReport.eCommerceShipment.ShipFrom.Address2.ToUpper();
                }

                re.xrRichText1.Text += System.Environment.NewLine + frayteeCommerceLabelReport.eCommerceShipment.ShipFrom.City.ToUpper();

                if (!string.IsNullOrEmpty(frayteeCommerceLabelReport.eCommerceShipment.ShipFrom.PostCode))
                {
                    re.xrRichText1.Text += " -  " + frayteeCommerceLabelReport.eCommerceShipment.ShipFrom.PostCode.ToUpper();
                }
                if (!string.IsNullOrEmpty(frayteeCommerceLabelReport.eCommerceShipment.ShipFrom.State))
                {
                    re.xrRichText1.Text += System.Environment.NewLine + frayteeCommerceLabelReport.eCommerceShipment.ShipFrom.State.ToUpper();
                }

                // Set Shipto Address In Label
                re.xrRichText2.Text = frayteeCommerceLabelReport.eCommerceShipment.ShipTo.FirstName.ToUpper() + " " + frayteeCommerceLabelReport.eCommerceShipment.ShipTo.LastName.ToUpper() +
                                      System.Environment.NewLine +
                                      frayteeCommerceLabelReport.eCommerceShipment.ShipTo.CompanyName.ToUpper() + System.Environment.NewLine + frayteeCommerceLabelReport.eCommerceShipment.ShipTo.Address.ToUpper();

                if (!string.IsNullOrEmpty(frayteeCommerceLabelReport.eCommerceShipment.ShipTo.Address2))
                {
                    re.xrRichText2.Text += ", " + frayteeCommerceLabelReport.eCommerceShipment.ShipTo.Address2.ToUpper();
                }

                re.xrRichText2.Text += System.Environment.NewLine + frayteeCommerceLabelReport.eCommerceShipment.ShipTo.City.ToUpper();
                if (!string.IsNullOrEmpty(frayteeCommerceLabelReport.eCommerceShipment.ShipTo.PostCode))
                {
                    re.xrRichText2.Text += " -  " + frayteeCommerceLabelReport.eCommerceShipment.ShipTo.PostCode.ToUpper();
                }
                if (!string.IsNullOrEmpty(frayteeCommerceLabelReport.eCommerceShipment.ShipTo.State))
                {
                    re.xrRichText2.Text += System.Environment.NewLine + frayteeCommerceLabelReport.eCommerceShipment.ShipTo.State.ToUpper();
                }

                reportDataSource.Add(frayteeCommerceLabelReport);


                re.Parameters["LogoPath"].Value = obj.BarcodePath;
                re.DataSource = reportDataSource;

                string filePathToSave = AppSettings.eCommerceLabelFolder + "/" + eCommerceShipmentId.ToString();
                if (AppSettings.ShipmentCreatedFrom == "BATCH")
                {
                    filePathToSave = AppSettings.eCommerceUploadLabelFolder + eCommerceShipmentId.ToString();
                    //filePathToSave = filePathToSave;
                }
                else
                {
                    filePathToSave = HttpContext.Current.Server.MapPath(filePathToSave);
                }
                //DevExpress.XtraPrinting.ExportOptions options = re.ExportOptions;
                DevExpress.XtraPrinting.ImageExportOptions imageOptions = re.PrintingSystem.ExportOptions.Image;
                if (imageOptions != null)
                {
                    // Setup 150 DPI as default resolution for image exports
                    imageOptions.Resolution = 3000;
                    imageOptions.Format = ImageFormat.Jpeg;
                }

                DevExpress.XtraPrinting.PdfExportOptions pdfOptions = re.ExportOptions.Pdf;
                //pdfOptions.PageRange = "1";
                pdfOptions.ImageQuality = DevExpress.XtraPrinting.PdfJpegImageQuality.Highest;
                pdfOptions.PdfACompatibility = DevExpress.XtraPrinting.PdfACompatibility.PdfA2b;

                if (!System.IO.Directory.Exists(filePathToSave))
                    System.IO.Directory.CreateDirectory(filePathToSave);
                //re.ExportToImage(HttpContext.Current.Server.MapPath(AppSettings.eCommerceLabelFolder) + "/" + eCommerceShipmentId + "/" + FileName.ToString() + ".Jpeg", imageOptions);
                if (AppSettings.ShipmentCreatedFrom == "BATCH")
                {
                    re.ExportToImage(AppSettings.eCommerceUploadLabelFolder + eCommerceShipmentId + "/" + filename + ".jpg", imageOptions);
                    re.ExportToPdf(AppSettings.eCommerceUploadLabelFolder + eCommerceShipmentId + "/" + filename + ".pdf", pdfOptions);
                }
                else
                {
                    re.ExportToImage(HttpContext.Current.Server.MapPath(AppSettings.eCommerceLabelFolder) + "/" + eCommerceShipmentId + "/" + filename + ".jpg", imageOptions);
                    re.ExportToPdf(HttpContext.Current.Server.MapPath(AppSettings.eCommerceLabelFolder) + "/" + eCommerceShipmentId + "/" + filename + ".pdf", pdfOptions);
                }
                result.Status = true;
            }
            else
            {
                result.Status = false;
            }

            return result;
        }

        public FrayteResult GeteCommerceShipmentLabelReportDetailList(int eCommerceShipmentId, List<FrayteeCommerceShipmentLabelReport> obj, string filename)
        {
            FrayteResult result = new FrayteResult();

            List<FrayteeCommerceShipmentLabelReport> frayteeCommerceLabelReportList = new eCommerceShipmentRepository().GeteCommerceShipmentLabelReportDetail(obj);
            FrayteeCommerceShipmentLabelReport frayteeCommerceLabelReport = frayteeCommerceLabelReportList[0];
            List<FrayteeCommerceShipmentLabelReport> reportDataSource = new List<FrayteeCommerceShipmentLabelReport>();
            if (frayteeCommerceLabelReport != null)
            {
                ReportTemplate.Other.eCommerceLabelReport re = new ReportTemplate.Other.eCommerceLabelReport();

                // Set ShipFrom Address In Label
                re.xrRichText1.Text = frayteeCommerceLabelReport.eCommerceShipment.ShipFrom.FirstName.ToUpper() + " " + frayteeCommerceLabelReport.eCommerceShipment.ShipFrom.LastName.ToUpper() +
                                      System.Environment.NewLine +
                                      frayteeCommerceLabelReport.eCommerceShipment.ShipFrom.CompanyName.ToUpper() + System.Environment.NewLine + frayteeCommerceLabelReport.eCommerceShipment.ShipFrom.Address.ToUpper();

                if (!string.IsNullOrEmpty(frayteeCommerceLabelReport.eCommerceShipment.ShipFrom.Address2))
                {
                    re.xrRichText1.Text += System.Environment.NewLine + frayteeCommerceLabelReport.eCommerceShipment.ShipFrom.Address2.ToUpper();
                }

                re.xrRichText1.Text += System.Environment.NewLine + frayteeCommerceLabelReport.eCommerceShipment.ShipFrom.City.ToUpper();

                if (!string.IsNullOrEmpty(frayteeCommerceLabelReport.eCommerceShipment.ShipFrom.PostCode))
                {
                    re.xrRichText1.Text += " -  " + frayteeCommerceLabelReport.eCommerceShipment.ShipFrom.PostCode.ToUpper();
                }
                if (!string.IsNullOrEmpty(frayteeCommerceLabelReport.eCommerceShipment.ShipFrom.State))
                {
                    re.xrRichText1.Text += System.Environment.NewLine + frayteeCommerceLabelReport.eCommerceShipment.ShipFrom.State.ToUpper();
                }

                // Set Shipto Address In Label
                re.xrRichText2.Text = frayteeCommerceLabelReport.eCommerceShipment.ShipTo.FirstName.ToUpper() + " " + frayteeCommerceLabelReport.eCommerceShipment.ShipTo.LastName.ToUpper() +
                                      System.Environment.NewLine +
                                      frayteeCommerceLabelReport.eCommerceShipment.ShipTo.CompanyName.ToUpper() + System.Environment.NewLine + frayteeCommerceLabelReport.eCommerceShipment.ShipTo.Address.ToUpper();

                if (!string.IsNullOrEmpty(frayteeCommerceLabelReport.eCommerceShipment.ShipTo.Address2))
                {
                    re.xrRichText2.Text += ", " + frayteeCommerceLabelReport.eCommerceShipment.ShipTo.Address2.ToUpper();
                }

                re.xrRichText2.Text += System.Environment.NewLine + frayteeCommerceLabelReport.eCommerceShipment.ShipTo.City.ToUpper();
                if (!string.IsNullOrEmpty(frayteeCommerceLabelReport.eCommerceShipment.ShipTo.PostCode))
                {
                    re.xrRichText2.Text += " -  " + frayteeCommerceLabelReport.eCommerceShipment.ShipTo.PostCode.ToUpper();
                }
                if (!string.IsNullOrEmpty(frayteeCommerceLabelReport.eCommerceShipment.ShipTo.State))
                {
                    re.xrRichText2.Text += System.Environment.NewLine + frayteeCommerceLabelReport.eCommerceShipment.ShipTo.State.ToUpper();
                }

                reportDataSource.Add(frayteeCommerceLabelReport);
                re.Parameters["LogoPath"].Value = obj[0].BarcodePath;
                re.DataSource = frayteeCommerceLabelReportList;

                string filePathToSave = AppSettings.eCommerceLabelFolder + "/" + eCommerceShipmentId.ToString();
                if (AppSettings.ShipmentCreatedFrom == "BATCH")
                {
                     filePathToSave = AppSettings.eCommerceUploadLabelFolder + "/" + eCommerceShipmentId.ToString();
                }
                else
                {
                    filePathToSave = HttpContext.Current.Server.MapPath(filePathToSave);
                }                    

                //DevExpress.XtraPrinting.ExportOptions options = re.ExportOptions;
                DevExpress.XtraPrinting.ImageExportOptions imageOptions = re.PrintingSystem.ExportOptions.Image;
                if (imageOptions != null)
                {
                    // Setup 150 DPI as default resolution for image exports
                    imageOptions.Resolution = 3000;
                    imageOptions.Format = ImageFormat.Jpeg;
                }

                DevExpress.XtraPrinting.PdfExportOptions pdfOptions = re.ExportOptions.Pdf;
                //pdfOptions.PageRange = "1";
                pdfOptions.ImageQuality = DevExpress.XtraPrinting.PdfJpegImageQuality.Highest;
                pdfOptions.PdfACompatibility = DevExpress.XtraPrinting.PdfACompatibility.PdfA2b;

                if (!System.IO.Directory.Exists(filePathToSave))
                    System.IO.Directory.CreateDirectory(filePathToSave);
                //re.ExportToImage(HttpContext.Current.Server.MapPath(AppSettings.eCommerceLabelFolder) + "/" + eCommerceShipmentId + "/" + FileName.ToString() + ".Jpeg", imageOptions);
                if (AppSettings.ShipmentCreatedFrom == "BATCH")
                {
                    re.ExportToImage(AppSettings.eCommerceUploadLabelFolder + eCommerceShipmentId + "/" + filename + ".jpg", imageOptions);
                    re.ExportToPdf(AppSettings.eCommerceUploadLabelFolder + eCommerceShipmentId + "/" + filename + ".pdf", pdfOptions);
                }
                else
                {
                    re.ExportToImage(HttpContext.Current.Server.MapPath(AppSettings.eCommerceLabelFolder) + "/" + eCommerceShipmentId + "/" + filename + ".jpg", imageOptions);
                    re.ExportToPdf(HttpContext.Current.Server.MapPath(AppSettings.eCommerceLabelFolder) + "/" + eCommerceShipmentId + "/" + filename + ".pdf", pdfOptions);
                }
                result.Status = true;
            }
            else
            {
                result.Status = false;
            }

            return result;
        }

        public FrayteResult GenerateDirectShipemntReportPdf()
        {
            FrayteResult result = new FrayteResult();
            string pdfPath = HttpContext.Current.Server.MapPath("~/PackageLabel/" + "66" + "/") + "DHLPDF" + ".pdf";
            ReportTemplate.Other.DirectBookingLabelReport re = new ReportTemplate.Other.DirectBookingLabelReport();
            re.ExportToPdf(pdfPath);
            return result;
        }

        public FrayteResult GeteCommerceUploadShipmentLabelReportDetail(int eCommerceShipmentId, FrayteeCommerceUploadShipmentLabelReport obj, int FileName)
        {
            FrayteResult result = new FrayteResult();

            FrayteeCommerceUploadShipmentLabelReport frayteeCommerceLabelReport = new eCommerceUploadShipmentRepository().GeteCommerceUploadShipmentLabelReportDetail(obj);
            List<FrayteeCommerceUploadShipmentLabelReport> reportDataSource = new List<FrayteeCommerceUploadShipmentLabelReport>();
            if (frayteeCommerceLabelReport != null)
            {
                ReportTemplate.Other.eCommerceLabelReport re = new ReportTemplate.Other.eCommerceLabelReport();

                // Set ShipFrom Address In Label
                re.xrRichText1.Text = frayteeCommerceLabelReport.eCommerceShipment.ShipFrom.FirstName.ToUpper() + " " + frayteeCommerceLabelReport.eCommerceShipment.ShipFrom.LastName.ToUpper() +
                                      System.Environment.NewLine +
                                      frayteeCommerceLabelReport.eCommerceShipment.ShipFrom.CompanyName.ToUpper() + System.Environment.NewLine + frayteeCommerceLabelReport.eCommerceShipment.ShipFrom.Address.ToUpper();

                if (!string.IsNullOrEmpty(frayteeCommerceLabelReport.eCommerceShipment.ShipFrom.Address2))
                {
                    re.xrRichText1.Text += System.Environment.NewLine + frayteeCommerceLabelReport.eCommerceShipment.ShipFrom.Address2.ToUpper();
                }

                re.xrRichText1.Text += System.Environment.NewLine + frayteeCommerceLabelReport.eCommerceShipment.ShipFrom.City.ToUpper();

                if (!string.IsNullOrEmpty(frayteeCommerceLabelReport.eCommerceShipment.ShipFrom.PostCode))
                {
                    re.xrRichText1.Text += " -  " + frayteeCommerceLabelReport.eCommerceShipment.ShipFrom.PostCode.ToUpper();
                }
                if (!string.IsNullOrEmpty(frayteeCommerceLabelReport.eCommerceShipment.ShipFrom.State))
                {
                    re.xrRichText1.Text += System.Environment.NewLine + frayteeCommerceLabelReport.eCommerceShipment.ShipFrom.State.ToUpper();
                }

                // Set Shipto Address In Label
                re.xrRichText2.Text = frayteeCommerceLabelReport.eCommerceShipment.ShipTo.FirstName.ToUpper() + " " + frayteeCommerceLabelReport.eCommerceShipment.ShipTo.LastName.ToUpper() +
                                      System.Environment.NewLine +
                                      frayteeCommerceLabelReport.eCommerceShipment.ShipTo.CompanyName.ToUpper() + System.Environment.NewLine + frayteeCommerceLabelReport.eCommerceShipment.ShipTo.Address.ToUpper();

                if (!string.IsNullOrEmpty(frayteeCommerceLabelReport.eCommerceShipment.ShipTo.Address2))
                {
                    re.xrRichText2.Text += ", " + frayteeCommerceLabelReport.eCommerceShipment.ShipTo.Address2.ToUpper();
                }

                re.xrRichText2.Text += System.Environment.NewLine + frayteeCommerceLabelReport.eCommerceShipment.ShipTo.City.ToUpper();
                if (!string.IsNullOrEmpty(frayteeCommerceLabelReport.eCommerceShipment.ShipTo.PostCode))
                {
                    re.xrRichText2.Text += " -  " + frayteeCommerceLabelReport.eCommerceShipment.ShipTo.PostCode.ToUpper();
                }
                if (!string.IsNullOrEmpty(frayteeCommerceLabelReport.eCommerceShipment.ShipTo.State))
                {
                    re.xrRichText2.Text += System.Environment.NewLine + frayteeCommerceLabelReport.eCommerceShipment.ShipTo.State.ToUpper();
                }

                reportDataSource.Add(frayteeCommerceLabelReport);
                re.Parameters["LogoPath"].Value = obj.BarcodePath;
                re.DataSource = reportDataSource;

                string filePathToSave = AppSettings.eCommerceUploadShipmentLabelFolder + "/" + eCommerceShipmentId.ToString();
                DevExpress.XtraPrinting.PdfExportOptions pdfOptions = re.ExportOptions.Pdf;
                pdfOptions.ImageQuality = DevExpress.XtraPrinting.PdfJpegImageQuality.Highest;
                pdfOptions.PdfACompatibility = DevExpress.XtraPrinting.PdfACompatibility.PdfA2b;

                if (!System.IO.Directory.Exists(filePathToSave))
                    System.IO.Directory.CreateDirectory(filePathToSave);
                re.ExportToPdf(AppSettings.eCommerceUploadShipmentLabelFolder + "/" + eCommerceShipmentId + "/" + FileName.ToString() + ".pdf", pdfOptions);
                result.Status = true;
            }
            else
            {
                result.Status = false;
            }

            return result;
        }
    }
}
