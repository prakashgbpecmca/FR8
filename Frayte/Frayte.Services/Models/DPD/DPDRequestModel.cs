using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.DPD
{
    public class DPDRequestModel
    {
        public string job_id { get; set; }
        public bool collectionOnDelivery { get; set; }
        public string invoice { get; set; }
        public string collectionDate { get; set; }
        public bool consolidate { get; set; }
        public List<Consignment> consignment { get; set; }
        public int DraftShipmentId { get; set; }
    }
    public class DpdShipmentResponse
    {
        public DPDResponseModel DPDResponse { get; set; }
        public FratyteError Error { get; set; }
        public string LabelHTMLString { get; set; }
        public string LabeleplString { get; set; }
        public string PickupRef { get; set; }
    }
    public class DPDLoginResponeModel
    {
        public string error { get; set; }
        public DpdDataModel data { get; set; }
    }
    public class DpdDataModel
    {
        public string geoSession { get; set; }
        public bool consolidated { get; set; }
        public string shipmentId { get; set; }
        public List<consignmentDetail> consignmentDetail { get; set; }
    }
    public class consignmentDetail
    {
        public string consignmentNumber { get; set; }
        public List<string> parcelNumbers { get; set; }
    }
   
    
    public class Consignment
    {
        public string consignmentNumber { get; set; }
        public string consignmentRef { get; set; }
        public List<Parcels> parcels { get; set; }
        public CollectionDetails collectionDetails { get; set; }
        public DeliveryDetails deliveryDetails { get; set; }
        public string networkCode { get; set; }
        public int numberOfParcels { get; set; }
        public decimal totalWeight { get; set; }
        public string shippingRef1 { get; set; }
        public string shippingRef2 { get; set; }
        public string shippingRef3 { get; set; }
        public string customsValue { get; set; }
        public string deliveryInstructions { get; set; }
        public string parcelDescription { get; set; }
        public string liabilityValue { get; set; }
        public bool liability { get; set; }

    }
    public class Parcels
    {

    }
    public class CollectionDetails
    {
        public ContactDetails contactDetails { get; set; }
        public Address address { get; set; }
    }

    public class ContactDetails
    {
        public string contactName { get; set; }
        public string telephone { get; set; }
    }
    public class Address
    {
        public string organisation { get; set; }
        public string countryCode { get; set; }
        public string postcode { get; set; }
        public string street { get; set; }
        public string locality { get; set; }
        public string town { get; set; }
        public string county { get; set; }

    }
    public class DeliveryDetails
    {
        public ContactDetails contactDetails { get; set; }
        public Address address { get; set; }
        public NotificationDetails notificationDetails { get; set; }
    }
    public class NotificationDetails
    {
        public string email { get; set; }
        public string mobile { get; set; }
    }


    public class DPDResponseModel
    {
        public List<DpdError> error { get; set; }
        public DpdDataModel data { get; set; }

    }

    public class DpdError
    {
        public string errorCode { get; set; }
        public string errorType { get; set; }
        public string errorMessage { get; set; }
        public string errorAction { get; set; }
        public string Obj { get; set; }
    }
}
