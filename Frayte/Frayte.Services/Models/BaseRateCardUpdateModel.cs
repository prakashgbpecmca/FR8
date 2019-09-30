using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public class FrayteBaseRateCardZone
    {
        public int LogisticZoneId { get; set; }
        public string LogisticZoneName { get; set; }
    }

    public class FrayteBaseRateCardWeight
    {
        public int LogisticWeightId { get; set; }
        public decimal LogisticWeight { get; set; }
        public string LogisticShipmentType { get; set; }
    }
}