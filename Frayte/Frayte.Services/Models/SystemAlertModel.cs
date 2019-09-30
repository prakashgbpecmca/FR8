using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public class FrayteSystemAlert
    {
        public int SystemAlertId { get; set; }
        public string Heading { get; set; }
        public string Description { get; set; }
        public DateTime FromDate { get; set; }
        public string FromTime { get; set; }
        public DateTime ToDate { get; set; }
        public string ToTime { get; set; }
        public TimeZoneModal TimeZoneDetail { get; set; }
        public bool IsActive { get; set; }
        public int OperationZoneId { get; set; }
        public int TotalRows { get; set; }
    }

    public class TrackSystemAlert
    {
        public int OperationZoneId { get; set; }
        public DateTime? FromDate { get; set; }        
        public DateTime? ToDate { get; set; }
        public int CurrentPage { get; set; }
        public int TakeRows { get; set; }

    }
}
