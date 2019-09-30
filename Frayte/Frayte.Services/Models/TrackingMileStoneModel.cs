using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public class TrackingMileStoneModel
    {
        public int TrackingMileStoneId { get; set; }
        public string MileStoneKey { get; set; }
        public string Description { get; set; }
        public int OrderNumber { get; set; }
        public int ShipmentHandlerMethodId { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public int CreatedBy { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        public int UpdatedBy { get; set; }
        
    }
}
