using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public partial class FrayteTradelane
    {
        public int TradelaneId { get; set; }                
        public FrayteUserModel OriginatingAgent { get; set; }
        public FrayteCountryCode OriginatingCountry { get; set; }
        public FrayteUserModel DestinationAgent { get; set; }
        public FrayteCountryCode DestinationCountry { get; set; }
        public Nullable<bool> Direct { get; set; }
        public Nullable<bool> Deffered { get; set; }
        public FrayteCarrierModel Carrier { get; set; }
        public string CarrierType { get; set; }
        public string TransitTime { get; set; }

    }
}
