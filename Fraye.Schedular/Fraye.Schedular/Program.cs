using Frayte.Services.Models;
using Frayte.Services.Business;
using Frayte.Schedular.Utility;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using XStreamline.Log;
using Report.Generator.ManifestReport;
using System.Threading.Tasks;

namespace Frayte.Schedular
{
    class Program
    {
        static TimeSpan Time; static DateTime Lastdate, current;

        static void Main(string[] args)
        {
            string xsUserFolder = @"" + AppSettings.ErrorLoger + "FrayteSchedularlog.txt";
            BaseLog.Instance.SetLogFile(xsUserFolder);

            //Get a logger instance --Start
            Logger _log = Get_Log();
            _log.Info("Frayte schedular application start date and time: " + DateTime.UtcNow);

            Console.WriteLine("Welcome to schedular");
            Console.WriteLine("Please Wait...");

            Console.WriteLine("\n");

            //Send invoice email 
            //sendInvoiceEmail(FrayteShipmentServiceType.eCommerce);

            //Send Fuel SurCharge And Currency Mail
            //SendFuelSurchargeAndCurrencyEmail();
            //Console.WriteLine("Fuel Surcharge And Currency Update Alert Mail Sent At : " + DateTime.Now.ToString("HH:MM"));

            //Console.WriteLine("\n");

            //Update Exchage Rate History
            //UpdateExchangeRateHistory();
            //Console.WriteLine("Exchange Rate Mail Sent At : " + DateTime.Now.ToString("HH:MM"));

            //Console.WriteLine("\n");

            //Send Customer Rate Card Email
            //SendRateCardEmail();
            //Console.WriteLine("Customer Rate Card Mail Sent At : " + DateTime.Now.ToString("HH:MM"));

            //Console.WriteLine("\n");

            //try
            //{
            //    SendAgentConsolidatedMawbAllocationEmail();
            //}
            //catch (Exception e)
            //{
            //    _log.Info(e.Message);
            //}

            //MailSendController();
            //AddManifestRecord();
            //Console.WriteLine("Add Manifest : " + DateTime.Now.ToString("HH:MM"));

            //Console.WriteLine("\n");

            //Send POD Mail to Customer
            //SendPODMail();

            //Send consolidated mail in express

            //Consolidated POD Email
            //SendExpressPODConsolidatedMail();

            //Consolidated Dispatch Email
            SendExpressConsolidatedMail();

            //download driver manifest
            //DownloadDriverManifest();
        }

        public static Logger Get_Log()
        {
            Logger log = BaseLog.Instance.GetLogger(null);
            return log;
        }

        #region eCommerce Invoice Email

