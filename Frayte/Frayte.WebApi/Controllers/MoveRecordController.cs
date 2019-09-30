using Frayte.Services.Business;
using Frayte.Services.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Frayte.WebApi.Controllers
{

    public class MoveRecordController : ApiController
    {
        [HttpGet]
        public IHttpActionResult MoveTableValue()
        {
            bool status = new MoveRecordFromLiveToTestRepository().MoveRecord();
            return Ok(status);
        }
    }
}
