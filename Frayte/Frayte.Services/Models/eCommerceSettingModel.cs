using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public class eCommerceSettingModel
    {
        public int HsCodeId { get; set; }
        public string HSCode1 { get; set; }
        public string Description { get; set; }
        public string Vat { get; set; }
        public string Duty { get; set; }
        public int CountryId { get; set; }
    }
    public class eCommerceSettingCountry
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public string CountryPhoneCode { get; set; }
        public int TimeZoneId { get; set; }
        public string CountryCode2 { get; set; }
    }
}
