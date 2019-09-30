using System;
using Frayte.Services.Business;
using System.Collections.Generic;

using Report.Generator.ManifestReport;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frayte.Services.Models;

using System.Net.Http;
using Frayte.Services.Utility;
using System.Web;
using System.IO;
using RazorEngine;
using RazorEngine.Templating;
using XStreamline.Log;
using Frayte.Services.Models.UPS;

namespace FrayteUploadShipmentSchedular
{
    class Program
    {
        static void Main(string[] args)
        {
            string xsUserFolder = @"C:\FMS\" + "FrayteSchedularlog.txt";
            //string xsUserFolder = @"D:\ProjectFrayte\" + "FrayteScheduler.txt";
            BaseLog.Instance.SetLogFile(xsUserFolder);
            Logger _log = Get_Log();
            _log.Error("Enter in Batch");
            string shipmentIds = args[0];
            //string shipmentIds = "3179,0_DirectBooking";
            var ShipmentIdType = shipmentIds.Split('_');
            string ShipmentType = ShipmentIdType[1];
            _log.Error("This is Vikshit");
            int CustId = 0;
            int count = 1;

            if (ShipmentType == "ECommerce")
            {
                _log.Error("1");
                var Shipments = new eCommerceUploadShipmentRepository().SaveShipmentWithService(ShipmentIdType[0]);
                _log.Error("2");
                new eCommerceUploadShipmentRepository().RemoveBatchProcessShipment(Shipments[0].CustomerId, Shipments.Count);
                _log.Error("3");
                foreach (var shipment in Shipments)
                {
                    try
                    {
                        _log.Error("4");
                        //Insert values to directshipmentdraftModel
                        var GetShipmentInDraftModel = new eCommerceUploadShipmentRepository().eCommerceShipmentDraft(shipment, shipment.CustomerId);
                        _log.Error("5");
                        //Step 2: Shipment Integrations
                        CustId = shipment.CustomerId;
                        //EasyPost Integration here
                        _log.Error("6");
                        List<FrayteCommercePackageTrackingDetail> shipmentPackageTrackingDetail = new List<FrayteCommercePackageTrackingDetail>();
                        EasyPostResult EPR = new EasyPostResult();
                        EPR.Errors = new FratyteError();
                        _log.Error("6.2");
                        var res1 = new EasyPostRepository().MapUploadShipmentToEasyPost(shipment);
                        _log.Error("6.3");
                        //shipment.Error = EasyPostIntegration(shipment, out shipmentPackageTrackingDetail, out eCommerceShipmentId);
                        var easyPostResult = new EasyPostRepository().CreateShipment(res1);
                        _log.Error("6.4");
                        shipment.Error = easyPostResult.Errors;

                        try
                        {
                            if (shipment.Error.Status)
                            {

                            }
                        }
                        catch (Exception ex2)
                        {
                            _log.Error("7");
                            shipment.Error.Miscellaneous = new List<string>();
                            shipment.Error.Miscellaneous.Add(ex2.Message);
                            //Elmah.ErrorSignal.FromCurrentContext().Raise(ex2);
                            shipment.Error.Status = false;
                        }
                        if (easyPostResult != null && easyPostResult.Errors != null && easyPostResult.Errors.Status)
                        {
                            //Finally Save the Order Id too to get the information of multiple shpment in Tracking Detail page.
                            _log.Error("8");
                            var eCommerceShipment = new eCommerceUploadShipmentRepository().SaveOrderNumber(shipment, easyPostResult.Order.id, shipment.CustomerId);
                            GetShipmentInDraftModel.FrayteNumber = eCommerceShipment.Item2;

                            if (eCommerceShipment.Item1 > 0)
                            {
                                _log.Error("9");
                                var Id = new eCommerceUploadShipmentRepository().SaveBatchProcessProcessedShipment(count, shipment.CustomerId);

                            }
                            var easypostImage = new eCommerceShipmentRepository().DownloadEasyPostImages(easyPostResult.Order.shipments[0].tracking_code, easyPostResult.Order.shipments[0].postage_label.label_url, eCommerceShipment.Item1);
                            _log.Error("10");
                            // EasyPost label
                            string labelPath = AppSettings.eCommerceUploadLabelFolder + eCommerceShipment.Item1 + "/";

                            int calculatedHeight = 1400;
                            int LocationY = 0;

                            //After creating an order need to save the information in database
                            _log.Error("11");
                            var shipmentDetails = new eCommerceShipmentRepository().GetPackageDetails(eCommerceShipment.Item1);
                            int increment = 0;

                            int sum = GetShipmentInDraftModel.Packages.Sum(p => p.CartoonValue);
                            Program aa = new Program();
                            for (int j = 0; j < shipment.Package.Count; j++)
                            {
                                for (int i = 0; i < shipment.Package[j].CartoonValue; i++)
                                {
                                    increment++;
                                    var data = shipmentDetails[j];

                                    // Step 1 : database entry 
                                    var trackingDetail = new eCommerceShipmentRepository().SaveEasyPostDetailTrackingDeatil(GetShipmentInDraftModel, GetShipmentInDraftModel.Packages[j], data, easyPostResult.Order.shipments[0].tracking_code, easyPostResult.Order.shipments[0].postage_label.label_url, eCommerceShipment.Item1, increment);

                                    //  Step 2: Create frayte AWB label and save label name
                                    string awbLabelName = aa.generateAWBLabel(eCommerceShipment.Item1, GetShipmentInDraftModel, increment, GetShipmentInDraftModel.Packages[j]);
                                    new eCommerceShipmentRepository().SaveAWBLabelName(trackingDetail, awbLabelName + ".pdf");

                                    // Step 3 : crop image and save
                                    var res = new EasyPostRepository().CropeImage(trackingDetail, GetShipmentInDraftModel.CourierCompany, easypostImage, labelPath, calculatedHeight, LocationY, increment, sum);
                                    LocationY += calculatedHeight;

                                }
                            }
                            _log.Error("12");
                            // Generate Courier Label
                            aa.Generate_Seperate_PackageLabelPDF(eCommerceShipment.Item1, GetShipmentInDraftModel);

                            aa.generateAWBLabel(eCommerceShipment.Item1, GetShipmentInDraftModel, 0, null);
                            // Send SMS to consignee
                            string message = string.Empty;

                            //Start downloading the images from EasyPost server

                            if (shipment.Error.IsMailSend)
                            {
                                //Send mail to developer
                                // new ShipmentEmailRepository().SendeCommerceShipmentErrorMail(eCommerceBookingDetail, eCommerceBookingDetail.Error);
                            }
                            if (shipment.Error.Status)
                            {
                                //SendDirectBookingConfirmationMailAsync(directBookingDetail);
                                //new ShipmentEmailRepository().SendeCommercreBookingConfirmationMail(GetShipmentInDraftModel, eCommerceShipmentId);
                            }
                            FrayteDirectShipment shipmentdetail = new eCommerceShipmentRepository().GetShipmentDetail(eCommerceShipment.Item1);
                            if (shipmentdetail != null)
                            {
                                _log.Error("13");
                                GetShipmentInDraftModel.DirectShipmentDraftId = shipmentdetail.DirectShipmentId;
                                GetShipmentInDraftModel.ShipFrom.DirectShipmentAddressDraftId = shipmentdetail.FromAddressId;
                                GetShipmentInDraftModel.ShipTo.DirectShipmentAddressDraftId = shipmentdetail.ToAddressId;
                                GetShipmentInDraftModel.ShipmentStatusId = shipmentdetail.ShipmentStatusId;
                            }
                        }
                        //count++;
                    }
                    catch (Exception e)
                    {
                        //new ShipmentEmailRepository().SendShipmentErrorMail(e.Message);
                        new eCommerceUploadShipmentRepository().SaveBatchProcessUnprocessedShipment(count, CustId);
                        new eCommerceUploadShipmentRepository().SelectServiceShipmentStatus(shipment);
                        _log.Error(e.Message);
                        _log.Error("14");
                        continue;
                    }
                }
            }
            else if (ShipmentType == "DirectBooking")
            {
                var dBCustId = 0;
                try
                {
                    _log.Error("Enter in Direct Booking");
                    //var CourierAccount = "ParcelHub";
                    var Shipments = new DirectBookingUploadShipmentRepository().SaveShipmentWithService(ShipmentIdType[0]);
                    Program aa = new Program();
                    dBCustId = Shipments[0].CustomerId;

                    //new eCommerceUploadShipmentRepository().RemoveBatchProcessShipment(Shipments[0].CustomerId, Shipments.Count);

                    foreach (var res in Shipments)
                    {
                        try
                        {
                            _log.Error("Enter in ForEachLoop in directBooking" + res.CustomerRateCard.CourierName.ToString());

                            _log.Error(res.CurrencyCode + " " + Convert.ToString(res.CustomerRateCard.LogisticDescription));
                            if (res.CustomerRateCard.CourierName == "UPS")
                            {
                                var Result = new UploadShipmentBatchRepository().DBUploadShipmentUpsIntegration(res);
                                //Step:4 Generate PDF
                                aa.DB_Generate_Seperate_PackageLabelPDF(Result.Item3, Result.Item2);

                                if (Result.Item2.Error.Status)
                                {
                                    // Register Shipment tracking in aftership
                                    if (Result.Item2.CustomerRateCard.CourierName == FrayteCourierCompany.DHL || Result.Item2.CustomerRateCard.CourierName == FrayteCourierCompany.UPS || Result.Item2.CustomerRateCard.CourierName == FrayteCourierCompany.TNT)
                                    {
                                        var aftershipTracking = new AftershipTrackingRepository().MapDirectShipmentObjToAfterShip(Result.Item3, FrayteShipmentServiceType.DirectBooking);
                                        if (aftershipTracking != null)
                                        {
                                            //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Integration in aftership"));
                                            new AftershipTrackingRepository().CreateTracking(aftershipTracking);
                                        }
                                    }
                                    //SendDirectBookingConfirmationMailAsync(directBookingDetail);
                                    new ShipmentEmailRepository().SendDirectBookingConfirmationMail(Result.Item2, Result.Item3);
                                }
                            }
                            else if (res.CustomerRateCard.CourierName == "TNT")
                            {
                                if (res.ShipFrom.Country.Code == "HKG")
                                {
                                    res.ShipFrom.PostCode = "";
                                }
                                _log.Error("entered in else if");
                                var Result = new UploadShipmentBatchRepository().DBUploadShipmentTNTIntegration(res);

                                // check if label is generated
                                if (!string.IsNullOrEmpty(Result.Item4.TNTShipmentResponse.ConnoteReply))
                                {
                                    //Generate LableHtml
                                    string labelHtml = string.Empty;
                                    labelHtml = new TNTRepository().GenerateLabelHtml(Result.Item4);

                                    //var reportResult = new Report.Generator.ManifestReport.TNTHtmlToImage().HtmlToImage(Result.Item3, labelHtml, Result.Item4.TNTShipmentResponse.TrackingCode);
                                    var reportResult = new Report.Generator.ManifestReport.TNTHtmlToImage().TNTImageFromHtml(Result.Item3, labelHtml, Result.Item4.TNTShipmentResponse.TrackingCode);
                                    if (!string.IsNullOrEmpty(reportResult))
                                    {
                                        string lablPath = string.Empty;
                                        if (AppSettings.LabelSave == "")
                                        {
                                            lablPath = AppSettings.WebApiPath + "/PackageLabel/" + Result.Item3 + "/";
                                        }
                                        else
                                        {
                                            lablPath = AppSettings.WebApiPath + "/PackageLabel/" + Result.Item3 + "/";
                                        }

                                        //Step: Rotate TNT Image
                                        new TNTRepository().FlipTNTImage(reportResult, Result.Item5.Count);

                                        //Step : Split Image

                                        int calculatedHeight = 1120;

                                        // for trial version
                                        //int LocationY = 100;

                                        // for licenced version
                                        int LocationY = 8;

                                        for (int i = 0; i < Result.Item5.Count; i++)
                                        {
                                            string Image = FrayteShortName.TNT + "_" + Result.Item4.TNTShipmentResponse.TrackingCode + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (" + (i + 1) + "of" + Result.Item5.Count + ")" + ".jpg";
                                            new TNTRepository().CropImage(1590, calculatedHeight, LocationY, reportResult, lablPath + Image, Result.Item5.Count);
                                            LocationY += calculatedHeight - 3;

                                            //save image name in package tracking detail table
                                            try
                                            {
                                                new DirectShipmentRepository().SaveImage(Result.Item5[i], Image);
                                                Result.Item5[i].PackageImage = Image;
                                            }
                                            catch (Exception ex)
                                            {

                                            }
                                        }
                                    }
                                    Result.Item4.Error.Status = true;
                                }
                                else
                                {
                                    Result.Item4.Error.Status = false;
                                    var er = new FrayteKeyValue();
                                    er.Key = "Misscllaneous Errors";
                                    er.Value = new List<string>() { "Something bad happen please try again later." };
                                    Result.Item4.Error.MiscErrors.Add(er);
                                }
                                aa.DB_Generate_Seperate_PackageLabelPDF(Result.Item3, Result.Item2);
                                new TNTRepository().SaveTempShipmentXML(Result.Item4, Result.Item3);
                                if (Result.Item2.Error.Status)
                                {
                                    // Register Shipment tracking in aftership
                                    if (Result.Item2.CustomerRateCard.CourierName == FrayteCourierCompany.DHL || Result.Item2.CustomerRateCard.CourierName == FrayteCourierCompany.UPS || Result.Item2.CustomerRateCard.CourierName == FrayteCourierCompany.TNT)
                                    {
                                        var aftershipTracking = new AftershipTrackingRepository().MapDirectShipmentObjToAfterShip(Result.Item3, FrayteShipmentServiceType.DirectBooking);
                                        if (aftershipTracking != null)
                                        {
                                            //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Integration in aftership"));
                                            new AftershipTrackingRepository().CreateTracking(aftershipTracking);
                                        }
                                    }
                                    //SendDirectBookingConfirmationMailAsync(directBookingDetail);
                                    new ShipmentEmailRepository().SendDirectBookingConfirmationMail(Result.Item2, Result.Item3);
                                }
                            }
                            else if (res.CustomerRateCard.CourierName == "DHL")
                            {
                                _log.Error("Enter in DHL" + res.CustomerRateCard.CourierName);
                                var Result = new UploadShipmentBatchRepository().DBUploadShipmentDHLIntegration(res);

                                aa.DB_Generate_Seperate_PackageLabelPDF(Result.Item3, Result.Item2);
                                if (Result.Item2.Error.Status)
                                {
                                    // Register Shipment tracking in aftership
                                    if (Result.Item2.CustomerRateCard.CourierName == FrayteCourierCompany.DHL || Result.Item2.CustomerRateCard.CourierName == FrayteCourierCompany.UPS || Result.Item2.CustomerRateCard.CourierName == FrayteCourierCompany.TNT)
                                    {
                                        var aftershipTracking = new AftershipTrackingRepository().MapDirectShipmentObjToAfterShip(Result.Item3, FrayteShipmentServiceType.DirectBooking);
                                        if (aftershipTracking != null)
                                        {
                                            //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Integration in aftership"));
                                            new AftershipTrackingRepository().CreateTracking(aftershipTracking);
                                        }
                                    }
                                    //SendDirectBookingConfirmationMailAsync(directBookingDetail);
                                    new ShipmentEmailRepository().SendDirectBookingConfirmationMail(Result.Item2, Result.Item3);
                                }
                                _log.Error("done in DHL" + Result.ToString());
                            }
                            else if (res.CustomerRateCard.CourierName == "UKMail" || res.CustomerRateCard.CourierName == "Yodel" || res.CustomerRateCard.CourierName == "Hermes")
                            {
                                var Result = new UploadShipmentBatchRepository().DBUploadShipmentParcelHubIntegration(res);

                                //Start Mking Final One pdf file for all package label
                                aa.DB_Generate_Seperate_PackageLabelPDF(Result.Item3, Result.Item2);
                            }



                        }
                        catch (Exception e)
                        {
                            //new ShipmentEmailRepository().SendShipmentErrorMail(e.Message);
                            new eCommerceUploadShipmentRepository().SaveBatchProcessUnprocessedShipment(count, dBCustId);
                            new eCommerceUploadShipmentRepository().SelectServiceShipmentStatus(res);
                            _log.Error(e.Message);
                            continue;
                        }
                    }
                }
                catch (Exception e)
                {

                    _log.Error(e.Message);
                }

            }

        }
        public static Logger Get_Log()
        {
            Logger log = BaseLog.Instance.GetLogger(null);
            return log;
        }


