using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.ParcelHub
{
    public class ParcelHubResponse
    {
        public string Account { get; set; }
        public string UserAgent { get; set; }
        public FratyteError Error { get; set; }
        public ServiceInfo ServiceInfo { get; set; }
        public long ParcelhubShipmentId { get; set; }
        public ShippingInfo ShippingInfo { get; set; }
        public CollectionDetails CollectionDetails { get; set; }
        public CollectionAddress CollectionAddress { get; set; }
        public DeliveryAddress DeliveryAddress { get; set; }
        public string Reference1 { get; set; }
        public string Reference2 { get; set; }
        public string SpecialInstructions { get; set; }
        public string ContentsDescription { get; set; }
        public List<ParcelResponse> Parcel { get; set; }
        public DateTime ModifiedTime { get; set; }
        public CustomsDeclarationInfo CustomsDeclarationInfo { get; set; }
        public bool Deleted { get; set; }
        public bool HasBeenManifested { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
    }

    public struct ServiceInfo
    {
        public string ServiceId { get; set; }
        public string ServiceCustomerUID { get; set; }
        public string ServiceProviderId { get; set; }
    }

    public class ShippingInfo
    {
        public string CourierTrackingNumber { get; set; }
        public string ParcelhubTrackingNumber { get; set; }
        public string TrackingURL { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatingClientSoftware { get; set; }
        public string ServiceName { get; set; }
        public string ServiceDesc { get; set; }
        public string ServiceProviderName { get; set; }
    }

    public class CollectionDetails
    {
        public DateTime CollectionDate { get; set; }
        public TimeSpan CollectionReadyTime { get; set; }
        public string CollectionReadyTimeString { get; set; }
        public TimeSpan LocationCloseTime { get; set; }
        public string LocationCloseTimeString { get; set; }
        public string PackageLocation { get; set; }
        public string PickupSpecialInstructions { get; set; }
    }

    public class CollectionAddress
    {
        public string key { get; set; }
        public string ContactName { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string Area { get; set; }
        public string Postcode { get; set; }
        public string Country { get; set; }
        public string AddressType { get; set; }
        public string RestrictedAccessDescription { get; set; }
        public string RestrictedVehicleDescription { get; set; }
    }

    public class DeliveryAddress
    {
        public string key { get; set; }
        public string ContactName { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string Area { get; set; }
        public string Postcode { get; set; }
        public string Country { get; set; }
        public string AddressType { get; set; }
        public string RestrictedAccessDescription { get; set; }
        public string RestrictedVehicleDescription { get; set; }
    }

    public class Packages
    {
        public string key { get; set; }
        public PackageShippingInfo PackageShippingInfo { get; set; }
        public string PackageType { get; set; }
        public Dimension Dimension { get; set; }
        public decimal Weight { get; set; }
        public Value Value { get; set; }
        public string Contents { get; set; }
        public PackageCustomsDeclarationDto PackageCustomsDeclaration { get; set; }
    }

    public class PackageShippingInfo
    {
        public string key { get; set; }
        public List<Labels> Labels { get; set; }
        public string CourierTrackingNumber { get; set; }
    }

    public class Labels
    {
        public string key { get; set; }
        public string LabelData { get; set; }
    }

    public class Dimension
    {
        public int Length { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class Value
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }

    public class PackageCustomsDeclarationDto
    {
        public string key { get; set; }
        public string ContentsDescription { get; set; }
        public string Quantity { get; set; }
        public decimal Weight { get; set; }
        public Value Value { get; set; }
        public string HSTariffNumber { get; set; }
        public string CountryOfOrigin { get; set; }
    }

    public class CustomsDeclarationInfo
    {
        public string key { get; set; }
        public string TermsOfTrade { get; set; }
        public string SendersCustomsReference { get; set; }
        public string ImportersReference { get; set; }
        public string ImportersContactDetails { get; set; }
        public PostalCharges PostalCharges { get; set; }
        public string CategoryOfItem { get; set; }
        public string CategoryOfItemExplanation { get; set; }
        public string Comments { get; set; }
        public string LicenseNumber { get; set; }
        public string CertificateNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public string OfficeOfOrigin { get; set; }
    }

    public class PostalCharges
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }
}