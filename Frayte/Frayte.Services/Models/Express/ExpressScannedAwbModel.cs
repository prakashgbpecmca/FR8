using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.Express
{
    public class ExpressScannedAwbModel
    {
        public int AWBId { get; set; }
        public int customerId { get; set; }
        public string AWBNumber { get; set; }
        public string DocumentName { get; set; }
        public int TotalAWB { get; set; }
        public int CurrentPageAwb { get; set; }
        public string Document { get; set; }
        public string CustomerName { get; set; }
        public int ScannedBy { get; set; }
        public DateTime ScannedOn { get; set; }
        public string ScannedOnTime { get; set; }
        public int TotalRows { get; set; }

    }

    public class ExpressCustomerModel
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string customerAccNo { get; set; }
    }

    public class ExpressAWBTrackModel
    {
        public int CurrentPage { get; set; }
        public int TakeRows { get; set; }
        public int CustomerId { get; set; }
        public string AWBNumber { get; set; }
    }

    public class ExpressFile
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public bool Status { get; set; }
    }
}
