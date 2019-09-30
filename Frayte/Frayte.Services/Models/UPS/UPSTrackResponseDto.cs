using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.UPS
{
    public class UPSTrackResponseDto
    {
        public TrackResponseDto TrackResponse { get; set; }

        public TrackingShipmentDto Shipment { get; set; }
    }
    public class TrackResponseDto
    {
        public ResponseDto Response { get; set; }
    }

    public class TrackingShipmentDto
    {
        public InquiryNumberDto InquiryNumber { get; set; }
        public string ShipperNumber { get; set; }
        public TrackServiceDto Service { get; set; }
        public List<TrackPackageDto> Package { get; set; }
        public TrackShipmentWeightDto ShipmentWeight { get; set; }

        public string PickupDate { get; set; }
    }
    public class InquiryNumberDto
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
    }
    public class TrackServiceDto
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }

    public class TrackPackageDto
    {
        public string TrackingNumber { get; set; }
        public PackageWeight PackageWeight { get; set; }
        public List<ActivityDto> Activity { get; set; }
        public ActivityLocationDto ActivityLocation { get; set; }
        public StatusDto Status { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public List<DocumentDto> Document { get; set; }
        public PackageServiceOption PackageServiceOption { get; set; }



    }

    public class ActivityDto
    {
        public StatusDto Status { get; set; }
        public ActivityLocations1Dto ActivityLocation { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }

    }
    public class StatusDto
    {
        public string Type { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
    }

    public class ActivityLocationDto
    {
        public TrackAddressDto Address { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string SignedForByName { get; set; }
    }

    public class TrackAddressDto
    {
        public string AddressLine { get; set; }
        public string City { get; set; }
        public string StateProvinceCode { get; set; }
        public string PostalCode { get; set; }

        public string CountryCode { get; set; }
    }
    public class DocumentDto
    {
        public TypeDto Type { get; set; }
        public string Content { get; set; }
        public ImageFormatDto Format { get; set; }

    }

    public class TypeDto
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }
    public class ActivityLocations1Dto
    {
        public TrackAddress1Dto Address { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string SignedForByName { get; set; }
    }
    public class TrackAddress1Dto
    {
        public string CountryCode { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
    }

    public class TrackShipmentWeightDto
    {
        public UnitOfMeasurementDto UnitOfMeasurement { get; set; }
        public string Weight { get; set; }

    }
    public class PackageServiceOption
    {
        public Type Type { get; set; }
    }
    public class Type
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }
    public class PackageWeight
    {
        public UnitOfMeasurementDto UnitOfMeasurement { get; set; }
        public string Weight { get; set; }

    }
}
