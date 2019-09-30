using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Frayte.Services.Business;
using Frayte.Services.Models;

namespace Frayte.WebApi.Controllers
{
    [AllowAnonymous]
    public class CustomerStaffController : ApiController
    {
        [HttpPost]
        public List<FrayteCustomerStaff> GetInitials(FrayteCustomerStaffTrack track)
        {
            List<FrayteCustomerStaff> staff = new CustomerStaffRepository().GetCustomerStaff(track);
            return staff;
        }

        [HttpGet]
        public FrayteInternalUser GetCustomerStaffDetail(int UserId)
        {
            FrayteInternalUser customer = new CustomerStaffRepository().CustomerStaffDetail(UserId);
            return customer;
        }

        [HttpGet]
        public List<FrayteCustomerAssociatedUser> GetCustomer(string Name, int RoleId)
        {
            List<FrayteCustomerAssociatedUser> customer = new CustomerStaffRepository().GetCustomerDetail(Name, RoleId);
            return customer;
        }

        [HttpGet]
        public FrayteResult RemoveAssociateCustomer(int CustomerStaffDetailId)
        {
            FrayteResult result = new CustomerStaffRepository().RemoveAssociateCustomer(CustomerStaffDetailId);
            return result;
        }

        [HttpGet]
        public FrayteResult RemoveCustomerSatff(int UserId)
        {
            FrayteResult result = new CustomerStaffRepository().RemoveCustomerStaff(UserId);
            return result;
        }
    }
}
