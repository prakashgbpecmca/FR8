using Frayte.Services.Business;
using Frayte.Services.Models;
using Frayte.Services.Models.Tradelane;
using Frayte.Services.Utility;
using LinqToExcel;
using Report.Generator.ManifestReport;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace Frayte.WebApi.Controllers
{
    public class TradelaneBookingController : ApiController
    {
        #region Shipment Initials

        [HttpGet]
        public HttpResponseMessage BookingInitials(int userId)
        {
            //Step 1: Get Country list
            List<FrayteCountryCode> lstCountry = new List<FrayteCountryCode>();
            lstCountry = new CountryRepository().lstCountry();

            //Step 2: Get Shipment Hanler Methods
            var lstShipmentHandlerMethods = new MasterDataRepository().GetShipmentHandlerMethod();

            //Step 3: Get Currency
            var lstCurrencyTypes = new MasterDataRepository().GetCurrencyType();

            //Step4: Get Customer Detail
            var customerDetail = new TradelaneBookingRepository().GetCustomerDetail(userId);
            //Step4.1: Get Customer Detail
            var customerAddress = new TradelaneBookingRepository().UserDefaultAddresses(userId);

            // Step 5 : Get Country PhoneCode
            List<FrayteCountryPhoneCode> lstCountryPhones = new List<FrayteCountryPhoneCode>();

            lstCountryPhones = new CountryRepository().GetCountryPhoneCodeList();

            // step:6 get all operation zones
            var OperaionZones = new OperationZoneRepository().GetOperationZone();

            // step:7 get all Airlines 
            var listAirlines = new MasterDataRepository().GetAirlines();

            var timeZones = new TimeZoneRepository().GetShipmentTimeZones();

            var lstincoterm = new BreakBulkRepository().GetIncoterms();

            return this.Request.CreateResponse(
                HttpStatusCode.OK,
                new
                {
                    Countries = lstCountry,
                    CurrencyTypes = lstCurrencyTypes,
                    ShipmentMethods = lstShipmentHandlerMethods,
                    CustomerDetail = customerDetail,
                    CustomerAddress = customerAddress,
                    CountryPhoneCodes = lstCountryPhones,
                    AirLines = listAirlines,
                    TimeZones = timeZones,
                    OperaionZones = OperaionZones,
                    Incoterm = lstincoterm
                });
        }

        [HttpGet]
        public List<TradelaneAirport> AirPorts(int countryId)
        {
            List<TradelaneAirport> list = new MasterDataRepository().GetAirports(countryId);
            return list;
        }

        public List<TradelaneCustomer> GetCustomers(int userId, string moduleType)
        {
            try
            {
                List<TradelaneCustomer> customers = new TradelaneBookingRepository().GetCustomers(userId, moduleType);
                return customers;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet]
        public TradelaneCustomerDefaultAddress UserDefaultAddresses(int userId)
        {
            try
            {
                return new TradelaneBookingRepository().UserDefaultAddresses(userId);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }
        }
        #endregion

        #region  Booking

        [HttpPost]
        public IHttpActionResult PlaceBooking(TradelaneBooking shipment)
        {
            shipment = new TradelaneBookingRepository().SaveShipment(shipment, "");

            if (shipment.ShipmentStatusId != (int)FrayteTradelaneShipmentStatus.Draft)
            {
                TradelaneEmailModel model = new TradelaneEmailRepository().TradelaneEmailObj(shipment.TradelaneShipmentId);
                // Fill all the required info in the email model

                model.ShipmentDetail.ShipFrom.IsMailSend = shipment.ShipFrom.IsMailSend;
                model.ShipmentDetail.ShipTo.IsMailSend = shipment.ShipTo.IsMailSend;
                model.ShipmentDetail.NotifyParty.IsMailSend = shipment.NotifyParty.IsMailSend;

                new TradelaneEmailRepository().FillBookingInformationEmailModel(model);

                if (shipment.ShipmentStatusId == (int)FrayteTradelaneShipmentStatus.Pending)
                {
                    // Send Shipment info email created by  
                    new TradelaneEmailRepository().SendEmail_E1(model);

                    // Send Booking Confirmation email to customer
                    new TradelaneEmailRepository().FillBookingConfirmationEmailModel(model);

                    new TradelaneEmailRepository().SendEmail_E2(model);
                }
                else
                {
                    // Customer Confirmation email
                    new TradelaneEmailRepository().SendEmail_E2_1(model);

                    // Send MAWB AllocationEMail
                    new TradelaneEmailRepository().FillModel_E3(model);

                    new TradelaneEmailRepository().SendEmail_E3(model);

                    // Add shipment tracking  
                    new TradelaneBookingRepository().SaveShipmentTracking(shipment);

                    //  Add FLight Detail  
                    new TradelaneBookingRepository().SaveShipmentFlightDetail(shipment);
                }
            }
            return Ok(shipment);
        }

        [HttpPost]
        public IHttpActionResult CustomerAction(FrayteCustomerActionShippment confirmationDetail)
        {
            try
            {
                int shipmentId = new TradelaneBookingRepository().CustomerAction(confirmationDetail);

                if (shipmentId > 0)
                {
                    TradelaneEmailRepository shipmentEmailRepo = new TradelaneEmailRepository();

                    TradelaneEmailModel emailModel = shipmentEmailRepo.TradelaneEmailObj(shipmentId);

                    if (emailModel != null)
                    {
                        new TradelaneEmailRepository().FillBookingInformationEmailModel(emailModel);

                        if (emailModel.ShipmentDetail.ShipmentStatusId == (int)FrayteTradelaneShipmentStatus.ShipmentBooked)
                        {
                            return Ok("AlreadyConfirmed");
                        }
                        else if (emailModel.ShipmentDetail.ShipmentStatusId == (int)FrayteTradelaneShipmentStatus.Rejected)
                        {
                            return Ok("AlreadyRejected");
                        }
                        else
                        {
                            if (confirmationDetail.ActionType == ShipmentCustomerAction.Confirm)
                            {
                                // set  shipment confirm status
                                new TradelaneBookingRepository().SetShipmentStatus(shipmentId, confirmationDetail.ActionType);

                                //Customer Confirmation email
                                shipmentEmailRepo.SendEmail_E2_1(emailModel);

                                //Thank you email to customer  
                                shipmentEmailRepo.SendEmail_E2_3(emailModel);

                                //MAWB allocation email 
                                shipmentEmailRepo.SendEmail_E3(emailModel);

                                //Add shipment tracking
                                new TradelaneBookingRepository().SaveShipmentTracking(emailModel.ShipmentDetail);

                                //Add FLight Detail
                                new TradelaneBookingRepository().SaveShipmentFlightDetail(emailModel.ShipmentDetail);
                            }
                            else if (confirmationDetail.ActionType == ShipmentCustomerAction.Reject)
                            {
                                //Set shipment rejected status
                                new TradelaneBookingRepository().SetShipmentStatus(shipmentId, confirmationDetail.ActionType);

                                //Customer Rejection email 
                                shipmentEmailRepo.SendEmail_E2_2(emailModel);
                            }
                            return Ok();
                        }
                    }
                    return BadRequest();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        #endregion

        #region Shipment Detail

        [HttpGet]
        public TradelaneBooking TradelaneBookingDetails(int shipmentId, string callingType)
        {
            try
            {
                TradelaneBooking shipment = new TradelaneBookingRepository().GetTradelaneBookingDetails(shipmentId, callingType);
                return shipment;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet]
        public List<TradelanePreAlertDocument> TradelaneShipmentDocuments(int userId, int shipmentId)
        {
            try
            {
                return new TradelaneBookingRepository().TradelaneShipmenDocuments(userId, shipmentId);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #region Shipment Packages HAWB

        [HttpPost]
        public List<TrackTradelanePackage> GetShipmentPackages(TrackPackage track)
        {
            List<TrackTradelanePackage> list = new TradelaneBookingRepository().TrackTradelanePackages(track);
            return list;
        }

        [HttpGet]
        public FrayteHAWBAddress GetHAWBAddress(string HAWB)
        {
            return new TradelaneBookingRepository().GetHAWBAddress(HAWB);
        }

        [HttpGet]
        public TradelanePackageWeightObj GetTradelanePackageWeight(int tradelaneShipmentId)
        {
            TradelanePackageWeightObj obj = new TradelaneBookingRepository().TrackTradelanePackages(tradelaneShipmentId);
            return obj;
        }

        [HttpPost]
        public FrayteResult AssigneedHAWB(List<TrackTradelanePackage> packages)
        {
            FrayteResult result = new FrayteResult();

            result = new TradelaneBookingRepository().AssigneedHAWB(packages);

            return result;
        }

        [HttpPost]
        public IHttpActionResult SaveHAWBAddress(FrayteHAWBAddress hawbaddress)
        {
            return Ok(new TradelaneBookingRepository().SaveHAWBAddress(hawbaddress));
        }

        [HttpGet]
        public TradelaneMappedUnMappedShipment IsAllHawbAssigned(int tradelaneShipmentId)
        {
            TradelaneMappedUnMappedShipment result = new TradelaneMappedUnMappedShipment();
            result = new TradelaneBookingRepository().IsAllHawbAssigned(tradelaneShipmentId);
            return result;
        }

        [HttpGet]
        public List<HAWBTradelanePackage> GetGroupedHAWBPieces(int tradelaneShipmentId)
        {
            try
            {
                return new TradelaneBookingRepository().GetGroupedHAWBPieces(tradelaneShipmentId);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return null;
            }
        }

        [HttpGet]
        public FrayteResult UpdateHAWBNumber(int shipmentId, int hawbNumber)
        {
            FrayteResult result = new FrayteResult();
            result = new TradelaneBookingRepository().UpdateHAWBNumber(shipmentId, hawbNumber);
            return result;
        }

        #endregion

        #endregion

        #region MAWB

        [HttpPost]
        public IHttpActionResult ShipmentMAWBAsHAWB(ShipmentMAWBAsHAWBModel obj)
        {
            try
            {
                if (obj != null)
                {
                    bool status = new TradelaneBookingRepository().ShipmentMAWBAsHAWB(obj);
                    return Ok(status);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return BadRequest();
            }
        }

        [HttpGet]
        public TradelaneReportMAWBModel MAWBPreview(int tradelaneShipmentId)
        {
            TradelaneReportMAWBModel mawbDetail = new TradelaneReportsRepository().MAWBReportObj(tradelaneShipmentId);
            return mawbDetail;
        }

        [HttpPost]
        public IHttpActionResult ChangeMAWBAddress(TradelBookingAdress address)
        {
            try
            {
                FrayteResult result = new TradelaneBookingRepository().ChangeMAWBAddress(address);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        #endregion

        #region  Documents  

        [HttpGet]
        public IHttpActionResult RemoveTradelaneDocument(int TradelaneShipmentDocumentId)
        {
            try
            {
                FrayteResult result = new FrayteResult();
                result = new TradelaneBookingRepository().RemoveTradelaneDocument(TradelaneShipmentDocumentId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public IHttpActionResult UploadBatteryForms()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;

                ShipmentRepository shipmentRepository = new ShipmentRepository();

                HttpFileCollection files = httpRequest.Files;
                int ShipmentId = Convert.ToInt32(httpRequest.Form["ShipmentId"].ToString());
                int UserId = Convert.ToInt32(httpRequest.Form["UserId"].ToString());
                var docfiles = new List<string>();
                string filePathToSave = string.Format(FrayteTradelaneDocumentPath.ShipmentDocument, ShipmentId);
                string fileFullPath = "";
                filePathToSave = HttpContext.Current.Server.MapPath(filePathToSave);

                if (!System.IO.Directory.Exists(filePathToSave))
                    System.IO.Directory.CreateDirectory(filePathToSave);

                //This code will execute only when user will upload the document
                if (files.Count > 0)
                {
                    HttpPostedFile file = files[0];
                    if (!string.IsNullOrEmpty(file.FileName))
                    {
                        if (!File.Exists(filePathToSave + file.FileName))
                        {
                            //Save in server folder
                            fileFullPath = filePathToSave + file.FileName;
                            file.SaveAs(fileFullPath);

                            //Save file name and other information in DB
                            FrayteResult result = new TradelaneBookingRepository().SaveShipmentDocument(ShipmentId, FrayteShipmentDocumentType.BatteryForm, file.FileName, UserId);
                            return Ok(result);
                        }
                        else
                        {
                            return BadRequest("BatteryForm");
                        }
                    }
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public TradelanePackageDetailExcel GetPiecesDetailFromExcel()
        {
            TradelanePackageDetailExcel frayteShipmentDetailexcel = new TradelanePackageDetailExcel();
            var httpRequest = HttpContext.Current.Request;

            List<TradelanePackage> _shipmentdetail = new List<TradelanePackage>();

            if (httpRequest.Files.Count > 0)
            {
                HttpFileCollection files = httpRequest.Files;
                HttpPostedFile file = files[0];
                int ShipmentId = Convert.ToInt32(httpRequest.Form["ShipmentId"].ToString());

                if (!string.IsNullOrEmpty(file.FileName))
                {
                    //int ShipmentId = Convert.ToInt32(httpRequest.Form["ShipmentId"].ToString());
                    string connString = "";
                    string filename = DateTime.Now.ToString("MM_dd_yyyy_hh_mm_ss_") + file.FileName;
                    string filepath = HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + filename);
                    file.SaveAs(filepath);
                    connString = new DirectShipmentRepository().getExcelConnectionString(filename, filepath);

                    string fileExtension = "";
                    fileExtension = new DirectShipmentRepository().getFileExtensionString(filename);
                    try
                    {
                        if (!string.IsNullOrEmpty(fileExtension))
                        {
                            var ds = new DataSet();
                            if (fileExtension == FrayteFileExtension.CSV)
                            {
                                using (var conn = new OleDbConnection(connString))
                                {
                                    conn.Open();
                                    var query = "SELECT * FROM [" + Path.GetFileName(filename) + "]";
                                    using (var adapter = new OleDbDataAdapter(query, conn))
                                    {
                                        adapter.Fill(ds, "Pieces");
                                    }
                                }
                            }
                            else
                            {
                                var excel = new ExcelQueryFactory("excelFileName");
                                var oldCompanies = (from c in excel.Worksheet<TradelanePackage>("sheet1")
                                                    select c);

                                using (var conn = new OleDbConnection(connString))
                                {
                                    conn.Open();
                                    DataTable dbSchema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                                    string firstSheetName = dbSchema.Rows[0]["TABLE_NAME"].ToString();
                                    var query = "SELECT * FROM " + "[" + firstSheetName + "]";
                                    using (var adapter = new OleDbDataAdapter(query, conn))
                                    {
                                        adapter.Fill(ds, "Pieces");
                                    }
                                }
                            }

                            var exceldata = ds.Tables[0];

                            // Step 1: Validate for excel column

                            string PiecesColumnList = "CartonNumber,CartonQTY,Length,Width,Height,Weight";
                            bool IsExcelValid = UtilityRepository.CheckUploadExcelFormat(PiecesColumnList, exceldata);
                            if (!IsExcelValid)
                            {
                                frayteShipmentDetailexcel.Message = "Columns are not matching with provided template columns. Please check the column names.";
                            }
                            else
                            {
                                if (exceldata.Rows.Count > 0)
                                {
                                    frayteShipmentDetailexcel.TotalUploaded = exceldata.Rows.Count;
                                    _shipmentdetail = new TradelaneBookingRepository().GetPiecesDetail(exceldata);
                                    frayteShipmentDetailexcel.Packages = new List<TradelanePackage>();
                                    frayteShipmentDetailexcel.Packages = _shipmentdetail;
                                    frayteShipmentDetailexcel.Message = "OK";
                                    frayteShipmentDetailexcel.SuccessUploaded = _shipmentdetail.Count;
                                    new TradelaneBookingRepository().SavePackagedetailShipment(frayteShipmentDetailexcel.Packages, ShipmentId);
                                }
                                else
                                {
                                    frayteShipmentDetailexcel.Message = "No records found.";
                                }
                            }
                            if ((System.IO.File.Exists(filepath)))
                            {
                                System.IO.File.Delete(filepath);
                            }
                        }
                        else
                        {
                            frayteShipmentDetailexcel.Message = "Excel file not valid";
                        }
                    }
                    catch (Exception ex)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                        if (ex != null && !string.IsNullOrEmpty(ex.Message) && ex.Message.Contains("Sheet1$"))
                        {
                            frayteShipmentDetailexcel.Message = "Sheet name is invalid.";
                        }
                        else
                        {
                            frayteShipmentDetailexcel.Message = "Error while uploading the excel.";
                        }
                        return frayteShipmentDetailexcel;
                    }
                }
            }
            return frayteShipmentDetailexcel;
        }

        [HttpGet]
        public DirectBookingShipmentDetail GetDirectBookingShipment(string FrayteRefNo)
        {
            return new TradelaneBookingRepository().GetDirectBookingShipment(FrayteRefNo);
        }

        #endregion

        #region   Agent MAWB

        [HttpPost]

        public IHttpActionResult UploadMAWBDocument()
        {
            string status = string.Empty;
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                HttpFileCollection files = httpRequest.Files;
                HttpPostedFile file = files[0];

                if (!string.IsNullOrEmpty(file.FileName))
                {
                    try
                    {
                        int ShipmentId = Convert.ToInt32(httpRequest.Form["ShipmentId"].ToString());
                        int UserId = Convert.ToInt32(httpRequest.Form["UserId"].ToString());
                        string DocType = httpRequest.Form["DocType"].ToString();
                        string filename = file.FileName;
                        string documentType = string.Empty;
                        if (!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + ShipmentId + "/")))
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + ShipmentId + "/"));

                        string fileNameWithoutExtension = new TradelaneBookingRepository().GetFileNameFromString(filename);

                        if (!string.IsNullOrEmpty(fileNameWithoutExtension))
                        {
                            var list = new TradelaneBookingRepository().GetMAWBDocuments(ShipmentId, FrayteShipmentDocumentType.MAWB);
                            string extension = new TradelaneBookingRepository().GetFileExtensionFromString(filename);

                            if (list != null && list[0].RevisionNumber > 1)
                            {
                                filename = fileNameWithoutExtension + "_REV - " + (list[0].RevisionNumber + 1) + "." + extension;
                            }

                            string filepath = HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + ShipmentId + "/" + filename);

                            file.SaveAs(filepath);

                            FrayteResult result = new TradelaneBookingRepository().SaveShipmentDocument(ShipmentId, FrayteShipmentDocumentType.MAWB, filename, UserId);

                            if (result.Status)
                            {
                                status = "Ok";
                            }
                            else
                            {
                                status = "Failed";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    }
                }
            }
            return Ok(status);
        }


        [HttpPost]
        public IHttpActionResult SaveMAWBdetail(TradelaneMAWBDetail mawbDetail)
        {
            try
            {
                new TradelaneBookingRepository().SaveSalesOrderNumber(mawbDetail.HAWBpackages);
                var result = new MawbAllocationRepository().SaveMawbDetail(mawbDetail);

                if (result.Status)
                {
                    result = new TradelaneBookingRepository().UpdateMAWBDetail(mawbDetail);
                    new TradelaneEmailRepository().MawbUpdateMail(mawbDetail.TradelaneShipmentId);
                    return Ok(result);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }

        [HttpPost]
        public IHttpActionResult UploadOtherDocument()
        {
            string status = string.Empty;
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                HttpFileCollection files = httpRequest.Files;
                HttpPostedFile file = files[0];
                if (!string.IsNullOrEmpty(file.FileName))
                {
                    try
                    {
                        int ShipmentId = Convert.ToInt32(httpRequest.Form["ShipmentId"].ToString());
                        int UserId = Convert.ToInt32(httpRequest.Form["UserId"].ToString());
                        string DocType = httpRequest.Form["DocType"].ToString();
                        string filename = file.FileName;
                        string documentType = string.Empty;
                        if (!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + ShipmentId + "/")))
                            System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + ShipmentId + "/"));

                        string filepath = HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + ShipmentId + "/" + filename);

                        if ((System.IO.File.Exists(filepath)))
                        {
                            documentType = new TradelaneBookingRepository().GetFileDocumentType(ShipmentId, filename);
                            status = documentType;
                        }
                        else
                        {
                            file.SaveAs(filepath);
                            FrayteResult result = new TradelaneBookingRepository().SaveShipmentDocument(ShipmentId, FrayteShipmentDocumentType.OtherDocument, filename, UserId);
                            if (result.Status)
                            {
                                status = "Ok";
                            }
                            else
                            {
                                status = "Failed";
                            }

                        }

                    }
                    catch (Exception ex)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    }
                }
            }
            return Ok(status);
        }
        [HttpGet]
        public IHttpActionResult SendMAWBEmail()
        {
            try
            {
                new MawbAllocationRepository().GetMawbUnallocatedShipments();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public IHttpActionResult MAWBDetails(string action)
        {
            try
            {
                TradelaneMAWBDetail detail = new TradelaneMAWBDetail();

                var ads = WebUtility.UrlDecode(action);

                var wsdsd = UtilityRepository.Base64UrlDecode(ads);

                var dfd = UtilityRepository.Decrypt(wsdsd, EncriptionKey.PrivateKey);

                var InitialDetail = new TradelaneBookingRepository().GetDetailsFormString(dfd);

                if (InitialDetail != null && !string.IsNullOrEmpty(InitialDetail.FrayteNumber) &&
                    InitialDetail.AgentId > 0 && InitialDetail.ShipmentMethodHandlerId > 0 &&
                    (string.IsNullOrEmpty(InitialDetail.Leg) || (!string.IsNullOrEmpty(InitialDetail.Leg) || InitialDetail.Leg == "Leg1" || InitialDetail.Leg == "Leg2")))
                {
                    int TradelaneShipmentId = new TradelaneBookingRepository().ValidateMAWBInitial(InitialDetail);

                    bool validAgent = false;
                    if (TradelaneShipmentId > 0)
                    {
                        if (InitialDetail.ShipmentMethodHandlerId != 5)
                        {
                            validAgent = new TradelaneBookingRepository().ValidateAgent(InitialDetail.AgentId, TradelaneShipmentId);
                        }
                        else
                        {
                            validAgent = new TradelaneBookingRepository().ValidateAgent(InitialDetail.AgentId, TradelaneShipmentId, InitialDetail.Leg);
                        }

                        if (validAgent)
                        {

                            bool status = new TradelaneBookingRepository().IsMAWBUploaded(TradelaneShipmentId, InitialDetail.AgentId, InitialDetail.Leg);

                            detail.FrayteNumber = InitialDetail.FrayteNumber;
                            detail.AgentId = InitialDetail.AgentId;
                            detail.TradelaneShipmentId = TradelaneShipmentId;
                            detail.HAWBpackages = GetGroupedHAWBPieces(TradelaneShipmentId);
                            detail.List = new MawbAllocationRepository().GetMawbAllocation(TradelaneShipmentId, InitialDetail.Leg);
                            detail.IsMAWBAllocated = status;
                            return Ok(detail);
                        }
                        else
                        {
                            return BadRequest("SessionExpired");
                        }
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }

        public IHttpActionResult UpdateMAWBDetails(TradelaneMAWBDetail mawbDetail)
        {
            return Ok();
        }
        #endregion

        #region Warehouse Management

        [HttpGet]
        public TradelaneApiScanModel CartonScan(string CartonNumber)
        {
            return new TradelaneBookingRepository().CartonScan(CartonNumber);
        }

        [HttpPost]
        public IHttpActionResult PrintLabel(TradelanePrintLabel PrintLabelObj)
        {
            var Tradelaneshipment = new TradelaneBookingRepository().GetTradelaneDetail(PrintLabelObj);

            if (PrintLabelObj.CartonList != null && Tradelaneshipment != null && Tradelaneshipment.TradelaneShipmentId > 0)
            {
                var ShipmentDetail = new TradelaneBookingRepository().GetTradelaneBookingDetails(Tradelaneshipment.TradelaneShipmentId, "");
                var HawbCount = ShipmentDetail.HAWBPackages.Where(a => a.HAWB == PrintLabelObj.Hawb).FirstOrDefault().PackagesCount;
                var Result = new TradelaneBookingRepository().HawbUpdate(ShipmentDetail, PrintLabelObj);
                var Files = new TradelaneDocument().ShipmentCartonLabel(Tradelaneshipment.TradelaneShipmentId, HawbCount, Tradelaneshipment.TradelaneShipmentDetailId, PrintLabelObj.Hawb);
                TradelanePrintLabelResponseModel model = new TradelanePrintLabelResponseModel();
                string filePhysicalPath = string.Empty;
                model.FileName = Files != null && Files.FileName != null ? Files.FileName : "";
                model.FileExtension = ".png";
                filePhysicalPath = HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + Tradelaneshipment.TradelaneShipmentId + "/" + Files.FileName);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (FileStream file = new FileStream(filePhysicalPath, FileMode.Open, FileAccess.Read))
                    {
                        byte[] bytes = new byte[file.Length];
                        file.Read(bytes, 0, (int)file.Length);
                        ms.Write(bytes, 0, (int)file.Length);

                        //string test = ms.ToString();
                        //StreamReader sr = new StreamReader(filePhysicalPath);
                        //string tsd = sr.ReadToEnd();

                        //var res = File.ReadAllBytes(filePhysicalPath);
                        //string result = System.Text.Encoding.UTF8.GetString(res);

                        HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
                        httpResponseMessage.Content = new ByteArrayContent(bytes);
                        httpResponseMessage.Content.Headers.Add("download-status", "downloaded");
                        httpResponseMessage.Content.Headers.Add("x-filename", model.FileName);
                        httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                        httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                        httpResponseMessage.Content.Headers.ContentDisposition.FileName = model.FileName;
                        httpResponseMessage.StatusCode = HttpStatusCode.OK;
                        model.ByteArray = httpResponseMessage;
                    }
                }

                return Ok(model);
            }
            else
            {
                return BadRequest();
            }

        }

        [HttpPost]
        public IHttpActionResult PrintLabelWithPath(TradelanePrintLabel PrintLabelObj)
        {
            var Tradelaneshipment = new TradelaneBookingRepository().GetTradelaneDetail(PrintLabelObj);

            if (PrintLabelObj.CartonList != null && Tradelaneshipment != null && Tradelaneshipment.TradelaneShipmentId > 0)
            {
                var ShipmentDetail = new TradelaneBookingRepository().GetTradelaneBookingDetails(Tradelaneshipment.TradelaneShipmentId, "");
                var HawbCount = ShipmentDetail.HAWBPackages.Where(a => a.HAWB == PrintLabelObj.Hawb).FirstOrDefault().TotalCartons;
                var Result = new TradelaneBookingRepository().HawbUpdate(ShipmentDetail, PrintLabelObj);
                var Files = new TradelaneDocument().ShipmentCartonLabel(Tradelaneshipment.TradelaneShipmentId, HawbCount, Tradelaneshipment.TradelaneShipmentDetailId, PrintLabelObj.Hawb);
                TradelanePrintLabelResponseModel model = new TradelanePrintLabelResponseModel();
                string filePhysicalPath = string.Empty;
                model.FileName = Files != null && Files.FileName != null ? Files.FileName : "";
                model.FileExtension = ".jpeg"; // AppSettings.WebApiPath + "UploadFiles\\Tradelane\\
                model.FilePath = AppSettings.WebApiPath + "UploadFiles\\Tradelane\\" + Tradelaneshipment.TradelaneShipmentId + "/" + Files.FileName;
                return Ok(model);
            }
            else
            {
                return BadRequest();
            }

        }

        [HttpGet]
        public TradelaneApiErrorModel ShipmentReceived(string Mawb)
        {
            return new TradelaneBookingRepository().ShipmentReceived(Mawb);
        }

        [HttpGet]
        public TradelaneScannedStatusModel TradelaneScannedStatus(string Mawb)
        {
            return new TradelaneBookingRepository().TradelaneScannedStatus(Mawb);
        }

        [HttpPost]
        public TradelaneApiErrorModel TradelaneUpdateStatus(TradelaneUpdateStatusModel UpdateStatusObj)
        {
            return new TradelaneBookingRepository().TradelaneUpdateStatus(UpdateStatusObj);
        }

        [HttpGet]
        public TradelaneUpdateHawbModel TradelaneHawbTrackingUpdate(string HAWB, string Email)
        {
            return new TradelaneBookingRepository().TradelaneHawbTrackingUpdate(HAWB, Email);
        }

        #endregion

        #region AvinashCode

        [HttpGet]
        public IHttpActionResult AddAirport(int CountryId, string AirportName, string AirportCode)
        {
            return Ok(new TradelaneBookingRepository().SaveAirPort(CountryId, AirportName, AirportCode));
        }

        [HttpGet]
        public IHttpActionResult IsHAWBAddressExist(string HAWBNo)
        {
            try
            {
                return Ok(new TradelaneBookingRepository().HAWBAddressExist(HAWBNo));
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public IHttpActionResult UpdateHAWBAddress(ValidateHAWBAddress address)
        {
            return Ok(new TradelaneBookingRepository().UpdateHAWBAddress(address));
        }

        [HttpGet]
        public List<TradelanePackage> AssignedHAWBDetail(int TradelanshipmentId)
        {
            try
            {
                return new TradelaneBookingRepository().SendAssignedHAWB(TradelanshipmentId);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        public IHttpActionResult SavePcsHAWB(List<TradelanePackage> Pcs)
        {
            return Ok(new TradelaneBookingRepository().SavePcsHAWB(Pcs));
        }

        [HttpGet]
        public IHttpActionResult getHAWB(int TradelaneShipmentId)
        {
            return Ok(new TradelaneBookingRepository().GetHAWBData(TradelaneShipmentId));
        }

        [HttpGet]
        public IHttpActionResult CreateHAWBLabel(string HAWB, int index, int TradelaneShipmentDetailId)
        {
            return Ok(new TradelaneDocument().HAWBLabel(HAWB, index, TradelaneShipmentDetailId));
        }

        [HttpPost]
        public HttpResponseMessage DownloadHAWBLabel(string fileName, int TradelaneShipmentId)
        {
            string filePath = HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + TradelaneShipmentId + "/" + fileName);
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

        [HttpGet]
        public IHttpActionResult CreateDestinationManifest(int TradelaneShipmentId, int userId)
        {
            return Ok(new TradelaneDocument().TadelaneShipmentDriverManifest(TradelaneShipmentId, userId));
        }

        [HttpPost]
        public HttpResponseMessage DownloadDestinationManifest(string fileName, int TradelaneShipmentId)
        {
            string filePath = HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + TradelaneShipmentId + "/" + fileName);
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
    }
}