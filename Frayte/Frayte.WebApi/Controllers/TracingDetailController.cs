using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Frayte.Services.Business;
using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using System.Data.Entity.Validation;

namespace Frayte.WebApi.Controllers
{
    [Authorize]
    public class TracingDetailController : ApiController
    {
        [HttpGet]
        public List<TracingComment> GetTracingComments()
        {
            var list = new TracingRepository().GetTracingComment();
            return list;
        }

        [HttpPost]
        public IHttpActionResult SaveTracingComment(FrayteTracingComment comment)
        {
            new TracingRepository().SaveTracingComment(comment);
            return Ok();
        }

        [HttpPost]
        public IHttpActionResult SaveTracingDetail(FrayteShipmentDetailSave ship)
        {
            new TracingRepository().SaveTracingDetail(ship);
            return Ok();
        }

        [HttpGet]
        public List<FrayteShipmentTracingDetail> GetTracingDetail(string Barcode)
        {
            var list = new TracingRepository().GetTracingDetail(Barcode);
            return list;
        }
    }
}
