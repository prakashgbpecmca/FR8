using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public class FrayteZoneBaseRateCard
    {
        public int ZoneRateCardId { get; set; }
        public FrayteZone zone { get; set; }
        public LogisticShipmentType shipmentType { get; set; }
        public FrayteCourierAccount courierAccount { get; set; }
        public FrayteLogisticWeight LogisticWeight { get; set; }
        public FrayteLogisticDimension LogisticDimension { get; set; }
        public FrayteLogisticArea LogisticArea { get; set; }
        public int OperationZoneId { get; set; }
        public decimal Rate { get; set; }
        public string LogisticType { get; set; }
        public string CurrencyCode { get; set; }
        public bool IsChanged { get; set; }
        public string LogisticServicetype { get; set; }
        public float BusinessRate { get; set; }
        public string BusinessCurrency { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string ModuleType { get; set; }
        public int LogisticServiceAddOnId { get; set; }
    }

    public class FrayteAddOnRateCard
    {
        public string shipmentType { get; set; }
        public decimal WeightFrom { get; set; }
        public decimal WeightTo { get; set; }
        public List<AddOnRate> Rate { get; set; }
    }

    public class AddOnRate
    {
        public int ZoneRateCardId { get; set; }
        public decimal Rate { get; set; }
        public bool IsChanged { get; set; }
        public string CurrencyCode { get; set; }
    }

    public class FrayteLogisticDimension
    {
        public int LogisticServiceDimensionId { get; set; }
        public decimal DimensionFrom { get; set; }
        public decimal DimensionTo { get; set; }
        public string DimensionUnit { get; set; }
        public string Parceltype { get; set; }
        public string PackageType { get; set; }
        public string LogisticType { get; set; }
        public string LogisticServiceType { get; set; }
    }

    public class FrayteDimensionBaseRateCard
    {
        public int ZoneRateCardId { get; set; }
        public int LogisticServiceDimensionId { get; set; }
        public decimal DimensionFrom { get; set; }
        public decimal DimensionTo { get; set; }
        public string DimensionUnit { get; set; }
        public decimal DimensionRate { get; set; }
        public string Currency { get; set; }
        public string Parceltype { get; set; }
        public string PackageType { get; set; }
        public string LogisticType { get; set; }
        public string LogisticServiceType { get; set; }
        public float BusinessRate { get; set; }
        public string BusinessCurrency { get; set; }
    }

    public class FryateZoneBaseRateCardLimit
    {
        public int ZoneRateCardId { get; set; }
        public decimal OverWeightLimit { get; set; }
        public decimal OverWeightTo { get; set; }
        public decimal OverWeightLimitRate { get; set; }
        public string Currency { get; set; }
        public string UnitOfMeasurement { get; set; }
        public string WeightUnit { get; set; }
        public FrayteZone Zone { get; set; }
        public string LogisticType { get; set; }
        public string LogisticServiceType { get; set; }
        public string PackageType { get; set; }
        public string ParcelType { get; set; }
        public float BusinessRate { get; set; }
        public string BusinessCurrency { get; set; }
    }

    public class ShipmentDescripType
    {
        public int ShipmentTypeId { get; set; }
        public string Code { get; set; }
        public string ShipmentType { get; set; }
    }

    public class LogisticShipmentType
    {
        public int ShipmentTypeId { get; set; }
        public int LogisticServiceId { get; set; }
        public int OperationZoneId { get; set; }
        public string RateType { get; set; }
        public string RateTypeDisplay { get; set; }
        public string LogisticType { get; set; }
        public string LogisticDescription { get; set; }
        public string LogisticDescriptionDisplay { get; set; }
        public string ReportLogisticDisplay { get; set; }
        public LogisticShipmentService LogisticService { get; set; }
    }

    public class LogisticShipmentService
    {
        public int LogisticServiceId { get; set; }
        public int OperationZoneId { get; set; }
        public string LogisticCompany { get; set; }
        public string LogisticCompanyDisplay { get; set; }
        public string LogisticType { get; set; }
        public string LogisticTypeDisplay { get; set; }
        public string RateType { get; set; }
        public string RateTypeDisplay { get; set; }
        public string ModuleType { get; set; }
    }

    public class FrayteLogisticWeight
    {
        public int LogisticWeightId { get; set; }
        public LogisticShipmentType ShipmentType { get; set; }
        public decimal WeightFrom { get; set; }
        public decimal WeightTo { get; set; }
        public string UnitOfMeasurement { get; set; }
        public string WeightUnit { get; set; }
        public decimal WeightLimit { get; set; }
        public string PackageType { get; set; }
        public string ParcelType { get; set; }
        public string PackageDisplayType { get; set; }
        public string ParcelDisplayType { get; set; }
    }

    public class FrayteOperationZoneCurrency
    {
        public string CurrencyCode { get; set; }
    }

    public class FrayteLogisticArea
    {
        public int LogisticAreaId { get; set; }
        public int LogisticCountryId { get; set; }
        public string LogisticAreaName { get; set; }
        public FrayteCountryCode LogisticCountry { get; set; }
        public LogisticShipmentType LogisticShipment { get; set; }
    }

    public class FrayteBaseRate
    {
        public decimal Rate1 { get; set; }
        public decimal Rate2 { get; set; }
        public decimal Rate3 { get; set; }
        public decimal Rate4 { get; set; }
        public decimal Rate5 { get; set; }
        public decimal Rate6 { get; set; }
        public decimal Rate7 { get; set; }
        public decimal Rate8 { get; set; }
        public decimal Rate9 { get; set; }
        public decimal Rate10 { get; set; }
        public decimal Rate11 { get; set; }
        public decimal Rate12 { get; set; }
        public decimal Rate13 { get; set; }
        public string Weight { get; set; }
        public string PackageType { get; set; }
        public string ParcelType { get; set; }
        public string ShipmentType { get; set; }
        public int SortOrder { get; set; }
    }

    public class FrayteHermesParcelPOD
    {
        public decimal Rate1 { get; set; }
        public decimal Rate2 { get; set; }
        public decimal Rate3 { get; set; }
        public string Weight { get; set; }
        public string PackageType { get; set; }
        public string ShipmentType { get; set; }
        public int SortOrder { get; set; }
    }

    public class FrayteHermesPacketPOD
    {
        public decimal Rate1 { get; set; }
        public decimal Rate2 { get; set; }
        public decimal Rate3 { get; set; }
        public string Weight { get; set; }
        public string PackageType { get; set; }
        public string ShipmentType { get; set; }
        public int SortOrder { get; set; }
    }

    public class FrayteHermesParcelNONPOD
    {
        public decimal Rate1 { get; set; }
        public decimal Rate2 { get; set; }
        public decimal Rate3 { get; set; }
        public string Weight { get; set; }
        public string PackageType { get; set; }
        public string ShipmentType { get; set; }
        public int SortOrder { get; set; }
    }

    public class FrayteHermesPacketNONPOD
    {
        public decimal Rate1 { get; set; }
        public decimal Rate2 { get; set; }
        public decimal Rate3 { get; set; }
        public string Weight { get; set; }
        public string PackageType { get; set; }
        public string ShipmentType { get; set; }
        public int SortOrder { get; set; }
    }

    public class FrayteHermesBaseRate
    {
        public List<FrayteHermesParcelPOD> ParcelPOD { get; set; }
        public List<FrayteHermesPacketPOD> PacketPOD { get; set; }
        public List<FrayteHermesParcelNONPOD> ParcelNONPOD { get; set; }
        public List<FrayteHermesPacketNONPOD> PacketNONPOD { get; set; }
    }

    public class FrayteBaseRateZoneCountry
    {
        public string Country1 { get; set; }
        public string ZoneName1 { get; set; }
        public string Country2 { get; set; }
        public string ZoneName2 { get; set; }
        public string Country3 { get; set; }
        public string ZoneName3 { get; set; }
        public string Country4 { get; set; }
        public string ZoneName4 { get; set; }
    }

    public class CustomerAddOnRate
    {
        public decimal AddOnRate { get; set; }
        public decimal WeightFrom { get; set; }
        public decimal WeightTo { get; set; }
        public int LogisticServiceZoneId { get; set; }
        public int LogisticServiceAddOnId { get; set; }
        public string AddOnDescription { get; set; }
    }

    public class FrayteAddOnRate
    {
        public string ShipmentType { get; set; }
        public string Weight { get; set; }
        public decimal Rate1 { get; set; }
        public decimal Rate2 { get; set; }
        public decimal Rate3 { get; set; }
        public decimal Rate4 { get; set; }
        public decimal Rate5 { get; set; }
        public decimal Rate6 { get; set; }
        public decimal Rate7 { get; set; }
        public decimal Rate8 { get; set; }
        public decimal Rate9 { get; set; }
        public decimal Rate10 { get; set; }
        public decimal Rate11 { get; set; }
        public decimal Rate12 { get; set; }
        public decimal Rate13 { get; set; }
    }

    public class FrayteBaseRateWithCountry
    {
        public List<FrayteBaseRate> BaseRate { get; set; }
        public List<FrayteAddOnRate> AddOnRate { get; set; }
        public List<FrayteBaseRateZoneCountry> ZoneCountry { get; set; }
        public List<FrayteZoneMatrix> ThirdParty { get; set; }
        public List<FrayteApplyZone> Zone { get; set; }
    }

    public class FrayteHermes
    {
        public List<FrayteBaseRate> Packet { get; set; }
        public List<FrayteBaseRate> Parcel { get; set; }
    }

    public class FrayteYodel
    {
        public List<FrayteBaseRate> B2B { get; set; }
        public List<FrayteBaseRate> B2CNeighbourhood { get; set; }
        public List<FrayteBaseRate> B2CHome { get; set; }
    }

    public class LogisticServiceDefinition
    {
        public int OperationZoneId { get; set; }
        public string LogisticType { get; set; }
        public string CourierCompany { get; set; }
        public string RateType { get; set; }
        public string ModuleType { get; set; }
    }

    public class LogisticServiceDuration : LogisticServiceDefinition
    {
        public int LogisticServiceId { get; set; }
        public DateTime IssuedDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string RateTypeDisplay { get; set; }
    }

    public class FrayteAddRate
    {
        public string Shipment { get; set; }
        public decimal WeightFrom { get; set; }
        public decimal WeightTo { get; set; }
        public List<FrayteZoneBaseRateCard> Rate { get; set; }
    }
}
