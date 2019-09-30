using Frayte.Services.DataAccess;
using Frayte.Services.Models.Express;
using Frayte.Services.Utility;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frayte.Services.Models;
using System.Web;

namespace Frayte.Services.Business
{
    public class ExpressEmailRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        public ExpressEmailModel ExpressEmailObj(int shipmentId)
        {
            try
            {
                ExpressEmailModel model = new ExpressEmailModel();
                model.ShipmentDetail = new ExpressRepository().ScannedShipmentDetail(shipmentId, "");
                return model;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public FrayteResult SendExportManifest(ExpressEmailModel emailModel, string FilePath, int UserId)
        {
            FrayteResult result = new FrayteResult();
            emailModel.ImageHeader = "FrayteLogo";
            try
            {
                var operationzone = UtilityRepository.GetOperationZone();

                var result1 = (from Usr in dbContext.Users
                               join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId
                               join UA in dbContext.UserAdditionals on Usr.UserId equals UA.UserId into UAA
                               from UA in UAA.DefaultIfEmpty()
                               join OU in dbContext.Users on UA.OperationUserId equals OU.UserId into OUU
                               from OU in OUU.DefaultIfEmpty()
                               where Usr.UserId == emailModel.ManifestDetail.CustomerId
                               select new
                               {
                                   UserName = OU.ContactName,
                                   CustomerName = Usr.ContactName,
                                   StaffEmail = OU.Email,
                                   UserEmail = Rl.RoleId == 3 ? Usr.UserEmail : Usr.Email,
                                   UserPosition = OU.Position,
                                   CutomerCompany = Usr.CompanyName
                               }).FirstOrDefault();

                var result2 = (from Usr in dbContext.Users
                               join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId
                               join UA in dbContext.UserAdditionals on Usr.UserId equals UA.UserId into UAA
                               from UA in UAA.DefaultIfEmpty()
                               join OU in dbContext.Users on UA.OperationUserId equals OU.UserId into OUU
                               from OU in OUU.DefaultIfEmpty()
                               where Usr.UserId == UserId
                               select new
                               {
                                   UserName = Usr.ContactName,
                                   StaffEmail = OU.Email,
                                   UserEmail = Rl.RoleId == 3 ? Usr.UserEmail : Usr.Email,
                                   UserPosition = OU.Position,
                                   CutomerCompany = Usr.CompanyName
                               }).FirstOrDefault();


                if (operationzone.OperationZoneId == 1)
                {
                    emailModel.UserEmail = result1.StaffEmail;
                    emailModel.UserName = result1.UserName;
                    emailModel.UserPosition = result1.UserPosition;
                    emailModel.UserPhone = "(+852) 2148 4880";
                    //Res.SiteAddress = AppSettings.TrackingUrl;
                    emailModel.SiteCompany = "FRAYTE GLOBAL";
                    emailModel.SiteAddress = "www.FRAYTE.com";
                }
                if (operationzone.OperationZoneId == 2)
                {
                    emailModel.UserEmail = result1.StaffEmail;
                    emailModel.UserName = result1.UserName;
                    emailModel.UserPosition = result1.UserPosition;
                    emailModel.SiteCompany = "FRAYTE GLOBAL";
                    emailModel.UserPhone = "(+44) 01792 277295";
                    // Res.SiteAddress = AppSettings.TrackingUrl;
                    emailModel.SiteAddress = "www.FRAYTE.co.uk";
                }

                emailModel.CreatedOn = emailModel.ManifestDetail.CreatedOn != null ? emailModel.ManifestDetail.CreatedOn.Value.ToString("dd-MMM-yyyy hh:mm") : "";
                emailModel.CustomerName = result2.UserName;
                string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/Express/EXS_E5.cshtml");

                var templateService = new TemplateService();
                var EmailBody = templateService.Parse(template, emailModel, null, null);
                string EmailSubject = string.Empty;
                EmailSubject = result1.CutomerCompany + " - Origin Manifest  - " + emailModel.ManifestDetail.MAWB;

                var To = result2.UserEmail;
                var Status = "";

                #region Attach Labels 

                string Attachment = FilePath;
                //string Attachment = "";

                #endregion

                //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Customer Email " + emailModel.ShipmentDetail.FrayteNumber));
                //Send mail to Customer
                Send_FrayteEmail(To, AppSettings.TOCC, "FRAYTE - Booking (" + UtilityRepository.OperationZoneName(operationzone.OperationZoneId) + ")", EmailSubject, EmailBody, Attachment, Status, emailModel.ManifestDetail.CustomerId);
                result.Status = true;
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(EmailBody));
            }
            catch (Exception ex)
            {
                result.Status = false;
            }
            return result;
        }

