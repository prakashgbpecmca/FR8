using Frayte.Services.Business;
using Frayte.Services.Models;
using Frayte.Services.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Frayte.WebApi.Controllers
{
    [Authorize]
    public class ZoneCountryPostCodeController : ApiController
    {
        [HttpGet]
        public List<FrayteZoneCountryPostCode> GetZoneCountryPostCode(int OperationZoneId, string LogisticCompany, string LogisticType, string RateType)
        {
            var postcode = new ZoneCountryPostCodeRepository().ZoneCountryPostCode(OperationZoneId, LogisticCompany, LogisticType, RateType);
            return postcode;
        }

        [HttpPost]
        public IHttpActionResult SaveZoneCountryPostCode(FrayteZoneCountryPostCode zonepostcode)
        {
            var result = new ZoneCountryPostCodeRepository().AddZoneCountryPostCode(zonepostcode);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult DeleteZoneCountryPostCode(int LogisticZoneCountryPostCodeId)
        {
            var result = new ZoneCountryPostCodeRepository().RemoveLogisticZoneCountryPostCodeId(LogisticZoneCountryPostCodeId);
            return Ok(result);
        }
    }
}
