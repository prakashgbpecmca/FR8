using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frayte.Services.Models;
using System.Data.Entity;
using Frayte.Services.DataAccess;
using Spire.Barcode;
using System.Drawing;
using System.Web;
using Frayte.Services.Utility;
namespace Frayte.Services.Business

{
    public class eCommerceAppRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        #region Scan Manifest
        public AppResult ScanManifest(int userId, List<string> manifestBarcodeNumbers)
        {
            AppResult result = new AppResult();
            result.Messages = new List<ErrorMessage>();

            ErrorMessage message;
            foreach (var manifestBarcodeNumber in manifestBarcodeNumbers)
            {
                try
                {
                    if (!string.IsNullOrEmpty(manifestBarcodeNumber))
                    {
                        message = SetManifestStatus(manifestBarcodeNumber);
                    }
                    else
                    {
                        message = new ErrorMessage();
                        message.Values.Add(eCommerceAppErrorMessage.InvalidBarcode);
                    }
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Exception Scan Manifest : " + ex.Message));
                    message = new ErrorMessage();
                    message.Values.Add(eCommerceAppErrorMessage.ErrorProcessingBarcode);
                }
                if (message.Values != null && message.Values.Count > 0)
                {
                    result.Messages.Add(message);
                }
            }

            if (result.Messages != null && result.Messages.Count > 0)
            {
                result.Status = false;
            }
            else
            {
                result.Status = true;
            }
            return result;
        }


        #region Private Methods
        private ErrorMessage SetManifestStatus(string manifestBarcodeNumber)
        {

            ErrorMessage message = new ErrorMessage();
            message.Values = new List<string>();
            var manifestDetail = dbContext.Manifests.Where(p => p.BarCodeNumber == manifestBarcodeNumber).FirstOrDefault();
            if (manifestDetail != null)
            {
                manifestDetail.Status = eCommerceAppManifestTracking.Tracking1;
                dbContext.Entry(manifestDetail).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
            else
            {
                message.Key = manifestBarcodeNumber;

                message.Values.Add(eCommerceAppErrorMessage.InvalidBarcode);
            }
            return message;
        }

        #endregion
        #endregion

        #region Receive Shipment 


        public AppResult ReceiveShipments(int userId, List<string> shipmentBarcodeNumbers)
        {
            AppResult result = new AppResult();
            result.Messages = new List<ErrorMessage>();

            if (shipmentBarcodeNumbers != null && shipmentBarcodeNumbers.Count > 0)
            {
                ErrorMessage message;
                foreach (var barcode in shipmentBarcodeNumbers)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(barcode))
                        {
                            message = ReceiveShipment(userId, barcode);
                        }
                        else
                        {
                            message = new ErrorMessage();
                            message.Values.Add(eCommerceAppErrorMessage.InvalidBarcode);
                        }

                    }
                    catch (Exception ex)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Scan Parcel : " + ex.Message));
                        message = new ErrorMessage();
                        message.Key = barcode;
                        message.Values.Add(eCommerceAppErrorMessage.ErrorProcessingBarcode);
                    }

