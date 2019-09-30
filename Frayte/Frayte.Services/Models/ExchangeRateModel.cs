using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public class FrayteExchangeRate
    {
        public int OperationZoneExchangeRateId { get; set; }
        public FrayteOperationZone OperationZone { get; set; }
        public FrayteCurrencyDetail CurrencyDetail { get; set; }
        public decimal ExchangeRate { get; set; }
        public string ExchangeType { get; set; }
        public bool IsActive { get; set; }
    }

    public class FrayteCurrencyDetail
    {
        public string CurrencyCode { get; set; }
        public string Description { get; set; }
    }

    public class FrayteExchangeRateHistory
    {
        public FrayteOperationZone OperationZone { get; set; }
        public FrayteCurrencyDetail CurrencyDetail { get; set; }
        public string ExchangeRate { get; set; }
        public string ExchangeType { get; set; }        
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
    }

    public class FrayteExchangeMonth
    {
        public int MonthId { get; set; }
        public string MonthName { get; set; }
    }

    public class FrayteSearchExchangeHistory
    {
        public int OperationZoneId { get; set; }
        public string ExchangeType { get; set; }
        public int Year { get; set; }
        public FrayteExchangeMonth MonthName { get; set; }
    }
}
