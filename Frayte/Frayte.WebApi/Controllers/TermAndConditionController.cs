using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using Frayte.Services.Business;

namespace Frayte.WebApi.Controllers
{
    [Authorize]
    public class TermAndConditionController : ApiController
    {
        [HttpGet]
        public TermAndCondition GetTermAndCondition(int termConditionId)
        {
            TermAndCondition termAndCondition = new TermAndCondition();
             termAndCondition = new TermAndConditionRepository().GetTermAndCondition(termConditionId);
            return termAndCondition;
        }
        [HttpGet]

       
        public List<TermAndCondition> GetAllTermsAndCondition(int OperationZoneId , int userId)
        {
            var termAndConditions = new TermAndConditionRepository().GetAllTermAndCondition(OperationZoneId, userId);
            return termAndConditions;
        }

        [HttpGet]
        [AllowAnonymous]
        public TermAndCondition GetlatestTermsAndCondition(int OperationZoneId, string TermAndCondtionType , string shortCode)
        {
            var termAndConditions = new TermAndConditionRepository().GetLatestTermAndCondition(OperationZoneId, TermAndCondtionType, shortCode);
            return termAndConditions;
        }

        [HttpPost]
        public IHttpActionResult SaveTermAndCondition(TermAndCondition termAndCondition)
        {
            TermAndCondition result = new TermAndConditionRepository().SaveTermAndCondition(termAndCondition);
            if (result == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(result.TermAndConditionId);
            }
        }

    }
}
