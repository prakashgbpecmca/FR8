using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Threading.Tasks;
using Frayte.Services.Business;
using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using Frayte.Services.Utility;
using System.Data.Entity.Validation;
using RazorEngine;
using RazorEngine.Templating;
using System.IO;

namespace Frayte.Services.Business
{
    public class ReportSettingRepository
    {
        FrayteEntities dbContext = new FrayteEntities();
        string filpath = string.Empty;

        public List<FryateReportSetting> GetAllReportSettings()
        {
            List<FryateReportSetting> _frtsettings = new List<FryateReportSetting>();
            FryateReportSetting frt;
            try
            {
                var frtreportsettings = dbContext.CustomerSettings.ToList();
                foreach (var frset in frtreportsettings)
                {
                    frt = new FryateReportSetting();
                    frt.CustomerPODSettingId = frset.CustomerSettingId;
                    frt.UserId = frset.UserId;
                    frt.PODScheduleSetting = frset.ScheduleSetting;
                    frt.ScheduleType = frset.ScheduleType;
                    frt.ScheduleDate = frset.ScheduleDate.HasValue ? frset.ScheduleDate.Value : DateTime.Parse("01/01/1999");
                    frt.ScheduleDay = frset.ScheduleDay;
                    frt.ScheduleTime = frset.ScheduleTime;
                    frt.AdditionalMails = frset.AdditionalMails;
                    frt.ScheduleSettingType = frset.ScheduleSettingType;
                    frt.CreatedOn = frset.CreatedOn;
                    frt.UpdatedOn = frset.UpdatedOn;
                    frt.IsPdf = frset.IsPdf;
                    frt.IsExcel = frset.IsExcel;
                    frt.fryateUserDetail = new List<FryateUserDetail>();
                    frt.fryateUserDetail = (from fr in dbContext.Users
                                            where fr.UserId == frt.UserId
                                            select new FryateUserDetail
                                            {
                                                UserId = fr.UserId,
                                                UserName = fr.ContactName,
                                                Email = fr.Email,
                                                MobileNo = fr.MobileNo,
                                                OperationZoneId = fr.OperationZoneId
                                            }).ToList();
                    frt.fryateUserSettingDetail = new List<FryateUserSettingDetail>();
                    frt.fryateUserSettingDetail = (from usd in dbContext.CustomerLogistics
                                                   join ls in dbContext.LogisticServices on usd.LogisticServiceId equals ls.LogisticServiceId
                                                   where usd.UserId == frt.UserId
                                                   select new FryateUserSettingDetail
                                                   {
                                                       CustomerLogisticId = usd.CustomerLogisticId,
                                                       LogisticService = new FrayteLogisticServiceItem
                                                       {
                                                           LogisticServiceId = ls.LogisticServiceId,
                                                           OperationZoneId = ls.OperationZoneId,
                                                           LogisticCompany = ls.LogisticCompany,
                                                           LogisticCompanyDisplay = ls.LogisticCompanyDisplay,
                                                           LogisticType = ls.LogisticType,
                                                           LogisticTypeDisplay = ls.LogisticTypeDisplay,
                                                           RateType = ls.RateType,
                                                           RateTypeDisplay = ls.RateTypeDisplay,
                                                           ModuleType = ls.ModuleType
                                                       },
                                                   }).ToList();
                    _frtsettings.Add(frt);
                }
                return _frtsettings.OrderBy(p => p.UserId).ToList();
            }
            catch (Exception ex)
            {
                return _frtsettings;
            }
        }

