using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public class FrayteZoneCountryPostCode
    {
        public int LogisticZoneCountryPostCodeId { get; set; }
        public int OperationZoneId { get; set; }
        public string LogisticCompany { get; set; }
        public string LogisticType { get; set; }
        public string RateType { get; set; }
        public string CountryName { get; set; }
        public string FromPostCode { get; set; }
        public string ToPostCode { get; set; }
        public string Zone { get; set; }
        public string ZoneDisplay { get; set; }
    }
}
