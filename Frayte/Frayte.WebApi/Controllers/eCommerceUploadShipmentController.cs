using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Frayte.Services.Business;
using Frayte.Services.Models;
using Frayte.Services.Utility;
using System.Web;
using System.Data.OleDb;
using System.Data;
using LinqToExcel;
using Frayte.Services;
using System.IO;
using Newtonsoft.Json;
using EasyPost;
using Report.Generator.ManifestReport;
using RazorEngine.Templating;
using RazorEngine;
using System.Net.Http.Headers;
using System.Diagnostics;

namespace Frayte.WebApi.Controllers
{
    [Authorize]
    public class eCommerceUploadShipmentController : ApiController
    {

        #region eCommerce Upload Booking Save & Update

        [HttpPost]
        public IHttpActionResult SelectService(List<FrayteeCommerceUserShipment> UploadShipment)
        {

            List<FrayteUploadshipment> Shipments = new List<FrayteUploadshipment>();

            foreach (var a in UploadShipment)
            {

                if (a.Service != null && a.Service.CountryId > 0)
                {
                    //res.Service = a.Service;
                    var res = new eCommerceUploadShipmentRepository().GetShipmentFromDraft(a.ShipmentId, eCommerceShipmentType.eCommerceSS);
                    res.Service = new Frayte.Services.Models.CountryLogistic()
                    {
                        CountryLogisticId = a.Service.CountryLogisticId,
                        CountryId = a.Service.CountryId,
                        LogisticService = a.Service.LogisticService,
                        LogisticServiceDisplay = a.Service.LogisticServiceDisplay,
                        AccountId = a.Service.AccountId,
                        AccountNo = a.Service.AccountNo,
                        CountryCode = a.Service.CountryCode,
                        Description = a.Service.Description
                    };
                    res.ServiceValue = a.ServiceValue;
                    Shipments.Add(res);
                }
            }
            string ShipmentIds = "";
            int i = 0;
            foreach (var a in Shipments)
            {
                int len = Shipments.Count;
                if (i < len - 1)
                {
                    ShipmentIds = ShipmentIds + a.DirectShipmentDraftId.ToString() + "|";
                }
                else if (i == len - 1)
                {
                    ShipmentIds = ShipmentIds + a.DirectShipmentDraftId.ToString() + "," + a.Service.CountryLogisticId.ToString() + "_" + "ECommerce";
                }
                i++;
            }

            Process.Start(AppSettings.eCommerceUploadShipmentBatchProcess, ShipmentIds);

            int eCommerceShipmentId = 0;

            return Ok(Shipments);
        }



        [HttpPost]
        public IHttpActionResult RemoveShipmentWithService(List<FrayteeCommerceUserShipment> UploadShipment)
        {

            foreach (var Shipment in UploadShipment)
            {

                new eCommerceUploadShipmentRepository().DeleteShipment(Shipment.ShipmentId);

            }
            return Ok();
        }


        [HttpGet]
        public List<FrayteUserDirectShipment> GetUnSuccessfulShipments(int CustomerId)
        {
            return new eCommerceUploadShipmentRepository().GetUnSuccessfulShipments(CustomerId);
        }



        [HttpGet]
        public UploadShipmentBatchProcess GetUpdatedBatchProcess(int CustomerId)
        {
            return new eCommerceUploadShipmentRepository().GetUpdatedBatchProcess(CustomerId);
        }

        [HttpGet]
        public List<FrayteUploadshipment> GetShipmentErrors(int ShipmentId, string ServiceType)
        {
            return new eCommerceUploadShipmentRepository().GetShipmentErrors(ShipmentId, ServiceType);
        }


        [HttpGet]
        [AllowAnonymous]
        public List<CountryLogistic> GetServices()
        {
            return new eCommerceUploadShipmentRepository().GetServices();
        }

        public FrayteeCommerceWithServiceShipmentfilter GetShipmentsFromDraft(int CustomerId)
        {
            return new eCommerceUploadShipmentRepository().GetShipmentsFromDraft(CustomerId);
        }

