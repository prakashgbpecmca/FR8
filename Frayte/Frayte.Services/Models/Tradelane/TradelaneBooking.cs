﻿using Frayte.Services.DataAccess;
using Frayte.Services.Models.BreakBulk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.Tradelane
{

    public class TradelanePrintLabelResponse
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
    }

    public class TradelanePrintLabelResponseModel
    {
        public HttpResponseMessage ByteArray { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
    }

    public class TradelaneMAWBShipmentDocument
    {
        public string FileName { get; set; }
        public int RevisionNumber { get; set; }
    }

    public class TradelaneBookingDetail : TradelaneBooking
    {
        public List<HAWBPackageDetail> HAWBS { get; set; }
    }

    public class HAWBPackageDetail
    {
        public string HAWB { get; set; }
        public int CartonQty { get; set; }
        public decimal TotalWeight { get; set; }
    }

    public class TradelaneMappedUnMappedShipment
    {
        public int TotalShipments { get; set; }
        public int HAWBNumber { get; set; }
        public int AllocatedShipments { get; set; }
        public int UnAllocatedShipments { get; set; }
    }

    public class TradelaneBooking
    {
        public int TradelaneShipmentId { get; set; }
        public int OperationZoneId { get; set; }
        public int CustomerId { get; set; }
        public int HAWBNumber { get; set; }
        public string CustomerAccountNumber { get; set; }
        public int ShipmentStatusId { get; set; }
        public List<TradelanePackage> Packages { get; set; }
        public List<HAWBTradelanePackage> HAWBPackages { get; set; }
        public TradelBookingAdress ShipFrom { get; set; }
        public TradelBookingAdress ShipTo { get; set; }
        public string ShipperAdditionalNote { get; set; }
        public string ReceiverAdditionalNote { get; set; }
        public TradelBookingAdress NotifyParty { get; set; }
        public string NotifyPartyAdditionalNote { get; set; }
        public bool IsNotifyPartySameAsReceiver { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public string CreatedOnTime { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOnUtc { get; set; }
        public string FrayteNumber { get; set; }
        public string PakageCalculatonType { get; set; }
        public string LogisticType { get; set; }
        public TradelaneAirport DepartureAirport { get; set; }
        public TradelaneAirport DestinationAirport { get; set; }
        public TradelaneShipmentHandlerMethod ShipmentHandlerMethod { get; set; }
        public string ShipmentReference { get; set; }
        public string ShipmentDescription { get; set; }
        public decimal DeclaredValue { get; set; }
        public CurrencyType DeclaredCurrency { get; set; }
        public TradelaneAirline AirlinePreference { get; set; }
        public decimal TotalEstimatedWeight { get; set; }
        public decimal? DeclaredCustomValue { get; set; }
        public decimal? InsuranceAmount { get; set; }
        public string CertificateOfOrigin { get; set; }
        public string ExportLicenceNo { get; set; }
        public string PayTaxAndDuties { get; set; }
        public string TaxAndDutiesAccountNo { get; set; }
        public string TaxAndDutiesAcceptedBy { get; set; }
        public string CustomsSigner { get; set; }
        public bool? DangerousGoods { get; set; }
        public string MAWB { get; set; }
        public int? MAWBAgentId { get; set; }
        public string BatteryDeclarationType { get; set; }
        public string ManifestName { get; set; }
        public FratyteError Error { get; set; }
        public string AdditionalInfo { get; set; }
        public FrayteIncoterm Incoterm { get; set; }
    }

    public class TradelaneCustomer
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CompanyName { get; set; }
        public string AccountNumber { get; set; }
        public string EmailId { get; set; }
        public int OperationZoneId { get; set; }
        public int ValidDays { get; set; }
        public string CustomerCurrency { get; set; }
        public int OrderNumber { get; set; }
        public bool IsShipperPayTaxAndDuty { get; set; }
        public string DefaultShipmentType { get; set; }
    }

    public class TradelaneCustomerDefaultAddress
    {
        public TradelBookingAdress ShipFrom { get; set; }
        public TradelBookingAdress ShipTo { get; set; }
    }

    public class TradelBookingAdress
    {
        public int TradelaneShipmentAddressId { get; set; }
        public int TradelaneShipmentDetailId { get; set; }
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
    }

    public class TradelanePackageDetailExcel
    {
        public int TotalUploaded { get; set; }
        public int SuccessUploaded { get; set; }
        public string Message { get; set; }
        public List<TradelanePackage> Packages { get; set; }
    }

    public class TradelanePackageWeightObj
    {
        public decimal TotalWeight { get; set; }
        public decimal TotalCBM { get; set; }
        public decimal TotalGrossWeight { get; set; }
        public decimal TotalEstimatedWeight { get; set; }
        public decimal TotalChargeableWeight { get; set; }
    }

    public class TrackTradelanePackage : TradelanePackage
    {
        public int TotalRows { get; set; }
    }

    public class HAWBTradelanePackage
    {
        public int TradelaneShipmentId { get; set; }
        public string HAWB { get; set; }
        public int? HAWBNumber { get; set; }
        public decimal TotalVolume { get; set; }
        public int TotalCartons { get; set; }
        public decimal EstimatedWeight { get; set; }
        public decimal TotalWeight { get; set; }
        public int PackagesCount { get; set; }
        public string SONumber { get; set; }
        public List<TradelanePackage> Packages { get; set; }
    }

    public class TradelanePackage
    {
        public int TradelaneShipmentDetailId { get; set; }
        public int TradelaneShipmentId { get; set; }
        public string CartonNumber { get; set; }
        public int CartonValue { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public string HAWB { get; set; }
        public bool? IsScanned { get; set; }
        public string PackageCalculatonType { get; set; }
    }

    public class FrayteHAWBAddress
    {
        public TradelBookingAdress ShipFrom { get; set; }
        public TradelBookingAdress ShipTo { get; set; }
        public TradelBookingAdress NotifyParty { get; set; }
        public bool? IsNotifyPartySameAsReceiver { get; set; }
    }

    public class ValidateHAWBAddress
    {
        public int ShipToAddressId { get; set; }
        public int ShipFromAddressId { get; set; }
        public int NotifyPartyAddressId { get; set; }
        public int TradeLaneShipmentDetailId { get; set; }
        public bool? IsNotifyPartySameAsReceiver { get; set; }
    }

    public class TrackPackage
    {
        public int ShipmentId { get; set; }
        public string HAWB { get; set; }
        public string Type { get; set; }
        public int CurrentPage { get; set; }
        public int TakeRows { get; set; }
    }

    public class TradelaneAirport
    {
        public int AirportCodeId { get; set; }
        public string AirportCode { get; set; }
        public string AirportName { get; set; }
        public int CountryId { get; set; }
    }

    public class TradelaneAirline
    {
        public int AirlineId { get; set; }
        public string AirLineName { get; set; }
        public string AilineCode { get; set; }
        public string CarrierCode2 { get; set; }
        public string CarrierCode3 { get; set; }
    }

    public class TradelaneShipmentHandlerMethod
    {
        public int ShipmentHandlerMethodId { get; set; }
        public string ShipmentHandlerMethodName { get; set; }
        public string ShipmentHandlerMethodDisplay { get; set; }
        public string ShipmentHandlerMethodType { get; set; }
        public string ShipmentHandlerMethodCode { get; set; }
        public string DisplayName { get; set; }
    }

    public class TradelaneFile
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public int TradelaneShipmentId { get; set; }
    }

    public class TradelaneFileStatus
    {
        public string FileName { get; set; }
        public int TradelaneShipmentId { get; set; }
    }

    public class ApiErrorObj
    {
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class TradelaneApiErrorModel
    {
        public bool Status { get; set; }
        public List<ApiErrorObj> ErrorObject { get; set; }
    }

    public class TradelaneApiScanModel : TradelaneApiErrorModel
    {
        public string HawbNumber { get; set; }
        public string FrayteNumber { get; set; }
        public string Mawb { get; set; }
        public string CartonNumber { get; set; }
        public int TotalCarton { get; set; }
        public int ScanedCarton { get; set; }
        public Decimal Length { get; set; }
        public Decimal Width { get; set; }
        public Decimal Weight { get; set; }
        public Decimal Height { get; set; }
        public string CartonValue { get; set; }
    }

    public class TradelanePrintLabel
    {
        public List<string> CartonList { get; set; }
        public string Mawb { get; set; }
        public string Hawb { get; set; }
    }

    public class TradelaneDetail
    {
        public int TradelaneShipmentId { get; set; }
        public int TradelaneShipmentDetailId { get; set; }
    }

    public class CartonType
    {
        public string CartonNumber { get; set; }
        public bool IsSCanned { get; set; }
    }

    public class TradelaneHawbStatusModel
    {
        public string Hawb { get; set; }
        public List<CartonType> CartonList { get; set; }
    }

    public class TradelaneScannedStatusModel : TradelaneApiErrorModel
    {
        public List<TradelaneHawbStatusModel> HawbList { get; set; }
        public int MissingScanCount { get; set; }
        public int ScannedCount { get; set; }
    }

    public class TradelaneUpdateStatusModel : TradelaneApiErrorModel
    {
        public string Mawb { get; set; }
        public string StatusCode { get; set; }
        public string Description { get; set; }
        public string Airport { get; set; }
    }

    public class TradelaneUpdateHawbModel : TradelaneApiErrorModel
    {
        public int TotalHawbCarton { get; set; }
        public int TotalHawb { get; set; }
        public int PendingHawb { get; set; }
    }


    /// <summary>
    /// Hawb Label 
    /// </summary>
    public class HawbLabel
    {
        public int TradelaneShipmentId { get; set; }
        public string MAWBBarcode { get; set; }
        public string MAWBBarcodeDisplay { get; set; }
        public string MAWB { get; set; }
        public string DepartureAirportCode { get; set; }
        public string DestinationAirportCode { get; set; }
        public string HAWB { get; set; }
        public string HAWBBarCode { get; set; }
        public string HAWBBarCodeDisplay { get; set; }
        public int TotalCarton { get; set; }
        public int HAWBPCS { get; set; }
    }

    public class FrayteHAWBDetail
    {
        public int TradelaneShipmentDetailId { get; set; }
        public int TradelaneShipmentId { get; set; }
        public int CartonValue { get; set; }
        public string CartonNumber { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public string HAWB { get; set; }       
    }





}