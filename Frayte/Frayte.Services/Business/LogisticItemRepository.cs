using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Business
{
    public class LogisticItemRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        public LogisticItemList GetLogisticItem(int OperationZoneId)
        {
            var item = dbContext.LogisticServices.Where(p => p.OperationZoneId == OperationZoneId && p.IsActive == true).ToList();
            if (item != null && item.Count > 0)
            {
                LogisticItemList ls = new LogisticItemList();
                var company = item.Select(p => new { p.LogisticCompany, p.LogisticCompanyDisplay }).Distinct().OrderBy(p => p.LogisticCompany).ToList();
                var type = item.Select(p => new { p.LogisticType, p.LogisticTypeDisplay }).Distinct().OrderBy(p => p.LogisticType).ToList();
                var rate = item.Select(p => new { p.RateType, p.RateTypeDisplay }).Distinct().ToList();
                ls.ModuleType = item.Select(p => p.ModuleType).FirstOrDefault();

                ls.LogisticTypes = new List<LogisticServiceItem>();
                LogisticServiceItem lt;
                for (int i = 0; i < type.Count; i++)
                {
                    lt = new LogisticServiceItem();
                    lt.Value = type[i].LogisticType;
                    lt.Display = type[i].LogisticTypeDisplay;
                    ls.LogisticTypes.Add(lt);
                }

                ls.LogisticRateTypes = new List<LogisticServiceItem>();
                LogisticServiceItem lrt;
                for (int i = 0; i < rate.Count; i++)
                {
                    lrt = new LogisticServiceItem();
                    if (rate[i].RateType != null)
                    {
                        lrt.Value = rate[i].RateType;
                        lrt.Display = rate[i].RateTypeDisplay;
                        ls.LogisticRateTypes.Add(lrt);
                    }
                }

                ls.LogisticCompanies = new List<LogisticServiceItem>();
                LogisticServiceItem lc;
                for (int i = 0; i < company.Count; i++)
                {
                    lc = new LogisticServiceItem();
                    lc.Value = company[i].LogisticCompany;
                    lc.Display = company[i].LogisticCompanyDisplay;
                    ls.LogisticCompanies.Add(lc);
                }

                //Will be done Anil
                ls.DocTypes = new List<LogisticServiceItem>();
                ls.DocTypes.Add(new LogisticServiceItem() { Value = "Doc", Display = "Doc" });
                ls.DocTypes.Add(new LogisticServiceItem() { Value = "Nondoc", Display = "Nondoc" });
                ls.DocTypes.Add(new LogisticServiceItem() { Value = "Doc&Nondoc", Display = "Doc & Nondoc" });
                ls.DocTypes.Add(new LogisticServiceItem() { Value = "HeavyWeight", Display = "Heavy Weight" });

                return ls;
            }
            return null;
        }

        public List<FrayteRateCardLogisticServices> RateCardLogisticServices(int operationZoneId)
        {
            var OperationZone = UtilityRepository.GetOperationZone(); 
            var list = (from r in dbContext.LogisticServices
                        join ld in dbContext.LogisticServiceDetails on r.LogisticServiceId equals ld.LogisticServiceId into lsdTemp
                        from ldTemp in lsdTemp.DefaultIfEmpty()
                        join ls in dbContext.LogisticServiceShipmentTypes on r.LogisticServiceId equals ls.LogisticServiceId into lsTemp
                        from sTemp in lsTemp.DefaultIfEmpty()
                        where r.OperationZoneId == OperationZone.OperationZoneId && r.ModuleType == FrayteShipmentServiceType.DirectBooking && r.IsActive == true
                        select new FrayteRateCardLogisticServices
                        {
                            LogisticServiceId = r.LogisticServiceId,
                            DocType = sTemp == null? "": sTemp.LogisticDescription,
                            DocTypeDisplay = sTemp == null ? "" : sTemp.LogisticDescriptionDisplayType,
                            LogisticType = r.LogisticType,
                            LogisticTypeDisplay = r.LogisticTypeDisplay,
                            LogisticCompany = r.LogisticCompany,
                            LogisticCompanyDisplay = r.LogisticCompanyDisplay,
                            RateType = r.RateType,
                            RateTypeDisplay = r.RateTypeDisplay,
                            OperationZoneId = r.OperationZoneId,
                            AddressType = ldTemp == null? "": ldTemp.AddressType,
                            AddressTypeDisplay = ldTemp == null ? "" : ldTemp.AddressTypeDisplay,
                            PackageType = ldTemp == null ? "" : ldTemp.PackageType,
                            PackageTypeDisplay = ldTemp == null ? "" : ldTemp.PackageTypeDisplay,
                            ParcelType = ldTemp == null ? "" : ldTemp.ParcelType,
                            ParcelTypeDisplay = ldTemp == null ? "" : ldTemp.ParcelTypeDisplay,
                            PODType = ldTemp == null ? "" : ldTemp.PODType,
                            PODTypeDisplay = ldTemp == null ? "" : ldTemp.PODTypeDisplay,
                            ServiceType = ldTemp == null ? "" : ldTemp.ServiceType,
                            ServiceTypeDisplay = ldTemp == null ? "" : ldTemp.ServiceTypeDisplay,
                            ModuleType = r.ModuleType
                        }  ).ToList();

            return list;

        }

        public List<FrayteLogisticServices> LogisticServices(int operationZoneId)
        {
            List<FrayteLogisticServices> list = new List<FrayteLogisticServices>();
            var OperationZone = UtilityRepository.GetOperationZone();
            var collection = dbContext.LogisticServices.Where(p => p.OperationZoneId == OperationZone.OperationZoneId && p.IsActive == true).ToList();

            FrayteLogisticServices lt;

            foreach (var item in collection)
            {
                lt = new FrayteLogisticServices();
                lt.LogisticCompany = item.LogisticCompany;
                lt.LogisticCompanyDisplay = item.LogisticCompanyDisplay;
                lt.LogisticServiceId = item.LogisticServiceId;
                lt.LogisticType = item.LogisticType;
                lt.LogisticTypeDisplay = item.LogisticTypeDisplay;
                lt.RateType = item.RateType;
                lt.RateTypeDisplay = item.RateTypeDisplay;
                lt.ModuleType = item.ModuleType;
                lt.OperationZoneId = item.OperationZoneId;

                list.Add(lt);

            }

            return list;
        }

        public LogisticItemList UserLogisticItem(int UserId)
        {
            var item = (from ls in dbContext.LogisticServices
                        join cl in dbContext.CustomerLogistics on ls.LogisticServiceId equals cl.LogisticServiceId
                        join lsst in dbContext.LogisticServiceShipmentTypes on ls.LogisticServiceId equals lsst.LogisticServiceId
                        where cl.UserId == UserId
                        select new
                        {
                            ls.LogisticCompany,
                            ls.LogisticCompanyDisplay,
                            ls.LogisticType,
                            ls.LogisticTypeDisplay,
                            ls.RateType,
                            ls.RateTypeDisplay,
                            ls.ModuleType,
                            lsst.LogisticDescription,
                            lsst.LogisticDescriptionDisplayType
                        }).ToList();
            if (item != null && item.Count > 0)
            {
                LogisticItemList ls = new LogisticItemList();
                var company = item.Select(p => new { p.LogisticCompany, p.LogisticCompanyDisplay }).Distinct().OrderBy(p => p.LogisticCompany).ToList();
                var type = item.Select(p => new { p.LogisticType, p.LogisticTypeDisplay }).Distinct().OrderBy(p => p.LogisticType).ToList();
                var rate = item.Select(p => new { p.RateType, p.RateTypeDisplay }).Distinct().ToList();
                var shipment = item.Select(p => new { p.LogisticDescription, p.LogisticDescriptionDisplayType }).Distinct().ToList();
                ls.ModuleType = item.Select(p => p.ModuleType).FirstOrDefault();

                ls.LogisticTypes = new List<LogisticServiceItem>();
                LogisticServiceItem lt;
                for (int i = 0; i < type.Count; i++)
                {
                    lt = new LogisticServiceItem();
                    lt.Value = type[i].LogisticType;
                    lt.Display = type[i].LogisticTypeDisplay;
                    ls.LogisticTypes.Add(lt);
                }

                ls.LogisticRateTypes = new List<LogisticServiceItem>();
                LogisticServiceItem lrt;
                for (int i = 0; i < rate.Count; i++)
                {
                    lrt = new LogisticServiceItem();
                    if (rate[i].RateType != null)
                    {
                        lrt.Value = rate[i].RateType;
                        lrt.Display = rate[i].RateTypeDisplay;
                        ls.LogisticRateTypes.Add(lrt);
                    }
                }

                ls.LogisticCompanies = new List<LogisticServiceItem>();
                LogisticServiceItem lc;
                for (int i = 0; i < company.Count; i++)
                {
                    lc = new LogisticServiceItem();
                    lc.Value = company[i].LogisticCompany;
                    lc.Display = company[i].LogisticCompanyDisplay;
                    ls.LogisticCompanies.Add(lc);
                }

                ls.DocTypes = new List<LogisticServiceItem>();
                LogisticServiceItem lsst;
                for (int i = 0; i < shipment.Count; i++)
                {
                    lsst = new LogisticServiceItem();
                    lsst.Value = shipment[i].LogisticDescription;
                    lsst.Display = shipment[i].LogisticDescriptionDisplayType;
                    ls.DocTypes.Add(lsst);
                }

                return ls;
            }
            return null;
        }
    }
}