        //public void generateAWBLabel(int eCommerceShipmentId, FrayteUploadshipment eCommerceBookingDetail)
        //{
        //    FrayteeCommerceUploadShipmentLabelReport obj;
        //    int n = 0;

        //    foreach (var data in eCommerceBookingDetail.Package)
        //    {
        //        for (int i = 0; i < data.CartoonValue; i++)
        //        {
        //            n++;
        //            obj = new FrayteeCommerceUploadShipmentLabelReport();
        //            obj.CurrentShipment = n + " OF " + eCommerceBookingDetail.Package.Sum(p => p.CartoonValue);
        //            if (eCommerceBookingDetail.Package.Sum(p => p.Value).ToString().Contains("."))
        //            {
        //                obj.TotalValue = eCommerceBookingDetail.CurrencyCode + " " + eCommerceBookingDetail.Package.Sum(p => p.Value);
        //            }
        //            else
        //            {
        //                obj.TotalValue = eCommerceBookingDetail.CurrencyCode + " " + eCommerceBookingDetail.Package.Sum(p => p.Value) + ".00";
        //            }
        //            if (eCommerceBookingDetail.PackageCalculationType == FraytePakageCalculationType.kgtoCms)
        //            {
        //                if (UtilityRepository.GetShipmentChargeableWeightUploadShipment(eCommerceBookingDetail).ToString().Contains("."))
        //                {
        //                    obj.TotalChargeableWeight = UtilityRepository.GetShipmentChargeableWeightUploadShipment(eCommerceBookingDetail) + " " + WeightUOM.KG;
        //                }
        //                else
        //                {
        //                    obj.TotalChargeableWeight = UtilityRepository.GetShipmentChargeableWeightUploadShipment(eCommerceBookingDetail) + ".00" + " " + WeightUOM.KG;
        //                }
        //            }
        //            else if (eCommerceBookingDetail.PackageCalculationType == FraytePakageCalculationType.LbToInchs)
        //            {
        //                if (UtilityRepository.GetShipmentChargeableWeightUploadShipment(eCommerceBookingDetail).ToString().Contains("."))
        //                {
        //                    obj.TotalChargeableWeight = UtilityRepository.GetShipmentChargeableWeightUploadShipment(eCommerceBookingDetail) + " " + WeightUOM.LB;
        //                }
        //                else
        //                {
        //                    obj.TotalChargeableWeight = UtilityRepository.GetShipmentChargeableWeightUploadShipment(eCommerceBookingDetail) + ".00" + " " + WeightUOM.LB;
        //                }
        //            }
        //            else
        //            {
        //                if (UtilityRepository.GetShipmentChargeableWeightUploadShipment(eCommerceBookingDetail).ToString().Contains("."))
        //                {
        //                    obj.TotalChargeableWeight = UtilityRepository.GetShipmentChargeableWeightUploadShipment(eCommerceBookingDetail) + " " + WeightUOM.KG;
        //                }
        //                else
        //                {
        //                    obj.TotalChargeableWeight = UtilityRepository.GetShipmentChargeableWeightUploadShipment(eCommerceBookingDetail) + ".00" + " " + WeightUOM.KG;
        //                }
        //            }
        //            obj.eCommerceShipment = eCommerceBookingDetail;
        //            obj.BarcodePath = AppSettings.eCommerceUploadShipmentLabelFolder + eCommerceShipmentId + "/" + obj.eCommerceShipment.FrayteNumber + ".Png";
        //            obj.LabelPackages = new List<PackageLabel>();

