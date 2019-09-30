using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.Tradelane
{
    public class TradelaneShipmentDetailModel
    {
        public string Mawb { get; set; }
        public string FrayteRefNo { get; set; }
        public string DepartureAirport { get; set; }
        public string ArrivalAirport { get; set; }
        public TradelaneAirline Airlines { get; set; }
        public int TotalPieces { get; set; }
        public decimal TotalWeight { get; set; }
        public decimal Volume { get; set; }
    }   
}
