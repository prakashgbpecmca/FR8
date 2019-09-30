using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Windows.Forms;
using Frayte.Services.Business;
using Frayte.Services.Models;
using System.Configuration;
using System.Net.Http;
using System.Web;
using System.Diagnostics;
using frayteWinApp.Utility;
using System.Globalization;

namespace frayteWinApp
{
    public partial class frmEmailService : Form
    {

        #region Variable decalration

        string DevelopmentTeamEmail, OwnerMail, attachementpath;

        double workingTimeHour = 0;

        DateTime lastdatetime, Lastdate, current;

        TimeSpan Time, systemtime;

        bool workingfine, flag, formflag, RemainderSetting;

        #endregion

        #region Constructor

        public frmEmailService()
        {

            InitializeComponent();

            SetGlobalField();

            var body = "<!DOCTYPE html><html><head><title></title></head><body style=margin: 0; padding: 0; background: #eaf0f5; border: 1px solid #ddd; font-family: calibri, sans-serif;><header><div style=height: 70px; background: #fff !important; box-shadow: 0px 1px 4px -2px; padding: 0 30px; text-align: left;><img src=cid:FrayteLogo style=width: 150px; padding: 20px 0px;/>";
            body += "</div></header><div style=background: #fff; margin: 30px 20px; padding: 0 20px; border: 1px solid #ddd; font-size: 14px;><div style=margin: 30px 0;><p>Dear Sir</p><p>Windows service is alive and working fine.</p>";
            body += "<br /></div></div><footer><div style=height: 50px; border: 1px solid #ddd; background: #dce5ec !important; font-size: 12px; text-align: left; padding: 25px 30px;><p>FRAYTE LOGISTICS LTD. All rights reserved.</p></div></footer></body></html>";

            new ShipmentEmailRepository().SendMail(DevelopmentTeamEmail + ";" + OwnerMail, "", "Windows service is alive", body);

            flag = true;
            formflag = true;

            timer1 = new Timer();
            timer1.Tick += new EventHandler(timer1_Tick);
            //timer1.Interval = 900000; // 15 minutes
            timer1.Interval = 30000; //30000; //5 minute
            timer1.Enabled = true;
        }

        #endregion