        //            PackageLabel pL = new PackageLabel();
        //            pL.CartoonValue = 1;
        //            pL.Content = data.Content;
        //            pL.DirectShipmentDetailDraftId = data.DirectShipmentDetailDraftId;
        //            pL.Height = Convert.ToDecimal(data.Height);
        //            pL.Weight = Convert.ToDecimal(data.Weight);
        //            pL.Width = Convert.ToDecimal(data.Width);
        //            pL.Value = Convert.ToDecimal(data.Value);
        //            pL.TrackingNo = obj.eCommerceShipment.TrackingNo;
        //            pL.SerialNo = 1;
        //            pL.PackageTrackingDetailId = data.PackageTrackingDetailId;
        //            pL.LabelName = data.LabelName;
        //            pL.IsPrinted = data.IsPrinted;
        //            obj.LabelPackages.Add(pL);
        //            FrayteResult result = new eCommerceShipmentLabelReport().GeteCommerceUploadShipmentLabelReportDetail(eCommerceShipmentId, obj, n);
        //        }
        //    }
        //}


        //private string Generate_PackageLabelPDF(int eCommerceShipmentId)
        //{
        //    string pageStyleSheet = HttpContext.Current.Server.MapPath("~/Content/") + "print.css";
        //    string bootStrapStyleSheet = HttpContext.Current.Server.MapPath("~/Content/") + "bootstrap.min.css";
        //    string pdfLogo = HttpContext.Current.Server.MapPath("~/Content/") + "FrayteLogo.png";