        public List<FryateUserDetail> GetAllUserDetail()
        {
            try
            {
                var result = (from r in dbContext.Roles
                              join ur in dbContext.UserRoles on r.RoleId equals ur.RoleId
                              join u in dbContext.Users on ur.UserId equals u.UserId
                              join ua in dbContext.UserAdditionals on u.UserId equals ua.UserId                              
                              where 
                                    (ua.IsFuelSurCharge == true || ua.IsCurrency == true) && 
                                    ur.RoleId == (int)FrayteUserRole.Staff
                              select new FryateUserDetail
                              {
                                  UserId = u.UserId,
                                  UserName = u.ContactName,
                                  CompanyName = u.CompanyName,
                                  Email = u.Email,
                                  IsFuelSurCharge = ua.IsFuelSurCharge,
                                  IsCurrecy = ua.IsCurrency
                              }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void UpdateReportSettingDate(int CustomerPODSettingId, DateTime lastrun)
        {
            try
            {
                var rs = dbContext.CustomerSettings.Where(p => p.CustomerSettingId == CustomerPODSettingId).FirstOrDefault();
                if (rs != null && rs.CustomerSettingId > 0)
                {
                    rs.UpdatedOn = lastrun;
                    dbContext.Entry(rs).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void UpdateReportSettingTime_Date(int CustomerPODSettingId, DateTime lastrun, TimeSpan time)
        {
            try
            {
                var rs = dbContext.CustomerSettings.Where(p => p.CustomerSettingId == CustomerPODSettingId).FirstOrDefault();
                if (rs != null && rs.CustomerSettingId > 0)
                {
                    rs.UpdatedOn = lastrun;
                    rs.ScheduleTime = time;
                    // Update
                    dbContext.Entry(rs).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void UpdateDirectShipment(int UserId)
        {
            try
            {
                var shipment = dbContext.DirectShipments.Where(x => x.CustomerId == UserId).ToList();
                if (shipment != null)
                {
                    foreach (var sh in shipment)
                    {
                        sh.IsPODMailSent = true;
                        dbContext.Entry(sh).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void FuelSurChargeSendEmail(string toEmail, string ccEmail, string ContactName, string Attachment, int OperationZoneId)
        {
            string logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";
            string trackImage = AppSettings.EmailServicePath + "/Images/FuelSurCharge.png";

            List<string> ImagePath = new List<string>();
            ImagePath.Add(logoImage);
            ImagePath.Add(trackImage);

            DynamicViewBag viewBag = new DynamicViewBag();
            viewBag.AddValue("Staff", ContactName);
            viewBag.AddValue("StaffEmail", "accounts@frayte.com");
            viewBag.AddValue("ImageHeader", "FrayteLogo");
            viewBag.AddValue("TrackButton", "FuelSurCharge");
            if (OperationZoneId == 1)
            {
                viewBag.AddValue("FuelPath", AppSettings.TrackingUrl+  "/fuel-surcharge");
            }
            else
            {
                viewBag.AddValue("FuelPath", AppSettings.TrackingUrl + "/fuel-surcharge");
            }

            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/FuelSurCharge.cshtml");

            var templateService = new TemplateService();

            var EmailBody = templateService.Parse(template, "", viewBag, null);
            var EmailSubject = "Update Fuel Sur Charge - FRAYTE Logistics Ltd";

            FrayteEmail.SendMail(toEmail, ccEmail, EmailSubject, EmailBody, Attachment, ImagePath, "FuelCharge", OperationZoneId);
        }

        public void ExchangeRateSendEmail(string toEmail, string ccEmail, string ContactName, string Attachment, int OperationZoneId)
        {
            //Update Mail Status
            new ExchangeRateRepository().UpdateSendMailStatus(OperationZoneId);

            string logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";
            string trackImage = AppSettings.EmailServicePath + "/Images/ExchangeRate.png";

            List<string> ImagePath = new List<string>();
            ImagePath.Add(logoImage);
            ImagePath.Add(trackImage);

            DynamicViewBag viewBag = new DynamicViewBag();
            viewBag.AddValue("Staff", ContactName);
            viewBag.AddValue("StaffEmail", "accounts@frayte.com");
            viewBag.AddValue("ImageHeader", "FrayteLogo");
            viewBag.AddValue("TrackButton", "ExchangeRate");
            if(OperationZoneId == 1)
            {
                viewBag.AddValue("ExchangeRatePath", "http://app.frayte.com/");
            }
            else
            {
                viewBag.AddValue("ExchangeRatePath", "http://app.frayte.co.uk/");
            }

            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/ExchangeRate.cshtml");
            var templateService = new TemplateService();

            var EmailBody = templateService.Parse(template, "", viewBag, null);
            var EmailSubject = "Update Exchange Rate - FRAYTE Logistics Ltd";

            FrayteEmail.SendMail(toEmail, ccEmail, EmailSubject, EmailBody, Attachment, ImagePath, "ExchangeRate", OperationZoneId);
        }

        public void SendMail(string toEmail, string ccEmail, string EmailSubject, string EmailBody)
        {
            string logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";
            FrayteEmail.SendMail(toEmail, ccEmail, EmailSubject, EmailBody, filpath, logoImage);
        }

        public List<FrayteShipmentReport> GetShipmentId(int OperationZoneId)
        {
            List<FrayteShipmentReport> _shipmentpdf = new List<FrayteShipmentReport>();
            FrayteShipmentReport pdf;
            var shipment = dbContext.DirectShipments.Where(x => x.OpearionZoneId == OperationZoneId && x.ShipmentStatusId == (int)FrayteShipmentStatus.Past && x.IsPODMailSent == false).ToList();
            if (shipment != null && shipment.Count > 0)
            {
                foreach (var sh in shipment)
                {
                    var report = dbContext.spGet_NewShipmentReportDetail(sh.CustomerId);
                    if (report != null)
                    {
                        foreach (var rr in report)
                        {
                            pdf = new FrayteShipmentReport();
                            pdf.Destination = rr.Destination;
                            pdf.Country = rr.Country;
                            pdf.Booked_By = rr.Booked_by;
                            pdf.Consignee = rr.Consignee;
                            pdf.DeliveryDate = rr.DeliveryDate;
                            pdf.PODSign = rr.SignedBy;
                            _shipmentpdf.Add(pdf);
                        }
                    }
                }
            }
            return _shipmentpdf;
        }

        public void UpdateCustomerSettingInformation(int CustomerPODSettingId, int UserId)
        {
            var setting = dbContext.CustomerSettings.Where(p => p.CustomerSettingId == CustomerPODSettingId && p.UserId == UserId).FirstOrDefault();
            if (setting != null)
            {
                setting.UpdatedOn = DateTime.Now;
                dbContext.Entry(setting).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
        }
    }
}