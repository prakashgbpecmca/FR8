using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frayte.Services.Models
{
    public class FrayteCustomerStaff
    {
        public int UserId { get; set; }
        public string ContactName { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public int TotalRows { get; set; }
        public FrayteCountryCode Country { get; set; }
    }

    public class FrayteCustomerStaffTrack
    {
        public int CurrentPage { get; set; }
        public int TakeRows { get; set; }
        public int RoleId { get; set; }
        public int CreatedBy { get; set; }
    }
}