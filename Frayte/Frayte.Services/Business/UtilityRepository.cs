using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using Frayte.Services.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web;
using System.Net;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web.Hosting;
using System.Data;
using System.Security.Cryptography;
using Frayte.Services.Models.Tradelane;

namespace Frayte.Services.Business
{
    public static class UtilityRepository
    {
        static FrayteEntities dbContext = new FrayteEntities();

        public static TimeZoneModal GetTimezoneModelDetail(int? timeZoneId)
        {
            TimeZoneModal timeZoneModel = new TimeZoneModal();

            if (timeZoneId.HasValue && timeZoneId.Value > 0)
            {
                Timezone timezone = dbContext.Timezones.Where(p => p.TimezoneId == timeZoneId).FirstOrDefault();
                if (timezone != null)
                {
                    timeZoneModel.TimezoneId = timezone.TimezoneId;
                    timeZoneModel.Name = timezone.Name;
                    timeZoneModel.Offset = timezone.Offset;
                    timeZoneModel.OffsetShort = timezone.OffsetShort;
                }
            }

            return timeZoneModel;
        }

        public static DateTime? GetWorkingTime(TimeSpan? workingTime)
        {
            DateTime workingDayTime = new DateTime();
            if (workingTime.HasValue)
            {
                workingDayTime = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month,
                              DateTime.UtcNow.Day, workingTime.Value.Hours, workingTime.Value.Minutes, workingTime.Value.Seconds);

                return workingDayTime;
            }
            else
            {
                return null;
            }
        }

        public static TimeSpan? GetWorkingTime(DateTime? workingDateTime)
        {
            if (workingDateTime.HasValue)
            {
                return workingDateTime.Value.ToUniversalTime().TimeOfDay;
            }
            else
            {
                return null;
            }
        }

        //string should be in format yyyymmdd
        public static DateTime GettDateTimeFromString(string yyyymmdd)
        {
            if (!string.IsNullOrEmpty(yyyymmdd))
            {
                String modified = yyyymmdd.Insert(4, "-");// "yyyy-mmdd"
                modified = modified.Insert(7, "-"); // "yyyy-mm-dd"
                DateTime oDate = DateTime.Parse(modified);
                return oDate;
            }
            else
            {
                return DateTime.Now;
            }
        }

        //string should be in format yyyymmdd
        public static string GetFormattedDateInMMDDYYYY(string yyyymmdd)
        {
            if (!string.IsNullOrEmpty(yyyymmdd))
            {
                return GettDateTimeFromString(yyyymmdd).ToString("MM/dd/yyyy");
            }
            else
            {
                return DateTime.Now.ToString("MM/dd/yyyy");
            }
        }

