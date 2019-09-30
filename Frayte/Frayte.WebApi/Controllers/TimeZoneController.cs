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
   
    public class TimeZoneController : ApiController
    {
        [HttpGet]
        public List<FrayteTimeZone> GetTimeZoneList()
        {
            List<FrayteTimeZone> lstTimeZone = new List<FrayteTimeZone>();

            lstTimeZone = new TimeZoneRepository().GetTimeZoneList();

            return lstTimeZone;
        }

        [HttpGet]
        public List<TimeZoneModal> GetShipmentTimeZones()
        {
            List<TimeZoneModal> lstTimeZone = new List<TimeZoneModal>();

            lstTimeZone = new TimeZoneRepository().GetShipmentTimeZones();

            return lstTimeZone;
        }

        [HttpPost]
        public FrayteTimeZone SaveTimeZone(FrayteTimeZone timezone)
        {
            return new TimeZoneRepository().SaveTimeZone(timezone);
        }

        [HttpGet]
        public FrayteResult DeleteTimeZone(int timezoneId)
        {
            FrayteResult result = new FrayteResult();

            result = new TimeZoneRepository().DeleteTimeZone(timezoneId);

            return result;
        }
    }
}