        public FrayteResult SendDriverManifest(ExpressEmailModel emailModel, string FilePath, ExpressDownloadPDFModel TL)
        {
            FrayteResult result = new FrayteResult();
            emailModel.ImageHeader = "FrayteLogo";
            try
            {
                var operationzone = UtilityRepository.GetOperationZone();
                var exprtManifest = dbContext.ExpressManifests.Where(b => b.BarCode == emailModel.DriverManifestDetail.Barcode).FirstOrDefault();
                if (exprtManifest != null)
                {
                    emailModel.CreatedOn = exprtManifest != null ? exprtManifest.CreatedOn.Value.ToString("dd-MMM-yyyy hh:mm") : "";
                }

                var result1 = (from Usr in dbContext.Users
                               join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId
                               join UA in dbContext.UserAdditionals on Usr.UserId equals UA.UserId into UAA
                               from UA in UAA.DefaultIfEmpty()
                               join OU in dbContext.Users on UA.OperationUserId equals OU.UserId into OUU
                               from OU in OUU.DefaultIfEmpty()
                               where Usr.UserId == TL.CustomerId
                               select new
                               {
                                   UserName = Usr.ContactName,
                                   StaffName = OU.ContactName,
                                   StaffEmail = OU.Email,
                                   UserEmail = Rl.RoleId == 3 ? Usr.UserEmail : Usr.Email,
                                   UserPosition = OU.Position,
                                   CutomerCompany = Usr.CompanyName
                               }).FirstOrDefault();

                var result2 = (from Usr in dbContext.Users
                               join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId
                               join UA in dbContext.UserAdditionals on Usr.UserId equals UA.UserId into UAA
                               from UA in UAA.DefaultIfEmpty()
                               join OU in dbContext.Users on UA.OperationUserId equals OU.UserId into OUU
                               from OU in OUU.DefaultIfEmpty()
                               where Usr.UserId == TL.UserId
                               select new
                               {
                                   UserName = OU.ContactName,
                                   StaffEmail = OU.Email,
                                   UserEmail = Rl.RoleId == 3 ? Usr.UserEmail : Usr.Email,
                                   UserPosition = OU.Position,
                                   CutomerCompany = Usr.CompanyName
                               }).FirstOrDefault();


                if (operationzone.OperationZoneId == 1)
                {
                    emailModel.UserEmail = result1.StaffEmail;
                    emailModel.UserName = result1.StaffName;
                    emailModel.UserPosition = result1.UserPosition;
                    emailModel.UserPhone = "(+852) 2148 4880";
                    //Res.SiteAddress = AppSettings.TrackingUrl;
                    emailModel.SiteCompany = "FRAYTE GLOBAL";
                    emailModel.SiteAddress = "www.FRAYTE.com";
                }
                if (operationzone.OperationZoneId == 2)
                {
                    emailModel.UserEmail = result1.StaffEmail;
                    emailModel.UserName = result1.StaffName;
                    emailModel.UserPosition = result1.UserPosition;
                    emailModel.SiteCompany = "FRAYTE GLOBAL";
                    emailModel.UserPhone = "(+44) 01792 277295";
                    // Res.SiteAddress = AppSettings.TrackingUrl;
                    emailModel.SiteAddress = "www.FRAYTE.co.uk";
                }

                //emailModel.CreatedOn = emailModel.DriverManifestDetail != null ? emailModel.ShipmentDetail.CreatedOnUtc.ToString("dd-MMM-yyyy hh:mm") : "";
                emailModel.CustomerName = result1.UserName;
                string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/Express/EXS_E5_1.cshtml");

                var templateService = new TemplateService();
                var EmailBody = templateService.Parse(template, emailModel, null, null);
                string EmailSubject = string.Empty;
                EmailSubject = result1.CutomerCompany + " - Destination Manifest  - " + emailModel.DriverManifestDetail.MAWB;

                var To = result2.UserEmail;
                var Status = "";

                #region Attach Labels 

                string Attachment = FilePath;
                //string Attachment = "";
                #endregion

                //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Customer Email " + emailModel.ShipmentDetail.FrayteNumber));
                //Send mail to Customer
                Send_FrayteEmail(To, AppSettings.TOCC, "FRAYTE - Booking (" + UtilityRepository.OperationZoneName(operationzone.OperationZoneId) + ")", EmailSubject, EmailBody, Attachment, Status, TL.CustomerId);
                result.Status = true;
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(EmailBody));
            }
            catch (Exception ex)
            {
                result.Status = false;
            }

            return result;

        }

