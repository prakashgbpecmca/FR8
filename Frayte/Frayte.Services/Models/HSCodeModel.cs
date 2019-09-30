using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public class NonHsCodeShipments
    {
        public int ShipmentId { get; set; } 
        public string Customer { get; set; }
        public string ShippedFromCompany { get; set; }
        public string ShippedToCompany { get; set; }
        public string ShipmentDetail { get; set; }
        public string CourierCompany { get; set; }
        public string CourierCompanyDisplay { get; set; }
        public DateTime ShippingDate { get; set; }
        public string Status { get; set; }
        public string DisplayStatus { get; set; }
        public string Reference1 { get; set; }
        public string FrayteNumber { get; set; }
        public string TrackingNo { get; set; }
        public int TotalRows { get; set; }
        public int ManifestId { get; set; }
        public string ManifestName { get; set; }
    }

    public class TrackHSCodeShipment
    {
        public string ModuleType { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int ShipmentStatusId { get; set; }
        public string FrayteNumber { get; set; } 
        public string TrackingNo { get; set; } 
        public int CurrentPage { get; set; }
        public int TakeRows { get; set; }
        public int CustomerId { get; set; }
        public int OperationZoneId { get; set; }
    }

    public class HSCodeDescription
    {
        public int HsCodeId { get; set; }
        public string HSCode { get; set; }
        public string Description { get; set; }
    }

    public static class HSSeachType
    {
        public const string HSCode = "HSCode";
        public const string HSDescription = "HSDescription";
    }

}
