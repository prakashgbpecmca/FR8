using Frayte.Services.Models.Aftership;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public class ShipmentTracking : ShipmentTrackingAditional
    {
        public int DirectShipmentId { get; set; }
        public string Courier { get; set; }
        public string ExpressTrackingNumber { get; set; }
        public bool IsHeaderShow { get; set; }
        public bool IsPanelShow { get; set; }
        public bool IsShowHideDetail { get; set; }
        public string ShowHideValue { get; set; }
        public string CarriertrackingId { get; set; }
        public string TrackingNumber { get; set; }
        public string Status { get; set; }
        public string StatusDisplay { get; set; }
        public int StatusId { get; set; }
        public string Carrier { get; set; }
        public string SignedBy { get; set; }
        public string CreatedAtDate { get; set; }
        public string CreatedAtTime { get; set; }
        public string UpdatedAtDate { get; set; }
        public string UpdatedAtTime { get; set; }
        public string EstimatedDeliveryDate { get; set; }
        public string EstimatedDeliveryTime { get; set; }
        public double? EstimatedWeight { get; set; }
        public int NoOfPieces { get; set; }
        public FrayteServiceProviderDetail TrackingPicesDetail { get; set; }
        public List<ShipmentTrackingDetail> TrackingDetails { get; set; }
    }

    public class ShipmentTrackingDetail
    {
        public bool IsCollapsed { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public string Tag { get; set; }
        public string Activity { get; set; }
        public string Location { get; set; }
        public string EventType { get; set; }
        public List<string> Pieces { get; set; }
    }

    public class FrayteParcelHub
    {
        public int ShipmentId { get; set; }
        public string SendersReference { get; set; }
        public string ReceiversReference { get; set; }
        public DateTime Created { get; set; }
        public string shipmentTrackingReference { get; set; }
    }

    public class FrayteParcelHubTrackingDetail
    {
        public string eventId { get; set; }
        public DateTime timestamp { get; set; }
        public string details { get; set; }
        public string eventType { get; set; }
        public string eventClass { get; set; }
    }

    public class FrayteParcelHubTracking
    {
        public List<FrayteParcelHubTrackingDetail> ParcelHubDetail { get; set; }
    }

    public class FrayteServiceProviderDetail
    {
        public string shipmentId { get; set; }
        public string sendersReference { get; set; }
        public string receiversReference { get; set; }
        public string shipmentTrackingReference { get; set; }
        public string customer { get; set; }
        public FrayteDeliveryAddress deliveryAddress { get; set; }
        public FrayteProviderDetail provider { get; set; }
        public FrayteServiceDetail service { get; set; }
        public List<string> trackingReferences { get; set; }
    }

    public class FrayteDeliveryAddress
    {
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string area { get; set; }
        public string town { get; set; }
        public string postcode { get; set; }
        public string contactName { get; set; }
        public string contactCompany { get; set; }
        public string country { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
    }

    public class FrayteProviderDetail
    {
        public string name { get; set; }
    }

    public class FrayteServiceDetail
    {
        public string name { get; set; }
        public string description { get; set; }
    }

    public class ParcelHubTrackingObject
    {
        public string eventId { get; set; }
        public DateTime timestamp { get; set; }
        public string details { get; set; }
        public string eventType { get; set; }
        public string eventClass { get; set; }
    }

    public class FraytePiecesDetail
    {
        public string PartNo { get; set; }
        public List<ParcelHubTrackingObject> TrackingDetail { get; set; }
    }

    public class FrayteShipmentTracking
    {
        public bool Status { get; set; }       
        public List<ShipmentTracking> Tracking { get; set; }
    }
}
