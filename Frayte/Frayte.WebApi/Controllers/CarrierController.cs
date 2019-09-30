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
    public class CarrierController : ApiController
    {
        [HttpGet]
        public List<FrayteCarrier> GetCarrierList()
        {
            List<FrayteCarrier> lstCarrier = new List<FrayteCarrier>();

            lstCarrier = new CarrierRepository().GetCarrierList();

            return lstCarrier;
        }

        [HttpGet]
        public List<FrayteCarrierModel> GetCarrierList(string carrierType)
        {
            List<FrayteCarrierModel> lstCarrier = new List<FrayteCarrierModel>();

            lstCarrier = new CarrierRepository().GetCarrierList(carrierType);

            return lstCarrier;
        }

        [HttpPost]
        public FrayteCarrier SaveCarrier(FrayteCarrier carrier)
        {
            return new CarrierRepository().SaveCarrier(carrier);
        }

        [HttpGet]
        public FrayteResult DeleteCarrier(int carrierId)
        {
            FrayteResult result = new FrayteResult();

            result = new CarrierRepository().DeleteCarrier(carrierId);

            return result;
        }
    }
}
