using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frayte.Services.Utility;
using System.Data.Entity.Validation;

namespace Frayte.Services.Business
{
    public class CourierAccountRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        public void AddCourierAccount(FrayteCourierAccount fcAccount)
        {
            try
            {
                LogisticServiceCourierAccount courieraccount;
                if (fcAccount != null)
                {
                    int LogisticServiceId = 0;
                    if (fcAccount.LogisticService.LogisticType == FrayteLogisticType.Import
                        || fcAccount.LogisticService.LogisticType == FrayteLogisticType.Export
                        || fcAccount.LogisticService.LogisticType == FrayteLogisticType.ThirdParty
                        || fcAccount.LogisticService.LogisticType == FrayteLogisticType.EUImport
                        || fcAccount.LogisticService.LogisticType == FrayteLogisticType.EUExport)
                    {
                        var logistic = dbContext.LogisticServices.Where(p => p.OperationZoneId == fcAccount.LogisticService.OperationZoneId &&
                                                                                  p.LogisticType == fcAccount.LogisticService.LogisticType &&
                                                                                  p.LogisticCompany == fcAccount.LogisticService.LogisticCompany &&
                                                                                  p.RateType == fcAccount.LogisticService.RateType &&
                                                                                  p.ModuleType == fcAccount.LogisticService.ModuleType).FirstOrDefault();
                        if (logistic != null)
                        {
                            LogisticServiceId = logistic.LogisticServiceId;
                        }
                    }
                    else if (fcAccount.LogisticService.LogisticType == FrayteLogisticType.UKShipment)
                    {
                        var logistic = dbContext.LogisticServices.Where(p => p.OperationZoneId == fcAccount.LogisticService.OperationZoneId &&
                                                                                  p.LogisticType == fcAccount.LogisticService.LogisticType &&
                                                                                  p.LogisticCompany == fcAccount.LogisticService.LogisticCompany &&
                                                                                  p.ModuleType == fcAccount.LogisticService.ModuleType).FirstOrDefault();
                        if (logistic != null)
                        {
                            LogisticServiceId = logistic.LogisticServiceId;
                        }
                    }

                    if (fcAccount.LogisticServiceCourierAccountId > 0)
                    {
                        courieraccount = dbContext.LogisticServiceCourierAccounts.Where(x => x.LogisticServiceCourierAccountId == fcAccount.LogisticServiceCourierAccountId).FirstOrDefault();
                        if (courieraccount != null)
                        {
                            courieraccount.LogisticServiceCourierAccountId = fcAccount.LogisticServiceCourierAccountId;
                            courieraccount.LogisticServiceId = LogisticServiceId;
                            courieraccount.IntegrationAccountId = fcAccount.IntegrationAccountId;
                            courieraccount.AccountNo = fcAccount.AccountNo;
                            courieraccount.AccountCountryCode = fcAccount.AccountCountryCode;
                            courieraccount.Description = fcAccount.Description;
                            courieraccount.ColorCode = fcAccount.ColorCode;
                            courieraccount.IsActive = true;
                            dbContext.Entry(courieraccount).State = System.Data.Entity.EntityState.Modified;
                        }
                    }
                    else
                    {
                        courieraccount = new LogisticServiceCourierAccount();
                        courieraccount.LogisticServiceCourierAccountId = fcAccount.LogisticServiceCourierAccountId;
                        courieraccount.LogisticServiceId = LogisticServiceId;
                        courieraccount.IntegrationAccountId = fcAccount.IntegrationAccountId;
                        courieraccount.AccountNo = fcAccount.AccountNo;
                        courieraccount.AccountCountryCode = fcAccount.AccountCountryCode;
                        courieraccount.Description = fcAccount.Description;
                        courieraccount.ColorCode = fcAccount.ColorCode;
                        courieraccount.IsActive = true;
                        dbContext.LogisticServiceCourierAccounts.Add(courieraccount);
                    }
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }
        }

