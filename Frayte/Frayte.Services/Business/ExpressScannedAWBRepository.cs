using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using Frayte.Services.Models.Express;
using System.IO;
using System.Drawing;
using Frayte.Services.Utility;
using System.Web.Hosting;
using System.Web;
using System.Text.RegularExpressions;

namespace Frayte.Services.Business
{
    public class ExpressScannedAWBRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        public List<ExpressScannedAwbModel> GetScannedAWB(ExpressAWBTrackModel track)
        {
            List<ExpressScannedAwbModel> Result = new List<ExpressScannedAwbModel>();
            int OperationZone = UtilityRepository.GetOperationZone().OperationZoneId;
            try
            {
                int SkipRows = 0;
                SkipRows = (track.CurrentPage - 1) * track.TakeRows;
                if (track.CustomerId > 0)
                {
                    Result = (from Exp in dbContext.Expresses
                              join Cus in dbContext.Users on Exp.CustomerId equals Cus.UserId
                              where Exp.ShipmentStatusId == (int)FrayteExpressShipmentStatus.Scanned
                              && Exp.CustomerId == track.CustomerId
                              && Exp.OperationZoneId == OperationZone
                              select new ExpressScannedAwbModel
                              {
                                  AWBId = Exp.ExpressId,
                                  customerId = Cus.UserId,
                                  CustomerName = Cus.CompanyName,
                                  AWBNumber = Exp.AWBBarcode,
                                  ScannedOn = Exp.AWBScannedOnUtc,
                                  ScannedBy = Exp.AWBScannedBy,
                                  DocumentName = Exp.AWBBarcode + ".jpg",
                                  TotalRows = 0
                              }).OrderByDescending(x => x.ScannedOn).ToList();
                }
                else
                {
                    Result = (from Exp in dbContext.Expresses
                              join Cus in dbContext.Users on Exp.CustomerId equals Cus.UserId
                              where Exp.ShipmentStatusId == (int)FrayteExpressShipmentStatus.Scanned
                              && Exp.OperationZoneId == OperationZone
                              select new ExpressScannedAwbModel
                              {
                                  AWBId = Exp.ExpressId,
                                  customerId = Cus.UserId,
                                  CustomerName = Cus.CompanyName,
                                  AWBNumber = Exp.AWBBarcode,
                                  ScannedOn = Exp.AWBScannedOnUtc,
                                  ScannedBy = Exp.AWBScannedBy,
                                  DocumentName = Exp.AWBBarcode + ".jpg",
                                  TotalRows = 0
                              }).OrderByDescending(x => x.ScannedOn).ToList();
                }

                if (Result.Count > 0)
                {
                    foreach (var r in Result)
                    {
                        var GetFile = System.IO.File.Exists(AppSettings.AWBImagePath + '/' + r.DocumentName);
                        if (GetFile == false)
                        {
                            r.DocumentName = "Image not available.";
                        }
                        var res = dbContext.Users.Where(a => a.UserId == r.ScannedBy).FirstOrDefault();
                        if (res != null)
                        {
                            var timeZone = dbContext.Timezones.Where(p => p.TimezoneId == res.TimezoneId).FirstOrDefault();
                            var TZ = new TimeZoneModal();
                            if (timeZone != null)
                            {

                                TZ.TimezoneId = timeZone.TimezoneId;
                                TZ.Name = timeZone.Name;
                                TZ.Offset = timeZone.Offset;
                                TZ.OffsetShort = timeZone.OffsetShort;
                            }
                            var TimeZone = TimeZoneInfo.FindSystemTimeZoneById(TZ.Name);

                            var tm = UtilityRepository.UtcDateToOtherTimezone(r.ScannedOn.Date, r.ScannedOn.TimeOfDay, TimeZone);
                            r.ScannedOn = tm.Item1;
                            r.ScannedOnTime = tm.Item2.Substring(0, 2) + ":" + tm.Item2.Substring(2, 2);
                        }
                    }

                    if (!string.IsNullOrEmpty(track.AWBNumber))
                    {
                        track.AWBNumber = track.AWBNumber.Replace(" ", "");
                    }

                    if (track.CustomerId > 0 && string.IsNullOrEmpty(track.AWBNumber))
                    {
                        Result = Result.Where(a => a.customerId == track.CustomerId).ToList();
                    }
                    else if (track.CustomerId == 0 && !string.IsNullOrEmpty(track.AWBNumber))
                    {
                        Result = Result.Where(a => a.AWBNumber.Contains(track.AWBNumber)).ToList();
                    }
                    else if (track.CustomerId == 0 && string.IsNullOrEmpty(track.AWBNumber))
                    {

                    }
                    else if (track.CustomerId > 0 && !string.IsNullOrEmpty(track.AWBNumber))
                    {
                        Result = Result.Where(a => a.AWBNumber.Contains(track.AWBNumber) && a.customerId == track.CustomerId).ToList();
                    }
                }

                int total = Result.Count();
                Result = Result.OrderBy(p => p.AWBId).Skip(SkipRows).Take(track.TakeRows).ToList();

                foreach (var r in Result)
                {
                    r.TotalAWB = total;
                    r.CurrentPageAwb = Result.Count;
                    r.AWBNumber = r.AWBNumber.Substring(0, 3) + " " + r.AWBNumber.Substring(3, 3) + " " + r.AWBNumber.Substring(6, 3) + " " + r.AWBNumber.Substring(9, 3);
                }

                Result.ForEach(p => p.TotalRows = total);
            }
            catch (Exception ex)
            {

            }
            return Result;
        }

        public List<DirectBookingCustomer> GetCustomers(int userId)
        {
            var operationzone = UtilityRepository.GetOperationZone();

            var userDetail = (from r in dbContext.Users
                              join ur in dbContext.UserRoles on r.UserId equals ur.UserId
                              where r.UserId == userId
                              select new
                              {
                                  UserEmail = r.UserEmail,
                                  RoleId = ur.RoleId
                              }).FirstOrDefault();

            if (userDetail.RoleId == (int)FrayteUserRole.CustomerStaff)
            {
                var customers = (from cs in dbContext.CustomerStaffs
                                 join r in dbContext.Users on cs.UserId equals r.UserId
                                 join ua in dbContext.UserAdditionals on r.UserId equals ua.UserId
                                 where cs.CustomerStaffId == userId &&
                                       cs.IsActive == true &&
                                       r.IsActive == true &&
                                       ua.IsExpressSolutions == true &&
                                       r.OperationZoneId == operationzone.OperationZoneId
                                 select new DirectBookingCustomer
                                 {
                                     CustomerId = r.UserId,
                                     CustomerName = r.ContactName,
                                     CompanyName = r.CompanyName,
                                     AccountNumber = ua.AccountNo,
                                     EmailId = r.UserEmail,
                                     ValidDays = ua.DaysValidity.HasValue ? ua.DaysValidity.Value : 0,
                                     CustomerCurrency = ua.CreditLimitCurrencyCode,
                                     OperationZoneId = r.OperationZoneId,
                                     IsShipperTaxAndDuty = ua.IsShipperTaxAndDuty.HasValue ? ua.IsShipperTaxAndDuty.Value : false
                                 }).Distinct().ToList();

                return customers.OrderBy(p => p.CompanyName).ToList();
            }
            else
            {
                var customers = (from r in dbContext.Users
                                 join ua in dbContext.UserAdditionals on r.UserId equals ua.UserId
                                 join ur in dbContext.UserRoles on r.UserId equals ur.UserId
                                 where ur.RoleId == (int)FrayteUserRole.Customer &&
                                       r.IsActive == true &&
                                       ua.IsExpressSolutions == true &&
                                       r.OperationZoneId == operationzone.OperationZoneId
                                 select new DirectBookingCustomer
                                 {
                                     CustomerId = r.UserId,
                                     CustomerName = r.ContactName,
                                     CompanyName = r.CompanyName,
                                     AccountNumber = ua.AccountNo,
                                     EmailId = r.UserEmail,
                                     ValidDays = ua.DaysValidity.HasValue ? ua.DaysValidity.Value : 0,
                                     CustomerCurrency = ua.CreditLimitCurrencyCode,
                                     OperationZoneId = r.OperationZoneId,
                                     IsShipperTaxAndDuty = ua.IsShipperTaxAndDuty.HasValue ? ua.IsShipperTaxAndDuty.Value : false
                                 }).Distinct().ToList();

                return customers.OrderBy(p => p.CompanyName).ToList();
            }
        }

        public ExpressFile ImageToByte(int AWBId)
        {
            ExpressFile EF = new ExpressFile();
            EF.Status = true;
            var Result = dbContext.Expresses.Where(a => a.ExpressId == AWBId).FirstOrDefault();
            if (Result != null)
            {
                try
                {
                    EF.FilePath = AppSettings.WebApiPath + "AwbImage/" + Result.AWBBarcode + ".jpg";
                    EF.FileName = Result.AWBBarcode + ".jpg";
                    if (System.IO.File.Exists(AppSettings.AWBImagePath + "/" + Result.AWBBarcode + ".jpg"))
                    {

                    }
                    else
                    {
                        EF.FilePath = "";
                        EF.FileName = "";
                        EF.Status = false;
                    }

                }
                catch (Exception ex)
                {
                    EF.Status = false;
                }
            }
            else
            {
                EF.Status = false;
            }
            return EF;
        }