        public static void sendInvoiceEmail(string eCommerce)
        {
            try
            {
                string[] chars = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P" };

                // Need to conver date time in customer time zone
                current = Frayte.Services.CommonConversion.ConvertToDateTime(DateTime.UtcNow.ToString("MM/dd/yyyy"));
                var eCommInvoices = new eCommerceShipmentRepository().GetUnpaideCommerceInvoives();
                if (eCommInvoices != null && eCommInvoices.Count > 0)
                {
                    foreach (var item in eCommInvoices)
                    {
                        var FreeStorageDateTime = item.ETADate.AddHours(item.FreeStorageTime.TotalHours);
                        if (current > FreeStorageDateTime)
                        {
                            // Generate new invoice with free storage charge 
                            var invoices = new eCommerceShipmentRepository().GetShipmentInvoices(item.eCommerceShimentId);
                            string cha = string.Empty;
                            foreach (string x in chars)
                            {
                                if (invoices.FirstOrDefault().InvoiceRef.Contains(x))
                                {
                                    // Process...
                                    cha = x;
                                    break;
                                }
                            }

                            int pos = Array.IndexOf(chars, cha);
                            string invoiceToMake = string.Empty;
                            if (pos > -1)
                            {
                                invoiceToMake = chars[pos + 1];
                            }
                            new eCommerceShipmentRepository().GenerateNewInvoice(invoiceToMake, item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger _log = Get_Log();
                _log.Info("Error due to eCommerce Invoive");
                _log.Error(ex);
            }
            throw new NotImplementedException();
        }

        #endregion

        public static void SendFuelSurchargeAndCurrencyEmail()
        {
            try
            {
                TimeSpan ts = TimeSpan.Parse(AppSettings.FuelSurCharge);
                TimeSpan ts1 = TimeSpan.Parse(AppSettings.CustomerCurrency);
                Time = TimeSpan.Parse(DateTime.Now.ToString("HH:mm"));
                current = Frayte.Services.CommonConversion.ConvertToDateTime(DateTime.UtcNow.ToString("MM/dd/yyyy"));
                DateTime comparedate = Frayte.Services.CommonConversion.ConvertToDateTime(DateTime.UtcNow.Month + "/23/" + DateTime.UtcNow.Year);

                for (int i = 1; i < 3; i++)
                {
                    FrayteStatus mailstatus = new FuelSurChargeRepository().GetSendMailStatus(DateTime.UtcNow, i);
                    var setting = new ReportSettingRepository().GetAllUserDetail();
                    if (setting != null)
                    {
                        foreach (var status in setting)
                        {
                            if (status.IsFuelSurCharge == true)
                            {
                                if (Time >= ts && comparedate == current)
                                {
                                    if (mailstatus.FuelMailSentOn != current)
                                    {
                                        if (i == 1)
                                        {
                                            FrayteStatus updatestatus = new FuelSurChargeRepository().UpdateStatus(DateTime.UtcNow, i);
                                            if (updatestatus.IsFuelSurCharge == false)
                                            {
                                                new ReportSettingRepository().FuelSurChargeSendEmail(status.Email, Frayte.Schedular.Utility.AppSettings.HKEmail, status.UserName, "", i);
                                            }
                                        }
                                        else if (i == 2)
                                        {
                                            FrayteStatus updatestatus = new FuelSurChargeRepository().UpdateStatus(DateTime.UtcNow, i);
                                            if (updatestatus.IsFuelSurCharge == false)
                                            {
                                                new ReportSettingRepository().FuelSurChargeSendEmail(status.Email, Frayte.Schedular.Utility.AppSettings.UKEmail, status.UserName, "", i);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                for (int j = 1; j < 3; j++)
                {
                    FrayteStatus mailstatus = new ExchangeRateRepository().GetSendMailStatus(j);
                    var setting = new ReportSettingRepository().GetAllUserDetail();
                    if (setting != null)
                    {
                        foreach (var status in setting)
                        {
                            if (status.IsCurrecy == true)
                            {
                                if (Time >= ts1)
                                {
                                    if (j == 1)
                                    {
                                        if (mailstatus.CurrencyMailSentOn != current)
                                        {
                                            new ReportSettingRepository().ExchangeRateSendEmail(status.Email, "", status.UserName, "", j);
                                        }
                                    }
                                    else if (j == 2)
                                    {
                                        if (mailstatus.CurrencyMailSentOn != current)
                                        {
                                            new ReportSettingRepository().ExchangeRateSendEmail(status.Email, "", status.UserName, "", j);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger _log = Get_Log();
                _log.Info("Error due to fuel sur charge and exchange rate");
                _log.Error(ex);
            }
        }

        public static void UpdateExchangeRateHistory()
        {
            try
            {
                TimeSpan ts = TimeSpan.Parse(AppSettings.CustomerCurrency);
                Time = TimeSpan.Parse(DateTime.Now.ToString("HH:mm"));
                if (Time >= ts)
                {
                    FrayteStatus mailstatus = new ExchangeRateRepository().ExchangeRateHistoryUpdateStatus();
                    if (mailstatus.HistoryStatus == false)
                    {
                        new ExchangeRateRepository().SaveExchnageRateHistory();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger _log = Get_Log();
                _log.Info("Error due to update exchange rate");
                _log.Error(ex);
            }
        }

        public static void SendRateCardEmail()
        {
            try
            {
                var setting = new ReportSettingRepository().GetAllReportSettings().Where(p => p.ScheduleSettingType == FrayteCustomerMailSetting.RateCard && p.IsPdf == true || p.IsExcel == true).ToList();

                foreach (var status in setting)
                {
                    if (status.ScheduleSettingType == FrayteCustomerMailSetting.RateCard)
                    {
                        if (status.fryateUserDetail.Count > 0)
                        {
                            if (status.CustomerPODSettingId > 0)
                            {
                                TimeSpan ts = TimeSpan.Parse(AppSettings.RateCard);
                                Time = DateTime.Now.TimeOfDay;
                                current = Frayte.Services.CommonConversion.ConvertToDateTime(DateTime.Now.ToString("MM/dd/yyyy"));

                                Logger _log = Get_Log();
                                _log.Info(setting.Count.ToString() + "\n " + status.UserId + " " + status.ScheduleSettingType + "\n" + status.fryateUserDetail.Count.ToString() + "\n" + ts + " " + Time + " " + current);

                                switch (status.PODScheduleSetting)
                                {
                                    //Check PODScheduleSetting is Scheduled
                                    case FrayteReportSettingDays.Scheduled:
                                        switch (status.ScheduleType)
                                        {
                                            case FrayteReportSettingDays.Daily:
                                                if (status.UpdatedOn != null)
                                                {
                                                    Lastdate = Frayte.Services.CommonConversion.ConvertToDateTime(status.UpdatedOn.Value.AddDays(1).ToString("MM/dd/yyyy"));
                                                    if (current.Date == Lastdate.Date && Time >= status.ScheduleTime)
                                                    {
                                                        if (current.Date == Lastdate.Date && Time >= status.ScheduleTime)
                                                        {
                                                            if (status.IsPdf == true && status.IsExcel == true)
                                                            {
                                                                SendPdfRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                                SendExcelRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                            }
                                                            else if (status.IsExcel == true)
                                                            {
                                                                SendExcelRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                            }
                                                            else if (status.IsPdf == true)
                                                            {
                                                                SendPdfRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                            }
                                                            new ReportSettingRepository().UpdateReportSettingDate(status.CustomerPODSettingId, (current.Date > Lastdate ? current.Date : Lastdate));
                                                        }
                                                        else if (current.Date > status.CreatedOn.Date && Time >= status.ScheduleTime)
                                                        {
                                                            if (status.IsPdf == true && status.IsExcel == true)
                                                            {
                                                                SendPdfRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                                SendExcelRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                            }
                                                            else if (status.IsExcel == true)
                                                            {
                                                                SendExcelRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                            }
                                                            else if (status.IsPdf == true)
                                                            {
                                                                SendPdfRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                            }
                                                            new ReportSettingRepository().UpdateReportSettingTime_Date(status.CustomerPODSettingId, current.AddDays(1), ts);
                                                        }
                                                    }
                                                    else if (current.Date > Lastdate.Date && Time >= status.ScheduleTime)
                                                    {
                                                        if (status.IsPdf == true && status.IsExcel == true)
                                                        {
                                                            SendPdfRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                            SendExcelRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                        }
                                                        else if (status.IsExcel == true)
                                                        {
                                                            SendExcelRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                        }
                                                        else if (status.IsPdf == true)
                                                        {
                                                            SendPdfRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                        }
                                                        new ReportSettingRepository().UpdateReportSettingDate(status.CustomerPODSettingId, (current.Date > Lastdate ? current.Date : Lastdate));
                                                    }
                                                }
                                                break;
                                            case FrayteReportSettingDays.Weekly:
                                                if (status.UpdatedOn != null)
                                                {
                                                    Lastdate = Frayte.Services.CommonConversion.ConvertToDateTime(status.UpdatedOn.Value.AddDays(7).ToString("MM/dd/yyyy"));
                                                    if (current.Date == Lastdate.Date && Time >= status.ScheduleTime)
                                                    {
                                                        if (current.Date == Lastdate.Date && Time >= status.ScheduleTime)
                                                        {
                                                            if (status.IsPdf == true && status.IsExcel == true)
                                                            {
                                                                SendPdfRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                                SendExcelRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                            }
                                                            else if (status.IsExcel == true)
                                                            {
                                                                SendExcelRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                            }
                                                            else if (status.IsPdf == true)
                                                            {
                                                                SendPdfRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                            }
                                                            new ReportSettingRepository().UpdateReportSettingDate(status.CustomerPODSettingId, (current.Date > Lastdate ? current.Date : Lastdate));
                                                        }
                                                        else if (current.Date > status.CreatedOn.Date && Time >= status.ScheduleTime)
                                                        {
                                                            if (status.IsPdf == true && status.IsExcel == true)
                                                            {
                                                                SendPdfRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                                SendExcelRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                            }
                                                            else if (status.IsExcel == true)
                                                            {
                                                                SendExcelRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                            }
                                                            else if (status.IsPdf == true)
                                                            {
                                                                SendPdfRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                            }
                                                            new ReportSettingRepository().UpdateReportSettingTime_Date(status.CustomerPODSettingId, current.AddDays(7), ts);
                                                        }
                                                    }
                                                    else if (current.Date > status.CreatedOn.Date && Time >= status.ScheduleTime)
                                                    {
                                                        if (status.IsPdf == true && status.IsExcel == true)
                                                        {
                                                            SendPdfRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                            SendExcelRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                        }
                                                        else if (status.IsExcel == true)
                                                        {
                                                            SendExcelRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                        }
                                                        else if (status.IsPdf == true)
                                                        {
                                                            SendPdfRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                        }
                                                        new ReportSettingRepository().UpdateReportSettingDate(status.CustomerPODSettingId, (current.Date > Lastdate ? current.Date : Lastdate));
                                                    }
                                                }
                                                break;
                                            case FrayteReportSettingDays.Monthly:
                                                if (status.UpdatedOn != null)
                                                {
                                                    Lastdate = Frayte.Services.CommonConversion.ConvertToDateTime(status.UpdatedOn.Value.AddMonths(1).ToString("MM/dd/yyyyy"));
                                                    if (current.Date == Lastdate.Date && Time >= status.ScheduleTime)
                                                    {
                                                        if (current.Date == Lastdate.Date && Time >= status.ScheduleTime)
                                                        {
                                                            if (status.IsPdf == true && status.IsExcel == true)
                                                            {
                                                                SendPdfRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                                SendExcelRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                            }
                                                            else if (status.IsExcel == true)
                                                            {
                                                                SendExcelRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                            }
                                                            else if (status.IsPdf == true)
                                                            {
                                                                SendPdfRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                            }
                                                            new ReportSettingRepository().UpdateReportSettingDate(status.CustomerPODSettingId, (current.Date > Lastdate ? current.Date : Lastdate));
                                                        }
                                                        else if (current.Date > status.CreatedOn.Date && Time == status.ScheduleTime)
                                                        {
                                                            if (status.IsPdf == true && status.IsExcel == true)
                                                            {
                                                                SendPdfRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                                SendExcelRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                            }
                                                            else if (status.IsExcel == true)
                                                            {
                                                                SendExcelRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                            }
                                                            else if (status.IsPdf == true)
                                                            {
                                                                SendPdfRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                            }
                                                            new ReportSettingRepository().UpdateReportSettingTime_Date(status.CustomerPODSettingId, current.AddDays(1), ts);
                                                        }
                                                    }
                                                    else if (current.Date > status.CreatedOn.Date && Time >= status.ScheduleTime)
                                                    {
                                                        if (status.IsPdf == true && status.IsExcel == true)
                                                        {
                                                            SendPdfRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                            SendExcelRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                        }
                                                        else if (status.IsExcel == true)
                                                        {
                                                            SendExcelRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                        }
                                                        else if (status.IsPdf == true)
                                                        {
                                                            SendPdfRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                        }
                                                        new ReportSettingRepository().UpdateReportSettingDate(status.CustomerPODSettingId, (current.Date > Lastdate ? current.Date : Lastdate));
                                                    }
                                                }
                                                break;
                                            case FrayteReportSettingDays.Yearly:
                                                if (status.UpdatedOn != null)
                                                {
                                                    Lastdate = Frayte.Services.CommonConversion.ConvertToDateTime(status.UpdatedOn.Value.AddYears(1).ToString("MM/dd/yyyy"));
                                                    if (current.Date == Lastdate.Date && Time >= status.ScheduleTime)
                                                    {
                                                        if (current.Date == Lastdate.Date && Time >= status.ScheduleTime)
                                                        {
                                                            if (status.IsPdf == true && status.IsExcel == true)
                                                            {
                                                                SendPdfRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                                SendExcelRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                            }
                                                            else if (status.IsExcel == true)
                                                            {
                                                                SendExcelRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                            }
                                                            else if (status.IsPdf == true)
                                                            {
                                                                SendPdfRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                            }
                                                            new ReportSettingRepository().UpdateReportSettingDate(status.CustomerPODSettingId, (current.Date > Lastdate ? current.Date : Lastdate));
                                                        }
                                                        else if (current.Date > status.CreatedOn.Date && Time == status.ScheduleTime)
                                                        {
                                                            if (status.IsPdf == true && status.IsExcel == true)
                                                            {
                                                                SendPdfRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                                SendExcelRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                            }
                                                            else if (status.IsExcel == true)
                                                            {
                                                                SendExcelRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                            }
                                                            else if (status.IsPdf == true)
                                                            {
                                                                SendPdfRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                            }
                                                            new ReportSettingRepository().UpdateReportSettingTime_Date(status.CustomerPODSettingId, current.AddDays(1), ts);
                                                        }
                                                    }
                                                    else if (current.Date > status.CreatedOn.Date && Time >= status.ScheduleTime)
                                                    {
                                                        if (status.IsPdf == true && status.IsExcel == true)
                                                        {
                                                            SendPdfRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                            SendExcelRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                        }
                                                        else if (status.IsExcel == true)
                                                        {
                                                            SendExcelRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                        }
                                                        else if (status.IsPdf == true)
                                                        {
                                                            SendPdfRateCardMail(status.fryateUserSettingDetail, status.UserId, status.fryateUserDetail[0].UserName, status.fryateUserDetail[0].OperationZoneId);
                                                        }
                                                        new ReportSettingRepository().UpdateReportSettingDate(status.CustomerPODSettingId, (current.Date > Lastdate ? current.Date : Lastdate));
                                                    }
                                                }
                                                break;
                                            default:
                                                break;
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger _log = Get_Log();
                _log.Info("Error due to send customer rate card pdf and excel");
                _log.Error(e.Message);
            }
        }

        public static void AddManifestRecord()
        {
            try
            {
                DateTime current = Frayte.Services.CommonConversion.ConvertToDateTime(DateTime.UtcNow.ToString("MM/dd/yyyy"));
                TimeSpan FromTime = TimeSpan.Parse(DateTime.Now.AddHours(1).ToString("HH:00:00"));
                TimeSpan ToTime = TimeSpan.Parse(DateTime.Now.AddHours(1).ToString("HH:59:59"));
                new eCommerceCustomManifestRepository().GetMainfestData(current, FromTime, ToTime);
                new eCommerceCustomManifestRepository().GetManifestDetailFromExcel();
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
        }

        public static void MailSendController()
        {
            try
            {
                //Array res = new Array[2];

                if (AppSettings.EmailSendTime != "")
                {
                    var res = AppSettings.EmailSendTime.Split(':');
                    var res1 = Convert.ToInt32(res[0]);
                    if (res1 > 0)
                    {
                        var res2 = Convert.ToInt32(res[1]);
                        var MailsendTime = new DateTime(2017, 03, 10, res1, res2, 00, DateTimeKind.Local);

                        if (MailsendTime.TimeOfDay.Hours == DateTime.Now.TimeOfDay.Hours && MailsendTime.TimeOfDay.Minutes == DateTime.Now.TimeOfDay.Minutes)
                        {
                            Logger _log = Get_Log();
                            _log.Info("enter Unpaid Shipments section");
                            _log.Error(MailsendTime.TimeOfDay.Hours.ToString() + " " + DateTime.Now.TimeOfDay.Hours.ToString());
                            //new eCommerceControlDeptRepository().GetUnpaidShipments();
                            var result = new ControlDeptGenerateExcel().GenerateManifestReport();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger _log = Get_Log();
                _log.Info("Error due to Unpaid Shipments");
                _log.Error(e.Message);
            }
        }

        public static void SendAgentConsolidatedMawbAllocationEmail()
        {
            try
            {
                var Result = new MawbAllocationRepository().GetMawbUnallocatedShipments();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static void SendExpressConsolidatedMail()
        {
            try
            {
                var Result = new ExpresShipmentRepository().SendExpressConsolidatedMail();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static void SendExpressPODConsolidatedMail()
        {
            try
            {
                TimeSpan ts = TimeSpan.Parse(AppSettings.FuelSurCharge);
                Time = TimeSpan.Parse(DateTime.Now.ToString("HH:mm"));
                if (Time >= ts)
                {
                    var Result = new ExpresShipmentRepository().SendExpressPODConsolidatedMail();
                }
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }

        public static void DownloadDriverManifest()
        {
            try
            {

                var ExpressIdList = new ExpresShipmentRepository().DownloadDriverManifest();
                if (ExpressIdList.Count > 0)
                {
                    foreach (var Ex in ExpressIdList)
                    {
                        new TradelaneDocument().ShipmentDriverManifest(Ex.TradelaneShipmentId, Ex.UserId);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        internal static void SendPdfRateCardMail(List<FryateUserSettingDetail> _customerlogistics, int UserId, string UserName, int OperationZoneId)
        {
            try
            {
                int n = 0;
                CustomerRate rate;
                List<string> filepath = new List<string>();
                List<string> filename = new List<string>();
                foreach (var Obj in _customerlogistics)
                {
                    rate = new CustomerRate();
                    rate.UserId = UserId;
                    rate.CustomerName = UserName;
                    rate.LogisticServiceId = Obj.LogisticService.LogisticServiceId;
                    rate.LogisticType = Obj.LogisticService.LogisticType;
                    rate.RateType = Obj.LogisticService.RateType;
                    rate.FileType = FrayteCustomerBaseRateFileType.Pdf;
                    FrayteManifestName file = new CustomerBaseRateReport().CustomerBaseRate(rate);

                    filepath.Add(file.FilePath);
                    filename.Add(file.FileName);
                    n++;
                }

                //Sened Mail
                if (filepath.Count > 0)
                {
                    FrayteResult result = new ShipmentEmailRepository().EmailCustomerRateCard(UserId, filename, filepath, OperationZoneId);
                    Logger _log = Get_Log();
                    _log.Info("(" + n + ") " + filepath.Count + "\n");
                    _log.Error(result.Status.ToString() + "\n" + result.Errors);
                }
            }
            catch (Exception ex)
            {
                Logger _log = Get_Log();
                _log.Info("Error sending customer rate card pdf " + ex.Message);
                _log.Error(ex);
            }
        }

        internal static void SendExcelRateCardMail(List<FryateUserSettingDetail> _customerlogistics, int UserId, string UserName, int OperationZoneId)
        {
            try
            {
                int n = 0;
                CustomerRate rate;
                List<string> filepath = new List<string>();
                List<string> filename = new List<string>();
                foreach (var Obj in _customerlogistics)
                {
                    rate = new CustomerRate();
                    rate.UserId = UserId;
                    rate.CustomerName = UserName;
                    rate.LogisticServiceId = Obj.LogisticService.LogisticServiceId;
                    rate.LogisticType = Obj.LogisticService.LogisticType;
                    rate.RateType = Obj.LogisticService.RateType;
                    rate.FileType = FrayteCustomerBaseRateFileType.Excel;
                    FrayteManifestName file = new CustomerBaseRateReport().CustomerBaseRate(rate);

                    filepath.Add(file.FilePath);
                    filename.Add(file.FileName);
                    n++;
                }

                //Sened Mail
                if (filepath.Count > 0)
                {
                    FrayteResult result = new ShipmentEmailRepository().EmailCustomerRateCard(UserId, filename, filepath, OperationZoneId);
                    Logger _log = Get_Log();
                    _log.Info("(" + n + ") " + filepath.Count + "\n");
                    _log.Error(result.Status.ToString() + "\n" + result.Errors);
                }
            }
            catch (Exception ex)
            {
                Logger _log = Get_Log();
                _log.Info("Error sending customer rate card excel " + ex.Message);
                _log.Error(ex);
            }
        }

        internal static void SendPODMail()
        {
            try
            {
                var setting = new ReportSettingRepository().GetAllReportSettings().Where(p => p.PODScheduleSetting == FrayteCustomerMailSetting.Scheduled && p.ScheduleSettingType == FrayteCustomerMailSetting.POD).ToList();
                if (setting.Count > 0)
                {
                    foreach (FryateReportSetting report in setting)
                    {
                        //Get server date in utc standard
                        DateTime serverdate = UtilityRepository.ConvertDateTimetoUniversalTime(DateTime.Now).Date;
                        //Get server time in HH:mm format
                        TimeSpan servertime = TimeSpan.Parse(DateTime.Now.ToString("HH:mm"));
                        DateTime scheduledate = new DateTime(report.ScheduleDate.Year, report.ScheduleDate.Month, report.ScheduleDate.Day);
                        if (report.ScheduleTime <= servertime)
                        {
                            if (report.UpdatedOn == null || report.UpdatedOn.Value.Date < DateTime.Now.Date)
                            {
                                UpdateDirectShipmentPOD(report.UserId);

                                //Update Customer Setting UpdateOn information
                                new ReportSettingRepository().UpdateCustomerSettingInformation(report.CustomerPODSettingId, report.UserId);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger _log = Get_Log();
                _log.Info("Error due to send customer pod information mail");
                _log.Error(ex.Message);
            }
        }

        internal static void UpdateDirectShipmentPOD(int UserId)
        {
            try
            {
                new DirectShipmentRepository().UpdatePODMailInformation(UserId);
            }
            catch (Exception ex)
            {
                Logger _log = Get_Log();
                _log.Info("Error due to update pod information and Sending mail");
                _log.Error(ex.Message);
            }
        }
    }
}