        #region Events

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (flag)
                {
                    setWorkingMailHours();

                    if (workingTimeHour >= 12)
                    {
                        #region send mail to development team and owner of the product windows service working fine.

                        lastdatetime = DateTime.Now;
                        workingTimeHour = 0;

                        string body = "<!DOCTYPE html><html><head><title></title></head><body style=margin: 0; padding: 0; background: #eaf0f5; border: 1px solid #ddd; font-family: calibri, sans-serif;><header><div style=height: 70px; background: #fff !important; box-shadow: 0px 1px 4px -2px; padding: 0 30px; text-align: left;><img src=cid:FrayteLogo style=width: 150px; padding: 20px 0px;/>";
                        body += "</div></header><div style=background: #fff; margin: 30px 20px; padding: 0 20px; border: 1px solid #ddd; font-size: 14px;><div style=margin: 30px 0;><p>Dear Sir</p><p>Window Service is working fine.</p>";
                        body += "<br /></div></div><footer><div style=height: 50px; border: 1px solid #ddd; background: #dce5ec !important; font-size: 12px; text-align: left; padding: 25px 30px;><p>FRAYTE LOGISTICS LTD. All rights reserved.</p></div></footer></body></html>";

                        new ShipmentEmailRepository().SendMail(DevelopmentTeamEmail + ";" + OwnerMail, "", "Window Service is working fine", body);

                        #endregion
                    }

                    flag = false;

                    #region Send email

                    var _shipmentdetail = new ShipmentEmailRepository().GetShipmentEmailDetail();

                    foreach (var shd in _shipmentdetail)
                    {

                        #region Chetram Old

                        ////var tzWorkingStartTime = UtilityRepository.GetTimeZoneTimeSpan(shd.WorkingStartTime, shd.TimeZoneName);
                        ////var tzWorkingEndTime = UtilityRepository.GetTimeZoneTimeSpan(shd.WorkingEndTime, shd.TimeZoneName);
                        ////var tzEmailSentTime = UtilityRepository.GetTimeZoneTimeSpan(shd.EmailSentTime, shd.TimeZoneName);
                        ////var tzCurrent = UtilityRepository.GetTimeZoneTimeSpan(DateTime.UtcNow.TimeOfDay, shd.TimeZoneName);
                        ////var tzmailDate = DateTime.UtcNow.Date; // This logic may change
                        ////TimeSpan? tzmailCurrent = new TimeSpan?();

                        ////if (RemainderSetting)
                        ////{
                        ////    if (shd.RemindType == "AFTER")
                        ////    {
                        ////        tzmailCurrent = UtilityRepository.GetTimeZoneTimeSpan(DateTime.UtcNow.AddHours(-shd.ReminderIn).TimeOfDay, shd.TimeZoneName);
                        ////        tzmailDate = DateTime.UtcNow.Date; // This logic may change
                        ////    }
                        ////    else if (shd.RemindType == "BEFORE")
                        ////    {
                        ////        tzmailCurrent = UtilityRepository.GetTimeZoneTimeSpan(DateTime.UtcNow.AddHours(shd.ReminderIn).TimeOfDay, shd.TimeZoneName);
                        ////        tzmailDate = DateTime.UtcNow.Date; // This logic may change
                        ////    }
                        ////}
                        ////else
                        ////{
                        ////    if (shd.RemindType == "AFTER")
                        ////    {
                        ////        tzmailCurrent = UtilityRepository.GetTimeZoneTimeSpan(DateTime.UtcNow.AddMinutes(-shd.ReminderIn).TimeOfDay, shd.TimeZoneName);
                        ////        tzmailDate = DateTime.UtcNow.Date; // This logic may change
                        ////    }
                        ////    else if (shd.RemindType == "BEFORE")
                        ////    {
                        ////        tzmailCurrent = UtilityRepository.GetTimeZoneTimeSpan(DateTime.UtcNow.AddMinutes(shd.ReminderIn).TimeOfDay, shd.TimeZoneName);
                        ////        tzmailDate = DateTime.UtcNow.Date; // This logic may change
                        ////    }
                        ////}

                        #endregion

                        #region Prakash

                        // Requirement
                        // Need MailTo Working Start Time And Working End Time 
                        // Server Current Date and Time amd convert it to time according to MailTo TimeZone 
                        // EmailSentDate and EmailSentTime from ShipmentEmailService  in DB

                        var MailToWorkingStartTime = UtilityRepository.GetTimeZoneTimeSpan(shd.WorkingStartTime, shd.TimeZoneName);
                        var MailToWorkingEndTime = UtilityRepository.GetTimeZoneTimeSpan(shd.WorkingEndTime, shd.TimeZoneName);
                        var DbEmailSentTime = shd.EmailSentTime;
                        var DBMailToEmailSentTime = UtilityRepository.GetTimeZoneTimeSpan(DbEmailSentTime, shd.TimeZoneName);
                        var DBEmailSentDate = shd.EmailSentDate;
                        // DBEmaiSentDate according to MailTo TimeZone
                        //TimeZoneInfo.ConvertTime(DBEmailSentDate.Value, TimeZoneInfo.FindSystemTimeZoneById(shd.TimeZoneName));
                        var DBMMailToEmailSentDate = shd.EmailSentDate;
                        var ServerCurrentTime = DateTime.UtcNow.TimeOfDay;
                        var MailToServerCurrentTime = UtilityRepository.GetTimeZoneTimeSpan(ServerCurrentTime, shd.TimeZoneName); // Server Current Time
                        var ServerCurrentDate = DateTime.UtcNow.Date;
                        // Server Current Date According to MailTo TimeZone
                        //TimeZoneInfo.ConvertTime(ServerCurrentDate, TimeZoneInfo.FindSystemTimeZoneById(shd.TimeZoneName));
                        var MailToServerCurrentDate = ServerCurrentDate;

                        #region
                        //  TimeSpan? MailToCurrentTimeAfterReminderIn = new TimeSpan?();
                        //if (RemainderSetting)
                        //{
                        //    if (shd.RemindType == "AFTER")
                        //    {
                        //        MailToCurrentTimeAfterReminderIn = UtilityRepository.GetTimeZoneTimeSpan(DateTime.UtcNow.AddHours(-shd.ReminderIn).TimeOfDay, shd.TimeZoneName);
                        //    }
                        //    else if (shd.RemindType == "BEFORE")
                        //    {
                        //        MailToCurrentTimeAfterReminderIn = UtilityRepository.GetTimeZoneTimeSpan(DateTime.UtcNow.AddHours(shd.ReminderIn).TimeOfDay, shd.TimeZoneName);
                        //    }
                        //}
                        //else
                        //{
                        //    if (shd.RemindType == "AFTER")
                        //    {
                        //        MailToCurrentTimeAfterReminderIn = UtilityRepository.GetTimeZoneTimeSpan(DateTime.UtcNow.AddMinutes(-shd.ReminderIn).TimeOfDay, shd.TimeZoneName);
                        //    }
                        //    else if (shd.RemindType == "BEFORE")
                        //    {
                        //        MailToCurrentTimeAfterReminderIn = UtilityRepository.GetTimeZoneTimeSpan(DateTime.UtcNow.AddMinutes(shd.ReminderIn).TimeOfDay, shd.TimeZoneName);
                        //    }
                        //}
                        #endregion
                        #endregion

                        try
                        {
                            #region Chetram Old

                            ////if (tzCurrent >= tzWorkingStartTime && tzCurrent <= tzWorkingEndTime && tzEmailSentTime <= tzmailCurrent && !string.IsNullOrEmpty(shd.ManagerEEmailTemplateId) && shd.IsRepeatReminder == true
                            ////  && shd.EmailSentToManagerCount > 0 && shd.EmailSentCount < shd.EmailSentToManagerCount - 1 && shd.RemindType == "AFTER" && shd.IsPublicHoliday == false)
                            ////{
                            ////    var currutctime = GetTimeZoneUTCTime(tzCurrent.ToString(), shd.TimeZoneName);
                            ////    sendEmail(shd.ShipmentId, shd.EmailTemplateId);
                            ////    new ShipmentEmailRepository().SetShipmentEmailService(shd.ShipmentEmailServiceId, currutctime, shd.EmailSentCount + 1);
                            ////}
                            ////else if (tzCurrent >= tzWorkingStartTime && tzCurrent <= tzWorkingEndTime && tzEmailSentTime <= tzmailCurrent && !string.IsNullOrEmpty(shd.ManagerEEmailTemplateId) && shd.IsRepeatReminder == true
                            ////   && shd.EmailSentToManagerCount > 0 && shd.EmailSentCount >= shd.EmailSentToManagerCount - 1 && shd.RemindType == "AFTER" && shd.IsPublicHoliday == false)
                            ////{
                            ////    var currutctime = GetTimeZoneUTCTime(tzCurrent.ToString(), shd.TimeZoneName);
                            ////    sendEmail(shd.ShipmentId, shd.ManagerEEmailTemplateId);
                            ////    new ShipmentEmailRepository().SetShipmentEmailService(shd.ShipmentEmailServiceId, currutctime, shd.EmailSentCount + 1);
                            ////}
                            ////else if (tzCurrent >= tzWorkingStartTime && tzCurrent <= tzWorkingEndTime && tzEmailSentTime <= tzmailCurrent && shd.IsRepeatReminder == true && shd.RemindType == "AFTER" && shd.IsPublicHoliday == false)
                            ////{
                            ////    var currutctime = GetTimeZoneUTCTime(tzCurrent.ToString(), shd.TimeZoneName);
                            ////    sendEmail(shd.ShipmentId, shd.EmailTemplateId);
                            ////    new ShipmentEmailRepository().SetShipmentEmailService(shd.ShipmentEmailServiceId, currutctime, shd.EmailSentCount + 1);
                            ////}
                            ////else if (tzCurrent >= tzWorkingStartTime && tzCurrent <= tzWorkingEndTime && tzEmailSentTime <= tzmailCurrent && shd.IsRepeatReminder == false && shd.RemindType == "AFTER" && shd.IsPublicHoliday == false)
                            ////{
                            ////    sendEmail(shd.ShipmentId, shd.EmailTemplateId);
                            ////    new ShipmentEmailRepository().DeleteShipmentEmailService(shd.ShipmentEmailServiceId);
                            ////    if (shd.EmailTemplateId == EmailTempletType.E7)
                            ////    {
                            ////        new ShipmentEmailRepository().LogForEmailSending(EmailTempletType.E7_1, shd.ShipmentId, DateTime.UtcNow.Date, DateTime.UtcNow.TimeOfDay, 1);
                            ////    }
                            ////    else if (shd.EmailTemplateId == EmailTempletType.E13)
                            ////    {
                            ////        new ShipmentEmailRepository().LogForEmailSending(EmailTempletType.E13_1, shd.ShipmentId, DateTime.UtcNow.Date, DateTime.UtcNow.TimeOfDay, 1);
                            ////    }
                            ////}
                            ////else if (tzCurrent >= tzWorkingStartTime && tzCurrent <= tzWorkingEndTime && tzEmailSentTime <= tzmailCurrent && shd.IsRepeatReminder == false && shd.RemindType == "BEFORE" && shd.IsPublicHoliday == false)
                            ////{
                            ////    sendEmail(shd.ShipmentId, shd.EmailTemplateId);
                            ////    new ShipmentEmailRepository().DeleteShipmentEmailService(shd.ShipmentEmailServiceId);
                            ////    if (shd.EmailTemplateId == EmailTempletType.E8)
                            ////    {
                            ////        new ShipmentEmailRepository().LogForEmailSending(EmailTempletType.E8_1, shd.ShipmentId, DateTime.UtcNow.Date, DateTime.UtcNow.TimeOfDay, 1);
                            ////    }
                            ////}
                            ////else if (tzCurrent >= tzWorkingStartTime && tzCurrent <= tzWorkingEndTime && tzEmailSentTime <= tzmailCurrent && shd.IsRepeatReminder == true && shd.RemindType == "BEFORE" && shd.IsPublicHoliday == false)
                            ////{
                            ////    var currutctime = GetTimeZoneUTCTime(tzCurrent.ToString(), shd.TimeZoneName);
                            ////    sendEmail(shd.ShipmentId, shd.EmailTemplateId);
                            ////    new ShipmentEmailRepository().SetShipmentEmailService(shd.ShipmentEmailServiceId, currutctime, shd.EmailSentCount + 1);
                            ////}

                            #endregion

                            #region Prakash

                            if (MailToServerCurrentTime >= MailToWorkingStartTime && MailToServerCurrentTime <= MailToWorkingEndTime
                                && DateTime.Compare(DBMMailToEmailSentDate.Value, MailToServerCurrentDate) == 0 && shd.IsPublicHoliday == false)
                            {
                                #region
                                if (shd.RemindType == "AFTER")
                                {
                                    TimeSpan time1 = TimeSpan.FromHours(1);
                                    var DBEmailSentTimeAfterReminderIn = DBMailToEmailSentTime.Value.Add(time1);
                                    if (TimeSpan.Compare(DBEmailSentTimeAfterReminderIn, MailToServerCurrentTime.Value) == -1 || TimeSpan.Compare(DBEmailSentTimeAfterReminderIn, MailToServerCurrentTime.Value) == 0)
                                    {
                                        if (shd.EmailSentCount >= shd.EmailSentToManagerCount)
                                        {
                                            // Logic to Send Mail  to MailTo
                                            if (!string.IsNullOrEmpty(shd.ManagerEmailTemplateId))
                                            {
                                                sendEmail(shd.ShipmentId, shd.ManagerEmailTemplateId);
                                            }
                                            else
                                            {
                                                sendEmail(shd.ShipmentId, shd.EmailTemplateId);
                                            }
                                            shd.EmailSentCount = shd.EmailSentCount + 1;
                                        }
                                        else
                                        {
                                            // Logic to Send Mail  to MailTo
                                            sendEmail(shd.ShipmentId, shd.EmailTemplateId);

                                            if (shd.IsRepeatReminder == false)
                                            {
                                                new ShipmentEmailRepository().DeleteShipmentEmailService(shd.ShipmentEmailServiceId);
                                                if (shd.EmailTemplateId == "E7")
                                                {
                                                    //Also log the email sending time for ShipmentEmailService.
                                                    new ShipmentEmailRepository().LogForEmailSending(EmailTempletType.E7_1, shd.ShipmentId, ServerCurrentDate, ServerCurrentTime, shd.EmailSentCount);
                                                }
                                                if (shd.EmailTemplateId == "E8")
                                                {
                                                    //Also log the email sending time for ShipmentEmailService.
                                                    new ShipmentEmailRepository().LogForEmailSending(EmailTempletType.E8_1, shd.ShipmentId, ServerCurrentDate, ServerCurrentTime, shd.EmailSentCount);
                                                }
                                                if (shd.EmailTemplateId == "E13")
                                                {
                                                    //Also log the email sending time for ShipmentEmailService.
                                                    new ShipmentEmailRepository().LogForEmailSending(EmailTempletType.E13_1, shd.ShipmentId, ServerCurrentDate, ServerCurrentTime, shd.EmailSentCount);

                                                }
                                            }
                                            else
                                            {
                                                shd.EmailSentCount = shd.EmailSentCount + 1;
                                            }
                                        }
                                        // Add ReminderIn in MailToServerCurrentTime
                                        TimeSpan time2 = TimeSpan.FromHours(1);
                                        var MailToServerCurrentTimeAfterReminderIn = MailToServerCurrentTime.Value.Add(time2);
                                        // need to check the IsRepoeatReminder before updating the database 
                                        if (TimeSpan.Compare(MailToServerCurrentTimeAfterReminderIn, MailToWorkingEndTime.Value) == 1 && shd.IsRepeatReminder)
                                        {
                                            // Need to update EmailSentTime and EmailSentDate in database EmailSentTime = WST-ReminderIn
                                            TimeSpan time3 = TimeSpan.FromHours(-1);
                                            var AfterReminderIn = MailToWorkingStartTime.Value.Add(time3);
                                            var MailToWorkingStartTimeAfterReminderIn = UtilityRepository.GetTimeZoneUTCTime(AfterReminderIn, shd.TimeZoneName);

                                            // Save in Database
                                            new ShipmentEmailRepository().SetShipmentEmailService(shd.ShipmentEmailServiceId, MailToServerCurrentDate.AddDays(1), MailToWorkingStartTimeAfterReminderIn, shd.EmailSentCount);
                                        }
                                        else if ((TimeSpan.Compare(MailToServerCurrentTimeAfterReminderIn, MailToWorkingEndTime.Value) == -1 || TimeSpan.Compare(MailToServerCurrentTimeAfterReminderIn, MailToWorkingEndTime.Value) == 0) && shd.IsRepeatReminder)
                                        {
                                            // Need to update EmailSentTime and EmailSentDate in Database 
                                            new ShipmentEmailRepository().SetShipmentEmailService(shd.ShipmentEmailServiceId, ServerCurrentDate, ServerCurrentTime, shd.EmailSentCount);
                                        }
                                    }

                                    else if (TimeSpan.Compare(DBEmailSentTimeAfterReminderIn, MailToServerCurrentTime.Value) == 1 && TimeSpan.Compare(DBEmailSentTimeAfterReminderIn, MailToWorkingEndTime.Value) == 1)
                                    {

                                        // Need to update EmailSentTime and EmailSentDate in database EmailSentTime = WST-ReminderIn

                                        TimeSpan time3 = TimeSpan.FromHours(-1);
                                        var AfterReminderIn = MailToWorkingStartTime.Value.Add(time3);
                                        var MailToWorkingStartTimeAfterReminderIn = UtilityRepository.GetTimeZoneUTCTime(AfterReminderIn, shd.TimeZoneName);
                                        var ServerNextDay = DateTime.UtcNow.AddDays(1).Date;
                                        new ShipmentEmailRepository().SetShipmentEmailService(shd.ShipmentEmailServiceId, ServerNextDay, MailToWorkingStartTimeAfterReminderIn, shd.EmailSentCount);
                                    }

                                }
                                if (shd.RemindType == "BEFORE")
                                {

                                }
                                #endregion
                            }
                            else if (MailToServerCurrentTime >= MailToWorkingStartTime && MailToServerCurrentTime <= MailToWorkingEndTime
                                && DateTime.Compare(MailToServerCurrentDate, DBMMailToEmailSentDate.Value) == 1)
                            {
                                #region
                                if (shd.RemindType == "AFTER")
                                {
                                    // Need to set EmailSentDate to Server Current Date 
                                    TimeSpan time = TimeSpan.FromHours(-1);
                                    var AfterReminderIn = MailToWorkingStartTime.Value.Add(time);
                                    var MailToWorkingStartTimeAfterReminderIn = UtilityRepository.GetTimeZoneUTCTime(AfterReminderIn, shd.TimeZoneName);
                                    new ShipmentEmailRepository().SetShipmentEmailService(shd.ShipmentEmailServiceId, ServerCurrentDate, MailToWorkingStartTimeAfterReminderIn, shd.EmailSentCount);
                                    // Update RemindType and set ReminderIn= 1

                                    // Calculate the EmailSent Time accordingly
                                }
                                #endregion
                            }
                            // if ServerCurrentTime > WorkingStartTime 
                            else if (MailToServerCurrentTime >= MailToWorkingStartTime && MailToServerCurrentTime >= MailToWorkingEndTime && DateTime.Compare(MailToServerCurrentDate, DBMMailToEmailSentDate.Value) == 0)
                            {
                                #region
                                // Check RemindType= After
                                if (shd.RemindType == "AFTER")
                                {
                                    TimeSpan time3 = TimeSpan.FromHours(-1);
                                    var AfterReminderIn = MailToWorkingStartTime.Value.Add(time3);
                                    var MailToWorkingStartTimeAfterReminderIn = UtilityRepository.GetTimeZoneUTCTime(AfterReminderIn, shd.TimeZoneName);
                                    var ServerNextDay = DateTime.UtcNow.AddDays(1).Date;
                                    // Need to update EmailSentTime and EmailSentDate in database EmailSentTime = WST-ReminderIn
                                    new ShipmentEmailRepository().SetShipmentEmailService(shd.ShipmentEmailServiceId, ServerNextDay, MailToWorkingStartTimeAfterReminderIn, shd.EmailSentCount);

                                }
                                #endregion
                            }
                            else
                            {
                                // implementaion 
                            }
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            #region Exception

                            SendExceptionMail(ex);

                            #endregion
                        }
                    }

                    #endregion

                    flag = true;
                }

