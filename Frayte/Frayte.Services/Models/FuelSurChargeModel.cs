using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public class FuelSurChargeModel
    {
        public int FuelSurchargeId { get; set; }
        public OperationZoneModel OperationZone { get; set; }
        public string FrayteFuelPercent { get; set; }
        public string ExpryeFuelPercent { get; set; }
        public string DomesticFuelPercent { get; set; }
        public string RoadFuelPercent { get; set; }
        public DateTime FuelMonthYear { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public bool? IsUpdated { get; set; }
    }

    public class FrayteFuelSurcharge
    {
        public string LogisticCompany { get; set; }
        public int UpdatedBy { get; set; }
        public List<FrayteLogistictype> Type { get; set; }
    }

    public class FrayteLogistictype
    {
        public string LogisticType { get; set; }
        public string RateType { get; set; }
        public List<FrayteFuelMonthYear> MonthYear { get; set; }
    }

    public class FrayteFuelMonthYear
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public string RateType { get; set; }
        public string LogsiticType { get; set; }
        public int FuelSurchargeId { get; set; }
        public decimal FrayteFuelPercent { get; set; }
        public bool IsChange { get; set; }
    }

    public class FrayteFuelSurChargeList
    {
        public string LogisticCompany { get; set; }
        public List<FuelSurChargeModel> FuelSurCharge { get; set; }
    }

    public class FrayteFuelSurChargeSaveModel
    {
        public int UserId { get; set; }
        public int OperationZoneId { get; set; }
        public DateTime Year { get; set; }
        public string LogisticCompany { get; set; }
    }

    public class LogisticCompanyList
    {
        public int LogisticServiceId { get; set; }
        public string LogisticCompany { get; set; }
        public string LogisticCompanyDisplay { get; set; }

    }

    public class FrayteMonthYear
    {
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
