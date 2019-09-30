using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public class FrayteManifest
    {
        public int ManifestId { get; set; }
        public string ManifestName { get; set; }
        public string Courier { get; set; }
        public string CourierDisplay { get; set; }
        public int NoOfShipments { get; set; }
        public float TotalWeight { get; set; }
        public DateTime CreateOn { get; set; }
        public int CustomerId { get; set; }
        public string ModuleType { get; set; }
        public string SubModuleType { get; set; }
        public int TotalRows { get; set; }
        public string Customer { get; set; }
    }

    public sealed class FrayteManifestShipment
    {
        public string ModuleType { get; set; }
        public string SubModuleType { get; set; }
        public int UserId { get; set; }
        public int CreatedBy { get; set; }
        public List<FrayteUserDirectShipment> DirectShipments { get; set; }
    }

    public class FrayteViewManifest
    {
        public int ManifestId { get; set; }
        public string ManifestName { get; set; }
        public List<FrayteManifestDetail> ManifestedList{ get; set; }
    }
    public  class FrayteManifestDetail
    {
        public string CourierCompany { get; set; }
        public string RateType { get; set; }
        public string CourierCompanyDisplay { get; set; }
        public string RateTypeDisplay { get; set; }
        public List<FrayteUserDirectShipment> DirectShipments { get; set; }
    }

    public class FrayteBookingType
    {
        public string  BookingType { get; set; }
        public string BookingTypeDisplay { get; set; }
    }
    public class TrackManifest
    {
        public int UserId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string ModuleType { get; set; }
        public string subModuleType { get; set; }
        public int CurrentPage { get; set; }
        public int TakeRows { get; set; }
        public string ManifestName { get; set; }
    }

    public class ManifestReport
    {
        public string ManifestName { get; set; }
        public string ManifestFileName { get; set; }
        public string CustomerName { get; set; }
        public string CompanyName { get; set; }
        public string ManifestDate { get; set; }
        public string TimeZone { get; set; }
        public string AccountNo { get; set; }
        public int TotalShipments { get; set; }
        public float TotalWeights { get; set; }
        public string PrintedBy { get; set; }
        public List<ManifestCollection> Collection { get; set; }
    }

    public class ManifestCollection
    {
        public string Customer { get; set; }
        public string FromCompany { get; set; }
        public string ToCompany { get; set; }
        public string DisplayName { get; set; }
        public string LogisticCompany { get; set; }
        public string LogisticType { get; set; }
        public string RateTypeDisplay { get; set; }
        public DateTime ShippingDate { get; set; }        
        public string Status { get; set; }
        public string Reference { get; set; }
        public string FrayteNumber { get; set; }
        public string PlateNo { get; set; }
        public int TotalPcs { get; set; }
        public float TotalWeight { get; set; }     
    }

    public class ReportFile
    {
        public string FileName { get; set; }
        public string ModuleType { get; set; }
    }   

    public class fileName
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }

    public class ReportFileCustomerRate
    {
        public string FileName { get; set; }
        public int UserId { get; set; }
    }
}
