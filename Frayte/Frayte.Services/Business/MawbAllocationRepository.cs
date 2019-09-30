using Frayte.Services.Business;
using Frayte.Services.Models;
using Frayte.Services.Models.Tradelane;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frayte.Services.DataAccess;
using Frayte.Services.Utility;
using System.IO;
using RazorEngine;
using RazorEngine.Templating;
using System.Web;
using System.Net;
using XStreamline.Log;

namespace Frayte.Services.Business
{

    public class MawbAllocationRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        public FrayteResult SaveMawbDetail(TradelaneMAWBDetail mawbDetail)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                if (mawbDetail.List.Count > 0)
                {
                    var Allocation = dbContext.TradelaneShipmentAllocations.Find(mawbDetail.List[0].MawbAllocationId);
                    if (Allocation != null && (string.IsNullOrEmpty(Allocation.LegNum) || Allocation.LegNum == "Leg1"))
                    {
                        var shipment = dbContext.TradelaneShipments.Find(mawbDetail.TradelaneShipmentId);
                        shipment.MAWB = mawbDetail.MAWB;
                        dbContext.SaveChanges();
                        if (!string.IsNullOrEmpty(shipment.MAWB))
                        {
                            new TradelaneBookingRepository().SaveTradelaneMawb(shipment.MAWB, shipment.TradelaneShipmentId);
                        }
                    }
                }

