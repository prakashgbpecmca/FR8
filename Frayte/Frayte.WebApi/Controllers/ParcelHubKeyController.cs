using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Frayte.Services.Business;
using Frayte.Services.DataAccess;
using Frayte.Services.Models;

namespace Frayte.WebApi.Controllers
{
    [Authorize]
    public class ParcelHubKeyController : ApiController
    {
        public IHttpActionResult GetParcelHubApiKeys()
        {
            var list = new ParcelHubKeyRepository().GetParcelHubApiKeys();

            return Ok(list);
        }

        public FrayteResult SaveParcelHubKey(ParcelHubKeyModel ParcelHubKey)
        {
            FrayteResult result = new FrayteResult();
            result = new ParcelHubKeyRepository().SaveParcelHubKey(ParcelHubKey);
            return result;
        }

        [HttpPost]
        public FrayteResult DeleteParcelHubKey(int APIId)
        {
            FrayteResult result = new FrayteResult();
            result = new ParcelHubKeyRepository().DeleteParcelHubKey(APIId);
            return result;
        }
             
    }
}
