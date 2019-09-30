using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.AU
{
    public class AURequestModel
    {
        public int DirectShipmentDraftid { get; set; }
        public string ClientId { get; set; }
        public string CustomerId { get; set; }
        public string WarehouseId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal Quantity { get; set; }
        public decimal Value { get; set; }
        public Decimal Weight { get; set; }
        public string CollectionDate { get; set; }
        public string DestinationCountry { get; set; }
        public string CarrierType { get; set; }
        public string Currency { get; set; }
        public string ProductDescription { get; set; }
        public ShipToModel ShipTo { get; set; }
        public ShipFromModel ShipFrom { get; set; }

    }
    public class ShipToModel
    {
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }

    }

    public class ShipFromModel
    {
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }

    }

    public class AUResponseModel
    {
        public string InvoiceNumber { get; set; }
        public string TempDownload_Folder { get; set; }
        public string TempDownlaodLabelName { get; set; }
        public bool Status { get; set; }
        public decimal Quantity { get; set; }
        public string Error { get; set; }
    }

}
