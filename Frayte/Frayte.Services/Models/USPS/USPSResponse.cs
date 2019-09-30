using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.USPS
{
    public class USPSResponse : USPSError
    {
        public string trackingNumber { get; set; }
        public string labelImage { get; set; }
        public string labelZplCode { get; set; }
        public int postageBalance { get; set; }
        public USPSQuote quote { get; set; }
    }

    public class USPSError
    {
        public string error { get; set; }
    }

    public class USPSQuote
    {
        public decimal quotedWeight { get; set; }
        public string quotedWeightUnit { get; set; }
        public string stringquotedWeightType { get; set; }
        public int baseCharge { get; set; }
        public int total { get; set; }
        public List<USPSExtraServices> extraServices { get; set; }
    }

    public class USPSExtraServices
    {
        public int data { get; set; }
    }
}
