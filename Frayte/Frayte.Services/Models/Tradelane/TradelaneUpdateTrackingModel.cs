using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.Tradelane
{

    public class TradelaneTracking
    {
        public bool Status { get; set; }
        public string ModuleType { get; set; }
        public List<FrayteShipmentTracking> Tracking { get; set; }
        public List<FrayteShipmentTracking> ExpressTracking { get; set; }
        public List<FrayteShipmentTracking> BagTracking { get; set; }        
        public List<TradelaneTrackingModel> Trackingmodel { get; set; }
    }

    public class TradelaneTrackingModel
    {
        public List<TradelaneOperationslTrackingModel> TradelaneOperationalDetail { get; set; }
        public List<TradelaneUpdateTrackingModel> TradelaneTrackingDetail { get; set; }
        public TradelanePublicDetail ShipmentDetail { get; set; }
        public List<TradelaneTrackingShipmentStatus> TradelaneStatus { get; set; }
    }

    public class TradelanePublicDetail
    {
        public string FrayteNumber { get; set; }
        public string Mawb { get; set; }
        public int TotalPieces { get; set; }
        public string CurrentStatus { get; set; }
        public decimal EstimatedWeight { get; set; }
        public DateTime UpdatedOn { get; set; }
        public decimal TotalVolume { get; set; }
        public string DepartureAirportCode { get; set; }
        public string DestinationAirportCode { get; set; }
    }

    public class TradelaneUpdateTrackingModel
    {
        public int TradelaneFlightId { get; set; }
        public int TradelaneShipmentId { get; set; }
        public string FlightNo { get; set; }
        public DateTime? DepartureDate { get; set; }
        public string DepartureTime { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public string ArrivalTime { get; set; }
        public string DepartureAirportCode { get; set; }
        public string DestinationAirportCode { get; set; }
        public int TotalPeices { get; set; }
        public decimal TotalWeight { get; set; }
        public decimal Volume { get; set; }
        public string BookingStatus { get; set; }
    }

    public class TradelaneOperationslTrackingModel
    {
        public int TradelaneShipmentTrackingId { get; set; }
        public int TradelaneShipmentId { get; set; }
        public string TrackingCode { get; set; }
        public string FlightNo { get; set; }
        public int Pieces { get; set; }
        public decimal Weight { get; set; }
        public string TrackingDescription { get; set; }
        public string AirportCode { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public int CreatedBy { get; set; }
    }

    public class TradelaneTrackingShipmentStatus
    {
        public string AirportCode { get; set; }
        public List<TradelaneTrackingStatus> TrackingStatus { get; set; }
    }

    public class TradelaneTrackingStatus
    {
        public string ShipmentCode { get; set; }
        public int Pieces { get; set; }
        public decimal TotalWeight { get; set; }
        public DateTime? Date { get; set; }
    }
}
