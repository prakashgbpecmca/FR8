using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{

    public class UPSShipmentResponseDto
    {
        public ShipmentResponseDto ShipmentResponse { get; set; }      
        public FratyteError Error { get; set; }
    }
    public class ShipmentResponseDto
    {
        public ResponseDto Response { get; set; }
        public ShipmentResultsDto ShipmentResults { get; set; }
    }
    public class ResponseDto
    {
        public ResponseStatusDto ResponseStatus { get; set; }
        public TransactionReferenceDto TransactionReference { get; set; }
    }

    public class ShipmentResultsDto
    {
        public ShipmentChargesDto ShipmentCharges { get; set; }
        public NegotiatedRateChargesDto NegotiatedRateCharges { get; set; }
        public BillingWeightDto BillingWeight { get; set; }
        public string ShipmentIdentificationNumber { get; set; }
        public string PickupRef { get; set; }
        public List<PackageResultsDto> PackageResults { get; set; }
    }

    public class ResponseStatusDto
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }

    public class TransactionReferenceDto
    {
        public string CustomerContext { get; set; }
    }

    public class ShipmentChargesDto
    {
        public TransportationChargesDto TransportationCharges { get; set; }
        public ServiceOptionsChargesDto ServiceOptionsCharges { get; set; }
        public TotalChargesDto TotalCharges { get; set; }
    }

    public class TransportationChargesDto
    {
        public string CurrencyCode { get; set; }
        public string MonetaryValue { get; set; }
    }

    public class ServiceOptionsChargesDto
    {
        public string CurrencyCode { get; set; }
        public string MonetaryValue { get; set; }
    }

    public class TotalChargesDto
    {
        public string CurrencyCode { get; set; }
        public string MonetaryValue { get; set; }
    }
    public class NegotiatedRateChargesDto
    {
        public TotalChargeDto TotalCharge { get; set; }
    }

    public class TotalChargeDto
    {
        public string CurrencyCode { get; set; }
        public string MonetaryValue { get; set; }
    }

    public class BillingWeightDto
    {
        public UnitOfMeasurementDto UnitOfMeasurement { get; set; }
        public string Weight { get; set; }
    }

    public class UnitOfMeasurementDto
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }

    public class PackageResultsDto
    {
        public string TrackingNumber { get; set; }
        public ServiceOptionsChargesDto ServiceOptionsCharges { get; set; }
        public ShippingLabelDto ShippingLabel { get; set; }
    }

    public class ShippingLabelDto
    {
        public ImageFormatDto ImageFormat { get; set; }
        public string GraphicImage { get; set; }
        public string HTMLImage { get; set; }
    }

    public class ImageFormatDto
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
