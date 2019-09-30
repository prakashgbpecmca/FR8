using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Frayte.Services.Business;
using Frayte.Services.Models;
using Frayte.Services.Models.Tradelane;

namespace Frayte.WebApi.Controllers
{
    public class TradelaneUpdateTrackingController : ApiController
    {

        [HttpPost]
        public IHttpActionResult SaveTradelaneShipmentOperationalTracking(TradelaneOperationslTrackingModel TrackingList)
        {
            var res = new UpdateTradelaneTrackingRepository().SaveTradelaneShipmentTracking(TrackingList);
            return Ok(res);
        }

        [HttpPost]
        public IHttpActionResult SaveTradelaneTracking(TradelaneUpdateTrackingModel TrackingList)
        {
            var a = new UpdateTradelaneTrackingRepository().SaveTradelaneTracking(TrackingList);
            return Ok(a);
        }
        [HttpGet]
        public TradelaneTrackingModel GetTradelaneShipmentTracking(int TradelaneShipmentId)
        {
            return new UpdateTradelaneTrackingRepository().GetTradelaneShipmentTracking(TradelaneShipmentId);

        }

        [HttpGet]
        public List<TradelaneAirport> GetAirPorts()
        {
            return new UpdateTradelaneTrackingRepository().GetAirports();
        }

        [HttpGet]
        public List<TrackingMileStoneModel> GetTradelaneMilestone(int ShipmentHandlerMethodId)
        {
            return new UpdateTradelaneTrackingRepository().GetTradelaneMilestone(ShipmentHandlerMethodId);
        }

        [HttpGet]
        public int GetShipmentHandlerMethodId(int TradelaneShipmentId)
        {
            return new UpdateTradelaneTrackingRepository().GetTradelaneShipemntHandlerMethodId(TradelaneShipmentId);
        }

        [HttpGet]
        public FrayteResult DeleteTradelaneTracking(int FlightDetailTrackingId)
        {
            return new UpdateTradelaneTrackingRepository().DeleteTradelaneTracking(FlightDetailTrackingId);
        }

        [HttpGet]
        public FrayteResult DeleteTradelaneOperationalTracking(int TradelaneShipmentTrackingId)
        {
            return new UpdateTradelaneTrackingRepository().DeleteTradelaneOperationalTracking(TradelaneShipmentTrackingId);
        }

        [HttpGet]
        public TradelaneShipmentDetailModel GetShipmentDetail(int TradelaneShipmentId)
        {
            return new UpdateTradelaneTrackingRepository().GetShipmentDetail(TradelaneShipmentId);
        }

        [HttpGet]
        public List<TradelaneTrackingShipmentStatus> GetShipmentTrackingStatus(int TradelaneShipmentId)
        {
            return new UpdateTradelaneTrackingRepository().GetShipmentTrackingStatus(TradelaneShipmentId);
        }

        [HttpGet]
        public int GetTradelaneTracking(string Number, string NumberType)
        {
            return new UpdateTradelaneTrackingRepository().GetTradelaneTracking(Number, NumberType);
        }
        [HttpGet]
        public List<TradelaneTracking> GetTracking(string Number)
        {
            return new UpdateTradelaneTrackingRepository().GetTracking(Number);
        }
    }
}
