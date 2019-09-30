using Frayte.Services.Business;
using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using Frayte.Services.Models.Express;
using Frayte.Services.Models.Tradelane;
using Frayte.Services.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace Frayte.WebApi.Controllers
{
    public class ExpressManifestController : ApiController
    {

        #region Tradelane Integration 

        [HttpPost]
        public IHttpActionResult SaveTradelaneIntegration(ExpressTradelaneIntegration integratedShipment)
        {
            TradelaneBooking shipment = new TradelaneBooking();
            new ExpressManifestRepository().MapIntegratedShipmentToTradelaneShipment(integratedShipment, shipment);
            TradelaneBooking dbShipment = new TradelaneBookingRepository().SaveShipment(shipment, "Express");

            if (!dbShipment.Error.Status)
            {
                TradelaneEmailModel model = new TradelaneEmailRepository().TradelaneEmailObj(dbShipment.TradelaneShipmentId);
                // Fill all the required info in the email model
                integratedShipment.MAWBList.FirstOrDefault().TradelaneId = dbShipment.TradelaneShipmentId;
                // model.ShipmentDetail.ShipFrom = new TradelBookingAdress();
                model.ShipmentDetail.ShipFrom.IsMailSend = dbShipment.ShipFrom.IsMailSend;
                //  model.ShipmentDetail.ShipTo = new TradelBookingAdress();
                model.ShipmentDetail.ShipTo.IsMailSend = dbShipment.ShipTo.IsMailSend;
                model.ShipmentDetail.NotifyParty.IsMailSend = dbShipment.NotifyParty.IsMailSend;

                new TradelaneEmailRepository().FillBookingInformationEmailModel(model);
                // Update Status for AWB 
                // Send Shipment info email created by 
                //  new TradelaneEmailRepository().SendEmail_E1(model);

                // Customer Confirmation email
                new TradelaneEmailRepository().SendEmail_E2_1(model);

                // Send MAWB AllocationEMail
                new TradelaneEmailRepository().FillModel_E3(model);
                new TradelaneEmailRepository().SendEmail_E3(model);


                // Add shipment tracking  
                new TradelaneBookingRepository().SaveShipmentTracking(shipment);
                //  Add FLight Detail  
                new TradelaneBookingRepository().SaveShipmentFlightDetail(shipment);
                new ExpressManifestRepository().UpdateExpressShipmentStatus(integratedShipment.Shipment.Packages);

                // Update MAWBAllocation 
                var File = new TradelaneShipmentsController().CreateDocument(dbShipment.TradelaneShipmentId, 0, FrayteTradelaneShipmentDocumentEnum.CoLoadForm, "");
                var FilePath = AppSettings.UploadFolderPath + "/Tradelane" + "/" + integratedShipment.MAWBList.FirstOrDefault().TradelaneId + "/" + File.FileName;
                if (File.FileName != null && File.FileName != "")
                {
                }
                else
                {
                    FilePath = "";
                }

                new MawbAllocationRepository().SaveExpressMawbAllocation(integratedShipment.MAWBList, FilePath, "Express");

                // update tradelane Shipment id  in express  table 
                new ExpressManifestRepository().SaveExportManifest(dbShipment.TradelaneShipmentId, integratedShipment.Shipment);

                var file = new TradelaneShipmentsController().CreateDocument(dbShipment.TradelaneShipmentId, 0, FrayteTradelaneShipmentDocumentEnum.ExportManifest, "");
                new TradelaneBookingRepository().SaveShipmentDocument(dbShipment.TradelaneShipmentId, FrayteTradelaneShipmentDocumentEnum.ExportManifest, file.FileName, integratedShipment.Shipment.CreatedBy);

                var file1 = new TradelaneShipmentsController().CreateDocument(dbShipment.TradelaneShipmentId, dbShipment.CreatedBy, FrayteTradelaneShipmentDocumentEnum.DriverManifest, "");
                new TradelaneBookingRepository().SaveShipmentDocument(dbShipment.TradelaneShipmentId, FrayteTradelaneShipmentDocumentEnum.DriverManifest, file1.FileName, integratedShipment.Shipment.CreatedBy);

                new MawbAllocationRepository().SendMawbAllocationMail(integratedShipment.MAWBList, FilePath, "Express");
            }
            return Ok(integratedShipment);
        }

        [HttpGet]
        public ExpressTradelaneIntegration TradelaneHubInitials(int customerId, int hubId)
        {
            ExpressTradelaneIntegration integrationShipment = new ExpressTradelaneIntegration();
            integrationShipment.Shipment = new ExpressManifestRepository().TradelaneHubInitials(customerId, hubId);
            return integrationShipment;
        }

        #endregion

        #region Upload Document  

        [HttpPost]
        public IHttpActionResult UploadBatteryForms()
        {
            try
            {
                int id = 0;
                var httpRequest = HttpContext.Current.Request;
                ShipmentRepository shipmentRepository = new ShipmentRepository();
                HttpFileCollection files = httpRequest.Files;
                int ShipmentId = Convert.ToInt32(httpRequest.Form["ShipmentId"].ToString());
                int CustomerId = Convert.ToInt32(httpRequest.Form["CustomerId"].ToString());
                int UserId = Convert.ToInt32(httpRequest.Form["UserId"].ToString());
                int HubId = Convert.ToInt32(httpRequest.Form["HubId"].ToString());
                var docfiles = new List<string>();

                // Entry in tradelane tables  
                id = new ExpressManifestRepository().TradelaneEntryFromManifest(ShipmentId, UserId, CustomerId, HubId);

                string filePathToSave = string.Format(FrayteTradelaneDocumentPath.ShipmentDocument, id);
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
                            FrayteResult result = new TradelaneBookingRepository().SaveShipmentDocument(id, FrayteShipmentDocumentType.BatteryForm, file.FileName, UserId);

                            if (result.Status)
                            {
                                return Ok(id);
                            }
                            else
                            {
                                return BadRequest("BatteryForm");
                            }
                        }
                        else
                        {
                            return BadRequest("BatteryForm");
                        }
                    }
                }

                if (id > 0)
                {
                    return Ok(id);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public IHttpActionResult UploadMAWBForms()
        {
            try
            {
                int id = 0;
                var httpRequest = HttpContext.Current.Request;
                ShipmentRepository shipmentRepository = new ShipmentRepository();
                HttpFileCollection files = httpRequest.Files;
                //This code will execute only when user will upload the document
                if (files.Count > 0)
                {

                    int ShipmentId = Convert.ToInt32(httpRequest.Form["ShipmentId"].ToString());
                    int CustomerId = Convert.ToInt32(httpRequest.Form["CustomerId"].ToString());
                    int UserId = Convert.ToInt32(httpRequest.Form["UserId"].ToString());
                    int HubId = Convert.ToInt32(httpRequest.Form["HubId"].ToString());

                    var docfiles = new List<string>();
                    // Entry in tradelane tables  
                    id = new ExpressManifestRepository().TradelaneEntryFromManifest(ShipmentId, UserId, CustomerId, HubId);

                    if (id > 0)
                    {
                        string filePathToSave = string.Format(FrayteTradelaneDocumentPath.ShipmentDocument, id);
                        string fileFullPath = "";
                        filePathToSave = HttpContext.Current.Server.MapPath(filePathToSave);

                        if (!System.IO.Directory.Exists(filePathToSave))
                            System.IO.Directory.CreateDirectory(filePathToSave);

                        HttpPostedFile file = files[0];
                        if (!string.IsNullOrEmpty(file.FileName))
                        {

                            //Save in server folder
                            fileFullPath = filePathToSave + file.FileName;
                            file.SaveAs(fileFullPath);

                            //Save file name and other information in DB
                            FrayteResult result = new TradelaneBookingRepository().SaveShipmentDocument(id, FrayteShipmentDocumentType.MAWB, file.FileName, UserId);

                            if (result.Status)
                            {
                                return Ok(id);
                            }
                            else
                            {
                                return BadRequest("MAWB");
                            }
                        }
                        else
                        {
                            return BadRequest("MAWB");
                        }
                    }
                }

                if (id > 0)
                {
                    return Ok(id);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public IHttpActionResult UploadOtherDocument()
        {
            try
            {
                int id = 0;
                var httpRequest = HttpContext.Current.Request;
                ShipmentRepository shipmentRepository = new ShipmentRepository();
                HttpFileCollection files = httpRequest.Files;
                //This code will execute only when user will upload the document
                if (files.Count > 0)
                {
                    int ShipmentId = Convert.ToInt32(httpRequest.Form["ShipmentId"].ToString());
                    int CustomerId = Convert.ToInt32(httpRequest.Form["CustomerId"].ToString());
                    int UserId = Convert.ToInt32(httpRequest.Form["UserId"].ToString());
                    int HubId = Convert.ToInt32(httpRequest.Form["HubId"].ToString());

                    var docfiles = new List<string>();

                    // Entry in tradelane tables  
                    id = new ExpressManifestRepository().TradelaneEntryFromManifest(ShipmentId, UserId, CustomerId, HubId);

                    string filePathToSave = string.Format(FrayteTradelaneDocumentPath.ShipmentDocument, id);
                    string fileFullPath = "";
                    filePathToSave = HttpContext.Current.Server.MapPath(filePathToSave);

                    if (!System.IO.Directory.Exists(filePathToSave))
                        System.IO.Directory.CreateDirectory(filePathToSave);

                    HttpPostedFile file = files[0];
                    if (!string.IsNullOrEmpty(file.FileName))
                    {
                        if (!File.Exists(filePathToSave + file.FileName))
                        {
                            //Save in server folder
                            fileFullPath = filePathToSave + file.FileName;
                            file.SaveAs(fileFullPath);

                            //Save file name and other information in DB
                            FrayteResult result = new TradelaneBookingRepository().SaveShipmentDocument(id, FrayteShipmentDocumentType.OtherDocument, file.FileName, UserId);

                            if (result.Status)
                            {
                                return Ok(id);
                            }
                            else
                            {
                                return BadRequest("OtherDocument");
                            }
                        }
                        else
                        {
                            return BadRequest("OtherDocument");
                        }
                    }
                }
                if (id > 0)
                {
                    return Ok(id);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        #endregion

        #region vikshit code

        [HttpGet]
        public MainfestDetailModel GetNonManifestedShipments(int OperationZoneId, int UserId, int CreatedBy, string moduleType, string subModuleType)
        {
            try
            {
                var list = new ExpressManifestRepository().GetNonManifestedShipments(OperationZoneId, UserId, CreatedBy, moduleType, subModuleType);
                return list;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        //[AllowAnonymous]
        //[HttpGet]
        //public int GetUnmanifestedJobCount(int userId)
        //{
        //    return new ExpressManifestRepository().GetUnmanifestedJobCount(userId);
        //}

        [HttpPost]
        public List<ExpressManifestModel> GetManifests(ExpressTrackManifest trackManifest)
        {
            try
            {
                var list = new ExpressManifestRepository().GetManifests(trackManifest);
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet]
        public ExpressViewManifest GetManifestDetail(int ManifestId)
        {
            try
            {
                var list = new ExpressManifestRepository().GetManifestDetail(ManifestId);
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet]
        public TradelaneFile GetBagLabel(int BagId)
        {
            try
            {
                var list = new ExpressManifestRepository().GetBagLabel(BagId);
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        public HttpResponseMessage DownBagLabel(TradelaneFile File)
        {
            return DownloadExpressBagLabel(File);
        }

        public ExpressViewManifest GetBagDetail(int HubId, int CustomerId)
        {
            try
            {
                var list = new ExpressManifestRepository().GetBagsDetail(HubId, CustomerId);
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<HubDetailModel> GetHubs()
        {
            try
            {
                var list = new ExpressManifestRepository().GetHubs();
                return list;
            }

            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet]
        public List<ExpressGetShipmentModel> GetShipments(int BagId)
        {
            return new ExpressManifestRepository().GetShipments(BagId);
        }

        [HttpGet]
        public ExpressViewManifest GetManifestShipments(int ManifestId)
        {
            return new ExpressManifestRepository().GetManifestShipments(ManifestId);
        }

        [HttpPost]
        public TradelaneFile GetExportManifest(ExpressDownloadPDFModel DownloadPDFModel)
        {
            TradelaneFile File = new TradelaneShipmentsController().CreateDocument(DownloadPDFModel.TradelaneShipmentId, DownloadPDFModel.CustomerId, FrayteTradelaneShipmentDocumentEnum.ExportManifest, "");
            File.FilePath = AppSettings.WebApiPath + "UploadFiles/Tradelane" + "/" + DownloadPDFModel.TradelaneShipmentId + "/" + File.FileName;
            File.TradelaneShipmentId = DownloadPDFModel.TradelaneShipmentId;
            if (File.FileName != null && File.FilePath != null)
            {
                var FilePath = AppSettings.UploadFolderPath + "/Tradelane" + "/" + DownloadPDFModel.TradelaneShipmentId + "/" + File.FileName;
                new ExpressManifestRepository().SendExportManifest(DownloadPDFModel, FilePath);
            }
            return File;
        }

        [HttpPost]
        public TradelaneFile GetDriverManifest(ExpressDownloadPDFModel DownloadPDFModel)
        {
            TradelaneFile File = new TradelaneShipmentsController().CreateDocument(DownloadPDFModel.TradelaneShipmentId, DownloadPDFModel.CustomerId, FrayteTradelaneShipmentDocumentEnum.DriverManifest, "");
            File.FilePath = AppSettings.WebApiPath + "UploadFiles/Tradelane" + "/" + DownloadPDFModel.TradelaneShipmentId + "/" + File.FileName;
            File.TradelaneShipmentId = DownloadPDFModel.TradelaneShipmentId;
            if (File.FileName != null && File.FilePath != null)
            {
                var FilePath = AppSettings.UploadFolderPath + "/Tradelane" + "/" + DownloadPDFModel.TradelaneShipmentId + "/" + File.FileName;
                new ExpressManifestRepository().SendDriverManifest(DownloadPDFModel, FilePath);
            }
            return File;
        }

        [HttpPost]
        public HttpResponseMessage DownloadExpressReport(TradelaneFile File)
        {
            string filePath = HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + File.TradelaneShipmentId + "/" + File.FileName);
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
                    httpResponseMessage.Content.Headers.Add("x-filename", File.FileName);
                    httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    httpResponseMessage.Content.Headers.ContentDisposition.FileName = File.FileName;
                    httpResponseMessage.StatusCode = HttpStatusCode.OK;
                    return httpResponseMessage;
                }
            }
        }

        [HttpPost]
        public HttpResponseMessage DownloadExpressBagLabel(TradelaneFile File)
        {
            string filePath = HttpContext.Current.Server.MapPath("~/UploadFiles/ExpressBag/" + File.TradelaneShipmentId + "/" + File.FileName);
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
                    httpResponseMessage.Content.Headers.Add("x-filename", File.FileName);
                    httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    httpResponseMessage.Content.Headers.ContentDisposition.FileName = File.FileName;
                    httpResponseMessage.StatusCode = HttpStatusCode.OK;
                    return httpResponseMessage;
                    ms.Dispose();
                }
            }
        }

        [HttpGet]
        public string GetTimeZoneName(int BagId)
        {
            return new ExpressManifestRepository().GetTimeZoneName(BagId);
        }

        #endregion

        #region Avinash Code

        [AllowAnonymous]
        [HttpGet]
        public IHttpActionResult GetCurrency()
        {
            return Ok(new MasterDataRepository().GetCurrencyType());
        }

        #region Custom Manifest        

        [AllowAnonymous]
        [HttpGet]
        public IHttpActionResult GetCustomManifest(int ManifestId)
        {
            return Ok(new ManifestRepository().GenerateCustomManifest(ManifestId));
        }

        [HttpPost]
        public HttpResponseMessage DownloadCustomManifest(FrayteManifestName File)
        {

            string filePath = HttpContext.Current.Server.MapPath("~/UploadFiles/PDFGenerator/HTMLFile/" + File.FileName);
            using (MemoryStream ms = new MemoryStream())
            using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite))
            {
                byte[] bytes = new byte[file.Length];
                file.Read(bytes, 0, (int)file.Length);
                ms.Write(bytes, 0, (int)file.Length);
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
                httpResponseMessage.Content = new ByteArrayContent(bytes);
                httpResponseMessage.Content.Headers.Add("download-status", "downloaded");
                httpResponseMessage.Content.Headers.Add("x-filename", File.FileName);
                httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                httpResponseMessage.Content.Headers.ContentDisposition.FileName = File.FileName;
                httpResponseMessage.StatusCode = HttpStatusCode.OK;
                return httpResponseMessage;
            }
        }

        #endregion

        #region AddProductCatalog

        [HttpPost]
        public IHttpActionResult AddCanadaProductCatalog(CanadaProductcatalog obj)
        {
            return Ok(new ManifestRepository().SaveCanadaProductCatalog(obj));
        }

        [HttpPost]
        public IHttpActionResult AddSWissProductCatalog(SwissProductcatalog obj)
        {
            return Ok(new ManifestRepository().SaveSWISSProductCatalog(obj));
        }

        [HttpPost]
        public IHttpActionResult AddUKProductCatalog(UKProductcatalog obj)
        {
            return Ok(new ManifestRepository().SaveUKproductCatalog(obj));
        }

        [HttpPost]
        public IHttpActionResult AddUSAProductCatalog(USAProductcatalog obj)
        {
            return Ok(new ManifestRepository().SaveUSAproductCatalog(obj));
        }

        [HttpPost]
        public IHttpActionResult AddNorwayProductCatalog(NorwayProductcatalog obj)
        {
            return Ok(new ManifestRepository().SaveNorwayproductCatalog(obj));
        }

        [HttpPost]
        public IHttpActionResult AddSINProductCatalog(SINProductcatalog obj)
        {
            return Ok(new ManifestRepository().SaveSINproductCatalog(obj));
        }

        [HttpPost]
        public IHttpActionResult AddNRTProductCatalog(JapanProductcatalog obj)
        {
            return Ok(new ManifestRepository().SaveJapanProductCatalog(obj));
        }

        #endregion

        #region FetchproductCatalog

        [HttpPost]
        public IHttpActionResult FetchSwissProductcatalog(FrayteProductCatalogTrack track)
        {
            return Ok(new ManifestRepository().GetSwissProductCatalog(track));
        }

        [HttpPost]
        public IHttpActionResult FetchUKProductcatalog(FrayteProductCatalogTrack track)
        {
            return Ok(new ManifestRepository().GetUkProductCatalog(track));
        }

        [HttpPost]
        public IHttpActionResult FetchCANProductcatalog(FrayteProductCatalogTrack track)
        {
            return Ok(new ManifestRepository().GetCanadaProductCatalog(track));
        }

        [HttpPost]
        public IHttpActionResult FetchUSAProductcatalog(FrayteProductCatalogTrack track)
        {
            return Ok(new ManifestRepository().GetUSAProductCatalog(track));
        }

        [HttpPost]
        public IHttpActionResult FetchNorwayProductcatalog(FrayteProductCatalogTrack track)
        {
            return Ok(new ManifestRepository().GetNorwayProductCatalog(track));
        }

        [HttpPost]
        public IHttpActionResult FetchSINProductcatalog(FrayteProductCatalogTrack track)
        {
            return Ok(new ManifestRepository().GetSINProductCatalog(track));
        }

        [HttpPost]
        public IHttpActionResult FetchNRTProductcatalog(FrayteProductCatalogTrack track)
        {
            return Ok(new ManifestRepository().GetNRTProductCatalog(track));
        }

        #endregion

        #region EditproductCatalog

        [HttpGet]
        public IHttpActionResult EditProductCatalog(int ProductcatalogId)
        {
            return Ok(new ManifestRepository().EditProductcatalog(ProductcatalogId));
        }

        #endregion

        #endregion
    }
}
