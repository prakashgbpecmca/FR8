using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public enum FrayteAddressType
    {
        MainAddress = 1,
        OtherAddress = 2
        //Agent = 1,//AddressTypeId=1
        //Billing,//AddressTypeId=2
        //Customer,//AddressTypeId=3
        //Consignment,//AddressTypeId=4
        //Factory,//AddressTypeId=5
        //HeadOffice,//AddressTypeId=6
        //Pickup,//AddressTypeId=7
        //Receiver,//AddressTypeId=8
        //Shipper,//AddressTypeId=9
        //Warehouse,//AddressTypeId=10
        //AgentDelivery,
        //AgentOther,
        //CustomerOther
    }

    public enum FrayteBusinessUnit
    {
        HKG = 1,
        UK = 2
    }

    public static class FrayteApplicationMode
    {
        public const string Test = "Test";
        public const string Live = "Live";
    }

    public class FrayteCustomerTypeEnum
    {
        public const string SPECIAL = "SPECIAL";
        public const string NORMAL = "NORMAL";
    }

    public static class ModuleLevelUpdateCase
    {
        public const string RoleModuleLevel = "RoleModuleLevel";
        public const string RoleModuleDetailLevel = "RoleModuleDetailLevel";
    }

    public static class FrayteFromToAddressType
    {
        public const string FromAddress = "FromAddress";
        public const string NotifyPartyAddress = "NotifyPartyAddress";
        public const string ToAddress = "ToAddress";
        public const string AllAddress = "All";
    }

    public enum FrayteUserRole
    {
        Admin = 1, //RoleId = 1
        Agent, //RoleId = 2
        Customer, //RoleId = 3
        Receiver, //RoleId = 4
        Shipper, //RoleId = 5
        Staff, //RoleId = 6
        Warehouse, //RoleId = 7
        HSCodeOperator, //RoleId = 8
        MasterAdmin, //RoleId = 9
        WarehouseAgent, //RoleId = 10
        HSCodeOperatorManager, //RoleId = 11
        CallCenterOperator, //RoleId = 12
        CallCenterManger, //RoleId = 13
        Accountant, //RoleId = 14
        Consolidator, //RoleId = 15
        PreAlertAndTracking, //RoleId = 16
        UserCustomer, //RoleId = 17
        MobileUser, //RoleId = 19
        CustomerStaff = 20, //RoleId = 20
        FactoryUser //RoleId = 21
    }

    public static class FrayteFileExtension
    {
        public const string EXCEL = "EXCEL";
        public const string CSV = "CSV";
    }

    public enum FrayteShipmentStatus
    {
        NewBooking = 1,//ShipmentStatusId=1
        CustomerConfirm,//ShipmentStatusId=2
        CustomerAmended,//ShipmentStatusId=3
        CustomerReject,//ShipmentStatusId=4
        CommericalInvoiceUploaded,//5
        AgentConfirm,//6
        AgentReject,//7
        UpdateFlightSea,//8
        AWBUploaded,//9
        DestinatingAgentAnticipated,//10
        Close,//11
        Current,//12
        Past,//13
        Draft,//14
        Delayed,//15
        Cancel,//16
        eCCurrent,//17
        eCPast,//18
        eCDraft,//19
        eCDelayed,//20
        eCCancel,//21
        eCDepartured,//22
        eCHandOver,//23
        Delivered,//24
    }

    public enum FrayteeCommerceShipmentStatus
    {
        Current = 17,
        Past,//18
        Draft,//19
        Delayed,//20
        Cancel//21
    }

    public enum FrayteTradelaneShipmentStatus
    {
        Draft = 27,
        ShipmentBooked,//28
        Pending,//29
        Departed,//30
        InTransit,//31
        Arrived,//32
        Rejected = 34,
        Delivered = 35,
    }

    public enum FrayteExpressShipmentStatus
    {
        Scanned = 37,
        Current,
        Draft,
        Departed,
        Delivered,
        InTransit,
        HubReceived
    }

    public static class FrayteReportSettingDays
    {
        public const string Daily = "Daily";
        public const string Weekly = "Weekly";
        public const string Monthly = "Monthly";
        public const string Yearly = "Yearly";
        public const string PerShipment = "PerShipment";
        public const string Scheduled = "Scheduled";
    }

    public static class FrayteBookingStatusType
    {
        public const string Draft = "Draft";
        public const string Current = "Current";
    }

    public static class FraytePakageCalculationType
    {
        public const string kgtoCms = "kgToCms";
        public const string LbToInchs = "lbToInchs";
    }

    public static class FrayteCustomerPODRateSetting
    {
        public const string POD = "POD";
        public const string RateCard = "RateCard";
    }

    public static class ShipmentCustomerAction
    {
        public const string Confirm = "c";
        public const string Reject = "r";
    }

    public static class FrayteExportType
    {
        public const string Export = "Export";
        public const string Import = "Import";
    }

    public static class ShipmentRateServiceType
    {
        public const string ExpressWorldwideNonDoc = "ExpressWorldwideNonDoc";
        public const string MedicalExpressNonDoc = "MedicalExpressNonDoc";
    }

    public static class FrayteShipmentType
    {
        public const string Air = "Air";
        public const string Courier = "Courier";
        public const string Expryes = "Expryes";
        public const string Sea = "Sea";
        public const string Doc = "Doc";
        public const string Nondoc = "Nondoc";
        public const string DocAndNondoc = "Doc&Nondoc";
        public const string HeavyWeight = "HeavyWeight";
        public const string WeightType = "Heavy Weight";
    }

    public static class FrayteAssociateType
    {
        public const string Account = "Account";
        public const string Document = "Document";
        public const string Manager = "Manager";
        public const string Operation = "Operation";
    }

    public static class FrayteShipmentDocumentType
    {
        public const string MAWB = "MAWB";
        public const string MAWBDisplay = "Master Air Way Bill - MAWB";
        public const string HAWB = "HAWB";
        public const string HAWBDisplay = "House Air Way Bill - HAWB";
        public const string Manifest = "Manifest";
        public const string ManifestDisplay = "Manifest";
        public const string ShipmentDetail = "ShipmentDetail";
        public const string ShipmentDetailDisplay = "Shipment Detail";
        public const string BatteryForm = "BatteryForm";
        public const string BatteryFormDisplay = "Batter Declaration";
        public const string CoLoadForm = "CoLoadForm";
        public const string CoLoadFormDisplay = "Coload Form";
        public const string OtherDocument = "OtherDocument";
        public const string OtherDocumentDisplay = "Other Document";
        public const string CommercialInvoice = "CommercialInvoice";
        public const string AirWayBill = "AirWayBill";
        public const string TelexDocument = "TelexDocument";
    }

    public static class FrayteTradelaneShipmentDocumentEnum
    {
        public const string REV = "REV";
        public const string MAWB = "MAWB";
        public const string MAWBDisplay = "Master Air Way Bill - MAWB";
        public const string HAWB = "HAWB";
        public const string HAWBDisplay = "House Air Way Bill - HAWB";
        public const string Manifest = "Manifest";
        public const string ManifestDisplay = "Manifest";
        public const string ShipmentDetail = "ShipmentDetail";
        public const string ShipmentDetailDisplay = "Shipment Detail";
        public const string BatteryForm = "BatteryForm";
        public const string BatteryFormDisplay = "Batter Declaration";
        public const string CoLoadForm = "CoLoadForm";
        public const string CoLoadFormDisplay = "Coload Form";
        public const string OtherDocument = "OtherDocument";
        public const string OtherDocumentDisplay = "Other Document";
        public const string CartonLabel = "CartonLabel";
        public const string CartonLabelDisplay = "Carton Label";

        public const string BagNumber = "BagNumber";
        public const string BagNumberDisplay = "Bag Number";
        public const string BagLabel = "BagLabel";
        public const string BagLabelDisplay = "Bag Label";
        public const string DriverManifest = "DriverManifest";
        public const string DriverManifestDisplay = "Driver Manifest";
        public const string ExportManifest = "ExportManifest";
        public const string ExportManifestDisplay = "Export Manifest";
    }

    public static class WeightUOM
    {
        public const string KG = "KG";
        public const string LB = "LB";
    }

    public static class FrayteDocumentPath
    {
        public const string ShipmentDocument = "~/UploadFiles/Shipments/{0}/";
    }

    public static class FrayteTradelaneDocumentPath
    {
        public const string ShipmentDocument = "~/UploadFiles/Tradelane/{0}/";
    }

    public static class EmailTempletType
    {
        public const string E1 = "E1";
        public const string E2 = "E2";
        public const string E3 = "E3";
        public const string E3_1 = "E3_1";
        public const string E4 = "E4";
        public const string E4_1 = "E4_1";
        public const string E5 = "E5";
        public const string E5_1 = "E5_1";
        public const string E6 = "E6";
        public const string E6_1 = "E6_1";
        public const string E6_2 = "E6_2";
        public const string E6_3 = "E6_3";
        public const string E6_4 = "E6_4";
        public const string E6_5 = "E6_5";
        public const string E6_6 = "E6_6";
        public const string E7 = "E7";
        public const string E7_1 = "E7_1";
        public const string E8 = "E8";
        public const string E8_1 = "E8_1";
        public const string E9 = "E9";
        public const string E9_1 = "E9_1";
        public const string E10 = "E10";
        public const string E10_1 = "E10_1";
        public const string E11 = "E11";
        public const string E12 = "E12";
        public const string E13 = "E13";
        public const string E13_1 = "E13_1";
        public const string E14 = "E14";
        public const string E15 = "E15";
        public const string E16 = "E16";
    }

    public static class EmailReminderType
    {
        public const string After = "AFTER";
        public const string Before = "BEFORE";
    }

    public static class FrayteTestType
    {
        public const string ShipmentBagDetail = "ShipmentBagDetail";
    }

    public static class FrayteParcelHubTrackingDetailHide
    {
        public const string Awaiting = "Awaiting Collection";
        public const string Received = "Not Received";
        public const string CustomerStatus = "Parcel(s) not Received from Customer";
        public const string DateFailure = "Date Failure";
        public const string Deleted = "Shipment Deleted";
    }

    public static class FrayteLogisticType
    {
        public const string Import = "Import";
        public const string Export = "Export";
        public const string ThirdParty = "ThirdParty";
        public const string UKShipment = "UKShipment";
        public const string EUImport = "EUImport";
        public const string EUExport = "EUExport";
        public const string AUShipment = "AUShipment";
        public const string SkyPostalBrazilShipment = "SkyPostalBrazilShipment";
        public const string SkyPostalMexicoShipment = "SkyPostalMexicoShipment";
        public const string SkyPostalChileShipment = "SkyPostalChileShipment";
        public const string EAMShipment = "EAMShipment";
    }

    public static class FrayteLogisticServiceType
    {
        public const string UkMail = "UKMail";
        public const string Yodel = "Yodel";
        public const string Hermes = "Hermes";
        public const string DHL = "DHL";
        public const string TNT = "TNT";
        public const string UPS = "UPS";
        public const string DPD = "DPD";
        public const string AU = "AU";
        public const string SKYPOSTAL = "SKYPOSTAL";
        public const string DPDCH = "DPDCH";
        public const string DomesticB = "Domestic-B";
        public const string DomesticA = "Domestic-A";
        public const string EAMExpress = "EAM Express";
    }

    public static class FrayteLogisticServiceDisplayType
    {
        public const string UKMail = "UK Mail";
    }

    public static class FrayteShipmentServiceType
    {
        public const string DirectBooking = "DirectBooking";
        public const string TradeLaneBooking = "TradeLaneBooking";
        public const string BreakBulkBooking = "BreakBulkBooking";
        public const string ExpressBooking = "ExpressBooking";
        public const string eCommerce = "eCommerce";
        public const string Express = "Express";
        public const string BreakBulk = "BreakBulk";

        public const string DirectBookingDisplay = "Direct Booking";
        public const string TradeLaneBookingDisplay = "TradeLane Booking";
        public const string BreakBulkBookingDisplay = "BreakBulk Booking";
        public const string ExpressBookingDisplay = "Express Booking";
        public const string eCommerceDisplay = "eCommerce";
        public const string Quotation = "Quotation";
    }

    public static class DirectBookingShippingStatus
    {
        public const int Current = 12;
        public const int Past = 13;
        public const int Draft = 14;
        public const int Cancel = 16;
    }

    public static class SKYPostalMerchantEmail
    {
        public const string BrazilMerchantEmail = "EMAIL_BR@FRAYTE.COM";
        public const string MexicoMerchantEmail = "EMAIL_MX@FRAYTE.COM";
        public const string ChileMerchantEmail = "";
    }

    public static class SKYPostalLogisticType
    {
        public const string SkyPostalBrazil = "SkyPostalBrazil";
        public const string SkyPostalMexico = "SkyPostalMexico";
        public const string SkyPostalChile = "SkyPostalChile";
    }

    public static class FrayteCourierCompany
    {
        public const string DHL = "DHL";
        public const string FedEx = "FedEx";
        public const string TNT = "TNT";
        public const string UKMail = "UKMail";
        public const string Yodel = "Yodel";
        public const string Hermes = "Hermes";
        public const string UK_EU = "UK/EU - Shipment";
        public const string DHLExpress = "DHLExpress";
        public const string TNTExpress = "TNTExpress";
        public const string DHL_Express = "DHL Express";
        public const string UPS = "UPS";
        public const string DPD = "DPD";
        public const string AU = "AU";
        public const string SKYPOSTAL = "SKYPOSTAL";
        public const string EAM = "EAM";
        public const string EAMDHL = "EAM-DHL";
        public const string EAMTNT = "EAM-TNT";
        public const string EAMFedEx = "EAM-FedEx";
        public const string DPDCH = "DPDCH";
        public const string BRING = "BRING";
        public const string CANADAPOST = "CANADAPOST";
        public const string FrayteDomesticJP = "Frayte Domestic-JP";
        public const string DomesticB = "Domestic-B";
        public const string DomesticA = "Domestic-A";
        public const string EAMExpress = "EAM Express";
        public const string USPS = "USPS";
    }

    public static class FrayteShortName
    {
        public const string Frayte = "FRT";
        public const string DHL = "DHL";
        public const string TNT = "TNT";
        public const string UKMail = "UKM";
        public const string Yodel = "YDL";
        public const string Hermes = "HRM";
        public const string UPS = "UPS";
        public const string DPD = "DPD";
        public const string AU = "AU";
        public const string SKYPOSTAL = "SKYPOSTAL";
        public const string EAMExpress = "EAM Express";
        public const string DomesticA = "Domestic-A";
        public const string DPDCH = "DPDCH";
        public const string BRING = "BRING";
        public const string CANADAPOST = "CANADAPOST";
        public const string FrayteDomesticJP = "Frayte Domestic-JP";
        public const string FrayteDomesticSG = "Frayte Domestic-SG";
        public const string USPS = "USPS";
    }

    public static class FrayteTermsOfTrade
    {
        public const string Receiver = "Receiver";
        public const string Shipper = "Shipper";
        public const string ThirdParty = "ThirdParty";
    }

    public static class FrayteCatagoryOfItem
    {
        public const string Sold = "Sold";
        public const string NotSold = "Not Sold";
        public const string PersonalEffects = "Personal Effects";
        public const string Gift = "Gift";
        public const string Documents = "Documents";
        public const string CommercialSample = "Commercial Sample";
        public const string ReturnedGoods = "Returned Goods";
        public const string Other = "Other";
    }

    public static class FrayteCustomerMailSetting
    {
        public const string POD = "POD";
        public const string RateCard = "RateCard";
        public const string Scheduled = "Scheduled";
    }

    public static class FrayteUpdateDirectShipmentStatus
    {
        public const string Delivered = "Deliverd";
        public const string Delayed = "Delayed";
    }

    public static class FrayteCourierAccountCode
    {
        public const string DHL = "DHL";
        public const string UKMail = "UKMail";
        public const string Yodel = "Yodel";
        public const string Hermes = "Hermes";
    }

    public static class FrayteOperationZoneExchangeType
    {
        public const string Sell = "Sell";
        public const string Buy = "Buy";
    }

    public static class FrayteCallingType
    {
        public const string ShipmentDraft = "ShipmentDraft";
        public const string FrayteApiDraft = "FrayteApiDraft";
        public const string ShipmentClone = "ShipmentClone";
        public const string ShipmentReturn = "ShipmentReturn";
        public const string ShipmentQuotation = "quotation-shipment";
        public const string FrayteApi = "FrayteApi";
    }

    public static class FrayteTableType
    {
        public const string DirectBooking = "DirectBooking";
        public const string AddressBook = "AddressBook";
    }

    public static class FrayteLogisticRateType
    {
        public const string Saver = "Saver";
        public const string Express = "Express";
        public const string Economy = "Economy";
        public const string Expedited = "Expedited";
        public const string SaverLanes = "SaverLanes";
        public const string ExpressLanes = "ExpressLanes";
    }

    public static class FrayteCustomerBaseRateFileType
    {
        public const string Pdf = "PDF";
        public const string Excel = "EXCEL";
        public const string Email = "EMAIL";
        public const string Summery = "SUMMERY";
    }

    public static class FrayteOperationZoneName
    {
        public const string HKG = "HKG";
        public const string UK = "UK";
    }

    public static class FrayteOperationCurrency
    {
        public const string HKD = "HKD";
        public const string GBP = "GBP";
    }

    public static class FrayteOperationFullName
    {
        public const string HK = "Hong Kong";
        public const string UK = "United Kingdom";
    }

    public static class eCommerceAWBLabel
    {
        public const string ECN = "ECN";
        public const string DDP = "DDP";
        public const string DDU = "DDU";
    }

    public static class FrayteCustomerRateCardType
    {
        public const string NORMAL = "NORMAL";
        public const string ADVANCE = "ADVANCE";
    }

    public static class FraytePickUpDay
    {
        public const string Sunday = "Sunday";
        public const string Saturday = "Saturday";
    }

    public static class FrayteEasyPostService
    {
        public const string DomesticExpress = "DomesticExpress";
        public const string ExpressWorlwide = "ExpressWorldwideNonDoc";
        public const string ExpressWorldwideECX = "ExpressWorldwideECX";
    }

    public static class EncriptionKey
    {
        public const string PrivateKey = "IRAS-Soft-Techno";
    }

    public static class FrayteShippingPaymentType
    {
        public const string Shipper = "Shipper";
        public const string Receiver = "Receiver";
        public const string ThirdParty = "ThirdParty";
    }

    public static class FrayteLogisticOptionServices
    {
        public const string NDS = "NDS";
    }

    public static class FrayteUserType
    {
        public const string NORMAL = "NORMAL";
        public const string SPECIAL = "SPECIAL";
    }

    public static class TradelaneStatus
    {
        public const string Booked = "BKD";
        public const string Departed = "DEP";
        public const string Delivered = "DLV";
        public const string Arrived = "ARV";
        public const string Intransit = "INT";
        public const string WarehouseDeparted = "WHD"; //Mobile Api Status "Departed from Warehouse"
        public const string AirportArrived = "ADP"; //Mobile Api Status "Arrived at Origin Airport"
        public const string AirportDeparture = "ADT"; //Mobile Api Status "Arrived at Destination Airport"
        public const string ShipmentReceived = "RCB"; //Mobile Api Status "Shipment Received at Hub"
    }

    public class ParcelHubAddressType
    {
        public const string Residential = "Residential";
        public const string Business = "Business";
        public const string Parcelshop = "Parcelshop";
    }

    public class ModuleNumber
    {
        public const string DBK = "001";
        public const string TDL = "002";
        public const string EXS = "003";
        public const string BBK = "004";
    }
}