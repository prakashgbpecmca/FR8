using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using Frayte.Services.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Business
{
    public class ZoneCountryPostCodeRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        public List<FrayteZoneCountryPostCode> ZoneCountryPostCode(int OperationZoneId, string LogisticCompany, string LogisticType, string RateType)
        {
            try
            {
                var item = (from lzcpc in dbContext.LogisticZoneCountryPostCodes
                            join lszc in dbContext.LogisticServiceZoneCountries on lzcpc.LogisticZoneCountryId equals lszc.LogisticZoneCountryId
                            join cc in dbContext.Countries on lszc.CountryId equals cc.CountryId
                            join lsz in dbContext.LogisticServiceZones on lszc.LogisticServiceZoneId equals lsz.LogisticServiceZoneId
                            join ls in dbContext.LogisticServices on lsz.LogisticServiceId equals ls.LogisticServiceId
                            where ls.OperationZoneId == OperationZoneId &&
                                  ls.LogisticCompany == LogisticCompany &&
                                  ls.LogisticType == LogisticType &&
                                  ls.RateType == RateType
                            select new FrayteZoneCountryPostCode
                            {
                                LogisticZoneCountryPostCodeId = lzcpc.LogisticZoneCountryPostCodeId,
                                LogisticCompany = ls.LogisticCompany,
                                LogisticType = ls.LogisticType,
                                RateType = ls.RateType,
                                OperationZoneId = ls.OperationZoneId,
                                CountryName = cc.CountryName,
                                FromPostCode = lzcpc.FromPostCode,
                                ToPostCode = lzcpc.ToPostCode,
                                Zone = lsz.ZoneName,
                                ZoneDisplay = lsz.ZoneDisplayName
                            }).ToList();

                return item;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public FrayteResult AddZoneCountryPostCode(FrayteZoneCountryPostCode zonepostcode)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                if (zonepostcode != null)
                {
                    int countryId = dbContext.Countries.Where(p => p.CountryName == zonepostcode.CountryName).Select(p => p.CountryId).FirstOrDefault();
                    if (countryId > 0)
                    {
                        if (zonepostcode.LogisticZoneCountryPostCodeId > 0)
                        {
                            var postcode = dbContext.LogisticZoneCountryPostCodes.Where(p => p.LogisticZoneCountryPostCodeId == zonepostcode.LogisticZoneCountryPostCodeId).FirstOrDefault();
                            if (postcode != null)
                            {
                                if (UtilityRepository.LogisticZoneCountryId(zonepostcode.OperationZoneId, countryId, zonepostcode.LogisticCompany, zonepostcode.LogisticType, zonepostcode.RateType, zonepostcode.Zone) > 0)
                                {
                                    postcode.LogisticZoneCountryId = UtilityRepository.LogisticZoneCountryId(zonepostcode.OperationZoneId, countryId, zonepostcode.LogisticCompany, zonepostcode.LogisticType, zonepostcode.RateType, zonepostcode.Zone);
                                    postcode.FromPostCode = zonepostcode.FromPostCode;
                                    postcode.ToPostCode = zonepostcode.ToPostCode;
                                    dbContext.Entry(postcode).State = System.Data.Entity.EntityState.Modified;
                                    dbContext.SaveChanges();
                                    result.Status = true;
                                }
                                else
                                {
                                    if (postcode.FromPostCode == zonepostcode.FromPostCode && postcode.ToPostCode == zonepostcode.ToPostCode)
                                    {
                                        result.Status = false;
                                    }
                                    else
                                    {
                                        var servicezone = (from ls in dbContext.LogisticServices
                                                           join lsz in dbContext.LogisticServiceZones on ls.LogisticServiceId equals lsz.LogisticServiceId
                                                           where ls.OperationZoneId == zonepostcode.OperationZoneId &&
                                                                 ls.LogisticCompany == zonepostcode.LogisticCompany &&
                                                                 ls.LogisticType == zonepostcode.LogisticType &&
                                                                 ls.RateType == (zonepostcode.RateType == null ? null : zonepostcode.RateType) &&
                                                                 lsz.ZoneName == zonepostcode.Zone
                                                           select new
                                                           {
                                                               lsz.LogisticServiceZoneId
                                                           }).FirstOrDefault();

                                        LogisticServiceZoneCountry lszc = new LogisticServiceZoneCountry();
                                        lszc.OperationZoneId = zonepostcode.OperationZoneId;
                                        lszc.LogisticServiceZoneId = servicezone.LogisticServiceZoneId;
                                        lszc.CountryId = countryId;
                                        dbContext.LogisticServiceZoneCountries.Add(lszc);
                                        if (lszc != null)
                                        {
                                            dbContext.SaveChanges();
                                        }

                                        LogisticZoneCountryPostCode lzcpc = new LogisticZoneCountryPostCode();
                                        lzcpc.LogisticZoneCountryId = lszc.LogisticZoneCountryId;
                                        lzcpc.FromPostCode = zonepostcode.FromPostCode;
                                        lzcpc.ToPostCode = zonepostcode.ToPostCode;
                                        dbContext.LogisticZoneCountryPostCodes.Add(lzcpc);
                                        if (lzcpc != null)
                                        {
                                            dbContext.SaveChanges();
                                        }

                                        result.Status = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            var servicezone = (from ls in dbContext.LogisticServices
                                               join lsz in dbContext.LogisticServiceZones on ls.LogisticServiceId equals lsz.LogisticServiceId
                                               join lszc in dbContext.LogisticServiceZoneCountries on lsz.LogisticServiceZoneId equals lszc.LogisticServiceZoneId
                                               where ls.OperationZoneId == zonepostcode.OperationZoneId &&
                                                     ls.LogisticCompany == zonepostcode.LogisticCompany &&
                                                     ls.LogisticType == zonepostcode.LogisticType &&
                                                     ls.RateType == (zonepostcode.RateType == null ? null : zonepostcode.RateType) &&
                                                     lsz.ZoneName == zonepostcode.Zone &&
                                                     lszc.CountryId == countryId
                                               select new
                                               {
                                                   lszc.LogisticZoneCountryId
                                               }).FirstOrDefault();

                            if (servicezone != null)
                            {
                                LogisticZoneCountryPostCode lzcpc = new LogisticZoneCountryPostCode();
                                lzcpc.LogisticZoneCountryId = servicezone.LogisticZoneCountryId;
                                lzcpc.FromPostCode = zonepostcode.FromPostCode;
                                lzcpc.ToPostCode = zonepostcode.ToPostCode;
                                dbContext.LogisticZoneCountryPostCodes.Add(lzcpc);
                                if (lzcpc != null)
                                {
                                    dbContext.SaveChanges();
                                }

                                result.Status = true;
                            }
                            else
                            {
                                LogisticServiceZoneCountry lszc = new LogisticServiceZoneCountry();
                                lszc.OperationZoneId = zonepostcode.OperationZoneId;
                                lszc.LogisticServiceZoneId = UtilityRepository.LogisticServiceZoneId(zonepostcode.OperationZoneId, zonepostcode.LogisticCompany, zonepostcode.LogisticType, zonepostcode.RateType, zonepostcode.Zone);
                                lszc.CountryId = countryId;
                                dbContext.LogisticServiceZoneCountries.Add(lszc);
                                if (lszc != null)
                                {
                                    dbContext.SaveChanges();
                                }

                                LogisticZoneCountryPostCode lzcpc = new LogisticZoneCountryPostCode();
                                lzcpc.LogisticZoneCountryId = lszc.LogisticZoneCountryId;
                                lzcpc.FromPostCode = zonepostcode.FromPostCode;
                                lzcpc.ToPostCode = zonepostcode.ToPostCode;
                                dbContext.LogisticZoneCountryPostCodes.Add(lzcpc);
                                if (lzcpc != null)
                                {
                                    dbContext.SaveChanges();
                                }

                                result.Status = true;
                            }
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
        }

        public FrayteResult RemoveLogisticZoneCountryPostCodeId(int LogisticZoneCountryPostCodeId)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                var item = dbContext.LogisticZoneCountryPostCodes.Where(p => p.LogisticZoneCountryPostCodeId == LogisticZoneCountryPostCodeId).FirstOrDefault();
                if (item != null)
                {
                    dbContext.LogisticZoneCountryPostCodes.Remove(item);
                    dbContext.SaveChanges();
                    result.Status = true;
                }
                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
        }
    }
}
