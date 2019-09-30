using Frayte.Services.Business;
using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using Frayte.Services.Utility;
using Microsoft.Office.Interop.Excel;
using Report.Generator.ManifestReport;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;

namespace Frayte.WebApi.Controllers
{
    [Authorize]
    public class ManifestController : ApiController
    {
        [HttpGet]
        public MainfestDetailModel GetNonManifestedShipments(int OperationZoneId, int UserId, int CreatedBy, string moduleType, string subModuleType)
        {
            try
            {
                var list = new ManifestRepository().GetNonManifestedShipments(OperationZoneId, UserId, CreatedBy, moduleType, subModuleType);
                return list;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public int GetUnmanifestedJobCount(int userId)
        {
            return new ManifestRepository().GetUnmanifestedJobCount(userId);
        }

        [HttpPost]
        public List<FrayteManifest> GetManifests(TrackManifest trackManifest)
        {
            try
            {
                var list = new ManifestRepository().GetManifests(trackManifest);
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet]
        public FrayteViewManifest GetManifestDetail(int ManifestId, string moduleType, string subModuleType)
        {
            try
            {
                var list = new ManifestRepository().GetManifestDetail(ManifestId, moduleType);
                return list;
            }

            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        public FrayteResult SetETAETD(ETAETDManifest manifestETAETD)
        {
            try
            {
                FrayteResult result = new ManifestRepository().SetETAETD(manifestETAETD);
                return result;
            }
            catch (Exception ex)
            {
                return new FrayteResult() { Status = false };
            }
        }

        [HttpPost]
        public IHttpActionResult CreateManifest(FrayteManifestShipment ManifestShipment)
        {
            var result = new ManifestRepository().CreateManifest(ManifestShipment);

            if (ManifestShipment.ModuleType == FrayteShipmentServiceType.eCommerce)
            {
                foreach (var item in ManifestShipment.DirectShipments)
                {
                    // If All the Hscode for a shipment is mapped then send invoice email
                    var Status = new eCommerceShipmentRepository().ISAllHSCodeMapped(item.ShipmentId);
                    if (Status.Status)
                    {
                        eCommerceTaxAndDutyInvoiceReport invoiceReport = new eCommerceShipmentRepository().GeteCommerceInvoiceObj(item.ShipmentId);
                        var reportResult = new Report.Generator.ManifestReport.eCommerceInvoiceReport().GenerateInvoiceReport(item.ShipmentId, invoiceReport);
                        if (reportResult.Status)
                        {
                            var status = new eCommerceShipmentRepository().SaveeCommerceInvoice(item.ShipmentId, invoiceReport);
                            try
                            {
                                // send mail to receiver with attached invoice
                                new ShipmentEmailRepository().sendInVoiceMail(item.ShipmentId);
                            }
                            catch (Exception ex)
                            {
                                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                            }
                        }
                    }
                }
            }
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult RemoveShipmentFromManifest(int shipmentId, string moduleType)
        {
            FrayteResult result = new ManifestRepository().RemoveShipmentFromManifest(shipmentId, moduleType);
            return Ok(result);
        }

        [HttpGet]
        public FrayteUserManifest IsManifestSupprt(int userId)
        {
            FrayteUserManifest IsManifestSupport = new ManifestRepository().IsManifestSupprt(userId);
            if (IsManifestSupport.Services != null && IsManifestSupport.Services.Count > 0)
            {
                IsManifestSupport.Services.OrderBy(p => p.OrderNumber);
            }
            return IsManifestSupport;
        }

        [HttpGet]
        public IHttpActionResult GenerateManifest(int ManifestId, string moduleType, int UserId, int RoleId)
        {
            FrayteManifestName result = new FrayteManifestName();
            result = new FrayteManifestReport().GenerateManifestReport(ManifestId, moduleType, UserId, RoleId);
            return Ok(result);
        }

        [HttpPost]
        public HttpResponseMessage DownloadReport(ReportFile fileName)
        {
            try
            {
                if (fileName != null && !string.IsNullOrEmpty(fileName.FileName))
                {
                    return DownloadManifestReport(fileName.FileName, fileName.ModuleType);
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

        [HttpGet]
        public List<FrayteBookingType> GetBookingTypes(int UserId)
        {
            return new ManifestRepository().GetFrayteBookingType(UserId);
        }

        [HttpGet]
        public List<DirectBookingCustomer> GetCustomerList()
        {
            return new ManifestRepository().GetCustomerList();
        }

        [HttpGet]
        public List<CustomManifestDetail> GetCustomManifestDetail()
        {
            return new ManifestRepository().GetCustomManifestDetail();
        }

        [HttpPost]
        public List<CustomManifestDetail> GetCustomManifests(TrackCustomManifest trackManifest)
        {
            try
            {
                var list = new ManifestRepository().GetCustomManifests(trackManifest);
                return list;
            }

            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet]
        public List<UploadShipmentPackage> CustomManifestPackageDetail(int EcommerceShipmentId)
        {
            return new ManifestRepository().CustomManifestPackageDetail(EcommerceShipmentId);
        }

        [HttpGet]
        public List<eCommerceViewManifest> ViewCustomManifestDetail(int ManifestId)
        {
            return new ManifestRepository().ViewCustomManifestDetail(ManifestId);
        }

        [HttpGet]
        public fileName GenerateCustomManifest(int ManifestId, string ManifestName)
        {
            if (ManifestName != null && !ManifestName.Contains(".csv"))
            {
                ManifestName = ManifestName + ".csv";
            }

            TrackCustomManifest TCM = new TrackCustomManifest();
            fileName fN = new fileName();
            var flag = true;
            var filePaths = Directory.GetFiles(AppSettings.ManifestFolderPath);
            var filePath = AppSettings.WebApiPath + "Manifestedshipments/" + ManifestName;
            if (ManifestName != null && ManifestName != "")
            {
                for (int i = 0; i < filePaths.Length; i++)
                {
                    var filePathLength = filePaths[i].Length;
                    var FileName = filePaths[i].Split('\\');
                    var FileNameLength = FileName.Length;
                    var ManifestFileName = FileName[FileNameLength - 1];

                    if (ManifestFileName == ManifestName)
                    {
                        flag = false;
                        fN.FileName = ManifestName;
                        fN.FilePath = filePath;
                        break;
                    }
                }
            }
            if (flag == true && ManifestName != null && ManifestName != "")
            {
                var gmd = new ManifestRepository().GetMainfestDataUI(ManifestId, ManifestName);

                fN.FileName = new ManifestRepository().ManifestExcelWriteFromUI(gmd, ManifestName);
                if (fN.FileName.Contains(".csv"))
                {
                    fN.FileName = fN.FileName;
                }
                else
                {
                    fN.FileName = fN.FileName + ".csv";
                }
                fN.FilePath = filePath;
            }
            return fN;
        }

        [HttpPost]
        public HttpResponseMessage DownloadCustomManifest(fileName fileName)
        {
            return DownloadCustomManifest(fileName.FileName);
        }

        [HttpGet]
        public List<FrayteManifestOnExcel> GetMainfestData(int ManifestId, string ManifestFileName)
        {
            return new ManifestRepository().GetMainfestData(ManifestId, ManifestFileName);
        }

        [HttpGet]
        public FrayteManifestName GenerateManifestExcel(int ManifestId)
        {
            FrayteManifestName result = new FrayteManifestName();
            result = new FrayteTrackAndTraceReport().GenerateManifestExcel(ManifestId);
            return result;
        }

        [HttpPost]
        public HttpResponseMessage DownloadManifest(ReportFile fileName)
        {
            try
            {
                if (fileName != null && !string.IsNullOrEmpty(fileName.FileName))
                {
                    return DownloadManifestReport(fileName.FileName);
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

        #region Private method

        private HttpResponseMessage DownloadCustomManifest(string fileName)
        {
            string filePath = HttpContext.Current.Server.MapPath("~/pdfdownload/" + fileName);
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

        private HttpResponseMessage DownloadManifestReport(string fileName, string moduleType)
        {
            string ModuleType = string.Empty;
            string filePath = string.Empty;

            if (moduleType == FrayteShipmentServiceType.DirectBooking)
                filePath = HttpContext.Current.Server.MapPath("~/ReportFiles/" + fileName);
            else if (moduleType == FrayteShipmentServiceType.eCommerce)
                filePath = HttpContext.Current.Server.MapPath("~/ReportFiles/eCommerce/" + fileName);

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

        private HttpResponseMessage DownloadManifestReport(string fileName)
        {
            string filePath = HttpContext.Current.Server.MapPath("~/ReportFiles/Manifest/" + fileName);
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
