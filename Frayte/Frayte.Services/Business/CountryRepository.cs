using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Business
{
    public class CountryRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        public List<FrayteCountry> GetCountryList()
        {
            var lstCountry = dbContext.Countries.ToList();

            var lstCountryDocument = dbContext.CountryDocuments.ToList();

            List<FrayteCountry> lstFrayteCountry = new List<FrayteCountry>();

            foreach (Country country in lstCountry)
            {
                FrayteCountry frayteCountry = new FrayteCountry();
                frayteCountry.Air = lstCountryDocument.Where(p => p.CountryId == country.CountryId && p.ShipmentType == FrayteShipmentType.Air).ToList().Count > 0 ? true : false;
                frayteCountry.Courier = lstCountryDocument.Where(p => p.CountryId == country.CountryId && p.ShipmentType == FrayteShipmentType.Courier).ToList().Count > 0 ? true : false;
                frayteCountry.Expryes = lstCountryDocument.Where(p => p.CountryId == country.CountryId && p.ShipmentType == FrayteShipmentType.Expryes).ToList().Count > 0 ? true : false;
                frayteCountry.Sea = lstCountryDocument.Where(p => p.CountryId == country.CountryId && p.ShipmentType == FrayteShipmentType.Sea).ToList().Count > 0 ? true : false;

                frayteCountry.CountryId = country.CountryId;
                frayteCountry.Name = country.CountryName;
                frayteCountry.Code = country.CountryCode;

                frayteCountry.CountryDocuments = new List<CountryDocument>();

                lstFrayteCountry.Add(frayteCountry);
            }

            return lstFrayteCountry;
        }

        public List<FrayteCountryCode> ListToCountry()
        {
            var lstFrayteCountry = (from r in dbContext.Countries
                                    join w in dbContext.Warehouses on r.CountryId equals w.CountryId
                                    join tz in dbContext.Timezones on r.TimeZoneId equals tz.TimezoneId
                                    select new FrayteCountryCode
                                    {
                                        Name = r.CountryName,
                                        Code = r.CountryCode,
                                        Code2 = r.CountryCode2,
                                        CountryId = r.CountryId,
                                        TimeZoneDetail = new TimeZoneModal()
                                        {
                                            Name = tz.Name,
                                            Offset = tz.Offset,
                                            TimezoneId = tz.TimezoneId,
                                            OffsetShort = tz.OffsetShort
                                        }
                                    }).ToList();
            return lstFrayteCountry;
        }

        public FrayteCountryCode GetCountryByTimezone(int timezoneId)
        {
            var country = dbContext.Countries.Where(p => p.TimeZoneId == timezoneId).FirstOrDefault();
            FrayteCountryCode frayteCountry = new FrayteCountryCode();
            if (country != null)
            {
                frayteCountry.CountryId = country.CountryId;
                frayteCountry.Name = country.CountryName;
                frayteCountry.Code = country.CountryCode;
                frayteCountry.TimeZoneDetail = new TimeZoneModal();
                if (country.TimeZoneId != null && country.TimeZoneId > 0)
                {
                    var timeZone = dbContext.Timezones.Find(country.TimeZoneId.Value);
                    if (timeZone != null)
                    {
                        frayteCountry.TimeZoneDetail.TimezoneId = timeZone.TimezoneId;
                        frayteCountry.TimeZoneDetail.Name = timeZone.Name;
                        frayteCountry.TimeZoneDetail.OffsetShort = timeZone.OffsetShort;
                        frayteCountry.TimeZoneDetail.Offset = timeZone.Offset;
                    }
                }
                return frayteCountry;
            }


            return null;
        }

        public List<FrayteCountryCode> lstCountry()
        {
            var lstCountry = dbContext.Countries.ToList();

            List<FrayteCountryCode> lstFrayteCountry = new List<FrayteCountryCode>();

            foreach (Country country in lstCountry)
            {
                FrayteCountryCode frayteCountry = new FrayteCountryCode();
                frayteCountry.CountryId = country.CountryId;
                frayteCountry.Name = country.CountryName;
                frayteCountry.Code = country.CountryCode;
                frayteCountry.Code2 = country.CountryCode2;
                frayteCountry.TimeZoneDetail = new TimeZoneModal();
                if (country.TimeZoneId != null && country.TimeZoneId > 0)
                {
                    var timeZone = dbContext.Timezones.Find(country.TimeZoneId.Value);
                    if (timeZone != null)
                    {
                        frayteCountry.TimeZoneDetail.TimezoneId = timeZone.TimezoneId;
                        frayteCountry.TimeZoneDetail.Name = timeZone.Name;
                        frayteCountry.TimeZoneDetail.OffsetShort = timeZone.OffsetShort;
                        frayteCountry.TimeZoneDetail.Offset = timeZone.Offset;
                    }
                }

                lstFrayteCountry.Add(frayteCountry);
            }

            return lstFrayteCountry;
        }

        public FrayteResult DeleteCountryHoliday(int countryHolydayId)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                var publicHoliday = dbContext.CountryPublicHolidays.Find(countryHolydayId);
                if (publicHoliday != null)
                {
                    dbContext.CountryPublicHolidays.Remove(publicHoliday);
                    dbContext.SaveChanges();
                    result.Status = true;
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;

            }

            return result;
        }

        public List<FrayteCountryPhoneCode> GetCountryPhoneCodeList()
        {
            var lstCountry = dbContext.Countries.ToList();

            List<FrayteCountryPhoneCode> lstFrayteCountry = new List<FrayteCountryPhoneCode>();

            foreach (Country country in lstCountry)
            {
                //var result = lstFrayteCountry.Where(p => p.PhoneCode == country.CountryPhoneCode).FirstOrDefault();
                //if (result == null)
                //{
                FrayteCountryPhoneCode frayteCountry = new FrayteCountryPhoneCode();
                //switch (country.CountryPhoneCode)
                //{
                //    case "1":
                //        frayteCountry.Name = "United States & Canada";
                //        break;
                //    case "212":
                //        frayteCountry.Name = "Morocco & Western Sahara";
                //        break;
                //    case "262":
                //        frayteCountry.Name = "Mayotte & Reunion";
                //        break;
                //    case "47":
                //        frayteCountry.Name = "Svalbard and Jan Mayen & Norway";
                //        break;
                //    case "590":
                //        frayteCountry.Name = "Saint Barthelemy & Saint Martin";
                //        break;
                //    case "599":
                //        frayteCountry.Name = "Netherlands Antilles & Curacao";
                //        break;
                //    case "61":
                //        frayteCountry.Name = "Australia, Christmas Island & Cocos Islands";
                //        break;
                //    case "64":
                //        frayteCountry.Name = "New Zealand & Pitcairn";
                //        break;
                //    case "7":

                //        frayteCountry.Name = "Russia & Kazakhstan";
                //        break;
                //    default:
                //        frayteCountry.Name = country.CountryName;
                //        break;
                //}
                frayteCountry.Name = country.CountryName;
                frayteCountry.PhoneCode = country.CountryPhoneCode;
                frayteCountry.CountryCode = country.CountryCode;

                lstFrayteCountry.Add(frayteCountry);
                //}
            }

            return lstFrayteCountry;
        }

        public List<FrayteCountryDocument> GetCountyDocumentList(int countryId)
        {

            List<FrayteCountryDocument> lstCountryDocuments = new List<FrayteCountryDocument>();

            lstCountryDocuments = (from cd in dbContext.CountryDocuments
                                   where cd.CountryId == countryId
                                   select new FrayteCountryDocument()
                                   {
                                       CountryId = countryId,
                                       CountryDocumentId = cd.CountryDocumentId,
                                       DocumentName = cd.DocumentName,
                                       ShipmentType = cd.ShipmentType
                                   }
                                    ).ToList();

            return lstCountryDocuments;
        }

        public List<FrayteCountryPublicHoliday> GetCountyPublicHolidayList(int countryId)
        {

            List<FrayteCountryPublicHoliday> lstCountryPublicHoliday = new List<FrayteCountryPublicHoliday>();

            lstCountryPublicHoliday = (from cd in dbContext.CountryPublicHolidays
                                       where cd.CountryId == countryId
                                       select new FrayteCountryPublicHoliday()
                                       {
                                           CountryPublicHolidayId = cd.CountryPublicHolidayId,
                                           Description = cd.Description,
                                           PublicHolidayDate = cd.PublicHolidayDate
                                       }
                                    ).ToList();

            return lstCountryPublicHoliday;
        }

        public FrayteCountry SaveCountry(FrayteCountry frayteCountry)
        {
            Country saveCountry;

            if (frayteCountry.CountryId > 0)
            {
                saveCountry = dbContext.Countries.Where(p => p.CountryId == frayteCountry.CountryId).FirstOrDefault();

                saveCountry.CountryName = frayteCountry.Name;
                saveCountry.CountryCode = frayteCountry.Code;
            }
            else
            {
                saveCountry = new Country();

                saveCountry.CountryName = frayteCountry.Name;
                saveCountry.CountryCode = frayteCountry.Code;

                dbContext.Countries.Add(saveCountry);
            }

            dbContext.SaveChanges();
            frayteCountry.CountryId = saveCountry.CountryId;

            //After saving the country information, we need to save its document information
            if (frayteCountry.CountryDocuments != null && frayteCountry.CountryDocuments.Count > 0)
            {

                foreach (CountryDocument document in frayteCountry.CountryDocuments)
                {
                    CountryDocument saveDocument;
                    if (document.CountryDocumentId > 0)
                    {
                        saveDocument = dbContext.CountryDocuments.Where(p => p.CountryDocumentId == document.CountryDocumentId).FirstOrDefault();

                        saveDocument.DocumentName = document.DocumentName;
                        saveDocument.ShipmentType = document.ShipmentType;
                    }
                    else
                    {
                        saveDocument = new CountryDocument();

                        saveDocument.CountryId = frayteCountry.CountryId;
                        saveDocument.DocumentName = document.DocumentName;
                        saveDocument.ShipmentType = document.ShipmentType;

                        dbContext.CountryDocuments.Add(saveDocument);
                    }

                    dbContext.SaveChanges();
                    document.CountryDocumentId = saveDocument.CountryDocumentId;
                }
            }

            if (frayteCountry.CountryPublicHolidays != null && frayteCountry.CountryPublicHolidays.Count > 0)
            {

                foreach (var document in frayteCountry.CountryPublicHolidays)
                {
                    CountryPublicHoliday countrypublicholiday;
                    if (document.CountryPublicHolidayId > 0)
                    {
                        countrypublicholiday = dbContext.CountryPublicHolidays.Where(p => p.CountryPublicHolidayId == document.CountryPublicHolidayId).FirstOrDefault();

                        countrypublicholiday.CountryId = frayteCountry.CountryId;
                        countrypublicholiday.Description = document.Description;
                        countrypublicholiday.PublicHolidayDate = document.PublicHolidayDate;
                    }
                    else
                    {
                        countrypublicholiday = new CountryPublicHoliday();

                        countrypublicholiday.CountryId = frayteCountry.CountryId;
                        countrypublicholiday.Description = document.Description;
                        countrypublicholiday.PublicHolidayDate = document.PublicHolidayDate;

                        dbContext.CountryPublicHolidays.Add(countrypublicholiday);
                    }

                    dbContext.SaveChanges();
                    document.CountryPublicHolidayId = countrypublicholiday.CountryPublicHolidayId;
                }
            }

            return frayteCountry;
        }

        public FrayteResult DeleteCountry(int countryId)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                //Step 1: First remove all the documents related to the country
                var lstCountryDocument = dbContext.CountryDocuments.Where(p => p.CountryId == countryId).ToList();

                if (lstCountryDocument != null && lstCountryDocument.Count > 0)
                {
                    foreach (CountryDocument document in lstCountryDocument)
                    {
                        dbContext.CountryDocuments.Attach(document);
                        dbContext.CountryDocuments.Remove(document);
                        dbContext.SaveChanges();
                    }
                }

                //Step 2: After deleting all the country documents, delete the country
                var country = new Country { CountryId = countryId };
                dbContext.Countries.Attach(country);
                dbContext.Countries.Remove(country);
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

        public FrayteResult DeleteCountryDocument(int countryDocumentId)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                var countryDocument = new CountryDocument { CountryDocumentId = countryDocumentId };
                dbContext.CountryDocuments.Attach(countryDocument);
                dbContext.CountryDocuments.Remove(countryDocument);
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

        public FrayteResult DeleteCountryPublicHoliday(int countrypublcHolidayId)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                var countryPublicHoliday = new CountryPublicHoliday { CountryPublicHolidayId = countrypublcHolidayId };

                dbContext.CountryPublicHolidays.Attach(countryPublicHoliday);
                dbContext.CountryPublicHolidays.Remove(countryPublicHoliday);
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

        public FrayteCountry GetCountryDetail(int countryId)
        {
            FrayteCountry frayteCountry = new FrayteCountry();
            var country = dbContext.Countries.Where(p => p.CountryId == countryId).FirstOrDefault();

            if (country != null)
            {
                var countryDocument = dbContext.CountryDocuments.Where(p => p.CountryId == countryId).ToList();

                //To Dos: Need to add public holiday information here
                var countryPublicHoliday = new List<FrayteCountryPublicHoliday>();

                frayteCountry.Air = countryDocument.Where(p => p.CountryId == country.CountryId && p.ShipmentType == FrayteShipmentType.Air).ToList().Count > 0 ? true : false;
                frayteCountry.Courier = countryDocument.Where(p => p.CountryId == country.CountryId && p.ShipmentType == FrayteShipmentType.Courier).ToList().Count > 0 ? true : false;
                frayteCountry.Expryes = countryDocument.Where(p => p.CountryId == country.CountryId && p.ShipmentType == FrayteShipmentType.Expryes).ToList().Count > 0 ? true : false;
                frayteCountry.Sea = countryDocument.Where(p => p.CountryId == country.CountryId && p.ShipmentType == FrayteShipmentType.Sea).ToList().Count > 0 ? true : false;

                frayteCountry.CountryId = country.CountryId;
                frayteCountry.Name = country.CountryName;
                frayteCountry.Code = country.CountryCode;

                frayteCountry.CountryDocuments = new List<CountryDocument>();
                frayteCountry.CountryDocuments = countryDocument;

                frayteCountry.CountryPublicHolidays = new List<FrayteCountryPublicHoliday>();
                frayteCountry.CountryPublicHolidays = GetCountyPublicHolidayList(countryId);

            }

            return frayteCountry;
        }

        public Country GetCountryByNameAndContryCode(string countryName, string countryCode)
        {
            var result = dbContext.Countries.FirstOrDefault(c => c.CountryName == countryName || c.CountryCode == countryCode);
            return result;
        }
    }
}
