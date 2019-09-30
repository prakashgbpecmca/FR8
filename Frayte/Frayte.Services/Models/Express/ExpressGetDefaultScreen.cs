using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.Express
{
    public class ExpressGetDefaultScreen
    {
        public bool IsCollectionDriver { get; set; }
        public bool IsConfirmReceiving { get; set; }
        public bool IsCreateBag { get; set; }
        public bool IsWareHouseDispatch { get; set; }
        public bool IsOriginPickUp { get; set; }
        public bool IsAirportDropOff { get; set; }
        public bool IsAirportPickUp { get; set; }
        public bool IsHandOver { get; set; }
    }

    public class ZplTwoModel
    {
        public string Image1 { get; set; }
        public string Image2 { get; set; }
    }

    public class LoginScreenModules : ExpressApiErrorModel
    {
        public List<MasterTrackingModel> MasterTracking { get; set; }
    }

    public class SubModules
    {
        public bool IsDefault { get; set; }
        public string SubModuleName { get; set; }
        public int SubModuleId { get; set; }
    }

    public class ScanInitalAwbModel
    {
        public string AwbNumber { get; set; }
        public int MobileEventId { get; set; }
        public byte[] Photo { get; set; }
        public int ScannedBy { get; set; }
        public string Lattitude { get; set; }
        public string Longitude { get; set; }
    }

    public class ScanAwbMobileModel : ExpressApiErrorModel
    {
        public int ExpressId { get; set; }
        public string AwbNumber { get; set; }
        public string DriverName { get; set; }
    }

    public class ExpressApiErrorModel
    {
        public bool Status { get; set; }
        public List<MobApiErrorObj> ErrorObject { get; set; }
    }

    public class MobApiErrorObj
    {
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class CreateBagModel : ExpressApiErrorModel
    {
        public int BagId { get; set; }
        public int CustomerId { get; set; }
        public string BagNumber { get; set; }
        public string Carrier { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string HubName { get; set; }
        public string HubCode { get; set; }
        public int TotalShipments { get; set; }
        public bool IsClosed { get; set; }
        public bool IsBagCreated { get; set; }
        public decimal TotalWeight { get; set; }
        public bool IsBagFull { get; set; }
        public decimal RemainingWeight { get; set; }
        public bool IsBagExpired { get; set; }
        public bool IsError { get; set; }
    }

    public class ScanAwbList
    {
        public int ScannedBy { get; set; }
        public int MobileId { get; set; }
        public string Lattitude { get; set; }
        public string Longitude { get; set; }
        public List<AWBs> AwbNos { get; set; }
    }

    public class AWBs
    {
        public string AWBNumber { get; set; }
        public bool IsScanned { get; set; }
    }

    public class Couriers : ExpressApiErrorModel
    {
        public string CourierNumber { get; set; }
        public bool IsScanned { get; set; }
        public bool IsBagExpired { get; set; }
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class BagListModel
    {
        public int HubId { get; set; }
        public string HubName { get; set; }
        public string Code { get; set; }
        public List<CreateBagModel> BagInfo { get; set; }
    }

    public class GetBagsModel : ExpressApiErrorModel
    {
        public int BagId { get; set; }
        public string BagNumber { get; set; }
        public List<AWBs> AWB { get; set; }
    }

    public class ScanExportManifestModel : ExpressApiErrorModel
    {
        public int ExportManifestId { get; set; }
        public int ScannedBy { get; set; }
        public string ExportManifestNumber { get; set; }
        public int MobileEventId { get; set; }
        public string Lattitude { get; set; }
        public string Longitude { get; set; }
        public List<BagsModel> Bags { get; set; }
    }

    public class BagsModel : ExpressApiErrorModel
    {
        public int BagId { get; set; }
        public string BagNumber { get; set; }
        public string Carrier { get; set; }
        public byte[] Photo { get; set; }
        public bool IsScanned { get; set; }
        public int TotalShipments { get; set; }
        public bool IsError { get; set; }
        public bool IsBagExpired { get; set; }
        public string ErrorMessage { get; set; }
        public List<Couriers> Couriers { get; set; }
    }

    public class ScanDriverManifestModel : ExpressApiErrorModel
    {
        public int DriverManifestId { get; set; }
        public string Carrier { get; set; }
        public int ScannedBy { get; set; }
        public int MobileEventId { get; set; }
        public string Lattitude { get; set; }
        public string Longitude { get; set; }
        public List<BagsModel> Bags { get; set; }
    }

    public class SavePODDocumentModel
    {
        public int BagId { get; set; }
        public byte[] Photo { get; set; }
    }

    public class DriverPODModel
    {
        public int DriverManifestId { get; set; }
        public int ScannedBy { get; set; }
        public byte[] Signature { get; set; }
        public string SignedBy { get; set; }
    }

    //Return Mobile Event 
    public class ReturnShipment : ExpressApiErrorModel
    {
        public string TrackingNo { get; set; }
        public string BagNo { get; set; }
        public string AwbNo { get; set; }
        public string Carrier { get; set; }
        public ReturnShipmentAddress DeliveryAddress { get; set; }
    }

    public class ReturnShipmentAddress
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string Area { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostCode { get; set; }
        public string CountryName { get; set; }
    }

}
