using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public class FrayteCarrier 
    {
        public int CarrierId { get; set; }
        public string CarrierName { get; set; }
        public string Code { get; set; }
        public string Prefix { get; set; }

        public string CarrierType { get; set; }
    }

    public class FrayteCarrierModel
    {
        public int CarrierId { get; set; }
        public string CarrierName { get; set; }     
    }
}
