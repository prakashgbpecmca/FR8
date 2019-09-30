using Frayte.Services.Models.Tradelane;
using Frayte.Services.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Frayte.WebApi.Controllers
{
    public class TradelaneStaffDashBoardController : ApiController
    {
        [HttpGet]
        public TradelaneStaffDashBoardModel StaffDashBoardInitialDetail()
        {
            return new TradelaneStaffDashBoardRepository().StaffDashBoardInitialDetail();
        }
    }
}
