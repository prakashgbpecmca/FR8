using Frayte.Services.Business;
using Frayte.Services.Models;
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
    public class BaseRateCardUpdateController : ApiController
    {
        [AllowAnonymous]
        [HttpPost]
        public FrayteResult UploadBaseRate()
        {
            FrayteResult result = new FrayteResult();
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                HttpFileCollection files = httpRequest.Files;

                HttpPostedFile file = files[0];

                if (!string.IsNullOrEmpty(file.FileName))
                {
                    string connString = "";
                    string filename = DateTime.Now.ToString("MM_dd_yyyy_hh_mm_ss_") + file.FileName;
                    string filepath = HttpContext.Current.Server.MapPath("~/UploadFiles/Shipments/" + filename);
                    file.SaveAs(filepath);
                    connString = new BaseRateCardUpdateRepository().getExcelConnectionString(filename, filepath);
                    string fileExtension = "";
                    fileExtension = new BaseRateCardUpdateRepository().getFileExtensionString(filename);
                    try
                    {
                        if (!string.IsNullOrEmpty(fileExtension))
                        {
                            var ds = new DataSet();
                            using (var conn = new OleDbConnection(connString))
                            {
                                conn.Open();
                                DataTable dbSchema = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                                //for (int j = 0; j < dbSchema.Rows.Count; j++)
                                //{
                                string firstSheetName = dbSchema.Rows[0]["TABLE_NAME"].ToString();
                                var query = "SELECT * FROM " + "[" + firstSheetName + "]";
                                using (var adapter = new OleDbDataAdapter(query, conn))
                                {
                                    adapter.Fill(ds, "Rates");
                                }

                                var exceldata = ds.Tables[0];
                                if (exceldata.Rows.Count > 0)
                                {
                                    result = new BaseRateCardUpdateRepository().UpdateRates(exceldata);
                                }
                                //}
                            }

                            if ((System.IO.File.Exists(filepath)))
                            {
                                System.IO.File.Delete(filepath);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    }
                }
            }
            return result;
        }

        [AllowAnonymous]
        [HttpGet]
        public IHttpActionResult DownloadRateCardExcelTemplate(string CourierCompany, string LogisticType, string RateType)
        {
            FrayteManifestName file = new FrayteManifestName();
            file = new BaseRateCardUpdateRepository().DownloadRateCardExcelTemplate(CourierCompany, LogisticType, RateType);
            return Ok(file);
        }

        [AllowAnonymous]
        [HttpPost]
        public HttpResponseMessage DownloadrateRateCardExcelTemplate(FrayteCommercialInvoiceFileName FileName)
        {
            try
            {
                if (FileName != null && !string.IsNullOrEmpty(FileName.FileName))
                {
                    return DownloadRateCardTemplate(FileName.FileName);
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

        private HttpResponseMessage DownloadRateCardTemplate(string FileName)
        {
            string filePath = HttpContext.Current.Server.MapPath("~/UploadFiles/BaseRateCardUpdate/" + FileName);
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
    }
}
