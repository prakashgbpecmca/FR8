using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frayte.Services.DataAccess;
using Frayte.Services.Models;

namespace Frayte.Services.Business
{
    public class ZoneRepository
    {
        FrayteEntities dbContext = new FrayteEntities();
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

        public List<FrayteZone> GetZoneDetail(int OperationZoneId, string CourierCompany, string ModuleType)
        {
            var list = (from ls in dbContext.LogisticServices
                        join lsz in dbContext.LogisticServiceZones on ls.LogisticServiceId equals lsz.LogisticServiceId
                        where ls.OperationZoneId == OperationZoneId &&
                              ls.LogisticCompany == CourierCompany &&
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

        public List<FrayteZone> GetZoneList()
        {
            FrayteOperationZone OperationZone = UtilityRepository.GetOperationZone();
            //var list = dbContext.Zones.Where(p => p.OperationZoneId == OperationZone.OperationZoneId).ToList();
            var list = (from ls in dbContext.LogisticServices
                        join lsz in dbContext.LogisticServiceZones on ls.LogisticServiceId equals lsz.LogisticServiceId
                        where ls.OperationZoneId == OperationZone.OperationZoneId
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

        public List<FrayteZone> GetZoneList(int OperationZoneId)
        {
            //var list = dbContext.Zones.Where(p => p.OperationZoneId == OperationZoneId).ToList();
            var list = (from ls in dbContext.LogisticServices
                        join lsz in dbContext.LogisticServiceZones on ls.LogisticServiceId equals lsz.LogisticServiceId
                        where ls.OperationZoneId == OperationZoneId
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
    }
}
