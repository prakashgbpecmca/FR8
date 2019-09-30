using Frayte.Services.Business;
using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web;
using System.Web.Http;
using System.Threading;
using RazorEngine;
using RazorEngine.Templating;
using System.Net.Mime;
using Frayte.WebApi.Utility;
using System.Data.Entity.Validation;
using Frayte.Services.Utility;
using System.Data.OleDb;
using System.Data;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Drawing.Drawing2D;
using Spire.Barcode;

namespace Frayte.WebApi.Controllers
{
    public class ShipmentController : ApiController
    {
        [HttpGet]
        public string getPrefixViaTradelane(int shipmentId)
        {
            // Get TradealaneId
            int tradelaneId = new ShipmentRepository().GetTradelaneId(shipmentId);
            // Get carrierId And PreFix
            if (tradelaneId > 0)
            {
                int carrierId = new TradelaneRepository().GetCarrierId(tradelaneId);
                if (carrierId > 0)
                {
                    var PreFix = new CarrierRepository().GetPreFix(carrierId);
                    return PreFix;
                }
            }
            return "";
        }

        [HttpPost]
        public FrayteShipmentDetailExcel GetPiecesDetail()
        {

            FrayteShipmentDetailExcel frayteShipmentDetailexcel = new Services.Models.FrayteShipmentDetailExcel();

            var httpRequest = HttpContext.Current.Request;

            List<FrayteShipmentDetail> _shipmentdetail = new List<FrayteShipmentDetail>();
            FrayteShipmentDetail frayteshipment;

            if (httpRequest.Files.Count > 0)
            {
                HttpFileCollection files = httpRequest.Files;

                HttpPostedFile file = files[0];

                if (!string.IsNullOrEmpty(file.FileName))
                {
                    int ShipmentId = Convert.ToInt32(httpRequest.Form["ShipmentId"].ToString());
                    string connString = "";
                    string filename = DateTime.Now.ToString("MM_dd_yyyy_hh_mm_ss_") + file.FileName;
                    string filepath = HttpContext.Current.Server.MapPath("~/UploadFiles/Shipments/" + filename);

                    file.SaveAs(filepath);

                    connString = new ShipmentRepository().getExcelConnectionString(filename, filepath);

                    OleDbConnection oledbConn = new OleDbConnection(connString);
                    try
                    {
                        oledbConn.Open();
                        OleDbCommand cmd = new OleDbCommand("SELECT * FROM [Sheet1$]", oledbConn);
                        OleDbDataAdapter oleda = new OleDbDataAdapter();
                        oleda.SelectCommand = cmd;
                        DataSet ds = new DataSet();
                        oleda.Fill(ds, "Employees");

                        var exceldata = ds.Tables[0];

                        if (exceldata != null && exceldata.Rows.Count > 0)
                        {
                            if (new ShipmentRepository().CheckValidExcel(exceldata))
                            {
                                _shipmentdetail = new ShipmentRepository().GetPiecesDetail(ShipmentId, exceldata);
                                frayteShipmentDetailexcel.FrayteShipmentDetail = new List<FrayteShipmentDetail>();
                                frayteShipmentDetailexcel.FrayteShipmentDetail = _shipmentdetail;
                                frayteShipmentDetailexcel.Message = "OK";
                            }
                            else
                            {
                                frayteShipmentDetailexcel.Message = "Excel file not valid";
                            }
                        }
                        oledbConn.Close();
                        if ((System.IO.File.Exists(filepath)))
                        {
                            System.IO.File.Delete(filepath);
                        }
                    }
                    catch (Exception ex)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    }
                    finally
                    {
                        oledbConn.Close();
                    }
                }
            }
            return frayteShipmentDetailexcel;
        }

        [HttpGet]
        public FrayteShipmentTelex GetSeaInitial(int shipmentId)
        {
            FrayteShipmentTelex seaInitial = new FrayteShipmentTelex();
            try
            {
                seaInitial = new ShipmentRepository().GetSeaInitial(shipmentId);
            }
            catch (Exception e)
            {

            }
            return seaInitial;
        }

        [HttpGet]
        public HttpResponseMessage GetInitials()
        {

            List<WorkingWeekDay> lstWeekDays = new List<WorkingWeekDay>();

            lstWeekDays = new WeekDaysRepository().GetWeekDaysDataList();

            List<FrayteCountryCode> lstCountry = new List<FrayteCountryCode>();

            lstCountry = new CountryRepository().lstCountry();

            List<FrayteCountryPhoneCode> lstCountryPhones = new List<FrayteCountryPhoneCode>();

            lstCountryPhones = new CountryRepository().GetCountryPhoneCodeList();

            List<FrayteShipmentCourier> lstCouriers = new List<FrayteShipmentCourier>();

            lstCouriers = new CourierRepository().GetShipmentCourierList();

            List<TimeZoneModal> lstTimeZone = new List<TimeZoneModal>();

            lstTimeZone = new TimeZoneRepository().GetShipmentTimeZones();

            List<ShipmentType> lstShipmentType = new List<ShipmentType>();

            lstShipmentType = new ShipmentRepository().GetShipmentTypes();

            List<ShipmentWarehouse> lstWarehouse = new List<ShipmentWarehouse>();

            lstWarehouse = new WarehouseRepository().GetWarehouseList();

            List<TransportToWarehouse> lstTransportToWarehouse = new List<TransportToWarehouse>();

            lstTransportToWarehouse = new WarehouseRepository().GetTransportToWarehouse();

            FrayteOperationZone OperationZone = UtilityRepository.GetOperationZone();

            var termAndConditionId = new TermAndConditionRepository().GetTermAndConditionDetail();
            var OperationZones = new OperationZoneRepository().GetOperationZone();
            MasterDataRepository mdRepository = new MasterDataRepository();

            var lstShipmentTerms = mdRepository.GetShipmentTerms();
            var lstPackagingTypes = mdRepository.GetPackagingType();
            var lstSpecialDelivery = mdRepository.GetSpecialDelivery();
            var lstCurrencyTypes = mdRepository.GetCurrencyType();
            var lstShipmentPorts = mdRepository.GetShipmentPorts();
            string PiecesExcelDownloadPath = AppSettings.WebApiPath + "UploadFiles/PieceDetails.xlsx";

            return this.Request.CreateResponse(
                HttpStatusCode.OK,
                new
                {
                    WorkingWeekDays = lstWeekDays,
                    Countries = lstCountry,
                    CountryPhoneCodes = lstCountryPhones,
                    Couriers = lstCouriers,
                    TimeZones = lstTimeZone,
                    ShipmentTypes = lstShipmentType,
                    Warehouses = lstWarehouse,
                    TransportToWarehouses = lstTransportToWarehouse,
                    ShipmentTerms = lstShipmentTerms,
                    PackagingTypes = lstPackagingTypes,
                    SpecialDeliveries = lstSpecialDelivery,
                    CurrencyTypes = lstCurrencyTypes,
                    TermAndConditionId = termAndConditionId,
                    PiecesExcelDownloadPath = PiecesExcelDownloadPath,
                    ShipmentPorts = lstShipmentPorts,
                    OperationZone = OperationZone,
                    OperationZones = OperationZones
                });
        }

        [HttpGet]
        public FrayteShipment GetShipmentDetail(int shipmentId)
        {
            FrayteShipment result = new FrayteShipment();
            try
            {
                result = new ShipmentRepository().GetShipmentDetail(shipmentId);
                result.ShipmentPDF = AppSettings.WebApiPath + "UploadFiles/PDFGenerator/ShipmentDetail_" + shipmentId + ".PDF";
            }
            catch (DbEntityValidationException dbEx)
            {
                string validationErrorMsg = "";
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        validationErrorMsg = validationErrorMsg + string.Format("Property: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                    }
                }
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(validationErrorMsg));
            }
            catch (Exception ex)
            {

            }


