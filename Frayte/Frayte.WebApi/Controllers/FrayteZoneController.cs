using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Frayte.Services.Business;
using Frayte.Services.Models;
using Frayte.Services.DataAccess;

namespace Frayte.WebApi.Controllers
{
    [Authorize]
    public class FrayteZoneController : ApiController
    {
        [HttpPost]
        public IHttpActionResult SaveFrayteZoneCountry(FrayteZoneCountry zonecountry)
        {
            new ZoneCountryRepository().AddCountryZone(zonecountry);
            return Ok();
        }

        [HttpGet]
        public List<FrayteOperationZone> OperationZone()
        {
            var list = new ZoneCountryRepository().GetOperationZone();
            return list;
        }        

        [HttpGet]
        public List<FrayteDirectBookingCountry> Country(int OperationZoneId, int LogisticZoneId, string LogisticType, string CourierCompany, string RateType, string ModuleType)
        {
            var list = new ZoneCountryRepository().GetDirectBookingCountry(OperationZoneId, LogisticZoneId, LogisticType, CourierCompany, RateType, ModuleType);
            return list;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Staff")]
        public List<FrayteZoneCountry> ZoneCountryDetail(int OperationZoneId, string LogisticType, string CourierCompany, string RateType, string ModuleType)
        {
            var list = new ZoneCountryRepository().GetZoneCountryDetail(OperationZoneId, LogisticType, CourierCompany, RateType, ModuleType);
            return list;
        }
    }
}
