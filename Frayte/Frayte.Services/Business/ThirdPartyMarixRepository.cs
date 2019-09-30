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
    public class ThirdPartyMarixRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        public void EditThirPartyMatrix(List<FrayteThirdPartyMatrix> _party)
        {
            try
            {
                LogisticServiceThirdPartyMatrix tpm;
                if (_party != null && _party.Count > 0)
                {
                    foreach (var pr in _party)
                    {
                        tpm = dbContext.LogisticServiceThirdPartyMatrices.Where(x => x.LogisticServiceThirdPartyMatrixId == pr.ThirdPartyMatrixId).FirstOrDefault();
                        if (tpm != null)
                        {
                            tpm.FromLogisticServiceZoneId = pr.FromZone.ZoneId;
                            tpm.ToLogisticServiceZoneId = pr.ToZone.ZoneId;
                            tpm.ApplyLogisticServiceZoneId = pr.ApplyZone.ZoneId;
                            tpm.OperationZoneId = pr.OperationZone.OperationZoneId;
                            dbContext.Entry(tpm).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public List<FrayteThirdPartyMatrix> GetThirdPartyMatrixDetail(int OperationZoneId, string LogisticType, string CourierCompany, string RateType, string ModuleType)
        {
            List<FrayteThirdPartyMatrix> _thirdparty = new List<FrayteThirdPartyMatrix>();
            try
            {
                OperationZone oz = dbContext.OperationZones.Where(x => x.OperationZoneId == OperationZoneId).FirstOrDefault();

                _thirdparty = (from tpm in dbContext.LogisticServiceThirdPartyMatrices
                               join fz in dbContext.LogisticServiceZones on tpm.FromLogisticServiceZoneId equals fz.LogisticServiceZoneId
                               join tz in dbContext.LogisticServiceZones on tpm.ToLogisticServiceZoneId equals tz.LogisticServiceZoneId
                               join az in dbContext.LogisticServiceZones on tpm.ApplyLogisticServiceZoneId equals az.LogisticServiceZoneId into tmpAZ
                               from azT in tmpAZ.DefaultIfEmpty()
                               join ls in dbContext.LogisticServices on tz.LogisticServiceId equals ls.LogisticServiceId
                               where tpm.OperationZoneId == OperationZoneId &&
                                     ls.OperationZoneId == OperationZoneId &&
                                     ls.LogisticCompany == CourierCompany &&
                                     ls.LogisticType == LogisticType &&
                                     ls.RateType == RateType &&
                                     ls.ModuleType == ModuleType
                               select new FrayteThirdPartyMatrix
                               {
                                   ApplyZone = new FrayteZone()
                                   {
                                       OperationZoneId = OperationZoneId,
                                       ZoneId = (azT.LogisticServiceId > 0 ? azT.LogisticServiceZoneId : 0),
                                       ZoneName = (azT.ZoneName == null ? "N/A" : azT.ZoneName),
                                       ZoneColor = (azT.ZoneColor == null ? "" : azT.ZoneColor),
                                       ZoneDisplayName = (azT.ZoneDisplayName == null ? "N/A" : azT.ZoneDisplayName),
                                       LogisticType = ls.LogisticType,
                                       CourierComapny = ls.LogisticCompany,
                                       RateType = ls.RateType,
                                       ModuleType = ls.ModuleType
                                   },
                                   FromZone = new FrayteZone()
                                   {
                                       OperationZoneId = OperationZoneId,
                                       ZoneId = fz.LogisticServiceZoneId,
                                       ZoneName = fz.ZoneName,
                                       ZoneColor = fz.ZoneColor,
                                       ZoneDisplayName = fz.ZoneDisplayName,
                                       LogisticType = ls.LogisticType,
                                       CourierComapny = ls.LogisticCompany,
                                       RateType = ls.RateType,
                                       ModuleType = ls.ModuleType
                                   },
                                   ToZone = new FrayteZone()
                                   {
                                       OperationZoneId = OperationZoneId,
                                       ZoneId = tz.LogisticServiceZoneId,
                                       ZoneName = tz.ZoneName,
                                       ZoneColor = tz.ZoneColor,
                                       ZoneDisplayName = tz.ZoneDisplayName,
                                       LogisticType = ls.LogisticType,
                                       CourierComapny = ls.LogisticCompany,
                                       RateType = ls.RateType,
                                       ModuleType = ls.ModuleType
                                   },
                                   OperationZone = new FrayteOperationZone()
                                   {
                                       OperationZoneId = OperationZoneId,
                                       OperationZoneName = oz.Name
                                   },
                                   ThirdPartyMatrixId = tpm.LogisticServiceThirdPartyMatrixId,
                               }).ToList();

            }
            catch (Exception ex)
            {
                return null;
            }
            return _thirdparty;
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
                            ZoneRateName = lsz.ReportZoneDisplay,
                            ZoneColor = lsz.ZoneColor,
                            LogisticType = ls.LogisticType,
                            CourierComapny = ls.LogisticCompany,
                            RateType = ls.RateType,
                            ModuleType = ls.ModuleType
                        }).ToList();

            return list;
        }

        public List<FrayteZone> GetZoneDetail(int LogisticServiceId)
        {
            var list = (from ls in dbContext.LogisticServices
                        join lsz in dbContext.LogisticServiceZones on ls.LogisticServiceId equals lsz.LogisticServiceId
                        where ls.LogisticServiceId == LogisticServiceId
                        select new FrayteZone
                        {
                            ZoneId = lsz.LogisticServiceZoneId,
                            OperationZoneId = ls.OperationZoneId,
                            ZoneName = lsz.ZoneName,
                            ZoneDisplayName = lsz.ZoneDisplayName,
                            ZoneRateName = "Rate " + lsz.ReportZoneDisplay,
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

        public List<FrayteRateThirdPartyMatrix> GetThirdPartyMatrix(int LogisticServiceId)
        {
            List<FrayteRateThirdPartyMatrix> _matrix = new List<FrayteRateThirdPartyMatrix>();

            FrayteRateThirdPartyMatrix third;
            var matrix = dbContext.Get_ThirdPartyMatrix(LogisticServiceId).ToList();
            if (matrix != null && matrix.Count > 0)
            {
                foreach (var Obj in matrix)
                {
                    third = new FrayteRateThirdPartyMatrix();
                    third.FromLogisticServiceZoneId = Obj.FromLogisticServiceZoneId;
                    third.FromZoneDisplay = Obj.FromZoneDisplayName;
                    third.ToLogisticServiceZoneId = Obj.ToLogisticServiceZoneId;
                    third.ToZoneDisplay = Obj.ToZoneDisplayName;
                    third.ApplyLogisticServiceZoneId = Obj.ApplyLogisticServiceZoneId;
                    third.ApplyZoneDisplay = Obj.ApplyZoneDisplayName;
                    _matrix.Add(third);
                }
            }

            return _matrix;
        }
    }
}
