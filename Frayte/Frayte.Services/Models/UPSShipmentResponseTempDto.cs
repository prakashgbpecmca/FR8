using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{

    public class UPSShipmentResponseTempDto
    {
        public ShipmentResponseTempDto ShipmentResponse { get; set; }

        public FratyteError Error { get; set; }
    }
    public class ShipmentResponseTempDto
    {
        public ResponseTempDto Response { get; set; }
        public ShipmentResultsTempDto ShipmentResults { get; set; }
    }
    public class ResponseTempDto
    {
        public ResponseStatusTempDto ResponseStatus { get; set; }
        public TransactionReferenceTempDto TransactionReference { get; set; }

    }
    public class ShipmentResultsTempDto
    {
        public ShipmentChargesTempDto ShipmentCharges { get; set; }
        public NegotiatedRateChargesTempDto NegotiatedRateCharges { get; set; }
        public BillingWeightTempDto BillingWeight { get; set; }
        public string ShipmentIdentificationNumber { get; set; }
        public PackageResultsTempDto PackageResults { get; set; }
    }

    public class ResponseStatusTempDto
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }
    public class TransactionReferenceTempDto
    {
        public string CustomerContext { get; set; }
    }

    public class ShipmentChargesTempDto
    {
        public TransportationChargesTempDto TransportationCharges { get; set; }
        public ServiceOptionsChargesTempDto ServiceOptionsCharges { get; set; }

        public TotalChargesTempDto TotalCharges { get; set; }
    }

    public class TransportationChargesTempDto
    {
        public string CurrencyCode { get; set; }
        public string MonetaryValue { get; set; }
    }

    public class ServiceOptionsChargesTempDto
    {
        public string CurrencyCode { get; set; }
        public string MonetaryValue { get; set; }
    }

    public class TotalChargesTempDto
    {
        public string CurrencyCode { get; set; }
        public string MonetaryValue { get; set; }
    }
    public class NegotiatedRateChargesTempDto
    {
        public TotalChargeTempDto TotalCharge { get; set; }
    }

    public class TotalChargeTempDto
    {
        public string CurrencyCode { get; set; }
        public string MonetaryValue { get; set; }
    }

    public class BillingWeightTempDto
    {
        public UnitOfMeasurementTempDto UnitOfMeasurement { get; set; }
        public string Weight { get; set; }
    }

    public class UnitOfMeasurementTempDto
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }
    public class PackageResultsTempDto
    {
        public string TrackingNumber { get; set; }
        public ServiceOptionsChargesTempDto ServiceOptionsCharges { get; set; }
        public ShippingLabelTempDto ShippingLabel { get; set; }
    }

    public class ShippingLabelTempDto
    {
        public ImageFormatTempDto ImageFormat { get; set; }
        public string GraphicImage { get; set; }
        public string HTMLImage { get; set; }
    }

    public class ImageFormatTempDto
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }
}


