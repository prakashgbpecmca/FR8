using Frayte.Services.Models.ApiModal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frayte.Api.Models
{
    public class FrayteTrackDto
    {
        public Security Security { get; set; }
        public TrackRequest TrackRequest { get; set; }
    }

    public class TrackRequest
    {
        public string TrackingNumber { get; set; }  
        public string Courier { get; set; }  
    }
}