                foreach (var item in mawbDetail.List)
                {
                    var Timezone = dbContext.Timezones.Where(a => a.TimezoneId == item.TimezoneId).FirstOrDefault();
                    TimeZoneModal TZM = new TimeZoneModal();
                    if (Timezone != null)
                    {
                        TZM.TimezoneId = Timezone.TimezoneId;
                        TZM.Name = Timezone.Name;
                    }
                    var Allocation = dbContext.TradelaneShipmentAllocations.Where(a => a.TradelaneShipmentAllocationId == item.MawbAllocationId).FirstOrDefault();
                    if (Allocation != null)
                    {
                        Allocation.AirlineId = item.AirlineId;
                        Allocation.TimezoneId = item.TimezoneId;
                        Allocation.EstimatedDateofArrival = item.ETA != null && item.ETA.Value.Year != 1 ? UtilityRepository.ConvertToUniversalTimeWitDate(item.ETATime, item.ETA.Value, TZM) : (DateTime?)null;
                        Allocation.EstimatedDateofDelivery = item.ETD != null && item.ETD.Value.Year != 1 ? UtilityRepository.ConvertToUniversalTimeWitDate(item.ETDTime, item.ETD.Value, TZM) : (DateTime?)null;
                        Allocation.FlightNumber = item.FlightNumber;
                        Allocation.MAWB = mawbDetail.MAWB;
                        Allocation.CreatedBy = mawbDetail.AgentId;
                        dbContext.Entry(Allocation).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                        if (!string.IsNullOrEmpty(Allocation.MAWB))
                        {
                            new TradelaneBookingRepository().SaveTradelaneMawb(Allocation.MAWB, (int)Allocation.TradelaneShipmentId);
                        }
                    }
                }
                result.Status = true;
            }
            catch (Exception ex)
            {
                result.Status = false;
            }

            return result;
        }

        public FratyteError SaveMawbAllocation(List<MawbAllocationModel> MAList, string FilePath, string ShipmentType)
        {
            FratyteError MAM = new FratyteError();
            var TradelaneId = MAList.FirstOrDefault().TradelaneId;
            if (MAList.Count > 0)
            {
                var Result = dbContext.TradelaneShipments.Where(a => a.TradelaneShipmentId == TradelaneId).FirstOrDefault();

                if (Result != null)
                {
                    Result.MAWBAgentId = MAList.FirstOrDefault().AgentId;
                    Result.MAWB = MAList.FirstOrDefault().MAWB;
                    dbContext.Entry(Result).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    if (!string.IsNullOrEmpty(Result.MAWB))
                    {
                        new TradelaneBookingRepository().SaveTradelaneMawb(Result.MAWB, Result.TradelaneShipmentId);
                    }
                    foreach (var MAAllocation in MAList)
                    {
                        var Timezone = dbContext.Timezones.Where(a => a.TimezoneId == MAAllocation.TimezoneId).FirstOrDefault();
                        TimeZoneModal TZM = new TimeZoneModal();
                        if (Timezone != null)
                        {

                            TZM.TimezoneId = Timezone.TimezoneId;
                            TZM.Name = Timezone.Name;
                        }
                        var Res = dbContext.TradelaneShipmentAllocations.Where(a => a.TradelaneShipmentAllocationId == MAAllocation.MawbAllocationId).FirstOrDefault();
                        if (Res != null)
                        {
                            Res.TradelaneShipmentId = MAAllocation.TradelaneId;
                            Res.AgentId = MAAllocation.AgentId;
                            Res.AirlineId = MAAllocation.AirlineId;
                            Res.TimezoneId = MAAllocation.TimezoneId;
                            Res.CreatedBy = MAAllocation.CreatedBy;
                            Res.EstimatedDateofArrival = MAAllocation.ETA != null && MAAllocation.ETA.Value.Year != 1 ? UtilityRepository.ConvertToUniversalTimeWitDate(MAAllocation.ETATime, MAAllocation.ETA.Value, TZM) : (DateTime?)null;
                            Res.EstimatedDateofDelivery = MAAllocation.ETD != null && MAAllocation.ETD.Value.Year != 1 ? UtilityRepository.ConvertToUniversalTimeWitDate(MAAllocation.ETDTime, MAAllocation.ETD.Value, TZM) : (DateTime?)null;
                            Res.FlightNumber = MAAllocation.FlightNumber;
                            Res.LegNum = MAAllocation.LegNum;
                            Res.MAWB = MAAllocation.MAWB;
                            dbContext.Entry(Res).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                            if (Res.TradelaneShipmentId > 0 && !string.IsNullOrEmpty(Res.MAWB))
                            {
                                new TradelaneBookingRepository().SaveTradelaneMawb(Res.MAWB, (int)Res.TradelaneShipmentId);
                            }
                        }
                        else
                        {
                            TradelaneShipmentAllocation TSA = new TradelaneShipmentAllocation();
                            TSA.TradelaneShipmentId = MAAllocation.TradelaneId;
                            TSA.AgentId = MAAllocation.AgentId;
                            TSA.AirlineId = MAAllocation.AirlineId;
                            TSA.TimezoneId = MAAllocation.TimezoneId;
                            TSA.CreatedBy = MAAllocation.CreatedBy;
                            TSA.CreatedOnUTC = DateTime.UtcNow;
                            TSA.EstimatedDateofArrival = MAAllocation.ETA != null && MAAllocation.ETA.Value.Year != 1 ? UtilityRepository.ConvertToUniversalTimeWitDate(MAAllocation.ETATime, MAAllocation.ETA.Value, TZM) : (DateTime?)null;
                            TSA.EstimatedDateofDelivery = MAAllocation.ETD != null && MAAllocation.ETD.Value.Year != 1 ? UtilityRepository.ConvertToUniversalTimeWitDate(MAAllocation.ETDTime, MAAllocation.ETD.Value, TZM) : (DateTime?)null;
                            TSA.FlightNumber = MAAllocation.FlightNumber;
                            TSA.LegNum = MAAllocation.LegNum;
                            TSA.MAWB = MAAllocation.MAWB;
                            dbContext.TradelaneShipmentAllocations.Add(TSA);
                            dbContext.SaveChanges();
                            if (TSA.TradelaneShipmentId > 0 && !string.IsNullOrEmpty(TSA.MAWB))
                            {
                                new TradelaneBookingRepository().SaveTradelaneMawb(TSA.MAWB, (int)TSA.TradelaneShipmentId);
                            }
                        }
                    }

                    MAM.Status = true;
                }
                else
                {
                    TradelaneShipment result = new TradelaneShipment();
                    result.MAWBAgentId = MAList.FirstOrDefault().AgentId;
                    result.MAWB = MAList.FirstOrDefault().MAWB;
                    dbContext.TradelaneShipments.Add(result);
                    dbContext.SaveChanges();
                    foreach (var MAAllocation in MAList)
                    {
                        TimeZoneModal TZM = new TimeZoneModal();
                        var Timezone = dbContext.Timezones.Where(a => a.TimezoneId == MAAllocation.TimezoneId).FirstOrDefault();
                        if (Timezone != null)
                        {

                            TZM.TimezoneId = Timezone.TimezoneId;
                            TZM.Name = Timezone.Name;
                        }
                        var Res = dbContext.TradelaneShipmentAllocations.Where(a => a.TradelaneShipmentAllocationId == MAAllocation.MawbAllocationId).FirstOrDefault();
                        if (Res != null)
                        {
                            Res.TradelaneShipmentId = MAAllocation.TradelaneId;
                            Res.AgentId = MAAllocation.AgentId;
                            Res.AirlineId = MAAllocation.AirlineId;
                            Res.CreatedBy = MAAllocation.CreatedBy;
                            Res.TimezoneId = MAAllocation.TimezoneId;
                            Res.EstimatedDateofArrival = MAAllocation.ETA != null && MAAllocation.ETA.Value.Year != 1 ? UtilityRepository.ConvertToUniversalTimeWitDate(MAAllocation.ETATime, MAAllocation.ETA.Value, TZM) : (DateTime?)null;
                            Res.EstimatedDateofDelivery = MAAllocation.ETD != null && MAAllocation.ETD.Value.Year != 1 ? UtilityRepository.ConvertToUniversalTimeWitDate(MAAllocation.ETDTime, MAAllocation.ETD.Value, TZM) : (DateTime?)null;
                            Res.FlightNumber = MAAllocation.FlightNumber;
                            Res.LegNum = MAAllocation.LegNum;
                            Res.MAWB = MAAllocation.MAWB;
                            dbContext.Entry(Res).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                            if (Res.TradelaneShipmentId > 0 && !string.IsNullOrEmpty(Res.MAWB))
                            {
                                new TradelaneBookingRepository().SaveTradelaneMawb(Res.MAWB, (int)Res.TradelaneShipmentId);
                            }
                        }
                        else
                        {
                            TradelaneShipmentAllocation TSA = new TradelaneShipmentAllocation();
                            TSA.TradelaneShipmentId = MAAllocation.TradelaneId;
                            TSA.AgentId = MAAllocation.AgentId;
                            TSA.AirlineId = MAAllocation.AirlineId;
                            TSA.CreatedBy = MAAllocation.CreatedBy;
                            TSA.TimezoneId = MAAllocation.TimezoneId;
                            TSA.CreatedOnUTC = DateTime.UtcNow;
                            TSA.EstimatedDateofArrival = MAAllocation.ETA != null && MAAllocation.ETA.Value.Year != 1 ? UtilityRepository.ConvertToUniversalTimeWitDate(MAAllocation.ETATime, MAAllocation.ETA.Value, TZM) : (DateTime?)null;
                            TSA.EstimatedDateofDelivery = MAAllocation.ETD != null && MAAllocation.ETD.Value.Year != 1 ? UtilityRepository.ConvertToUniversalTimeWitDate(MAAllocation.ETDTime, MAAllocation.ETD.Value, TZM) : (DateTime?)null;
                            TSA.FlightNumber = MAAllocation.FlightNumber;
                            TSA.LegNum = MAAllocation.LegNum;
                            TSA.MAWB = MAAllocation.MAWB;
                            dbContext.TradelaneShipmentAllocations.Add(TSA);
                            dbContext.SaveChanges();
                            if (TSA.TradelaneShipmentId > 0 && !string.IsNullOrEmpty(TSA.MAWB))
                            {
                                new TradelaneBookingRepository().SaveTradelaneMawb(TSA.MAWB, (int)TSA.TradelaneShipmentId);
                            }
                        }
                    }
                    MAM.Status = true;
                }
                if (Result.ShipmentHandlerMethodId == 5)
                {
                    if (MAList.FirstOrDefault().AgentId > 0)
                    {
                        SendMailtoAgent(MAList.FirstOrDefault(), MAList.FirstOrDefault().AgentId, MAList.FirstOrDefault().TradelaneId, FilePath);
                    }

                    if (MAList.Skip(1).FirstOrDefault().AgentId > 0)
                    {
                        SendMailtoAgent(MAList.Skip(1).FirstOrDefault(), MAList.Skip(1).FirstOrDefault().AgentId, MAList.Skip(1).FirstOrDefault().TradelaneId, FilePath);
                    }
                }
                else
                {
                    if (MAList.FirstOrDefault().AgentId > 0)
                    {
                        SendMailtoAgent(MAList.FirstOrDefault(), MAList.FirstOrDefault().AgentId, MAList.FirstOrDefault().TradelaneId, FilePath);
                    }
                }

                if (MAM.Status && ShipmentType == "Tradelane")
                {
                    new TradelaneBookingRepository().SaveIsAgentMawbAllocationDocument(MAList.FirstOrDefault().TradelaneId);
                }
                return MAM;
            }
            else
            {
                MAM.Status = false;
                return MAM;
            }
        }

        public FratyteError SaveExpressMawbAllocation(List<MawbAllocationModel> MAList, string FilePath, string ShipmentType)
        {
            FratyteError MAM = new FratyteError();
            var TradelaneId = MAList.FirstOrDefault().TradelaneId;
            if (MAList.Count > 0)
            {
                var Result = dbContext.TradelaneShipments.Where(a => a.TradelaneShipmentId == TradelaneId).FirstOrDefault();

                if (Result != null)
                {
                    Result.MAWBAgentId = MAList.FirstOrDefault().AgentId;
                    Result.MAWB = MAList.FirstOrDefault().MAWB;
                    dbContext.Entry(Result).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    if (!string.IsNullOrEmpty(Result.MAWB))
                    {
                        new TradelaneBookingRepository().SaveTradelaneMawb(Result.MAWB, Result.TradelaneShipmentId);
                    }
                    foreach (var MAAllocation in MAList)
                    {
                        var Timezone = dbContext.Timezones.Where(a => a.TimezoneId == MAAllocation.TimezoneId).FirstOrDefault();
                        TimeZoneModal TZM = new TimeZoneModal();
                        if (Timezone != null)
                        {

                            TZM.TimezoneId = Timezone.TimezoneId;
                            TZM.Name = Timezone.Name;
                        }
                        var Res = dbContext.TradelaneShipmentAllocations.Where(a => a.TradelaneShipmentAllocationId == MAAllocation.MawbAllocationId).FirstOrDefault();
                        if (Res != null)
                        {
                            Res.TradelaneShipmentId = MAAllocation.TradelaneId;
                            Res.AgentId = MAAllocation.AgentId;
                            Res.AirlineId = MAAllocation.AirlineId;
                            Res.TimezoneId = MAAllocation.TimezoneId;
                            Res.CreatedBy = MAAllocation.CreatedBy;
                            Res.EstimatedDateofArrival = MAAllocation.ETA != null && MAAllocation.ETA.Value.Year != 1 ? UtilityRepository.ConvertToUniversalTimeWitDate(MAAllocation.ETATime, MAAllocation.ETA.Value, TZM) : (DateTime?)null;
                            Res.EstimatedDateofDelivery = MAAllocation.ETD != null && MAAllocation.ETD.Value.Year != 1 ? UtilityRepository.ConvertToUniversalTimeWitDate(MAAllocation.ETDTime, MAAllocation.ETD.Value, TZM) : (DateTime?)null;
                            Res.FlightNumber = MAAllocation.FlightNumber;
                            Res.LegNum = MAAllocation.LegNum;
                            Res.MAWB = MAAllocation.MAWB;
                            dbContext.Entry(Res).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                            if (Res.TradelaneShipmentId > 0 && !string.IsNullOrEmpty(Res.MAWB))
                            {
                                new TradelaneBookingRepository().SaveTradelaneMawb(Res.MAWB, (int)Res.TradelaneShipmentId);
                            }
                        }
                        else
                        {
                            TradelaneShipmentAllocation TSA = new TradelaneShipmentAllocation();
                            TSA.TradelaneShipmentId = MAAllocation.TradelaneId;
                            TSA.AgentId = MAAllocation.AgentId;
                            TSA.AirlineId = MAAllocation.AirlineId;
                            TSA.TimezoneId = MAAllocation.TimezoneId;
                            TSA.CreatedBy = MAAllocation.CreatedBy;
                            TSA.CreatedOnUTC = DateTime.UtcNow;
                            TSA.EstimatedDateofArrival = MAAllocation.ETA != null && MAAllocation.ETA.Value.Year != 1 ? UtilityRepository.ConvertToUniversalTimeWitDate(MAAllocation.ETATime, MAAllocation.ETA.Value, TZM) : (DateTime?)null;
                            TSA.EstimatedDateofDelivery = MAAllocation.ETD != null && MAAllocation.ETD.Value.Year != 1 ? UtilityRepository.ConvertToUniversalTimeWitDate(MAAllocation.ETDTime, MAAllocation.ETD.Value, TZM) : (DateTime?)null;
                            TSA.FlightNumber = MAAllocation.FlightNumber;
                            TSA.LegNum = MAAllocation.LegNum;
                            TSA.MAWB = MAAllocation.MAWB;
                            dbContext.TradelaneShipmentAllocations.Add(TSA);
                            dbContext.SaveChanges();
                            if (TSA.TradelaneShipmentId > 0 && !string.IsNullOrEmpty(TSA.MAWB))
                            {
                                new TradelaneBookingRepository().SaveTradelaneMawb(TSA.MAWB, (int)TSA.TradelaneShipmentId);
                            }
                        }
                    }

                    MAM.Status = true;
                }
                else
                {
                    TradelaneShipment result = new TradelaneShipment();
                    result.MAWBAgentId = MAList.FirstOrDefault().AgentId;
                    result.MAWB = MAList.FirstOrDefault().MAWB;
                    dbContext.TradelaneShipments.Add(result);
                    dbContext.SaveChanges();
                    foreach (var MAAllocation in MAList)
                    {
                        TimeZoneModal TZM = new TimeZoneModal();
                        var Timezone = dbContext.Timezones.Where(a => a.TimezoneId == MAAllocation.TimezoneId).FirstOrDefault();
                        if (Timezone != null)
                        {

                            TZM.TimezoneId = Timezone.TimezoneId;
                            TZM.Name = Timezone.Name;
                        }
                        var Res = dbContext.TradelaneShipmentAllocations.Where(a => a.TradelaneShipmentAllocationId == MAAllocation.MawbAllocationId).FirstOrDefault();
                        if (Res != null)
                        {
                            Res.TradelaneShipmentId = MAAllocation.TradelaneId;
                            Res.AgentId = MAAllocation.AgentId;
                            Res.AirlineId = MAAllocation.AirlineId;
                            Res.CreatedBy = MAAllocation.CreatedBy;
                            Res.TimezoneId = MAAllocation.TimezoneId;
                            Res.EstimatedDateofArrival = MAAllocation.ETA != null && MAAllocation.ETA.Value.Year != 1 ? UtilityRepository.ConvertToUniversalTimeWitDate(MAAllocation.ETATime, MAAllocation.ETA.Value, TZM) : (DateTime?)null;
                            Res.EstimatedDateofDelivery = MAAllocation.ETD != null && MAAllocation.ETD.Value.Year != 1 ? UtilityRepository.ConvertToUniversalTimeWitDate(MAAllocation.ETDTime, MAAllocation.ETD.Value, TZM) : (DateTime?)null;
                            Res.FlightNumber = MAAllocation.FlightNumber;
                            Res.LegNum = MAAllocation.LegNum;
                            Res.MAWB = MAAllocation.MAWB;
                            dbContext.Entry(Res).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                            if (Res.TradelaneShipmentId > 0 && !string.IsNullOrEmpty(Res.MAWB))
                            {
                                new TradelaneBookingRepository().SaveTradelaneMawb(Res.MAWB, (int)Res.TradelaneShipmentId);
                            }
                        }
                        else
                        {
                            TradelaneShipmentAllocation TSA = new TradelaneShipmentAllocation();
                            TSA.TradelaneShipmentId = MAAllocation.TradelaneId;
                            TSA.AgentId = MAAllocation.AgentId;
                            TSA.AirlineId = MAAllocation.AirlineId;
                            TSA.CreatedBy = MAAllocation.CreatedBy;
                            TSA.TimezoneId = MAAllocation.TimezoneId;
                            TSA.CreatedOnUTC = DateTime.UtcNow;
                            TSA.EstimatedDateofArrival = MAAllocation.ETA != null && MAAllocation.ETA.Value.Year != 1 ? UtilityRepository.ConvertToUniversalTimeWitDate(MAAllocation.ETATime, MAAllocation.ETA.Value, TZM) : (DateTime?)null;
                            TSA.EstimatedDateofDelivery = MAAllocation.ETD != null && MAAllocation.ETD.Value.Year != 1 ? UtilityRepository.ConvertToUniversalTimeWitDate(MAAllocation.ETDTime, MAAllocation.ETD.Value, TZM) : (DateTime?)null;
                            TSA.FlightNumber = MAAllocation.FlightNumber;
                            TSA.LegNum = MAAllocation.LegNum;
                            TSA.MAWB = MAAllocation.MAWB;
                            dbContext.TradelaneShipmentAllocations.Add(TSA);
                            dbContext.SaveChanges();
                            if (TSA.TradelaneShipmentId > 0 && !string.IsNullOrEmpty(TSA.MAWB))
                            {
                                new TradelaneBookingRepository().SaveTradelaneMawb(TSA.MAWB, (int)TSA.TradelaneShipmentId);
                            }
                        }
                    }
                    MAM.Status = true;
                }
                if (MAM.Status && ShipmentType == "Tradelane")
                {
                    new TradelaneBookingRepository().SaveIsAgentMawbAllocationDocument(MAList.FirstOrDefault().TradelaneId);
                }
                return MAM;
            }
            else
            {
                MAM.Status = false;
                return MAM;
            }
        }

        public FratyteError SendMawbAllocationMail(List<MawbAllocationModel> MAList, string FilePath, string ShipmentType)
        {
            FratyteError MAM = new FratyteError();
            var TradelaneId = MAList.FirstOrDefault().TradelaneId;
            if (MAList.Count > 0)
            {
                var Result = dbContext.TradelaneShipments.Where(a => a.TradelaneShipmentId == TradelaneId).FirstOrDefault();

                if (Result != null)
                {
                    if (Result.ShipmentHandlerMethodId == 5)
                    {
                        if (MAList.FirstOrDefault().AgentId > 0)
                        {
                            SendMailtoAgent(MAList.FirstOrDefault(), MAList.FirstOrDefault().AgentId, MAList.FirstOrDefault().TradelaneId, FilePath);
                        }

                        if (MAList.Skip(1).FirstOrDefault().AgentId > 0)
                        {
                            SendMailtoAgent(MAList.Skip(1).FirstOrDefault(), MAList.Skip(1).FirstOrDefault().AgentId, MAList.Skip(1).FirstOrDefault().TradelaneId, FilePath);
                        }
                    }
                    else
                    {
                        if (MAList.FirstOrDefault().AgentId > 0)
                        {
                            SendMailtoAgent(MAList.FirstOrDefault(), MAList.FirstOrDefault().AgentId, MAList.FirstOrDefault().TradelaneId, FilePath);
                        }
                    }
                }
                else
                {
                    TradelaneShipment result = new TradelaneShipment();
                    result.MAWBAgentId = MAList.FirstOrDefault().AgentId;
                    result.MAWB = MAList.FirstOrDefault().MAWB;
                    dbContext.TradelaneShipments.Add(result);
                    dbContext.SaveChanges();
                    MAM.Status = true;
                }
                return MAM;
            }
            else
            {
                MAM.Status = false;
                return MAM;
            }
        }

        public List<MawbAllocationModel> GetMawbAllocation(int TradelaneShipmentId, string Leg)
        {
            List<MawbAllocationModel> MAMList = new List<MawbAllocationModel>();
            var userid = dbContext.TradelaneShipments.Where(a => a.TradelaneShipmentId == TradelaneShipmentId).FirstOrDefault().CustomerId;
            var CustomerDetail = new CustomerRepository().GetCustomerDetail(userid);

            List<TradelaneShipmentAllocation> Result = new List<TradelaneShipmentAllocation>();

            if (!string.IsNullOrEmpty(Leg))
            {
                Result = dbContext.TradelaneShipmentAllocations.Where(a => a.TradelaneShipmentId == TradelaneShipmentId && a.LegNum == Leg).ToList();
            }
            else
            {
                Result = dbContext.TradelaneShipmentAllocations.Where(a => a.TradelaneShipmentId == TradelaneShipmentId).ToList();
            }

            if (Result != null && Result.Count > 0)
            {
                foreach (var res in Result)
                {
                    TimeZoneModal TZM = new TimeZoneModal();
                    var Timezone = dbContext.Timezones.Where(a => a.TimezoneId == res.TimezoneId).FirstOrDefault();
                    if (Timezone != null)
                    {

                        TZM.TimezoneId = Timezone.TimezoneId;
                        TZM.Name = Timezone.Name;
                    }
                    MawbAllocationModel MAM = new MawbAllocationModel();
                    MAM.AgentId = res.AgentId.Value;
                    MAM.AirlineId = res.AirlineId.Value;
                    MAM.CreatedBy = res.CreatedBy.Value;
                    MAM.CreatedOnUTC = res.CreatedOnUTC.Value;
                    MAM.ETA = res.EstimatedDateofArrival != null ? UtilityRepository.ConvertDatetoSpecifiedTimeZoneTime(res.EstimatedDateofArrival.Value, TZM) : (DateTime?)null;
                    MAM.ETD = res.EstimatedDateofDelivery != null ? UtilityRepository.ConvertDatetoSpecifiedTimeZoneTime(res.EstimatedDateofDelivery.Value, TZM) : (DateTime?)null;
                    MAM.ETATime = res.EstimatedDateofArrival != null ? UtilityRepository.ConvertToCustomerTimeZone(res.EstimatedDateofArrival.Value.TimeOfDay, TZM) : "";
                    MAM.ETDTime = res.EstimatedDateofDelivery != null ? UtilityRepository.ConvertToCustomerTimeZone(res.EstimatedDateofDelivery.Value.TimeOfDay, TZM) : "";
                    MAM.TimezoneId = res.TimezoneId.Value;
                    MAM.FlightNumber = res.FlightNumber;
                    MAM.LegNum = res.LegNum;
                    MAM.MAWB = res.MAWB;
                    MAM.MawbAllocationId = res.TradelaneShipmentAllocationId;
                    MAM.TradelaneId = res.TradelaneShipmentId.Value;
                    MAMList.Add(MAM);
                }
                return MAMList;
            }
            else
            {
                return MAMList;
            }
        }

        public string GetMawbDocumentName(int TradelaneShipmentId)
        {
            var DocName = "";
            var result1 = dbContext.TradelaneShipmentDocuments.Where(a => a.TradelaneShipmentId == TradelaneShipmentId && a.DocumentType == "MAWB").ToList();
            result1.Reverse();
            var result = result1.FirstOrDefault();
            if (result != null)
            {
                DocName = result.DocumentName;
            }
            return DocName;
        }

        public List<DirectBookingCustomer> GetAgents()
        {
            // To Do : customer should come according to moduleType
            var operationzone = UtilityRepository.GetOperationZone();
            var customers = (from r in dbContext.Users
                             join ua in dbContext.UserAdditionals on r.UserId equals ua.UserId
                             join ur in dbContext.UserRoles on r.UserId equals ur.UserId
                             where
                                ur.RoleId == (int)FrayteUserRole.Agent &&
                                r.IsActive == true &&
                                r.OperationZoneId == operationzone.OperationZoneId
                             select new DirectBookingCustomer
                             {
                                 CustomerId = r.UserId,
                                 CustomerName = r.ContactName,
                                 CompanyName = r.CompanyName,
                                 AccountNumber = ua.AccountNo,
                                 EmailId = r.UserEmail,
                                 ValidDays = ua.DaysValidity.HasValue ? ua.DaysValidity.Value : 0,
                                 CustomerCurrency = ua.CreditLimitCurrencyCode,
                                 OperationZoneId = r.OperationZoneId
                             }).Distinct().ToList();

            return customers.OrderBy(p => p.CompanyName).ToList();
        }

        public FrayteResult DeleteMawbAllocation(int AllocationId)
        {
            FrayteResult FE = new FrayteResult();
            FE.Status = false;
            var Result = dbContext.TradelaneShipmentAllocations.Where(a => a.TradelaneShipmentAllocationId == AllocationId).FirstOrDefault();
            if (Result != null)
            {
                dbContext.TradelaneShipmentAllocations.Remove(Result);
                dbContext.SaveChanges();
                FE.Status = true;
            }
            return FE;
        }

        public ShipmentRoute GetShipmentHandlerId(int TradelaneShipemntId)
        {
            var Result = (from TS in dbContext.TradelaneShipments
                          join SH in dbContext.ShipmentHandlerMethods on TS.ShipmentHandlerMethodId equals SH.ShipmentHandlerMethodId into PS
                          from SH in PS.DefaultIfEmpty()
                          where TS.TradelaneShipmentId == TradelaneShipemntId
                          select new ShipmentRoute
                          {
                              MAWB = TS.MAWB,
                              RouteFrom = TS.DepartureAirportCode,
                              RouteTo = TS.DestinationAirportCode,
                              ShipemntHandlerMethodCode = SH.ShipmentHandlerMethodCode,
                              ShipmentHandlerMethodId = SH.ShipmentHandlerMethodId
                          }).FirstOrDefault();

            return Result;
        }

        public string GetAttachment(int TradelaneId)
        {
            var res = dbContext.TradelaneShipments.Where(a => a.TradelaneShipmentId == TradelaneId).FirstOrDefault();
            string Attachment = "";
            if (res != null && res.BatteryDeclarationType != "None")
            {
                var Res = dbContext.TradelaneShipmentDocuments.Where(a => a.TradelaneShipmentId == TradelaneId && a.DocumentType != FrayteTradelaneShipmentDocumentEnum.ExportManifest).ToList();
                var count = 1;
                foreach (var r in Res)
                {
                    if (count == Res.Count)
                    {
                        Attachment = Attachment + HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + TradelaneId) + '/' + r.DocumentName;

                    }
                    else
                    {
                        Attachment = Attachment + HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + TradelaneId) + '/' + r.DocumentName + ';';
                    }
                    count++;
                }
            }
            if (string.IsNullOrEmpty(Attachment))
            {
                var Res = dbContext.TradelaneShipmentDocuments.Where(a => a.TradelaneShipmentId == TradelaneId && a.DocumentType == FrayteTradelaneShipmentDocumentEnum.DriverManifest).FirstOrDefault();

                if (Res != null)
                {
                    Attachment = Attachment + HttpContext.Current.Server.MapPath("~/UploadFiles/Tradelane/" + TradelaneId) + '/' + Res.DocumentName;
                }
            }

            return Attachment;
        }

        public void SendMailtoAgent(MawbAllocationModel model, int UserId, int TradelaneId, string FilePath)
        {
            new TradelaneEmailRepository().SendMailtoAgent(model, UserId, TradelaneId, FilePath);
        }

        public string GetMawbUnallocatedShipments()
        {
            try
            {
                // To Do : customer should come according to moduleType
                var operationzone = UtilityRepository.GetOperationZone();
                var result = (from r in dbContext.Users
                              join ua in dbContext.UserAdditionals on r.UserId equals ua.UserId
                              join ur in dbContext.UserRoles on r.UserId equals ur.UserId
                              where
                                 ur.RoleId == (int)FrayteUserRole.Agent &&
                                 r.IsActive == true &&
                                 r.OperationZoneId == operationzone.OperationZoneId
                              select new
                              {
                                  r.UserId,
                                  r.ContactName,
                                  r.CompanyName,
                                  ua.AccountNo,
                                  r.UserEmail,
                                  ua.DaysValidity,
                                  ua.CreditLimitCurrencyCode,
                                  r.OperationZoneId,
                                  r.WorkingStartTime,
                                  r.WorkingEndTime,
                                  r.WorkingWeekDayId
                              }).ToList();


                if (result.Count > 0)
                {
                    foreach (var res in result)
                    {
                        var date = DateTime.UtcNow;
                        //var workingday = dbContext.WorkingWeekDays.Where(a => a.WorkingWeekDayId == res.WorkingWeekDayId).FirstOrDefault();
                        //var WorkingDays = workingday.Description.Split('-');
                        //var WokingStarday = WorkingDays[0];
                        //var WokingEndday = WorkingDays[1];
                        //if (WokingStarday.Contains("Mon"))
                        //{

                        //}

                        //if (WokingEndday.Contains("Thur"))
                        //{
                        //   var tm = DateTime.UtcNow.Month;
                        //}
                        //if (WokingEndday.Contains("Fri"))
                        //{

                        //}
                        //if (WokingEndday.Contains("Sat"))
                        //{

                        //}

                        var startdate = new DateTime(date.Year, date.Month, date.Day, res.WorkingStartTime.Value.Hours, res.WorkingStartTime.Value.Minutes, res.WorkingStartTime.Value.Seconds, DateTimeKind.Utc);
                        var enddate = new DateTime(date.Year, date.Month, date.Day, res.WorkingEndTime.Value.Hours, res.WorkingEndTime.Value.Minutes, res.WorkingEndTime.Value.Seconds, DateTimeKind.Utc);

                        if (date.TimeOfDay >= startdate.TimeOfDay && date.TimeOfDay <= enddate.TimeOfDay)
                        {

                            List<TradelaneGetShipmentModel> Shipments = (from TS in dbContext.TradelaneShipments
                                                                         join Usr in dbContext.Users on TS.CustomerId equals Usr.UserId
                                                                         join TSFA in dbContext.TradelaneShipmentAddresses on TS.FromAddressId equals TSFA.TradelaneShipmentAddressId
                                                                         join TSTA in dbContext.TradelaneShipmentAddresses on TS.ToAddressId equals TSTA.TradelaneShipmentAddressId
                                                                         where TS.MAWBAgentId == res.UserId
                                                                         && (TS.IsAgentMAWBAllocated == false || TS.IsAgentMAWBAllocated == null)
                                                                         && (TS.ShipmentStatusId == 28 || TS.ShipmentStatusId == 30 || TS.ShipmentStatusId == 31 || TS.ShipmentStatusId == 32)
                                                                         select new TradelaneGetShipmentModel()
                                                                         {
                                                                             ConsigneeCompanyName = TSTA.CompanyName,
                                                                             ShipperCompanyName = TSFA.CompanyName,
                                                                             CreatedOn = TS.CreatedOnUtc,
                                                                             Customer = Usr.UserName,
                                                                             IsAgentMAWBAllocated = TS.IsAgentMAWBAllocated != null ? TS.IsAgentMAWBAllocated.Value : false,
                                                                             MAWBAgentId = TS.MAWBAgentId != null ? TS.MAWBAgentId.Value : 0,
                                                                             TradelaneShipmentId = TS.TradelaneShipmentId,
                                                                             MAWB = TS.MAWB,
                                                                             LegNum = TS.ShipmentHandlerMethodId == 5 ? "Leg1" : "",
                                                                             FrayteRefNo = TS.FrayteNumber,
                                                                             ShipmentMethodHandlerId = TS.ShipmentHandlerMethodId.HasValue ? TS.ShipmentHandlerMethodId.Value : 0,
                                                                         }).ToList();


                            var TradelaneshipmentAllocationLeg2Id = dbContext.TradelaneShipmentAllocations.Where(a => a.AgentId == res.UserId && a.LegNum == "Leg2").ToList();
                            if (TradelaneshipmentAllocationLeg2Id != null)
                            {
                                List<TradelaneGetShipmentModel> ShipmentsLeg2TransShip = (from TSA in dbContext.TradelaneShipmentAllocations
                                                                                          join TS in dbContext.TradelaneShipments on TSA.TradelaneShipmentId equals TS.TradelaneShipmentId
                                                                                          join Usr in dbContext.Users on TS.CustomerId equals Usr.UserId
                                                                                          join TSFA in dbContext.TradelaneShipmentAddresses on TS.FromAddressId equals TSFA.TradelaneShipmentAddressId
                                                                                          join TSTA in dbContext.TradelaneShipmentAddresses on TS.ToAddressId equals TSTA.TradelaneShipmentAddressId
                                                                                          where TSA.AgentId == res.UserId
                                                                                          && TSA.LegNum == "Leg2"
                                                                                          && (TS.IsAgentMAWBAllocated == false || TS.IsAgentMAWBAllocated == null)
                                                                                          && TS.ShipmentStatusId != 27
                                                                                          select new TradelaneGetShipmentModel()
                                                                                          {
                                                                                              ConsigneeCompanyName = TSTA.CompanyName,
                                                                                              ShipperCompanyName = TSFA.CompanyName,
                                                                                              CreatedOn = TS.CreatedOnUtc,
                                                                                              Customer = Usr.UserName,
                                                                                              LegNum = "Leg2",
                                                                                              IsAgentMAWBAllocated = TS.IsAgentMAWBAllocated != null ? TS.IsAgentMAWBAllocated.Value : false,
                                                                                              MAWBAgentId = TS.MAWBAgentId != null ? TS.MAWBAgentId.Value : 0,
                                                                                              TradelaneShipmentId = TS.TradelaneShipmentId,
                                                                                              MAWB = TS.MAWB,
                                                                                              FrayteRefNo = TS.FrayteNumber,
                                                                                              ShipmentMethodHandlerId = TS.ShipmentHandlerMethodId.HasValue ? TS.ShipmentHandlerMethodId.Value : 0,
                                                                                          }

                                                                                          ).ToList();
                                var sd = ShipmentsLeg2TransShip.GroupBy(a => a.FrayteRefNo).Select(a => a.First()).ToList();
                                if (ShipmentsLeg2TransShip.Count > 0)
                                {
                                    Shipments.AddRange(sd);
                                }

                            }

                            SendAgentConsolidateShipmentMail(Shipments, res.UserId);
                        }
                        else
                        {

                        }
                    }
                }
                return "";
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public void SendAgentConsolidateShipmentMail(List<TradelaneGetShipmentModel> TSList, int UserId)
        {

            new TradelaneEmailRepository().SendAgentConsolidateShipmentMail(TSList, UserId);

        }

        public string GetTimeZoneName(int TradelaneShipmentId)
        {
            string timezone = (from ts in dbContext.TradelaneShipments
                               join tsa in dbContext.TradelaneShipmentAddresses on ts.FromAddressId equals tsa.TradelaneShipmentAddressId
                               join cc in dbContext.Countries on tsa.CountryId equals cc.CountryId
                               join tz in dbContext.Timezones on cc.TimeZoneId equals tz.TimezoneId
                               where ts.TradelaneShipmentId == TradelaneShipmentId
                               select tz.Name).FirstOrDefault();

            return timezone;
        }
    }
}
