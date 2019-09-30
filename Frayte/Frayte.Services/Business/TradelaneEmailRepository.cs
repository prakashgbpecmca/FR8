﻿using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using Frayte.Services.Utility;
using RazorEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using RazorEngine.Templating;
using System.Globalization;
using System.Net;
using System.Drawing;
using System.Drawing.Imaging;
using Elmah;
using Frayte.Services.Models.Aftership;
using Frayte.Services.Models.Tradelane;
using Frayte.Services.Models.ApiModal;
using XStreamline.Log;

namespace Frayte.Services.Business
{
    public class TradelaneEmailRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        #region PreAlert Email

        public FrayteResult SendEmail_E5(TradelaneEmailModel model, TradelanePreAlertInitial preAlerDetail)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                FillBookingInformationEmailModel(model);
                model.ImageHeader = "FrayteLogo";
                var OpeartionZone = UtilityRepository.GetOperationZone();
                if (OpeartionZone.OperationZoneId == 1)
                {
                    model.Site = "www.FRAYTE.com";
                }
                else
                {
                    model.Site = "www.FRAYTE.co.uk";
                }
                var userDetail = (from r in dbContext.Users
                                  join ua in dbContext.UserAdditionals on r.UserId equals ua.UserId
                                  join uad in dbContext.UserAddresses on r.UserId equals uad.UserId
                                  join c in dbContext.Countries on uad.CountryId equals c.CountryId
                                  join ur in dbContext.UserRoles on r.UserId equals ur.UserId
                                  where r.UserId == preAlerDetail.UserId
                                  select new
                                  {
                                      Email = r.Email,
                                      UserEmail = r.UserEmail,
                                      Name = r.ContactName,
                                      Phone = r.PhoneNumber,
                                      TelePhone = r.TelephoneNo,
                                      RoleId = ur.RoleId,
                                      CountryPhoneCode = c.CountryPhoneCode
                                  }).FirstOrDefault();