                    if (message.Values != null && message.Values.Count > 0)
                    {
                        result.Status = false;
                        result.Messages.Add(message);
                    }
                    else
                    {
                        result.Status = true;
                    }
                }
            }
            else
            {
                result.Status = false;
            }


            return result;
        }


        #region Private Scan Parcel

        private ErrorMessage ReceiveShipment(int userId, string shipmentBarcodeNumber)
        {

            ErrorMessage message = new ErrorMessage();
            message.Values = new List<string>();

            try
            {
                if (!string.IsNullOrEmpty(shipmentBarcodeNumber))
                {
                    var shipmentDetail = dbContext.eCommerceShipments.Where(p => p.BarCodeNumber == shipmentBarcodeNumber).FirstOrDefault();
                    if (shipmentDetail == null)
                    {
                        message.Key = shipmentBarcodeNumber;
                        message.Values.Add(eCommerceAppErrorMessage.InvalidBarcode);
                    }
                    else
                    {

                        shipmentDetail.ScanedOn = DateTime.UtcNow;
                        shipmentDetail.ScanedBy = userId;
                        dbContext.Entry(shipmentDetail).State = EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                    // Later  eCommerce tracking  
                    //  Result = SetShipmentTracking(shipmentBarcodeNumber);
                }
                else
                {
                    message.Key = shipmentBarcodeNumber;
                    message.Values.Add(eCommerceAppErrorMessage.InvalidBarcode);
                }
            }
            catch (Exception ex)
            {
                message.Key = shipmentBarcodeNumber;
                message.Values.Add(eCommerceAppErrorMessage.ErrorProcessingBarcode);
            }

            return message;
        }



        private eCommerceAppResult SetShipmentTracking(string shipmentBarcodeNumber)
        {
            eCommerceAppResult Result = new eCommerceAppResult();
            Result.Messages = new List<string>();

            var shipmentDetail = dbContext.eCommerceShipments.Where(p => p.BarCodeNumber == shipmentBarcodeNumber).FirstOrDefault();
            if (shipmentDetail != null)
            {
                eCommerceTracking track = new eCommerceTracking();
                track.TrackingDescriptionCode = eCommerceAppShipmentTracking.Tracking1;
                track.TrackingDescription = eCommerceAppShipmentTracking.Tracking1;
                track.eCommerceShipmentId = shipmentDetail.eCommerceShipmentId;
                track.FrayteNumber = shipmentDetail.FrayteNumber;

                var shipmentPackageDetail = dbContext.eCommerceShipmentDetails.Where(p => p.eCommerceShipmentId == shipmentDetail.eCommerceShipmentId).ToList();

                if (shipmentPackageDetail != null && shipmentPackageDetail.Count > 0)
                {
                    int id = shipmentPackageDetail[0].eCommerceShipmentDetailId;
                    var data = dbContext.eCommercePackageTrackingDetails.Where(p => p.eCommerceShipmentDetailId == id).FirstOrDefault();
                    if (data != null)
                    {
                        track.TrackingNumber = data.TrackingNo;
                    }

                }
                track.CreatedOnUtc = DateTime.UtcNow;
                dbContext.eCommerceTrackings.Add(track);
                dbContext.SaveChanges();
                Result.Status = true;
            }
            else
            {
                Result.Status = false;
                Result.Messages.Add(eCommerceAppErrorMessage.InvalidBarcode);
            }
            return Result;
        }
        #endregion

        #endregion

        #region Create Bags

        public BagResult CreateBag(FrayteeCommerceBag bag)
        {
            // If bagmanifest is not available and BagClosed is false the create a new bag manifest and create a new  bag for shipments
            // If bagmanifest is available and BagClosed is false the create new bag 
            // If bagmanifest is available and BagClosed is true the create new bag 

            BagResult result = new BagResult();
            try
            {
                result.Messages = new List<ErrorMessage>();

                string bagManifestName = string.Empty;

                if (string.IsNullOrEmpty(bag.BagManifest) || bag.BagClosed)
                {
                    // create a new bag manifest
                    bagManifestName = createBagManifest(bag);
                    // create bag 
                }
                else
                {
                    bagManifestName = bag.BagManifest;
                }
                int id = createBag(bagManifestName);
                // add shipments in the bag
                addShipmentsInBag(bag, id);
                result.Status = true;
                result.ManifestNumber = bagManifestName;
            }
            catch (Exception ex)
            {
                result.Status = false;
            }
            return result;
        }



        //public eCommerceAppResult CreateBag(eCommerceBag bagDetail)
        //{
        //    eCommerceAppResult Result = new eCommerceAppResult();
        //    Result.Messages = new List<string>();

        //    try
        //    {
        //        if (bagDetail != null)
        //        {

        //            CreateShipmentBag(bagDetail);
        //        }

        //        else
        //        {
        //            Result.Status = false;
        //            Result.Messages.Add(eCommerceAppErrorMessage.InvalidBarcode);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Result.Status = false;
        //        Result.Messages.Add(eCommerceAppErrorMessage.ErrorProcessing);
        //    }

        //    return Result;
        //}

        #region Private Methods CreateBag , Barcode ,Label  BagDetail

        private void addShipmentsInBag(FrayteeCommerceBag bag, int id)
        {
            if (bag != null && bag.ShipmentBarcodes != null && bag.ShipmentBarcodes.Count > 0)
            {
                foreach (var ship in bag.ShipmentBarcodes)
                {
                    var shipment = dbContext.eCommerceShipments.Where(p => p.BarCodeNumber == ship).FirstOrDefault();
                    if (shipment != null)
                    {
                        shipment.eCommerceBagId = id;
                        dbContext.SaveChanges();
                    }
                }
            }
        }

        private int createBag(string bagMnaifestName)
        {
            int id = 0;
            try
            {
                eCommerceBag bag = new eCommerceBag();
                var bagManifest = dbContext.eCommerceBagManifests.Where(p => p.Name == bagMnaifestName).FirstOrDefault();

                bag.CreatedOn = DateTime.UtcNow;
                if (bagManifest != null)
                    bag.eCommerceBagManiferstId = bagManifest.eCommerceBagManifestId;
                dbContext.eCommerceBags.Add(bag);
                dbContext.SaveChanges();
                id = bag.eCommerceBagId;
            }
            catch (Exception ex)
            {
                id = 0;
            }

            return id;
        }

        private string createBagManifest(FrayteeCommerceBag bag)
        {
            eCommerceBagManifest bagManifest;

            string bagMnaifestName = string.Empty;
            if (bag != null)
            {
                if (string.IsNullOrEmpty(bag.BagManifest))
                {
                    // FIRST CREATE BAG MANIFEST 
                    bagManifest = new eCommerceBagManifest();
                    bagManifest.CreatedOn = DateTime.UtcNow;
                    bagManifest.IsClosed = bag.BagClosed;
                    dbContext.eCommerceBagManifests.Add(bagManifest);
                    dbContext.SaveChanges();
                }
                else
                {
                    bagManifest = dbContext.eCommerceBagManifests.Where(p => p.Name == bag.BagManifest).FirstOrDefault();

                }

                // THEN UPDATE BAG MANIFEST 
                if (bagManifest != null)
                {
                    bagManifest.Name = "BM-UK-" + bagManifest.eCommerceBagManifestId;
                    bagManifest.IsClosed = bag.BagClosed;
                    dbContext.SaveChanges();
                    bagMnaifestName = bagManifest.Name;
                    dbContext.SaveChanges();
                }

            }
            return bagMnaifestName;
        }

        //private eCommerceAppResult CreateShipmentBag(eCommerceBag bagDetail)
        //{
        //    eCommerceAppResult result = new eCommerceAppResult();
        //    result.Messages = new List<string>();

        //    ShipmentBag bag = new ShipmentBag();
        //    bag.BagName = bagDetail.BagNumber;
        //    bag.ModuleType = FrayteShipmentServiceType.eCommerce;
        //    bag.CreatedOn = DateTime.UtcNow;
        //    dbContext.ShipmentBags.Add(bag);
        //    dbContext.SaveChanges();
        //    SetBagBarcode(bag, bagDetail);
        //    SaveShipmentsInBag(bag, bagDetail);
        //    result.Status = true;
        //    return result;
        //}

        //private void SetBagBarcode(ShipmentBag bag, eCommerceBag bagDetail)
        //{
        //    string bar = string.Empty;
        //    bar = bag.ShipmentBagId.ToString() + bagDetail.ShipmentBarcodeNumbers.Count;

        //    // To Do : add Desination Country Code in barcode 
        //    // Find destination country code dfrom barcode (Last three character)
        //    var id = bagDetail.ShipmentBarcodeNumbers[0];
        //    var shipmentData = (from r in dbContext.eCommerceShipments
        //                        join da in dbContext.eCommerceShipmentAddresses on r.ToAddressId equals da.eCommerceShipmentAddressId
        //                        join cc in dbContext.Countries on da.CountryId equals cc.CountryId
        //                        where r.BarCodeNumber == id
        //                        select new FrayteCountryCode
        //                        {
        //                            Code = cc.CountryCode,
        //                            Code2 = cc.CountryCode2,
        //                            CountryId = cc.CountryId,
        //                            Name = cc.CountryName
        //                        }
        //                 ).FirstOrDefault();

        //    if (shipmentData != null)
        //    {
        //        bar += shipmentData.Code;
        //    }

        //    BarcodeSettings settings = new BarcodeSettings();
        //    string data = string.Empty;
        //    string type = "Code128";
        //    short fontSize = 8;
        //    string font = "SimSun";
        //    data = bar;

        //    settings.Data2D = data;
        //    settings.Data = data;
        //    settings.Type = (BarCodeType)Enum.Parse(typeof(BarCodeType), type);

        //    if (fontSize != 0 && fontSize.ToString().Length > 0 && Int16.TryParse(fontSize.ToString(), out fontSize))
        //    {
        //        if (font != null && font.Length > 0)
        //        {
        //            settings.TextFont = new System.Drawing.Font(font, fontSize, FontStyle.Bold);
        //        }
        //    }
        //    short barHeight = 15;
        //    if (barHeight != 0 && barHeight.ToString().Length > 0 && Int16.TryParse(barHeight.ToString(), out barHeight))
        //    {
        //        settings.BarHeight = barHeight;
        //    }

        //    BarCodeGenerator generator = new BarCodeGenerator(settings);
        //    Image barcode = generator.GenerateImage();

        //    // Path where we will have barcode 
        //    string filePathToSave = AppSettings.eCommerceBag + bag.ShipmentBagId.ToString();
        //    filePathToSave = HttpContext.Current.Server.MapPath(filePathToSave);

        //    if (!System.IO.Directory.Exists(filePathToSave))
        //    {
        //        System.IO.Directory.CreateDirectory(filePathToSave);
        //    }

        //    barcode.Save(System.Web.Hosting.HostingEnvironment.MapPath(AppSettings.eCommerceBag + bag.ShipmentBagId.ToString() + "/" + bag.BagName + ".Png"));

        //    bag.Barcode = settings.Data;
        //    dbContext.Entry(bag).State = System.Data.Entity.EntityState.Modified;
        //    dbContext.SaveChanges();
        //}

        //private void SaveShipmentsInBag(ShipmentBag bag, eCommerceBag bagDetail)
        //{
        //    foreach (var data in bagDetail.ShipmentBarcodeNumbers)
        //    {
        //        var ship = dbContext.eCommerceShipments.Where(p => p.BarCodeNumber == data).FirstOrDefault();
        //        if (ship != null)
        //        {
        //            var shipDetail = dbContext.eCommerceShipmentDetails.Where(p => p.eCommerceShipmentId == ship.eCommerceShipmentId).ToList();
        //            if (shipDetail != null && shipDetail.Count > 0)
        //            {
        //                int total = 0;
        //                foreach (var c in shipDetail)
        //                {
        //                    total += c.CartoonValue;
        //                }
        //                ShipmentBagDetail shipmentBagDetail = new ShipmentBagDetail();
        //                shipmentBagDetail.ShipmentBagId = bag.ShipmentBagId;
        //                shipmentBagDetail.ShipmentId = ship.eCommerceShipmentId;
        //                shipmentBagDetail.ModuleType = FrayteShipmentServiceType.eCommerce;
        //                shipmentBagDetail.FrayteAWB = ship.FrayteNumber;
        //                shipmentBagDetail.CartonQty = total;
        //                dbContext.ShipmentBagDetails.Add(shipmentBagDetail);
        //                dbContext.SaveChanges();
        //            }

        //        }
        //    }




        //}

        //public ShipmentBag GetBarcodePath(string number)
        //{
        //    var data = dbContext.ShipmentBags.Where(p => p.BagName == number).FirstOrDefault();
        //    return data;
        //}

        //public FrayteCountryCode DestinationCountry(eCommerceBag bagDetail)
        //{
        //    var id = bagDetail.ShipmentBarcodeNumbers[0];
        //    var shipmentData = (from r in dbContext.eCommerceShipments
        //                        join da in dbContext.eCommerceShipmentAddresses on r.ToAddressId equals da.eCommerceShipmentAddressId
        //                        join cc in dbContext.Countries on da.CountryId equals cc.CountryId
        //                        where r.BarCodeNumber == id
        //                        select new FrayteCountryCode
        //                        {
        //                            Code = cc.CountryCode,
        //                            Code2 = cc.CountryCode2,
        //                            CountryId = cc.CountryId,
        //                            Name = cc.CountryName
        //                        }
        //               ).FirstOrDefault();
        //    return shipmentData;
        //}
        #endregion

        #endregion

        #region DepartureParcels 
        public AppResult DepartureParcels(int userId)
        {
            AppResult result = new AppResult();
            result = Departure(userId);
            return result;
        }

        #region Private Methods 
        private AppResult Departure(int userId)
        {
            AppResult result = new AppResult();
            try
            {
                var shipments = dbContext.eCommerceShipments.Where(p => p.ScanedBy != null &&
           p.ScanedBy == userId && p.ScanedOn != null &&
           p.ShipmentStatusId != (int)FrayteShipmentStatus.eCDepartured).ToList();
                if (shipments != null && shipments.Count > 0)
                {
                    foreach (var data in shipments)
                    {
                        data.ShipmentStatusId = (int)FrayteShipmentStatus.eCDepartured;
                        dbContext.Entry(data).State = EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                    result.Status = true;
                }
                else
                {
                    result.Status = false;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                result.Status = false;
            }
            return result;
        }
        #endregion
        #endregion

        #region Tax and Duty Status

        public TaxAndDutyResult getTaxAndDutyStatus(eCommBarcode barCode)
        {
            TaxAndDutyResult result = new TaxAndDutyResult();
            result = TaxAndDutyStatus(barCode);
            return result;
        }

        public AppResult HandOverParcels(ScanBarcode parcels)
        {
            AppResult result = new AppResult();
            try
            {
                if (parcels != null && parcels.Barcodes != null && parcels.Barcodes.Count > 0)
                {
                    foreach (var data in parcels.Barcodes)
                    {
                        var shipment = dbContext.eCommerceShipments.Where(P => P.BarCodeNumber == data).FirstOrDefault();
                        if (shipment != null)
                        {
                            shipment.ShipmentStatusId = (int)FrayteShipmentStatus.eCHandOver;
                            shipment.WarehouseLocationId = null;
                            dbContext.Entry(shipment).State = EntityState.Modified;
                            dbContext.SaveChanges();
                        }
                    }
                    result.Status = true;
                }
                else
                {
                    result.Status = false;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                result.Status = false;
            }

            return result;
        }

        #region Private Methods
        private TaxAndDutyResult TaxAndDutyStatus(eCommBarcode barCode)
        {
            TaxAndDutyResult result = new TaxAndDutyResult();
            try
            {
                //   var data = (from r in dbContext.eCommerceShipments
                //               join ecV in dbContext.eCommerceInvoices on r.eCommerceShipmentId equals ecV.ShipmentId
                //               where r.BarCodeNumber == barCode.Barcode
                //               select r
                //).FirstOrDefault();

                if (barCode != null && !string.IsNullOrEmpty(barCode.Barcode))
                {

                    if (barCode.Barcode == "16460533|GBR|3" || barCode.Barcode == "45241162|GBR|3")
                    {
                        result.Status = true;
                        result.PaymentStatus = eCommerceAppTaxAndDutyStatus.TaxAndDutyPaid;
                    }
                    else if (barCode.Barcode == "24559515|GBR|1" || barCode.Barcode == "22785328|GBR|6")
                    {
                        result.Status = true;
                        result.PaymentStatus = eCommerceAppTaxAndDutyStatus.TaxAndDutyPartiallyPaid;
                    }
                    else
                    {
                        result.Status = false;
                        result.PaymentStatus = eCommerceAppTaxAndDutyStatus.TaxAndDutyUnPaid;
                    }
                }
                else
                {
                    result.Status = false;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
            }
            return result;
        }
        #endregion
        #endregion

        #region Manage WareHouse 

        public FrayteResult CreateWarehouseLocation(string LocationName, string countryCode)
        {
            FrayteResult result = new FrayteResult();

            Location location = new Location();
            location.WarehouseId = 1;
            location.LocationName = LocationName;
            location.CountryId = 228;
            dbContext.Locations.Add(location);
            dbContext.SaveChanges();

            location.Barcode = CommonConversion.GetNewFrayteNumber() + "|" + countryCode + "|" + location.LocationId;
            dbContext.SaveChanges();

            BarcodeSettings settings = new BarcodeSettings();
            string data = location.Barcode;
            string type = "Code128";
            short fontSize = 8;
            string font = "SimSun";

            settings.Data2D = data;
            settings.Data = data;
            settings.Type = (BarCodeType)Enum.Parse(typeof(BarCodeType), type);

            if (fontSize != 0 && fontSize.ToString().Length > 0 && Int16.TryParse(fontSize.ToString(), out fontSize))
            {
                if (font != null && font.Length > 0)
                {
                    settings.TextFont = new System.Drawing.Font(font, fontSize, FontStyle.Bold);
                }
            }
            short barHeight = 15;
            if (barHeight != 0 && barHeight.ToString().Length > 0 && Int16.TryParse(barHeight.ToString(), out barHeight))
            {
                settings.BarHeight = barHeight;
            }

            BarCodeGenerator generator = new BarCodeGenerator(settings);
            Image barcode = generator.GenerateImage();

            // Path where we will have barcode 
            string filePathToSave = AppSettings.LabelFolder + "Waerehouse/";
            filePathToSave = HttpContext.Current.Server.MapPath(filePathToSave + location.LocationId);

            if (!System.IO.Directory.Exists(filePathToSave))
                System.IO.Directory.CreateDirectory(filePathToSave);
            barcode.Save(filePathToSave + "/" + location.LocationName + ".Png");
            result.Status = true;


            return result;
        }

        public List<FrayteWarehouseLocation> GetAllLocations(int userId)
        {
            List<FrayteWarehouseLocation> locations = new List<FrayteWarehouseLocation>();
            try
            {
                var OpearatiopZone = UtilityRepository.GetOperationZone();
                var Country = dbContext.Countries.Where(p => p.CountryCode == OpearatiopZone.OperationZoneName).FirstOrDefault();
                var list = dbContext.Locations.Where(p => p.CountryId == Country.CountryId).ToList();
                if (list != null && list.Count > 0)
                {
                    FrayteWarehouseLocation location;
                    foreach (var data in list)
                    {
                        location = new FrayteWarehouseLocation();
                        location.LocationName = data.LocationName;
                        location.Barcode = data.Barcode;
                        locations.Add(location);
                    }
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }

            return locations;
        }

        public List<FrayteAWBWithLocation> SearchAWBLocation(string searchText)
        {

            List<FrayteAWBWithLocation> list = new List<FrayteAWBWithLocation>();
            var query = (from r in dbContext.eCommerceShipments
                         join l in dbContext.Locations on r.WarehouseLocationId equals l.LocationId
                         where r.FrayteNumber.Contains(searchText)
                         select new
                         {
                             LocationId = l.LocationId,
                             LocationName = l.LocationName,
                             LocationBarcode = l.Barcode,
                             ShipmentBarcode = r.BarCodeNumber,
                             FrayteAWB = r.FrayteNumber
                         }).ToList();

            FrayteAWBWithLocation location;
            foreach (var data in query)
            {
                location = new FrayteAWBWithLocation();
                location.LocationName = data.LocationName;
                location.LocationBarcode = data.LocationBarcode;
                location.ShipmentBarcode = data.ShipmentBarcode;
                location.FrayteAWB = data.FrayteAWB;
                list.Add(location);
            }

            return list;

        }
        public List<eCommBarcode> GetLocationShipments(string warehouseLocation)
        {
            List<eCommBarcode> list = new List<eCommBarcode>();

            var shipments = (from r in dbContext.Locations
                             join es in dbContext.eCommerceShipments on r.LocationId equals es.WarehouseLocationId
                             where r.Barcode == warehouseLocation
                             select es
                              ).ToList();

            if (shipments != null)
            {
                eCommBarcode ship;
                foreach (var data in shipments)
                {
                    ship = new eCommBarcode();
                    ship.Barcode = data.BarCodeNumber;
                    list.Add(ship);
                }
            }
            return list;
        }
        public AppResult ManageParcelLocation(eCommShipmentLocation shipmentsLocations)
        {
            AppResult result = new AppResult();
            try
            {
                if (shipmentsLocations.UserId > 0 && !string.IsNullOrEmpty(shipmentsLocations.LocationBarcode) && shipmentsLocations.ShipmentBarcodes != null && shipmentsLocations.ShipmentBarcodes.Count > 0)
                {
                    foreach (var data in shipmentsLocations.ShipmentBarcodes)
                    {
                        var query1 = dbContext.eCommerceShipments.Where(p => p.BarCodeNumber == data).FirstOrDefault();
                        var query2 = dbContext.Locations.Where(p => p.Barcode == shipmentsLocations.LocationBarcode).FirstOrDefault();
                        if (query1 != null && query2 != null)
                        {
                            query1.WarehouseLocationId = query2.LocationId;
                            dbContext.SaveChanges();
                            result.Status = true;
                        }
                        else
                        {
                            result.Status = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                result.Status = false;
            }

            return result;
        }


        #endregion
    }
}
