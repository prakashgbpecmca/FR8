using Frayte.Services.DataAccess;
using Frayte.Services.Models.Tradelane;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.Express
{

    public class ExpressBagNumberReport
    {
        public string Ref { get; set; }
        public string Hub { get; set; }
        public int TradelaneShipmentId { get; set; }
        public string Carrier { get; set; }
        public string BagCount { get; set; } 
    }

    public class ExpressReportBagLabel
    {
        public int TradelaneShipmentId { get; set; }
        public string DateTime { get; set; }
        public string Ref { get; set; }
        public string Hub { get; set; }
        public string Destination { get; set; }
        public string ServiceType { get; set; }
        public string Cargotype { get; set; }
        public string DestinationAgent { get; set; }
        public int PalletNumber { get; set; }
        public int TotalQty { get; set; }
        public decimal BagWeight { get; set; }
        public string BagBarCode { get; set; }
    }

    public class ExpressReportDriverManifest
    {
        public string Barcode { get; set; }
        public string DriverManifestName { get; set; }
        public string Ref { get; set; }
        public string Hub { get; set; }
        public string PuickUpAddress { get; set; }
        public string PrintedBy { get; set; }
        public string PrintedDateTime { get; set; }
        public string MAWB { get; set; }
        public decimal MAWBGrossWeight { get; set; }
        public decimal MAWBChargeableWeight { get; set; }
        public string FlightNumber { get; set; }
        public string Airline { get; set; }
        public string Code { get; set; }
        public string ETA { get; set; }
        public string ETATimeZone { get; set; }
        public List<ExpressReportDriverManifestBagDetail> CarrierBags { get; set; }
        public List<ExpressReportDriverCarrierManifest> CarrierManifests { get; set; }
    }

    public class ExpressReportDriverManifestBagDetail
    {
        public string CutOffTime { get; set; }
        public string Carrier { get; set; }
        public int NoOfBags { get; set; }
        public int TotalPieces { get; set; }
        public decimal TotalWeight { get; set; }
    }


    public class ExpressReportDriverCarrierManifest
    {
        public string CarrierManifest { get; set; }
        public string CarrierManifestBarcoede { get; set; }
        public string DropOffAddress { get; set; }
        public string CarrierTermAndCondition { get; set; }
        public List<ExpressReportCarrierBagDetail> CarrierBagDetails { get; set; }
    }

    public class ExpressReportCarrierBagDetail
    {
        public int BagId { get; set; }
        public string ExporterName { get; set; }
        public string Carrier { get; set; }
        public string BagNumber { get; set; }
        public int TotalPieces { get; set; }
        public decimal TotalWeight { get; set; }
        public string TermAndCondition { get; set; }
    }
}
