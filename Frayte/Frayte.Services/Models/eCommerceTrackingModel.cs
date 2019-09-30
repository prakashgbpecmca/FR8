using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public class eCommerceManualTracking
    {
        public int eCommerceTrackingId { get; set; }
        public int eCommerceShipmentId { get; set; }
        public string TrackingNumber { get; set; }
        public string FrayteNumber { get; set; }
        public string TrackingDescription { get; set; }
        public string TrackingDescriptionCode { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class eCommTracking
    {
        public bool SentEmailToReceiver { get; set; }
      public  eCommerceManualTracking Tracking { get; set; }
    }
    public static class eCommerceTrackingMode
    {
        public const string ManualTracking = "Manual";
    }
}
