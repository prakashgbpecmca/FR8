using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frayte.Services.Models;

namespace Frayte.Services.Models
{
    public class ManifestShipmentModel
    {
        public string StatusType { get; set; }
        public string Courier { get; set; }
        public string LogisticType { get; set; }

        //Ship From Info
        public string ShipFromName { get; set; }
        public string ShipFromCompany { get; set; }
        public string ShipFromAddress { get; set; }
        public string ShipFromAddress2 { get; set; }
        public string ShipFromCity { get; set; }
        public string ShipFromState { get; set; }
        public string ShipFromPostCode { get; set; }
        public string ShipFromCountry { get; set; }
        public string ShipFromPhone { get; set; }
        public string ShipFromEmail { get; set; }

        //Ship To Info
        public string ShipToName { get; set; }
        public string ShipToCompany { get; set; }
        public string ShipToAddress { get; set; }
        public string ShipToAddress2 { get; set; }
        public string ShipToCity { get; set; }
        public string ShipToState { get; set; }
        public string ShipToPostCode { get; set; }
        public string ShipToCountry { get; set; }
        public string ShipToPhone { get; set; }
        public string ShipToEmail { get; set; }

        //Custom Information
        public string ContentsType { get; set; }
        public string RestrictionType { get; set; }
        public string ContentsExplanation { get; set; }
        public string NonDeliveryOption { get; set; }
        public string CustomsSigner { get; set; }

        //Shipment Detail
        public string TrackingNo { get; set; }
        public string FrayteNumber { get; set; }
        public string ParcelType { get; set; }
        public string ShipmentReference { get; set; }
        public string ShipmentType { get; set; }
        public string PackageCalculationType { get; set; }
        public int TotalCartons { get; set; }
        public decimal ShipmentWeight { get; set; }
        public string PaymentPartyTaxAndDuties { get; set; }
        public string PaymentPartyTaxAndDutiesAcceptedBy { get; set; }
        public string ManifestNumber { get; set; }
        public string ShipmentContent { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string DeclaredValueCurrency { get; set; }
        public decimal EstimatedCost { get; set; }
        public decimal FuelSurcharge { get; set; }
        public string FuelPercent { get; set; }
        public DateTime? FuelMonthYear { get; set; }
        public string CollectionDateandTime { get; set; }
        public DateTime? CollectionDate { get; set; }
        public string CollectionTime { get; set; }
        public string PickUpRef { get; set; }
        public string DeliveryDate { get; set; }
        public DateTime? DeliveryDateValue { get; set; }
        public string DeliveryTime { get; set; }
        public string SignedBy { get; set; }

        //Customer Detail

        public string CustomerCompany { get; set; }
        public string CustomerAccountNo { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCountry { get; set; }
        public string CustomerPostCode { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhoneNo { get; set; }
        public string CustomerAddress1 { get; set; }
        public string CustomerAddress2 { get; set; }
        public string CustomerAddress3 { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Suburb { get; set; }
        public string Area { get; set; }
    }
}