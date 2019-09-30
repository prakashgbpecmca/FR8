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
    public class ExchangeRateRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        public void SaveExchangeRate(List<FrayteExchangeRate> exchangeRate)
        {
            try
            {
                OperationZoneExchangeRate exRate;
                if (exchangeRate != null && exchangeRate.Count > 0)
                {
                    foreach (var rr in exchangeRate)
                    {
                        if (rr.OperationZoneExchangeRateId > 0)
                        {
                            exRate = dbContext.OperationZoneExchangeRates.Find(rr.OperationZoneExchangeRateId);
                            if (exRate != null)
                            {
                                exRate.OperationZoneId = rr.OperationZone.OperationZoneId;
                                if (rr.IsActive)
                                {
                                    exRate.ExchangeRate = rr.ExchangeRate;
                                }
                                else
                                {
                                    exRate.ExchangeRate = 0;
                                }
                                exRate.IsActive = rr.IsActive;
                                dbContext.Entry(exRate).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();
                            }

                            //Insert ExchangeRate In History
                            OperationZoneExchangeRateHistory hrate = new OperationZoneExchangeRateHistory();
                            hrate.OperationZoneId = rr.OperationZone.OperationZoneId;
                            hrate.CurrencyCode = rr.CurrencyDetail.CurrencyCode;
                            hrate.ExchangeRate = rr.ExchangeRate;
                            hrate.ExchangeType = rr.ExchangeType;
                            hrate.StartDate = DateTime.UtcNow.AddDays(-1);
                            hrate.FinishDate = DateTime.UtcNow;
                            dbContext.OperationZoneExchangeRateHistories.Add(hrate);
                            dbContext.SaveChanges();
                        }
                        else
                        {
                            exRate = dbContext.OperationZoneExchangeRates.Where(p => p.OperationZoneId == rr.OperationZone.OperationZoneId &&
                                                                                     p.CurrencyCode == rr.CurrencyDetail.CurrencyCode).FirstOrDefault();
                            if (exRate != null)
                            {
                                exRate.ExchangeRate = rr.IsActive ? rr.ExchangeRate : 0;
                                exRate.IsActive = rr.IsActive;
                                dbContext.SaveChanges();
                            }
                            else if (rr.IsActive)
                            {
                                exRate = new OperationZoneExchangeRate();
                                exRate.OperationZoneId = rr.OperationZone.OperationZoneId;
                                exRate.CurrencyCode = rr.CurrencyDetail.CurrencyCode;
                                exRate.ExchangeRate = rr.ExchangeRate;
                                exRate.ExchangeType = rr.ExchangeType;
                                exRate.IsActive = true;
                                dbContext.OperationZoneExchangeRates.Add(exRate);
                                dbContext.SaveChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void SaveExchnageRateHistory()
        {
            try
            {
                var rate = dbContext.OperationZoneExchangeRates.Where(p => p.IsActive == true).ToList();
                if (rate != null)
                {
                    OperationZoneExchangeRateHistory hrate;
                    foreach (var Obj in rate)
                    {
                        hrate = new OperationZoneExchangeRateHistory();
                        hrate.OperationZoneId = Obj.OperationZoneId;
                        hrate.CurrencyCode = Obj.CurrencyCode;
                        hrate.ExchangeRate = Obj.ExchangeRate;
                        hrate.ExchangeType = Obj.ExchangeType;
                        hrate.StartDate = DateTime.UtcNow.AddDays(-1);
                        hrate.FinishDate = DateTime.UtcNow;
                        dbContext.OperationZoneExchangeRateHistories.Add(hrate);
                        dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public FrayteStatus ExchangeRateHistoryUpdateStatus()
        {
            FrayteStatus fs = new FrayteStatus();
            //MM/dd/yyyy
            DateTime dt = Frayte.Services.CommonConversion.ConvertToDateTime(DateTime.UtcNow.Month.ToString() + "/" + DateTime.UtcNow.Day.ToString() + "/" + DateTime.UtcNow.Year.ToString());
            var item = dbContext.OperationZoneExchangeRateHistories.Where(p => p.FinishDate == dt.Date).ToList();
            if (item != null && item.Count > 0)
            {
                fs.HistoryStatus = true;
            }
            else
            {
                fs.HistoryStatus = false;
            }
            return fs;
        }

        public List<int> GetDistinctYear(int OperationZoneId, string Type)
        {
            try
            {
                List<int> year = dbContext.OperationZoneExchangeRateHistories.Where(p => p.OperationZoneId == OperationZoneId && p.ExchangeType == Type)
                                                                                        .Select(p => p.StartDate.Value.Year).Distinct()
                                                                                        .ToList();
                return year;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<FrayteExchangeMonth> GetDistinctMonth(int OperationZoneId, string Type)
        {
            List<FrayteExchangeMonth> _month = new List<FrayteExchangeMonth>();
            try
            {
                List<int> month = dbContext.OperationZoneExchangeRateHistories.Where(p => p.OperationZoneId == OperationZoneId && p.ExchangeType == Type)
                                                                                         .Select(p => p.StartDate.Value.Month).Distinct()
                                                                                         .ToList();

                if (month != null && month.Count > 0)
                {
                    FrayteExchangeMonth fraytemonth;
                    foreach (int Obj in month)
                    {
                        fraytemonth = new FrayteExchangeMonth();
                        switch (Obj)
                        {
                            case 1:
                                fraytemonth.MonthId = 1;
                                fraytemonth.MonthName = "January";
                                break;
                            case 2:
                                fraytemonth.MonthId = 2;
                                fraytemonth.MonthName = "February";
                                break;
                            case 3:
                                fraytemonth.MonthId = 3;
                                fraytemonth.MonthName = "March";
                                break;
                            case 4:
                                fraytemonth.MonthId = 4;
                                fraytemonth.MonthName = "April";
                                break;
                            case 5:
                                fraytemonth.MonthId = 5;
                                fraytemonth.MonthName = "May";
                                break;
                            case 6:
                                fraytemonth.MonthId = 6;
                                fraytemonth.MonthName = "June";
                                break;
                            case 7:
                                fraytemonth.MonthId = 7;
                                fraytemonth.MonthName = "July";
                                break;
                            case 9:
                                fraytemonth.MonthId = 8;
                                fraytemonth.MonthName = "August";
                                break;
                            case 8:
                                fraytemonth.MonthId = 9;
                                fraytemonth.MonthName = "September";
                                break;
                            case 10:
                                fraytemonth.MonthId = 10;
                                fraytemonth.MonthName = "October";
                                break;
                            case 11:
                                fraytemonth.MonthId = 11;
                                fraytemonth.MonthName = "November";
                                break;
                            case 12:
                                fraytemonth.MonthId = 12;
                                fraytemonth.MonthName = "December";
                                break;
                            default:
                                break;
                        }
                        _month.Add(fraytemonth);
                    }
                }
                return _month.OrderBy(x => x.MonthId).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<FrayteExchangeRateHistory> GetExchangeRateHistory(FrayteSearchExchangeHistory search)
        {
            try
            {
                List<FrayteExchangeRateHistory> _rate = new List<FrayteExchangeRateHistory>();
                var detail = dbContext.OperationZoneExchangeRateHistories.Where(p => p.OperationZoneId == search.OperationZoneId && p.ExchangeType == search.ExchangeType
                                                                                     && p.StartDate.Value.Year == search.Year && p.StartDate.Value.Month == search.MonthName.MonthId)
                                                                                     .ToList();

                if (detail != null && detail.Count > 0)
                {
                    FrayteExchangeRateHistory frayte;
                    foreach (var Obj in detail)
                    {
                        frayte = new FrayteExchangeRateHistory();
                        OperationZone oz = dbContext.OperationZones.Where(p => p.OperationZoneId == Obj.OperationZoneId.Value).FirstOrDefault();
                        if (oz != null)
                        {
                            frayte.OperationZone = new FrayteOperationZone();
                            {
                                frayte.OperationZone.OperationZoneId = oz.OperationZoneId;
                                frayte.OperationZone.OperationZoneName = oz.Name;
                            }
                        }
                        CurrencyType ct = dbContext.CurrencyTypes.Where(x => x.CurrencyCode == Obj.CurrencyCode).FirstOrDefault();
                        if (ct != null)
                        {
                            frayte.CurrencyDetail = new FrayteCurrencyDetail();
                            {
                                frayte.CurrencyDetail.CurrencyCode = ct.CurrencyCode;
                                frayte.CurrencyDetail.Description = ct.CurrencyDescription;
                            }
                        }
                        frayte.ExchangeType = Obj.ExchangeType;
                        if (Obj.ExchangeRate.HasValue)
                        {
                            frayte.ExchangeRate = Obj.ExchangeRate.Value.ToString();
                        }
                        if (Obj.StartDate.HasValue)
                        {
                            frayte.StartDate = Obj.StartDate.Value;
                        }
                        if (Obj.FinishDate.HasValue)
                        {
                            frayte.FinishDate = Obj.FinishDate.Value;
                        }
                        _rate.Add(frayte);
                    }
                }
                return _rate;
            }
            catch (Exception x)
            {
                return null;
            }
        }

        public List<FrayteExchangeRate> GetOperationExchangeRateDetail(int operationZoneId)
        {
            try
            {
                var exchangeRateList = (from er in dbContext.OperationZoneExchangeRates
                                        join
                                            oz in dbContext.OperationZones on er.OperationZoneId equals oz.OperationZoneId
                                        join
                                            ct in dbContext.CurrencyTypes on er.CurrencyCode equals ct.CurrencyCode
                                        where er.IsActive == true && er.OperationZoneId == operationZoneId
                                        select new FrayteExchangeRate()
                                        {
                                            OperationZoneExchangeRateId = er.OperationZoneExchangeRateId,
                                            OperationZone = new FrayteOperationZone()
                                            {
                                                OperationZoneId = er.OperationZoneId,
                                                OperationZoneName = oz.Name
                                            },
                                            CurrencyDetail = new FrayteCurrencyDetail()
                                            {
                                                CurrencyCode = ct.CurrencyCode,
                                                Description = ct.CurrencyDescription
                                            },
                                            ExchangeRate = er.ExchangeRate,
                                            ExchangeType = er.ExchangeType,
                                            IsActive = er.IsActive
                                        }).ToList();

                return exchangeRateList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<CurrencyType> GetCurrency()
        {
            var list = dbContext.CurrencyTypes.ToList();
            return list;
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

        public FrayteStatus GetSendMailStatus(int OperationZoneId)
        {
            FrayteStatus fs = new FrayteStatus();
            //int OperationZoneId = UtilityRepository.GetOperationZone().OperationZoneId;
            var item = dbContext.OperationZoneExchangeRates.Where(p => p.OperationZoneId == OperationZoneId).ToList();
            if (item != null)
            {
                foreach (var Obj in item)
                {
                    fs.CurrencyMailSentOn = Obj.MailSendOn == null ? DateTime.Parse("01/01/0001") : Obj.MailSendOn.Value;
                }
            }
            return fs;
        }

        public void UpdateSendMailStatus(int OperationZoneId)
        {
            //int OperationZoneId = UtilityRepository.GetOperationZone().OperationZoneId;
            var item = dbContext.OperationZoneExchangeRates.Where(p => p.OperationZoneId == OperationZoneId).ToList();
            if (item != null)
            {
                foreach (var Obj in item)
                {
                    Obj.MailSendOn = DateTime.UtcNow;
                    // Update
                    dbContext.Entry(Obj).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                }
            }
        }
    }
}
