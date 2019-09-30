using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public class TradelaneGetShipmentModel
    {
        public int TradelaneShipmentId { get; set; }
        public int CustomerId { get; set; }
        public string Status { get; set; }
        public string StatusDisplay { get; set; }
        public string Customer { get; set; }
        public string MAWB { get; set; }
        public string FrayteRefNo { get; set; }
        public string ShipperCompanyName { get; set; }
        public string ConsigneeCompanyName { get; set; }
        public int MAWBAgentId { get; set; }
        public bool IsAgentMAWBAllocated { get; set; }
        public int TotalRows { get; set; }
        public DateTime CreatedOn { get; set; }
        public string MAWBURL { get; set; }
        public string LegNum { get; set; }
        public int ShipmentMethodHandlerId { get; set; }

        public decimal TotalWeight { get; set; }
        public decimal TotalCarton { get; set; }
        public string DestinationAirport { get; set; }
        public string DepartureAirport { get; set; }

    }

    public class TradelaneTrackDirectBooking
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int ShipmentStatusId { get; set; }
        [StringLength(10)]
        public string FrayteNumber { get; set; }
        public string MAWB { get; set; }
        public int CurrentPage { get; set; }
        public int TakeRows { get; set; }
        public int CustomerId { get; set; }
        public int UserId { get; set; }
        public int SpecialSearchId { get; set; }
    }

    public class MAWBCustomizedFiled
    {
        public int MAWBCustomizedeFieldId { get; set; }
        public int TradelaneShipmentId { get; set; }
        public string IssuingCarriersAgentNameandCity { get; set; }
        public string DeclaredValueForCarriage { get; set; }
        public string DeclaredValueForCustoms { get; set; }
        public string ValuationCharge { get; set; }
        public string Tax { get; set; }
        public string TotalOtherChargesDueAgent { get; set; }
        public string TotalOtherChargesDueCarrier { get; set; }
        public string OtherCharges { get; set; }
        public string ChargesAtDestination { get; set; }
        public string TotalCollectCharges { get; set; }
        public string CurrencyConversionRates { get; set; }
        public string TotalPrepaid { get; set; }
        public string TotalCollect { get; set; }
        public string HandlingInformation { get; set; }
        public string AgentsIATACode { get; set; }
        public string AccountNo { get; set; }
    }
}
