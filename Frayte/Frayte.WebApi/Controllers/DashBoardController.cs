using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Frayte.Services.Business;
using Frayte.Services.Models;

namespace Frayte.WebApi.Controllers
{
    [Authorize]
    public class DashBoardController : ApiController
    {
        [AllowAnonymous]
        [HttpGet]
        public DashBoardModel GetDashBoardInitialDetail()
        {
            return new DashBoardRepository().GetDashBoardInitialDetail();
        }
    }
}
