using Frayte.Services.Business;
using Frayte.Services.Models;
using Frayte.Services.Utility;
using Frayte.Services.Models.Tradelane;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Report.Generator;
using System.Web;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.UI;
using System.IO;
using DevExpress.XtraPrinting;
using Frayte.Services.Models.Express;


namespace Report.Generator.ManifestReport
{
    public class TradelaneDocument
    {
        public TradelaneFile ShipmentDetails(int tradelaneShipmentId)
        {
            TradelaneFile result = new TradelaneFile();
            TradelaneBooking Shipment = new TradelaneBookingRepository().GetTradelaneBookingDetails(tradelaneShipmentId, "");

            List<HAWBTradelanePackage> _package = new List<HAWBTradelanePackage>();
            HAWBTradelanePackage hawbpackage;
            foreach (HAWBTradelanePackage trade in Shipment.HAWBPackages)
            {
                hawbpackage = new HAWBTradelanePackage();
                hawbpackage.TradelaneShipmentId = trade.TradelaneShipmentId;
                hawbpackage.HAWB = trade.HAWB;
                hawbpackage.HAWBNumber = trade.HAWBNumber;
                hawbpackage.TotalVolume = trade.TotalVolume;
                hawbpackage.TotalCartons = trade.TotalCartons;
                hawbpackage.EstimatedWeight = trade.EstimatedWeight;
                hawbpackage.TotalWeight = trade.TotalWeight;
                hawbpackage.PackagesCount = trade.PackagesCount;
                _package.Add(hawbpackage);
            }

            ReportTemplate.Tradelane.ShipmentDetail report = new ReportTemplate.Tradelane.ShipmentDetail();

            string mawbno, fromemail, fromphone, frompostcode, toemail, tophone, topostcode, notifyemail, notifyphone, notifypostcode;
            if (Shipment.MAWB == null || Shipment.MAWB == "")
            {
                mawbno = string.Empty;
            }
            else
            {
                mawbno = "(" + Shipment.AirlinePreference.CarrierCode2 + " " + Shipment.AirlinePreference.AilineCode + " - " + Shipment.MAWB + ")";
            }

            if (Shipment.ShipFrom.Email == null || Shipment.ShipFrom.Email == "")
            {
                fromemail = string.Empty;
            }
            else
            {
                fromemail = "Email: " + Shipment.ShipFrom.Email;
            }

            if (Shipment.ShipFrom.Phone == null || Shipment.ShipFrom.Phone == "")
            {
                fromphone = string.Empty;
            }
            else
            {
                fromphone = "Telephone No.: " + "(+" + Shipment.ShipFrom.Country.CountryPhoneCode + ")" + Shipment.ShipFrom.Phone;
            }

            if (Shipment.ShipFrom.PostCode == null || Shipment.ShipFrom.PostCode == "")
            {
                frompostcode = string.Empty;
            }
            else
            {
                frompostcode = " - " + Shipment.ShipFrom.PostCode;
            }

            if (Shipment.ShipTo.Email == null || Shipment.ShipTo.Email == "")
            {
                toemail = string.Empty;
            }
            else
            {
                toemail = "Email: " + Shipment.ShipTo.Email;
            }

            if (Shipment.ShipTo.Phone == null || Shipment.ShipTo.Phone == "")
            {
                tophone = string.Empty;
            }
            else
            {
                tophone = "Telephone No.: " + "(+" + Shipment.ShipTo.Country.CountryPhoneCode + ")" + Shipment.ShipTo.Phone;
            }

            if (Shipment.ShipTo.PostCode == null || Shipment.ShipTo.PostCode == "")
            {
                topostcode = string.Empty;
            }
            else
            {
                topostcode = " - " + Shipment.ShipTo.PostCode;
            }

            if (Shipment.NotifyParty.Email == null || Shipment.NotifyParty.Email == "")
            {
                notifyemail = string.Empty;
            }
            else
            {
                notifyemail = "Email: " + Shipment.NotifyParty.Email;
            }

            if (Shipment.NotifyParty.Phone == null || Shipment.NotifyParty.Phone == "")
            {
                notifyphone = string.Empty;
            }
            else
            {
                notifyphone = "Telephone No.: " + "(+" + Shipment.NotifyParty.Country.CountryPhoneCode + ")" + Shipment.NotifyParty.Phone;
            }

            if (Shipment.NotifyParty.PostCode == null || Shipment.NotifyParty.PostCode == "")
            {
                notifypostcode = string.Empty;
            }
            else
            {
                notifypostcode = " - " + Shipment.NotifyParty.PostCode;
            }

            report.Parameters["ShipmentHeading"].Value = Shipment.ShipmentHandlerMethod.ShipmentHandlerMethodCode + " - " + Shipment.DepartureAirport.AirportCode + " to " + Shipment.DestinationAirport.AirportCode + " " + mawbno;
            report.Parameters["ShipFrom"].Value = Shipment.ShipFrom.FirstName + " " + Shipment.ShipFrom.LastName + "\n" + Shipment.ShipFrom.CompanyName + "\n" + Shipment.ShipFrom.Address + "\n" + Shipment.ShipFrom.Address2 + "\n" + Shipment.ShipFrom.City + " " + Shipment.ShipFrom.State + frompostcode + "\n" + Shipment.ShipFrom.Country.Name + "\n" + "\n" + fromemail + "\n" + "\n" + fromphone;
            report.Parameters["ShipTo"].Value = Shipment.ShipTo.FirstName + " " + Shipment.ShipTo.LastName + "\n" + Shipment.ShipTo.CompanyName + "\n" + Shipment.ShipTo.Address + "\n" + Shipment.ShipTo.Address2 + "\n" + Shipment.ShipTo.City + " " + Shipment.ShipTo.State + topostcode + "\n" + Shipment.ShipTo.Country.Name + "\n" + "\n" + toemail + "\n" + "\n" + tophone;
            report.Parameters["NotifyParty"].Value = Shipment.IsNotifyPartySameAsReceiver ? Shipment.ShipTo.FirstName + " " + Shipment.ShipTo.LastName + "\n" + Shipment.ShipTo.CompanyName + "\n" + Shipment.ShipTo.Address + "\n" + Shipment.ShipTo.Address2 + "\n" + Shipment.ShipTo.City + " " + Shipment.ShipTo.State + topostcode + "\n" + Shipment.ShipTo.Country.Name + "\n" + "\n" + toemail + "\n" + "\n" + tophone : Shipment.NotifyParty.FirstName + " " + Shipment.NotifyParty.LastName + "\n" + Shipment.NotifyParty.CompanyName + "\n" + Shipment.NotifyParty.Address + "\n" + Shipment.NotifyParty.Address2 + "\n" + Shipment.NotifyParty.City + " " + Shipment.NotifyParty.State + notifypostcode + "\n" + Shipment.NotifyParty.Country.Name + "\n" + "\n" + notifyemail + "\n" + "\n" + notifyphone;
            report.Parameters["ShipmentHandler"].Value = Shipment.ShipmentHandlerMethod.ShipmentHandlerMethodDisplay;
            report.Parameters["AirlinePerfernce"].Value = Shipment.AirlinePreference.AirLineName;
            report.Parameters["DepartureAirport"].Value = Shipment.DepartureAirport.AirportName + " - " + Shipment.DepartureAirport.AirportCode;
            report.Parameters["DestinationAirport"].Value = Shipment.DestinationAirport.AirportName + " - " + Shipment.DestinationAirport.AirportCode;
            report.Parameters["CreatedOn"].Value = Shipment.CreatedOnUtc.ToString();
            report.Parameters["FrayteRef"].Value = Shipment.FrayteNumber;
            report.Parameters["PaymentParty"].Value = Shipment.PayTaxAndDuties;
            report.Parameters["DeclaredValue"].Value = Shipment.DeclaredValue.ToString() + " " + Shipment.DeclaredCurrency.CurrencyCode;
            report.Parameters["Description"].Value = Shipment.ShipmentDescription;

            report.DataSource = _package;

            PdfExportOptions options = new PdfExportOptions();
            options.ImageQuality = PdfJpegImageQuality.Highest;
            options.PdfACompatibility = PdfACompatibility.None;

            string fileName = "ShipmentDetail_" + Shipment.FrayteNumber + ".pdf";
            string filePath = AppSettings.WebApiPath + "/UploadFiles/Tradelane/" + tradelaneShipmentId + "/" + fileName;
            string filePhysicalPath = HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + tradelaneShipmentId + "/" + fileName);
            if (!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + tradelaneShipmentId + "/")))
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + tradelaneShipmentId + "/"));

            if (File.Exists(filePhysicalPath))
            {
                File.Delete(filePhysicalPath);
            }

            report.ExportToPdf(filePhysicalPath, options);

            result.FileName = fileName;
            result.FilePath = filePath;
            return result;
        }

        public TradelaneFile ShipmentHAWB(int tradelaneShipmentId, string documentTypeName)
        {
            TradelaneFile result = new TradelaneFile();
            List<TradelaneBookingReportHAWB> model = new TradelaneReportsRepository().GetHAWBObj(tradelaneShipmentId, documentTypeName);

            ReportTemplate.Tradelane.HAWB report = new ReportTemplate.Tradelane.HAWB();
            report.DataSource = model;

            string fileName = "HAWB_No_" + model[0].HAWB + ".pdf";
            model[0].HAWB = "HAWB No: " + model[0].HAWB;

            PdfExportOptions options = new PdfExportOptions();
            options.ImageQuality = PdfJpegImageQuality.Highest;
            options.PdfACompatibility = PdfACompatibility.None;

            string filePath = AppSettings.WebApiPath + "/UploadFiles/Tradelane/" + tradelaneShipmentId + "/" + fileName;
            string filePhysicalPath = HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + tradelaneShipmentId + "/" + fileName);
            if (!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + tradelaneShipmentId + "/")))
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + tradelaneShipmentId + "/"));

            if (File.Exists(filePhysicalPath))
            {
                File.Delete(filePhysicalPath);
            }

            report.ExportToPdf(filePhysicalPath, options);

            result.FileName = fileName;
            result.FilePath = filePath;
            return result;
        }

        public TradelaneFile ShipmentManifest(int tradelaneShipmentId, int userId)
        {
            TradelaneFile result = new TradelaneFile();
            List<TradelaneManifestReport> model = new TradelaneReportsRepository().ManifestReportModel(tradelaneShipmentId, userId);
            ReportTemplate.Tradelane.Manifest report = new ReportTemplate.Tradelane.Manifest();
            report.DataSource = model;

            PdfExportOptions options = new PdfExportOptions();
            options.ImageQuality = PdfJpegImageQuality.Highest;
            options.PdfACompatibility = PdfACompatibility.None;

            string fileName = "Summary_Manifest_" + (!string.IsNullOrEmpty(model[0].MAWB) ? model[0].AirlineCode + "-" + model[0].MAWB : model[0].FrayteNumber) + ".pdf";
            string filePath = AppSettings.WebApiPath + "/UploadFiles/Tradelane/" + tradelaneShipmentId + "/" + fileName;
            string filePhysicalPath = HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + tradelaneShipmentId + "/" + fileName);
            if (!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + tradelaneShipmentId + "/")))
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + tradelaneShipmentId + "/"));

            if (File.Exists(filePhysicalPath))
            {
                File.Delete(filePhysicalPath);
            }

            report.ExportToPdf(filePhysicalPath, options);

            result.FileName = fileName;
            result.FilePath = filePath;
            result.TradelaneShipmentId = tradelaneShipmentId;
            return result;
        }

        public TradelaneFile ShipmentCoLoadForm(int tradelaneShipmentId)
        {
            TradelaneFile result = new TradelaneFile();
            TradelaneBooking ShipmentDetail = new TradelaneBookingRepository().GetTradelaneBookingDetails(tradelaneShipmentId, "");
            List<TradelaneBookingCoLoadFormModel> ColoadModel = new TradelaneReportsRepository().ColoadReportObj(ShipmentDetail);
            ReportTemplate.Tradelane.CoLoadBookingForm report = new ReportTemplate.Tradelane.CoLoadBookingForm();
            if (ColoadModel[0].OperationZoneId == 1)
            {
                report.Parameters["lblWebSite"].Value = "WWW.FRAYTE.COM";
                report.Parameters["lblCompany"].Value = "FRAYTE LOGISTICS LTD";
                report.Parameters["lblAddress1"].Value = "UNIT 2306, 23/F, Trendy Center";
                report.Parameters["lblAddress2"].Value = "682-684 Castle Peak Road";
                report.Parameters["lblAddress3"].Value = "CHEUNG SHA WAN, HONG KONG";
            }
            else
            {
                report.Parameters["lblWebSite"].Value = "WWW.FRAYTE.CO.UK";
                report.Parameters["lblCompany"].Value = "FRAYTE LOGISTICS LTD";
                report.Parameters["lblAddress1"].Value = "UNIT 2306, 23/F, Trendy Center";
                report.Parameters["lblAddress2"].Value = "682-684 Castle Peak Road";
                report.Parameters["lblAddress3"].Value = "CHEUNG SHA WAN, HONG KONG";
            }
            report.DataSource = ColoadModel;

            PdfExportOptions options = new PdfExportOptions();
            options.ImageQuality = PdfJpegImageQuality.Highest;
            options.PdfACompatibility = PdfACompatibility.None;

            string fileName = "Co_Load_Booking_Form_for_" + (!string.IsNullOrEmpty(ColoadModel[0].MawbNo) ? ColoadModel[0].DestinationAirport + "_" + ColoadModel[0].MawbNo : ColoadModel[0].FrayteNumber) + ".pdf";
            string filePath = AppSettings.WebApiPath + "/UploadFiles/Tradelane/" + tradelaneShipmentId + "/" + fileName;
            string filePhysicalPath = HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + tradelaneShipmentId + "/");

            filePhysicalPath += fileName;

            if (!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + tradelaneShipmentId + "/")))
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + tradelaneShipmentId + "/"));


            if (File.Exists(filePhysicalPath))
            {
                File.Delete(filePhysicalPath);
            }

            report.ExportToPdf(filePhysicalPath, options);

            result.FileName = fileName;
            result.FilePath = filePath;
            return result;
        }

        public TradelaneFile ShipmentMAWB(int tradelaneShipmentId, string documentTypeName)
        {
            TradelaneFile result = new TradelaneFile();
            List<TradelaneBookingReportMAWB> model = new TradelaneReportsRepository().GetMAWBObj(tradelaneShipmentId);
            ReportTemplate.Tradelane.MAWB report = new Report.Generator.ReportTemplate.Tradelane.MAWB();
            report.DataSource = model;

            string fileName = "MAWB_" + model[0].MAWBWithCode + ".pdf";
            string filePath = AppSettings.WebApiPath + "/UploadFiles/Tradelane/" + tradelaneShipmentId + "/" + fileName;
            string filePhysicalPath = HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + tradelaneShipmentId + "/" + fileName);

            if (!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + tradelaneShipmentId + "/")))
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + tradelaneShipmentId + "/"));

            if (File.Exists(filePhysicalPath))
            {
                File.Delete(filePhysicalPath);
            }

            report.ExportToPdf(filePhysicalPath);

            result.FileName = fileName;
            result.FilePath = filePath;
            return result;
        }

        public TradelaneFile ShipmentCartonLabel(int tradelaneShipmentId, int CartonCount, int TradelaneShipmentDetailId, string Hawb)
        {

            TradelaneFile result = new TradelaneFile();

            var model = new TradelaneReportsRepository().GetCartonLabelObj(tradelaneShipmentId, Hawb);
            ReportTemplate.Tradelane.PackageLabel shipmentDetailReport = new Report.Generator.ReportTemplate.Tradelane.PackageLabel();
            model.FirstOrDefault().ScannedPieces = int.Parse(model.FirstOrDefault().TotalPieces);
            model.FirstOrDefault().TotalPieces = model.FirstOrDefault().HawbScannedCarton + "/" + model.FirstOrDefault().HAWBTotalPieces;
            shipmentDetailReport.DataSource = model;
            ImageExportOptions options = new ImageExportOptions();
            options.Resolution = 150;
            var Name = new TradelaneBookingRepository().GetLastScannedCarton(tradelaneShipmentId);
            string fileName = Name + ".jpeg";
            string filePath = AppSettings.WebApiPath + "/UploadFiles/Tradelane/" + tradelaneShipmentId + "/" + fileName;
            string filePhysicalPath = HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + tradelaneShipmentId + "/" + fileName);
            if (!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + tradelaneShipmentId + "/")))
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + tradelaneShipmentId + "/"));
            if (File.Exists(filePhysicalPath))
            {
                File.Delete(filePhysicalPath);
            }

            shipmentDetailReport.ExportToImage(filePhysicalPath, options);

            string resultPath = @"" + filePhysicalPath + "";
            using (System.Drawing.Image img = System.Drawing.Image.FromFile(resultPath))
            {
                img.RotateFlip(System.Drawing.RotateFlipType.Rotate90FlipNone);
                resultPath = @"" + filePhysicalPath + "";
                img.Save(resultPath, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            result.FileName = fileName;
            result.FilePath = filePath;
            return result;
        }

        public TradelaneFile ShipmentExportBagNumber(int bagId, int userId)
        {
            TradelaneFile result = new TradelaneFile();
            List<ExpressBagNumberReport> model = new ExpressReportRepository().GetBagNumberReportObj(bagId, userId);

            // List<TradelaneBookingReportMAWB> model = new TradelaneReportsRepository().GetMAWBObj(tradelaneShipmentId);
            ReportTemplate.Express.EXSBagNumber report = new ReportTemplate.Express.EXSBagNumber();
            report.DataSource = model;
            ImageExportOptions options = new ImageExportOptions();
            options.Resolution = 150;
            string fileName = "BGNo-" + "EXS-" + model[0].Ref + ".jpeg";
            int tradelaneShipmentId = model[0].TradelaneShipmentId;

            string filePath = AppSettings.WebApiPath + "/UploadFiles/ExpressBag/" + bagId + "/" + fileName;
            string filePhysicalPath = HttpContext.Current.Server.MapPath("~/UploadFiles/ExpressBag/" + bagId + "/" + fileName);

            if (!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~/UploadFiles/ExpressBag/" + bagId + "/")))
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/UploadFiles/ExpressBag/" + bagId + "/"));

            if (File.Exists(filePhysicalPath))
            {
                File.Delete(filePhysicalPath);
            }
            report.ExportToImage(filePhysicalPath, options);
            string resultPath = @"" + filePhysicalPath + "";
            using (System.Drawing.Image img = System.Drawing.Image.FromFile(resultPath))
            {
                img.RotateFlip(System.Drawing.RotateFlipType.Rotate90FlipNone);
                resultPath = @"" + filePhysicalPath + "";
                img.Save(resultPath, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            result.FileName = fileName;
            result.FilePath = filePath;
            result.TradelaneShipmentId = tradelaneShipmentId;
            return result;
        }

        public TradelaneFile ShipmentBagLabel(int BagId, int userId)
        {
            TradelaneFile result = new TradelaneFile();

            List<ExpressReportBagLabel> model = new ExpressReportRepository().GetBagLabelReportObj(BagId, userId);

            // List<TradelaneBookingReportMAWB> model = new TradelaneReportsRepository().GetMAWBObj(tradelaneShipmentId);
            ReportTemplate.Express.EXSBagLabel report = new ReportTemplate.Express.EXSBagLabel();
            report.DataSource = model;
            ImageExportOptions options = new ImageExportOptions();
            options.Resolution = 150;
            string fileName = "EXS-BGL-" + model[0].Hub + "-" + model[0].Ref + ".jpeg";
            //int tradelaneShipmentId = model[0].TradelaneShipmentId;

            string filePath = AppSettings.WebApiPath + "/UploadFiles/ExpressBag/" + BagId + "/" + fileName;
            string filePhysicalPath = HttpContext.Current.Server.MapPath("~/UploadFiles/ExpressBag/" + BagId + "/" + fileName);

            if (!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~/UploadFiles/ExpressBag/" + BagId + "/")))
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/UploadFiles/ExpressBag/" + BagId + "/"));

            if (File.Exists(filePhysicalPath))
            {
                File.Delete(filePhysicalPath);
            }
            report.ExportToImage(filePhysicalPath, options);

            string resultPath = @"" + filePhysicalPath + "";
            using (System.Drawing.Image img = System.Drawing.Image.FromFile(resultPath))
            {
                img.RotateFlip(System.Drawing.RotateFlipType.Rotate90FlipNone);
                resultPath = @"" + filePhysicalPath + "";
                img.Save(resultPath, System.Drawing.Imaging.ImageFormat.Jpeg);
            }

            result.FileName = fileName;
            result.FilePath = filePath;
            return result;
        }

        public TradelaneFile ShipmentDriverManifest(int tradelaneShipmentId, int userId)
        {
            TradelaneFile result = new TradelaneFile();

            List<ExpressReportDriverManifest> model = new ExpressReportRepository().GetDriverManifestReportObj(tradelaneShipmentId, userId);

            ReportTemplate.Express.DriverManifestExpressSolution report = new ReportTemplate.Express.DriverManifestExpressSolution();

            report.DataSource = model;
            string filePhysicalPath = string.Empty;
            string fileName = "Destination Manifest EXS" + "-" + "MN" + "-" + model[0].Hub + "-" + model[0].Ref + ".pdf";
            string filePath = AppSettings.WebApiPath + "/UploadFiles/Tradelane/" + tradelaneShipmentId + "/" + fileName;
            if (AppSettings.ShipmentCreatedFrom == "BATCH")
            {
                filePhysicalPath = AppSettings.UploadFolderPath + "/Tradelane/" + tradelaneShipmentId + "/" + fileName;
                if (!System.IO.Directory.Exists(AppSettings.UploadFolderPath + "/Tradelane/" + tradelaneShipmentId + "/"))
                    System.IO.Directory.CreateDirectory(AppSettings.UploadFolderPath + "/Tradelane/" + tradelaneShipmentId + "/");
            }
            else
            {
                filePhysicalPath = HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + tradelaneShipmentId + "/" + fileName);
                if (!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + tradelaneShipmentId + "/")))
                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + tradelaneShipmentId + "/"));
            }
            if (File.Exists(filePhysicalPath))
            {
                File.Delete(filePhysicalPath);
            }
            report.ExportToPdf(filePhysicalPath);
            result.FileName = fileName;
            result.FilePath = filePath;
            return result;
        }

        public TradelaneFile ShipmentExportManifest(int tradelaneShipmentId)
        {
            TradelaneFile result = new TradelaneFile();
            List<ExportManifestPdfModel> ModelList = new List<ExportManifestPdfModel>();
            ExportManifestPdfModel models = new ExpressReportRepository().GetExportManifestPDFDataSource(tradelaneShipmentId);
            ModelList.Add(models);
            ReportTemplate.Express.ExportManifestExpressSolution report = new ReportTemplate.Express.ExportManifestExpressSolution();
            report.DataSource = ModelList;

            string fileName = models.ExportManifestName + ".pdf";
            string filePath = AppSettings.WebApiPath + "/UploadFiles/Tradelane/" + tradelaneShipmentId + "/" + fileName;
            string filePhysicalPath = HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + tradelaneShipmentId + "/" + fileName);

            if (!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + tradelaneShipmentId + "/")))
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + tradelaneShipmentId + "/"));

            if (File.Exists(filePhysicalPath))
            {
                File.Delete(filePhysicalPath);
            }

            report.ExportToPdf(filePhysicalPath);

            result.FileName = fileName;
            result.FilePath = filePath;
            return result;
        }

        internal bool ThumbnailCallback()
        {
            return true;
        }

        #region AvinashCode

        public TradelaneFile HAWBLabel(string HAWB, int index, int TradelaneShipmentDetailId)
        {
            TradelaneFile result = new TradelaneFile();
            List<HawbLabel> _model = new List<HawbLabel>();

            HawbLabel model = new TradelaneBookingRepository().CreateHawbLabels(HAWB, index, TradelaneShipmentDetailId);
            _model.Add(model);

            ReportTemplate.Tradelane.HAWBLabel report = new ReportTemplate.Tradelane.HAWBLabel();
            report.DataSource = _model;

            string fileName = "HAWB_No_" + model.HAWB + ".pdf";

            PdfExportOptions options = new PdfExportOptions();
            options.ImageQuality = PdfJpegImageQuality.Highest;
            options.PdfACompatibility = PdfACompatibility.None;

            string filePath = AppSettings.WebApiPath + "/UploadFiles/Tradelane/" + model.TradelaneShipmentId + "/" + fileName;
            string filePhysicalPath = HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + model.TradelaneShipmentId + "/" + fileName);
            if (!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + model.TradelaneShipmentId + "/")))
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + model.TradelaneShipmentId + "/"));

            if (File.Exists(filePhysicalPath))
            {
                File.Delete(filePhysicalPath);
            }

            report.ExportToPdf(filePhysicalPath, options);

            result.FileName = fileName;
            result.FilePath = filePath;
            return result;
        }

        public TradelaneFile TadelaneShipmentDriverManifest(int TradelaneShipmentId, int userId)
        {
            TradelaneFile result = new TradelaneFile();

            List<ExpressReportDriverManifest> model = new TradelaneBookingRepository().TradeLaneDriverManifest(TradelaneShipmentId, userId);

            ReportTemplate.Tradelane.DriverManifestExpressSolution report = new ReportTemplate.Tradelane.DriverManifestExpressSolution();

            report.DataSource = model;
            string filePhysicalPath = string.Empty;
            string fileName = "Destination Manifest TLN" + "-" + "MN" + "-" + model[0].Code + "-" + model[0].MAWB + ".pdf";
            string filePath = AppSettings.WebApiPath + "/UploadFiles/Tradelane/" + TradelaneShipmentId + "/" + fileName;
            if (AppSettings.ShipmentCreatedFrom == "BATCH")
            {
                filePhysicalPath = AppSettings.UploadFolderPath + "/Tradelane/" + TradelaneShipmentId + "/" + fileName;
                if (!System.IO.Directory.Exists(AppSettings.UploadFolderPath + "/Tradelane/" + TradelaneShipmentId + "/"))
                    System.IO.Directory.CreateDirectory(AppSettings.UploadFolderPath + "/Tradelane/" + TradelaneShipmentId + "/");
            }
            else
            {
                filePhysicalPath = HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + TradelaneShipmentId + "/" + fileName);
                if (!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + TradelaneShipmentId + "/")))
                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + TradelaneShipmentId + "/"));
            }
            if (File.Exists(filePhysicalPath))
            {
                File.Delete(filePhysicalPath);
            }
            report.ExportToPdf(filePhysicalPath);
            result.FileName = fileName;
            result.FilePath = filePath;
            return result;
        }

        #endregion
    }
}
