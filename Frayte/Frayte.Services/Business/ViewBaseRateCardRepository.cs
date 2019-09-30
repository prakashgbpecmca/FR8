using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using Frayte.Services.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Validation;

namespace Frayte.Services.Business
{
    public class ViewBaseRateCardRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        public List<int> GetDistinctYear()
        {
            try
            {
                List<int> yearwithcurrent = new List<int>();
                List<int> year = dbContext.LogisticServiceBaseRateCardHistories.Select(p => p.ReportYear).Distinct().ToList();

                var data = DateTime.UtcNow.Year;
                year.Add(data);
                yearwithcurrent = year.Distinct().ToList();
                return yearwithcurrent;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<FrayteLogisticServiceItem> GetLogisticServiceItems(int Year)
        {
            FrayteOperationZone OperationZone = UtilityRepository.GetOperationZone();

            var list = (from ls in dbContext.LogisticServices
                        join lsbrch in dbContext.LogisticServiceBaseRateCardHistories on ls.LogisticServiceId equals lsbrch.LogisticServiceId into leftJoin
                        from tempRate in leftJoin.DefaultIfEmpty()
                        where ls.OperationZoneId == OperationZone.OperationZoneId &&
                              ls.IsActive == true &&
                              tempRate.ReportYear == Year
                        select new FrayteLogisticServiceItem
                        {
                            LogisticServiceId = ls.LogisticServiceId,
                            LogisticCompany = ls.LogisticCompany,
                            LogisticCompanyDisplay = ls.LogisticCompanyDisplay,
                            LogisticType = ls.LogisticType,
                            LogisticTypeDisplay = ls.LogisticTypeDisplay,
                            RateType = ls.RateType,
                            RateTypeDisplay = ls.RateTypeDisplay,
                            IssueDate = ls.IssuedDate.Value,
                            ExpiryDate = ls.ExpiryDate.Value
                        }).ToList();

            return list;
        }

        public string GetFileName(int LogisticServiceId, int ReportYear)
        {
            var name = (from lsbrch in dbContext.LogisticServiceBaseRateCardHistories
                        where lsbrch.LogisticServiceId == LogisticServiceId &&
                              lsbrch.ReportYear == ReportYear
                        select lsbrch).FirstOrDefault();

            if (name != null)
            {
                return name.ReportFileName;
            }
            else
            {
                return null;
            }
        }

        public List<FrayteLogisticServiceItem> GetCustomerLogisticServiceItems(int UserId)
        {
            FrayteOperationZone OperationZone = UtilityRepository.GetOperationZone();

            var list = (from cl in dbContext.CustomerLogistics
                        join ls in dbContext.LogisticServices on cl.LogisticServiceId equals ls.LogisticServiceId
                        where
                            cl.UserId == UserId &&
                            ls.IsActive == true
                        select new FrayteLogisticServiceItem
                        {
                            LogisticServiceId = ls.LogisticServiceId,
                            LogisticCompany = ls.LogisticCompany,
                            LogisticCompanyDisplay = ls.LogisticCompanyDisplay,
                            LogisticType = ls.LogisticType,
                            LogisticTypeDisplay = ls.LogisticTypeDisplay,
                            RateType = ls.RateType,
                            RateTypeDisplay = ls.RateTypeDisplay,
                            IssueDate = ls.IssuedDate.HasValue ? ls.IssuedDate.Value : DateTime.UtcNow,
                            ExpiryDate = ls.ExpiryDate.HasValue ? ls.ExpiryDate.Value : DateTime.UtcNow
                        }).OrderBy(p => p.LogisticCompany).ToList();

            return list;
        }

        public List<FrayteLogisticServiceItem> LogisticServiceItems()
        {
            var list = (from ls in dbContext.LogisticServices
                        join lsz in dbContext.LogisticServiceZones on ls.LogisticServiceId equals lsz.LogisticServiceId
                        join lsst in dbContext.LogisticServiceShipmentTypes on ls.LogisticServiceId equals lsst.LogisticServiceId
                        join lsw in dbContext.LogisticServiceWeights on lsst.LogisticServiceShipmentTypeId equals lsw.LogisticServiceShipmentTypeId
                        join lsca in dbContext.LogisticServiceCourierAccounts on ls.LogisticServiceId equals lsca.LogisticServiceId
                        join lsbrc in dbContext.LogisticServiceBaseRateCards on new { lsz.LogisticServiceZoneId, lsst.LogisticServiceShipmentTypeId, lsw.LogisticServiceWeightId, lsca.LogisticServiceCourierAccountId } equals
                                                                                new { lsbrc.LogisticServiceZoneId, lsbrc.LogisticServiceShipmentTypeId, lsbrc.LogisticServiceWeightId, lsbrc.LogisticServiceCourierAccountId }

                        where
                              ls.IsActive == true
                        select new FrayteLogisticServiceItem
                        {
                            LogisticServiceId = ls.LogisticServiceId,
                            LogisticCompany = ls.LogisticCompany,
                            LogisticCompanyDisplay = ls.LogisticCompanyDisplay,
                            LogisticType = ls.LogisticType,
                            LogisticTypeDisplay = ls.LogisticTypeDisplay,
                            RateType = ls.RateType,
                            RateTypeDisplay = ls.RateTypeDisplay
                        }).Distinct().ToList();

            return list;
        }

        public FrayteLogisticServiceItem GetLogisticServices(int LogisticServiceId)
        {
            var list = (from ls in dbContext.LogisticServices
                        where ls.LogisticServiceId == LogisticServiceId &&
                              ls.IsActive == true
                        select new FrayteLogisticServiceItem
                        {
                            OperationZoneId = ls.OperationZoneId,
                            LogisticServiceId = ls.LogisticServiceId,
                            LogisticCompany = ls.LogisticCompany,
                            LogisticCompanyDisplay = ls.LogisticCompanyDisplay,
                            LogisticType = ls.LogisticType,
                            LogisticTypeDisplay = ls.LogisticTypeDisplay,
                            RateType = ls.RateType,
                            RateTypeDisplay = ls.RateTypeDisplay,
                            ModuleType = ls.ModuleType
                        }).FirstOrDefault();

            return list;
        }

        public List<FrayteZoneBaseRateCard> ExportBaseRateExcel(int OperationZoneId, string LogisticType, string CourierCompany, string RateType, string ModuleType)
        {
            List<FrayteZoneBaseRateCard> baserate = new List<FrayteZoneBaseRateCard>();

            var BaseRateCard = (from lsbrc in dbContext.LogisticServiceBaseRateCards
                                join lsst in dbContext.LogisticServiceShipmentTypes on lsbrc.LogisticServiceShipmentTypeId equals lsst.LogisticServiceShipmentTypeId
                                join lsw in dbContext.LogisticServiceWeights on lsbrc.LogisticServiceWeightId equals lsw.LogisticServiceWeightId
                                join lsz in dbContext.LogisticServiceZones on lsbrc.LogisticServiceZoneId equals lsz.LogisticServiceZoneId
                                join ls in dbContext.LogisticServices on lsst.LogisticServiceId equals ls.LogisticServiceId
                                where
                                      ls.LogisticType == LogisticType &&
                                      ls.LogisticCompany == CourierCompany &&
                                      ls.ModuleType == ModuleType &&
                                      ls.OperationZoneId == OperationZoneId &&
                                      ls.RateType == RateType &&
                                      lsst.LogisticType == LogisticType

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
                                        LogisticDescriptionDisplay = lsst.LogisticDescriptionDisplayType,
                                        ReportLogisticDisplay = lsst.LogisticDescriptionReportDisplayType
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
                                            ReportLogisticDisplay = lsst.LogisticDescriptionReportDisplayType
                                        },
                                    },
                                    Rate = lsbrc.LogisticRate,
                                    CurrencyCode = lsbrc.LogisticCurrency,
                                    LogisticType = ls.LogisticType,
                                    ModuleType = lsbrc.ModuleType

                                }).ToList();


