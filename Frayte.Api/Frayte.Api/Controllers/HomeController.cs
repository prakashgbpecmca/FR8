using Frayte.Api.Business;
using Frayte.Api.Models;
using Frayte.Services.Models.ParcelHub;
using Frayte.Services.Business;
using Frayte.Services.Models;
using Frayte.Services.Models.ApiModal;
using Frayte.Services.DataAccess;
using Frayte.Services.Models.TNT;
using Frayte.Services.Models.DHL;
using Frayte.Services.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Frayte.Api.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            List<FrayteApiErrorDto> item = new ErrorRepository().GetApiErrorCode();
            ViewBag.Title = "API Docs - FRAYTE";
            return View(item);
        }
    }
}
