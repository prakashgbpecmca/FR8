using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public class UPSShipmentRequestDto
    {
        public UPSSecurityDto UPSSecurity { get; set; }
        public ShipmentRequestDto ShipmentRequest { get; set; }
        public int DraftShipmentId { get; set; }
    }

    public class UPSSecurityDto
    {
        public UsernameTokenDto UsernameToken { get; set; }
        public ServiceAccessTokenDto ServiceAccessToken { get; set; }
    }

    public class UsernameTokenDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class ServiceAccessTokenDto
    {
        public string AccessLicenseNumber { get; set; }
    }
    public class ShipmentRequestDto
    {
        public RequestDto Request { get; set; }
        public ShipmentDto Shipment { get; set; }
        public LabelSpecificationDto LabelSpecification { get; set; }
    }
    public class RequestDto
    {
        public string RequestOption { get; set; }
        public TransactionReferenceDto TransactionReference { get; set; }
    }
    public class ShipmentDto
    {
        public string Description { get; set; }
        public ShipperDto Shipper { get; set; }
        public ShipToAddressDto ShipTo { get; set; }
        public ShipFromAddressDto ShipFrom { get; set; }
        public PaymentInformationDto PaymentInformation { get; set; }
        public ServiceDto Service { get; set; }
        public List<PackageDto> Package { get; set; }
    }
    public class PackageDto
    {
        public string Description { get; set; }
        public PackagingDto Packaging { get; set; }
        public DimensionsDto Dimensions { get; set; }
        public PackageWeightDto PackageWeight { get; set; }
    }
    public class DimensionsDto
    {
        public string Length { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
        public UnitOfMeasurementDto UnitOfMeasurement { get; set; }
    }
    public class PackageWeightDto
    {
        public UnitOfMeasurementDto UnitOfMeasurement { get; set; }
        public string Weight { get; set; }
    }

    public class PackagingDto
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }
    public class ServiceDto
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }
    public class PaymentInformationDto
    {
        public ShipmentChargeDto ShipmentCharge { get; set; }
    }
    public class ShipmentChargeDto
    {
        public string Type { get; set; }
        public BillShipperDto BillShipper { get; set; }
    }
    public class BillShipperDto
    {
        public string AccountNumber { get; set; }
    }
    public class ShipperDto
    {
        public string Name { get; set; }
        public string AttentionName { get; set; }
        public string TaxIdentificationNumber { get; set; }
        public PhoneDto Phone { get; set; }
        public string ShipperNumber { get; set; }
        public string FaxNumber { get; set; }
        public AddressDto Address { get; set; }
    }
    public class ShipFromAddressDto
    {
        public string Name { get; set; }
        public string AttentionName { get; set; }
        public string TaxIdentificationNumber { get; set; }
        public PhoneDto Phone { get; set; }
        public string FaxNumber { get; set; }
        public AddressDto Address { get; set; }
    }
    public class PhoneDto
    {
        public string Number { get; set; }
        public string Extension { get; set; }
    }
    public class AddressDto
    {
        public string AddressLine { get; set; }
        public string City { get; set; }
        public string StateProvinceCode { get; set; }
        public string PostalCode { get; set; }
        public string CountryCode { get; set; }
    }
    public class ShipToAddressDto
    {
        public string Name { get; set; }
        public string AttentionName { get; set; }

        public PhoneDto Phone { get; set; }
        public AddressDto Address { get; set; }
    }
    public class LabelSpecificationDto
    {
        public LabelImageFormatDto LabelImageFormat { get; set; }
        public string HTTPUserAgent { get; set; }
    }

    public class LabelImageFormatDto
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }


}

