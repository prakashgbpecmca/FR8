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
    public class LogisticItemController : ApiController
    {

        [HttpGet]
        public List<FrayteLogisticServices> LogisticServices(int operationZoneId)
        {
            List<FrayteLogisticServices> list = new LogisticItemRepository().LogisticServices(operationZoneId);
            return list;
        }

        [HttpGet]
        public List<FrayteRateCardLogisticServices> RateCardLogisticServices(int operationZoneId)
        {
            List<FrayteRateCardLogisticServices> list = new LogisticItemRepository().RateCardLogisticServices(operationZoneId);
            return list;
        }

        [HttpGet]
        public LogisticItemList GetLogisticItemList(int OperationZoneId)
        {
            var item = new LogisticItemRepository().GetLogisticItem(OperationZoneId);
            return item;
        }

        [HttpGet]
        public LogisticItemList UserLogisticItem(int UserId)
        {
            var item = new LogisticItemRepository().UserLogisticItem(UserId);
            return item;
        }
    }
}
