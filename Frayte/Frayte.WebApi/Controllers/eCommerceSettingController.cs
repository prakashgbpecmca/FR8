using Frayte.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Frayte.Services.Business;

namespace Frayte.WebApi.Controllers
{
    public class eCommerceSettingController : ApiController
    {
        [HttpGet]
        public List<eCommerceSettingModel> GeteCommerceHSCodeDetail(int CountryId)
        {
            return new eCommerceSettingRepository().GeteCommerceHSCodeDetail(CountryId);
        }

        [HttpGet]
        public List<eCommerceSettingCountry> CountryList()
        {
            return new eCommerceSettingRepository().CountryList();
        }

        [HttpPost]
        public FrayteResult AddEditHSCodeSetting(eCommerceSettingModel HSCodeDetail)
        {
            return new eCommerceSettingRepository().AddEditHSCodeSetting(HSCodeDetail);
        }

        [HttpGet]
        public FrayteResult DeleteHSCodeSetting(int HSCodeId)
        {
            return new eCommerceSettingRepository().DeleteHSCodeSetting(HSCodeId);
        }
    }
}