        public void SendAWBsMail(List<ExpressShipmentBookedConsolidateModel> Shipments, int UserId)
        {
            ExpressEmailModel Res = new ExpressEmailModel();
            Res.AWbShipmentList = Shipments;
            var AwbsCount = 0;
            var TotalWeight = 0m;

            foreach (var ship in Shipments)
            {
                AwbsCount = AwbsCount + ship.Shipments.Count;
                TotalWeight = TotalWeight + ship.Shipments.Sum(a => a.TotalWeight);
            }

            Res.TotalAwbs = AwbsCount;
            Res.TotalWeight = TotalWeight;
            Res.CreatedOn = DateTime.UtcNow.Date.AddDays(-1).ToString("dd-MMM-yyyy");
            string logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";
            List<string> ImagePath = new List<string>();
            ImagePath.Add(logoImage);
            var OperationName = UtilityRepository.GetOperationZone();
            string Site = string.Empty;

            if (OperationName.OperationZoneId == 1)
            {
                //Res.SiteAddress = AppSettings.TrackingUrl;
            }
            else if (OperationName.OperationZoneId == 2)
            {
                //Res.SiteAddress = AppSettings.TrackingUrl;
            }

            Res.ImageHeader = "FrayteLogo";
            string OperationUserEmail = "";
            int RoleId = 0;
            var Email = "";

            var result = (from Usr in dbContext.Users
                          join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId
                          join UA in dbContext.UserAdditionals on Usr.UserId equals UA.UserId
                          where Usr.UserId == UserId

                          select new
                          {
                              Usr.UserEmail,
                              Rl.RoleId,
                              Usr.ContactName,
                              Usr.CompanyName,
                              UA.OperationUserId
                          }).FirstOrDefault();

            if (result != null)
            {
                RoleId = result.RoleId;
                Email = result.UserEmail;
                Res.CustomerCompanyName = result.CompanyName;
            }

            var result1 = (from Usr in dbContext.Users
                           join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId
                           join UA in dbContext.UserAdditionals on Usr.UserId equals UA.UserId
                           where Usr.UserId == result.OperationUserId
                           select new
                           {
                               Usr.Email,
                               Rl.RoleId,
                               Usr.ContactName,
                               Usr.CompanyName,
                               UA.OperationUserId,
                               Usr.Position
                           }).FirstOrDefault();

            if (result1 != null)
            {
                RoleId = result1.RoleId;
                OperationUserEmail = result1.Email;
            }

            if (OperationName.OperationZoneId == 1)
            {
                Res.UserEmail = result1.Email;
                Res.UserName = result1.ContactName;
                Res.UserPosition = result1.Position;
                Res.CustomerName = result.ContactName;
                Res.CustomerEmail = result.UserEmail;
                Res.SystemEmail = "system@frayte.com";
                Res.UserPhone = "(+852) 2148 4880";
                //Res.SiteAddress = AppSettings.TrackingUrl;
                Res.Site = "www.FRAYTE.com";
            }

            if (OperationName.OperationZoneId == 2)
            {
                Res.UserEmail = result1.Email;
                Res.UserPosition = result1.Position;
                Res.UserName = result1.ContactName;
                Res.CustomerName = result.ContactName;
                Res.CustomerEmail = result.UserEmail;
                Res.SystemEmail = "system@frayte.co.uk";
                Res.UserPhone = "(+44) 01792 277295";
                // Res.SiteAddress = AppSettings.TrackingUrl;
                Res.Site = "www.FRAYTE.co.uk";
            }

            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/Express/EXS_EE1.cshtml");
            var EmailBody = Engine.Razor.RunCompile(template, "User10", null, Res, null);
            var EmailSubject = "";
            var Mawbvar = "";
            var Status = "";

            if (!string.IsNullOrEmpty(Res.CustomerCompanyName))
            {
                EmailSubject = Res.CustomerCompanyName + " - EXS Booking Summary – " + DateTime.UtcNow.Date.AddDays(-1).ToString("dd-MMM-yyyy");
            }

            FrayteEmail.SendMail(Email, "", EmailSubject, EmailBody, "", logoImage);

            ExpressSchedulerEmail ExpSchEm1 = new ExpressSchedulerEmail();
            ExpSchEm1.CustomerId = UserId;
            ExpSchEm1.EmailSentOn = DateTime.UtcNow.Date;
            ExpSchEm1.EmailContent = EmailBody;
            dbContext.ExpressSchedulerEmails.Add(ExpSchEm1);
            dbContext.SaveChanges();
        }