        public List<FrayteUploadshipment> GetShipmentsFromDraftTable(int CustomerId, string ServiceType)
        {
            return new eCommerceUploadShipmentRepository().GetShipmentsFromDraftTable(CustomerId, ServiceType);
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
                var gmd = new eCommerceUploadShipmentRepository().GetShipmentErrors(Shipment.ShipmentId, "ECOMMERCE_SS");
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

            var name = new eCommerceUploadShipmentRepository().UnsuccessfulExcelWrite(FUS, "ECOMMERCE_SS");
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
                    new eCommerceUploadShipmentRepository().DeleteShipment(Shipment.ShipmentId);
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
                var gmd = new eCommerceUploadShipmentRepository().GetShipmentErrors(Shipment.ShipmentId, "ECOMMERCE_WS");
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

            var Result = new eCommerceUploadShipmentRepository().UnsuccessfulExcelWrite(FUS, "ECOMMERCE_WS");

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
                    new eCommerceUploadShipmentRepository().DeleteShipment(Shipment.ShipmentId);
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
                                var res = false;
                                if (ServiceType == "ECOMMERCE_WS")
                                {
                                    res = new eCommerceUploadShipmentRepository().CheckValidWithoutServiceExcel(exceldata);
                                }
                                else if (ServiceType == "ECOMMERCE_SS")
                                {
                                    res = new eCommerceUploadShipmentRepository().CheckValidWithServiceExcel(exceldata);
                                }
                                if (res)
                                {
                                    frayteShipment = new eCommerceUploadShipmentRepository().GetAllShipments(exceldata, ServiceType, LogisticService);

                                    new eCommerceUploadShipmentRepository().ErrorLog(frayteShipment, ServiceType);

                                    if (ServiceType == "ECOMMERCE_WS")
                                    {
                                        foreach (var shipment in frayteShipment)
                                        {
                                            shipment.CustomerId = CustomerId;
                                            //Save data in draft table 


                                            result = new eCommerceUploadShipmentRepository().SaveShipment(shipment, CustomerId, ServiceType);

                                            //Insert values to directshipmentdraftModel
                                            var GetShipmentInDraftModel = new eCommerceUploadShipmentRepository().eCommerceShipmentDraft(shipment, CustomerId);


                                            if (shipment.Errors.Count == 0 && ServiceType == "ECOMMERCE_WS")
                                            {

                                                //Save data in eCommerce tables
                                                var getId = new eCommerceUploadShipmentRepository().SaveOrderNumber(shipment, shipment.TrackingNo, CustomerId);

                                                // EasyPost label
                                                string labelPath = HttpContext.Current.Server.MapPath("~/PackageLabel/eCommerce/" + getId.Item1 + "/");

                                                int calculatedHeight = 1400;
                                                int LocationY = 0;

                                                //After creating an order need to save the information in database

                                                var shipmentDetails = new eCommerceShipmentRepository().GetPackageDetails(getId.Item1);
                                                int increment = 0;
                                                for (int j = 0; j < shipment.Package.Count; j++)
                                                {
                                                    for (int i = 0; i < shipment.Package[j].CartoonValue; i++)
                                                    {
                                                        increment++;
                                                        var data = shipmentDetails[j];

                                                        // Step 1 : database entry 
                                                        var trackingDetail = new eCommerceShipmentRepository().SaveEasyPostDetailTrackingDeatil(GetShipmentInDraftModel, GetShipmentInDraftModel.Packages[j], data, GetShipmentInDraftModel.TrackingCode, null, getId.Item1, increment);

                                                        //  Step 2: Create frayte AWB label and save label name
                                                        string awbLabelName = generateAWBLabel(getId.Item1, GetShipmentInDraftModel, increment, GetShipmentInDraftModel.Packages[j]);
                                                        new eCommerceShipmentRepository().SaveAWBLabelName(trackingDetail, awbLabelName + ".pdf");


                                                    }

                                                }


                                                generateAWBLabel(getId.Item1, GetShipmentInDraftModel, 0, null);
                                                // Send SMS to consignee
                                                string message = string.Empty;

                                                //// Create Invoice while creating the shipment if all the HS Codes are mapped and send an email to receiver.
                                                //var Status = new eCommerceUploadShipmentRepository().ISAllHSCodeMapped(getId.Item1);

                                                //if (Status.Status)
                                                //{
                                                //    eCommerceTaxAndDutyInvoiceReport invoiceReport = new eCommerceShipmentRepository().GeteCommerceInvoiceObj(getId.Item1);
                                                //    var reportResult = new Report.Generator.ManifestReport.eCommerceInvoiceReport().GenerateInvoiceReport(getId.Item1, invoiceReport);
                                                //    if (reportResult.Status)
                                                //    {
                                                //        var status = new eCommerceShipmentRepository().SaveeCommerceInvoice(getId.Item1, invoiceReport);
                                                //        try
                                                //        {
                                                //            // send mail to receiver with attached invoice
                                                //            new ShipmentEmailRepository().sendInVoiceMail(getId.Item1);
                                                //        }
                                                //        catch (Exception ex)
                                                //        {
                                                //            Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                                                //        }
                                                //    }
                                                //}
                                            }

                                        }
                                    }
                                    else if (ServiceType == "ECOMMERCE_SS")
                                    {
                                        // check the validation in the excel
                                        new eCommerceUploadShipmentRepository().ErrorLog(frayteShipment, ServiceType);

                                        foreach (var shipment in frayteShipment)
                                        {
                                            //TextWriter tw = File.CreateText(@"C:\FMS\ecomm.godemowithus.com\WebApi\abc1.txt");
                                            //tw.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(shipment).ToCharArray());
                                            //tw.Close();
                                            result = new eCommerceUploadShipmentRepository().SaveShipment(shipment, CustomerId, ServiceType);
                                            //if (result.Status == false)
                                            //{
                                            //    frayteShipment.Remove(shipment);
                                            //}

                                        }
                                    }
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
                        //conn.Close();
                        //if ((System.IO.File.Exists(filepath)))
                        //{
                        //    System.IO.File.Delete(filepath);
                        //}
                    }
                }
            }

