using Frayte.Services.DataAccess;
using Frayte.Services.Models.ApiModal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{

    public class FrayteOfflineTracking
    {
        public int ShipmentId { get; set; }
        public string Message { get; set; }
        public string Location { get; set; }
        public DateTime CreatedOn { get; set; }
    }
    public class FrayteLogicalPhysicalPath
    {
        public string LogicalPath { get; set; }
        public string PhysicalPath { get; set; }
    }

    public class IntegrtaionResult
    {
        public string CourierName { get; set; }
        public string TrackingNumber { get; set; }
        public string ShipmentImage { get; set; }
        public string PickupRef { get; set; }
        public List<CourierPieceDetail> PieceTrackingDetails { get; set; }
        public FratyteError Error { get; set; }
        public bool Status { get; set; }
        public string IntegrationReponse { get; set; }
        public List<FrayteApiError> ErrorCode { get; set; }
    }

    public class CourierPieceDetail
    {
        public int DirectShipmentDetailId { get; set; }
        public string PieceTrackingNumber { get; set; }
        public string ImageByte { get; set; }
        public string ImageUrl { get; set; }
        public string LabelName { get; set; }
    }

    public class FrayteParcelType
    {
        public string ParcelType { get; set; }
        public string ParcelDescription { get; set; }
    }

    public class DirectBookingCustomer
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CompanyName { get; set; }
        public string AccountNumber { get; set; }
        public string EmailId { get; set; }
        public int OperationZoneId { get; set; }
        public int ValidDays { get; set; }
        public string CustomerCurrency { get; set; }
        public bool IsShipperTaxAndDuty { get; set; }
    }

    public class CustomerLogisticService
    {
        public int UserId { get; set; }
        public string LogisticCompany { get; set; }
        public string LogisticServiceType { get; set; }
    }

    public class FrayteTrackDirectBooking
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int ShipmentStatusId { get; set; }
        [StringLength(10)]
        public string FrayteNumber { get; set; }
        public string BookingMethod { get; set; }
        public string CallingFrom { get; set; }
        public string eCommerceShipmentType { get; set; }
        [StringLength(50)]
        public string TrackingNo { get; set; }
        public string LogisticType { get; set; }
        public int CurrentPage { get; set; }
        public int TakeRows { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string LogisticServiceType { get; set; }
        public int UserId { get; set; }
    }

    public class FrayteUserDirectShipment
    {
        public int ShipmentId { get; set; }
        public string ShipmentCode { get; set; }
        public string Customer { get; set; }
        public string ShippedFromCompany { get; set; }
        public string ShippedToCompany { get; set; }
        public string ShipmentDetail { get; set; }
        public string ShippingBy { get; set; }
        public string DisplayName { get; set; }
        public string RateType { get; set; }
        public string RateTypeDisplay { get; set; }
        public DateTime ShippingDate { get; set; }
        public DateTime? DateOfDelivery { get; set; }
        public string Status { get; set; }
        public string DisplayStatus { get; set; }
        public string LogisticType { get; set; }
        public string LogisticTypeDisplay { get; set; }
        public FrayteUserRole UserRoleId { get; set; }
        public string Reference1 { get; set; }
        public string FrayteNumber { get; set; }
        public string TrackingNo { get; set; }
        public int TotalRows { get; set; }
        public int ManifestId { get; set; }
        public string ManifestName { get; set; }
        public string BookingApp { get; set; }
        public bool IsTrackingShow { get; set; }
        public bool IsSuccessFull { get; set; }
        public bool IsEasyPostError { get; set; }
        public bool? IsSelectServiceStatus { get; set; }
        public int SessionId { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public string ServiceCode { get; set; }
        public int TotalPieces { get; set; }
        public decimal TotalWeight { get; set; }
        public string FromCountry { get; set; }
        public string ToCountry { get; set; }
        public string MAWB { get; set; }
    }

    public class MainfestDetailModel
    {
        public List<FrayteCustomer> Customers { get; set; }
        public List<FrayteUserDirectShipment> Shipments { get; set; }
    }

    public class DirectBookingService
    {
        public int ZoneRateCardId { get; set; }
        public int CustomerId { get; set; }
        public string WeightType { get; set; }
        public int CourierId { get; set; }
        public int LogisticServiceId { get; set; }
        public string LogisticShipmentCode { get; set; }
        public decimal Rate { get; set; }
        public decimal BaseRate { get; set; }
        public decimal Margin { get; set; }
        public decimal MarginPercent { get; set; }
        public string CourierName { get; set; }
        public string DisplayName { get; set; }
        public int CourierAccountId { get; set; }
        public string CourierAccountNo { get; set; }
        public string CourierDescription { get; set; }
        public string CourierAccountCountryCode { get; set; }
        public string IntegrationAccountId { get; set; }
        public string PakageType { get; set; }
        public string ParcelServiceType { get; set; }
        public string LogisticType { get; set; }
        public int LogisticShipmentId { get; set; }
        public string LogisticDescription { get; set; }
        public string LogisticServiceType { get; set; }
        public string UnitOfMeasurement { get; set; }
        public float FuelSurcharge { get; set; }
        public float FuelSurchargePercent { get; set; }
        public float AdditionalSurcharge { get; set; }
        public float TotalEstimatedCharge { get; set; }
        public string CurrencyCode { get; set; }
        public string CustomerCurrency { get; set; }
        public string FuelMonth { get; set; }
        public DateTime? FuelDate { get; set; }
        public string RateType { get; set; }
        public string RateTypeDisplay { get; set; }
        public string PackageCalculationType { get; set; }
        public decimal Weight { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public string TransitTime { get; set; }
        public bool IsWeightShow { get; set; }
        public bool IsFuelSurchargeCalculate { get; set; }
        public string NetworkCode { get; set; }
        public string CarrierLogo { get; set; }
        public List<DirectBookingOptionalServices> OptionalServices { get; set; }
    }

    public class DirectBookingOptionalServices
    {
        public int LogisticOptionalServiceId { get; set; }
        public string LogisticCompany { get; set; }
        public string ServiceCode { get; set; }
        public string ServiceDescription { get; set; }
        public bool IsEnable { get; set; }
    }

    public class FrayteUserDefaultAddresses
    {
        public DirectBookingCollection ShipFrom { get; set; }
        public DirectBookingCollection ShipTo { get; set; }
    }

    public class DirectBookingCollection
    {
        public int DirectShipmentAddressId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CurrencyCode { get; set; }
        public FrayteCountryCode Country { get; set; }
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
        public bool IsMailSend { get; set; }
        public string AddressType { get; set; }
        public bool IsShipperTaxAndDuty { get; set; }
        public bool? IsRateShow { get; set; }
        public bool IsDefaultAddress { get; set; }
    }

    public class ReferenceDetail
    {
        public string Reference1 { get; set; }
        public string Reference2 { get; set; }
        public string SpecialInstruction { get; set; }
        public string ContentDescription { get; set; }
        public DateTime? CollectionDate { get; set; }
        public string CollectionTime { get; set; }
        public bool IsCollection { get; set; }
    }

    public class Package
    {
        public int DirectShipmentDetailId { get; set; }
        public int PackageTrackingDetailId { get; set; }
        public int CartoonValue { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public decimal Value { get; set; }
        public string Content { get; set; }
        public string LabelName { get; set; }
        public string Label { get; set; }
        public string FrayteLabelName { get; set; }
        public string TrackingNo { get; set; }
        public bool? IsPrinted { get; set; }
    }

    public class DirectBookingShipmentDetail
    {
        public int OpearionZoneId { get; set; }
        public int DirectShipmentId { get; set; }
        public DirectBookingCollection ShipFrom { get; set; }
        public DirectBookingCollection ShipTo { get; set; }
        public FrayteParcelType ParcelType { get; set; }
        public string PayTaxAndDuties { get; set; }
        public string PaymentPartyAccountNumber { get; set; }
        public CurrencyType Currency { get; set; }
        public List<Package> Packages { get; set; }
        public ReferenceDetail ReferenceDetail { get; set; }
        public CustomInformation CustomInfo { get; set; }
        public DirectBookingService CustomerRateCard { get; set; }
        public int CustomerId { get; set; }
        public int ShipmentStatusId { get; set; }
        public string LogisticCompany { get; set; }
        public string PakageCalculatonType { get; set; }
        public FratyteError Error { get; set; }
        public decimal BaseRate { get; set; }
        public decimal MarginCost { get; set; }
        public DateTime FuelMonth { get; set; }
        public float FuelPercent { get; set; }
        public float FuelSurCharge { get; set; }
        public float AdditionalSurcharge { get; set; }
        public decimal EstimatedWeight { get; set; }
        public float EstimatedCost { get; set; }
        public float EstimatedTotalCost { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public TimeSpan? DeliveryTime { get; set; }
        public string SignedBy { get; set; }
        public string TaxAndDutiesAcceptedBy { get; set; }
        public string FrayteNumber { get; set; }
        public DateTime CreatedOn { get; set; }
        public TimeZoneModal TimeZone { get; set; }
        public string CreatedBy { get; set; }
        public int CreatedByUserId { get; set; }
        public int CreatedByRoleId { get; set; }
        public string PickUpRef { get; set; }
        public string ShipmentStatus { get; set; }
        public string TrackingNo { get; set; }
        public bool IsRateShow { get; set; }
    }

    public class DirectBookingFindService
    {
        public FrayteCountryCode FromCountry { get; set; }
        public string FromPostCode { get; set; }
        public FrayteCountryCode ToCountry { get; set; }
        public string ToPostCode { get; set; }
        public decimal Weight { get; set; }
        public int OperationZoneId { get; set; }
        public string PackageType { get; set; }
        public string DocType { get; set; }
        public int CustomerId { get; set; }
        public DateTime? Date { get; set; }
        public string Currency { get; set; }
        public string AddressType { get; set; }
        public string CallingFrom { get; set; }
        public string PackageCalculationType { get; set; }
        public List<PackageDraft> Packages { get; set; }
    }

    public class FraytePackageTrackingDetail
    {
        public int PackageTrackingDetailId { get; set; }
        public int DirectShipmentDetailId { get; set; }
        public string LabelUrl { get; set; }
        public string TrackingNo { get; set; }
        public string PackageImage { get; set; }
        public bool IsDownloaded { get; set; }
        public bool IsPrinted { get; set; }
    }

    public class DirectBookingShipmentDraftDetail
    {
        public int DirectShipmentDraftId { get; set; }
        public int OpearionZoneId { get; set; }
        public DirectBookingDraftCollection ShipFrom { get; set; }
        public DirectBookingDraftCollection ShipTo { get; set; }
        public FrayteParcelType ParcelType { get; set; }
        public string PayTaxAndDuties { get; set; }
        public string PaymentPartyAccountNumber { get; set; }
        public CurrencyType Currency { get; set; }
        public List<PackageDraft> Packages { get; set; }
        public ReferenceDetail ReferenceDetail { get; set; }
        public CustomInformation CustomInfo { get; set; }
        public DirectBookingService CustomerRateCard { get; set; }
        public int CustomerId { get; set; }
        public int CreatedBy { get; set; }
        public int ShipmentStatusId { get; set; }
        public int ShippingMethodId { get; set; }
        public string PakageCalculatonType { get; set; }
        public FratyteError Error { get; set; }
        public decimal BaseRate { get; set; }
        public decimal MarginCost { get; set; }
        public DateTime FuelMonth { get; set; }
        public float FuelPercent { get; set; }
        public float AdditionalSurcharge { get; set; }
        public float FuelSurCharge { get; set; }
        public float EstimatedCost { get; set; }
        public float EstimatedTotalCost { get; set; }
        public string TaxAndDutiesAcceptedBy { get; set; }
        public string BookingStatusType { get; set; }
        public string FrayteNumber { get; set; }
        public string AddressType { get; set; }
        public bool IsPublic { get; set; }
        public int RoleId { get; set; }
        public int LogInUseId { get; set; }
        public int SessionId { get; set; }
    }


    public class DirectBookingShipmentDraftDetail12
    {
        public int DirectShipmentDraftId { get; set; }
        public int OpearionZoneId { get; set; }
        public DirectBookingDraftCollection ShipFrom { get; set; }
        public DirectBookingDraftCollection ShipTo { get; set; }
        public FrayteParcelType ParcelType { get; set; }
        public string PayTaxAndDuties { get; set; }
        public string PaymentPartyAccountNumber { get; set; }
        public CurrencyType Currency { get; set; }
        public List<PackageDraft> Packages { get; set; }
        public ReferenceDetail ReferenceDetail { get; set; }
        public CustomInformation CustomInfo { get; set; }
        public DirectBookingService CustomerRateCard { get; set; }
        public int CustomerId { get; set; }
        public int CreatedBy { get; set; }
        public int ShipmentStatusId { get; set; }
        public int ShippingMethodId { get; set; }
        public string PakageCalculatonType { get; set; }
        public FratyteError Error { get; set; }
        public decimal BaseRate { get; set; }
        public decimal MarginCost { get; set; }
        public DateTime FuelMonth { get; set; }
        public float FuelPercent { get; set; }
        public float AdditionalSurcharge { get; set; }
        public float FuelSurCharge { get; set; }
        public float EstimatedCost { get; set; }
        public float EstimatedTotalCost { get; set; }
        public string TaxAndDutiesAcceptedBy { get; set; }
        public string BookingStatusType { get; set; }
        public string FrayteNumber { get; set; }
        public string AddressType { get; set; }
        public bool IsPublic { get; set; }
        public int RoleId { get; set; }
        public int LogInUseId { get; set; }
    }

    public class DirectBookingDraftCollection
    {
        public int AddressBookId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CurrencyCode { get; set; }
        public FrayteCountryCode Country { get; set; }
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
        public bool IsMailSend { get; set; }
        public string AddressType { get; set; }
        public bool IsShipperTaxAndDuty { get; set; }
        public bool IsFavorites { get; set; }
        public bool IsDefaultAddess { get; set; }
    }

    public class PackageDraft
    {
        public int DirectShipmentDetailDraftId { get; set; }
        public int PackageTrackingDetailId { get; set; }
        public int CartoonValue { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public decimal Value { get; set; }
        public string HSCode { get; set; }
        public string Content { get; set; }
        public string LabelName { get; set; }
        public string TrackingNo { get; set; }
        public bool? IsPrinted { get; set; }
    }

    public class FrayteDirectBookingAddressBook
    {
        public int AddressbookId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public bool FromAddress { get; set; }
        public bool ToAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Area { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostCode { get; set; }
        public FrayteCountryCode Country { get; set; }
        public bool IsActive { get; set; }
        public bool IsFavorites { get; set; }
        public string TableType { get; set; }
        public int TotalRows { get; set; }
        public bool IsDefault { get; set; }
    }

    public class FrayteCustomerAddressSearch
    {
        public string AddressType { get; set; }
        public string AddressSearch { get; set; }
        public string SearchBy { get; set; }
        public string SearchText { get; set; }
        public int CustomerId { get; set; }
        public int CurrentPage { get; set; }
        public int TakeRows { get; set; }
        public string ModuleType { get; set; }
        public int CountryId { get; set; }
    }

    public class FraytePostCodeAddress
    {
        public string CompanyName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Area { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
    }

    public class FrayteCommercialInvoice
    {
        public string ShipperInfo { get; set; }
        public string ConsigneeInfo { get; set; }
        public DateTime DateOfExport { get; set; }
        public string Reference { get; set; }
        public string Currency { get; set; }
        public string FromCountry { get; set; }
        public string ToCountry { get; set; }
        public string AirWayBillNo { get; set; }
        public int TotalValue { get; set; }
        public List<FrayteCommercialInvoicePackageInfo> PackageInfo { get; set; }
    }

    public class FrayteCommercilaFileName
    {
        public string CompanyName { get; set; }
        public string FrayetNo { get; set; }
        public string LogisticType { get; set; }
        public string RateType { get; set; }
        public DateTime CreationDate { get; set; }
    }

    public class FrayteCommercialInvoicePackageInfo
    {
        public string ShipmentContents { get; set; }
        public int Quantity { get; set; }
        public decimal DeclaredValue { get; set; }
    }

    public class FrayteCommercialInvoiceFileName
    {
        public string FileName { get; set; }
    }

    public class PdfImage
    {
        public string FullPath { get; set; }
    }

    public class FrayteServiceExchangeRate
    {
        public string Currency { get; set; }
        public decimal ExchangeRate { get; set; }
    }

    public class FrayteCustomerCurrency
    {
        public string CreditLimitCurrencyCode { get; set; }
        public decimal ExchangeRate { get; set; }
    }

    public class FraytePODInfomation
    {
        public int DirectShipmentId { get; set; }
        public string Carrier { get; set; }
        public string TrackingNo { get; set; }
        public string DeliveryDate { get; set; }
        public string DeliveryTime { get; set; }
        public string SignedBy { get; set; }
    }

    public class PODInformation
    {
        public List<FraytePODInfomation> PodDetail { get; set; }
    }    
}