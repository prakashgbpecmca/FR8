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
    
    public partial class spGet_GetExpressBags_Result
    {
        public Nullable<int> BagId { get; set; }
        public string BagBarCode { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public string ContactName { get; set; }
        public string CountryName { get; set; }
        public Nullable<System.DateTime> CreatedOnUtc { get; set; }
        public string BarCode { get; set; }
        public string Carrier { get; set; }
        public Nullable<int> TotalNoOfShipments { get; set; }
        public Nullable<decimal> TotalWeight { get; set; }
    }
}
