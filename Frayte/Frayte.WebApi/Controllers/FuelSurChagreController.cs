using Frayte.Services.Business;
using Frayte.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Frayte.WebApi.Controllers
{
    //[Authorize]
    public class FuelSurChargeController : ApiController
    {

        [HttpPost]
        public IHttpActionResult SaveFuelSurCharge(FrayteFuelSurChargeSaveModel FrayteSurCharge)
        {
            FuelSurChargeRepository fscr = new FuelSurChargeRepository();
            FrayteResult result = fscr.SaveFuelSurCharge(FrayteSurCharge);
            return Ok(result);
        }

        [HttpGet]
        public List<FrayteFuelSurcharge> GetFuelSurCharge(int OperationZoneId, int Year)
        {
            List<FrayteFuelSurcharge> fuelmodel = new FuelSurChargeRepository().GetFuelSurCharge(OperationZoneId, Year);
            return fuelmodel;
        }

        [HttpGet]
        [AllowAnonymous]
        public List<FrayteFuelSurChargeList> GetThreeFuelSurCharge(int OperationZoneId, DateTime Year)
        {
            List<FrayteFuelSurChargeList> fuelmodel = new FuelSurChargeRepository().GetThreeMonthFuelRate(OperationZoneId, Year);
            return fuelmodel;
        }

        [HttpPost]
        public IHttpActionResult UpdateFuelSurCharge(List<FrayteFuelSurcharge> fuelSurCharge)
        {
            FrayteResult result = new FuelSurChargeRepository().UpdateFuelSurCharge(fuelSurCharge);
            return Ok(result);
        }

        [HttpGet]
        public List<int> GetDistinctFuelSurchargeYear()
        {
            try
            {
                return new FuelSurChargeRepository().GetDistinctFuelSurchargeYear();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet]
        public List<FrayteMonthYear> GetFuelSurchargeMonthYear(int OperationZoneId)
        {
            try
            {
                return new FuelSurChargeRepository().GetFuelSurchargeMonthYear(OperationZoneId);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet]
        public List<LogisticCompanyList> GetDistinctLogisticCompany(int OperationZoneId)
        {
              return new FuelSurChargeRepository().GetDistinctLogisticCompany(OperationZoneId);
        }
    }
}