using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.BreakBulk
{
    public class FrayteIncoterm
    {
        public int IncotermID { get; set; }
        public string ShipmentType { get; set; }
        public string IncotermCode { get; set; }
        public string IncotermDisplayValue { get; set; }
        public string IncotermValue { get; set; }
    }

    public class CustomFieldModel
    {
        public int CustomFieldId { get; set; }
        public string CustomFieldType { get; set; }
        public string CustomFieldName { get; set; }
        public string CustomFieldValue { get; set; }
        public string ModuleType { get; set; }
    }

    public class BreakBulkServiceModel
    {
        public int FromCountryId { get; set; }
        public string FromPostCode { get; set; }
        public string FromState { get; set; }
        public int ToCountryId { get; set; }
        public string ToPostCode { get; set; }
        public string ToState { get; set; }
        public decimal TotalWeight { get; set; }
        public string WeightUnit { get; set; }
        public int CustomerId { get; set; }
    }

    public class HubService
    {
        public int HubCarrierId { get; set; }
        public int HubCarrierServiceId { get; set; }
        public string HubCarrier { get; set; }
        public string LogisticServiceType { get; set; }
        public string HubCarrierDisplay { get; set; }
        public string CourierAccountNo { get; set; }
        public string RateType { get; set; }
        public string RateTypeDisplay { get; set; }
        public decimal BillingWeight { get; set; }
        public decimal ActualWeight { get; set; }
        public string WeightRoundLogic { get; set; }
        public string CarrierLogo { get; set; }
        public string TransitTime { get; set; }
        public string NetworkCode { get; set; }
    }
}
