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
    public class ZonePostCodeController : ApiController
    {
        [HttpGet]
        public List<FrayteCountryUK> GetCountryUK()
        {
            var countrylist = new ZonePostCodeRepository().UKCountryList();
            return countrylist;
        }

        [HttpGet]
        public FrayteUKPostCode GetCountryUKPostCode(int OperationZoneId, string LogisticType, string CourierCompany, string RateType, string ModuleType, int ZoneId)
        {
            //It will list out all the PostCode which in not assigned to any zone yet.
            var postcode = new ZonePostCodeRepository().UKCountryPostCodeList(OperationZoneId, LogisticType, CourierCompany, RateType, ModuleType, ZoneId);
            return postcode;
        }

        [HttpGet]
        public List<FrayteZonePostCode> GetPostCodeList(int OperationZoneId, string LogisticType, string CourierCompany, string RateType, string ModuleType)
        {
            //It will list out all the PostCodes assigned to the zones.
            var list = new ZonePostCodeRepository().GetZonePostCodeList(OperationZoneId, LogisticType, CourierCompany, RateType, ModuleType);
            return list;
        }

        [HttpGet]
        public List<FrayteZonePostCode> GetZonePostCodeList(int OperationZoneId, string LogisticType, int ZoneId, string SearchPostcode, int CurrentPage, int TakeRows)
        {            
            //It will list out all the PostCodes assigned to particular zones.
            var list = new ZonePostCodeRepository().GetZonePostCodeList(OperationZoneId, LogisticType, ZoneId, SearchPostcode, CurrentPage, TakeRows);
            return list;
        }

        [HttpPost]
        public IHttpActionResult SavePostCode(FraytePostCodeUK _postcode)
        {
            new ZonePostCodeRepository().SaveZonePostCode(_postcode);
            return Ok();
        }
    }
}
