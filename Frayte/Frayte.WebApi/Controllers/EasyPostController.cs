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
    public class EasyPostController : ApiController
    {
      //  [HttpPost]
        //public IHttpActionResult ProcessWebHook(EasyPostWebHook webHookDetail)
        //{
        //    //foreach (var webHook in webHookDetail.result.tracking_details)
        //    //{
        //    //    if (webHook.status == "Delivered")
        //    //    {
        //    //        int ShipmentId = new ShipmentRepository().GetShipmentIdByTrackingCode(webHook.tracking_code);
        //    //        // send pod mail
        //    //        new ShipmentEmailRepository().SendEmail_E14(ShipmentId);
        //    //    }
        //    //}
        //    if (webHookDetail.result.status == "Delivered")
        //    {
        //        var Shipment = new ShipmentRepository().GetShipmentIdByTrackingCode(webHookDetail.result.tracking_code);
        //        if (Shipment != null)
        //        {
        //            if (Shipment.ShipmentServiceType == FrayteShipmentServiceType.DirectBooking)
        //            {
        //                int DirectShipmentId = new ShipmentRepository().GetDirectShipmentIdByShipmentId(Shipment.ShipmentId);
        //                if (DirectShipmentId > 0)
        //                {
        //                    foreach (var webHook in webHookDetail.result.tracking_details)
        //                    {
        //                        if (webHook.status == "Delivered")
        //                        {
        //                            string DeliveryTime = webHook.datetime;
        //                            string SignedBy = webHookDetail.result.signed_by;
        //                            string DeliveryDate = webHookDetail.result.updated_at;
        //                            //Update Direct Booking
        //                            new ShipmentRepository().UpdateDirectShipmentStatus(DirectShipmentId, (int)FrayteShipmentStatus.Past, SignedBy, DeliveryDate, DeliveryTime);
        //                            //Send mail
        //                          //  new ShipmentEmailRepository().SendDirectBookingDeliveryMail(DirectShipmentId);
        //                        }
        //                    }
        //                }
        //            }
        //            else if (Shipment.ShipmentServiceType == FrayteShipmentServiceType.TradeLaneBooking)
        //            {
        //                int shipmentid = new ShipmentRepository().GetShipmentId(Shipment.ShipmentId);
        //                if (shipmentid > 0)
        //                {
        //                    //Update Shipment
        //                    new ShipmentRepository().UpdateShipmentStatus(shipmentid, (int)FrayteShipmentStatus.Past);
        //                    // send pod mail
        //                    new ShipmentEmailRepository().SendEmail_E14(Shipment.ShipmentId);
        //                }
        //            }
        //        }
        //    }
        //    //dynamic json = Newtonsoft.Json.JsonConvert.SerializeObject(webHookDetail);
        //    //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(json));
        //    //return Ok(json);
        //    return Ok();
        //}
    }


    public class PreviousAttributes
    {
        public string status { get; set; }
    }

    public class TrackingLocation
    {
        public string @object { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public object country { get; set; }
        public string zip { get; set; }
    }

    public class TrackingDetail
    {
        public string @object { get; set; }
        public string message { get; set; }
        public string status { get; set; }
        public string datetime { get; set; }
        public string source { get; set; }
        public TrackingLocation tracking_location { get; set; }
    }

    public class Result
    {
        public string id { get; set; }
        public string @object { get; set; }
        public string mode { get; set; }
        public string tracking_code { get; set; }
        public string status { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string signed_by { get; set; }
        public object weight { get; set; }
        public string est_delivery_date { get; set; }
        public string shipment_id { get; set; }
        public string carrier { get; set; }
        public List<TrackingDetail> tracking_details { get; set; }
        public object carrier_detail { get; set; }
        public List<object> fees { get; set; }
    }

    public class EasyPostWebHook
    {
        public string mode { get; set; }
        public string description { get; set; }
        public PreviousAttributes previous_attributes { get; set; }
        public List<string> pending_urls { get; set; }
        public List<object> completed_urls { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public Result result { get; set; }
        public string id { get; set; }
        public string @object { get; set; }
    }

}