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



namespace frayteWinApp
{
    public partial class Form1 : Form
    {
        #region Variable decalration

        string DevelopmentTeamEmail, OwnerMail;

        double workingTimeHour = 0;

        DateTime lastdatetime;

        bool workingfine, flag, formflag;

        #endregion

        #region Constructor
        public Form1()
        {

            InitializeComponent();

            SetGlobalField();

            new ShipmentEmailRepository().SendMail(DevelopmentTeamEmail + ";" + OwnerMail, "", "Windows service is alive", "Windows service is alive and working fine");

            flag = true;
            formflag = true;

            timer1 = new Timer();
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Interval = 1000;
            timer1.Enabled = true;

        }

        #endregion

        #region Events

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (flag)
            {
                setWorkingMailHours();

                if (workingTimeHour >= 1)
                {
                    #region send mail to development team and owner of the product windows service working fine.

                    lastdatetime = DateTime.Now;
                    workingTimeHour = 0;

                    string EmailBody = "<br/><br/>" + "Dear Sir" + "<br/><br/>" + "Window Service is working fine" + "<br/><br/>";
                    new ShipmentEmailRepository().SendMail(DevelopmentTeamEmail + ";" + OwnerMail, "", "Window Service is working fine", EmailBody);

                    #endregion
                }

                flag = false;

                #region Send email

                var _shipmentdetail = new ShipmentEmailRepository().GetShipmentEmailDetail();

                foreach (var shd in _shipmentdetail)
                {
                    var tzWorkingStartTime = UtilityRepository.GetTimeZoneTimeSpan(shd.WorkingStartTime, shd.TimeZoneName);
                    var tzWorkingEndTime = UtilityRepository.GetTimeZoneTimeSpan(shd.WorkingEndTime, shd.TimeZoneName);
                    var tzEmailSentTime = UtilityRepository.GetTimeZoneTimeSpan(shd.EmailSentTime, shd.TimeZoneName);
                    var tzCurrent = UtilityRepository.GetTimeZoneTimeSpan(DateTime.UtcNow.TimeOfDay, shd.TimeZoneName);

                    TimeSpan? tzmatCurrent = new TimeSpan?();

                    if (shd.RemindType == "AFTER")
                    {
                        tzmatCurrent = UtilityRepository.GetTimeZoneTimeSpan(DateTime.UtcNow.AddHours(shd.ReminderIn).TimeOfDay, shd.TimeZoneName);
                    }
                    else
                    {
                        tzmatCurrent = UtilityRepository.GetTimeZoneTimeSpan(DateTime.UtcNow.AddHours(-shd.ReminderIn).TimeOfDay, shd.TimeZoneName);
                    }

                    try
                    {
                        if (tzCurrent >= tzWorkingStartTime && tzCurrent <= tzWorkingEndTime && tzmatCurrent >= tzEmailSentTime && !string.IsNullOrEmpty(shd.ManagerEEmailTemplateId) && shd.IsRepeatReminder == true && shd.EmailSentCount >= shd.EmailSentToManagerCount && shd.RemindType == "AFTER")
                        {
                            sendEmail(shd.ShipmentId, shd.ManagerEEmailTemplateId);
                            new ShipmentEmailRepository().SetShipmentEmailService(shd.ShipmentEmailServiceId, tzCurrent, shd.EmailSentCount + 1);
                            return;
                        }
                        else if (tzCurrent >= tzWorkingStartTime && tzCurrent <= tzWorkingEndTime && tzmatCurrent >= tzEmailSentTime && shd.IsRepeatReminder == true && shd.RemindType == "AFTER")
                        {
                            sendEmail(shd.ShipmentId, shd.EmailTemplateId);
                            new ShipmentEmailRepository().SetShipmentEmailService(shd.ShipmentEmailServiceId, tzCurrent, shd.EmailSentCount + 1);
                            return;
                        }
                        else if (tzCurrent >= tzWorkingStartTime && tzCurrent <= tzWorkingEndTime && tzmatCurrent >= tzEmailSentTime && shd.IsRepeatReminder == false && shd.RemindType == "BEFORE")
                        {
                            sendEmail(shd.ShipmentId, shd.EmailTemplateId);
                            new ShipmentEmailRepository().DeleteShipmentEmailService(shd.ShipmentEmailServiceId);
                            return;
                        }
                        //else if (tzCurrent >= tzWorkingStartTime && tzCurrent <= tzWorkingEndTime && tzCurrent >= tzEmailSentTime && shd.IsRepeatReminder == false && shd.RemindType == "BEFORE")
                        //{
                        //    sendEmail(shd.ShipmentId, shd.EmailTemplateId);
                        //    new ShipmentEmailRepository().DeleteShipmentEmailService(shd.ShipmentEmailServiceId);
                        //    return;
                        //}

                        #region comment

                        //if ((DateTime.Now.TimeOfDay - shd.EmailSentTime).Hours >= shd.ReminderIn && !string.IsNullOrEmpty(shd.ManagerEEmailTemplateId) && shd.IsRepeatReminder == true && shd.EmailSentCount >= shd.EmailSentToManagerCount && shd.RemindType == "AFTER")
                        //{
                        //    sendEmail(shd.ShipmentId, shd.ManagerEEmailTemplateId);
                        //    new ShipmentEmailRepository().SetShipmentEmailService(shd.ShipmentEmailServiceId, DateTime.Now.TimeOfDay, shd.EmailSentCount + 1);
                        //    return;
                        //}
                        //else if ((DateTime.Now.TimeOfDay - shd.EmailSentTime).Hours >= shd.ReminderIn && shd.IsRepeatReminder == true && shd.RemindType == "AFTER")
                        //{
                        //    sendEmail(shd.ShipmentId, shd.EmailTemplateId);
                        //    new ShipmentEmailRepository().SetShipmentEmailService(shd.ShipmentEmailServiceId, DateTime.Now.TimeOfDay, shd.EmailSentCount + 1);
                        //    return;
                        //}
                        //else if (DateTime.Now.AddHours(shd.ReminderIn).TimeOfDay <= shd.EmailSentTime && shd.IsRepeatReminder == false && shd.RemindType == "BEFORE")
                        //{
                        //    sendEmail(shd.ShipmentId, shd.EmailTemplateId);
                        //    new ShipmentEmailRepository().DeleteShipmentEmailService(shd.ShipmentEmailServiceId);
                        //    return;
                        //}
                        //else if ((DateTime.Now.TimeOfDay - shd.EmailSentTime).Hours >= shd.ReminderIn && shd.IsRepeatReminder == false)
                        //{
                        //    sendEmail(shd.ShipmentId, shd.EmailTemplateId);
                        //    new ShipmentEmailRepository().DeleteShipmentEmailService(shd.ShipmentEmailServiceId);
                        //    return;
                        //}

                        #endregion
                    }
                    catch (Exception ex)
                    {
                        string Content = "<br/><br/>" + ex.Message + "<br/><br/>";// +ex.StackTrace + "<br/><br/>";
                        new ShipmentEmailRepository().SendMail(DevelopmentTeamEmail, "", "Error have been gone in Frayte Logistic", Content);
                    }
                }

                #endregion

                flag = true;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (formflag)
            {
                new ShipmentEmailRepository().SendMail(DevelopmentTeamEmail + ";" + OwnerMail, "", "Window Service has been stoped", "<br/><br/>" + "Dear Sir" + "<br/><br/>" + "Window Service has been stop due to any error or fault" + "<br/><br/>");
                formflag = false;
                Application.Exit();
            }

            if (e.CloseReason == CloseReason.TaskManagerClosing)
            {
                new ShipmentEmailRepository().SendMail(DevelopmentTeamEmail + ";" + OwnerMail, "", "Window Service has been stoped", "<br/><br/>" + "Dear Sir" + "<br/><br/>" + "Window Service has been stop due to any error or fault" + "<br/><br/>");
                formflag = false;
                Application.Exit();
            }

            e.Cancel = true;
        }

