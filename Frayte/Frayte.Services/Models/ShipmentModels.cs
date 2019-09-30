using Frayte.Services.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Frayte.Services.Models
{
    public class FrayteShipmentEmailDetail
    {
        public Guid ShipmentEmailServiceId { get; set; }
        public int ShipmentId { get; set; }
        public DateTime? EmailSentDate { get; set; }
        public TimeSpan EmailSentTime { get; set; }
        public int ReminderIn { get; set; }
        public string RemindType { get; set; }
        public string EmailTemplateId { get; set; }
        public bool IsRepeatReminder { get; set; }
        public int EmailSentCount { get; set; }
        public int EmailSentToManagerCount { get; set; }
        public string ManagerEmailTemplateId { get; set; }
        public string TimeZoneName { get; set; }
        public TimeSpan WorkingStartTime { get; set; }
        public TimeSpan WorkingEndTime { get; set; }
        public bool IsPublicHoliday { get; set; }
    }

    public class FrayteCustomerAmendShipment
    {
        public int ShipmentId { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public int CustomerAccountNumber { get; set; }
        public FrayteUser Receiver { get; set; }
        public FrayteAddress ReceiverAddress { get; set; }
        public FrayteShipmentCourier DeliveredBy { get; set; }
        public FrayteUserRole UserRoleId { get; set; }
        public bool IsLogin { get; set; }
        public CountryShipmentPort FrayteShipmentPortOfArrival { get; set; }
    }

    public class FrayteShipment
    {
        public int ShipmentId { get; set; }
        public DateTime? OriginatingPlannedDepartureDate { get; set; }
        public string OriginatingPlannedDepartureTime { get; set; }
        public DateTime? OriginatingPlannedArrivalDate { get; set; }
        public string OriginatingPlannedArrivalTime { get; set; }
        public string CargoWiseSo { get; set; }
        public string BarCodeSo { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string TrackingNumber { get; set; }
        public string VehicleRegNo { get; set; }
        public string PaymentPartyAccountNo { get; set; }
        public string PaymentPartyTaxDutiesAccountNo { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public int CustomerAccountNumber { get; set; }
        public FrayteUser Shipper { get; set; }
        public FrayteAddress ShipperAddress { get; set; }
        public FrayteUser Receiver { get; set; }
        public FrayteAddress ReceiverAddress { get; set; }
        public ShipmentTerm ShipmentTerm { get; set; }
        public PackagingType PackagingType { get; set; }
        public SpecialDelivery SpecialDelivery { get; set; }
        public CustomInformation CustomInfo { get; set; }
        public FrayteAddress PickupAddress { get; set; }
        public string PiecesCaculatonType { get; set; }
        public List<FrayteShipmentDetail> ShipmentDetails { get; set; }
        public string ContentDescription { get; set; }
        public string Guidelines { get; set; }
        public ShipmentType ShipmentDuitable { get; set; }
        public string ShippingReference { get; set; }
        public DateTime ShippingDate { get; set; }
        public string ShippingTime { get; set; }
        public string PaymentParty { get; set; }
        public string PaymentPartyTaxDuties { get; set; }
        public Nullable<decimal> DeclaredValue { get; set; }
        public CurrencyType DeclaredCurrency { get; set; }
        public FrayteShipmentCourier DeliveredBy { get; set; }
        public bool OtherPickupAddress { get; set; }
        public string ShipmentPickupContactName { get; set; }
        public string ShipmentPickupContactPhoneNumber { get; set; }
        public string LocationType { get; set; }
        public string LocationOfShipment { get; set; }
        public string SpecialInstruction { get; set; }
        public Nullable<System.DateTime> PickupDate { get; set; }
        public string ShipmentReadyBy { get; set; }
        public string OfficeCloseAt { get; set; }
        public TimeZoneModal Timezone { get; set; }
        public string ExportType { get; set; }
        public ShipmentWarehouse Warehouse { get; set; }
        public TransportToWarehouse TransportToWarehouse { get; set; }
        public bool TermAndCondition { get; set; }
        public int TermAndConditionId { get; set; }
        public Nullable<System.Guid> CustomerConfirmCode { get; set; }
        public int TradeLaneId { get; set; }
        public string CommercialInvoice { get; set; }
        public string PackingList { get; set; }
        public string CustomDocument { get; set; }
        public string ControlNumber { get; set; }
        public string AirWayBill { get; set; }
        public string AirWayBillDocument { get; set; }
        public Nullable<System.DateTime> AnticipatedPickupDate { get; set; }
        public Nullable<System.TimeSpan> AnticipatedPickupTime { get; set; }
        public string AWB { get; set; }
        public string OtherDocs { get; set; }
        public Nullable<int> OriginatingAgentId { get; set; }
        public Nullable<int> OriginatingAddressId { get; set; }
        public Nullable<System.DateTime> OriginatingDeliveryDate { get; set; }
        public Nullable<System.TimeSpan> OriginatingDeliveryTime { get; set; }
        public Nullable<System.DateTime> WarehouseDropOffDate { get; set; }
        public Nullable<System.TimeSpan> WarehouseDropOffTime { get; set; }
        public string FlightVessel { get; set; }
        public Nullable<System.DateTime> ETDDate { get; set; }
        public Nullable<System.TimeSpan> ETDTime { get; set; }
        public Nullable<System.DateTime> ETADate { get; set; }
        public Nullable<System.TimeSpan> ETATime { get; set; }
        public string MABBL { get; set; }
        public Nullable<int> DestinatingAgentId { get; set; }
        public Nullable<System.DateTime> DestinatingDeliveryDate { get; set; }
        public Nullable<System.TimeSpan> DestinatingDeliveryTime { get; set; }
        public string DestinatingDeliveryCustomIssue { get; set; }
        public Nullable<System.DateTime> FinalDeliveryDate { get; set; }
        public Nullable<System.TimeSpan> FinalDeliveryTime { get; set; }
        public string Signature { get; set; }
        public string FinalImage { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public FrayteUserRole UserRoleId { get; set; }
        public bool IsLogin { get; set; }
        public string ShipmentPDF { get; set; }
        public string TransportToWareHouseCarrier { get; set; }
        public string OtherTransportToWareHouseCarrier { get; set; }
        public CountryShipmentPort FrayteShipmentPortOfDeparture { get; set; }
        public CountryShipmentPort FrayteShipmentPortOfArrival { get; set; }
    }

    public class FrayteShipmentPort
    {
        public int CountryShipmentPortId { get; set; }
        public int PortName { get; set; }
        public string PortType { get; set; }
        public string PortCode { get; set; }
        public int CountryId { get; set; }
    }

    public class CustomInformation
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

    public class FrayteShipmentTelex
    {
        public int ShipmentId { get; set; }
        public string FlightVessel { get; set; }
        public string MABBL { get; set; }
        public string SeaPOL { get; set; }
        public string SeaPOD { get; set; }
        public string SeaTelexDocument { get; set; }
        public string SeaTelexStamp { get; set; }
    }

    public class FrayteShipmentShipperReceiver
    {
        public int ShipmentId { get; set; }
        public string FrayteCargoWiseSo { get; set; }
        public string PurchaseOrderNo { get; set; }
        public FrayteUser Shipper { get; set; }
        public FrayteAddress ShipperAddress { get; set; }
        public FrayteUser Receiver { get; set; }
        public FrayteAddress ReceiverAddress { get; set; }
    }

    public class FraytePackageDetailExcel
    {
        public string Message { get; set; }
        public List<Package> FrayteShipmentDetail { get; set; }
    }

    public class FrayteShipmentDetailExcel
    {
        public string Message { get; set; }
        public List<FrayteShipmentDetail> FrayteShipmentDetail { get; set; }
    }

    public class FrayteShipmentDetail
    {
        public int ShipmentDetailId { get; set; }
        public int ShipmentId { get; set; }
        public string JobNumber { get; set; }
        public string JobStyle { get; set; }
        public int HSCode { get; set; }
        public int CartonQty { get; set; }
        public int Pieces { get; set; }
        public Nullable<decimal> WeightKg { get; set; }
        public Nullable<decimal> Lcms { get; set; }
        public Nullable<decimal> Wcms { get; set; }
        public Nullable<decimal> Hcms { get; set; }
        public string PiecesContent { get; set; }
    }

    public class FrayteCustomerActionShippment
    {
        public string ActionType { get; set; }
        public string ConfirmationCode { get; set; }
    }

    public class FrayteUserShipment
    {
        public int ShipmentId { get; set; }
        public string ShipmentCode { get; set; }
        public string CargoWiseSo { get; set; }
        public string Customer { get; set; }
        public string ShippedFrom { get; set; }
        public string ShippedTo { get; set; }
        public string ShipmentDetail { get; set; }
        public string ShippingType { get; set; }
        public DateTime ShippingDate { get; set; }
        public DateTime? DateOfDelivery { get; set; }
        public string Status { get; set; }
        public FrayteUserRole UserRoleId { get; set; }
    }

    public class FrayteShipmentDocument
    {
        public int ShipmentId { get; set; }
        public string DocumentType { get; set; }
        public string DocumentTitle { get; set; }
        public string DocumentPath { get; set; }
    }

    public class FrayteShipmentDropOff
    {
        public int ShipmentId { get; set; }
        public DateTime? DropOffDate { get; set; }
        public string DropOffTime { get; set; }
    }

    public class FrayteShipmentOriginatingAgentDetails
    {
        public int ShipmentId { get; set; }
        public FrayteAddress DeliveryAddress { get; set; }
        public DateTime? OriginatingPlannedDepartureDate { get; set; }
        public string OriginatingPlannedDepartureTime { get; set; }
        public DateTime? OriginatingPlannedArrivalDate { get; set; }
        public string OriginatingPlannedArrivalTime { get; set; }
    }

    public class FrayteShipmentOriginatingAgentReject
    {
        public int ShipmentId { get; set; }
        public string RejectionReason { get; set; }
    }

    public class FrayteShipmentDestinatingAgentAnticipatedDetail
    {
        public int ShipmentId { get; set; }
        public DateTime AnticipatedDeliveryDate { get; set; }
        public string AnticipatedDeliveryTime { get; set; }
        public string OtherCustomIssues { get; set; }
        public string UnexpectedClearance { get; set; }
    }

    public class FrayteShipmentFlightSeaDetail
    {
        public int ShipmentId { get; set; }
        public string FlightVessel { get; set; }
        public DateTime ETDDate { get; set; }
        public string ETDTime { get; set; }
        public DateTime ETADate { get; set; }
        public string ETATime { get; set; }
        public string MABBL { get; set; }
    }

    public class FrayteShipmentAWBDetail
    {
        public int ShipmentId { get; set; }
        public string AWB { get; set; }
        public string OtherDocs { get; set; }
    }

    public class FrayteShipmentReslectAgentModel
    {
        public int ShipmentId { get; set; }
        public string CustomerName { get; set; }
        public string ShippingMethod { get; set; }
        public FrayteCountryCode Country { get; set; }
        public FrayteUserModel Agent { get; set; }
    }

    public class FrayteShipmentReslectAgentInitials
    {
        public int ShipmentId { get; set; }
        public FrayteUserModel OriginatingAgent { get; set; }
    }

    public class FrayteShipmentReslectAgent
    {
        public int ShipmentId { get; set; }
        public FrayteUserModel OriginatingAgent { get; set; }
    }

    public class FrayteShipmentPODDetail
    {
        public int ShipmentId { get; set; }
        public DateTime PODDeliveryDate { get; set; }
        public string PODDeliveryTime { get; set; }
        public string Signature { get; set; }
        public string ExceptionNote { get; set; }
        public string SignatureImage { get; set; }
    }

    public class FrayteShipmentDetailPdf
    {
        public string PageStyleSheet { get; set; }
        public string BootStrapStyleSheet { get; set; }
        public int TotalPieces { get; set; }
        public string TotalKgs { get; set; }
        public string TotalWeight { get; set; }
        public string DeclaredValue { get; set; }
        public string pdfLogo { get; set; }
        public string TermAndConditionNew { get; set; }
        public FrayteShipment ShipmentDetail { get; set; }
    }

    public class FrayteShipmentDeliveryNotePdf
    {
        public string PageStyleSheet { get; set; }
        public string BootStrapStyleSheet { get; set; }
        public string ImgSrc { get; set; }
        public int TotalPieces { get; set; }
        public string TotalKgs { get; set; }
        public string TotalWeight { get; set; }
        public string DeclaredValue { get; set; }
        public FrayteShipment ShipmentDetail { get; set; }
        public FrayteAgent Agent { get; set; }
    }
    
    public class EasyPostTracking
    {
        public FrayteCourier Carrier { get; set; }
        public string TrackingCode { get; set; }
    }

    public class FrayteCoLoaderBookingFormPdf
    {
        public string PageStyleSheet { get; set; }
        public string BootStrapStyleSheet { get; set; }
        public string PDFLogo { get; set; }
        public string ShipperName { get; set; }
        public string ShiiperTelephoneNo { get; set; }
        public string ShipperFaxNo { get; set; }
        public string OriginatingAirPort { get; set; }
        public string DestinatingAirPort { get; set; }
        public int NoOfPacktes { get; set; }
        public string GrossWeight { get; set; }
        public string DescriptionOfGoods { get; set; }
        public string TotalCBM { get; set; }
        public string SpecifyCurrency { get; set; }
        public string DeclaredValue { get; set; }
        public string CountryOfOrigin { get; set; }
        public string AirLine { get; set; }
        public string SpecialInstruction { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverTelephoneNo { get; set; }
        public string Notifyparty { get; set; }
        public string MAWBNO { get; set; }
    }

    public class FrayteEasyPostShipment
    {
        public int ShipmentId { get; set; }
        public string ShipmentServiceType { get; set; }
    }
}
