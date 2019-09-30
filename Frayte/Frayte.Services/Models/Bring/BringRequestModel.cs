using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.Bring
{
    public class Contact
    {
        public string name { get; set; }
        public string email { get; set; }
        public string phoneNumber { get; set; }
    }

    public class Sender
    {
        public string name { get; set; }
        public string addressLine { get; set; }
        public object addressLine2 { get; set; }
        public string postalCode { get; set; }
        public string city { get; set; }
        public string countryCode { get; set; }
        public string reference { get; set; }
        public string additionalAddressInfo { get; set; }
        public Contact contact { get; set; }
    }



    public class Recipient
    {
        public string name { get; set; }
        public string addressLine { get; set; }
        public string addressLine2 { get; set; }
        public string postalCode { get; set; }
        public string city { get; set; }
        public string countryCode { get; set; }
        public string reference { get; set; }
        public string additionalAddressInfo { get; set; }
        public Contact contact { get; set; }
    }

    public class Parties
    {
        public Sender sender { get; set; }
        public Recipient recipient { get; set; }
        public object pickupPoint { get; set; }
    }

    public class Product
    {
        public string id { get; set; }
        public string customerNumber { get; set; }
        public object services { get; set; }
        public object customsDeclaration { get; set; }
    }

    public class Dimensions
    {
        public decimal heightInCm { get; set; }
        public decimal widthInCm { get; set; }
        public decimal lengthInCm { get; set; }
    }

    public class Package
    {
        public string weightInKg { get; set; }
        public string goodsDescription { get; set; }
        public Dimensions dimensions { get; set; }
        public object containerId { get; set; }
        public object packageType { get; set; }
        public object numberOfItems { get; set; }
        public string correlationId { get; set; }
    }

    public class ConsignmentModel
    {
        public string shippingDateTime { get; set; }
        public Parties parties { get; set; }
        public Product product { get; set; }
        public object purchaseOrder { get; set; }
        public string correlationId { get; set; }
        public List<Package> packages { get; set; }
    }

    public class BringRequestModel
    {
        public bool testIndicator { get; set; }
        public int schemaVersion { get; set; }
        public List<ConsignmentModel> consignments { get; set; }
    }
}
