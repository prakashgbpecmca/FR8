using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Frayte.Services.Business;
using Frayte.Services.Models;


namespace Frayte.WebApi.Controllers
{
    public class ManifestTrackingController : ApiController
    {
        [HttpPost]
        public List<ManifestTrackingModel> ManifestTracking(ManifestTrackingModel MTM)
        {
            return new ManifestTrackingRepository().ManifestTracking(MTM);
        }

        [HttpPost]
        public List<ManifestTrackingReceiver> GetManifestTracking(string ManifestId)
        {
            return new ManifestTrackingRepository().GetManifestTracking(ManifestId);
        }
    }
}
