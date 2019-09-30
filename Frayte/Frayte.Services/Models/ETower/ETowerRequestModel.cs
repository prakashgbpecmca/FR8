using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.ETower
{

    public class ETowerRequestModel
    {
        public string trackingNo { get; set; }
        public string referenceNo { get; set; }
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string addressLine3 { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string description { get; set; }
        public string nativeDescription { get; set; }
        public string email { get; set; }
        public string facility { get; set; }
        public string instruction { get; set; }
        public string invoiceCurrency { get; set; }
        public decimal invoiceValue { get; set; }
        public string phone { get; set; }
        public string platform { get; set; }
        public string postcode { get; set; }
        public string recipientCompany { get; set; }
        public string recipientName { get; set; }
        public string serviceCode { get; set; }
        public string serviceOption { get; set; }
        public string sku { get; set; }
        public string state { get; set; }
        public string weightUnit { get; set; }
        public decimal weight { get; set; }
        public string dimensionUnit { get; set; }
        public decimal length { get; set; }
        public decimal width { get; set; }
        public decimal height { get; set; }
        public decimal volume { get; set; }
        public string shipperName { get; set; }
        public string shipperAddressLine1 { get; set; }
        public string shipperAddressLine2 { get; set; }
        public string shipperAddressLine3 { get; set; }
        public string shipperCity { get; set; }
        public string shipperState { get; set; }
        public string shipperPostcode { get; set; }
        public string shipperCountry { get; set; }
        public string shipperPhone { get; set; }
        public string recipientTaxId { get; set; }
        public string authorityToLeave { get; set; }
        public string incoterm { get; set; }
        public string lockerService { get; set; }
        public ExtendData extendData { get; set; }
        public List<OrderItem> orderItems { get; set; }
    }

    public class ExtendData
    {
        public string nationalNumber { get; set; }
        public string nationalIssueDate { get; set; }
        public string cyrillicName { get; set; }
        public string imei { get; set; }
        public bool isImei { get; set; }
        public string vendorid { get; set; }
        public string gstexemptioncode { get; set; }
        public string abnnumber { get; set; }
        public string sortCode { get; set; }
    }

    public class OrderItem
    {
        public string itemNo { get; set; }
        public string sku { get; set; }
        public string description { get; set; }
        public string nativeDescription { get; set; }
        public string hsCode { get; set; }
        public string originCountry { get; set; }
        public decimal unitValue { get; set; }
        public int itemCount { get; set; }
        public decimal weight { get; set; }
        public string productURL { get; set; }
    }

    public class EtowerLableRequest
    {
        public List<string> orderIds { get; set; }
        public int labelType { get; set; }
        public bool packinglist { get; set; }
        public bool merged { get; set; }
        public string labelFormat { get; set; }
    }

}
