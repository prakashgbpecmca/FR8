using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.Express
{
    public class ExpressGetShipmentModel
    {
        public int ExpressShipmentId { get; set; }
        public string Shipper { get; set; }
        public int BagId { get; set; }
        public string HubCode { get; set; }
        public string Receiver { get; set; }
        public string Carrier { get; set; }
        public int CustomerId { get; set; }
        public string Status { get; set; }
        public string StatusDisplay { get; set; }
        public string Customer { get; set; }
        public string MAWB { get; set; }
        public string AWBNumber { get; set; }
        public string TrackingNumber { get; set; }
        public string BagBarcodeNo { get; set; }
        public int TotalRows { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedOnTime { get; set; }
        public decimal TotalWeight { get; set; }
        public decimal TotalCarton { get; set; }
        public bool IsMissing { get; set; }
        public bool IsExpired { get; set; }

    }

    public class ExpressTrackDirectBooking
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int ShipmentStatusId { get; set; }
        public string AWBNumber { get; set; }
        public string MAWB { get; set; }
        public int CurrentPage { get; set; }
        public int TakeRows { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int UserId { get; set; }
        public int SpecialSearchId { get; set; }
        public string CourierTrackingNo { get; set; }
    }

}