        public ExpressFile SaveImageToByte(int AWBId, byte[] imageAWBDocumentName)
        {
            ExpressFile EF = new ExpressFile();
            EF.Status = true;
            var Result = dbContext.Expresses.Where(a => a.ExpressId == AWBId).FirstOrDefault();
            if (Result != null && imageAWBDocumentName != null)
            {
                try
                {
                    byte[] image = imageAWBDocumentName;

                    System.Drawing.Image labelimage;
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream(image))
                    {
                        labelimage = System.Drawing.Image.FromStream(ms);
                        string labelName = string.Empty;
                        EF.FilePath = AppSettings.WebApiPath + "AwbImage/" + Result.AWBBarcode + ".jpg";
                        EF.FileName = Result.AWBBarcode + ".jpg";
                        if (System.IO.Directory.Exists(AppSettings.AWBImagePath))
                        {
                            string ImgName = Result.AWBBarcode + ".jpg";
                            System.Drawing.Bitmap bmpUploadedImage = new System.Drawing.Bitmap(labelimage);
                            System.Drawing.Image thumbnailImage = bmpUploadedImage.GetThumbnailImage(700, 1200, new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero);
                            string extension = Path.GetExtension(ImgName);
                            string FileName = Path.GetFileName(ImgName);
                            string[] ff = FileName.ToString().Split('.');
                            string name = ff[0].ToString();
                            thumbnailImage.Save(AppSettings.AWBImagePath + "/" + name + extension);
                        }
                    }
                }
                catch (Exception ex)
                {
                    EF.Status = false;
                }

            }
            else
            {
                EF.Status = false;
            }
            return EF;
        }

        public FrayteResult UpdateBagNumber(string aWBNumber, string carrier, int bagId)
        {
            FrayteResult result = new Models.FrayteResult();

            var bag = dbContext.ExpressBags.Find(bagId);
            if (bag != null)
            {
                if (bag.Courier == carrier && (!bag.IsClosed.HasValue || !bag.IsClosed.Value))
                {

                }
            }

            throw new NotImplementedException();
        }

        //convert image to bytearray
        public byte[] imgToByteArray(Image img)
        {
            using (MemoryStream mStream = new MemoryStream())
            {
                img.Save(mStream, img.RawFormat);
                return mStream.ToArray();
            }
        }

        internal bool ThumbnailCallback()
        {
            return true;
        }

        public LoginScreenModules GetDefaultScreen(string EmailId)
        {
            LoginScreenModules LSM = new LoginScreenModules();
            LSM.MasterTracking = new List<MasterTrackingModel>();
            var userInfo = dbContext.Users.Where(p => p.Email == EmailId).FirstOrDefault();
            if (userInfo != null && userInfo.UserId > 0)
            {
                var mobileUserDetail = dbContext.MobileUserConfigurations.Where(p => p.UserId == userInfo.UserId).ToList();
                var collection = (from MUD in dbContext.MobileUserConfigurations
                                  join td in dbContext.MasterTrackingDetails on MUD.MasterTrackingDetailId equals td.MasterTrackingDetailId
                                  join r in dbContext.MasterTrackings on td.MasterTrackingId equals r.MasterTrackingId
                                  where MUD.UserId == userInfo.UserId
                                  && MUD.IsActive == true
                                  select new
                                  {
                                      MasterTrackingId = r.MasterTrackingId,
                                      ModuleType = r.ModuleType,
                                      EventName = r.EventKey,
                                      EventKey = r.EventKey,
                                      EventKeyDisplay = r.EventKeyDisplay,
                                      IsDefault = r.IsDefault,
                                      IsOnlyForTracking = r.IsOnlyForTracking,
                                      IsOnlyMAWBTracking = r.IsOnlyMAWBTracking,
                                      MasterTrackingDetailId = td.MasterTrackingDetailId,
                                      SubEventKey = td.EventKey,
                                      SubEventDisplay = td.EventDisplay,
                                      IsExternal = td.IsExternal,
                                      SubIsDefault = td.IsDefault,
                                      SubEventMessage = td.EventMessage
                                  }).ToList();

                if (collection.Count > 0)
                {
                    LSM.MasterTracking = collection
                                         .Where(p => p.ModuleType == FrayteShipmentServiceType.Express)
                                         .GroupBy(p => p.MasterTrackingId)
                                         .Select(group => new MasterTrackingModel
                                         {
                                             MasterTrackingId = group.FirstOrDefault().MasterTrackingId,
                                             EventKey = group.FirstOrDefault().EventKey,
                                             EventDisplay = group.FirstOrDefault().EventKeyDisplay,
                                             ModuleType = group.FirstOrDefault().ModuleType,
                                             TrackingDetail = group.Select(p => new MasterTrackingDetailModel
                                             {
                                                 EventDisplay = p.SubEventDisplay,
                                                 EventKey = p.SubEventKey,
                                                 IsDefault = p.SubIsDefault,
                                                 IsExternal = p.IsExternal,
                                                 MasterTrackingDetailId = p.MasterTrackingDetailId,
                                                 Message = p.SubEventMessage,
                                             }).OrderBy(a => a.MasterTrackingDetailId).ToList()
                                         }).OrderBy(a => a.MasterTrackingId).ToList();
                    LSM.Status = true;
                }
                else
                {
                    LSM.Status = false;
                    LSM.ErrorObject = new List<MobApiErrorObj>();
                    MobApiErrorObj AEO = new MobApiErrorObj();
                    LSM.Status = false;
                    AEO.ErrorCode = "Master Details not exist.";
                    AEO.ErrorMessage = "Master details does not exist.";
                    LSM.ErrorObject.Add(AEO);
                }
                LSM.Status = true;
            }
            else
            {
                LSM.Status = false;
                LSM.ErrorObject = new List<MobApiErrorObj>();
                MobApiErrorObj AEO = new MobApiErrorObj();
                LSM.Status = false;
                AEO.ErrorCode = "User not exist.";
                AEO.ErrorMessage = "User does not exist.";
                LSM.ErrorObject.Add(AEO);
            }
            return LSM;
        }

        public List<ScanAwbMobileModel> GetCollectionScanMobileAwb(int ScannedBy, int MobileEventId)
        {
            List<ScanAwbMobileModel> LSAW = new List<ScanAwbMobileModel>();
            ExpressApiErrorModel TAS = new ExpressApiErrorModel();
            if (MobileEventId > 0)
            {
                List<Express> Result = new List<Express>();
                if (MobileEventId == 1)
                {
                    Result = dbContext.Expresses.Where(a => a.ShipmentStatusId == 37 && a.MasterTrackingDetailId == 1).ToList();
                }
                else if (MobileEventId == 2)
                {
                    Result = dbContext.Expresses.Where(a => (a.ShipmentStatusId == 37 || a.ShipmentStatusId == 38) && a.MasterTrackingDetailId == 1).ToList();
                }
                else if (MobileEventId == 3)
                {
                    Result = dbContext.Expresses.Where(a => (a.ShipmentStatusId == 37 || a.ShipmentStatusId == 38) && a.MasterTrackingDetailId == 2).ToList();
                }

                if (Result.Count > 0)
                {
                    foreach (var res in Result)
                    {
                        ScanAwbMobileModel SAW = new ScanAwbMobileModel();
                        SAW.DriverName = dbContext.Users.Where(a => a.UserId == res.AWBScannedBy).FirstOrDefault().ContactName;
                        SAW.ExpressId = res.ExpressId;
                        SAW.AwbNumber = res.AWBBarcode.Substring(0, 3) + " " + res.AWBBarcode.Substring(3, 3) + " " + res.AWBBarcode.Substring(6, 3) + " " + res.AWBBarcode.Substring(9, 3);
                        LSAW.Add(SAW);
                    }
                }
                else
                {
                    ScanAwbMobileModel SAW = new ScanAwbMobileModel();
                    SAW.Status = false;
                    SAW.ErrorObject = new List<MobApiErrorObj>();
                    MobApiErrorObj AEO = new MobApiErrorObj();
                    AEO.ErrorCode = "AWB does not exist for this user.";
                    AEO.ErrorMessage = "AWB does not exist for this user.";
                    SAW.ErrorObject.Add(AEO);
                    LSAW.Add(SAW);
                }
            }
            else
            {
                ScanAwbMobileModel SAW = new ScanAwbMobileModel();
                SAW.Status = false;
                SAW.ErrorObject = new List<MobApiErrorObj>();
                MobApiErrorObj AEO = new MobApiErrorObj();
                AEO.ErrorCode = "Mobile event does not exist.";
                AEO.ErrorMessage = "Mobile event does not exist.";
                SAW.ErrorObject.Add(AEO);
                LSAW.Add(SAW);
            }
            return LSAW;
        }

        public ScanAwbMobileModel CollectionScanMobileAwb(ScanInitalAwbModel AWBDetail)
        {
            ScanAwbMobileModel SAW = new ScanAwbMobileModel();
            ExpressApiErrorModel TAS = new ExpressApiErrorModel();
            SAW.Status = false;
            var regexItem = new Regex("^[a-zA-Z0-9 ]*$");
            var OperationZone = UtilityRepository.GetOperationZone();
            if (!string.IsNullOrEmpty(AWBDetail.AwbNumber) && regexItem.IsMatch(AWBDetail.AwbNumber) && AWBDetail.AwbNumber.Replace(" ", "").Length == 12)
            {
                string CustomerAccNo = AWBDetail.AwbNumber.Substring(0, 3);
                var Result = (from UsrAdd in dbContext.UserAdditionals
                              join Usr in dbContext.Users on UsrAdd.UserId equals Usr.UserId
                              where UsrAdd.AccountNo.Substring(0, 3) == CustomerAccNo
                              select new
                              {
                                  Usr.UserId
                              }).FirstOrDefault();

                if (Result != null && Result.UserId > 0)
                {
                    var AwbNo = AWBDetail.AwbNumber.Replace(" ", "");
                    var Shipment = dbContext.Expresses.Where(a => a.AWBBarcode == AwbNo).FirstOrDefault();
                    if (Shipment == null)
                    {
                        Express Ship = new Express();
                        Ship.AWBBarcode = AwbNo;
                        Ship.CustomerId = Result.UserId;
                        Ship.AWBScannedBy = AWBDetail.ScannedBy;
                        Ship.CustomerId = Result.UserId;
                        Ship.MasterTrackingDetailId = AWBDetail.MobileEventId;
                        Ship.ShipmentStatusId = 37;
                        Ship.OperationZoneId = OperationZone.OperationZoneId;
                        Ship.CreatedBy = Result.UserId;
                        Ship.CreatedOnUtc = DateTime.UtcNow;
                        Ship.AWBScannedOnUtc = DateTime.UtcNow;
                        dbContext.Expresses.Add(Ship);
                        dbContext.SaveChanges();

                        SAW.ExpressId = Ship.ExpressId;
                        SAW.AwbNumber = AwbNo;
                        SAW.Status = true;
                        if (AWBDetail.Photo != null && AWBDetail.Photo.Length > 0)
                        {
                            SaveImageToByte(Ship.ExpressId, AWBDetail.Photo);
                        }

                        TrackingDetail TD = new TrackingDetail();
                        TD.ShipmentId = SAW.ExpressId;
                        TD.ModuleType = FrayteShipmentServiceType.ExpressBooking;
                        TD.MobileEventConfigurationId = AWBDetail.MobileEventId;
                        TD.CreatedBy = AWBDetail.ScannedBy;
                        var ScannedByDetail = GetScannedByDetail(AWBDetail.ScannedBy);
                        if (ScannedByDetail != null)
                        {
                            TD.Country = ScannedByDetail.Country.Name;
                            TD.City = ScannedByDetail.City;
                        }
                        TD.CreatedOn = DateTime.UtcNow;
                        TD.Lattitude = AWBDetail.Lattitude;
                        TD.Longitude = AWBDetail.Longitude;
                        dbContext.TrackingDetails.Add(TD);
                        dbContext.SaveChanges();

                        TrackingNumberRoute TNR = new TrackingNumberRoute();
                        TNR.Number = AwbNo;
                        TNR.ShipmentId = SAW.ExpressId;
                        TNR.ModuleType = FrayteShipmentServiceType.ExpressBooking;
                        TNR.IsAWB = true;
                        dbContext.TrackingNumberRoutes.Add(TNR);
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        SAW.Status = false;
                        SAW.ErrorObject = new List<MobApiErrorObj>();
                        MobApiErrorObj AEO = new MobApiErrorObj();
                        AEO.ErrorCode = "Parcel already scanned.";
                        AEO.ErrorMessage = "This parcel is already scanned.";
                        SAW.ErrorObject.Add(AEO);
                    }
                }
                else
                {
                    SAW.Status = false;
                    SAW.ErrorObject = new List<MobApiErrorObj>();
                    MobApiErrorObj AEO = new MobApiErrorObj();
                    AEO.ErrorCode = "User Does not exist.";
                    AEO.ErrorMessage = "User Does not exist.";
                    SAW.ErrorObject.Add(AEO);
                }
            }
            else
            {
                SAW.ExpressId = 0;
                SAW.Status = false;
                SAW.ErrorObject = new List<MobApiErrorObj>();
                MobApiErrorObj AEO = new MobApiErrorObj();
                AEO.ErrorCode = "Wrong AWB number or may be MobileEventid is wrong.";
                AEO.ErrorMessage = "This AWB number is Wrong or may be MobileEventid is wrong.";
                SAW.ErrorObject.Add(AEO);
            }
            return SAW;
        }

        public byte[] GetImage()
        {
            using (Image image = Image.FromFile(@"D:\ProjectFrayte\Frayte\Frayte.WebApi\UploadFiles\ProfilePhoto\5_544810894.png"))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    byte[] imageBytes = m.ToArray();
                    return imageBytes;
                }
            }
        }

        public FrayteAddress GetScannedByDetail(int SCannedBy)
        {
            var UserDTL = (from ua in dbContext.UserAdditionals
                           join u1 in dbContext.UserAddresses on ua.UserId equals u1.UserId
                           join Ctn in dbContext.Countries on u1.CountryId equals Ctn.CountryId
                           where ua.UserId == SCannedBy
                           select new FrayteAddress()
                           {
                               City = u1.City,
                               Country = new FrayteCountryCode()
                               {
                                   Name = Ctn.CountryName
                               }
                           }
                  ).FirstOrDefault();

            return UserDTL;
        }

        public List<ScanAwbMobileModel> SubmitAwb(ScanAwbList scanAwb)
        {
            List<ScanAwbMobileModel> TAW = new List<ScanAwbMobileModel>();
            ExpressApiErrorModel TAS = new ExpressApiErrorModel();
            bool Descrepencies = false;
            var GetEventInfo = dbContext.MasterTrackingDetails.Where(a => a.MasterTrackingDetailId == scanAwb.MobileId).FirstOrDefault();
            if (GetEventInfo != null && GetEventInfo.EventKey == "EFG")
            {
                if (scanAwb.AwbNos.Count > 0)
                {
                    foreach (var awb in scanAwb.AwbNos)
                    {
                        ScanAwbMobileModel SAW = new ScanAwbMobileModel();
                        if (!string.IsNullOrEmpty(awb.AWBNumber) && awb.AWBNumber.Replace(" ", "").Length == 12)
                        {
                            var AWB = awb.AWBNumber.Substring(0, 3);
                            string CustomerAccNo = AWB;
                            string AWBNo = awb.AWBNumber.Replace(" ", "");
                            var Shipment = dbContext.Expresses.Where(a => a.AWBBarcode == AWBNo).FirstOrDefault();
                            if (Shipment != null)
                            {
                                var Res = dbContext.TrackingDetails.Where(a => a.ShipmentId == Shipment.ExpressId && a.MobileEventConfigurationId == GetEventInfo.MasterTrackingDetailId).FirstOrDefault();
                                if (Res == null)
                                {
                                    TrackingDetail TD = new TrackingDetail();
                                    TD.ShipmentId = Shipment.ExpressId;
                                    TD.ModuleType = FrayteShipmentServiceType.ExpressBooking;
                                    TD.MobileEventConfigurationId = GetEventInfo.MasterTrackingDetailId;
                                    TD.CreatedBy = scanAwb.ScannedBy;
                                    var ScannedByDetail = GetScannedByDetail(scanAwb.ScannedBy);
                                    if (ScannedByDetail != null)
                                    {
                                        TD.Country = ScannedByDetail.Country.Name;
                                        TD.City = ScannedByDetail.City;
                                    }
                                    TD.CreatedOn = DateTime.UtcNow;
                                    TD.IsMissing = awb.IsScanned == true ? false : true;
                                    TD.Longitude = scanAwb.Longitude;
                                    TD.Lattitude = scanAwb.Lattitude;
                                    dbContext.TrackingDetails.Add(TD);
                                    dbContext.SaveChanges();

                                    SAW.Status = true;
                                    SAW.AwbNumber = awb.AWBNumber;
                                    TAW.Add(SAW);
                                }
                                else
                                {
                                    SAW.Status = false;
                                    SAW.AwbNumber = awb.AWBNumber;
                                    SAW.ErrorObject = new List<MobApiErrorObj>();
                                    MobApiErrorObj AEO = new MobApiErrorObj();
                                    SAW.Status = false;
                                    AEO.ErrorCode = "Parcel already scanned.";
                                    AEO.ErrorMessage = "This parcel is already scanned.";
                                    SAW.ErrorObject.Add(AEO);
                                    TAW.Add(SAW);
                                }

                                Shipment.MasterTrackingDetailId = GetEventInfo.MasterTrackingDetailId;
                                dbContext.Entry(Shipment).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();
                            }
                            else
                            {
                                SAW.Status = false;
                                SAW.AwbNumber = awb.AWBNumber;
                                SAW.ErrorObject = new List<MobApiErrorObj>();
                                MobApiErrorObj AEO = new MobApiErrorObj();
                                SAW.Status = false;
                                AEO.ErrorCode = "AWB Not Exist.";
                                AEO.ErrorMessage = "AWB Not Exist.";
                                SAW.ErrorObject.Add(AEO);
                                TAW.Add(SAW);
                            }
                        }
                    }
                    return TAW;
                }
                else
                {
                    ScanAwbMobileModel SAW = new ScanAwbMobileModel();
                    SAW.ExpressId = 0;
                    SAW.Status = false;
                    SAW.AwbNumber = "";
                    SAW.DriverName = "";
                    SAW.ErrorObject = null;
                    TAW.Add(SAW);
                    return TAW;
                }
            }
            else if (GetEventInfo != null && GetEventInfo.EventKey == "QWE")
            {
                if (scanAwb.AwbNos.Count > 0)
                {
                    foreach (var awb in scanAwb.AwbNos)
                    {
                        ScanAwbMobileModel SAW = new ScanAwbMobileModel();
                        if (!string.IsNullOrEmpty(awb.AWBNumber) && awb.AWBNumber.Replace(" ", "").Length == 12)
                        {
                            string CustomerAccNo = awb.AWBNumber.Substring(0, 3); ;
                            string AWBNo = awb.AWBNumber.Replace(" ", "");
                            var Shipment = dbContext.Expresses.Where(a => a.AWBBarcode == AWBNo).FirstOrDefault();
                            if (Shipment != null)
                            {
                                var Res = dbContext.TrackingDetails.Where(a => a.ShipmentId == Shipment.ExpressId && a.MobileEventConfigurationId == GetEventInfo.MasterTrackingDetailId).FirstOrDefault();
                                if (Res == null)
                                {
                                    TrackingDetail TD = new TrackingDetail();
                                    TD.ShipmentId = Shipment.ExpressId;
                                    TD.ModuleType = FrayteShipmentServiceType.ExpressBooking;
                                    TD.MobileEventConfigurationId = GetEventInfo.MasterTrackingDetailId;
                                    TD.CreatedBy = scanAwb.ScannedBy;
                                    var ScannedByDetail = GetScannedByDetail(scanAwb.ScannedBy);
                                    if (ScannedByDetail != null)
                                    {
                                        TD.Country = ScannedByDetail.Country.Name;
                                        TD.City = ScannedByDetail.City;
                                    }
                                    TD.CreatedOn = DateTime.UtcNow;
                                    TD.Longitude = scanAwb.Longitude;
                                    TD.Lattitude = scanAwb.Lattitude;
                                    dbContext.TrackingDetails.Add(TD);
                                    dbContext.SaveChanges();
                                    SAW.Status = true;
                                    SAW.AwbNumber = awb.AWBNumber;
                                    TAW.Add(SAW);

                                    Shipment.MasterTrackingDetailId = GetEventInfo.MasterTrackingDetailId;
                                    dbContext.Entry(Shipment).State = System.Data.Entity.EntityState.Modified;
                                    dbContext.SaveChanges();
                                }
                                else
                                {
                                    SAW.Status = false;
                                    SAW.AwbNumber = awb.AWBNumber;
                                    SAW.ErrorObject = new List<MobApiErrorObj>();
                                    MobApiErrorObj AEO = new MobApiErrorObj();
                                    SAW.Status = false;
                                    AEO.ErrorCode = "Parcel already scanned.";
                                    AEO.ErrorMessage = "This parcel is already scanned.";
                                    SAW.ErrorObject.Add(AEO);
                                    TAW.Add(SAW);
                                }
                            }
                            else
                            {
                                SAW.Status = false;
                                SAW.AwbNumber = awb.AWBNumber;
                                SAW.ErrorObject = new List<MobApiErrorObj>();
                                MobApiErrorObj AEO = new MobApiErrorObj();
                                SAW.Status = false;
                                AEO.ErrorCode = "AWB Not Exist.";
                                AEO.ErrorMessage = "AWB Not Exist.";
                                SAW.ErrorObject.Add(AEO);
                                TAW.Add(SAW);
                            }
                        }
                    }
                    return TAW;
                }
                else
                {
                    ScanAwbMobileModel SAW = new ScanAwbMobileModel();
                    SAW.ExpressId = 0;
                    SAW.Status = false;
                    SAW.AwbNumber = "";
                    SAW.DriverName = "";
                    SAW.ErrorObject = null;
                    TAW.Add(SAW);
                    return TAW;
                }
            }
            else
            {
                ScanAwbMobileModel SAW = new ScanAwbMobileModel();
                SAW.ExpressId = 0;
                SAW.Status = false;
                SAW.ErrorObject = new List<MobApiErrorObj>();
                MobApiErrorObj AEO = new MobApiErrorObj();
                AEO.ErrorCode = "Wrong Mobile Event.";
                AEO.ErrorMessage = "Wrong Mobile Event.";
                SAW.ErrorObject.Add(AEO);
                TAW.Add(SAW);
                return TAW;
            }
        }

        public CreateBagModel CreateBag(string AWBNumber, int ScannedBy, int MobileEventId)
        {
            CreateBagModel CBM = new CreateBagModel();
            var awbNo = AWBNumber.Replace(" ", "");
            var exsShipment = dbContext.Expresses.Where(p => p.AWBBarcode == awbNo && p.ShipmentStatusId == 38).FirstOrDefault();

            HubCarrierService hubCarrierService = new HubCarrierService();
            HubCarrier hubCarrier = new HubCarrier();
            ExpressBag BagDtl = new ExpressBag();
            Hub hub = new Hub();
            if (exsShipment != null && (exsShipment.BagId == null || exsShipment.BagId == 0))
            {
                hubCarrierService = dbContext.HubCarrierServices.Where(a => a.HubCarrierServiceId == exsShipment.HubCarrierServiceId).FirstOrDefault();
                hubCarrier = dbContext.HubCarriers.Where(a => a.HubCarrierId == hubCarrierService.HubCarrierId).FirstOrDefault();
                hub = dbContext.Hubs.Where(a => a.HubId == hubCarrier.HubId).FirstOrDefault();
                BagDtl = dbContext.ExpressBags.Where(a => a.HubCarrierId == hubCarrier.HubCarrierId && a.IsClosed != true && a.CustomerId == exsShipment.CustomerId && a.CreatedBy == ScannedBy).FirstOrDefault();

                if (BagDtl == null)
                {
                    if (exsShipment.CreatedOnUtc.Date.AddDays(7) < DateTime.Now.Date)
                    {
                        CBM.ErrorObject = new List<MobApiErrorObj>();
                        MobApiErrorObj AEO = new MobApiErrorObj();
                        CBM.Status = false;
                        CBM.IsBagCreated = false;
                        CBM.IsBagExpired = true;
                        CBM.IsError = true;
                        AEO.ErrorCode = "Parcel number or shipment can not older than 7 days.";
                        AEO.ErrorMessage = "Parcel number or shipment can not older than 7 days.";
                        CBM.ErrorObject.Add(AEO);
                        CBM.Status = false;
                        return CBM;
                    }
                    else
                    {
                        ExpressBag EG = new ExpressBag();
                        EG.BagBarCode = "BGL" + "-" + hub.Code + "-" + new Random().Next(10000000, 99999999);
                        EG.Courier = hubCarrier != null ? hubCarrier.Carrier : "";
                        EG.CreatedBy = ScannedBy;
                        EG.CreatedOn = DateTime.UtcNow;
                        EG.HubCarrierId = hubCarrier != null ? hubCarrier.HubCarrierId : 0;
                        EG.CustomerId = exsShipment.CustomerId;
                        dbContext.ExpressBags.Add(EG);
                        dbContext.SaveChanges();

                        TrackingNumberRoute TNR = new TrackingNumberRoute();
                        TNR.Number = EG.BagBarCode;
                        TNR.ShipmentId = EG.BagId;
                        TNR.IsBag = true;
                        TNR.ModuleType = FrayteShipmentServiceType.ExpressBooking;
                        dbContext.TrackingNumberRoutes.Add(TNR);
                        dbContext.SaveChanges();

                        BagManifestTracking BMT = new BagManifestTracking();
                        BMT.BagId = EG.BagId;
                        BMT.CreatedBy = ScannedBy;
                        var ScannedByDetail = GetScannedByDetail(ScannedBy);
                        if (ScannedByDetail != null)
                        {
                            BMT.Country = ScannedByDetail.Country.Name;
                            BMT.City = ScannedByDetail.City;
                        }
                        BMT.CreatedOnUtc = DateTime.UtcNow;
                        BMT.IsMissing = false;
                        BMT.ManifestId = 0;
                        BMT.MobileEventConfigurationId = MobileEventId;
                        BMT.Description = dbContext.MasterTrackingDetails.Where(a => a.MasterTrackingDetailId == BMT.MobileEventConfigurationId).FirstOrDefault().EventDisplay;
                        dbContext.BagManifestTrackings.Add(BMT);
                        dbContext.SaveChanges();

                        var bagList = dbContext.ExpressBags.Where(p => p.Courier == hubCarrier.Carrier && p.IsClosed == true && p.CreatedBy == ScannedBy && (p.ManifestId == null || p.ManifestId == 0)).ToList();
                        var bagCount = bagList.Count + 1;
                        string bagNumber = hub.Code + "-" + hubCarrier.Carrier + "-" + bagCount;

                        var bag = dbContext.ExpressBags.Find(EG.BagId);
                        if (bag != null)
                        {
                            bag.BagNumber = bagNumber;
                            dbContext.SaveChanges();
                        }

                        var AwbDetail = dbContext.Expresses.Where(a => a.AWBBarcode == awbNo).FirstOrDefault();
                        if (AwbDetail != null)
                        {
                            AwbDetail.BagId = EG.BagId;
                            AwbDetail.MasterTrackingDetailId = MobileEventId;
                            dbContext.Entry(AwbDetail).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }

                        CBM.HubCode = hub.Code;
                        CBM.HubName = hub.Name;
                        CBM.BagId = EG.BagId;
                        CBM.IsBagCreated = true;
                        CBM.Status = true;
                        CBM.IsBagExpired = false;
                        CBM.IsError = false;
                        CBM.BagNumber = EG.BagBarCode;
                        CBM.Carrier = hubCarrier != null ? hubCarrier.Carrier : "";
                        CBM.TotalShipments = dbContext.Expresses.Where(a => a.BagId == CBM.BagId).Count();
                        CBM.IsBagFull = false;
                        CBM.TotalWeight = (AwbDetail.ActualWeight.HasValue ? AwbDetail.ActualWeight.Value : 0.00m);
                        CBM.RemainingWeight = 30.0m - (AwbDetail.ActualWeight.HasValue ? AwbDetail.ActualWeight.Value : 0.00m);
                        return CBM;
                    }
                }
                else
                {
                    if (exsShipment.CreatedOnUtc.Date.AddDays(7) < DateTime.Now.Date)
                    {
                        CBM.ErrorObject = new List<MobApiErrorObj>();
                        MobApiErrorObj AEO = new MobApiErrorObj();
                        CBM.Status = false;
                        CBM.IsBagCreated = false;
                        AEO.ErrorCode = "Parcel number or shipment can not older than 7 days.";
                        AEO.ErrorMessage = "Parcel number or shipment can not older than 7 days.";
                        CBM.ErrorObject.Add(AEO);
                        CBM.Status = false;
                        CBM.IsBagExpired = true;
                        CBM.IsError = true;
                        return CBM;
                    }
                    else
                    {
                        int bagid = BagDtl != null ? BagDtl.BagId : 0;
                        var baginfo = dbContext.Expresses.Where(x => x.BagId == bagid).ToList();
                        if (baginfo != null && baginfo.Count > 0)
                        {
                            decimal weight = baginfo.Sum(p => (p.ActualWeight.HasValue ? p.ActualWeight.Value : 0.00m)) + (exsShipment.ActualWeight.HasValue ? exsShipment.ActualWeight.Value : 0.00m);
                            if (weight > 30.0m)
                            {
                                CBM.ErrorObject = new List<MobApiErrorObj>();
                                MobApiErrorObj AEO = new MobApiErrorObj();
                                CBM.Status = false;
                                CBM.IsBagFull = true;
                                CBM.RemainingWeight = (30.0m) - ((weight) - (exsShipment.ActualWeight.HasValue ? exsShipment.ActualWeight.Value : 0.0m));
                                CBM.IsBagCreated = false;
                                CBM.TotalWeight = baginfo.Sum(p => (p.ActualWeight.HasValue ? p.ActualWeight.Value : 0.00m));
                                AEO.ErrorCode = "Bag can not exceed more than 30 kg.";
                                AEO.ErrorMessage = "Bag can not exceed more than 30 kg.";
                                CBM.ErrorObject.Add(AEO);
                                CBM.Status = false;
                                CBM.IsBagExpired = true;
                                CBM.IsError = true;
                                return CBM;
                            }
                            else
                            {
                                var Bag = dbContext.ExpressBags.Where(a => a.BagId == BagDtl.BagId && (a.IsClosed == null || a.IsClosed == false))
                                                   .Select(p => new { p.BagId, p.BagBarCode }).FirstOrDefault();
                                if (Bag != null)
                                {
                                    var AwbDetail = dbContext.Expresses.Where(a => a.AWBBarcode == awbNo).FirstOrDefault();
                                    if (AwbDetail != null)
                                    {
                                        AwbDetail.BagId = BagDtl.BagId;
                                        AwbDetail.MasterTrackingDetailId = MobileEventId;
                                        dbContext.Entry(AwbDetail).State = System.Data.Entity.EntityState.Modified;
                                        dbContext.SaveChanges();

                                        CBM.BagId = Bag.BagId;
                                        CBM.Status = true;
                                        CBM.IsBagCreated = false;
                                        CBM.IsBagExpired = false;
                                        CBM.IsError = false;
                                        CBM.BagNumber = Bag.BagBarCode;
                                        CBM.HubCode = hub.Code;
                                        CBM.HubName = hub.Name;
                                        CBM.TotalShipments = dbContext.Expresses.Where(a => a.BagId == CBM.BagId).ToList().Count;
                                        CBM.Carrier = hubCarrier.Carrier != null ? hubCarrier.Carrier : "";
                                        CBM.IsBagFull = false;
                                        CBM.TotalWeight = weight;
                                        CBM.RemainingWeight = 30.0m - weight;
                                        return CBM;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (exsShipment != null && exsShipment.BagId != null && exsShipment.BagId != 0)
            {
                CBM.ErrorObject = new List<MobApiErrorObj>();
                MobApiErrorObj AEO = new MobApiErrorObj();
                CBM.Status = false;
                CBM.IsError = true;
                CBM.IsBagExpired = false;
                CBM.IsBagCreated = false;
                AEO.ErrorCode = "Parcel already scanned.";
                AEO.ErrorMessage = "Parcel already scanned.";
                CBM.ErrorObject.Add(AEO);
                CBM.Status = false;
                return CBM;
            }
            else
            {
                CBM.ErrorObject = new List<MobApiErrorObj>();
                MobApiErrorObj AEO = new MobApiErrorObj();
                CBM.Status = false;
                CBM.IsBagCreated = false;
                CBM.IsError = true;
                CBM.IsBagExpired = false;
                AEO.ErrorCode = "Please scan or enter valid parcel number or shipment is not created for this parcel number.";
                AEO.ErrorMessage = "Please scan or enter valid parcel number or shipment is not created for this parcel number.";
                CBM.ErrorObject.Add(AEO);
                CBM.Status = false;
                return CBM;
            }
            return CBM;
        }

        public List<BagListModel> GetCreatedBag(int CreatedBy)
        {
            List<BagListModel> BLMList = new List<BagListModel>();
            try
            {
                List<int> HubIds = new List<int>();
                var HubIdsGet = new List<int>();
                var Bags = dbContext.ExpressBags.Where(a => a.CreatedBy == CreatedBy && (a.ManifestId == null || a.ManifestId == 0))
                                    .Select(p => new { p.HubCarrierId }).ToList();
                if (Bags.Count > 0)
                {
                    var HubCarrierIds = Bags.GroupBy(a => a.HubCarrierId);
                    if (HubCarrierIds.Count() > 0)
                    {
                        foreach (var Id in HubCarrierIds)
                        {
                            var Hub = dbContext.HubCarriers.Where(a => a.HubCarrierId == Id.Key).FirstOrDefault();
                            if (Hub != null)
                            {
                                HubIds.Add(Hub.HubId);
                            }
                        }
                    }
                    else
                    {
                        BagListModel SAWL = new BagListModel();
                        SAWL.BagInfo = new List<CreateBagModel>();
                        CreateBagModel SAW = new CreateBagModel();
                        SAW.ErrorObject = new List<MobApiErrorObj>();
                        MobApiErrorObj AEO = new MobApiErrorObj();
                        SAW.Status = false;
                        AEO.ErrorCode = "HubCarrier does not exist.";
                        AEO.ErrorMessage = "HubCarrier does not exist.";
                        SAW.ErrorObject.Add(AEO);
                        SAWL.BagInfo.Add(SAW);
                        BLMList.Add(SAWL);
                        return BLMList;
                    }
                    var FinalIds = HubIds.Distinct();
                    foreach (var Id in FinalIds)
                    {
                        BagListModel BLM = new BagListModel();
                        if (Id > 0)
                        {
                            var Hub = dbContext.Hubs.Where(a => a.HubId == Id).FirstOrDefault();
                            if (Hub != null)
                            {
                                BLM.HubId = Hub.HubId;
                                BLM.HubName = Hub.Name;
                                BLM.Code = Hub.Code;
                                var HubCarrier = dbContext.HubCarriers.Where(a => a.HubId == Id).ToList();
                                BLM.BagInfo = new List<CreateBagModel>();
                                foreach (var iD in HubCarrier)
                                {
                                    HubIdsGet.Add(iD.HubCarrierId);
                                    var Records = dbContext.ExpressBags.Where(a => a.HubCarrierId == iD.HubCarrierId && a.CreatedBy == CreatedBy && (a.ManifestId == null || a.ManifestId == 0))
                                                           .Select(p => new { p.BagId, p.BagBarCode, p.Courier, p.IsClosed }).ToList();
                                    if (Records.Count > 0)
                                    {
                                        foreach (var Re in Records)
                                        {
                                            CreateBagModel CBM = new CreateBagModel();
                                            CBM.BagId = Re.BagId;
                                            CBM.BagNumber = Re.BagBarCode;
                                            CBM.Carrier = Re.Courier;
                                            CBM.TotalShipments = dbContext.Expresses.Where(a => a.BagId == Re.BagId).Count();
                                            CBM.IsClosed = Re.IsClosed != null ? Re.IsClosed.Value : false;
                                            BLM.BagInfo.Add(CBM);
                                        }
                                    }
                                }
                            }
                            else
                            {

                            }
                        }
                        if (BLM.HubId > 0 && BLM.BagInfo != null && BLM.BagInfo.Count > 0)
                        {
                            BLMList.Add(BLM);
                        }
                    }
                }
                else
                {
                    BagListModel SAWL = new BagListModel();
                    SAWL.BagInfo = new List<CreateBagModel>();
                    CreateBagModel SAW = new CreateBagModel();
                    SAW.ErrorObject = new List<MobApiErrorObj>();
                    MobApiErrorObj AEO = new MobApiErrorObj();
                    SAW.Status = false;
                    AEO.ErrorCode = "Bags does not exist for this user.";
                    AEO.ErrorMessage = "BagId does not exist for this user.";
                    SAW.ErrorObject.Add(AEO);
                    SAWL.BagInfo.Add(SAW);
                    BLMList.Add(SAWL);
                }
            }
            catch (Exception e)
            {
                BagListModel SAWL = new BagListModel();
                SAWL.BagInfo = new List<CreateBagModel>();
                CreateBagModel SAW = new CreateBagModel();
                SAW.ErrorObject = new List<MobApiErrorObj>();
                MobApiErrorObj AEO = new MobApiErrorObj();
                SAW.Status = false;
                AEO.ErrorCode = "Some Unhandeled Exception.";
                AEO.ErrorMessage = "Some Unhandeled Exception.";
                SAW.ErrorObject.Add(AEO);
                SAWL.BagInfo.Add(SAW);
                BLMList.Add(SAWL);
            }
            return BLMList;
        }

        public CreateBagModel BagClose(int BagId)
        {
            CreateBagModel CBM = new CreateBagModel();
            var BagStatus = dbContext.ExpressBags.Where(a => a.BagId == BagId).FirstOrDefault();
            if (BagStatus != null && BagStatus.IsClosed != true)
            {
                BagStatus.IsClosed = true;
                dbContext.Entry(BagStatus).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                CBM.BagNumber = BagStatus.BagNumber;
                CBM.BagId = BagStatus.BagId;
                CBM.Status = true;
                CBM.CustomerId = BagStatus.CustomerId != null ? BagStatus.CustomerId.Value : 0;
                CBM.IsClosed = BagStatus.IsClosed.Value;
            }
            else if (BagStatus != null && BagStatus.IsClosed == true)
            {
                CBM.Status = false;
                CBM.BagId = BagId;
                CBM.ErrorObject = new List<MobApiErrorObj>();
                MobApiErrorObj AEO = new MobApiErrorObj();
                AEO.ErrorCode = "Bag Already Closed.";
                AEO.ErrorMessage = "Bag Already Closed.";
                CBM.ErrorObject.Add(AEO);
            }
            else
            {
                CBM.Status = false;
                CBM.BagId = BagId;
                CBM.ErrorObject = new List<MobApiErrorObj>();
                MobApiErrorObj AEO = new MobApiErrorObj();
                AEO.ErrorCode = "Bag Not Exist.";
                AEO.ErrorMessage = "Bag Does Not Exist.";
                CBM.ErrorObject.Add(AEO);
            }
            return CBM;
        }

        public GetBagsModel OpenBag(int BagId)
        {
            List<GetBagsModel> FinalResult = new List<GetBagsModel>();
            GetBagsModel GBM = new GetBagsModel();
            var Result = (from bg in dbContext.ExpressBags
                          join Awb in dbContext.Expresses on bg.BagId equals Awb.BagId
                          where bg.BagId == BagId
                          select new
                          {
                              bg.BagId,
                              bg.BagBarCode,
                              Awb.AWBBarcode
                          }).ToList();

            FinalResult = Result.GroupBy(a => a.BagId).Select(group => new GetBagsModel
            {
                BagId = group.FirstOrDefault().BagId,
                BagNumber = group.FirstOrDefault().BagBarCode,
                AWB = group.Select(ab => new AWBs
                {
                    AWBNumber = ab.AWBBarcode.Substring(0, 3) + " " + ab.AWBBarcode.Substring(3, 3) + " " + ab.AWBBarcode.Substring(6, 3) + " " + ab.AWBBarcode.Substring(9, 3)
                }).ToList()
            }).ToList();

            if (FinalResult.FirstOrDefault().BagId > 0)
            {
                FinalResult.FirstOrDefault().Status = true;
            }
            else
            {
                FinalResult.FirstOrDefault().Status = false;
            }

            return FinalResult.FirstOrDefault();
        }

        public CreateBagModel GetBagsModel(int BagId)
        {
            CreateBagModel CBM = new CreateBagModel();
            var BagDetail = dbContext.ExpressBags.Where(a => a.BagId == BagId).FirstOrDefault();
            if (BagDetail != null)
            {
                CBM.BagId = BagDetail.BagId;
                CBM.CustomerId = BagDetail.CustomerId != null ? BagDetail.CustomerId.Value : 0;
            }
            else
            {
                CBM.Status = false;
            }
            return CBM;
        }

        //Export Manifest Scan
        public ScanExportManifestModel ExportManifestScan(string ExportManifestNumber)
        {
            ScanExportManifestModel SEMList = new ScanExportManifestModel();
            SEMList.Bags = new List<BagsModel>();
            ExportManifestNumber = ExportManifestNumber.ToUpper();
            if (!string.IsNullOrEmpty(ExportManifestNumber) && ExportManifestNumber.Length > 12 && ExportManifestNumber.Contains("MNESX"))
            {
                var ManifestDetail = dbContext.ExpressManifests.Where(a => a.BarCode == ExportManifestNumber).FirstOrDefault();
                if (ManifestDetail != null)
                {
                    SEMList.ExportManifestId = ManifestDetail.ExpressManifestId;
                    SEMList.ExportManifestNumber = ManifestDetail.BarCode;
                    var Bags = dbContext.ExpressBags.Where(a => a.ManifestId == ManifestDetail.ExpressManifestId).ToList();
                    if (Bags.Count > 0)
                    {
                        foreach (var Bag in Bags)
                        {
                            BagsModel bag = new BagsModel();
                            bag.BagId = Bag.BagId;
                            bag.TotalShipments = dbContext.Expresses.Where(ab => ab.BagId == Bag.BagId).Count();
                            bag.BagNumber = Bag.BagBarCode;
                            bag.Carrier = bag.Carrier;
                            SEMList.Bags.Add(bag);
                        }
                    }
                    SEMList.Status = true;
                }
                else
                {
                    SEMList.Status = false;
                    SEMList.ErrorObject = new List<MobApiErrorObj>();
                    MobApiErrorObj AEO = new MobApiErrorObj();
                    AEO.ErrorCode = "Export Manifest Number Not Exist.";
                    AEO.ErrorMessage = "Export Manifest Number Does Not Exist.";
                    SEMList.ErrorObject.Add(AEO);
                }
            }
            else
            {
                SEMList.Status = false;
                SEMList.ErrorObject = new List<MobApiErrorObj>();
                MobApiErrorObj AEO = new MobApiErrorObj();
                AEO.ErrorCode = "Export Manifest Number is wrong.";
                AEO.ErrorMessage = "Export Manifest Number is wrong.";
                SEMList.ErrorObject.Add(AEO);
            }
            return SEMList;
        }

        //Bag Scan after Export Manifest Scanned
        public ScanExportManifestModel BagExportScan(ScanExportManifestModel ScannedBags)
        {
            ScanExportManifestModel SEM = new ScanExportManifestModel();
            SEM.Bags = new List<BagsModel>();
            SEM.Status = true;
            if (ScannedBags.Bags.Count > 0)
            {
                var Manifestdtl = dbContext.ExpressManifests.Where(a => a.ExpressManifestId == ScannedBags.ExportManifestId).FirstOrDefault();
                if (Manifestdtl != null)
                {
                    foreach (var Bag in ScannedBags.Bags)
                    {
                        if (Bag.BagId > 0 && Bag.IsScanned == true)
                        {
                            var AwbDetail = dbContext.Expresses.Where(a => a.BagId == Bag.BagId).ToList();
                            if (AwbDetail != null && AwbDetail.Count > 0)
                            {
                                foreach (var Awb in AwbDetail)
                                {
                                    var AwbDl = dbContext.TrackingDetails.Where(a => a.ShipmentId == Awb.ExpressId && a.MobileEventConfigurationId == ScannedBags.MobileEventId).FirstOrDefault();
                                    if (AwbDl == null)
                                    {
                                        TrackingDetail TD = new TrackingDetail();
                                        TD.ShipmentId = Awb.ExpressId;
                                        TD.ModuleType = FrayteShipmentServiceType.ExpressBooking;
                                        TD.MobileEventConfigurationId = ScannedBags.MobileEventId;
                                        TD.CreatedBy = ScannedBags.ScannedBy;
                                        var ScannedByDetail = GetScannedByDetail(ScannedBags.ScannedBy);
                                        if (ScannedByDetail != null)
                                        {
                                            TD.Country = ScannedByDetail.Country.Name;
                                            TD.City = ScannedByDetail.City;
                                        }
                                        TD.IsMissing = false;
                                        TD.CreatedOn = DateTime.UtcNow;
                                        TD.Lattitude = ScannedBags.Lattitude;
                                        TD.Longitude = ScannedBags.Longitude;
                                        dbContext.TrackingDetails.Add(TD);
                                        dbContext.SaveChanges();
                                    }
                                }
                            }
                        }
                        if (Bag.BagId > 0 && Bag.IsScanned == false)
                        {
                            var AwbDetail = dbContext.Expresses.Where(a => a.BagId == Bag.BagId).ToList();
                            if (AwbDetail != null && AwbDetail.Count > 0)
                            {
                                foreach (var Awb in AwbDetail)
                                {
                                    var AwbDl = dbContext.TrackingDetails.Where(a => a.ShipmentId == Awb.ExpressId && a.MobileEventConfigurationId == ScannedBags.MobileEventId).FirstOrDefault();
                                    if (AwbDl == null)
                                    {
                                        TrackingDetail TD = new TrackingDetail();
                                        TD.ShipmentId = Awb.ExpressId;
                                        TD.ModuleType = FrayteShipmentServiceType.ExpressBooking;
                                        TD.MobileEventConfigurationId = ScannedBags.MobileEventId;
                                        TD.CreatedBy = ScannedBags.ScannedBy;
                                        var ScannedByDetail = GetScannedByDetail(ScannedBags.ScannedBy);
                                        if (ScannedByDetail != null)
                                        {
                                            TD.Country = ScannedByDetail.Country.Name;
                                            TD.City = ScannedByDetail.City;
                                        }
                                        TD.IsMissing = true;
                                        TD.Lattitude = ScannedBags.Lattitude;
                                        TD.Longitude = ScannedBags.Longitude;
                                        TD.CreatedOn = DateTime.UtcNow;
                                        dbContext.TrackingDetails.Add(TD);
                                        dbContext.SaveChanges();
                                    }
                                }
                            }
                        }

                        var BagDL = dbContext.BagManifestTrackings.Where(a => a.BagId == Bag.BagId && a.MobileEventConfigurationId == ScannedBags.MobileEventId).FirstOrDefault();
                        if (BagDL == null)
                        {
                            BagManifestTracking BMT = new BagManifestTracking();
                            BMT.BagId = Bag.BagId;
                            BMT.ManifestId = ScannedBags.ExportManifestId;
                            BMT.MobileEventConfigurationId = ScannedBags.MobileEventId;
                            BMT.IsMissing = Bag.IsScanned == true ? false : true;
                            BMT.CreatedBy = ScannedBags.ScannedBy;
                            var ScannedByDetail = GetScannedByDetail(ScannedBags.ScannedBy);
                            if (ScannedByDetail != null)
                            {
                                BMT.Country = ScannedByDetail.Country.Name;
                                BMT.City = ScannedByDetail.City;
                            }
                            BMT.CreatedOnUtc = DateTime.UtcNow;
                            BMT.Description = dbContext.MasterTrackingDetails.Where(a => a.MasterTrackingDetailId == ScannedBags.MobileEventId).FirstOrDefault().EventDisplay;
                            dbContext.BagManifestTrackings.Add(BMT);
                            dbContext.SaveChanges();
                            Bag.Status = true;
                        }
                        else
                        {
                            BagsModel Ba = new BagsModel();
                            Ba.Status = false;
                            Ba.BagId = Bag.BagId;
                            Ba.ErrorObject = new List<MobApiErrorObj>();
                            MobApiErrorObj AEO = new MobApiErrorObj();
                            AEO.ErrorCode = "Bag already scanned or wrong BagId.";
                            AEO.ErrorMessage = "Bag already scanned or wrong BagId.";
                            Ba.ErrorObject.Add(AEO);
                            SEM.Status = false;
                            SEM.Bags.Add(Ba);
                        }
                    }
                }
                else
                {
                    SEM.Status = false;
                    SEM.ErrorObject = new List<MobApiErrorObj>();
                    MobApiErrorObj AEO = new MobApiErrorObj();
                    SEM.ExportManifestId = ScannedBags.ExportManifestId;
                    AEO.ErrorCode = "Export manifest does not exist.";
                    AEO.ErrorMessage = "Export manifest does not exist.";
                    SEM.ErrorObject.Add(AEO);
                }
            }
            return SEM;
        }

        //Driver Manifest Scan
        public ScanDriverManifestModel DriverManifestScan(string DriverManifestNumber)
        {
            ScanDriverManifestModel SEMList = new ScanDriverManifestModel();
            SEMList.Bags = new List<BagsModel>();
            DriverManifestNumber = DriverManifestNumber.ToUpper();
            if (!string.IsNullOrEmpty(DriverManifestNumber) && DriverManifestNumber.Length > 16 && DriverManifestNumber.Contains("MNDR"))
            {
                var ManifestDetail = dbContext.ExpressDriverManifests.Where(a => a.DriverManifestBarCode == DriverManifestNumber).FirstOrDefault();
                if (ManifestDetail != null)
                {
                    var Bags = dbContext.ExpressBags.Where(a => a.DriverManifestId == ManifestDetail.ExpressDriverManifestId).GroupBy(a => a.Courier).ToList();
                    if (Bags.Count > 0)
                    {
                        foreach (var Bag in Bags)
                        {
                            SEMList.DriverManifestId = ManifestDetail.ExpressDriverManifestId;
                            SEMList.Carrier = Bag.Key;
                            foreach (var b in Bag)
                            {
                                BagsModel bag = new BagsModel();
                                if (b.CreatedOn.Value.AddDays(7).Date < DateTime.Now.Date)
                                {
                                    bag.BagId = b.BagId;
                                    bag.BagNumber = b.BagBarCode;
                                    bag.TotalShipments = dbContext.Expresses.Where(a => a.BagId == b.BagId).Count();
                                    bag.Carrier = b.Courier;
                                    bag.IsBagExpired = true;
                                    bag.IsError = true;
                                    bag.ErrorMessage = "Bag has been expired.";
                                }
                                else
                                {
                                    bag.BagId = b.BagId;
                                    bag.BagNumber = b.BagBarCode;
                                    bag.TotalShipments = dbContext.Expresses.Where(a => a.BagId == b.BagId).Count();
                                    bag.Carrier = b.Courier;
                                    bag.IsBagExpired = false;
                                    bag.IsError = false;
                                    bag.ErrorMessage = "";
                                }
                                SEMList.Bags.Add(bag);
                            }
                        }
                    }
                    SEMList.Status = true;
                }
                else
                {
                    SEMList.Status = false;
                    SEMList.ErrorObject = new List<MobApiErrorObj>();
                    MobApiErrorObj AEO = new MobApiErrorObj();
                    AEO.ErrorCode = "Driver Manifest Number Not Exist.";
                    AEO.ErrorMessage = "Driver Manifest Number Does Not Exist.";
                    SEMList.ErrorObject.Add(AEO);
                }
            }
            else
            {
                SEMList.Status = false;
                SEMList.ErrorObject = new List<MobApiErrorObj>();
                MobApiErrorObj AEO = new MobApiErrorObj();
                AEO.ErrorCode = "Driver Manifest Number is wrong.";
                AEO.ErrorMessage = "Driver Manifest Number is wrong.";
                SEMList.ErrorObject.Add(AEO);
            }
            return SEMList;
        }

        //Bag Scan after Driver Manifest Scanned
        public List<ScanDriverManifestModel> BagDriverScan(ScanDriverManifestModel ScannedBags)
        {
            List<ScanDriverManifestModel> DL = new List<ScanDriverManifestModel>();
            ScanDriverManifestModel SEM = new ScanDriverManifestModel();
            SEM.Bags = new List<BagsModel>();
            SEM.Status = true;
            if (ScannedBags.Bags.Count > 0)
            {
                var Manifestdtl = dbContext.ExpressDriverManifests.Where(a => a.ExpressDriverManifestId == ScannedBags.DriverManifestId).FirstOrDefault();
                if (Manifestdtl != null)
                {
                    foreach (var Bag in ScannedBags.Bags)
                    {
                        if (Bag.BagId > 0)
                        {
                            var AwbDetail = dbContext.Expresses.Where(a => a.BagId == Bag.BagId).ToList();
                            if (AwbDetail.Count > 0)
                            {
                                foreach (var Awb in AwbDetail)
                                {
                                    var info = dbContext.ExpressBags.Where(x => x.BagId == Bag.BagId)
                                                        .Select(p => new { p.CreatedOn }).FirstOrDefault();
                                    if (info != null && info.CreatedOn.Value.AddDays(7).Date < DateTime.Now.Date)
                                    {
                                        //In case of expire no need to save trackingdetail for bag
                                    }
                                    else
                                    {
                                        var AwbDl = dbContext.TrackingDetails.Where(a => a.ShipmentId == Awb.ExpressId && a.MobileEventConfigurationId == ScannedBags.MobileEventId).FirstOrDefault();
                                        if (AwbDl == null)
                                        {
                                            TrackingDetail TD = new TrackingDetail();
                                            TD.ShipmentId = Awb.ExpressId;
                                            TD.ModuleType = FrayteShipmentServiceType.ExpressBooking;
                                            TD.MobileEventConfigurationId = ScannedBags.MobileEventId;
                                            TD.CreatedBy = ScannedBags.ScannedBy;
                                            var ScannedByDetail = GetScannedByDetail(ScannedBags.ScannedBy);
                                            if (ScannedByDetail != null)
                                            {
                                                TD.Country = ScannedByDetail.Country.Name;
                                                TD.City = ScannedByDetail.City;
                                            }
                                            TD.IsMissing = Bag.IsScanned == true ? false : true;
                                            TD.CreatedOn = DateTime.UtcNow;
                                            TD.Longitude = ScannedBags.Longitude;
                                            TD.Lattitude = ScannedBags.Lattitude;
                                            dbContext.TrackingDetails.Add(TD);
                                            dbContext.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }
                        var BagDtl = dbContext.ExpressBags.Where(a => a.BagId == Bag.BagId)
                                              .Select(p => new { p.CreatedOn }).FirstOrDefault();
                        if (BagDtl != null)
                        {
                            if (BagDtl.CreatedOn.Value.AddDays(7).Date < DateTime.Now.Date)
                            {
                                //In case of expire no need to save bagmanifesttracking for bag
                            }
                            else
                            {
                                var BagDL = dbContext.BagManifestTrackings.Where(a => a.BagId == Bag.BagId && a.MobileEventConfigurationId == ScannedBags.MobileEventId).FirstOrDefault();
                                if (BagDL == null)
                                {
                                    BagManifestTracking BMT = new BagManifestTracking();
                                    BMT.BagId = Bag.BagId;
                                    BMT.MobileEventConfigurationId = ScannedBags.MobileEventId;
                                    BMT.CreatedBy = ScannedBags.ScannedBy;
                                    var ScannedByDetail = GetScannedByDetail(ScannedBags.ScannedBy);
                                    if (ScannedByDetail != null)
                                    {
                                        BMT.Country = ScannedByDetail.Country.Name;
                                        BMT.City = ScannedByDetail.City;
                                    }
                                    BMT.CreatedOnUtc = DateTime.UtcNow;
                                    BMT.IsMissing = Bag.IsScanned == true ? false : true;
                                    BMT.Description = dbContext.MasterTrackingDetails.Where(a => a.MasterTrackingDetailId == ScannedBags.MobileEventId).FirstOrDefault().EventDisplay;
                                    dbContext.BagManifestTrackings.Add(BMT);
                                    dbContext.SaveChanges();
                                }
                                else
                                {
                                    BagsModel Ba = new BagsModel();
                                    Ba.Status = false;
                                    Ba.BagId = Bag.BagId;
                                    Ba.IsBagExpired = false;
                                    Ba.IsError = false;
                                    Ba.ErrorObject = new List<MobApiErrorObj>();
                                    MobApiErrorObj AEO = new MobApiErrorObj();
                                    AEO.ErrorCode = "Bag already scanned or wrong BagId.";
                                    AEO.ErrorMessage = "Bag already scanned or wrong BagId.";
                                    Ba.ErrorObject.Add(AEO);
                                    SEM.Status = false;
                                    SEM.Bags.Add(Ba);
                                }
                                if (Bag.BagId > 0)
                                {

                                }
                            }
                        }
                        else
                        {
                            BagsModel Ba = new BagsModel();
                            Ba.Status = false;
                            Ba.BagId = Bag.BagId;
                            Ba.IsBagExpired = false;
                            Ba.IsError = false;
                            Ba.ErrorObject = new List<MobApiErrorObj>();
                            MobApiErrorObj AEO = new MobApiErrorObj();
                            AEO.ErrorCode = "Bag does not exist.";
                            AEO.ErrorMessage = "Bag does not exist.";
                            Ba.ErrorObject.Add(AEO);
                            SEM.Status = false;
                            SEM.Bags.Add(Ba);
                        }
                    }

                    var Res = ScannedBags.Bags.GroupBy(a => a.Carrier);
                    foreach (var a in Res)
                    {
                        ScanDriverManifestModel SDM = new ScanDriverManifestModel();
                        SDM.Bags = new List<Models.Express.BagsModel>();
                        SDM.Carrier = a.Key;
                        foreach (var r in a)
                        {
                            var BagInfo = dbContext.ExpressBags.Where(x => x.BagId == r.BagId && r.IsScanned == true)
                                                   .Select(p => new { p.BagId, p.BagBarCode, p.Courier }).FirstOrDefault();
                            BagsModel BM = new BagsModel();
                            if (BagInfo != null)
                            {
                                BM.Couriers = new List<Couriers>();
                                BM.BagId = BagInfo.BagId;
                                BM.TotalShipments = dbContext.Expresses.Where(ab => ab.BagId == BagInfo.BagId).Count();
                                BM.BagNumber = BagInfo.BagBarCode;
                                BM.Carrier = BagInfo.Courier;

                                var carrier = (from ex in dbContext.Expresses
                                               join hcs in dbContext.HubCarrierServices on ex.HubCarrierServiceId equals hcs.HubCarrierServiceId
                                               join hc in dbContext.HubCarriers on hcs.HubCarrierId equals hc.HubCarrierId
                                               where ex.BagId == r.BagId
                                               select new
                                               {
                                                   TrackingNo = ex.TrackingNumber,
                                                   CarrierName = hc.Carrier,
                                                   CreatedOn = ex.CreatedOnUtc
                                               }).ToList();

                                if (carrier != null && carrier.Count > 0)
                                {
                                    foreach (var awb in carrier)
                                    {
                                        Couriers courier = new Couriers();
                                        if (awb.CarrierName == FrayteCourierCompany.BRING)
                                        {
                                            courier.CourierNumber = "00" + awb.TrackingNo;
                                            if (awb.CreatedOn.AddDays(7).Date < DateTime.Now.Date)
                                            {
                                                courier.IsBagExpired = true;
                                                courier.IsError = true;
                                                courier.ErrorMessage = "Parcel number or shipment can not older than 7 days.";
                                            }
                                        }
                                        else if (awb.CarrierName == FrayteCourierCompany.Yodel)
                                        {
                                            courier.CourierNumber = "J" + awb.TrackingNo;
                                            if (awb.CreatedOn.AddDays(7).Date < DateTime.Now.Date)
                                            {
                                                courier.IsBagExpired = true;
                                                courier.IsError = true;
                                                courier.ErrorMessage = "Parcel number or shipment can not older than 7 days.";
                                            }
                                        }
                                        else
                                        {
                                            courier.CourierNumber = awb.TrackingNo;
                                            if (awb.CreatedOn.AddDays(7).Date < DateTime.Now.Date)
                                            {
                                                courier.IsBagExpired = true;
                                                courier.IsError = true;
                                                courier.ErrorMessage = "Parcel number or shipment can not older than 7 days.";
                                            }
                                        }
                                        BM.Couriers.Add(courier);
                                    }
                                }
                                else
                                {
                                    var Awbs = dbContext.Expresses.Where(ad => ad.BagId == r.BagId).ToList();
                                    if (Awbs != null && Awbs.Count > 0)
                                    {
                                        foreach (var awb in Awbs)
                                        {
                                            Couriers courier = new Couriers();
                                            courier.CourierNumber = awb.TrackingNumber;
                                            if (awb.CreatedOnUtc.AddDays(7).Date < DateTime.Now.Date)
                                            {
                                                courier.IsBagExpired = true;
                                                courier.IsError = true;
                                                courier.ErrorMessage = "Parcel number or shipment can not older than 7 days.";
                                            }
                                            BM.Couriers.Add(courier);
                                        }
                                    }
                                    else
                                    {

                                    }
                                }
                            }
                            else
                            {
                                BM.Status = false;
                                BM.BagId = r.BagId;
                                BM.IsBagExpired = false;
                                BM.IsError = false;
                                BM.ErrorObject = new List<MobApiErrorObj>();
                                MobApiErrorObj AEO = new MobApiErrorObj();
                                AEO.ErrorCode = "Bag does not exist.";
                                AEO.ErrorMessage = "Bag does not exist.";
                                BM.ErrorObject.Add(AEO);
                                SDM.Status = false;
                            }
                            SDM.Bags.Add(BM);
                        }
                        DL.Add(SDM);
                    }
                }
                else
                {
                    SEM.Status = false;
                    SEM.ErrorObject = new List<MobApiErrorObj>();
                    MobApiErrorObj AEO = new MobApiErrorObj();
                    SEM.DriverManifestId = ScannedBags.DriverManifestId;
                    AEO.ErrorCode = "Driver manifest does not exist.";
                    AEO.ErrorMessage = "Driver manifest does not exist.";
                    SEM.ErrorObject.Add(AEO);
                    DL.Add(SEM);
                }
            }
            return DL;
        }

        //Final POD with Signature
        public ExpressApiErrorModel SavePodDocument(SavePODDocumentModel PODDocument)
        {
            ExpressApiErrorModel MAEO = new ExpressApiErrorModel();
            MAEO.Status = false;
            if (PODDocument.Photo != null && PODDocument.Photo.Length > 0)
            {
                var BagDetail = dbContext.ExpressBags.Where(a => a.BagId == PODDocument.BagId).FirstOrDefault();
                if (BagDetail != null)
                {
                    BagDetail.Photo = PODDocument.Photo;
                    dbContext.Entry(BagDetail).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    MAEO.Status = true;
                }
            }
            return MAEO;
        }

        //Awbs Scan after Driver Manifest Scanned
        public ScanDriverManifestModel AWBDriverScan(List<ScanDriverManifestModel> ScannedBags)
        {
            ScanDriverManifestModel SEM = new ScanDriverManifestModel();
            SEM.Bags = new List<BagsModel>();
            SEM.Status = true;
            BagsModel Ba = new BagsModel();
            Ba.Couriers = new List<Couriers>();
            if (ScannedBags.Count > 0)
            {
                foreach (var SB in ScannedBags)
                {
                    foreach (var Bag in SB.Bags)
                    {
                        if (Bag.BagId > 0)
                        {
                            var AwbDetail = dbContext.Expresses.Where(a => a.BagId == Bag.BagId).ToList();
                            if (AwbDetail.Count > 0)
                            {
                                foreach (var Awb in Bag.Couriers)
                                {
                                    if (SB.Carrier == FrayteCourierCompany.BRING)
                                    {
                                        Awb.CourierNumber = Awb.CourierNumber.Remove(0, 2);
                                    }
                                    else if (SB.Carrier == FrayteCourierCompany.Yodel)
                                    {
                                        Awb.CourierNumber = Awb.CourierNumber.Remove(0, 1);
                                    }

                                    var AWBId = dbContext.Expresses.Where(a => a.TrackingNumber == Awb.CourierNumber && Awb.IsScanned == true).FirstOrDefault();
                                    if (AWBId != null)
                                    {
                                        var ES = dbContext.Expresses.Find(AWBId.ExpressId);
                                        if (ES != null)
                                        {
                                            ES.ShipmentStatusId = 41;
                                            dbContext.Entry(ES).State = System.Data.Entity.EntityState.Modified;
                                            dbContext.SaveChanges();
                                        }

                                        var AwbDl = dbContext.TrackingDetails.Where(a => a.ShipmentId == AWBId.ExpressId && a.MobileEventConfigurationId == SB.MobileEventId).FirstOrDefault();
                                        if (AwbDl == null)
                                        {
                                            TrackingDetail TD = new TrackingDetail();
                                            TD.ShipmentId = AWBId.ExpressId;
                                            TD.ModuleType = FrayteShipmentServiceType.ExpressBooking;
                                            TD.MobileEventConfigurationId = SB.MobileEventId;
                                            TD.CreatedBy = SB.ScannedBy;
                                            var ScannedByDetail = GetScannedByDetail(SB.ScannedBy);
                                            if (ScannedByDetail != null)
                                            {
                                                TD.Country = ScannedByDetail.Country.Name;
                                                TD.City = ScannedByDetail.City;
                                            }
                                            TD.IsMissing = Awb.IsScanned == true ? false : true;
                                            TD.CreatedOn = DateTime.UtcNow;
                                            TD.Longitude = SB.Longitude;
                                            TD.Lattitude = SB.Lattitude;
                                            dbContext.TrackingDetails.Add(TD);
                                            dbContext.SaveChanges();

                                            SEM.Status = true;
                                            Couriers Co = new Couriers();
                                            Co.CourierNumber = Awb.CourierNumber;
                                            Co.Status = true;
                                            Ba.Couriers.Add(Co);
                                        }
                                        else
                                        {
                                            SEM.Status = false;
                                            Couriers Co = new Couriers();
                                            Co.Status = false;
                                            Co.CourierNumber = Awb.CourierNumber;
                                            Co.ErrorObject = new List<MobApiErrorObj>();
                                            MobApiErrorObj AEO = new MobApiErrorObj();
                                            AEO.ErrorCode = "Parcel already scanned.";
                                            AEO.ErrorMessage = "Parcel already scanned.";
                                            Co.ErrorObject.Add(AEO);
                                            Ba.Couriers.Add(Co);
                                        }
                                    }
                                    else
                                    {
                                        SEM.Status = false;
                                        Couriers Co = new Couriers();
                                        Co.Status = false;
                                        Co.CourierNumber = Awb.CourierNumber;
                                        Co.ErrorObject = new List<MobApiErrorObj>();
                                        MobApiErrorObj AEO = new MobApiErrorObj();
                                        AEO.ErrorCode = "AWB Does not exist or not scanned.";
                                        AEO.ErrorMessage = "AWB Does not existor not scanned.";
                                        Co.ErrorObject.Add(AEO);
                                        Ba.Couriers.Add(Co);
                                        SEM.Status = false;
                                    }
                                }
                            }
                            else
                            {
                                Ba.Status = false;
                                Ba.BagId = Bag.BagId;
                                Ba.ErrorObject = new List<MobApiErrorObj>();
                                MobApiErrorObj AEO = new MobApiErrorObj();
                                AEO.ErrorCode = "Bag does not exist.";
                                AEO.ErrorMessage = "Bag does not exist.";
                                Ba.ErrorObject.Add(AEO);
                                SEM.Status = false;
                            }
                        }
                        SEM.Bags.Add(Ba);
                    }
                }
            }
            return SEM;
        }

        //Driver Manifest Pod
        public ExpressApiErrorModel DriverPod(DriverPODModel DriverPOD)
        {
            ExpressApiErrorModel EAM = new ExpressApiErrorModel();
            var Res = dbContext.ExpressDriverManifests.Where(a => a.ExpressDriverManifestId == DriverPOD.DriverManifestId && a.PodSignature == null).FirstOrDefault();
            if (Res != null)
            {
                Res.PodSignature = DriverPOD.Signature;
                Res.SignedBy = DriverPOD.SignedBy;
                Res.PodBy = DriverPOD.ScannedBy;
                Res.PodOn = DateTime.UtcNow;
                dbContext.Entry(Res).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                EAM.Status = true;

                #region Old Code

                //foreach (BagsModel bag in DriverPOD.Bags)
                //{
                //    var Bags = dbContext.ExpressBags.Find(bag.BagId);
                //    if (Bags != null)
                //    {
                //        var bagmaifesttracking = dbContext.BagManifestTrackings.Where(a => a.BagId == Bags.BagId && a.IsMissing == false).FirstOrDefault();
                //        if (bagmaifesttracking != null)
                //        {
                //            foreach (Couriers courier in bag.Couriers)
                //            {
                //                if (DriverPOD.CourierName == FrayteCourierCompany.BRING)
                //                {
                //                    courier.CourierNumber = courier.CourierNumber.Remove(0, 2);
                //                }
                //                else if (DriverPOD.CourierName == FrayteCourierCompany.Yodel)
                //                {
                //                    courier.CourierNumber = courier.CourierNumber.Remove(0, 1);
                //                }

                //                var express = dbContext.Database.SqlQuery<Express>("select * from Express where BagId = " + Bags.BagId + " And TrackingNumber = " + courier.CourierNumber).FirstOrDefault();
                //                if (express != null)
                //                {
                //                    var ES = dbContext.Expresses.Find(express.ExpressId);
                //                    if (ES != null)
                //                    {
                //                        ES.ShipmentStatusId = 41;
                //                        dbContext.Entry(ES).State = System.Data.Entity.EntityState.Modified;
                //                        dbContext.SaveChanges();
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}

                //var Bags = dbContext.ExpressBags.Where(a => a.DriverManifestId == DriverPOD.DriverManifestId).ToList();
                //if (Bags.Count > 0)
                //{
                //    foreach (var b in Bags)
                //    {
                //        var bagmaifesttracking = dbContext.BagManifestTrackings.Where(a => a.BagId == b.BagId && a.IsMissing == false).FirstOrDefault();
                //        if (bagmaifesttracking != null)
                //        {
                //            var carrier = (from ex in dbContext.Expresses
                //                           join hcs in dbContext.HubCarrierServices on ex.HubCarrierServiceId equals hcs.HubCarrierServiceId
                //                           join hc in dbContext.HubCarriers on hcs.HubCarrierId equals hc.HubCarrierId
                //                           where ex.TrackingNumber == DriverPOD.TrackingNo
                //                           select hc).FirstOrDefault();

                //            if (carrier != null && carrier.Carrier == FrayteCourierCompany.BRING)
                //            {
                //                DriverPOD.TrackingNo = DriverPOD.TrackingNo.Remove(0, 2);
                //            }
                //            else if(carrier != null && carrier.Carrier == FrayteCourierCompany.Yodel)
                //            {
                //                DriverPOD.TrackingNo = DriverPOD.TrackingNo.Remove(0, 1);
                //            }

                //            var expresslst = dbContext.Expresses.Where(a => a.BagId == b.BagId).ToList();
                //            if (expresslst.Count > 0)
                //            {
                //                foreach (var s in expresslst)
                //                {
                //                    var TD = dbContext.TrackingDetails.Where(a => a.ShipmentId == s.ExpressId && a.IsMissing == true).FirstOrDefault();
                //                    if (TD == null)
                //                    {
                //                        var ES = dbContext.Expresses.Where(a => a.ExpressId == s.ExpressId).FirstOrDefault();
                //                        if (ES != null)
                //                        {
                //                            ES.ShipmentStatusId = 41;
                //                            dbContext.Entry(ES).State = System.Data.Entity.EntityState.Modified;
                //                            dbContext.SaveChanges();
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}

                #endregion                
            }
            else
            {
                EAM.Status = false;
            }
            return EAM;
        }

        public ZplTwoModel GetZpl2Image()
        {
            ZplTwoModel Zpl = new ZplTwoModel();
            var Image1Path = AppSettings.UploadFolderPath + "/Zpl/" + "AtualZPL1.txt";
            var Image2Path = AppSettings.UploadFolderPath + "/Zpl/" + "AtualZPL2.txt";
            Zpl.Image1 = File.ReadAllText(Image1Path);
            Zpl.Image2 = File.ReadAllText(Image2Path);
            return Zpl;
        }

        #region AvinashCode

        public string DeleteScannedAwb(List<string> scannedawb)
        {
            foreach (string expressId in scannedawb)
            {
                var express = dbContext.Expresses.Find(int.Parse(expressId));
                if (express != null)
                {
                    dbContext.Expresses.Remove(express);
                    dbContext.SaveChanges();
                }
            }
            return "Selected AWBs Deleted Successfully !";
        }

        public ReturnShipment CreateReturn(string TrackingNumber)
        {
            ReturnShipment shipment = new ReturnShipment();

            var db = (from ex in dbContext.Expresses
                      join eb in dbContext.ExpressBags on ex.BagId equals eb.BagId
                      join ea in dbContext.ExpressAddresses on ex.ToAddressId equals ea.ExpressAddressId
                      join hcs in dbContext.HubCarrierServices on ex.HubCarrierServiceId equals hcs.HubCarrierServiceId
                      join hh in dbContext.HubCarriers on hcs.HubCarrierId equals hh.HubCarrierId
                      join cc in dbContext.Countries on ea.CountryId equals cc.CountryId
                      where ex.TrackingNumber == TrackingNumber
                      select new ReturnShipment
                      {
                          TrackingNo = ex.TrackingNumber,
                          BagNo = eb.BagBarCode,
                          AwbNo = ex.AWBBarcode,
                          Carrier = hh.Carrier,
                          DeliveryAddress = new ReturnShipmentAddress()
                          {
                              FirstName = ea.ContactFirstName != null ? ea.ContactFirstName : "",
                              LastName = ea.ContactLastName != null ? ea.ContactLastName : "",
                              CompanyName = ea.CompanyName != null ? ea.CompanyName : "",
                              Email = ea.Email != null ? ea.Email : "",
                              Phone = ea.PhoneNo != null ? ea.PhoneNo : "",
                              Address = ea.Address1 != null ? ea.Address1 : "",
                              Address2 = ea.Address2 != null ? ea.Address2 : "",
                              Area = ea.Area != null ? ea.Area : "",
                              City = ea.City != null ? ea.City : "",
                              State = ea.State != null ? ea.State : "",
                              PostCode = ea.Zip != null ? ea.Zip : "",
                              CountryName = cc.CountryName != null ? cc.CountryName : ""
                          }

                      }).FirstOrDefault();

            if (db != null)
            {
                return shipment = db;
            }
            else
            {
                var track = (from ex in dbContext.Expresses
                             join ed in dbContext.ExpressDetails on ex.ExpressId equals ed.ExpressId
                             join edp in dbContext.ExpressDetailPackageLabels on ed.ExpressDetailId equals edp.ExpressShipmentDetailId
                             join eb in dbContext.ExpressBags on ex.BagId equals eb.BagId
                             join ea in dbContext.ExpressAddresses on ex.ToAddressId equals ea.ExpressAddressId
                             join hcs in dbContext.HubCarrierServices on ex.HubCarrierServiceId equals hcs.HubCarrierServiceId
                             join hh in dbContext.HubCarriers on hcs.HubCarrierId equals hh.HubCarrierId
                             join cc in dbContext.Countries on ea.CountryId equals cc.CountryId
                             where edp.TrackingNumber == TrackingNumber
                             select new ReturnShipment
                             {
                                 TrackingNo = edp.TrackingNumber,
                                 BagNo = eb.BagBarCode,
                                 AwbNo = ex.AWBBarcode,
                                 Carrier = hh.Carrier,
                                 DeliveryAddress = new ReturnShipmentAddress()
                                 {
                                     FirstName = ea.ContactFirstName != null ? ea.ContactFirstName : "",
                                     LastName = ea.ContactLastName != null ? ea.ContactLastName : "",
                                     CompanyName = ea.CompanyName != null ? ea.CompanyName : "",
                                     Email = ea.Email != null ? ea.Email : "",
                                     Phone = ea.PhoneNo != null ? ea.PhoneNo : "",
                                     Address = ea.Address1 != null ? ea.Address1 : "",
                                     Address2 = ea.Address2 != null ? ea.Address2 : "",
                                     Area = ea.Area != null ? ea.Area : "",
                                     City = ea.City != null ? ea.City : "",
                                     State = ea.State != null ? ea.State : "",
                                     PostCode = ea.Zip != null ? ea.Zip : "",
                                     CountryName = cc.CountryName != null ? cc.CountryName : ""
                                 }

                             }).FirstOrDefault();

                if (track != null)
                {
                    return shipment = track;
                }
                else if (db == null && track == null)
                {
                    shipment.Status = false;
                    shipment.ErrorObject = new List<MobApiErrorObj>();
                    MobApiErrorObj AEO = new MobApiErrorObj();
                    shipment.Status = false;
                    AEO.ErrorCode = "Tracking Number Does not exist.";
                    AEO.ErrorMessage = "Tracking Number Does not exist.";
                    shipment.ErrorObject.Add(AEO);
                }
            }
            return shipment;
        }

        public ExpressApiErrorModel ConfirmReturn(string AwbNo, bool IsActive)
        {
            ExpressApiErrorModel api = new ExpressApiErrorModel();

            var es = dbContext.Expresses.Where(x => x.AWBBarcode == AwbNo).FirstOrDefault();
            if (es != null)
            {
                es.IsActive = false;
                es.MobileEvent = "Shipment Return";
                es.ShipmentStatusId = 53;
                dbContext.Entry(es).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                api.Status = true;
            }
            else
            {
                api.ErrorObject = new List<MobApiErrorObj>();
                MobApiErrorObj obj = new MobApiErrorObj();
                obj.ErrorCode = "AWb Number does not exist.";
                obj.ErrorMessage = "AWb Number does not exist.";
                api.Status = false;
                api.ErrorObject.Add(obj);
            }
            return api;
        }
    }

    #endregion
}