        public void SendDeliveredAWBsMail(List<ExpressShipmentDeliveredConsolidateModel> Shipments, int UserId)
        {
            ExpressEmailModel Res = new ExpressEmailModel();
            Res.DeliveredShipmentList = Shipments;
            var AwbsCount = 0;
            var TotalWeight = 0m;
            Res.ShipInfo = new List<ExpressMissingShipmentsInfo>();

            foreach (var ship in Shipments)
            {
                ExpressMissingShipmentsInfo EMSI = new ExpressMissingShipmentsInfo();
                EMSI.Hub = ship.HubCode;
                var TotalShipment = ship.MAWBBags.Sum(s => s.BagInfo.Sum(a => a.TotalShipments)).ToString();
                var DeliveredCount = ship.MAWBBags.Sum(s => s.BagInfo.Sum(a => a.DeliveredShipments)).ToString();
                EMSI.ShipmentCount = DeliveredCount + "/" + TotalShipment;
                Res.ShipInfo.Add(EMSI);
                AwbsCount = AwbsCount + ship.MAWBBags.Count;
                TotalWeight = TotalWeight + ship.MAWBBags.Sum(a => a.BagInfo.Sum(ab => ab.TotalWeight));
            }

            Res.TotalAwbs = AwbsCount;
            Res.TotalWeight = TotalWeight;
            Res.CreatedOn = DateTime.UtcNow.Date.AddDays(-1).ToString("dd-MMM-yyyy");
            string logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";
            List<string> ImagePath = new List<string>();
            ImagePath.Add(logoImage);
            var OperationName = UtilityRepository.GetOperationZone();
            string Site = string.Empty;

            if (OperationName.OperationZoneId == 1)
            {
                //Res.SiteAddress = AppSettings.TrackingUrl;
            }

            else if (OperationName.OperationZoneId == 2)
            {
                //Res.SiteAddress = AppSettings.TrackingUrl;
            }

            Res.ImageHeader = "FrayteLogo";
            string OperationUserEmail = "";
            int RoleId = 0;
            var Email = "";

            var result = (from Usr in dbContext.Users
                          join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId
                          join UA in dbContext.UserAdditionals on Usr.UserId equals UA.UserId
                          where Usr.UserId == UserId
                          select new
                          {
                              Usr.UserEmail,
                              Rl.RoleId,
                              Usr.ContactName,
                              Usr.CompanyName,
                              UA.OperationUserId

                          }).FirstOrDefault();

            if (result != null)
            {
                RoleId = result.RoleId;
                Email = result.UserEmail;
                Res.CustomerCompanyName = result.CompanyName;
            }

            var result1 = (from Usr in dbContext.Users
                           join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId
                           join UA in dbContext.UserAdditionals on Usr.UserId equals UA.UserId
                           where Usr.UserId == result.OperationUserId
                           select new
                           {
                               Usr.Email,
                               Rl.RoleId,
                               Usr.ContactName,
                               Usr.CompanyName,
                               UA.OperationUserId,
                               Usr.Position

                           }).FirstOrDefault();

            if (result1 != null)
            {
                RoleId = result1.RoleId;
                OperationUserEmail = result1.Email;
            }

            if (OperationName.OperationZoneId == 1)
            {
                Res.UserEmail = result1.Email;
                Res.UserName = result1.ContactName;
                Res.UserPosition = result1.Position;
                Res.CustomerName = result.ContactName;
                Res.CustomerEmail = result.UserEmail;
                Res.SystemEmail = "system@frayte.com";
                Res.UserPhone = "(+852) 2148 4880";
                //Res.SiteAddress = AppSettings.TrackingUrl;
                Res.Site = "www.FRAYTE.com";
            }

            if (OperationName.OperationZoneId == 2)
            {
                Res.UserEmail = result1.Email;
                Res.UserPosition = result1.Position;
                Res.UserName = result1.ContactName;
                Res.CustomerName = result.ContactName;
                Res.CustomerEmail = result.UserEmail;
                Res.SystemEmail = "system@frayte.co.uk";
                Res.UserPhone = "(+44) 01792 277295";
                // Res.SiteAddress = AppSettings.TrackingUrl;
                Res.Site = "www.FRAYTE.co.uk";
            }

            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/Express/EXS_E3.cshtml");
            var EmailBody = Engine.Razor.RunCompile(template, "Usr1", null, Res, null);
            var EmailSubject = "";
            var Mawbvar = "";
            var Status = "";

            if (!string.IsNullOrEmpty(Res.CustomerCompanyName))
            {
                EmailSubject = Res.CustomerCompanyName + " - EXS Delivered Shipment Summary  – " + DateTime.UtcNow.Date.AddDays(-1).ToString("dd-MMM-yyyy");
            }

            FrayteEmail.SendMail(Email, "", EmailSubject, EmailBody, "", logoImage);

            ExpressSchedulerEmail ExpSchEm1 = new ExpressSchedulerEmail();
            ExpSchEm1.CustomerId = UserId;
            ExpSchEm1.EmailSentOn = DateTime.UtcNow.Date;
            ExpSchEm1.EmailContent = EmailBody;
            dbContext.ExpressSchedulerEmails.Add(ExpSchEm1);
            dbContext.SaveChanges();

        }

