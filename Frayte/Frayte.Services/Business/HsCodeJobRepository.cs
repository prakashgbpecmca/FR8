using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frayte.Services.Models;
using Frayte.Services.DataAccess;
using System.Data.Entity.Core.Objects;

namespace Frayte.Services.Business
{
    public class HsCodeJobRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        #region UnAssigned Jobs
        public List<FrayteUnAssignedJob> GetUnAssignedJobs(TrackHSCodeJob obj)
        {
            List<FrayteUnAssignedJob> list = new List<FrayteUnAssignedJob>();
            try
            {
                int SkipRows = 0;
                SkipRows = (obj.CurrentPage - 1) * obj.TakeRows;
                var OperationZone = UtilityRepository.GetOperationZone();
                // To Do : Get ETD Date And Time  in the list
                var jobs = dbContext.TrackUnAssignedJob(obj.FromDate, obj.ToDate, SkipRows, obj.TakeRows, OperationZone.OperationZoneId).ToList();

                var d = jobs.Where(p => p.EstimatedDateOfDelivery.HasValue).ToList();
                if (jobs != null && jobs.Count > 0)
                {
                    foreach (var data in jobs)
                    {
                        FrayteUnAssignedJob job = new FrayteUnAssignedJob();
                        job.CourierCompany = data.LogisticCompany;
                        job.CourierCompanyDisplay = data.LogisticCompanyDisplay;
                        job.Customer = data.ContactName;
                        job.DisplayStatus = data.StatusName;
                        job.ShippedFromCompany = data.FromCompany;
                        job.ShippedToCompany = data.ToCompany;
                        job.ShippingDate = data.CreatedOn;
                        job.TotalRows = data.TotalRows.HasValue ? data.TotalRows.Value : 0;
                        job.TrackingNo = data.TrackingNo;
                        job.EstimatedDateOfDeparture = data.EstimatedDateOfDelivery;
                        job.EstimatedTimeOfDeparture = UtilityRepository.GetTimeZoneTime(data.EstimatedTimeofDelivery);
                        job.EstimatedDateOfArrival = data.EstimatedDateofArrival;
                        job.EstimatedTimeOfArrival = UtilityRepository.GetTimeZoneTime(data.EstimatedTimeofArrival);
                        job.ShipmentId = data.DirectShipmentId;
                        job.ShipmentDescription = data.ContentDescription;
                        job.Reference1 = data.Reference1;
                        job.FrayteNumber = data.FrayteNumber;
                        job.FromCountry = data.FromCountry;
                        job.ToCountry = data.ToCountry;

                        list.Add(job);
                    }
                }

                // To Do : OrderBy on ETD date and then ETD Time 
                var collection = list.OrderByDescending(p => p.EstimatedDateOfDeparture).ThenByDescending(p => UtilityRepository.GetTimeFromString(p.EstimatedTimeOfDeparture)).ToList();
                return collection;

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return list;
            }


        }



