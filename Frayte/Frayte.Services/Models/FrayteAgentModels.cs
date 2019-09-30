using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public class FrayteAgentConfirmation 
    {
        public int ShipmentId { get; set; }
        public DateTime? OriginatingPlannedDepartureDate { get; set; }
        public string OriginatingPlannedDepartureTime { get; set; }
        public DateTime? OriginatingPlannedArrivalDate { get; set; }
        public string OriginatingPlannedArrivalTime { get; set; }
        public FrayteAddress UserAddress { get; set; }

    }
}
