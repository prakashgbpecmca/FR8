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
    
    public partial class OperationZone
    {
        public int OperationZoneId { get; set; }
        public string Name { get; set; }
        public string Currency { get; set; }
        public Nullable<int> OverSize { get; set; }
        public Nullable<int> OverWeight { get; set; }
        public Nullable<int> IATAOverSize { get; set; }
        public Nullable<int> IATAOverWeight { get; set; }
        public bool IsManifestSupport { get; set; }
    }
}
