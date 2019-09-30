using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public class FrayteCustomerSpecificAdminCharges
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public List<AdminChargesTypes> Charges { get; set; }

    }

    public class AdminChargesTypes
    {
        public int AdminChargeId { get; set; }
        public int CreatedBy { get; set; }
        public string ChargeType { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CurrencyCode { get; set; }
    }


}
