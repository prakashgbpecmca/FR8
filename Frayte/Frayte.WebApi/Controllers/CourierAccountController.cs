using Frayte.Services.Business;
using Frayte.Services.Models;
using Frayte.Services.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Frayte.WebApi.Controllers
{
    [Authorize]
    public class CourierAccountController : ApiController
    {
        [HttpPost]
        public IHttpActionResult SaveCourierAccount(FrayteCourierAccount fcAccount)
        {
            new CourierAccountRepository().AddCourierAccount(fcAccount);
            return Ok();
        }

        [HttpGet]
        public List<FrayteCourierAccount> GetCourierAccounts(int OperationZoneId, string LogisticType, string CourierCompany, string RateType, string ModuleType)
        {
            var CourierAccounts = new CourierAccountRepository().GetCourierAccounts(OperationZoneId, LogisticType, CourierCompany, RateType, ModuleType);
            return CourierAccounts;
        }

        public List<FrayteCourierAccount> GetCourierAccounts()
        {
            var CourierAccounts = new CourierAccountRepository().GetCourierAccounts();
            return CourierAccounts;
        }

        [HttpGet]
        public List<FrayteShipmentCourier> ShipmentCourierList()
        {
            var list = new CourierAccountRepository().GetShipmentCourierList();
            return list;
        }

        [HttpGet]
        public List<FrayteOperationZone> OperationZoneList()
        {
            var list = new CourierAccountRepository().GetOperationZone();
            return list;
        }

        [HttpGet]
        public FrayteResult DeleteCourierAccount(int CourierAccountId)
        {
            return new CourierAccountRepository().DeleteCourierAccount(CourierAccountId);
        }
    }
}
