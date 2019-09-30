using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public class FrayteResult
    {
        public bool Status { get; set; }
        public List<string> Errors { get; set; }
    }
  
    public class FrayteQuotationResult
    {
        public bool Status { get; set; }
        public FrayteQuotationShipment QuotationDetail { get; set; }
        public FrayteSurchargeDetail SurchargeDetail { get; set; }
        public List<string> Errors { get; set; }
    }    

    public class FryatePODResult
    {
        public string PDFPath { get; set; }        
    }

    public class FrayteDirectShipmentTrackingObject
    {
        public int DirectShipmentId { get; set; }
        public string eventId { get; set; }
        public DateTime timestamp { get; set; }
        public string details { get; set; }
        public string eventType { get; set; }
        public string eventClass { get; set; }
    }

    public class FrayteParcelHubPieces
    {
        public string PartNo { get; set; }
        public List<FrayteDirectShipmentTrackingObject> TrackingDetail { get; set; }
    }

    public class FrayteStatus
    {
        public bool IsFuelSurCharge { get; set; }
        public bool IsCurrency { get; set; }
        public DateTime FuelMailSentOn { get; set; }
        public DateTime CurrencyMailSentOn { get; set; }
        public bool HistoryStatus { get; set; }
    }

    public class FrayteTNTResult
    {
        public int OperationZoneId { get; set; }
        public string LogisticCompany { get; set; }
    }

    public class FrayteCountryCurrentDateTime
    {
        public string CurrentDate { get; set; }
        public string CurrentTime { get; set; }
    }
}