        public void SendDispatchedAWBsMail(List<ExpressDispatchedMawbModel> Mawbs, List<ExpressShipmentBookedConsolidateModel> Shipments, int UserId)
        {
            ExpressEmailModel Res = new ExpressEmailModel();
            Res.TotalMawb = Mawbs.Count;
            //foreach (var mawb in Mawbs)
            //{
            //    MawbCount = MawbCount + mawb.GrossWeight
            //}
            Res.DispatchedMawbs = Mawbs;
            Res.AWbShipmentList = Shipments;
            DynamicViewBag viewBag = new DynamicViewBag();
            var Hubs = Shipments.Select(a => a.HubCode).Distinct();
            Res.ShipList = new List<ExpressHubShipmentCountModel>();

            foreach (var h in Hubs)
            {
                var ShipmentCount = 0;
                foreach (var Sh in Shipments)
                {
                    if (h == Sh.HubCode)
                    {
                        ShipmentCount = ShipmentCount + Sh.Shipments.Count;
                    }
                }

                Res.ShipList.Add(new ExpressHubShipmentCountModel { HubCode = h, ShipmentCount = ShipmentCount });
            }

            Res.TotalWeight = Mawbs.Sum(a => a.GrossWeight);
            Res.ShipInfo = new List<ExpressMissingShipmentsInfo>();

            Res.TotalAwbs = 0;
            for (int i = 0; i < Shipments.Count; i++)
            {
                Res.TotalAwbs = Res.TotalAwbs + Shipments[i].Shipments.Count;
            }

            Res.CreatedOn = DateTime.UtcNow.Date.AddDays(-1).ToString("dd-MMM-yyyy");
            string logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";
            List<string> ImagePath = new List<string>();
            ImagePath.Add(logoImage);
            var OperationName = UtilityRepository.GetOperationZone();
            string Site = string.Empty;

            if (OperationName.OperationZoneId == 1)
            {
                //Res.SiteAddress = AppSettings.TrackingUrl;
            }

            else if (OperationName.OperationZoneId == 2)
            {
                //Res.SiteAddress = AppSettings.TrackingUrl;
            }

            Res.ImageHeader = "FrayteLogo";
            string OperationUserEmail = "";
            int RoleId = 0;
            var Email = "";

            var result = (from Usr in dbContext.Users
                          join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId
                          join UA in dbContext.UserAdditionals on Usr.UserId equals UA.UserId
                          where Usr.UserId == UserId
                          select new
                          {
                              Usr.UserEmail,
                              Rl.RoleId,
                              Usr.ContactName,
                              Usr.CompanyName,
                              UA.OperationUserId

                          }).FirstOrDefault();

            if (result != null)
            {
                RoleId = result.RoleId;
                Email = result.UserEmail;
                Res.CustomerCompanyName = result.CompanyName;
            }

            var result1 = (from Usr in dbContext.Users
                           join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId
                           join UA in dbContext.UserAdditionals on Usr.UserId equals UA.UserId
                           where Usr.UserId == result.OperationUserId
                           select new
                           {
                               Usr.Email,
                               Rl.RoleId,
                               Usr.ContactName,
                               Usr.CompanyName,
                               UA.OperationUserId,
                               Usr.Position

                           }).FirstOrDefault();

            if (result1 != null)
            {
                RoleId = result1.RoleId;
                OperationUserEmail = result1.Email;
            }

            if (OperationName.OperationZoneId == 1)
            {
                Res.UserEmail = result1.Email;
                Res.UserName = result1.ContactName;
                Res.UserPosition = result1.Position;
                Res.CustomerName = result.ContactName;
                Res.CustomerEmail = result.UserEmail;
                Res.SystemEmail = "system@frayte.com";
                Res.UserPhone = "(+852) 2148 4880";
                //Res.SiteAddress = AppSettings.TrackingUrl;
                Res.Site = "www.FRAYTE.com";
            }

            if (OperationName.OperationZoneId == 2)
            {
                Res.UserEmail = result1.Email;
                Res.UserPosition = result1.Position;
                Res.UserName = result1.ContactName;
                Res.CustomerName = result.ContactName;
                Res.CustomerEmail = result.UserEmail;
                Res.SystemEmail = "system@frayte.co.uk";
                Res.UserPhone = "(+44) 01792 277295";
                // Res.SiteAddress = AppSettings.TrackingUrl;
                Res.Site = "www.FRAYTE.co.uk";
            }

            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/Express/EXS_E2.cshtml");
            var EmailBody = Engine.Razor.RunCompile(template, "Usr3", null, Res, null);
            var EmailSubject = "";
            var Mawbvar = "";
            var Status = "";
            if (!string.IsNullOrEmpty(Res.CustomerCompanyName))
            {
                EmailSubject = Res.CustomerCompanyName + " - EXS Dispatched Shipment Summary   – " + DateTime.UtcNow.Date.AddDays(-1).ToString("dd-MMM-yyyy");
            }

            FrayteEmail.SendMail(Email, "", EmailSubject, EmailBody, "", logoImage);

            ExpressSchedulerEmail ExpSchEm1 = new ExpressSchedulerEmail();
            ExpSchEm1.CustomerId = UserId;
            ExpSchEm1.EmailSentOn = DateTime.UtcNow.Date;
            ExpSchEm1.EmailContent = EmailBody;
            dbContext.ExpressSchedulerEmails.Add(ExpSchEm1);
            dbContext.SaveChanges();
        }

