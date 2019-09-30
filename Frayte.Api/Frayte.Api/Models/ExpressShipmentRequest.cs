using Frayte.Services.Models.ApiModal;
using Frayte.Services.Models.Express;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frayte.Api.Models
{
    public class ExpressShipmentRequest
    {
        public Security Security { get; set; }
        public string AWBNumber { get; set; }
        public ShipFrom ShipFrom { get; set; }
        public ShipTo ShipTo { get; set; }
        public List<Package> Package { get; set; }
        public ApiCustomInformation CustomInformation { get; set; }
        public string ServiceCode { get; set; }
        public HubService Service { get; set; }
        public string DeclaredCurrencyCode { get; set; }
        public string ShipmentReference { get; set; }
        public string PaymentPartyTaxAndDuties { get; set; }
        public string PackageCalculationType { get; set; }

    }
}