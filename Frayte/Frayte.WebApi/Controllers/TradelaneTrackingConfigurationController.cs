using Frayte.Services.Business;
using Frayte.Services.Models;
using Frayte.Services.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Frayte.WebApi.Controllers
{
    public class TradelaneTrackingConfigurationController : ApiController
    {
        [HttpGet]
        public List<ShipmentHandlerModel> GetShimentHandlerMethods()
        {
            return new TradelaneTrackingConfigurationRepository().GetShimentHandlerMethods();
        }

        [HttpGet]
        public List<TrackingMileStoneModel> GetTrackingMileStone(int ShipmentHandlerMethodId)
        {
            return new TradelaneTrackingConfigurationRepository().GetTrackingMileStone(ShipmentHandlerMethodId);
        }

        [HttpPost]
        public IHttpActionResult SavingTrackingConfiguration(TradelaneTrackingConfigurationModel TradelaneConfig)
        {
            var a = new TradelaneTrackingConfigurationRepository().SavingTrackingConfiguration(TradelaneConfig);
            return Ok(a);
        }

        [HttpGet]
        public bool DeleteTrackingConfigurationDetail(int TradelaneUserTrackingConfigurationDetailId)
        {
            var a = new TradelaneTrackingConfigurationRepository().DeleteTrackingConfigurationDetail(TradelaneUserTrackingConfigurationDetailId);
            return a;
        }

        [HttpGet]
        public List<TradelaneTrackingMileStoneModel> GetTrackingConfiguration(int UserId, string TrackIds)
        {
            return new TradelaneTrackingConfigurationRepository().GetTrackingConfiguration(UserId, TrackIds);
        }
        [HttpGet]
        public TradelaneTrackingPreAlert GetPreAlert(string UserId)
        {
            return new TradelaneTrackingConfigurationRepository().GetPreAlert(UserId);
        }
        //[HttpGet]
        //public TradelaneTrackingPreAlert GetTrackingconfigCustomer(string UserId)
        //{
        //    return new TradelaneTrackingConfigurationRepository().GetTrackingconfigCustomer(UserId);
        //}
    }
}
