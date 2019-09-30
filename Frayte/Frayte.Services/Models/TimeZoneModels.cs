using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public class FrayteTimeZone : FrayteSession
    {
        public int TimezoneId { get; set; }
        public string Name { get; set; }
        public string Offset { get; set; }
        public string OffsetShort { get; set; }
    }

    public class TimeZoneModal
    {
        public int TimezoneId { get; set; }
        public string Name { get; set; }
        public string Offset { get; set; }
        public string OffsetShort { get; set; }
    }
}