        //    //   var detail = new DirectShipmentRepository().GetPackageList(eCommerceShipmentId, CourierCompany, RateType);
        //    var detail = new eCommerceShipmentRepository().GetPackageList(eCommerceShipmentId);
        //    string TrackingNo = new eCommerceShipmentRepository().GetTrackingNo(eCommerceShipmentId);

        //    FrayteShipmentPackageLabel obj = new FrayteShipmentPackageLabel();
        //    obj.PackageLabel = new List<FraytePackageLabel>();
        //    obj.PackageLabel = detail;
        //    obj.PageStyleSheet = pageStyleSheet;
        //    obj.BootStrapStyleSheet = bootStrapStyleSheet;
        //    obj.pdfLogo = pdfLogo;

        //    string template = File.ReadAllText(HttpContext.Current.Server.MapPath("~/Templates/PrintLabel.cshtml"));
        //    var templateService = new TemplateService();
        //    var EmailBody = Engine.Razor.RunCompile(template, "PackageLabel_", null, obj);

        //    string pdfFileName = string.Empty;
        //    pdfFileName = TrackingNo + ".html";

        //    string pdfFilePath = HttpContext.Current.Server.MapPath("~/UploadFiles/PDFGenerator/HTMLFile");
        //    string pdfFullPath = pdfFilePath + @"\" + pdfFileName;

