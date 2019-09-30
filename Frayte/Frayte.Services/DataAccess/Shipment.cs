//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Frayte.Services.DataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class Shipment
    {
        public int ShipmentId { get; set; }
        public int ShipmentStatusId { get; set; }
        public string CargoWiseSo { get; set; }
        public string BarCodeSo { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public int ShipperId { get; set; }
        public int ShipperAddressId { get; set; }
        public int ReceiverId { get; set; }
        public int ReceiverAddressId { get; set; }
        public string ShipmentTermCode { get; set; }
        public Nullable<int> PackagingTypeId { get; set; }
        public Nullable<int> SpecialDeliveryId { get; set; }
        public string ContentDescription { get; set; }
        public string Guidelines { get; set; }
        public string ShipmentDuitable { get; set; }
        public string ShippingReference { get; set; }
        public System.DateTime ShippingDate { get; set; }
        public System.TimeSpan ShippingTime { get; set; }
        public string PaymentParty { get; set; }
        public Nullable<decimal> DeclaredValue { get; set; }
        public string DeclaredCurrency { get; set; }
        public int DeliveredBy { get; set; }
        public int ShipmentPickupAddressId { get; set; }
        public string ShipmentPickupContactName { get; set; }
        public string ShipmentPickupContactPhoneNumber { get; set; }
        public Nullable<int> TransportToWarehouseId { get; set; }
        public Nullable<int> WarehouseId { get; set; }
        public int TradeLaneId { get; set; }
        public string LocationType { get; set; }
        public string LocationOfShipment { get; set; }
        public string SpecialInstruction { get; set; }
        public Nullable<System.DateTime> PickupDate { get; set; }
        public Nullable<System.TimeSpan> ShipmentReadyBy { get; set; }
        public int TimezoneId { get; set; }
        public int TermAndConditionId { get; set; }
        public Nullable<System.Guid> CustomerConfirmCode { get; set; }
        public string CommercialInvoice { get; set; }
        public string PackingList { get; set; }
        public string CustomDocument { get; set; }
        public string ControlNumber { get; set; }
        public string AirWayBill { get; set; }
        public string AirWayBillDocument { get; set; }
        public Nullable<System.DateTime> AnticipatedPickupDate { get; set; }
        public Nullable<System.TimeSpan> AnticipatedPickupTime { get; set; }
        public string AWB { get; set; }
        public string OtherDocs { get; set; }
        public Nullable<int> OriginatingAgentId { get; set; }
        public Nullable<int> OriginatingAddressId { get; set; }
        public Nullable<System.DateTime> OriginatingPlannedDepartureDate { get; set; }
        public Nullable<System.TimeSpan> OriginatingPlannedDepartureTime { get; set; }
        public Nullable<System.DateTime> WarehouseDropOffDate { get; set; }
        public Nullable<System.TimeSpan> WarehouseDropOffTime { get; set; }
        public string FlightVessel { get; set; }
        public Nullable<System.DateTime> ETDDate { get; set; }
        public Nullable<System.TimeSpan> ETDTime { get; set; }
        public Nullable<System.DateTime> ETADate { get; set; }
        public Nullable<System.TimeSpan> ETATime { get; set; }
        public string MABBL { get; set; }
        public Nullable<int> DestinatingAgentId { get; set; }
        public Nullable<System.DateTime> DestinatingDeliveryDate { get; set; }
        public Nullable<System.TimeSpan> DestinatingDeliveryTime { get; set; }
        public string DestinatingDeliveryCustomIssue { get; set; }
        public Nullable<System.DateTime> FinalDeliveryDate { get; set; }
        public Nullable<System.TimeSpan> FinalDeliveryTime { get; set; }
        public string Signature { get; set; }
        public string FinalImage { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public string TrackingNumber { get; set; }
        public string VehicleRegNo { get; set; }
        public string PaymentPartyAccountNo { get; set; }
        public Nullable<System.DateTime> OriginatingPlannedArrivalDate { get; set; }
        public Nullable<System.TimeSpan> OriginatingPlannedArrivalTime { get; set; }
        public string SeaPOL { get; set; }
        public string SeaPOD { get; set; }
        public string SeaTelexDocument { get; set; }
        public string TelexReleaseBy { get; set; }
        public string PiecesCaculatonType { get; set; }
        public string PaymentPartyTaxDuties { get; set; }
        public string PaymentPartyTaxDutiesAccountNo { get; set; }
        public string TransportToWareHouseCarrier { get; set; }
        public string FrayteAWB { get; set; }
        public Nullable<int> PortOfDepartureId { get; set; }
        public Nullable<int> PortOfArrivalId { get; set; }
        public string UnexpectedClearance { get; set; }
        public string ExceptionNote { get; set; }
    }
}
