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
    public class SystemAlertController : ApiController
    {
        [AllowAnonymous]
        [HttpPost]
        public List<FrayteSystemAlert> GetSystemAlerts(TrackSystemAlert trackSystemAlert)
        {
            try
            {
                List<FrayteSystemAlert> list = new List<FrayteSystemAlert>();
                list = new SystemAlertRepository().GetSystemAlerts(trackSystemAlert);
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public FrayteSystemAlert GetSystemAlertDetail(int operationZoneId, int systemAlertId)
        {
            try
            {
                FrayteSystemAlert FrayteSystemAlertResult = new SystemAlertRepository().GetSystemAlertDetail(operationZoneId, systemAlertId);
                return FrayteSystemAlertResult;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        [AllowAnonymous]
        [HttpPost]
        public IHttpActionResult SaveUpdateSystemAlerts(FrayteSystemAlert frayteSystemAlert)
        {
            FrayteResult result = new FrayteResult();
            result = new SystemAlertRepository().SaveUpdateSystemAlerts(frayteSystemAlert);
            if (result.Status)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest();
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public List<FrayteSystemAlert> GetPublicSystemAlerts(int operationZoneId, DateTime? currentDate)
        {
            var list = new SystemAlertRepository().GetSystemAlertsPublic(operationZoneId, currentDate);
            return list;
        }
        [AllowAnonymous]
        [HttpGet]
        public FrayteSystemAlert GetPublicSystemAlertDetail(int operationZoneId, string systemAlertHeading)
        {
            try
            {
                FrayteSystemAlert FrayteSystemAlertResult = new SystemAlertRepository().GetSystemAlertDetailPublic(operationZoneId, systemAlertHeading);
                return FrayteSystemAlertResult;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public FrayteResult SystemAlertHeadingAvailability(int operationZoneId, string systemAlertHeading)
        {
            FrayteResult FrayteSystemAlertAvailabilityResult = new SystemAlertRepository().SystemAlertHeadingAvailability(operationZoneId, systemAlertHeading);
            return FrayteSystemAlertAvailabilityResult;

        }

        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult DeleteSystemAlert(int systemAlertId)
        {
            FrayteResult result = new FrayteResult();
            result = new SystemAlertRepository().DeleteSystemAlert(systemAlertId);
            return Ok(result);
        }
    }
}
