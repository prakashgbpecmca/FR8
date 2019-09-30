using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.AU
{
    public class AUTrackingModel
    {
        public bool Status { get; set; }
        public List<Trackeventdetails> table { get; set; }
    }

    public class Trackeventdetails
    {
        public string invoice_number { get; set; }
        public string barcode { get; set; }
        public string trackeventdetails { get; set; }
        public string trackeventdateoccured { get; set; }
        public string city { get; set; }
        public string country { get; set; }

    }
}
