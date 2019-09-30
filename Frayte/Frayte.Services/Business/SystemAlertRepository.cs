using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frayte.Services.Models;
using Frayte.Services.DataAccess;

namespace Frayte.Services.Business
{
    public class SystemAlertRepository
    {
        FrayteEntities dbContext = new FrayteEntities();
        public List<FrayteSystemAlert> GetSystemAlerts(TrackSystemAlert trackSystemAlert)
        {
            int SkipRows = 0;
            SkipRows = (trackSystemAlert.CurrentPage - 1) * trackSystemAlert.TakeRows;
            var result = dbContext.spGet_GetSystemAlerts(trackSystemAlert.OperationZoneId, trackSystemAlert.FromDate, trackSystemAlert.ToDate,
                                                         SkipRows, trackSystemAlert.TakeRows).ToList();
            List<FrayteSystemAlert> list = new List<FrayteSystemAlert>();
            if (result != null && result.Count > 0)
            {
                FrayteSystemAlert SystemAlertResult;
                foreach (var data in result)
                {
                    SystemAlertResult = new FrayteSystemAlert();
                    SystemAlertResult.SystemAlertId = data.SystemAlertId;
                    SystemAlertResult.OperationZoneId = data.OperationZoneId;
                    SystemAlertResult.Heading = data.Heading;
                    SystemAlertResult.Description = data.Description;
                    SystemAlertResult.FromDate = data.FromDate;
                    SystemAlertResult.FromTime = UtilityRepository.GetTimeZoneTime(data.FromTime);
                    SystemAlertResult.ToDate = data.ToDate;
                    SystemAlertResult.ToTime = UtilityRepository.GetTimeZoneTime(data.ToTime);
                    SystemAlertResult.IsActive = data.IsActive;
                    SystemAlertResult.TotalRows = data.TotalRows.Value;
                    SystemAlertResult.TimeZoneDetail = new TimeZoneModal();
                    SystemAlertResult.TimeZoneDetail.Name = data.TimeZoneName;
                    if (data.TimezoneId.HasValue)
                    {
                        SystemAlertResult.TimeZoneDetail.TimezoneId = data.TimezoneId.Value;
                    }
                    SystemAlertResult.TimeZoneDetail.Offset = data.TimeZoneOffset;
                    SystemAlertResult.TimeZoneDetail.OffsetShort = data.TimeZoneOffsetShort;
                    var TimeZoneInformation = TimeZoneInfo.FindSystemTimeZoneById(data.TimeZoneName);
                    SystemAlertResult.FromTime = UtilityRepository.UtcDateToOtherTimezone(data.FromDate, data.FromTime, TimeZoneInformation).Item2;
                    SystemAlertResult.FromDate = UtilityRepository.UtcDateToOtherTimezone(data.FromDate, data.FromTime, TimeZoneInformation).Item1;
                    SystemAlertResult.ToTime = UtilityRepository.UtcDateToOtherTimezone(data.ToDate, data.ToTime, TimeZoneInformation).Item2;
                    SystemAlertResult.ToDate = UtilityRepository.UtcDateToOtherTimezone(data.ToDate, data.ToTime, TimeZoneInformation).Item1;
                    list.Add(SystemAlertResult);
                }
            }
            return list;
        }

        public FrayteSystemAlert GetSystemAlertDetail(int operationZoneId, int systemAlertId)
        {
            var FrayteSystemAlertResult = (from r in dbContext.SystemAlerts
                                           join tz in dbContext.Timezones on r.TimeZoneId equals tz.TimezoneId
                                           where r.SystemAlertId == systemAlertId && r.OperationZoneId == operationZoneId
                                           select new FrayteSystemAlert()
                                           {
                                               SystemAlertId = r.SystemAlertId,
                                               Heading = r.Heading,
                                               FromDate = r.FromDate,
                                               FromTime = r.FromTime.ToString(),
                                               ToDate = r.ToDate,
                                               ToTime = r.ToTime.ToString(),
                                               Description = r.Description,
                                               OperationZoneId = r.OperationZoneId,
                                               IsActive = r.IsActive,
                                               TimeZoneDetail = new TimeZoneModal()
                                               {
                                                   TimezoneId = tz.TimezoneId,
                                                   Name = tz.Name,
                                                   Offset = tz.Offset,
                                                   OffsetShort = tz.OffsetShort
                                               }
                                           }).FirstOrDefault();

            return FrayteSystemAlertResult;
        }

        public FrayteResult DeleteSystemAlert(int systemAlertId)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                var systemAlert = dbContext.SystemAlerts.Find(systemAlertId);

                if (systemAlert != null)
                {
                    systemAlert.IsActive = false;
                    dbContext.Entry(systemAlert).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    result.Status = true;
                }
                else
                {
                    result.Status = false;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
            }

            return result;
        }

