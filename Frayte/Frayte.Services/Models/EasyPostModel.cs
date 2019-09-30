using Frayte.Services.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public class EasyPostShipmentDetail
    {
        public int DraftShipmentId { get; set; }
        public EasyPostAddress ShipFrom { get; set; }
        public EasyPostAddress ShipTo { get; set; }
        public EasyPostParcelType ParcelType { get; set; }
        public DateTime CreatedOn { get; set; }
        public string PayTaxAndDuties { get; set; }
        public string PaymentPartyAccountNumber { get; set; }
        public EasyPostCurrency Currency { get; set; }
        public List<EasyPostPackage> Packages { get; set; }
        public EasyPostReferenceDetail ReferenceDetail { get; set; }
        public EasyPostCustomInformation CustomInfo { get; set; }
        public EasyPostServicerService EasyPostService { get; set; }
        public string PakageCalculatonType { get; set; }
        public string TaxAndDutiesAcceptedBy { get; set; }
        public string BookingStatusType { get; set; }
        public string FrayteNumber { get; set; }
        public string ModuleType { get; set; }
        public string BookingApp { get; set; }
    }

    public class EasyPostMode
    {
        public const string Test = "test";
        public const string Live = "production";
    }

    public class EasyPostAddressType
    {
        public const string ShipFrom = "ShipFrom";
        public const string ShipTo = "ShipTo";
    }

    public class EasyPostParcelType
    {
        public string ParcelType { get; set; }
        public string ParcelDescription { get; set; }
    }

    public class EasyPostCurrency
    {
        public string CurrencyCode { get; set; }
        public string CurrencyDescription { get; set; }
    }

    public class EasyPostCustomInformation
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

    public class EasyPostReferenceDetail
    {
        public string Reference1 { get; set; }
        public string SpecialInstruction { get; set; }
        public string ContentDescription { get; set; }
        public DateTime? CollectionDate { get; set; }
        public string CollectionTime { get; set; }
    }

    public class EasyPostServicerService
    {
        public string Courier { get; set; }
        public string CourierDisplay { get; set; }
        public string CourierAccountNumber { get; set; }
        public string CourierAccountId { get; set; }
        public string RateType { get; set; }
    }

    public class EasyPostResult
    {
        public FratyteError Errors { get; set; }
        public EasyPost.Order Order { get; set; }
    }

    public class EasyPostOrder
    {
        public string OrderId { get; set; }
        public string TrackingNumber { get; set; }
    }

    public class EasyPostAddress
    {
        public EasyPostCountry Country { get; set; }
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

    public class EasyPostCountry
    {
        public int CountryId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Code2 { get; set; }
    }

    public class EasyPostPackage
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