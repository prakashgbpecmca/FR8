using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Frayte.Services.Business;
using Frayte.Services.Models;
using Frayte.Services.Utility;
using Report.Generator.ManifestReport;
using System.Web;
namespace Frayte.WebApi.Controllers
{
    [Authorize]
    public class eCommerceAppController : ApiController
    {

        #region Consolidator and Warrehouse Agent API

        #region Scan Manifest
        [AllowAnonymous]
        [HttpPost]
        public AppResult ScanManifest(ScanBarcode scanBarcodes)
        {
            AppResult result = new eCommerceAppRepository().ScanManifest(scanBarcodes.UserId, scanBarcodes.Barcodes);
            return result;
        }

        //[HttpPost]
        //public AppResult ScanManifest(List<string> manifestBarcodeNumbers)
        //{
        //    AppResult result = new eCommerceAppRepository().ScanManifest(manifestBarcodeNumbers);
        //    return result;
        //}

        #endregion

        #region Scan Parcel
        [AllowAnonymous]
        [HttpPost]
        public AppResult ScanParcels(ScanBarcode scanBarcodes)
        {
            var Result = new eCommerceAppRepository().ReceiveShipments(scanBarcodes.UserId, scanBarcodes.Barcodes);

            return Result;
        }

        //[HttpPost]
        //public AppResult ScanParcels(List<string> shipmentBarcodes)
        //{
        //    var Result = new eCommerceAppRepository().ReceiveShipments(0, shipmentBarcodes);
        //    return Result;
        //}

        #endregion

        #region Create Bag
        [AllowAnonymous]
        [HttpPost]
        public BagResult CreateBag(FrayteeCommerceBag bag)
        {
            BagResult result = new BagResult();

            result = new eCommerceAppRepository().CreateBag(bag);
            if (bag.BagClosed)
            {
                result.ManifestNumber = "";
            }
            return result;
        }

        //public eCommerceAppResult CreateBag(List<FrayteECommerceBag> bagDetail)
        //{
        //    try
        //    {
        //        eCommerceAppResult Result = new eCommerceAppResult();
        //        int no = 0;
        //        foreach (var d in bagDetail)
        //        {
        //            no++;
        //            Result = new eCommerceAppRepository().CreateBag(d);
        //            // Create label for each bag
        //            GenerateeCommercebagLabel(d, no, bagDetail.Count);
        //        }
        //        return Result;
        //    }
        //    catch (Exception ex)
        //    {
        //        return new eCommerceAppResult()
        //        {
        //            Status = false,
        //            Messages = new List<string>() { eCommerceAppErrorMessage.ErrorProcessing }
        //        };
        //    }

        //}
        //private FrayteResult GenerateeCommercebagLabel(FrayteECommerceBag bagDetail, int currentBag, int totalBags)
        //{
        //    FrayteResult result = new FrayteResult();
        //    eCommerceReportBagLabel eCommerceBagLabelReport = new eCommerceReportBagLabel();

        //    eCommerceBagLabelReport.TotalBags = totalBags;
        //    eCommerceBagLabelReport.CurrentBag = currentBag;
        //    eCommerceBagLabelReport.CreatedOn = DateTime.UtcNow.Date;
        //    eCommerceBagLabelReport.AgentName = "";
        //    eCommerceBagLabelReport.MAWB = "";
        //    eCommerceBagLabelReport.FlightNumber = "";

        //    var data = new eCommerceAppRepository().GetBarcodePath(bagDetail.BagNumber);
        //    if (data != null)
        //    {
        //        eCommerceBagLabelReport.BarCodePath = HttpContext.Current.Server.MapPath(AppSettings.eCommerceBag) + data.ShipmentBagId.ToString() + "/" + data.BagName + ".Png";
        //        eCommerceBagLabelReport.ShipmentBagId = data.ShipmentBagId;
        //    }

        //    var shipmentData = new eCommerceAppRepository().DestinationCountry(bagDetail);

        //    eCommerceBagLabelReport.DestinationCountry = new FrayteCountryCode();
        //    eCommerceBagLabelReport.DestinationCountry.Code = shipmentData.Code;
        //    eCommerceBagLabelReport.DestinationCountry.Code2 = shipmentData.Code2;
        //    eCommerceBagLabelReport.DestinationCountry.Name = shipmentData.Name;
        //    eCommerceBagLabelReport.DestinationCountry.CountryId = shipmentData.CountryId;

        //    new eCommerceBagLabelReport().generateeCommerceBagLabelReport(eCommerceBagLabelReport);

        //    return result;
        //}

        #endregion

        #region Departure Parcels
        [AllowAnonymous]
        [HttpGet]
        public AppResult DepartureParcels(int userId)
        {
            AppResult result = new AppResult();
            result = new eCommerceAppRepository().DepartureParcels(userId);
            return result;
        }

        #endregion

        #region Tax Duty Paid
        [HttpPost]
        [AllowAnonymous]
        public TaxAndDutyResult ParcelTaxAndDuty(eCommBarcode barCode)
        {
            TaxAndDutyResult result = new TaxAndDutyResult();

            result = new eCommerceAppRepository().getTaxAndDutyStatus(barCode);

            return result;
        }

        #endregion

        #region HandOver To Local Courier
        [HttpPost]
        [AllowAnonymous]
        public AppResult HandOverParcels(ScanBarcode parcels)
        {
            AppResult result = new eCommerceAppRepository().HandOverParcels(parcels);
            return result;
        }
        #endregion
        #endregion

        #region Manage WareHouse 
        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult CreateWarehouseLocation(string locationName, string countryCode)
        {
            FrayteResult result = new eCommerceAppRepository().CreateWarehouseLocation(locationName, countryCode);
            return Ok(result);
        }
        [HttpGet]
        [AllowAnonymous]
        public List<FrayteWarehouseLocation> GetAllLocations(int userId)
        {
            var result = new eCommerceAppRepository().GetAllLocations(userId);
            return result;
        }

        [HttpPost]
        [AllowAnonymous]
        public AppResult ManageParcelLocation(eCommShipmentLocation shipmentsLocations)
        {
            AppResult result = new AppResult();
            result = new eCommerceAppRepository().ManageParcelLocation(shipmentsLocations);
            return result;
        }
        [HttpGet]
        [AllowAnonymous]
        public List<eCommBarcode> GetLocationShipments(string warehouseLocation)
        {
            List<eCommBarcode> list = new eCommerceAppRepository().GetLocationShipments(warehouseLocation);
            return list;
        }

        [HttpGet]
        [AllowAnonymous]
        public List<FrayteAWBWithLocation> SearchAWBLocation(string serachText)
        {
            var list = new eCommerceAppRepository().SearchAWBLocation(serachText);
            return list;
        }
        #endregion
    }

}
