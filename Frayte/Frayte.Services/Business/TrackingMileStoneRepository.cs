using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using Frayte.Services.Utility;

namespace Frayte.Services.Business
{
    public class TrackingMileStoneRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        public int SaveMileStone(TrackingMileStoneModel TMSM)
        {
            var result = dbContext.TrackingMileStones.Where(a => a.TrackingMileStoneId == TMSM.TrackingMileStoneId).FirstOrDefault();
            var ShipHandelCount = dbContext.TrackingMileStones.Where(a => a.ShipmentHandlerMethodId == TMSM.ShipmentHandlerMethodId).ToList();
            if (result == null)
            {
                TrackingMileStone TMS = new TrackingMileStone();
                TMS.MileStoneKey = TMSM.MileStoneKey;
                TMS.Description = TMSM.Description;
                TMS.OrderNumber = TMSM.OrderNumber;
                TMS.ShipmentHandlerMethodId = TMSM.ShipmentHandlerMethodId;
                TMS.CreatedBy = TMSM.CreatedBy;
                TMS.CreatedOnUtc = DateTime.UtcNow;
                TMS.UpdatedBy = TMSM.UpdatedBy;
                TMS.UpdatedOnUtc = DateTime.UtcNow;
                dbContext.TrackingMileStones.Add(TMS);
                dbContext.SaveChanges();
                SetOrderAddEdit(TMS);
                return TMS.TrackingMileStoneId;
            }
            else
            {
                result.MileStoneKey = TMSM.MileStoneKey;
                result.Description = TMSM.Description;
                result.OrderNumber = TMSM.OrderNumber;
                result.ShipmentHandlerMethodId = TMSM.ShipmentHandlerMethodId;
                result.UpdatedBy = TMSM.UpdatedBy;
                result.UpdatedOnUtc = DateTime.UtcNow;
                dbContext.Entry(result).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                SetOrderAddEdit(result);
                return result.TrackingMileStoneId;
            }

        }

        public bool CheckOrderNo(TrackingMileStoneModel order)
        {
            var result = dbContext.TrackingMileStones.Where(a => a.ShipmentHandlerMethodId == order.ShipmentHandlerMethodId).Select(s => s.OrderNumber).ToList();

            if (result != null && result.Count > 0 && order.TrackingMileStoneId == 0)
            {
                var maxval = result.Max() + 1;
                if (order.OrderNumber != 0 && order.OrderNumber <= maxval)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                var maxval = 0;
                if (result.Count > 0)
                {
                    maxval = result.Max();
                }
                else
                {
                    maxval = 1;
                }
                if (order.OrderNumber != 0 && order.OrderNumber <= maxval)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
           
        }

        public bool SetOrderAddEdit(TrackingMileStone order)
        {
            if (order != null)
            {
                var result2 = dbContext.TrackingMileStones.Where(a => a.TrackingMileStoneId != order.TrackingMileStoneId).ToList();
                var Res = result2.Where(a => a.ShipmentHandlerMethodId == order.ShipmentHandlerMethodId).ToList();
                //var resultSmaller = Res.Where(a => a.OrderNumber < order.OrderNumber).OrderBy(a => a.OrderNumber).ToList();
                var result = Res.Where(a => a.OrderNumber >= order.OrderNumber).OrderBy(a => a.OrderNumber).ToList();
                if (result == null || result.Count == 0)
                {

                }
                else if (result != null && result.Count > 0)
                {
                    //var res = dbContext.TrackingMileStones.Where(a => a.OrderNumber >= order.OrderNumber).ToList();
                    var Count = 1;
                    foreach (var result1 in result)
                    {
                        result1.OrderNumber = order.OrderNumber + Count;
                        dbContext.Entry(result1).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                        Count++;
                    }
                }
            }
            return true;
        }

        public bool SetOrderDelete(TrackingMileStone TM)
        {
            var Res = dbContext.TrackingMileStones.Where(a => a.ShipmentHandlerMethodId == TM.ShipmentHandlerMethodId).ToList();
            var res = Res.Where(a => a.OrderNumber >= TM.OrderNumber).OrderBy(a => a.OrderNumber).ToList();
            if (res != null)
            {
                var count1 = 1;
                foreach (var result in res)
                {
                    result.OrderNumber = result.OrderNumber - 1;
                    dbContext.Entry(result).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    count1++;
                }
            }
            return true;
        }

        public List<ShipmentHandlerModel> GetShimentHandlerMethods()
        {
            List<ShipmentHandlerModel> SHMList = new List<ShipmentHandlerModel>();
            var ShipHandMet = dbContext.ShipmentHandlerMethods.ToList();
            if (ShipHandMet != null)
            {
                foreach (var SHMet in ShipHandMet)
                {
                    ShipmentHandlerModel SHM = new ShipmentHandlerModel();
                    SHM.ShipmentHandlerMethodId = SHMet.ShipmentHandlerMethodId;
                    SHM.ShipmentHandlerMethodName = SHMet.ShipmentHandlerMethodName;
                    SHM.ShipmentHandlerMethodDisplay = SHMet.ShipmentHandlerMethodDisplay;
                    SHM.ShipmentHandlerMethodType = SHMet.ShipmentHandlerMethodType;
                    SHMList.Add(SHM);
                }
            }
            return SHMList;
        }

        public List<TrackingMileStoneModel> GetTrackingMileStone(int ShipmentHandlerMethodId)
        {
            List<TrackingMileStoneModel> SMList = new List<TrackingMileStoneModel>();
            var ShipMileStn = dbContext.TrackingMileStones.Where(a => a.ShipmentHandlerMethodId == ShipmentHandlerMethodId).ToList();
            if (ShipMileStn != null)
            {
                foreach (var SHMet in ShipMileStn)
                {
                    TrackingMileStoneModel TMS = new TrackingMileStoneModel();
                    TMS.TrackingMileStoneId = SHMet.TrackingMileStoneId;
                    TMS.MileStoneKey = SHMet.MileStoneKey;
                    TMS.Description = SHMet.Description;
                    TMS.OrderNumber = SHMet.OrderNumber;
                    TMS.CreatedBy = SHMet.CreatedBy;
                    TMS.CreatedOnUtc = SHMet.CreatedOnUtc;
                    TMS.UpdatedBy = SHMet.UpdatedBy.Value;
                    TMS.UpdatedOnUtc = SHMet.UpdatedOnUtc.Value;
                    TMS.ShipmentHandlerMethodId = SHMet.ShipmentHandlerMethodId;
                    SMList.Add(TMS);
                }
            }
            return SMList.OrderBy(a => a.OrderNumber).ToList();
        }

        public bool DeleteMileStoneRecord(int TrackingMileStoneId)
        {
            var result = dbContext.TrackingMileStones.Where(a => a.TrackingMileStoneId == TrackingMileStoneId).FirstOrDefault();
            if (result != null)
            {
                dbContext.TrackingMileStones.Remove(result);
                dbContext.SaveChanges();
                SetOrderDelete(result);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckTrackingMileStoneKey(TrackingMileStoneModel TrackingMSM)
        {
            var Res = dbContext.TrackingMileStones.Where(a => a.ShipmentHandlerMethodId == TrackingMSM.ShipmentHandlerMethodId).ToList();
            var result = Res.Where(a => a.MileStoneKey == TrackingMSM.MileStoneKey).FirstOrDefault();
            if (result != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
