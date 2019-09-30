using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Frayte.Services.Utility;
using Frayte.WebApi.Utility;
using Frayte.Services.Business;
using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using System.Drawing;
using System.Drawing.Drawing2D;
using Spire.Barcode;
using System.Web;

namespace Frayte.WebApi.Controllers
{
    [Authorize]
    public class ShipmentExprysController : ApiController
    {
        [HttpGet]
        public List<ConsignmentBag> GetConsignementBags()
        {
            var lst = new ShipmentExprysRepositery().GetConsignmentBag();
            return lst;
        }

        [HttpGet]
        public List<FrayteAWBShipmentId> GetAllFrayteAWB()
        {
            var list = new ShipmentExprysRepositery().GetFrayteAWB();
            return list;
        }

        [HttpPost]
        public IHttpActionResult SaveConsignmentBag(FrayteConsignment consg)
        {
            new ShipmentExprysRepositery().SaveConsignmentBag(consg);
            return Ok();
        }

        [HttpPost]
        public IHttpActionResult SaveConsigmentBagDetails(FrayteShipmentExpreysBagDetail model)
        {
            new ShipmentExprysRepositery().SaveConsigmentBagDetails(model);
            return Ok();
        }

        [HttpGet]
        public List<FrayteUserShipment> GetExprysShipment()
        {
            var currentShipments = new ShipmentExprysRepositery().GetExprysShipment();
            return currentShipments;
        }

        [HttpGet]
        public IHttpActionResult CreateManifest(int ShipmentBagId)
        {
            SaveShipmentBagBarcode(ShipmentBagId);
            return Ok();
        }

        #region Private Function

        private void SaveShipmentBagBarcode(int ShipmentBagId)
        {
            Random random = new Random();
            string bar = string.Empty;

            for (int i = 1; i < 19; i++)
            {
                bar += random.Next(0, 9).ToString();
            }

            BarcodeSettings settings = new BarcodeSettings();
            string data = string.Empty;
            string type = "Code128";
            short fontSize = 8;
            string font = "SimSun";

            if (ShipmentBagId.ToString().Length == 1)
            {
                data = "FRYT-" + bar + "-00000" + ShipmentBagId;
            }
            else if (ShipmentBagId.ToString().Length == 2)
            {
                data = "FRYT-" + bar + "-0000" + ShipmentBagId;
            }
            else if (ShipmentBagId.ToString().Length == 3)
            {
                data = "FRYT-" + bar + "-000" + ShipmentBagId;
            }
            else if (ShipmentBagId.ToString().Length == 4)
            {
                data = "FRYT-" + bar + "-00" + ShipmentBagId;
            }
            else if (ShipmentBagId.ToString().Length == 5)
            {
                data = "FRYT-" + bar + "-0" + ShipmentBagId;
            }
            else if (ShipmentBagId.ToString().Length == 6)
            {
                data = "FRYT-" + bar + "-" + ShipmentBagId;
            }
            else if (ShipmentBagId.ToString().Length > 6)
            {
                data = "FRYT-" + bar + "-" + ShipmentBagId;
            }

            settings.Data2D = data;
            settings.Data = data;
            settings.Type = (BarCodeType)Enum.Parse(typeof(BarCodeType), type);

            if (fontSize != 0 && fontSize.ToString().Length > 0 && Int16.TryParse(fontSize.ToString(), out fontSize))
            {
                if (font != null && font.Length > 0)
                {
                    settings.TextFont = new System.Drawing.Font(font, fontSize, FontStyle.Bold);
                }
            }
            short barHeight = 15;
            if (barHeight != 0 && barHeight.ToString().Length > 0 && Int16.TryParse(barHeight.ToString(), out barHeight))
            {
                settings.BarHeight = barHeight;
            }

            BarCodeGenerator generator = new BarCodeGenerator(settings);
            Image barcode = generator.GenerateImage();
            //string filePathToSave = GetShipmentDocumentsSavePath(ShipmentBagId);
            barcode.Save(System.Web.Hosting.HostingEnvironment.MapPath(AppSettings.LabelFolder + "barcode_" + ShipmentBagId + ".Png"));

            //Save  ConsignmentBagBarCode
            //new ShipmentExprysRepositery().SaveShipmentBagBarcode(ShipmentBagId, data);
        }

        //private string GetShipmentDocumentsSavePath(int ShipmentBagId)
        //{
        //    string filePathToSave = string.Format(FrayteDocumentPath.ShipmentDocument, ShipmentBagId);
        //    filePathToSave = HttpContext.Current.Server.MapPath(filePathToSave);

        //    if (!System.IO.Directory.Exists(filePathToSave))
        //        System.IO.Directory.CreateDirectory(filePathToSave);

        //    return filePathToSave;
        //}

        #endregion
    }
}
