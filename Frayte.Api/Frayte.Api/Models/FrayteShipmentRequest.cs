using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Frayte.Services.Models;
using Frayte.Services.Models.ApiModal;

namespace Frayte.Api.Models
{
    public class FrayteShipmentRequest
    {
        public Security Security { get; set; }
        public ShipFrom ShipFrom { get; set; }
        public ShipTo ShipTo { get; set; }       
        public List<Package> Package { get; set; }       
        public ApiCustomInformation CustomInformation { get; set; }
        public string ParcelType { get; set; }
        public string AddressType { get; set; }      
        public string DeclaredCurrencyCode { get; set; }
        public string ShipmentReference { get; set; }
        public string PaymentPartyTaxAndDuties { get; set; }       
        public DateTime? RequestedPickupDate { get; set; }    
        public string PackageCalculationType { get; set; }
    }

    public class ShipTo
    {
        public string CompanyName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }       
        public AddressTo Address { get; set; }
    }

    public class ShipFrom
    {
        public string CompanyName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }      
        public AddressFrom Address { get; set; }
    }

    public class AddressTo
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Area { get; set; }
        public string Postcode { get; set; }
        public string CountryCode { get; set; }       
    }

    public class AddressFrom
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Area { get; set; }
        public string Postcode { get; set; }
        public string CountryCode { get; set; }     
    }

    public class Package
    {
        public int CartoonValue { get; set; }
        public string Length { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
        public decimal Weight { get; set; }
        public decimal DeclaredValue { get; set; }
        public string ShipmentContents { get; set; }
    }
    
    public class ApiCustomInformation
    {
        public string ContentsType { get; set; }
        public string ContentsExplanation { get; set; }
        public string RestrictionType { get; set; }
        public string RestrictionComments { get; set; }
        public string CustomsSigner { get; set; }
        public string NonDeliveryOption { get; set; }
    }
}