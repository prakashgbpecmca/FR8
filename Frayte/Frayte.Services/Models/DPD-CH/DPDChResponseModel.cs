using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.DPD_CH
{
   public class DPDChResponseModel
    {
        public string ParcelLabelPDF { get; set; }
        public bool Status { get; set; }
        public ShipmentResponse ShipmentResponses { get; set; }
        public string Response { get; set; }
        public string Request { get; set; }
        public FratyteError Error { get; set; }
    }

    public class  ShipmentResponse
    {
        public string mpsId { get; set; }
        public List<string> ParcelLabelNumber { get; set; }
    }
}
