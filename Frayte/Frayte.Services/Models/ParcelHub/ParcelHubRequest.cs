using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.ParcelHub
{
    public class ParcelHubShipmentRequest
    {
        public ParcelHubSecurity Security { get; set; }
        public ParcelHubShippingAddress CollectionAddress { get; set; }
        public ParcelHubShippingAddress DeliveryAddress { get; set; }
        public List<ParcelHubPackageDetail> Package { get; set; }
        public ParcelHubShipmentCustomeInfo CustomInfo { get; set; }
        public string ContentsDescription { get; set; }
        public string Reference1 { get; set; }
        public string Reference2 { get; set; }
        public int DraftShipmentId { get; set; }
        public string PackageCalculationType { get; set; }
        public string CourierAccountCountryCode { get; set; }
        public string CourierAccountNo { get; set; }
        public string CourierName { get; set; }
        public string CourierDescription { get; set; }
        public int CustomerId { get; set; }
    }

    public class CollectionDetail
    {
        public DateTime CollectionDate { get; set; }
        public string CollectionReadyTime { get; set; }
        public string LocationCloseTime { get; set; }
    }

    public class ParcelHubSecurity
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string UserAgent { get; set; }
        public string ServiceUrl { get; set; }
    }

    public class ParcelHubShippingAddress
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Area { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public string Country { get; set; }
    }

    public class ParcelHubPackageDetail
    {
        public string Contents { get; set; }
        public ParcelHubDimensions ParcelDimensions { get; set; }
        public ParcelHubPackageCustomsDeclaration PackageCustomsDeclaration { get; set; }
        public ParcelHubAmountOfMoney Value { get; set; }
        public decimal? Weight { get; set; }
    }

    public class ParcelHubDimensions
    {
        public int Height { get; set; }
        public int Length { get; set; }
        public int Width { get; set; }
        public int Cartonvalue { get; set; }
        public decimal Weight { get; set; }
        public ParcelHubAmountOfMoney Value { get; set; }
        public string Contents { get; set; }
    }

    public class ParcelHubPackageCustomsDeclaration
    {
        public string ContentsDescription { get; set; }
        public string CountryOfOrigin { get; set; }
        public string HSTariffNumber { get; set; }
        public string Quantity { get; set; }
        public ParcelHubAmountOfMoney Value { get; set; }
        public string Weight { get; set; }
    }

    public class ParcelHubShipmentCustomeInfo
    {
        public string CategoryOfItem { get; set; }
        public string SendersCustomsReference { get; set; }
        public string TermsOfTrade { get; set; }
        public string CategoryOfItemExplanation { get; set; }
        public string CertificateNumber { get; set; }
        public string Comments { get; set; }
        public string ImportersContactDetails { get; set; }
        public string ImportersReference { get; set; }
        public string InvoiceNumber { get; set; }
        public string LicenseNumber { get; set; }
        public string OfficeOfOrigin { get; set; }
        public ParcelHubAmountOfMoney PostalCharges { get; set; }
    }

    public class ParcelHubAmountOfMoney
    {
        public decimal Amount { get; set; }
        public string ParcelCurrency { get; set; }
    }
}