using Frayte.Services.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{ 

    public class MobileInternalUserConfiguration
    {
        public int UserId { get; set; } 
        public List<MasterTrackingModel> DirectBookingConfiguration { get; set; }
        public List<MasterTrackingModel> TradelaneConfiguration { get; set; }
        public List<MasterTrackingModel> ECommerceConfiguration { get; set; }
        public List<MasterTrackingModel> BreakBulkConfiguration { get; set; }
        public List<MasterTrackingModel> ExpressConfiguration { get; set; }
        public List<MasterTrackingModel> WarehouseTransportConfiguration { get; set; }
    }


    public class MasterTrackingModel
    {
        public int MasterTrackingId { get; set; }
        public string ModuleType { get; set; }
        public string EventKey { get; set; }
        public string EventDisplay { get; set; }
        public List<MasterTrackingDetailModel> TrackingDetail { get; set; }


    }
    public class MasterTrackingDetailModel
    {
        public int MobileUserConfigurationId { get; set; }
        public int MasterTrackingDetailId { get; set; }
        public string EventKey { get; set; }
        public string EventDisplay { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public string Message { get; set; }
        public bool IsExternal { get; set; }

    }
    public class FrayteUserTabStatus
    {
        public string Status { get; set; }
        public List<FrayteTab> tabs { get; set; }
    }

    public class FrayteLoginUserLogin
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string Password { get; set; }
        public int OperationStaffId { get; set; }
        public int RoleId { get; set; }
    }

    public class ShipmentDetailE6
    {
        public string ImageHeader { get; set; }
        public string TimeZoneName { get; set; }
        public string TimeZoneShort { get; set; }
        public int ShipmentId { get; set; }
        public string ShipmentMethod { get; set; }
        public Nullable<int> TotalCTNs { get; set; }
        public Nullable<int> TotalWeight { get; set; }
        public string UserName { get; set; }
        public string UserPosition { get; set; }
        public string UserEmail { get; set; }
        public string UserPhone { get; set; }
        public string UserSkype { get; set; }
        public string UserFax { get; set; }
        public string UserManagerEmail { get; set; }
        public string ShipperEmail { get; set; }
        public string ClientName { get; set; }
        public string ShipperName { get; set; }
        public List<string> CountryDocuments { get; set; }
        public string UploadLinkHref { get; set; }
        public string EasyPostTrackingUrl { get; set; }
        public string EsayPostLabel { get; set; }
        public string ShipmentType { get; set; }
        public string EasyPostTrackingCode { get; set; }
    }

    public class NewCustomerAdminMail
    {
        public int CustomerId { get; set; }
        public int OperationStaffId { get; set; }
        public string UserName { get; set; }
        public string UserPosition { get; set; }
        public string UserEmail { get; set; }
        public string UserPhone { get; set; }
        public string UserSkype { get; set; }
        public string UserFax { get; set; }
        public string ImageHeader { get; set; }
        public string HostUrl { get; set; }
    }

    public class ShipmentDetailE62
    {
        public string ImageHeader { get; set; }
        public string MailDate { get; set; }
        public string MailTime { get; set; }
        public int ShipmentId { get; set; }
        public string ShipmentMethod { get; set; }
        public string ShipperName { get; set; }
        public string ReceiverName { get; set; }
        public Nullable<int> TotalCTNs { get; set; }
        public Nullable<int> TotalWeight { get; set; }
        public Nullable<int> GrowssWeight { get; set; }
        public Nullable<decimal> CBM { get; set; }
        public Nullable<decimal> CBMValue { get; set; }
        public Nullable<decimal> Chargableweight { get; set; }
        public string UploadLinkHref { get; set; }
        public string PAddress { get; set; }
        public string PAddress2 { get; set; }
        public string PAddress3 { get; set; }
        public string PCity { get; set; }
        public string PState { get; set; }
        public string PCountry { get; set; }
        public string PZip { get; set; }
        public string RAddress { get; set; }
        public string RAddress2 { get; set; }
        public string RAddress3 { get; set; }
        public string RCity { get; set; }
        public string RState { get; set; }
        public string RCountry { get; set; }
        public string RZip { get; set; }
        public string UserName { get; set; }
        public string UserPosition { get; set; }
        public string UserEmail { get; set; }
        public string UserPhone { get; set; }
        public string UserSkype { get; set; }
        public string UserFax { get; set; }
        public string CustomerName { get; set; }
        public string UserManagerEmail { get; set; }
        public string TimeZoneName { get; set; }
        public string TimeZoneShort { get; set; }
        public string DrpofAddress { get; set; }
        public string DropofCity { get; set; }
        public string DropofState { get; set; }
        public string DropofZip { get; set; }
        public string DropofCountry { get; set; }
        public string DepartureDate { get; set; }
        public string DepartureTime { get; set; }
        public string ArrivalDate { get; set; }
        public string ArrivalTime { get; set; }
        public string ShipmentType { get; set; }
    }

    public class FrayteAdminAction
    {
        public string ActionType { get; set; }
        public int CustomerId { get; set; }
        public int OperationUserId { get; set; }
    }

    public class AdminActionMailModel
    {
        public string UserName { get; set; }
        public string UserPosition { get; set; }
        public string UserEmail { get; set; }
        public string UserPhone { get; set; }
        public string UserSkype { get; set; }
        public string UserFax { get; set; }
        public string ToName { get; set; }
        public string ImageHeader { get; set; }
        public string HostUrl { get; set; }
    }

    public class AdminActionAmendMailModel : FrayteCustomer
    {
        public string UserName { get; set; }
        public string ToName { get; set; }
        public string UserPosition { get; set; }
        public string StaffName { get; set; }
        public string StaffEmail { get; set; }
        public string UserPhone { get; set; }
        public string UserSkype { get; set; }
        public string UserFax { get; set; }
        public string ImageHeader { get; set; }
        public string HostUrl { get; set; }
        public string SiteAddress { get; set; }
        public List<string> UpdateDetail { get; set; }
    }

    public class FrayteUser
    {
        public int UserId { get; set; }
        public string CargoWiseId { get; set; }
        public string CustomerAccountNo { get; set; }
        public string CargoWiseBardCode { get; set; }
        public WorkingWeekDay WorkingWeekDay { get; set; }
        public string CompanyName { get; set; }
        public Nullable<int> ClientId { get; set; }
        public Nullable<bool> IsClient { get; set; }
        public string CountryOfOperation { get; set; }
        public string ContactName { get; set; }
        public string Email { get; set; }
        public string TelephoneNo { get; set; }
        public string MobileNo { get; set; }
        public string FaxNumber { get; set; }
        public DateTime WorkingStartTime { get; set; }
        public string startTime { get; set; }
        public string EndTime { get; set; }
        public DateTime WorkingEndTime { get; set; }
        public TimeZoneModal Timezone { get; set; }
        public string VATGST { get; set; }
        public string ShortName { get; set; }
        public string Position { get; set; }
        public string Skype { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public int CreatedByRoleId { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public FrayteAddress UserAddress { get; set; }
        public int RoleId { get; set; }
        public string CustomerRateCardType { get; set; }
        public string UserType { get; set; }
        public int LoginUserId { get; set; }
    }

    public class FrayteAddress
    {
        public int UserAddressId { get; set; }
        public int UserId { get; set; }
        public int AddressTypeId { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Suburb { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public FrayteCountryCode Country { get; set; }
        public string EasyPostAddressId { get; set; }
    }

    public class FrayteShipperReceiver : FrayteUser
    {
        public int ShipperId { get; set; }
        public List<FrayteAddress> PickupAddresses { get; set; }
    }


    public class CustomerConfigurationSetUp
    {
        public int UserId { get; set; } 
        public int CustomerCompanyDetailId { get; set; } 
        public string LogoFileName { get; set; }
        public string LogoFilePath { get; set; }
        public string SMTPHostName { get; set; }
        public string SMTPport { get; set; }
        public string SMTPUserName { get; set; }
        public string SMTPPassword { get; set; }
        public string SMTPDisplayName { get; set; }
        public string SMTPFromMail { get; set; }
        public string CompanyName { get; set; }
        public bool SMTPEnableSsl { get; set; }
        public string Website { get; set; }
        public string KindRegards { get; set; }  
        public string SiteLink { get; set; }
        public string SiteAddress { get; set; }
        public string OperationStaff { get; set; }
        public string OperationStaffPhone { get; set; }
        public string OperationStaffPhoneCode { get; set; }
        public string UserPosition { get; set; }
        public string OperationStaffEmail { get; set; }  
    }

    public class FrayteCustomer : FrayteUser
    {
        public string UserEmail { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string AccountMail { get; set; }
        public string CreditLimitCurrencyCode { get; set; }
        public decimal CreditLimit { get; set; }
        public string TermsOfPayment { get; set; }
        public string TaxAndDuties { get; set; }
        public bool IsDirectBooking { get; set; }
        public bool IsTradeLaneBooking { get; set; }
        public bool IsBreakBulkBooking { get; set; }
        public bool IsECommerce { get; set; }
        public bool IsFuelSurcharge { get; set; }
        public bool IsCurrency { get; set; }
        public bool IsWarehouseTransport { get; set; }
        public bool IsExpressSolutions { get; set; }
        public bool IsShipperTaxAndDuty { get; set; }
        public bool IsRateCardAssigned { get; set; }
        public bool IsAllowRate { get; set; }
        public bool IsApiAllow { get; set; }
        public int DaysValidity { get; set; }
        public string CustomerType { get; set; }
        public int OperationZoneId { get; set; }
        public bool IsServiceSelected { get; set; }
        public bool IsWithoutService { get; set; }
        public string FreeStorageTime { get; set; }
        public decimal FreeStorageCharge { get; set; }
        public string UserDocument { get; set; }
        public string UserDocumentDisplay { get; set; }
        public string FreeStorageChargeCurrencyCode { get; set; }
        public FrayteCustomerAssociatedUser AccountUser { get; set; }
        public FrayteCustomerAssociatedUser DocumentUser { get; set; }
        public FrayteCustomerAssociatedUser ManagerUser { get; set; }
        public FrayteCustomerAssociatedUser OperationUser { get; set; }
        public FrayteCustomerAssociatedUser SalesRepresentative { get; set; }
        public List<FrayteAddress> OtherAddresses { get; set; }
        public List<FrayteTradelane> Tradelanes { get; set; }
        public FrayteCustomerSetting CustomerPODSetting { get; set; }
        public FrayteCustomerSetting CustomerRateSetting { get; set; }
    }

    public class CustomerBasicDetail
    {
        public int CustomerId { get; set; }
        public string CustomerEmail { get; set; }
        public string AccountNumber { get; set; }
    }

    public class FrayteAgent : FrayteUser
    {
        public int UserShipmentTypeId { get; set; }
        public bool? IsAir { get; set; }
        public bool? IsSea { get; set; }
        public bool? IsExpryes { get; set; }
        public FrayteCustomerAssociatedUser AccountUser { get; set; }
        public FrayteCustomerAssociatedUser DocumentUser { get; set; }
        public FrayteCustomerAssociatedUser ManagerUser { get; set; }
        public FrayteCustomerAssociatedUser OperationUser { get; set; }
        public List<FrayteAddress> OtherAddresses { get; set; }
        public List<FrayteAgentAssociatedUser> AssociatedUsers { get; set; }
    }

    public class FrayteAgentAssociatedUser
    {
        public int AgentAssociatedUserId { get; set; }
        public int AgentId { get; set; }
        public string UserType { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string TelephoneNo { get; set; }
        public string WorkingStartTime { get; set; }
        public string WorkingEndTime { get; set; }
        public string WorkingWeekDays { get; set; }
    }

    public class FrayteCustomerAssociatedUser
    {
        public int UserId { get; set; }
        public string AssociateType { get; set; }
        public string ContactName { get; set; }
        public string Email { get; set; }
        public string TelephoneNo { get; set; }
        public string WorkingHours { get; set; }
        public string CompanyName { get; set; }
        public string Position { get; set; }
    }

    public class TrackFryateUser
    {
        public int RoleId { get; set; }
        public int CurrentPage { get; set; }
        public int TakeRows { get; set; }
    }

    public class FrayteSystemRole
    {
        public int RoleId { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool IsDefault { get; set; }
    }

    public class FrayteInternalUser : FrayteUser
    {
        public string RoleName { get; set; }
        public FrayteCustomerAssociatedUser ManagerUser { get; set; }
        public List<FrayteAssociateCustomer> AssociateCustomer { get; set; }
        public bool IsFuelSurCharge { get; set; }
        public bool IsCurrency { get; set; }
        public int TotalRows { get; set; }
        public bool IsDirectBooking { get; set; }
        public bool IsTradelaneBooking { get; set; }
        public bool IsBreakBulk { get; set; }
        public bool IsExpressBooking { get; set; }
        public bool IsECommerce { get; set; }
        public bool IsWarehouseTransport { get; set; }
    }

    public class FrayteAssociateCustomer
    {
        public int CustomerStaffDetailId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
        public string WorkingHours { get; set; }
    }

    public class FrayteUserModel
    {
        public int UserId { get; set; }
        public string Name { get; set; }
    }

    public class FrayteAgentModel
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public bool IsAir { get; set; }
        public bool IsSea { get; set; }
        public bool IsExpryes { get; set; }
    }

    public class FrayteCustomerMarginCost
    {
        public int UserId { get; set; }
        public int OperationZoneId { get; set; }
        public int CustomerMarginOptionId { get; set; }
        public string CourierCompany { get; set; }
        public string CourierCompanyDisplay { get; set; }
        public string RateType { get; set; }
        public string RateTypeDisplay { get; set; }
        public string LogisticType { get; set; }
        public string LogisticTypeDisplay { get; set; }
        public string ModuleType { get; set; }
        public bool IsChange { get; set; }
        public List<FrayteCustomerMargin> CustomerMargin { get; set; }
    }

    public class FrayteCustomerRateCard
    {
        public int UserId { get; set; }
        public int OperationZoneId { get; set; }
        public FrayteCustomerSetting CustomerRateSetting { get; set; }
        public List<FrayteRegistredServices> RegistredServices { get; set; }
    }

    public class FrayteRegistredServices
    {
        public int LogisticServiceId { get; set; }
        public string LogisticServiceType { get; set; }
    }
    public class FrayteTrackingConfiguration
    {
        public int UserTrackingConfigurationId { get; set; }
        public int CustomerId { get; set; }
        public List<FrayteEmailModel> DeliveredEmails { get; set; }
        public List<FrayteEmailModel> InTransitEmails { get; set; }
        public List<FrayteEmailModel> OutForDeliveryEmails { get; set; }
        public List<FrayteEmailModel> PendingEmails { get; set; }
        public List<FrayteEmailModel> AttemptFailEmails { get; set; }
        public List<FrayteEmailModel> InfoReceivedEmails { get; set; }
        public List<FrayteEmailModel> ExceptionEmails { get; set; }
        public List<FrayteEmailModel> ExpiredEmails { get; set; }
        public string DeliveredEmail { get; set; }
        public string InTransitEmail { get; set; }
        public string OutForDeliveryEmail { get; set; }
        public string PendingEmail { get; set; }
        public string AttemptFailEmail { get; set; }
        public string InfoReceivedEmail { get; set; }
        public string ExceptionEmail { get; set; }
        public string ExpiredEmail { get; set; }
    }

    public class FrayteEmailModel
    {
        public string Email { get; set; }
    }

    public class FrayteCustomerAdvanceRateCard
    {
        public int CustomerAdvanceMarginCostId { get; set; }
        public int CustomerId { get; set; }
        public int OperationZoneId { get; set; }
        public int LogisticServiceZoneId { get; set; }
        public int LogisticServiceWeightId { get; set; }
        public int LogisticServiceShipmentTypeId { get; set; }
        public decimal AdvancePercentage { get; set; }
        public string ModuleType { get; set; }
    }

    public class FrayteCustomerLogisticCompany
    {
        public string LogisticCompany { get; set; }
        public string LogisticCompanyDisplay { get; set; }
    }

    public class FrayteCustomerApiDetail
    {
        public string CustomerName { get; set; }
        public string APIKey { get; set; }
        public string AccountNo { get; set; }
    }

    public class FrayteSpecialCustomerCompanyDetail
    {
        public string ComapnyName { get; set; }
        public string MainWebsite { get; set; }
        public string OperationStaffName { get; set; }
        public string OperationStaffEmail { get; set; }
        public string UserPosition { get; set; }
        public string PhoneNo { get; set; }
    }

    public class FrayteCustomerMarginOptions
    {
        public int OperationZoneId { get; set; }
        public string LogisticCompany { get; set; }
        public string LogisticCompanyDisplay { get; set; }
        public List<FrayetMarginOptions> Options { get; set; }
        public List<int> OptionId { get; set; }
    }

    public class FrayetMarginOptions
    {
        public string OptionName { get; set; }
        public string OptionDisplayName { get; set; }
        public List<FrayteMarginRates> MarginRates { get; set; }
    }

    public class MarginOptions
    {
        public int CustomerMarginOptionId { get; set; }
        public int OperationZoneId { get; set; }
        public string OptionName { get; set; }
        public string OptionNameDisplay { get; set; }
        public string LogisticCompany { get; set; }
        public string LogisticCompanyDisplay { get; set; }
        public string ShipmentType { get; set; }
        public string ShipmentTypeDisplay { get; set; }
        public decimal MarginPercentage { get; set; }
    }

    public class FrayteMarginRates
    {
        public int CustomerMarginOptionId { get; set; }
        public string ShipmentType { get; set; }
        public string ShipmentTypeDisplayText { get; set; }
        public decimal MarginPercentage { get; set; }
    }

    public class FrayteCustomerMargin
    {
        public FrayteZone Zone { get; set; }
        public LogisticShipmentType ShipmentType { get; set; }
        public Nullable<decimal> PercentOfMargin { get; set; }
    }

    public class FrayteCustomerSetting
    {
        public int CustomerSettingId { get; set; }
        public string ScheduleSetting { get; set; }
        public string ScheduleType { get; set; }
        public Nullable<DateTime> ScheduleDate { get; set; }
        public string ScheduleDay { get; set; }
        public string ScheduleTime { get; set; }
        public string AdditionalMails { get; set; }
        public string ScheduleSettingType { get; set; }
        public bool IsPdf { get; set; }
        public bool IsExcel { get; set; }
        public List<FrayteCustomerSettingDetail> CustomerSettingDetail { get; set; }
    }

    public class FrayteCustomerSettingDetail
    {
        public int CustomerSettingDetailId { get; set; }
        public int CustomerSettingId { get; set; }
        public FrayteShipmentCourier CourierShipment { get; set; }
    }

    public class RatePriceValues
    {
        public string LogisticType { get; set; }
        public string ZoneName { get; set; }
        public string Description { get; set; }
        public string CourierName { get; set; }
        public decimal WeightTo { get; set; }
        public decimal? Price { get; set; }
    }

    public class FrayteCustomerModule
    {
        public bool IsDirectBooking { get; set; }
        public bool IsTradeLaneBooking { get; set; }
        public bool IsBreakBulkBooking { get; set; }
        public bool IseCommerceBooking { get; set; }
        public bool IsWarehouseAndTransport { get; set; }
        public bool IsExpresSolutions { get; set; }
    }

    public class CustomerService
    {
        public int OrderNumber { get; set; }
        public string ServiceName { get; set; }
    }

    public class FrayteUserManifest
    {
        public bool ManifestSupport { get; set; }
        public List<CustomerService> Services { get; set; }
    }

    public class FrayteCustomerBaseRate
    {
        public int ZoneId { get; set; }
        public string ZoneName { get; set; }
        public string ZoneDisplayName { get; set; }
        public string RateType { get; set; }
        public int ShipmentTypeId { get; set; }
        public string ShipmentType { get; set; }
        public string ShipmentDisplayType { get; set; }
        public string ReportShipmentDisplay { get; set; }
        public string CourierCompany { get; set; }
        public string LogisticType { get; set; }
        public string PackageType { get; set; }
        public string ParcelType { get; set; }
        public string PackageDisplayType { get; set; }
        public string ParcelDisplayType { get; set; }
        public int LogisticWeightId { get; set; }
        public decimal WeightFrom { get; set; }
        public decimal Weight { get; set; }
        public decimal Rate { get; set; }
        public decimal MarginCost { get; set; }
        public string CustomerCurrency { get; set; }
    }

    public class CustomerRate
    {
        public int UserId { get; set; }
        public int LogisticServiceId { get; set; }
        public string LogisticCompany { get; set; }
        public string LogisticType { get; set; }
        public string RateType { get; set; }
        public string FileType { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CCEmail { get; set; }
        public string BCCEmail { get; set; }
        public string MailContent { get; set; }
        public string SendingOption { get; set; }
    }

    public class CustomerSupplementryCharge
    {
        public string Rule { get; set; }
    }

    public class FrayteCustomerAdvanceMarginCost
    {
        public int CustomerAdvanceMarginCostId { get; set; }
        public FrayteZone zone { get; set; }
        public LogisticShipmentType shipmentType { get; set; }
        public FrayteLogisticWeight LogisticWeight { get; set; }
        public FrayteLogisticDimension LogisticDimension { get; set; }
        public int OperationZoneId { get; set; }
        public decimal Rate { get; set; }
        public bool IsChanged { get; set; }
        public string ModuleType { get; set; }
    }

    public class FrayteMarginPercentage
    {
        public decimal MarginPercent { get; set; }
    }

    public class FrayteCuastomerRateCard
    {
        public int OperationZoneId { get; set; }
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public string CustomerEmail { get; set; }
        public string StaffEmail { get; set; }
        public string StaffName { get; set; }
        public string Business { get; set; }
        public string SiteAddress { get; set; }
        public string SiteLink { get; set; }
        public string PhoneNumber { get; set; }
        public List<string> FileName { get; set; }
        public string SenderDept { get; set; }
    }

    public class UserDocumentFile
    {
        public bool status { get; set; }
        public string FileName { get; set; }
        public string SavedFileName { get; set; }
    }

    public class FrayteCustomerAddressBook
    {
        public int CustomerId { get; set; }
        public bool FromAddress { get; set; }
        public bool ToAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address1 { get; set; }
        public string Area { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public int CountryId { get; set; }
        public bool IsActive { get; set; }
        public string TableType { get; set; }
        public bool IsFavorites { get; set; }
        public bool IsDefault { get; set; }
    }

    public class FrayteCustomerAddressBookExcel
    {
        public bool Status { get; set; }
        public List<string> RowErrors { get; set; }
    }
}