                // Send Email For Shipment POD Report
                sendPODReportEmail();

                // Send Email For RateCard Report
                SendRateCardEmail();

                //Update Delivered or Delayed Shipment
                UpdateDirectShipment();

                //Send Fuel SurCharge Mail
                //SendFuelSurchargeAndCurrencyEmail();                
            }
            catch (Exception ex)
            {
                #region Exception

                SendExceptionMail(ex);

                #endregion
            }
        }

        private void frmEmailService_FormClosing(object sender, FormClosingEventArgs e)
        {
            string body = "<!DOCTYPE html><html><head><title></title></head><body style=margin: 0; padding: 0; background: #eaf0f5; border: 1px solid #ddd; font-family: calibri, sans-serif;><header><div style=height: 70px; background: #fff !important; box-shadow: 0px 1px 4px -2px; padding: 0 30px; text-align: left;><img src=cid:FrayteLogo style=width: 150px; padding: 20px 0px;/>";
            body += "</div></header><div style=background: #fff; margin: 30px 20px; padding: 0 20px; border: 1px solid #ddd; font-size: 14px;><div style=margin: 30px 0;><p>Dear Sir</p><p>Window Service has been stop due to any error or fault.</p>";
            body += "<br /></div></div><footer><div style=height: 50px; border: 1px solid #ddd; background: #dce5ec !important; font-size: 12px; text-align: left; padding: 25px 30px;><p>FRAYTE LOGISTICS LTD. All rights reserved.</p></div></footer></body></html>";

            if (formflag)
            {
                new ShipmentEmailRepository().SendMail(DevelopmentTeamEmail + ";" + OwnerMail, "", "Window Service has been stoped", body);
                formflag = false;
                Application.Exit();
            }

            if (e.CloseReason == CloseReason.TaskManagerClosing)
            {
                new ShipmentEmailRepository().SendMail(DevelopmentTeamEmail + ";" + OwnerMail, "", "Window Service has been stoped", body);
                formflag = false;
                Application.Exit();
            }

            e.Cancel = true;
        }


        #endregion

        #region Methods

        public void SendExceptionMail(Exception ex)
        {

            string Content = "<!DOCTYPE html><html><head><title></title></head><body style=margin: 0; padding: 0; background: #eaf0f5; border: 1px solid #ddd; font-family: calibri, sans-serif;><header><div style=height: 70px; background: #fff !important; box-shadow: 0px 1px 4px -2px; padding: 0 30px; text-align: left;><img src=cid:FrayteLogo style=width: 150px; padding: 20px 0px;/>";
            Content += "</div></header><div style=background: #fff; margin: 30px 20px; padding: 0 20px; border: 1px solid #ddd; font-size: 14px;><div style=margin: 30px 0;><p>Dear Sir</p><p>" + ex.Message + ".</p>";
            if (ex.StackTrace != null)
            {
                Content += "<p>" + ex.StackTrace + ".</p>";
            }
            if (ex.InnerException != null && ex.InnerException.Message != null)
            {
                Content += "<p>" + ex.InnerException.Message + ".</p>";

                if (ex.InnerException.StackTrace != null)
                {
                    Content += "<p>" + ex.InnerException.StackTrace + ".</p>";
                }
                if (ex.InnerException.InnerException != null && ex.InnerException.InnerException.Message != null)
                {
                    Content += "<p>" + ex.InnerException.InnerException.Message + ".</p>";
                }

            }
            Content += "<br /></div></div><footer><div style=height: 50px; border: 1px solid #ddd; background: #dce5ec !important; font-size: 12px; text-align: left; padding: 25px 30px;><p>FRAYTE LOGISTICS LTD. All rights reserved.</p></div></footer></body></html>";

            new ShipmentEmailRepository().SendMail(DevelopmentTeamEmail, "", "Error have been gone in Frayte Logistic", Content);
        }

        public TimeSpan? GetTimeZoneUTCTime(string time, string timeZoneName)
        {
            if (!string.IsNullOrEmpty(time) && !string.IsNullOrEmpty(timeZoneName))
            {
                var hh = Convert.ToInt32(time.Substring(0, 2));
                var mm = Convert.ToInt32(time.Substring(3, 2));

                var finalTime = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, hh, mm, 0);

                TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneName);
                DateTime dtConverted = TimeZoneInfo.ConvertTimeToUtc(finalTime, timeZone);

                return dtConverted.TimeOfDay;
            }

            return null;
        }

        public void sendEmail(int shipmentid, string templatetype)
        {
            switch (templatetype)
            {
                case EmailTempletType.E1:
                    new ShipmentEmailRepository().SendEmail_E1(shipmentid);
                    break;
                case EmailTempletType.E4_1:
                    new ShipmentEmailRepository().SendEmail_E4_1(shipmentid);
                    break;
                case EmailTempletType.E6:
                    new ShipmentEmailRepository().SendEmail_E6(shipmentid);
                    break;
                case EmailTempletType.E6_1:
                    new ShipmentEmailRepository().SendEmail_E61(shipmentid);
                    break;
                case EmailTempletType.E6_2:
                    new ShipmentEmailRepository().SendEmail_E62(shipmentid);
                    break;
                case EmailTempletType.E6_3:
                    new ShipmentEmailRepository().SendEmail_E63(shipmentid);
                    break;
                case EmailTempletType.E6_6:
                    new ShipmentEmailRepository().SendEmail_E66(shipmentid);
                    break;
                case EmailTempletType.E7:
                    new ShipmentEmailRepository().SendEmail_E7(shipmentid);
                    break;
                case EmailTempletType.E7_1:
                    new ShipmentEmailRepository().SendEmail_E71(shipmentid);
                    break;
                case EmailTempletType.E8:
                    new ShipmentEmailRepository().SendEmail_E8(shipmentid);
                    break;
                case EmailTempletType.E8_1:
                    new ShipmentEmailRepository().SendEmail_E81(shipmentid);
                    break;
                case EmailTempletType.E9:
                    new ShipmentEmailRepository().SendEmail_E9(shipmentid);
                    break;
                case EmailTempletType.E9_1:
                    new ShipmentEmailRepository().SendEmail_E91(shipmentid);
                    break;
                case EmailTempletType.E13:
                    new ShipmentEmailRepository().SendEmail_E13(shipmentid);
                    break;
                case EmailTempletType.E13_1:
                    new ShipmentEmailRepository().SendEmail_E131(shipmentid);
                    break;
                default:
                    break;
            }
        }

        public void setWorkingMailHours()
        {
            try
            {
                workingTimeHour = DateTime.Now.Subtract(lastdatetime).TotalHours;
            }
            catch { }
        }

        public void SetGlobalField()
        {
            DevelopmentTeamEmail = ConfigurationManager.AppSettings["DevelopmentMail"].ToString();
            OwnerMail = ConfigurationManager.AppSettings["OwnerMail"].ToString();
            lastdatetime = DateTime.Now;

            if (ConfigurationManager.AppSettings["RemainderSetting"].ToString() == "Hour")
            {
                RemainderSetting = true;
            }
        }

        public void sendPODReportEmail()
        {
            try
            {
                var setting = new ReportSettingRepository().GetAllReportSettings();
                foreach (var status in setting)
                {
                    if (status.ScheduleSettingType == FrayteCustomerMailSetting.POD)
                    {
                        if (status.CustomerPODSettingId > 0)
                        {
                            TimeSpan ts = TimeSpan.Parse("10:00:00");
                            Time = TimeSpan.Parse(DateTime.Now.ToString("HH:mm"));
                            current = Frayte.Services.CommonConversion.ConvertToDateTime(DateTime.UtcNow.ToString("MM/dd/yyyy"));
                            switch (status.PODScheduleSetting)
                            {
                                //Check PODScheduleSetting is Scheduled
                                case FrayteReportSettingDays.PerShipment:
                                    switch (status.ScheduleType)
                                    {
                                        case FrayteReportSettingDays.Daily:
                                            if (status.UpdatedOn != null)
                                            {
                                                Lastdate = status.UpdatedOn.Value.AddDays(1);
                                                if (current == Lastdate && Time >= status.ScheduleTime)
                                                {
                                                    if (current == Lastdate && Time >= status.ScheduleTime)
                                                    {
                                                        new ReportSettingRepository().SendEmail(status.CustomerPODSettingId, status.UserId, CommunicateWebApi.GetPODPath());
                                                        new ReportSettingRepository().UpdateDirectShipment(status.UserId);
                                                        new ReportSettingRepository().UpdateReportSettingDate(status.CustomerPODSettingId, Lastdate);
                                                    }
                                                    else if (current.Date > status.UpdatedOn && Time == status.ScheduleTime)
                                                    {
                                                        new ReportSettingRepository().SendEmail(status.CustomerPODSettingId, status.UserId, CommunicateWebApi.GetPODPath());
                                                        new ReportSettingRepository().UpdateDirectShipment(status.UserId);
                                                        new ReportSettingRepository().UpdateReportSettingTime_Date(status.CustomerPODSettingId, current.AddDays(1), ts);
                                                    }
                                                }
                                            }
                                            break;
                                        case FrayteReportSettingDays.Weekly:
                                            if (status.UpdatedOn != null)
                                            {
                                                Lastdate = status.UpdatedOn.Value.AddDays(7);
                                                if (current == Lastdate && Time >= status.ScheduleTime)
                                                {
                                                    if (current == Lastdate && Time >= status.ScheduleTime)
                                                    {
                                                        new ReportSettingRepository().SendEmail(status.CustomerPODSettingId, status.UserId, CommunicateWebApi.GetPODPath());
                                                        new ReportSettingRepository().UpdateDirectShipment(status.UserId);
                                                        new ReportSettingRepository().UpdateReportSettingDate(status.CustomerPODSettingId, Lastdate);
                                                    }
                                                    else if (current.Date > status.CreatedOn && Time == status.ScheduleTime)
                                                    {
                                                        new ReportSettingRepository().SendEmail(status.CustomerPODSettingId, status.UserId, CommunicateWebApi.GetPODPath());
                                                        new ReportSettingRepository().UpdateDirectShipment(status.UserId);
                                                        new ReportSettingRepository().UpdateReportSettingTime_Date(status.CustomerPODSettingId, current.AddDays(1), ts);
                                                    }
                                                }
                                            }
                                            break;
                                        case FrayteReportSettingDays.Monthly:
                                            if (status.UpdatedOn != null)
                                            {
                                                Lastdate = status.UpdatedOn.Value.AddMonths(1);
                                                if (current == Lastdate && Time >= status.ScheduleTime)
                                                {
                                                    if (current == Lastdate && Time >= status.ScheduleTime)
                                                    {
                                                        new ReportSettingRepository().SendEmail(status.CustomerPODSettingId, status.UserId, CommunicateWebApi.GetPODPath());
                                                        new ReportSettingRepository().UpdateDirectShipment(status.UserId);
                                                        new ReportSettingRepository().UpdateReportSettingDate(status.CustomerPODSettingId, Lastdate);
                                                    }
                                                    else if (current.Date > status.CreatedOn && Time == status.ScheduleTime)
                                                    {
                                                        new ReportSettingRepository().SendEmail(status.CustomerPODSettingId, status.UserId, CommunicateWebApi.GetPODPath());
                                                        new ReportSettingRepository().UpdateDirectShipment(status.UserId);
                                                        new ReportSettingRepository().UpdateReportSettingTime_Date(status.CustomerPODSettingId, current.AddDays(1), ts);
                                                    }
                                                }
                                            }
                                            break;
                                        case FrayteReportSettingDays.Yearly:
                                            if (status.UpdatedOn != null)
                                            {
                                                Lastdate = status.UpdatedOn.Value.AddYears(1);
                                                if (current == Lastdate && Time >= status.ScheduleTime)
                                                {
                                                    if (current == Lastdate && Time >= status.ScheduleTime)
                                                    {
                                                        new ReportSettingRepository().SendEmail(status.CustomerPODSettingId, status.UserId, CommunicateWebApi.GetPODPath());
                                                        new ReportSettingRepository().UpdateDirectShipment(status.UserId);
                                                        new ReportSettingRepository().UpdateReportSettingDate(status.CustomerPODSettingId, Lastdate);
                                                    }
                                                    else if (current.Date > status.CreatedOn && Time == status.ScheduleTime)
                                                    {
                                                        new ReportSettingRepository().SendEmail(status.CustomerPODSettingId, status.UserId, CommunicateWebApi.GetPODPath());
                                                        new ReportSettingRepository().UpdateDirectShipment(status.UserId);
                                                        new ReportSettingRepository().UpdateReportSettingTime_Date(status.CustomerPODSettingId, current.AddDays(1), ts);
                                                    }
                                                }
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SendExceptionMail(ex);
            }
        }

        public void SendRateCardEmail()
        {
            try
            {
                var setting = new ReportSettingRepository().GetAllReportSettings();
                foreach (var status in setting)
                {
                    if (status.ScheduleSettingType == FrayteCustomerMailSetting.RateCard)
                    {
                        if (status.CustomerPODSettingId > 0)
                        {
                            TimeSpan ts = TimeSpan.Parse(AppSettings.RateCard);
                            Time = TimeSpan.Parse(DateTime.Now.ToString("HH:mm"));
                            current = Frayte.Services.CommonConversion.ConvertToDateTime(DateTime.UtcNow.ToString("MM/dd/yyyy"));
                            switch (status.PODScheduleSetting)
                            {
                                //Check PODScheduleSetting is Scheduled
                                case FrayteReportSettingDays.Scheduled:
                                    switch (status.ScheduleType)
                                    {
                                        case FrayteReportSettingDays.Daily:
                                            if (status.UpdatedOn != null)
                                            {
                                                Lastdate = status.UpdatedOn.Value.AddDays(1);
                                                if (current == Lastdate && Time >= status.ScheduleTime)
                                                {
                                                    //foreach (var courier in status.fryateUserSettingDetail)
                                                    //{
                                                    if (current == Lastdate && Time >= status.ScheduleTime)
                                                    {
                                                        if (status.IsPdf == true && status.IsExcel == true)
                                                        {
                                                            new ReportSettingRepository().RateCardPdfSendEmail(status.CustomerPODSettingId, status.UserId, CommunicateWebApi.GetPDFPath(status.UserId));
                                                            new ReportSettingRepository().RateCardExcelSendEmail(status.CustomerPODSettingId, status.UserId, CommunicateWebApi.GetExcelPath(status.UserId));
                                                        }
                                                        else if (status.IsExcel == true)
                                                        {
                                                            new ReportSettingRepository().RateCardExcelSendEmail(status.CustomerPODSettingId, status.UserId, CommunicateWebApi.GetExcelPath(status.UserId));
                                                        }
                                                        else if (status.IsPdf == true)
                                                        {
                                                            new ReportSettingRepository().RateCardPdfSendEmail(status.CustomerPODSettingId, status.UserId, CommunicateWebApi.GetPDFPath(status.UserId));
                                                        }
                                                        new ReportSettingRepository().UpdateReportSettingDate(status.CustomerPODSettingId, Lastdate);
                                                    }
                                                    else if (current.Date > status.UpdatedOn && Time == status.ScheduleTime)
                                                    {
                                                        if (status.IsPdf == true && status.IsExcel == true)
                                                        {
                                                            new ReportSettingRepository().RateCardPdfSendEmail(status.CustomerPODSettingId, status.UserId, CommunicateWebApi.GetPDFPath(status.UserId));
                                                            new ReportSettingRepository().RateCardExcelSendEmail(status.CustomerPODSettingId, status.UserId, CommunicateWebApi.GetExcelPath(status.UserId));
                                                        }
                                                        else if (status.IsExcel == true)
                                                        {
                                                            new ReportSettingRepository().RateCardExcelSendEmail(status.CustomerPODSettingId, status.UserId, CommunicateWebApi.GetExcelPath(status.UserId));
                                                        }
                                                        else if (status.IsPdf == true)
                                                        {
                                                            new ReportSettingRepository().RateCardPdfSendEmail(status.CustomerPODSettingId, status.UserId, CommunicateWebApi.GetPDFPath(status.UserId));
                                                        }
                                                        new ReportSettingRepository().UpdateReportSettingTime_Date(status.CustomerPODSettingId, current.AddDays(1), ts);
                                                        //}
                                                    }
                                                }
                                            }
                                            break;
                                        case FrayteReportSettingDays.Weekly:
                                            if (status.UpdatedOn != null)
                                            {
                                                Lastdate = status.UpdatedOn.Value.AddDays(7);
                                                if (current == Lastdate && Time >= status.ScheduleTime)
                                                {
                                                    //foreach (var courier in status.fryateUserSettingDetail)
                                                    //{
                                                    if (current == Lastdate && Time >= status.ScheduleTime)
                                                    {
                                                        if (status.IsPdf == true && status.IsExcel == true)
                                                        {
                                                            new ReportSettingRepository().RateCardPdfSendEmail(status.CustomerPODSettingId, status.UserId, CommunicateWebApi.GetPDFPath(status.UserId));
                                                            new ReportSettingRepository().RateCardExcelSendEmail(status.CustomerPODSettingId, status.UserId, CommunicateWebApi.GetExcelPath(status.UserId));
                                                        }
                                                        else if (status.IsExcel == true)
                                                        {
                                                            new ReportSettingRepository().RateCardExcelSendEmail(status.CustomerPODSettingId, status.UserId, CommunicateWebApi.GetExcelPath(status.UserId));
                                                        }
                                                        else if (status.IsPdf == true)
                                                        {
                                                            new ReportSettingRepository().RateCardPdfSendEmail(status.CustomerPODSettingId, status.UserId, CommunicateWebApi.GetPDFPath(status.UserId));
                                                        }
                                                        new ReportSettingRepository().UpdateReportSettingDate(status.CustomerPODSettingId, Lastdate);
                                                    }
                                                    else if (current.Date > status.CreatedOn && Time == status.ScheduleTime)
                                                    {
                                                        if (status.IsPdf == true && status.IsExcel == true)
                                                        {
                                                            new ReportSettingRepository().RateCardPdfSendEmail(status.CustomerPODSettingId, status.UserId, CommunicateWebApi.GetPDFPath(status.UserId));
                                                            new ReportSettingRepository().RateCardExcelSendEmail(status.CustomerPODSettingId, status.UserId, CommunicateWebApi.GetExcelPath(status.UserId));
                                                        }
                                                        else if (status.IsExcel == true)
                                                        {
                                                            new ReportSettingRepository().RateCardExcelSendEmail(status.CustomerPODSettingId, status.UserId, CommunicateWebApi.GetExcelPath(status.UserId));
                                                        }
                                                        else if (status.IsPdf == true)
                                                        {
                                                            new ReportSettingRepository().RateCardPdfSendEmail(status.CustomerPODSettingId, status.UserId, CommunicateWebApi.GetPDFPath(status.UserId));
                                                        }
                                                        new ReportSettingRepository().UpdateReportSettingTime_Date(status.CustomerPODSettingId, current.AddDays(7), ts);
                                                    }
                                                    //}
                                                }
                                            }
                                            break;
                                        case FrayteReportSettingDays.Monthly:
                                            if (status.UpdatedOn != null)
                                            {
                                                Lastdate = status.UpdatedOn.Value.AddMonths(1);
                                                if (current == Lastdate && Time >= status.ScheduleTime)
                                                {
                                                    //foreach (var courier in status.fryateUserSettingDetail)
                                                    //{
                                                    if (current == Lastdate && Time >= status.ScheduleTime)
                                                    {

                                                        if (status.IsPdf == true && status.IsExcel == true)
                                                        {
                                                            new ReportSettingRepository().RateCardPdfSendEmail(status.CustomerPODSettingId, status.UserId, CommunicateWebApi.GetPDFPath(status.UserId));
                                                            new ReportSettingRepository().RateCardExcelSendEmail(status.CustomerPODSettingId, status.UserId, CommunicateWebApi.GetExcelPath(status.UserId));
                                                        }
                                                        else if (status.IsExcel == true)
                                                        {
                                                            new ReportSettingRepository().RateCardExcelSendEmail(status.CustomerPODSettingId, status.UserId, CommunicateWebApi.GetExcelPath(status.UserId));
                                                        }
                                                        else if (status.IsPdf == true)
                                                        {
                                                            new ReportSettingRepository().RateCardPdfSendEmail(status.CustomerPODSettingId, status.UserId, CommunicateWebApi.GetPDFPath(status.UserId));
                                                        }
                                                        new ReportSettingRepository().UpdateReportSettingDate(status.CustomerPODSettingId, Lastdate);
                                                    }
                                                    else if (current.Date > status.CreatedOn && Time == status.ScheduleTime)
                                                    {
                                                        if (status.IsPdf == true && status.IsExcel == true)
                                                        {
                                                            new ReportSettingRepository().RateCardPdfSendEmail(status.CustomerPODSettingId, status.UserId, CommunicateWebApi.GetPDFPath(status.UserId));
                                                            new ReportSettingRepository().RateCardExcelSendEmail(status.CustomerPODSettingId, status.UserId, CommunicateWebApi.GetExcelPath(status.UserId));
                                                        }
                                                        else if (status.IsExcel == true)
                                                        {
                                                            new ReportSettingRepository().RateCardExcelSendEmail(status.CustomerPODSettingId, status.UserId, CommunicateWebApi.GetExcelPath(status.UserId));
                                                        }
                                                        else if (status.IsPdf == true)
                                                        {
                                                            new ReportSettingRepository().RateCardPdfSendEmail(status.CustomerPODSettingId, status.UserId, CommunicateWebApi.GetPDFPath(status.UserId));
                                                        }
                                                        new ReportSettingRepository().UpdateReportSettingTime_Date(status.CustomerPODSettingId, current.AddDays(1), ts);
                                                    }
                                                    //}
                                                }
                                            }
                                            break;
                                        case FrayteReportSettingDays.Yearly:
                                            if (status.UpdatedOn != null)
                                            {
                                                Lastdate = status.UpdatedOn.Value.AddYears(1);
                                                if (current == Lastdate && Time >= status.ScheduleTime)
                                                {
                                                    //foreach (var courier in status.fryateUserSettingDetail)
                                                    //{
                                                    if (current == Lastdate && Time >= status.ScheduleTime)
                                                    {
                                                        if (status.IsPdf == true && status.IsExcel == true)
                                                        {
                                                            new ReportSettingRepository().RateCardPdfSendEmail(status.CustomerPODSettingId, status.UserId, CommunicateWebApi.GetPDFPath(status.UserId));
                                                            new ReportSettingRepository().RateCardExcelSendEmail(status.CustomerPODSettingId, status.UserId, CommunicateWebApi.GetExcelPath(status.UserId));
                                                        }
                                                        else if (status.IsExcel == true)
                                                        {
                                                            new ReportSettingRepository().RateCardExcelSendEmail(status.CustomerPODSettingId, status.UserId, CommunicateWebApi.GetExcelPath(status.UserId));
                                                        }
                                                        else if (status.IsPdf == true)
                                                        {
                                                            new ReportSettingRepository().RateCardPdfSendEmail(status.CustomerPODSettingId, status.UserId, CommunicateWebApi.GetPDFPath(status.UserId));
                                                        }
                                                        new ReportSettingRepository().UpdateReportSettingDate(status.CustomerPODSettingId, Lastdate);
                                                    }
                                                    else if (current.Date > status.CreatedOn && Time == status.ScheduleTime)
                                                    {
                                                        if (status.IsPdf == true && status.IsExcel == true)
                                                        {
                                                            new ReportSettingRepository().RateCardPdfSendEmail(status.CustomerPODSettingId, status.UserId, CommunicateWebApi.GetPDFPath(status.UserId));
                                                            new ReportSettingRepository().RateCardExcelSendEmail(status.CustomerPODSettingId, status.UserId, CommunicateWebApi.GetExcelPath(status.UserId));
                                                        }
                                                        else if (status.IsExcel == true)
                                                        {
                                                            new ReportSettingRepository().RateCardExcelSendEmail(status.CustomerPODSettingId, status.UserId, CommunicateWebApi.GetExcelPath(status.UserId));
                                                        }
                                                        else if (status.IsPdf == true)
                                                        {
                                                            new ReportSettingRepository().RateCardPdfSendEmail(status.CustomerPODSettingId, status.UserId, CommunicateWebApi.GetPDFPath(status.UserId));
                                                        }
                                                        new ReportSettingRepository().UpdateReportSettingTime_Date(status.CustomerPODSettingId, current.AddDays(1), ts);
                                                    }
                                                    //}
                                                }
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SendExceptionMail(ex);
            }
        }

        public void UpdateDirectShipment()
        {
            try
            {
                var track = new ShipmentRepository().GetParcelHubStatus();
                if (track != null)
                {
                    for (int i = 0; i < track.Count; i++)
                    {
                        if (track[i].TrackingDetails.Where(p => p.Activity == FrayteUpdateDirectShipmentStatus.Delivered).ToList().Count > 0)
                        {
                            int DirectId = track[i].DirectShipmentId;
                            if (DirectId > 0)
                            {
                                new ShipmentRepository().UpdateDirectShipmentStatus(DirectId, (int)FrayteShipmentStatus.Past, track[i].SignedBy, track[i].UpdatedAtDate, track[i].UpdatedAtTime);

                                //Write Mail Sending Logic Here
                                new ShipmentEmailRepository().SendDirectBookingDeliveryMail(DirectId);
                            }
                        }
                        else if (track[i].TrackingDetails.Where(p => p.Activity == FrayteUpdateDirectShipmentStatus.Delayed).ToList().Count > 0)
                        {
                            int DirectId = track[i].DirectShipmentId;
                            if (DirectId > 0)
                            {
                                new ShipmentRepository().UpdateDirectShipmentStatus(DirectId, (int)FrayteShipmentStatus.Delayed);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SendExceptionMail(ex);
            }
        }
        
        #endregion
    }
}
