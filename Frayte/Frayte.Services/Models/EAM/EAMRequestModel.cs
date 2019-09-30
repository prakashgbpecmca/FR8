using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models.EAM
{
    public class EAMRequestModel
    {
        public string Account { get; set; }
        public string Password { get; set; }
        public string CustomerTransactionId { get; set; }
        public string Hawb { get; set; }
        public string Service { get; set; }
        public string Mawb { get; set; }
        public string Date { get; set; }
        public string Company { get; set; }
        public string Contact { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Town { get; set; }
        public string Country { get; set; }
        public string Postcode { get; set; }
        public string telephone { get; set; }
        public string noOfPieces { get; set; }
        public decimal Weight { get; set; }
        public string DoxNonDox { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public string Currency { get; set; }
        public string Agent { get; set; }
        public string Notes { get; set; }
        public string ShipperCompany { get; set; }
        public string ShipperContact { get; set; }
        public string ShipperAddress1 { get; set; }
        public string ShipperAddress2 { get; set; }
        public string ShipperAddress3 { get; set; }
        public string ShipperTown { get; set; }
        public string ShipperCountry { get; set; }
        public string ShipperPostcode { get; set; }
        public string ShipperTelephone { get; set; }
    }

    public class EAMGlobalResponse
    {
        public bool Status { get; set; }
        public string LabelUrl { get; set; }
        public string AWB { get; set; }
        public List<string> ERROR { get; set; }
        public string TrackingNumber { get; set; }
    }
}