        #endregion

        #region Methods

        public void sendEmail(int shipmentid, string templatetype)
        {
            switch (templatetype)
            {
                case EmailTempletType.E1:
                    //logic :
                    new ShipmentEmailRepository().SendEmail_E1(shipmentid);
                    break;
                //case EmailTempletType.E2:
                //    new ShipmentEmailRepository().SendEmail_E2(shipmentid);
                //    break;
                //case EmailTempletType.E3:
                //    new ShipmentEmailRepository().SendEmail_E3(shipmentid);
                //    break;
                //case EmailTempletType.E3_1:
                //    new ShipmentEmailRepository().SendEmail_E3_1(shipmentid);
                //    break;
                //case EmailTempletType.E4:
                //    new ShipmentEmailRepository().SendEmail_E4(shipmentid);
                //    break;
                //case EmailTempletType.E4_1:
                //    new ShipmentEmailRepository().SendEmail_E4_1(shipmentid);
                //    break;
                //case EmailTempletType.E5:
                //    new ShipmentEmailRepository().SendEmail_E5(shipmentid);
                //    break;
                //case EmailTempletType.E5_1:
                //    new ShipmentEmailRepository().SendEmail_E5_1(shipmentid);
                //    break;
                //case EmailTempletType.E6:
                //    new ShipmentEmailRepository().SendEmail_E6(shipmentid);
                //    break;
                case EmailTempletType.E6_1:
                    new ShipmentEmailRepository().SendEmail_E61(shipmentid);
                    break;
                //case EmailTempletType.E6_2:
                //    new ShipmentEmailRepository().SendEmail_E62(shipmentid);
                //    break;
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
                //case EmailTempletType.E10:
                //    new ShipmentEmailRepository().SendEmail_E10(shipmentid);
                //    break;
                //case EmailTempletType.E11:
                //    new ShipmentEmailRepository().SendEmail_E11(shipmentid);
                //    break;
                //case EmailTempletType.E12:
                //    new ShipmentEmailRepository().SendEmail_E12(shipmentid);
                //    break;
                case EmailTempletType.E13:
                    new ShipmentEmailRepository().SendEmail_E13(shipmentid);
                    break;
                //case EmailTempletType.E13_1:
                //    new ShipmentEmailRepository().SendEmail_E131(shipmentid);
                //    break;
                //case EmailTempletType.E14:
                //    new ShipmentEmailRepository().SendEmail_E14(shipmentid);
                //    break;
                //case EmailTempletType.E15:
                //    new ShipmentEmailRepository().SendEmail_E15(shipmentid);
                //    break;
                //case EmailTempletType.E16:
                //    new ShipmentEmailRepository().SendEmail_E16(shipmentid);
                //    break;
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
        }

        #endregion
    }
}
