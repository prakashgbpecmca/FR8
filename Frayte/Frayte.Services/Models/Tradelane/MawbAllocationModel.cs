using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.Tradelane
{

    public class MawbAllocationModel
    {
        public int MawbAllocationId { get; set; }
        public int MawbDocumentName { get; set; }
        public int TradelaneId { get; set; }
        public int TimezoneId { get; set; }
        public string MAWB { get; set; }
        public int AgentId { get; set; }
        public string FlightNumber { get; set; }
        public int AirlineId { get; set; }
        public DateTime? ETA { get; set; }
        public DateTime? ETD { get; set; }
        public string ETATime { get; set; }
        public string ETDTime { get; set; }
        public int RouteFrom { get; set; }
        public int RouteTo { get; set; }
        public string LegNum { get; set; }
        public DateTime CreatedOnUTC { get; set; }
        public int CreatedBy { get; set; }
    }

    public class ShipmentRoute
    {
        public int TradelaneId { get; set; }
        public int ShipmentHandlerMethodId { get; set; }
        public string MAWB { get; set; }
        public string ShipemntHandlerMethodCode { get; set; }
        public string RouteFrom { get; set; }
        public string RouteTo { get; set; }
    }

    public class TradelaneMAWBDetail
    {
        public int TradelaneShipmentId { get; set; }
        public int AgentId { get; set; }
        public string MAWB { get; set; }
        public string FrayteNumber { get; set; }
        public bool IsMAWBAllocated { get; set; }
        public string MAWBFileName { get; set; }

        public List<HAWBTradelanePackage> HAWBpackages { get; set; }

        public List<MawbAllocationModel> List { get; set; }

    }

    public class TradelaneMAWBUlpoadInitial
    {
        public string FrayteNumber { get; set; }
        public string Leg { get; set; }
        public int AgentId { get; set; }
        public int ShipmentMethodHandlerId { get; set; } 
    }


}