        public void SendPODAWBsMail(List<ExpressDispatchedMawbModel> Mawbs, List<ExpressShipmentBookedConsolidateModel> Shipments, int UserId)
        {
            ExpressEmailModel Res = new ExpressEmailModel();
            Res.TotalMawb = Mawbs.Count;
            Res.DispatchedMawbs = Mawbs;
            Res.AWbShipmentList = Shipments;
            var Hubs = Shipments.Select(a => a.HubCode).Distinct();
            Res.ShipList = new List<ExpressHubShipmentCountModel>();

            foreach (var h in Hubs)
            {
                var ShipmentCount = 0;

                foreach (var Sh in Shipments)
                {
                    if (h == Sh.HubCode)
                    {
                        ShipmentCount = ShipmentCount + Sh.Shipments.Count;
                    }
                }
                Res.ShipList.Add(new ExpressHubShipmentCountModel { HubCode = h, ShipmentCount = ShipmentCount });
            }

            Res.TotalWeight = Mawbs.Sum(a => a.GrossWeight);
            Res.ShipInfo = new List<ExpressMissingShipmentsInfo>();

            Res.TotalAwbs = 0;
            for (int i = 0; i < Shipments.Count; i++)
            {
                Res.TotalAwbs = Res.TotalAwbs + Shipments[i].Shipments.Count;
            }

            Res.CreatedOn = DateTime.UtcNow.Date.AddDays(-1).ToString("dd-MMM-yyyy");
            string logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";
            List<string> ImagePath = new List<string>();
            ImagePath.Add(logoImage);
            var OperationName = UtilityRepository.GetOperationZone();
            Res.ImageHeader = "FrayteLogo";
            string OperationUserEmail = "";
            int RoleId = 0;
            var Email = "";

            var result = (from Usr in dbContext.Users
                          join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId
                          join UA in dbContext.UserAdditionals on Usr.UserId equals UA.UserId
                          where Usr.UserId == UserId
                          select new
                          {
                              Usr.UserEmail,
                              Rl.RoleId,
                              Usr.ContactName,
                              Usr.CompanyName,
                              UA.OperationUserId
                          }).FirstOrDefault();

            if (result != null)
            {
                RoleId = result.RoleId;
                Email = result.UserEmail;
                Res.CustomerCompanyName = result.CompanyName;
            }

            var result1 = (from Usr in dbContext.Users
                           join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId
                           join UA in dbContext.UserAdditionals on Usr.UserId equals UA.UserId
                           where Usr.UserId == result.OperationUserId
                           select new
                           {
                               Usr.Email,
                               Rl.RoleId,
                               Usr.ContactName,
                               Usr.CompanyName,
                               UA.OperationUserId,
                               Usr.Position
                           }).FirstOrDefault();

            if (result1 != null)
            {
                RoleId = result1.RoleId;
                OperationUserEmail = result1.Email;
            }

            if (OperationName.OperationZoneId == 1)
            {
                Res.UserEmail = result1.Email;
                Res.UserName = result1.ContactName;
                Res.UserPosition = result1.Position;
                Res.CustomerName = result.ContactName;
                Res.CustomerEmail = result.UserEmail;
                Res.SystemEmail = "system@frayte.com";
                Res.UserPhone = "(+852) 2148 4880";
                //Res.SiteAddress = AppSettings.TrackingUrl;
                Res.Site = "www.FRAYTE.com";
            }

            if (OperationName.OperationZoneId == 2)
            {
                Res.UserEmail = result1.Email;
                Res.UserPosition = result1.Position;
                Res.UserName = result1.ContactName;
                Res.CustomerName = result.ContactName;
                Res.CustomerEmail = result.UserEmail;
                Res.SystemEmail = "system@frayte.co.uk";
                Res.UserPhone = "(+44) 01792 277295";
                // Res.SiteAddress = AppSettings.TrackingUrl;
                Res.Site = "www.FRAYTE.co.uk";
            }

            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/Express/EXS_E2_POD.cshtml");
            var EmailBody = Engine.Razor.RunCompile(template, "Usr3", null, Res, null);
            var EmailSubject = "";
            var Mawbvar = "";
            var Status = "";
            if (!string.IsNullOrEmpty(Res.CustomerCompanyName))
            {
                EmailSubject = Res.CustomerCompanyName + " - EXS POD Summary   – " + DateTime.UtcNow.Date.AddDays(-1).ToString("dd-MMM-yyyy");
            }

            FrayteEmail.SendMail(Email, "", EmailSubject, EmailBody, "", logoImage);

            ExpressSchedulerEmail ExpSchEm1 = new ExpressSchedulerEmail();
            ExpSchEm1.CustomerId = UserId;
            ExpSchEm1.EmailSentOn = DateTime.UtcNow.Date;
            ExpSchEm1.EmailContent = EmailBody;
            dbContext.ExpressSchedulerEmails.Add(ExpSchEm1);
            dbContext.SaveChanges();
        }

        public void SendMissingAWBsMail(List<ExpressMissingShipmentModel> Awbs, int UserId)
        {
            ExpressEmailModel Res = new ExpressEmailModel();
            Res.ImageHeader = "FrayteLogo";
            Res.TotalMawb = Awbs.Count;
            //foreach (var mawb in Mawbs)
            //{
            //    MawbCount = MawbCount + mawb.GrossWeight
            //}
            Res.MissingAwbs = Awbs;

            Res.TotalWeight = Awbs.Sum(a => a.TotalWeight);
            Res.ShipInfo = new List<ExpressMissingShipmentsInfo>();
            Res.TotalAwbs = Awbs.Count;
            Res.CreatedOn = DateTime.UtcNow.Date.AddDays(-1).ToString("dd-MMM-yyyy");
            string logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";
            List<string> ImagePath = new List<string>();
            ImagePath.Add(logoImage);
            var OperationName = UtilityRepository.GetOperationZone();
            string Site = string.Empty;
            if (OperationName.OperationZoneId == 1)
            {
                //Res.SiteAddress = AppSettings.TrackingUrl;
            }
            else if (OperationName.OperationZoneId == 2)
            {
                //Res.SiteAddress = AppSettings.TrackingUrl;
            }
            Res.ImageHeader = "FrayteLogo";
            string OperationUserEmail = "";
            int RoleId = 0;
            var Email = "";

            var result = (from Usr in dbContext.Users
                          join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId
                          join UA in dbContext.UserAdditionals on Usr.UserId equals UA.UserId
                          where Usr.UserId == UserId
                          select new
                          {
                              Usr.UserEmail,
                              Rl.RoleId,
                              Usr.ContactName,
                              Usr.CompanyName,
                              UA.OperationUserId
                          }).FirstOrDefault();
            if (result != null)
            {
                RoleId = result.RoleId;
                Email = result.UserEmail;
                Res.CustomerCompanyName = result.CompanyName;
            }

            var result1 = (from Usr in dbContext.Users
                           join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId
                           join UA in dbContext.UserAdditionals on Usr.UserId equals UA.UserId
                           where Usr.UserId == result.OperationUserId
                           select new
                           {
                               Usr.Email,
                               Rl.RoleId,
                               Usr.ContactName,
                               Usr.CompanyName,
                               UA.OperationUserId,
                               Usr.Position
                           }).FirstOrDefault();

            if (result1 != null)
            {
                RoleId = result1.RoleId;
                OperationUserEmail = result1.Email;
            }
            if (OperationName.OperationZoneId == 1)
            {
                Res.UserEmail = result1.Email;
                Res.UserName = result1.ContactName;
                Res.UserPosition = result1.Position;
                Res.CustomerName = result.ContactName;
                Res.CustomerEmail = result.UserEmail;
                Res.SystemEmail = "system@frayte.com";
                Res.UserPhone = "(+852) 2148 4880";
                //Res.SiteAddress = AppSettings.TrackingUrl;
                Res.Site = "www.FRAYTE.com";
            }
            if (OperationName.OperationZoneId == 2)
            {
                Res.UserEmail = result1.Email;
                Res.UserPosition = result1.Position;
                Res.UserName = result1.ContactName;
                Res.CustomerName = result.ContactName;
                Res.CustomerEmail = result.UserEmail;
                Res.SystemEmail = "system@frayte.co.uk";
                Res.UserPhone = "(+44) 01792 277295";
                // Res.SiteAddress = AppSettings.TrackingUrl;
                Res.Site = "www.FRAYTE.co.uk";
            }

            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/Express/EXS_E4.cshtml");
            var EmailBody = Engine.Razor.RunCompile(template, "Usr4", null, Res, null);
            var EmailSubject = "";
            var Mawbvar = "";
            var Status = "";
            if (!string.IsNullOrEmpty(Res.CustomerCompanyName))
            {
                EmailSubject = Res.CustomerCompanyName + " - EXS Discrepancies Shipment Summary – " + DateTime.UtcNow.Date.AddDays(-1).ToString("dd-MMM-yyyy");
            }

            FrayteEmail.SendMail(Email, "", EmailSubject, EmailBody, "", logoImage);

            ExpressSchedulerEmail ExpSchEm1 = new ExpressSchedulerEmail();
            ExpSchEm1.CustomerId = UserId;
            ExpSchEm1.EmailSentOn = DateTime.UtcNow.Date;
            ExpSchEm1.EmailContent = EmailBody;
            dbContext.ExpressSchedulerEmails.Add(ExpSchEm1);
            dbContext.SaveChanges();

        }

