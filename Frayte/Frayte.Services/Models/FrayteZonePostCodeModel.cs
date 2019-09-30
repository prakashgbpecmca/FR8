using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public class FrayteZonePostCode
    {
        public int ZonePostCodeId { get; set; }
        public FrayteZone PostCodeZone { get; set; }
        public string PostCode { get; set; }
        public bool IsActive { get; set; }
        public int TotalRows { get; set; }
        public string LogisticCompany { get; set; }
        public int TransitTime { get; set; }
    }

    public class FraytePostCodeUK
    {
        public int LogisticServiceZoneId { get; set; }
        public int TransitTime { get; set; }
        public List<FrayteZonePostCode> FrayteZonePostCodeList { get; set; }
    }

        public class FrayteCountryUK
    {
        public int CountryUKId { get; set; }
        public string CountryName { get; set; }
    }

    public class FrayteCountryUKPostCode
    {
        public int CountryUKId { get; set; }
        public string PostCode { get; set; }
    }

    public class FrayteUKPostCode
    {
        public int TransitTime { get; set; }
        public List<FrayteCountryUKPostCode> FrayteZonePostCodeList { get; set; }
    }
}
