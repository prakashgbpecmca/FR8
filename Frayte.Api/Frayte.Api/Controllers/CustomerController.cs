using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Frayte.Api.Business;
using Frayte.Api.Models;

namespace Frayte.Api.Controllers
{
    public class CustomerController : ApiController
    {
        [HttpPost]
        public List<FrayteCustomerLogistic> GetCustomerLogistic(int CustomerId)
        {
            List<FrayteCustomerLogistic> _logsitic = new List<FrayteCustomerLogistic>();
            _logsitic = new CustomerRepository().CustomerLogisticServices(CustomerId);
            return _logsitic;
        }
    }
}
