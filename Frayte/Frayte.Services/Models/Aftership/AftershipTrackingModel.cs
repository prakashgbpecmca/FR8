using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.Aftership
{

    #region Webhook models

    public class CustomFields
    {
        public string customer_name { get; set; }
        public string customer_email { get; set; }
        public string user_customer_email { get; set; }
        public string module_type { get; set; }
    }

    public class Checkpoint
    {
        public string location { get; set; }
        public object country_name { get; set; }
        public object country_iso3 { get; set; }
        public object state { get; set; }
        public object city { get; set; }
        public object zip { get; set; }
        public string message { get; set; }
        public List<object> coordinates { get; set; }
        public string tag { get; set; }
        public string subtag { get; set; }
        public string subtag_message { get; set; }
        public DateTime created_at { get; set; }
        public DateTime checkpoint_time { get; set; }
        public string slug { get; set; }
    }

    public class Msg
    {
        public string id { get; set; }
        public string tracking_number { get; set; }
        public string title { get; set; }
        public object note { get; set; }
        public object origin_country_iso3 { get; set; }
        public object destination_country_iso3 { get; set; }
        public string courier_destination_country_iso3 { get; set; }
        public int shipment_package_count { get; set; }
        public bool active { get; set; }
        public object order_id { get; set; }
        public object order_id_path { get; set; }
        public object customer_name { get; set; }
        public string source { get; set; }
        public List<object> emails { get; set; }
        public List<object> smses { get; set; }
        public List<object> subscribed_smses { get; set; }
        public List<object> subscribed_emails { get; set; }
        public List<object> android { get; set; }
        public List<object> ios { get; set; }
        public bool return_to_sender { get; set; }
        public CustomFields custom_fields { get; set; }
        public string tag { get; set; }
        public string subtag { get; set; }
        public string subtag_message { get; set; }
        public int tracked_count { get; set; }
        public object expected_delivery { get; set; }
        public string signed_by { get; set; }
        public object shipment_type { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string slug { get; set; }
        public string unique_token { get; set; }
        public string path { get; set; }
        public double shipment_weight { get; set; }
        public string shipment_weight_unit { get; set; }
        public string content_type { get; set; }
        public int delivery_time { get; set; }
        public object last_mile_tracking_supported { get; set; }
        public object language { get; set; }
        public DateTime shipment_pickup_date { get; set; }
        public object shipment_delivery_date { get; set; }
        public DateTime last_updated_at { get; set; }
        public List<Checkpoint> checkpoints { get; set; }
        public object tracking_account_number { get; set; }
        public object tracking_destination_country { get; set; }
        public object tracking_key { get; set; }
        public string tracking_postal_code { get; set; }
        public object tracking_ship_date { get; set; }
        public object tracking_state { get; set; }



    }

    public class AftershipWebhookObject
    {
        public string @event { get; set; }
        public Msg msg { get; set; }
        public int ts { get; set; }
    }

    #endregion

    public class FrayteAfterShipTracking
    {
        public string Slug { get; set; }
        public string TrackingNumber { get; set; }
        public string Title { get; set; }
        public List<string> Smses { get; set; }
        public List<string> Emails { get; set; }
        public string OrderID { get; set; }
        public string orderIDPath { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string UserCustomerEmail { get; set; }
        public string ModuleType { get; set; }

    }

    public class ShipmentTrackingAditional
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string OriginCountry { get; set; }
        public string DestinationCountry { get; set; }
        public bool Active { get; set; }
        public string Source { get; set; }
        public int InTransitDays { get; set; }
        public string ShipmentType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    public class TrackAndTraceDashboard
    {
        public int TotalShipments { get; set; }

    }

    public class TrackAfterShipTracking
    {
        public int CustomerId { get; set; }
        public int StatusId { get; set; }
        public DateTime createdAtMax { get; set; }
        public DateTime createdAtMin { get; set; }
        public string Keyword { get; set; }
        public string Lang { get; set; }
        public int Limit { get; set; }
        public int Page { get; set; }
        public int Total { get; set; }
    }
    public enum FrayteAftershipStatusTag
    {
        Pending = 0,
        InfoReceived = 1,
        InTransit = 2,
        OutForDelivery = 3,
        AttemptFail = 4,
        Delivered = 5,
        Exception = 6,
        Expired = 7
    }
    public static class FrayteAftershipStatusTagString
    {
        public const string Pending = "Pending";
        public const string InfoReceived = "InfoReceived";
        public const string InTransit = "InTransit";
        public const string OutForDelivery = "OutForDelivery";
        public const string AttemptFail = "AttemptFail";
        public const string Delivered = "Delivered";
        public const string Exception = "Exception";
        public const string Expired = "Expired";
    }
    public static class FrayteAftershipStatus
    {
        public const string Pending = "Pending";
        public const string InfoReceived = "Info Received";
        public const string InTransit = "In Transit";
        public const string OutForDelivery = "Out For Delivery";
        public const string AttemptFail = "Failed Attempt";
        public const string Delivered = "Delivered";
        public const string Exception = "Exception";
        public const string Expired = "Expired";
    }
    public static class FrayteCourierSlugs
    {
        public const string DHL = "dhl";
        public const string DHLExpress = "";
        public const string DHLEconomy = "";
        public const string TNT = "tnt";
        public const string TNTExpress = "";
        public const string TNTEconomy = "";
        public const string UPS = "ups";
        public const string Yodel = "yodel";
        public const string Hermese = "myhermes-uk";
        public const string UkMail = "uk-mail";
        public const string DPD = "dpd-uk";
    }
    public static class FrayteCourierSlugsDisplay
    {
        public const string DHL = "DHL";
        public const string DHLExpress = "DHL Express";
        public const string DHLEconomy = "DHL Economy";
        public const string TNT = "TNT";
        public const string TNTExpress = "TNT Express";
        public const string TNTEconomy = "TNT Economy";
        public const string UPS = "UPS";
        public const string Yodel = "Yodel";
        public const string Hermese = "Hermes";
        public const string UkMail = "UK Mail";
    }
    public class FrayteAftershipmentTrackingEmail : AftershipWebhookObject
    {
        public string TrackingLink { get; set; }
    }
}
