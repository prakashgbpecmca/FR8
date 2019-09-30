using Frayte.Services.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public class GrayteGoogleMap
    {
        public decimal latitude { get; set; }
        public decimal longitude { get; set; }
    }
    public class FrayteWarehouse
    {
        public int WarehouseId { get; set; }        
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public FrayteCountryCode Country { get; set; }
        public WorkingWeekDay WorkingWeekDay { get; set; }
        public string LocationName { get; set; }
        public GrayteGoogleMap MapDetail { get; set; }
        public int Zoom { get; set; }
        public string LocationMapImage { get; set; }
        public GrayteGoogleMap MarkerDetail { get; set; }
        public FrayteCustomerAssociatedUser Manager { get; set; }
        public string Email { get; set; }
        public string TelephoneNo { get; set; }
        public string MobileNo { get; set; }
        public string Fax { get; set; }
        public string WorkingStartTime { get; set; }
        public string WorkingEndTime { get; set; }
        public TimeZoneModal Timezone { get; set; }
    }

    public class ShipmentWarehouse
    {
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }

        public int CountryId { get; set; }
    }
}
