using Frayte.Services.Models.ApiModal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frayte.Api.Models
{
   
    public class ExpressShipmentResponseModel
    {
        public int ShipmentBookingId { get; set; }
        public bool Status { get; set; }
        public string Description { get; set; }
        public string TrackingNumber { get; set; }
        public string AWBNumber { get; set; }
        public string CreatedOn { get; set; }
        public string Currency { get; set; }
        public FromAddressDto FromAddress { get; set; }
        public ToAddressDto ToAddress { get; set; }
        public ApiCustomInformation CustomInfo { get; set; }
        public string AllLlabelUrl { get; set; }
    }
 
}