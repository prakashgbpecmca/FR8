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
    
    public partial class UserTrackingConfiguration
    {
        public int UserTrackingConfigurationId { get; set; }
        public int UserId { get; set; }
        public string DeliveredEmails { get; set; }
        public string InTransitEmails { get; set; }
        public string OutForDeliveryEmails { get; set; }
        public string PendingEmails { get; set; }
        public string AttemptFailEmails { get; set; }
        public string InfoReceivedEmails { get; set; }
        public string ExceptionEmails { get; set; }
        public string ExpiredEmails { get; set; }
    }
}
