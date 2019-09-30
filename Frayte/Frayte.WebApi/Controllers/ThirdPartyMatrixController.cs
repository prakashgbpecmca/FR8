using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Frayte.Services.Business;
using Frayte.Services.Models;
using Frayte.Services.DataAccess;

namespace Frayte.WebApi.Controllers
{
    [Authorize]
    public class ThirdPartyMatrixController : ApiController
    {
        [HttpGet]
        public List<FrayteThirdPartyMatrix> GetThirdPartyMatrixDetail(int OperationZoneId, string LogisticType, string CourierCompany, string RateType, string ModuleType)
        {
            var list = new ThirdPartyMarixRepository().GetThirdPartyMatrixDetail(OperationZoneId, LogisticType, CourierCompany, RateType, ModuleType);
            return list;
        }

        [HttpPost]
        public IHttpActionResult EditThirdPartyMarix(List<FrayteThirdPartyMatrix> _party)
        {
            new ThirdPartyMarixRepository().EditThirPartyMatrix(_party);
            return Ok();
        }

        [HttpGet]
        public List<FrayteZone> GetZoneDetail(int OperationZoneId, string LogisticType, string CourierCompany, string RateType, string ModuleType)
        {
            var list = new ThirdPartyMarixRepository().GetZoneDetail(OperationZoneId, LogisticType, CourierCompany, RateType, ModuleType);
            return list;
        }

        [HttpGet]
        public List<FrayteOperationZone> GetOperationZone()
        {
            var list = new BaseRateCardRepository().GetOperationZone();
            return list;
        }
    }
}
