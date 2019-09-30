using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public class TrackingModel
    {
        public string ModuleType { get; set; }
        public int ShipmentId { get; set; }
        public string TrackingType { get; set; }
        public string CarrierType { get; set; }
        public string Number { get; set; }
    }
}