        public List<FrayteCourierAccount> GetCourierAccounts(int OperationZoneId, string LogisticType, string CourierCompany, string RateType, string ModuleType)
        {
            List<FrayteCourierAccount> FrayteCourierAccounts = new List<FrayteCourierAccount>();
            try
            {
                FrayteCourierAccount fca;
                if (LogisticType == FrayteLogisticType.Import)
                {
                    var list = (from ls in dbContext.LogisticServices
                                join lsca in dbContext.LogisticServiceCourierAccounts on ls.LogisticServiceId equals lsca.LogisticServiceId
                                join oz in dbContext.OperationZones on ls.OperationZoneId equals oz.OperationZoneId
                                where ls.OperationZoneId == OperationZoneId &&
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

                    if (list != null && list.Count > 0)
                    {
                        foreach (var rr in list)
                        {
                            fca = new FrayteCourierAccount();
                            fca.LogisticServiceCourierAccountId = rr.LogisticServiceCourierAccountId;
                            fca.OperationZone = new FrayteOperationZone()
                            {
                                OperationZoneId = rr.OperationZoneId,
                                OperationZoneName = rr.Name
                            };
                            fca.LogisticService = new LogisticShipmentService()
                            {
                                LogisticServiceId = 0,
                                OperationZoneId = rr.OperationZoneId,
                                LogisticCompany = rr.LogisticCompany,
                                LogisticCompanyDisplay = rr.LogisticCompanyDisplay,
                                LogisticType = rr.LogisticType,
                                LogisticTypeDisplay = rr.LogisticTypeDisplay,
                                RateType = rr.RateType,
                                RateTypeDisplay = rr.RateTypeDisplay,
                                ModuleType = rr.ModuleType,
                            };
                            fca.AccountNo = rr.AccountNo;
                            fca.AccountCountryCode = rr.AccountCountryCode;
                            fca.Description = rr.Description;
                            fca.IntegrationAccountId = rr.IntegrationAccountId;
                            fca.ColorCode = rr.ColorCode;
                            fca.IsActive = false;
                            FrayteCourierAccounts.Add(fca);
                        }
                    }
                }
                else if (LogisticType == FrayteLogisticType.Export)
                {
                    var list = (from ls in dbContext.LogisticServices
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

                    if (list != null && list.Count > 0)
                    {
                        foreach (var rr in list)
                        {
                            fca = new FrayteCourierAccount();
                            fca.LogisticServiceCourierAccountId = rr.LogisticServiceCourierAccountId;
                            fca.OperationZone = new FrayteOperationZone()
                            {
                                OperationZoneId = rr.OperationZoneId,
                                OperationZoneName = rr.Name
                            };
                            fca.LogisticService = new LogisticShipmentService()
                            {
                                LogisticServiceId = 0,
                                OperationZoneId = rr.OperationZoneId,
                                LogisticCompany = rr.LogisticCompany,
                                LogisticCompanyDisplay = rr.LogisticCompanyDisplay,
                                LogisticType = rr.LogisticType,
                                LogisticTypeDisplay = rr.LogisticTypeDisplay,
                                RateType = rr.RateType,
                                RateTypeDisplay = rr.RateTypeDisplay,
                                ModuleType = rr.ModuleType,
                            };
                            fca.IntegrationAccountId = rr.IntegrationAccountId;
                            fca.AccountNo = rr.AccountNo;
                            fca.AccountCountryCode = rr.AccountCountryCode;
                            fca.Description = rr.Description;
                            fca.ColorCode = rr.ColorCode;
                            fca.IsActive = rr.IsActive;
                            FrayteCourierAccounts.Add(fca);
                        }
                    }
                }
                else if (LogisticType == FrayteLogisticType.ThirdParty)
                {
                    var list = (from ls in dbContext.LogisticServices
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

                    if (list != null && list.Count > 0)
                    {
                        foreach (var rr in list)
                        {
                            fca = new FrayteCourierAccount();
                            fca.LogisticServiceCourierAccountId = rr.LogisticServiceCourierAccountId;
                            fca.OperationZone = new FrayteOperationZone()
                            {
                                OperationZoneId = rr.OperationZoneId,
                                OperationZoneName = rr.Name
                            };
                            fca.LogisticService = new LogisticShipmentService()
                            {
                                LogisticServiceId = 0,
                                OperationZoneId = rr.OperationZoneId,
                                LogisticCompany = rr.LogisticCompany,
                                LogisticCompanyDisplay = rr.LogisticCompanyDisplay,
                                LogisticType = rr.LogisticType,
                                LogisticTypeDisplay = rr.LogisticTypeDisplay,
                                RateType = rr.RateType,
                                RateTypeDisplay = rr.RateTypeDisplay,
                                ModuleType = rr.ModuleType,
                            };
                            fca.IntegrationAccountId = rr.IntegrationAccountId;
                            fca.AccountNo = rr.AccountNo;
                            fca.AccountCountryCode = rr.AccountCountryCode;
                            fca.Description = rr.Description;
                            fca.ColorCode = rr.ColorCode;
                            fca.IsActive = rr.IsActive;
                            FrayteCourierAccounts.Add(fca);
                        }
                    }
                }
                else if (LogisticType == FrayteLogisticType.UKShipment)
                {
                    var list = (from ls in dbContext.LogisticServices
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

                    if (list != null && list.Count > 0)
                    {
                        foreach (var rr in list)
                        {
                            fca = new FrayteCourierAccount();
                            fca.LogisticServiceCourierAccountId = rr.LogisticServiceCourierAccountId;
                            fca.OperationZone = new FrayteOperationZone()
                            {
                                OperationZoneId = rr.OperationZoneId,
                                OperationZoneName = rr.Name
                            };
                            fca.LogisticService = new LogisticShipmentService()
                            {
                                LogisticServiceId = 0,
                                OperationZoneId = rr.OperationZoneId,
                                LogisticCompany = rr.LogisticCompany,
                                LogisticCompanyDisplay = rr.LogisticCompanyDisplay,
                                LogisticType = rr.LogisticType,
                                LogisticTypeDisplay = rr.LogisticTypeDisplay,
                                RateType = rr.RateType,
                                RateTypeDisplay = rr.RateTypeDisplay,
                                ModuleType = rr.ModuleType,
                            };
                            fca.IntegrationAccountId = rr.IntegrationAccountId;
                            fca.AccountNo = rr.AccountNo;
                            fca.AccountCountryCode = rr.AccountCountryCode;
                            fca.Description = rr.Description;
                            fca.ColorCode = rr.ColorCode;
                            fca.IsActive = rr.IsActive;
                            FrayteCourierAccounts.Add(fca);
                        }
                    }
                }
                else if (LogisticType == FrayteLogisticType.AUShipment)
                {
                    var list = (from ls in dbContext.LogisticServices
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

                    if (list != null && list.Count > 0)
                    {
                        foreach (var rr in list)
                        {
                            fca = new FrayteCourierAccount();
                            fca.LogisticServiceCourierAccountId = rr.LogisticServiceCourierAccountId;
                            fca.OperationZone = new FrayteOperationZone()
                            {
                                OperationZoneId = rr.OperationZoneId,
                                OperationZoneName = rr.Name
                            };
                            fca.LogisticService = new LogisticShipmentService()
                            {
                                LogisticServiceId = 0,
                                OperationZoneId = rr.OperationZoneId,
                                LogisticCompany = rr.LogisticCompany,
                                LogisticCompanyDisplay = rr.LogisticCompanyDisplay,
                                LogisticType = rr.LogisticType,
                                LogisticTypeDisplay = rr.LogisticTypeDisplay,
                                RateType = rr.RateType,
                                RateTypeDisplay = rr.RateTypeDisplay,
                                ModuleType = rr.ModuleType,
                            };
                            fca.IntegrationAccountId = rr.IntegrationAccountId;
                            fca.AccountNo = rr.AccountNo;
                            fca.AccountCountryCode = rr.AccountCountryCode;
                            fca.Description = rr.Description;
                            fca.ColorCode = rr.ColorCode;
                            fca.IsActive = rr.IsActive;
                            FrayteCourierAccounts.Add(fca);
                        }
                    }
                }
                else if (LogisticType == FrayteLogisticType.SkyPostalBrazilShipment)
                {
                    var list = (from ls in dbContext.LogisticServices
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

                    if (list != null && list.Count > 0)
                    {
                        foreach (var rr in list)
                        {
                            fca = new FrayteCourierAccount();
                            fca.LogisticServiceCourierAccountId = rr.LogisticServiceCourierAccountId;
                            fca.OperationZone = new FrayteOperationZone()
                            {
                                OperationZoneId = rr.OperationZoneId,
                                OperationZoneName = rr.Name
                            };
                            fca.LogisticService = new LogisticShipmentService()
                            {
                                LogisticServiceId = 0,
                                OperationZoneId = rr.OperationZoneId,
                                LogisticCompany = rr.LogisticCompany,
                                LogisticCompanyDisplay = rr.LogisticCompanyDisplay,
                                LogisticType = rr.LogisticType,
                                LogisticTypeDisplay = rr.LogisticTypeDisplay,
                                RateType = rr.RateType,
                                RateTypeDisplay = rr.RateTypeDisplay,
                                ModuleType = rr.ModuleType,
                            };
                            fca.IntegrationAccountId = rr.IntegrationAccountId;
                            fca.AccountNo = rr.AccountNo;
                            fca.AccountCountryCode = rr.AccountCountryCode;
                            fca.Description = rr.Description;
                            fca.ColorCode = rr.ColorCode;
                            fca.IsActive = rr.IsActive;
                            FrayteCourierAccounts.Add(fca);
                        }
                    }
                }
                else if (LogisticType == FrayteLogisticType.SkyPostalMexicoShipment)
                {
                    var list = (from ls in dbContext.LogisticServices
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

                    if (list != null && list.Count > 0)
                    {
                        foreach (var rr in list)
                        {
                            fca = new FrayteCourierAccount();
                            fca.LogisticServiceCourierAccountId = rr.LogisticServiceCourierAccountId;
                            fca.OperationZone = new FrayteOperationZone()
                            {
                                OperationZoneId = rr.OperationZoneId,
                                OperationZoneName = rr.Name
                            };
                            fca.LogisticService = new LogisticShipmentService()
                            {
                                LogisticServiceId = 0,
                                OperationZoneId = rr.OperationZoneId,
                                LogisticCompany = rr.LogisticCompany,
                                LogisticCompanyDisplay = rr.LogisticCompanyDisplay,
                                LogisticType = rr.LogisticType,
                                LogisticTypeDisplay = rr.LogisticTypeDisplay,
                                RateType = rr.RateType,
                                RateTypeDisplay = rr.RateTypeDisplay,
                                ModuleType = rr.ModuleType,
                            };
                            fca.IntegrationAccountId = rr.IntegrationAccountId;
                            fca.AccountNo = rr.AccountNo;
                            fca.AccountCountryCode = rr.AccountCountryCode;
                            fca.Description = rr.Description;
                            fca.ColorCode = rr.ColorCode;
                            fca.IsActive = rr.IsActive;
                            FrayteCourierAccounts.Add(fca);
                        }
                    }
                }
                else if (LogisticType == FrayteLogisticType.SkyPostalChileShipment)
                {
                    var list = (from ls in dbContext.LogisticServices
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

                    if (list != null && list.Count > 0)
                    {
                        foreach (var rr in list)
                        {
                            fca = new FrayteCourierAccount();
                            fca.LogisticServiceCourierAccountId = rr.LogisticServiceCourierAccountId;
                            fca.OperationZone = new FrayteOperationZone()
                            {
                                OperationZoneId = rr.OperationZoneId,
                                OperationZoneName = rr.Name
                            };
                            fca.LogisticService = new LogisticShipmentService()
                            {
                                LogisticServiceId = 0,
                                OperationZoneId = rr.OperationZoneId,
                                LogisticCompany = rr.LogisticCompany,
                                LogisticCompanyDisplay = rr.LogisticCompanyDisplay,
                                LogisticType = rr.LogisticType,
                                LogisticTypeDisplay = rr.LogisticTypeDisplay,
                                RateType = rr.RateType,
                                RateTypeDisplay = rr.RateTypeDisplay,
                                ModuleType = rr.ModuleType,
                            };
                            fca.IntegrationAccountId = rr.IntegrationAccountId;
                            fca.AccountNo = rr.AccountNo;
                            fca.AccountCountryCode = rr.AccountCountryCode;
                            fca.Description = rr.Description;
                            fca.ColorCode = rr.ColorCode;
                            fca.IsActive = rr.IsActive;
                            FrayteCourierAccounts.Add(fca);
                        }
                    }
                }
                else if (LogisticType == FrayteLogisticType.EAMShipment)
                {
                    var list = (from ls in dbContext.LogisticServices
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

                    if (list != null && list.Count > 0)
                    {
                        foreach (var rr in list)
                        {
                            fca = new FrayteCourierAccount();
                            fca.LogisticServiceCourierAccountId = rr.LogisticServiceCourierAccountId;
                            fca.OperationZone = new FrayteOperationZone()
                            {
                                OperationZoneId = rr.OperationZoneId,
                                OperationZoneName = rr.Name
                            };
                            fca.LogisticService = new LogisticShipmentService()
                            {
                                LogisticServiceId = 0,
                                OperationZoneId = rr.OperationZoneId,
                                LogisticCompany = rr.LogisticCompany,
                                LogisticCompanyDisplay = rr.LogisticCompanyDisplay,
                                LogisticType = rr.LogisticType,
                                LogisticTypeDisplay = rr.LogisticTypeDisplay,
                                RateType = rr.RateType,
                                RateTypeDisplay = rr.RateTypeDisplay,
                                ModuleType = rr.ModuleType,
                            };
                            fca.IntegrationAccountId = rr.IntegrationAccountId;
                            fca.AccountNo = rr.AccountNo;
                            fca.AccountCountryCode = rr.AccountCountryCode;
                            fca.Description = rr.Description;
                            fca.ColorCode = rr.ColorCode;
                            fca.IsActive = rr.IsActive;
                            FrayteCourierAccounts.Add(fca);
                        }
                    }
                }
                else if (LogisticType == FrayteLogisticType.EUExport)
                {
                    var list = (from ls in dbContext.LogisticServices
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

                    if (list != null && list.Count > 0)
                    {
                        foreach (var rr in list)
                        {
                            fca = new FrayteCourierAccount();
                            fca.LogisticServiceCourierAccountId = rr.LogisticServiceCourierAccountId;
                            fca.OperationZone = new FrayteOperationZone()
                            {
                                OperationZoneId = rr.OperationZoneId,
                                OperationZoneName = rr.Name
                            };
                            fca.LogisticService = new LogisticShipmentService()
                            {
                                LogisticServiceId = 0,
                                OperationZoneId = rr.OperationZoneId,
                                LogisticCompany = rr.LogisticCompany,
                                LogisticCompanyDisplay = rr.LogisticCompanyDisplay,
                                LogisticType = rr.LogisticType,
                                LogisticTypeDisplay = rr.LogisticTypeDisplay,
                                RateType = rr.RateType,
                                RateTypeDisplay = rr.RateTypeDisplay,
                                ModuleType = rr.ModuleType,
                            };
                            fca.IntegrationAccountId = rr.IntegrationAccountId;
                            fca.AccountNo = rr.AccountNo;
                            fca.AccountCountryCode = rr.AccountCountryCode;
                            fca.Description = rr.Description;
                            fca.ColorCode = rr.ColorCode;
                            fca.IsActive = rr.IsActive;
                            FrayteCourierAccounts.Add(fca);
                        }
                    }
                }
                else if (LogisticType == FrayteLogisticType.EUImport)
                {
                    var list = (from ls in dbContext.LogisticServices
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

                    if (list != null && list.Count > 0)
                    {
                        foreach (var rr in list)
                        {
                            fca = new FrayteCourierAccount();
                            fca.LogisticServiceCourierAccountId = rr.LogisticServiceCourierAccountId;
                            fca.OperationZone = new FrayteOperationZone()
                            {
                                OperationZoneId = rr.OperationZoneId,
                                OperationZoneName = rr.Name
                            };
                            fca.LogisticService = new LogisticShipmentService()
                            {
                                LogisticServiceId = 0,
                                OperationZoneId = rr.OperationZoneId,
                                LogisticCompany = rr.LogisticCompany,
                                LogisticCompanyDisplay = rr.LogisticCompanyDisplay,
                                LogisticType = rr.LogisticType,
                                LogisticTypeDisplay = rr.LogisticTypeDisplay,
                                RateType = rr.RateType,
                                RateTypeDisplay = rr.RateTypeDisplay,
                                ModuleType = rr.ModuleType,
                            };
                            fca.IntegrationAccountId = rr.IntegrationAccountId;
                            fca.AccountNo = rr.AccountNo;
                            fca.AccountCountryCode = rr.AccountCountryCode;
                            fca.Description = rr.Description;
                            fca.ColorCode = rr.ColorCode;
                            fca.IsActive = rr.IsActive;
                            FrayteCourierAccounts.Add(fca);
                        }
                    }
                }
                return FrayteCourierAccounts;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<FrayteShipmentCourier> GetShipmentCourierList()
        {
            List<FrayteShipmentCourier> lstCouriers = new List<FrayteShipmentCourier>();

            var result = dbContext.Couriers.ToList();

            foreach (Courier courier in result)
            {
                if (courier.ShipmentType == "Air")
                {
                    FrayteShipmentCourier frayteCourier = new FrayteShipmentCourier();
                    frayteCourier.CourierId = courier.CourierId;
                    frayteCourier.Name = courier.CourierName;
                    frayteCourier.Website = courier.Website;
                    frayteCourier.CourierType = courier.ShipmentType;
                    frayteCourier.LatestBookingTime = UtilityRepository.GetWorkingTime(courier.LatestBookingTime).Value;

                    lstCouriers.Add(frayteCourier);
                }

            }
            foreach (Courier courier in result)
            {
                if (courier.ShipmentType == "Sea")
                {
                    FrayteShipmentCourier frayteCourier = new FrayteShipmentCourier();
                    frayteCourier.CourierId = courier.CourierId;
                    frayteCourier.Name = courier.CourierName;
                    frayteCourier.Website = courier.Website;
                    frayteCourier.CourierType = courier.ShipmentType;
                    frayteCourier.LatestBookingTime = UtilityRepository.GetWorkingTime(courier.LatestBookingTime).Value;

                    lstCouriers.Add(frayteCourier);
                }

            }
            foreach (Courier courier in result)
            {
                if (courier.ShipmentType == "Courier")
                {
                    FrayteShipmentCourier frayteCourier = new FrayteShipmentCourier();
                    frayteCourier.CourierId = courier.CourierId;
                    frayteCourier.Name = courier.CourierName;
                    frayteCourier.Website = courier.Website;
                    frayteCourier.CourierType = courier.ShipmentType;
                    frayteCourier.DisplayName = courier.DisplayName != null ? courier.DisplayName : FrayteLogisticServiceDisplayType.UKMail;
                    frayteCourier.LatestBookingTime = UtilityRepository.GetWorkingTime(courier.LatestBookingTime).Value;

                    lstCouriers.Add(frayteCourier);
                }

            }
            foreach (Courier courier in result)
            {
                if (courier.ShipmentType == "Expryes")
                {
                    FrayteShipmentCourier frayteCourier = new FrayteShipmentCourier();
                    frayteCourier.CourierId = courier.CourierId;
                    frayteCourier.Name = courier.CourierName;
                    frayteCourier.Website = courier.Website;
                    frayteCourier.CourierType = courier.ShipmentType;
                    frayteCourier.LatestBookingTime = UtilityRepository.GetWorkingTime(courier.LatestBookingTime).Value;

                    lstCouriers.Add(frayteCourier);
                }

            }
            return lstCouriers;
        }

        public List<FrayteOperationZone> GetOperationZone()
        {
            List<FrayteOperationZone> _operation = new List<FrayteOperationZone>();
            try
            {
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
            catch (Exception ex)
            {
                return null;
            }
            return _operation;
        }

        public FrayteResult DeleteCourierAccount(int CourierAccountId)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                var courieraccount = dbContext.LogisticServiceCourierAccounts.Where(x => x.LogisticServiceCourierAccountId == CourierAccountId).FirstOrDefault();
                if (courieraccount != null && courieraccount.LogisticServiceCourierAccountId > 0)
                {
                    courieraccount.IsActive = false;
                    dbContext.Entry(courieraccount).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
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

        public List<FrayteCourierAccount> GetCourierAccounts()
        {
            FrayteCourierAccount fca;
            List<FrayteCourierAccount> FrayteCourierAccounts = new List<FrayteCourierAccount>();
            var list = dbContext.LogisticServiceCourierAccounts.Where(p => p.IsActive == true).ToList();
            foreach (var rr in list)
            {
                fca = new FrayteCourierAccount();
                fca.LogisticServiceCourierAccountId = rr.LogisticServiceCourierAccountId;
                LogisticService ls = dbContext.LogisticServices.Where(p => p.LogisticServiceId == rr.LogisticServiceId).FirstOrDefault();
                LogisticShipmentService lss = new LogisticShipmentService();
                fca.LogisticService = new LogisticShipmentService();
                if (ls != null)
                {
                    lss.LogisticServiceId = 0;
                    lss.OperationZoneId = ls.OperationZoneId;
                    lss.LogisticCompany = ls.LogisticCompany;
                    lss.LogisticCompanyDisplay = ls.LogisticCompanyDisplay;
                    lss.LogisticType = ls.LogisticType;
                    lss.LogisticTypeDisplay = ls.LogisticTypeDisplay;
                    lss.RateType = ls.RateType;
                    lss.RateTypeDisplay = ls.RateTypeDisplay;
                    lss.ModuleType = ls.ModuleType;
                };
                OperationZone oz = dbContext.OperationZones.Where(x => x.OperationZoneId == ls.OperationZoneId).FirstOrDefault();
                FrayteOperationZone fo = new FrayteOperationZone();
                fca.OperationZone = new FrayteOperationZone();
                if (oz != null)
                {
                    fo.OperationZoneId = oz.OperationZoneId > 0 ? oz.OperationZoneId : 0;
                    fo.OperationZoneName = oz.Name != null ? oz.Name : "";
                };
                fca.OperationZone = fo;
                fca.LogisticService = lss;
                fca.IntegrationAccountId = rr.IntegrationAccountId;
                fca.AccountNo = rr.AccountNo;
                fca.AccountCountryCode = rr.AccountCountryCode;
                fca.Description = rr.Description;
                fca.ColorCode = rr.ColorCode;
                FrayteCourierAccounts.Add(fca);
            }
            return FrayteCourierAccounts;
        }
    }
}
