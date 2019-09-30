using Frayte.Services.Business;
using Frayte.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Frayte.WebApi.Controllers
{
    [Authorize]
    public class WeekDaysController : ApiController
    {
        [HttpGet]
        public FrayteWeekDays GetWeekDays(int weekDayId)
        {
            var weekDays1 = new WeekDaysRepository().GetWeekDays(weekDayId);

            return weekDays1;
        }
        [HttpGet]
        public List<FrayteWeekDays> GetWeekDaysList()
        {
            var weekDaysList = new WeekDaysRepository().GetWeekDaysList();

            return weekDaysList;
        }
        [HttpPost]
        public IHttpActionResult SaveWeekDay(FrayteWeekDays frayteWeekDay)
        {
            FrayteResult result = new WeekDaysRepository().SaveWeekDay(frayteWeekDay);

            if (result != null && result.Status)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }

        }

        [HttpGet]
        public IHttpActionResult DeleteWorkingWeekDay(int weekDayId)
        {
            FrayteResult result = new WeekDaysRepository().DeleteWorkingWeekDay(weekDayId);

            if (result != null && result.Status)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }
    }
}