            return BaseRateCard;
        }

        public List<FrayteZoneBaseRateCard> ExportUKBaseRateExcel(int OperationZoneId, string LogisticType, string CourierCompany, string ModuleType)
        {
            List<FrayteZoneBaseRateCard> _baserate = new List<FrayteZoneBaseRateCard>();

            try
            {
                if (CourierCompany == FrayteLogisticServiceType.Yodel)
                {
                    #region Yodel Base Rate

                    var BaseRateCard = (from lsbrc in dbContext.LogisticServiceBaseRateCards
                                        join lsst in dbContext.LogisticServiceShipmentTypes on lsbrc.LogisticServiceShipmentTypeId equals lsst.LogisticServiceShipmentTypeId
                                        join lsw in dbContext.LogisticServiceWeights on lsbrc.LogisticServiceWeightId equals lsw.LogisticServiceWeightId
                                        join lsz in dbContext.LogisticServiceZones on lsbrc.LogisticServiceZoneId equals lsz.LogisticServiceZoneId
                                        join ls in dbContext.LogisticServices on lsz.LogisticServiceId equals ls.LogisticServiceId

                                        where
                                            ls.LogisticType == LogisticType &&
                                            ls.LogisticCompany == CourierCompany &&
                                            ls.OperationZoneId == OperationZoneId &&
                                            ls.ModuleType == ModuleType

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
                                                LogisticDescriptionDisplay = lsst.LogisticDescriptionDisplayType
                                            },
                                            LogisticWeight = new FrayteLogisticWeight
                                            {
                                                LogisticWeightId = lsw.LogisticServiceWeightId,
                                                WeightFrom = lsw.WeightFromDisplay,
                                                WeightTo = lsw.WeightToDisplay,
                                                UnitOfMeasurement = lsw.UOM,
                                                WeightUnit = lsw.WeightUnit,
                                                PackageType = lsw.PackageType,
                                                ParcelType = lsw.ParcelType,
                                                PackageDisplayType = lsw.PackageDisplayType,
                                                ParcelDisplayType = lsw.ParcelDisplayType
                                            },
                                            Rate = lsbrc.LogisticRate,
                                            CurrencyCode = lsbrc.LogisticCurrency,
                                            LogisticType = ls.LogisticType,
                                            LogisticServicetype = ls.LogisticCompany,
                                            BusinessRate = (float)lsbrc.LogisticRate,
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
                                        join lsw in dbContext.LogisticServiceWeights on lsbrc.LogisticServiceWeightId equals lsw.LogisticServiceWeightId
                                        join lsz in dbContext.LogisticServiceZones on lsbrc.LogisticServiceZoneId equals lsz.LogisticServiceZoneId
                                        join ls in dbContext.LogisticServices on lsz.LogisticServiceId equals ls.LogisticServiceId

                                        where
                                             ls.LogisticType == LogisticType &&
                                             ls.LogisticCompany == CourierCompany &&
                                             ls.OperationZoneId == OperationZoneId &&
                                             ls.ModuleType == ModuleType

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
                                                LogisticDescriptionDisplay = lsst.LogisticDescriptionDisplayType
                                            },
                                            LogisticWeight = new FrayteLogisticWeight
                                            {
                                                LogisticWeightId = lsw.LogisticServiceWeightId,
                                                WeightFrom = lsw.WeightFromDisplay,
                                                WeightTo = lsw.WeightToDisplay,
                                                UnitOfMeasurement = lsw.UOM,
                                                WeightUnit = lsw.WeightUnit,
                                                PackageType = lsw.PackageType,
                                                ParcelType = lsw.ParcelType
                                            },
                                            Rate = lsbrc.LogisticRate,
                                            CurrencyCode = lsbrc.LogisticCurrency,
                                            LogisticType = ls.LogisticType,
                                            LogisticServicetype = ls.LogisticCompany,
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
                                        join lsw in dbContext.LogisticServiceWeights on lsbrc.LogisticServiceWeightId equals lsw.LogisticServiceWeightId
                                        join ls in dbContext.LogisticServices on lsz.LogisticServiceId equals ls.LogisticServiceId
                                        where
                                             ls.LogisticType == LogisticType &&
                                             ls.LogisticCompany == CourierCompany &&
                                             ls.OperationZoneId == OperationZoneId
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
                                                ParcelType = lsw.ParcelType
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
                                        join lsw in dbContext.LogisticServiceWeights on lsbrc.LogisticServiceWeightId equals lsw.LogisticServiceWeightId
                                        join lsz in dbContext.LogisticServiceZones on lsbrc.LogisticServiceZoneId equals lsz.LogisticServiceZoneId
                                        join ls in dbContext.LogisticServices on lsz.LogisticServiceId equals ls.LogisticServiceId
                                        where
                                            ls.LogisticType == LogisticType &&
                                            ls.LogisticCompany == CourierCompany &&
                                            ls.OperationZoneId == OperationZoneId &&
                                            ls.ModuleType == ModuleType

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
                                            LogisticWeight = new FrayteLogisticWeight
                                            {
                                                LogisticWeightId = lsw.LogisticServiceWeightId,
                                                WeightFrom = lsw.WeightFromDisplay,
                                                WeightTo = lsw.WeightToDisplay,
                                                UnitOfMeasurement = lsw.UOM,
                                                WeightUnit = lsw.WeightUnit,
                                                PackageType = lsw.PackageType,
                                                ParcelType = lsw.ParcelType
                                            },
                                            Rate = lsbrc.LogisticRate,
                                            CurrencyCode = lsbrc.LogisticCurrency,
                                            LogisticType = ls.LogisticType,
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

        public FrayteManifestName SaveLogisticServiceBaseRateHistory(int LogisticServiceId, int Year, string FileName)
        {
            FrayteManifestName name = new FrayteManifestName();

            try
            {
                LogisticServiceBaseRateCardHistory history = new LogisticServiceBaseRateCardHistory();
                history.LogisticServiceId = LogisticServiceId;
                history.ReportYear = Year;
                history.ReportFileName = FileName;
                dbContext.LogisticServiceBaseRateCardHistories.Add(history);
                dbContext.SaveChanges();

                //Retun Filename and Path
                name.FileName = FileName;
                name.FilePath = AppSettings.WebApiPath + "ReportFiles/BaseRateCard/" + FileName;
            }
            catch (DbEntityValidationException dbEx)
            {
                string validationErrorMsg = "";
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        validationErrorMsg = validationErrorMsg + string.Format("Property: {0} Error: {1}",
                        validationError.PropertyName,
                        validationError.ErrorMessage);
                    }
                }
            }
            return name;
        }
    }
}