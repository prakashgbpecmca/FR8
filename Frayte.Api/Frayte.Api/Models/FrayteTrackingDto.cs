using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frayte.Api.Models
{
    public class FrayteTrackingDto
    {
        public string Courier { get; set; }
        public string CourierService { get; set; }
        public string TrackingNumber { get; set; }
        public string Status { get; set; }
        public string SignedBy { get; set; }
        public string CreatedOn { get; set; }
        public string UpdatedOn { get; set; }
        public string EstimatedDeliveryOn { get; set; }
        public double? EstimatedWeight { get; set; }
        public int NoOfPieces { get; set; }
        public List<ActivityDedtail> ActivityDedtail { get; set; }
    }
    public class ActivityDedtail
    {
        public string Activity { get; set; }
        public string ActivityOn { get; set; }
        public string Location { get; set; }
        public string EventType { get; set; }
        public  List<string > Pieces { get; set; }
    }
}