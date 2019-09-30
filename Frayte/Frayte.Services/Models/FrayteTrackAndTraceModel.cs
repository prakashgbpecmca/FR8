using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public class FrayteTrackAndTraceModel
    {
        public string StatusType { get; set; }
        public string CustomerName { get; set; }
        public string Courier { get; set; }
        public string TrackingNo { get; set; }
        public string FrayteNumber { get; set; }        
        public string Shipper { get; set; }
        public string Consignee { get; set; }
        public string ToCountry { get; set; }
        public string ShipmentReference { get; set; }
        public string ShipmentContent { get; set; }
        public string CollectionDateandTime { get; set; }
        public string CreatedBy { get; set; }
        public string ShipmentType { get; set; }
        public string Currency { get; set; }
        public decimal BaseRate { get; set; }
        public decimal FuelSurcharge { get; set; }
        public string FuelPercent { get; set; }
        public decimal AdditionalSurcharge { get; set; }
        public string ParcelType { get; set; }
        public string LogisticType { get; set; }
        public string PaymentPartyTaxAndDuties { get; set; }
        public string DeclaredValueCurrency { get; set; }
        public decimal ShipmentWeight { get; set; }
    }
}
