using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frayte.Api.Models
{
    public class FrayteShipmentResponseDto
    {
        public string ShipmentBookingId { get; set; }
        public  bool Status { get; set; }
        public string Description { get; set; }
        public string TrackingNumber { get; set; }
        public string Mode { get; set; }
        public DateTime? CreatedOn { get; set; }      
        public string Currency { get; set; }
        public PaymentPartyDto PaymentParty { get; set; }
        public FromAddressDto FromAddress { get; set; }
        public ToAddressDto ToAddress { get; set; }
        public RateDto Rates { get; set; }
        public ApiCustomInformation CustomInfo { get; set; }
        public PackageDetailDto PackageDetails { get; set; }
    }

    public class PaymentPartyDto
    {
        public string PartyType { get; set; }
        public string AccountNo { get; set; }
    }

    public class FromAddressDto
    {
        public string CompanyName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public ShipAddressDto Address { get; set; }
    }

    public class ToAddressDto
    {
        public string CompanyName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public ShipAddressDto Address { get; set; }
    }

    public class ShipAddressDto
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Area { get; set; }
        public string Postcode { get; set; }
        public string CountryCode { get; set; }
    }

    public class RateDto
    {
        public string RateId { get; set; }
        public string RateType { get; set; }
        public Decimal TotalCost { get; set; }
        public string RateCurrencyCode { get; set; }
        public string CourierName { get; set; }
    }   

    public class PackageDetailDto
    {
        public string PackageTrackingNumber { get; set; }
        public List<string> LabelUrl { get; set; }
    }

    public class ShippingLabel
    {   
        public string ImageBase64 { get; set; }
    }
}