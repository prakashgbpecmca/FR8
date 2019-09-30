using Frayte.Services.Business;
using Frayte.Services.Models;
using Frayte.Services.Utility;
using Frayte.WebApi.Utility;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using static Frayte.Services.Models.FrayteQuotationEmailModel;
using Elmah;
using Report.Generator.ManifestReport;

namespace Frayte.WebApi.Controllers
{
    
    public class QuotationController : ApiController
    {
        [HttpPost]
        public IHttpActionResult SaveQuotation(FrayteQuotationShipment quotationDetail)
        {
            try
            {
                FrayteQuotationResult result = new FrayteQuotationResult();
                result = new QuotationRepository().SaveQuotation(quotationDetail);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public IHttpActionResult EditQuotation(FrayteQuotationShipment quotationDetail)
        {
            try
            {
                FrayteQuotationResult result = new FrayteQuotationResult();
                result = new QuotationRepository().EditQuotation(quotationDetail);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public IHttpActionResult RemoveQuotation(int QuotationShipmentId)
        {
            FrayteResult result = new FrayteResult();
            result = new QuotationRepository().DeleteQuotation(QuotationShipmentId);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult GetQuotationShipments(int operationZoneId, int UserId, int CustomerId)
        {
            try
            {
                List<FrayteQuotationShipment> list = new List<FrayteQuotationShipment>();
                list = new QuotationRepository().GetQuotationShipments(operationZoneId, UserId, CustomerId);
                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public IHttpActionResult GetQuotationValidity(int QuotationShipmentId)
        {
            FrayteResult result = new FrayteResult();
            result = new QuotationRepository().QuotationValidity(QuotationShipmentId);
            return Ok(result);
        }

        [HttpPost]
        public IHttpActionResult GenerateQuotationShipmentPdf(FrayteQuotationShipment quotationDetail)
        {
            try
            {
                FrayteManifestName fileResult = new FrayteManifestName();
                if (quotationDetail.QuotationShipmentId > 0)
                {
                    fileResult = new QuoteReport().GetQuotation(quotationDetail.QuotationShipmentId, quotationDetail.CustomerName, quotationDetail.QuotationFromAddress.PostCode, quotationDetail.QuotationToAddress.PostCode, quotationDetail.CreatedBy);
                    return Ok(fileResult);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return BadRequest();
            }
        }

        [HttpPost]
        public HttpResponseMessage DownloadQuotationReport(ReportFile file)
        {
            try
            {
                if (file != null && !string.IsNullOrEmpty(file.FileName))
                {
                    return DownloadQuotationReportPdf(file.FileName);
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

        [HttpPost]
        public IHttpActionResult QuotationServices(DirectBookingFindService serviceRequest)
        {
            try
            {
                List<DirectBookingService> lstDirectBookingServices = new List<DirectBookingService>();
                lstDirectBookingServices = new QuotationRepository().GetServices(serviceRequest);
                return Ok(lstDirectBookingServices);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        public IHttpActionResult SendQuotationMail(FrayteQuotationEmailMail quotationEmailDetail)
        {
            try
            {
                FrayteQuotationResult result = new FrayteQuotationResult();
                if (quotationEmailDetail.QuotationDetail.QuotationShipmentId > 0)
                {
                    //Step 1: Get Surcharge Detail According Choose Service
                    var fileResult = new QuoteReport().GetQuotation(quotationEmailDetail.QuotationDetail.QuotationShipmentId, quotationEmailDetail.Name, quotationEmailDetail.QuotationDetail.QuotationFromAddress.PostCode, quotationEmailDetail.QuotationDetail.QuotationToAddress.PostCode, quotationEmailDetail.LoginUserId);
                    var ratecard = new CustomerBaseRateReport().CustomerQuoteRateCard(quotationEmailDetail.QuotationDetail.QuotationShipmentId);
                    result = new ShipmentEmailRepository().SendQuotationMail(quotationEmailDetail, fileResult, ratecard);
                    return Ok(result);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return BadRequest();
            }
        }

        [HttpPost]
        public IHttpActionResult SendCustomerQuoteMail(FrayteQuotationEmailMail CustomerEmailDetail)
        {
            try
            {
                FrayteQuotationResult result = new FrayteQuotationResult();
                var fileResult = new QuoteReport().GetQuotation(CustomerEmailDetail.QuotationDetail.QuotationShipmentId, CustomerEmailDetail.Name, CustomerEmailDetail.QuotationDetail.QuotationFromAddress.PostCode, CustomerEmailDetail.QuotationDetail.QuotationToAddress.PostCode, CustomerEmailDetail.LoginUserId);
                var ratecard = new CustomerBaseRateReport().CustomerQuoteRateCard(CustomerEmailDetail.QuotationDetail.QuotationShipmentId);
                result = new ShipmentEmailRepository().SendCustomerQuoteMail(CustomerEmailDetail, fileResult, ratecard);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return BadRequest();
            }
        }

        [HttpGet]
        public FrayteSalesRepresentiveEmail GetSalesRepresentiveEmail(int UserId, int RoleId)
        {
            FrayteSalesRepresentiveEmail email = new QuotationRepository().SalesRepresentiveEmail(UserId, RoleId);
            return email;
        }

        [HttpGet]
        public IHttpActionResult GetCustomerAddressType(int CustomerId)
        {
            string address = new QuotationRepository().CustomerAddressType(CustomerId);
            return Ok(address);
        }

        [HttpGet]
        public IHttpActionResult GetTNTSupplemetoryInformation(int LogisticServiceId)
        {
            var info = new QuotationRepository().GetTNTInforamtion(LogisticServiceId);
            return Ok(info);
        }

        [HttpGet]
        public IHttpActionResult GetCustomerLogisticServices(int CustomerId)
        {
            bool services = new QuotationRepository().CustomerLogisticServices(CustomerId);
            return Ok(services);
        }

        #region DownloadQuotationReport

        private HttpResponseMessage DownloadQuotationReportPdf(string fileName)
        {
            string filePath = HttpContext.Current.Server.MapPath("~/ReportFiles/" + fileName);
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
