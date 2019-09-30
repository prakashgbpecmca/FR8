using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.DPD_CH
{
    public class DPDChRequestModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string AuthToken { get; set; }
        public string SendingDepot { get; set; }
        public string Product { get; set; }
        public string ReferenceNumber1 { get; set; }
        public string ReferenceNumber2 { get; set; }
        public string OrderType { get; set; }
        public int Channel { get; set; }
        public string Value { get; set; }
        public string FrayteNumber { get; set; }
        public ContactDetail Shipper { get; set; }
        public ContactDetail Recipient { get; set; }
        public List<DPDPackage> Package { get; set; }
        public int DraftShipmentId { get; set; }
    }

    public class ContactDetail
    {
        public string Contact { get; set; }
        public string Company { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Town { get; set; }   
        public string Country { get; set; }
        public string Postcode { get; set; }
        public string Telephone { get; set; }
    }

    public class DPDPackage
    {
        public string ReferenceNumber1 { get; set; }
        public string ReferenceNumber2 { get; set; }
        public string Volume { get; set; }
        public string Weight { get; set; }
    }
}