                if (userDetail != null)
                {
                    DynamicViewBag viewBag = new DynamicViewBag();
                    viewBag.AddValue("CreatedOn", DateTime.Now.ToString("dd-MMM-yyyy hh:mm"));

                    if (model.ShipmentDetail.ShipmentHandlerMethod.ShipmentHandlerMethodId != 5)
                    {
                        var alloation = new MawbAllocationRepository().GetMawbAllocation(model.ShipmentDetail.TradelaneShipmentId, "").FirstOrDefault();
                        if (alloation != null && alloation.ETD.HasValue && alloation.ETA.HasValue && !string.IsNullOrEmpty(alloation.ETDTime) && !string.IsNullOrEmpty(alloation.ETATime))
                        {
                            var ETDTM = alloation.ETDTime.Substring(0, 2) + ':' + alloation.ETDTime.Substring(2, 2);
                            var ETATM = alloation.ETATime.Substring(0, 2) + ':' + alloation.ETATime.Substring(2, 2);
                            viewBag.AddValue("ETD_Shipment", alloation.ETD.Value.ToString("dd-MMM-yyyy") + " " + ETDTM);
                            viewBag.AddValue("ETA_Shipment", alloation.ETA.Value.ToString("dd-MMM-yyyy") + " " + ETATM);
                        }
                        else
                        {
                            viewBag.AddValue("ETD_Shipment", "");
                            viewBag.AddValue("ETA_Shipment", "");
                        }
                    }
                    else
                    {
                        var alloation = new MawbAllocationRepository().GetMawbAllocation(model.ShipmentDetail.TradelaneShipmentId, "Leg1").FirstOrDefault();
                        if (alloation != null && alloation.ETD.HasValue && alloation.ETA.HasValue && !string.IsNullOrEmpty(alloation.ETDTime) && !string.IsNullOrEmpty(alloation.ETATime))
                        {
                            var ETDTM = alloation.ETDTime.Substring(0, 2) + ':' + alloation.ETDTime.Substring(2, 2);
                            var ETATM = alloation.ETATime.Substring(0, 2) + ':' + alloation.ETATime.Substring(2, 2);
                            viewBag.AddValue("ETD_Shipment", alloation.ETD.Value.ToString("dd-MMM-yyyy") + " " + ETDTM);
                            viewBag.AddValue("ETA_Shipment", alloation.ETA.Value.ToString("dd-MMM-yyyy") + " " + ETATM);
                        }
                        else
                        {
                            viewBag.AddValue("ETD_Shipment", "");
                            viewBag.AddValue("ETA_Shipment", "");
                        }
                    }

                    viewBag.AddValue("EmailBody", preAlerDetail.EmailBody);
                    viewBag.AddValue("AdditionalInfo", preAlerDetail.AdditionalInfo);

                    string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/Tradelane/E5.cshtml");
                    var templateService = new TemplateService();
                    string EmailSubject = preAlerDetail.EmailSubject;
                    var EmailBody = templateService.Parse(template, model, viewBag, null);

                    //Set To , CC , BCC  
                    string TO = string.Empty;
                    string CC = string.Empty;
                    string BCC = string.Empty;

                    TO = new TradelaneShipmentRepository().PreAlertEmails(preAlerDetail, "TO");
                    CC = new TradelaneShipmentRepository().PreAlertEmails(preAlerDetail, "CC");
                    BCC = new TradelaneShipmentRepository().PreAlertEmails(preAlerDetail, "BCC");

                    CC += userDetail.RoleId == (int)FrayteUserRole.Customer ? userDetail.UserEmail : userDetail.Email;

                    string Status = "Confirmation";

                    #region Attach Labels

                    string Attachments = new TradelaneShipmentRepository().PreAlertShipmentDocuments(preAlerDetail);

                    #endregion

                    //Send mail to Customer
                    Send_PreAlertEmail(TO, CC, BCC, userDetail.RoleId == (int)FrayteUserRole.Customer ? userDetail.UserEmail : userDetail.Email, EmailSubject, EmailBody, Attachments, Status);
                    result.Status = true;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
            }
            return result;
        }

        #endregion

        #region  Traelane Email Model 

        public TradelaneEmailModel TradelaneEmailObj(int shipmentId)
        {
            try
            {
                TradelaneEmailModel model = new TradelaneEmailModel();
                model.ShipmentDetail = new TradelaneBookingRepository().GetTradelaneBookingDetails(shipmentId, "");
                return model;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion

        #region Customer Action Emails

        public void SendEmail_E2_1(TradelaneEmailModel emailModel)
        {
            //Get Customer Name and Customer User Detail
            var customerDetail = (from u in dbContext.Users

                                  join ua in dbContext.UserAdditionals on u.UserId equals ua.UserId
                                  join ua1 in dbContext.UserAdditionals on ua.OperationUserId equals ua1.UserId
                                  join u1 in dbContext.Users on ua1.UserId equals u1.UserId
                                  join cum in dbContext.Users on ua.ManagerUserId equals cum.UserId into leftjoinCManagerTemp
                                  from CustomerManager in leftjoinCManagerTemp.DefaultIfEmpty()
                                  join uma in dbContext.Users on ua1.ManagerUserId equals uma.UserId into leftjoininUManagerTemp
                                  from UserManager in leftjoininUManagerTemp.DefaultIfEmpty()
                                  where u.UserId == emailModel.ShipmentDetail.CustomerId
                                  select new
                                  {
                                      StaffName = u1.ContactName,
                                      StaffEmail = u1.Email,
                                      StaffCompany = u1.CompanyName,
                                      CustomerName = u.ContactName,
                                      CustomerEmail = u.UserEmail,
                                      CompanyName = u.CompanyName,
                                      StaffManagerEmail = UserManager == null ? "" : UserManager.Email,
                                      StaffManagerName = UserManager == null ? "" : UserManager.ContactName,
                                      CustomerLineMangerEmail = CustomerManager == null ? "" : CustomerManager.Email,
                                      CustomerLineManagerName = CustomerManager == null ? "" : CustomerManager.ContactName,
                                      TimeZoneId = u.TimezoneId
                                  }).FirstOrDefault();


            var shipmentDetails = (from u in dbContext.Users
                                   where u.UserId == emailModel.ShipmentDetail.CreatedBy
                                   select new
                                   {
                                       CreatedByName = u.ContactName,
                                       CreatedByEmail = u.UserEmail,
                                       CreatedByCompany = u.CompanyName,
                                   }).FirstOrDefault();

            emailModel.SiteAddress = AppSettings.TrackingUrl;
            emailModel.TO = shipmentDetails.CreatedByEmail + ";" + customerDetail.StaffEmail + (!string.IsNullOrEmpty(customerDetail.StaffManagerEmail) ? ";" + customerDetail.StaffManagerEmail : "");
            emailModel.CC = (!string.IsNullOrEmpty(customerDetail.StaffManagerEmail) ? ";" + customerDetail.StaffManagerEmail : "") + (!string.IsNullOrEmpty(customerDetail.CustomerLineMangerEmail) ? ";" + customerDetail.CustomerLineMangerEmail : "");
            emailModel.BCC = string.Empty;
            emailModel.ReplyTo = string.Empty;
            emailModel.CompanyName = customerDetail.CompanyName != null && customerDetail.CompanyName != "" ? customerDetail.CompanyName : customerDetail.CustomerName;
            emailModel.CustomerName = customerDetail.CustomerName != null && customerDetail.CustomerName != "" ? customerDetail.CustomerName : "";

            var operationzone = UtilityRepository.GetOperationZone();

            if (operationzone != null && operationzone.OperationZoneId == 1)
            {
                emailModel.Site = "www.FRAYTE.com";
            }
            else
            {
                emailModel.Site = "www.FRAYTE.co.uk";
            }
            DynamicViewBag viewBag = new DynamicViewBag();

            var timeZone = dbContext.Timezones.Where(p => p.TimezoneId == customerDetail.TimeZoneId).FirstOrDefault();
            var Timezone = new TimeZoneModal();
            if (timeZone != null)
            {

                Timezone.TimezoneId = timeZone.TimezoneId;
                Timezone.Name = timeZone.Name;
                Timezone.Offset = timeZone.Offset;
                Timezone.OffsetShort = timeZone.OffsetShort;
            }

            var TimeZoneDetail = TimeZoneInfo.FindSystemTimeZoneById(timeZone.Name);
            DateTime createdon = dbContext.TradelaneShipments.Where(a => a.TradelaneShipmentId == emailModel.ShipmentDetail.TradelaneShipmentId).FirstOrDefault().CreatedOnUtc;

            var CreatedOn1 = UtilityRepository.UtcDateToOtherTimezone(createdon, createdon.TimeOfDay, TimeZoneDetail).Item1;
            emailModel.CreatedOnDate = CreatedOn1 != null ? CreatedOn1.ToString("dd-MMM-yyyy hh:mm") : "";
            emailModel.TimeZoneOffset = emailModel.UserTimeZone != null ? emailModel.UserTimeZone.OffsetShort : "";
            emailModel.ImageHeader = "FrayteLogo";

            string logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";
            List<string> ImagePath = new List<string>();
            ImagePath.Add(logoImage);

            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/Tradelane/E2.1.cshtml");
            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, emailModel, viewBag, null);

            var EmailSubject = "";
            if (!string.IsNullOrEmpty(emailModel.CompanyName) && !string.IsNullOrEmpty(emailModel.CustomerName))
            {
                EmailSubject = emailModel.ShipmentDetail.FrayteNumber + " (" + emailModel.ShipmentDetail.DestinationAirport.AirportCode + ")" + " - " + emailModel.CompanyName + " - Booking Confirmation";
            }
            else
            {
                EmailSubject = emailModel.ShipmentDetail.FrayteNumber + " (" + emailModel.ShipmentDetail.DestinationAirport.AirportCode + ")" + " - Booking Confirmation";
            }

            string Attachmentpath = "";

            if (!string.IsNullOrEmpty(AppSettings.TOCC))
            {
                FrayteEmail.SendPreAlertMail(AppSettings.TOCC, "", "", "", EmailSubject, EmailBody, Attachmentpath, ImagePath, "Confirmation");
            }
            else
            {
                FrayteEmail.SendPreAlertMail(emailModel.TO, emailModel.CC, emailModel.BCC, emailModel.ReplyTo, EmailSubject, EmailBody, Attachmentpath, ImagePath, "Confirmation");
            }
        }

        public void FillModel_E3(TradelaneEmailModel model)
        {
            var detail = (from r in dbContext.Users
                          join ur in dbContext.UserRoles on r.UserId equals ur.UserId
                          where r.UserId == model.ShipmentDetail.CreatedBy
                          select new
                          {
                              Name = r.ContactName,
                              Compnay = r.CompanyName,
                              Email = r.Email,
                              UserEmail = r.UserEmail,
                              RoleId = ur.RoleId
                          }).FirstOrDefault();

            var customerDetail = (from u in dbContext.Users

                                  join ua in dbContext.UserAdditionals on u.UserId equals ua.UserId
                                  join ua1 in dbContext.UserAdditionals on ua.OperationUserId equals ua1.UserId
                                  join u1 in dbContext.Users on ua1.UserId equals u1.UserId
                                  join cum in dbContext.Users on ua.ManagerUserId equals cum.UserId into leftjoinCManagerTemp
                                  from CustomerManager in leftjoinCManagerTemp.DefaultIfEmpty()
                                  join uadd1 in dbContext.UserAddresses on u1.UserId equals uadd1.UserId
                                  join c1 in dbContext.Countries on uadd1.CountryId equals c1.CountryId
                                  join uma in dbContext.Users on ua1.ManagerUserId equals uma.UserId into leftjoininUManagerTemp
                                  from UserManager in leftjoininUManagerTemp.DefaultIfEmpty()
                                  where u.UserId == model.ShipmentDetail.CustomerId
                                  select new
                                  {
                                      StaffName = u1.ContactName,
                                      StaffEmail = u1.Email,
                                      StaffPhone = !string.IsNullOrEmpty(c1.CountryPhoneCode) ? "(" + c1.CountryPhoneCode + ") " : "" + u1.TelephoneNo,
                                      StaffPosition = u1.Position,
                                      StaffCompany = u1.CompanyName,
                                      CustomerName = u.ContactName,
                                      CustomerEmail = u.UserEmail,
                                      CompanyName = u.CompanyName,
                                      StaffManagerEmail = UserManager == null ? "" : UserManager.Email,
                                      StaffManagerName = UserManager == null ? "" : UserManager.ContactName,
                                      CustomerLineMangerEmail = CustomerManager == null ? "" : CustomerManager.Email,
                                      CustomerLineManagerName = CustomerManager == null ? "" : CustomerManager.ContactName
                                  }).FirstOrDefault();

            if (detail.RoleId == (int)FrayteUserRole.Staff)
            {
                model.UserTimeZone = (from uu in dbContext.Users
                                      join tz in dbContext.Timezones on uu.TimezoneId equals tz.TimezoneId
                                      where uu.UserId == model.ShipmentDetail.CreatedBy
                                      select new TimeZoneModal
                                      {
                                          Name = tz.Name,
                                          Offset = tz.Offset,
                                          OffsetShort = tz.OffsetShort,
                                          TimezoneId = tz.TimezoneId
                                      }
                          ).FirstOrDefault();

                model.TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(model.UserTimeZone.Name);
                model.CustomerName = detail.Name;
            }
            else
            {
                model.UserTimeZone = (from uu in dbContext.Users
                                      join ua in dbContext.UserAdditionals on uu.UserId equals ua.UserId
                                      join u1 in dbContext.Users on ua.OperationUserId equals u1.UserId
                                      join tz in dbContext.Timezones on u1.TimezoneId equals tz.TimezoneId
                                      where uu.UserId == model.ShipmentDetail.CustomerId
                                      select new TimeZoneModal
                                      {
                                          Name = tz.Name,
                                          Offset = tz.Offset,
                                          OffsetShort = tz.OffsetShort,
                                          TimezoneId = tz.TimezoneId
                                      }).FirstOrDefault();

                model.TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(model.UserTimeZone.Name);
                model.CustomerName = customerDetail.StaffName;
            }

            var packages = dbContext.TradelaneShipmentDetails.Where(p => p.TradelaneShipmentId == model.ShipmentDetail.TradelaneShipmentId && !string.IsNullOrEmpty(p.HAWB)).ToList();
            model.TotalCarton = model.ShipmentDetail.HAWBPackages.Sum(p => p.TotalCartons);
            model.TotalWeight = model.ShipmentDetail.HAWBPackages.Sum(p => p.TotalWeight);
            model.WeightUnit = model.ShipmentDetail.PakageCalculatonType == FraytePakageCalculationType.kgtoCms ? "kgs" : "lbs";
            model.DimensionUnit = model.ShipmentDetail.PakageCalculatonType == FraytePakageCalculationType.kgtoCms ? "cms" : "inchs";
            model.UserName = customerDetail.StaffName;
            model.UserEmail = customerDetail.StaffEmail;
            model.UserPhone = customerDetail.StaffPhone;
            model.UserPosition = customerDetail.StaffPosition;
            model.SiteAddress = AppSettings.TrackingUrl;

            //To CC BCC 
            model.TO = (detail.RoleId == (int)FrayteUserRole.Staff ? detail.Email : "") + ";" + customerDetail.StaffEmail;

            model.CC = (!string.IsNullOrEmpty(customerDetail.StaffManagerEmail) ? customerDetail.StaffManagerEmail : "");
            model.BCC = string.Empty;
            model.ReplyTo = string.Empty;
        }

        public void SendEmail_E2_3(TradelaneEmailModel emailModel)
        {
            //Get Customer Name and Customer User Detail
            var customerDetail = (from u in dbContext.Users

                                  join ua in dbContext.UserAdditionals on u.UserId equals ua.UserId
                                  join ua1 in dbContext.UserAdditionals on ua.OperationUserId equals ua1.UserId
                                  join u1 in dbContext.Users on ua1.UserId equals u1.UserId
                                  join cum in dbContext.Users on ua.ManagerUserId equals cum.UserId into leftjoinCManagerTemp
                                  from CustomerManager in leftjoinCManagerTemp.DefaultIfEmpty()
                                  join uma in dbContext.Users on ua1.ManagerUserId equals uma.UserId into leftjoininUManagerTemp
                                  from UserManager in leftjoininUManagerTemp.DefaultIfEmpty()
                                  where u.UserId == emailModel.ShipmentDetail.CustomerId
                                  select new
                                  {
                                      StaffName = u1.ContactName,
                                      StaffEmail = u1.Email,
                                      StaffCompany = u1.CompanyName,
                                      CustomerName = u.ContactName,
                                      CustomerEmail = u.UserEmail,
                                      CompanyName = u.CompanyName,
                                      StaffManagerEmail = UserManager == null ? "" : UserManager.Email,
                                      StaffManagerName = UserManager == null ? "" : UserManager.ContactName,
                                      CustomerLineMangerEmail = CustomerManager == null ? "" : CustomerManager.Email,
                                      CustomerLineManagerName = CustomerManager == null ? "" : CustomerManager.ContactName,
                                      TimeZoneId = u.TimezoneId
                                  }).FirstOrDefault();

            var shipmentDetails = (from u in dbContext.Users
                                   where u.UserId == emailModel.ShipmentDetail.CreatedBy
                                   select new
                                   {
                                       CreatedByName = u.ContactName,
                                       CreatedByEmail = u.UserEmail,
                                       CreatedByCompany = u.CompanyName,

                                   }).FirstOrDefault();

            emailModel.SiteAddress = AppSettings.TrackingUrl;

            emailModel.CustomerName = customerDetail.CompanyName;
            emailModel.CompanyName = customerDetail.CompanyName;
            emailModel.Name = customerDetail.CustomerName;
            emailModel.TO = customerDetail.CustomerEmail;
            emailModel.CC = "";
            emailModel.BCC = string.Empty;
            emailModel.ReplyTo = string.Empty;
            var operationzone = UtilityRepository.GetOperationZone();
            if (operationzone != null && operationzone.OperationZoneId == 1)
            {
                emailModel.Site = "www.FRAYTE.com";
            }
            else
            {
                emailModel.Site = "www.FRAYTE.co.uk";
            }
            DynamicViewBag viewBag = new DynamicViewBag();

            var timeZone = dbContext.Timezones.Where(p => p.TimezoneId == customerDetail.TimeZoneId).FirstOrDefault();
            var Timezone = new TimeZoneModal();
            if (timeZone != null)
            {

                Timezone.TimezoneId = timeZone.TimezoneId;
                Timezone.Name = timeZone.Name;
                Timezone.Offset = timeZone.Offset;
                Timezone.OffsetShort = timeZone.OffsetShort;
            }

            var TimeZoneDetail = TimeZoneInfo.FindSystemTimeZoneById(timeZone.Name);
            DateTime createdon = dbContext.TradelaneShipments.Where(a => a.TradelaneShipmentId == emailModel.ShipmentDetail.TradelaneShipmentId).FirstOrDefault().CreatedOnUtc;

            var CreatedOn1 = UtilityRepository.UtcDateToOtherTimezone(createdon, createdon.TimeOfDay, TimeZoneDetail).Item1;
            emailModel.CreatedOnDate = CreatedOn1 != null ? CreatedOn1.ToString("dd-MMM-yyyy hh:mm") : "";

            viewBag.AddValue("CreatedOn", emailModel.CreatedOnDate);
            viewBag.AddValue("TimeZone", emailModel.UserTimeZone.OffsetShort);
            emailModel.ImageHeader = "FrayteLogo";
            string logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";
            List<string> ImagePath = new List<string>();
            ImagePath.Add(logoImage);

            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/Tradelane/E2.3.cshtml");
            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, emailModel, viewBag, null);
            var EmailSubject = "";
            if (!string.IsNullOrEmpty(emailModel.CompanyName) && !string.IsNullOrEmpty(emailModel.Name))
            {
                EmailSubject = emailModel.ShipmentDetail.FrayteNumber + " (" + emailModel.ShipmentDetail.DepartureAirport.AirportCode + " To " + emailModel.ShipmentDetail.DestinationAirport.AirportCode + ")" + " - " + emailModel.CompanyName + ": " + emailModel.Name + " - Shipment Acceptance";
            }
            else
            {
                EmailSubject = emailModel.ShipmentDetail.FrayteNumber + " (" + emailModel.ShipmentDetail.DepartureAirport.AirportCode + " To " + emailModel.ShipmentDetail.DestinationAirport.AirportCode + ")" + " - Shipment Acceptance";
            }

            string Attachmentpath = "";
            if (!string.IsNullOrEmpty(AppSettings.TOCC))
            {
                FrayteEmail.SendPreAlertMail(AppSettings.TOCC, "", "", "", EmailSubject, EmailBody, Attachmentpath, ImagePath, "Confirmation");
            }
            else
            {
                FrayteEmail.SendPreAlertMail(emailModel.TO, emailModel.CC, "", "", EmailSubject, EmailBody, Attachmentpath, ImagePath, "Confirmation");
            }
        }

        public void SendEmail_E2_2(TradelaneEmailModel emailModel)
        {
            //Get Customer Name and Customer User Detail
            var customerDetail = (from u in dbContext.Users

                                  join ua in dbContext.UserAdditionals on u.UserId equals ua.UserId
                                  join ua1 in dbContext.UserAdditionals on ua.OperationUserId equals ua1.UserId
                                  join u1 in dbContext.Users on ua1.UserId equals u1.UserId
                                  join cum in dbContext.Users on ua.ManagerUserId equals cum.UserId into leftjoinCManagerTemp
                                  from CustomerManager in leftjoinCManagerTemp.DefaultIfEmpty()
                                  join uma in dbContext.Users on ua1.ManagerUserId equals uma.UserId into leftjoininUManagerTemp
                                  from UserManager in leftjoininUManagerTemp.DefaultIfEmpty()
                                  where u.UserId == emailModel.ShipmentDetail.CustomerId
                                  select new
                                  {
                                      StaffName = u1.ContactName,
                                      StaffEmail = u1.Email,
                                      StaffCompany = u1.CompanyName,
                                      CustomerName = u.ContactName,
                                      CustomerEmail = u.UserEmail,
                                      CompanyName = u.CompanyName,
                                      StaffManagerEmail = UserManager == null ? "" : UserManager.Email,
                                      StaffManagerName = UserManager == null ? "" : UserManager.ContactName,
                                      CustomerLineMangerEmail = CustomerManager == null ? "" : CustomerManager.Email,
                                      CustomerLineManagerName = CustomerManager == null ? "" : CustomerManager.ContactName
                                  }).FirstOrDefault();


            var shipmentDetails = (from u in dbContext.Users
                                   where u.UserId == emailModel.ShipmentDetail.CreatedBy
                                   select new
                                   {
                                       CreatedByName = u.ContactName,
                                       CreatedByEmail = u.UserEmail,
                                       CreatedByCompany = u.CompanyName,

                                   }).FirstOrDefault();

            emailModel.SiteAddress = AppSettings.TrackingUrl;
            emailModel.CustomerName = shipmentDetails.CreatedByName;
            emailModel.CompanyName = customerDetail.CompanyName;
            emailModel.Name = customerDetail.CustomerName;
            emailModel.TO = shipmentDetails.CreatedByEmail + ";" + customerDetail.StaffEmail + (!string.IsNullOrEmpty(customerDetail.StaffManagerEmail) ? ";" + customerDetail.StaffManagerEmail : "");
            emailModel.CC = (!string.IsNullOrEmpty(customerDetail.StaffManagerEmail) ? ";" + customerDetail.StaffManagerEmail : "") + (!string.IsNullOrEmpty(customerDetail.CustomerLineMangerEmail) ? ";" + customerDetail.CustomerLineMangerEmail : "");
            emailModel.BCC = string.Empty;
            emailModel.ReplyTo = string.Empty;
            var operationzone = UtilityRepository.GetOperationZone();
            if (operationzone != null && operationzone.OperationZoneId == 1)
            {
                emailModel.Site = "www.FRAYTE.com";
            }
            else
            {
                emailModel.Site = "www.FRAYTE.co.uk";
            }
            DynamicViewBag viewBag = new DynamicViewBag();
            viewBag.AddValue("CreatedOn", DateTime.Now.ToString("dd-MMM-yyyy hh:mm"));

            viewBag.AddValue("TimeZone", "asas");
            emailModel.ImageHeader = "FrayteLogo";
            string logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";
            List<string> ImagePath = new List<string>();
            ImagePath.Add(logoImage);

            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/Tradelane/E2.2.cshtml");
            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, emailModel, viewBag, null);
            var EmailSubject = "";
            if (!string.IsNullOrEmpty(emailModel.CompanyName) && !string.IsNullOrEmpty(emailModel.Name))
            {
                EmailSubject = emailModel.ShipmentDetail.FrayteNumber + " (" + emailModel.ShipmentDetail.DepartureAirport.AirportCode + " To " + emailModel.ShipmentDetail.DestinationAirport.AirportCode + ")" + " - " + emailModel.CompanyName + ": " + emailModel.Name + " - Booking Rejection";
            }
            else
            {
                EmailSubject = emailModel.ShipmentDetail.FrayteNumber + " (" + emailModel.ShipmentDetail.DepartureAirport.AirportCode + " To " + emailModel.ShipmentDetail.DestinationAirport.AirportCode + ")" + " - Booking Rejection";
            }

            string Attachmentpath = "";

            if (!string.IsNullOrEmpty(AppSettings.TOCC))
            {
                FrayteEmail.SendPreAlertMail(AppSettings.TOCC, "", "", "", EmailSubject, EmailBody, Attachmentpath, ImagePath, "Confirmation");
            }
            else
            {
                FrayteEmail.SendPreAlertMail(emailModel.TO, emailModel.CC, "", "", EmailSubject, EmailBody, Attachmentpath, ImagePath, "Confirmation");
            }
        }

        public void SendEmail_E3(TradelaneEmailModel model)
        {
            var customerDetail = (from u in dbContext.Users

                                  join ua in dbContext.UserAdditionals on u.UserId equals ua.UserId
                                  join ua1 in dbContext.UserAdditionals on ua.OperationUserId equals ua1.UserId
                                  join u1 in dbContext.Users on ua1.UserId equals u1.UserId
                                  join cum in dbContext.Users on ua.ManagerUserId equals cum.UserId into leftjoinCManagerTemp
                                  from CustomerManager in leftjoinCManagerTemp.DefaultIfEmpty()
                                  join uma in dbContext.Users on ua1.ManagerUserId equals uma.UserId into leftjoininUManagerTemp
                                  from UserManager in leftjoininUManagerTemp.DefaultIfEmpty()
                                  where u.UserId == model.ShipmentDetail.CustomerId
                                  select new
                                  {
                                      StaffName = u1.ContactName,
                                      StaffEmail = u1.Email,
                                      StaffCompany = u1.CompanyName,
                                      CustomerName = u.ContactName,
                                      CustomerEmail = u.UserEmail,
                                      CompanyName = u.CompanyName,
                                      StaffManagerEmail = UserManager == null ? "" : UserManager.Email,
                                      StaffManagerName = UserManager == null ? "" : UserManager.ContactName,
                                      CustomerLineMangerEmail = CustomerManager == null ? "" : CustomerManager.Email,
                                      CustomerLineManagerName = CustomerManager == null ? "" : CustomerManager.ContactName,
                                      TimeZoneId = u.TimezoneId
                                  }).FirstOrDefault();

            var shipmentDetails = (from u in dbContext.Users
                                   join ur in dbContext.UserRoles on u.UserId equals ur.UserId
                                   where u.UserId == model.ShipmentDetail.CreatedBy
                                   select new
                                   {
                                       RoleId = ur.RoleId,
                                       CreatedByName = u.ContactName,
                                       CreatedByEmail = u.UserEmail,
                                       CreatedByCompany = u.CompanyName,
                                   }).FirstOrDefault();

            DynamicViewBag viewBag = new DynamicViewBag();
            var timeZone = dbContext.Timezones.Where(p => p.TimezoneId == customerDetail.TimeZoneId).FirstOrDefault();
            var Timezone = new TimeZoneModal();
            if (timeZone != null)
            {
                Timezone.TimezoneId = timeZone.TimezoneId;
                Timezone.Name = timeZone.Name;
                Timezone.Offset = timeZone.Offset;
                Timezone.OffsetShort = timeZone.OffsetShort;
            }

            var TimeZoneDetail = TimeZoneInfo.FindSystemTimeZoneById(timeZone.Name);
            DateTime createdon = dbContext.TradelaneShipments.Where(a => a.TradelaneShipmentId == model.ShipmentDetail.TradelaneShipmentId).FirstOrDefault().CreatedOnUtc;

            var CreatedOn1 = UtilityRepository.UtcDateToOtherTimezone(createdon, createdon.TimeOfDay, TimeZoneDetail).Item1;
            model.CreatedOnDate = CreatedOn1 != null ? CreatedOn1.ToString("dd-MMM-yyyy hh:mm") : "";
            model.TimeZoneOffset = model.UserTimeZone.OffsetShort;
            model.CompanyName = customerDetail.CompanyName != null ? customerDetail.CompanyName : "";
            model.Name = customerDetail.CustomerName != null ? customerDetail.CustomerName : "";
            viewBag.AddValue("CreatedOn", model.CreatedOnDate);
            viewBag.AddValue("TimeZone", model.UserTimeZone.OffsetShort);
            var OpeartionZone = UtilityRepository.GetOperationZone();
            if (OpeartionZone.OperationZoneId == 1)
            {
                model.Site = "www.FRAYTE.com";
                model.UserPhone = "(+852) 2148 4880";
            }
            else
            {
                model.Site = "www.FRAYTE.co.uk";
                model.UserPhone = "(+44) 01792 277295";
            }
            model.ImageHeader = "FrayteLogo";
            string logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";
            List<string> ImagePath = new List<string>();
            ImagePath.Add(logoImage);

            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/Tradelane/E3.cshtml");
            var templateService = new TemplateService();
            var EmailBody = templateService.Parse(template, model, viewBag, null);

            var EmailSubject = "";
            if (!string.IsNullOrEmpty(model.CompanyName) && !string.IsNullOrEmpty(model.Name))
            {
                EmailSubject = model.ShipmentDetail.FrayteNumber + " (" + model.ShipmentDetail.DepartureAirport.AirportCode + " To " + model.ShipmentDetail.DestinationAirport.AirportCode + ")" + " - " + model.CompanyName + ": " + model.Name + " - Shipment Agent Allocation";
            }
            else
            {
                EmailSubject = model.ShipmentDetail.FrayteNumber + " (" + model.ShipmentDetail.DepartureAirport.AirportCode + " To " + model.ShipmentDetail.DestinationAirport.AirportCode + ")" + " - Shipment Agent Allocation";
            }

            model.TO = customerDetail.StaffEmail;

            if (shipmentDetails.RoleId == (int)FrayteUserRole.Staff)
            {
                model.TO += ";" + shipmentDetails.CreatedByEmail;
            }

            string Attachmentpath = "";

            if (!string.IsNullOrEmpty(AppSettings.TOCC))
            {
                FrayteEmail.SendPreAlertMail(AppSettings.TOCC, "", "", "", EmailSubject, EmailBody, Attachmentpath, ImagePath, "Confirmation");
            }
            else
            {
                FrayteEmail.SendPreAlertMail(model.TO, model.CC, model.BCC, model.ReplyTo, EmailSubject, EmailBody, Attachmentpath, ImagePath, "Confirmation");
            }
        }

        #endregion

        #region Tradelane Emails

        public void FillBookingInformationEmailModel(TradelaneEmailModel model)
        {
            var customerDetail = (from u in dbContext.Users
                                  join ua in dbContext.UserAdditionals on u.UserId equals ua.UserId
                                  join ua1 in dbContext.UserAdditionals on ua.OperationUserId equals ua1.UserId
                                  join u1 in dbContext.Users on ua1.UserId equals u1.UserId
                                  join uadd1 in dbContext.UserAddresses on u1.UserId equals uadd1.UserId
                                  join c1 in dbContext.Countries on uadd1.CountryId equals c1.CountryId
                                  where u.UserId == model.ShipmentDetail.CustomerId
                                  select new
                                  {
                                      StaffPhone = (!string.IsNullOrEmpty(c1.CountryPhoneCode) ? "(+" + c1.CountryPhoneCode + ") " : "") + u1.TelephoneNo,
                                      StaffName = u1.ContactName,
                                      StaffPosition = u1.Position,
                                      StaffEmail = u1.Email,
                                      StaffCompany = u1.CompanyName,
                                      CustomerName = u.ContactName,
                                      CustomerEmail = u.UserEmail,
                                      CompanyName = u.CompanyName
                                  }).FirstOrDefault();

            model.UserTimeZone = (from uu in dbContext.Users
                                  join tz in dbContext.Timezones on uu.TimezoneId equals tz.TimezoneId
                                  where uu.UserId == model.ShipmentDetail.CreatedBy
                                  select new TimeZoneModal
                                  {
                                      Name = tz.Name,
                                      Offset = tz.Offset,
                                      OffsetShort = tz.OffsetShort,
                                      TimezoneId = tz.TimezoneId
                                  }
                                ).FirstOrDefault();

            model.TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(model.UserTimeZone.Name);

            var packages = dbContext.TradelaneShipmentDetails.Where(p => p.TradelaneShipmentId == model.ShipmentDetail.TradelaneShipmentId && !string.IsNullOrEmpty(p.HAWB)).ToList();
            model.TotalCarton = model.ShipmentDetail.HAWBPackages.Sum(p => p.TotalCartons);
            model.TotalWeight = model.ShipmentDetail.HAWBPackages.Sum(p => p.TotalWeight);
            model.WeightUnit = model.ShipmentDetail.PakageCalculatonType == FraytePakageCalculationType.kgtoCms ? "kgs" : "lbs";
            model.DimensionUnit = model.ShipmentDetail.PakageCalculatonType == FraytePakageCalculationType.kgtoCms ? "cms" : "inchs";
            model.UserName = customerDetail.StaffName;
            model.UserEmail = customerDetail.StaffEmail;
            model.UserPhone = customerDetail.StaffPhone;
            model.UserPosition = customerDetail.StaffPosition;
            model.SiteAddress = AppSettings.TrackingUrl;
            model.CompanyName = customerDetail.CompanyName;
            model.Name = customerDetail.CustomerName;

            //To CC BCC
            var detail = (from r in dbContext.Users
                          join ur in dbContext.UserRoles on r.UserId equals ur.UserId
                          where r.UserId == model.ShipmentDetail.CreatedBy
                          select new
                          {
                              Name = r.ContactName,
                              Compnay = r.CompanyName,
                              Email = r.Email,
                              UserEmail = r.UserEmail,
                              RoleId = ur.RoleId
                          }).FirstOrDefault();

            model.CustomerName = detail.Name;
            model.CustomerCompanyName = detail.Compnay;
            model.TO = (detail.RoleId == (int)FrayteUserRole.Customer ? detail.UserEmail : detail.Email) + ";" + customerDetail.StaffEmail + (model.ShipmentDetail.NotifyParty.IsMailSend ? ";" + model.ShipmentDetail.NotifyParty.Email : "");

            if (model.ShipmentDetail.ShipFrom.IsMailSend)
            {
                if (model.ShipmentDetail.ShipFrom.Email.Contains(','))
                {
                    string[] emails = model.ShipmentDetail.ShipFrom.Email.Split(',');
                    for (int x = 0; x < emails.Length; x++)
                    {
                        model.TO += ";" + emails[x].ToString();
                    }
                }
                else if (model.ShipmentDetail.ShipFrom.Email.Contains(';'))
                {
                    string[] emails = model.ShipmentDetail.ShipFrom.Email.Split(';');
                    for (int x = 0; x < emails.Length; x++)
                    {
                        model.TO += ";" + emails[x].ToString();
                    }
                }
                else
                {
                    model.TO += ";" + model.ShipmentDetail.ShipFrom.Email;
                }
            }

            if (model.ShipmentDetail.ShipTo.IsMailSend)
            {
                if (model.ShipmentDetail.ShipTo.Email.Contains(','))
                {
                    string[] emails = model.ShipmentDetail.ShipTo.Email.Split(',');
                    for (int x = 0; x < emails.Length; x++)
                    {
                        model.TO += ";" + emails[x].ToString();
                    }
                }
                else if (model.ShipmentDetail.ShipTo.Email.Contains(';'))
                {
                    string[] emails = model.ShipmentDetail.ShipTo.Email.Split(';');
                    for (int x = 0; x < emails.Length; x++)
                    {
                        model.TO += ";" + emails[x].ToString();
                    }
                }
                else
                {
                    model.TO += ";" + model.ShipmentDetail.ShipTo.Email;
                }
            }

            model.CC = "";
            model.BCC = "";
            model.ReplyTo = "";
        }

        public void SendEmail_E1(TradelaneEmailModel model)
        {
            try
            {
                var operationzone = UtilityRepository.GetOperationZone();
                if (operationzone != null && operationzone.OperationZoneId == 1)
                {
                    model.Site = "www.FRAYTE.com";
                }
                else
                {
                    model.Site = "www.FRAYTE.co.uk";
                }
                DynamicViewBag viewBag = new DynamicViewBag();
                DateTime craetedon = UtilityRepository.UtcDateToOtherTimezone(model.ShipmentDetail.CreatedOnUtc, model.ShipmentDetail.CreatedOnUtc.TimeOfDay, model.TimeZoneInfo).Item1;
                DateTime createdon = dbContext.TradelaneShipments.Where(a => a.TradelaneShipmentId == model.ShipmentDetail.TradelaneShipmentId).FirstOrDefault().CreatedOnUtc;
                var CreatedOn1 = UtilityRepository.UtcDateToOtherTimezone(createdon, createdon.TimeOfDay, model.TimeZoneInfo).Item1;
                model.CreatedOnDate = CreatedOn1 != null ? CreatedOn1.ToString("dd-MMM-yyyy hh:mm") : "";
                model.ImageHeader = "FrayteLogo";
                viewBag.AddValue("CreatedOn", model.CreatedOnDate);
                viewBag.AddValue("TimeZone", model.UserTimeZone.OffsetShort);
                string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/Tradelane/E1.cshtml");
                var templateService = new TemplateService();

                var EmailBody = templateService.Parse(template, model, viewBag, null);

                string EmailSubject = string.Empty;
                if (!string.IsNullOrEmpty(model.CompanyName) && !string.IsNullOrEmpty(model.Name))
                {
                    EmailSubject = model.ShipmentDetail.FrayteNumber + " (" + model.ShipmentDetail.DepartureAirport.AirportCode + " To " + model.ShipmentDetail.DestinationAirport.AirportCode + ")" + " - " + model.CompanyName + ": " + model.Name + " - Booking Notification";
                }
                else
                {
                    EmailSubject = model.ShipmentDetail.FrayteNumber + " (" + model.ShipmentDetail.DepartureAirport.AirportCode + " To " + model.ShipmentDetail.DestinationAirport.AirportCode + ")" + " - Booking Notification";
                }

                var To = model.TO;

                var CC = model.CC;

                string Status = "Confirmation";

                #region Attach Labels

                //string Attachment = UtilityRepository.PackageLabelPath(DirectShipmentid, shipmentDetail.CustomerRateCard.CourierName, shipmentDetail.CustomerRateCard.RateType);

                #endregion

                //Send mail to Customer
                SendMail_New(To, CC, "FRAYTE - Booking (" + UtilityRepository.OperationZoneName(operationzone.OperationZoneId) + ")", EmailSubject, EmailBody, "", Status);
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
            }
        }

        public void FillBookingConfirmationEmailModel(TradelaneEmailModel model)
        {
            var customerDetail = (from u in dbContext.Users
                                  join ua in dbContext.UserAdditionals on u.UserId equals ua.UserId
                                  join ua1 in dbContext.UserAdditionals on ua.OperationUserId equals ua1.UserId
                                  join u1 in dbContext.Users on ua1.UserId equals u1.UserId
                                  join uadd1 in dbContext.UserAddresses on u1.UserId equals uadd1.UserId
                                  join c1 in dbContext.Countries on uadd1.CountryId equals c1.CountryId
                                  where u.UserId == model.ShipmentDetail.CustomerId
                                  select new
                                  {
                                      StaffPhone = !string.IsNullOrEmpty(c1.CountryPhoneCode) ? "(" + c1.CountryPhoneCode + ") " : "" + u1.TelephoneNo,
                                      StaffName = u1.ContactName,
                                      StaffPosition = u1.Position,
                                      StaffEmail = u1.Email,
                                      StaffCompany = u1.CompanyName,
                                      CustomerName = u.ContactName,
                                      CustomerEmail = u.UserEmail,
                                      CompanyName = u.CompanyName
                                  }).FirstOrDefault();

            model.UserTimeZone = (from uu in dbContext.Users
                                  join tz in dbContext.Timezones on uu.TimezoneId equals tz.TimezoneId
                                  where uu.UserId == model.ShipmentDetail.CustomerId
                                  select new TimeZoneModal
                                  {
                                      Name = tz.Name,
                                      Offset = tz.Offset,
                                      OffsetShort = tz.OffsetShort,
                                      TimezoneId = tz.TimezoneId
                                  }).FirstOrDefault();

            model.TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(model.UserTimeZone.Name);

            var detail = (from r in dbContext.Users
                          join ur in dbContext.UserRoles on r.UserId equals ur.UserId
                          where r.UserId == model.ShipmentDetail.CreatedBy
                          select new
                          {
                              Name = r.ContactName,
                              Company = r.CompanyName,
                              Email = r.Email,
                              UserEmail = r.UserEmail,
                              RoleId = ur.RoleId
                          }).FirstOrDefault();

            model.CustomerName = detail.Company;
            model.CustomerCompanyName = detail.Company;
            model.Name = customerDetail.CustomerName;
            model.TO = customerDetail.CustomerEmail;

            model.CC = "";
            model.BCC = "";
            model.ReplyTo = "";
        }

        public void SendEmail_E2(TradelaneEmailModel model)
        {
            try
            {
                var operationzone = UtilityRepository.GetOperationZone();

                if (operationzone != null && operationzone.OperationZoneId == 1)
                {
                    model.Site = "www.FRAYTE.com";
                }
                else
                {
                    model.Site = "www.FRAYTE.co.uk";
                }

                DynamicViewBag viewBag = new DynamicViewBag();

                #region  Link Action

                var ship = dbContext.TradelaneShipments.Find(model.ShipmentDetail.TradelaneShipmentId);
                var confirmLink = AppSettings.TrackingUrl + "/action/c/" + ship.CustomerConfirmationCode.ToString();
                var rejectLinkLink = AppSettings.TrackingUrl + "/action/r/" + ship.CustomerConfirmationCode.ToString();

                #endregion

                viewBag.AddValue("ConfirmLink", confirmLink);
                viewBag.AddValue("RejectLink", rejectLinkLink);

                //Get Customer Name and Customer User Detail
                var customerDetail = (from u in dbContext.Users
                                      join ua in dbContext.UserAdditionals on u.UserId equals ua.UserId
                                      join ua1 in dbContext.UserAdditionals on ua.OperationUserId equals ua1.UserId
                                      join u1 in dbContext.Users on ua1.UserId equals u1.UserId
                                      join cum in dbContext.Users on ua.ManagerUserId equals cum.UserId into leftjoinCManagerTemp
                                      from CustomerManager in leftjoinCManagerTemp.DefaultIfEmpty()
                                      join uma in dbContext.Users on ua1.ManagerUserId equals uma.UserId into leftjoininUManagerTemp
                                      from UserManager in leftjoininUManagerTemp.DefaultIfEmpty()
                                      where u.UserId == model.ShipmentDetail.CustomerId
                                      select new
                                      {
                                          StaffName = u1.ContactName,
                                          StaffEmail = u1.Email,
                                          StaffCompany = u1.CompanyName,
                                          CustomerName = u.ContactName,
                                          CustomerEmail = u.UserEmail,
                                          CompanyName = u.CompanyName,
                                          StaffManagerEmail = UserManager == null ? "" : UserManager.Email,
                                          StaffManagerName = UserManager == null ? "" : UserManager.ContactName,
                                          CustomerLineMangerEmail = CustomerManager == null ? "" : CustomerManager.Email,
                                          CustomerLineManagerName = CustomerManager == null ? "" : CustomerManager.ContactName,
                                          TimeZoneId = u.TimezoneId
                                      }).FirstOrDefault();

                var timeZone = dbContext.Timezones.Where(p => p.TimezoneId == customerDetail.TimeZoneId).FirstOrDefault();
                var Timezone = new TimeZoneModal();
                if (timeZone != null)
                {
                    Timezone.TimezoneId = timeZone.TimezoneId;
                    Timezone.Name = timeZone.Name;
                    Timezone.Offset = timeZone.Offset;
                    Timezone.OffsetShort = timeZone.OffsetShort;
                }

                var TimeZoneDetail = TimeZoneInfo.FindSystemTimeZoneById(timeZone.Name);
                DateTime createdon = dbContext.TradelaneShipments.Where(a => a.TradelaneShipmentId == model.ShipmentDetail.TradelaneShipmentId).FirstOrDefault().CreatedOnUtc;
                var CreatedOn1 = UtilityRepository.UtcDateToOtherTimezone(createdon, createdon.TimeOfDay, TimeZoneDetail).Item1;
                model.CreatedOnDate = CreatedOn1 != null ? CreatedOn1.ToString("dd-MMM-yyyy hh:mm") : "";
                model.ImageHeader = "FrayteLogo";
                model.TimeZoneOffset = model.UserTimeZone != null ? model.UserTimeZone.OffsetShort : "";
                viewBag.AddValue("CreatedOn", model.CreatedOnDate);
                viewBag.AddValue("TimeZone", model.UserTimeZone.OffsetShort);

                string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/Tradelane/E2.cshtml");

                var templateService = new TemplateService();
                var EmailBody = templateService.Parse(template, model, viewBag, null);
                string EmailSubject = string.Empty;

                if (!string.IsNullOrEmpty(customerDetail.CompanyName) && !string.IsNullOrEmpty(customerDetail.CustomerName))
                {
                    EmailSubject = model.ShipmentDetail.FrayteNumber + " (" + model.ShipmentDetail.DepartureAirport.AirportCode + " To " + model.ShipmentDetail.DestinationAirport.AirportCode + ")" + " - " + customerDetail.CompanyName + ": " + customerDetail.CustomerName + " - Booking Summary";
                }
                else
                {
                    EmailSubject = model.ShipmentDetail.FrayteNumber + " (" + model.ShipmentDetail.DepartureAirport.AirportCode + " To " + model.ShipmentDetail.DestinationAirport.AirportCode + ")" + " - Booking Summary";
                }
                var To = model.TO;
                var CC = model.CC;
                string Status = "Confirmation";

                #region Attach Labels

                //string Attachment = UtilityRepository.PackageLabelPath(DirectShipmentid, shipmentDetail.CustomerRateCard.CourierName, shipmentDetail.CustomerRateCard.RateType);

                #endregion

                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Customer Confirmation Email " + model.ShipmentDetail.FrayteNumber));
                //Send mail to Customer
                SendMail_New(To, CC, "FRAYTE - Booking (" + UtilityRepository.OperationZoneName(operationzone.OperationZoneId) + ")", EmailSubject, EmailBody, "", Status);
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(EmailBody));
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
            }
        }

        public string GetUpadateTrackingEmail(TradelaneBooking TB, TradelaneOperationslTrackingModel TOTM, string CustomerEmail)
        {
            //Tracking configuration emails of the customer for which we are sending mail
            var Email = "";
            var Res = dbContext.TrackingMileStones.Where(a => a.MileStoneKey == TOTM.TrackingCode && a.ShipmentHandlerMethodId == TB.ShipmentHandlerMethod.ShipmentHandlerMethodId).FirstOrDefault();
            if (Res != null)
            {
                var Result = dbContext.TradelaneUserTrackingConfigurations.Where(a => a.UserId == TB.CustomerId && a.TrackingMileStoneId == Res.TrackingMileStoneId).FirstOrDefault();
                if (Result != null && Result.IsEmailSend)
                {
                    var ResultNew = dbContext.TradelaneUserTrackingConfigurationDetails.Where(a => a.TradelaneUserTrackingConfigurationId == Result.TradelaneUserTrackingConfigurationId).ToList();
                    var count = 1;
                    foreach (var RN in ResultNew)
                    {
                        if (count == ResultNew.Count)
                        {
                            Email = Email + RN.Email;
                        }
                        else
                        {
                            Email = Email + RN.Email + ";";
                        }
                    }
                }
            }

            //if Email is empty send mail to customer
            if (string.IsNullOrEmpty(Email))
            {
                Email = CustomerEmail;
            }

            var Result1 = dbContext.TradelaneShipments.Where(a => a.TradelaneShipmentId == TB.TradelaneShipmentId).FirstOrDefault();
            if (Result1 != null)
            {
                if (Result1.IsNotifyPartySameAsReceiver)
                {
                    var Res2 = dbContext.TradelaneShipmentAddresses.Where(a => a.TradelaneShipmentAddressId == Result1.ToAddressId).FirstOrDefault();
                    if (Res2 != null && !string.IsNullOrEmpty(Res2.Email) && !string.IsNullOrEmpty(Email))
                    {
                        Email = Email + ';' + Res2.Email;
                    }
                    else if (Res2 != null && !string.IsNullOrEmpty(Res2.Email) && string.IsNullOrEmpty(Email))
                    {
                        Email = Res2.Email;
                    }
                }
                else
                {
                    var Res1 = dbContext.TradelaneShipmentAddresses.Where(a => a.TradelaneShipmentAddressId == Result1.NotifyPartyAddressId).FirstOrDefault();
                    if (Res1 != null && !string.IsNullOrEmpty(Res1.Email))
                    {
                        Email = Email + ';' + Res1.Email;
                    }
                }
            }

            return Email;
        }

        public FrayteResult SendUpdateTrackingEmail(TradelaneOperationslTrackingModel TM)
        {
            FrayteResult FR = new FrayteResult();
            string logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";
            var ImagePath = new List<string>();
            ImagePath.Add(logoImage);
            var Res = TradelaneEmailObj(TM.TradelaneShipmentId);
            Res.PackageCount = Res.ShipmentDetail.HAWBPackages != null && Res.ShipmentDetail.HAWBPackages.Count > 0 ? Res.ShipmentDetail.HAWBPackages.Sum(a => a.Packages.Count) : 0;
            Res.ShipmentDetail.TotalEstimatedWeight = Res.ShipmentDetail.HAWBPackages != null && Res.ShipmentDetail.HAWBPackages.Count > 0 ? Res.ShipmentDetail.HAWBPackages.Sum(a => a.EstimatedWeight) : 0;
            DynamicViewBag viewBag = new DynamicViewBag();
            viewBag.AddValue("PackageCount", Res.PackageCount);
            Res.PackageCount = Res.PackageCount;
            NewUserModel mailDetail = new NewUserModel();
            var user = dbContext.Users.Find(Res.ShipmentDetail.CustomerId);
            var OperationName = UtilityRepository.GetOperationZone();
            string Site = string.Empty;
            if (OperationName.OperationZoneId == 1)
            {
                Site = AppSettings.TrackingUrl;
            }
            else if (OperationName.OperationZoneId == 2)
            {
                Site = AppSettings.TrackingUrl;
            }

            Res.ImageHeader = "FrayteLogo";
            string OperationUserEmail = "";
            int OperationStaffId = 0;
            string ContactName = "";
            string UserPosition = "";
            int RoleId = 0;

            var result1 = (from Usr in dbContext.Users
                           join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId
                           join UsrAdd in dbContext.UserAdditionals on Usr.UserId equals UsrAdd.UserId
                           where Usr.UserId == Res.ShipmentDetail.CustomerId
                           select new
                           {
                               Usr.UserEmail,
                               Rl.RoleId,
                               Usr.TimezoneId,
                               UsrAdd.OperationUserId,
                               Usr.CompanyName,
                               Usr.ContactName
                           }).FirstOrDefault();

            if (result1 != null)
            {
                var result = (from Usr in dbContext.Users
                              join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId
                              where Usr.UserId == result1.OperationUserId
                              select new
                              {
                                  Usr.Email,
                                  Usr.ContactName,
                                  Rl.RoleId,
                                  Usr.TimezoneId,
                                  Usr.Position
                              }).FirstOrDefault();

                if (result != null)
                {
                    RoleId = result.RoleId;
                    OperationUserEmail = result.Email;
                    ContactName = result.ContactName;
                    UserPosition = result.Position;
                }
                mailDetail.RoleId = result.RoleId;

            }
            var email = GetUpadateTrackingEmail(Res.ShipmentDetail, TM, result1.UserEmail);
            var timeZone = dbContext.Timezones.Where(p => p.TimezoneId == result1.TimezoneId).FirstOrDefault();
            var Timezone = new TimeZoneModal();
            if (timeZone != null)
            {

                Timezone.TimezoneId = timeZone.TimezoneId;
                Timezone.Name = timeZone.Name;
                Timezone.Offset = timeZone.Offset;
                Timezone.OffsetShort = timeZone.OffsetShort;
            }
            var TimeZoneDetail = TimeZoneInfo.FindSystemTimeZoneById(timeZone.Name);
            DateTime createdon = dbContext.TradelaneShipments.Where(a => a.TradelaneShipmentId == Res.ShipmentDetail.TradelaneShipmentId).FirstOrDefault().CreatedOnUtc;
            var CreatedOn1 = UtilityRepository.UtcDateToOtherTimezone(createdon, createdon.TimeOfDay, TimeZoneDetail).Item1;
            Res.CreatedOnDate = CreatedOn1 != null ? CreatedOn1.ToString("dd-MMM-yyyy") : "";
            Res.MAWBDisplay = Res.ShipmentDetail.AirlinePreference.AilineCode + " " + Res.ShipmentDetail.MAWB.Substring(0, 4) + " " + Res.ShipmentDetail.MAWB.Substring(4, 4);
            Res.CreatedOnTime = CreatedOn1.ToString(@"hh\:mm");
            Res.ImageHeader = "FrayteLogo";
            Res.TimeZoneOffset = Timezone.OffsetShort;
            Res.ShipmentDescription = dbContext.TradelaneShipmentTrackings.Where(a => a.TradlaneShipmentId == TM.TradelaneShipmentId).OrderByDescending(a => a.TradelaneShipmentTrackingId).FirstOrDefault().TrackingDescription == null ? "" : dbContext.TradelaneShipmentTrackings.Where(a => a.TradlaneShipmentId == TM.TradelaneShipmentId).OrderByDescending(a => a.TradelaneShipmentTrackingId).FirstOrDefault().TrackingDescription;
            Res.ShipmentCurrentStatus = dbContext.ShipmentStatus.Where(a => a.ShipmentStatusId == Res.ShipmentDetail.ShipmentStatusId).FirstOrDefault().StatusName;
            if (OperationName.OperationZoneId == 1)
            {
                Res.UserEmail = OperationUserEmail;
                Res.UserName = result1.ContactName;
                Res.CustomerName = ContactName;
                Res.UserPosition = UserPosition;
                Res.UserPhone = "(+852) 2148 4880";
                Res.SiteAddress = AppSettings.TrackingUrl;
                Res.Site = "www.FRAYTE.com";
            }
            if (OperationName.OperationZoneId == 2)
            {
                Res.UserEmail = OperationUserEmail;
                Res.UserName = result1.ContactName;
                Res.CustomerName = ContactName;
                Res.UserPosition = UserPosition;
                Res.UserPhone = "(+44) 01792 277295";
                Res.SiteAddress = AppSettings.TrackingUrl;
                Res.Site = "www.FRAYTE.co.uk";
            }

            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/Tradelane/E6.cshtml");
            var EmailBody = Engine.Razor.RunCompile(template, "New", null, Res);
            var Mawbvar = "";
            var EmailSubject = "";
            if (!string.IsNullOrEmpty(Res.ShipmentDetail.MAWB))
            {
                Mawbvar = "MAWB# " + Res.ShipmentDetail.AirlinePreference.AilineCode + " " + Res.ShipmentDetail.MAWB.Substring(0, 4) + " " + Res.ShipmentDetail.MAWB.Substring(4, 4);
            }
            else
            {
                Mawbvar = Res.ShipmentDetail.FrayteNumber;
            }

            if (!string.IsNullOrEmpty(result1.CompanyName) && !string.IsNullOrEmpty(result1.ContactName))
            {
                EmailSubject = Mawbvar + " (" + Res.ShipmentDetail.DepartureAirport.AirportCode + " To " + Res.ShipmentDetail.DestinationAirport.AirportCode + ")" + " - " + result1.CompanyName + ": " + result1.ContactName + " - Tracking Status Updated";
            }
            else
            {
                EmailSubject = Mawbvar + " (" + Res.ShipmentDetail.DepartureAirport.AirportCode + " To " + Res.ShipmentDetail.DestinationAirport.AirportCode + ")" + " - Tracking Status Updated";
            }
            var Status = "Confirmation";
            var em = !string.IsNullOrEmpty(email) ? email : "";
            if (em != "")
            {
                FrayteEmail.SendPreAlertMail(em, "", "", "", EmailSubject, EmailBody, "", ImagePath, Status);
                FR.Status = true;
            }
            else
            {
                FR.Status = false;
            }
            return FR;
        }

        public FrayteResult ClaimShipmentEmail(TradelaneClaimShipmentModel TM)
        {
            FrayteResult FR = new FrayteResult();
            FR.Status = false;
            if (TM.ToEmail.Contains(','))
            {
                TM.ToEmail = TM.ToEmail.Replace(",", ";");
            }
            string logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";
            List<string> ImagePath = new List<string>();
            ImagePath.Add(logoImage);

            var Res = TradelaneEmailObj(TM.TradelaneShipmentId);
            var user = dbContext.Users.Find(Res.ShipmentDetail.CustomerId);
            var OperationName = UtilityRepository.GetOperationZone();
            string Site = string.Empty;
            Res.ImageHeader = "FrayteLogo";
            string OperationUserEmail = "";
            int RoleId = 0;
            var PackageCount = Res.ShipmentDetail.HAWBPackages != null && Res.ShipmentDetail.HAWBPackages.Count > 0 ? Res.ShipmentDetail.HAWBPackages.Sum(a => a.PackagesCount) : 0;
            Res.ShipmentDetail.TotalEstimatedWeight = Res.ShipmentDetail.HAWBPackages != null && Res.ShipmentDetail.HAWBPackages.Count > 0 ? Res.ShipmentDetail.HAWBPackages.Sum(a => a.EstimatedWeight) : 0;
            //var CreatedOnTime = 
            DynamicViewBag viewBag = new DynamicViewBag();
            viewBag.AddValue("Description", TM.Description);
            viewBag.AddValue("PackageCount", PackageCount);
            var StaffMail = "";
            var CreatedByMail = "";
            var CustomerMail = "";
            var AgentMail = "";
            var AgentName = "";
            var AgentNameLeg1 = "";
            var AgentNameLeg2 = "";
            var AgentMailLeg1 = "";
            var AgentMailLeg2 = "";

            if (Res.ShipmentDetail.ShipmentHandlerMethod.ShipmentHandlerMethodId == 5)
            {
                var Agentid = Res.ShipmentDetail.MAWBAgentId;
                var AgentDetail1 = (from Usr in dbContext.Users
                                    join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId
                                    where Usr.UserId == Agentid
                                    select new
                                    {
                                        Usr.Email,
                                        Rl.RoleId,
                                        Usr.ContactName
                                    }).FirstOrDefault();

                if (AgentDetail1 != null)
                {
                    AgentMail = AgentDetail1.Email;
                    AgentNameLeg1 = AgentDetail1.ContactName;
                }
                var Leg2Agent = dbContext.TradelaneShipmentAllocations.Where(a => a.TradelaneShipmentId == Res.ShipmentDetail.TradelaneShipmentId && a.LegNum == "Leg2").FirstOrDefault();
                if (Leg2Agent != null)
                {
                    var AgentDetail2 = (from Usr in dbContext.Users
                                        join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId
                                        where Usr.UserId == Leg2Agent.AgentId
                                        select new
                                        {
                                            Usr.Email,
                                            Rl.RoleId,
                                            Usr.ContactName
                                        }).FirstOrDefault();

                    if (AgentDetail2 != null)
                    {
                        AgentMailLeg2 = AgentDetail2.Email;
                        AgentNameLeg2 = AgentDetail2.ContactName;
                    }
                }
            }
            else
            {
                var AgentDetail = (from Usr in dbContext.Users
                                   join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId
                                   where Usr.UserId == Res.ShipmentDetail.MAWBAgentId
                                   select new
                                   {
                                       Usr.Email,
                                       Rl.RoleId,
                                       Usr.ContactName
                                   }).FirstOrDefault();

                if (AgentDetail != null)
                {
                    AgentMail = AgentDetail.Email;
                    AgentName = AgentDetail.ContactName;
                }
            }

            var CreatedBy = (from Usr in dbContext.Users
                             join usrAdd in dbContext.UserAdditionals on Usr.UserId equals usrAdd.UserId
                             join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId
                             where Usr.UserId == Res.ShipmentDetail.CreatedBy
                             select new
                             {
                                 Usr.Email,
                                 Rl.RoleId,
                                 usrAdd.OperationUserId,
                                 Usr.ContactName
                             }).FirstOrDefault();

            if (CreatedBy != null)
            {
                CreatedByMail = CreatedBy.Email;
            }
            var Customer = (from Usr in dbContext.Users
                            join usrAdd in dbContext.UserAdditionals on user.UserId equals usrAdd.UserId
                            join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId
                            where Usr.UserId == Res.ShipmentDetail.CustomerId
                            select new
                            {
                                Usr.UserEmail,
                                Rl.RoleId,
                                usrAdd.OperationUserId,
                                Usr.ContactName,
                                Usr.TimezoneId
                            }).FirstOrDefault();

            if (Customer != null)
            {

                CustomerMail = Customer.UserEmail;
            }

            var staffDetail = (from Usr in dbContext.Users
                               join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId
                               where Usr.UserId == Customer.OperationUserId
                               select new
                               {
                                   Usr.Email,
                                   Rl.RoleId,
                                   Usr.ContactName,
                                   Usr.Position
                               }).FirstOrDefault();

            if (staffDetail != null)
            {
                StaffMail = staffDetail.Email;
            }

            var timeZone = dbContext.Timezones.Where(p => p.TimezoneId == Customer.TimezoneId).FirstOrDefault();
            var Timezone = new TimeZoneModal();
            if (timeZone != null)
            {
                Timezone.TimezoneId = timeZone.TimezoneId;
                Timezone.Name = timeZone.Name;
                Timezone.Offset = timeZone.Offset;
                Timezone.OffsetShort = timeZone.OffsetShort;
            }

            var TimeZoneDetail = TimeZoneInfo.FindSystemTimeZoneById(timeZone.Name);
            DateTime createdon = dbContext.TradelaneShipments.Where(a => a.TradelaneShipmentId == Res.ShipmentDetail.TradelaneShipmentId).FirstOrDefault().CreatedOnUtc;
            var CreatedOn1 = UtilityRepository.UtcDateToOtherTimezone(createdon, createdon.TimeOfDay, TimeZoneDetail).Item1;

            Res.CreatedOnDate = CreatedOn1 != null ? CreatedOn1.ToString("dd-MMM-yyyy") : "";
            Res.ImageHeader = "FrayteLogo";
            Res.CreatedOnTime = CreatedOn1.ToString(@"hh\:mm");
            Res.MAWBDisplay = Res.ShipmentDetail.AirlinePreference.AilineCode + " " + Res.ShipmentDetail.MAWB.Substring(0, 4) + " " + Res.ShipmentDetail.MAWB.Substring(4, 4);
            Res.TimeZoneOffset = Timezone.OffsetShort;

            if (OperationName.OperationZoneId == 1)
            {
                Res.UserEmail = staffDetail.Email;
                Res.UserName = staffDetail.ContactName;
                Res.UserPosition = staffDetail.Position;
                Res.UserPhone = "(+852) 2148 4880";
                Res.SiteAddress = AppSettings.TrackingUrl;
                Res.Site = "www.FRAYTE.com";
            }
            if (OperationName.OperationZoneId == 2)
            {
                Res.UserEmail = staffDetail.Email;
                Res.UserName = staffDetail.ContactName;
                Res.UserPosition = staffDetail.Position;
                Res.UserPhone = "(+44) 01792 277295";
                Res.SiteAddress = AppSettings.TrackingUrl;
                Res.Site = "www.FRAYTE.co.uk";
            }
            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/Tradelane/E7.cshtml");
            var templateService = new TemplateService();
            var EmailSubject = TM.Subject != null && TM.Subject != "" ? TM.Subject : "";
            var Status = "Confirmation";
            TM.CC = !string.IsNullOrEmpty(TM.CC) ? TM.CC : "";
            TM.BCC = !string.IsNullOrEmpty(TM.BCC) ? TM.BCC : "";
            if (!string.IsNullOrEmpty(CustomerMail))
            {
                Res.CustomerName = Customer.ContactName;
                var EmailBody = templateService.Parse(template, Res, viewBag, null);
                FrayteEmail.SendPreAlertMail(CustomerMail, TM.CC, TM.BCC, "", EmailSubject, EmailBody, "", ImagePath, Status);
            }
            if (!string.IsNullOrEmpty(StaffMail))
            {
                Res.CustomerName = staffDetail.ContactName;
                var EmailBody = templateService.Parse(template, Res, viewBag, null);
                FrayteEmail.SendPreAlertMail(StaffMail, TM.CC, TM.BCC, "", EmailSubject, EmailBody, "", ImagePath, Status);
            }
            if (!string.IsNullOrEmpty(CreatedByMail))
            {
                Res.CustomerName = CreatedBy.ContactName;
                var EmailBody = templateService.Parse(template, Res, viewBag, null);
                FrayteEmail.SendPreAlertMail(CreatedByMail, TM.CC, TM.BCC, "", EmailSubject, EmailBody, "", ImagePath, Status);
            }

            if (!string.IsNullOrEmpty(TM.ToEmail))
            {
                if (Res.ShipmentDetail.ShipmentHandlerMethod.ShipmentHandlerMethodId == 5)
                {
                    Res.CustomerName = AgentNameLeg1;
                }
                else
                {
                    Res.CustomerName = AgentName;
                }
                var EmailBody = templateService.Parse(template, Res, viewBag, null);
                FrayteEmail.SendPreAlertMail(TM.ToEmail, TM.CC, TM.BCC, "", EmailSubject, EmailBody, "", ImagePath, Status);
            }
            FR.Status = true;
            UpdateIsClaim(Res.ShipmentDetail.TradelaneShipmentId);
            SaveClaimLog(Res.ShipmentDetail);
            return FR;
        }

        public FrayteResult MawbUpdateMail(int TradelaneShipmentId)
        {
            FrayteResult FR = new FrayteResult();
            FR.Status = false;
            string logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";
            List<string> ImagePath = new List<string>();
            ImagePath.Add(logoImage);
            var Res = TradelaneEmailObj(TradelaneShipmentId);
            var OperationName = UtilityRepository.GetOperationZone();
            string Site = string.Empty;
            Res.ImageHeader = "FrayteLogo";
            string OperationUserEmail = "";
            int RoleId = 0;
            var PackageCount = Res.ShipmentDetail.HAWBPackages != null && Res.ShipmentDetail.HAWBPackages.Count > 0 ? Res.ShipmentDetail.HAWBPackages.Sum(a => a.PackagesCount) : 0;
            Res.ShipmentDetail.TotalEstimatedWeight = Res.ShipmentDetail.HAWBPackages != null && Res.ShipmentDetail.HAWBPackages.Count > 0 ? Res.ShipmentDetail.HAWBPackages.Sum(a => a.EstimatedWeight) : 0;
            //var CreatedOnTime = 
            DynamicViewBag viewBag = new DynamicViewBag();
            viewBag.AddValue("PackageCount", PackageCount);
            var Mail = "";
            var StaffMail = "";
            var CreatedByMail = "";

            var CreatedBy = (from Usr in dbContext.Users
                             join usrAdd in dbContext.UserAdditionals on Usr.UserId equals usrAdd.UserId
                             join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId
                             where Usr.UserId == Res.ShipmentDetail.CreatedBy
                             select new
                             {
                                 Usr.Email,
                                 Rl.RoleId,
                                 usrAdd.OperationUserId,
                                 Usr.ContactName
                             }).FirstOrDefault();

            if (CreatedBy != null)
            {
                Mail = CreatedBy.Email;
            }
            var Customer = (from Usr in dbContext.Users
                            join usrAdd in dbContext.UserAdditionals on Usr.UserId equals usrAdd.UserId
                            join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId
                            where Usr.UserId == Res.ShipmentDetail.CustomerId
                            select new
                            {
                                Usr.UserEmail,
                                Rl.RoleId,
                                usrAdd.OperationUserId,
                                Usr.ContactName,
                                Usr.TimezoneId,
                                Usr.CompanyName
                            }).FirstOrDefault();
            if (Customer != null)
            {
                //Mail = Mail + ';' + Customer.UserEmail;
            }
            var staffDetail = (from Usr in dbContext.Users
                               join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId
                               where Usr.UserId == Customer.OperationUserId
                               select new
                               {
                                   Usr.Email,
                                   Rl.RoleId,
                                   Usr.ContactName,
                                   Usr.Position
                               }).FirstOrDefault();
            if (staffDetail != null)
            {
                Mail = Mail + ';' + staffDetail.Email;
            }
            var timeZone = dbContext.Timezones.Where(p => p.TimezoneId == Customer.TimezoneId).FirstOrDefault();
            var Timezone = new TimeZoneModal();
            if (timeZone != null)
            {
                Timezone.TimezoneId = timeZone.TimezoneId;
                Timezone.Name = timeZone.Name;
                Timezone.Offset = timeZone.Offset;
                Timezone.OffsetShort = timeZone.OffsetShort;
            }
            var TimeZoneDetail = TimeZoneInfo.FindSystemTimeZoneById(timeZone.Name);
            DateTime createdon = dbContext.TradelaneShipments.Where(a => a.TradelaneShipmentId == Res.ShipmentDetail.TradelaneShipmentId).FirstOrDefault().CreatedOnUtc;

            var CreatedOn1 = UtilityRepository.UtcDateToOtherTimezone(createdon, createdon.TimeOfDay, TimeZoneDetail).Item1;
            Res.CreatedOnDate = CreatedOn1 != null ? CreatedOn1.ToString("dd-MMM-yyyy") : "";
            Res.CreatedOnTime = CreatedOn1.ToString(@"hh\:mm");
            Res.ImageHeader = "FrayteLogo";
            Res.MAWBDisplay = Res.ShipmentDetail.AirlinePreference.AilineCode + " " + Res.ShipmentDetail.MAWB.Substring(0, 4) + " " + Res.ShipmentDetail.MAWB.Substring(4, 4);
            Res.TimeZoneOffset = Timezone.OffsetShort;

            if (OperationName.OperationZoneId == 1)
            {
                Res.UserEmail = staffDetail.Email;
                Res.UserName = staffDetail.ContactName;
                Res.UserPosition = staffDetail.Position;
                Res.UserPhone = "(+852) 2148 4880";
                Res.SiteAddress = AppSettings.TrackingUrl;
                Res.Site = "www.FRAYTE.com";
            }
            if (OperationName.OperationZoneId == 2)
            {
                Res.UserEmail = staffDetail.Email;
                Res.UserName = staffDetail.ContactName;
                Res.UserPosition = staffDetail.Position;
                Res.UserPhone = "(+44) 01792 277295";
                Res.SiteAddress = AppSettings.TrackingUrl;
                Res.Site = "www.FRAYTE.co.uk";
            }
            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/Tradelane/E4.3.cshtml");
            var templateService = new TemplateService();
            var Mawbvar = "";
            var EmailSubject = "";
            var Status = "Confirmation";

            if (!string.IsNullOrEmpty(Res.ShipmentDetail.MAWB))
            {
                Mawbvar = "MAWB# " + Res.ShipmentDetail.AirlinePreference.AilineCode + " " + Res.ShipmentDetail.MAWB.Substring(0, 4) + " " + Res.ShipmentDetail.MAWB.Substring(4, 4);
            }
            else
            {
                Mawbvar = Res.ShipmentDetail.FrayteNumber;
            }

            if (!string.IsNullOrEmpty(Customer.CompanyName) && !string.IsNullOrEmpty(Customer.ContactName))
            {

                EmailSubject = Mawbvar + " (" + Res.ShipmentDetail.DepartureAirport.AirportCode + " To " + Res.ShipmentDetail.DestinationAirport.AirportCode + ")" + " - " + Customer.CompanyName + ": " + Customer.ContactName + " - MAWB Information Updated";
            }
            else
            {
                EmailSubject = Mawbvar + " (" + Res.ShipmentDetail.DepartureAirport.AirportCode + " To " + Res.ShipmentDetail.DestinationAirport.AirportCode + ")" + " - MAWB Information Updated";
            }

            if (!string.IsNullOrEmpty(Mail))
            {
                Res.CustomerName = !string.IsNullOrEmpty(staffDetail.ContactName) ? staffDetail.ContactName : "";
                var EmailBody = templateService.Parse(template, Res, viewBag, null);
                FrayteEmail.SendPreAlertMail(Mail, "", "", "", EmailSubject, EmailBody, "", ImagePath, Status);
            }
            FR.Status = true;
            return FR;
        }

        public FrayteResult SendResolvedClaimShipment(TradelaneClaimShipmentModel TM)
        {
            FrayteResult FR = new FrayteResult();
            FR.Status = false;
            if (TM.ToEmail.Contains(','))
            {
                TM.ToEmail = TM.ToEmail.Replace(",", ";");
            }
            string logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";
            List<string> ImagePath = new List<string>();
            ImagePath.Add(logoImage);
            var Res = TradelaneEmailObj(TM.TradelaneShipmentId);
            var user = dbContext.Users.Find(Res.ShipmentDetail.CustomerId);
            var OperationName = UtilityRepository.GetOperationZone();
            string Site = string.Empty;
            Res.ImageHeader = "FrayteLogo";
            string OperationUserEmail = "";
            int RoleId = 0;
            var PackageCount = Res.ShipmentDetail.HAWBPackages != null && Res.ShipmentDetail.HAWBPackages.Count > 0 ? Res.ShipmentDetail.HAWBPackages.Sum(a => a.PackagesCount) : 0;
            Res.ShipmentDetail.TotalEstimatedWeight = Res.ShipmentDetail.HAWBPackages != null && Res.ShipmentDetail.HAWBPackages.Count > 0 ? Res.ShipmentDetail.HAWBPackages.Sum(a => a.EstimatedWeight) : 0;

            DynamicViewBag viewBag = new DynamicViewBag();
            viewBag.AddValue("Description", TM.Description);
            viewBag.AddValue("PackageCount", PackageCount);
            var StaffMail = "";
            var AgentMail = "";
            var AgentName = "";
            var AgentNameLeg1 = "";
            var AgentNameLeg2 = "";
            var CreatedByMail = "";
            var AgentMailLeg1 = "";
            var AgentMailLeg2 = "";

            var CreatedBy = (from Usr in dbContext.Users
                             join usrAdd in dbContext.UserAdditionals on user.UserId equals usrAdd.UserId
                             join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId
                             where Usr.UserId == Res.ShipmentDetail.CustomerId
                             select new
                             {
                                 Usr.UserEmail,
                                 Rl.RoleId,
                                 usrAdd.OperationUserId,
                                 Usr.ContactName,
                                 Usr.TimezoneId
                             }).FirstOrDefault();

            if (CreatedBy != null)
            {
                CreatedByMail = CreatedBy.UserEmail;
            }

            var staffDetail = (from Usr in dbContext.Users
                               join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId
                               where Usr.UserId == CreatedBy.OperationUserId
                               select new
                               {
                                   Usr.Email,
                                   Rl.RoleId,
                                   Usr.ContactName
                               }).FirstOrDefault();

            if (staffDetail != null)
            {
                StaffMail = staffDetail.Email;
            }

            var timeZone = dbContext.Timezones.Where(p => p.TimezoneId == CreatedBy.TimezoneId).FirstOrDefault();
            var Timezone = new TimeZoneModal();
            if (timeZone != null)
            {
                Timezone.TimezoneId = timeZone.TimezoneId;
                Timezone.Name = timeZone.Name;
                Timezone.Offset = timeZone.Offset;
                Timezone.OffsetShort = timeZone.OffsetShort;
            }

            var TimeZoneDetail = TimeZoneInfo.FindSystemTimeZoneById(timeZone.Name);
            DateTime createdon = dbContext.TradelaneShipments.Where(a => a.TradelaneShipmentId == Res.ShipmentDetail.TradelaneShipmentId).FirstOrDefault().CreatedOnUtc;

            var CreatedOn1 = UtilityRepository.UtcDateToOtherTimezone(createdon, createdon.TimeOfDay, TimeZoneDetail).Item1;
            Res.CreatedOnDate = CreatedOn1 != null ? CreatedOn1.ToString("dd-MMM-yyyy") : "";
            Res.ImageHeader = "FrayteLogo";
            Res.CreatedOnTime = CreatedOn1.ToString(@"hh\:mm");
            Res.TimeZoneOffset = Timezone.OffsetShort;
            Res.MAWBDisplay = Res.ShipmentDetail.AirlinePreference.AilineCode + " " + Res.ShipmentDetail.MAWB.Substring(0, 4) + " " + Res.ShipmentDetail.MAWB.Substring(4, 4);

            if (Res.ShipmentDetail.ShipmentHandlerMethod.ShipmentHandlerMethodId == 5)
            {
                var Agentid = Res.ShipmentDetail.MAWBAgentId;
                var AgentDetail1 = (from Usr in dbContext.Users
                                    join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId
                                    where Usr.UserId == Agentid
                                    select new
                                    {
                                        Usr.Email,
                                        Rl.RoleId,
                                        Usr.ContactName
                                    }).FirstOrDefault();

                if (AgentDetail1 != null)
                {
                    AgentMail = AgentDetail1.Email;
                    AgentNameLeg1 = AgentDetail1.ContactName;
                }
                var Leg2Agent = dbContext.TradelaneShipmentAllocations.Where(a => a.TradelaneShipmentId == Res.ShipmentDetail.TradelaneShipmentId && a.LegNum == "Leg2").FirstOrDefault();
                if (Leg2Agent != null)
                {
                    var AgentDetail2 = (from Usr in dbContext.Users
                                        join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId
                                        where Usr.UserId == Leg2Agent.AgentId
                                        select new
                                        {
                                            Usr.Email,
                                            Rl.RoleId,
                                            Usr.ContactName
                                        }).FirstOrDefault();

                    if (AgentDetail2 != null)
                    {
                        AgentMailLeg2 = AgentDetail2.Email;
                        AgentNameLeg2 = AgentDetail2.ContactName;
                    }
                }
            }
            else
            {
                var AgentDetail = (from Usr in dbContext.Users
                                   join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId
                                   where Usr.UserId == Res.ShipmentDetail.MAWBAgentId
                                   select new
                                   {
                                       Usr.Email,
                                       Rl.RoleId,
                                       Usr.ContactName
                                   }).FirstOrDefault();

                if (AgentDetail != null)
                {
                    AgentMail = AgentDetail.Email;
                    AgentName = AgentDetail.ContactName;
                }
            }

            var result = (from Usr in dbContext.Users
                          join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId
                          join usrAdd in dbContext.UserAdditionals on Usr.UserId equals usrAdd.UserId
                          where Usr.UserId == Res.ShipmentDetail.CustomerId
                          select new
                          {
                              Usr.UserEmail,
                              Rl.RoleId,
                              Usr.ContactName,
                              usrAdd.OperationUserId
                          }).FirstOrDefault();

            if (result != null)
            {
                RoleId = result.RoleId;
                OperationUserEmail = result.UserEmail;
            }

            var staffDetail1 = (from Usr in dbContext.Users
                                join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId

                                where Usr.UserId == result.OperationUserId
                                select new
                                {
                                    Usr.Email,
                                    Rl.RoleId,
                                    Usr.ContactName,
                                    Usr.Position
                                }).FirstOrDefault();

            if (staffDetail1 != null)
            {
                StaffMail = staffDetail1.Email;
            }

            if (OperationName.OperationZoneId == 1)
            {
                Res.UserEmail = staffDetail1.Email;
                Res.UserName = staffDetail1.ContactName;
                Res.UserPosition = staffDetail1.Position;
                Res.UserPhone = "(+852) 2148 4880";
                Res.SiteAddress = AppSettings.TrackingUrl;
                Res.Site = "www.FRAYTE.com";
            }
            if (OperationName.OperationZoneId == 2)
            {
                Res.UserEmail = staffDetail1.Email;
                Res.UserName = staffDetail1.ContactName;
                Res.UserPosition = staffDetail1.Position;
                Res.UserPhone = "(+44) 01792 277295";
                Res.SiteAddress = AppSettings.TrackingUrl;
                Res.Site = "www.FRAYTE.co.uk";
            }
            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/Tradelane/E7.1.cshtml");
            var templateService = new TemplateService();
            var EmailSubject = TM.Subject != null && TM.Subject != "" ? TM.Subject : "";
            var Status = "Confirmation";
            TM.CC = !string.IsNullOrEmpty(TM.CC) ? TM.CC : "";
            TM.BCC = !string.IsNullOrEmpty(TM.BCC) ? TM.BCC : "";
            if (!string.IsNullOrEmpty(CreatedByMail))
            {
                Res.CustomerName = CreatedBy.ContactName;
                var EmailBody = templateService.Parse(template, Res, viewBag, null);
                FrayteEmail.SendPreAlertMail(CreatedByMail, TM.CC, TM.BCC, "", EmailSubject, EmailBody, "", ImagePath, Status);
            }

            if (!string.IsNullOrEmpty(StaffMail))
            {
                Res.CustomerName = staffDetail.ContactName;
                var EmailBody = templateService.Parse(template, Res, viewBag, null);
                FrayteEmail.SendPreAlertMail(StaffMail, TM.CC, TM.BCC, "", EmailSubject, EmailBody, "", ImagePath, Status);
            }

            if (!string.IsNullOrEmpty(TM.ToEmail))
            {
                Res.CustomerName = result.ContactName;
                var EmailBody = templateService.Parse(template, Res, viewBag, null);
                FrayteEmail.SendPreAlertMail(TM.ToEmail, TM.CC, TM.BCC, "", EmailSubject, EmailBody, "", ImagePath, Status);
            }
            FR.Status = true;
            return FR;
        }

        public FrayteResult SendMawbCorrectionShipment(TradelaneShipmentAllocation Model, TradelaneClaimShipmentModel TM, string FilePath)
        {
            FrayteResult FR = new FrayteResult();
            FR.Status = false;
            if (TM.ToEmail.Contains(','))
            {
                TM.ToEmail = TM.ToEmail.Replace(",", ";");
            }
            var Attachment = FilePath == null ? "" : FilePath;
            string logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";
            List<string> ImagePath = new List<string>();
            ImagePath.Add(logoImage);

            var Res = TradelaneEmailObj(TM.TradelaneShipmentId);

            string siteUrl = AppSettings.TrackingUrl + "/agent-action/";
            string key = string.Empty;

            if (Res.ShipmentDetail.ShipmentHandlerMethod.ShipmentHandlerMethodId != 5)
            {
                key = Res.ShipmentDetail.FrayteNumber + "-" + Res.ShipmentDetail.MAWBAgentId + "-" + Res.ShipmentDetail.ShipmentHandlerMethod.ShipmentHandlerMethodId;
            }
            else
            {
                key = Res.ShipmentDetail.FrayteNumber + "-" + Model.LegNum + "-" + Res.ShipmentDetail.MAWBAgentId + "-" + Res.ShipmentDetail.ShipmentHandlerMethod.ShipmentHandlerMethodId;
            }

            var df = UtilityRepository.Encrypt(key, EncriptionKey.PrivateKey);
            var basae = UtilityRepository.Base64UrlEncode(df);
            var en = WebUtility.UrlEncode(basae);
            siteUrl += en;

            DynamicViewBag viewBag = new DynamicViewBag();
            viewBag.AddValue("MAWBURL", siteUrl);

            var user = dbContext.Users.Find(Res.ShipmentDetail.CustomerId);
            var OperationName = UtilityRepository.GetOperationZone();
            string Site = string.Empty;

            Res.ImageHeader = "FrayteLogo";
            string OperationUserEmail = "";
            int RoleId = 0;
            var PackageCount = Res.ShipmentDetail.HAWBPackages != null && Res.ShipmentDetail.HAWBPackages.Count > 0 ? Res.ShipmentDetail.HAWBPackages.Sum(a => a.PackagesCount) : 0;
            Res.ShipmentDetail.TotalEstimatedWeight = Res.ShipmentDetail.HAWBPackages != null && Res.ShipmentDetail.HAWBPackages.Count > 0 ? Res.ShipmentDetail.HAWBPackages.Sum(a => a.EstimatedWeight) : 0;

            viewBag.AddValue("Description", TM.Description);
            viewBag.AddValue("PackageCount", PackageCount);
            var StaffMail = "";
            var AgentMail = "";
            var AgentName = "";
            var AgentNameLeg1 = "";
            var AgentNameLeg2 = "";
            var CreatedByMail = "";
            var AgentMailLeg1 = "";
            var AgentMailLeg2 = "";

            var CreatedBy = (from Usr in dbContext.Users
                             join usrAdd in dbContext.UserAdditionals on user.UserId equals usrAdd.UserId
                             join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId
                             where Usr.UserId == Res.ShipmentDetail.CustomerId
                             select new
                             {
                                 Usr.Email,
                                 Rl.RoleId,
                                 usrAdd.OperationUserId,
                                 Usr.UserName,
                                 Usr.TimezoneId
                             }).FirstOrDefault();

            if (CreatedBy != null)
            {
                CreatedByMail = CreatedBy.Email;
            }

            var staffDetail = (from Usr in dbContext.Users
                               join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId
                               where Usr.UserId == CreatedBy.OperationUserId
                               select new
                               {
                                   Usr.Email,
                                   Rl.RoleId,
                                   Usr.UserName
                               }).FirstOrDefault();

            if (staffDetail != null)
            {
                StaffMail = CreatedBy.Email;
            }

            var timeZone = dbContext.Timezones.Where(p => p.TimezoneId == CreatedBy.TimezoneId).FirstOrDefault();
            var Timezone = new TimeZoneModal();
            if (timeZone != null)
            {
                Timezone.TimezoneId = timeZone.TimezoneId;
                Timezone.Name = timeZone.Name;
                Timezone.Offset = timeZone.Offset;
                Timezone.OffsetShort = timeZone.OffsetShort;
            }

            var TimeZoneDetail = TimeZoneInfo.FindSystemTimeZoneById(timeZone.Name);
            DateTime createdon = dbContext.TradelaneShipments.Where(a => a.TradelaneShipmentId == Res.ShipmentDetail.TradelaneShipmentId).FirstOrDefault().CreatedOnUtc;

            var CreatedOn1 = UtilityRepository.UtcDateToOtherTimezone(createdon, createdon.TimeOfDay, TimeZoneDetail).Item1;
            Res.CreatedOnDate = CreatedOn1 != null ? CreatedOn1.ToString("dd-MMM-yyyy") : "";

            Res.CreatedOnTime = CreatedOn1.ToString(@"hh\:mm");
            Res.TimeZoneOffset = Timezone.OffsetShort;
            Res.MAWBDisplay = Res.ShipmentDetail.AirlinePreference.AilineCode + " " + Res.ShipmentDetail.MAWB.Substring(0, 4) + " " + Res.ShipmentDetail.MAWB.Substring(4, 4);

            if (Res.ShipmentDetail.ShipmentHandlerMethod.ShipmentHandlerMethodId == 5)
            {
                var Agentid = Res.ShipmentDetail.MAWBAgentId;
                var AgentDetail1 = (from Usr in dbContext.Users
                                    join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId
                                    where Usr.UserId == Agentid
                                    select new
                                    {
                                        Usr.Email,
                                        Rl.RoleId,
                                        Usr.ContactName
                                    }).FirstOrDefault();

                if (AgentDetail1 != null)
                {
                    AgentMail = AgentDetail1.Email;
                    AgentNameLeg1 = AgentDetail1.ContactName;
                }
                var Leg2Agent = dbContext.TradelaneShipmentAllocations.Where(a => a.TradelaneShipmentId == Res.ShipmentDetail.TradelaneShipmentId && a.LegNum == "Leg2").FirstOrDefault();
                if (Leg2Agent != null)
                {
                    var AgentDetail2 = (from Usr in dbContext.Users
                                        join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId
                                        where Usr.UserId == Leg2Agent.AgentId
                                        select new
                                        {
                                            Usr.UserEmail,
                                            Rl.RoleId,
                                            Usr.ContactName
                                        }).FirstOrDefault();

                    if (AgentDetail2 != null)
                    {
                        AgentMailLeg2 = AgentDetail2.UserEmail;
                        AgentNameLeg2 = AgentDetail2.ContactName;
                    }
                }
            }
            else
            {
                var AgentDetail = (from Usr in dbContext.Users
                                   join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId
                                   where Usr.UserId == Res.ShipmentDetail.MAWBAgentId
                                   select new
                                   {
                                       Usr.UserEmail,
                                       Rl.RoleId,
                                       Usr.ContactName
                                   }).FirstOrDefault();

                if (AgentDetail != null)
                {
                    AgentMail = AgentDetail.UserEmail;
                    AgentName = AgentDetail.ContactName;
                }
            }

            var result = (from Usr in dbContext.Users
                          join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId
                          join usrAdd in dbContext.UserAdditionals on Usr.UserId equals usrAdd.UserId
                          where Usr.UserId == Res.ShipmentDetail.CustomerId
                          select new
                          {
                              Usr.UserEmail,
                              Rl.RoleId,
                              Usr.ContactName,
                              usrAdd.OperationUserId
                          }).FirstOrDefault();

            if (result != null)
            {
                RoleId = result.RoleId;
                OperationUserEmail = result.UserEmail;
            }

            var staffDetail1 = (from Usr in dbContext.Users
                                join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId
                                where Usr.UserId == result.OperationUserId
                                select new
                                {
                                    Usr.Email,
                                    Rl.RoleId,
                                    Usr.ContactName,
                                    Usr.Position
                                }).FirstOrDefault();

            if (staffDetail != null)
            {
                StaffMail = CreatedBy.Email;
            }

            if (OperationName.OperationZoneId == 1)
            {
                Res.UserEmail = staffDetail1.Email;
                Res.UserName = staffDetail1.ContactName;
                Res.UserPosition = staffDetail1.Position;
                Res.UserPhone = "(+852) 2148 4880";
                Res.SiteAddress = AppSettings.TrackingUrl;
                Res.Site = "www.FRAYTE.com";
            }
            if (OperationName.OperationZoneId == 2)
            {
                Res.UserEmail = staffDetail1.Email;
                Res.UserPosition = staffDetail1.Position;
                Res.UserName = staffDetail1.ContactName;
                Res.UserPhone = "(+44) 01792 277295";
                Res.SiteAddress = AppSettings.TrackingUrl;
                Res.Site = "www.FRAYTE.co.uk";
            }
            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/Tradelane/E4.2.cshtml");
            var templateService = new TemplateService();
            var EmailSubject = TM.Subject != null && TM.Subject != "" ? TM.Subject : "";
            TM.CC = !string.IsNullOrEmpty(TM.CC) ? TM.CC : "";
            TM.BCC = !string.IsNullOrEmpty(TM.BCC) ? TM.BCC : "";
            var Status = "Confirmation";
            if (!string.IsNullOrEmpty(TM.ToEmail))
            {
                if (Res.ShipmentDetail.ShipmentHandlerMethod.ShipmentHandlerMethodId == 5)
                {
                    viewBag.AddValue("UserName", AgentNameLeg1);
                }
                else
                {
                    viewBag.AddValue("UserName", AgentName);
                }

                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Mawb correction email " + Res.ShipmentDetail.FrayteNumber));
                var EmailBody = templateService.Parse(template, Res, viewBag, null);
                FrayteEmail.SendPreAlertMail(TM.ToEmail, TM.CC, TM.BCC, "", EmailSubject, EmailBody, Attachment, ImagePath, Status);
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(EmailBody));
            }
            FR.Status = true;
            return FR;
        }

        public void SaveClaimLog(TradelaneBooking ShipmentDetail)
        {
            try
            {
                TradelaneLogging log = new TradelaneLogging();
                log.ShipmentId = ShipmentDetail.TradelaneShipmentId;
                log.SentBy = ShipmentDetail.CustomerId;
                log.SentOnUtc = DateTime.UtcNow;
                log.Type = TradelaneLoggingType.Claim;
                dbContext.TradelaneLoggings.Add(log);
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }

        public void UpdateIsClaim(int TradelaneShipmentId)
        {
            if (TradelaneShipmentId > 0)
            {
                var Res = dbContext.TradelaneShipments.Where(a => a.TradelaneShipmentId == TradelaneShipmentId).FirstOrDefault();
                if (Res != null)
                {
                    Res.IsClaim = true;
                    dbContext.Entry(Res).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                }
                else
                {
                    TradelaneShipment TS = new TradelaneShipment();
                    TS.IsClaim = true;
                    dbContext.TradelaneShipments.Add(TS);
                    dbContext.SaveChanges();
                }
            }
        }

        public void SendMailtoAgent(MawbAllocationModel Model, int UserId, int TradelaneId, string FilePath)
        {
            string logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";
            List<string> ImagePath = new List<string>();
            ImagePath.Add(logoImage);
            NewUserModel mailDetail = new NewUserModel();
            var user = dbContext.Users.Find(UserId);
            var OperationName = UtilityRepository.GetOperationZone();
            var Attchament = new MawbAllocationRepository().GetAttachment(TradelaneId);
            Attchament = Attchament + ';' + FilePath;
            var Res = TradelaneEmailObj(TradelaneId);

            string siteUrl = AppSettings.TrackingUrl + "/agent-action/";
            string key = string.Empty;

            if (Res.ShipmentDetail.ShipmentHandlerMethod.ShipmentHandlerMethodId != 5)
            {
                key = Res.ShipmentDetail.FrayteNumber + "-" + UserId + "-" + Res.ShipmentDetail.ShipmentHandlerMethod.ShipmentHandlerMethodId;
            }
            else
            {
                key = Res.ShipmentDetail.FrayteNumber + "-" + Model.LegNum + "-" + UserId + "-" + Res.ShipmentDetail.ShipmentHandlerMethod.ShipmentHandlerMethodId;
            }

            var df = UtilityRepository.Encrypt(key, EncriptionKey.PrivateKey);
            var basae = UtilityRepository.Base64UrlEncode(df);
            var en = WebUtility.UrlEncode(basae);
            siteUrl += en;

            DynamicViewBag viewBag = new DynamicViewBag();
            viewBag.AddValue("MAWBURL", siteUrl);

            string Site = string.Empty;
            if (OperationName.OperationZoneId == 1)
            {
                Res.SiteAddress = AppSettings.TrackingUrl;
            }
            else if (OperationName.OperationZoneId == 2)
            {
                Res.SiteAddress = AppSettings.TrackingUrl;
            }

            Res.ImageHeader = "FrayteLogo";
            string OperationUserEmail = "";
            int RoleId = 0;

            var result = (from Usr in dbContext.Users
                          join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId
                          join UA in dbContext.UserAdditionals on Usr.UserId equals UA.UserId
                          where Usr.UserId == Res.ShipmentDetail.CustomerId
                          select new
                          {
                              Usr.Email,
                              Rl.RoleId,
                              Usr.ContactName,
                              Usr.CompanyName,
                              UA.OperationUserId
                          }).FirstOrDefault();


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

            mailDetail.RoleId = result.RoleId;
            var UserEmail = "";
            var resultUser = (from Usr in dbContext.Users
                              join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId
                              where Usr.UserId == UserId
                              select new
                              {
                                  Usr.Email,
                                  Usr.ContactName
                              }).FirstOrDefault();

            if (resultUser != null)
            {
                UserEmail = resultUser.Email;
            }
            if (OperationName.OperationZoneId == 1)
            {
                Res.UserEmail = result1.Email;
                Res.UserName = result1.ContactName;
                Res.UserPosition = result1.Position;
                Res.CustomerName = resultUser.ContactName;
                Res.CustomerEmail = resultUser.Email;
                Res.UserPhone = "(+852) 2148 4880";
                Res.SiteAddress = AppSettings.TrackingUrl;
                Res.Site = "www.FRAYTE.com";
            }
            if (OperationName.OperationZoneId == 2)
            {
                Res.UserEmail = result1.Email;
                Res.UserPosition = result1.Position;
                Res.UserName = result1.ContactName;
                Res.CustomerName = resultUser.ContactName;
                Res.CustomerEmail = resultUser.Email;
                Res.UserPhone = "(+44) 01792 277295";
                Res.SiteAddress = AppSettings.TrackingUrl;
                Res.Site = "www.FRAYTE.co.uk";
            }
            var templateService = new TemplateService();
            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/Tradelane/E4.cshtml");
            var EmailBody = templateService.Parse(template, Res, viewBag, null);
            var EmailSubject = "";
            var Mawbvar = "";
            var Status = "Confirmation";
            if (!string.IsNullOrEmpty(Res.ShipmentDetail.MAWB))
            {
                Mawbvar = "MAWB# " + Res.ShipmentDetail.AirlinePreference.AilineCode + " " + Res.ShipmentDetail.MAWB.Substring(0, 4) + " " + Res.ShipmentDetail.MAWB.Substring(4, 4);
            }
            else
            {
                Mawbvar = Res.ShipmentDetail.FrayteNumber;
            }
            if (!string.IsNullOrEmpty(resultUser.ContactName))
            {
                EmailSubject = Mawbvar + " (" + Res.ShipmentDetail.DepartureAirport.AirportCode + " To " + Res.ShipmentDetail.DestinationAirport.AirportCode + ")" + " - " + resultUser.ContactName + " - Shipment MAWB Allocation";
            }

            Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Agent allocation email " + key));
            FrayteEmail.SendPreAlertMail(Res.CustomerEmail, "", "", "", EmailSubject, EmailBody, Attchament, ImagePath, Status);
            Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(EmailBody));
        }

        public void SendAgentConsolidateShipmentMail(List<TradelaneGetShipmentModel> TSList, int UserId)
        {
            if (TSList.Count > 0)
            {
                string logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";
                TradelaneEmailModel TEM = new TradelaneEmailModel();
                TEM.ConsolidateShipments = new AgentConsolidateMailModel();
                List<string> ImagePath = new List<string>();
                ImagePath.Add(logoImage);

                foreach (var item in TSList)
                {
                    string siteUrl = AppSettings.TrackingUrl + "/agent-action/";
                    string key = string.Empty;

                    if (item.ShipmentMethodHandlerId != 5)
                    {
                        key = item.FrayteRefNo + "-" + item.MAWBAgentId + "-" + item.ShipmentMethodHandlerId;
                    }
                    else
                    {
                        key = item.FrayteRefNo + "-" + item.LegNum + item.MAWBAgentId + "-" + item.ShipmentMethodHandlerId;
                    }

                    var df = UtilityRepository.Encrypt(key, EncriptionKey.PrivateKey);
                    var basae = UtilityRepository.Base64UrlEncode(df);
                    var en = WebUtility.UrlEncode(basae);
                    siteUrl += en;
                    item.MAWBURL = siteUrl;
                }

                TEM.ConsolidateShipments.User = new NewUserModel();
                TEM.ConsolidateShipments.TradelaneShipmentList = new List<TradelaneGetShipmentModel>();
                TEM.ConsolidateShipments.TradelaneShipmentList.AddRange(TSList);
                var user = dbContext.Users.Find(UserId);
                var OperationName = UtilityRepository.GetOperationZone();
                var Result = TSList;

                foreach (var TS in TEM.ConsolidateShipments.TradelaneShipmentList)
                {
                    TS.Status = TS.CreatedOn != null ? TS.CreatedOn.Date.ToString("dd-MMM-yyyy") : "";
                }

                DynamicViewBag DVB = new DynamicViewBag();
                DVB.AddValue("TsList", TSList);

                string Site = string.Empty;

                if (OperationName.OperationZoneId == 1)
                {
                    Site = AppSettings.TrackingUrl;
                }
                else if (OperationName.OperationZoneId == 2)
                {
                    Site = "";
                }

                TEM.UserName = user.ContactName;
                TEM.ConsolidateShipments.User.UserName = user.UserName;
                TEM.ConsolidateShipments.User.ImageHeader = "FrayteLogo";
                TEM.ImageHeader = "FrayteLogo";

                string OperationUserEmail = "";
                int RoleId = 0;

                var result = (from Usr in dbContext.Users
                              join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId
                              where Usr.UserId == user.UserId
                              select new
                              {
                                  Usr.Email,
                                  Rl.RoleId,
                                  Usr.CompanyName,
                                  Usr.ContactName
                              }).FirstOrDefault();

                if (result != null)
                {
                    RoleId = result.RoleId;
                    OperationUserEmail = result.Email;
                }

                TEM.ConsolidateShipments.User.RoleId = result.RoleId;
                if (OperationName.OperationZoneId == 1)
                {
                    TEM.UserEmail = result.Email;
                    TEM.UserPhone = "(+852) 2148 4880";
                    TEM.SiteAddress = AppSettings.TrackingUrl;
                    TEM.CustomerCompanyName = result.CompanyName;
                    TEM.SystemEmail = "system@frayte.com";
                    TEM.Site = "www.FRAYTE.com";
                }
                if (OperationName.OperationZoneId == 2)
                {
                    TEM.UserEmail = result.Email;
                    TEM.UserPhone = "(+44) 01792 277295";
                    TEM.SiteAddress = AppSettings.TrackingUrl;
                    TEM.CustomerCompanyName = result.CompanyName;
                    TEM.SystemEmail = "system@frayte.co.uk";
                    TEM.Site = "www.FRAYTE.co.uk";
                }

                string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/Tradelane/E4.1.cshtml");
                var templateService = new TemplateService();

                var EmailBody = Engine.Razor.RunCompile(template, "NewUse5", null, TEM);
                var Status = "Confirmation";
                var EmailSubject = "";
                if (!string.IsNullOrEmpty(result.ContactName))
                {
                    EmailSubject = "Consolidate Mail" + " - " + result.ContactName + " - Shipment Pending MAWB Allocation(s)";
                }
                if (!string.IsNullOrEmpty(TEM.UserEmail))
                {
                    FrayteEmail.SendPreAlertMail(TEM.UserEmail, "", "", "", EmailSubject, EmailBody, "", ImagePath, Status);
                }
            }
        }

        public void ShipmentReceivedMail(int TradelaneShipmentId)
        {
            string logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";
            List<string> ImagePath = new List<string>();
            ImagePath.Add(logoImage);
            var OperationName = UtilityRepository.GetOperationZone();

            var Res = TradelaneEmailObj(TradelaneShipmentId);
            string Site = string.Empty;
            if (OperationName.OperationZoneId == 1)
            {
                Res.SiteAddress = AppSettings.TrackingUrl;
            }
            else if (OperationName.OperationZoneId == 2)
            {
                Res.SiteAddress = AppSettings.TrackingUrl;
            }
            Res.ImageHeader = "FrayteLogo";
            string OperationUserEmail = "";
            int RoleId = 0;
            var Email = "";

            var result = (from Usr in dbContext.Users
                          join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId
                          join UA in dbContext.UserAdditionals on Usr.UserId equals UA.UserId
                          where Usr.UserId == Res.ShipmentDetail.CustomerId
                          select new
                          {
                              Usr.UserEmail,
                              Rl.RoleId,
                              Usr.ContactName,
                              Usr.CompanyName,
                              UA.OperationUserId
                          }).FirstOrDefault();

            Email = result.UserEmail + ';';
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
                Email = Email + result1.Email + ';';
                OperationUserEmail = result1.Email;
            }

            var UserEmail = "";
            var resultUser = (from Usr in dbContext.Users
                              join Rl in dbContext.UserRoles on Usr.UserId equals Rl.UserId
                              where Usr.UserId == Res.ShipmentDetail.CreatedBy
                              select new
                              {
                                  Usr.Email,
                                  Usr.ContactName
                              }).FirstOrDefault();

            if (resultUser != null)
            {
                Email = Email + resultUser.Email;
                UserEmail = resultUser.Email;
            }
            if (OperationName.OperationZoneId == 1)
            {
                Res.UserEmail = result1.Email;
                Res.UserName = result1.ContactName;
                Res.UserPosition = result1.Position;
                Res.CustomerName = result.ContactName;
                Res.CustomerEmail = result.UserEmail;
                Res.UserPhone = "(+852) 2148 4880";
                Res.SiteAddress = AppSettings.TrackingUrl;
                Res.Site = "www.FRAYTE.com";
            }
            if (OperationName.OperationZoneId == 2)
            {
                Res.UserEmail = result1.Email;
                Res.UserPosition = result1.Position;
                Res.UserName = result1.ContactName;
                Res.CustomerName = result.ContactName;
                Res.CustomerEmail = result.UserEmail;
                Res.UserPhone = "(+44) 01792 277295";
                Res.SiteAddress = AppSettings.TrackingUrl;
                Res.Site = "www.FRAYTE.co.uk";
            }

            string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/Tradelane/ShipmentReceived.cshtml");
            var EmailBody = Engine.Razor.RunCompile(template, "NewUser10", null, Res, null);
            var EmailSubject = "";
            var Mawbvar = "";
            var Status = "Confirmation";
            if (!string.IsNullOrEmpty(Res.ShipmentDetail.MAWB))
            {
                Mawbvar = "MAWB# " + Res.ShipmentDetail.MAWB.Substring(0, 4) + " " + Res.ShipmentDetail.MAWB.Substring(4, 4) + " " + Res.ShipmentDetail.AirlinePreference.AilineCode;
            }
            else
            {
                Mawbvar = Res.ShipmentDetail.FrayteNumber;
            }
            if (!string.IsNullOrEmpty(result.CompanyName) && !string.IsNullOrEmpty(result.ContactName))
            {

                EmailSubject = Mawbvar + " (" + Res.ShipmentDetail.DepartureAirport.AirportCode + " To " + Res.ShipmentDetail.DestinationAirport.AirportCode + ")" + " - " + result.CompanyName + ": " + result.ContactName + " - Shipment Received";
            }
            else
            {
                EmailSubject = Mawbvar + " (" + Res.ShipmentDetail.DepartureAirport.AirportCode + " To " + Res.ShipmentDetail.DestinationAirport.AirportCode + ")" + " - Shipment Received";
            }

            FrayteEmail.SendPreAlertMail(Email, "", "", "", EmailSubject, EmailBody, "", ImagePath, Status);
        }

        public void Send_PreAlertEmail(string toEmail, string ccEmail, string bccEmail, string replyTo, string EmailSubject, string EmailBody, string AttachmentFilePath, string Status)
        {
            string logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";
            List<string> ImagePath = new List<string>();
            ImagePath.Add(logoImage);

            if (!string.IsNullOrEmpty(AppSettings.TOCC))
            {
                FrayteEmail.SendPreAlertMail(AppSettings.TOCC, "", "", "", EmailSubject, EmailBody, AttachmentFilePath, ImagePath, Status);
            }
            else
            {
                FrayteEmail.SendPreAlertMail(toEmail, ccEmail, bccEmail, replyTo, EmailSubject, EmailBody, AttachmentFilePath, ImagePath, Status);
            }
        }

        public void SendMail_New(string toEmail, string ccEmail, string DisplayName, string EmailSubject, string EmailBody, string AttachmentFilePath, string Status)
        {
            string logoImage = AppSettings.EmailServicePath + "/Images/FrayteLogo.png";
            string amdentImage = AppSettings.EmailServicePath + "/Images/Amend.png";
            string confirmImage = AppSettings.EmailServicePath + "/Images/Confirm.png";
            string rejectImage = AppSettings.EmailServicePath + "/Images/Reject.png";

            List<string> ImagePath = new List<string>();
            ImagePath.Add(logoImage);
            ImagePath.Add(amdentImage);
            ImagePath.Add(confirmImage);
            ImagePath.Add(rejectImage);

            if (!string.IsNullOrEmpty(AppSettings.TOCC))
            {
                FrayteEmail.SendPreAlertMail(AppSettings.TOCC, "", "", "", EmailSubject, EmailBody, AttachmentFilePath, ImagePath, Status);
            }
            else
            {
                FrayteEmail.SendPreAlertMail(toEmail, ccEmail, "", "", EmailSubject, EmailBody, AttachmentFilePath, ImagePath, Status);
            }
        }

        public void SendMAWBEmail(int shipmentId, string leg)
        {
            var shipmentDetail = new TradelaneBookingRepository().GetTradelaneBookingDetails(shipmentId, "");
            if (shipmentDetail != null)
            {
                string url = string.Empty;
                DynamicViewBag viewBag = new DynamicViewBag();
                var db = dbContext.TradelaneShipmentAllocations.Where(p => p.TradelaneShipmentId == shipmentId).ToList();
                if (shipmentDetail.ShipmentHandlerMethod.ShipmentHandlerMethodId != 5)
                {
                    int AgentId = db[0].AgentId.HasValue ? db[0].AgentId.Value : 0;
                    var userDeatil = dbContext.Users.Find(AgentId);
                    url = shipmentDetail.FrayteNumber + "-" + shipmentDetail.ShipmentHandlerMethod.ShipmentHandlerMethodCode + "-" + AgentId;
                }
                else
                {
                    foreach (var item in db)
                    {
                        url = shipmentDetail.FrayteNumber + "-" + item.LegNum + "-" + shipmentDetail.ShipmentHandlerMethod.ShipmentHandlerMethodCode + "-" + item.AgentId;
                    }
                }

                viewBag.AddValue("MAWBFef", "");
                string template = File.ReadAllText(AppSettings.EmailServicePath + "/EmailTeamplate/UpdateTradelaneTracking.cshtml");
                var EmailBody = Engine.Razor.RunCompile(template, null, viewBag, null);
                var EmailSubject = "Update Tracking URL for tradelane system.";
                FrayteEmail.SendMail("prakash.pant@irasys.biz", "", EmailSubject, EmailBody, "", "");
            }
        }

        #endregion
    }
}