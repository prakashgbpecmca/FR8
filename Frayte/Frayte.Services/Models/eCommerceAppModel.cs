using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{


    public class FrayteAWBWithLocation
    {
        public string ShipmentBarcode { get; set; }
        public string FrayteAWB { get; set; } 
        public string LocationName { get; set; }
        public string LocationBarcode { get; set; }
    }

    public class eCommShipmentLocation
    {
        public int UserId { get; set; }
        public string LocationBarcode { get; set; }
        public List<string> ShipmentBarcodes { get; set; }
    }

    public class FrayteWarehouseLocation
    { 
        public string LocationName { get; set; }
        public string Barcode { get; set; }
    }

    public class FrayteECommerceBag
    {
        public string BagNumber { get; set; }
        public List<string> ShipmentBarcodeNumbers { get; set; }
    }

    public class eCommerceReportBagLabel
    {
        public FrayteCountryCode DestinationCountry { get; set; }
        public int ShipmentBagId { get; set; }
        public string BagName { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Date { get; set; }
        public string BarCodePath { get; set; }
        public string AgentName { get; set; }
        public string FlightNumber { get; set; }
        public int CurrentBag { get; set; }
        public int TotalBags { get; set; }
        public string MAWB { get; set; }
    }

    public class eCommerceAppResult
    {
        public bool Status { get; set; }
        public List<string> Messages { get; set; }
    }

    public class ScanBarcode
    {
        public int UserId { get; set; }
        public List<string> Barcodes { get; set; }

    }
    public class AppResult
    {
        public bool Status { get; set; }
        public List<ErrorMessage> Messages { get; set; }
    }
    public class BagResult : AppResult
    {
        public string ManifestNumber { get; set; }
    }

    public class FrayteeCommerceBag
    {
        public int UserId { get; set; }
        public bool BagClosed { get; set; }
        public string BagManifest { get; set; }
        public List<string> ShipmentBarcodes { get; set; }
    }


    public class ErrorMessage
    {
        public string Key { get; set; }
        public List<string> Values { get; set; }
    }

    public class eCommBarcode
    {
        public string Barcode { get; set; }
    }

    public class TaxAndDutyResult : AppResult
    {
        public string PaymentStatus { get; set; }
    }
}
