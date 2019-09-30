using Frayte.Services.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public class FrayteCountryCode
    {
        public int CountryId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string CountryPhoneCode { get; set; }
        public string Code2 { get; set; }
        public TimeZoneModal TimeZoneDetail { get; set; }
    }

    public class FrayteCountryPhoneCode
    {
        public string Name { get; set; }
        public string CountryCode { get; set; }
        public string PhoneCode { get; set; }
    }

    public class FrayteCountry
    {
        public int CountryId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool Air { get; set; }
        public bool Courier { get; set; }
        public bool Expryes { get; set; }
        public bool Sea { get; set; }
        public List<CountryDocument> CountryDocuments { get; set; }
        public List<FrayteCountryPublicHoliday> CountryPublicHolidays { get; set; }
    }

    public class FrayteCountryPublicHoliday
    {
        public int CountryPublicHolidayId { get; set; }
        public DateTime PublicHolidayDate { get; set; }
        public string Description { get; set; }
    }

    public class FrayteCountryDocument
    {
        public int CountryDocumentId { get; set; }
        public int CountryId { get; set; }
        public string ShipmentType { get; set; }
        public string DocumentName { get; set; }
    }

    public class FrayteCountryState
    {
        public string StateName { get; set; }
        public string StateCode { get; set; }
    }
}
