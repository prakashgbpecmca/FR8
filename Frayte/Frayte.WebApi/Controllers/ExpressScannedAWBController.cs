using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Frayte.Services.Business;
using Frayte.Services.Models.Express;
using Frayte.Services.Models;
using System.Web;
using System.IO;
using System.Net.Http.Headers;
using Frayte.Services.DataAccess;

namespace Frayte.WebApi.Controllers
{
    public class ExpressScannedAWBController : ApiController
    {
        [HttpPost]
        public List<ExpressScannedAwbModel> GetScannedAWB(ExpressAWBTrackModel Track)
        {
            return new ExpressScannedAWBRepository().GetScannedAWB(Track);
        }

        [HttpGet]
        public List<DirectBookingCustomer> GetCustomers(int UserId)
        {
            return new ExpressScannedAWBRepository().GetCustomers(UserId);
        }

        [HttpGet]
        public ExpressFile ImageToByte(int AWBId)
        {
            return new ExpressScannedAWBRepository().ImageToByte(AWBId);
        }

        [HttpPost]
        public HttpResponseMessage DownloadAWBImage(FrayteCommercialInvoiceFileName file)
        {
            string filePath = HttpContext.Current.Server.MapPath("~/AwbImage/" + file.FileName);
            using (MemoryStream ms = new MemoryStream())
            {
                using (FileStream filestrm = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    byte[] bytes = new byte[filestrm.Length];
                    filestrm.Read(bytes, 0, (int)filestrm.Length);
                    ms.Write(bytes, 0, (int)filestrm.Length);
                    HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
                    httpResponseMessage.Content = new ByteArrayContent(bytes);
                    httpResponseMessage.Content.Headers.Add("download-status", "downloaded");
                    httpResponseMessage.Content.Headers.Add("x-filename", file.FileName);
                    httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    httpResponseMessage.Content.Headers.ContentDisposition.FileName = file.FileName;
                    httpResponseMessage.StatusCode = HttpStatusCode.OK;
                    return httpResponseMessage;
                }
            }
        }
        
        // Delete Multiple selected option
        // Avinash 22-apr-2019

        [HttpPost]
        public IHttpActionResult DeleteScannedAWBList(List<string> scannedAwbList)
        {
            return Ok(new ExpressScannedAWBRepository().DeleteScannedAwb(scannedAwbList));
        }

    }
}
