using Frayte.Services.Models;
using Frayte.Services.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RazorEngine.Templating;
using Frayte.Services.Utility;
using System.IO;
using System.Diagnostics;
using XStreamline.Log;

namespace Frayte.Services.Business
{
    public class ManifestTrackingRepository
    {
        FrayteEntities dbContext = new FrayteEntities();
        public List<ManifestTrackingModel> ManifestTracking(ManifestTrackingModel MTM)
        {
            // Get Shipments from database according to manifestId
            var ManifestData = (from ECS in dbContext.eCommerceShipments
                                    //join EADF in dbContext.eCommerceShipmentAddresses on ECS.FromAddressId equals EADF.eCommerceShipmentAddressId
                                join EADT in dbContext.eCommerceShipmentAddresses on ECS.ToAddressId equals EADT.eCommerceShipmentAddressId
                                join PDD in dbContext.eCommerceShipmentDetails on ECS.eCommerceShipmentId equals PDD.eCommerceShipmentId
                                join TN in dbContext.eCommercePackageTrackingDetails on PDD.eCommerceShipmentDetailId equals TN.eCommerceShipmentDetailId
                                //join TD in dbContext.eCommercePackageTrackingDetails on PDD.eCommerceShipmentDetailId equals TD.eCommerceShipmentDetailId
                                //join Cur in dbContext.CurrencyTypes on ECS.CurrencyCode equals Cur.CurrencyCode
                                //join Cun in dbContext.Countries on EADF.CountryId equals Cun.CountryId
                                where
                                      ECS.ManifestId == MTM.ManifestId
                                select new ManifestTrackingModel
                                {
                                    eCommerceShipmentId = ECS.eCommerceShipmentId,
                                    TrackingNo = TN.TrackingNo,
                                    FrayteNo = ECS.FrayteNumber,
                                    CreatedOnUtc = DateTime.UtcNow,
                                    TrackingDescription = MTM.TrackingDescription,
                                    TrackingDescriptionCode = MTM.TrackingDescriptionCode,
                                    TrackingMode = MTM.TrackingMode,
                                    UserId = MTM.UserId,
                                    shipToEmail = EADT.Email
                                }).ToList();

            var MD = ManifestData.GroupBy(a => a.eCommerceShipmentId).Select(x => new ManifestTrackingModel
            {
                eCommerceShipmentId = x.Select(c => c.eCommerceShipmentId).First(),
                TrackingNo = x.Select(c => c.TrackingNo).First(),
                FrayteNo = x.Select(c => c.FrayteNo).First(),
                CreatedOnUtc = x.Select(c => c.CreatedOnUtc).First(),
                TrackingDescription = x.Select(c => c.TrackingDescription).First(),
                UserId = x.Select(c => c.UserId).First(),
                TrackingMode = x.Select(c => c.TrackingMode).First(),
                TrackingDescriptionCode = x.Select(c => c.TrackingDescriptionCode).First()
            }).ToList();

            //Save ManifestTracking

            var SaveStatus = SaveManifestTracking(MD);

            //Mail send to Receiver
            if (SaveStatus.Status == true)
            {
              
                Process.Start(AppSettings.eCommerceManifestBatchProcess, MTM.ManifestId.ToString());
            }

            return MD;
        }
        public void SendMailToReceivers(ManifestTrackingReceiver MTM)
        {
            string xsUserFolder = @"C:\FMS\" + "FrayteSchedularlog.txt";
            BaseLog.Instance.SetLogFile(xsUserFolder);
            Logger _log = Get_Log();
            //Get Customer Name and Customer User Detail
            var customerDetail = (from u in dbContext.Users
                                  join ua in dbContext.UserAdditionals on u.UserId equals ua.UserId
                                  join ua1 in dbContext.UserAdditionals on ua.OperationUserId equals ua1.UserId
                                  join u1 in dbContext.Users on ua1.UserId equals u1.UserId
                                  join tz in dbContext.Timezones on u.TimezoneId equals tz.TimezoneId
                                  where u.UserId == MTM.CustomerId
                                  select new
                                  {
                                      CustomerName = u.ContactName,
                                      CustomerEmail = u.Email,
                                      CompanyName = u.CompanyName,
                                      UserName = u1.ContactName,
                                      UserPosition = u1.Position,
                                      UserEmail = u1.Email,
                                      UserPhone = u1.TelephoneNo,
                                      UserSkype = u1.Skype,
                                      UserFax = u1.FaxNumber,
                                      TimeZoneDetail = new TimeZoneModal
                                      {
                                          Name = tz.Name,
                                          Offset = tz.Offset,
                                          OffsetShort = tz.OffsetShort,
                                          TimezoneId = tz.TimezoneId
                                      }
                                  }).FirstOrDefault();
            var operationzone = UtilityRepository.GetOperationZone();
            string logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";
            DynamicViewBag viewBag = new DynamicViewBag();
            if (customerDetail.TimeZoneDetail != null)
            {
                viewBag.AddValue("CreatedOn", UtilityRepository.GetTimeZoneCurrentDateTime(customerDetail.TimeZoneDetail.Name).ToString("dd-MMM-yyyy hh:mm")); ;
                viewBag.AddValue("TimeZone", customerDetail.TimeZoneDetail.OffsetShort);
            }
            else
            {
                viewBag.AddValue("CreatedOn", DateTime.Now.ToString("dd-MMM-yyyy hh:mm"));
            }
            
            viewBag.AddValue("TrackingDescription", MTM.TrackingDescription);
            viewBag.AddValue("TrackingNo", MTM.ReceiverTrackingNo);
            viewBag.AddValue("CustomerName", MTM.ReceiverName);
            viewBag.AddValue("UserEmail", customerDetail.CustomerEmail);
            viewBag.AddValue("UserPhone", customerDetail.UserPhone);
            viewBag.AddValue("ImageHeader", "FrayteLogo");

            if (operationzone.OperationZoneId == 1)
            {
                viewBag.AddValue("SiteAddress", AppSettings.TrackingUrl);
            }
            else
            {
                viewBag.AddValue("SiteAddress", AppSettings.TrackingUrl);
            }

            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/eCommerceManifestTracking.cshtml");
            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, MTM, viewBag, null);
            string EmailSubject = "Manifest Tracking";
            //var To = MTM.ReceiverMail;
            _log.Error(MTM.ReceiverMail);
            var To = "vikshit9292@gmail.com";
            var CC = customerDetail.UserEmail;
            string Status = "Confirmation";

