using Frayte.Services.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public partial class FrayteeCommerceInvoice
    {
        public int InvoiceId { get; set; }
        public int CustomerId { get; set; }
        public int eCommerceShimentId { get; set; }
        public decimal FreeStorageCharge { get; set; }
        public string FreeStorageCurrency { get; set; }
        public TimeSpan FreeStorageTime { get; set; }
        public DateTime ETADate { get; set; }
        public string ETATime { get; set; }
        public string InvoiceRef { get; set; }
        public string InvoiceCurrencyCode { get; set; }
        public string InvoiceFullName { get; set; } 
        public string Status { get; set; }
    }
    public class LabelReportFile
    {
        public string FileName { get; set; }
        public int eCommerceShipmentId { get; set; }

    }

    public class PrintLabel
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public bool FileStatus { get; set; }
    }

    public static class eCommLabelType
    {
        public const string CourierLabel = "CourierLabel";
        public const string FrayteLabel = "FrayteLabel";
    }
    public class ETAETDManifest
    {
        public int UserId { get; set; }
        public DateTime? EstimatedDateofArrival { get; set; }
        public string EstimatedTimeofArrival { get; set; }
        public DateTime? EstimatedDateofDelivery { get; set; }
        public string EstimatedTimeofDelivery { get; set; }
        public List<FrayteUserDirectShipment> Shipments { get; set; }
    }
    public class HSCodeMapped
    {
        public bool Status { get; set; }
        public int Id { get; set; }
    }

    public class FrayteCommercePackageTrackingDetail
    {
        public int eCommercePackageTrackingDetailId { get; set; }
        public int eCommerceShipmentDetailId { get; set; }
        public string LabelUrl { get; set; }
        public string TrackingNo { get; set; }
        public string PackageImage { get; set; }
        public string FraytePackageImage { get; set; }
        public bool IsDownloaded { get; set; }
        public bool IsPrinted { get; set; }
    }
    public class FrayteCommerceShipmentDraft
    {
        public int DirectShipmentDraftId { get; set; }
        public int OpearionZoneId { get; set; }
        public int WareHouseId { get; set; }
        public int ManifestId { get; set; }
        public string TrackingCode { get; set; }
        public eCommerceShipmentAddressDraft ShipFrom { get; set; }
        public eCommerceShipmentAddressDraft ShipTo { get; set; }
        public FrayteParcelType ParcelType { get; set; }
        public DateTime CreatedOn { get; set; }
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
        public string CourierCompany { get; set; }
        public string CourierCompanyDisplay { get; set; }
        public string ModuleType { get; set; }
        public string BookingApp { get; set; }
    }

    public class FrayteeCommerceShipment
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
        public float EstimatedCost { get; set; }
        public float EstimatedTotalCost { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public TimeSpan? DeliveryTime { get; set; }
        public string SignedBy { get; set; }
        public string TaxAndDutiesAcceptedBy { get; set; }
        public string FrayteNumber { get; set; }

    }
    public class eCommerceShipmentAddressDraft
    {
        public int DirectShipmentAddressDraftId { get; set; }
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
        public string ModuleType { get; set; }
    }
    public class eCommerceWareHouse
    {
        public int WarehouseId { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public FrayteCountryCode Country { get; set; }
        public string Email { get; set; }
        public string TelephoneNo { get; set; }

    }
    public class FrayteeCommerceShipmentDetail
    {
        public int OpearionZoneId { get; set; }
        public int eCommerceShipmentId { get; set; }
        public FrayteeCommerceShipmentAddress ShipFrom { get; set; }
        public FrayteeCommerceShipmentAddress ShipTo { get; set; }
        public FrayteParcelType ParcelType { get; set; }
        public DateTime? CreatedOn { get; set; }
        public eCommerceLogisticService LogisticDetail { get; set; }
        public string PayTaxAndDuties { get; set; }
        public string PaymentPartyAccountNumber { get; set; }
        public eCommerceWareHouse Warehouse { get; set; }
        public CurrencyType Currency { get; set; }
        public List<eCommercePackage> Packages { get; set; }
        public ReferenceDetail ReferenceDetail { get; set; }
        public CustomInformation CustomInfo { get; set; }
        public DirectBookingService CustomerRateCard { get; set; }
        public int CustomerId { get; set; }
        public int ShipmentStatusId { get; set; }
        public string LogisticCompany { get; set; }
        public string PakageCalculatonType { get; set; }
        public FratyteError Error { get; set; }

        public DateTime? DeliveryDate { get; set; }
        public TimeSpan? DeliveryTime { get; set; }
        public string SignedBy { get; set; }
        public string TaxAndDutiesAcceptedBy { get; set; }
        public string FrayteNumber { get; set; }
        public string CourierCompany { get; set; }
        public string CourierCompanyDisplay { get; set; }
        public string ModuleType { get; set; }
        public string CreatedBy { get; set; }
        public string BookingApp { get; set; } 
        public DateTime? EstimatedDateofArrival { get; set; }
        public string EstimatedTimeofArrival { get; set; }
        public DateTime? EstimatedDateofDelivery { get; set; }
        public string EstimatedTimeofDelivery { get; set; }
    }
    public class eCommercePackage
    {
        public int eCommerceShhipmentId { get; set; }
        public int eCommerceShipmentDetailId { get; set; }
        public int eCommercePackageTrackingDetailId { get; set; }
        public int CartoonValue { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public decimal Value { get; set; }
        public string Content { get; set; }
        public string HSCode { get; set; }
        public string LabelName { get; set; }
        public string Label { get; set; }
        public string FrayteLabelName { get; set; }
        public string FrayteLabel { get; set; }
        public string TrackingNo { get; set; }
        public bool? IsPrinted { get; set; }
        public bool? IsFrayteAWBPrinted { get; set; }
    }
    public class FrayteeCommerceShipmentAddress
    {
        public int eCommerceShipmentAddressId { get; set; }
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
    }
    public class FrayteeCommerceShipmentLabelReport
    {
        public FrayteCommerceShipmentDraft eCommerceShipment { get; set; }
        public string CustomerName { get; set; }
        public string AccountNo { get; set; }
        public string CurrentShipment { get; set; }
        public int TotalShipments { get; set; }
        public string TotalChargeableWeight { get; set; }
        public string WeightUnit { get; set; }
        public float TotalWeigth { get; set; }
        public string ServiceType { get; set; }
        public string TaxAndDutyType { get; set; }
        public string CourierCompanyDisplay { get; set; }
        public string TotalValue { get; set; }
        public string BarcodePath { get; set; }
        public List<PackageLabel> LabelPackages { get; set; }
    }
    public class PackageLabel
    {
        public int DirectShipmentDetailDraftId { get; set; }
        public int PackageTrackingDetailId { get; set; }
        public int CartoonValue { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public decimal Value { get; set; }
        public string Content { get; set; }
        public string LabelName { get; set; }
        public string TrackingNo { get; set; }
        public bool? IsPrinted { get; set; }
        public int SerialNo { get; set; }
    }
    public class eCommerceLogisticService
    {
        public int CountryLogisticId { get; set; }
        public string LogisticService { get; set; }
        public string LogisticServiceDisplay { get; set; }
        public string AccountNo { get; set; }
        public string AccountId { get; set; }
        public string CountryCode { get; set; }
        public int CountryId { get; set; }
        public string Description { get; set; }
    }


    public class InvoiceAdminCharge
    {
        public string ChargeDescription { get; set; }
        public string ChargeShortname { get; set; }
        public decimal Amount { get; set; }
        public string AmountWithCurrency { get; set; }
        public string CurrencyCode { get; set; }

    }
    public class eCommerceTaxAndDutyInvoiceReport
    {
        public FrayteeCommerceShipmentDetail eCommerceBookingDetail { get; set; }
        public FaryteBankAccount BankDetail { get; set; }
        public FrayteCompany CompanyDetail { get; set; }
        public List<InvoiceAdminCharge> AdminCharges { get; set; }
        public string InvoiveFullName { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDateTime { get; set; }
        public string InvoiceDate { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public string ArrivalDate { get; set; }
        public string ArrivalTime { get; set; }
        public DateTime FreeStorageDateTime { get; set; }
        public string FreeStorageDate { get; set; }
        public string FreeStorageTime { get; set; }
        public string CongigmentNo { get; set; }
        public string InvoiceRef { get; set; }
        public string CurrencyCode { get; set; }
        public int TotalDeclaration { get; set; }
        public decimal TotalDuty { get; set; }
        public decimal TotalVAT { get; set; }
        public decimal AdminSpecClearServ { get; set; }
        public decimal AdminDisbursement { get; set; }
        public decimal TotalAdminVAT { get; set; }
        public decimal TotalAdminCharge { get; set; }
        public decimal TotalOtherLevy { get; set; }
        public decimal NettCharge { get; set; }
        public decimal TotalCustomCharge { get; set; }
        public decimal AmountDue { get; set; }
        public DateTime DueDate { get; set; }
        public string DueTime { get; set; }
        public string Status { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAccountNo { get; set; }  
    }

    public class FrayteCompany
    {
        public string CompanyName { get; set; }
        public FrayteBankAddress Address { get; set; }
        public string AccountEmail { get; set; }
        public string CashEmail { get; set; }
        public string SalesEmail { get; set; }
        public string CompanyPhoneCode { get; set; }
        public string CompanyNo { get; set; }
        public string CompanyPhone { get; set; }
        public string CrestCode { get; set; }

    }
    public class FaryteBankAccount
    {
        public string Name { get; set; }
        public FrayteBankAddress Address { get; set; }
        public string AcountNo { get; set; }
        public string SortCode { get; set; }
        public string IBANCode { get; set; }
        public string SwiftCode { get; set; }
    }

    public class FrayteBankAddress
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string State { get; set; }
        public FrayteCountryCode Country { get; set; }
    }

    public class FrayteManifestOnExcel
    {
        public int ECommerceShipmentId { get; set; }
        public string TrackingNumber { get; set; }
        public string Reference { get; set; }
        public int InternalAccountNumber { get; set; }
        public string ShipperName { get; set; }
        public string ShipperAddress1 { get; set; }
        public string ShipperAddress2 { get; set; }
        //public string ShipperAddress3 { get; set; }
        public string ShipperCity { get; set; }
        public string ShipperZip { get; set; }
        public string ShipperState { get; set; }
        public string ShipperPhoneNo { get; set; }
        public string ShipperEmail { get; set; }
        public string ShipperCountryCode { get; set; }
        public string ConsigneeName { get; set; }
        public string ConsigneeAddress1 { get; set; }
        public string ConsigneeAddress2 { get; set; }
        //public string ConsigneeAddress3 { get; set; }
        public string ConsigneeCity { get; set; }
        public string ConsigneeZip { get; set; }
        public string ConsigneeState { get; set; }
        public string ConsigneePhoneNo { get; set; }
        public string ConsigneeEmail { get; set; }
        public string ConsigneeCountryCode { get; set; }
        public int Pieces { get; set; }
        public decimal TotalWeight { get; set; }
        public string WeightUOM { get; set; }
        public decimal? TotalValue { get; set; }
        public string Currency { get; set; }
        public string Incoterms { get; set; }
        public string ItemDescription { get; set; }
        public string ItemHScodes { get; set; }
        //public string ItemQuantity { get; set; }
        public decimal? ItemValue { get; set; }

        public string CustomCommodityMap { get; set; }
        public string CustomEntryType { get; set; }
        public decimal CustomTotalValue { get; set; }
        public decimal CustomTotalVAT { get; set; }
        public decimal CustomDuty { get; set; }
        public DateTime? EstimatedDateofDelivery { get; set; }
        public TimeSpan? EstimatedTimeofDelivery { get; set; }
        public DateTime? EstimatedDateofArrival { get; set; }
        public TimeSpan? EstimatedTimeofArrival { get; set; }

    }

    public class FrayteManifestExcel
    {
        public string Message { get; set; }
        public List<FrayteManifestOnExcel> FrayteManifestDetail { get; set; }
    }

    public class CustomManifestDetail
    {
        public string ManifestName { get; set; }
        public string Courier { get; set; }
        public DateTime? ManifestDate { get; set; }
        public int NumberofShipments { get; set; }
        public decimal? TotalWeight { get; set; }
        public int ManifestId { get; set; }
        public string CourierDisplay { get; set; }
        public int NoOfShipments { get; set; }
        public DateTime CreateOn { get; set; }
        public int CustomerId { get; set; }
        public string ModuleType { get; set; }
        public int TotalRows { get; set; }
    }

    public class TrackCustomManifest
    {
        public int UserId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string ModuleType { get; set; }
        public string ShipperName { get; set; }
        public string ConsigneeName { get; set; }
        public int CurrentPage { get; set; }
        public int TakeRows { get; set; }
        public string ManifestName { get; set; }
    }

    public class eCommerceViewManifest
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
        public FrayteUserRole UserRoleId { get; set; }
        public string Reference1 { get; set; }
        public string FrayteNumber { get; set; }
        public string TrackingNo { get; set; }
        public int TotalRows { get; set; }
        public int ManifestId { get; set; }
        public string ManifestName { get; set; }

        public List<UploadShipmentPackage> PackageDetail { get; set; }
    }

    public class LogisticServiceShipment
    {
        public int LogisticServiceShipmentTypeId { get; set; }
        public int LogisticServiceId { get; set; }
        public string ServiceCode { get; set; }
    }


}