            return result;
        }

        [HttpGet]
        public FrayteShipmentShipperReceiver GetShipmentShipperReceiverDetail(int shipmentId)
        {
            FrayteShipmentShipperReceiver result = new FrayteShipmentShipperReceiver();
            result = new ShipmentRepository().GetShipmentShipperReceiverDetail(shipmentId);
            return result;
        }

        public List<CountryShipmentPort> GetCountryPort(int countryId)
        {
            var shipmentPorts = new ShipmentRepository().GetCountryPort(countryId);
            return shipmentPorts;
        }

        public IHttpActionResult SaveAmmendShipment(FrayteCustomerAmendShipment frayteAmendShipment)
        {
            FrayteResult frayteResult = new FrayteResult();
            string newPDFPath = "";
            try
            {
                ShipmentRepository shipmentRepository = new ShipmentRepository();
                if (frayteAmendShipment.UserRoleId == FrayteUserRole.Customer)
                {
                    frayteResult = shipmentRepository.SaveAmmendShipment(frayteAmendShipment);
                    if (frayteResult.Status)
                    {
                        //After success, create shipment PDF                    
                        GeneratePDF_ShipmentDetail(frayteAmendShipment.ShipmentId);
                        newPDFPath = AppSettings.WebApiPath + "UploadFiles/PDFGenerator/ShipmentDetail_" + frayteAmendShipment.ShipmentId + ".PDF";
                        ShipmentEmailRepository shipmentEmailRepo = new ShipmentEmailRepository();

                        //When to send: After the customer has amended the booking. 
                        shipmentEmailRepo.SendEmail_E3_1(frayteAmendShipment.ShipmentId);

                        //shipmentRepository.SaveShipmentStatus(shipment, (int)FrayteShipmentStatus.CustomerAmended);
                        shipmentRepository.SaveShipmentStatus(frayteAmendShipment.ShipmentId, (int)FrayteShipmentStatus.CustomerAmended, frayteAmendShipment.CustomerId.Value);

                    }
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                string validationErrorMsg = "";
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        validationErrorMsg = validationErrorMsg + string.Format("Property: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                    }
                }
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(validationErrorMsg));
                return InternalServerError(dbEx);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return InternalServerError(ex);
            }
            return Ok(newPDFPath);
        }

        [HttpGet]
        public FrayteShipmentTracking GetTrackingdeatil(string CarrierName, string TrackingNumber)
        {
            try
            {
                var tracker = new ShipmentRepository().GetTrackerInfo(CarrierName, TrackingNumber);
                return tracker;
            }
            catch (Exception e)
            {

            }
            return null;
        }

        [HttpGet]
        public ShipmentTracking GetTracking(string CarrierName, string TrackingNumber)
        {
            var tracker = new ShipmentRepository().GetTracking(CarrierName, TrackingNumber);
            return tracker;
        }

        [HttpPost]
        public FrayteShipmentTracking GetBulkTrackingDetails(List<EasyPostTracking> TrackingInfo)
        {
            FrayteShipmentTracking tracker = new FrayteShipmentTracking();
            try
            {
                tracker = new ShipmentRepository().GetAllTrackerInfo(TrackingInfo);
                return tracker;
            }
            catch (Exception e)
            {

            }
            return tracker;
        }

        [HttpPost]
        public IHttpActionResult CustomerAction(FrayteCustomerActionShippment confirmationDetail)
        {
            ShipmentRepository shipmentRepository = new ShipmentRepository();
            int shipmentId = shipmentRepository.CustomerAction(confirmationDetail);
            int warehouseId = new WarehouseRepository().GetWareHouseId(shipmentId);
            if (shipmentId > 0)
            {
                ShipmentEmailRepository shipmentEmailRepo = new ShipmentEmailRepository();

                //If customer confirm the new shipment then need to send the mail
                if (confirmationDetail.ActionType == ShipmentCustomerAction.Confirm)
                {
                    // Save barCodeSo 
                    SaveShipmentBarcode(shipmentId);

                    string courierType = shipmentRepository.GetShipmentCourierType(shipmentId);

                    //Step 1: Cargowise Integration
                    string xmlPath = new ShipmentRepository().CreateCargoWiseXML(shipmentId);
                    if (AppSettings.ApplicationMode == FrayteApplicationMode.Live)
                    {
                        try
                        {
                            eAdapterInboundWebClient.SendMessage(
                                "https://FRYHKGservices.wisegrid.net/eAdapterStreamedService.svc",
                                xmlPath,
                                "FRYHKGHKG",
                                "IntegrateCargo",
                                "Cargo#1234");

                            //Successfully created shipment in CargoWise
                        }
                        catch (Exception ex)
                        {
                            //Some error occurred in CargoWise side.
                        }
                    }

                    //Step 2: Assign trade lande details to the shipment (Only when CourierType != Courier)
                    //Set OriginatingAgentId, DestinationAgentId and CarrierId
                    //Set OriginatingAgentId and DestinatingAgentId

                    if (courierType != FrayteShipmentType.Courier)
                    {
                        shipmentRepository.SetShipmentTradelane(shipmentId);
                    }

                    //Generate the Intrim Receipt, so that attachment can be send with E3 mail
                    GeneratePDF_DeliveryNote(shipmentId);

                    // EasyPost Integration For Courier Shipment
                    if (courierType == FrayteShipmentType.Courier)
                    {
                        EasyPostIntegration(shipmentId);
                    }
                    //Step 2: Send Emails

                    if (warehouseId > 0)
                    {
                        //Shipper : When to send: After the client has confirmed the booking.
                        shipmentEmailRepo.SendEmail_E3(shipmentId);

                        //Warehouse : When to send: Once client confirms the booking.
                        shipmentEmailRepo.SendEmail_E4(shipmentId);
                    }
                    //Step 3: Check Courier Type
                    if (courierType != FrayteShipmentType.Courier)
                    {
                        GeneratePDF_CoLoaderBookingForm(shipmentId);
                        //If not a courier type then need to send the mail to Agent
                        //Agent : When to send: Once client confirms the booking.
                        shipmentEmailRepo.SendEmail_E5(shipmentId);
                    }
                    else
                    {
                        //On this email we have to send the link where shipper can upload commercial invocie information
                        shipmentEmailRepo.SendEmail_E6(shipmentId);

                        //Also log the email sending time for ShipmentEmailService.
                        shipmentEmailRepo.LogForEmailSending(EmailTempletType.E6_1, shipmentId, DateTime.UtcNow.Date, DateTime.UtcNow.TimeOfDay);
                    }
                }
                else if (confirmationDetail.ActionType == ShipmentCustomerAction.Reject)
                {
                    // On Customer Rejection send mail to shipper 
                    shipmentEmailRepo.SendEmail_E3_2(shipmentId);
                    new ShipmentRepository().SaveShipmentStatus(shipmentId, (int)FrayteShipmentStatus.CustomerReject, 0);

                }
                //Wheter Customer Confirm or Reject, need to delete the record from ShipmentEmailService table

                shipmentEmailRepo.DeleteShipmentEmailService(EmailTempletType.E1, shipmentId);

                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public IHttpActionResult UploadCommercialInoiceDocuments()
        {
            var httpRequest = HttpContext.Current.Request;

            if (httpRequest.Files.Count > 0)
            {
                ShipmentRepository shipmentRepository = new ShipmentRepository();
                ShipmentEmailRepository shipmentEmailRepository = new ShipmentEmailRepository();

                HttpFileCollection files = httpRequest.Files;
                int ShipmentId = Convert.ToInt32(httpRequest.Form["ShipmentId"].ToString());
                var CommercialInvoiceFileName = httpRequest.Form["CommercialInvoiceFileName"];
                var CommercialInvoice = httpRequest.Form["CommercialInvoice"];
                var PackingListFileName = httpRequest.Form["PackingListFileName"];
                var PackingList = httpRequest.Form["PackingList"];
                var CustomDoc = httpRequest.Form["CustomDoc"];
                var CustomDocFileName = httpRequest.Form["CustomDocFileName"];
                var docfiles = new List<string>();
                int warehouseId = new WarehouseRepository().GetWareHouseId(ShipmentId);
                string filePathToSave = GetShipmentDocumentsSavePath(ShipmentId);

                //Created the object, so that we can pass multiple values to the function
                Shipment shipment = new Shipment();

                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFile file = files[i];

                    string fileFullPath = "";
                    string fileExt = "";

                    if (file.FileName == CommercialInvoiceFileName)
                    {
                        fileExt = Path.GetExtension(CommercialInvoiceFileName);

                        //Before saving make sure that extension is not added twice
                        if (CommercialInvoice.Contains(fileExt))
                        {
                            CommercialInvoice = CommercialInvoice.Replace(fileExt, "");
                        }

                        //Save in server folder
                        fileFullPath = filePathToSave + CommercialInvoice + fileExt;
                        file.SaveAs(fileFullPath);
                        docfiles.Add(CommercialInvoiceFileName);

                        shipment.CommercialInvoice = CommercialInvoice + fileExt;
                    }
                    else if (file.FileName == PackingListFileName)
                    {
                        fileExt = Path.GetExtension(PackingListFileName);

                        //Before saving make sure that extension is not added twice
                        if (PackingList.Contains(fileExt))
                        {
                            PackingList = PackingList.Replace(fileExt, "");
                        }

                        //Save in server folder
                        fileFullPath = filePathToSave + PackingList + fileExt;
                        file.SaveAs(fileFullPath);
                        docfiles.Add(PackingListFileName);

                        shipment.PackingList = PackingList + fileExt;
                    }
                    else if (file.FileName == CustomDocFileName)
                    {
                        fileExt = Path.GetExtension(CustomDocFileName);

                        //Before saving make sure that extension is not added twice
                        if (CustomDoc.Contains(fileExt))
                        {
                            CustomDoc = CustomDoc.Replace(fileExt, "");
                        }

                        //Save in server folder
                        fileFullPath = filePathToSave + CustomDoc + fileExt;
                        file.SaveAs(fileFullPath);
                        docfiles.Add(CustomDocFileName);

                        shipment.CustomDocument = CustomDoc + fileExt;
                    }
                }

                //Save file name and other information in DB
                shipmentRepository.SaveShipmentDocument(ShipmentId, FrayteShipmentDocumentType.CommercialInvoice, shipment);

                shipmentRepository.SaveShipmentStatus(ShipmentId, (int)FrayteShipmentStatus.CommericalInvoiceUploaded, 0);

                string courierType = shipmentRepository.GetShipmentCourierType(ShipmentId);

                if (courierType != FrayteShipmentType.Courier)
                {
                    if (courierType == FrayteShipmentType.Air)
                    {
                        bool checkStatus = shipmentRepository.IsMAWBUploaded(ShipmentId);
                        //Alert mail for Destinating Agent
                        //When to send: As soon as the shipper uploads the documents into the system and the Agent enters the MAWB.
                        if (checkStatus)
                        {
                            shipmentEmailRepository.SendEmail_E11(ShipmentId);
                        }
                    }
                }
                else
                {
                    //On this email we have to send the link to Warehouse user, who can upload AWB and AWB# information
                    shipmentEmailRepository.SendEmail_E4_1(ShipmentId);

                    //Also log the email sending time for ShipmentEmailService.
                    new ShipmentEmailRepository().LogForEmailSending(EmailTempletType.E4_1, ShipmentId, DateTime.UtcNow.Date, DateTime.UtcNow.TimeOfDay);
                }

                //After shipper uploaded the required documents need to delete the record from ShipmentEmailService table
                shipmentEmailRepository.DeleteShipmentEmailService(EmailTempletType.E6_1, ShipmentId);

                return Ok(docfiles);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public IHttpActionResult UploadTelexDocuments()
        {
            var httpRequest = HttpContext.Current.Request;

            if (httpRequest.Files.Count > 0)
            {
                ShipmentRepository shipmentRepository = new ShipmentRepository();
                ShipmentEmailRepository shipmentEmailRepository = new ShipmentEmailRepository();

                HttpFileCollection files = httpRequest.Files;
                int ShipmentId = Convert.ToInt32(httpRequest.Form["ShipmentId"].ToString());
                var SeaTelexStampName = httpRequest.Form["SeaTelexStampName"];
                var TelexReleaseBy = httpRequest.Form["TelexReleaseBy"];
                var SeaTelexDocumentName = httpRequest.Form["SeaTelexDocumentName"];
                var SeaTelexDocument = httpRequest.Form["SeaTelexDocument"];
                var SeaPOD = httpRequest.Form["SeaPOD"];
                var SeaPOL = httpRequest.Form["SeaPOL"];
                var docfiles = new List<string>();

                string filePathToSave = GetShipmentDocumentsSavePath(ShipmentId);

                //Created the object, so that we can pass multiple values to the function
                Shipment shipment = new Shipment();

                shipment.SeaPOD = SeaPOD;
                shipment.SeaPOL = SeaPOL;
                shipment.TelexReleaseBy = TelexReleaseBy;
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFile file = files[i];

                    string fileFullPath = "";
                    string fileExt = "";

                    //if (file.FileName == SeaTelexStampName)
                    //{
                    //    fileExt = Path.GetExtension(SeaTelexStampName);

                    //    //Before saving make sure that extension is not added twice
                    //    if (SeaTelexStamp.Contains(fileExt))
                    //    {
                    //        SeaTelexStamp = SeaTelexStamp.Replace(fileExt, "");
                    //    }

                    //    //Save in server folder
                    //    fileFullPath = filePathToSave + SeaTelexStamp + fileExt;
                    //    file.SaveAs(fileFullPath);
                    //    docfiles.Add(SeaTelexStampName);

                    //    shipment.SeaTelexStamp = SeaTelexStamp + fileExt;
                    //}

                    if (file.FileName == SeaTelexDocumentName)
                    {
                        fileExt = Path.GetExtension(SeaTelexDocumentName);

                        //Before saving make sure that extension is not added twice
                        if (SeaTelexDocument.Contains(fileExt))
                        {
                            SeaTelexDocument = SeaTelexDocument.Replace(fileExt, "");
                        }

                        //Save in server folder
                        fileFullPath = filePathToSave + SeaTelexDocument + fileExt;
                        file.SaveAs(fileFullPath);
                        docfiles.Add(SeaTelexDocumentName);

                        shipment.SeaTelexDocument = SeaTelexDocument + fileExt;
                    }
                }

                //Save file name and other information in DB
                shipmentRepository.SaveShipmentDocument(ShipmentId, FrayteShipmentDocumentType.TelexDocument, shipment);

                //To Do 
                // shipmentRepository.SaveShipmentStatus(ShipmentId, (int)FrayteShipmentStatus.CommericalInvoiceUploaded, 0);

                string courierType = shipmentRepository.GetShipmentCourierType(ShipmentId);

                if (courierType != FrayteShipmentType.Courier)
                {
                    // Mail to destinating agent
                    new ShipmentEmailRepository().SendEmail_E101(ShipmentId);

                    // Change in Process Flow
                    // bool checkStatus = shipmentRepository.IsMAWBUploaded(ShipmentId);
                    //Alert mail for Destinating Agent
                    //When to send: As soon as the shipper uploads the documents into the system and the Agent enters the MAWB.
                    //if (checkStatus)
                    //{
                    // shipmentEmailRepository.SendEmail_E11(ShipmentId);
                    //}
                    shipmentEmailRepository.SendEmail_E11(ShipmentId);
                }

                return Ok(docfiles);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public IHttpActionResult UploadAirWayBill()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                var ShipmentType = httpRequest.Form["ShipmentType"];

                ShipmentRepository shipmentRepository = new ShipmentRepository();

                HttpFileCollection files = httpRequest.Files;
                int ShipmentId = Convert.ToInt32(httpRequest.Form["ShipmentId"].ToString());
                var AirWayBillHash = httpRequest.Form["AirWayBillHash"];
                var AirWayBillDocument = httpRequest.Form["AirWayBillDocument"];

                var docfiles = new List<string>();

                string filePathToSave = GetShipmentDocumentsSavePath(ShipmentId);
                string fileExt = "";
                string fileFullPath = "";

                //This code will execute only when user will upload the document
                if (files.Count > 0)
                {
                    var AirWayBillDocumentFileName = httpRequest.Form["AirWayBillDocumentFileName"];

                    HttpPostedFile file = files[0];


                    if (file.FileName == AirWayBillDocumentFileName)
                    {
                        fileExt = Path.GetExtension(AirWayBillDocumentFileName);

                        //Before saving make sure that extension is not added twice
                        if (AirWayBillDocument.Contains(fileExt))
                        {
                            AirWayBillDocument = AirWayBillDocument.Replace(fileExt, "");
                        }

                        //Save in server folder
                        fileFullPath = filePathToSave + AirWayBillDocument + fileExt;
                        file.SaveAs(fileFullPath);
                    }
                }

                //Created the object, so that we can pass multiple values to the function
                Shipment shipment = new Shipment();
                if (ShipmentType == "i")
                {
                    shipment.AirWayBillDocument = AirWayBillDocument + fileExt;
                }

                shipment.AirWayBill = AirWayBillHash;

                //Save file name and other information in DB
                shipmentRepository.SaveShipmentDocument(ShipmentId, FrayteShipmentDocumentType.AirWayBill, shipment);

                string courierType = shipmentRepository.GetShipmentCourierType(ShipmentId);

                //Step 3: Check Courier Type
                if (courierType != FrayteShipmentType.Courier)
                {
                    //If not a courier type then need to send the mail to Agent
                    //SendEmail_E5(shipment.ShipmentId);
                }
                else
                {
                    if (ShipmentType == "i")
                    {

                        //For Courier case, close the shipment
                        shipmentRepository.SaveShipmentStatus(ShipmentId, (int)FrayteShipmentStatus.Close, 0);

                        //  Send AWB information Email to the Shipper.
                        new ShipmentEmailRepository().SendEmail_E15(ShipmentId);

                        //Send AWB information  Email to the Receiver. So that he can check the shipment detail in Courier's website.
                        new ShipmentEmailRepository().SendEmail_E16(ShipmentId);

                        // for courier if warehuse uploads the AWB# and AWB document then delete the E4_1 record from database
                        new ShipmentEmailRepository().DeleteShipmentEmailService(EmailTempletType.E4_1, ShipmentId);

                    }
                    else
                    {
                        new ShipmentEmailRepository().SendEmail_E65(ShipmentId);
                        new ShipmentEmailRepository().DeleteShipmentEmailService(EmailTempletType.E4_1, ShipmentId);
                    }
                }

                docfiles.Add(fileFullPath);
                return Ok(docfiles);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public IHttpActionResult UpdateDropOffDetail(FrayteShipmentDropOff dropOffDetail)
        {
            var result = new ShipmentRepository().UpdateDropOffDetail(dropOffDetail);

            if (result != null && result.Status)
            {
                string courierType = new ShipmentRepository().GetShipmentCourierType(dropOffDetail.ShipmentId);
                // For EmailSentTime in EailService Table
                TimeSpan MailToDropOffTime = new TimeSpan();

                var DropOffTime = UtilityRepository.GetTimeFromString(dropOffDetail.DropOffTime);
                if (courierType == FrayteShipmentType.Air)
                {
                    if (DropOffTime.HasValue)
                    {
                        TimeSpan time = TimeSpan.FromHours(3);
                        MailToDropOffTime = DropOffTime.Value.Add(time);
                    }

                    //Also log the email sending time for ShipmentEmailService.
                    new ShipmentEmailRepository().LogForEmailSending(EmailTempletType.E7, dropOffDetail.ShipmentId, dropOffDetail.DropOffDate, MailToDropOffTime, 1);

                    ////Also need to log E7_1
                    //DateTime dt = DateTime.Now.AddHours(4);
                    //new ShipmentEmailRepository().LogForEmailSending(EmailTempletType.E7_1, dropOffDetail.ShipmentId, dt.ToUniversalTime().TimeOfDay, 1);
                    //Issue on logging is we are not saving date information, we are just saving time information
                }
                else if (courierType == FrayteShipmentType.Sea)
                {
                    TimeSpan time = TimeSpan.FromHours(-1);
                    MailToDropOffTime = DropOffTime.Value.Add(time);
                    DateTime DropOffDate = new DateTime();
                    DropOffDate = dropOffDetail.DropOffDate.Value.AddDays(1);
                    //Also log the email sending time for ShipmentEmailService.
                    new ShipmentEmailRepository().LogForEmailSending(EmailTempletType.E7, dropOffDetail.ShipmentId, DropOffDate, MailToDropOffTime, 1);

                    ////Also need to log E7_1
                    //DateTime dt = DateTime.Now.AddHours(24);
                    //new ShipmentEmailRepository().LogForEmailSending(EmailTempletType.E7_1, dropOffDetail.ShipmentId, dt.ToUniversalTime().TimeOfDay, 1);
                    //Issue on logging is we are not saving date information, we are just saving time information
                }

                //After drop off time updated by warehouse user, need to remove the logging information
                new ShipmentEmailRepository().DeleteShipmentEmailService(EmailTempletType.E6_2, dropOffDetail.ShipmentId);

                return Ok(result);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public IHttpActionResult OperationStaffConfirmation(int shipmentId)
        {
            ShipmentEmailRepository shipmentEmailRepository = new ShipmentEmailRepository();

            //  Shipper: When to send: After the Agent has confirmed the booking. 
            shipmentEmailRepository.SendEmail_E6(shipmentId);

            //  Warehouse: When to send: After the Agent has confirmed the booking. 
            //  Warehouse need to update the drop off time
            shipmentEmailRepository.SendEmail_E62(shipmentId);

            // Also need to log the entry in ShipmentEmailService table
            //  So that backend service can send email after 1 hour if Shipper or Warehouse user will not respond in 1 hour.
            new ShipmentEmailRepository().LogForEmailSending(EmailTempletType.E6_1, shipmentId, DateTime.UtcNow.Date, DateTime.UtcNow.TimeOfDay);
            new ShipmentEmailRepository().LogForEmailSending(EmailTempletType.E6_2, shipmentId, DateTime.UtcNow.Date, DateTime.UtcNow.TimeOfDay);

            new ShipmentEmailRepository().DeleteShipmentEmailService(EmailTempletType.E6_3, shipmentId);

            return Ok();
        }

        [HttpPost]
        public IHttpActionResult UpdateOriginatingAgentDetail(FrayteShipmentOriginatingAgentDetails originatingAgentDetail)
        {
            var result = new ShipmentRepository().UpdateOriginatingAgentDetail(originatingAgentDetail);
            int wareHouseId = new WarehouseRepository().GetWareHouseId(originatingAgentDetail.ShipmentId);
            string courierType = new ShipmentRepository().GetShipmentCourierType(originatingAgentDetail.ShipmentId);
            if (result != null && result.Status)
            {
                ShipmentEmailRepository shipmentEmailRepository = new ShipmentEmailRepository();

                // Customer-Opretion Staff
                var DepartureTime = UtilityRepository.GetTimeFromString(originatingAgentDetail.OriginatingPlannedDepartureTime);

                shipmentEmailRepository.SendEmail_E63(originatingAgentDetail.ShipmentId);

                if (wareHouseId == 0)
                {
                    shipmentEmailRepository.SendEmail_E67(originatingAgentDetail.ShipmentId);
                }
                ////Shipper: When to send: After the Agent has confirmed the booking. 
                //shipmentEmailRepository.SendEmail_E6(originatingAgentDetail.ShipmentId);

                ////Warehouse: When to send: After the Agent has confirmed the booking. 
                ////Warehouse need to update the drop off time
                //shipmentEmailRepository.SendEmail_E62(originatingAgentDetail.ShipmentId);

                ////Also need to log the entry in ShipmentEmailService table
                ////So that backend service can send email after 1 hour if Shipper or Warehouse user will not respond in 1 hour.
                //new ShipmentEmailRepository().LogForEmailSending(EmailTempletType.E6_1, originatingAgentDetail.ShipmentId, DateTime.UtcNow.TimeOfDay);
                //new ShipmentEmailRepository().LogForEmailSending(EmailTempletType.E6_2, originatingAgentDetail.ShipmentId, DateTime.UtcNow.TimeOfDay);

                new ShipmentEmailRepository().LogForEmailSending(EmailTempletType.E6_3, originatingAgentDetail.ShipmentId, originatingAgentDetail.OriginatingPlannedDepartureDate, DepartureTime.Value);
                return Ok(result);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public IHttpActionResult OriginatingAgentReject(FrayteShipmentOriginatingAgentReject rejectionReason)
        {
            var result = new ShipmentRepository().OriginatingAgentReject(rejectionReason);

            if (result != null && result.Status)
            {
                ShipmentEmailRepository shipmentEmailRepository = new ShipmentEmailRepository();

                shipmentEmailRepository.SendEmail_E5_1(rejectionReason.ShipmentId);
                return Ok(result);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public IHttpActionResult UpdateDestinatingAgentAnticipatedDetail(FrayteShipmentDestinatingAgentAnticipatedDetail destinatingAgentAnticipatedDetail)
        {
            var result = new ShipmentRepository().UpdateDestinatingAgentAnticipatedDetail(destinatingAgentAnticipatedDetail);

            if (result != null && result.Status)
            {
                ShipmentEmailRepository shipmentEmailRepository = new ShipmentEmailRepository();

                var AnticipatedTime = UtilityRepository.GetTimeFromString(destinatingAgentAnticipatedDetail.AnticipatedDeliveryTime);
                //When to send: As soon as the Destination Agent confirms the anticipated delivery date and time.
                shipmentEmailRepository.SendEmail_E12(destinatingAgentAnticipatedDetail.ShipmentId);

                TimeSpan AnticipatedAfterTime = new TimeSpan();
                TimeSpan time = TimeSpan.FromHours(1);
                AnticipatedAfterTime = AnticipatedTime.Value.Add(time);

                //Also log the email sending time for ShipmentEmailService.
                shipmentEmailRepository.LogForEmailSending(EmailTempletType.E13, destinatingAgentAnticipatedDetail.ShipmentId, destinatingAgentAnticipatedDetail.AnticipatedDeliveryDate, AnticipatedAfterTime);

                //shipmentEmailRepository.LogForEmailSending(EmailTempletType.E13_1, destinatingAgentAnticipatedDetail.ShipmentId, DateTime.UtcNow.TimeOfDay.Add(new TimeSpan(2, 0, 0)));

                return Ok(result);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public IHttpActionResult UpdateShipperAnticipatedDetail(FrayteShipmentDestinatingAgentAnticipatedDetail destinatingAgentAnticipatedDetail)
        {
            var result = new ShipmentRepository().UpdateShipperAnticipatedDetail(destinatingAgentAnticipatedDetail);

            if (result != null && result.Status)
            {
                TimeSpan AnticipatedDeliveryTime = UtilityRepository.GetTimeFromString(destinatingAgentAnticipatedDetail.AnticipatedDeliveryTime).Value;
                new ShipmentEmailRepository().LogForEmailSending(EmailTempletType.E6_6, destinatingAgentAnticipatedDetail.ShipmentId, destinatingAgentAnticipatedDetail.AnticipatedDeliveryDate, AnticipatedDeliveryTime);
                return Ok(result);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public IHttpActionResult UpdateFlightSeaDetail(FrayteShipmentFlightSeaDetail flightSeaDetail)
        {
            ShipmentRepository shipmentRepo = new ShipmentRepository();

            var result = shipmentRepo.UpdateFlightSeaDetail(flightSeaDetail);

            if (result != null && result.Status)
            {
                string courierType = shipmentRepo.GetShipmentCourierType(flightSeaDetail.ShipmentId);

                var FlightSeaETDTime = UtilityRepository.GetTimeFromString(flightSeaDetail.ETDTime);

                TimeSpan FlightSeaTime = new TimeSpan();
                if (courierType == FrayteShipmentType.Air)
                {
                    //Log the Email E.8 entry - 2 hour before flight departure
                    TimeSpan time = TimeSpan.FromHours(-3);
                    FlightSeaTime = FlightSeaETDTime.Value.Add(time);
                    new ShipmentEmailRepository().LogForEmailSending(EmailTempletType.E8, flightSeaDetail.ShipmentId, flightSeaDetail.ETDDate, FlightSeaTime, 1);

                    ////Also need to log E8_1                     
                    //new ShipmentEmailRepository().LogForEmailSending(EmailTempletType.E8_1, flightSeaDetail.ShipmentId, flightSeaDetail.ETDTime.ToUniversalTime().TimeOfDay.Add(new TimeSpan(-2,0,0)), 1);
                }
                else if (courierType == FrayteShipmentType.Sea)
                {
                    //Log the Email E.8 entry - 2 hour before flight departure
                    var FlightSeaDate = flightSeaDetail.ETADate.AddDays(-1);
                    TimeSpan time = TimeSpan.FromHours(-1);
                    FlightSeaTime = FlightSeaETDTime.Value.Add(time);
                    new ShipmentEmailRepository().LogForEmailSending(EmailTempletType.E8, flightSeaDetail.ShipmentId, FlightSeaDate, FlightSeaTime, 1);

                    ////Also need to log E8_1                     
                    //new ShipmentEmailRepository().LogForEmailSending(EmailTempletType.E8_1, flightSeaDetail.ShipmentId, flightSeaDetail.ETDTime.ToUniversalTime().TimeOfDay.Add(new TimeSpan(-24, 0, 0)), 1);

                    // new mail to shipper for telex info (Process Flow Changed)
                    // new ShipmentEmailRepository().SendEmail_E64(flightSeaDetail.ShipmentId);
                }

                //Send pre-alert mail to Destinating agent for Flight/Vessel details
                new ShipmentEmailRepository().SendEmail_E10(flightSeaDetail.ShipmentId);

                //After Agent confirm the Flight/Vessel information, need to delete the record from ShipmentEmailService table
                new ShipmentEmailRepository().DeleteShipmentEmailService(EmailTempletType.E7, flightSeaDetail.ShipmentId);
                new ShipmentEmailRepository().DeleteShipmentEmailService(EmailTempletType.E7_1, flightSeaDetail.ShipmentId);

                return Ok(result);
            }
            else
            {
                return NotFound();
            }
        }

        public FrayteShipmentFlightSeaDetail GetETAETDDetail(int shipmentId)
        {
            FrayteShipmentFlightSeaDetail frayteETAETDDetail = new FrayteShipmentFlightSeaDetail();

            frayteETAETDDetail = new ShipmentRepository().GetETAETDDetail(shipmentId);
            if (frayteETAETDDetail != null)
            {
                return frayteETAETDDetail;
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        public IHttpActionResult UploadAWBDetail()
        {
            var httpRequest = HttpContext.Current.Request;

            if (httpRequest.Files.Count > 0)
            {
                ShipmentRepository shipmentRepository = new ShipmentRepository();

                HttpFileCollection files = httpRequest.Files;
                int ShipmentId = Convert.ToInt32(httpRequest.Form["ShipmentId"].ToString());
                var AWBFileName = httpRequest.Form["AWBFileName"];
                var AWB = httpRequest.Form["AWB"];
                var OtherDocsFileName = httpRequest.Form["OtherDocsFileName"];
                var OtherDocs = httpRequest.Form["OtherDocs"];

                var docfiles = new List<string>();

                string filePathToSave = GetShipmentDocumentsSavePath(ShipmentId);

                FrayteShipmentAWBDetail awbDetail = new FrayteShipmentAWBDetail();
                awbDetail.ShipmentId = ShipmentId;
                awbDetail.AWB = AWB;
                awbDetail.OtherDocs = OtherDocs;
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFile file = files[i];

                    string fileFullPath = "";
                    string fileExt = "";

                    if (file.FileName == AWBFileName)
                    {
                        fileExt = Path.GetExtension(AWBFileName);

                        //Before saving make sure that extension is not added twice
                        if (AWB.Contains(fileExt))
                        {
                            AWB = AWB.Replace(fileExt, "");
                        }

                        //Save in server folder
                        fileFullPath = filePathToSave + AWB + fileExt;
                        file.SaveAs(fileFullPath);
                        docfiles.Add(AWBFileName);

                        awbDetail.AWB = AWB + fileExt;
                    }
                    else if (file.FileName == OtherDocsFileName)
                    {
                        //Save in server folder
                        fileFullPath = filePathToSave + OtherDocs;
                        file.SaveAs(fileFullPath);
                        docfiles.Add(OtherDocsFileName);

                        awbDetail.OtherDocs = OtherDocsFileName;
                    }
                }

                //Save file name and other information in DB
                var result = shipmentRepository.UpdateAWBDetail(awbDetail);

                ShipmentEmailRepository shipmentEmailRepository = new ShipmentEmailRepository();

                //Also log the email sending time for ShipmentEmailService.
                //Only send when shipment is Third Party
                //shipmentEmailRepository.LogForEmailSending(EmailTempletType.E9, ShipmentId, DateTime.UtcNow.TimeOfDay);

                string courierType = shipmentRepository.GetShipmentCourierType(ShipmentId);

                //Step 3: Check Courier Type
                if (courierType != FrayteShipmentType.Courier)
                {
                    bool checkStatus = false;
                    if (courierType == FrayteShipmentType.Air)
                    {
                        checkStatus = shipmentRepository.IsOutstandingDocumentsUploaded(ShipmentId);
                    }
                    else if (courierType == FrayteShipmentType.Sea)
                    {
                        // newmail to shipper for telex info (Change in Process Flow)
                        shipmentEmailRepository.SendEmail_E64(ShipmentId);
                    }
                    //else if (courierType == FrayteShipmentType.Sea)
                    //{
                    //    checkStatus = shipmentRepository.IsTelexUploaded(ShipmentId);
                    //}
                    //Alert mail for Destinating Agent
                    //When to send: As soon as the shipper uploads the documents into the system and the Agent enters the MAWB.
                    if (checkStatus)
                    {
                        shipmentEmailRepository.SendEmail_E11(ShipmentId);
                    }

                    //Setp 4: After Agent uploaded the MAWB details, need to delete the record from ShipmentEmailService table
                    new ShipmentEmailRepository().DeleteShipmentEmailService(EmailTempletType.E8, ShipmentId);
                    new ShipmentEmailRepository().DeleteShipmentEmailService(EmailTempletType.E8_1, ShipmentId);
                }
                else
                {
                    //  Send AWB information Email to the Shipper.
                    new ShipmentEmailRepository().SendEmail_E15(ShipmentId);

                    //Send AWB information  Email to the Receiver. So that he can check the shipment detail in Courier's website.
                    new ShipmentEmailRepository().SendEmail_E16(ShipmentId);
                    new ShipmentEmailRepository().DeleteShipmentEmailService(EmailTempletType.E6_6, ShipmentId);
                    //For Courier case, close the shipment
                    shipmentRepository.SaveShipmentStatus(ShipmentId, (int)FrayteShipmentStatus.Close, 0);
                }

                if (result != null && result.Status)
                {
                    return Ok(docfiles);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public FrayteShipmentReslectAgentModel GetReselectAgentShipmentDetail(int shipmentId)
        {
            var result = new ShipmentRepository().GetReselectAgentShipmentDetail(shipmentId);
            return result;
        }

        [HttpPost]
        public IHttpActionResult ReselectAgent(FrayteShipmentReslectAgent agentDetail)
        {
            var result = new ShipmentRepository().ReselectAgent(agentDetail);

            if (result != null && result.Status)
            {
                ShipmentEmailRepository shipmentEmailRepository = new ShipmentEmailRepository();

                //To Do : Need to send detail mail to new agent so that he can further process the shipment.
                //Agent : When to send: Once frayte staff reselect new agent.
                shipmentEmailRepository.SendEmail_E5(agentDetail.ShipmentId);
                return Ok(result);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public IHttpActionResult UploadPODDetail()
        {
            var httpRequest = HttpContext.Current.Request;

            if (httpRequest.Files.Count > 0)
            {
                ShipmentRepository shipmentRepository = new ShipmentRepository();

                HttpFileCollection files = httpRequest.Files;
                int ShipmentId = Convert.ToInt32(httpRequest.Form["ShipmentId"].ToString());
                var PODDeliveryDate = httpRequest.Form["PODDeliveryDate"];
                var PODDeliveryTime = httpRequest.Form["PODDeliveryTime"];
                var Signature = httpRequest.Form["Signature"];
                var SignatureImageFileName = httpRequest.Form["SignatureImageFileName"];
                var SignatureImage = httpRequest.Form["SignatureImage"];
                var ExceptionNote = httpRequest.Form["ExceptionNote"];

                var docfiles = new List<string>();

                string filePathToSave = GetShipmentDocumentsSavePath(ShipmentId);

                FrayteShipmentPODDetail podDetail = new FrayteShipmentPODDetail();
                podDetail.ShipmentId = ShipmentId;
                podDetail.Signature = Signature;
                podDetail.PODDeliveryDate = Convert.ToDateTime(PODDeliveryDate);
                podDetail.PODDeliveryTime = PODDeliveryTime;
                podDetail.ExceptionNote = ExceptionNote;

                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFile file = files[i];

                    string fileFullPath = "";
                    string fileExt = "";

                    if (file.FileName == SignatureImageFileName)
                    {
                        fileExt = Path.GetExtension(SignatureImageFileName);

                        //Before saving make sure that extension is not added twice
                        if (SignatureImage.Contains(fileExt))
                        {
                            SignatureImage = SignatureImage.Replace(fileExt, "");
                        }

                        //Save in server folder
                        fileFullPath = filePathToSave + SignatureImage + fileExt;
                        file.SaveAs(fileFullPath);
                        docfiles.Add(SignatureImageFileName);

                        podDetail.SignatureImage = SignatureImage + fileExt;
                    }
                }

                //Save file name and other information in DB
                var result = shipmentRepository.UdatePODDetail(podDetail);

                //Customer & Shipper: When to send: As soon as the Destination Agent uploads the Proof of Delivery document into the system.
                new ShipmentEmailRepository().SendEmail_E14(ShipmentId);

                //After POD uploaded by destinating agent, need to remove the logging information
                new ShipmentEmailRepository().DeleteShipmentEmailService(EmailTempletType.E13, ShipmentId);
                new ShipmentEmailRepository().DeleteShipmentEmailService(EmailTempletType.E13_1, ShipmentId);

                if (result != null && result.Status)
                {
                    //To Do: Need to send the final delivery mails here

                    return Ok(docfiles);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public List<FrayteUserShipment> GetCurrentShipment(int roleId)
        {
            var currentShipments = new ShipmentRepository().GetCurrentShipment(roleId);

            return currentShipments;
        }

        [HttpGet]
        public List<FrayteUserShipment> GetPastShipment(int roleId)
        {
            var pastShipments = new ShipmentRepository().GetPastShipment(roleId);

            return pastShipments;
        }

        [HttpGet]
        public List<FrayteShipmentDocument> GetShipmentDocuments(int shipmentId)
        {
            string shipmentDocumentPath = AppSettings.WebApiPath + string.Format("UploadFiles/Shipments/{0}/", shipmentId);
            return new ShipmentRepository().GetShipmentDocuments(shipmentId, shipmentDocumentPath);
        }

        [HttpGet]
        public IHttpActionResult SendShipmentMail(int shipmentId, int emailNo)
        {
            ShipmentEmailRepository shipmentEmailRepository = new ShipmentEmailRepository();

            if (emailNo == 1)
            {
                shipmentEmailRepository.SendEmail_E1(shipmentId);
            }
            else if (emailNo == 2)
            {
                shipmentEmailRepository.SendEmail_E2(shipmentId);
            }
            else if (emailNo == 3)
            {
                shipmentEmailRepository.SendEmail_E3(shipmentId);
            }
            else if (emailNo == 31)
            {
                shipmentEmailRepository.SendEmail_E3_1(shipmentId);
            }
            else if (emailNo == 4)
            {
                shipmentEmailRepository.SendEmail_E4(shipmentId);
            }
            else if (emailNo == 41)
            {
                shipmentEmailRepository.SendEmail_E4_1(shipmentId);
            }
            else if (emailNo == 5)
            {
                shipmentEmailRepository.SendEmail_E5(shipmentId);
            }
            else if (emailNo == 51)
            {
                shipmentEmailRepository.SendEmail_E5_1(shipmentId);
            }
            else if (emailNo == 6)
            {
                shipmentEmailRepository.SendEmail_E6(shipmentId);
            }
            else if (emailNo == 61)
            {
                shipmentEmailRepository.SendEmail_E61(shipmentId);
            }
            else if (emailNo == 62)
            {
                shipmentEmailRepository.SendEmail_E62(shipmentId);
            }
            else if (emailNo == 64)
            {
                shipmentEmailRepository.SendEmail_E64(shipmentId);
            }
            else if (emailNo == 65)
            {
                shipmentEmailRepository.SendEmail_E65(shipmentId);
            }
            else if (emailNo == 66)
            {
                shipmentEmailRepository.SendEmail_E66(shipmentId);
            }
            else if (emailNo == 67)
            {
                shipmentEmailRepository.SendEmail_E67(shipmentId);
            }
            else if (emailNo == 7)
            {
                shipmentEmailRepository.SendEmail_E7(shipmentId);
            }
            else if (emailNo == 71)
            {
                shipmentEmailRepository.SendEmail_E71(shipmentId);
            }
            else if (emailNo == 8)
            {
                shipmentEmailRepository.SendEmail_E8(shipmentId);
            }
            else if (emailNo == 81)
            {
                shipmentEmailRepository.SendEmail_E81(shipmentId);
            }
            else if (emailNo == 9)
            {
                shipmentEmailRepository.SendEmail_E9(shipmentId);
            }
            else if (emailNo == 91)
            {
                shipmentEmailRepository.SendEmail_E91(shipmentId);
            }
            else if (emailNo == 10)
            {
                shipmentEmailRepository.SendEmail_E10(shipmentId);
            }
            else if (emailNo == 11)
            {
                shipmentEmailRepository.SendEmail_E11(shipmentId);
            }
            else if (emailNo == 12)
            {
                shipmentEmailRepository.SendEmail_E12(shipmentId);
            }
            else if (emailNo == 13)
            {
                shipmentEmailRepository.SendEmail_E13(shipmentId);
            }
            else if (emailNo == 131)
            {
                shipmentEmailRepository.SendEmail_E131(shipmentId);
            }
            else if (emailNo == 14)
            {
                shipmentEmailRepository.SendEmail_E14(shipmentId);
            }
            else if (emailNo == 15)
            {
                shipmentEmailRepository.SendEmail_E15(shipmentId);
            }
            else if (emailNo == 16)
            {
                shipmentEmailRepository.SendEmail_E16(shipmentId);
            }
            else if (emailNo == 17)
            {
                GeneratePDF_ShipmentDetail(shipmentId);
            }
            else if (emailNo == 18)
            {
                GeneratePDF_DeliveryNote(shipmentId);
            }
            else if (emailNo == 20)
            {
                new ShipmentEmailRepository().SendEmail_E1(shipmentId);
            }
            //else if (emailNo == 21)
            //{
            //    string cargoXML = new ShipmentRepository().CreateCargoWiseXML(shipmentId);
            //    try
            //    {
            //        eAdapterInboundWebClient.SendMessage(
            //            "https://FRYHKGservices.wisegrid.net/eAdapterStreamedService.svc",
            //            cargoXML,
            //            "FRYHKGHKG",
            //            "IntegrateCargo",
            //            "Cargo#1234");

            //        //Successfully created shipment in CargoWise
            //    }
            //    catch (Exception ex)
            //    {
            //        //Some error occurred in CargoWise side.
            //    }
            //}
            else if (emailNo == 999)
            {
                EasyPostIntegration(shipmentId);
            }

            return Ok();
        }

        [HttpGet]
        public IHttpActionResult CheckBarcodeValid(string Barcode)
        {
            if (!String.IsNullOrEmpty(Barcode))
            {
                FrayteResult result = new ShipmentRepository().CheckValidBarCode(Barcode);
                if (result.Status)
                {
                    return Ok();
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        public FrayteShipmentTracking ParcelHubTracking(string carrierName, string TrackingNo)
        {
            var data = new ShipmentRepository().GetParcelHubTracingInfo(carrierName, TrackingNo);
            return data;
        }

        private void EasyPostIntegration(int shipmentId)
        {
            //string regex = "(\\(.*\\))";

            FrayteShipment shipmentDetail = new ShipmentRepository().GetShipmentDetail(shipmentId);

            //Step 1: Setup Easy Post Key
            EasyPost.ClientManager.SetCurrent(AppSettings.EasyPostKey);
            //Step 2. Create From Address      
            string shipmentType = "";
            if (shipmentDetail.Warehouse != null)
            {
                shipmentType = UtilityRepository.GetShipmentExportImportType(shipmentDetail.Warehouse.WarehouseId);
            }
            else
            {
                shipmentType = FrayteExportType.Import;
            }
            EasyPost.Address fromAddress = new EasyPost.Address();
            // Set From Address Based on Shipment Type
            if (shipmentType == FrayteExportType.Import)
            {
                if (shipmentDetail.OtherPickupAddress == true && shipmentDetail.PickupAddress != null && shipmentDetail.PickupAddress.UserAddressId > 0)
                {
                    fromAddress.company = shipmentDetail.Shipper.CompanyName;
                    fromAddress.street1 = shipmentDetail.PickupAddress.Address;
                    fromAddress.street2 = shipmentDetail.PickupAddress.Address2;
                    fromAddress.city = shipmentDetail.PickupAddress.City;
                    fromAddress.state = shipmentDetail.PickupAddress.State;
                    fromAddress.country = shipmentDetail.PickupAddress.Country.Code2;
                    fromAddress.zip = shipmentDetail.PickupAddress.Zip;
                    fromAddress.phone = Regex.Replace(shipmentDetail.ShipmentPickupContactPhoneNumber, "(\\(.*\\))", "").Trim();
                }
                else
                {
                    fromAddress.company = shipmentDetail.Shipper.CompanyName;
                    fromAddress.street1 = shipmentDetail.ShipperAddress.Address;
                    fromAddress.street2 = shipmentDetail.ShipperAddress.Address2;
                    fromAddress.city = shipmentDetail.ShipperAddress.City;
                    fromAddress.state = shipmentDetail.ShipperAddress.State;
                    fromAddress.country = shipmentDetail.ShipperAddress.Country.Code2;
                    fromAddress.zip = shipmentDetail.ShipperAddress.Zip;
                    fromAddress.phone = Regex.Replace(shipmentDetail.Shipper.TelephoneNo, "(\\(.*\\))", "").Trim();
                }
            }
            else if (shipmentType == FrayteExportType.Export)
            {
                var WareHouseDeatil = new WarehouseRepository().GetWarehouseDetail(shipmentDetail.Warehouse.WarehouseId);
                fromAddress.company = shipmentDetail.Shipper.CompanyName;
                fromAddress.street1 = WareHouseDeatil.Address;
                fromAddress.street2 = WareHouseDeatil.Address2;
                fromAddress.city = WareHouseDeatil.City;
                fromAddress.state = WareHouseDeatil.State;
                fromAddress.country = WareHouseDeatil.Country.Code2;
                fromAddress.zip = WareHouseDeatil.Zip;
                fromAddress.phone = Regex.Replace(WareHouseDeatil.TelephoneNo, "(\\(.*\\))", "").Trim();
            }
            //EasyPost.Address fromAddress = new EasyPost.Address()
            //{
            //    company = shipmentDetail.Shipper.CompanyName,
            //    street1 = shipmentDetail.ShipperAddress.Address,
            //    street2 = shipmentDetail.ShipperAddress.Address2,
            //    city = shipmentDetail.ShipperAddress.City,
            //    state = shipmentDetail.ShipperAddress.State,
            //    country = "HK",
            //    zip = shipmentDetail.ShipperAddress.Zip,
            //    phone = Regex.Replace(shipmentDetail.Shipper.TelephoneNo, "(\\(.*\\))", "").Trim()
            //};

            fromAddress.Create();
            dynamic json = Newtonsoft.Json.JsonConvert.SerializeObject(fromAddress);
            Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(json));

            var ShippreAddress = new ShipperRepository().GetShipperMainAddress(shipmentDetail.Shipper.UserId);
            // save to database
            new ShipperRepository().SaveEasyPostAddressId(shipmentDetail.ShipperAddress.UserAddressId, fromAddress.id);
            // string fromAddressId = fromAddress.id;


            //Step 3. Create To Address            
            EasyPost.Address toAddress = new EasyPost.Address()
            {
                company = shipmentDetail.Receiver.CompanyName,
                street1 = shipmentDetail.ReceiverAddress.Address,
                street2 = shipmentDetail.ReceiverAddress.Address2,
                city = shipmentDetail.ReceiverAddress.City,
                state = shipmentDetail.ReceiverAddress.State,
                country = shipmentDetail.ReceiverAddress.Country.Code2,
                zip = shipmentDetail.ReceiverAddress.Zip,
                phone = Regex.Replace(shipmentDetail.Receiver.TelephoneNo, "(\\(.*\\))", "").Trim()
            };

            toAddress.Create();
            json = Newtonsoft.Json.JsonConvert.SerializeObject(toAddress);
            Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(json));
            // save to database
            new ReceiverRepository().SaveEasyPostReceiverAddressId(shipmentDetail.ReceiverAddress.UserAddressId, toAddress.id);
            // string toAddressId = toAddress.id;

            //Step 4. Create Parcel
            EasyPost.Parcel parcel = EasyPost.Parcel.Create(new Dictionary<string, object>()
            {
              {"length", shipmentDetail.ShipmentDetails[0].Lcms},
              {"width", shipmentDetail.ShipmentDetails[0].Hcms},
              {"height", shipmentDetail.ShipmentDetails[0].Wcms},
              {"weight", shipmentDetail.ShipmentDetails[0].WeightKg}
            });

            string parcelId = parcel.id;



            EasyPost.CustomsInfo customsInfo = new EasyPost.CustomsInfo();
            EasyPost.Shipment shipment = new EasyPost.Shipment();

            // Check that originating and destinating country are different 
            if (shipmentDetail.Warehouse != null && shipmentDetail.Warehouse.WarehouseId > 0 && shipmentDetail.Warehouse.CountryId != shipmentDetail.ReceiverAddress.Country.CountryId)
            {
                // custom info
                customsInfo = CreateCustomInformation(shipmentDetail, shipmentType);
                // add custom info to shipment 
                shipment.customs_info = customsInfo;
            }
            if (shipmentDetail.PickupAddress.Country != null && shipmentDetail.PickupAddress.Country.CountryId != shipmentDetail.ReceiverAddress.Country.CountryId)
            {
                // custom info
                customsInfo = CreateCustomInformation(shipmentDetail, shipmentType);
                // add custom info to shipment 
                shipment.customs_info = customsInfo;
            }

            if (shipmentDetail.Warehouse != null && shipmentDetail.Warehouse.WarehouseId == 0 && shipmentDetail.PickupAddress.Country == null && shipmentDetail.ShipperAddress.Country.CountryId != shipmentDetail.ReceiverAddress.Country.CountryId)
            {
                // custom info
                customsInfo = CreateCustomInformation(shipmentDetail, shipmentType);
                // add custom info to shipment 
                shipment.customs_info = customsInfo;
            }

            //Step 5. Create Shipment             
            // EasyPost.Shipment shipment = new EasyPost.Shipment();
            shipment.from_address = fromAddress;
            shipment.to_address = toAddress;
            shipment.parcel = parcel;
            // if different country
            //   shipment.customs_info = customsInfo;
            shipment.Create();
            json = Newtonsoft.Json.JsonConvert.SerializeObject(shipment);
            Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(json));
            //Step 6. Get Rate Cards from different Courier Company.
            EasyPost.Rate shipmentRate = new EasyPost.Rate();
            foreach (EasyPost.Rate rate in shipment.rates)
            {
                //For Testing use USPS
                //Description USPS Account 
                //Account ID ca_cea60c9054624c8897fae9f90ddddbdb 

                if (rate.carrier.Contains("USPS"))
                {
                    shipmentRate = rate;

                    //Save rate in database
                    new ShipmentRepository().SaveEasyPostRate(shipmentId, shipmentRate);
                    break;
                }


                if (shipmentDetail.DeliveredBy.Name == "DHLExpress")
                {
                    if (shipmentType == FrayteExportType.Export)
                    {
                        if (rate.carrier_account_id == "ca_feddc1219b4c47a79844e523f50ca3b0" && rate.service == ShipmentRateServiceType.ExpressWorldwideNonDoc)
                        {
                            shipmentRate = rate;
                            //Save rate in database
                            new ShipmentRepository().SaveEasyPostRate(shipmentId, shipmentRate);
                            break;
                        }
                    }

                    else if (shipmentType == FrayteExportType.Import)
                    {
                        if (rate.carrier_account_id == "ca_41c0a95c0cac4387838048b1fd829353" && rate.service == ShipmentRateServiceType.ExpressWorldwideNonDoc)
                        {
                            shipmentRate = rate;
                            new ShipmentRepository().SaveEasyPostRate(shipmentId, shipmentRate);
                            break;
                        }
                    }

                    else if (shipmentType == FrayteExportType.Import)
                    {
                        if (rate.carrier_account_id == "ca_9121bd4c61a44bb6bafc4885af96be9a" && rate.service == ShipmentRateServiceType.ExpressWorldwideNonDoc)
                        {
                            shipmentRate = rate;
                            new ShipmentRepository().SaveEasyPostRate(shipmentId, shipmentRate);
                            break;
                        }
                    }
                    //if (rate.carrier.Contains("DHL"))
                    //{
                    //    shipmentRate = rate;
                    //    break;
                    //}
                }
                else if (shipmentDetail.DeliveredBy.Name.Contains("FedEx"))
                {

                    //if (rate.carrier.Contains("FedEx"))
                    //{
                    //    shipmentRate = rate;
                    //    break;
                    //}
                    if (rate.carrier_account_id == "ca_41c0a95c0cac4387838048b1fd829353" && rate.service == "INTERNATIONAL_PRIORITY")
                    {
                        shipmentRate = rate;
                        new ShipmentRepository().SaveEasyPostRate(shipmentId, shipmentRate);
                        break;
                    }
                    else if (rate.carrier_account_id == "ca_41c0a95c0cac4387838048b1fd829353" && rate.service == "INTERNATIONAL_ECONOMY")
                    {
                        shipmentRate = rate;
                        new ShipmentRepository().SaveEasyPostRate(shipmentId, shipmentRate);
                        break;
                    }
                }
                //else if (shipmentDetail.DeliveredBy.Name.Contains("TNT"))
                //{
                //    if (rate.carrier.Contains("TNT"))
                //    {
                //        shipmentRate = rate;
                //        break;
                //    }
                //}
                //else if (shipmentDetail.DeliveredBy.Name.Contains("UPS"))
                //{
                //    if (rate.carrier.Contains("UPS"))
                //    {
                //        shipmentRate = rate;
                //        break;
                //    }
                //}
            }
            // Only For Testing 
            if (shipmentRate != null && shipmentRate.id == null && shipment.rates.Count > 0)
            {
                shipmentRate = shipment.rates[0];
                new ShipmentRepository().SaveEasyPostRate(shipmentId, shipmentRate);
            }
            try
            {
                //Step 7. Buy the shipment from Courier Service.
                //shipment.Buy(shipmentRate);
                shipment.Buy(shipmentRate.id);

                // Save postage_label.label_url , tracking_code 
                new ShipmentRepository().SaveEasyPostDetailTrackingDeatil(shipmentDetail, shipment);

                // Print PNG link
                string printLabelURL = shipment.postage_label.label_url;
                // Print Tracking Code
                string trackingCode = shipment.tracking_code;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }

        public string GetDateFormFile()
        {
            string a = "";
            string bb = "";
            string result = "";
            string[] fileNames = Directory.GetFiles(AppSettings.AssetFolderPath);
            foreach (var res in fileNames)
            {
                if (res.Contains(".css"))
                {
                    a = res;
                    string[] b = a.Split('-');
                    if (b[4].Contains(".css"))
                    {
                        string[] aa = b[4].Split('.');
                        bb = aa[0];
                    }
                    else
                    {
                        bb = b[4];
                    }


                    result = b[2] + '-' + b[3] + '-' + bb;
                    break;
                }
            }
            return result;
        }

        #region -- Private Methods --

        private EasyPost.CustomsInfo CreateCustomInformation(FrayteShipment shipmentDetail, string shipmentType)
        {
            string originCountry = "";
            if (shipmentType == FrayteExportType.Export)
            {
                if (shipmentDetail.Warehouse != null)
                {
                    var country = new WarehouseRepository().GetWareHouseCountry(shipmentDetail.Warehouse.CountryId);
                    if (country != null)
                    {
                        originCountry = country.Code2;
                    }
                }
            }
            else if (shipmentType == FrayteExportType.Import)
            {
                if (shipmentDetail.OtherPickupAddress == true && shipmentDetail.PickupAddress != null && shipmentDetail.PickupAddress.UserAddressId > 0)
                {
                    originCountry = shipmentDetail.PickupAddress.Country.Code2;
                }
                else
                {
                    originCountry = shipmentDetail.ShipperAddress.Country.Code2;
                }
            }
            EasyPost.CustomsItem customsItem1 = EasyPost.CustomsItem.Create(new Dictionary<string, object>() {
                  {"description", shipmentDetail.ShipmentDetails[0].PiecesContent},
                  {"quantity", shipmentDetail.ShipmentDetails[0].CartonQty},
                  {"weight", shipmentDetail.ShipmentDetails[0].WeightKg},
                  {"value", shipmentDetail.DeclaredValue},
                  {"origin_country",originCountry},
                  {"hs_tariff_number", shipmentDetail.ShipmentDetails[0].HSCode}
                });

            EasyPost.CustomsInfo customsInfo = new EasyPost.CustomsInfo();
            customsInfo.customs_certify = shipmentDetail.CustomInfo.CustomsCertify.Value ? "true" : "false";
            customsInfo.eel_pfc = shipmentDetail.CustomInfo.EelPfc;
            customsInfo.customs_signer = shipmentDetail.CustomInfo.CustomsSigner;
            customsInfo.contents_type = shipmentDetail.CustomInfo.ContentsType;
            customsInfo.customs_items = new List<EasyPost.CustomsItem>();

            customsInfo.customs_items.Add(customsItem1);

            return customsInfo;
        }

        private string GetShipmentDocumentsSavePath(int shipmentId)
        {
            string filePathToSave = string.Format(FrayteDocumentPath.ShipmentDocument, shipmentId);
            filePathToSave = HttpContext.Current.Server.MapPath(filePathToSave);

            if (!System.IO.Directory.Exists(filePathToSave))
                System.IO.Directory.CreateDirectory(filePathToSave);
            return filePathToSave;
        }

        private string GeneratePDF_ShipmentDetail(int shipmentId)
        {
            string pageStyleSheet = HttpContext.Current.Server.MapPath("~/Content/") + "confirm.css";
            string bootStrapStyleSheet = HttpContext.Current.Server.MapPath("~/Content/") + "bootstrap.min.css";
            string PDFLogo = HttpContext.Current.Server.MapPath("~/Content/") + "FrayteLogo.png";
            var detail = new ShipmentRepository().GetShipmentDetail(shipmentId);

            var TerAndConditionDetail = new TermAndConditionRepository().GetTermAndCondition(detail.TermAndConditionId);

            FrayteShipmentDetailPdf obj = new FrayteShipmentDetailPdf();
            obj.ShipmentDetail = new FrayteShipment();
            obj.ShipmentDetail = detail;
            obj.PageStyleSheet = pageStyleSheet;
            obj.BootStrapStyleSheet = bootStrapStyleSheet;
            obj.pdfLogo = PDFLogo;
            if (TerAndConditionDetail != null)
            {
                var TermCondition = TerAndConditionDetail.Detail;
                obj.TermAndConditionNew = TermCondition;
            }

            if (detail != null && detail.ShipmentDetails != null && detail.ShipmentDetails.Count > 0)
            {
                obj.TotalPieces = detail.ShipmentDetails.Sum(x => x.Pieces);

                var tWeight = GetTotalVolWeight(detail.ShipmentDetails);
                obj.TotalWeight = String.Format("{0:0.##}", tWeight);

                var tkgs = detail.ShipmentDetails.Sum(x => x.WeightKg);
                obj.TotalKgs = String.Format("{0:0.##}", tkgs);
            }
            else
            {
                obj.TotalPieces = 0;
                obj.TotalWeight = "0.00";
                obj.TotalKgs = "0.00";
            }

            if (detail != null && detail.DeclaredValue != null)
            {
                var declarvalue = detail.DeclaredValue.ToString();
                if (declarvalue.Contains('.'))
                {
                    var arr = declarvalue.Split('.');
                    obj.DeclaredValue = arr[0];
                }
            }

            if (detail != null && detail.ShipmentReadyBy != null)
            {
                if (detail.ShipmentReadyBy.Contains(':'))
                    obj.ShipmentDetail.ShipmentReadyBy = detail.ShipmentReadyBy;
                else
                {
                    var str1 = detail.ShipmentReadyBy.Substring(0, 2);
                    var str2 = detail.ShipmentReadyBy.Substring(2, 2);
                    obj.ShipmentDetail.ShipmentReadyBy = str1 + ":" + str2;
                }
            }

            string template1 = File.ReadAllText(HttpContext.Current.Server.MapPath("~/Templates/") + "ShipmentDetailNew.cshtml");
            var EmailBody = Engine.Razor.RunCompile(template1, "ShipmentDetail" + shipmentId, null, obj);
            string pdfFileName = "ShipmentDetail_" + shipmentId + "_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss") + ".html";
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
            string pdfPath = HttpContext.Current.Server.MapPath("~/UploadFiles/PDFGenerator/") + "ShipmentDetail_" + shipmentId + ".PDF";
            if (File.Exists(pdfPath))
            {
                File.Delete(pdfPath);
            }

            string pdfFile = PDFGenerator.HtmlToPdf("~/UploadFiles/PDFGenerator", "ShipmentDetail_" + shipmentId, lstHtmlFiles.ToArray(), null);

            if (System.IO.File.Exists(pdfFullPath))
            {
                System.IO.File.Delete(pdfFullPath);
            }

            return pdfFile;
        }

        // Generate Pd for co-loader booking form
        public string GeneratePDF_CoLoaderBookingForm(int shipmentId)
        {
            string pageStyleSheet = HttpContext.Current.Server.MapPath("~/Content/") + "co_loadBookingForm.css";
            string bootStrapStyleSheet = HttpContext.Current.Server.MapPath("~/Content/") + "bootstrap.min.css";
            string PDFLogo = HttpContext.Current.Server.MapPath("~/Content/") + "frayteHeader.png";
            var detail = new ShipmentRepository().GetShipmentDetail(shipmentId);

            FrayteShipmentDetailPdf obj = new FrayteShipmentDetailPdf();
            FrayteCoLoaderBookingFormPdf obj1 = new FrayteCoLoaderBookingFormPdf();
            obj1.PageStyleSheet = pageStyleSheet;
            obj1.BootStrapStyleSheet = bootStrapStyleSheet;
            obj1.PDFLogo = PDFLogo;
            if (detail.Shipper != null)
            {
                obj1.ShipperName = detail.Shipper.ContactName;
                obj1.ShiiperTelephoneNo = detail.Shipper.TelephoneNo;
                obj1.ShipperFaxNo = detail.Shipper.FaxNumber;
            }
            obj1.SpecialInstruction = detail.SpecialInstruction;
            obj1.Notifyparty = detail.PaymentParty;
            if (detail.Receiver != null)
            {
                obj1.ReceiverName = detail.Receiver.ContactName;
                obj1.ReceiverTelephoneNo = detail.Receiver.TelephoneNo;
            }
            if (detail.FrayteShipmentPortOfDeparture != null && detail.FrayteShipmentPortOfArrival != null)
            {
                obj1.OriginatingAirPort = detail.FrayteShipmentPortOfDeparture.PortName;
                obj1.DestinatingAirPort = detail.FrayteShipmentPortOfArrival.PortName;
            }

            obj1.DeclaredValue = detail.DeclaredValue.ToString();
            obj1.DescriptionOfGoods = detail.ContentDescription;

            if (detail.Warehouse != null && detail.Warehouse.WarehouseId > 0)
            {
                var OCountry = new CountryRepository().GetCountryDetail(detail.Warehouse.CountryId);
                if (OCountry != null)
                {
                    obj1.CountryOfOrigin = OCountry.Name;
                }
            }
            else
            {
                if (detail.PickupAddress != null && detail.PickupAddress.Country != null && detail.PickupAddress.Country.CountryId > 0)
                {
                    obj1.CountryOfOrigin = detail.PickupAddress.Country.Name;
                }
                else
                {
                    obj1.CountryOfOrigin = detail.ShipperAddress.Country.Name;
                }
            }

            if (detail != null && detail.ShipmentDetails != null && detail.ShipmentDetails.Count > 0)
            {
                obj1.NoOfPacktes = detail.ShipmentDetails.Sum(x => x.Pieces);
                var tWeight = GetTotalVolWeight(detail.ShipmentDetails);
                obj1.TotalCBM = String.Format("{0:0.##}", tWeight);
                var tkgs = detail.ShipmentDetails.Sum(x => x.WeightKg);
                obj1.GrossWeight = String.Format("{0:0.##}", tkgs);
            }
            else
            {
                obj1.NoOfPacktes = 0;
                obj1.TotalCBM = "0.00";
                obj1.GrossWeight = "0.00";
            }

            if (detail != null && detail.DeclaredValue != null)
            {
                var declarvalue = detail.DeclaredValue.ToString();
                if (declarvalue.Contains('.'))
                {
                    var arr = declarvalue.Split('.');
                    obj1.DeclaredValue = arr[0];
                }
                else
                {
                    obj1.DeclaredValue = detail.DeclaredValue.ToString();
                }
            }

            if (detail.DeclaredCurrency != null)
            {
                obj1.SpecifyCurrency = detail.DeclaredCurrency.CurrencyCode;
            }

            string template3 = File.ReadAllText(HttpContext.Current.Server.MapPath("~/Templates/") + "co_loadBookingForm.cshtml");
            var EmailBody = Engine.Razor.RunCompile(template3, "CO_Load_Booking_Form_", null, obj1);
            string pdfFileName = "CO_Load_Booking_Form_" + shipmentId + ".html";
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
            string pdfPath = HttpContext.Current.Server.MapPath("~/UploadFiles/PDFGenerator/") + "CO_Load_Booking_Form_" + shipmentId + ".PDF";
            if (File.Exists(pdfPath))
            {
                File.Delete(pdfPath);
            }

            string pdfFile = PDFGenerator.HtmlToPdf("~/UploadFiles/PDFGenerator", "CO_Load_Booking_Form_" + shipmentId, lstHtmlFiles.ToArray(), null);

            if (System.IO.File.Exists(pdfFullPath))
            {
                System.IO.File.Delete(pdfFullPath);
            }
            return pdfFile;
        }

        private void GeneratePDF_DeliveryNote(int shipmentId)
        {
            string pageStyleSheet = HttpContext.Current.Server.MapPath("~/Content/") + "DeliveryNote.css";
            string bootStrapStyleSheet = HttpContext.Current.Server.MapPath("~/Content/") + "bootstrap.min.css";
            string imgSrc = HttpContext.Current.Server.MapPath("~/Content/") + "FrayteLogo.png";

            var detail = new ShipmentRepository().GetShipmentDetail(shipmentId);
            if (detail.OriginatingAgentId.HasValue && detail.OriginatingAgentId > 0)
            {

                var originatigAgent = new AgentRepository().GetAgentDetail(detail.OriginatingAgentId);
                FrayteShipmentDeliveryNotePdf obj = new FrayteShipmentDeliveryNotePdf();
                obj.ShipmentDetail = new FrayteShipment();
                obj.ShipmentDetail = detail;
                obj.PageStyleSheet = pageStyleSheet;
                obj.Agent = originatigAgent;
                obj.BootStrapStyleSheet = bootStrapStyleSheet;
                obj.ImgSrc = imgSrc;

                if (detail != null && detail.ShipmentDetails != null && detail.ShipmentDetails.Count > 0)
                {
                    obj.TotalPieces = detail.ShipmentDetails.Sum(x => x.Pieces);

                    var tWeight = GetTotalVolWeight(detail.ShipmentDetails);
                    obj.TotalWeight = String.Format("{0:0.##}", tWeight);

                    var tkgs = detail.ShipmentDetails.Sum(x => x.WeightKg);
                    obj.TotalKgs = String.Format("{0:0.##}", tkgs);
                }
                else
                {
                    obj.TotalPieces = 0;
                    obj.TotalWeight = "0.00";
                    obj.TotalKgs = "0.00";
                }
                if (detail != null && detail.ShippingTime != null)
                {
                    var declarvalue = detail.ShippingTime.ToString();
                    if (declarvalue.Contains('.'))
                    {
                        var arr = declarvalue.Split('.');
                        obj.DeclaredValue = arr[0];
                    }
                }

                if (detail != null && detail.ShipmentReadyBy != null)
                {
                    if (detail.ShippingTime.Contains(':'))
                        obj.ShipmentDetail.ShippingTime = detail.ShippingTime;
                    else
                    {
                        var str1 = detail.ShippingTime.Substring(0, 2);
                        var str2 = detail.ShippingTime.Substring(2, 2);
                        obj.ShipmentDetail.ShippingTime = str1 + ":" + str2;
                    }
                }
                string template = File.ReadAllText(HttpContext.Current.Server.MapPath("~/Templates/") + "DeliveryNote.cshtml");
                var EmailBody = Engine.Razor.RunCompile(template, "DeliveryNote", null, obj);
                string pdfFileName = "DeliveryNote_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss") + ".html";
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

                string pdfFile = PDFGenerator.HtmlToPdf("~/UploadFiles/PDFGenerator", "DeliveryNote_" + shipmentId, lstHtmlFiles.ToArray(), null);

                if (System.IO.File.Exists(pdfFullPath))
                {
                    System.IO.File.Delete(pdfFullPath);
                }
            }
        }

        private decimal GetTotalVolWeight(List<FrayteShipmentDetail> shipmentDetails)
        {
            decimal total = 0;
            foreach (var product in shipmentDetails)
            {
                decimal len = 0, wid = 0, height = 0;

                if (product.Lcms == null)
                {
                    len = 0;
                }
                else
                {
                    len = (decimal)product.Lcms;
                }

                if (product.Wcms == null)
                {
                    wid = 0;
                }
                else
                {
                    wid = (decimal)product.Wcms;
                }

                if (product.Hcms == null)
                {
                    height = 0;
                }
                else
                {
                    height = (decimal)product.Hcms;
                }

                if (len > 0 && wid > 0 && height > 0)
                {
                    total += ((len * wid * height) / 5000);
                }
            }
            return total;
        }

        private void SaveShipmentBarcode(int ShipmentId)
        {
            Random random = new Random();
            string bar = string.Empty;

            for (int i = 1; i < 21; i++)
            {
                bar += random.Next(0, 9).ToString();
            }

            BarcodeSettings settings = new BarcodeSettings();
            string data = string.Empty;
            string type = "Code128";
            short fontSize = 8;
            string font = "SimSun";

            if (ShipmentId.ToString().Length == 1)
            {
                data = "FRYT-" + bar + "-00000" + ShipmentId;
            }
            else if (ShipmentId.ToString().Length == 2)
            {
                data = "FRYT-" + bar + "-0000" + ShipmentId;
            }
            else if (ShipmentId.ToString().Length == 3)
            {
                data = "FRYT-" + bar + "-000" + ShipmentId;
            }
            else if (ShipmentId.ToString().Length == 4)
            {
                data = "FRYT-" + bar + "-00" + ShipmentId;
            }
            else if (ShipmentId.ToString().Length == 5)
            {
                data = "FRYT-" + bar + "-0" + ShipmentId;
            }
            else if (ShipmentId.ToString().Length == 6)
            {
                data = "FRYT-" + bar + "-" + ShipmentId;
            }
            else if (ShipmentId.ToString().Length > 6)
            {
                data = "FRYT-" + bar + "-" + ShipmentId;
            }

            settings.Data2D = data;
            settings.Data = data;
            settings.Type = (BarCodeType)Enum.Parse(typeof(BarCodeType), type);

            if (fontSize != 0 && fontSize.ToString().Length > 0 && Int16.TryParse(fontSize.ToString(), out fontSize))
            {
                if (font != null && font.Length > 0)
                {
                    settings.TextFont = new System.Drawing.Font(font, fontSize, FontStyle.Bold);
                }
            }
            short barHeight = 15;
            if (barHeight != 0 && barHeight.ToString().Length > 0 && Int16.TryParse(barHeight.ToString(), out barHeight))
            {
                settings.BarHeight = barHeight;
            }

            BarCodeGenerator generator = new BarCodeGenerator(settings);
            Image barcode = generator.GenerateImage();
            string filePathToSave = GetShipmentDocumentsSavePath(ShipmentId);
            barcode.Save(filePathToSave + "barcode_" + ShipmentId + ".Png");

            // Save  BarCodeSo 
            new ShipmentRepository().SaveBarCodeSO(ShipmentId, data);
        }

        #endregion
    }
}