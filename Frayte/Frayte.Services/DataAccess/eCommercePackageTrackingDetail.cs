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
    
    public partial class eCommercePackageTrackingDetail
    {
        public int eCommercePackageTrackingDetailId { get; set; }
        public int eCommerceShipmentDetailId { get; set; }
        public string TrackingNo { get; set; }
        public string FraytePackageImage { get; set; }
        public string PackageImage { get; set; }
        public Nullable<bool> IsDownloaded { get; set; }
        public Nullable<bool> IsPrinted { get; set; }
        public Nullable<bool> IsFRAYTEAWBPrinted { get; set; }
    }
}
