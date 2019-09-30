using Frayte.Services.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.Express
{
    public class CustomManifest
    {
        //Ship From 
        public string ShipperName { get; set; }
        public string ShipperAddress1 { get; set; }
        public string ShipperAddress2 { get; set; }
        public string ShipperAddress3 { get; set; }
        public string ShipperCity { get; set; }
        public string ShipperZip { get; set; }
        public string ShipperState { get; set; }
        public string ShipperPhoneNo { get; set; }
        public string ShipperEmail { get; set; }
        public string ShipperCountryCode { get; set; }
        public string CountryName { get; set; }

        //Ship To
        public string ConsigneeName { get; set; }
        public string ConsigneeAddress1 { get; set; }
        public string ConsigneeAddress2 { get; set; }
        public string ConsigneeAddress3 { get; set; }
        public string ConsigneeCity { get; set; }
        public string Consigneestate { get; set; }
        public string Consigneepostcode { get; set; }
        public string Consigneecountry { get; set; }
        public string ConsigneePhoneNo { get; set; }
        public string ConsigneeEmail { get; set; }


        public string ShipmentReference { get; set; }
        public string DeclaredCurrencyCode { get; set; }
        public decimal Value { get; set; }
        public string Carrier { get; set; }
        public int HubId { get; set; }
        public string ExporterArea { get; set; }
        public string TrackingNumber { get; set; }
        public int PaymentPartyTaxAndDutyAccountNumber { get; set; }
        public string Area { get; set; }
        public string CountryCode { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public string PiecesContent { get; set; }
        public decimal? Weight { get; set; }
        public int CartonQty { get; set; }
        public int DeclaredValue { get; set; }
        public string AccountNo { get; set; }
        public string ECommerceShipmentId { get; set; }
        public string ExporterCountryName { get; set; }
        public string MAWBNumber { get; set; }
        public string FlightNumber { get; set; }
        public DateTime? FlightDate { get; set; }
        public string HAWBNumber { get; set; }
        public int? Pieces { get; set; }
        public decimal? NetWeight { get; set; }
        public decimal? VolumeWeight { get; set; }
        public string ReferenceInvoiceNR { get; set; }
        public string Currency { get; set; }
        public string VersendLand { get; set; }
        public string Anzahl { get; set; }
        public string Netto { get; set; }
        public string Brutto { get; set; }
        public string TarifCode { get; set; }
        public string SwissValue { get; set; }
        public string OriginCountry { get; set; }
        public string PacType { get; set; }
        public string PacQty { get; set; }
        public string ZeichenPackstucke { get; set; }
        public string ArtVorpapier { get; set; }
        public string ZeichenVorpapier { get; set; }
        public string ProductDescription { get; set; }

        public string Reference { get; set; }
        public string InternalAccountnumber { get; set; }
        public string WeightUOM { get; set; }
        public string TotalValue { get; set; }
        public string CanadaCurrency { get; set; }
        public string CanadaPieces { get; set; }
        public string ItemHSCode { get; set; }

        //Uk Product Catalog
        public string Vat { get; set; }
        public string Pce { get; set; }
        public string Number { get; set; }
        public string TCode { get; set; }
        public string UKCurrency { get; set; }
        public string UKValue { get; set; }
        public string Weblink { get; set; }
        public string SKU { get; set; }
        public string ProvinceCode { get; set; }
        public string Incoterm { get; set; }
        public string CustomEntryType { get; set; }
        public string CustomTotalValue { get; set; }
        public string CustomTotalVAT { get; set; }
        public string CustomDuty { get; set; }
        public string No { get; set; }
        public string CodeKg { get; set; }
        public string DeclaredValueOfCustom { get; set; }
        public string Origin { get; set; }
        public string OriginCode { get; set; }
        public string Itemvalue { get; set; }
        public string CustomCommodityMap { get; set; }
        public string HubCode { get; set; }
        public string DepartureAirpotCode { get; set; }
    }

    public class CanadaProductcatalog
    {
        public int ProductCatalogId { get; set; }
        public int CustomerId { get; set; }
        public int FactoryUserId { get; set; }
        public int HubId { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public decimal DeclaredValue { get; set; }
        public string ProductDescription { get; set; }
        public string Reference { get; set; }
        public string InternalAccountnumber { get; set; }
        public FrayteWeightUOM WeightUOM { get; set; }
        public string TotalValue { get; set; }
        public FrayteCurrency Currency { get; set; }
        public string ItemHSCode { get; set; }
        public string Pieces { get; set; }
        public string SKU { get; set; }
        public string ProvinceCode { get; set; }
    }

    public class SwissProductcatalog
    {
        public int ProductCatalogId { get; set; }
        public int CustomerId { get; set; }
        public int FactoryUserId { get; set; }
        public int HubId { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public decimal DeclaredValue { get; set; }
        public string ProductDescription { get; set; }
        public string ReferenceInvoiceNR { get; set; }
        public FrayteCurrency Currency { get; set; }
        public string VersendLand { get; set; }
        public string Anzahl { get; set; }
        public string Netto { get; set; }
        public string Brutto { get; set; }
        public string TarifCode { get; set; }
        public string Value { get; set; }
        public string OriginCountry { get; set; }
        public string PacType { get; set; }
        public string PacQty { get; set; }
        public string ZeichenPackstucke { get; set; }
        public string ArtVorpapier { get; set; }
        public string ZeichenVorpapier { get; set; }
    }

    public class USAProductcatalog
    {
        public int ProductCatalogId { get; set; }
        public int CustomerId { get; set; }
        public int FactoryUserId { get; set; }
        public int HubId { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public decimal DeclaredValue { get; set; }
        public string ProductDescription { get; set; }
        public string Reference { get; set; }
        public string InternalAccountnumber { get; set; }
        public FrayteWeightUOM WeightUOM { get; set; }
        public string ItemHSCode { get; set; }
        public string CustomEntryType { get; set; }
        public string CustomTotalValue { get; set; }
        public string CustomTotalVAT { get; set; }
        public string CustomDuty { get; set; }
        public string Pieces { get; set; }
        public string ItemValue { get; set; }
        public string CustomCommodityMap { get; set; }
        public string ECommerceShipmentId { get; set; }
        public FrayteCurrency Currency { get; set; }
    }

    public class UKProductcatalog
    {
        public int ProductCatalogId { get; set; }
        public int CustomerId { get; set; }
        public int FactoryUserId { get; set; }
        public int HubId { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public decimal DeclaredValue { get; set; }
        public string ProductDescription { get; set; }
        public string Vat { get; set; }
        public string Pce { get; set; }
        public string Number { get; set; }
        public string TCode { get; set; }
        public FrayteCurrency Currency { get; set; }
        public string Value { get; set; }
        public string Weblink { get; set; }

    }

    public class ProductcatalogDetail
    {
        public int ProductcatalogId { get; set; }
        public string CustomerName { get; set; }
        public string HubName { get; set; }
        public string ProductDescription { get; set; }
        public int ProductcatalogDetailId { get; set; }
        public string Value { get; set; }
        public string ItemHSCode { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public decimal DeclaredValue { get; set; }
        public string CurrencyCode { get; set; }
        public string WeightUOM { get; set; }
        public int TotalRows { get; set; }
    }

    public class NorwayProductcatalog
    {
        public int ProductCatalogId { get; set; }
        public int CustomerId { get; set; }
        public int FactoryUserId { get; set; }
        public int HubId { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public decimal DeclaredValue { get; set; }
        public string ProductDescription { get; set; }
        public string Vat { get; set; }
        public string Pce { get; set; }
        public string Number { get; set; }
        public string TCode { get; set; }
        public FrayteCurrency Currency { get; set; }
        public string Value { get; set; }
        public string Weblink { get; set; }
    }

    public class SINProductcatalog
    {
        public int ProductCatalogId { get; set; }
        public int CustomerId { get; set; }
        public int FactoryUserId { get; set; }
        public int HubId { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public decimal DeclaredValue { get; set; }
        public string ProductDescription { get; set; }
        public string Vat { get; set; }
        public string Pce { get; set; }
        public string Number { get; set; }
        public string TCode { get; set; }
        public FrayteCurrency Currency { get; set; }
        public string Value { get; set; }
        public string Weblink { get; set; }
    }

    public class EAMProductcatalog
    {
        public int CustomerId { get; set; }
        public int FactoryUserId { get; set; }
        public int HubId { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public decimal DeclaredValue { get; set; }
        public string ProductDescription { get; set; }
        public string Reference { get; set; }
        public string InternalAccountnumber { get; set; }
        public FrayteWeightUOM WeightUOM { get; set; }
        public string TotalValue { get; set; }
        public FrayteCurrency Currency { get; set; }
        public string ItemHSCode { get; set; }
        public string CustomEntryType { get; set; }
        public string CustomTotalValue { get; set; }
        public string CustomTotalVAT { get; set; }
        public string CustomDuty { get; set; }
        public string Pieces { get; set; }
    }

    public class JapanProductcatalog
    {
        public int ProductCatalogId { get; set; }
        public int CustomerId { get; set; }
        public int FactoryUserId { get; set; }
        public int HubId { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public decimal DeclaredValue { get; set; }
        public string ProductDescription { get; set; }
        public string Number { get; set; }
        public string Code { get; set; }
        public string DeclaredValueOfCustom { get; set; }
        public FrayteCurrency Currency { get; set; }
        public string Origin { get; set; }
        public string OriginCode { get; set; }
    }

    public class FrayteCurrency
    {
        public string CurrencyCode { get; set; }
        public string CurrencyDescription { get; set; }
    }

    public class FrayteWeightUOM
    {
        public string UnitValue { get; set; }
        public string UnitDisplay { get; set; }
    }

    public class FrayteProductCatalogTrack
    {
        public int CustomerId { get; set; }
        public int HubId { get; set; }
        public string ProductDescription { get; set; }
        public int CurrentPage { get; set; }
        public int TakeRows { get; set; }
    }
}
