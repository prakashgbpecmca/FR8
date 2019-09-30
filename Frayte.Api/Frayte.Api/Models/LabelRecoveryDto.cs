using Frayte.Services.Models.ApiModal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frayte.Api.Models
{
    public class LabelRecoveryDto
    {
        public Security Security { get; set; }
        public LabelRequestDto  LabelRequest { get; set; }

    }

    public class LabelRequestDto
    {
        public string TrackingNumber { get; set; }
        public string Courier { get; set; }
        public string LabelType { get; set; }
    }

    public class LabelResponeDto
    {
        public bool Status { get; set; }
        public string Discription { get; set; }
        public string TrackingNo { get; set; }
        
        public List<LabelDetails> LabelDetail { get; set; }
    }
    public class LabelDetails
    {
        public string TrackingNo { get; set; }
        public string LabelType { get; set; }
        public string LabelUrl { get; set; }
        public string LabelName { get; set; }
       // public string ImageBase64 { get; set; }
    }
}