using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Frayte.Services.Models;

namespace Frayte.WebApi.Controllers
{
    [Authorize]
    public class FrayteTestController : ApiController
    {
        public IHttpActionResult CreateJSON(string fryateTestType)
        {
            switch (fryateTestType)
            {
                case FrayteTestType.ShipmentBagDetail:
                    FrayteShipmentBag sbm = new FrayteShipmentBag();
                    dynamic json = Newtonsoft.Json.JsonConvert.SerializeObject(sbm);
                    return Ok(json);
            }
            return Ok();
        }
    }
}
