using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.Express
{
    public class ExpressEmailModel
    {
        public ExpressShipmentModel ShipmentDetail { get; set; }
        public int TotalCarton { get; set; }
        public decimal TotalWeight { get; set; }
        public string TrackingURL { get; set; }
        public string TrackingWebsite { get; set; }
        public string To { get; set; }
        public string CC { get; set; }
        public string Attachments { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerCompanyName { get; set; }
        public string CompanyName { get; set; }
        public string Name { get; set; }
        public string StaffUserName { get; set; }
        public string StaffUserEmail { get; set; }
        public string UserName { get; set; }
        public string UserPosition { get; set; }
        public string UserEmail { get; set; }
        public string SiteAddress { get; set; }
        public string Site { get; set; }
        public string UserPhone { get; set; }
        public string ImageHeader { get; set; }
        public string SystemEmail { get; set; }
        public string SiteCompany { get; set; }
        public List<ExpressShipmentBookedConsolidateModel> AWbShipmentList { get; set; }
        public List<ExpressHubShipmentCountModel> ShipList { get; set; }
        public int TotalAwbs { get; set; }
        public string CreatedOn { get; set; }
        public List<ExpressShipmentDeliveredConsolidateModel> DeliveredShipmentList { get; set; }
        public List<ExpressMissingShipmentsInfo> ShipInfo { get; set; }
        public int TotalMawb { get; set; }
        public List<ExpressDispatchedMawbModel> DispatchedMawbs { get; set; }
        //Missing Awbs
        public List<ExpressMissingShipmentModel> MissingAwbs { get; set; }
        //Send Export Manifest
        public ExpressManifestModel ManifestDetail { get; set; }
        //Send Driver Manifest
        public ExpressReportDriverManifest DriverManifestDetail { get; set; }
    }

    public class ExpressShipmentBookedConsolidateModel
    {
        public string HubCode { get; set; }
        public string Mawb { get; set; }
        public List<ExpressGetShipmentModel> Shipments { get; set; }
    }

    public class ExpressHubShipmentCountModel
    {
        public string HubCode { get; set; }
        public int ShipmentCount { get; set; }
    }

    public class ExpressShipmentDeliveredConsolidateModel
    {
        public string HubCode { get; set; }
        public List<ExpressMawbShipments> MAWBBags { get; set; }
    }

    public class ExpressMawbShipments
    {
        public string Mawb { get; set; }
        public List<BagDetail> BagInfo { get; set; }
    }

    public class ExpressShipmentInfoModel
    {
        public string totalWeight { get; set; }
        public string Mawb { get; set; }
        public List<ExpressGetShipmentModel> Shipments { get; set; }
    }

    public class ExpressMissingShipmentsInfo
    {
        public string Hub { get; set; }
        public string ShipmentCount { get; set; }
    }

    public class ExpressDispatchedMawbModel
    {
        public int TradelaneShipmentId { get; set; }
        public string Mawb { get; set; }
        public string FlightNo { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedOnTime { get; set; }
        public string ShipTo { get; set; }
        public decimal GrossWeight { get; set; }
        public int TotalCarton { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public TimeSpan ?DeliveryTime { get; set; }
        public string SignedBy { get; set; }
    }

    public class ExpressMissingShipmentModel
    {
        public int TradelaneShipmentId { get; set; }
        public string Mawb { get; set; }
        public string BagNumber { get; set; }
        public string AWBNumber { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedOnTime { get; set; }
        public string ShipFromCompany { get; set; }
        public string ShipToCompany { get; set; }
        public decimal TotalWeight { get; set; }
        public string IsAWBMissing { get; set; }
        public string IsBagMissing { get; set; }
    }
}