        public FrayteResult AssignJobToOperator(OpeartorJob job)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                if (job != null && job.OperatorId > 0 && job.jobs != null && job.jobs.Count > 0)
                {

                    foreach (var data in job.jobs)
                    {
                        var ship = dbContext.eCommerceShipments.Find(data.ShipmentId);
                        if (ship != null)
                        {
                            ship.AssignedTo = job.OperatorId;
                            //dbContext.Entry<ship>
                            dbContext.SaveChanges();
                            result.Status = true;
                        }

                    }
                }
                else
                {
                    result.Status = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                result.Status = false;
                return result;
            }


        }


        #endregion

        #region  Jobs In Progress

        public JobsInProgressCount GetJobsInProgressCount()
        {
            JobsInProgressCount jobCount = new JobsInProgressCount();
            try
            {
                var OperationZone = UtilityRepository.GetOperationZone();
                var data = dbContext.eCommerceShipments
                              .Where(p => (p.CustomManifestId == null || p.CustomManifestId == 0) && p.ShipmentStatusId == (int)FrayteShipmentStatus.eCCurrent &&
                              p.OpearionZoneId == OperationZone.OperationZoneId).ToList();
                if (data != null && data.Count > 0)
                {
                    jobCount.TotalJobs = data.Count;
                }
                else
                {
                    jobCount.TotalJobs = 0;
                }

                var mycount1 = (from cnt in dbContext.eCommerceShipmentDetails
                                join ec in dbContext.eCommerceShipments on cnt.eCommerceShipmentId equals ec.eCommerceShipmentId
                                where ec.OpearionZoneId == OperationZone.OperationZoneId && // ec.eCommerceShipmentId == 11 &&
                                ec.ShipmentStatusId == (int)FrayteShipmentStatus.eCCurrent && (ec.CustomManifestId == null || ec.CustomManifestId == 0)
                                && ec.AssignedTo != null
                                group cnt by cnt.eCommerceShipmentId into g
                                select new
                                {
                                    name = g.Key,
                                    HSCode = g.Min(r => string.IsNullOrEmpty(r.HSCode) ? "" : r.HSCode),
                                    count = g.Count()
                                }).ToList().Where(p => !string.IsNullOrEmpty(p.HSCode)).ToList();
                var mycount = mycount1.Count;
                jobCount.CompletedJobs = mycount;
                return jobCount;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return jobCount;
            }
        }
        public List<FrayteMappedJobs> GetAssignedJobs(TrackAssignedJob obj)
        {
            List<FrayteMappedJobs> list = new List<FrayteMappedJobs>();
            try
            {
                var OperationZone = UtilityRepository.GetOperationZone();
                int SkipRows = 0;
                SkipRows = (obj.CurrentPage - 1) * obj.TakeRows;
                var mappedJobs = dbContext.spGet_TrackAssignedJob(obj.FromDate, obj.ToDate, SkipRows, obj.TakeRows, OperationZone.OperationZoneId, obj.OperatorId, obj.DestinationCountry).ToList();
                var idList = mappedJobs.Select(p => p.eCommerceShipmentId).ToList();

                var detailList = (from eCD in dbContext.eCommerceShipmentDetails
                                  where idList.Contains(eCD.eCommerceShipmentId) && (obj.AllShipments == true || string.IsNullOrEmpty(eCD.HSCode))
                                  select new eCommercePackage
                                  {
                                      eCommerceShipmentDetailId = eCD.eCommerceShipmentDetailId,
                                      eCommerceShhipmentId = eCD.eCommerceShipmentId,
                                      Value = eCD.DeclaredValue.HasValue ? eCD.DeclaredValue.Value : 0,
                                      CartoonValue = eCD.CartoonValue,
                                      Weight = eCD.Weight,
                                      Length = eCD.Length,
                                      Height = eCD.Height,
                                      Content = eCD.PiecesContent,
                                      Width = eCD.Width,
                                      HSCode = eCD.HSCode,
                                  }).ToList();

                list = mappedJobs.Select(group => new FrayteMappedJobs
                {
                    ShipmentId = group.eCommerceShipmentId,
                    CourierCompany = group.LogisticCompany,
                    CourierCompanyDisplay = group.LogisticCompanyDisplay,
                    Customer = group.ContactName,
                    DisplayStatus = group.StatusName,
                    FrayteNumber = group.FrayteNumber,
                    FromCountry = group.FromCountry,
                    EstimatedDateOfDeparture = group.EstimatedDateOfDelivery,
                    EstimatedTimeOfDeparture = UtilityRepository.GetTimeZoneTime(group.EstimatedTimeofDelivery),
                    EstimatedDateOfArrival = group.EstimatedDateofArrival,
                    EstimatedTimeOfArrival = UtilityRepository.GetTimeZoneTime(group.EstimatedTimeofArrival),
                    ShippedFromCompany = group.FromCompany,
                    ToCountry = group.ToCountry,
                    ShippedToCompany = group.ToCompany,
                    ShipmentDescription = group.ContentDescription,
                    ShippingDate = group.CreatedOn,
                    Status = group.StatusName,
                    TotalRows = group.TotalRows.HasValue ? group.TotalRows.Value : 0,
                    Reference1 = group.Reference1,
                    TrackingNo = group.TrackingNo,
                    StaffAssigned = group.OperatorName,
                    StaffCompanyName = group.OperatorCompanyName,
                    StaffEmail = group.OperatorEmail,
                    Packages = detailList.Where(p => p.eCommerceShhipmentId == group.eCommerceShipmentId).Select(subGroup => new eCommercePackage
                    {
                        eCommerceShipmentDetailId = subGroup.eCommerceShipmentDetailId,
                        CartoonValue = subGroup.CartoonValue,
                        Content = subGroup.Content,
                        Height = subGroup.Height,
                        HSCode = subGroup.HSCode,
                        Length = subGroup.Length,
                        Value = subGroup.Value,
                        Width = subGroup.Width,
                        Weight = subGroup.Weight,
                        IsPrinted = string.IsNullOrEmpty(subGroup.HSCode) ? false : true
                    }).ToList()
                }).OrderByDescending(p => p.EstimatedDateOfDeparture)
                  .ThenByDescending(p => UtilityRepository.GetTimeFromString(p.EstimatedTimeOfDeparture).HasValue ? UtilityRepository.GetTimeFromString(p.EstimatedTimeOfDeparture).Value : DateTime.UtcNow.TimeOfDay).ToList();
                var data = list.Where(p => p.Packages.Count > 0).ToList();
                return data;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return list;
            }
        }
        public FrayteResult SetShipmentHSCode(int eCommerceShipmentDetailid, string hSCode, string Description, int HsCodeId)
        {

            FrayteResult result = new FrayteResult();
            try
            {
                if (eCommerceShipmentDetailid > 0)
                {
                    var data = dbContext.eCommerceShipmentDetails.Find(eCommerceShipmentDetailid);

                    if (data != null)
                    {
                        data.HSCode = hSCode;
                        //data.PiecesContent = Description;
                        dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                        result.Status = true;
                    }
                    else
                    {
                        result.Status = false;
                    }

                    var HscodeData = dbContext.HSCodes.Where(a => a.HSCodeId == HsCodeId).FirstOrDefault();

                    if (HscodeData != null)
                    {
                        HscodeData.Description = HscodeData.Description + ',' + data.PiecesContent;
                        //data.PiecesContent = Description;
                        dbContext.Entry(HscodeData).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                        //result.Status = true;
                    }
                    //else
                    //{
                    //    result.Status = false;
                    //}

                    try
                    {
                        var shipDetail = dbContext.eCommerceShipmentDetails
                                             .Where(p => p.eCommerceShipmentId == data.eCommerceShipmentId &&
                                             string.IsNullOrEmpty(p.HSCode)).ToList();
                        if (shipDetail == null || (shipDetail != null && shipDetail.Count == 0))
                        {
                            var ship = dbContext.eCommerceShipments.Find(data.eCommerceShipmentId);
                            if (ship != null)
                            {
                                ship.MappedOn = DateTime.UtcNow;
                                dbContext.Entry(ship).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
            }
            return result;
        }

        private void sendInvoiceMailToReceiver(int shipmentId)
        {

            new eCommerceShipmentRepository().GenerateInvoice(shipmentId);

        }

        public FrayteResult ReAssignJobs(OpeartorReAssignedJob job)
        {
            FrayteResult result = new FrayteResult();
            try
            {
                if (job != null && job.OperatorId > 0 && job.jobs != null && job.jobs.Count > 0)
                {

                    foreach (var data in job.jobs)
                    {
                        var ship = dbContext.eCommerceShipments.Find(data.ShipmentId);
                        if (ship != null)
                        {
                            ship.AssignedTo = job.OperatorId;
                            dbContext.SaveChanges();
                            result.Status = true;
                        }

                    }
                }
                else
                {
                    result.Status = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                result.Status = false;
                return result;
            }

        }

        #endregion

        #region Total HSCode Output/Operator/Hour
        public List<HSCodeAvgOutput> HSCodeOutputPerOperatorPerHour(int userId)
        {
            List<HSCodeAvgOutput> list = new List<HSCodeAvgOutput>();
            try
            {
                // get all operators of the manager

                var operators = (from r in dbContext.Users
                                 join u in dbContext.UserAdditionals on r.UserId equals u.UserId
                                 join t in dbContext.Timezones on r.TimezoneId equals t.TimezoneId
                                 where u.ManagerUserId == userId && r.IsActive == true
                                 select new
                                 {
                                     OperatorId = r.UserId,
                                     Name = r.ContactName,
                                     WST = r.WorkingStartTime,
                                     WET = r.WorkingEndTime,
                                     TimeZone = new TimeZoneModal
                                     {
                                         Name = t.Name,
                                         TimezoneId = t.TimezoneId,
                                         Offset = t.Offset,
                                         OffsetShort = t.OffsetShort
                                     }
                                 }
                                
                                ).ToList();

                // Get avg jobs per hour or each operator

                foreach (var data in operators)
                {
                    HSCodeAvgOutput jobOutput = new HSCodeAvgOutput();
                    var mycount = (from cnt in dbContext.eCommerceShipmentDetails
                                   join ec in dbContext.eCommerceShipments on cnt.eCommerceShipmentId equals ec.eCommerceShipmentId
                                   where !string.IsNullOrEmpty(cnt.HSCode) && ec.AssignedTo == data.OperatorId && ec.MappedOn != null
                                   group ec by ec.eCommerceShipmentId into g
                                   select new
                                   {
                                       name = g.Key,
                                       MappedOn = System.Data.Entity.DbFunctions.TruncateTime(g.FirstOrDefault().MappedOn.Value),
                                       count = g.Count()
                                   }
                                   ).ToList();

                    var count = mycount.GroupBy(x => x.MappedOn.Value.Date)
                        .Select(AS => new
                        {
                            name = AS.Key,
                            count = AS.Count()
                        }).ToList().Count;

                    var wst = UtilityRepository.GetFormattedTimeString(UtilityRepository.GetTimeZoneTime(data.WST, data.TimeZone.Name));
                    var wet = UtilityRepository.GetFormattedTimeString(UtilityRepository.GetTimeZoneTime(data.WET, data.TimeZone.Name));
                    DateTime dtFrom = DateTime.Parse(wst);
                    DateTime dtTo = DateTime.Parse(wet);
                    var timeDiffHrs = dtTo.Subtract(dtFrom).Hours;
                    var timeDiffMins = dtTo.Subtract(dtFrom).Minutes;
                    timeDiffHrs = timeDiffMins / 60 + timeDiffHrs;
                    var avgjobs = mycount.Count / timeDiffHrs * count;
                    jobOutput.AvgJobs = avgjobs;
                    jobOutput.Name = data.Name;
                    jobOutput.OperatorId = data.OperatorId;
                    list.Add(jobOutput);
                }

                int sum = 0;

                foreach (var data in list)
                {
                    sum += data.AvgJobs;
                }
                return list;
            }
            catch (Exception ex)
            {
                return list;
            }
        }

        public int HSCodeOutputPerPerHour(int userId)
        {
            List<HSCodeAvgOutput> list = new List<HSCodeAvgOutput>();
            try
            {
                // get all operators of the manager

                var operators = (from r in dbContext.Users
                                 join u in dbContext.UserAdditionals on r.UserId equals u.UserId
                                 join t in dbContext.Timezones on r.TimezoneId equals t.TimezoneId
                                 where u.ManagerUserId == userId && r.IsActive == true
                                 select new
                                 {
                                     OperatorId = r.UserId,
                                     Name = r.ContactName,
                                     WST = r.WorkingStartTime,
                                     WET = r.WorkingEndTime,
                                     TimeZone = new TimeZoneModal
                                     {
                                         Name = t.Name,
                                         TimezoneId = t.TimezoneId,
                                         Offset = t.Offset,
                                         OffsetShort = t.OffsetShort
                                     }
                                 }
                                ).ToList();

                // Get avg jobs per hour or each operator

                foreach (var data in operators)
                {
                    HSCodeAvgOutput jobOutput = new HSCodeAvgOutput();
                    var mycount = (from cnt in dbContext.eCommerceShipmentDetails
                                   join ec in dbContext.eCommerceShipments on cnt.eCommerceShipmentId equals ec.eCommerceShipmentId
                                   where !string.IsNullOrEmpty(cnt.HSCode) && ec.AssignedTo == data.OperatorId && ec.MappedOn != null
                                   group ec by ec.eCommerceShipmentId into g
                                   select new
                                   {
                                       name = g.Key,
                                       MappedOn = System.Data.Entity.DbFunctions.TruncateTime(g.FirstOrDefault().MappedOn.Value),
                                       count = g.Count()
                                   }
                                   ).ToList();

                    var count = mycount.GroupBy(x => x.MappedOn.Value.Date)
                        .Select(AS => new
                        {
                            name = AS.Key,
                            count = AS.Count()
                        }).ToList().Count;

                    var wst = UtilityRepository.GetFormattedTimeString(UtilityRepository.GetTimeZoneTime(data.WST, data.TimeZone.Name));
                    var wet = UtilityRepository.GetFormattedTimeString(UtilityRepository.GetTimeZoneTime(data.WET, data.TimeZone.Name));
                    DateTime dtFrom = DateTime.Parse(wst);
                    DateTime dtTo = DateTime.Parse(wet);
                    var timeDiffHrs = dtTo.Subtract(dtFrom).Hours;
                    var timeDiffMins = dtTo.Subtract(dtFrom).Minutes;
                    timeDiffHrs = timeDiffMins / 60 + timeDiffHrs;
                    var avgjobs = mycount.Count / timeDiffHrs * count;
                    jobOutput.AvgJobs = avgjobs;
                    jobOutput.Name = data.Name;
                    jobOutput.OperatorId = data.OperatorId;
                    list.Add(jobOutput);
                }

                int sum = 0;

                foreach (var data in list)
                {
                    sum += data.AvgJobs;
                }
                if (list.Count > 0)
                    sum = sum / list.Count;
                else
                    sum = 0;
                return sum;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        #endregion

        #region Common

        public List<OperatorWithJobs> GetOperatorsWithJobs(int userId)
        {
            List<OperatorWithJobs> list = new List<OperatorWithJobs>();
            try
            {
                var operatorList = (from r in dbContext.UserAdditionals
                                    join u in dbContext.Users on r.UserId equals u.UserId
                                    where r.ManagerUserId == userId && u.IsActive == true
                                    select new
                                    {
                                        UserId = u.UserId,
                                        Name = u.ContactName,
                                        CompanyName = u.CompanyName,
                                        Email = u.Email
                                    }
                               ).ToList();

                if (operatorList != null)
                {
                    foreach (var data in operatorList)
                    {
                        OperatorWithJobs operatorJob = new OperatorWithJobs();
                        operatorJob.UserId = data.UserId;
                        operatorJob.Name = data.Name;
                        var jobs = dbContext.eCommerceShipments.Where(p => p.AssignedTo == data.UserId  // &&  (p.CustomManifestId == null || p.CustomManifestId == 0) 
                        && p.ShipmentStatusId == (int)FrayteShipmentStatus.eCCurrent).ToList();
                        if (jobs != null && jobs.Count > 0)
                        {
                            operatorJob.JobsAssigned = jobs.Count;
                        }
                        else
                        {
                            operatorJob.JobsAssigned = 0;
                        }
                        list.Add(operatorJob);
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return list;
            }

        }

        public List<MangerOperator> GetMangerOperators(int userId)
        {
            List<MangerOperator> list = new List<MangerOperator>();
            try
            {
                list = (from r in dbContext.UserAdditionals
                        join u in dbContext.Users on r.UserId equals u.UserId
                        where r.ManagerUserId == userId && u.IsActive == true
                        select new MangerOperator
                        {
                            UserId = u.UserId,
                            Name = u.ContactName,
                            CompanyName = u.CompanyName,
                            Email = u.Email
                        }).ToList();
                return list;

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return list;
            }
        }

        public JobDetail GetJobsDetails(int userId)
        {
            JobDetail job = new JobDetail();
            try
            {
                var OperationZone = UtilityRepository.GetOperationZone();

                //Total Jobs with no hscode 
                var data = (from r in dbContext.eCommerceShipments
                            join ed in dbContext.eCommerceShipmentDetails on r.eCommerceShipmentId equals ed.eCommerceShipmentId
                            where string.IsNullOrEmpty(ed.HSCode) //&& (r.CustomManifestId == null || r.CustomManifestId == 0)
                             && (r.AssignedTo == null || r.AssignedTo == 0) && r.OpearionZoneId == OperationZone.OperationZoneId
                             && r.ShipmentStatusId == (int)FrayteShipmentStatus.eCCurrent
                            group r by r.eCommerceShipmentId into g
                            select g.FirstOrDefault()
                         ).ToList();


                //Total Jobs
                var data3 = dbContext.eCommerceShipments.Where(p => //(p.CustomManifestId == null || p.CustomManifestId == 0) &&
                    p.ShipmentStatusId == (int)FrayteShipmentStatus.eCCurrent &&
                     p.OpearionZoneId == OperationZone.OperationZoneId).ToList();


                // Jobs In Progress

                var mycount1 = (from cnt in dbContext.eCommerceShipmentDetails
                                join ec in dbContext.eCommerceShipments on cnt.eCommerceShipmentId equals ec.eCommerceShipmentId
                                where ec.OpearionZoneId == OperationZone.OperationZoneId &&
                                ec.ShipmentStatusId == (int)FrayteShipmentStatus.eCCurrent // &&   (ec.CustomManifestId == null || ec.CustomManifestId == 0)
                                && ec.AssignedTo != null
                                group cnt by cnt.eCommerceShipmentId into g
                                select new
                                {
                                    name = g.Key,
                                    HSCode = g.Min(r => string.IsNullOrEmpty(r.HSCode) ? "" : r.HSCode),
                                    count = g.Count()
                                }).ToList().Where(p => string.IsNullOrEmpty(p.HSCode)).ToList();


                var data1 = dbContext.eCommerceShipments.Where(p => p.AssignedTo != null // &&    (p.CustomManifestId == null || p.CustomManifestId == 0) &&
                  && p.ShipmentStatusId == (int)FrayteShipmentStatus.eCCurrent &&
                p.OpearionZoneId == OperationZone.OperationZoneId).ToList();

                // Manager's Opeerator
                var data2 = GetMangerOperators(userId);


                // AvgJobs Per Hour
                int avg = new HsCodeJobRepository().HSCodeOutputPerPerHour(userId);

                job.AvgJobsPerHour = avg;
                if (data != null && data.Count > 0)
                {
                    job.ToltalUnAssignedJobs = data.Count;
                }
                else
                {
                    job.ToltalUnAssignedJobs = 0;
                }
                if (data3 != null && data3.Count > 0)
                {
                    job.ToltalJobs = data3.Count;
                }
                else
                {
                    job.ToltalJobs = 0;
                }
                if (mycount1 != null && mycount1.Count > 0)
                {
                    job.JobsInProgress = mycount1.Count;

                }
                else
                {
                    job.JobsInProgress = 0;
                }
                if (data2 != null && data2.Count > 0)
                {
                    job.ToltalOperators = data2.Count;
                }
                else
                {
                    job.ToltalOperators = 0;
                }

                return job;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return job;
            }

        }

        #endregion
    }
}
