using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Business
{
    public class WeekDaysRepository
    {
        FrayteEntities dbContext = new FrayteEntities();
        public FrayteWeekDays GetWeekDays(int weekDayId)
        {
            FrayteWeekDays weekDays = new FrayteWeekDays();
            //step 1 Get Working Week Day
            var weekDay = dbContext.WorkingWeekDays.Where(p => p.WorkingWeekDayId == weekDayId).FirstOrDefault();
            if (weekDay != null)
            {
                weekDays.WorkingWeekDayId = weekDay.WorkingWeekDayId;
                weekDays.Description = weekDay.Description;
                weekDay.IsDefault = weekDay.IsDefault;
            }


            //step 2 Get Working Week Day Detals
            weekDays.WorkingWeekDetails = new List<FrayteWorkingWeekDayDetail>();
            var WorkingWeekDetail = dbContext.WorkingWeekDayDetails.Where(p => p.WorkingWeekDayId == weekDay.WorkingWeekDayId).ToList();

            if (WorkingWeekDetail != null)
            {
                foreach (WorkingWeekDayDetail detail in WorkingWeekDetail)
                {
                    FrayteWorkingWeekDayDetail newWorkingDetial = new FrayteWorkingWeekDayDetail();
                    newWorkingDetial.WorkingWeekDayDetailId = detail.WorkingWeekDayDetailId;
                    newWorkingDetial.WorkingWeekDayId = detail.WorkingWeekDayId;
                    newWorkingDetial.DayId = detail.DayId;
                    newWorkingDetial.DayName = detail.DayName;
                    newWorkingDetial.DayHalfTime = detail.DayHalfTime;
                    weekDays.WorkingWeekDetails.Add(newWorkingDetial);
                }

            }
            return weekDays;
        }

        public List<FrayteWeekDays> GetWeekDaysList()
        {
            var list = dbContext.WorkingWeekDays.ToList();
            var weekDaylist = new List<FrayteWeekDays>();
            foreach (WorkingWeekDay bee in list)
            {
                FrayteWeekDays newWeek = new FrayteWeekDays();
                newWeek.WorkingWeekDayId = bee.WorkingWeekDayId;
                newWeek.Description = bee.Description;
                newWeek.IsDefault = bee.IsDefault;

                //step 2 Get Working Week Day Detals
                newWeek.WorkingWeekDetails = new List<FrayteWorkingWeekDayDetail>();
                var NewWorkingWeekDetail = dbContext.WorkingWeekDayDetails.Where(p => p.WorkingWeekDayId == newWeek.WorkingWeekDayId).ToList();

                if (NewWorkingWeekDetail != null)
                {
                    foreach (WorkingWeekDayDetail detail in NewWorkingWeekDetail)
                    {
                        FrayteWorkingWeekDayDetail newWorkingDetial = new FrayteWorkingWeekDayDetail();
                        newWorkingDetial.WorkingWeekDayDetailId = detail.WorkingWeekDayDetailId;
                        newWorkingDetial.WorkingWeekDayId = detail.WorkingWeekDayId;
                        newWorkingDetial.DayId = detail.DayId;
                        newWorkingDetial.DayName = detail.DayName;
                        newWorkingDetial.DayHalfTime = detail.DayHalfTime;
                        newWeek.WorkingWeekDetails.Add(newWorkingDetial);
                    }

                }

                weekDaylist.Add(newWeek);
            }

            return weekDaylist;
        }

        public List<WorkingWeekDay> GetWeekDaysDataList()
        {
            FrayteEntities dbContext = new FrayteEntities();
            var list = dbContext.WorkingWeekDays.ToList();
            return list;
        }

        public FrayteResult SaveWeekDay(FrayteWeekDays frayteWeekDay)
        {
            FrayteResult result = new FrayteResult();
            FrayteWeekDays returnFrayteWeekDays = new FrayteWeekDays();
            // Step 1 Save Working Week day 
            WorkingWeekDay(frayteWeekDay);


            // Step21 Save Working WeekDay Details            
            WorkingWeekDayDetails(frayteWeekDay.WorkingWeekDayId, frayteWeekDay.WorkingWeekDetails);

            result.Status = true;

            return result;

        }

        private void WorkingWeekDayDetails(int workingId, List<FrayteWorkingWeekDayDetail> frayteWorkingWeekDetail)
        {
            foreach (FrayteWorkingWeekDayDetail frayWeekDetail in frayteWorkingWeekDetail)
            {

                WorkingWeekDayDetail weekDetail = new WorkingWeekDayDetail();
                if (frayWeekDetail.WorkingWeekDayDetailId == 0)
                {

                    weekDetail.WorkingWeekDayId = workingId;
                    weekDetail.DayId = frayWeekDetail.DayId;
                    weekDetail.DayName = frayWeekDetail.DayName;
                    weekDetail.DayHalfTime = frayWeekDetail.DayHalfTime;

                    dbContext.WorkingWeekDayDetails.Add(weekDetail);
                }
                else
                {
                    weekDetail = dbContext.WorkingWeekDayDetails.Where(p => p.WorkingWeekDayDetailId == frayWeekDetail.WorkingWeekDayDetailId).FirstOrDefault();
                    if (weekDetail != null)
                    {
                        weekDetail.WorkingWeekDayId = workingId;
                        weekDetail.DayId = frayWeekDetail.DayId;
                        weekDetail.DayName = frayWeekDetail.DayName;
                        weekDetail.DayHalfTime = frayWeekDetail.DayHalfTime;

                    }

                }

                if (weekDetail != null)
                {
                    dbContext.SaveChanges();
                }

                frayWeekDetail.WorkingWeekDayDetailId = frayWeekDetail.WorkingWeekDayDetailId;
            }
        }

        private void WorkingWeekDay(FrayteWeekDays frayteWeekDay)
        {
            WorkingWeekDay weekday;
            if (frayteWeekDay.WorkingWeekDayId > 0)
            {

                weekday = dbContext.WorkingWeekDays.Where(p => p.WorkingWeekDayId == frayteWeekDay.WorkingWeekDayId).FirstOrDefault();
                if (weekday != null)
                {
                    weekday.Description = frayteWeekDay.Description;
                    if (frayteWeekDay.IsDefault)
                    {

                        var weekDays = dbContext.WorkingWeekDays.Where(p => p.IsDefault == true).ToList();
                        foreach (var data in weekDays)
                        {
                            data.IsDefault = false;
                            dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }
                    }
                    weekday.IsDefault = frayteWeekDay.IsDefault;
                    dbContext.SaveChanges();
                    frayteWeekDay.WorkingWeekDayId = weekday.WorkingWeekDayId;

                }
            }
            else
            {
                weekday = new WorkingWeekDay();
                weekday.Description = frayteWeekDay.Description;
                if (frayteWeekDay.IsDefault)
                {

                    var weekDays = dbContext.WorkingWeekDays.Where(p => p.IsDefault == true).ToList();
                    foreach (var data in weekDays)
                    {
                        data.IsDefault = false;
                        dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                }
                weekday.IsDefault = frayteWeekDay.IsDefault;
                dbContext.WorkingWeekDays.Add(weekday);
                if (weekday != null)
                {
                    dbContext.SaveChanges();
                }
                frayteWeekDay.WorkingWeekDayId = weekday.WorkingWeekDayId;
            }
        }

        public FrayteResult DeleteWorkingWeekDay(int weekDayId)
        {
            FrayteResult result = new FrayteResult();

            //Step 1: Remove Working Week Details
            var workingWeekDetails = dbContext.WorkingWeekDayDetails.Where(p => p.WorkingWeekDayId == weekDayId).ToList();
            if (workingWeekDetails != null && workingWeekDetails.Count > 0)
            {
                foreach (WorkingWeekDayDetail weekDayDetail in workingWeekDetails)
                {
                    //dbContext.WorkingWeekDayDetails.Add(weekDayDetail);
                    dbContext.WorkingWeekDayDetails.Remove(weekDayDetail);
                    dbContext.SaveChanges();
                }
            }

            //Step 2: Remove Working Week
            var workingWeek = dbContext.WorkingWeekDays.Find(weekDayId);
            if (workingWeek != null)
            {
                dbContext.WorkingWeekDays.Remove(workingWeek);
                dbContext.SaveChanges();
            }

            result.Status = true;

            return result;
        }
    }

}
