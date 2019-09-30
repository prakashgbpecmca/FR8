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
    
    public partial class DirectShipmentDraft
    {
        public int DirectShipmentDraftId { get; set; }
        public Nullable<int> OpearionZoneId { get; set; }
        public Nullable<int> ShipmentStatusId { get; set; }
        public int CustomerId { get; set; }
        public Nullable<int> FromAddressId { get; set; }
        public Nullable<int> ToAddressId { get; set; }
        public string PaymentPartyTaxAndDuties { get; set; }
        public string PaymentPartyTaxAndDutiesAccountNo { get; set; }
        public string Reference1 { get; set; }
        public string SpecialInstruction { get; set; }
        public string ContentDescription { get; set; }
        public Nullable<System.TimeSpan> CollectionTime { get; set; }
        public Nullable<System.DateTime> CollectionDate { get; set; }
        public Nullable<System.DateTime> LastUpdated { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<int> ShipmentTypeId { get; set; }
        public Nullable<decimal> BaseRate { get; set; }
        public Nullable<decimal> Margin { get; set; }
        public Nullable<decimal> FuelSurCharge { get; set; }
        public Nullable<decimal> AdditionalSurcharge { get; set; }
        public Nullable<decimal> FuelSurchargePercent { get; set; }
        public Nullable<System.DateTime> FuelMonthYear { get; set; }
        public string ParcelType { get; set; }
        public Nullable<bool> IsPODMailSent { get; set; }
        public Nullable<int> CourierAccountId { get; set; }
        public string LogisticType { get; set; }
        public string LogisticServiceType { get; set; }
        public string CurrencyCode { get; set; }
        public string PackageCaculatonType { get; set; }
        public string TaxAndDutiesAcceptedBy { get; set; }
        public string FrayteNumber { get; set; }
        public Nullable<int> ManifestId { get; set; }
        public string ModuleType { get; set; }
        public Nullable<System.DateTime> MappedOn { get; set; }
        public Nullable<System.DateTime> EstimatedDateofArrival { get; set; }
        public Nullable<System.TimeSpan> EstimatedTimeofArrival { get; set; }
        public Nullable<System.DateTime> EstimatedDateofDelivery { get; set; }
        public Nullable<System.TimeSpan> EstimatedTimeofDelivery { get; set; }
        public string AddressType { get; set; }
        public string BookingApp { get; set; }
        public string EasyPostOrderObject { get; set; }
        public string EasyPostPickUpObject { get; set; }
        public string EasyPostErrorObject { get; set; }
        public string TrackingDetail { get; set; }
        public Nullable<bool> IsSelectServiceStatus { get; set; }
        public string ServiceCode { get; set; }
        public string PickUpRef { get; set; }
        public string ShipmentImage { get; set; }
        public bool IsPublic { get; set; }
        public Nullable<int> SessionId { get; set; }
        public Nullable<decimal> ChargeableWeight { get; set; }
    }
}
