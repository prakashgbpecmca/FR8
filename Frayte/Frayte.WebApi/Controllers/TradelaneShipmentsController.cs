using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Frayte.Services.Business;
using Frayte.Services.Models;
using Frayte.Services.Utility;
using Frayte.Services.DataAccess;
using System.IO;
using Frayte.Services.Models.Tradelane;
using System.Web;
using System.Net.Http.Headers;
using Report.Generator.ManifestReport;

namespace Frayte.WebApi.Controllers
{
    public class TradelaneShipmentsController : ApiController
    {
        [HttpPost]
        public List<TradelaneGetShipmentModel> GetTradelaneShipments(TradelaneTrackDirectBooking Track)
        {
            return new TradelaneShipmentRepository().GetTradelaneShipments(Track);
        }

        [HttpGet]
        public List<ShipmentStatu> GetDirectShipmentStatusList(string BookingType)
        {
            return new TradelaneShipmentRepository().GetDirectShipmentStatusList(BookingType);
        }

        [HttpGet]

        public List<DirectBookingCustomer> GetTradlaneCustomers()
        {
            return new TradelaneShipmentRepository().GetTradlaneCustomers();
        }

        [HttpPost]
        public FrayteResult SendClaimShipment(TradelaneClaimShipmentModel ClaimShipment)
        {
            return new TradelaneEmailRepository().ClaimShipmentEmail(ClaimShipment);
        }

        [HttpPost]
        public FrayteResult SendResolvedClaimShipment(TradelaneClaimShipmentModel ClaimShipment)
        {
            return new TradelaneEmailRepository().SendResolvedClaimShipment(ClaimShipment);
        }

        [HttpPost]
        public FrayteResult SendMawbCorrectionShipment(TradelaneClaimShipmentModel ClaimShipment)
        {
            return new TradelaneShipmentRepository().MawbCorrection(ClaimShipment);
        }

        [HttpPost]
        public fileName GetLatestMawbDocuments(int TradelaneShipmentId)
        {
            return new TradelaneShipmentRepository().GetLatestMawbDocuments(TradelaneShipmentId);
        }

        [HttpGet]
        public TradelaneAgentEmailModel GetAgentMail(int TradelaneShipmentId)
        {
            return new TradelaneShipmentRepository().GetAgentsMail(TradelaneShipmentId);
        }

        [HttpGet]
        public TradelaneGetShipmentModel GetShipmentDetail(string FrayteNumber, string FrayteType)
        {
            return new TradelaneShipmentRepository().GetShipmentDetail(FrayteNumber, FrayteType);
        }

        #region Documents 

