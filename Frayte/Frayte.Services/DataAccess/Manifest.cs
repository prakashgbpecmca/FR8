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
    
    public partial class Manifest
    {
        public int ManifestId { get; set; }
        public string ManifestName { get; set; }
        public int CustomerId { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string ModuleType { get; set; }
        public string SubModuleType { get; set; }
        public string BarCodeNumber { get; set; }
        public string Status { get; set; }
    }
}
