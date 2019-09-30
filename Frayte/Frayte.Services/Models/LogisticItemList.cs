using Frayte.Services.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public class FrayteLogisticServices
    {
        public int LogisticServiceId { get; set; }
        public int OperationZoneId { get; set; }
        public string LogisticCompany { get; set; }
        public string LogisticCompanyDisplay { get; set; }
        public string LogisticType { get; set; }
        public string LogisticTypeDisplay { get; set; }
        public string RateType { get; set; }
        public string RateTypeDisplay { get; set; } 
        public string ModuleType { get; set; }
    }

    public class FrayteRateCardLogisticServices : FrayteLogisticServices
    {
        public string DocType { get; set; }
        public string DocTypeDisplay { get; set; }
        public string ParcelType { get; set; }
        public string ParcelTypeDisplay { get; set; }
        public string PackageType { get; set; }
        public string PackageTypeDisplay { get; set; }
        public string ServiceType { get; set; }
        public string ServiceTypeDisplay { get; set; }
        public string AddressType { get; set; }
        public string AddressTypeDisplay { get; set; }
        public string PODType { get; set; }
        public string PODTypeDisplay { get; set; }

    }
    public class LogisticItemList
    {
        public string ModuleType { get; set; }
        public List<LogisticServiceItem> LogisticTypes { get; set; }
        public List<LogisticServiceItem> LogisticCompanies { get; set; }
        public List<LogisticServiceItem> LogisticRateTypes { get; set; }
        public List<LogisticServiceItem> DocTypes { get; set; }
    }

    public class LogisticServiceItem
    {
        public string Value { get; set; }
        public string Display { get; set; }
    }

    public class FrayteLogisticServiceItem
    {
        public int LogisticServiceId { get; set; }
        public int OperationZoneId { get; set; }
        public string LogisticCompany { get; set; }
        public string LogisticCompanyDisplay { get; set; }
        public string LogisticType { get; set; }
        public string LogisticTypeDisplay { get; set; }
        public string RateType { get; set; }
        public string RateTypeDisplay { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string ModuleType { get; set; }
        public bool IsExported { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}