        [HttpPost]
        public HttpResponseMessage DownloadDocument(TradelaneFileStatus File)
        {
            string filePhysicalPath = HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + File.TradelaneShipmentId + "/" + File.FileName);
            using (MemoryStream ms = new MemoryStream())
            {
                using (FileStream file = new FileStream(filePhysicalPath, FileMode.Open, FileAccess.Read))
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

        [HttpGet]
        public TradelaneFile CreateDocument(int tradelaneShipmentId, int userId, string documentType, string documentTypeName)
        {
            try
            {
                TradelaneFile result = new TradelaneFile();

                switch (documentType)
                {
                    case FrayteTradelaneShipmentDocumentEnum.ShipmentDetail:
                        result = new TradelaneDocument().ShipmentDetails(tradelaneShipmentId);
                        break;

                    case FrayteTradelaneShipmentDocumentEnum.HAWB:
                        result = new TradelaneDocument().ShipmentHAWB(tradelaneShipmentId, documentTypeName);
                        break;

                    case FrayteTradelaneShipmentDocumentEnum.Manifest:
                        result = new TradelaneDocument().ShipmentManifest(tradelaneShipmentId, userId);
                        break;

                    case FrayteTradelaneShipmentDocumentEnum.CoLoadForm:
                        result = new TradelaneDocument().ShipmentCoLoadForm(tradelaneShipmentId);
                        break;

                    case FrayteTradelaneShipmentDocumentEnum.MAWB:
                        result = new TradelaneDocument().ShipmentMAWB(tradelaneShipmentId, documentTypeName);
                        break;

                    case FrayteTradelaneShipmentDocumentEnum.CartonLabel:
                        result = new TradelaneDocument().ShipmentCartonLabel(tradelaneShipmentId, 0, 0, "");
                        break;

                    case FrayteTradelaneShipmentDocumentEnum.BagLabel:
                        result = new TradelaneDocument().ShipmentBagLabel(tradelaneShipmentId, userId);
                        break;

                    case FrayteTradelaneShipmentDocumentEnum.DriverManifest:
                        result = new TradelaneDocument().ShipmentDriverManifest(tradelaneShipmentId, userId);
                        break;

                    case FrayteTradelaneShipmentDocumentEnum.ExportManifest:
                        result = new TradelaneDocument().ShipmentExportManifest(tradelaneShipmentId);
                        break;
                    case FrayteTradelaneShipmentDocumentEnum.BagNumber:
                        result = new TradelaneDocument().ShipmentExportBagNumber(tradelaneShipmentId, userId);
                        break;
                    case FrayteTradelaneShipmentDocumentEnum.REV:
                        //   new TradelaneShipmentRepository().ShipmentMAWBDocDownload(tradelaneShipmentId, 0, 0);
                        break;

                }

                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion

        #region  PreALert 
        [HttpGet]
        public TradelanePreAlertInitial PreALertInitials(int shipmentId)
        {
            try
            {
                TradelanePreAlertInitial detail = new TradelaneShipmentRepository().PreALertInitials(shipmentId);
                return detail;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet]
        public List<TradelanePreAlertDocument> GetShipmentDocuments(int shipmentId)
        {
            List<TradelanePreAlertDocument> list = new TradelaneBookingRepository().GetShipmentDocuments(shipmentId);
            return list;
        }

        [HttpGet]
        public List<TradelanePreAlertDocument> GetShipmentOtherDocuments(int shipmentId)
        {
            List<TradelanePreAlertDocument> list = new TradelaneBookingRepository().GetShipmentOtherDocuments(shipmentId);
            return list;
        }

        [HttpGet]
        public FrayteResult DeleteTradelaneShipment(int TradelaneShipmentId)
        {
            return new TradelaneShipmentRepository().DeleteTradelaneShipment(TradelaneShipmentId);
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
        public FrayteResult RemoveOtherDocument(int tradelaneShipmentDocument)
        {
            FrayteResult result = new TradelaneShipmentRepository().RemoveOtherDocument(tradelaneShipmentDocument);
            return result;
        }

        [HttpPost]
        public IHttpActionResult SendPreAlertEmail(TradelanePreAlertInitial preAlerDetail)
        {
            FrayteResult result = new TradelaneShipmentRepository().SendPreAlertEmail(preAlerDetail);
            if (result.Status)
            {
                new TradelaneShipmentRepository().SavePreAlertLog(preAlerDetail);
            }

            return Ok(result);
        }

        #endregion

        #region  Preview HAWB

        public List<string> GetShipmentHAWB(int shipmentId)
        {
            try
            {
                List<string> list = new TradelaneShipmentRepository().GetShipmentHAWB(shipmentId);
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion

        #region AvinashCode

        [HttpPost]
        public IHttpActionResult AddMAWBCustomized(MAWBCustomizedFiled mawbCustomizefield)
        {
            return Ok(new TradelaneShipmentRepository().SaveMAWBCustomized(mawbCustomizefield));
        }

        [HttpGet]
        public MAWBCustomizedFiled GetMawbCustomizePdf(int TradelaneShipmentId)
        {
            return new TradelaneShipmentRepository().GetMawbCustomizePdf(TradelaneShipmentId);
        }

        #endregion
    }
}
