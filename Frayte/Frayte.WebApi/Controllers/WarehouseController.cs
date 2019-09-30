using Frayte.Services.Business;
using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Frayte.WebApi.Controllers
{
    [Authorize]
    public class WarehouseController : ApiController
    {
        [HttpGet]
        public List<ShipmentWarehouse> GetWarehouseList()
        {
            List<ShipmentWarehouse> lstTimeZone = new List<ShipmentWarehouse>();

            lstTimeZone = new WarehouseRepository().GetWarehouseList();

            return lstTimeZone;
        }

        [HttpGet]
        public List<FrayteWarehouse> GetAllWarehouseList()
        {
            List<FrayteWarehouse> lstFraytehouse = new List<FrayteWarehouse>();

            lstFraytehouse = new WarehouseRepository().GetAllWarehouseList();

            return lstFraytehouse;
        }

        [HttpGet]
        public FrayteWarehouse GetWarehouseDetail(int warehouseId)
        {
            FrayteWarehouse warehouseDetail = new FrayteWarehouse();

            warehouseDetail = new WarehouseRepository().GetWarehouseDetail(warehouseId);

            return warehouseDetail;
        }

        [HttpGet]
        public List<TransportToWarehouse> GetTransportToWarehouse()
        {
            List<TransportToWarehouse> lstTimeZone = new List<TransportToWarehouse>();

            lstTimeZone = new WarehouseRepository().GetTransportToWarehouse();

            return lstTimeZone;
        }

        [HttpPost]
        public FrayteWarehouse SaveWarehouse(FrayteWarehouse warehouse)
        {
            try
            {
                return new WarehouseRepository().SaveWarehouse(warehouse);
            }
            catch (DbEntityValidationException dbEx)
            {
                string validationErrorMsg = "";
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        validationErrorMsg = validationErrorMsg + string.Format("Property: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                    }
                }
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(validationErrorMsg));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }

            return null;
        }

        public class warehouseMap
        {
            public string base64Image { get; set; }            
        }

        [HttpPost]
        public IHttpActionResult SaveWarehouseMap(warehouseMap warehouseMapDetail)
        {
            var bytes = Convert.FromBase64String(warehouseMapDetail.base64Image);
            string filename = "abc_" + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss") + ".png";
            using (var imageFile = new FileStream(@"D:\IRA\frayte\Frayte\Frayte.WebApi\UploadFiles\Warehouse\" + filename, FileMode.Create))
            {
                imageFile.Write(bytes, 0, bytes.Length);
                imageFile.Flush();
            }

            return Ok();
        }

        [HttpGet]
        public FrayteResult DeleteWarehouse(int warehouseId)
        {
            FrayteResult result = new FrayteResult();

            result = new WarehouseRepository().DeleteWarehouse(warehouseId);

            return result;
        }
    }
}