        public FrayteResult SendLabelEmail(ExpressEmailModel emailModel)
        {
            FrayteResult result = new FrayteResult();
            emailModel.ImageHeader = "FrayteLogo";
            try
            {
                var operationzone = UtilityRepository.GetOperationZone();

                var result1 = (from s in dbContext.Expresses
                               join Usr in dbContext.Users on s.CustomerId equals Usr.UserId
                               join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId
                               join UA in dbContext.UserAdditionals on Usr.UserId equals UA.UserId
                               join OU in dbContext.Users on UA.OperationUserId equals OU.UserId
                               join OUA in dbContext.UserAdditionals on OU.UserId equals OUA.UserId
                               where Usr.UserId == emailModel.ShipmentDetail.CustomerId
                               select new
                               {
                                   UserName = OU.ContactName,
                                   Name = Usr.ContactName,
                                   UserEmail = OU.Email,
                                   UserPosition = OU.Position,
                                   CutomerCompany = Usr.CompanyName
                               }).FirstOrDefault();

                var detail = dbContext.CustomerCompanyDetails.Where(p => p.UserId == emailModel.ShipmentDetail.CustomerId).FirstOrDefault();

                if (detail != null)
                {
                    emailModel.StaffUserEmail = detail.OperationStaffEmail;
                    emailModel.UserPosition = detail.UserPosition;
                    emailModel.StaffUserName = detail.OperationStaff;
                    emailModel.CompanyName = detail.CompanyName;
                    emailModel.UserPhone = detail.OperationStaffPhone;
                    emailModel.SiteAddress = detail.SiteAddress;
                    emailModel.TrackingWebsite = detail.TrackingUrl + "/#/tracking/" + emailModel.ShipmentDetail.AWBNumber.Replace(" ", "");
                    emailModel.TrackingURL = detail.TrackingUrl + "/#/tracking/" + emailModel.ShipmentDetail.TrackingNumber.Replace("Order_", "");
                }
                else
                {
                    emailModel.UserEmail = result1.UserEmail;
                    emailModel.UserName = result1.UserName;
                    emailModel.UserPosition = result1.UserPosition;
                    emailModel.SiteCompany = "FRAYTE GLOBAL";
                    emailModel.UserPhone = UtilityRepository.GetOperationZone().OperationZoneId == 1 ? "(+852) 2148 4880" : "(+44) 01792 277295";
                    emailModel.SiteAddress = UtilityRepository.GetOperationZone().OperationZoneId == 1 ? "www.FRAYTE.com" : "www.FRAYTE.co.uk";
                    emailModel.TrackingWebsite = UtilityRepository.GetOperationZone().OperationZoneId == 1 ? "https://frayte.com/tracking-detail/" + emailModel.ShipmentDetail.AWBNumber.Replace(" ", "") : "https://frayte.co.uk/tracking-detail/" + emailModel.ShipmentDetail.AWBNumber.Replace(" ", "");
                    emailModel.TrackingURL = AppSettings.TrackingUrl + "/tracking-detail/" + emailModel.ShipmentDetail.TrackingNumber.Replace("Order_", ""); ;
                }

                emailModel.TotalCarton = emailModel.ShipmentDetail.Packages.Sum(p => p.CartonValue);
                emailModel.TotalWeight = emailModel.ShipmentDetail.Packages.Sum(p => p.CartonValue * p.Weight);
                emailModel.CreatedOn = emailModel.ShipmentDetail.CreatedOnUtc != null ? emailModel.ShipmentDetail.CreatedOnUtc.ToString("dd-MMM-yyyy hh:mm") : "";
                emailModel.CustomerName = result1.Name;
                string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/Express/EXS_E1_3.cshtml");

                var templateService = new TemplateService();
                var EmailBody = templateService.Parse(template, emailModel, null, null);
                string EmailSubject = string.Empty;
                EmailSubject = result1.CutomerCompany + " - Shipment Detail with Label - " + emailModel.ShipmentDetail.AWBNumber;

                var To = emailModel.To;
                var Status = "";

                #region Attach Labels 

                string Attachment = PackageLabelPath_EXS_E1(emailModel.ShipmentDetail.ExpressId);

                #endregion

                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Customer Email " + emailModel.ShipmentDetail.FrayteNumber));
                //Send mail to Customer
                Send_FrayteEmail(To, AppSettings.TOCC, "FRAYTE - Booking (" + UtilityRepository.OperationZoneName(operationzone.OperationZoneId) + ")", EmailSubject, EmailBody, Attachment, Status, emailModel.ShipmentDetail.CustomerId);
                result.Status = true;
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(EmailBody));
            }
            catch (Exception ex)
            {
                result.Status = false;
            }

