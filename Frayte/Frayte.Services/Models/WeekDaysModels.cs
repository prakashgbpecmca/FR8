using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public class FrayteWeekDays
    {
        public int WorkingWeekDayId { get; set; }
        public string Description { get; set; }
        public bool IsDefault { get; set; }
        public List<FrayteWorkingWeekDayDetail> WorkingWeekDetails { get; set; }
    }

    public class FrayteWorkingWeekDayDetail
    {
        public int WorkingWeekDayDetailId { get; set; }
        public int WorkingWeekDayId { get; set; }
        public int DayId { get; set; }
        public string DayName { get; set; }
        public string DayHalfTime { get; set; }
    }
}
