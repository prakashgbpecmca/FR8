using Frayte.Services.Models.ApiModal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frayte.Api.Models
{
    public class ExpressTrackingRequestModel
    {
        public Security Security { get; set; }
        public string Number { get; set; }
    }
}