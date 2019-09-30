using Frayte.Services.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public class FrayteUploadshipment
    {
        public int CustomerId { get; set; }
        public string CourierCompany { get; set; }
        public string CourierCompanyDisplay { get; set; }
        public string CustomerName { get; set; }
        public int WareHouseId { get; set; }
        public int OpearionZoneId { get; set; }
        public string BookingStatusType { get; set; }
        public string BookingApp { get; set; }
        public DirectBookingCollection ShipFrom { get; set; }
        public DirectBookingCollection ShipTo { get; set; }
        public FrayteCountryCode FromCountry { get; set; }
        public string FromPostCode { get; set; }
        public string FromContactFirstName { get; set; }
        public string FromContactLastName { get; set; }
        public string FromAddress { get; set; }
        public string FromAddress2 { get; set; }
        public string FromCity { get; set; }
        public string FromTelephoneNo { get; set; }
        public FrayteCountryCode ToCountry { get; set; }
        public string ToPostCode { get; set; }
        public string ToContactFirstName { get; set; }
        public string ToContactLastName { get; set; }
        public string ToAddress { get; set; }
        public string ToAddress2 { get; set; }
        public string ToCity { get; set; }
        public string ToTelephoneNo { get; set; }
        public string PackageCalculationType { get; set; }
        public string parcelType { get; set; }
        public string CurrencyCode { get; set; }
        public string ShipmentReference { get; set; }
        public string ShipmentDescription { get; set; }
        public string TrackingNo { get; set; }
        public DateTime? EstimatedDateofArrival { get; set; }
        public string EstimatedTimeofArrival { get; set; }
        public DateTime? EstimatedDateofDelivery { get; set; }
        public string EstimatedTimeofDelivery { get; set; }
        public DateTime CreatedOn { get; set; }
        public string FrayteNumber { get; set; }
        public FratyteError Error { get; set; }
        public int DirectShipmentDraftId { get; set; }
        public int ShipmentStatusId { get; set; }
        public string PayTaxAndDuties { get; set; }
        public List<UploadShipmentPackage> Package { get; set; }
        public CustomInformation CustomInfo { get; set; }
        public CountryLogistic Service { get; set; }
        public bool ServiceValue { get; set; }
        public List<string> Errors { get; set; }
        public string ModuleType { get; set; }
        public string EasyPostError { get; set; }
        public string EasyPostPickUpObj { get; set; }
        public bool? IsSelectServiceStatus { get; set; }
        public List<FailedValidationObj> RemainedFields { get; set; }
        public string ServiceCode { get; set; }
        public DirectBookingService CustomerRateCard { get; set; }
        public int ShippingMethodId { get; set; }
        public int UserId { get; set; }
        public string AddressType { get; set; }
        public string CollectionDate { get; set; }
        public string CollectionTime { get; set; }
        public int SessionId { get; set; }
        public List<string> ValidationErrors { get; set; }
        
    }

    public class FrayteServiceCode
    {
        public string LogisticCompany { get; set; }

        public string LogisticCompanyDisplay { get; set; }
        public string LogisticDescription { get; set; }
        public string ServiceCode { get; set; }
        public decimal WeightTo { get; set; }
        public decimal WeightFrom { get; set; }

        public string LogisticType { get; set; }
        public string RateType { get; set; }
    }


    public class FailedValidationObj
    {
        public object FieldName { get; set; }
        public string FileType { get; set; }
        public string FieldLabel { get; set; }
        public List<string> RadioButtonValues { get; set; }
        public string DashString { get; set; }
        public string IterationFor1 { get; set; }
        public string IterationFor2 { get; set; }

        public string IterationForAs { get; set; }
        public string TrackBYOBJ { get; set; }
        public List<object> GeneralObj { get; set; }

        public string LabelName { get; set; }
        public string InputTypeName { get; set; }

        public string ShowDiv { get; set; }
        public string RequiredMessage { get; set; }

        public string ConditionalRequired { get; set; }

        public string RequiredString { get; set; }

    }

    public class UploadShipmentBatchProcess
    {
        public int? CustomerId { get; set; }
        public int? ProcessedShipment { get; set; }
        public int? UnprocessedShipment { get; set; }

        public int? TotalShipments { get; set; }
    }
    public class FrayteCommerceUploadShipmentDraft
    {
        public int DirectShipmentDraftId { get; set; }
        public int OpearionZoneId { get; set; }
        public int WareHouseId { get; set; }
        public int ManifestId { get; set; }
        public string TrackingCode { get; set; }

        public string TrackingNo { get; set; }

        public CountryLogistic CourierCompany { get; set; }

        public string LogisticCompany { get; set; }
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
        public string ModuleType { get; set; }
        public string BookingApp { get; set; }
    }


    public class FraytDBUploadShipmentDraft
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
        public string PakageCalculatonType { get; set; }
        public string TaxAndDutiesAcceptedBy { get; set; }
        public string BookingStatusType { get; set; }
        public string AddressType { get; set; }

    }

    public class FrayteeCommerceUserShipment
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

        public CountryLogistic Service { get; set; }

        public bool ServiceValue { get; set; }

        public string ServiceCode { get; set; }
    }

    public class CountryLogistic
    {
        public int CountryLogisticId { get; set; }
        public int CountryId { get; set; }
        public string LogisticService { get; set; }
        public string LogisticServiceDisplay { get; set; }
        public string AccountId { get; set; }
        public int AccountNo { get; set; }
        public string CountryCode { get; set; }
        public string Description { get; set; }
    }

    public class UploadShipmentPackage
    {

        public int CartoonValue { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public decimal Value { get; set; }
        public string Content { get; set; }
        public int DirectShipmentDetailDraftId { get; set; }

        public int PackageTrackingDetailId { get; set; }

        public string LabelName { get; set; }

        public bool? IsPrinted { get; set; }

        public string TrackingNo { get; set; }


    }

    public class FayteeCommerceUploadShipmentExcel
    {
        public string Message { get; set; }
        public List<FrayteUploadshipment> FrayteManifestDetail { get; set; }
    }

    public class FrayteUploadShipmentWithServiceOnExcel
    {
        public int FromCountryCode { get; set; }
        public string FromPostCode { get; set; }
        public string FromContactFirstName { get; set; }
        public int FromContactLastName { get; set; }
        public string FromCompanyName { get; set; }
        public string FromAddress1 { get; set; }

        public string FromAddress2 { get; set; }
        public string FromCity { get; set; }
        public string FromTelephoneNo { get; set; }
        public string FromEmail { get; set; }
        public string ToCountryCode { get; set; }
        public string ToPostCode { get; set; }
        public string ToContactFirstName { get; set; }
        public string ToContactLastName { get; set; }
        public string ToCompanyName { get; set; }
        //public string ConsigneeAddress3 { get; set; }
        public string ToAddress1 { get; set; }
        public string ToAddress2 { get; set; }
        public string ToCity { get; set; }
        public string ToTelephoneNo { get; set; }
        public string ToEmail { get; set; }
        public string PackageCalculationType { get; set; }
        public int ParcelType { get; set; }
        public decimal Currency { get; set; }
        public string ShipmentReference { get; set; }
        public decimal? ShipmentDescription { get; set; }
        public int CartonValue { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        //public string ItemQuantity { get; set; }
        public decimal Weight { get; set; }

        public decimal DeclaredValue { get; set; }
        public decimal ShipmentContents { get; set; }

    }

    public class FrayteUploadShipmentWithOutServiceOnExcel
    {
        public int FromCountryCode { get; set; }
        public string FromPostCode { get; set; }
        public string FromContactFirstName { get; set; }
        public int FromContactLastName { get; set; }
        public string FromCompanyName { get; set; }
        public string FromAddress1 { get; set; }
        //public string ShipperAddress3 { get; set; }
        public string FromAddress2 { get; set; }
        public string FromCity { get; set; }
        public string FromTelephoneNo { get; set; }
        public string FromEmail { get; set; }
        public string ToCountryCode { get; set; }
        public string ToPostCode { get; set; }
        public string ToContactFirstName { get; set; }
        public string ToContactLastName { get; set; }
        public string ToCompanyName { get; set; }
        //public string ConsigneeAddress3 { get; set; }
        public string ToAddress1 { get; set; }
        public string ToAddress2 { get; set; }
        public string ToCity { get; set; }
        public string ToTelephoneNo { get; set; }
        public string ToEmail { get; set; }
        public string PackageCalculationType { get; set; }
        public int ParcelType { get; set; }
        public decimal Currency { get; set; }
        public string ShipmentReference { get; set; }
        public decimal? ShipmentDescription { get; set; }
        public int TrackingNo { get; set; }
        public string CourierCompany { get; set; }

        public int CartonValue { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        //public string ItemQuantity { get; set; }
        public decimal Weight { get; set; }

        public decimal DeclaredValue { get; set; }
        public decimal ShipmentContents { get; set; }

    }

    public class FrayteeCommerceWithServiceShipmentfilter
    {
        public List<FrayteUserDirectShipment> SucessfulShipments { get; set; }
        public List<FrayteUserDirectShipment> UnsucessfulShipments { get; set; }
    }


    public class FrayteeCommerceUploadShipmentLabelReport
    {
        public FrayteUploadshipment eCommerceShipment { get; set; }
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

    public static class eCommerceString
    {
        public const int AddressStringLength = 35;
        public const int PhoneStringLength = 20;
    }

    public class ContentTypeModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
    public class RestrictionTypeModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
    public class NonDeliveryOptionModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
    public class ItemCatogoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class SessionModel
    {
        public int SessionId { get; set; }
        public string SessionName { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string SessionStatus { get; set; }
        public int TotalShipments { get; set; }
        public DateTime? PrintedOn { get; set; }
    }
}
