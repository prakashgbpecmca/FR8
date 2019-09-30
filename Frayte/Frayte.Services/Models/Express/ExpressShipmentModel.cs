using Frayte.Services.DataAccess;
using Frayte.Services.Models.ApiModal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.Express
{
    public class ExpressAwbLabel
    {
        public string path { get; set; }
        public string physicalPath { get; set; }
    }

    public class ExpressLabelEmail
    {
        public int ShipmentId { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; }
    }

    public class ExpressServiceRequestModel
    {
        public int FromCountryId { get; set; }
        public string FromPostCode { get; set; }
        public string FromState { get; set; }
        public int ToCountryId { get; set; }
        public string ToPostCode { get; set; }
        public string ToState { get; set; }
        public decimal TotalWeight { get; set; }
        public string WeightUnit { get; set; }
        public int CustomerId { get; set; }
    }

    public class ExpressServiceResponseModel
    {
        public int HubCarrierId { get; set; }
        public string HubCarrier { get; set; }
        public string LogisticServiceType { get; set; }
        public string HubCarrierDisplay { get; set; }
        public string CourierAccountNo { get; set; }
        public string RateType { get; set; }
        public List<DirectBookingOptionalServices> OptionalServices { get; set; }
    }

    public class ExpressShipmentModel
    {
        public int ExpressId { get; set; }
        public int OperationZoneId { get; set; }
        public int CustomerId { get; set; }
        public string AWBNumber { get; set; }
        public string TrackingNo { get; set; }
        public string CustomerAccountNumber { get; set; }
        public int ShipmentStatusId { get; set; }
        public string ShipmentStatusDisplay { get; set; }
        public List<ExpressPackageModel> Packages { get; set; }
        public List<ExpressPackageDetailModel> DetailPackages { get; set; }
        public string LabelName { get; set; }
        public string LabelPath { get; set; }
        public ExpressAddressModel ShipFrom { get; set; }
        public ExpressAddressModel ShipTo { get; set; }
        public ExpressCustomInformationModel CustomInformation { get; set; }
        public int CreatedBy { get; set; }
        public HubService Service { get; set; }
        public DateTime ScannedOnDate { get; set; }
        public string ScannedOnTime { get; set; }
        public string ScannedBy { get; set; }
        public int ScannedById { get; set; }
        public string AWBPath { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public string CreatedOnTime { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOnUtc { get; set; }
        public string FrayteNumber { get; set; }
        public string PakageCalculatonType { get; set; }
        public string LogisticType { get; set; }
        public string ShipmentReference { get; set; }
        public string ShipmentDescription { get; set; }
        public decimal DeclaredValue { get; set; }
        public CurrencyType DeclaredCurrency { get; set; }
        public decimal ActualWeight { get; set; }
        public string PayTaxAndDuties { get; set; }
        public FrayteParcelType ParcelType { get; set; }
        public string TrackingNumber { get; set; }
        public string TaxAndDutiesAccountNo { get; set; }
        public string TaxAndDutiesAcceptedBy { get; set; }
        public bool? DangerousGoods { get; set; }
        public string Carrier { get; set; }
        public string CarrierService { get; set; }
        public FratyteError Error { get; set; }
        public string CreatedByName { get; set; }
        public string AdditionalInfo { get; set; }
    }

    public class HubService
    {
        public int HubCarrierId { get; set; }
        public int HubCarrierServiceId { get; set; }
        public string HubCarrier { get; set; }
        public string LogisticServiceType { get; set; }
        public string HubCarrierDisplay { get; set; }
        public string CourierAccountNo { get; set; }
        public string RateType { get; set; }
        public string RateTypeDisplay { get; set; }
        public decimal BillingWeight { get; set; }
        public decimal ActualWeight { get; set; }
        public string WeightRoundLogic { get; set; }
        public string CarrierLogo { get; set; }
        public string TransitTime { get; set; }
        public string NetworkCode { get; set; }
        public List<DirectBookingOptionalServices> OptionalServices { get; set; }
        public string DefaultCurrency { get; set; }
    }

    public class ExpressServiceObj
    {
        public int ToCountryId { get; set; }
        public string ToPostCode { get; set; }
        public int CustomerId { get; set; }
    }

    public class ExpressCustomInformationModel
    {
        public int ShipmentCustomDetailId { get; set; }
        public int ShipmentId { get; set; }
        public string ContentsType { get; set; }
        public string ContentsExplanation { get; set; }
        public string RestrictionType { get; set; }
        public string RestrictionComments { get; set; }
        public bool? CustomsCertify { get; set; }
        public string CustomsSigner { get; set; }
        public string NonDeliveryOption { get; set; }
        public string EelPfc { get; set; }
        public string CatagoryOfItem { get; set; }
        public string CatagoryOfItemExplanation { get; set; }
        public string CommodityCode { get; set; }
        public string TermOfTrade { get; set; }
        public string ModuleType { get; set; }
    }

    public class ExpressAddressModel
    {
        public int ExpressAddressId { get; set; }
        public int CustomerId { get; set; }
        public string AddressType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string Area { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostCode { get; set; }
        public bool IsDefault { get; set; }
        public bool IsMailSend { get; set; }
        public FrayteCountryCode Country { get; set; }
        public int HubId { get; set; }
        public string HubCode { get; set; }
    }

    public class ExpressPackageDetailModel
    {
        public int ExpressDetailId { get; set; }
        public int ExpressDetailPackageLabelId { get; set; }
        public string LabelName { get; set; }
        public string LabelPath { get; set; }
        public int ExpressId { get; set; }
        public string CartonNumber { get; set; }
        public int CartonValue { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public string Content { get; set; }
        public decimal Value { get; set; }
        public string TrackingNumber { get; set; }
        public bool IsDownloaded { get; set; }
    }

    public class ExpressPackageModel
    {
        public int ExpressDetailId { get; set; }
        public int ExpressId { get; set; }
        public string CartonNumber { get; set; }
        public int CartonValue { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public string Content { get; set; }
        public decimal Value { get; set; }
        public int ProductCatalogId { get; set; }
    }

    public class ExpressState
    {
        public int HubCarrierServiceCountryStateId { get; set; }
        public int HubId { get; set; }
        public int HubCarrierServiceId { get; set; }
        public int CountryId { get; set; }
        public string State { get; set; }
        public string StateDisplay { get; set; }
    }

    public class ExpressErrorResponse
    {
        public bool Status { get; set; }
        public string Description { get; set; }
        public string ShipmentId { get; set; }
        public List<FrayteApiError> Errors { get; set; }
    }

    public class ExpressTrackandTrace
    {
        public string Mawb { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string AwbNo { get; set; }
        public string TrackingNo { get; set; }
        public int ShipmentStatusId { get; set; }
        public int CustomerId { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string CustomerName { get; set; }
    }
}