        public FrayteResult SaveUpdateSystemAlerts(FrayteSystemAlert frayteSystemAlert)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                if (frayteSystemAlert != null)
                {
                    SystemAlert systemAlert;
                    if (frayteSystemAlert.SystemAlertId == 0)
                    {
                        systemAlert = new SystemAlert();
                        systemAlert.IsActive = true;
                        systemAlert.Heading = frayteSystemAlert.Heading;
                        systemAlert.Description = frayteSystemAlert.Description;
                        systemAlert.OperationZoneId = frayteSystemAlert.OperationZoneId;
                        systemAlert.FromDate = UtilityRepository.ConvertDateTimetoUniversalTime(frayteSystemAlert.FromDate);
                        systemAlert.FromTime = UtilityRepository.TimeSpanConversion(systemAlert.FromDate.Hour, systemAlert.FromDate.Minute, systemAlert.FromDate.Second);
                        systemAlert.ToDate = UtilityRepository.ConvertDateTimetoUniversalTime(frayteSystemAlert.ToDate);
                        systemAlert.ToTime = UtilityRepository.TimeSpanConversion(systemAlert.ToDate.Hour, systemAlert.ToDate.Minute, systemAlert.ToDate.Second); ;
                        if (frayteSystemAlert.TimeZoneDetail != null)
                        {
                            systemAlert.TimeZoneId = frayteSystemAlert.TimeZoneDetail.TimezoneId;
                        }

                        dbContext.SystemAlerts.Add(systemAlert);
                        dbContext.SaveChanges();
                        result.Status = true;
                    }
                    else
                    {
                        systemAlert = dbContext.SystemAlerts.Find(frayteSystemAlert.SystemAlertId);
                        if (systemAlert != null)
                        {
                            systemAlert.FromDate = UtilityRepository.ConvertDateTimetoUniversalTime(frayteSystemAlert.FromDate);
                            systemAlert.FromTime = UtilityRepository.TimeSpanConversion(systemAlert.FromDate.Hour, systemAlert.FromDate.Minute, systemAlert.FromDate.Second);
                            systemAlert.ToDate = UtilityRepository.ConvertDateTimetoUniversalTime(frayteSystemAlert.ToDate);
                            systemAlert.ToTime = UtilityRepository.TimeSpanConversion(systemAlert.ToDate.Hour, systemAlert.ToDate.Minute, systemAlert.ToDate.Second); ;
                            systemAlert.Description = frayteSystemAlert.Description;
                            if (frayteSystemAlert.TimeZoneDetail != null)
                            {
                                systemAlert.TimeZoneId = frayteSystemAlert.TimeZoneDetail.TimezoneId;
                            }
                            systemAlert.Heading = frayteSystemAlert.Heading;
                            systemAlert.IsActive = frayteSystemAlert.IsActive;
                            dbContext.Entry(systemAlert).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                            result.Status = true;
                        }
                    }
                }
                else
                {
                    result.Status = false;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
            }

            return result;
        }

        public List<FrayteSystemAlert> GetSystemAlertsPublic(int operationZoneId, DateTime? currentDate)
        {
            List<FrayteSystemAlert> list = new List<FrayteSystemAlert>();
            if (operationZoneId > 0 && currentDate != null)
            {
                var CurrentDate = currentDate.Value.Date;
                var CurrentTime = currentDate.Value.TimeOfDay;
                list = (from r in dbContext.SystemAlerts
                        join tz in dbContext.Timezones on r.TimeZoneId equals tz.TimezoneId
                        where
                            r.OperationZoneId == operationZoneId &&
                            (currentDate == null || (CurrentDate <= r.ToDate && CurrentTime <= r.ToTime && CurrentDate >= r.FromDate && CurrentTime >= r.FromTime)) &&
                            r.IsActive == true
                        select new FrayteSystemAlert()
                        {
                            SystemAlertId = r.SystemAlertId,
                            Heading = r.Heading,
                            FromDate = r.FromDate,
                            FromTime = r.FromTime.ToString(),
                            ToDate = r.ToDate,
                            ToTime = r.ToTime.ToString(),
                            Description = r.Description,
                            OperationZoneId = r.OperationZoneId,
                            IsActive = r.IsActive,
                            TimeZoneDetail = new TimeZoneModal()
                            {
                                TimezoneId = tz.TimezoneId,
                                Name = tz.Name,
                                Offset = tz.Offset,
                                OffsetShort = tz.OffsetShort
                            }
                        }).ToList();
            }
            return list;
        }

        public FrayteSystemAlert GetSystemAlertDetailPublic(int operationZoneId, string systemAlertHeading)
        {
            var FrayteSystemAlertResult = (from r in dbContext.SystemAlerts
                                           join tz in dbContext.Timezones on r.TimeZoneId equals tz.TimezoneId
                                           where r.Heading == systemAlertHeading && r.OperationZoneId == operationZoneId
                                           select new FrayteSystemAlert()
                                           {
                                               SystemAlertId = r.SystemAlertId,
                                               Heading = r.Heading,
                                               FromDate = r.FromDate,
                                               FromTime = r.FromTime.ToString(),
                                               ToDate = r.ToDate,
                                               ToTime = r.ToTime.ToString(),
                                               Description = r.Description,
                                               OperationZoneId = r.OperationZoneId,
                                               IsActive = r.IsActive,
                                               TimeZoneDetail = new TimeZoneModal()
                                               {
                                                   TimezoneId = tz.TimezoneId,
                                                   Name = tz.Name,
                                                   Offset = tz.Offset,
                                                   OffsetShort = tz.OffsetShort
                                               }
                                           }).FirstOrDefault();

            return FrayteSystemAlertResult;
        }

        public FrayteResult SystemAlertHeadingAvailability(int operationZoneId, string systemAlertHeading)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                var data = dbContext.SystemAlerts.Where(p => p.OperationZoneId == operationZoneId && p.Heading == systemAlertHeading).FirstOrDefault();
                if (data == null)
                {
                    result.Status = true;
                }
                else
                {
                    result.Status = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                result.Status = false;
                return result;
            }
        }
    }
}
