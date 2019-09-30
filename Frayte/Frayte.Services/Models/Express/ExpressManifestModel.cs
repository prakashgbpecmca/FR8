using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.Express
{
    public class ExpressManifestModel
    {
        public int ManifestId { get; set; }
        public int TradelaneShipmentId { get; set; }
        public int TradelaneshipmentStatusId { get; set; }
        public string ManifestName { get; set; }
        public string Courier { get; set; }
        public string CourierDisplay { get; set; }
        public int NoOfBags { get; set; }
        public int NoOfShipments { get; set; }
        public float TotalWeight { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedOnTime { get; set; }
        public int CustomerId { get; set; }
        public string ModuleType { get; set; }
        public string SubModuleType { get; set; }
        public int TotalRows { get; set; }
        public string Customer { get; set; }
        public string MAWB { get; set; }
    }

    public class ExpressTrackManifest
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

    public class ExpressViewManifest
    {
        public int ManifestId { get; set; }
        public string ManifestName { get; set; }
        public List<ExpressManifestDetail> ManifestedList { get; set; }
        public List<ExpressGetShipmentModel> ManifestedShipmentList { get; set; }
    }

    public class ExpressManifestDetail
    {
        public int BagId { get; set; }
        public string Customer { get; set; }
        public string BagNumber { get; set; }
        public string Carrier { get; set; }
        public int TotalShipments { get; set; }
        public decimal TotalWeight { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedOnTime { get; set; }
    }

    public class HubDetailModel
    {
        public int HubId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string DefaultCurrency { get; set; }
      
    }
    public class ExpressDownloadPDFModel
    {
        public int TradelaneShipmentId { get; set; }
        public int CustomerId { get; set; }
        public int UserId { get; set; }
    }

    public class ExportManifestPdfModel
    {
        public string PickUpAddress { get; set; }
        public string ExportManifestName { get; set; }
        public string BarCode { get; set; }
        public string PrintedBy { get; set; }
        public string PrintedOn { get; set; }
        public string Notes { get; set; }
        public List<ExpressMawbInformationModel> MawbInfo { get; set; }
        public List<BagDetail> BagsInfo { get; set; }
    }

    public class ExpressMawbInformationModel
    {
        public string Mawb { get; set; }
        public string FlightNo { get; set; }
        public string Airline { get; set; }
        public string ETD { get; set; }
        public string ETA { get; set; }
        public string ETDTimeZone { get; set; }
        public string ETATimeZone { get; set; }
    }

    public class BagDetail
    {
        public string ExporterName { get; set; }
        public string Carrier { get; set; }
        public string BagNumber { get; set; }
        public int TotalPieces { get; set; }
        public decimal TotalWeight { get; set; }
        public int MissingShipments { get; set; }
        public int DeliveredShipments { get; set; }
        public int TotalShipments { get; set; }
        public string Mawb { get; set; }
        public string HubCode { get; set; }
    }

    public class ExpressDownloadDriverIds
    {
        public int UserId { get; set; }
        public int TradelaneShipmentId { get; set; }
       
    }
    
}
