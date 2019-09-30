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
    public class CountryController : ApiController
    {
        [HttpGet]
        public List<FrayteCountry> GetCountyList()
        {
            List<FrayteCountry> lstCountry = new List<FrayteCountry>();

            lstCountry = new CountryRepository().GetCountryList();

            return lstCountry;
        }

        [HttpGet]
        public FrayteCountry GetCountryDetail(int countryId)
        {
            FrayteCountry countryDetail = new FrayteCountry();

            countryDetail = new CountryRepository().GetCountryDetail(countryId);

            return countryDetail;
        }

        [HttpGet]
        public List<FrayteCountryCode> GetCountryCodeList()
        {
            List<FrayteCountryCode> lstCountry = new List<FrayteCountryCode>();

            lstCountry = new CountryRepository().lstCountry();

            return lstCountry;
        }

        public FrayteCountryCode GetCountryByTimezone(int timezoneId)
        {
            var country = new CountryRepository().GetCountryByTimezone(timezoneId);
            if (country != null)
            {
                return country;
            }
            return null;
        }

        [HttpGet]
        public List<FrayteCountryDocument> GetCountyDocumentList(int countryId)
        {
            List<FrayteCountryDocument> lstCountryDocument = new List<FrayteCountryDocument>();

            lstCountryDocument = new CountryRepository().GetCountyDocumentList(countryId);

            return lstCountryDocument;
        }

        [HttpPost]
        public FrayteCountry SaveCountry(FrayteCountry frayteCountry)
        {
            return new CountryRepository().SaveCountry(frayteCountry);
        }

        [HttpGet]
        public FrayteResult DeleteCountry(int countryId)
        {
            FrayteResult result = new FrayteResult();

            result = new CountryRepository().DeleteCountry(countryId);

            return result;
        }

        [HttpGet]
        public FrayteResult DeleteCountryHoliday(int countryHolydayId)
        {
            FrayteResult result = new FrayteResult();

            result = new CountryRepository().DeleteCountryHoliday(countryHolydayId);

            return result;
        }

        [HttpGet]
        public FrayteResult DeleteCountryDocument(int countryDocumentId)
        {
            FrayteResult result = new FrayteResult();

            result = new CountryRepository().DeleteCountryDocument(countryDocumentId);

            return result;
        }
    }
}
