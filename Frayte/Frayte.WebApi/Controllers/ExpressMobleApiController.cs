using Frayte.Services.Business;
using Frayte.Services.Models;
using Frayte.Services.Models.Express;
using Report.Generator.ManifestReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Frayte.WebApi.Controllers
{
    //[Authorize]
    public class ExpressMobleApiController : ApiController
    {
        //[Authorize(Roles = "MobileUser")]
        //Get Side Menu after login

        [HttpGet]
        public LoginScreenModules GetDefaultScreen(string EmailId)
        {
            Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new Exception("Email Id value for GetDefaultScreen Action is : " + EmailId)));
            return new ExpressScannedAWBRepository().GetDefaultScreen(EmailId);
        }

        //Core Pickup awb
        [HttpPost]
        public ScanAwbMobileModel ScanAwb(ScanInitalAwbModel AWBDetail)
        {
            Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new Exception("AWBDetail object for ScanAwb Action is : " + Newtonsoft.Json.JsonConvert.SerializeObject(AWBDetail))));
            return new ExpressScannedAWBRepository().CollectionScanMobileAwb(AWBDetail);
        }

        [HttpGet]
        public List<ScanAwbMobileModel> GetAwb(int ScannedBy, int MobileEventId)
        {
            Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new Exception("ScannedBy and MobileEventId for GetAwb Action is : " + ScannedBy + ", " + MobileEventId)));
            return new ExpressScannedAWBRepository().GetCollectionScanMobileAwb(ScannedBy, MobileEventId);
        }

        //Scan awb at drop off in warehouse and receiving.
        [HttpPost]
        public List<ScanAwbMobileModel> SubmitAwb(ScanAwbList ScanAwbList)
        {
            Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new Exception("ScanAwbList object for SubmitAwb Action is : " + Newtonsoft.Json.JsonConvert.SerializeObject(ScanAwbList))));
            return new ExpressScannedAWBRepository().SubmitAwb(ScanAwbList);
        }

        //Create bag at hub
        [HttpGet]
        public CreateBagModel CreateBag(string AWBNumber, int ScannedBy, int MobileEventId)
        {
            CreateBagModel result = new ExpressScannedAWBRepository().CreateBag(AWBNumber, ScannedBy, MobileEventId);

            if (result.BagId > 0 && result.Status == true && result.IsBagCreated == true)
            {
                var result1 = new TradelaneDocument().ShipmentExportBagNumber(result.BagId, ScannedBy);
                result.FilePath = result1.FilePath;
                result.FileName = result1.FileName;
            }
            Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new Exception("AWBNumber, ScannedBy, MobileEventId values for CreateBag Action is : " + AWBNumber + ", " + ScannedBy + ", " + MobileEventId)));
            return result;
        }

        //Create bag at hub
        [HttpGet]
        public List<BagListModel> GetCreatedBag(int CreatedBy)
        {
            Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new Exception("CreatedBy value for GetCreatedBag Action is : " + CreatedBy)));
            return new ExpressScannedAWBRepository().GetCreatedBag(CreatedBy);
        }

        //Close Bag at hub
        [HttpPost]
        public List<CreateBagModel> AllBagClose(List<int> BagIds)
        {
            List<CreateBagModel> CBMLst = new List<CreateBagModel>();
            CreateBagModel CBM = new CreateBagModel();
            if (BagIds.Count > 0)
            {
                foreach (var Lst in BagIds)
                {
                    var BgCl = BagClose(Lst);
                    CBMLst.Add(BgCl);
                }
            }
            else
            {
                CBM.Status = false;
                CBM.ErrorObject = new List<MobApiErrorObj>();
                MobApiErrorObj AEO = new MobApiErrorObj();
                AEO.ErrorCode = "Don't have BagIds.";
                AEO.ErrorMessage = "Don't have BagIds.";
                CBM.ErrorObject.Add(AEO);
                CBMLst.Add(CBM);
            }
            Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new Exception("BagIds list of object for AllBagClose Action is : " + Newtonsoft.Json.JsonConvert.SerializeObject(BagIds))));
            return CBMLst;
        }

        //Create bag at hub
        [HttpGet]
        public CreateBagModel BagClose(int BagId)
        {
            var BagDtl = new ExpressScannedAWBRepository().BagClose(BagId);

            if (BagDtl.BagId > 0 && BagDtl.Status == true)
            {
                var result1 = new TradelaneDocument().ShipmentBagLabel(BagDtl.BagId, BagDtl.CustomerId);
                BagDtl.FilePath = result1.FilePath;
                BagDtl.FileName = result1.FileName;
            }
            Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new Exception("BagId value for BagClose Action is : " + BagId)));
            return BagDtl;
        }

        [HttpGet]
        public CreateBagModel GetBagLabel(int BagId)
        {
            var BagDtl = new ExpressScannedAWBRepository().GetBagsModel(BagId);
            if (BagDtl.BagId > 0)
            {
                BagDtl.Status = false;
                var result1 = new TradelaneDocument().ShipmentBagLabel(BagDtl.BagId, BagDtl.CustomerId);
                if (!string.IsNullOrEmpty(result1.FileName) && !string.IsNullOrEmpty(result1.FilePath))
                {
                    BagDtl.FilePath = result1.FilePath;
                    BagDtl.FileName = result1.FileName;
                    BagDtl.Status = true;
                }
            }
            else
            {
                BagDtl.Status = false;
            }
            Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new Exception("BagId value for GetBagLabel Action is : " + BagId)));
            return BagDtl;
        }

        //Create bag at hub
        [HttpGet]
        public GetBagsModel OpenBag(int BagId)
        {
            Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new Exception("BagId value for OpenBag Action is : " + BagId)));
            return new ExpressScannedAWBRepository().OpenBag(BagId);
        }

        //Scan Export Manifest
        [HttpGet]
        public ScanExportManifestModel ExportManifestScan(string ExportManifestNumber)
        {
            Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new Exception("ExportManifestNumber value for ExportManifestScan Action is : " + ExportManifestNumber)));
            return new ExpressScannedAWBRepository().ExportManifestScan(ExportManifestNumber);
        }

        [HttpPost]
        public ScanExportManifestModel BagExprortScan(ScanExportManifestModel ScannedBags)
        {
            Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new Exception("ScannedBags object for BagExprortScan Action is : " + Newtonsoft.Json.JsonConvert.SerializeObject(ScannedBags))));
            return new ExpressScannedAWBRepository().BagExportScan(ScannedBags);
        }

        //Scan Driver Manifest
        [HttpGet]
        public ScanDriverManifestModel DriverManifestScan(string DriverManifestNumber)
        {
            Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new Exception("DriverManifestNumber value for DriverManifestScan Action is : " + DriverManifestNumber)));
            return new ExpressScannedAWBRepository().DriverManifestScan(DriverManifestNumber);
        }

        [HttpPost]
        public List<ScanDriverManifestModel> BagDriverScan(ScanDriverManifestModel ScannedBags)
        {
            Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new Exception("ScannedBags object for BagDriverScan Action is : " + Newtonsoft.Json.JsonConvert.SerializeObject(ScannedBags))));
            return new ExpressScannedAWBRepository().BagDriverScan(ScannedBags);
        }

        [HttpPost]
        public ScanDriverManifestModel AWBDriverScan(List<ScanDriverManifestModel> ScannedBags)
        {
            Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new Exception("ScannedBags list of object for AWBDriverScan Action is : " + Newtonsoft.Json.JsonConvert.SerializeObject(ScannedBags))));
            return new ExpressScannedAWBRepository().AWBDriverScan(ScannedBags);
        }

        [HttpPost]
        public ExpressApiErrorModel SavePodDocument(SavePODDocumentModel PODDocuemnt)
        {
            Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new Exception("PODDocuemnt object for SavePodDocument Action is : " + Newtonsoft.Json.JsonConvert.SerializeObject(PODDocuemnt))));
            return new ExpressScannedAWBRepository().SavePodDocument(PODDocuemnt);
        }

        [HttpPost]
        public ExpressApiErrorModel DriverPod(DriverPODModel DriverPOD)
        {
            Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new Exception("DriverPOD object for DriverPod Action is : " + Newtonsoft.Json.JsonConvert.SerializeObject(DriverPOD))));
            return new ExpressScannedAWBRepository().DriverPod(DriverPOD);
        }

        [HttpGet]
        public ZplTwoModel GetZpl2Image()
        {
            return new ExpressScannedAWBRepository().GetZpl2Image();
        }

        #region AvinashCode

        [HttpGet]
        public ReturnShipment CreateReturn(string TrackingNumber)
        {
            Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new Exception("CreateReturn value for CreateReturn Action is : " + TrackingNumber)));
            return new ExpressScannedAWBRepository().CreateReturn(TrackingNumber);
        }

        [HttpGet]
        public ExpressApiErrorModel ConfirmReturn(string AwbNo, bool IsActive)
        {
            Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(new Exception("ConfirmReturn values for ConfirmReturn Action is : " + AwbNo + ", " + IsActive)));
            return new ExpressScannedAWBRepository().ConfirmReturn(AwbNo, IsActive);
        }

        #endregion
    }
}
