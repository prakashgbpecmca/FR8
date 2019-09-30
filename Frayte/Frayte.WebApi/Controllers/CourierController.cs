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
    public class CourierController : ApiController
    {
        [HttpGet]
        [AllowAnonymous]
        public List<FrayteCourier> GetCourierList()
        {
            List<FrayteCourier> lstCarrier = new List<FrayteCourier>();

            lstCarrier = new CourierRepository().GetCourierList();

            return lstCarrier;
        }

        [AllowAnonymous]
        [HttpGet]
        public List<FrayteCourier> GetUKCourier()
        {
            List<FrayteCourier> fc = new CourierRepository().GetUKCourier();
            return fc;
        }

        [AllowAnonymous]
        [HttpPost]
        public FrayteCourier SaveCourier(FrayteCourier courier)
        {
            return new CourierRepository().SaveCourier(courier);
        }
        [AllowAnonymous]
        [HttpGet]
        public FrayteResult DeleteCourier(int courierId)
        {
            FrayteResult result = new FrayteResult();

            result = new CourierRepository().DeleteCourier(courierId);

            return result;
        }
    }
}
