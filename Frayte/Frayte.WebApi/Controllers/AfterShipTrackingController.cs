using Frayte.Services.Business;
using Frayte.Services.Models;
using Frayte.Services.Models.Aftership;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Frayte.WebApi.Controllers
{
    public class AftershipTrackingController : ApiController
    {
        #region Get Tracking Detail of a single tracking number 
        public IHttpActionResult GetTracking(string CarrierName, string TrackingNumber)
        {
            try
            {
                var tracker = new AftershipTrackingRepository().GetTracking(CarrierName, TrackingNumber);
                return Ok(tracker);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return null;
        }

        #endregion

        #region Get tracking detail for per customer

        [HttpGet]
        public IHttpActionResult GetMultipleTrackings(int userId)
        {
            try
            {
                var tracker = new AftershipTrackingRepository().GetMultipleTrackings(userId);
                return Ok(tracker);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return null;
        }

        #endregion

        #region Get tracking detail for per customer

        [HttpPost]
        public IHttpActionResult TrackAndTraceDashboard(TrackAfterShipTracking track)
        {
            try
            {
                var tracker = new AftershipTrackingRepository().GetMultipleTrackings(track);
                return Ok(tracker);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return null;
        }

        #endregion

        #region WebHook

        [HttpPost]
        public IHttpActionResult ProcessWebHook(AftershipWebhookObject webHookDetail)
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("AfterShip hit"));
            if (webHookDetail != null)
            {
                FrayteResult result = new FrayteResult();
                try
                {
                    dynamic json = Newtonsoft.Json.JsonConvert.SerializeObject(webHookDetail);
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(json));

                    new DirectShipmentRepository().UpdateShipmentStatus(webHookDetail.msg.custom_fields.module_type, webHookDetail.msg.tag, webHookDetail.msg.title);
                    new DirectShipmentRepository().UpdateShipmentDetail(webHookDetail.msg.delivery_time, webHookDetail.msg.shipment_delivery_date, webHookDetail.msg.signed_by, webHookDetail.msg.title, webHookDetail.msg.custom_fields.module_type);
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Status updtaed"));

                    if (webHookDetail.msg.custom_fields.module_type == FrayteShipmentServiceType.DirectBooking)
                        result = new AftershipTrackingRepository().SendTrackingStatusEmail(webHookDetail);
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Email sent"));

                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                }
                if (result.Status)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest();
            }

        }

        #endregion
    }
}
