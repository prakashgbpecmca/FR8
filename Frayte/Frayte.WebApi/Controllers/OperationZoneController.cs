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
    [Authorize]
    public class OperationZoneController : ApiController
    {
        [HttpGet]
        [AllowAnonymous]
        public List<OperationZoneModel> OperationZone()
        {
            var list = new OperationZoneRepository().GetOperationZone();
            return list;
        }
        [HttpGet]
        [AllowAnonymous]
        public FrayteOperationZone GetCurrentOperationZone()
        {
            var OperationZone =  UtilityRepository.GetOperationZone();
            return OperationZone;
        }
         
    }
}
