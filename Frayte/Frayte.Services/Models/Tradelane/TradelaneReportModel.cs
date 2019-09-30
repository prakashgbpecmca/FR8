using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.Tradelane
{
    public class TradelaneReportHAWBModel
    {

        public string HAWB { get; set; }
        public decimal TotalVolume { get; set; }
        public string VolumeUnit { get; set; }
        public int TotalCartons { get; set; }
        public decimal EstimatedWeight { get; set; }
        public decimal TotalWeight { get; set; }
        public string WeightUnit { get; set; }
        public string DimensionUnit { get; set; }
        public TradelaneBooking ShipmentDetail { get; set; }
    }

    public class ShipmentMAWBAsHAWBModel
    {
        public int TradelaneShipmentId { get; set; }
        public string MAWB { get; set; }
    }

    public class TradelaneReportMAWBModel
    {

        public string MAWB { get; set; }
        public decimal TotalVolume { get; set; }
        public string VolumeUnit { get; set; }
        public int TotalCartons { get; set; }
        public decimal EstimatedWeight { get; set; }
        public decimal TotalWeight { get; set; }
        public string WeightUnit { get; set; }
        public string DimensionUnit { get; set; }
        public TradelaneBooking ShipmentDetail { get; set; }
    }


    public class TradelaneManifestReport
    {
        public string PrintedBy { get; set; }
        public string Console { get; set; }
        public string MAWB { get; set; }
        public string AirlineCode { get; set; }
        public string FrayteNumber { get; set; }
        public DateTime CreatedOn { get; set; }
        public int TotalPackages { get; set; }
        public int TotalCartons { get; set; }
        public int TotalShipments { get; set; }
        public string TotalWeight { get; set; }
        public string TotalVolume { get; set; }
        public string ExportAgent { get; set; }
        public string ImportAgent { get; set; }
        public string Arrival { get; set; }
        public string FlightAndDate { get; set; }
        public string Loading { get; set; }
        public string DisCharge { get; set; }
        public DateTime? ETA { get; set; }
        public DateTime? ETD { get; set; }
        public List<TradelaneManifestReportDetail> ShipmentDetail { get; set; }
    }

    public class TradelaneManifestReportDetail
    {
        public string HAWB { get; set; }
        public string BillNumbers { get; set; }
        public string TotalWeight { get; set; }
        public string TotalVolume { get; set; }
        public string TotalCarton { get; set; }
        public string Shipper { get; set; }
        public string Receiver { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string ShipmentDescription { get; set; }
    }
    public class TradelaneBookingCoLoadFormModel
    {
        public string FrayteNumber { get; set; }
        public string ColoadTitle { get; set; }
        public string ShipperAddress { get; set; }
        public string ConsigneeAddress { get; set; }
        public string CTCPerson { get; set; }
        public int OperationZoneId { get; set; }
        public string ShipperPhoneNo { get; set; }
        public string ConsigneePhoneNo { get; set; }
        public string FaxNo { get; set; }
        public string NotifyPartyAddress { get; set; }
        public string NotifyPartyPhoneNo { get; set; }
        public string CagroReadyDate { get; set; }
        public string DepartureAirport { get; set; }
        public string MawbNo { get; set; }
        public string DestinationAirport { get; set; }
        public string SpecialInstruction { get; set; }
        public string ExportLicenceNo { get; set; }
        public string AirLine { get; set; }
        public string TotalPackages { get; set; }
        public string ShipmentDescription { get; set; }
        public string GrossWeight { get; set; }
        public string Volume { get; set; }
        public string CopyrightText { get; set; }
    }

    public class TradelaneBookingReportHAWB
    {
        public string HAWB { get; set; }
        public string HAWBBarCode { get; set; }
        public string MAWB { get; set; }
        public string MAWBCode { get; set; }
        public string MAWBWithCode { get; set; }
        public string MAWBCountryCode { get; set; }
        public string ShipperAddress { get; set; }
        public string ShipperAccountNumber { get; set; }
        public string ConsigneeAddress { get; set; }
        public string ConsigneeAccountNumber { get; set; }
        public string IssuedBy { get; set; }
        public string CarrierAgent { get; set; }
        public string NotifyParty { get; set; }
        public string AgentIATACode { get; set; }
        public string AccountNo { get; set; }
        public string Reference { get; set; }
        public string OptionalShippingInformation { get; set; }
        public string AirportofDeparture { get; set; }
        public string DestinationAirport { get; set; }
        public string DestinationAirportCode { get; set; }
        public string ByFirstCarrier { get; set; }
        public string CurencyCode { get; set; }
        public string CHGSCode { get; set; }
        public string WTVALPPD { get; set; }
        public string WTVALCOLL { get; set; }
        public string OtherPPD { get; set; }
        public string OtherCOLL { get; set; }
        public decimal DeclaredValueForCarriage { get; set; }
        public decimal DeclaredValueForCustoms { get; set; }
        public decimal AmountOfInsurance { get; set; }

        public decimal TotalVolume { get; set; }
        public string VolumeUnit { get; set; }
        public int TotalCartons { get; set; }
        public decimal EstimatedWeight { get; set; }
        public decimal TotalWeight { get; set; }
        public string WeightUnit { get; set; }
        public string DimensionUnit { get; set; }
        public string ShipmentDescription { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedOnCountry { get; set; }
        public string Signature { get; set; }
        public string Airline { get; set; }

    }
    public class TradelaneBookingReportMAWB
    {
        public string HAWB { get; set; }
        public string MAWB { get; set; }
        public string MAWBCode { get; set; }
        public string MAWBWithCode { get; set; }
        public string MAWBCountryCode { get; set; }
        public string ShipperAddress { get; set; }
        public string ShipperAccountNumber { get; set; }
        public string ConsigneeAddress { get; set; }
        public string ConsigneeAccountNumber { get; set; }
        public string IssuedBy { get; set; }
        public string CarrierAgent { get; set; }
        public string NotifyParty { get; set; }
        public string AccountNo { get; set; }
        public string Reference { get; set; }
        public string OptionalShippingInformation { get; set; }
        public string AirportofDeparture { get; set; }
        public string DestinationAirport { get; set; }
        public string DestinationAirportCode { get; set; }
        public string ByFirstCarrier { get; set; }
        public string CurencyCode { get; set; }
        public string CHGSCode { get; set; }
        public string WTVALPPD { get; set; }
        public string WTVALCOLL { get; set; }
        public string OtherPPD { get; set; }
        public string OtherCOLL { get; set; }
        public decimal AmountOfInsurance { get; set; }

        public decimal TotalVolume { get; set; }
        public string VolumeUnit { get; set; }
        public int TotalCartons { get; set; }
        public decimal EstimatedWeight { get; set; }
        public decimal TotalWeight { get; set; }
        public string WeightUnit { get; set; }
        public string DimensionUnit { get; set; }
        public string ShipmentDescription { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedOnCountry { get; set; }
        public string Signature { get; set; }
        public string Airline { get; set; }
        public string IssuingCarriersAgentNameandCity { get; set; }
        public string ValuationCharge { get; set; }
        public string Tax { get; set; }
        public string TotalOtherChargesDueAgent { get; set; }
        public string TotalOtherChargesDueCarrier { get; set; }
        public string OtherCharges { get; set; }
        public string ChargesAtDestination { get; set; }
        public string TotalCollectCharges { get; set; }
        public string CurrencyConversionRates { get; set; }
        public string TotalPrepaid { get; set; }
        public string TotalCollect { get; set; }
        public string HandlingInformation { get; set; }
        public string AgentsIATACode { get; set; }
        public string DeclaredValueForCarriage { get; set; }
        public string DeclaredValueForCustoms { get; set; }
    }

    public class TradelaneCartonLabelReport
    {
        public string MAWB { get; set; }
        public string MAWBBarCode { get; set; }
        public string TotalPieces { get; set; }
        public int HawbScannedCarton { get; set; }
        public string Destination { get; set; }
        public string DestinationAirportCode { get; set; }
        public string HAWB { get; set; }
        public string HAWBBarCode { get; set; }
        public string HAWBBarCodeValue { get; set; }
        public string Departure { get; set; }
        public string DepartureAirportCode { get; set; }
        public string CarrierCode2 { get; set; }
        public int HAWBTotalPieces { get; set; }
        public int ScannedPieces { get; set; }
    }
}
