using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Frayte.Api.Models;
using Frayte.Services;
using Frayte.Services.DataAccess;
using Frayte.Services.Models;

namespace Frayte.Api.Business
{
    public class CustomerRepository
    {
        FrayteEntities dbContext = new FrayteEntities();

        public List<FrayteCustomerLogistic> CustomerLogisticServices(int CustomerId)
        {
            var services = (from uu in dbContext.Users
                            join cl in dbContext.CustomerLogistics on uu.UserId equals cl.UserId
                            join ls in dbContext.LogisticServices on cl.LogisticServiceId equals ls.LogisticServiceId
                            where uu.UserId == CustomerId
                            select new FrayteCustomerLogistic
                            {
                                CustomerName = uu.UserName,
                                CourierName = ls.LogisticCompany,
                                CourierDisplay = ls.LogisticCompanyDisplay,
                                RateType = ls.RateType,
                                RateTypeDisplay = ls.RateTypeDisplay,
                                LogisticType = ls.LogisticType,
                                LogisticTypeDisplay = ls.LogisticTypeDisplay,
                                AddressType = cl.LogisticServiceType
                            }).ToList();

            return services;
        }
    }
}