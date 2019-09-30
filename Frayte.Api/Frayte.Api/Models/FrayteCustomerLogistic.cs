using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frayte.Api.Models
{
    public class FrayteCustomerLogistic
    {
        public string CustomerName { get; set; }
        public string CourierName { get; set; }
        public string CourierDisplay { get; set; }
        public string LogisticType { get; set; }
        public string LogisticTypeDisplay { get; set; }
        public string RateType { get; set; }
        public string RateTypeDisplay { get; set; }
        public string AddressType { get; set; }
    }
}