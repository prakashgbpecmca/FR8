using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.BreakBulk
{
    public class BreakBulkShipmentDetail
    {
        public int CustomerId { get; set; }
        public int FactoryUserId { get; set; }
        public int POStatusId { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public AddressCollection ShipFrom { get; set; }
        public AddressCollection ShipTo { get; set; }
        public FraytePurchaseOrder PurchaseOrder { get; set; }
        public string CareOf { get; set; }
        public DateTime UpdatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public CustomFieldModel CustomField1 { get; set; }
        public CustomFieldModel CustomField2 { get; set; }
        public CustomFieldModel CustomField3 { get; set; }
        public List<FraytePurchaseOrderDetail> PurchaseOrderDetail { get; set; }
        public FratyteError Error { get; set; }
    }

    public class FraytePurchaseOrder
    {
        public int purchaseOrderId { get; set; }
        public int PONumber { get; set; }
        public int StyleNumber { get; set; }
        public string StyleName { get; set; }
        public string ConsignmentNumber { get; set; }
        public ShipmentHandlerModel ShipmentType { get; set; }
        public string DeliveryType { get; set; }
        public FrayteIncoterm IncotermId { get; set; }
        public DateTime ExFactoryDate { get; set; }
        public string DefaultShipmentType { get; set; }
        public int HubCarrierServiceId { get; set; }
    }

    public class FraytePurchaseOrderDetail
    {
        public int PurchaseOrderDetailId { get; set; }
        public int PurchaseOrderId { get; set; }
        public int ProductCatalogId { get; set; }
        public int JobStatusId { get; set; }
        public int JobNo { get; set; }
        public string JobName { get; set; }
        public string Description { get; set; }
        public int OrderQTY { get; set; }
    }

    public class AddressCollection
    {
        public int AddressBookId { get; set; }
        public int CustomerId { get; set; }
        public FrayteCountryCode Country { get; set; }
        public FrayteAirport Airport { get; set; }
        public string CompanyName { get; set; }
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string Area { get; set; }
        public string State { get; set; }
        public string PhoneNo { get; set; }
        public string PostCode { get; set; }
        public string AlertEmail { get; set; }
        public string AddressType { get; set; }
        public bool IsFavorites { get; set; }
        public bool IsDefaultAddess { get; set; }
    }

    public class FrayteAirport
    {
        public int AirportCodeId { get; set; }
        public string AirportName { get; set; }
    }

    public class FrayteHubAddress
    {
        public int HubId { get; set; }
        public string HubCode { get; set; }
        public FrayteCountryCode Country { get; set; }
        public string PostCode { get; set; }
        public string CompanyName { get; set; }
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Area { get; set; }
        public string PhoneNo { get; set; }
        public string AlertEmail { get; set; }
    }

    public class customerCustomFieldModel
    {
        public int CustomerCustomFieldId { get; set; }
        public int UserId { get; set; }
        public string CustomFieldType { get; set; }
        public string customFieldName { get; set; }
        public string CustomFieldValue { get; set; }
        public string ModuleType { get; set; }
    }

    public class UserAdditionalModel
    {
        public int UserId { get; set; }
        public string DefaultShipmentType { get; set; }
    }

    public class FraytePOviewPurchaseOrder
    {
        public int PurchaseOrderId { get; set; }
        public int PONumber { get; set; }
        public int StyleNumber { get; set; }
        public string StyleName { get; set; }
        public DateTime ExFactoryDate { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public string POStatus { get; set; }
        public string AirportCode { get; set; }
        public int TotalRows { get; set; }
        public List<Jobview> jobs { get; set; }
    }

    public class FrayteJobViewPurchaseOrder
    {
        public int PurchaseOrderDetailId { get; set; }
        public int JobNo { get; set; }
        public int PONumber { get; set; }
        public int StyleNumber { get; set; }
        public int OrderQTY { get; set; }
        public string JobStatus { get; set; }
        public string TrackingNo { get; set; }
        public string ServiceType { get; set; }
        public string Carrier { get; set; }
        public int TotalRows { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }

    public class FraytePurchaseOrderTrack
    {
        public int PoNo { get; set; }
        public int JobNo { get; set; }
        public int styleNo { get; set; }
        public DateTime? ToDate { get; set; }
        public int TakeRows { get; set; }
        public int CurrentPage { get; set; }
        public Nullable<DateTime> FromDate { get; set; }
        public int CustomerId { get; set; }
        public string MAWB { get; set; }
        public int OperationZoneId { get; set; }
        public int ShipmentStatusId { get; set; }
        public string TrackingNo { get; set; }
        public string CustomField { get; set; }
        public string FrayteNumber { get; set; }
    }

    public class Jobview
    {
        public int JobNo { get; set; }
        public string JobName { get; set; }
        public string JobStatus { get; set; }
        public int OrderQTY { get; set; }
        public string TrackingNo { get; set; }
        public string ServiceType { get; set; }
        public string Carrier { get; set; }
    }


    public class BBKShipmentStatus
    {
        public string BBKStatus { get; set; }
        public int ShipmentStatusId { get; set; }
        public string StatusName { get; set; }
        public string BookingType { get; set; }
        public string DisplayStatusName { get; set; }
    }

    public class BBKAirport
    {
        public int AirportCodeId { get; set; }
        public int CountrtId { get; set; }
        public string AirportCode { get; set; }
        public string AirportName { get; set; }
    }

}
