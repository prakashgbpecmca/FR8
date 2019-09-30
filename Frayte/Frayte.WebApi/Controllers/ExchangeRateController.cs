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
    public class ExchangeRateController : ApiController
    {
        [HttpPost]
        public IHttpActionResult SaveExchangeRate(List<FrayteExchangeRate> exchangeRate)
        {
            new ExchangeRateRepository().SaveExchangeRate(exchangeRate);
            return Ok();
        }

        [HttpGet]
        public List<FrayteExchangeRate> GetOperationExchangeRate(int operationZoneId)
        {
            var list = new ExchangeRateRepository().GetOperationExchangeRateDetail(operationZoneId);
            return list;
        }

        [HttpGet]
        public List<CurrencyType> GetCurrencyDetail()
        {
            var list = new ExchangeRateRepository().GetCurrency();
            return list;
        }

        [HttpGet]
        public List<FrayteOperationZone> GetOperationZone()
        {
            var list = new BaseRateCardRepository().GetOperationZone();
            return list;
        }

        [HttpGet]
        public List<int> DistinctYear(int OperationZoneId, string Type)
        {
            List<int> _year = new ExchangeRateRepository().GetDistinctYear(OperationZoneId, Type);
            return _year;
        }

        [HttpGet]
        public List<FrayteExchangeMonth> DistinctMonth(int OperationZoneId, string Type)
        {
            List<FrayteExchangeMonth> _month = new ExchangeRateRepository().GetDistinctMonth(OperationZoneId, Type);
            return _month;
        }

        [HttpPost]
        public List<FrayteExchangeRateHistory> ExchangeRateHistory(FrayteSearchExchangeHistory search)
        {
            List<FrayteExchangeRateHistory> _rate = new ExchangeRateRepository().GetExchangeRateHistory(search);
            return _rate;
        }
    }
}
