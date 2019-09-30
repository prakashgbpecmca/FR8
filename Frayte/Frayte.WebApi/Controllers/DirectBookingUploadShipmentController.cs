using Frayte.Services.Business;
using Frayte.Services.Models;
using Frayte.Services.Utility;
using Report.Generator.ManifestReport;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace Frayte.WebApi.Controllers
{
    public class DirectBookingUploadShipmentController : ApiController
    {
        #region Direct Booking Upload Booking Save & Update

        [HttpPost]
        public IHttpActionResult SelectService(List<FrayteeCommerceUserShipment> UploadShipment)
        {

            List<FrayteUploadshipment> Shipments = new List<FrayteUploadshipment>();

            foreach (var a in UploadShipment)
            {
                //if (a.ServiceCode != null && a.ServiceCode != "")
                //{
                var res = new DirectBookingUploadShipmentRepository().GetShipmentFromDraft(a.ShipmentId, "DirectBooking_SS");
                //var result = new DirectBookingUploadShipmentRepository().UpdateServiceCode(a);
                Shipments.Add(res);
                //}
            }
            new eCommerceUploadShipmentRepository().RemoveBatchProcessShipment(Shipments[0].CustomerId, Shipments.Count);
            string ShipmentIds = "";
            int i = 0;

            if (Shipments.Count >= 5)
            {
                int len = Shipments.Count;
                foreach (var a in Shipments)
                {
                    if (len >= 5)
                    {
                        if (i < 4)
                        {
                            ShipmentIds = ShipmentIds + a.DirectShipmentDraftId.ToString() + "|";
                        }
                        else if (i == 4)
                        {
                            ShipmentIds = ShipmentIds + a.DirectShipmentDraftId.ToString() + "," + 0 + "_" + "DirectBooking";
                        }

                        //ShipmentIds = a.DirectShipmentDraftId.ToString() + "," + 0 + "_" + "DirectBooking";

                        i++;
                        if (i == 5)
                        {
                            Process.Start(AppSettings.eCommerceUploadShipmentBatchProcess, ShipmentIds);
                            ShipmentIds = "";
                            len = len - 5;
                            i = 0;
                        }
                    }
                    else
                    {
                        if (i < len - 1)
                        {
                            ShipmentIds = ShipmentIds + a.DirectShipmentDraftId.ToString() + "|";
                        }
                        else if (i == len - 1)
                        {
                            ShipmentIds = ShipmentIds + a.DirectShipmentDraftId.ToString() + "," + 0 + "_" + "DirectBooking";
                        }
                        i++;

                        //Process.Start(AppSettings.eCommerceUploadShipmentBatchProcess, ShipmentIds);
                        if (ShipmentIds.Contains("DirectBooking"))
                        {
                            Process.Start(AppSettings.eCommerceUploadShipmentBatchProcess, ShipmentIds);
                        }

                    }
                }
            }
            else
            {
                foreach (var a in Shipments)
                {
                    int len = Shipments.Count;

                    if (i < len - 1)
                    {
                        ShipmentIds = ShipmentIds + a.DirectShipmentDraftId.ToString() + "|";
                    }
                    else if (i == len - 1)
                    {
                        ShipmentIds = ShipmentIds + a.DirectShipmentDraftId.ToString() + "," + 0 + "_" + "DirectBooking";
                    }
                    i++;
                }
                Process.Start(AppSettings.eCommerceUploadShipmentBatchProcess, ShipmentIds);
            }
            int eCommerceShipmentId = 0;

            return Ok(Shipments);
        }

        public IHttpActionResult PlaceShipmentBatch(List<FrayteUploadshipment> UploadShipment)
        {

            string ShipmentIds = "";
            int i = 0;
            foreach (var a in UploadShipment)
            {
                int len = UploadShipment.Count;
                if (i < len - 1)
                {
                    ShipmentIds = ShipmentIds + a.DirectShipmentDraftId.ToString() + "|";
                }
                else if (i == len - 1)
                {
                    ShipmentIds = ShipmentIds + a.DirectShipmentDraftId.ToString() + "," + 0 + "_" + "DirectBooking";
                }
                i++;
            }
            new DirectBookingUploadShipmentRepository().ErrorLog(UploadShipment, "ECOMMERCE_SS");

            FrayteeCommerceWithServiceShipmentfilter shipmentfilter = new FrayteeCommerceWithServiceShipmentfilter();
            var SucessfulShipments = new List<FrayteUploadshipment>();
            var UnsucessfulShipments = new List<FrayteUploadshipment>();
            foreach (var res in UploadShipment)
            {
                if (res.Errors.Count > 0)
                {
                    UnsucessfulShipments.Add(res);
                }
                else
                {
                    SucessfulShipments.Add(res);
                }
            }
            if (UnsucessfulShipments.Count > 0)
            {
                new eCommerceUploadShipmentRepository().SaveBatchProcessUnprocessedShipment(UploadShipment[0].CustomerId, UnsucessfulShipments.Count);
            }
            //new eCommerceUploadShipmentRepository().RemoveBatchProcessShipment(UploadShipment[0].CustomerId, SucessfulShipments.Count);
            Process.Start(AppSettings.eCommerceUploadShipmentBatchProcess, ShipmentIds);

            int eCommerceShipmentId = 0;

            return Ok(UploadShipment);
        }

        [HttpPost]
        public IHttpActionResult RemoveShipmentWithService(List<FrayteeCommerceUserShipment> UploadShipment)
        {
            foreach (var Shipment in UploadShipment)
            {
                new DirectBookingUploadShipmentRepository().DeleteShipment(Shipment.ShipmentId);
            }
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult DeleteShipment(int DirectShipmentDraftId)
        {
            new DirectBookingUploadShipmentRepository().DeleteShipment(DirectShipmentDraftId);
            return Ok();
        }

        [HttpGet]
        public List<FrayteUserDirectShipment> GetUnSuccessfulShipments(int CustomerId)
        {
            return new eCommerceUploadShipmentRepository().GetUnSuccessfulShipments(CustomerId);
        }

        [HttpGet]
        public List<SessionModel> GetSessionNameList(int UserId)
        {
            return new DirectBookingUploadShipmentRepository().GetSessionNameList(UserId);
        }

        [HttpGet]
        public List<SessionModel> GetSessionList(int UserId)
        {
            return new DirectBookingUploadShipmentRepository().GetSessionList(UserId);
        }

        [HttpGet]
        public List<FrayteUserDirectShipment> GetShipmentList(int SessionId)
        {
            return new DirectBookingUploadShipmentRepository().GetShipmentList(SessionId);
        }

        public List<FrayteUserDirectShipment> GetShipmentDraftList(int SessionId)
        {
            return new DirectBookingUploadShipmentRepository().GetDraftShipmentList(SessionId);
        }

        [HttpGet]
        public UploadShipmentBatchProcess GetUpdatedBatchProcess(int CustomerId)
        {
            return new eCommerceUploadShipmentRepository().GetUpdatedBatchProcess(CustomerId);
        }

        [HttpGet]
        public List<FrayteUploadshipment> GetShipmentErrors(int ShipmentId, string ServiceType)
        {
            return new DirectBookingUploadShipmentRepository().GetShipmentErrors(ShipmentId, ServiceType);
        }

        [HttpGet]
        [AllowAnonymous]
        public List<CountryLogistic> GetServices()
        {
            return new eCommerceUploadShipmentRepository().GetServices();
        }

        [HttpGet]
        public List<FrayteUserDirectShipment> GetShipmentsFromDraft(int CustomerId)
        {
            return new DirectBookingUploadShipmentRepository().GetShipmentsFromDraft(CustomerId);
        }

        [HttpGet]
        public List<LogisticServiceShipment> GetServiceCode()
        {
            return new DirectBookingUploadShipmentRepository().GetServiceCode();
        }

        [HttpGet]
        public FrayteResult SaveServiceCode(string ServiceCode, int DirectShipmentDraftId)
        {
            return new DirectBookingUploadShipmentRepository().SaveServiceCode(ServiceCode, DirectShipmentDraftId);
        }

        public List<FrayteUploadshipment> GetShipmentsFromDraftTable(int CustomerId, string ServiceType)
        {
            return new DirectBookingUploadShipmentRepository().GetShipmentsFromDraftTable(CustomerId, ServiceType);
        }

        [HttpPost]
        public fileName GenerateUnsucessfulShipmentsWithServcie(List<FrayteUserDirectShipment> Shipments)
        {

            fileName fN = new fileName();
            var flag = true;
            //fN.FilePath = @"D:\ProjectFrayte\Frayte\Frayte.WebApi\Manifestedshipments\";
            //if (ServiceType == "ECOMMERCE_WS")
            //{
            fN.FilePath = AppSettings.WebApiPath + "Manifestedshipments/Incomplete Shipments With Courier Service Download.csv";
            fN.FileName = "Incomplete Shipments With Courier Service Download";
            //}
            //else if (ServiceType == "ECOMMERCE_SS")
            //{
            //    fN.FilePath = AppSettings.WebApiPath + "Manifestedshipments/UnsuccessfulShipmentWithService.csv";
            //    fN.FileName = "UnsuccessfulShipmentWithService";
            //}

            //var gmd = GetShipmentsFromDraftTable(CustomerId, ServiceType);

            List<FrayteUploadshipment> FUS = new List<FrayteUploadshipment>();
            //List<FrayteUploadshipment> FUSNew = new List<FrayteUploadshipment>();
            foreach (var Shipment in Shipments)
            {
                var gmd = new DirectBookingUploadShipmentRepository().GetShipmentErrors(Shipment.ShipmentId, "DirectBooking_SS");
                var res = gmd[0];
                FUS.Add(res);
            }

            //new eCommerceUploadShipmentRepository().ErrorLog(gmd, ServiceType);
            //List<FrayteUploadshipment> FUS = new List<FrayteUploadshipment>();
            //foreach (var g in gmd)
            //{
            //    if (g.Errors.Count > 0)
            //    {
            //        FUS.Add(g);
            //    }
            //}

            var name = new DirectBookingUploadShipmentRepository().UnsuccessfulExcelWrite(FUS, "ECOMMERCE_SS");
            if (fN.FileName.Contains(".csv"))
            {
                fN.FileName = fN.FileName;
            }
            else
            {
                fN.FileName = fN.FileName + ".csv";
            }
            //fN.FilePath = filePath;

            if (name.Message == "True")
            {
                foreach (var Shipment in Shipments)
                {
                    new DirectBookingUploadShipmentRepository().DeleteShipment(Shipment.ShipmentId);
                }
            }

            return fN;
        }

        [HttpPost]
        public fileName GenerateUnsucessfulShipmentsWithoutService(List<FrayteUserDirectShipment> ShipmentsWithoutService)
        {

            fileName fN = new fileName();
            var flag = true;
            //fN.FilePath = @"D:\ProjectFrayte\Frayte\Frayte.WebApi\Manifestedshipments\";

            fN.FilePath = AppSettings.WebApiPath + "Manifestedshipments/Incomplete Shipments Without Courier Service Download.csv";
            fN.FileName = "Incomplete Shipments Without Courier Service Download";

            List<FrayteUploadshipment> FUS = new List<FrayteUploadshipment>();
            List<FrayteUploadshipment> FUSNew = new List<FrayteUploadshipment>();
            foreach (var Shipment in ShipmentsWithoutService)
            {
                var gmd = new DirectBookingUploadShipmentRepository().GetShipmentErrors(Shipment.ShipmentId, "ECOMMERCE_WS");
                var res = gmd[0];
                FUS.Add(res);
            }
            //new eCommerceUploadShipmentRepository().ErrorLog(gmd, ServiceType);

            //foreach (var g in FUS)
            //{
            //    if (g.Errors.Count > 0)
            //    {
            //        FUSNew.Add(g);
            //    }
            //}

            var Result = new DirectBookingUploadShipmentRepository().UnsuccessfulExcelWrite(FUS, "ECOMMERCE_WS");

            if (fN.FileName.Contains(".csv"))
            {
                fN.FileName = fN.FileName;
            }
            else
            {
                fN.FileName = fN.FileName + ".csv";
            }
            //fN.FilePath = filePath;
            if (Result.Message == "True")
            {
                foreach (var Shipment in ShipmentsWithoutService)
                {
                    new DirectBookingUploadShipmentRepository().DeleteShipment(Shipment.ShipmentId);
                }
            }

            return fN;
        }

        [HttpPost]
        public HttpResponseMessage DownloadUnsucessfulShipments(FrayteCommercialInvoiceFileName FileName)
        {
            try
            {
                if (FileName != null && !string.IsNullOrEmpty(FileName.FileName))
                {
                    return UnsucessfulShipments(FileName.FileName);
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

        private HttpResponseMessage UnsucessfulShipments(string FileName)
        {
            string filePath = HttpContext.Current.Server.MapPath("~/Manifestedshipments/" + FileName);
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
                    httpResponseMessage.Content.Headers.Add("x-filename", FileName);
                    httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    httpResponseMessage.Content.Headers.ContentDisposition.FileName = FileName;
                    httpResponseMessage.StatusCode = HttpStatusCode.OK;
                    return httpResponseMessage;
                }
            }
        }

        [HttpPost]
        public IHttpActionResult UploadShipments(int CustomerId, string LogisticService, string ServiceType)
        {
            //int eCommerceShipmentId = 0;
            List<FrayteUploadshipment> frayteShipment = new List<FrayteUploadshipment>();
            FrayteResult result = new FrayteResult();
            var httpRequest = HttpContext.Current.Request;

            if (httpRequest.Files.Count > 0)
            {
                HttpFileCollection files = httpRequest.Files;

                HttpPostedFile file = files[0];

                FrayteSession s = new FrayteSession();
                var a = s.EmployeeId;
                if (!string.IsNullOrEmpty(file.FileName))
                {

                    string connString = "";
                    OleDbConnection conn;
                    string filename = DateTime.Now.ToString("MM_dd_yyyy_hh_mm_ss_") + file.FileName;
                    string filepath = HttpContext.Current.Server.MapPath("~/UploadFiles/Shipments/" + filename);
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
                                using (conn = new OleDbConnection(connString))
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
                                using (conn = new OleDbConnection(connString))
                                {
                                    conn.Open();
                                    DataTable dbSchema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                                    string firstSheetName = dbSchema.Rows[0]["TABLE_NAME"].ToString();
                                    var query = "SELECT * FROM " + "[" + firstSheetName + "]";//[Sheet1$]";
                                    using (var adapter = new OleDbDataAdapter(query, conn))
                                    {
                                        adapter.Fill(ds, "Pieces");
                                    }
                                }

                            }

                            var exceldata = ds.Tables[0];


                            if (exceldata != null)
                            {
                                var res = new DirectBookingUploadShipmentRepository().CheckValidWithServiceExcel(exceldata);

                                if (res)
                                {
                                    frayteShipment = new DirectBookingUploadShipmentRepository().GetAllShipments(exceldata, ServiceType, LogisticService);
                                    var flag = false;

                                    foreach (var shipment in frayteShipment)
                                    {
                                        shipment.CollectionTime = shipment.CollectionTime != "" ? new DirectBookingUploadShipmentRepository().ConvertStringToTime(shipment.CollectionTime) : "";
                                        shipment.CollectionDate = shipment.CollectionDate != "" ? new DirectBookingUploadShipmentRepository().ConvertStringToDate(shipment.CollectionDate) : "";
                                        var count = 0;
                                        var packagelist = new List<UploadShipmentPackage>();
                                        var package = new List<UploadShipmentPackage>();
                                        packagelist.AddRange(shipment.Package);
                                        shipment.Package.RemoveRange(0, shipment.Package.Count);
                                        foreach (var ship in packagelist)
                                        {
                                            
                                            if (ship.CartoonValue == 0 && ship.Content == "" && Convert.ToInt32(ship.Height) == 0 &&
                                                Convert.ToInt32(ship.Length) == 0 && Convert.ToInt32(ship.Value) == 0 &&
                                                Convert.ToInt32(ship.Weight) == 0 && Convert.ToInt32(ship.Width) == 0)
                                            {
                                                //shipment.Package.RemoveAt((packagelist.Count - 1) - count);
                                            }
                                            else
                                            {
                                                shipment.Package.Add(ship);
                                            }
                                            count++;
                                        }
                                    }
                                    
                                    // check the validation in the excel
                                    new DirectBookingUploadShipmentRepository().ErrorLog(frayteShipment, ServiceType);

                                    List<FrayteUploadshipment> FUS = new List<FrayteUploadshipment>();
                                    int count1 = 1;

                                    foreach (var shipment in frayteShipment)
                                    {
                                        shipment.ValidationErrors = new List<string>();
                                        if (shipment.ShipFrom.Country == null || (shipment.ShipFrom.Country != null && shipment.ShipFrom.Country.CountryId == 0))
                                        {
                                            shipment.ValidationErrors.Add("From Country is Missing in row " + count1);
                                            flag = true;
                                        }
                                        if (shipment.ShipTo.Country == null || (shipment.ShipTo.Country != null && shipment.ShipTo.Country.CountryId == 0))
                                        {
                                            shipment.ValidationErrors.Add("To Country is Missing in row " + count1);
                                            flag = true;
                                        }
                                        if (shipment.CurrencyCode == null || shipment.CurrencyCode == "")
                                        {
                                            shipment.ValidationErrors.Add("Currency is Missing in row " + count1);
                                            flag = true;
                                        }
                                        count1++;
                                    }
                                    var count2 = 0;
                                    foreach (var shipment in frayteShipment)
                                    {
                                        if (shipment.ValidationErrors.Count > 0)
                                        {
                                            count2++;
                                        }
                                    }
                                    var SessionId = 0;
                                    //if (frayteShipment.Count == count2)
                                    //{

                                    //}
                                    //else
                                    //{
                                    //    SessionId = new DirectBookingUploadShipmentRepository().SaveSession();
                                    //}
                                    //int Count = 1;
                                    //foreach (var shipment in frayteShipment)
                                    //{

                                    //    if (CustomerId > 0)
                                    //    {
                                    //        shipment.CustomerId = CustomerId;
                                    //        shipment.ShipmentStatusId = 14;
                                    //    }
                                    //    if (SessionId > 0)
                                    //    {
                                    //        shipment.SessionId = SessionId;
                                    //    }
                                    //    if (shipment.ValidationErrors.Count == 0 || (
                                    //        !shipment.ValidationErrors[shipment.ValidationErrors.Count - 1].Contains("Currency is Missing in row") &&
                                    //        !shipment.ValidationErrors[shipment.ValidationErrors.Count - 1].Contains("To Country is Missing in row") &&
                                    //        !shipment.ValidationErrors[shipment.ValidationErrors.Count - 1].Contains("From Country is Missing in row")))
                                    //    {
                                    //        result = new DirectBookingUploadShipmentRepository().SaveShipment(shipment, CustomerId, ServiceType);
                                    //    }
                                    //    if (shipment.Errors.Count == 0)
                                    //    {
                                    //        FUS.Add(shipment);
                                    //    }
                                    //    Count++;
                                    //}

                                    if (!flag)
                                    {
                                        SessionId = new DirectBookingUploadShipmentRepository().SaveSession(CustomerId);
                                        foreach (var shipment in frayteShipment)
                                        {

                                            if (CustomerId > 0)
                                            {
                                                shipment.CustomerId = CustomerId;
                                                shipment.ShipmentStatusId = 14;
                                            }
                                            if (SessionId > 0)
                                            {
                                                shipment.SessionId = SessionId;

                                            }

                                            if (shipment.PackageCalculationType != null && shipment.PackageCalculationType != "" && shipment.PackageCalculationType.ToLower() == "kgtocms")
                                            {
                                                shipment.PackageCalculationType = "kgToCms";
                                            }
                                            else if (shipment.PackageCalculationType != null && shipment.PackageCalculationType != "" && shipment.PackageCalculationType.ToLower() == "lbtoinchs")
                                            {
                                                shipment.PackageCalculationType = "lbToInchs";
                                            }

                                            result = new DirectBookingUploadShipmentRepository().SaveShipment(shipment, CustomerId, ServiceType);
                                        }

                                    }
                                    else
                                    {
                                        return BadRequest("from country or to country or shipmentcurrency name is missing");
                                    }
                                    //var Unprocessedshipmentcount = frayteShipment.Count - FUS.Count;
                                    //new eCommerceUploadShipmentRepository().RemoveBatchProcessShipment(frayteShipment[0].CustomerId, frayteShipment.Count);
                                    //new eCommerceUploadShipmentRepository().SaveBatchProcessUnprocessedShipment(Unprocessedshipmentcount, frayteShipment[0].CustomerId);
                                    //if (FUS.Count > 0)
                                    //{
                                    //    PlaceShipmentBatch(FUS);
                                    //}
                                }
                                else
                                {
                                    return BadRequest("CSV file not valid, missing header name or may be wrong name");
                                }
                            }
                            conn.Close();
                            if ((System.IO.File.Exists(filepath)))
                            {
                                System.IO.File.Delete(filepath);
                            }
                        }
                        else
                        {
                            //frayteShipmentDetailexcel.Message = "Excel file not valid";
                        }
                    }
                    catch (Exception ex)
                    {
                        return BadRequest();
                    }
                    finally
                    {

                    }
                }
            }

            var aa = GetUnSuccessfulShipments(CustomerId);

            return Ok(frayteShipment);

        }

        [HttpPost]
        public IHttpActionResult FrayteUploadShipments(FrayteUploadshipment Shipment)
        {

            var GetShipmentInDraftModel = new DirectBookingUploadShipmentRepository().eCommerceUploadShipmentDraft(Shipment, Shipment.CustomerId);

            var a = new eCommerceUploadShipmentController().SaveEcommUploadShipment(GetShipmentInDraftModel);
            return Ok();

        }

        [HttpGet]
        public FratyteError GetFrayteError(int DirectShipmentDraftId)
        {
            return new DirectBookingUploadShipmentRepository().GetFrayteError(DirectShipmentDraftId);
        }


        //[HttpPost]
        //public IHttpActionResult SaveEcommUploadShipment(FrayteCommerceUploadShipmentDraft Shipment)
        //{
        //    //Finally Save the Order Id too to get the information of multiple shpment in Tracking Detail page.
        //    var getId = new eCommerceUploadShipmentRepository().SaveEcommFormUploadShipment(Shipment);

        //    //Change the model
        //    var GetShipmentInEcommDraftModel = new eCommerceUploadShipmentRepository().eCommerceChangeModelShipmentDraft(Shipment, Shipment.CustomerId);

        //    //EasyPost label
        //    string labelPath = HttpContext.Current.Server.MapPath("~/PackageLabel/eCommerce/" + getId + "/");

        //    int calculatedHeight = 1400;
        //    int LocationY = 0;

        //    //After creating an order need to save the information in database

        //    var shipmentDetails = new eCommerceShipmentRepository().GetPackageDetails(getId);
        //    int increment = 0;
        //    for (int j = 0; j < Shipment.Packages.Count; j++)
        //    {
        //        for (int i = 0; i < Shipment.Packages[j].CartoonValue; i++)
        //        {
        //            increment++;
        //            var data = shipmentDetails[j];

        //            // Step 1 : database entry 
        //            var trackingDetail = new eCommerceShipmentRepository().SaveEasyPostDetailTrackingDeatil(GetShipmentInEcommDraftModel, GetShipmentInEcommDraftModel.Packages[j], data, GetShipmentInEcommDraftModel.TrackingCode, null, getId, increment);

        //            //  Step 2: Create frayte AWB label and save label name
        //            string awbLabelName = generateAWBLabel(getId, GetShipmentInEcommDraftModel, increment, GetShipmentInEcommDraftModel.Packages[j]);
        //            new eCommerceShipmentRepository().SaveAWBLabelName(trackingDetail, awbLabelName + ".pdf");


        //        }

        //    }

        //    return Ok();
        //}

        //[HttpPost]

        //public IHttpActionResult SaveShipmentDraftForm(FrayteCommerceUploadShipmentDraft Shipment)
        //{
        //    //Change the model
        //    var GetShipmentInEcommDraftModel = new eCommerceUploadShipmentRepository().eCommerceModelChangeFrayteUploadShipment(Shipment, Shipment.CustomerId);
        //    var res = SaveShipmentDraft(GetShipmentInEcommDraftModel);

        //    return Ok();
        //}
        [HttpPost]
        public IHttpActionResult SaveShipmentDraftForm(DirectBookingShipmentDraftDetail directBookingDetail)
        {
            // Change the model
            var GetShipmentInEcommDraftModel = new DirectShipmentRepository().SaveDirectBooking(directBookingDetail);
            return Ok(GetShipmentInEcommDraftModel);
        }

        //[HttpPost]
        //public IHttpActionResult SaveShipmentDraftForm(DirectBookingShipmentDraftDetail12 directBookingDetail)
        //{
        //    //Change the model
        //    //var GetShipmentInEcommDraftModel = new DirectShipmentRepository().SaveDirectBooking(directBookingDetail);
        //    //var res = SaveShipmentDraft(GetShipmentInEcommDraftModel);

        //    return Ok();
        //}

        [HttpPost]
        public IHttpActionResult SaveShipmentDraft(FrayteUploadshipment Shipment)
        {
            var result = new DirectBookingUploadShipmentRepository().SaveShipment(Shipment, Shipment.CustomerId, "DirectBooking_SS");
            return Ok();
        }

        [HttpGet]
        public List<FrayteServiceCode> GetLogisticServiceCode(int OperationZoneId, int CustomerId)
        {
            List<FrayteServiceCode> _code = new List<FrayteServiceCode>();
            _code = new DirectBookingUploadShipmentRepository().GetLogisticServiceCode(OperationZoneId, CustomerId);
            return _code;
        }

        #endregion

        [HttpGet]
        public bool SaveIsSessionPrint(int SessionId)
        {
            return new DirectBookingUploadShipmentRepository().SaveIsSessionPrint(SessionId);
        }
    }
}
