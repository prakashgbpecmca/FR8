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
    public class BaseRateCardRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        public FrayteResult UpdateLogisticSerice(LogisticServiceDuration obj)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                var logisticService = dbContext.LogisticServices.Find(obj.LogisticServiceId);
                if (logisticService != null)
                {
                    logisticService.IssuedDate = obj.IssuedDate;
                    logisticService.ExpiryDate = obj.ExpiryDate;
                    dbContext.Entry(logisticService).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
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

        public LogisticServiceDuration GetLogisticSericeDuration(int OperationZoneId, string LogisticType, string CourierCompany, string RateType, string ModuleType)
        {
            LogisticServiceDuration result = new LogisticServiceDuration();
            try
            {
                var logisticService = dbContext.LogisticServices.Where(p => p.OperationZoneId == OperationZoneId &&
                p.ModuleType == ModuleType &&
                p.LogisticType == LogisticType &&
                p.LogisticCompany == CourierCompany &&
                p.RateType == RateType &&
                p.IsActive == true).FirstOrDefault();
                if (logisticService != null)
                {
                    result.LogisticServiceId = logisticService.LogisticServiceId;
                    result.IssuedDate = logisticService.IssuedDate.HasValue ? logisticService.IssuedDate.Value : DateTime.Now;
                    result.ExpiryDate = logisticService.ExpiryDate.HasValue ? logisticService.ExpiryDate.Value : DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return result;
        }

        public FrayteResult EditZoneRateCard(List<FrayteZoneBaseRateCard> _ratecard)
        {
            FrayteResult result = new Models.FrayteResult();
            try
            {
                LogisticServiceBaseRateCard lsbrc;
                if (_ratecard != null && _ratecard.Count > 0)
                {
                    foreach (var rate in _ratecard)
                    {
                        if (rate.IsChanged)
                        {
                            lsbrc = dbContext.LogisticServiceBaseRateCards.Find(rate.ZoneRateCardId);
                            if (lsbrc != null)
                            {
                                lsbrc.LogisticServiceZoneId = rate.zone.ZoneId;
                                lsbrc.LogisticServiceCourierAccountId = rate.courierAccount.LogisticServiceCourierAccountId;
                                lsbrc.LogisticRate = rate.Rate;
                                lsbrc.LogisticCurrency = rate.CurrencyCode;
                                dbContext.Entry(lsbrc).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();
                            }
                        }
                    }
                    result.Status = true;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Errors = new List<string>();
                result.Errors.Add(ex.Message);
            }

            return result;
        }

        public List<FryateZoneBaseRateCardLimit> GetZoneBaseRateCardLimit(int OperationZoneId, string LogisticType, string CourierComapny, string RateType, string PackageType, string ParcelType, string ModuleType)
        {
            LogisticServiceWeightLimit weightLimit = new LogisticServiceWeightLimit();
            List<LogisticServiceWeightLimit> _weightLimit = new List<LogisticServiceWeightLimit>();
            List<FryateZoneBaseRateCardLimit> frayeBaseRate = new List<FryateZoneBaseRateCardLimit>();

            try
            {
                if (LogisticType == FrayteLogisticType.UKShipment)
                {
                    switch (CourierComapny)
                    {
                        case FrayteLogisticServiceType.Yodel:
                            weightLimit = (from ls in dbContext.LogisticServices
                                           join lswl in dbContext.LogisticServiceWeightLimits on ls.LogisticServiceId equals lswl.LogisticServiceId
                                           where ls.OperationZoneId == OperationZoneId &&
                                                 ls.LogisticType == LogisticType &&
                                                 ls.LogisticCompany == CourierComapny &&
                                                 ls.RateType == RateType &&
                                                 ls.ModuleType == ModuleType
                                           select new LogisticServiceWeightLimit
                                           {
                                               LogisticServiceWeightLimitId = lswl.LogisticServiceWeightLimitId,
                                               LogisticServiceId = lswl.LogisticServiceId,
                                               ParcelType = lswl.ParcelType,
                                               PackageType = lswl.PackageType,
                                               WeightLimit = lswl.WeightLimit,
                                               WeightUnit = lswl.WeightUnit
                                           }).FirstOrDefault();
                            break;
                        case FrayteLogisticServiceType.Hermes:
                            _weightLimit = (from ls in dbContext.LogisticServices
                                            join lswl in dbContext.LogisticServiceWeightLimits on ls.LogisticServiceId equals lswl.LogisticServiceId
                                            where ls.OperationZoneId == OperationZoneId &&
                                                  ls.LogisticType == LogisticType &&
                                                  ls.LogisticCompany == CourierComapny &&
                                                  ls.RateType == RateType &&
                                                  ls.ModuleType == ModuleType
                                            select new LogisticServiceWeightLimit
                                            {
                                                LogisticServiceWeightLimitId = lswl.LogisticServiceWeightLimitId,
                                                LogisticServiceId = lswl.LogisticServiceId,
                                                ParcelType = lswl.ParcelType,
                                                PackageType = lswl.PackageType,
                                                WeightLimit = lswl.WeightLimit,
                                                WeightUnit = lswl.WeightUnit
                                            }).ToList();
                            break;
                        default:
                            weightLimit = (from ls in dbContext.LogisticServices
                                           join lswl in dbContext.LogisticServiceWeightLimits on ls.LogisticServiceId equals lswl.LogisticServiceId
                                           where ls.OperationZoneId == OperationZoneId &&
                                                 ls.LogisticType == LogisticType &&
                                                 ls.LogisticCompany == CourierComapny &&
                                                 ls.RateType == RateType &&
                                                 ls.ModuleType == ModuleType
                                           select new LogisticServiceWeightLimit
                                           {
                                               LogisticServiceWeightLimitId = lswl.LogisticServiceWeightLimitId,
                                               LogisticServiceId = lswl.LogisticServiceId,
                                               ParcelType = lswl.ParcelType,
                                               PackageType = lswl.PackageType,
                                               WeightLimit = lswl.WeightLimit,
                                               WeightUnit = lswl.WeightUnit
                                           }).FirstOrDefault();
                            break;
                    }
                }
                else if (LogisticType == FrayteLogisticType.EUImport || LogisticType == FrayteLogisticType.EUExport)
                {
                    weightLimit = (from ls in dbContext.LogisticServices
                                   join lswl in dbContext.LogisticServiceWeightLimits on ls.LogisticServiceId equals lswl.LogisticServiceId
                                   where ls.OperationZoneId == OperationZoneId &&
                                         ls.LogisticType == LogisticType &&
                                         ls.LogisticCompany == CourierComapny &&
                                         ls.RateType == RateType &&
                                         ls.ModuleType == ModuleType
                                   select new LogisticServiceWeightLimit
                                   {
                                       LogisticServiceWeightLimitId = lswl.LogisticServiceWeightLimitId,
                                       LogisticServiceId = lswl.LogisticServiceId,
                                       ParcelType = lswl.ParcelType,
                                       PackageType = lswl.PackageType,
                                       WeightLimit = lswl.WeightLimit,
                                       WeightUnit = lswl.WeightUnit
                                   }).FirstOrDefault();
                }

                if (weightLimit != null)
                {
                    var specialBaseRateCard = (from lsbrc in dbContext.LogisticServiceBaseRateCards
                                               join lsw in dbContext.LogisticServiceWeights on lsbrc.LogisticServiceWeightId equals lsw.LogisticServiceWeightId
                                               join lsst in dbContext.LogisticServiceShipmentTypes on lsw.LogisticServiceShipmentTypeId equals lsst.LogisticServiceShipmentTypeId
                                               join ls in dbContext.LogisticServices on lsst.LogisticServiceId equals ls.LogisticServiceId into tmpZone
                                               from zt in tmpZone.DefaultIfEmpty()
                                               join lsz in dbContext.LogisticServiceZones on zt.LogisticServiceId equals lsz.LogisticServiceId

                                               join oz in dbContext.OperationZones on OperationZoneId equals oz.OperationZoneId
                                               join opzer in dbContext.OperationZoneExchangeRates on oz.OperationZoneId equals opzer.OperationZoneId into leftjoininOZTemp
                                               from ljOZER in leftjoininOZTemp.DefaultIfEmpty()
                                               join oz1 in dbContext.OperationZones on ljOZER.OperationZoneId equals oz1.OperationZoneId

                                               where zt.LogisticType == LogisticType &&
                                                     zt.LogisticCompany == CourierComapny &&
                                                     zt.RateType == RateType &&
                                                     zt.ModuleType == ModuleType &&
                                                     lsw.LogisticServiceShipmentTypeId == 0 &&
                                                     lsw.WeightFrom >= weightLimit.WeightLimit &&
                                                     lsw.PackageType == PackageType &&
                                                     lsw.ParcelType == ParcelType &&
                                                     (zt.OperationZoneId == OperationZoneId || zt == null) &&
                                                     ljOZER.CurrencyCode == lsbrc.LogisticCurrency &&
                                                     ljOZER.ExchangeType == FrayteOperationZoneExchangeType.Sell

                                               select new FryateZoneBaseRateCardLimit()
                                               {
                                                   ZoneRateCardId = lsbrc.LogisticServiceBaseRateCardId,
                                                   OverWeightLimit = lsw.WeightFrom,
                                                   OverWeightTo = lsw.WeightTo,
                                                   UnitOfMeasurement = lsw.UOM,
                                                   WeightUnit = lsw.WeightUnit,
                                                   OverWeightLimitRate = lsbrc.LogisticRate,
                                                   Currency = lsbrc.LogisticCurrency,
                                                   Zone = zt == null ? null : new FrayteZone()
                                                   {
                                                       OperationZoneId = zt.OperationZoneId,
                                                       ZoneId = lsz.LogisticServiceZoneId,
                                                       ZoneName = lsz.ZoneName,
                                                       ZoneDisplayName = lsz.ZoneDisplayName
                                                   },
                                                   LogisticType = zt.LogisticType,
                                                   LogisticServiceType = zt.LogisticCompany,
                                                   BusinessRate = (float)(lsbrc.LogisticCurrency != ljOZER.CurrencyCode ? lsbrc.LogisticRate : Math.Round(lsbrc.LogisticRate / ljOZER.ExchangeRate, 2)),
                                                   BusinessCurrency = oz1.Currency

                                               }).ToList();

                    return specialBaseRateCard;
                }
                else if (_weightLimit != null && _weightLimit.Count > 0)
                {
                    foreach (LogisticServiceWeightLimit weight in _weightLimit)
                    {
                        var specialBaseRateCard = (from lsbrc in dbContext.LogisticServiceBaseRateCards
                                                   join lsw in dbContext.LogisticServiceWeights on lsbrc.LogisticServiceWeightId equals lsw.LogisticServiceWeightId
                                                   join lwL in dbContext.LogisticServiceWeightLimits on new { lsw.PackageType, lsw.ParcelType } equals
                                                                                                        new { lwL.PackageType, lwL.ParcelType }
                                                   join lsst in dbContext.LogisticServiceShipmentTypes on lsw.LogisticServiceShipmentTypeId equals lsst.LogisticServiceShipmentTypeId
                                                   join ls in dbContext.LogisticServices on lsst.LogisticServiceId equals ls.LogisticServiceId into tmpZone
                                                   from zt in tmpZone.DefaultIfEmpty()
                                                   join lsz in dbContext.LogisticServiceZones on zt.LogisticServiceId equals lsz.LogisticServiceId

                                                   join oz in dbContext.OperationZones on lsbrc.OperationZoneId equals oz.OperationZoneId
                                                   join opzer in dbContext.OperationZoneExchangeRates on oz.OperationZoneId equals opzer.OperationZoneId into leftjoininOZTemp
                                                   from ljOZER in leftjoininOZTemp.DefaultIfEmpty()
                                                   join oz1 in dbContext.OperationZones on ljOZER.OperationZoneId equals oz1.OperationZoneId

                                                   where zt.LogisticType == LogisticType &&
                                                         zt.LogisticCompany == CourierComapny &&
                                                         zt.RateType == RateType &&
                                                         zt.ModuleType == ModuleType &&
                                                         lsw.PackageType == PackageType &&
                                                         lsw.WeightFrom >= weight.WeightLimit &&
                                                         lsbrc.OperationZoneId == OperationZoneId &&
                                                         ljOZER.CurrencyCode == lsbrc.LogisticCurrency &&
                                                         ljOZER.ExchangeType == FrayteOperationZoneExchangeType.Sell

                                                   select new FryateZoneBaseRateCardLimit()
                                                   {
                                                       ZoneRateCardId = lsbrc.LogisticServiceBaseRateCardId,
                                                       OverWeightLimit = lsw.WeightFrom,
                                                       OverWeightTo = lsw.WeightTo,
                                                       OverWeightLimitRate = lsbrc.LogisticRate,
                                                       Currency = lsbrc.LogisticCurrency,
                                                       UnitOfMeasurement = lsw.UOM,
                                                       WeightUnit = lsw.WeightUnit,
                                                       PackageType = lsw.PackageType,
                                                       ParcelType = lsw.ParcelType,
                                                       Zone = new FrayteZone()
                                                       {
                                                           OperationZoneId = lsbrc.OperationZoneId,
                                                           ZoneId = lsz.LogisticServiceZoneId,
                                                           ZoneName = lsz.ZoneName,
                                                           ZoneDisplayName = lsz.ZoneDisplayName
                                                       },
                                                       LogisticType = zt.LogisticType,
                                                       LogisticServiceType = zt.LogisticCompany,
                                                       BusinessRate = (float)(lsbrc.LogisticCurrency != ljOZER.CurrencyCode ? lsbrc.LogisticRate : Math.Round(lsbrc.LogisticRate / ljOZER.ExchangeRate, 2)),
                                                       BusinessCurrency = oz1.Currency

                                                   }).FirstOrDefault();

                        if (specialBaseRateCard != null)
                        {
                            frayeBaseRate.Add(specialBaseRateCard);
                        }
                    }
                    return frayeBaseRate;
                }
                else
                {
                    var specialBaseRateCard = (from lsbrc in dbContext.LogisticServiceBaseRateCards
                                               join lsw in dbContext.LogisticServiceWeights on lsbrc.LogisticServiceWeightId equals lsw.LogisticServiceWeightId
                                               join lsst in dbContext.LogisticServiceShipmentTypes on lsw.LogisticServiceShipmentTypeId equals lsst.LogisticServiceShipmentTypeId
                                               join ls in dbContext.LogisticServices on lsst.LogisticServiceId equals ls.LogisticServiceId into tmpZone
                                               from zt in tmpZone.DefaultIfEmpty()
                                               join lsz in dbContext.LogisticServiceZones on zt.LogisticServiceId equals lsz.LogisticServiceId

                                               join oz in dbContext.OperationZones on lsbrc.OperationZoneId equals oz.OperationZoneId
                                               join opzer in dbContext.OperationZoneExchangeRates on oz.OperationZoneId equals opzer.OperationZoneId into leftjoininOZTemp
                                               from ljOZER in leftjoininOZTemp.DefaultIfEmpty()
                                               join oz1 in dbContext.OperationZones on ljOZER.OperationZoneId equals oz1.OperationZoneId

                                               where zt.LogisticType == LogisticType &&
                                                     zt.LogisticCompany == CourierComapny &&
                                                     zt.RateType == RateType &&
                                                     zt.ModuleType == ModuleType &&
                                                     lsw.PackageType == PackageType &&
                                                     lsw.ParcelType == ParcelType &&
                                                     lsw.LogisticServiceShipmentTypeId == 0 &&
                                                     lsw.WeightFrom == weightLimit.WeightLimit &&
                                                     (zt.OperationZoneId == OperationZoneId || zt == null) &&
                                                     ljOZER.CurrencyCode == lsbrc.LogisticCurrency &&
                                                     ljOZER.ExchangeType == FrayteOperationZoneExchangeType.Sell

                                               select new FryateZoneBaseRateCardLimit()
                                               {
                                                   Currency = lsbrc.LogisticCurrency,
                                                   OverWeightLimit = weightLimit.WeightLimit,
                                                   OverWeightLimitRate = lsbrc.LogisticRate,
                                                   ZoneRateCardId = lsbrc.LogisticServiceBaseRateCardId,
                                                   UnitOfMeasurement = lsw.UOM,
                                                   Zone = zt == null ? null : new FrayteZone()
                                                   {
                                                       OperationZoneId = zt.OperationZoneId,
                                                       ZoneId = lsz.LogisticServiceZoneId,
                                                       ZoneName = lsz.ZoneName,
                                                       ZoneDisplayName = lsz.ZoneDisplayName
                                                   },
                                                   BusinessRate = (float)(lsbrc.LogisticCurrency != ljOZER.CurrencyCode ? lsbrc.LogisticRate : Math.Round(lsbrc.LogisticRate / ljOZER.ExchangeRate, 2)),
                                                   BusinessCurrency = oz1.Currency

                                               }).ToList();

                    return specialBaseRateCard;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<FrayteZoneBaseRateCard> GetBaseRate(int OperationZoneId, string LogisticType, string CourierCompany, string RateType, string DocType, string ModuleType)
        {
            List<FrayteZoneBaseRateCard> _baserate = new List<FrayteZoneBaseRateCard>();

            try
            {
                var BaseRateCard = (from lsbrc in dbContext.LogisticServiceBaseRateCards
                                    join lsst in dbContext.LogisticServiceShipmentTypes on lsbrc.LogisticServiceShipmentTypeId equals lsst.LogisticServiceShipmentTypeId
                                    join oz in dbContext.OperationZones on OperationZoneId equals oz.OperationZoneId
                                    join opzer in dbContext.OperationZoneExchangeRates on oz.OperationZoneId equals opzer.OperationZoneId
                                    join lsca in dbContext.LogisticServiceCourierAccounts on lsbrc.LogisticServiceCourierAccountId equals lsca.LogisticServiceCourierAccountId
                                    join lsw in dbContext.LogisticServiceWeights on lsbrc.LogisticServiceWeightId equals lsw.LogisticServiceWeightId
                                    join lsz in dbContext.LogisticServiceZones on lsbrc.LogisticServiceZoneId equals lsz.LogisticServiceZoneId
                                    join ls in dbContext.LogisticServices on lsst.LogisticServiceId equals ls.LogisticServiceId

                                    where
                                          ls.LogisticType == LogisticType &&
                                          ls.LogisticCompany == CourierCompany &&
                                          ls.ModuleType == ModuleType &&
                                          ls.OperationZoneId == OperationZoneId &&
                                          ls.RateType == RateType &&
                                          lsst.LogisticDescription == DocType &&
                                          lsst.LogisticType == LogisticType &&
                                          opzer.CurrencyCode == lsbrc.LogisticCurrency &&
                                          opzer.ExchangeType == FrayteOperationZoneExchangeType.Sell

                                    select new FrayteZoneBaseRateCard
                                    {
                                        OperationZoneId = lsbrc.OperationZoneId,
                                        ZoneRateCardId = lsbrc.LogisticServiceBaseRateCardId,
                                        zone = new FrayteZone
                                        {
                                            OperationZoneId = ls.OperationZoneId,
                                            ZoneId = (lsz == null ? 0 : lsz.LogisticServiceZoneId),
                                            ZoneName = (lsz == null ? "" : lsz.ZoneName),
                                            ZoneDisplayName = (lsz == null ? "" : lsz.ZoneDisplayName),
                                            LogisticType = ls.LogisticType,
                                            CourierComapny = ls.LogisticCompany,
                                            RateType = ls.RateType,
                                            ModuleType = ls.ModuleType
                                        },
                                        shipmentType = new LogisticShipmentType
                                        {
                                            ShipmentTypeId = lsst.LogisticServiceShipmentTypeId,
                                            LogisticServiceId = lsst.LogisticServiceId,
                                            LogisticType = lsst.LogisticType,
                                            LogisticDescription = lsst.LogisticDescription,
                                            LogisticDescriptionDisplay = lsst.LogisticDescriptionDisplayType
                                        },

                                        courierAccount = new FrayteCourierAccount
                                        {
                                            LogisticServiceCourierAccountId = (lsca == null ? 0 : lsca.LogisticServiceCourierAccountId),
                                            AccountCountryCode = (lsca == null ? "" : lsca.AccountCountryCode),
                                            AccountNo = (lsca == null ? "" : lsca.AccountNo),
                                            ColorCode = (lsca == null ? "" : lsca.ColorCode),
                                            Description = (lsca == null ? "" : lsca.Description),
                                            IntegrationAccountId = (lsca == null ? "" : lsca.IntegrationAccountId),
                                            IsActive = (lsca == null ? false : lsca.IsActive),
                                            LogisticService = new LogisticShipmentService
                                            {
                                                LogisticServiceId = ls.LogisticServiceId,
                                                OperationZoneId = ls.OperationZoneId,
                                                LogisticCompany = ls.LogisticCompany,
                                                LogisticCompanyDisplay = ls.LogisticCompanyDisplay,
                                                LogisticType = ls.LogisticType,
                                                LogisticTypeDisplay = ls.LogisticTypeDisplay,
                                                RateType = ls.RateType,
                                                RateTypeDisplay = ls.RateTypeDisplay,
                                                ModuleType = ls.ModuleType
                                            },
                                            OperationZone = new FrayteOperationZone
                                            {
                                                OperationZoneId = (oz == null ? 0 : oz.OperationZoneId),
                                                OperationZoneName = (oz == null ? "" : oz.Name)
                                            }
                                        },
                                        LogisticWeight = new FrayteLogisticWeight
                                        {
                                            LogisticWeightId = lsw.LogisticServiceWeightId,
                                            WeightFrom = lsw.WeightFromDisplay,
                                            WeightTo = lsw.WeightToDisplay,
                                            UnitOfMeasurement = lsw.UOM,
                                            WeightUnit = lsw.WeightUnit,
                                            ShipmentType = new LogisticShipmentType
                                            {
                                                ShipmentTypeId = lsst.LogisticServiceShipmentTypeId,
                                                LogisticServiceId = lsst.LogisticServiceId,
                                                LogisticType = lsst.LogisticType,
                                                LogisticDescription = lsst.LogisticDescription,
                                                LogisticDescriptionDisplay = lsst.LogisticDescriptionDisplayType
                                            },
                                        },
                                        Rate = lsbrc.LogisticRate,
                                        CurrencyCode = lsbrc.LogisticCurrency,
                                        LogisticType = ls.LogisticType,
                                        BusinessRate = (float)(lsbrc.LogisticCurrency != opzer.CurrencyCode ? lsbrc.LogisticRate : Math.Round(lsbrc.LogisticRate / opzer.ExchangeRate, 2)),
                                        BusinessCurrency = oz.Currency,
                                        ModuleType = lsbrc.ModuleType

                                    }).OrderBy(p => p.zone.ZoneId).ToList();


                return BaseRateCard;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<FrayteZoneBaseRateCard> GetUKBaseRate(int OperationZoneId, string LogisticType, string CourierCompany, string RateType, string PackageType, string ParcelType, string ModuleType)
        {
            try
            {
                if (CourierCompany == FrayteLogisticServiceType.Yodel)
                {
                    #region Yodel Base Rate

                    var BaseRateCard = (from lsbrc in dbContext.LogisticServiceBaseRateCards
                                        join lsst in dbContext.LogisticServiceShipmentTypes on lsbrc.LogisticServiceShipmentTypeId equals lsst.LogisticServiceShipmentTypeId
                                        join lsz in dbContext.LogisticServiceZones on lsbrc.LogisticServiceZoneId equals lsz.LogisticServiceZoneId
                                        join ls in dbContext.LogisticServices on lsz.LogisticServiceId equals ls.LogisticServiceId
                                        join ca in dbContext.LogisticServiceCourierAccounts on lsbrc.LogisticServiceCourierAccountId equals ca.LogisticServiceCourierAccountId into leftJoinTemp
                                        from caTemp in leftJoinTemp.DefaultIfEmpty()
                                        join coz in dbContext.OperationZones on OperationZoneId equals coz.OperationZoneId into leftJoinOpZone
                                        from cozTemp in leftJoinOpZone.DefaultIfEmpty()

                                        join lsw in dbContext.LogisticServiceWeights on lsbrc.LogisticServiceWeightId equals lsw.LogisticServiceWeightId
                                        join lsst1 in dbContext.LogisticServiceShipmentTypes on lsw.LogisticServiceShipmentTypeId equals lsst1.LogisticServiceShipmentTypeId
                                        join ls1 in dbContext.LogisticServices on lsst1.LogisticServiceId equals ls1.LogisticServiceId

                                        join oz in dbContext.OperationZones on OperationZoneId equals oz.OperationZoneId
                                        join opzer in dbContext.OperationZoneExchangeRates on oz.OperationZoneId equals opzer.OperationZoneId into leftjoininOZTemp
                                        from ljOZER in leftjoininOZTemp.DefaultIfEmpty()
                                        join oz1 in dbContext.OperationZones on ljOZER.OperationZoneId equals oz1.OperationZoneId

                                        where
                                            ls.LogisticType == LogisticType &&
                                            ls.LogisticCompany == CourierCompany &&
                                            ls.OperationZoneId == OperationZoneId &&
                                            ls.ModuleType == ModuleType &&
                                            lsw.ParcelType == ParcelType &&
                                            lsw.PackageType == PackageType &&
                                            ljOZER.CurrencyCode == lsbrc.LogisticCurrency &&
                                            ljOZER.ExchangeType == FrayteOperationZoneExchangeType.Sell

                                        select new FrayteZoneBaseRateCard
                                        {
                                            OperationZoneId = lsbrc.OperationZoneId,
                                            ZoneRateCardId = lsbrc.LogisticServiceBaseRateCardId,
                                            zone = new FrayteZone
                                            {
                                                OperationZoneId = ls.OperationZoneId,
                                                ZoneId = lsz.LogisticServiceZoneId,
                                                ZoneName = lsz.ZoneName,
                                                ZoneDisplayName = lsz.ZoneDisplayName,
                                                LogisticType = ls.LogisticType,
                                                CourierComapny = ls.LogisticCompany,
                                                RateType = ls.RateType,
                                                ModuleType = ls.ModuleType
                                            },

                                            shipmentType = new LogisticShipmentType
                                            {
                                                ShipmentTypeId = lsst.LogisticServiceShipmentTypeId,
                                                LogisticServiceId = lsst.LogisticServiceId,
                                                LogisticType = lsst.LogisticType,
                                                LogisticDescription = lsst.LogisticDescription,
                                                LogisticDescriptionDisplay = lsst.LogisticDescriptionDisplayType,
                                                LogisticService = new LogisticShipmentService
                                                {
                                                    LogisticServiceId = ls.LogisticServiceId,
                                                    OperationZoneId = ls.OperationZoneId,
                                                    LogisticCompany = ls.LogisticCompany,
                                                    LogisticCompanyDisplay = ls.LogisticCompanyDisplay,
                                                    LogisticType = ls.LogisticType,
                                                    LogisticTypeDisplay = ls.LogisticTypeDisplay,
                                                    RateType = ls.RateType,
                                                    RateTypeDisplay = ls.RateTypeDisplay,
                                                    ModuleType = ls.ModuleType
                                                },
                                            },

                                            courierAccount = new FrayteCourierAccount
                                            {
                                                LogisticServiceCourierAccountId = (caTemp == null ? 0 : caTemp.LogisticServiceCourierAccountId),
                                                AccountCountryCode = (caTemp == null ? "" : caTemp.AccountCountryCode),
                                                AccountNo = (caTemp == null ? "" : caTemp.AccountNo),
                                                ColorCode = (caTemp == null ? "" : caTemp.ColorCode),
                                                Description = (caTemp == null ? "" : caTemp.Description),
                                                IntegrationAccountId = (caTemp == null ? "" : caTemp.IntegrationAccountId),
                                                IsActive = (caTemp == null ? false : caTemp.IsActive),
                                                LogisticService = new LogisticShipmentService
                                                {
                                                    LogisticServiceId = ls.LogisticServiceId,
                                                    OperationZoneId = ls.OperationZoneId,
                                                    LogisticCompany = ls.LogisticCompany,
                                                    LogisticCompanyDisplay = ls.LogisticCompanyDisplay,
                                                    LogisticType = ls.LogisticType,
                                                    LogisticTypeDisplay = ls.LogisticTypeDisplay,
                                                    RateType = ls.RateType,
                                                    RateTypeDisplay = ls.RateTypeDisplay,
                                                    ModuleType = ls.ModuleType
                                                },
                                                OperationZone = new FrayteOperationZone
                                                {
                                                    OperationZoneId = (cozTemp == null ? 0 : cozTemp.OperationZoneId),
                                                    OperationZoneName = (cozTemp == null ? "" : cozTemp.Name)
                                                }
                                            },
                                            LogisticWeight = new FrayteLogisticWeight
                                            {
                                                LogisticWeightId = lsw.LogisticServiceWeightId,
                                                WeightFrom = lsw.WeightFromDisplay,
                                                WeightTo = lsw.WeightToDisplay,
                                                UnitOfMeasurement = lsw.UOM,
                                                WeightUnit = lsw.WeightUnit,
                                                ShipmentType = new LogisticShipmentType
                                                {
                                                    ShipmentTypeId = lsst1.LogisticServiceShipmentTypeId,
                                                    LogisticServiceId = lsst1.LogisticServiceId,
                                                    LogisticType = lsst1.LogisticType,
                                                    LogisticDescription = lsst1.LogisticDescription,
                                                    LogisticDescriptionDisplay = lsst1.LogisticDescriptionDisplayType,
                                                    LogisticService = new LogisticShipmentService
                                                    {
                                                        LogisticServiceId = ls1.LogisticServiceId,
                                                        OperationZoneId = ls1.OperationZoneId,
                                                        LogisticCompany = ls1.LogisticCompany,
                                                        LogisticCompanyDisplay = ls1.LogisticCompanyDisplay,
                                                        LogisticType = ls1.LogisticType,
                                                        LogisticTypeDisplay = ls1.LogisticTypeDisplay,
                                                        RateType = ls1.RateType,
                                                        RateTypeDisplay = ls1.RateTypeDisplay,
                                                        ModuleType = ls1.ModuleType
                                                    },
                                                },
                                            },
                                            LogisticDimension = new FrayteLogisticDimension()
                                            {
                                                LogisticServiceDimensionId = lsbrc.LogisticServiceDimensionId,
                                                DimensionFrom = 0.0m,
                                                DimensionTo = 0.0m,
                                                DimensionUnit = null,
                                                Parceltype = null,
                                                PackageType = null,
                                                LogisticType = null,
                                                LogisticServiceType = null
                                            },
                                            Rate = lsbrc.LogisticRate,
                                            CurrencyCode = lsbrc.LogisticCurrency,
                                            LogisticType = ls.LogisticType,
                                            LogisticServicetype = ls.LogisticCompany,
                                            BusinessRate = (float)(lsbrc.LogisticCurrency != ljOZER.CurrencyCode ? lsbrc.LogisticRate : Math.Round(lsbrc.LogisticRate / ljOZER.ExchangeRate, 2)),
                                            BusinessCurrency = oz1.Currency,
                                            ModuleType = lsbrc.ModuleType

                                        }).ToList();


                    return BaseRateCard;

                    #endregion
                }
                else if (CourierCompany == FrayteLogisticServiceType.Hermes)
                {
                    #region Hermes Base Rate

                    var BaseRateCard = (from lsbrc in dbContext.LogisticServiceBaseRateCards
                                        join lsst in dbContext.LogisticServiceShipmentTypes on lsbrc.LogisticServiceShipmentTypeId equals lsst.LogisticServiceShipmentTypeId
                                        join lsz in dbContext.LogisticServiceZones on lsbrc.LogisticServiceZoneId equals lsz.LogisticServiceZoneId
                                        join ls in dbContext.LogisticServices on lsz.LogisticServiceId equals ls.LogisticServiceId
                                        join ca in dbContext.LogisticServiceCourierAccounts on lsbrc.LogisticServiceCourierAccountId equals ca.LogisticServiceCourierAccountId into leftJoinTemp
                                        from caTemp in leftJoinTemp.DefaultIfEmpty()
                                        join coz in dbContext.OperationZones on OperationZoneId equals coz.OperationZoneId into leftJoinOpZone
                                        from cozTemp in leftJoinOpZone.DefaultIfEmpty()
                                        join lsw in dbContext.LogisticServiceWeights on lsbrc.LogisticServiceWeightId equals lsw.LogisticServiceWeightId
                                        join lsst1 in dbContext.LogisticServiceShipmentTypes on lsw.LogisticServiceShipmentTypeId equals lsst1.LogisticServiceShipmentTypeId
                                        join ls1 in dbContext.LogisticServices on lsst1.LogisticServiceId equals ls1.LogisticServiceId
                                        join lswl in dbContext.LogisticServiceWeightLimits on new { lsw.PackageType, lsw.ParcelType } equals
                                                                                              new { lswl.PackageType, lswl.ParcelType } into leftJoinLWL
                                        from ljWLL in leftJoinLWL.DefaultIfEmpty()
                                        join oz in dbContext.OperationZones on OperationZoneId equals oz.OperationZoneId
                                        join opzer in dbContext.OperationZoneExchangeRates on oz.OperationZoneId equals opzer.OperationZoneId into leftjoininOZTemp
                                        from ljOZER in leftjoininOZTemp.DefaultIfEmpty()
                                        join oz1 in dbContext.OperationZones on ljOZER.OperationZoneId equals oz1.OperationZoneId

                                        where
                                             ls.LogisticType == LogisticType &&
                                             ls.LogisticCompany == CourierCompany &&
                                             ls.OperationZoneId == OperationZoneId &&
                                             lsw.PackageType == PackageType &&
                                             ljOZER.CurrencyCode == lsbrc.LogisticCurrency &&
                                             ljOZER.ExchangeType == FrayteOperationZoneExchangeType.Sell

                                        select new FrayteZoneBaseRateCard
                                        {
                                            OperationZoneId = lsbrc.OperationZoneId,
                                            ZoneRateCardId = lsbrc.LogisticServiceBaseRateCardId,
                                            zone = new FrayteZone
                                            {
                                                OperationZoneId = ls.OperationZoneId,
                                                ZoneId = lsz.LogisticServiceZoneId,
                                                ZoneName = lsz.ZoneName,
                                                ZoneDisplayName = lsz.ZoneDisplayName,
                                                LogisticType = ls.LogisticType,
                                                CourierComapny = ls.LogisticCompany,
                                                RateType = ls.RateType,
                                                ModuleType = ls.ModuleType
                                            },

                                            shipmentType = new LogisticShipmentType
                                            {
                                                ShipmentTypeId = lsst.LogisticServiceShipmentTypeId,
                                                LogisticServiceId = lsst.LogisticServiceId,
                                                LogisticType = lsst.LogisticType,
                                                LogisticDescription = lsst.LogisticDescription,
                                                LogisticDescriptionDisplay = lsst.LogisticDescriptionDisplayType,
                                                LogisticService = new LogisticShipmentService
                                                {
                                                    LogisticServiceId = ls.LogisticServiceId,
                                                    OperationZoneId = ls.OperationZoneId,
                                                    LogisticCompany = ls.LogisticCompany,
                                                    LogisticCompanyDisplay = ls.LogisticCompanyDisplay,
                                                    LogisticType = ls.LogisticType,
                                                    LogisticTypeDisplay = ls.LogisticTypeDisplay,
                                                    RateType = ls.RateType,
                                                    RateTypeDisplay = ls.RateTypeDisplay,
                                                    ModuleType = ls.ModuleType
                                                },
                                            },

                                            courierAccount = new FrayteCourierAccount
                                            {
                                                LogisticServiceCourierAccountId = (caTemp == null ? 0 : caTemp.LogisticServiceCourierAccountId),
                                                AccountCountryCode = (caTemp == null ? "" : caTemp.AccountCountryCode),
                                                AccountNo = (caTemp == null ? "" : caTemp.AccountNo),
                                                ColorCode = (caTemp == null ? "" : caTemp.ColorCode),
                                                Description = (caTemp == null ? "" : caTemp.Description),
                                                IntegrationAccountId = (caTemp == null ? "" : caTemp.IntegrationAccountId),
                                                IsActive = (caTemp == null ? false : caTemp.IsActive),
                                                LogisticService = new LogisticShipmentService
                                                {
                                                    LogisticServiceId = ls.LogisticServiceId,
                                                    OperationZoneId = ls.OperationZoneId,
                                                    LogisticCompany = ls.LogisticCompany,
                                                    LogisticCompanyDisplay = ls.LogisticCompanyDisplay,
                                                    LogisticType = ls.LogisticType,
                                                    LogisticTypeDisplay = ls.LogisticTypeDisplay,
                                                    RateType = ls.RateType,
                                                    RateTypeDisplay = ls.RateTypeDisplay,
                                                    ModuleType = ls.ModuleType
                                                },
                                                OperationZone = new FrayteOperationZone
                                                {
                                                    OperationZoneId = (cozTemp == null ? 0 : cozTemp.OperationZoneId),
                                                    OperationZoneName = (cozTemp == null ? "" : cozTemp.Name)
                                                }
                                            },
                                            LogisticWeight = new FrayteLogisticWeight
                                            {
                                                LogisticWeightId = lsw.LogisticServiceWeightId,
                                                WeightFrom = lsw.WeightFromDisplay,
                                                WeightTo = lsw.WeightToDisplay,
                                                UnitOfMeasurement = lsw.UOM,
                                                WeightUnit = lsw.WeightUnit,
                                                ShipmentType = new LogisticShipmentType
                                                {
                                                    ShipmentTypeId = lsst.LogisticServiceShipmentTypeId,
                                                    LogisticServiceId = lsst.LogisticServiceId,
                                                    LogisticType = lsst.LogisticType,
                                                    LogisticDescription = lsst.LogisticDescription,
                                                    LogisticDescriptionDisplay = lsst.LogisticDescriptionDisplayType,
                                                    LogisticService = new LogisticShipmentService
                                                    {
                                                        LogisticServiceId = ls1.LogisticServiceId,
                                                        OperationZoneId = ls1.OperationZoneId,
                                                        LogisticCompany = ls1.LogisticCompany,
                                                        LogisticCompanyDisplay = ls1.LogisticCompanyDisplay,
                                                        LogisticType = ls1.LogisticType,
                                                        LogisticTypeDisplay = ls1.LogisticTypeDisplay,
                                                        RateType = ls1.RateType,
                                                        RateTypeDisplay = ls1.RateTypeDisplay,
                                                        ModuleType = ls1.ModuleType
                                                    },
                                                },
                                                PackageType = lsw.PackageType,
                                                ParcelType = lsw.ParcelType,
                                                WeightLimit = ljWLL == null ? 0 : ljWLL.WeightLimit
                                            },
                                            LogisticDimension = new FrayteLogisticDimension()
                                            {
                                                LogisticServiceDimensionId = lsbrc.LogisticServiceDimensionId,
                                                DimensionFrom = 0.0m,
                                                DimensionTo = 0.0m,
                                                DimensionUnit = null,
                                                Parceltype = null,
                                                PackageType = null,
                                                LogisticType = null,
                                                LogisticServiceType = null
                                            },
                                            Rate = lsbrc.LogisticRate,
                                            CurrencyCode = lsbrc.LogisticCurrency,
                                            LogisticType = ls.LogisticType,
                                            LogisticServicetype = ls.LogisticCompany,
                                            BusinessRate = (float)(lsbrc.LogisticCurrency != ljOZER.CurrencyCode ? lsbrc.LogisticRate : Math.Round(lsbrc.LogisticRate / ljOZER.ExchangeRate, 2)),
                                            BusinessCurrency = oz1.Currency,
                                            ModuleType = lsbrc.ModuleType

                                        }).ToList();

                    return BaseRateCard;

                    #endregion
                }
                else if (CourierCompany == FrayteLogisticServiceType.DHL)
                {
                    #region DHL Base Rate

                    var BaseRateCard = (from lsbrc in dbContext.LogisticServiceBaseRateCards
                                        join lsst in dbContext.LogisticServiceShipmentTypes on lsbrc.LogisticServiceShipmentTypeId equals lsst.LogisticServiceShipmentTypeId
                                        join lsz in dbContext.LogisticServiceZones on lsbrc.LogisticServiceZoneId equals lsz.LogisticServiceZoneId
                                        join ls in dbContext.LogisticServices on lsz.LogisticServiceId equals ls.LogisticServiceId
                                        join lsw in dbContext.LogisticServiceWeights on lsbrc.LogisticServiceWeightId equals lsw.LogisticServiceWeightId
                                        join oz in dbContext.OperationZones on OperationZoneId equals oz.OperationZoneId
                                        join opzer in dbContext.OperationZoneExchangeRates on oz.OperationZoneId equals opzer.OperationZoneId
                                        join ca in dbContext.LogisticServiceCourierAccounts on lsbrc.LogisticServiceCourierAccountId equals ca.LogisticServiceCourierAccountId into leftJoinTemp
                                        from caTemp in leftJoinTemp.DefaultIfEmpty()

                                        where
                                             ls.LogisticType == LogisticType &&
                                             ls.LogisticCompany == CourierCompany &&
                                             ls.OperationZoneId == OperationZoneId &&
                                             ls.RateType == RateType &&
                                             opzer.CurrencyCode == lsbrc.LogisticCurrency &&
                                             opzer.ExchangeType == FrayteOperationZoneExchangeType.Sell

                                        select new FrayteZoneBaseRateCard
                                        {
                                            OperationZoneId = lsbrc.OperationZoneId,
                                            ZoneRateCardId = lsbrc.LogisticServiceBaseRateCardId,
                                            zone = new FrayteZone
                                            {
                                                OperationZoneId = ls.OperationZoneId,
                                                ZoneId = lsz.LogisticServiceZoneId,
                                                ZoneName = lsz.ZoneName,
                                                ZoneDisplayName = lsz.ZoneDisplayName,
                                                LogisticType = ls.LogisticType,
                                                CourierComapny = ls.LogisticCompany,
                                                RateType = ls.RateType,
                                                ModuleType = ls.ModuleType
                                            },

                                            shipmentType = new LogisticShipmentType
                                            {
                                                ShipmentTypeId = lsst.LogisticServiceShipmentTypeId,
                                                LogisticServiceId = lsst.LogisticServiceId,
                                                LogisticType = lsst.LogisticType,
                                                LogisticDescription = lsst.LogisticDescription,
                                                LogisticDescriptionDisplay = lsst.LogisticDescriptionDisplayType,
                                                LogisticService = new LogisticShipmentService
                                                {
                                                    LogisticServiceId = ls.LogisticServiceId,
                                                    OperationZoneId = ls.OperationZoneId,
                                                    LogisticCompany = ls.LogisticCompany,
                                                    LogisticCompanyDisplay = ls.LogisticCompanyDisplay,
                                                    LogisticType = ls.LogisticType,
                                                    LogisticTypeDisplay = ls.LogisticTypeDisplay,
                                                    RateType = ls.RateType,
                                                    RateTypeDisplay = ls.RateTypeDisplay,
                                                    ModuleType = ls.ModuleType
                                                },
                                            },
                                            courierAccount = new FrayteCourierAccount
                                            {
                                                LogisticServiceCourierAccountId = (caTemp == null ? 0 : caTemp.LogisticServiceCourierAccountId),
                                                AccountCountryCode = (caTemp == null ? "" : caTemp.AccountCountryCode),
                                                AccountNo = (caTemp == null ? "" : caTemp.AccountNo),
                                                ColorCode = (caTemp == null ? "" : caTemp.ColorCode),
                                                Description = (caTemp == null ? "" : caTemp.Description),
                                                IntegrationAccountId = (caTemp == null ? "" : caTemp.IntegrationAccountId),
                                                IsActive = (caTemp == null ? false : caTemp.IsActive),
                                                LogisticService = new LogisticShipmentService
                                                {
                                                    LogisticServiceId = ls.LogisticServiceId,
                                                    OperationZoneId = ls.OperationZoneId,
                                                    LogisticCompany = ls.LogisticCompany,
                                                    LogisticCompanyDisplay = ls.LogisticCompanyDisplay,
                                                    LogisticType = ls.LogisticType,
                                                    LogisticTypeDisplay = ls.LogisticTypeDisplay,
                                                    RateType = ls.RateType,
                                                    RateTypeDisplay = ls.RateTypeDisplay,
                                                    ModuleType = ls.ModuleType
                                                },
                                                OperationZone = new FrayteOperationZone
                                                {
                                                    OperationZoneId = (oz == null ? 0 : oz.OperationZoneId),
                                                    OperationZoneName = (oz == null ? "" : oz.Name)
                                                }
                                            },
                                            LogisticWeight = new FrayteLogisticWeight
                                            {
                                                LogisticWeightId = lsw.LogisticServiceWeightId,
                                                WeightFrom = lsw.WeightFromDisplay,
                                                WeightTo = lsw.WeightToDisplay,
                                                UnitOfMeasurement = lsw.UOM,
                                                WeightUnit = lsw.WeightUnit,
                                                ShipmentType = new LogisticShipmentType
                                                {
                                                    ShipmentTypeId = lsst.LogisticServiceShipmentTypeId,
                                                    LogisticServiceId = lsst.LogisticServiceId,
                                                    LogisticType = lsst.LogisticType,
                                                    LogisticDescription = lsst.LogisticDescription,
                                                    LogisticDescriptionDisplay = lsst.LogisticDescriptionDisplayType,
                                                    LogisticService = new LogisticShipmentService
                                                    {
                                                        LogisticServiceId = ls.LogisticServiceId,
                                                        OperationZoneId = ls.OperationZoneId,
                                                        LogisticCompany = ls.LogisticCompany,
                                                        LogisticCompanyDisplay = ls.LogisticCompanyDisplay,
                                                        LogisticType = ls.LogisticType,
                                                        LogisticTypeDisplay = ls.LogisticTypeDisplay,
                                                        RateType = ls.RateType,
                                                        RateTypeDisplay = ls.RateTypeDisplay,
                                                        ModuleType = ls.ModuleType
                                                    },
                                                },
                                                PackageType = lsw.PackageType,
                                                ParcelType = lsw.ParcelType,
                                                WeightLimit = 0
                                            },
                                            LogisticDimension = new FrayteLogisticDimension()
                                            {
                                                LogisticServiceDimensionId = lsbrc.LogisticServiceDimensionId,
                                                DimensionFrom = 0.0m,
                                                DimensionTo = 0.0m,
                                                DimensionUnit = null,
                                                Parceltype = null,
                                                PackageType = null,
                                                LogisticType = null,
                                                LogisticServiceType = null
                                            },
                                            Rate = lsbrc.LogisticRate,
                                            CurrencyCode = lsbrc.LogisticCurrency,
                                            LogisticType = ls.LogisticType,
                                            LogisticServicetype = ls.LogisticCompany,
                                            BusinessRate = (float)(lsbrc.LogisticCurrency != opzer.CurrencyCode ? lsbrc.LogisticRate : Math.Round(lsbrc.LogisticRate / opzer.ExchangeRate, 2)),
                                            BusinessCurrency = oz.Currency,
                                            ModuleType = lsbrc.ModuleType

                                        }).ToList();

                    return BaseRateCard;

                    #endregion
                }
                else
                {
                    #region Base Rate

                    var BaseRateCard = (from lsbrc in dbContext.LogisticServiceBaseRateCards
                                        join lsst in dbContext.LogisticServiceShipmentTypes on lsbrc.LogisticServiceShipmentTypeId equals lsst.LogisticServiceShipmentTypeId
                                        join oz in dbContext.OperationZones on OperationZoneId equals oz.OperationZoneId
                                        join opzer in dbContext.OperationZoneExchangeRates.Where(p => p.ExchangeType == FrayteOperationZoneExchangeType.Sell) on oz.OperationZoneId equals opzer.OperationZoneId into leftOZER
                                        from OpzTemp in leftOZER.DefaultIfEmpty()
                                        join lsw in dbContext.LogisticServiceWeights on lsbrc.LogisticServiceWeightId equals lsw.LogisticServiceWeightId
                                        join lswl in dbContext.LogisticServiceWeightLimits on new { lsw.PackageType, lsw.ParcelType } equals
                                                                                              new { lswl.PackageType, lswl.ParcelType }
                                        join lsca in dbContext.LogisticServiceCourierAccounts on lsbrc.LogisticServiceCourierAccountId equals lsca.LogisticServiceCourierAccountId into leftJoin
                                        from caTemp in leftJoin.DefaultIfEmpty()
                                        join lsz in dbContext.LogisticServiceZones on lsbrc.LogisticServiceZoneId equals lsz.LogisticServiceZoneId
                                        join ls in dbContext.LogisticServices on lsst.LogisticServiceId equals ls.LogisticServiceId

                                        where
                                            ls.LogisticType == LogisticType &&
                                            ls.LogisticCompany == CourierCompany &&
                                            ls.OperationZoneId == OperationZoneId &&
                                            ls.ModuleType == ModuleType &&
                                            lsw.PackageType == PackageType &&
                                            lsw.ParcelType == ParcelType &&
                                            OpzTemp.CurrencyCode == lsbrc.LogisticCurrency

                                        select new FrayteZoneBaseRateCard
                                        {
                                            OperationZoneId = lsbrc.OperationZoneId,
                                            ZoneRateCardId = lsbrc.LogisticServiceBaseRateCardId,
                                            zone = new FrayteZone
                                            {
                                                OperationZoneId = ls.OperationZoneId,
                                                ZoneId = lsz.LogisticServiceZoneId,
                                                ZoneName = lsz.ZoneName,
                                                ZoneDisplayName = lsz.ZoneDisplayName,
                                                LogisticType = ls.LogisticType,
                                                CourierComapny = ls.LogisticCompany,
                                                RateType = ls.RateType,
                                                ModuleType = ls.ModuleType
                                            },
                                            shipmentType = new LogisticShipmentType
                                            {
                                                ShipmentTypeId = lsst.LogisticServiceShipmentTypeId,
                                                LogisticServiceId = lsst.LogisticServiceId,
                                                LogisticType = lsst.LogisticType,
                                                LogisticDescription = lsst.LogisticDescription,
                                                LogisticDescriptionDisplay = lsst.LogisticDescriptionDisplayType,
                                                LogisticService = new LogisticShipmentService
                                                {
                                                    LogisticServiceId = ls.LogisticServiceId,
                                                    OperationZoneId = ls.OperationZoneId,
                                                    LogisticCompany = ls.LogisticCompany,
                                                    LogisticCompanyDisplay = ls.LogisticCompanyDisplay,
                                                    LogisticType = ls.LogisticType,
                                                    LogisticTypeDisplay = ls.LogisticTypeDisplay,
                                                    RateType = ls.RateType,
                                                    RateTypeDisplay = ls.RateTypeDisplay,
                                                    ModuleType = ls.ModuleType
                                                },
                                            },

                                            courierAccount = new FrayteCourierAccount
                                            {
                                                LogisticServiceCourierAccountId = (caTemp == null ? 0 : caTemp.LogisticServiceCourierAccountId),
                                                AccountCountryCode = (caTemp == null ? "" : caTemp.AccountCountryCode),
                                                AccountNo = (caTemp == null ? "" : caTemp.AccountNo),
                                                ColorCode = (caTemp == null ? "" : caTemp.ColorCode),
                                                Description = (caTemp == null ? "" : caTemp.Description),
                                                IntegrationAccountId = (caTemp == null ? "" : caTemp.IntegrationAccountId),
                                                IsActive = (caTemp == null ? false : caTemp.IsActive),
                                                LogisticService = new LogisticShipmentService
                                                {
                                                    LogisticServiceId = ls.LogisticServiceId,
                                                    OperationZoneId = ls.OperationZoneId,
                                                    LogisticCompany = ls.LogisticCompany,
                                                    LogisticCompanyDisplay = ls.LogisticCompanyDisplay,
                                                    LogisticType = ls.LogisticType,
                                                    LogisticTypeDisplay = ls.LogisticTypeDisplay,
                                                    RateType = ls.RateType,
                                                    RateTypeDisplay = ls.RateTypeDisplay,
                                                    ModuleType = ls.ModuleType
                                                },
                                                OperationZone = new FrayteOperationZone
                                                {
                                                    OperationZoneId = (oz == null ? 0 : oz.OperationZoneId),
                                                    OperationZoneName = (oz == null ? "" : oz.Name)
                                                }
                                            },
                                            LogisticWeight = new FrayteLogisticWeight
                                            {
                                                LogisticWeightId = lsw.LogisticServiceWeightId,
                                                WeightFrom = lsw.WeightFromDisplay,
                                                WeightTo = lsw.WeightToDisplay,
                                                UnitOfMeasurement = lsw.UOM,
                                                WeightUnit = lsw.WeightUnit,
                                                ShipmentType = new LogisticShipmentType
                                                {
                                                    ShipmentTypeId = lsst.LogisticServiceShipmentTypeId,
                                                    LogisticServiceId = lsst.LogisticServiceId,
                                                    LogisticType = lsst.LogisticType,
                                                    LogisticDescription = lsst.LogisticDescription,
                                                    LogisticDescriptionDisplay = lsst.LogisticDescriptionDisplayType,
                                                    LogisticService = new LogisticShipmentService
                                                    {
                                                        LogisticServiceId = ls.LogisticServiceId,
                                                        OperationZoneId = ls.OperationZoneId,
                                                        LogisticCompany = ls.LogisticCompany,
                                                        LogisticCompanyDisplay = ls.LogisticCompanyDisplay,
                                                        LogisticType = ls.LogisticType,
                                                        LogisticTypeDisplay = ls.LogisticTypeDisplay,
                                                        RateType = ls.RateType,
                                                        RateTypeDisplay = ls.RateTypeDisplay,
                                                        ModuleType = ls.ModuleType
                                                    },
                                                },
                                                WeightLimit = lswl.WeightLimit > 0 ? lswl.WeightLimit : 0
                                            },
                                            Rate = lsbrc.LogisticRate,
                                            CurrencyCode = lsbrc.LogisticCurrency,
                                            LogisticType = ls.LogisticType,
                                            BusinessRate = (float)(lsbrc.LogisticCurrency != OpzTemp.CurrencyCode ? lsbrc.LogisticRate : Math.Round(lsbrc.LogisticRate / OpzTemp.ExchangeRate, 2)),
                                            BusinessCurrency = oz.Currency,
                                            ModuleType = lsbrc.ModuleType

                                        }).ToList();


                    return BaseRateCard;

                    #endregion
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<FrayteZoneBaseRateCard> GetEUBaseRate(int OperationZoneId, string LogisticType, string CourierCompany, string RateType, string ModuleType)
        {
            List<FrayteZoneBaseRateCard> _baserate = new List<FrayteZoneBaseRateCard>();

            try
            {
                var BaseRateCard = (from lsbrc in dbContext.LogisticServiceBaseRateCards
                                    join lsst in dbContext.LogisticServiceShipmentTypes on lsbrc.LogisticServiceShipmentTypeId equals lsst.LogisticServiceShipmentTypeId
                                    join lsz in dbContext.LogisticServiceZones on lsbrc.LogisticServiceZoneId equals lsz.LogisticServiceZoneId
                                    join ls in dbContext.LogisticServices on lsz.LogisticServiceId equals ls.LogisticServiceId
                                    join lsao in dbContext.LogisticServiceAddOns on ls.LogisticServiceId equals lsao.LogisticServiceId into leftAddOnRate
                                    from laoTemp in leftAddOnRate.DefaultIfEmpty()
                                    join ca in dbContext.LogisticServiceCourierAccounts on lsbrc.LogisticServiceCourierAccountId equals ca.LogisticServiceCourierAccountId into leftJoinTemp
                                    from caTemp in leftJoinTemp.DefaultIfEmpty()
                                    join coz in dbContext.OperationZones on OperationZoneId equals coz.OperationZoneId into leftJoinOpZone
                                    from cozTemp in leftJoinOpZone.DefaultIfEmpty()
                                    join lw in dbContext.LogisticServiceWeights on lsbrc.LogisticServiceWeightId equals lw.LogisticServiceWeightId

                                    join oz in dbContext.OperationZones on OperationZoneId equals oz.OperationZoneId
                                    join opzer in dbContext.OperationZoneExchangeRates on oz.OperationZoneId equals opzer.OperationZoneId into leftjoininOZTemp
                                    from ljOZER in leftjoininOZTemp.DefaultIfEmpty()
                                    join oz1 in dbContext.OperationZones on ljOZER.OperationZoneId equals oz1.OperationZoneId

                                    where
                                          ls.LogisticType == LogisticType &&
                                          ls.LogisticCompany == CourierCompany &&
                                          ls.ModuleType == ModuleType &&
                                          ls.OperationZoneId == OperationZoneId &&
                                          ls.RateType == RateType &&
                                          ljOZER.CurrencyCode == lsbrc.LogisticCurrency &&
                                          ljOZER.ExchangeType == FrayteOperationZoneExchangeType.Sell

                                    select new FrayteZoneBaseRateCard
                                    {
                                        OperationZoneId = lsbrc.OperationZoneId,
                                        ZoneRateCardId = lsbrc.LogisticServiceBaseRateCardId,
                                        zone = new FrayteZone
                                        {
                                            OperationZoneId = ls.OperationZoneId,
                                            ZoneId = lsz.LogisticServiceZoneId,
                                            ZoneName = lsz.ZoneName,
                                            ZoneDisplayName = lsz.ZoneDisplayName
                                        },
                                        shipmentType = new LogisticShipmentType
                                        {
                                            ShipmentTypeId = 0,
                                            LogisticServiceId = 0,
                                            LogisticType = "",
                                            LogisticDescription = "",
                                            LogisticDescriptionDisplay = ""
                                        },

                                        courierAccount = new FrayteCourierAccount
                                        {
                                            LogisticServiceCourierAccountId = (caTemp == null ? 0 : caTemp.LogisticServiceCourierAccountId),
                                            AccountCountryCode = (caTemp == null ? "" : caTemp.AccountCountryCode),
                                            AccountNo = (caTemp == null ? "" : caTemp.AccountNo),
                                            ColorCode = (caTemp == null ? "" : caTemp.ColorCode),
                                            Description = (caTemp == null ? "" : caTemp.Description),
                                            IntegrationAccountId = (caTemp == null ? "" : caTemp.IntegrationAccountId),
                                            IsActive = (caTemp == null ? false : caTemp.IsActive),
                                            LogisticService = new LogisticShipmentService
                                            {
                                                LogisticServiceId = ls.LogisticServiceId,
                                                OperationZoneId = ls.OperationZoneId,
                                                LogisticCompany = ls.LogisticCompany,
                                                LogisticCompanyDisplay = ls.LogisticCompanyDisplay,
                                                LogisticType = ls.LogisticType,
                                                LogisticTypeDisplay = ls.LogisticTypeDisplay,
                                                RateType = ls.RateType,
                                                RateTypeDisplay = ls.RateTypeDisplay,
                                                ModuleType = ls.ModuleType
                                            },
                                            OperationZone = new FrayteOperationZone
                                            {
                                                OperationZoneId = (cozTemp == null ? 0 : cozTemp.OperationZoneId),
                                                OperationZoneName = (cozTemp == null ? "" : cozTemp.Name)
                                            }
                                        },
                                        LogisticWeight = new FrayteLogisticWeight
                                        {
                                            LogisticWeightId = lw.LogisticServiceWeightId,
                                            WeightFrom = lw.WeightFrom,
                                            WeightTo = lw.WeightTo,
                                            UnitOfMeasurement = lw.UOM,
                                            WeightUnit = lw.WeightUnit,
                                            ShipmentType = new LogisticShipmentType
                                            {
                                                ShipmentTypeId = 0,
                                                LogisticServiceId = 0,
                                                LogisticType = "",
                                                LogisticDescription = "",
                                                LogisticDescriptionDisplay = ""
                                            },
                                        },
                                        Rate = lsbrc.LogisticRate,
                                        CurrencyCode = lsbrc.LogisticCurrency,
                                        LogisticType = ls.LogisticType,
                                        BusinessRate = (float)(lsbrc.LogisticCurrency != ljOZER.CurrencyCode ? lsbrc.LogisticRate : Math.Round(lsbrc.LogisticRate / ljOZER.ExchangeRate, 2)),
                                        BusinessCurrency = oz1.Currency,
                                        ModuleType = lsbrc.ModuleType,
                                        LogisticServiceAddOnId = (laoTemp == null ? 0 : laoTemp.LogisticServiceAddOnId)

                                    }).ToList();


                return BaseRateCard;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<FrayteOperationZoneCurrency> OperationZoneCurrencyCode(int OperationZoneId, string exchangeType)
        {
            List<FrayteOperationZoneCurrency> _code = new List<FrayteOperationZoneCurrency>();
            try
            {
                var currency = dbContext.OperationZoneExchangeRates.Where(x => x.OperationZoneId == OperationZoneId &&
                                                                               x.ExchangeType == exchangeType &&
                                                                               x.IsActive == true).ToList();

                if (currency != null && currency.Count > 0)
                {
                    FrayteOperationZoneCurrency foc;
                    foreach (var myData in currency)
                    {
                        foc = new FrayteOperationZoneCurrency();
                        foc.CurrencyCode = myData.CurrencyCode;
                        _code.Add(foc);
                    }
                    return _code;
                }
                return _code;
            }
            catch (Exception ex)
            {

            }
            return _code;
        }

        public List<FrayteZone> GetZoneDetail(int OperationZoneId, string LogisticType, string CourierCompany, string RateType, string ModuleType)
        {
            var list = (from ls in dbContext.LogisticServices
                        join lsz in dbContext.LogisticServiceZones on ls.LogisticServiceId equals lsz.LogisticServiceId
                        where ls.OperationZoneId == OperationZoneId &&
                              ls.LogisticType == LogisticType &&
                              ls.LogisticCompany == CourierCompany &&
                              ls.RateType == RateType &&
                              ls.ModuleType == ModuleType
                        select new FrayteZone
                        {
                            ZoneId = lsz.LogisticServiceZoneId,
                            OperationZoneId = ls.OperationZoneId,
                            ZoneName = lsz.ZoneName,
                            ZoneDisplayName = lsz.ZoneDisplayName,
                            ZoneColor = lsz.ZoneColor,
                            LogisticType = ls.LogisticType,
                            CourierComapny = ls.LogisticCompany,
                            RateType = ls.RateType,
                            ModuleType = ls.ModuleType
                        }).ToList();

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

        public List<LogisticShipmentType> GetShipmentType(int OperationZoneId, string LogisticType, string CourierCompany, string RateType, string ModuleType)
        {
            List<LogisticShipmentType> _shipType = new List<LogisticShipmentType>();

            _shipType = (from ls in dbContext.LogisticServices
                         join lsst in dbContext.LogisticServiceShipmentTypes on ls.LogisticServiceId equals lsst.LogisticServiceId
                         where ls.OperationZoneId == OperationZoneId &&
                               ls.LogisticType == LogisticType &&
                               ls.LogisticCompany == CourierCompany &&
                               ls.RateType == RateType &&
                               ls.ModuleType == ModuleType
                         select new LogisticShipmentType
                         {
                             ShipmentTypeId = lsst.LogisticServiceShipmentTypeId,
                             LogisticServiceId = lsst.LogisticServiceId,
                             LogisticType = lsst.LogisticType,
                             LogisticDescription = lsst.LogisticDescription,
                             LogisticDescriptionDisplay = lsst.LogisticDescriptionDisplayType
                         }).ToList();

            return _shipType;
        }

        public List<FrayteLogisticWeight> GetLogisticWeight(int OperationZoneId, string LogisticType, string CourierCompany, string RateType, string PackageType, string ParcelType, string ModuleType)
        {
            List<FrayteLogisticWeight> _weight = new List<FrayteLogisticWeight>();

            if (LogisticType == FrayteLogisticType.UKShipment)
            {
                if (CourierCompany == FrayteLogisticServiceType.UkMail)
                {
                    _weight = (from ls in dbContext.LogisticServices
                               join lsst in dbContext.LogisticServiceShipmentTypes on ls.LogisticServiceId equals lsst.LogisticServiceId
                               join lsw in dbContext.LogisticServiceWeights on lsst.LogisticServiceShipmentTypeId equals lsw.LogisticServiceShipmentTypeId
                               where ls.OperationZoneId == OperationZoneId &&
                                     ls.LogisticType == LogisticType &&
                                     ls.LogisticCompany == CourierCompany &&
                                     ls.ModuleType == ModuleType &&
                                     lsw.ParcelType == ParcelType &&
                                     lsw.PackageType == PackageType
                               select new FrayteLogisticWeight
                               {
                                   LogisticWeightId = lsw.LogisticServiceWeightId,
                                   ShipmentType = new LogisticShipmentType()
                                   {
                                       ShipmentTypeId = lsst.LogisticServiceShipmentTypeId,
                                       LogisticServiceId = lsst.LogisticServiceId,
                                       LogisticType = lsst.LogisticType,
                                       LogisticDescription = lsst.LogisticDescription,
                                       LogisticDescriptionDisplay = lsst.LogisticDescriptionDisplayType
                                   },
                                   WeightFrom = lsw.WeightFromDisplay,
                                   WeightTo = lsw.WeightToDisplay,
                                   UnitOfMeasurement = lsw.UOM,
                                   WeightUnit = lsw.WeightUnit
                               }).ToList();
                }
                else if (CourierCompany == FrayteLogisticServiceType.Yodel)
                {
                    _weight = (from ls in dbContext.LogisticServices
                               join lsst in dbContext.LogisticServiceShipmentTypes on ls.LogisticServiceId equals lsst.LogisticServiceId
                               join lsw in dbContext.LogisticServiceWeights on lsst.LogisticServiceShipmentTypeId equals lsw.LogisticServiceShipmentTypeId
                               where ls.OperationZoneId == OperationZoneId &&
                                     ls.LogisticType == LogisticType &&
                                     ls.LogisticCompany == CourierCompany &&
                                     ls.ModuleType == ModuleType &&
                                     lsw.ParcelType == ParcelType &&
                                     lsw.PackageType == PackageType
                               select new FrayteLogisticWeight
                               {
                                   LogisticWeightId = lsw.LogisticServiceWeightId,
                                   ShipmentType = new LogisticShipmentType()
                                   {
                                       ShipmentTypeId = lsst.LogisticServiceShipmentTypeId,
                                       LogisticServiceId = lsst.LogisticServiceId,
                                       LogisticType = lsst.LogisticType,
                                       LogisticDescription = lsst.LogisticDescription,
                                       LogisticDescriptionDisplay = lsst.LogisticDescriptionDisplayType
                                   },
                                   WeightFrom = lsw.WeightFrom,
                                   WeightTo = lsw.WeightTo,
                                   UnitOfMeasurement = lsw.UOM,
                                   WeightUnit = lsw.WeightUnit
                               }).ToList();
                }
                else if (CourierCompany == FrayteLogisticServiceType.Hermes)
                {
                    _weight = (from ls in dbContext.LogisticServices
                               join lsst in dbContext.LogisticServiceShipmentTypes on ls.LogisticServiceId equals lsst.LogisticServiceId
                               join lsw in dbContext.LogisticServiceWeights on lsst.LogisticServiceShipmentTypeId equals lsw.LogisticServiceShipmentTypeId
                               where ls.OperationZoneId == OperationZoneId &&
                                     ls.LogisticType == LogisticType &&
                                     ls.LogisticCompany == CourierCompany &&
                                     ls.ModuleType == ModuleType &&
                                     lsw.PackageType == PackageType
                               select new FrayteLogisticWeight
                               {
                                   LogisticWeightId = lsw.LogisticServiceWeightId,
                                   ShipmentType = new LogisticShipmentType()
                                   {
                                       ShipmentTypeId = lsst.LogisticServiceShipmentTypeId,
                                       LogisticServiceId = lsst.LogisticServiceId,
                                       LogisticType = lsst.LogisticType,
                                       LogisticDescription = lsst.LogisticDescription,
                                       LogisticDescriptionDisplay = lsst.LogisticDescriptionDisplayType
                                   },
                                   WeightFrom = lsw.WeightFrom,
                                   WeightTo = lsw.WeightTo,
                                   UnitOfMeasurement = lsw.UOM,
                                   WeightUnit = lsw.WeightUnit,
                                   PackageType = lsw.PackageType,
                                   ParcelType = lsw.ParcelType,
                               }).ToList();
                }
                else if (CourierCompany == FrayteLogisticServiceType.DHL)
                {
                    _weight = (from ls in dbContext.LogisticServices
                               join lsst in dbContext.LogisticServiceShipmentTypes on ls.LogisticServiceId equals lsst.LogisticServiceId
                               join lsw in dbContext.LogisticServiceWeights on lsst.LogisticServiceShipmentTypeId equals lsw.LogisticServiceShipmentTypeId
                               where ls.OperationZoneId == OperationZoneId &&
                                     ls.LogisticType == LogisticType &&
                                     ls.LogisticCompany == CourierCompany &&
                                     ls.ModuleType == ModuleType
                               select new FrayteLogisticWeight
                               {
                                   LogisticWeightId = lsw.LogisticServiceWeightId,
                                   ShipmentType = new LogisticShipmentType()
                                   {
                                       ShipmentTypeId = lsst.LogisticServiceShipmentTypeId,
                                       LogisticServiceId = lsst.LogisticServiceId,
                                       LogisticType = lsst.LogisticType,
                                       LogisticDescription = lsst.LogisticDescription,
                                       LogisticDescriptionDisplay = lsst.LogisticDescriptionDisplayType
                                   },
                                   WeightFrom = lsw.WeightFrom,
                                   WeightTo = lsw.WeightTo,
                                   UnitOfMeasurement = lsw.UOM,
                                   WeightUnit = lsw.WeightUnit,
                                   PackageType = lsw.PackageType,
                                   ParcelType = lsw.ParcelType,
                               }).ToList();
                }
            }
            else
            {
                _weight = (from ls in dbContext.LogisticServices
                           join lsst in dbContext.LogisticServiceShipmentTypes on ls.LogisticServiceId equals lsst.LogisticServiceId
                           join lsw in dbContext.LogisticServiceWeights on lsst.LogisticServiceShipmentTypeId equals lsw.LogisticServiceShipmentTypeId
                           where ls.OperationZoneId == OperationZoneId &&
                                 ls.LogisticType == LogisticType &&
                                 ls.LogisticCompany == CourierCompany &&
                                 ls.RateType == RateType &&
                                 ls.ModuleType == ModuleType &&
                                 lsw.ParcelType == ParcelType &&
                                 lsw.PackageType == PackageType
                           select new FrayteLogisticWeight
                           {
                               LogisticWeightId = lsw.LogisticServiceWeightId,
                               ShipmentType = new LogisticShipmentType()
                               {
                                   ShipmentTypeId = lsst.LogisticServiceShipmentTypeId,
                                   LogisticServiceId = lsst.LogisticServiceId,
                                   LogisticType = lsst.LogisticType,
                                   LogisticDescription = lsst.LogisticDescription,
                                   LogisticDescriptionDisplay = lsst.LogisticDescriptionDisplayType
                               },
                               WeightFrom = lsw.WeightFrom,
                               WeightTo = lsw.WeightTo,
                               UnitOfMeasurement = lsw.UOM,
                               WeightUnit = lsw.WeightUnit,
                               PackageType = lsw.PackageType,
                               ParcelType = lsw.ParcelType,
                           }).ToList();
            }

            return _weight;
        }

        public List<FrayteCourierAccount> GetCourierAccount(int OperationZoneId, string LogisticType, string CourierCompany, string RateType, string ModuleType)
        {
            List<FrayteCourierAccount> _courieraccountList = new List<FrayteCourierAccount>();
            try
            {
                switch (LogisticType)
                {
                    case FrayteLogisticType.Import:
                        var Importlist = (from ls in dbContext.LogisticServices
                                          join lsca in dbContext.LogisticServiceCourierAccounts on ls.LogisticServiceId equals lsca.LogisticServiceId
                                          join oz in dbContext.OperationZones on ls.OperationZoneId equals oz.OperationZoneId
                                          where
                                               ls.OperationZoneId == OperationZoneId &&
                                               ls.LogisticType == LogisticType &&
                                               ls.LogisticCompany == CourierCompany &&
                                               ls.RateType == RateType &&
                                               ls.ModuleType == ModuleType &&
                                               lsca.IsActive == true
                                          select new
                                          {
                                              ls.LogisticServiceId,
                                              ls.LogisticCompany,
                                              ls.LogisticCompanyDisplay,
                                              ls.LogisticType,
                                              ls.LogisticTypeDisplay,
                                              ls.RateType,
                                              ls.RateTypeDisplay,
                                              ls.ModuleType,
                                              oz.OperationZoneId,
                                              oz.Name,
                                              lsca.LogisticServiceCourierAccountId,
                                              lsca.IntegrationAccountId,
                                              lsca.AccountNo,
                                              lsca.AccountCountryCode,
                                              lsca.Description,
                                              lsca.ColorCode,
                                              lsca.IsActive
                                          }).ToList();

                        if (Importlist != null && Importlist.Count > 0)
                        {
                            FrayteCourierAccount Importcourier;
                            foreach (var cc in Importlist)
                            {
                                Importcourier = new FrayteCourierAccount();
                                Importcourier.LogisticServiceCourierAccountId = cc.LogisticServiceCourierAccountId;
                                Importcourier.OperationZone = new FrayteOperationZone()
                                {
                                    OperationZoneId = cc.OperationZoneId,
                                    OperationZoneName = cc.Name
                                };
                                Importcourier.LogisticService = new LogisticShipmentService()
                                {
                                    LogisticServiceId = cc.LogisticServiceId,
                                    OperationZoneId = cc.OperationZoneId,
                                    LogisticCompany = cc.LogisticCompany,
                                    LogisticCompanyDisplay = cc.LogisticCompanyDisplay,
                                    LogisticType = cc.LogisticType,
                                    LogisticTypeDisplay = cc.LogisticTypeDisplay,
                                    RateType = cc.RateType,
                                    RateTypeDisplay = cc.RateTypeDisplay,
                                    ModuleType = cc.ModuleType,
                                };
                                Importcourier.AccountNo = cc.AccountNo;
                                Importcourier.IntegrationAccountId = cc.IntegrationAccountId;
                                Importcourier.AccountCountryCode = cc.AccountCountryCode;
                                Importcourier.Description = cc.Description;
                                Importcourier.ColorCode = cc.ColorCode;
                                Importcourier.IsActive = cc.IsActive;
                                _courieraccountList.Add(Importcourier);
                            }
                        }
                        return _courieraccountList;
                    case FrayteLogisticType.Export:
                        var Exportlist = (from ls in dbContext.LogisticServices
                                          join lsca in dbContext.LogisticServiceCourierAccounts on ls.LogisticServiceId equals lsca.LogisticServiceId
                                          join oz in dbContext.OperationZones on ls.OperationZoneId equals oz.OperationZoneId
                                          where
                                               ls.OperationZoneId == OperationZoneId &&
                                               ls.LogisticType == LogisticType &&
                                               ls.LogisticCompany == CourierCompany &&
                                               ls.RateType == RateType &&
                                               ls.ModuleType == ModuleType &&
                                               lsca.IsActive == true
                                          select new
                                          {
                                              ls.LogisticServiceId,
                                              ls.LogisticCompany,
                                              ls.LogisticCompanyDisplay,
                                              ls.LogisticType,
                                              ls.LogisticTypeDisplay,
                                              ls.RateType,
                                              ls.RateTypeDisplay,
                                              ls.ModuleType,
                                              oz.OperationZoneId,
                                              oz.Name,
                                              lsca.LogisticServiceCourierAccountId,
                                              lsca.IntegrationAccountId,
                                              lsca.AccountNo,
                                              lsca.AccountCountryCode,
                                              lsca.Description,
                                              lsca.ColorCode,
                                              lsca.IsActive
                                          }).ToList();

                        if (Exportlist != null && Exportlist.Count > 0)
                        {
                            FrayteCourierAccount Exportcourier;
                            foreach (var cc in Exportlist)
                            {
                                Exportcourier = new FrayteCourierAccount();
                                Exportcourier.LogisticServiceCourierAccountId = cc.LogisticServiceCourierAccountId;
                                Exportcourier.OperationZone = new FrayteOperationZone()
                                {
                                    OperationZoneId = cc.OperationZoneId,
                                    OperationZoneName = cc.Name
                                };
                                Exportcourier.LogisticService = new LogisticShipmentService()
                                {
                                    LogisticServiceId = cc.LogisticServiceId,
                                    OperationZoneId = cc.OperationZoneId,
                                    LogisticCompany = cc.LogisticCompany,
                                    LogisticCompanyDisplay = cc.LogisticCompanyDisplay,
                                    LogisticType = cc.LogisticType,
                                    LogisticTypeDisplay = cc.LogisticTypeDisplay,
                                    RateType = cc.RateType,
                                    RateTypeDisplay = cc.RateTypeDisplay,
                                    ModuleType = cc.ModuleType,
                                };
                                Exportcourier.AccountNo = cc.AccountNo;
                                Exportcourier.IntegrationAccountId = cc.IntegrationAccountId;
                                Exportcourier.AccountCountryCode = cc.AccountCountryCode;
                                Exportcourier.Description = cc.Description;
                                Exportcourier.IsActive = cc.IsActive;
                                _courieraccountList.Add(Exportcourier);
                            }
                        }
                        return _courieraccountList;
                    case FrayteLogisticType.ThirdParty:
                        var ThirdPartylist = (from ls in dbContext.LogisticServices
                                              join lsca in dbContext.LogisticServiceCourierAccounts on ls.LogisticServiceId equals lsca.LogisticServiceId
                                              join oz in dbContext.OperationZones on ls.OperationZoneId equals oz.OperationZoneId
                                              where
                                                   ls.OperationZoneId == OperationZoneId &&
                                                   ls.LogisticType == LogisticType &&
                                                   ls.LogisticCompany == CourierCompany &&
                                                   ls.RateType == RateType &&
                                                   ls.ModuleType == ModuleType &&
                                                   lsca.IsActive == true
                                              select new
                                              {
                                                  ls.LogisticServiceId,
                                                  ls.LogisticCompany,
                                                  ls.LogisticCompanyDisplay,
                                                  ls.LogisticType,
                                                  ls.LogisticTypeDisplay,
                                                  ls.RateType,
                                                  ls.RateTypeDisplay,
                                                  ls.ModuleType,
                                                  oz.OperationZoneId,
                                                  oz.Name,
                                                  lsca.LogisticServiceCourierAccountId,
                                                  lsca.IntegrationAccountId,
                                                  lsca.AccountNo,
                                                  lsca.AccountCountryCode,
                                                  lsca.Description,
                                                  lsca.ColorCode,
                                                  lsca.IsActive
                                              }).ToList();

                        if (ThirdPartylist != null && ThirdPartylist.Count > 0)
                        {
                            FrayteCourierAccount ThirdPartycourier;
                            foreach (var cc in ThirdPartylist)
                            {
                                ThirdPartycourier = new FrayteCourierAccount();
                                ThirdPartycourier.LogisticServiceCourierAccountId = cc.LogisticServiceCourierAccountId;
                                ThirdPartycourier.OperationZone = new FrayteOperationZone()
                                {
                                    OperationZoneId = cc.OperationZoneId,
                                    OperationZoneName = cc.Name
                                };
                                ThirdPartycourier.LogisticService = new LogisticShipmentService()
                                {
                                    LogisticServiceId = cc.LogisticServiceId,
                                    OperationZoneId = cc.OperationZoneId,
                                    LogisticCompany = cc.LogisticCompany,
                                    LogisticCompanyDisplay = cc.LogisticCompanyDisplay,
                                    LogisticType = cc.LogisticType,
                                    LogisticTypeDisplay = cc.LogisticTypeDisplay,
                                    RateType = cc.RateType,
                                    RateTypeDisplay = cc.RateTypeDisplay,
                                    ModuleType = cc.ModuleType,
                                };
                                ThirdPartycourier.IntegrationAccountId = cc.IntegrationAccountId;
                                ThirdPartycourier.AccountCountryCode = cc.AccountCountryCode;
                                ThirdPartycourier.Description = cc.Description;
                                ThirdPartycourier.AccountNo = cc.AccountNo;
                                ThirdPartycourier.IsActive = cc.IsActive;
                                _courieraccountList.Add(ThirdPartycourier);
                            }
                        }
                        return _courieraccountList;
                    case FrayteLogisticType.UKShipment:
                        var UKlist = (from ls in dbContext.LogisticServices
                                      join lsca in dbContext.LogisticServiceCourierAccounts on ls.LogisticServiceId equals lsca.LogisticServiceId
                                      join oz in dbContext.OperationZones on ls.OperationZoneId equals oz.OperationZoneId
                                      where
                                           ls.OperationZoneId == OperationZoneId &&
                                           ls.LogisticType == LogisticType &&
                                           ls.LogisticCompany == CourierCompany &&
                                           ls.ModuleType == ModuleType &&
                                           lsca.IsActive == true
                                      select new
                                      {
                                          ls.LogisticServiceId,
                                          ls.LogisticCompany,
                                          ls.LogisticCompanyDisplay,
                                          ls.LogisticType,
                                          ls.LogisticTypeDisplay,
                                          ls.RateType,
                                          ls.RateTypeDisplay,
                                          ls.ModuleType,
                                          oz.OperationZoneId,
                                          oz.Name,
                                          lsca.LogisticServiceCourierAccountId,
                                          lsca.IntegrationAccountId,
                                          lsca.AccountNo,
                                          lsca.AccountCountryCode,
                                          lsca.Description,
                                          lsca.ColorCode,
                                          lsca.IsActive
                                      }).ToList();

                        if (UKlist != null && UKlist.Count > 0)
                        {
                            FrayteCourierAccount UKcourier;
                            foreach (var cc in UKlist)
                            {
                                UKcourier = new FrayteCourierAccount();
                                UKcourier.LogisticServiceCourierAccountId = cc.LogisticServiceCourierAccountId;
                                UKcourier.OperationZone = new FrayteOperationZone()
                                {
                                    OperationZoneId = cc.OperationZoneId,
                                    OperationZoneName = cc.Name
                                };
                                UKcourier.LogisticService = new LogisticShipmentService()
                                {
                                    LogisticServiceId = cc.LogisticServiceId,
                                    OperationZoneId = cc.OperationZoneId,
                                    LogisticCompany = cc.LogisticCompany,
                                    LogisticCompanyDisplay = cc.LogisticCompanyDisplay,
                                    LogisticType = cc.LogisticType,
                                    LogisticTypeDisplay = cc.LogisticTypeDisplay,
                                    RateType = cc.RateType,
                                    RateTypeDisplay = cc.RateTypeDisplay,
                                    ModuleType = cc.ModuleType,
                                };
                                UKcourier.AccountNo = cc.AccountNo;
                                UKcourier.IntegrationAccountId = cc.IntegrationAccountId;
                                UKcourier.AccountCountryCode = cc.AccountCountryCode;
                                UKcourier.Description = cc.Description;
                                UKcourier.IsActive = cc.IsActive;
                                _courieraccountList.Add(UKcourier);
                            }
                        }
                        return _courieraccountList;
                    case FrayteLogisticType.EUExport:
                        var Economylist = (from ls in dbContext.LogisticServices
                                           join lsca in dbContext.LogisticServiceCourierAccounts on ls.LogisticServiceId equals lsca.LogisticServiceId
                                           join oz in dbContext.OperationZones on ls.OperationZoneId equals oz.OperationZoneId
                                           where
                                                ls.OperationZoneId == OperationZoneId &&
                                                ls.LogisticType == LogisticType &&
                                                ls.LogisticCompany == CourierCompany &&
                                                ls.RateType == RateType &&
                                                ls.ModuleType == ModuleType &&
                                                lsca.IsActive == true
                                           select new
                                           {
                                               ls.LogisticServiceId,
                                               ls.LogisticCompany,
                                               ls.LogisticCompanyDisplay,
                                               ls.LogisticType,
                                               ls.LogisticTypeDisplay,
                                               ls.RateType,
                                               ls.RateTypeDisplay,
                                               ls.ModuleType,
                                               oz.OperationZoneId,
                                               oz.Name,
                                               lsca.LogisticServiceCourierAccountId,
                                               lsca.IntegrationAccountId,
                                               lsca.AccountNo,
                                               lsca.AccountCountryCode,
                                               lsca.Description,
                                               lsca.ColorCode,
                                               lsca.IsActive
                                           }).ToList();

                        if (Economylist != null && Economylist.Count > 0)
                        {
                            FrayteCourierAccount Economycourier;
                            foreach (var cc in Economylist)
                            {
                                Economycourier = new FrayteCourierAccount();
                                Economycourier.LogisticServiceCourierAccountId = cc.LogisticServiceCourierAccountId;
                                Economycourier.OperationZone = new FrayteOperationZone()
                                {
                                    OperationZoneId = cc.OperationZoneId,
                                    OperationZoneName = cc.Name
                                };
                                Economycourier.LogisticService = new LogisticShipmentService()
                                {
                                    LogisticServiceId = cc.LogisticServiceId,
                                    OperationZoneId = cc.OperationZoneId,
                                    LogisticCompany = cc.LogisticCompany,
                                    LogisticCompanyDisplay = cc.LogisticCompanyDisplay,
                                    LogisticType = cc.LogisticType,
                                    LogisticTypeDisplay = cc.LogisticTypeDisplay,
                                    RateType = cc.RateType,
                                    RateTypeDisplay = cc.RateTypeDisplay,
                                    ModuleType = cc.ModuleType,
                                };
                                Economycourier.AccountNo = cc.AccountNo;
                                Economycourier.IntegrationAccountId = cc.IntegrationAccountId;
                                Economycourier.AccountCountryCode = cc.AccountCountryCode;
                                Economycourier.Description = cc.Description;
                                Economycourier.IsActive = cc.IsActive;
                                _courieraccountList.Add(Economycourier);
                            }
                        }
                        return _courieraccountList;
                    case FrayteLogisticType.EUImport:
                        var EUImportList = (from ls in dbContext.LogisticServices
                                            join lsca in dbContext.LogisticServiceCourierAccounts on ls.LogisticServiceId equals lsca.LogisticServiceId
                                            join oz in dbContext.OperationZones on ls.OperationZoneId equals oz.OperationZoneId
                                            where
                                                 ls.OperationZoneId == OperationZoneId &&
                                                 ls.LogisticType == LogisticType &&
                                                 ls.LogisticCompany == CourierCompany &&
                                                 ls.RateType == RateType &&
                                                 ls.ModuleType == ModuleType &&
                                                 lsca.IsActive == true
                                            select new
                                            {
                                                ls.LogisticServiceId,
                                                ls.LogisticCompany,
                                                ls.LogisticCompanyDisplay,
                                                ls.LogisticType,
                                                ls.LogisticTypeDisplay,
                                                ls.RateType,
                                                ls.RateTypeDisplay,
                                                ls.ModuleType,
                                                oz.OperationZoneId,
                                                oz.Name,
                                                lsca.LogisticServiceCourierAccountId,
                                                lsca.IntegrationAccountId,
                                                lsca.AccountNo,
                                                lsca.AccountCountryCode,
                                                lsca.Description,
                                                lsca.ColorCode,
                                                lsca.IsActive
                                            }).ToList();

                        if (EUImportList != null && EUImportList.Count > 0)
                        {
                            FrayteCourierAccount EconomyImportcourier;
                            foreach (var cc in EUImportList)
                            {
                                EconomyImportcourier = new FrayteCourierAccount();
                                EconomyImportcourier.LogisticServiceCourierAccountId = cc.LogisticServiceCourierAccountId;
                                EconomyImportcourier.OperationZone = new FrayteOperationZone()
                                {
                                    OperationZoneId = cc.OperationZoneId,
                                    OperationZoneName = cc.Name
                                };
                                EconomyImportcourier.LogisticService = new LogisticShipmentService()
                                {
                                    LogisticServiceId = cc.LogisticServiceId,
                                    OperationZoneId = cc.OperationZoneId,
                                    LogisticCompany = cc.LogisticCompany,
                                    LogisticCompanyDisplay = cc.LogisticCompanyDisplay,
                                    LogisticType = cc.LogisticType,
                                    LogisticTypeDisplay = cc.LogisticTypeDisplay,
                                    RateType = cc.RateType,
                                    RateTypeDisplay = cc.RateTypeDisplay,
                                    ModuleType = cc.ModuleType,
                                };
                                EconomyImportcourier.AccountNo = cc.AccountNo;
                                EconomyImportcourier.IntegrationAccountId = cc.IntegrationAccountId;
                                EconomyImportcourier.AccountCountryCode = cc.AccountCountryCode;
                                EconomyImportcourier.Description = cc.Description;
                                EconomyImportcourier.IsActive = cc.IsActive;
                                _courieraccountList.Add(EconomyImportcourier);
                            }
                        }
                        return _courieraccountList;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {

            }
            return _courieraccountList;
        }

        public FrayteZoneBaseRateCard GetBaseRateCard(int baseRateCardId)
        {
            FrayteZoneBaseRateCard zoneBaseRate = new FrayteZoneBaseRateCard();
            LogisticServiceBaseRateCard rateCard = dbContext.LogisticServiceBaseRateCards.Find(baseRateCardId);
            if (rateCard != null)
            {
                var ratedetail = (from lsbrc in dbContext.LogisticServiceBaseRateCards
                                  join lsw in dbContext.LogisticServiceWeights on lsbrc.LogisticServiceWeightId equals lsw.LogisticServiceWeightId
                                  join lsca in dbContext.LogisticServiceCourierAccounts on lsbrc.LogisticServiceCourierAccountId equals lsca.LogisticServiceCourierAccountId into leftCourier
                                  from co in leftCourier.DefaultIfEmpty()
                                  join lsst in dbContext.LogisticServiceShipmentTypes on lsbrc.LogisticServiceShipmentTypeId equals lsst.LogisticServiceShipmentTypeId
                                  join ls in dbContext.LogisticServices on lsst.LogisticServiceId equals ls.LogisticServiceId
                                  join lsz in dbContext.LogisticServiceZones on ls.LogisticServiceId equals lsz.LogisticServiceId
                                  join oz in dbContext.OperationZones on ls.OperationZoneId equals oz.OperationZoneId
                                  where
                                       lsbrc.LogisticServiceBaseRateCardId == rateCard.LogisticServiceBaseRateCardId &&
                                       lsbrc.LogisticServiceCourierAccountId == rateCard.LogisticServiceCourierAccountId &&
                                       lsbrc.LogisticServiceWeightId == rateCard.LogisticServiceWeightId
                                  select new FrayteZoneBaseRateCard
                                  {
                                      ZoneRateCardId = rateCard.LogisticServiceBaseRateCardId,
                                      zone = new FrayteZone()
                                      {
                                          OperationZoneId = lsbrc.OperationZoneId,
                                          ZoneId = lsz.LogisticServiceZoneId,
                                          ZoneName = lsz.ZoneName,
                                          ZoneDisplayName = lsz.ZoneDisplayName
                                      },
                                      shipmentType = new LogisticShipmentType()
                                      {
                                          ShipmentTypeId = lsst.LogisticServiceShipmentTypeId,
                                          LogisticServiceId = lsst.LogisticServiceId,
                                          LogisticType = lsst.LogisticType,
                                          LogisticDescription = lsst.LogisticDescription,
                                          LogisticDescriptionDisplay = lsst.LogisticDescriptionDisplayType
                                      },
                                      courierAccount = new FrayteCourierAccount()
                                      {
                                          LogisticServiceCourierAccountId = (co == null ? 0 : co.LogisticServiceCourierAccountId), //lsca.LogisticServiceCourierAccountId,
                                          IntegrationAccountId = (co == null ? "" : co.IntegrationAccountId),
                                          AccountNo = (co == null ? "" : co.AccountNo),
                                          AccountCountryCode = (co == null ? "" : co.AccountCountryCode),
                                          Description = (co == null ? "" : co.Description),
                                          ColorCode = (co == null ? "" : co.ColorCode),
                                          LogisticService = new LogisticShipmentService()
                                          {
                                              LogisticServiceId = ls.LogisticServiceId,
                                              OperationZoneId = ls.OperationZoneId,
                                              LogisticCompany = ls.LogisticCompany,
                                              LogisticCompanyDisplay = ls.LogisticCompanyDisplay,
                                              LogisticType = ls.LogisticType,
                                              LogisticTypeDisplay = ls.LogisticTypeDisplay,
                                              RateType = ls.RateType,
                                              RateTypeDisplay = ls.RateTypeDisplay,
                                              ModuleType = ls.ModuleType
                                          },
                                          OperationZone = new FrayteOperationZone()
                                          {
                                              OperationZoneId = oz.OperationZoneId,
                                              OperationZoneName = oz.Name
                                          },
                                      },
                                      LogisticWeight = new FrayteLogisticWeight()
                                      {
                                          LogisticWeightId = lsw.LogisticServiceWeightId,
                                          ShipmentType = new LogisticShipmentType()
                                          {
                                              ShipmentTypeId = lsst.LogisticServiceShipmentTypeId,
                                              LogisticServiceId = lsst.LogisticServiceId,
                                              LogisticType = lsst.LogisticType,
                                              LogisticDescription = lsst.LogisticDescription,
                                              LogisticDescriptionDisplay = lsst.LogisticDescriptionDisplayType
                                          },
                                          WeightFrom = lsw.WeightFromDisplay,
                                          WeightTo = lsw.WeightToDisplay,
                                          UnitOfMeasurement = lsw.UOM
                                      },
                                      Rate = lsbrc.LogisticRate,
                                      LogisticType = ls.LogisticType,
                                      CurrencyCode = lsbrc.LogisticCurrency
                                  }).FirstOrDefault();

                zoneBaseRate = ratedetail;
            }
            return zoneBaseRate;
        }

        public FrayteResult SaveZoneBaseRateCardLimit(List<FryateZoneBaseRateCardLimit> _baseRateCardLimit)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                LogisticServiceBaseRateCard lsbrc;
                if (_baseRateCardLimit != null && _baseRateCardLimit.Count > 0)
                {
                    foreach (var rateLimit in _baseRateCardLimit)
                    {
                        lsbrc = dbContext.LogisticServiceBaseRateCards.Find(rateLimit.ZoneRateCardId);
                        if (lsbrc != null)
                        {
                            lsbrc.LogisticRate = rateLimit.OverWeightLimitRate;
                            lsbrc.LogisticCurrency = rateLimit.Currency;
                            dbContext.Entry(lsbrc).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }
                    }
                }
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

        public FrayteResult SaveDimensionBaseRate(List<FrayteDimensionBaseRateCard> _baseRateCardLimit)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                LogisticServiceBaseRateCard lsbrc;
                if (_baseRateCardLimit != null && _baseRateCardLimit.Count > 0)
                {
                    foreach (var rateLimit in _baseRateCardLimit)
                    {
                        lsbrc = dbContext.LogisticServiceBaseRateCards.Find(rateLimit.ZoneRateCardId);
                        if (lsbrc != null)
                        {
                            lsbrc.LogisticRate = rateLimit.DimensionRate;
                            lsbrc.LogisticCurrency = rateLimit.Currency;
                            dbContext.Entry(lsbrc).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }
                    }
                }
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

        public List<FrayteZoneBaseRateCard> GetUKAddOnRate(int OperationZoneId, string LogisticType, string CourierCompany, string RateType, string ParcelType, string PackageType, string ModuleType)
        {
            try
            {
                var addonrate = (from lsa in dbContext.LogisticServiceAddOns
                                 join lswl in dbContext.LogisticServiceWeightLimits on lsa.LogisticServiceWeightLimitId equals lswl.LogisticServiceWeightLimitId
                                 join lsz in dbContext.LogisticServiceZones on lsa.LogisticServiceZoneId equals lsz.LogisticServiceZoneId
                                 join ls in dbContext.LogisticServices on lsa.LogisticServiceId equals ls.LogisticServiceId
                                 where
                                        ls.OperationZoneId == OperationZoneId &&
                                        ls.LogisticCompany == CourierCompany &&
                                        ls.LogisticType == LogisticType &&
                                        lswl.ParcelType == ParcelType &&
                                        lswl.PackageType == PackageType
                                 select new FrayteZoneBaseRateCard
                                 {
                                     OperationZoneId = ls.OperationZoneId,
                                     ZoneRateCardId = lsa.LogisticServiceAddOnId,
                                     zone = new FrayteZone
                                     {
                                         OperationZoneId = ls.OperationZoneId,
                                         ZoneId = lsz.LogisticServiceZoneId,
                                         ZoneName = lsz.ZoneName,
                                         ZoneDisplayName = lsz.ZoneDisplayName,
                                         LogisticType = ls.LogisticType,
                                         CourierComapny = ls.LogisticCompany,
                                         RateType = ls.RateType,
                                         ModuleType = ls.ModuleType
                                     },
                                     shipmentType = new LogisticShipmentType
                                     {
                                         LogisticDescription = lswl.AddOnDescription,
                                         LogisticDescriptionDisplay = lswl.AddOnDescription
                                     },
                                     LogisticWeight = new FrayteLogisticWeight
                                     {
                                         WeightFrom = lsa.WeightFrom,
                                         WeightTo = lsa.WeightTo,
                                     },
                                     Rate = lsa.AddOnRate,
                                     BusinessCurrency = lsa.AddOnCurrency,
                                     ModuleType = ls.ModuleType
                                 }).ToList();

                return addonrate;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<FrayteAddOnRateCard> GetAddOnRate(int OperationZoneId, string LogisticType, string CourierCompany, string RateType, string ParcelType, string PackageType, string ModuleType)
        {
            try
            {
                var addonrate = (from lsa in dbContext.LogisticServiceAddOns
                                 join lswl in dbContext.LogisticServiceWeightLimits on lsa.LogisticServiceWeightLimitId equals lswl.LogisticServiceWeightLimitId
                                 join lsz in dbContext.LogisticServiceZones on lsa.LogisticServiceZoneId equals lsz.LogisticServiceZoneId
                                 join ls in dbContext.LogisticServices on lsa.LogisticServiceId equals ls.LogisticServiceId
                                 where
                                      ls.OperationZoneId == OperationZoneId &&
                                      ls.LogisticCompany == CourierCompany &&
                                      ls.LogisticType == LogisticType &&
                                      ls.RateType == RateType
                                 select new FrayteZoneBaseRateCard
                                 {
                                     OperationZoneId = ls.OperationZoneId,
                                     ZoneRateCardId = lsa.LogisticServiceAddOnId,
                                     zone = new FrayteZone
                                     {
                                         OperationZoneId = ls.OperationZoneId,
                                         ZoneId = lsz.LogisticServiceZoneId,
                                         ZoneName = lsz.ZoneName,
                                         ZoneDisplayName = lsz.ZoneDisplayName,
                                         LogisticType = ls.LogisticType,
                                         CourierComapny = ls.LogisticCompany,
                                         RateType = ls.RateType,
                                         ModuleType = ls.ModuleType
                                     },
                                     shipmentType = new LogisticShipmentType
                                     {
                                         LogisticDescription = lswl.AddOnDescription,
                                         LogisticDescriptionDisplay = lswl.AddOnDescription
                                     },
                                     LogisticWeight = new FrayteLogisticWeight
                                     {
                                         WeightFrom = lsa.WeightFrom,
                                         WeightTo = lsa.WeightTo,
                                     },
                                     Rate = lsa.AddOnRate,
                                     BusinessCurrency = lsa.AddOnCurrency,
                                     ModuleType = ls.ModuleType
                                 }).ToList();

                var group = addonrate.GroupBy(p => new { p.LogisticWeight.WeightFrom, p.LogisticWeight.WeightTo, p.shipmentType.LogisticDescriptionDisplay })
                                     .Select(g => new FrayteAddOnRateCard
                                     {
                                         shipmentType = g.Key.LogisticDescriptionDisplay,
                                         WeightFrom = g.Key.WeightFrom,
                                         WeightTo = g.Key.WeightTo,
                                         Rate = g.Select(p => new { p.ZoneRateCardId, p.Rate, p.BusinessCurrency, p.IsChanged })
                                                 .GroupBy(m => new { m.ZoneRateCardId, m.Rate, m.BusinessCurrency, m.IsChanged })
                                                 .Select(t => new AddOnRate
                                                 {
                                                     ZoneRateCardId = t.Key.ZoneRateCardId,
                                                     Rate = t.Key.Rate,
                                                     IsChanged = t.Key.IsChanged,
                                                     CurrencyCode = t.Key.BusinessCurrency
                                                 }).ToList()
                                     }).ToList();

                return group;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public FrayteResult UpdateUKAddOnRate(List<FrayteAddRate> _Ukaddon)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                LogisticServiceAddOn lsao;
                if (_Ukaddon != null && _Ukaddon.Count > 0)
                {
                    foreach (var rateLimit in _Ukaddon)
                    {
                        foreach (var rate in rateLimit.Rate)
                        {
                            if (rate.IsChanged == true)
                            {
                                lsao = dbContext.LogisticServiceAddOns.Find(rate.ZoneRateCardId);
                                if (lsao != null)
                                {
                                    lsao.AddOnRate = rate.Rate;
                                    dbContext.Entry(lsao).State = System.Data.Entity.EntityState.Modified;
                                    dbContext.SaveChanges();
                                }
                            }
                        }
                    }
                    result.Status = true;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Errors = new List<string>();
                result.Errors.Add(ex.Message);
            }
            return result;
        }

        public FrayteResult UpdateAddOnRate(List<FrayteAddRate> _addrate)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                LogisticServiceAddOn lsao;
                if (_addrate != null && _addrate.Count > 0)
                {
                    foreach (var rateLimit in _addrate)
                    {
                        foreach (var rate in rateLimit.Rate)
                        {
                            if (rate.IsChanged == true)
                            {
                                lsao = dbContext.LogisticServiceAddOns.Find(rate.ZoneRateCardId);
                                if (lsao != null)
                                {
                                    lsao.AddOnRate = rate.Rate;
                                    dbContext.Entry(lsao).State = System.Data.Entity.EntityState.Modified;
                                    dbContext.SaveChanges();
                                }
                            }
                        }
                    }
                    result.Status = true;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Errors = new List<string>();
                result.Errors.Add(ex.Message);
            }
            return result;
        }

        public FrayteResult ValidAddOnRateId(int OperationZoneId, string LogisticType, string CourierCompany, string RateType, string PackageType, string ParcelType, string ModuleType)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                if (LogisticType == FrayteLogisticType.UKShipment)
                {
                    var AddOnId = (from ls in dbContext.LogisticServices
                                   join lswl in dbContext.LogisticServiceWeightLimits on ls.LogisticServiceId equals lswl.LogisticServiceId
                                   join lsao in dbContext.LogisticServiceAddOns on lswl.LogisticServiceWeightLimitId equals lsao.LogisticServiceWeightLimitId
                                   where
                                      ls.LogisticCompany == CourierCompany &&
                                      ls.LogisticType == LogisticType &&
                                      ls.OperationZoneId == OperationZoneId &&
                                      ls.ModuleType == ModuleType &&
                                      lswl.PackageType == PackageType &&
                                      lswl.ParcelType == ParcelType
                                   select new
                                   {
                                       lsao.LogisticServiceAddOnId
                                   }).FirstOrDefault();

                    if (AddOnId != null)
                    {
                        result.Status = true;
                    }
                    else
                    {
                        result.Status = false;
                    }
                    return result;
                }
                else
                {
                    var AddOnId = (from ls in dbContext.LogisticServices
                                   join lswl in dbContext.LogisticServiceWeightLimits on ls.LogisticServiceId equals lswl.LogisticServiceId
                                   join lsao in dbContext.LogisticServiceAddOns on lswl.LogisticServiceWeightLimitId equals lsao.LogisticServiceWeightLimitId
                                   where
                                      ls.LogisticCompany == CourierCompany &&
                                      ls.LogisticType == LogisticType &&
                                      ls.RateType == RateType &&
                                      ls.OperationZoneId == OperationZoneId &&
                                      ls.ModuleType == ModuleType &&
                                      lswl.PackageType == PackageType &&
                                      lswl.ParcelType == ParcelType
                                   select new
                                   {
                                       lsao.LogisticServiceAddOnId
                                   }).FirstOrDefault();

                    if (AddOnId != null)
                    {
                        result.Status = true;
                    }
                    else
                    {
                        result.Status = false;
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                return result;
            }
        }
    }
}
