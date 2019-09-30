using Frayte.Services.Business;
using Frayte.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Frayte.WebApi.Controllers
{
    [Authorize]
    public class TradelaneController : ApiController
    {
        [HttpGet]
        public List<FrayteTradelane> GetTradelaneList()
        {
            List<FrayteTradelane> lstTradelanes = new List<FrayteTradelane>();
            lstTradelanes = new TradelaneRepository().GetTradelaneList();
            return lstTradelanes;
        }

        [HttpPost]
        public FrayteTradelane SaveTradelane(FrayteTradelane tradelane)
        {
            return new TradelaneRepository().SaveTradelane(tradelane);
        }

        [HttpGet]
        public FrayteResult DeleteTradelane(int tradelaneId)
        {
            FrayteResult result = new FrayteResult();
            result = new TradelaneRepository().DeleteTradelane(tradelaneId);
            return result;
        }
    }
}
