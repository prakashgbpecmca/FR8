using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.DHL
{
  

    public class DHLShipmentRequestDto
    {
        public DHLRequestDto Request { get; set; }
        public string LanguageCode { get; set; }
        public string RegionCode { get; set; }
        public string PiecesEnabled { get; set; }
        public BillingDto Billing { get; set; }        
        public ConsigneeDto Consignee { get; set; }
        public DutiableDto Dutiable { get; set; }
        public ReferenceDto Reference { get; set; }      
        
        public ShipmentDetailsDto ShipmentDetails { get; set; }
        public DHLShipperDto Shipper { get; set; }
        public string LabelImageFormat { get; set; }
        public string CollectionTime { get; set; }
        public string AWBNumber { get; set; }
        public int DraftShipmentId { get; set;}
        public bool IsNDSEnable { get; set; }
        public String DoorTo { get; set; }
        public bool IsPickup { get; set; }

    }

    public class DHLRequestDto
    {
        public ServiceHeaderDto ServiceHeader { get; set; }
    }

    public class ServiceHeaderDto
    {
        public string MessageTime { get; set; }
        public string MessageReference { get; set; }
        public string SiteID { get; set; }
        public string Password { get; set; }
    }

    public class BillingDto
    {
        public string ShipperAccountNumber { get; set; }
        public string ShippingPaymentType { get; set; }
        public string BillingAccountNumber { get; set; }
        public string DutyPaymentType { get; set; }
        public string DutyAccountNumber { get; set; }
    }

    public class ConsigneeDto
    {

        public string CompanyName { get; set; }        
        public List<DHLAddressDto> AddressLine { get; set; }
        public string City { get; set; }
        public string DivisionCode { get; set; }
        public string PostalCode { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public ContactDto Contact { get; set; }
    }

    public class DHLAddressDto
    {
        public string AddressLine { get; set; }
    }

    public class ContactDto
    {
        public string PersonName { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class DutiableDto
    {
        public string DeclaredValue { get; set; }
        public string CurrencyCode { get; set; }
    }

    public class ShipmentDetailsDto
    {
        public string NumberOfPieces { get; set; }
        public string CurrencyCode { get; set; }
        public List<PieceDto> Pieces { get; set; }
        public string PackageType { get; set; }
        public string Weight { get; set; }
        public string DimensionUnit { get; set; }
        public string WeightUnit { get; set; }
        public string GlobalProductCode { get; set; }
        public string LocalProductCode { get; set; }
        public DateTime Date { get; set; }
        public string Contents { get; set; }
        public string IsDutiable { get; set; }
    }
    
    public class PieceDto
    {
        public decimal Weight { get; set; }
        public string Depth { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
        public string Value { get; set; }
        public string ProductDescription { get; set; }
    }

    public class DHLShipperDto
    {
        public string ShipperID { get; set; }      
        public string CompanyName { get; set; }
        public List<DHLAddressDto> AddressLine { get; set; }
        public string City { get; set; }
        public string DivisionCode { get; set; }
        public string PostalCode { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public ContactDto Contact { get; set; }
    }

    public class ReferenceDto
    {
        public string ReferenceID { get; set; }
        public string ReferenceType { get; set; }
    }
    
}
