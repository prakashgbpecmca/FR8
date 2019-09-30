using Frayte.Services.Business;
using Frayte.Services.Models;
using Frayte.Services.Models.Express;
using Frayte.Services.Utility;
using Report.Generator.ReportTemplate;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Report.Generator.ManifestReport
{
    public class PackageLabelReport
    {
        public FrayteResult GenerateSeperateLabelReport(int DirectShipmentId, string image)
        {

            FrayteResult result = new FrayteResult();

            try
            {
                ReportTemplate.Other.PackageLabelReport re = new ReportTemplate.Other.PackageLabelReport();

                DevExpress.XtraPrinting.PdfExportOptions pdfOptions = re.ExportOptions.Pdf;

                pdfOptions.ImageQuality = DevExpress.XtraPrinting.PdfJpegImageQuality.Highest;
                pdfOptions.PdfACompatibility = DevExpress.XtraPrinting.PdfACompatibility.PdfA2b;


                string[] ff = image.ToString().Split('.');

                string name = ff[0].ToString();

                var imageToSave = HttpContext.Current.Server.MapPath(AppSettings.LabelFolder) + "/" + DirectShipmentId + "/" + image;

                re.Parameters["ImagePath"].Value = imageToSave;

                if (File.Exists(HttpContext.Current.Server.MapPath(AppSettings.LabelFolder) + "/" + DirectShipmentId + "/" + name + ".pdf"))
                {
                    File.Delete(HttpContext.Current.Server.MapPath(AppSettings.LabelFolder) + "/" + DirectShipmentId + "/" + name + ".pdf");
                }

                if (!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath(AppSettings.LabelFolder) + "/" + DirectShipmentId))
                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.LabelFolder) + "/" + DirectShipmentId);

                re.ExportToPdf(HttpContext.Current.Server.MapPath(AppSettings.LabelFolder) + "/" + DirectShipmentId + "/" + name + ".pdf", pdfOptions);
                result.Status = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Status = false;
                return result;
            }
        }

        public FrayteResult GenerateAllLabelReport(int DirectShipmentId, DirectBookingShipmentDraftDetail directBookingDetail, List<string> imgList, string CourierName, string LogisticLabel)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                List<PdfImage> imglist = new List<PdfImage>();
                if (AppSettings.LabelSave == "")
                {
                    PdfImage pbimage;
                    foreach (var img in imgList)
                    {
                        pbimage = new PdfImage();
                        pbimage.FullPath = AppSettings.WebApiPath + "/PackageLabel/" + DirectShipmentId + "/" + img;
                        imglist.Add(pbimage);
                    }

                    string pdfPath = string.Empty;
                    if (directBookingDetail != null)
                    {
                        pdfPath = AppSettings.WebApiPath + "/PackageLabel/" + DirectShipmentId + "/" + LogisticLabel;
                    }
                    else
                    {
                        string[] ff = imgList[0].ToString().Split('.');
                        string name = ff[0].ToString();
                        pdfPath = AppSettings.WebApiPath + "/PackageLabel/" + DirectShipmentId + "/" + name + ".pdf";

                    }
                    if (File.Exists(pdfPath))
                    {
                        File.Delete(pdfPath);
                    }
                    if (!System.IO.Directory.Exists(AppSettings.WebApiPath + "/PackageLabel/" + DirectShipmentId))
                        System.IO.Directory.CreateDirectory(AppSettings.WebApiPath + "/PackageLabel/" + DirectShipmentId);

                    if (CourierName == FrayteCourierCompany.TNT)
                    {
                        ReportTemplate.Other.TNTImagesToPDFReport report = new ReportTemplate.Other.TNTImagesToPDFReport();
                        report.DataSource = imglist;
                        DevExpress.XtraPrinting.PdfExportOptions pdfOptions = report.ExportOptions.Pdf;
                        pdfOptions.ImageQuality = DevExpress.XtraPrinting.PdfJpegImageQuality.Highest;
                        pdfOptions.PdfACompatibility = DevExpress.XtraPrinting.PdfACompatibility.PdfA2b;
                        report.ExportToPdf(pdfPath, pdfOptions);
                        result.Status = true;
                    }
                    else 
                    {
                        ReportTemplate.Other.DHLImageToPDFReport dhlReport = new ReportTemplate.Other.DHLImageToPDFReport();
                        dhlReport.DataSource = imglist;
                        DevExpress.XtraPrinting.PdfExportOptions pdfOptions = dhlReport.ExportOptions.Pdf;
                        pdfOptions.ImageQuality = DevExpress.XtraPrinting.PdfJpegImageQuality.Highest;
                        pdfOptions.PdfACompatibility = DevExpress.XtraPrinting.PdfACompatibility.PdfA2b;
                        dhlReport.ExportToPdf(pdfPath, pdfOptions);
                        result.Status = true;
                    }
                   
                    return result;
                }
                else
                {
                    PdfImage pbimage;
                    foreach (var img in imgList)
                    {
                        pbimage = new PdfImage();
                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                        {
                            pbimage.FullPath = AppSettings.LabelFolder + "/" + DirectShipmentId + "/" + img;
                        }
                        else
                        {
                            pbimage.FullPath = HttpContext.Current.Server.MapPath(AppSettings.LabelFolder) + DirectShipmentId + "/" + img;
                        }
                        
                        imglist.Add(pbimage);
                    }

                    string pdfPath = string.Empty;
                    if (directBookingDetail != null)
                    {
                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                        {
                            pdfPath = AppSettings.LabelFolder + "/" + DirectShipmentId + "/" + LogisticLabel;
                        }
                        else
                        {
                            pdfPath = HttpContext.Current.Server.MapPath("~/PackageLabel/" + DirectShipmentId + "/") + LogisticLabel;
                        }
                    }
                    else
                    {
                        string[] ff = imgList[0].ToString().Split('.');
                        string name = ff[0].ToString();
                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                        {
                            pdfPath = AppSettings.LabelFolder + "/" + DirectShipmentId + "/" + name + ".pdf";
                        }
                        else
                        {
                            pdfPath = HttpContext.Current.Server.MapPath(AppSettings.LabelFolder) + DirectShipmentId + "/" + name + ".pdf";
                        }
                    }
                    if (File.Exists(pdfPath))
                    {
                        File.Delete(pdfPath);
                    }
                    if (AppSettings.ShipmentCreatedFrom == "BATCH")
                    {
                        if (!System.IO.Directory.Exists(AppSettings.LabelFolder + "/" + DirectShipmentId))
                            System.IO.Directory.CreateDirectory(AppSettings.LabelFolder + "/" + DirectShipmentId);
                    }
                    else
                    {
                        if (!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath(AppSettings.LabelFolder) + "/" + DirectShipmentId))
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.LabelFolder) + "/" + DirectShipmentId);
                    }
                    if (CourierName == FrayteCourierCompany.TNT)
                    {
                        ReportTemplate.Other.TNTImagesToPDFReport report = new ReportTemplate.Other.TNTImagesToPDFReport();
                        report.DataSource = imglist;
                        DevExpress.XtraPrinting.PdfExportOptions pdfOptions = report.ExportOptions.Pdf;
                        pdfOptions.ImageQuality = DevExpress.XtraPrinting.PdfJpegImageQuality.Highest;
                        pdfOptions.PdfACompatibility = DevExpress.XtraPrinting.PdfACompatibility.PdfA2b;
                        report.ExportToPdf(pdfPath, pdfOptions);
                        result.Status = true;
                    }
                    else 
                    {
                        ReportTemplate.Other.DHLImageToPDFReport dhlReport = new ReportTemplate.Other.DHLImageToPDFReport();
                        dhlReport.DataSource = imglist;
                        DevExpress.XtraPrinting.PdfExportOptions pdfOptions = dhlReport.ExportOptions.Pdf;
                        pdfOptions.ImageQuality = DevExpress.XtraPrinting.PdfJpegImageQuality.Highest;
                        pdfOptions.PdfACompatibility = DevExpress.XtraPrinting.PdfACompatibility.PdfA2b;
                        dhlReport.ExportToPdf(pdfPath, pdfOptions);
                        result.Status = true;
                    }
                   
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;

                return result;
            }
        }
             
        public FrayteResult ExpressGenerateAllLabelReport(int DirectShipmentId, ExpressShipmentModel directBookingDetail, List<string> imgList, string CourierName, string LogisticLabel)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                List<PdfImage> imglist = new List<PdfImage>();
                if (AppSettings.LabelSave == "")
                {
                    PdfImage pbimage;
                    foreach (var img in imgList)
                    {
                        pbimage = new PdfImage();
                        pbimage.FullPath = AppSettings.WebApiPath + "/PackageLabel/Express/" + DirectShipmentId + "/" + img;
                        imglist.Add(pbimage);
                    }

                    string pdfPath = string.Empty;
                    if (directBookingDetail != null)
                    {
                        pdfPath = AppSettings.WebApiPath + "/PackageLabel/Express/" + DirectShipmentId + "/" + LogisticLabel;
                    }
                    else
                    {
                        string[] ff = imgList[0].ToString().Split('.');
                        string name = ff[0].ToString();
                        pdfPath = AppSettings.WebApiPath + "/PackageLabel/Express/" + DirectShipmentId + "/" + name + ".pdf";

                    }
                    if (File.Exists(pdfPath))
                    {
                        File.Delete(pdfPath);
                    }
                    if (!System.IO.Directory.Exists(AppSettings.WebApiPath + "/PackageLabel/Express/" + DirectShipmentId))
                        System.IO.Directory.CreateDirectory(AppSettings.WebApiPath + "/PackageLabel/Express/" + DirectShipmentId);

                    if (CourierName == FrayteCourierCompany.TNT)
                    {
                        ReportTemplate.Other.TNTImagesToPDFReport report = new ReportTemplate.Other.TNTImagesToPDFReport();
                        report.DataSource = imglist;
                        DevExpress.XtraPrinting.PdfExportOptions pdfOptions = report.ExportOptions.Pdf;
                        pdfOptions.ImageQuality = DevExpress.XtraPrinting.PdfJpegImageQuality.Highest;
                        pdfOptions.PdfACompatibility = DevExpress.XtraPrinting.PdfACompatibility.PdfA2b;
                        report.ExportToPdf(pdfPath, pdfOptions);
                        result.Status = true;
                    }
                    else
                    {
                        ReportTemplate.Other.DHLImageToPDFReport dhlReport = new ReportTemplate.Other.DHLImageToPDFReport();
                        dhlReport.DataSource = imglist;
                        DevExpress.XtraPrinting.PdfExportOptions pdfOptions = dhlReport.ExportOptions.Pdf;
                        pdfOptions.ImageQuality = DevExpress.XtraPrinting.PdfJpegImageQuality.Highest;
                        pdfOptions.PdfACompatibility = DevExpress.XtraPrinting.PdfACompatibility.PdfA2b;
                        dhlReport.ExportToPdf(pdfPath, pdfOptions);
                        result.Status = true;
                    }

                    return result;
                }
                else
                {
                    PdfImage pbimage;
                    foreach (var img in imgList)
                    {
                        pbimage = new PdfImage();
                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                        {
                            pbimage.FullPath = AppSettings.LabelFolder + "/Express/" + DirectShipmentId + "/" + img;
                        }
                        else
                        {
                            pbimage.FullPath = HttpContext.Current.Server.MapPath(AppSettings.LabelFolder+ "/Express/") + DirectShipmentId + "/" + img;
                        }

                        imglist.Add(pbimage);
                    }

                    string pdfPath = string.Empty;
                    if (directBookingDetail != null)
                    {
                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                        {
                            pdfPath = AppSettings.LabelFolder + "/Express/" + DirectShipmentId + "/" + LogisticLabel;
                        }
                        else
                        {
                            pdfPath = HttpContext.Current.Server.MapPath("~/PackageLabel/Express/" + DirectShipmentId + "/") + LogisticLabel;
                        }
                    }
                    else
                    {
                        string[] ff = imgList[0].ToString().Split('.');
                        string name = ff[0].ToString();
                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                        {
                            pdfPath = AppSettings.LabelFolder + "/Express/" + DirectShipmentId + "/" + name + ".pdf";
                        }
                        else
                        {
                            pdfPath = HttpContext.Current.Server.MapPath(AppSettings.LabelFolder+ "/Express/") + DirectShipmentId + "/" + name + ".pdf";
                        }
                    }
                    if (File.Exists(pdfPath))
                    {
                        File.Delete(pdfPath);
                    }
                    if (AppSettings.ShipmentCreatedFrom == "BATCH")
                    {
                        if (!System.IO.Directory.Exists(AppSettings.LabelFolder + "/Express/" + DirectShipmentId))
                            System.IO.Directory.CreateDirectory(AppSettings.LabelFolder + "/Express/" + DirectShipmentId);
                    }
                    else
                    {
                        if (!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath(AppSettings.LabelFolder+ "/Express") + "/" + DirectShipmentId))
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.LabelFolder+ "/Express") + "/" + DirectShipmentId);
                    }
                    if (CourierName == FrayteCourierCompany.TNT)
                    {
                        ReportTemplate.Other.TNTImagesToPDFReport report = new ReportTemplate.Other.TNTImagesToPDFReport();
                        report.DataSource = imglist;
                        DevExpress.XtraPrinting.PdfExportOptions pdfOptions = report.ExportOptions.Pdf;
                        pdfOptions.ImageQuality = DevExpress.XtraPrinting.PdfJpegImageQuality.Highest;
                        pdfOptions.PdfACompatibility = DevExpress.XtraPrinting.PdfACompatibility.PdfA2b;
                        report.ExportToPdf(pdfPath, pdfOptions);
                        result.Status = true;
                    }
                    else
                    {
                        ReportTemplate.Other.DHLImageToPDFReport dhlReport = new ReportTemplate.Other.DHLImageToPDFReport();
                        dhlReport.DataSource = imglist;
                        DevExpress.XtraPrinting.PdfExportOptions pdfOptions = dhlReport.ExportOptions.Pdf;
                        pdfOptions.ImageQuality = DevExpress.XtraPrinting.PdfJpegImageQuality.Highest;
                        pdfOptions.PdfACompatibility = DevExpress.XtraPrinting.PdfACompatibility.PdfA2b;
                        dhlReport.ExportToPdf(pdfPath, pdfOptions);
                        result.Status = true;
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                return result;
            }
        }     
  
        public FrayteResult GenerateECommerceLabelReport(int eCommerceShipmentId, FrayteCommerceShipmentDraft eCommerceBookingDetail, List<string> imgList, string CourierName, string LogisticLabel)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                List<PdfImage> imglist = new List<PdfImage>();

                PdfImage pbimage;
                foreach (var img in imgList)
                {
                    pbimage = new PdfImage();
                    if (AppSettings.ShipmentCreatedFrom == "BATCH")
                    {
                        pbimage.FullPath = AppSettings.eCommerceUploadLabelFolder + eCommerceShipmentId + "/" + img;
                    }
                    else
                    {
                        pbimage.FullPath = HttpContext.Current.Server.MapPath(AppSettings.eCommerceLabelFolder) + eCommerceShipmentId + "/" + img;
                    }
                    imglist.Add(pbimage);
                }

                string pdfPath = string.Empty;

                if (eCommerceBookingDetail != null)
                {
                    string CourierCompany = eCommerceBookingDetail.CourierCompany;
                    string TrackingNo = new eCommerceShipmentRepository().GetTrackingNo(eCommerceShipmentId);
                    if (!string.IsNullOrEmpty(CourierCompany))
                    {
                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                        {
                            pdfPath = AppSettings.eCommerceUploadLabelFolder + eCommerceShipmentId + "/" + LogisticLabel;
                        }
                        else
                        {
                            pdfPath = HttpContext.Current.Server.MapPath("~/PackageLabel/eCommerce/" + eCommerceShipmentId + "/") + LogisticLabel;
                        }
                    }
                    else
                    {
                        if (AppSettings.ShipmentCreatedFrom == "BATCH")
                        {
                            pdfPath = AppSettings.eCommerceUploadLabelFolder + eCommerceShipmentId + "/" + LogisticLabel;
                        }
                        else
                        {
                            pdfPath = HttpContext.Current.Server.MapPath("~/PackageLabel/eCommerce/" + eCommerceShipmentId + "/") + LogisticLabel;
                        }
                    }
                }
                else
                {
                    string[] ff = imgList[0].ToString().Split('.');
                    string name = ff[0].ToString();
                    if (AppSettings.ShipmentCreatedFrom == "BATCH")
                    {
                        pdfPath = AppSettings.eCommerceUploadLabelFolder + eCommerceShipmentId + "/" + name + ".pdf";
                    }
                    else
                    {
                        pdfPath = HttpContext.Current.Server.MapPath(AppSettings.eCommerceLabelFolder) + eCommerceShipmentId + "/" + name + ".pdf";
                    }
                }
                if (File.Exists(pdfPath))
                {
                    File.Delete(pdfPath);
                }
                if (AppSettings.ShipmentCreatedFrom == "BATCH")
                {
                    if (!System.IO.Directory.Exists(AppSettings.eCommerceUploadLabelFolder + "/" + eCommerceShipmentId))
                        System.IO.Directory.CreateDirectory(AppSettings.eCommerceUploadLabelFolder + "/" + eCommerceShipmentId);
                }
                else
                {
                    if (!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath(AppSettings.eCommerceLabelFolder) + "/" + eCommerceShipmentId))
                        System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.eCommerceLabelFolder) + "/" + eCommerceShipmentId);
                }
                if (CourierName == FrayteCourierCompany.TNT)
                {
                    ReportTemplate.Other.TNTImagesToPDFReport report = new ReportTemplate.Other.TNTImagesToPDFReport();
                    report.DataSource = imglist;
                    DevExpress.XtraPrinting.PdfExportOptions pdfOptions = report.ExportOptions.Pdf;
                    pdfOptions.ImageQuality = DevExpress.XtraPrinting.PdfJpegImageQuality.Highest;
                    pdfOptions.PdfACompatibility = DevExpress.XtraPrinting.PdfACompatibility.PdfA2b;
                    report.ExportToPdf(pdfPath, pdfOptions);
                    result.Status = true;
                }
                else if (CourierName.Contains(FrayteCourierCompany.DHL))
                {
                    ReportTemplate.Other.DHLImageToPDFReport dhlReport = new ReportTemplate.Other.DHLImageToPDFReport();
                    dhlReport.DataSource = imglist;
                    DevExpress.XtraPrinting.PdfExportOptions pdfOptions = dhlReport.ExportOptions.Pdf;
                    pdfOptions.ImageQuality = DevExpress.XtraPrinting.PdfJpegImageQuality.Highest;
                    pdfOptions.PdfACompatibility = DevExpress.XtraPrinting.PdfACompatibility.PdfA2b;
                    dhlReport.ExportToPdf(pdfPath, pdfOptions);
                    result.Status = true;
                }
                else
                {
                    ReportTemplate.Other.PackageLabelsPdfReport pbreport = new ReportTemplate.Other.PackageLabelsPdfReport();
                    pbreport.DataSource = imglist;
                    DevExpress.XtraPrinting.PdfExportOptions pdfOptions = pbreport.ExportOptions.Pdf;
                    pdfOptions.ImageQuality = DevExpress.XtraPrinting.PdfJpegImageQuality.Highest;
                    pdfOptions.PdfACompatibility = DevExpress.XtraPrinting.PdfACompatibility.PdfA2b;
                    pbreport.ExportToPdf(pdfPath, pdfOptions);
                    result.Status = true;
                }


                return result;
            }
            catch (Exception ex)
            {
                result.Status = false;

                return result;
            }
        }

        public FrayteResult GenerateECommerceLabelUploadShipmentReport(int eCommerceShipmentId, FrayteUploadshipment eCommerceBookingDetail, List<string> imgList, string CourierName)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                List<PdfImage> imglist = new List<PdfImage>();

                PdfImage pbimage;
                foreach (var img in imgList)
                {
                    pbimage = new PdfImage();
                    pbimage.FullPath = HttpContext.Current.Server.MapPath(AppSettings.eCommerceLabelFolder) + eCommerceShipmentId + "/" + img;
                    imglist.Add(pbimage);
                }

                string pdfPath = string.Empty;
                if (eCommerceBookingDetail != null)
                {
                    string CourierCompany = eCommerceBookingDetail.CourierCompany;
                    string TrackingNo = new eCommerceShipmentRepository().GetTrackingNo(eCommerceShipmentId);
                    if (!string.IsNullOrEmpty(CourierCompany))
                    {
                        pdfPath = HttpContext.Current.Server.MapPath("~/PackageLabel/eCommerce/" + eCommerceShipmentId + "/") + CourierCompany.Replace(" ", "") + "-" + TrackingNo + ".pdf";
                    }
                    else
                    {
                        pdfPath = HttpContext.Current.Server.MapPath("~/PackageLabel/eCommerce/" + eCommerceShipmentId + "/") + CourierCompany.Replace(" ", "") + "-" + TrackingNo + ".pdf";
                    }
                }
                else
                {
                    string[] ff = imgList[0].ToString().Split('.');
                    string name = ff[0].ToString();
                    pdfPath = HttpContext.Current.Server.MapPath(AppSettings.eCommerceLabelFolder) + eCommerceShipmentId + "/" + name + ".pdf";

                }
                if (File.Exists(pdfPath))
                {
                    File.Delete(pdfPath);
                }
                if (!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath(AppSettings.eCommerceLabelFolder) + "/" + eCommerceShipmentId))
                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.eCommerceLabelFolder) + "/" + eCommerceShipmentId);

                if (CourierName == FrayteCourierCompany.TNT)
                {
                    ReportTemplate.Other.TNTImagesToPDFReport report = new ReportTemplate.Other.TNTImagesToPDFReport();
                    report.DataSource = imglist;
                    DevExpress.XtraPrinting.PdfExportOptions pdfOptions = report.ExportOptions.Pdf;
                    pdfOptions.ImageQuality = DevExpress.XtraPrinting.PdfJpegImageQuality.Highest;
                    pdfOptions.PdfACompatibility = DevExpress.XtraPrinting.PdfACompatibility.PdfA2b;
                    report.ExportToPdf(pdfPath, pdfOptions);
                    result.Status = true;
                }
                else if (CourierName.Contains(FrayteCourierCompany.DHL))
                {
                    ReportTemplate.Other.DHLImageToPDFReport dhlReport = new ReportTemplate.Other.DHLImageToPDFReport();
                    dhlReport.DataSource = imglist;
                    DevExpress.XtraPrinting.PdfExportOptions pdfOptions = dhlReport.ExportOptions.Pdf;
                    pdfOptions.ImageQuality = DevExpress.XtraPrinting.PdfJpegImageQuality.Highest;
                    pdfOptions.PdfACompatibility = DevExpress.XtraPrinting.PdfACompatibility.PdfA2b;
                    dhlReport.ExportToPdf(pdfPath, pdfOptions);
                    result.Status = true;
                }
                else
                {
                    ReportTemplate.Other.PackageLabelsPdfReport pbreport = new ReportTemplate.Other.PackageLabelsPdfReport();
                    pbreport.DataSource = imglist;
                    DevExpress.XtraPrinting.PdfExportOptions pdfOptions = pbreport.ExportOptions.Pdf;
                    pdfOptions.ImageQuality = DevExpress.XtraPrinting.PdfJpegImageQuality.Highest;
                    pdfOptions.PdfACompatibility = DevExpress.XtraPrinting.PdfACompatibility.PdfA2b;
                    pbreport.ExportToPdf(pdfPath, pdfOptions);
                    result.Status = true;
                }


                return result;
            }
            catch (Exception ex)
            {
                result.Status = false;

                return result;
            }
        }
    }
}
