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
    
    public partial class BBKShipment
    {
        public int BreakBulkShipmentId { get; set; }
        public int PurchaseOrderId { get; set; }
        public int ShipmentCustomDetailID { get; set; }
        public int ShipmentStatusId { get; set; }
        public string ContentType { get; set; }
        public string ContentExplanation { get; set; }
        public string RestrictionType { get; set; }
        public string RestrictionExplanation { get; set; }
        public string NonDeliveryOption { get; set; }
        public string CutomSignature { get; set; }
        public string TrackingNo { get; set; }
        public bool IsDeclaration { get; set; }
        public int TotCartonPerShipment { get; set; }
        public int TotWeightPerShipment { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public string AllLabels { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string FrayteNumber { get; set; }
    }
}
