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
    
    public partial class LogisticServiceShipmentType
    {
        public int LogisticServiceShipmentTypeId { get; set; }
        public int LogisticServiceId { get; set; }
        public string LogisticType { get; set; }
        public string LogisticDescription { get; set; }
        public string LogisticDescriptionDisplayType { get; set; }
        public string LogisticDescriptionReportDisplayType { get; set; }
        public string DocNondocType { get; set; }
        public string LogisticServiceShipmentCode { get; set; }
        public string NetworkCode { get; set; }
    }
}
