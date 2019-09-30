using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Frayte.Services.Business;
using Frayte.Services.Models;
using Frayte.Services.Utility;

namespace Frayte.WebApi.Controllers
{
    public class TrackingMileStoneController : ApiController
    {

        [HttpPost]
        public IHttpActionResult SaveMileStone(TrackingMileStoneModel TMSM)
        {
            var res = new TrackingMileStoneRepository().SaveMileStone(TMSM);
            return Ok(res);
        }

        [HttpGet]
        public List<ShipmentHandlerModel> GetShimentHandlerMethods()
        {
            return new TrackingMileStoneRepository().GetShimentHandlerMethods();
        }

        [HttpGet]
        public List<TrackingMileStoneModel> GetTrackingMileStone(int ShipmentHandlerMethodId)
        {
            return new TrackingMileStoneRepository().GetTrackingMileStone(ShipmentHandlerMethodId);
        }

        [HttpGet]
        public bool DeleteMileStoneRecord(int MileStoneId)
        {
            var Result = new TrackingMileStoneRepository().DeleteMileStoneRecord(MileStoneId);
            return Result;
        }

        [HttpPost]
        public IHttpActionResult CheckTrackingMileStoneKey(TrackingMileStoneModel TrackingMSM)
        {
            var Result = new TrackingMileStoneRepository().CheckTrackingMileStoneKey(TrackingMSM);
            return Ok(Result);
        }
        [HttpPost]
        public bool CheckOrderNo(TrackingMileStoneModel order)
        {
            return new TrackingMileStoneRepository().CheckOrderNo(order);
        }

    }
}
