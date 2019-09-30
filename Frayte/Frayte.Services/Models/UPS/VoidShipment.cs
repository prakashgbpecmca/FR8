using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.UPS
{
    public class VoidShipDto
    {
        public SecurityDto UPSSecurity { get; set; }
        public VoidShipmentRequestDto VoidShipmentRequest { get; set; }
    }
    public class VoidShipmentRequestDto
    {
        public Request Request { get; set; }
        public VoidShipmentDto VoidShipment { get; set; }

    }
    public class VoidShipmentDto
    {
        public string ShipmentIdentificationNumber { get; set; }
    }
}
