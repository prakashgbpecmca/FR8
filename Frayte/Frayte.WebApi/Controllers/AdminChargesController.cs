using Frayte.Services.Business;
using Frayte.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Frayte.WebApi.Controllers
{
    [Authorize]
    public class AdminChargesController : ApiController
    {
        public IHttpActionResult CreateCharges(List<AdminChargesTypes> charges)
        {
            return Ok(new AdminChargesRepository().CreateCharges(charges));
        }

        public List<FrayteCustomerSpecificAdminCharges> GetCustomerSpecificAdminCharges()
        {
            return new AdminChargesRepository().GetCustomerSpecificAdminCharges();
        }

        [HttpGet]
        public IHttpActionResult DeleteAdminCharge(int adminChargeId)
        {
            FrayteResult result = new AdminChargesRepository().DeleteAdminCharge(adminChargeId);
            return Ok(result);
        }

        public List<AdminChargesTypes> GetAdminCharges(int userId)
        {
            return new AdminChargesRepository().GetAdminCharges(userId);
        }

        public FrayteCustomerSpecificAdminCharges GetCustomerChanges(int customerId)
        {
            return new AdminChargesRepository().GetCustomerAdminCharges(customerId);
        }
        public List<DirectBookingCustomer> GetCustomersWithoutCharges(int userId, string moduleType, string mode)
        {
            var customers = new AdminChargesRepository().GetCustomersWithoutCharges(userId, moduleType , mode);
            return customers;
        }
        public IHttpActionResult SaveCustomerCharge(FrayteCustomerSpecificAdminCharges charge)
        {
            FrayteResult result = new FrayteResult();
            result = new AdminChargesRepository().SaveCustomerCharge(charge);
            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult RemoveCustomerAdminCharges(int customerId , int userId)
        {
            FrayteResult result = new AdminChargesRepository().RemoveCustomerAdminCharges(customerId , userId);
            return Ok(result);
        }

    }
}
