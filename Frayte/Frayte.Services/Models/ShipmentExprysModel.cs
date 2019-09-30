using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public class FrayteConsignment
    {
        public int BagId { get; set; }
        public string Description { get; set; }
    }

    public class FrayteShipmentBag
    {
        public int ShipmentBagId { get; set; }        
        public string BagName { get; set; }
        public string Barcode { get; set; }
        public DateTime CreatedOn { get; set; }
        //public List<FrayteShipmentBagDetail> FrayteShipmentBagDetail { get; set; }        
    }

    public class FrayteShipmentBagDetail
    {
        public int ShipmentBagDetailId { get; set; }
        public int ShipmentBagId { get; set; }
        public string FrayteAWB { get; set; }
        public int CartonQty { get; set; }
        public int ShipmentId { get; set; }
    }
    public class FrayteShipmentExpreysBagDetail
    {
        public int ShipmentBagId { get; set; }
        public string BagName { get; set; }
        public string Barcode { get; set; }
        public DateTime CreatedOn { get; set; }
        public FrayteShipmentBagDetail BagDetail { get; set; }
    }

    public class FrayteAWBShipmentId
    {
        public int ShipmentId { get; set; }
        public string FrayteAWB { get; set; }
    }
}
