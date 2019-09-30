using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.Tradelane
{
    public class TradelaneEmailModel
    {
        public TradelaneBooking ShipmentDetail { get; set; }

        public int TotalCarton { get; set; } //E1
        public decimal TotalWeight { get; set; } //E1
        public string WeightUnit { get; set; } //E1
        public string DimensionUnit { get; set; } //E1

        public TimeZoneInfo TimeZoneInfo { get; set; }
        public TimeZoneModal UserTimeZone { get; set; }

        public string CustomerName { get; set; } //E1 //E4
        public string CustomerEmail { get; set; }//E1 // E4
        public string CustomerCompanyName { get; set; }//E1
        public string CompanyName { get; set; }//E1
        public string Name { get; set; }//E1
        public string StaffUserName { get; set; } //E1
        public string StaffUserEmail { get; set; }//E1


        public string UserName { get; set; } // E2.1
        public string UserPosition { get; set; }
        public string UserEmail { get; set; }// E2.1
        public string SiteAddress { get; set; }// E2.1
        public string UserPhone { get; set; }// E2.1
        public string ImageHeader { get; set; }

        public string ShipmentCurrentStatus { get; set; } //E6
        public int PackageCount { get; set; } //E6
        public string ShipmentDescription { get; set; } //E6
        public string CreatedOnDate { get; set; } //E6
        public string CreatedOnTime { get; set; } //E6
        public string  TimeZoneOffset { get; set; } //E6
        public AgentConsolidateMailModel ConsolidateShipments { get; set; } //E4.1
        public string SystemEmail { get; set; } // E4.1
        public string MAWBDisplay { get; set; }
        public string Site { get; set; }

        public string TO { get; set; }
        public string CC { get; set; }
        public string BCC { get; set; }
        public string ReplyTo { get; set; }
    }
}