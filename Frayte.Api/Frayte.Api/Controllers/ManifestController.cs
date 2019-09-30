using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Frayte.Api.Controllers
{
    public class ManifestController : ApiController
    {
        public IHttpActionResult GetManifestInfo()
        {
            return Ok();
        }
    }
}
