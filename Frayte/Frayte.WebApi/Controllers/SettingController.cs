using Frayte.Services.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Frayte.WebApi.Controllers
{
    [Authorize]
    public class SettingController : ApiController
    {
        [HttpGet]
        public List<string> GetPieceDetailsExcelPath()
        {
            List<string> paths = new List<string>();
            paths.Add(AppSettings.WebApiPath + "UploadFiles/PieceDetails.xlsx");
            paths.Add(AppSettings.WebApiPath + "UploadFiles/UploadCustomer.xlsx");
            paths.Add(AppSettings.WebApiPath + "UploadFiles/UploadShipper.xlsx");
            paths.Add(AppSettings.WebApiPath + "UploadFiles/UploadReceiver.xlsx");
            paths.Add(AppSettings.WebApiPath + "UploadFiles/UploadAgent.xlsx");
            paths.Add(AppSettings.WebApiPath + "UploadFiles/UploadShipmentWithoutServiceExcel.csv");
            paths.Add(AppSettings.WebApiPath + "UploadFiles/UploadShipmentWithServiceExcel.csv");
            paths.Add(AppSettings.WebApiPath + "UploadFiles/UploadShipmentWithoutServiceSample.csv");
            paths.Add(AppSettings.WebApiPath + "UploadFiles/UploadShipmentWithServiceSample.csv");
            return paths;
        }
    }
}
