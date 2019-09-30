using Frayte.Services.Business;
using Frayte.Services.Models;
using Frayte.Services.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EasyPost;
using Frayte.Services;
using System.Web;
using System.IO;
using RazorEngine.Templating;
using RazorEngine;
using Report.Generator.ManifestReport;
using System.Data.OleDb;
using System.Data;
using System.Net.Http.Headers;
using Frayte.Services.DataAccess;

namespace Frayte.WebApi.Controllers
{
    [Authorize]
    public class eCommerceController : ApiController
    {
        public FrayteResult DeleteeCommerceShipmentPackage(int eCommerceShipmentDetailDraftId)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                new eCommerceShipmentRepository().DeleteeCommerceShipmetPcsDetail(eCommerceShipmentDetailDraftId);
            }
            catch (Exception e)
            {
                result.Status = false;
            }
            return result;
        }

        public List<DirectBookingCustomer> GeteCommerceCustomers(int userId)
        {
            var customers = new eCommerceShipmentRepository().GetDirectBookingCustomers(userId);
            return customers;
        }


        #region eCommerce Booking Save & Update

        [HttpPost]
        public IHttpActionResult SaveBooking(FrayteCommerceShipmentDraft eCommerceBookingDetail)
        {
            int eCommerceShipmentId = 0;
            FrayteResult result = new FrayteResult();
            //Step 1: Save the information in database
            // eCommerceBookingDetail.Error  
            eCommerceBookingDetail.Error = new eCommerceShipmentRepository().SaveBooking(eCommerceBookingDetail);
            if (eCommerceBookingDetail.Error.Status && eCommerceBookingDetail.BookingStatusType == FrayteBookingStatusType.Current)
            {
                //Step 2: Shipment Integrations

                //EasyPost Integration here
                List<FrayteCommercePackageTrackingDetail> shipmentPackageTrackingDetail = new List<FrayteCommercePackageTrackingDetail>();

                // int Id = 0;
                var easyPostObj = new EasyPostRepository().MapShipmentToEasyPostShipment(eCommerceBookingDetail);

                var easyPostResult = new EasyPostRepository().CreateShipment(easyPostObj);

                eCommerceBookingDetail.Error = easyPostResult.Errors;

                try
                {
                    if (easyPostResult.Errors.Status)
                    {

                    }
                }
                catch (Exception ex2)
                {
                    easyPostResult.Errors.Miscellaneous = new List<string>();
                    easyPostResult.Errors.Miscellaneous.Add(ex2.Message);
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex2);
                    easyPostResult.Errors.Status = false;
                }


                if (eCommerceBookingDetail.Error != null && eCommerceBookingDetail.Error.Status == true)
                {

                    //Finally Save the Order Id too to get the information of multiple shpment in Tracking Detail page.
                    eCommerceShipmentId = new eCommerceShipmentRepository().SaveOrderNumber(eCommerceBookingDetail, easyPostResult.Order.id, eCommerceBookingDetail.CustomerId);

                    var easypostImage = new eCommerceShipmentRepository().DownloadEasyPostImages(easyPostResult.Order.shipments[0].tracking_code, easyPostResult.Order.shipments[0].postage_label.label_url, eCommerceShipmentId);

                    // EasyPost label
                    string labelPath = HttpContext.Current.Server.MapPath("~/PackageLabel/eCommerce/" + eCommerceShipmentId + "/");

                    int calculatedHeight = 1400;
                    int LocationY = 0;

                    //After creating an order need to save the information in database

                    var shipmentDetails = new eCommerceShipmentRepository().GetPackageDetails(eCommerceShipmentId);
                    int increment = 0;
                    int sum = eCommerceBookingDetail.Packages.Sum(p => p.CartoonValue);
                    for (int j = 0; j < eCommerceBookingDetail.Packages.Count; j++)
                    {
                        for (int i = 0; i < eCommerceBookingDetail.Packages[j].CartoonValue; i++)
                        {
                            increment++;
                            var data = shipmentDetails[j];

                            // Step 1 : database entry 
                            var trackingDetail = new eCommerceShipmentRepository().SaveEasyPostDetailTrackingDeatil(eCommerceBookingDetail, eCommerceBookingDetail.Packages[j], data, easyPostResult.Order.shipments[0].tracking_code, easyPostResult.Order.shipments[0].postage_label.label_url, eCommerceShipmentId, increment);

                            //  Step 2: Create frayte AWB label and save label name
                            string awbLabelName = generateAWBLabel(eCommerceShipmentId, eCommerceBookingDetail, increment, eCommerceBookingDetail.Packages[j]);
                            new eCommerceShipmentRepository().SaveAWBLabelName(trackingDetail, awbLabelName + ".pdf");

                            // Step 3 : crop image and save
                            var res = new EasyPostRepository().CropeImage(trackingDetail, eCommerceBookingDetail.CourierCompany, easypostImage, labelPath, calculatedHeight, LocationY, increment, sum);
                            LocationY += calculatedHeight;
                        }
                    }

                    // Generate Courier Label
                    Generate_Seperate_PackageLabelPDF(eCommerceShipmentId, eCommerceBookingDetail);

                    generateAWBLabel(eCommerceShipmentId, eCommerceBookingDetail, 0, null);
                    // Send SMS to consignee
                    string message = string.Empty;

                   
                    // Generate Invoice Report 

                    if (eCommerceBookingDetail.Error.IsMailSend)
                    {
                        //Send mail to developer
                        // new ShipmentEmailRepository().SendeCommerceShipmentErrorMail(eCommerceBookingDetail, eCommerceBookingDetail.Error);
                    }
                    if (eCommerceBookingDetail.Error.Status)
                    {
                        //SendDirectBookingConfirmationMailAsync(directBookingDetail);
                        new ShipmentEmailRepository().SendeCommercreBookingConfirmationMail(eCommerceBookingDetail, eCommerceShipmentId);
                    }
                    FrayteDirectShipment shipmentdetail = new eCommerceShipmentRepository().GetShipmentDetail(eCommerceShipmentId);
                    if (shipmentdetail != null)
                    {
                        eCommerceBookingDetail.DirectShipmentDraftId = shipmentdetail.DirectShipmentId;
                        eCommerceBookingDetail.ShipFrom.DirectShipmentAddressDraftId = shipmentdetail.FromAddressId;
                        eCommerceBookingDetail.ShipTo.DirectShipmentAddressDraftId = shipmentdetail.ToAddressId;
                        eCommerceBookingDetail.ShipmentStatusId = shipmentdetail.ShipmentStatusId;
                    }
                }
            }
            return Ok(eCommerceBookingDetail);
        }

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
                if (eCommerceBookingDetail.CourierCompany == FrayteCourierCompany.DHL || eCommerceBookingDetail.CourierCompany == FrayteCourierCompany.DHLExpress)
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
                result.Errors.Add("All the labels Pdf are not generated fo " + eCommercShipmentId.ToString());
            }
            return result;
        }

        #region AWB Label

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
                obj.BarcodePath = HttpContext.Current.Server.MapPath(AppSettings.eCommerceLabelFolder) + eCommerceShipmentId + "/" + obj.eCommerceShipment.FrayteNumber + ".Png";
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
                //Save FrayteLabel
                // Save Frayte Label Name 
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
                obj.BarcodePath = HttpContext.Current.Server.MapPath(AppSettings.eCommerceLabelFolder) + eCommerceShipmentId + "/" + obj.eCommerceShipment.FrayteNumber + ".Png";
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


        #endregion

        #region Generate Label Pdf
        private string Generate_PackageLabelPDF(int eCommerceShipmentId)
        {
            string pageStyleSheet = HttpContext.Current.Server.MapPath("~/Content/") + "print.css";
            string bootStrapStyleSheet = HttpContext.Current.Server.MapPath("~/Content/") + "bootstrap.min.css";
            string pdfLogo = HttpContext.Current.Server.MapPath("~/Content/") + "FrayteLogo.png";

            //   var detail = new DirectShipmentRepository().GetPackageList(eCommerceShipmentId, CourierCompany, RateType);
            var detail = new eCommerceShipmentRepository().GetPackageList(eCommerceShipmentId);
            string TrackingNo = new eCommerceShipmentRepository().GetTrackingNo(eCommerceShipmentId);

            FrayteShipmentPackageLabel obj = new FrayteShipmentPackageLabel();
            obj.PackageLabel = new List<FraytePackageLabel>();
            obj.PackageLabel = detail;
            obj.PageStyleSheet = pageStyleSheet;
            obj.BootStrapStyleSheet = bootStrapStyleSheet;
            obj.pdfLogo = pdfLogo;

            string template = File.ReadAllText(HttpContext.Current.Server.MapPath("~/Templates/PrintLabel.cshtml"));
            var templateService = new TemplateService();
            var EmailBody = Engine.Razor.RunCompile(template, "PackageLabel_", null, obj);

            string pdfFileName = string.Empty;
            pdfFileName = TrackingNo + ".html";

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
            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AppSettings.eCommerceLabelFolder + eCommerceShipmentId + "/"));
            string pdfPath = string.Empty;


            //    pdfPath = HttpContext.Current.Server.MapPath("~/PackageLabel/eCommerce/" + eCommerceShipmentId + "/") + TrackingNo + ".pdf";
            //"AWBUK-" + DateTime.UtcNow.Year.ToString().Substring(2, 2) + DateTime.UtcNow.ToString("MM") + GetFormattedManifestId(int eCommerceShipmentId)
            pdfPath = HttpContext.Current.Server.MapPath("~/PackageLabel/eCommerce/" + eCommerceShipmentId + "/") + "AWBUK-" + DateTime.UtcNow.Year.ToString().Substring(2, 2) + DateTime.UtcNow.ToString("MM") + GetFormattedManifestId(eCommerceShipmentId) + ".pdf";

            if (File.Exists(pdfPath))
            {
                File.Delete(pdfPath);
            }

            string pdfFile = string.Empty;
            pdfFile = Frayte.WebApi.Utility.PDFGenerator.HtmlToPdf("~/PackageLabel/eCommerce/" + eCommerceShipmentId + "/", "AWBUK-" + DateTime.UtcNow.Year.ToString().Substring(2, 2) + DateTime.UtcNow.ToString("MM") + GetFormattedManifestId(eCommerceShipmentId), lstHtmlFiles.ToArray(), null);
            if (System.IO.File.Exists(pdfFullPath))
            {
                System.IO.File.Delete(pdfFullPath);
            }

            return pdfFile;
        }
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
        #endregion

        #endregion

        #region Get eCommerceBooking Detail Clone,Return,Draft
        public FrayteCommerceShipmentDraft GeteCommerceBookingDetailDraft(int DirectShipmentDraftId, string CallingType)
        {
            return new eCommerceShipmentRepository().GeteCommerceBookingDetailDraft(DirectShipmentDraftId, CallingType);
        }
        [HttpGet]
        public FrayteCommerceShipmentDraft GeteCommerceWithServiceFormBookingDetailDraft(int DirectShipmentDraftId, string CallingType)
        {
            return new eCommerceShipmentRepository().GeteCommerceWithServiceFormBookingDetailDraft(DirectShipmentDraftId, CallingType);
        }


        #endregion

        #region Get eCommerceBookinfDetail For DetailPage 

        #region View Shipment
        public FrayteeCommerceShipmentDetail GeteCommerceBookingDetail(int eCommerceShipmentId, string CallingType)
        {
            return new eCommerceShipmentRepository().GeteCommerceBookingDetail(eCommerceShipmentId, CallingType);
        }

        public List<eCommerceManualTracking> GetManualTracking(int shipmentId)
        {
            List<eCommerceManualTracking> list = new List<eCommerceManualTracking>();
            list = new eCommerceShipmentRepository().GetManualTracking(shipmentId);
            return list;
        }

        [HttpPost]
        [AllowAnonymous]
        public IHttpActionResult SaveManualTracking(eCommTracking eCommerceTracking)
        {
            FrayteResult result = new FrayteResult();

            if (eCommerceTracking != null && eCommerceTracking.Tracking != null)
            {
                eCommerceManualTracking result1 = new eCommerceShipmentRepository().SaveManualTracking(eCommerceTracking.Tracking);
                if (eCommerceTracking.SentEmailToReceiver && result1 != null && result1.eCommerceTrackingId > 0)
                {
                    result = new ShipmentEmailRepository().SendManualTrackingEmail(eCommerceTracking.Tracking.eCommerceShipmentId, eCommerceTracking.Tracking);
                }
                return Ok(result1);
            }
            return BadRequest();
        }
        #endregion

        #region Invoice And Accounting
        [HttpGet]
        public eCommerceInvoiceAccounting InVoiceAndAcounting(int userId, int shipmentId)
        {

            eCommerceInvoiceAccounting result = new eCommerceShipmentRepository().GetInVoiceAndAcounting(userId, shipmentId);
            return result;

        }
        #endregion

        #region User Credit Note
        [HttpPost]
        public IHttpActionResult AddUserCreditNote(eCommerceInvoiceCreditNote creditNote)
        {
            FrayteResult result = new eCommerceShipmentRepository().AddUserCreditNote(creditNote);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public List<CurrencyType> GetCurrencies()
        {
            return new MasterDataRepository().GetCurrencyType();

        }
        #endregion

        #region Communtication
        [HttpPost]
        public IHttpActionResult CreateCommunication(InVoiceCommunication inVoiceCommunication)
        {
            try
            {
                FrayteResult result = new eCommerceShipmentRepository().CreateCommunication(inVoiceCommunication);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return BadRequest();
            }

        }

        public List<eCommerceInvoiceCommunication> GetCommunications(int userId, int shipmentId)
        {
            try
            {
                List<eCommerceInvoiceCommunication> list = new eCommerceShipmentRepository().GetCommunications(userId, shipmentId);
                return list;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }
        }

        public IHttpActionResult SaveEmailCommunication(InvoiceEmailCommunication emailCommunication)
        {
            FrayteResult result = new eCommerceShipmentRepository().SaveEmailCommunication(emailCommunication);
            return Ok(result);
        }

        public List<UserEmailCommunication> GetEmailCommunication(int userId, int shipmentId)
        {
            try
            {
                List<UserEmailCommunication> list = new eCommerceShipmentRepository().GetEmailCommunication(userId, shipmentId);
                return list;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }

        }


        #endregion

        #region Print Label Courier and FrayteAWB

         

        [HttpGet] 
        public IHttpActionResult PrintAllLabels(int eCommerceShipmentId , string LabelType)
        {
            FrayteResult result = new eCommerceShipmentRepository().PrintAllLabels(eCommerceShipmentId, LabelType);
            return Ok(result);
        }

        [HttpGet]
        public PrintLabel GetLabelFile(int eCommerceShipmentId, string LabelType)
        {
            PrintLabel file = new PrintLabel();
            file = new eCommerceShipmentRepository().GetLabelFile(eCommerceShipmentId, LabelType);
            return file;
        }
        [HttpGet]
        public PrintLabel GeneratePacakgelabel(int eCommerceShipmentId, int Id, string LabelType)
        {
            PrintLabel file = new PrintLabel();
            file = new eCommerceShipmentRepository().GeneratePacakgelabel(eCommerceShipmentId, Id, LabelType);

            return file;
        }
        [HttpPost]
        public HttpResponseMessage DownloadLabelReport(LabelReportFile file)
        {
            try
            {
                if (file != null && !string.IsNullOrEmpty(file.FileName))
                {
                    return DownloadLabelReport(file.eCommerceShipmentId, file.FileName);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #region DownloadQuotationReport
        private HttpResponseMessage DownloadLabelReport(int eCommerceShipmentId, string fileName)
        {
            string filePath = HttpContext.Current.Server.MapPath("~/PackageLabel/eCommerce/" + eCommerceShipmentId + "/" + fileName);
            using (MemoryStream ms = new MemoryStream())
            {
                using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    byte[] bytes = new byte[file.Length];
                    file.Read(bytes, 0, (int)file.Length);
                    ms.Write(bytes, 0, (int)file.Length);
                    HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
                    httpResponseMessage.Content = new ByteArrayContent(bytes);
                    httpResponseMessage.Content.Headers.Add("download-status", "downloaded");
                    httpResponseMessage.Content.Headers.Add("x-filename", fileName);
                    httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    httpResponseMessage.Content.Headers.ContentDisposition.FileName = fileName;
                    httpResponseMessage.StatusCode = HttpStatusCode.OK;
                    return httpResponseMessage;
                }
            }

        }

        #endregion
        #endregion

        #endregion

        #region Shipment HSCode
        [HttpGet]
        public FrayteResult SetShipmentHSCode(int eCommerceShipmentDetailid, string HSCode)
        {
            try
            {
                FrayteResult result = new eCommerceShipmentRepository().SetShipmentHSCode(eCommerceShipmentDetailid, HSCode);
                return result;
            }
            catch (Exception ex)
            {
                return new FrayteResult
                {
                    Status = false
                };
            }

        }
        #endregion
         
        [HttpGet]
        public FraytePackageResult PrintLabelAsPDF(int eCommerceShipmentId, string CourierCompany)
        {
            FraytePackageResult result = new FraytePackageResult();

            var detail = new eCommerceShipmentRepository().GeteCommerceBookingDetail(eCommerceShipmentId, "");
            var LabelDetail = new eCommerceShipmentRepository().GetLabelDetail(eCommerceShipmentId);
            string pdfFileName = string.Empty;
            string physycalpath = string.Empty;
            string str = string.Empty;
            string prfix = string.Empty;
            if (CourierCompany == FrayteCourierCompany.DHL || CourierCompany == FrayteCourierCompany.DHLExpress)
            {
                prfix = FrayteShortName.DHL;
            }
            str = prfix + "_" + detail.Packages[0].TrackingNo + " (All)";

            pdfFileName = AppSettings.WebApiPath + "PackageLabel/eCommerce/" + eCommerceShipmentId + "/";
            physycalpath = HttpContext.Current.Server.MapPath("~/PackageLabel/eCommerce/" + eCommerceShipmentId + "/" + str + ".pdf");

            if (File.Exists(physycalpath))
            {
                result.PackagePath = pdfFileName;
                result.IsDownloaded = true;
            }
            else
            {
                result.PackagePath = "";
                result.IsDownloaded = false;
            }
            return result;


        }

        [HttpGet]
        public IHttpActionResult SetPrintPackageStatus(int eCommercePackageTrackingDetailId, string Type)
        {
            FrayteResult result = new FrayteResult();
            result = new eCommerceShipmentRepository().SetPrintPackageStatus(eCommercePackageTrackingDetailId, Type);
            return Ok(result);
        }

        [HttpGet]

        public IHttpActionResult eCommerceDetailWithLabel(string ToMail, int eCommerceShipmentId, string CourierName)
        {
            new ShipmentEmailRepository().SendeCommerceBookingLabel(ToMail, eCommerceShipmentId, CourierName);
            return Ok();
        }
        //[HttpGet]
        //public List<FrayteManifestOnExcel> GetMainfestData(int eCommerceShipmentId)
        //{
        //    return new eCommerceShipmentRepository().GetMainfestData(eCommerceShipmentId);
        //}
    }
}
