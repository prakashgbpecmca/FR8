//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Frayte.Services.DataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class LogisticService
    {
        public int LogisticServiceId { get; set; }
        public int OperationZoneId { get; set; }
        public string LogisticCompany { get; set; }
        public string LogisticCompanyDisplay { get; set; }
        public string LogisticType { get; set; }
        public string LogisticTypeDisplay { get; set; }
        public string RateType { get; set; }
        public string RateTypeDisplay { get; set; }
        public Nullable<decimal> WeightRoundOff { get; set; }
        public Nullable<decimal> MaxWeightLimit { get; set; }
        public string ModuleType { get; set; }
        public bool IsActive { get; set; }
        public bool IsFuelSurchargeCalculate { get; set; }
        public string SupplemantoryFileName { get; set; }
        public Nullable<decimal> ByPoint5KgRound { get; set; }
        public Nullable<decimal> By1KgRound { get; set; }
        public Nullable<int> MarginOrder { get; set; }
        public Nullable<System.DateTime> IssuedDate { get; set; }
        public Nullable<System.DateTime> ExpiryDate { get; set; }
        public int AddOnTransitTime { get; set; }
        public Nullable<int> KgToCmsCalculationValue { get; set; }
        public Nullable<int> LbToInchCalculationValue { get; set; }
        public string CountryCode { get; set; }
    }
}
