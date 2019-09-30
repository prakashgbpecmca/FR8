using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frayte.Services.Models.ApiModal
{
    public class FrayteRateResponse
    {
        public bool Status { get; set; }
        public string Description { get; set; }
        public string ShipmentId { get; set; }
        public List<FrayteRateCard> RateCard { get; set; }
        public List<FrayteApiError> Errors { get; set; }
    }    

    public class FrayteRateCard
    {
        public string RateId { get; set; }
        public string RateType { get; set; }
        public string RateServiceType { get; set; }
        public string RateCalculationId { get; set; }
        public string RateTypeCalculation { get; set; }
        public string FuelCalcuatedOn { get; set; }
        public decimal TotalCost { get; set; }
        public string RateCurrencyCode { get; set; }
        public string CourierAccountId { get; set; }
        public string CourierName { get; set; }
        public string CourierDescription { get; set; }
        public string CourierCountryCode { get; set; }
    }

    public class IntegrationRequest
    {
        public Security Security { get; set; }
        public string ShipmentId { get; set; }
        public string RateCardId { get; set; }
    }
}