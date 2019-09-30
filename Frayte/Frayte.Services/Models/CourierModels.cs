using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public class FrayteCourier
    {
        public int CourierId { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Website { get; set; }
        public string CourierType { get; set; }
        public DateTime TransitTime { get; set; }
        public string LatestBookingTime { get; set; }
        public List<FrayteCourierAccount> CourierAccount { get; set; }
    }

    public class FrayteShipmentCourier
    {
        public int CourierId { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Website { get; set; }
        public string CourierType { get; set; }
        public DateTime TransitTime { get; set; }
        public DateTime LatestBookingTime { get; set; }
    }

    public class FrayteShipmentMethod
    {
        public int ShipmentMethodId { get; set; }
        public string ShipmentMethodName { get; set; }
        public string ShipmentDisplayName { get; set; }
    }
}
