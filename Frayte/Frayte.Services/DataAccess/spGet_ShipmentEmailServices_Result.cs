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
    
    public partial class spGet_ShipmentEmailServices_Result
    {
        public Nullable<System.Guid> ShipmentEmailServiceId { get; set; }
        public Nullable<int> ShipmentId { get; set; }
        public Nullable<System.DateTime> EmailSentDate { get; set; }
        public Nullable<System.TimeSpan> EmailSentTime { get; set; }
        public Nullable<int> ReminderIn { get; set; }
        public string RemindType { get; set; }
        public Nullable<bool> IsRepeatReminder { get; set; }
        public Nullable<int> EmailSentCount { get; set; }
        public string EmailTemplateId { get; set; }
        public Nullable<int> EmailSentToManagerCount { get; set; }
        public string ManagerEmailTemplateId { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<int> WarehouseId { get; set; }
        public Nullable<bool> IsPublicHoliday { get; set; }
        public string TimeZoneName { get; set; }
        public Nullable<System.TimeSpan> WorkingStartTime { get; set; }
        public Nullable<System.TimeSpan> WorkingEndTime { get; set; }
    }
}
