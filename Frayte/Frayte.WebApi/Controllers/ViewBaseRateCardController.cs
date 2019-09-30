using Frayte.Services.Business;
using Frayte.Services.Models;
using Frayte.Services.DataAccess;
using Report.Generator.ManifestReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using System.IO;
using System.Net.Http.Headers;

namespace Frayte.WebApi.Controllers
{
    [Authorize]
    public class ViewBaseRateCardController : ApiController
    {
        [HttpGet]
        public List<int> DistinctYear()
        {
            List<int> _year = new ViewBaseRateCardRepository().GetDistinctYear();
            return _year;
        }

        [HttpGet]
        public List<FrayteLogisticServiceItem> GetLogisticServiceItems(int Yaer)
        {
            List<FrayteLogisticServiceItem> item = new List<FrayteLogisticServiceItem>();
            item = new ViewBaseRateCardRepository().GetLogisticServiceItems(Yaer);
            return item;
        }

        [HttpGet]
        public List<FrayteLogisticServiceItem> GetCustomerLogisticService(int UserId)
        {
            List<FrayteLogisticServiceItem> item = new List<FrayteLogisticServiceItem>();
            item = new ViewBaseRateCardRepository().GetCustomerLogisticServiceItems(UserId);
            return item;
        }

        [HttpGet]
        public List<FrayteLogisticServiceItem> GetLogisticServices()
        {
            List<FrayteLogisticServiceItem> item = new List<FrayteLogisticServiceItem>();
            item = new ViewBaseRateCardRepository().LogisticServiceItems();
            return item;
        }

        [HttpGet]
        public IHttpActionResult GenerateReport(int LogisticServiceId, int Yaer)
        {
            FrayteManifestName result = new FrayteManifestName();
            result = new BaseRateReport().ExportBaseRateExcel(LogisticServiceId, Yaer);
            return Ok(result);
        }

        [HttpPost]
        public HttpResponseMessage DownLoadRateCardReport(ReportFile fileName)
        {
            try
            {
                if (fileName != null && !string.IsNullOrEmpty(fileName.FileName))
                {
                    return DownloadrateCardReport(fileName.FileName);
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

        private HttpResponseMessage DownloadrateCardReport(string fileName)
        {
            string filePath = HttpContext.Current.Server.MapPath("~/ReportFiles/BaseRateCard/" + fileName);
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
    }
}
