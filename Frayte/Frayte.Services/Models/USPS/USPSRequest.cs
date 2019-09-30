using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.USPS
{
    public class USPSRequest
    {
        public USPSHeader header { get; set; }
        public USPSShipment shipment { get; set; }
    }

    public class USPSHeader
    {
        public string key { get; set; }
        public string version { get; set; }
    }

    public class USPSShipment
    {
        public string date { get; set; }
        public string labelType { get; set; }
        public string mailClass { get; set; }
        public string packageType { get; set; }
        public decimal weight { get; set; }
        public USPSDimensions dimensions { get; set; }
        public USPSSenderAddress sender { get; set; }
        public USPSSenderAddress receiver { get; set; }
        public USPSOptions options { get; set; }
        public List<USPSItems> items { get; set; }
    }

    public class USPSDimensions
    {
        public decimal length { get; set; }
        public decimal width { get; set; }
        public decimal height { get; set; }
    }

    public class USPSSenderAddress
    {
        public string name { get; set; }
        public string company { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string zip4 { get; set; }
        public string country { get; set; }
        public string phone { get; set; }
    }

    public class USPSOptions
    {
        public string signature { get; set; }
        public int insurance { get; set; }
    }

    public class USPSItems
    {
        public string description { get; set; }
        public decimal unitValue { get; set; }
        public decimal weight { get; set; }
        public string weightUnit { get; set; }
        public int quantity { get; set; }
    }    
}
