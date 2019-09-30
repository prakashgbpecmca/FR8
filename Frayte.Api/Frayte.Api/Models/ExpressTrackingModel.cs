using Frayte.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frayte.Api.Models
{
    public class ExpressTrackingModel : FrayteApiErrorDto
    {
        public List<FrayteShipmentTracking> AWBTracking { get; set; }
    }
}