        //    using (FileStream fs = new FileStream(pdfFullPath, FileMode.Create))
        //    {
        //        using (StreamWriter w = new StreamWriter(fs, System.Text.Encoding.UTF8))
        //        {
        //            w.WriteLine(EmailBody);
        //        }
        //    }

        //    List<string> lstHtmlFiles = new List<string>();
        //    lstHtmlFiles.Add(pdfFullPath);

        //    //Before creating new PDF file, remove the earlier one.            
        //    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.eCommerceLabelFolder + eCommerceShipmentId + "/"));
        //    string pdfPath = string.Empty;


        //    //    pdfPath = HttpContext.Current.Server.MapPath("~/PackageLabel/eCommerce/" + eCommerceShipmentId + "/") + TrackingNo + ".pdf";
        //    //"AWBUK-" + DateTime.UtcNow.Year.ToString().Substring(2, 2) + DateTime.UtcNow.ToString("MM") + GetFormattedManifestId(int eCommerceShipmentId)
        //    pdfPath = HttpContext.Current.Server.MapPath("~/PackageLabel/eCommerce/" + eCommerceShipmentId + "/") + "AWBUK-" + DateTime.UtcNow.Year.ToString().Substring(2, 2) + DateTime.UtcNow.ToString("MM") + GetFormattedManifestId(eCommerceShipmentId) + ".pdf";

        //    if (File.Exists(pdfPath))
        //    {
        //        File.Delete(pdfPath);
        //    }

        //    string pdfFile = string.Empty;
        //    pdfFile = Frayte.WebApi.Utility.PDFGenerator.HtmlToPdf("~/PackageLabel/eCommerce/" + eCommerceShipmentId + "/", "AWBUK-" + DateTime.UtcNow.Year.ToString().Substring(2, 2) + DateTime.UtcNow.ToString("MM") + GetFormattedManifestId(eCommerceShipmentId), lstHtmlFiles.ToArray(), null);
        //    if (System.IO.File.Exists(pdfFullPath))
        //    {
        //        System.IO.File.Delete(pdfFullPath);
        //    }

        //    return pdfFile;
        //}

        public FrayteResult Generate_Seperate_PackageLabelPDF(int eCommercShipmentId, FrayteCommerceShipmentDraft eCommerceBookingDetail)
        {
            FrayteResult result = new FrayteResult();
            result.Errors = new List<string>();

            List<string> list = new List<string>();
            List<string> list1 = new List<string>();

            var Packages = new eCommerceShipmentRepository().GetPackageDetails(eCommercShipmentId);
            if (Packages != null)
            {
                foreach (var pack in Packages)
                {
                    var packageTracking = new eCommerceShipmentRepository().GeteCommercePackageTracking(pack.eCommerceShipmentDetailId);
                    if (packageTracking != null && packageTracking.Count > 0)
                    {
                        foreach (var data in packageTracking)
                        {
                            list.Add(data.PackageImage);
                            list1.Add(data.PackageImage);
                            var resultReport = new Report.Generator.ManifestReport.PackageLabelReport().GenerateECommerceLabelReport(eCommercShipmentId, null, list1, eCommerceBookingDetail.CourierCompany, "");
                            if (!resultReport.Status)
                            {
                                result.Status = false;
                                result.Errors.Add("All the labels Pdf are not generated for " + eCommercShipmentId.ToString());
                            }
                            list1.Remove(data.PackageImage);
                        }
                    }
                    else
                    {
                        result.Status = false;
                        result.Errors.Add("All the labels Pdf are not generated for " + eCommercShipmentId.ToString());
                    }
                }

                string labelName = string.Empty;
                string LogisticLabel = string.Empty;
                var TrackingNo = new eCommerceShipmentRepository().GetTrackingNo(eCommercShipmentId);
                if (eCommerceBookingDetail.CourierCompany == FrayteCourierCompany.DHL || eCommerceBookingDetail.CourierCompany == FrayteCourierCompany.DHLExpress || eCommerceBookingDetail.CourierCompany == FrayteCourierCompany.DHL_Express)
                {
                    labelName = FrayteShortName.DHL;
                }

                LogisticLabel = labelName + "_" + TrackingNo + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (All)" + ".pdf";
                var Result = new Report.Generator.ManifestReport.PackageLabelReport().GenerateECommerceLabelReport(eCommercShipmentId, eCommerceBookingDetail, list, eCommerceBookingDetail.CourierCompany, LogisticLabel);
                if (Result.Status)
                    new eCommerceShipmentRepository().SaveFrayteLabel(eCommercShipmentId, LogisticLabel, eCommLabelType.CourierLabel);
            }
            else
            {
                result.Status = false;
                result.Errors.Add("All the labels Pdf are not generated for " + eCommercShipmentId.ToString());
            }
            return result;
        }

