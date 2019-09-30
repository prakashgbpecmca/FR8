using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.Bring
{
    public class BringResponseModel
    {
        public List<Consignments> consignments { get; set; }
        public List<Error> errors { get; set; }
        public string request { get; set; }
        public string response { get; set; }
    }
    public class Consignments
    {
        public string correlationId { get; set; }
        public Confirmation confirmation { get; set; }
        public object errors { get; set; }
    }
    public class Links
    {
        public string labels { get; set; }
        public object waybill { get; set; }
        public string tracking { get; set; }
    }

    public class DateAndTimes
    {
        public object earliestPickup { get; set; }
        public long expectedDelivery { get; set; }
    }

    public class Packages
    {
        public string correlationId { get; set; }
        public string packageNumber { get; set; }
    }

    public class Confirmation
    {
        public string consignmentNumber { get; set; }
        public object productSpecificData { get; set; }
        public Links links { get; set; }
        public DateAndTimes dateAndTimes { get; set; }
        public List<Packages> packages { get; set; }
    }

    public class Message
    {
        public string lang { get; set; }
        public string message { get; set; }
    }

    public class Error
    {
        public string uniqueId { get; set; }
        public string code { get; set; }
        public List<Message> messages { get; set; }
        public object consignmentCorrelationId { get; set; }
        public object packageCorrelationId { get; set; }
    }
}
