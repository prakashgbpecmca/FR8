using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public class FrayteTracingComment
    {
        public int TracingCommentId { get; set; }
        public string Description { get; set; }
    }

    public class FrayteShipmentTracing
    {
        public string Comment { get; set; }
        public DateTime CommentDate { get; set; }
        public int ShipmentBagId { get; set; }
    }

    public class FrayteShipmentDetailSave
    {
        public List<FrayteShipmentTracing> FrayteShipmentTracingSave { get; set; }
    }

    public class FrayteShipmentTracingDetail
    {
        public string FrayteAWB { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Comment { get; set; }
        public DateTime CommentDate { get; set; }        
    }
}
