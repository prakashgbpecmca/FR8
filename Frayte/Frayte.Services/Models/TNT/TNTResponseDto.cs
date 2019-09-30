using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.TNT
{
    public class TNTResponseDto
    {
        public TNTShipmentResponseDto TNTShipmentResponse { get; set; }
        public FratyteError Error { get; set; }
    }
    public class TNTShipmentResponseDto
    {
        public string TrackingCode { get; set; }
        public string ConnoteReply { get; set; }
        public string shipmentXML { get; set; }
        public string ResultReply { get; set; }
        public string XmlIn { get; set; }
        public string BookingRefNo { get; set; }
    }


    public class TNTShipmentDetail
    {
        public int DraftShipmentId { get; set; }
        public TNTAddress ShipFrom { get; set; }
        public TNTAddress ShipTo { get; set; }
        public TNTParcelType ParcelType { get; set; }
        public DateTime CreatedOn { get; set; }
        public string PayTaxAndDuties { get; set; }
        public string PaymentPartyAccountNumber { get; set; }
        public TNTCurrency Currency { get; set; }
        public List<TNTPackage> Packages { get; set; }
        public TNTReferenceDetail ReferenceDetail { get; set; }
        public TNTCustomInformation CustomInfo { get; set; }
        public TNTServicerService TNTService { get; set; }
        public string PakageCalculatonType { get; set; }
        public string TaxAndDutiesAcceptedBy { get; set; }        
        public string BookingStatusType { get; set; }
        public string FrayteNumber { get; set; }
        public string ModuleType { get; set; }
        public string BookingApp { get; set; }
        public string TotalWeight { get; set; }
    }

    public class TNTMode
    {
        public const string Test = "test";
        public const string Live = "production";
    }

    public class TNTAddressType
    {
        public const string ShipFrom = "ShipFrom";
        public const string ShipTo = "ShipTo";
    }

    public class TNTParcelType
    {
        public string ParcelType { get; set; }
        public string ParcelDescription { get; set; }
    }

    public class TNTCurrency
    {
        public string CurrencyCode { get; set; }
        public string CurrencyDescription { get; set; }
    }

    public class TNTCustomInformation
    {
        public string ContentsType { get; set; }
        public string ContentsExplanation { get; set; }
        public string RestrictionType { get; set; }
        public string RestrictionComments { get; set; }
        public bool? CustomsCertify { get; set; }
        public string EelPfc { get; set; }
        public string CustomsSigner { get; set; }
        public string NonDeliveryOption { get; set; }
    }

    public class TNTReferenceDetail
    {
        public string Reference1 { get; set; }
        public string SpecialInstruction { get; set; }
        public string ContentDescription { get; set; }
        public DateTime? CollectionDate { get; set; }
        public string CollectionTime { get; set; }
    }

    public class TNTServicerService
    {
        public string Courier { get; set; }
        public string CourierDisplay { get; set; }
        public string CourierAccountNumber { get; set; }
        public string CourierAccountId { get; set; }
        public string RateType { get; set; }
        public string LogisticType { get; set; }
    }

    public class TNTAddress
    {
        public TNTCountry Country { get; set; }
        public string PostCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Area { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }

    public class TNTCountry
    {
        public int CountryId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Code2 { get; set; }
    }

    public class TNTPackage
    {
        public int CartoonValue { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public decimal Value { get; set; }
        public string Content { get; set; }
    }
}