        public static DateTime GetTimeZoneCurrentDateTime(string timeZoneName)
        {
            if (!string.IsNullOrEmpty(timeZoneName))
            {
                TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneName);
                DateTime currenttimezonedatetime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZone);
                return currenttimezonedatetime;
            }
            return DateTime.Now;
        }

        public static DateTime? GetTimeZoneDateTime(DateTime? PickupDate, TimeSpan? time, string timeZoneName)
        {
            if (time != null && !string.IsNullOrEmpty(timeZoneName) && PickupDate != null)
            {
                var finalTime = new DateTime(PickupDate.Value.Year, PickupDate.Value.Month, PickupDate.Value.Day, time.Value.Hours, time.Value.Minutes, 0);

                TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneName);
                DateTime dtConverted = TimeZoneInfo.ConvertTimeFromUtc(finalTime, timeZone);

                return dtConverted;
            }
            else if (!string.IsNullOrEmpty(timeZoneName))
            {
                TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneName);
                DateTime dtConverted = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now, timeZone);
                return dtConverted;
            }
            else
            {
                return DateTime.Now;
            }
        }

        public static TimeSpan? GetTimeZoneUTCTime(string time, string timeZoneName)
        {
            if (!string.IsNullOrEmpty(time) && !string.IsNullOrEmpty(timeZoneName))
            {
                var hh = Convert.ToInt32(time.Substring(0, 2));
                var mm = Convert.ToInt32(time.Substring(2, 2));

                var finalTime = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, hh, mm, 0);

                TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneName);
                DateTime dtConverted = TimeZoneInfo.ConvertTimeToUtc(finalTime, timeZone);

                return dtConverted.TimeOfDay;
            }

            return null;
        }

        public static TimeSpan? GetTimeZoneUTCTime(TimeSpan? time, string timeZoneName)
        {

            if (time.HasValue && !string.IsNullOrEmpty(timeZoneName))
            {
                var finalTime = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, time.Value.Hours, time.Value.Minutes, 0);

                TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneName);
                DateTime dtConverted = TimeZoneInfo.ConvertTimeToUtc(finalTime, timeZone);

                return dtConverted.TimeOfDay;
            }

            return null;
        }

        public static TimeSpan? GetTimeZoneUTCTime(string time, int? timeZoneId)
        {
            if (timeZoneId.HasValue && timeZoneId.Value > 0 && !string.IsNullOrEmpty(time))
            {
                if (time.Length == 3)
                {
                    time = "0" + time;
                }
                var hh = Convert.ToInt32(time.Substring(0, 2));
                var mm = Convert.ToInt32(time.Substring(2, 2));

                var finalTime = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, hh, mm, 0);

                var timeZoneResult = dbContext.Timezones.Where(p => p.TimezoneId == timeZoneId.Value).FirstOrDefault();

                if (timeZoneResult != null)
                {
                    TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneResult.Name);

                    DateTime dtConverted = TimeZoneInfo.ConvertTimeToUtc(finalTime, timeZone);

                    return dtConverted.TimeOfDay;
                }
            }

            return null;
        }

        public static TimeSpan? GetTimeFromString(string time)
        {
            if (!string.IsNullOrEmpty(time))
            {
                var hh = Convert.ToInt32(time.Substring(0, 2));
                var mm = Convert.ToInt32(time.Substring(2, 2));

                var finalTime = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, hh, mm, 0);

                return finalTime.TimeOfDay;
            }
            else
            {
                return null;
            }
        }

        public static TimeSpan? GetTimeFromDateString(string time)
        {
            if (!string.IsNullOrEmpty(time))
            {
                //var hh = Convert.ToInt32(time.Substring(11, 2));
                var h = time.Split(' ');
                var mm = Convert.ToInt32(time.Substring(13, 2));
                var hh = Convert.ToInt32(h[1].Substring(0, 1));
                var m = Convert.ToInt32(h[1].Substring(2, 2));
                var finalTime = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, hh, m, 0);

                return finalTime.TimeOfDay;
            }
            else
            {
                return null;
            }
        }

        public static string GetFormattedTimeString(string time)
        {
            if (!string.IsNullOrEmpty(time))
            {
                string arr = time.Substring(0, 2);
                string arr1 = time.Substring(2, 2);
                return arr + ":" + arr1;
            }
            else
            {
                return "";
            }
        }

        public static string GetTimeZoneTime(TimeSpan? time, string timeZoneName)
        {
            if (time.HasValue && !string.IsNullOrEmpty(timeZoneName))
            {
                var finalTime = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, time.Value.Hours, time.Value.Minutes, 0);

                TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneName);
                DateTime dtConverted = TimeZoneInfo.ConvertTimeFromUtc(finalTime, timeZone);

                return dtConverted.Hour.ToString("00") + dtConverted.Minute.ToString("00");
            }

            return "";
        }

        public static TimeSpan? GetTimeZoneTimeSpan(TimeSpan? time, string timeZoneName)
        {
            if (time.HasValue && !string.IsNullOrEmpty(timeZoneName))
            {
                var finalTime = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, time.Value.Hours, time.Value.Minutes, 0);

                TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneName);
                DateTime dtConverted = TimeZoneInfo.ConvertTimeFromUtc(finalTime, timeZone);

                return dtConverted.TimeOfDay;
            }

            return null;
        }

        public static string GetTimeZoneTime(TimeSpan? time, int? timeZoneId)
        {
            if (time.HasValue && timeZoneId.HasValue && timeZoneId.Value > 0)
            {
                var timeZoneResult = dbContext.Timezones.Where(p => p.TimezoneId == timeZoneId).FirstOrDefault();

                if (timeZoneResult != null)
                {
                    var finalTime = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, time.Value.Hours, time.Value.Minutes, 0);

                    TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneResult.Name);
                    DateTime dtConverted = TimeZoneInfo.ConvertTimeFromUtc(finalTime, timeZone);

                    return dtConverted.Hour.ToString("00") + dtConverted.Minute.ToString("00");
                }
            }

            return "";
        }

        public static string GetFlatTimeString(string time)
        {
            if (!string.IsNullOrEmpty(time))
            {
                var str = time.Split(':');
                if (str.Length > 0)
                {
                    return str[0] + str[1];
                }
                return "";
            }
            else
            {
                return "";
            }
        }

        public static string GetTimeZoneTime(TimeSpan? time)
        {
            if (time.HasValue)
            {
                return time.Value.Hours.ToString("00") + time.Value.Minutes.ToString("00");
            }
            else
            {
                return "";
            }
        }

        public static FrayteLogisticIntegration getLogisticIntegration(int operationZoneId, string mode, string integrationName)
        {
            var data = dbContext.LogisticIntegrations.Where(p => p.OperationZoneId == operationZoneId && p.Mode == mode && p.Name == integrationName).FirstOrDefault();
            if (data != null)
            {
                FrayteLogisticIntegration detail = new FrayteLogisticIntegration();
                detail.AppId = data.AppId;
                detail.AppVersion = data.AppVersion;
                detail.LogisticIntegrationId = data.Id;
                detail.UserName = data.UserName;
                detail.Password = data.Password;
                detail.IntegrationName = data.Name;
                detail.Mode = data.Mode;
                detail.ServiceUrl = data.Url;
                detail.UserAgent = data.UserAgent;
                detail.InetgrationKey = data.InetgrationKey;
                detail.VoidApiUrl = data.VoidApiUrl;
                detail.PickupApiUrl = data.PickupApiUrl;
                detail.LabelApiUrl = data.LabelApiUrl;
                return detail;
            }
            else
            {
                return null;
            }
        }

        public static FrayteUser UserMapping(User user)
        {
            FrayteUser frayteUser = new FrayteUser();
            frayteUser.UserId = user.UserId;
            frayteUser.CargoWiseId = user.CargoWiseId;
            frayteUser.CargoWiseBardCode = user.CargoWiseBardCode;
            frayteUser.CompanyName = user.CompanyName;
            frayteUser.ClientId = user.ClientId;
            frayteUser.IsClient = user.IsClient;
            frayteUser.CountryOfOperation = user.CountryOfOperation;
            frayteUser.ContactName = user.ContactName;
            frayteUser.Email = user.UserEmail;
            frayteUser.TelephoneNo = user.TelephoneNo;
            frayteUser.MobileNo = user.MobileNo;
            frayteUser.FaxNumber = user.FaxNumber;
            frayteUser.Timezone = UtilityRepository.GetTimezoneModelDetail(user.TimezoneId);
            frayteUser.WorkingStartTime = UtilityRepository.TimeZoneTime(user.WorkingStartTime, frayteUser.Timezone.Name);
            frayteUser.WorkingEndTime = UtilityRepository.TimeZoneTime(user.WorkingEndTime, frayteUser.Timezone.Name);
            frayteUser.VATGST = user.VATGST;
            frayteUser.ShortName = user.ShortName;
            frayteUser.Position = user.Position;
            frayteUser.Skype = user.Skype;
            return frayteUser;
        }

        public static FrayteInternalUser InternalUserMapping(User user)
        {
            FrayteInternalUser frayteUser = new FrayteInternalUser();
            frayteUser.UserId = user.UserId;
            frayteUser.CargoWiseId = user.CargoWiseId;
            frayteUser.CargoWiseBardCode = user.CargoWiseBardCode;
            frayteUser.WorkingWeekDay = new WorkingWeekDay();
            if (user.WorkingWeekDayId != null)
            {
                frayteUser.WorkingWeekDay.WorkingWeekDayId = user.WorkingWeekDayId.Value;
            }

            frayteUser.CompanyName = user.CompanyName;
            frayteUser.ClientId = user.ClientId;
            frayteUser.IsClient = user.IsClient;
            frayteUser.CountryOfOperation = user.CountryOfOperation;
            frayteUser.ContactName = user.ContactName;
            frayteUser.Email = user.Email;
            frayteUser.TelephoneNo = user.TelephoneNo;
            frayteUser.MobileNo = user.MobileNo;
            frayteUser.FaxNumber = user.FaxNumber;
            frayteUser.Timezone = UtilityRepository.GetTimezoneModelDetail(user.TimezoneId);
            frayteUser.WorkingStartTime = UtilityRepository.TimeZoneTime(user.WorkingStartTime, frayteUser.Timezone.Name);
            frayteUser.WorkingEndTime = UtilityRepository.TimeZoneTime(user.WorkingEndTime, frayteUser.Timezone.Name);
            frayteUser.startTime = UtilityRepository.ConvertToCustomerTimeZone(user.WorkingStartTime, frayteUser.Timezone);
            frayteUser.EndTime = UtilityRepository.ConvertToCustomerTimeZone(user.WorkingEndTime, frayteUser.Timezone);
            frayteUser.VATGST = user.VATGST;
            frayteUser.ShortName = user.ShortName;
            frayteUser.Position = user.Position;
            frayteUser.CreatedBy = user.CreatedBy;
            frayteUser.CreatedOn = user.CreatedOn;
            frayteUser.UpdatedBy = user.UpdatedBy;
            frayteUser.UpdatedOn = user.UpdatedOn;
            frayteUser.Skype = user.Skype;
            return frayteUser;
        }

        public static FrayteAgent AgentMapping(User user)
        {
            FrayteAgent frayteUser = new FrayteAgent();
            frayteUser.UserId = user.UserId;
            frayteUser.CargoWiseId = user.CargoWiseId;
            frayteUser.CargoWiseBardCode = user.CargoWiseBardCode;
            frayteUser.WorkingWeekDay = new WorkingWeekDay();
            if (user.WorkingWeekDayId > 0)
            {
                frayteUser.WorkingWeekDay.WorkingWeekDayId = user.WorkingWeekDayId.Value;
            }

            frayteUser.CompanyName = user.CompanyName;
            frayteUser.ClientId = user.ClientId;
            frayteUser.IsClient = user.IsClient;
            frayteUser.CountryOfOperation = user.CountryOfOperation;
            frayteUser.ContactName = user.ContactName;
            frayteUser.Email = user.Email;
            frayteUser.TelephoneNo = user.TelephoneNo;
            frayteUser.MobileNo = user.MobileNo;
            frayteUser.FaxNumber = user.FaxNumber;
            frayteUser.Timezone = UtilityRepository.GetTimezoneModelDetail(user.TimezoneId);
            frayteUser.WorkingStartTime = UtilityRepository.TimeZoneTime(user.WorkingStartTime, frayteUser.Timezone.Name);
            frayteUser.WorkingEndTime = UtilityRepository.TimeZoneTime(user.WorkingEndTime, frayteUser.Timezone.Name);

            frayteUser.VATGST = user.VATGST;
            frayteUser.ShortName = user.ShortName;
            frayteUser.Position = user.Position;
            frayteUser.Skype = user.Skype;
            return frayteUser;
        }

        public static FrayteCustomer CustomerMapping(User user)
        {
            FrayteCustomer frayteUser = new FrayteCustomer();
            frayteUser.UserId = user.UserId;
            frayteUser.CreatedBy = user.CreatedBy;
            frayteUser.OperationZoneId = user.OperationZoneId;
            frayteUser.CargoWiseId = user.CargoWiseId;
            frayteUser.CargoWiseBardCode = user.CargoWiseBardCode;
            if (user.WorkingWeekDayId > 0 && user.WorkingWeekDayId != null)
            {
                frayteUser.WorkingWeekDay = new WorkingWeekDay();
                frayteUser.WorkingWeekDay.WorkingWeekDayId = user.WorkingWeekDayId.Value;
            }
            frayteUser.CompanyName = user.CompanyName;
            frayteUser.ClientId = user.ClientId;
            frayteUser.IsClient = user.IsClient;
            frayteUser.CountryOfOperation = user.CountryOfOperation;
            frayteUser.ContactName = user.ContactName;
            frayteUser.UserEmail = user.UserEmail;
            frayteUser.Email = user.Email;
            frayteUser.TelephoneNo = user.TelephoneNo;
            frayteUser.MobileNo = user.MobileNo;
            frayteUser.FaxNumber = user.FaxNumber;
            frayteUser.Timezone = UtilityRepository.GetTimezoneModelDetail(user.TimezoneId);
            frayteUser.WorkingStartTime = UtilityRepository.TimeZoneTime(user.WorkingStartTime, frayteUser.Timezone.Name);
            frayteUser.WorkingEndTime = UtilityRepository.TimeZoneTime(user.WorkingEndTime, frayteUser.Timezone.Name);
            frayteUser.startTime = UtilityRepository.ConvertToCustomerTimeZone(user.WorkingStartTime, frayteUser.Timezone);
            frayteUser.EndTime = UtilityRepository.ConvertToCustomerTimeZone(user.WorkingEndTime, frayteUser.Timezone);
            frayteUser.VATGST = user.VATGST;
            frayteUser.ShortName = user.ShortName;
            frayteUser.Position = user.Position;
            frayteUser.Skype = user.Skype;
            return frayteUser;
        }

        public static FrayteShipperReceiver ShipperReceiverMapping(User user)
        {
            FrayteShipperReceiver frayteUser = new FrayteShipperReceiver();
            frayteUser.UserId = user.UserId;
            frayteUser.CargoWiseId = user.CargoWiseId;
            frayteUser.CargoWiseBardCode = user.CargoWiseBardCode;
            frayteUser.WorkingWeekDay = new WorkingWeekDay();
            if (user.WorkingWeekDayId != null)
            {
                frayteUser.WorkingWeekDay.WorkingWeekDayId = user.WorkingWeekDayId.Value;
            }

            frayteUser.CompanyName = user.CompanyName;
            frayteUser.ClientId = user.ClientId;
            frayteUser.IsClient = user.IsClient;
            frayteUser.CountryOfOperation = user.CountryOfOperation;
            frayteUser.ContactName = user.ContactName;
            frayteUser.Email = user.Email;
            frayteUser.TelephoneNo = user.TelephoneNo;
            frayteUser.MobileNo = user.MobileNo;
            frayteUser.FaxNumber = user.FaxNumber;
            frayteUser.Timezone = UtilityRepository.GetTimezoneModelDetail(user.TimezoneId);
            frayteUser.WorkingStartTime = UtilityRepository.TimeZoneTime(user.WorkingStartTime, frayteUser.Timezone.Name);
            frayteUser.WorkingEndTime = UtilityRepository.TimeZoneTime(user.WorkingEndTime, frayteUser.Timezone.Name);

            frayteUser.VATGST = user.VATGST;
            frayteUser.ShortName = user.ShortName;
            frayteUser.Position = user.Position;
            frayteUser.Skype = user.Skype;
            return frayteUser;
        }

        public static FrayteAddress UserAddressMapping(UserAddress userAddress)
        {
            FrayteAddress frayteAddress = new FrayteAddress();
            frayteAddress.UserAddressId = userAddress.UserAddressId;
            frayteAddress.UserId = userAddress.UserId;
            frayteAddress.AddressTypeId = userAddress.AddressTypeId;
            frayteAddress.Address = userAddress.Address;
            frayteAddress.Address2 = userAddress.Address2;
            frayteAddress.Address3 = userAddress.Address3;
            frayteAddress.City = userAddress.City;
            frayteAddress.Suburb = userAddress.Suburb;
            frayteAddress.State = userAddress.State;
            frayteAddress.Zip = userAddress.Zip;
            frayteAddress.Country = new FrayteCountryCode();
            frayteAddress.Country.CountryId = userAddress.CountryId;
            return frayteAddress;
        }

        internal static string GetRoleName(int roleId)
        {
            switch (roleId)
            {
                case 1:
                    return "Admin";
                case 2:
                    return "Agent";
                case 3:
                    return "Customer";
                case 4:
                    return "Receiver";
                case 5:
                    return "Shipper";
                case 6:
                    return "Staff";
                case 7:
                    return "Warehouse";
                case 8:
                    return "HS Code Operators";
                case 9:
                    return "Master Admin";
                case 10:
                    return "Warehouse Agents";
                case 11:
                    return "HS Code Managers";
                case 12:
                    return "Call Center Operators";
                case 13:
                    return "Call Center Managers";
                case 14:
                    return "Accounts";
                default:
                    return "";
            }
        }

        internal static string GetWorkingHours(TimeSpan? workingStartTime, TimeSpan? workingEndTime)
        {
            string workingHours = "";

            if (workingStartTime.HasValue && workingEndTime.HasValue)
            {
                //workingHours = workingStartTime.Value.ToString("hh:mm tt") + " - " + workingEndTime.Value.ToString("hh:mm tt");
                //var ff = workingStartTime.Value.ToString(@"hh\:mm\:ss");
                //var fff = workingStartTime.Value.ToString(@"hh\:mm");
                workingHours = workingStartTime.Value.ToString(@"hh\:mm") + " - " + workingEndTime.Value.ToString(@"hh\:mm");
            }

            return workingHours;
        }

        internal static string GetWorkingHoursWithTimeZone(TimeSpan? workingStartTime, TimeSpan? workingEndTime, int? timezoneId)
        {
            string workingHours = "";

            if (workingStartTime.HasValue && workingEndTime.HasValue && timezoneId.HasValue)
            {
                var timeZoneResult = dbContext.Timezones.Where(p => p.TimezoneId == timezoneId.Value).FirstOrDefault();

                var ff = GetTimeZoneTime(workingStartTime, timeZoneResult.Name);
                string WorkingStartTime = GetFormmatedTime(ff);
                var fff = GetTimeZoneTime(workingEndTime, timeZoneResult.Name);
                string WorkingEndTime = GetFormmatedTime(fff);
                //workingHours = workingStartTime.Value.ToString(@"hh\:mm") + " - " + workingEndTime.Value.ToString(@"hh\:mm");
                workingHours = WorkingStartTime + " - " + WorkingEndTime + "  " + timeZoneResult.OffsetShort;
            }

            return workingHours;
        }

        public static string GetFormmatedTime(string time)
        {
            if (!string.IsNullOrEmpty(time))
            {
                if (time.Length == 3)
                {
                    time = "0" + time;
                }
                string hh = time.Substring(0, 2);
                string mm = time.Substring(2, 2);
                string finalTime = string.Format("{0}:{1}", hh, mm);
                return finalTime;
            }

            return null;
        }

        internal static int GetAccountNumber(int shipmentId)
        {
            var result = (from s in dbContext.Shipments
                          join c in dbContext.UserAdditionals on s.CustomerId equals c.UserId
                          where s.ShipmentId == shipmentId
                          select c.AccountNo).FirstOrDefault();
            if (result != null)
            {
                return Convert.ToInt32(result);
            }
            else
            {
                return 0;
            }
        }

        public static string GetShipmentExportImportType(int? wareHouseId)
        {
            if (wareHouseId.HasValue && wareHouseId.Value > 0)
            {
                return FrayteExportType.Export;
            }
            else
            {
                return FrayteExportType.Import;
            }
        }

        public static FrayteOperationZone GetOperationZone()
        {
            FrayteOperationZone fz = new FrayteOperationZone();
            string value = AppSettings.OperationZone;
            string[] val = value.Split(new string[] { "#" }, StringSplitOptions.None);
            fz.OperationZoneId = int.Parse(val[0].ToString());
            fz.OperationZoneName = val[1].ToString();
            return fz;
        }

        public static string GetLogisticType(string operationZoneCountryCode, string fromCountryCode, string toCountryCode, bool isEuropeCountry)
        {
            string logisticType = "";

            //Check for UK Shipment
            if (fromCountryCode == "GBR" && toCountryCode == "GBR")
            {
                logisticType = FrayteLogisticType.UKShipment;
                return logisticType;
            }

            if ((operationZoneCountryCode == toCountryCode && fromCountryCode != toCountryCode) ||
                (operationZoneCountryCode == toCountryCode && fromCountryCode == toCountryCode))
            {
                logisticType = FrayteLogisticType.Import;
            }
            else if (operationZoneCountryCode == fromCountryCode && fromCountryCode != toCountryCode)
            {
                logisticType = FrayteLogisticType.Export;
            }
            else
            {
                logisticType = FrayteLogisticType.ThirdParty;
            }

            return logisticType;
        }

        public static string eCommLabelFileName(int id, string labelType)
        {
            string file = string.Empty;

            var result = (from r in dbContext.eCommerceShipments
                          join ea in dbContext.eCommerceShipmentAddresses on r.ToAddressId equals ea.eCommerceShipmentAddressId
                          join ed in dbContext.eCommerceShipmentDetails on r.eCommerceShipmentId equals ed.eCommerceShipmentId
                          join epd in dbContext.eCommercePackageTrackingDetails on ed.eCommerceShipmentDetailId equals epd.eCommerceShipmentDetailId
                          join cl in dbContext.CountryLogistics on ea.CountryId equals cl.CountryId
                          where r.eCommerceShipmentId == id
                          select new
                          {
                              CourierName = cl.LogisticService,
                              TrackingNumber = epd.TrackingNo

                          }).FirstOrDefault();

            var data = dbContext.eCommerceShipments.Find(id);
            if (data != null)
            {
                if (labelType == eCommLabelType.CourierLabel)
                {
                    if (result != null)
                    {
                        file = result.CourierName + "-" + result.TrackingNumber + ".pdf";
                    }
                }
                else if (labelType == eCommLabelType.FrayteLabel)
                {
                    file = "AWB-" + data.FrayteNumber + ".pdf";
                }
            }

            return file;
        }

        public static int GetZone(string logisticType, int operationZoneId, int fromCountryId, int toCountryId)
        {
            int applyZoneId = 0;

            //Get fromCountryId zone 
            LogisticServiceZoneCountry fromZoneCountry = (from lszc in dbContext.LogisticServiceZoneCountries
                                                          join lsz in dbContext.LogisticServiceZones on lszc.LogisticServiceZoneId equals lsz.LogisticServiceZoneId
                                                          join ls in dbContext.LogisticServices on lsz.LogisticServiceId equals ls.LogisticServiceId
                                                          where lszc.CountryId == fromCountryId &&
                                                                lszc.OperationZoneId == operationZoneId &&
                                                                ls.OperationZoneId == operationZoneId &&
                                                                ls.LogisticType == logisticType &&
                                                                ls.IsActive == true
                                                          select lszc).FirstOrDefault();

            //Get toCountryId zone
            LogisticServiceZoneCountry toZoneCountry = (from lszc in dbContext.LogisticServiceZoneCountries
                                                        join lsz in dbContext.LogisticServiceZones on lszc.LogisticServiceZoneId equals lsz.LogisticServiceZoneId
                                                        join ls in dbContext.LogisticServices on lsz.LogisticServiceId equals ls.LogisticServiceId
                                                        where lszc.CountryId == toCountryId &&
                                                              lszc.OperationZoneId == operationZoneId &&
                                                              ls.OperationZoneId == operationZoneId &&
                                                              ls.LogisticType == logisticType &&
                                                              ls.IsActive == true
                                                        select lszc).FirstOrDefault();

            if (logisticType == FrayteLogisticType.ThirdParty)
            {
                LogisticServiceThirdPartyMatrix zoneMatrixDetail = dbContext.LogisticServiceThirdPartyMatrices.Where(p => p.OperationZoneId == operationZoneId &&
                                                                                                                     p.FromLogisticServiceZoneId == fromZoneCountry.LogisticServiceZoneId &&
                                                                                                                     p.ToLogisticServiceZoneId == toZoneCountry.LogisticServiceZoneId).FirstOrDefault();

                applyZoneId = zoneMatrixDetail.ApplyLogisticServiceZoneId;
            }
            else if (logisticType == FrayteLogisticType.Import || logisticType == FrayteLogisticType.EUImport)
            {
                applyZoneId = fromZoneCountry.LogisticServiceZoneId;
            }
            else if (logisticType == FrayteLogisticType.Export || logisticType == FrayteLogisticType.EUExport)
            {
                applyZoneId = toZoneCountry.LogisticServiceZoneId;
            }

            return applyZoneId;
        }

        public static string PostCodeVerification(string PostCode, string CountryCode)
        {
            string postcode = string.Empty;
            if (CountryCode == "GB")
            {
                if (PostCode == null || PostCode == "")
                {
                    postcode = "";
                }
                else
                {
                    postcode = PostCode.Replace(" ", "");
                    if (postcode.Length == 5)
                    {
                        postcode = postcode.Substring(0, 2) + " " + postcode.Substring(2, 3);
                    }
                    else if (postcode.Length == 6)
                    {
                        postcode = postcode.Substring(0, 3) + " " + postcode.Substring(3, 3);
                    }
                    else if (postcode.Length == 7)
                    {
                        postcode = postcode.Substring(0, 4) + " " + postcode.Substring(4, 3);
                    }
                    else if (postcode.Length == 8)
                    {
                        postcode = postcode.Substring(0, 5) + " " + postcode.Substring(5, 3);
                    }
                }
            }
            else
            {
                postcode = PostCode;
            }
            return postcode;
        }

        public static string PackageLabelPath(int DirectShipmentId, string CourierName, string RateType)
        {
            FrayteLogicalPhysicalPath path = new DirectShipmentRepository().GetShipmentLogisticlabelPath(DirectShipmentId);
            string pdfFileName = string.Empty;
            string physycalpath = string.Empty;

            if (path != null)
            {
                if (path.PhysicalPath.Contains("~"))
                {
                    // For developement
                    pdfFileName = AppSettings.WebApiPath + "PackageLabel/" + DirectShipmentId + "/";
                    physycalpath = HttpContext.Current.Server.MapPath(path.PhysicalPath + "PackageLabel/" + DirectShipmentId + "/");
                }
                else
                {
                    pdfFileName = path.LogicalPath + "PackageLabel/" + DirectShipmentId + "/";
                    physycalpath = path.PhysicalPath + "/PackageLabel/" + DirectShipmentId + "/";
                }
            }
            else
            {
                pdfFileName = AppSettings.WebApiPath + "PackageLabel/" + DirectShipmentId + "/";
                physycalpath = HttpContext.Current.Server.MapPath("~/PackageLabel/" + DirectShipmentId + "/");
            }

            string Attachment = string.Empty;

            if (AppSettings.LabelSave == "")
            {
                var result = (from DS in dbContext.DirectShipments
                              where DS.DirectShipmentId == DirectShipmentId
                              select new
                              {
                                  LogisticLabel = DS.LogisticLabel
                              }).FirstOrDefault();

                if (result != null)
                {
                    if (!string.IsNullOrEmpty(result.LogisticLabel))
                    {
                        if (result.LogisticLabel.Contains(".html"))
                        {
                            string[] strAttachmentPath = result.LogisticLabel.Split(new string[] { ";" }, StringSplitOptions.None);
                            for (int i = 0; i < strAttachmentPath.Length; i++)
                            {
                                if (strAttachmentPath[i].Trim() != "")
                                {
                                    Attachment += physycalpath + strAttachmentPath[i].Trim() + ";";
                                }
                            }
                        }
                        else
                        {
                            Attachment += AppSettings.WebApiPath + "/PackageLabel/" + DirectShipmentId + "/" + result.LogisticLabel;
                        }
                    }
                }
            }
            else
            {
                var result = (from DS in dbContext.DirectShipments
                              where DS.DirectShipmentId == DirectShipmentId
                              select new
                              {
                                  LogisticLabel = DS.LogisticLabel
                              }).FirstOrDefault();

                if (result != null)
                {
                    if (!string.IsNullOrEmpty(result.LogisticLabel))
                    {
                        if (result.LogisticLabel.Contains(".html"))
                        {
                            string[] strAttachmentPath = result.LogisticLabel.Split(new string[] { ";" }, StringSplitOptions.None);
                            for (int i = 0; i < strAttachmentPath.Length; i++)
                            {
                                if (strAttachmentPath[i].Trim() != "")
                                {
                                    Attachment += physycalpath + strAttachmentPath[i].Trim() + ";";
                                }
                            }
                        }
                        else
                        {
                            Attachment += physycalpath + result.LogisticLabel;
                        }
                    }
                }
            }
            return Attachment;
        }

        public static decimal FuelSurCharge(int OperationZoneId, DateTime Date, string LogisticType)
        {
            decimal charge = 0.0m;
            if (LogisticType == FrayteLogisticType.UKShipment)
            {
                charge = dbContext.FuelSurCharges.Where(p => p.OperationZoneId == OperationZoneId && p.FuelMonthYear.Year == Date.Year && p.FuelMonthYear.Month == Date.Month).FirstOrDefault().DomesticFuelPercent == null ? 0.0m : dbContext.FuelSurCharges.Where(p => p.OperationZoneId == OperationZoneId && p.FuelMonthYear.Year == Date.Year && p.FuelMonthYear.Month == Date.Month).FirstOrDefault().DomesticFuelPercent.Value;
            }
            else
            {
                charge = dbContext.FuelSurCharges.Where(p => p.OperationZoneId == OperationZoneId && p.FuelMonthYear.Year == Date.Year && p.FuelMonthYear.Month == Date.Month).FirstOrDefault().FrayteFuelPercent;
            }
            return charge;
        }

        public static decimal TotalCustomerPrice(decimal TotalBasePrice, decimal CustomerMargin)
        {
            decimal roundmargin = Math.Round(((TotalBasePrice * CustomerMargin) / 100), 2);
            decimal TotalPrice = Math.Round((TotalBasePrice + roundmargin), 2);
            return TotalPrice;
        }

        public static decimal GrandTotal(decimal Rate, decimal FuelSurcharge, bool IsFuelSurchargeCalculate)
        {
            if (IsFuelSurchargeCalculate)
            {
                decimal roundfuel = Math.Round(((Rate * FuelSurcharge) / 100), 2);
                decimal TotalCharge = Math.Round((Rate + roundfuel), 2);
                return TotalCharge;
            }
            else
            {
                decimal TotalCharge = Math.Round(Rate, 2);
                return TotalCharge;
            }
        }

        public static LogisticService GetLogisticService(int LogisticServiceId)
        {
            LogisticService service = new LogisticService();
            if (LogisticServiceId > 0)
            {
                service = (from ls in dbContext.LogisticServices
                           where ls.LogisticServiceId == LogisticServiceId
                           select ls).FirstOrDefault();
            }
            return service;
        }

        public static bool CheckUploadExcelFormat(string PiecesColumnList, DataTable dtExcelData)
        {
            if (dtExcelData != null)
            {
                foreach (DataColumn col in dtExcelData.Columns)
                {
                    if (!PiecesColumnList.Split(',').Contains(col.ColumnName.Trim()))
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string QuotationFileName(string QuotationShipmentId, int OperationZoneId, string FileFormat)
        {
            string FileName = string.Empty;
            if (QuotationShipmentId.Length == 1)
            {
                FileName = "QU" + UtilityRepository.OperationZoneName(OperationZoneId) + "-" + DateTime.Today.Year.ToString().Substring(2, 2) + DateTime.Today.ToString("MM") + "000" + QuotationShipmentId + FileFormat;
            }
            else if (QuotationShipmentId.Length == 2)
            {
                FileName = "QU" + UtilityRepository.OperationZoneName(OperationZoneId) + "-" + DateTime.Today.Year.ToString().Substring(2, 2) + DateTime.Today.ToString("MM") + "00" + QuotationShipmentId + FileFormat;
            }
            else if (QuotationShipmentId.Length == 3)
            {
                FileName = "QU" + UtilityRepository.OperationZoneName(OperationZoneId) + "-" + DateTime.Today.Year.ToString().Substring(2, 2) + DateTime.Today.ToString("MM") + "0" + QuotationShipmentId + FileFormat;
            }
            else if (QuotationShipmentId.Length == 4)
            {
                FileName = "QU" + UtilityRepository.OperationZoneName(OperationZoneId) + "-" + DateTime.Today.Year.ToString().Substring(2, 2) + DateTime.Today.ToString("MM") + QuotationShipmentId + FileFormat;
            }
            return FileName;
        }

        public static string OperationZoneName(int OperationZoneId)
        {
            string Name = string.Empty;
            switch (OperationZoneId)
            {
                case 1:
                    Name = FrayteOperationZoneName.HKG;
                    break;
                case 2:
                    Name = FrayteOperationZoneName.UK;
                    break;
            }
            return Name;
        }

        public static FrayteUser GetCustomerComapnyDetail(int UserId)
        {
            FrayteUser detail = (from u in dbContext.Users
                                 where u.UserId == UserId
                                 select new FrayteUser
                                 {
                                     CompanyName = (u.CompanyName == "" ? u.ContactName : u.CompanyName)
                                 }).FirstOrDefault();

            return detail;
        }

        public static float GetShipmentChargeableWeight(FrayteCommerceShipmentDraft eCommerceBookingDetail)
        {
            float totalWeights = (float)eCommerceBookingDetail.Packages.Sum(p => p.CartoonValue * p.Weight);

            float total = 0;
            foreach (var data in eCommerceBookingDetail.Packages)
            {
                if (eCommerceBookingDetail.PakageCalculatonType == FraytePakageCalculationType.kgtoCms)
                {
                    total += (float)((data.Length * data.Width * data.Height) / 6000) * data.CartoonValue;
                }
                else if (eCommerceBookingDetail.PakageCalculatonType == FraytePakageCalculationType.LbToInchs)
                {
                    total += (float)((data.Length * data.Width * data.Height) / 166) * data.CartoonValue;
                }
            }

            string[] num;
            num = total.ToString().Split('.');

            if (num.Length > 1)
            {
                var asa = float.Parse(num[1]);
                if (asa == 0)
                {
                    if (totalWeights > float.Parse(num[0]))
                    {
                        return totalWeights;
                    }
                    else
                    {
                        return float.Parse(num[0]);
                    }
                }
                else
                {
                    if (asa > 49)
                    {

                        var r = float.Parse(num[0]) + 1;
                        if (totalWeights > r)
                        {
                            return totalWeights;
                        }
                        else
                        {
                            return r;
                        }
                    }
                    else
                    {
                        var s = float.Parse(num[0]) + (float)0.50;
                        if (totalWeights > s)
                        {
                            return totalWeights;
                        }
                        else
                        {
                            return s;
                        }
                    }
                }
            }

            return 0;
        }

        public static float GetShipmentChargeableWeightUploadShipment(FrayteUploadshipment eCommerceBookingDetail)
        {
            float totalWeights = (float)eCommerceBookingDetail.Package.Sum(p => p.CartoonValue * p.Weight);

            float total = 0;
            foreach (var data in eCommerceBookingDetail.Package)
            {
                if (eCommerceBookingDetail.PackageCalculationType == FraytePakageCalculationType.kgtoCms)
                {
                    total += (float)((data.Length * data.Width * data.Height) / 6000) * data.CartoonValue;
                }
                else if (eCommerceBookingDetail.PackageCalculationType == FraytePakageCalculationType.LbToInchs)
                {
                    total += (float)((data.Length * data.Width * data.Height) / 166) * data.CartoonValue;
                }
            }

            string[] num;
            num = total.ToString().Split('.');

            if (num.Length > 1)
            {
                var asa = float.Parse(num[1]);
                if (asa == 0)
                {
                    if (totalWeights > float.Parse(num[0]))
                    {
                        return totalWeights;
                    }
                    else
                    {
                        return float.Parse(num[0]);
                    }
                }
                else
                {
                    if (asa > 49)
                    {
                        var r = float.Parse(num[0]) + 1;
                        if (totalWeights > r)
                        {
                            return totalWeights;
                        }
                        else
                        {
                            return r;
                        }
                    }
                    else
                    {
                        var s = float.Parse(num[0]) + (float)0.50;
                        if (totalWeights > s)
                        {
                            return totalWeights;
                        }
                        else
                        {
                            return s;
                        }
                    }
                }
            }

            return 0;
        }

        public static string OperationZoneCurrency(int OperationZoneId)
        {
            string Currency = string.Empty;
            switch (OperationZoneId)
            {
                case 1:
                    Currency = FrayteOperationCurrency.HKD;
                    break;
                case 2:
                    Currency = FrayteOperationCurrency.GBP;
                    break;
            }
            return Currency;
        }

        public static string FullOperationZoneName(int OperationZoneId)
        {
            string FullName = string.Empty;
            switch (OperationZoneId)
            {
                case 1:
                    FullName = FrayteOperationFullName.HK;
                    break;
                case 2:
                    FullName = FrayteOperationFullName.UK;
                    break;
            }
            return FullName;
        }

        public static string FuelPercentage(int LogisticServiceId, int OperationZoneId, int Month, int Year)
        {
            var fuel = (from fs in dbContext.FuelSurCharges
                        where
                            fs.LogisticServiceId == LogisticServiceId &&
                            fs.OperationZoneId == OperationZoneId &&
                            fs.FuelMonthYear.Month == Month &&
                            fs.FuelMonthYear.Year == Year
                        select new
                        {
                            fs.FrayteFuelPercent,
                            fs.DomesticFuelPercent,
                            fs.FuelMonthYear
                        }).FirstOrDefault();

            if (fuel != null)
            {
                if (OperationZoneId == 1)
                {
                    return fuel.FrayteFuelPercent.ToString() + " % (" + fuel.FuelMonthYear.ToString("MMM") + " - " + fuel.FuelMonthYear.ToString("yy") + ")";
                }
                else
                {
                    //return (fuel.DomesticFuelPercent.HasValue ? fuel.DomesticFuelPercent.Value : 0.0m).ToString() + " % (" + fuel.FuelMonthYear.ToString("MMM") + " - " + fuel.FuelMonthYear.ToString("yy") + ")";
                    return fuel.FrayteFuelPercent.ToString() + " % (" + fuel.FuelMonthYear.ToString("MMM") + " - " + fuel.FuelMonthYear.ToString("yy") + ")";
                }
            }
            else
            {
                return "";
            }
        }

        public static string CustomerCurrency(int CustomerId)
        {
            var currency = dbContext.UserAdditionals.Find(CustomerId);
            if (currency != null)
            {
                return currency.CreditLimitCurrencyCode;
            }
            else
            {
                return "";
            }
        }

        public static string FrayteAccountNo(string AccountNo)
        {
            string ClientAccountNo = string.Empty;
            if (!string.IsNullOrEmpty(AccountNo))
            {
                for (int i = 0; i < AccountNo.Length; i++)
                {
                    if (i == 2)
                    {
                        ClientAccountNo += AccountNo[i].ToString() + "-";
                    }
                    else if (i == 5)
                    {
                        ClientAccountNo += AccountNo[i].ToString() + "-";
                    }
                    else
                    {
                        ClientAccountNo += AccountNo[i].ToString();
                    }
                }
            }
            return ClientAccountNo;
        }

        public static string MonthName(DateTime Date)
        {
            string MonthName = string.Empty;
            switch (Date.Month)
            {
                case 1:
                    MonthName = "Jan";
                    break;
                case 2:
                    MonthName = "Feb";
                    break;
                case 3:
                    MonthName = "Mar";
                    break;
                case 4:
                    MonthName = "Apr";
                    break;
                case 5:
                    MonthName = "May";
                    break;
                case 6:
                    MonthName = "Jun";
                    break;
                case 7:
                    MonthName = "Jul";
                    break;
                case 8:
                    MonthName = "Aug";
                    break;
                case 9:
                    MonthName = "Sep";
                    break;
                case 10:
                    MonthName = "Oct";
                    break;
                case 11:
                    MonthName = "Nov";
                    break;
                case 12:
                    MonthName = "Dec";
                    break;
            }
            return MonthName;
        }

        public static string Get24HourFormatedTime(string Time)
        {
            string HourTime = string.Empty;
            if (Time.Length == 4)
            {
                var subtime = Time.Substring(0, 2);
                var subsec = Time.Substring(2, 2);
                switch (subtime)
                {
                    case "01":
                        HourTime = "13" + subsec;
                        break;
                    case "02":
                        HourTime = "14" + subsec;
                        break;
                    case "03":
                        HourTime = "15" + subsec;
                        break;
                    case "04":
                        HourTime = "16" + subsec;
                        break;
                    case "05":
                        HourTime = "17" + subsec;
                        break;
                    default:
                        HourTime = "12" + subsec;
                        break;
                }
                return HourTime;
            }
            else
            {
                return HourTime = Time;
            }
        }

        public static int LogisticZoneCountryId(int OperationZoneId, int CountryId, string LogisticCompany, string LogisticType, string RateType, string Zone)
        {
            if (string.IsNullOrEmpty(RateType))
            {
                var id = (from ls in dbContext.LogisticServices
                          join lsz in dbContext.LogisticServiceZones on ls.LogisticServiceId equals lsz.LogisticServiceId
                          join lszc in dbContext.LogisticServiceZoneCountries on lsz.LogisticServiceZoneId equals lszc.LogisticServiceZoneId
                          where ls.OperationZoneId == OperationZoneId &&
                                ls.LogisticCompany == LogisticCompany &&
                                ls.LogisticType == LogisticType &&
                                lsz.ZoneName == Zone &&
                                lszc.CountryId == CountryId
                          select new
                          {
                              lszc.LogisticZoneCountryId
                          }).FirstOrDefault();

                if (id != null && id.LogisticZoneCountryId > 0)
                {
                    return id.LogisticZoneCountryId;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                var id = (from ls in dbContext.LogisticServices
                          join lsz in dbContext.LogisticServiceZones on ls.LogisticServiceId equals lsz.LogisticServiceId
                          join lszc in dbContext.LogisticServiceZoneCountries on lsz.LogisticServiceZoneId equals lszc.LogisticServiceZoneId
                          where ls.OperationZoneId == OperationZoneId &&
                                ls.LogisticCompany == LogisticCompany &&
                                ls.LogisticType == LogisticType &&
                                ls.RateType == RateType &&
                                lsz.ZoneName == Zone &&
                                lszc.CountryId == CountryId
                          select new
                          {
                              lszc.LogisticZoneCountryId
                          }).FirstOrDefault();

                if (id != null && id.LogisticZoneCountryId > 0)
                {
                    return id.LogisticZoneCountryId;
                }
                else
                {
                    return 0;
                }
            }
        }

        public static int LogisticServiceZoneId(int OperationZoneId, string LogisticCompany, string LogisticType, string RateType, string Zone)
        {
            if (string.IsNullOrEmpty(RateType))
            {
                var id = (from ls in dbContext.LogisticServices
                          join lsz in dbContext.LogisticServiceZones on ls.LogisticServiceId equals lsz.LogisticServiceId
                          where ls.OperationZoneId == OperationZoneId &&
                                ls.LogisticCompany == LogisticCompany &&
                                ls.LogisticType == LogisticType &&
                                lsz.ZoneName == Zone
                          select new
                          {
                              lsz.LogisticServiceZoneId
                          }).FirstOrDefault();

                if (id != null && id.LogisticServiceZoneId > 0)
                {
                    return id.LogisticServiceZoneId;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                var id = (from ls in dbContext.LogisticServices
                          join lsz in dbContext.LogisticServiceZones on ls.LogisticServiceId equals lsz.LogisticServiceId
                          where ls.OperationZoneId == OperationZoneId &&
                                ls.LogisticCompany == LogisticCompany &&
                                ls.LogisticType == LogisticType &&
                                ls.RateType == RateType &&
                                lsz.ZoneName == Zone
                          select new
                          {
                              lsz.LogisticServiceZoneId
                          }).FirstOrDefault();

                if (id != null && id.LogisticServiceZoneId > 0)
                {
                    return id.LogisticServiceZoneId;
                }
                else
                {
                    return 0;
                }
            }
        }

        public static double? GetEstimatedWeightByTrackingNumber(string trackingNumberCode)
        {
            try
            {
                var data = dbContext.DirectShipments.Where(p => p.TrackingDetail == trackingNumberCode).FirstOrDefault();
                if (data != null)
                {
                    var packages = dbContext.DirectShipmentDetails.Where(p => p.DirectShipmentId == data.DirectShipmentId).ToList();
                    double sum = 0.0;
                    foreach (var pack in packages)
                    {
                        sum += (double)pack.Weight * pack.CartoonValue;
                    }
                    return sum;
                }
                else
                {
                    return 0.0;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static string GetFormattedTimeFromString(string time)
        {
            string str = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(time))
                {
                    string myStr = string.Empty;

                    if (time.Count() == 4)
                    {
                        foreach (var chr in time)
                        {
                            str += chr;
                            if (str.Count() == 2)
                            {
                                str += ":";
                            }
                        }
                    }
                }
                return str;
            }
            catch (Exception ex)
            {
                return str;
            }
        }

        public static string GetFormattedTimeFromString(string time, double TotalHours)
        {
            string str = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(time))
                {
                    string myStr = string.Empty;

                    if (time.Count() == 4)
                    {
                        foreach (var chr in time)
                        {
                            str += chr;
                            if (str.Count() == 2)
                            {
                                str += ":";
                            }
                        }
                    }
                }
                if (TotalHours >= 12)
                {
                    str += " PM";
                }
                else
                {
                    str += " AM";
                }
                return str;
            }
            catch (Exception ex)
            {
                return str;
            }
        }

        public static string GetString(string OriginalString, int Length)
        {
            if (OriginalString.Length > Length)
            {
                return OriginalString.Substring(0, Length);
            }
            else
            {
                return OriginalString;
            }
        }

        #region

        public static DateTime ConvertDateTimetoUniversalTime(DateTime Date)
        {
            return Date.ToUniversalTime();
        }

        public static DateTime ConvertToUniversalTime(string Time, TimeZoneModal TimeZone)
        {
            var hr = Convert.ToInt32(Time.Substring(0, 2));
            var min = Convert.ToInt32(Time.Substring(2, 2));
            var dt = new DateTime(2018, 5, 21, hr, min, 00);
            var TZ = TimeZoneInfo.FindSystemTimeZoneById(TimeZone.Name);
            var dr = DateTime.SpecifyKind(dt, DateTimeKind.Unspecified);
            var a = dr.TimeOfDay.TotalHours;
            var res = TimeZoneInfo.ConvertTimeToUtc(dr, TZ);
            //var res1 =TimeZoneInfo.ConvertTimeFromUtc(res, TimeZone);
            return res;
        }

        public static DateTime ConvertToUniversalTimeWitDate(string Time, DateTime Date, TimeZoneModal TimeZone)
        {
            if (Time.Contains(":"))
            {
                var hr = Convert.ToInt32(Time.Substring(0, 2));
                var min = Convert.ToInt32(Time.Substring(3, 2));
                var dt = new DateTime(Date.Year, Date.Month, Date.Day, hr, min, 00);
                var TZ = TimeZoneInfo.FindSystemTimeZoneById(TimeZone.Name);
                var dr = DateTime.SpecifyKind(dt, DateTimeKind.Unspecified);
                var a = dr.TimeOfDay.TotalHours;
                var res = TimeZoneInfo.ConvertTimeToUtc(dr, TZ);
                return res;
            }
            else
            {
                var hr = Convert.ToInt32(Time.Substring(0, 2));
                var min = Convert.ToInt32(Time.Substring(2, 2));
                var dt = new DateTime(Date.Year, Date.Month, Date.Day, hr, min, 00);
                var TZ = TimeZoneInfo.FindSystemTimeZoneById(TimeZone.Name);
                var dr = DateTime.SpecifyKind(dt, DateTimeKind.Unspecified);
                var a = dr.TimeOfDay.TotalHours;
                var res = TimeZoneInfo.ConvertTimeToUtc(dr, TZ);
                return res;
            }
        }

        public static string ConvertToCustomerTimeZone(TimeSpan? Time, TimeZoneModal TimeZone)
        {
            var str = "";
            var dt = new DateTime(2018, 5, 21, Time.Value.Hours, Time.Value.Minutes, 00);
            var TZ = TimeZoneInfo.FindSystemTimeZoneById(TimeZone.Name);
            var dr = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
            var res = TimeZoneInfo.ConvertTimeFromUtc(dr, TZ);
            if (res.TimeOfDay.TotalHours < 10 && res.TimeOfDay.Minutes < 10)
            {
                str = "0" + res.TimeOfDay.Hours.ToString() + "0" + res.TimeOfDay.Minutes.ToString();
            }
            else if (res.TimeOfDay.TotalHours < 10 && res.TimeOfDay.Minutes > 9)
            {
                str = "0" + res.TimeOfDay.Hours.ToString() + res.TimeOfDay.Minutes.ToString();
            }
            if (res.TimeOfDay.TotalHours > 9 && res.TimeOfDay.Minutes < 10)
            {
                str = res.TimeOfDay.Hours.ToString() + "0" + res.TimeOfDay.Minutes.ToString();
            }
            else if (res.TimeOfDay.TotalHours > 9 && res.TimeOfDay.Minutes > 9)
            {
                str = res.TimeOfDay.Hours.ToString() + res.TimeOfDay.Minutes.ToString();
            }
            return str.ToString();
        }

        public static DateTime ConvertDatetoSpecifiedTimeZoneTime(DateTime Date, TimeZoneModal Tz)
        {
            var ab = Date.Date;
            var c = ab.ToFileTime();
            var date = new DateTime(Date.Year, Date.Month, Date.Day, Date.Hour, Date.Minute, Date.Second, DateTimeKind.Unspecified);
            var Tzn = TimeZoneInfo.FindSystemTimeZoneById(Tz.Name);
            var a = TimeZoneInfo.ConvertTime(date, Tzn).ToUniversalTime();
            return a;
        }

        public static TimeSpan TimeSpanConversion(int Hour, int Minutes, int Seconds)
        {
            return new TimeSpan(Hour, Minutes, Seconds);
        }

        #endregion

        #region

        public static Tuple<DateTime, string> UtcDateToOtherTimezone(DateTime Date, TimeSpan Time, TimeZoneInfo TimeZoneInformation)
        {
            var UTCDate = new DateTime(Date.Year, Date.Month, Date.Day, Time.Hours, Time.Minutes, Time.Seconds, DateTimeKind.Utc);
            var ConvertedDate = TimeZoneInfo.ConvertTimeFromUtc(UTCDate, TimeZoneInformation);
            var ConvertedTime = GetTimeZoneTime((TimeSpan?)TimeZoneInfo.ConvertTimeFromUtc(UTCDate, TimeZoneInformation).TimeOfDay);
            return Tuple.Create(ConvertedDate, ConvertedTime);
        }

        #endregion

        public static DateTime TimeZoneTime(TimeSpan? time, string timeZoneName)
        {
            if (time.HasValue && !string.IsNullOrEmpty(timeZoneName))
            {
                var finalTime = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, time.Value.Hours, time.Value.Minutes, 0);

                TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneName);
                DateTime dtConverted = TimeZoneInfo.ConvertTimeFromUtc(finalTime, timeZone);

                return dtConverted;
            }

            return new DateTime();
        }

        public static string Encrypt(string input, string PrivateKey)
        {
            byte[] inputArray = UTF8Encoding.UTF8.GetBytes(input);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();

            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(PrivateKey);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string Base64UrlEncode(string input)
        {
            return input.Replace('+', '-').Replace('/', '_');
        }

        public static string Base64UrlDecode(string input)
        {
            return input.Replace('-', '+').Replace('_', '/');
        }

        public static string Decrypt(string input, string PrivateKey)
        {
            byte[] inputArray = Convert.FromBase64String(input);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(PrivateKey);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        public static string ConvertStringToHex(String input, System.Text.Encoding encoding, string privateKey)
        {
            string keyToEncript = input + "|" + privateKey;
            Byte[] stringBytes = encoding.GetBytes(keyToEncript);
            StringBuilder sbBytes = new StringBuilder(stringBytes.Length * 2);

            foreach (byte b in stringBytes)
            {
                sbBytes.AppendFormat("{0:X2}", b);
            }
            return sbBytes.ToString();
        }

        public static string ConvertHexToString(String hexInput, System.Text.Encoding encoding)
        {
            int numberChars = hexInput.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hexInput.Substring(i, 2), 16);
            }
            return encoding.GetString(bytes);
        }

        public static string ConcatinateAddress(TradelBookingAdress TM, string AirportCode)
        {
            string Address = "";
            if (!string.IsNullOrEmpty(TM.FirstName) && !string.IsNullOrEmpty(TM.LastName))
            {
                Address = Address + TM.FirstName + " " + TM.LastName + Environment.NewLine;
            }
            if (!string.IsNullOrEmpty(TM.CompanyName))
            {
                Address = Address + TM.CompanyName + Environment.NewLine;
            }
            if (!string.IsNullOrEmpty(TM.Address))
            {
                Address = Address + TM.Address + Environment.NewLine;
            }
            if (!string.IsNullOrEmpty(TM.Address2))
            {
                Address = Address + TM.Address2 + Environment.NewLine;
            }
            if (!string.IsNullOrEmpty(TM.City))
            {
                Address = Address + TM.City;
            }
            if (!string.IsNullOrEmpty(TM.PostCode))
            {
                Address = Address + " - " + TM.PostCode + Environment.NewLine;
            }
            //if (!string.IsNullOrEmpty(TM.PostCode))
            //{
            //    Address = Address + " " + TM.PostCode;
            //}
            if (TM.Country != null && !string.IsNullOrEmpty(TM.Country.Name))
            {
                Address = Address + TM.Country.Name;
            }
            if (!string.IsNullOrEmpty(AirportCode))
            {
                Address = Address + " - " + AirportCode;
            }

            return Address;
        }

        public static string ConcatinateExpressAddress(TradelBookingAdress TM)
        {
            string Address = "";
            if (!string.IsNullOrEmpty(TM.FirstName) && !string.IsNullOrEmpty(TM.LastName))
            {
                Address = Address + TM.FirstName + " " + TM.LastName + Environment.NewLine;
            }
            if (!string.IsNullOrEmpty(TM.CompanyName))
            {
                Address = Address + TM.CompanyName + Environment.NewLine;
            }
            if (!string.IsNullOrEmpty(TM.Address))
            {
                Address = Address + TM.Address + Environment.NewLine;
            }
            if (!string.IsNullOrEmpty(TM.Address2))
            {
                Address = Address + TM.Address2 + Environment.NewLine;
            }
            if (!string.IsNullOrEmpty(TM.City))
            {
                Address = Address + TM.City;
            }
            if (!string.IsNullOrEmpty(TM.PostCode))
            {
                Address = Address + " - " + TM.PostCode + Environment.NewLine;
            }
            //if (!string.IsNullOrEmpty(TM.PostCode))
            //{
            //    Address = Address + " " + TM.PostCode;
            //}
            if (TM.Country != null && !string.IsNullOrEmpty(TM.Country.Name))
            {
                Address = Address + TM.Country.Name + Environment.NewLine + Environment.NewLine;
            }
            if (!string.IsNullOrEmpty(TM.Email))
            {
                Address = Address + "Email: " + TM.Email + Environment.NewLine;
            }
            if (!string.IsNullOrEmpty(TM.Phone))
            {
                Address = Address + "Telephone No: " + "(+" + TM.Country.CountryPhoneCode + ") " + TM.Phone;
            }

            return Address;
        }

        public static string GetUserType(int CreatedBy)
        {
            var type = dbContext.UserAdditionals.Where(p => p.UserId == CreatedBy).FirstOrDefault();
            if (type != null)
            {
                return type.UserType;
            }
            else
            {
                return null;
            }
        }

        public static CustomerCompanyDetail GetCustomerCompnay(int CreatedBy)
        {
            CustomerCompanyDetail detail = new CustomerCompanyDetail();
            var company = dbContext.CustomerCompanyDetails.Where(p => p.UserId == CreatedBy).FirstOrDefault();
            if (company != null)
            {
                detail.LogoFileName = company.LogoFileName;
                detail.CompanyName = company.CompanyName;
                detail.MainWebsite = company.MainWebsite;
                detail.SiteAddress = company.SiteAddress;
                detail.UserPosition = company.UserPosition;
                detail.OperationStaff = company.OperationStaff;
                detail.OperationStaffPhone = company.OperationStaffPhone;
                detail.OperationStaffEmail = company.OperationStaffEmail;
                detail.KindRegards = company.KindRegards;
            }
            return detail;
        }       
    }
}
