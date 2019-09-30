using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Frayte.Services.Models;
using Frayte.Services.Business;

namespace Frayte.WebApi.Controllers
{
    [Authorize]
    public class HSCodeController : ApiController
    {
        #region Get Non HsCodes Shipments
        [HttpPost]
        public List<NonHsCodeShipments> GetNonHSCodeShipments(TrackHSCodeShipment obj)
        {
            var data = new HSCodeRepository().GetNonHSCodeShipments(obj);
            return data;
        }
        #endregion

        #region Set HSCode
        [HttpGet]
        public FrayteResult SetShipmentHSCode(int eCommerceShipmentDetailid, string HSCode)
        {
            try
            {
                FrayteResult result = new HSCodeRepository().SetShipmentHSCode(eCommerceShipmentDetailid, HSCode);
                return result;
            }
            catch (Exception ex)
            {
                return new FrayteResult
                {
                    Status = false
                };
            }

        }
        #endregion

        #region Get HSCode
        [HttpGet]
        public IHttpActionResult GetHSCodes(string serachTerm, int countryId)
        {
            var data = new HSCodeRepository().GetHSCodes(serachTerm, countryId);
            return Ok(data);
        }
        #endregion
    }
}
