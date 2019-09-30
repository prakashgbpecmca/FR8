using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public class FrayteZone
    {
        public int ZoneId { get; set; }
        public int OperationZoneId { get; set; }
        public string ZoneName { get; set; }
        public string ZoneColor { get; set; }
        public string ZoneDisplayName { get; set; }
        public string ZoneRateName { get; set; }
        public string LogisticType { get; set; }
        public string CourierComapny { get; set; }
        public string RateType { get; set; }
        public string ModuleType { get; set; }
    }
    public class FrayteOperationZone
    {
        public int OperationZoneId { get; set; }
        public string OperationZoneName { get; set; }
    }
    public class FrayteZoneCountry
    {
        public int ZoneCountryId { get; set; }
        public int ZoneId { get; set; }
        public int OperationZoneId { get; set; }
        public string OperationZoneName { get; set; }
        public string ZoneName { get; set; }
        public string ZoneDisplayName { get; set; }
        public List<FrayteZoneCountryDetail> Fraytezonezountry { get; set; }
    }
    public class FrayteZoneCountryDetail
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public int TransitTime { get; set; }
        public bool IsSelected { get; set; }
    }
    public class FrayteDirectBookingCountry
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public int TransitTime { get; set; }
    }
}
