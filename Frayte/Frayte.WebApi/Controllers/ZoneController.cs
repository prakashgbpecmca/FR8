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
    public class ZoneController : ApiController
    {
        [HttpGet]
        public List<FrayteZone> GetZoneList(int OperationZoneId, string LogisticType, string CourierCompany, string RateType, string ModuleType)
        {
            List<FrayteZone> list = new List<FrayteZone>();
            if (!string.IsNullOrEmpty(LogisticType) && !string.IsNullOrEmpty(RateType))
            {
                list = new ZoneRepository().GetZoneDetail(OperationZoneId, LogisticType, CourierCompany, RateType, ModuleType);
            }
            else
            {
                list = new ZoneRepository().GetZoneDetail(OperationZoneId, CourierCompany, ModuleType);
            }
            return list;
        }

        [HttpGet]
        public List<FrayteZone> ZoneList()
        {
            var list = new ZoneRepository().GetZoneList();
            return list;
        }

        [HttpGet]
        public List<FrayteZone> ZoneListByOperationZone(int OperationZoneId)
        {
            var list = new ZoneRepository().GetZoneList(OperationZoneId);
            return list;
        }
    }
}
