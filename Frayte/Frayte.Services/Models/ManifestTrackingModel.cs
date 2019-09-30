using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public class ManifestTrackingModel
    {
        public int eCommerceShipmentId { get; set; }
        public int ManifestId { get; set; }
        public int UserId { get; set; }
        public string TrackingNo { get; set; }
        public string FrayteNo { get; set; }
        public string TrackingDescription { get; set;}
        public string TrackingDescriptionCode { get; set; }
        public string TrackingMode { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOnUtc {  get; set;}
        public string shipToEmail { get; set; }

        public string ReceiverName { get; set; }

    }
}
