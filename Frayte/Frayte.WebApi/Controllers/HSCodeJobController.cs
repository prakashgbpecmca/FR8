using Frayte.Services.Business;
using Frayte.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Frayte.WebApi.Controllers
{
    [Authorize]
    public class HSCodeJobController : ApiController
    {

        #region UnAssigned Jobs / Assigned Jobs

        [HttpPost]
        public IHttpActionResult GetUnAssignedJobs(TrackHSCodeJob obj)
        {
            try
            {
                List<FrayteUnAssignedJob> list = new List<FrayteUnAssignedJob>();
                list = new HsCodeJobRepository().GetUnAssignedJobs(obj);
                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest("Request can not be completed.");
            }
        }

        [HttpPost]
        public IHttpActionResult AsssignJobs(OpeartorJob jobs)
        {
            try
            {
                FrayteResult result = new FrayteResult();
                result = new HsCodeJobRepository().AssignJobToOperator(jobs);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return BadRequest("Couls not Complete the request.");
            }

        }

        #endregion

        #region  Jobs In Progress

        [HttpPost]
        public IHttpActionResult GetAssignedJobs(TrackAssignedJob obj)
        {
            try
            {
                List<FrayteMappedJobs> list = new List<FrayteMappedJobs>();
                list = new HsCodeJobRepository().GetAssignedJobs(obj);
                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest("Request can not be completed.");
            }
        }

        [HttpGet]
        public FrayteResult SetShipmentHSCode(int eCommerceShipmentDetailid, string HSCode, string Description, int HsCodeId)
        {
            try
            {
                FrayteResult result = new HsCodeJobRepository().SetShipmentHSCode(eCommerceShipmentDetailid, HSCode, Description, HsCodeId);
                if (result.Status)
                {
                    var Status = new HSCodeRepository().ISAllHSCodeMapped(eCommerceShipmentDetailid);
                    if (Status.Status)
                    {
                        // Get Shipment Detail
                        FrayteeCommerceShipmentDetail shipmentDetail = new eCommerceShipmentRepository().GeteCommerceBookingDetail(Status.Id, "");

                        var IsInVoiceCreated = new eCommerceShipmentRepository().IsInvoiceCreated(Status.Id);
                        if (!IsInVoiceCreated)
                        {
                            // Generate model for invoice
                            eCommerceTaxAndDutyInvoiceReport invoiceReport = new eCommerceShipmentRepository().GeteCommerceInvoiceObj(Status.Id);

                            // Generate Invoice Report 

                            var reportResult = new Report.Generator.ManifestReport.eCommerceInvoiceReport().GenerateInvoiceReport(Status.Id, invoiceReport);
                            if (reportResult.Status)
                            {
                                // Save Invoice Info
                                var status = new eCommerceShipmentRepository().SaveeCommerceInvoice(Status.Id, invoiceReport);
                                try
                                {
                                    // send mail to receiver with attached invoice
                                    new ShipmentEmailRepository().sendInVoiceMail(Status.Id);
                                }
                                catch (Exception ex)
                                {
                                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                                }
                            }
                        }

                    }

                }
                return result;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return new FrayteResult
                {
                    Status = false
                };
            }

        }

        [HttpPost]
        public IHttpActionResult ReAssignJobs(OpeartorReAssignedJob jobs)
        {
            try
            {
                FrayteResult result = new FrayteResult();
                result = new HsCodeJobRepository().ReAssignJobs(jobs);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return BadRequest("Could not complete the request at the moment.");
            }
        }

        [HttpGet]
        public IHttpActionResult GetJobsInProgressCount()
        {
            try
            {
                JobsInProgressCount jobsCount = new JobsInProgressCount();
                jobsCount = new HsCodeJobRepository().GetJobsInProgressCount();
                return Ok(jobsCount);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return BadRequest("Could not Complete the request.");
            }
        }

        #endregion

        #region Total HSCode Output/Operator/Hour
        [HttpGet]
        public IHttpActionResult HSCodeOutputPerOperatorPerHour(int userId)
        {
            try
            {
                List<HSCodeAvgOutput> list = new List<HSCodeAvgOutput>();
                list = new HsCodeJobRepository().HSCodeOutputPerOperatorPerHour(userId);

                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest("Request could not be completed at the moment.");
            }

        }

        #endregion

        #region Common

        [HttpGet]
        public IHttpActionResult GetJobsDetails(int userId)
        {
            try
            {
                JobDetail data = new HsCodeJobRepository().GetJobsDetails(userId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest("Can not complete the request");
            }
        }

        [HttpGet]
        public IHttpActionResult GetDestinationCountries(int userId)
        {
            try
            {
                List<FrayteCountryCode> lstToCountry = new List<FrayteCountryCode>();

                lstToCountry = new CountryRepository().ListToCountry();

                return Ok(lstToCountry);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return BadRequest();
            }

        }

        [HttpGet]
        public IHttpActionResult GetMangerOperators(int userId)
        {
            try
            {
                List<MangerOperator> list = new List<MangerOperator>();
                list = new HsCodeJobRepository().GetMangerOperators(userId);
                return Ok(list);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return BadRequest("Couls not Complete the request.");
            }
        }

        [HttpGet]
        public IHttpActionResult GetOperatorsWithJobs(int userId)
        {
            try
            {
                List<OperatorWithJobs> list = new List<OperatorWithJobs>();
                list = new HsCodeJobRepository().GetOperatorsWithJobs(userId);
                return Ok(list);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return BadRequest("Couls not Complete the request.");
            }

        }

        #endregion
    }
}