        public FrayteResult DB_Generate_Seperate_PackageLabelPDF(int DirectShipmentId, DirectBookingShipmentDraftDetail directBookingDetail)
        {
            FrayteResult result = new FrayteResult();
            result.Errors = new List<string>();

            List<string> list = new List<string>();
            List<string> list1 = new List<string>();

            var Packages = new DirectShipmentRepository().GetPackageDetails(DirectShipmentId);
            if (Packages != null)
            {
                foreach (var pack in Packages)
                {
                    var packageTracking = new DirectShipmentRepository().GetPackageTracking(pack.DirectShipmentDetailId);
                    if (packageTracking != null && packageTracking.Count > 0)
                    {
                        foreach (var data in packageTracking)
                        {
                            list.Add(data.PackageImage);
                            list1.Add(data.PackageImage);
                            var resultReport = new Report.Generator.ManifestReport.PackageLabelReport().GenerateAllLabelReport(DirectShipmentId, null, list1, directBookingDetail.CustomerRateCard.CourierName, "");
                            if (!resultReport.Status)
                            {
                                result.Status = false;
                                result.Errors.Add("All the labels Pdf are not generated for " + DirectShipmentId.ToString());
                            }
                            list1.Remove(data.PackageImage);
                        }
                    }
                    else
                    {
                        result.Status = false;
                        result.Errors.Add("All the labels Pdf are not generated for " + DirectShipmentId.ToString());
                    }
                }


                string RateType = directBookingDetail.CustomerRateCard.RateType;
                string CourierCompany = directBookingDetail.CustomerRateCard.DisplayName;
                string TrackingNo = new DirectShipmentRepository().GetTrackingNo(DirectShipmentId);
                string labelName = string.Empty;
                if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.Yodel)
                {
                    labelName = FrayteShortName.Yodel;
                }
                else if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.Hermes)
                {
                    labelName = FrayteShortName.Hermes;
                }
                else if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.UKMail)
                {
                    labelName = FrayteShortName.UKMail;
                }
                else if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.DHL)
                {
                    labelName = FrayteShortName.DHL;
                }
                else if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.TNT)
                {
                    labelName = FrayteShortName.TNT;
                }
                else if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.UPS)
                {
                    labelName = FrayteShortName.UPS;
                }
                string LogisticLabel = string.Empty;


                //if (!string.IsNullOrEmpty(directBookingDetail.CustomerRateCard.RateType))
                //{
                //    LogisticLabel = labelName + "_" + TrackingNo + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (All)" + ".pdf";
                //}
                //else
                //{
                //    LogisticLabel = labelName + "_" + TrackingNo + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (All)" + ".pdf";
                //}
                if (!string.IsNullOrEmpty(directBookingDetail.CustomerRateCard.RateType))
                {
                    if (TrackingNo.Contains("Order_"))
                    {
                        var Trackinginfo = TrackingNo.Replace("Order_", "");
                        LogisticLabel = labelName + "_" + Trackinginfo + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (All)" + ".pdf";
                    }
                    else
                    {
                        LogisticLabel = labelName + "_" + TrackingNo + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (All)" + ".pdf";
                    }
                }
                else
                {
                    if (TrackingNo.Contains("Order_"))
                    {
                        var Trackinginfo = TrackingNo.Replace("Order_", "");
                        LogisticLabel = labelName + "_" + Trackinginfo + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (All)" + ".pdf";
                    }
                    else
                    {
                        LogisticLabel = labelName + "_" + TrackingNo + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (All)" + ".pdf";
                    }
                }
                if (directBookingDetail.CustomerRateCard.CourierName == FrayteCourierCompany.DHL)
                {
                    var shipmentImange = new DirectShipmentRepository().GetShipmentImage(DirectShipmentId);
                    list.Add(shipmentImange.ShipmentImage);
                }
                var Result = new Report.Generator.ManifestReport.PackageLabelReport().GenerateAllLabelReport(DirectShipmentId, directBookingDetail, list, directBookingDetail.CustomerRateCard.CourierName, LogisticLabel);
                // After Creting label  save in DirectShipmentTable
                if (Result.Status)
                    new DirectShipmentRepository().SaveLogisticLabel(DirectShipmentId, LogisticLabel);
            }
            else
            {
                result.Status = false;
                result.Errors.Add("All the labels Pdf are not generated fo " + DirectShipmentId.ToString());
            }
            return result;
        }

        private string generateAWBLabel(int eCommerceShipmentId, FrayteCommerceShipmentDraft eCommerceBookingDetail, int i, PackageDraft data)
        {
            List<FrayteeCommerceShipmentLabelReport> list = new List<FrayteeCommerceShipmentLabelReport>();
            FrayteeCommerceShipmentLabelReport newObj;
            string filename = string.Empty;

            if (data != null)
            {
                FrayteeCommerceShipmentLabelReport obj;
                int n = i;
                obj = new FrayteeCommerceShipmentLabelReport();
                obj.CurrentShipment = n + " OF " + eCommerceBookingDetail.Packages.Sum(p => p.CartoonValue);
                if (eCommerceBookingDetail.Packages.Sum(p => p.Value).ToString().Contains("."))
                {
                    obj.TotalValue = eCommerceBookingDetail.Currency.CurrencyCode + " " + eCommerceBookingDetail.Packages.Sum(p => p.Value);
                }
                else
                {
                    obj.TotalValue = eCommerceBookingDetail.Currency.CurrencyCode + " " + eCommerceBookingDetail.Packages.Sum(p => p.Value) + ".00";
                }
                if (eCommerceBookingDetail.PakageCalculatonType == FraytePakageCalculationType.kgtoCms)
                {
                    if (UtilityRepository.GetShipmentChargeableWeight(eCommerceBookingDetail).ToString().Contains("."))
                    {
                        obj.TotalChargeableWeight = UtilityRepository.GetShipmentChargeableWeight(eCommerceBookingDetail) + " " + WeightUOM.KG;
                    }
                    else
                    {
                        obj.TotalChargeableWeight = UtilityRepository.GetShipmentChargeableWeight(eCommerceBookingDetail) + ".00" + " " + WeightUOM.KG;
                    }
                }
                else if (eCommerceBookingDetail.PakageCalculatonType == FraytePakageCalculationType.LbToInchs)
                {
                    if (UtilityRepository.GetShipmentChargeableWeight(eCommerceBookingDetail).ToString().Contains("."))
                    {
                        obj.TotalChargeableWeight = UtilityRepository.GetShipmentChargeableWeight(eCommerceBookingDetail) + " " + WeightUOM.LB;
                    }
                    else
                    {
                        obj.TotalChargeableWeight = UtilityRepository.GetShipmentChargeableWeight(eCommerceBookingDetail) + ".00" + " " + WeightUOM.LB;
                    }
                }
                else
                {
                    if (UtilityRepository.GetShipmentChargeableWeight(eCommerceBookingDetail).ToString().Contains("."))
                    {
                        obj.TotalChargeableWeight = UtilityRepository.GetShipmentChargeableWeight(eCommerceBookingDetail) + " " + WeightUOM.KG;
                    }
                    else
                    {
                        obj.TotalChargeableWeight = UtilityRepository.GetShipmentChargeableWeight(eCommerceBookingDetail) + ".00" + " " + WeightUOM.KG;
                    }
                }
                obj.eCommerceShipment = eCommerceBookingDetail;
                obj.BarcodePath = AppSettings.eCommerceUploadLabelFolder + eCommerceShipmentId + "/" + obj.eCommerceShipment.FrayteNumber + ".Png";
                obj.LabelPackages = new List<PackageLabel>();

                PackageLabel pL = new PackageLabel();
                if (data != null)
                {
                    pL.CartoonValue = 1;
                    pL.Content = data.Content;
                    pL.DirectShipmentDetailDraftId = data.DirectShipmentDetailDraftId;
                    pL.Height = data.Height;
                    pL.Weight = data.Weight;
                    pL.Width = data.Width;
                    pL.Value = data.Value;
                    pL.TrackingNo = data.TrackingNo;
                    pL.SerialNo = 1;
                    pL.PackageTrackingDetailId = data.PackageTrackingDetailId;
                    pL.LabelName = data.LabelName;
                    pL.IsPrinted = data.IsPrinted;
                    obj.LabelPackages.Add(pL);
                }

                int total = eCommerceBookingDetail.Packages.Sum(p => p.CartoonValue);
                filename = "FRT_AWB_" + eCommerceBookingDetail.FrayteNumber + DateTime.Now.ToString("dd_MM_yyyy") + " (" + n + "of" + total + ")";

                FrayteResult result = new eCommerceShipmentLabelReport().GeteCommerceShipmentLabelReportDetail(eCommerceShipmentId, obj, filename);

            }
            else
            {
                int j = 0;
                foreach (var package in eCommerceBookingDetail.Packages)
                {
                    for (int k = 0; k < package.CartoonValue; k++)
                    {
                        j++;
                        newObj = GenerateAwb(eCommerceShipmentId, package, eCommerceBookingDetail, j);
                        list.Add(newObj);
                    }
                }
                filename = "FRT" + "_" + "AWB_" + eCommerceBookingDetail.FrayteNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy") + " (All)";

                FrayteResult result = new eCommerceShipmentLabelReport().GeteCommerceShipmentLabelReportDetailList(eCommerceShipmentId, list, filename);
                if (result.Status)
                    new eCommerceShipmentRepository().SaveFrayteLabel(eCommerceShipmentId, filename + ".pdf", eCommLabelType.FrayteLabel);
            }

            return filename;
        }

        #region generate awb label

        private FrayteeCommerceShipmentLabelReport GenerateAwb(int eCommerceShipmentId, PackageDraft data, FrayteCommerceShipmentDraft eCommerceBookingDetail, int i)
        {
            FrayteeCommerceShipmentLabelReport obj = new FrayteeCommerceShipmentLabelReport();
            if (data != null)
            {


                int n = i;

                obj.CurrentShipment = n + " OF " + eCommerceBookingDetail.Packages.Sum(p => p.CartoonValue);
                if (eCommerceBookingDetail.Packages.Sum(p => p.Value).ToString().Contains("."))
                {
                    obj.TotalValue = eCommerceBookingDetail.Currency.CurrencyCode + " " + eCommerceBookingDetail.Packages.Sum(p => p.Value);
                }
                else
                {
                    obj.TotalValue = eCommerceBookingDetail.Currency.CurrencyCode + " " + eCommerceBookingDetail.Packages.Sum(p => p.Value) + ".00";
                }
                if (eCommerceBookingDetail.PakageCalculatonType == FraytePakageCalculationType.kgtoCms)
                {
                    if (UtilityRepository.GetShipmentChargeableWeight(eCommerceBookingDetail).ToString().Contains("."))
                    {
                        obj.TotalChargeableWeight = UtilityRepository.GetShipmentChargeableWeight(eCommerceBookingDetail) + " " + WeightUOM.KG;
                    }
                    else
                    {
                        obj.TotalChargeableWeight = UtilityRepository.GetShipmentChargeableWeight(eCommerceBookingDetail) + ".00" + " " + WeightUOM.KG;
                    }
                }
                else if (eCommerceBookingDetail.PakageCalculatonType == FraytePakageCalculationType.LbToInchs)
                {
                    if (UtilityRepository.GetShipmentChargeableWeight(eCommerceBookingDetail).ToString().Contains("."))
                    {
                        obj.TotalChargeableWeight = UtilityRepository.GetShipmentChargeableWeight(eCommerceBookingDetail) + " " + WeightUOM.LB;
                    }
                    else
                    {
                        obj.TotalChargeableWeight = UtilityRepository.GetShipmentChargeableWeight(eCommerceBookingDetail) + ".00" + " " + WeightUOM.LB;
                    }
                }
                else
                {
                    if (UtilityRepository.GetShipmentChargeableWeight(eCommerceBookingDetail).ToString().Contains("."))
                    {
                        obj.TotalChargeableWeight = UtilityRepository.GetShipmentChargeableWeight(eCommerceBookingDetail) + " " + WeightUOM.KG;
                    }
                    else
                    {
                        obj.TotalChargeableWeight = UtilityRepository.GetShipmentChargeableWeight(eCommerceBookingDetail) + ".00" + " " + WeightUOM.KG;
                    }
                }
                obj.eCommerceShipment = eCommerceBookingDetail;
                obj.BarcodePath = AppSettings.eCommerceUploadLabelFolder + eCommerceShipmentId + obj.eCommerceShipment.FrayteNumber + ".Png";
                obj.LabelPackages = new List<PackageLabel>();

                PackageLabel pL = new PackageLabel();
                if (data != null)
                {
                    pL.CartoonValue = 1;
                    pL.Content = data.Content;
                    pL.DirectShipmentDetailDraftId = data.DirectShipmentDetailDraftId;
                    pL.Height = data.Height;
                    pL.Weight = data.Weight;
                    pL.Width = data.Width;
                    pL.Value = data.Value;
                    pL.TrackingNo = data.TrackingNo;
                    pL.SerialNo = 1;
                    pL.PackageTrackingDetailId = data.PackageTrackingDetailId;
                    pL.LabelName = data.LabelName;
                    pL.IsPrinted = data.IsPrinted;
                    obj.LabelPackages.Add(pL);
                }

                string filename = eCommerceBookingDetail.FrayteNumber + "_" + DateTime.Now.ToString("dd_MM_yyyy_ss_fff");
                // FrayteResult result = new eCommerceShipmentLabelReport().GeteCommerceShipmentLabelReportDetail(eCommerceShipmentId, obj, filename);
            }
            return obj;
        }

        #endregion
        //public FrayteResult Generate_Seperate_PackageLabelPDF(int eCommercShipmentId, FrayteUploadshipment eCommerceBookingDetail)
        //{
        //    FrayteResult result = new FrayteResult();
        //    result.Errors = new List<string>();

        //    List<string> list = new List<string>();
        //    List<string> list1 = new List<string>();

        //    var Packages = new eCommerceShipmentRepository().GetPackageDetails(eCommercShipmentId);
        //    if (Packages != null)
        //    {
        //        foreach (var pack in Packages)
        //        {
        //            var packageTracking = new eCommerceShipmentRepository().GeteCommercePackageTracking(pack.eCommerceShipmentDetailId);
        //            if (packageTracking != null && packageTracking.Count > 0)
        //            {
        //                foreach (var data in packageTracking)
        //                {
        //                    list.Add(data.PackageImage);
        //                    list1.Add(data.PackageImage);
        //                    var resultReport = new Report.Generator.ManifestReport.PackageLabelReport().GenerateECommerceLabelReport(eCommercShipmentId, null, list1, eCommerceBookingDetail.CourierCompany);
        //                    if (!resultReport.Status)
        //                    {
        //                        result.Status = false;
        //                        result.Errors.Add("All the labels Pdf are not generated for " + eCommercShipmentId.ToString());
        //                    }
        //                    list1.Remove(data.PackageImage);
        //                }
        //            }
        //            else
        //            {
        //                result.Status = false;
        //                result.Errors.Add("All the labels Pdf are not generated for " + eCommercShipmentId.ToString());
        //            }
        //        }
        //        new Report.Generator.ManifestReport.PackageLabelReport().GenerateECommerceLabelUploadShipmentReport(eCommercShipmentId, eCommerceBookingDetail, list, eCommerceBookingDetail.CourierCompany);
        //    }
        //    else
        //    {
        //        result.Status = false;
        //        result.Errors.Add("All the labels Pdf are not generated fo " + eCommercShipmentId.ToString());
        //    }
        //    return result;
        //}
        private string GetFormattedManifestId(int manifestId)
        {

            string manifestName = string.Empty;
            if (manifestId.ToString().Length == 1)
            {
                manifestName = "000" + manifestId.ToString();
            }
            else if (manifestId.ToString().Length == 2)
            {
                manifestName = "00" + manifestId.ToString();
            }
            else if (manifestId.ToString().Length == 3)
            {
                manifestName = "0" + manifestId.ToString();
            }
            else
            {
                manifestName = manifestId.ToString();
            }

            return manifestName;
        }



    }
}
