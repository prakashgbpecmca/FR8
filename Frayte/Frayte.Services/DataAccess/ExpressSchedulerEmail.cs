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
    
    public partial class ExpressSchedulerEmail
    {
        public int ExpressSchedulerId { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public Nullable<System.DateTime> EmailSentOn { get; set; }
        public string ErrorObject { get; set; }
        public string EmailContent { get; set; }
    }
}