            var aa = GetUnSuccessfulShipments(CustomerId);

            return Ok(frayteShipment);

        }

        [HttpPost]
        public IHttpActionResult FrayteUploadShipments(FrayteUploadshipment Shipment)
        {

            var GetShipmentInDraftModel = new eCommerceUploadShipmentRepository().eCommerceUploadShipmentDraft(Shipment, Shipment.CustomerId);

            var a = new eCommerceUploadShipmentController().SaveEcommUploadShipment(GetShipmentInDraftModel);
            return Ok();

        }



        //#region EasyPost Integration
        //private FratyteError EasyPostIntegration(FrayteUploadshipment eCommerceBookingDetail, out List<FrayteCommercePackageTrackingDetail> shipmentPackageTrackingDetail, out int eCommerceShipmentId)
        //{
        //    eCommerceShipmentId = 0;
        //    FratyteError frayteError = new FratyteError();
        //    shipmentPackageTrackingDetail = new List<FrayteCommercePackageTrackingDetail>();

        //    //Step 1: Setup Easy Post Key
        //    EasyPost.ClientManager.SetCurrent(AppSettings.EasyPostKey);

        //    //Step 2: Create From Address
        //    EasyPost.Address fromAddress = new EasyPost.Address();
        //    setEasyPostFromAddress(eCommerceBookingDetail, frayteError, fromAddress);
        //    dynamic json = Newtonsoft.Json.JsonConvert.SerializeObject(fromAddress);
        //    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(json));

        //    //Step 3. Create To Address            
        //    EasyPost.Address toAddress = new EasyPost.Address();
        //    setEasyPostToAddress(eCommerceBookingDetail, frayteError, toAddress);
        //    json = Newtonsoft.Json.JsonConvert.SerializeObject(toAddress);
        //    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(json));


        //    EasyPost.Order dbOrder = new EasyPost.Order();
        //    dbOrder.mode = "production"; // production test
        //    dbOrder.from_address = fromAddress;
        //    dbOrder.to_address = toAddress;
        //    dbOrder.shipments = new List<EasyPost.Shipment>();
        //    dbOrder.created_at = DateTime.Now;

        //    // Step 4: Add custom info 
        //    EasyPost.CustomsInfo customsInfo = new EasyPost.CustomsInfo();
        //    //   customsInfo = CreateCustomInformation(eCommerceBookingDetail);
        //    dbOrder.customs_info = customsInfo;

        //    //Step 5: Create Shipment and add to orders 

        //    setShipmentsToOrder(eCommerceBookingDetail, dbOrder);

        //    //Step 6: create & buy order in easypost
        //    createEasyPostOrder(eCommerceBookingDetail, dbOrder, frayteError);


        //    //Step 7: After Integration remove data from Draft tables to eCommerce tables
        //    try
        //    {
        //        if (frayteError.Status)
        //        {
        //            //Finally Save the Order Id too to get the information of multiple shpment in Tracking Detail page.

        //            eCommerceShipmentId = new eCommerceUploadShipmentRepository().SaveOrderNumber(eCommerceBookingDetail, dbOrder.id, eCommerceBookingDetail.CustomerId);



        //            //  FrayteResult result = new eCommerceShipmentLabelReport().GeteCommerceShipmentLabelReportDetail(eCommerceBookingDetail);

        //            //After creating an order need to save the information in database
        //            int increment = 0;
        //            foreach (var package in eCommerceBookingDetail.Package)
        //            {
        //                FrayteCommercePackageTrackingDetail trackingDetail = new eCommerceUploadShipmentRepository().SaveEasyPostDetailTrackingDeatil(eCommerceBookingDetail, package, dbOrder.shipments[0], eCommerceShipmentId, increment); //directBookingDetail.DirectShipmentId
        //                shipmentPackageTrackingDetail.Add(trackingDetail);
        //                increment++;
        //            }
        //        }
        //    }
        //    catch (Exception ex2)
        //    {
        //        frayteError.Miscellaneous = new List<string>();
        //        frayteError.Miscellaneous.Add(ex2.Message);
        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex2);
        //        frayteError.Status = false;
        //    }
        //    return frayteError;
        //}

        [HttpPost]
        public IHttpActionResult SaveEcommUploadShipment(FrayteCommerceUploadShipmentDraft Shipment)
        {
            try
            {
                //Finally Save the Order Id too to get the information of multiple shpment in Tracking Detail page.
                var getId = new eCommerceUploadShipmentRepository().SaveEcommFormUploadShipment(Shipment);

                //Change the model
                var GetShipmentInEcommDraftModel = new eCommerceUploadShipmentRepository().eCommerceChangeModelShipmentDraft(Shipment, Shipment.CustomerId);

                //EasyPost label
                string labelPath = HttpContext.Current.Server.MapPath("~/PackageLabel/eCommerce/" + getId + "/");

                int calculatedHeight = 1400;
                int LocationY = 0;

                //After creating an order need to save the information in database

                var shipmentDetails = new eCommerceShipmentRepository().GetPackageDetails(getId);
                int increment = 0;
                for (int j = 0; j < Shipment.Packages.Count; j++)
                {
                    for (int i = 0; i < Shipment.Packages[j].CartoonValue; i++)
                    {
                        increment++;
                        var data = shipmentDetails[j];

                        // Step 1 : database entry 
                        var trackingDetail = new eCommerceShipmentRepository().SaveEasyPostDetailTrackingDeatil(GetShipmentInEcommDraftModel, GetShipmentInEcommDraftModel.Packages[j], data, GetShipmentInEcommDraftModel.TrackingCode, null, getId, increment);

                        //  Step 2: Create frayte AWB label and save label name
                        string awbLabelName = generateAWBLabel(getId, GetShipmentInEcommDraftModel, increment, GetShipmentInEcommDraftModel.Packages[j]);
                        new eCommerceShipmentRepository().SaveAWBLabelName(trackingDetail, awbLabelName + ".pdf");


                    }

                }
                generateAWBLabel(getId, GetShipmentInEcommDraftModel, 0, null);
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return Ok();
        }

        [HttpPost]

        public IHttpActionResult SaveShipmentDraftForm(FrayteCommerceUploadShipmentDraft Shipment)
        {
            //Change the model
            var GetShipmentInEcommDraftModel = new eCommerceUploadShipmentRepository().eCommerceModelChangeFrayteUploadShipment(Shipment, Shipment.CustomerId);
            var res = SaveShipmentDraft(GetShipmentInEcommDraftModel);

            return Ok();
        }
        [HttpPost]
        public IHttpActionResult SaveShipmentDraft(FrayteUploadshipment Shipment)
        {
            var result = new eCommerceUploadShipmentRepository().SaveShipment(Shipment, Shipment.CustomerId, eCommerceShipmentType.eCommerceSS);
            return Ok();
        }

        private void createEasyPostOrder(FrayteUploadshipment eCommerceBookingDetail, Order dbOrder, FratyteError frayteError)
        {
            try
            {
                dbOrder.Create();
                Dictionary<decimal, EasyPost.Rate> minEasyPostRate = new Dictionary<decimal, EasyPost.Rate>();
                EasyPost.Rate appliedRate = new EasyPost.Rate();



                var shippingMethod = new eCommerceUploadShipmentRepository().GetLogisticService(eCommerceBookingDetail.Service.LogisticService);                                                                                                                     // If Rate.Count == 0 then imple ment Address Verification and send info to ui
                if (dbOrder.rates != null && dbOrder.rates.Count == 0 && dbOrder.messages.Count > 0)
                {
                    if (!string.IsNullOrEmpty(shippingMethod.LogisticService))
                    {
                        frayteError.Address = new List<string>();
                        frayteError.Custom = new List<string>();
                        frayteError.Package = new List<string>();
                        frayteError.Miscellaneous = new List<string>();
                        frayteError.Service = new List<string>();
                        frayteError.ServiceError = new List<string>();
                        foreach (var err in dbOrder.messages)
                        {
                            FrayteGeneralError error = JsonConvert.DeserializeObject<FrayteGeneralError>(err);
                            if (error.carrier == shippingMethod.LogisticService)
                            {
                                if (error.message.Contains("country") || error.message.Contains("postal code") || error.message.Contains("postcode"))
                                {
                                    frayteError.Address.Add(error.message);
                                }
                                else if (error.message.Contains("custom"))
                                {
                                    frayteError.Custom.Add(error.message);
                                }
                                else if (error.message.Contains("package"))
                                {
                                    frayteError.Package.Add(error.message);
                                }
                                else
                                {
                                    frayteError.IsMailSend = true;
                                    frayteError.Miscellaneous.Add(error.message);
                                }
                            }
                        }
                    }
                    frayteError.Status = false;
                }
                else if (dbOrder.rates != null && dbOrder.rates.Count > 0)
                {
                    foreach (EasyPost.Rate rate in dbOrder.rates)
                    {
                        if (!string.IsNullOrEmpty(rate.service) &&
                            !string.IsNullOrEmpty(rate.carrier) && !string.IsNullOrEmpty(rate.carrier_account_id) &&
                            rate.carrier == shippingMethod.LogisticService && rate.carrier_account_id == shippingMethod.AccountId
                            //rate.carrier == shippingMethod.LogisticService
                            )
                        {
                            if (!minEasyPostRate.ContainsKey(CommonConversion.ConvertToDecimal(rate.rate)))
                            {
                                minEasyPostRate.Add(CommonConversion.ConvertToDecimal(rate.rate), rate);
                            }
                        }
                    }
                    if (minEasyPostRate.Count > 0)
                    {
                        appliedRate = minEasyPostRate.OrderBy(p => p.Key).First().Value;
                        dbOrder.Buy(appliedRate.carrier, appliedRate.service);
                        frayteError.Status = true;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(shippingMethod.LogisticService))
                        {
                            frayteError.Address = new List<string>();
                            frayteError.Custom = new List<string>();
                            frayteError.Package = new List<string>();
                            frayteError.Miscellaneous = new List<string>();
                            frayteError.Service = new List<string>();
                            frayteError.ServiceError = new List<string>();
                            foreach (var err in dbOrder.messages)
                            {
                                FrayteGeneralError error = JsonConvert.DeserializeObject<FrayteGeneralError>(err);
                                if (error.carrier == shippingMethod.LogisticService)
                                {
                                    if (error.message.Contains("country") || error.message.Contains("postal code") || error.message.Contains("postcode"))
                                    {
                                        frayteError.Address.Add(error.message);
                                    }
                                    else if (error.message.Contains("custom"))
                                    {
                                        frayteError.Custom.Add(error.message);
                                    }
                                    else if (error.message.Contains("package"))
                                    {
                                        frayteError.Package.Add(error.message);
                                    }
                                    else
                                    {
                                        frayteError.IsMailSend = true;
                                        frayteError.Miscellaneous.Add(error.message);
                                    }

                                }
                            }
                        }
                        frayteError.Status = false;
                    }
                }

            }
            catch (EasyPost.HttpException ex)
            {
                frayteError.Address = new List<string>();
                frayteError.Custom = new List<string>();
                frayteError.Package = new List<string>();
                frayteError.Miscellaneous = new List<string>();
                frayteError.Service = new List<string>();
                frayteError.ServiceError = new List<string>();
                FrayteGeneralError error = JsonConvert.DeserializeObject<FrayteGeneralError>(ex.Message);
                if (!string.IsNullOrEmpty(error.message))
                {
                    frayteError.Miscellaneous.Add(error.message);
                }
                else
                {
                    FrayteEasyPostError error1 = JsonConvert.DeserializeObject<FrayteEasyPostError>(ex.Message);
                    if (error1.error != null && !string.IsNullOrEmpty(error1.error.message))
                    {
                        frayteError.Miscellaneous.Add(error1.error.message);
                    }
                    else
                    {
                        frayteError.Miscellaneous.Add("Error While Buying the Shipment");
                    }
                }

                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                frayteError.Status = false;

                // result.Errors.Add(ex.Message);
            }
        }
        private void setShipmentsToOrder(FrayteUploadshipment eCommerceBookingDetail, Order dbOrder)
        {
            foreach (Frayte.Services.Models.UploadShipmentPackage dbPackage in eCommerceBookingDetail.Package)
            {
                if (dbPackage.CartoonValue > 0)
                {
                    for (int i = 0; i < dbPackage.CartoonValue; i++)
                    {
                        var weight = 0M;
                        var width = 0M;
                        var height = 0M;
                        var length = 0M;
                        if (eCommerceBookingDetail.PackageCalculationType == FraytePakageCalculationType.kgtoCms)
                        {
                            weight = dbPackage.Weight * 35.274M;
                            width = dbPackage.Width * 0.393701M;
                            height = dbPackage.Height * 0.393701M;
                            length = dbPackage.Length * 0.393701M;

                        }
                        else if (eCommerceBookingDetail.PackageCalculationType == FraytePakageCalculationType.LbToInchs)
                        {
                            weight = dbPackage.Weight * 16;
                            width = dbPackage.Width;
                            height = dbPackage.Height;
                            length = dbPackage.Length;
                        }
                        else
                        {
                            weight = dbPackage.Weight;
                            width = dbPackage.Width;
                            height = dbPackage.Height;
                            length = dbPackage.Length;
                        }
                        //Step 4. Create Parcel            
                        EasyPost.Parcel parcel = EasyPost.Parcel.Create(new Dictionary<string, object>()
                {
                  {"length", length},
                  {"width", width},
                  {"height", height},
                  {"weight", weight}
                });

                        string parcelId = parcel.id;

                        EasyPost.Shipment shipment = new EasyPost.Shipment();

                        // shipment.carrier_accounts = new List<EasyPost.CarrierAccount>();

                        //shipment.carrier_accounts.Add(rateAccount);

                        // Check that originating and destinating country are different 
                        if (eCommerceBookingDetail.ShipFrom.Country.CountryId != eCommerceBookingDetail.ShipTo.Country.CountryId)
                        {
                            // CreateCustomItem(eCommerceBookingDetail, dbOrder.customs_info, dbPackage);
                        }

                        //Step 5. Create Shipment                             
                        shipment.from_address = dbOrder.from_address;
                        shipment.to_address = dbOrder.to_address;
                        shipment.parcel = parcel;
                        if (eCommerceBookingDetail.PayTaxAndDuties == FrayteTermsOfTrade.Receiver)
                        {
                            shipment.options = new EasyPost.Options()
                            {
                                print_custom_1 = eCommerceBookingDetail.FrayteNumber + "-" + eCommerceBookingDetail.ShipmentReference,
                                delivered_duty_paid = false,
                                // bill_receiver_account = "12345678"
                            };
                        }
                        else if (eCommerceBookingDetail.PayTaxAndDuties == FrayteTermsOfTrade.Shipper)
                        {
                            shipment.options = new EasyPost.Options()
                            {
                                print_custom_1 = eCommerceBookingDetail.FrayteNumber + "-" + eCommerceBookingDetail.ShipmentReference,
                                delivered_duty_paid = true,
                                bill_receiver_account = "952491075"
                            };
                        }
                        else
                        {
                            shipment.options = new EasyPost.Options()
                            {
                                print_custom_1 = eCommerceBookingDetail.ShipmentReference,
                                delivered_duty_paid = false,
                                bill_receiver_account = "952491074"
                            };
                        }

                        //shipment.Create();
                        dbOrder.shipments.Add(shipment);
                        //   json = Newtonsoft.Json.JsonConvert.SerializeObject(shipment);
                        //  Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(json));
                    }
                }

            }
        }
        private void CreateCustomItem(FrayteUploadshipment eCommerceBookingDetail, EasyPost.CustomsInfo customsInfo, Frayte.Services.Models.PackageDraft dbPackage)
        {
            var weight = 0M;
            if (eCommerceBookingDetail.PackageCalculationType == FraytePakageCalculationType.kgtoCms)
            {
                weight = dbPackage.Weight * 35.274M;
            }
            else if (eCommerceBookingDetail.PackageCalculationType == FraytePakageCalculationType.LbToInchs)
            {
                weight = dbPackage.Weight * 16;
            }
            else
            {
                weight = dbPackage.Weight;
            }
            EasyPost.CustomsItem customsItem1 = EasyPost.CustomsItem.Create(new Dictionary<string, object>() {

                  {"description", dbPackage.Content},
                  {"quantity", 1},
                  {"weight", weight},
                  {"value", dbPackage.Value},
                  {"origin_country",eCommerceBookingDetail.ShipFrom.Country.Code2},
                  {"hs_tariff_number", ""}
                });
            customsInfo.customs_items.Add(customsItem1);
        }
        private EasyPost.CustomsInfo CreateCustomInformation(FrayteUploadshipment eCommerceBookingDetail)
        {
            EasyPost.CustomsInfo customsInfo = new EasyPost.CustomsInfo();
            customsInfo.customs_certify = eCommerceBookingDetail.CustomInfo.CustomsCertify.Value ? "true" : "false";
            customsInfo.eel_pfc = eCommerceBookingDetail.CustomInfo.EelPfc;
            customsInfo.customs_signer = eCommerceBookingDetail.CustomInfo.CustomsSigner;
            customsInfo.contents_type = eCommerceBookingDetail.CustomInfo.ContentsType;
            customsInfo.customs_items = new List<EasyPost.CustomsItem>();

            return customsInfo;
        }
        private void setEasyPostToAddress(FrayteUploadshipment eCommerceBookingDetail, FratyteError frayteError, Address toAddress)
        {
            var shipToCompany = "";
            var FromCountry = new eCommerceUploadShipmentRepository().GetCountry(eCommerceBookingDetail.ShipFrom.Country);
            var ToCountry = new eCommerceUploadShipmentRepository().GetCountry(eCommerceBookingDetail.ShipTo.Country);
            if (string.IsNullOrEmpty(eCommerceBookingDetail.ShipTo.CompanyName))
            {
                shipToCompany = eCommerceBookingDetail.ShipTo.FirstName + " " + eCommerceBookingDetail.ShipTo.LastName;
                toAddress.company = shipToCompany;
            }
            else
            {
                toAddress.company = eCommerceBookingDetail.ShipTo.CompanyName;
            }
            toAddress.street1 = eCommerceBookingDetail.ShipTo.Address;
            toAddress.street2 = eCommerceBookingDetail.ShipTo.Address2;
            toAddress.city = eCommerceBookingDetail.ShipTo.City;
            toAddress.state = eCommerceBookingDetail.ShipTo.State;
            toAddress.country = ToCountry.CountryCode2;
            toAddress.zip = eCommerceBookingDetail.ShipTo.PostCode;
            toAddress.phone = eCommerceBookingDetail.ShipTo.Phone;

            try
            {
                toAddress.Create();
            }
            catch (EasyPost.HttpException e)
            {
                FrayteGeneralError error = JsonConvert.DeserializeObject<FrayteGeneralError>(e.Message);
                frayteError.Address.Add(error.message);
                frayteError.Status = false;

            }

        }
        private void setEasyPostFromAddress(FrayteUploadshipment eCommerceBookingDetail, FratyteError frayteError, EasyPost.Address fromAddress)
        {
            var shipFromCompany = "";

            var FromCountry = new eCommerceUploadShipmentRepository().GetCountry(eCommerceBookingDetail.ShipFrom.Country);
            var ToCountry = new eCommerceUploadShipmentRepository().GetCountry(eCommerceBookingDetail.ShipTo.Country);


            var wareHouse = new eCommerceShipmentRepository().getWareHouseDetail(ToCountry.CountryId);
            if (wareHouse != null)
            {
                eCommerceBookingDetail.WareHouseId = wareHouse.WarehouseId;
                if (string.IsNullOrEmpty(eCommerceBookingDetail.ShipFrom.CompanyName))
                {
                    shipFromCompany = eCommerceBookingDetail.ShipFrom.FirstName + " " + eCommerceBookingDetail.ShipFrom.LastName;
                    fromAddress.company = shipFromCompany;
                }
                else
                {
                    fromAddress.company = eCommerceBookingDetail.ShipFrom.CompanyName;
                }
                fromAddress.street1 = wareHouse.Address;
                fromAddress.street2 = wareHouse.Address2;
                fromAddress.city = wareHouse.City;
                fromAddress.state = wareHouse.State;
                fromAddress.country = wareHouse.Country.Code2;
                fromAddress.zip = wareHouse.Zip;
                fromAddress.phone = wareHouse.TelephoneNo;
            }
            else
            {
                eCommerceBookingDetail.WareHouseId = 1;
                if (string.IsNullOrEmpty(eCommerceBookingDetail.ShipFrom.CompanyName))
                {
                    shipFromCompany = eCommerceBookingDetail.ShipFrom.FirstName + " " + eCommerceBookingDetail.ShipFrom.LastName;
                    fromAddress.company = shipFromCompany;
                }
                else
                {
                    fromAddress.company = eCommerceBookingDetail.ShipFrom.CompanyName;
                }
                fromAddress.street1 = eCommerceBookingDetail.ShipFrom.Address;
                fromAddress.street2 = eCommerceBookingDetail.ShipFrom.Address2;
                fromAddress.city = eCommerceBookingDetail.ShipFrom.City;
                fromAddress.state = eCommerceBookingDetail.ShipFrom.State;
                fromAddress.country = eCommerceBookingDetail.ShipFrom.Country.Code2;
                fromAddress.zip = eCommerceBookingDetail.ShipFrom.PostCode;
                fromAddress.phone = eCommerceBookingDetail.ShipFrom.Phone;
            }
            try
            {
                fromAddress.Create();
            }
            catch (EasyPost.HttpException e)
            {
                FrayteGeneralError error = JsonConvert.DeserializeObject<FrayteGeneralError>(e.Message);
                frayteError.Address.Add(error.message);
                frayteError.Status = false;
            }
        }

        #endregion

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

        //#endregion

        #region Get eCommerceBooking Detail Clone,Return,Draft
        public FrayteCommerceShipmentDraft GeteCommerceBookingDetailDraft(int DirectShipmentDraftId, string CallingType)
        {
            return new eCommerceShipmentRepository().GeteCommerceBookingDetailDraft(DirectShipmentDraftId, CallingType);
        }


        #endregion

        #region Get eCommerceBookinfDetail For DetailPage 
        public FrayteeCommerceShipmentDetail GeteCommerceBookingDetail(int eCommerceShipmentId, string CallingType)
        {
            return new eCommerceShipmentRepository().GeteCommerceBookingDetail(eCommerceShipmentId, CallingType);
        }
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
            string pdfFileName = string.Empty;
            string physycalpath = string.Empty;
            string str = string.Empty;
            //if (detail.CreatedOn.HasValue)
            //{
            //    str = "AWBUK-" + detail.CreatedOn.Value.Year.ToString().Substring(2, 2) + detail.CreatedOn.Value.ToString("MM") + GetFormattedManifestId(eCommerceShipmentId);
            //}
            //else
            //{
            //    str = "AWBUK-" + DateTime.UtcNow.Year.ToString().Substring(2, 2) + DateTime.UtcNow.ToString("MM") + GetFormattedManifestId(eCommerceShipmentId);
            //}
            string prfix = string.Empty;
            if (CourierCompany == FrayteCourierCompany.DHL || CourierCompany == FrayteCourierCompany.DHLExpress)
            {
                prfix = FrayteShortName.DHL;
            }
            str = prfix + "_" + detail.Packages[0].TrackingNo + " (All)";

            pdfFileName = AppSettings.WebApiPath + "PackageLabel/eCommerce/" + eCommerceShipmentId + "/" + str + ".pdf";
            physycalpath = HttpContext.Current.Server.MapPath("~/PackageLabel/eCommerce/" + eCommerceShipmentId + "/" + str + ".pdf");
            //}

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

        public IHttpActionResult eCommerceDetailWithLabel(string ToMail, int eCommerceShipmentId, string CourierName)
        {
            new ShipmentEmailRepository().SendeCommerceBookingLabel(ToMail, eCommerceShipmentId, CourierName);
            return Ok();
        }


    }
}
