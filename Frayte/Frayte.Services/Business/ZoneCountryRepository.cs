using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using Frayte.Services.Utility;
using System.Data.Entity.Validation;

namespace Frayte.Services.Business
{
    public class ZoneCountryRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        public void AddCountryZone(FrayteZoneCountry zone)
        {
            try
            {
                if (zone != null)
                {
                    //Step 1: Get the list of all the countries related to the zone from db.
                    var list = (from zoneCount in dbContext.LogisticServiceZoneCountries
                                where zoneCount.LogisticServiceZoneId == zone.ZoneId
                                select zoneCount).ToList();

                    //Step 2: Remove Countries, for this need to find the country which is in db list but not in UI list
                    var result = list.Where(p => !zone.Fraytezonezountry.Any(p2 => p2.CountryId == p.CountryId)).ToList();
                    foreach (var removeObj in result)
                    {
                        LogisticServiceZoneCountry removeData = dbContext.LogisticServiceZoneCountries.Where(p => p.CountryId == removeObj.CountryId && p.LogisticServiceZoneId == removeObj.LogisticServiceZoneId).FirstOrDefault();
                        if (removeData != null)
                        {
                            dbContext.LogisticServiceZoneCountries.Remove(removeData);
                            dbContext.SaveChanges();
                        }
                    }

                    //Step 3: Add Countries, for this need to find the countries which is in UI list but not in db list
                    var result1 = zone.Fraytezonezountry.Where(p => !list.Any(p2 => p2.CountryId == p.CountryId)).ToList();
                    foreach (var addObj in result1)
                    {
                        LogisticServiceZoneCountry newZoneCountry = new LogisticServiceZoneCountry();
                        newZoneCountry.LogisticServiceZoneId = zone.ZoneId;
                        newZoneCountry.CountryId = addObj.CountryId;
                        newZoneCountry.OperationZoneId = zone.OperationZoneId;
                        newZoneCountry.TransitTime = addObj.TransitTime;
                        dbContext.LogisticServiceZoneCountries.Add(newZoneCountry);
                        dbContext.SaveChanges();
                    }

                    //Step 4: Update Transit Time For Zone Country
                    var transitlist = (from zoneCount in dbContext.LogisticServiceZoneCountries
                                       where zoneCount.LogisticServiceZoneId == zone.ZoneId
                                       select zoneCount).ToList();

                    if (transitlist.Count > 0)
                    {
                        int j = 0;
                        foreach (var ll in transitlist)
                        {
                            if (ll.LogisticZoneCountryId > 0)
                            {
                                var transit = dbContext.LogisticServiceZoneCountries.Where(p => p.LogisticZoneCountryId == ll.LogisticZoneCountryId).FirstOrDefault();
                                if (transit != null)
                                {
                                    transit.LogisticZoneCountryId = ll.LogisticZoneCountryId;
                                    transit.TransitTime = zone.Fraytezonezountry[j].TransitTime;
                                    dbContext.Entry(transit).State = System.Data.Entity.EntityState.Modified;
                                    dbContext.SaveChanges();
                                }
                                j++;
                            }                            
                        }
                    }
                }
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var entityValidationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in entityValidationErrors.ValidationErrors)
                    {
                        string ss = "Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage;
                    }
                }
            }
        }

        public List<FrayteOperationZone> GetOperationZone()
        {
            List<FrayteOperationZone> _operation = new List<FrayteOperationZone>();
            var operation = dbContext.OperationZones.ToList();
            FrayteOperationZone fz;
            foreach (var rr in operation)
            {
                fz = new FrayteOperationZone();
                fz.OperationZoneId = rr.OperationZoneId;
                fz.OperationZoneName = rr.Name;
                _operation.Add(fz);
            }
            return _operation;
        }

        public List<FrayteDirectBookingCountry> GetDirectBookingCountry(int OperationZoneId, int LogisticZoneId, string LogisticType, string CourierCompany, string RateType, string ModuleType)
        {
            List<FrayteDirectBookingCountry> _country = new List<FrayteDirectBookingCountry>();
            try
            {
                //Step 1: Get OpeartionZoneId wise country list from ZoneCountry Table
                var zonecountry = (from ls in dbContext.LogisticServices
                                   join zz in dbContext.LogisticServiceZones on ls.LogisticServiceId equals zz.LogisticServiceId
                                   join zc in dbContext.LogisticServiceZoneCountries on zz.LogisticServiceZoneId equals zc.LogisticServiceZoneId
                                   join cc in dbContext.Countries on zc.CountryId equals cc.CountryId
                                   where ls.OperationZoneId == OperationZoneId &&
                                         ls.LogisticCompany == CourierCompany &&
                                         ls.LogisticType == LogisticType &&
                                         ls.RateType == RateType &&
                                         ls.ModuleType == ModuleType &&
                                         zc.LogisticZoneCountryId > 0
                                   select new
                                   {
                                       zc.LogisticServiceZoneId,
                                       cc.CountryId,
                                       cc.CountryName
                                   }).ToList();

                //Step 2: Get All Country from zonecountry list which belongs to LogisticZoneId
                var country = (from c in zonecountry
                               where c.LogisticServiceZoneId == LogisticZoneId
                               select new
                               {
                                   c.LogisticServiceZoneId,
                                   c.CountryId,
                                   c.CountryName
                               }).ToList();

                //Step 3: Make One new list which exclude country name from zonecountry which exixts in country list                
                var result = zonecountry.Except(country).ToList();

                //Step 4: Make Final Country List
                if (country != null && country.Count > 0)
                {
                    var final = dbContext.Countries.ToList().Where(p => !result.Any(x => x.CountryId == p.CountryId)).ToList();

                    FrayteDirectBookingCountry fd;
                    foreach (var cc in final)
                    {
                        fd = new FrayteDirectBookingCountry();
                        fd.CountryId = cc.CountryId;
                        fd.CountryName = cc.CountryName;
                        _country.Add(fd);
                    }
                }
                else
                {
                    FrayteDirectBookingCountry fd;
                    var final = dbContext.Countries.ToList();
                    foreach (var cc in final)
                    {
                        fd = new FrayteDirectBookingCountry();
                        fd.CountryId = cc.CountryId;
                        fd.CountryName = cc.CountryName;
                        _country.Add(fd);
                    }
                }
                return _country;
            }
            catch (Exception ex)
            {
                return null;
            }

            return _country;
        }

        public List<FrayteZoneCountry> GetZoneCountryDetail(int OperationZoneId, string LogisticType, string CourierCompany, string RateType, string ModuleType)
        {
            List<FrayteZoneCountry> _frayte = new List<FrayteZoneCountry>();
            try
            {
                //Step 1: Get all zones for selected operation zone and logistic type
                var lstZones = (from ls in dbContext.LogisticServices
                                join lsz in dbContext.LogisticServiceZones on ls.LogisticServiceId equals lsz.LogisticServiceId
                                join oz in dbContext.OperationZones on ls.OperationZoneId equals oz.OperationZoneId
                                where ls.OperationZoneId == OperationZoneId &&
                                      ls.LogisticType == LogisticType &&
                                      ls.LogisticCompany == CourierCompany &&
                                      ls.RateType == RateType &&
                                      ls.ModuleType == ModuleType
                                select new FrayteZoneCountry()
                                {
                                    ZoneId = lsz.LogisticServiceZoneId,
                                    ZoneName = lsz.ZoneName,
                                    ZoneDisplayName = lsz.ZoneDisplayName,
                                    OperationZoneId = oz.OperationZoneId,
                                    OperationZoneName = oz.Name
                                }).ToList();

                //Step 2: Fore each zone, list out all the zone countries.
                foreach (var zoneDetail in lstZones)
                {
                    zoneDetail.Fraytezonezountry = new List<FrayteZoneCountryDetail>();

                    var zoneCountries = (from zc in dbContext.LogisticServiceZoneCountries
                                         join c in dbContext.Countries on zc.CountryId equals c.CountryId
                                         where zc.LogisticServiceZoneId == zoneDetail.ZoneId
                                         select new
                                         {
                                             c.CountryId,
                                             c.CountryName,
                                             zc.TransitTime
                                         }).ToList();

                    foreach (var zoneCountry in zoneCountries)
                    {
                        FrayteZoneCountryDetail frayteZoneCountryDetail = new FrayteZoneCountryDetail();
                        frayteZoneCountryDetail.CountryId = zoneCountry.CountryId;
                        frayteZoneCountryDetail.CountryName = zoneCountry.CountryName;
                        frayteZoneCountryDetail.TransitTime = zoneCountry.TransitTime;
                        frayteZoneCountryDetail.IsSelected = true;
                        zoneDetail.Fraytezonezountry.Add(frayteZoneCountryDetail);
                    }

                    _frayte.Add(zoneDetail);
                }

                return (from ff in _frayte orderby ff.ZoneId ascending select ff).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
