using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frayte.Api.Models
{
    public class LabelDto
    {
        public string LogisticServiceType { get; set; }
        public string TrackingNumber { get; set; }
        public int DirectShipmentId { get; set; }
    }
    public class LabelInfo
    {
        public string TrackingNo { get; set; }
        public string Image { get; set; }       
        public int DirectShipmentId { get; set; }
        public string UniqueTrackingNumber { get; set; }
        public string LogisticLabel { get; set; }
    }
}