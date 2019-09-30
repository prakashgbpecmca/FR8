using Frayte.Services.Business;
using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using Frayte.Services.Models.Express;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Frayte.WebApi.Controllers
{
    public class ExpressShipmentController : ApiController
    {

        [HttpPost]
        public List<ExpressGetShipmentModel> GetExpressShipments(ExpressTrackDirectBooking Track)
        {
            return new ExpresShipmentRepository().GetExpressShipments(Track);            
        }

        [HttpGet]
        public List<ShipmentStatu> GetExpressStatusList(string BookingType)
        {
            return new ExpresShipmentRepository().GetExpressStatusList(BookingType);
        }

        [HttpGet]
        public List<DirectBookingCustomer> GetExpressCustomers(int RoleId, int UserId)
        {
            return new ExpresShipmentRepository().GetExpressCustomers(RoleId, UserId);
        }

        [HttpGet]
        public FrayteResult DeleteExpressShipment(int ExpressShipmentId)
        {
            return new ExpresShipmentRepository().DeleteExpressShipment(ExpressShipmentId);
        }

        [HttpGet]
        public FrayteShipmentTracking GetExpressAWBTracking(int ExpressShipmentId)
        {
            var TrackingType = "Internal";
            return new ExpresShipmentRepository().GetExpressAWBTracking(ExpressShipmentId, TrackingType);
        }

        [HttpGet]
        public FrayteShipmentTracking GetExpressBagTracking(int BagId)
        {
            var TrackingType = "Internal";
            return new ExpresShipmentRepository().GetExpressBagTracking(BagId, TrackingType);
        }

        [HttpGet]
        public int GetExpressTracking(string Number)
        {
            return new ExpresShipmentRepository().GetExpressTracking(Number);
        }

    }
}
