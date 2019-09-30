using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Frayte.Services.Models;
using Frayte.Services.Utility;
using Frayte.Services.Business;

namespace Frayte.WebApi.Controllers
{
    public class TrackingController : ApiController
    {
        [HttpGet]     
        public TrackingModel GetShipmentDetail(string FrayteNumber)
        {
            var Result = new TrackingRepository().ShipmentFilter(FrayteNumber);
        
            return new TrackingRepository().GetShipmentDetail(Result.ModuleType, Result.TrackingType, FrayteNumber);
        }
    }
}