            return result;
        }

        private string PackageLabelPath_EXS_E1(int expressId)
        {
            string attachments = string.Empty;

            var shipment = dbContext.Expresses.Find(expressId);

            if (shipment != null)
            {
                FrayteLogicalPhysicalPath path = new ExpressRepository().GetShipmentLogisticlabelPath(expressId);

                //= new ExpressRepository().GetShipmentAlllabelPath(expressId); 

                string pdfFileName = string.Empty;
                string physycalpath = string.Empty;
                var collection = (from r in dbContext.ExpressDetails
                                  join l in dbContext.ExpressDetailPackageLabels on r.ExpressDetailId equals l.ExpressShipmentDetailId
                                  where r.ExpressId == expressId
                                  select l).ToList();

                if (collection.Count > 0)
                {
                    foreach (var item in collection)
                    {
                        if (path != null)
                        {
                            if (path.PhysicalPath.Contains("~"))
                            {
                                physycalpath = HttpContext.Current.Server.MapPath(path.PhysicalPath + "PackageLabel/Express/" + expressId + "/");
                                attachments += ";" + physycalpath + item.PackageLabelName.Replace(".jpg", ".pdf");
                            }
                            else
                            {
                                attachments += ";" + path.PhysicalPath + "/PackageLabel/Express/" + expressId + "/" + item.PackageLabelName.Replace(".jpg", ".pdf"); ;
                            }
                        }
                        else
                        {
                            physycalpath = HttpContext.Current.Server.MapPath("~/PackageLabel/Express/" + expressId + "/");
                            attachments += ";" + physycalpath + item.PackageLabelName.Replace(".jpg", ".pdf"); ;
                        }
                    }
                }

                if (path != null)
                {
                    if (path.PhysicalPath.Contains("~"))
                    {
                        // For developement
                        pdfFileName = AppSettings.WebApiPath + "PackageLabel/Express/" + expressId + "/";
                        physycalpath = HttpContext.Current.Server.MapPath(path.PhysicalPath + "PackageLabel/Express/" + expressId + "/");
                        attachments += ";" + physycalpath + shipment.LogisticLabel;
                    }
                    else
                    {
                        physycalpath = path.PhysicalPath + "/PackageLabel/Express/" + expressId + "/";
                        attachments += ";" + physycalpath + shipment.LogisticLabel;
                    }
                }
                else
                {
                    physycalpath = HttpContext.Current.Server.MapPath("~/PackageLabel/Express/" + expressId + "/");
                    attachments += ";" + physycalpath + shipment.LogisticLabel;
                }

            }
            return attachments;
        }

        #region Email

        private void Send_FrayteEmail(string toEmail, string ccEmail, string DisplayName, string EmailSubject, string EmailBody, string AttachmentFilePath, string Status, int customerId)
        {
            var detail = dbContext.CustomerCompanyDetails.Where(p => p.UserId == customerId).FirstOrDefault();

            string logoImage = detail == null ? AppSettings.EmailServicePath + "/Images/FrayteLogo.png" : AppSettings.EmailServicePath + "/EmailTeamplate/" + customerId + "/Images/" + detail.LogoFileName;
            string trackImage = AppSettings.EmailServicePath + "/Images/TrackShipment.png";
            string amdentImage = AppSettings.EmailServicePath + "/Images/Amend.png";
            string confirmImage = AppSettings.EmailServicePath + "/Images/Confirm.png";
            string rejectImage = AppSettings.EmailServicePath + "/Images/Reject.png";

            List<string> ImagePath = new List<string>();
            if (Status == "Confirmation")
            {
                ImagePath.Add(logoImage);
                ImagePath.Add(trackImage);
            }
            else if (Status == "Cancel")
            {
                ImagePath.Add(logoImage);
                ImagePath.Add(rejectImage);
            }
            else if (Status == "Amend")
            {
                ImagePath.Add(logoImage);
                ImagePath.Add(amdentImage);
            }
            else if (Status == "Tracking")
            {
                ImagePath.Add(logoImage);
                ImagePath.Add(trackImage);
            }
            //For Special Customer
            else
            {
                ImagePath.Add(logoImage);
            }

            FrayteEmail.SendFrayteEmail(toEmail, ccEmail, DisplayName, EmailSubject, EmailBody, AttachmentFilePath, ImagePath, Status, customerId);
        }

        #endregion
    }
}