            //Send mail to Customer
            //SendMail_New(To, CC, "FRAYTE (" + UtilityRepository.OperationZoneName(operationzone.OperationZoneId) + ")", EmailSubject, EmailBody, Attachment, Status);
            FrayteEmail.SendMail(To, CC, EmailSubject, EmailBody, logoImage);
        }

        public static Logger Get_Log()
        {
            Logger log = BaseLog.Instance.GetLogger(null);
            return log;
        }

        public FrayteResult SaveManifestTracking(List<ManifestTrackingModel> MTM)
        {
            FrayteResult FR = new FrayteResult();
            try
            {
                foreach (var MT in MTM)
                {
                    var EMT = dbContext.eCommerceTrackings.Where(a => a.eCommerceShipmentId == MT.eCommerceShipmentId).FirstOrDefault();
                    if (EMT == null)
                    {
                        eCommerceTracking EMTNew = new eCommerceTracking();
                        EMTNew.eCommerceShipmentId = MT.eCommerceShipmentId;
                        EMTNew.FrayteNumber = MT.FrayteNo;
                        EMTNew.TrackingDescription = MT.TrackingDescription;
                        EMTNew.TrackingMode = MT.TrackingMode;
                        EMTNew.TrackingNumber = MT.TrackingNo;
                        EMTNew.TrackingDescriptionCode = MT.TrackingDescriptionCode;
                        EMTNew.CreatedBy = MT.CreatedBy;
                        EMTNew.CreatedOnUtc = MT.CreatedOnUtc;
                        EMTNew.CreatedBy = MT.UserId;
                        dbContext.eCommerceTrackings.Add(EMTNew);
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        EMT.eCommerceShipmentId = MT.eCommerceShipmentId;
                        EMT.FrayteNumber = MT.FrayteNo;
                        EMT.TrackingDescription = MT.TrackingDescription;
                        EMT.TrackingMode = MT.TrackingDescription;
                        EMT.TrackingNumber = MT.TrackingNo;
                        EMT.TrackingDescriptionCode = MT.TrackingDescriptionCode;
                        EMT.CreatedBy = MT.CreatedBy;
                        EMT.CreatedBy = MT.UserId;
                        EMT.CreatedOnUtc = MT.CreatedOnUtc;
                        dbContext.Entry(EMT).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                }
                FR.Status = true;
            }
            catch (Exception e)
            {
                FR.Status = false;
            }
            return FR;
        }

        public List<ManifestTrackingReceiver> GetManifestTracking(string ManifestId)
        {
            int MI = Convert.ToInt32(ManifestId);
            // Get Shipments from database according to manifestId
            var ManifestData = (from ECS in dbContext.eCommerceShipments
                                    //join EADF in dbContext.eCommerceShipmentAddresses on ECS.FromAddressId equals EADF.eCommerceShipmentAddressId
                                join EADT in dbContext.eCommerceShipmentAddresses on ECS.ToAddressId equals EADT.eCommerceShipmentAddressId
                                join PDD in dbContext.eCommerceShipmentDetails on ECS.eCommerceShipmentId equals PDD.eCommerceShipmentId
                                join TN in dbContext.eCommercePackageTrackingDetails on PDD.eCommerceShipmentDetailId equals TN.eCommerceShipmentDetailId
                                //join TD in dbContext.eCommercePackageTrackingDetails on PDD.eCommerceShipmentDetailId equals TD.eCommerceShipmentDetailId
                                //join Cur in dbContext.CurrencyTypes on ECS.CurrencyCode equals Cur.CurrencyCode
                                //join Cun in dbContext.Countries on EADF.CountryId equals Cun.CountryId
                                where
                                      ECS.ManifestId == MI
                                select new ManifestTrackingModel
                                {
                                    eCommerceShipmentId = ECS.eCommerceShipmentId,
                                    TrackingNo = TN.TrackingNo,
                                    FrayteNo = ECS.FrayteNumber,
                                    CreatedOnUtc = DateTime.UtcNow,
                                    UserId = ECS.CustomerId,
                                    shipToEmail = EADT.Email,
                                    ReceiverName = EADT.ContactFirstName + " " + EADT.ContactLastName
                                }).ToList();

            var Receivers = ManifestData.GroupBy(a => a.shipToEmail).Select(x => new ManifestTrackingModel
            {
                eCommerceShipmentId = x.Select(c => c.eCommerceShipmentId).First(),
                TrackingNo = x.Select(c => c.TrackingNo).First(),
                FrayteNo = x.Select(c => c.FrayteNo).First(),
                CreatedOnUtc = x.Select(c => c.CreatedOnUtc).First(),
                UserId = x.Select(c => c.UserId).First(),
                shipToEmail = x.Select(c => c.shipToEmail).First(),
                ReceiverName = x.Select(c => c.ReceiverName).First()

            }).ToList();

            var TotalShipment = ManifestData.GroupBy(a => a.eCommerceShipmentId).Select(x => new ManifestTrackingModel
            {
                eCommerceShipmentId = x.Select(c => c.eCommerceShipmentId).First(),
                TrackingNo = x.Select(c => c.TrackingNo).First(),
                FrayteNo = x.Select(c => c.FrayteNo).First(),
                CreatedOnUtc = x.Select(c => c.CreatedOnUtc).First(),
                UserId = x.Select(c => c.UserId).First()

            }).ToList();
            
            List<ManifestTrackingReceiver> MTR = new List<ManifestTrackingReceiver>();
            foreach (var RCV in Receivers)
            {
                var TrackingDescription = dbContext.eCommerceTrackings.Where(a => a.eCommerceShipmentId == RCV.eCommerceShipmentId).FirstOrDefault();
                ManifestTrackingReceiver MT = new ManifestTrackingReceiver();
                MT.CustomerId = RCV.UserId;
                MT.ReceiverMail = RCV.shipToEmail;
                MT.ReceiverName = RCV.ReceiverName;
                //MT.TrackingDescription = RCV.TrackingDescription;
                MT.TrackingDescription = TrackingDescription.TrackingDescription;
                MT.ReceiverTrackingNo = new List<ManifestReceiverTrackingNos>();
                foreach (var TS in TotalShipment)
                {
                    ManifestReceiverTrackingNos MRT = new ManifestReceiverTrackingNos();
                    MRT.ReceiverTrackingNo = TS.TrackingNo;
                    MRT.TrackingUrl = AppSettings.TrackingUrl + "/tracking-hub//" + TS.TrackingNo + "/";
                    MT.ReceiverTrackingNo.Add(MRT);
                }
                MTR.Add(MT);
            }

            //Send mail to receiver
            foreach (var MData in MTR)
            {
                SendMailToReceivers(MData);
            }

            return MTR;
        }

    }
}
