using Frayte.Api.Business;
using Frayte.Api.Models;
using Frayte.Services;
using Frayte.Services.Business;
using Frayte.Services.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Frayte.Api.Controllers
{
    public class TrackingController : ApiController
    {

        [HttpPost]
        public IHttpActionResult GetTrackInfo(FrayteTrackDto frayteTrack)
        {
            if (!string.IsNullOrWhiteSpace(frayteTrack.TrackRequest.TrackingNumber))
            {
                FrayteShipmentTracking trayteShipmentTracking = new FrayteShipmentTracking();
                
                trayteShipmentTracking = new AftershipTrackingRepository().GetTracking(frayteTrack.TrackRequest.Courier, frayteTrack.TrackRequest.TrackingNumber);
                                
                if (trayteShipmentTracking != null)
                {
                 
                    return Ok(trayteShipmentTracking);
                }
                else
                {
                    return Ok(new { Status = true, Message = "There is No Tracking Information availble", });
                }
            }
            return Ok(new { Status = true, Message = "There is No Tracking Information availble", });
        }
    }
}