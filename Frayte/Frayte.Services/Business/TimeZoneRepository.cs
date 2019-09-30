using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Business
{
    public class TimeZoneRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        public List<FrayteTimeZone> GetTimeZoneList()
        {
            var lstTimeZone = (from c in dbContext.Timezones
                               select new FrayteTimeZone()
                               {
                                   TimezoneId = c.TimezoneId,
                                   Name = c.Name,
                                   Offset = c.Offset,
                                   OffsetShort = c.OffsetShort
                               }).ToList();

            return lstTimeZone;
        }

        public List<TimeZoneModal> GetShipmentTimeZones()
        {
            var lstTimeZone = (from c in dbContext.Timezones
                               select new TimeZoneModal()
                               {
                                   TimezoneId = c.TimezoneId,
                                   Name = c.Name,
                                   Offset = c.Offset,
                                   OffsetShort = c.OffsetShort
                               }).ToList();

            return lstTimeZone;
        }

        public FrayteTimeZone SaveTimeZone(FrayteTimeZone timezone)
        {
            Timezone newTimeZone;
            if (timezone.TimezoneId > 0)
            {
                newTimeZone = dbContext.Timezones.Where(p => p.TimezoneId == timezone.TimezoneId).FirstOrDefault();

                newTimeZone.Name = timezone.Name;
                newTimeZone.Offset = timezone.Offset;
                newTimeZone.OffsetShort = timezone.OffsetShort;
            }
            else
            {
                newTimeZone = new Timezone();
                newTimeZone.Name = timezone.Name;
                newTimeZone.Offset = timezone.Offset;
                newTimeZone.OffsetShort = timezone.OffsetShort;
                dbContext.Timezones.Add(newTimeZone);
            }

            dbContext.SaveChanges();

            timezone.TimezoneId = newTimeZone.TimezoneId;

            return timezone;
        }

        public FrayteResult DeleteTimeZone(int timezoneId)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                var timezone = new Timezone { TimezoneId = timezoneId };
                dbContext.Timezones.Attach(timezone);
                dbContext.Timezones.Remove(timezone);
                dbContext.SaveChanges();
                result.Status = true;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Errors = new List<string>();
                result.Errors.Add(ex.Message);
            }

            return result;
        }

        public TimeZoneModal GetOperationTimezone()
        {
            FrayteOperationZone operationZone = UtilityRepository.GetOperationZone();

            var timeZone = (from tz in dbContext.Timezones join 
                                 c in dbContext.Countries on tz.TimezoneId equals c.TimeZoneId                                
                            where c.CountryCode == operationZone.OperationZoneName
                            select new TimeZoneModal()
                               {
                                   TimezoneId = tz.TimezoneId,
                                   Name = tz.Name,
                                   Offset = tz.Offset,
                                   OffsetShort = tz.OffsetShort
                               }).FirstOrDefault();

            return timeZone;
        }
    }
}
