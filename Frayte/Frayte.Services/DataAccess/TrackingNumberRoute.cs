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
    
    public partial class TrackingNumberRoute
    {
        public int TrackingNumberLoggingId { get; set; }
        public string Number { get; set; }
        public int ShipmentId { get; set; }
        public string ModuleType { get; set; }
        public Nullable<bool> IsTrackingNumber { get; set; }
        public Nullable<bool> IsFrayteNumber { get; set; }
        public Nullable<bool> IsTradelaneRefNumber { get; set; }
        public Nullable<bool> IsAWB { get; set; }
        public Nullable<bool> IsMAWB { get; set; }
        public Nullable<bool> IsHAWB { get; set; }
        public Nullable<bool> IsPiecesTrackingNo { get; set; }
        public Nullable<bool> IsBag { get; set; }
        public Nullable<bool> IsExpressManifestNumber { get; set; }
    }
}
