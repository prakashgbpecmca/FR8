using Frayte.Services.Business;
using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web;
using System.Web.Http;
using System.Threading;
using RazorEngine;
using RazorEngine.Templating;
using System.Net.Mime;
using System.Data.OleDb;
using System.Data;

namespace Frayte.WebApi.Controllers
{
    [Authorize]
    public class ShipperController : ApiController
    {
        [HttpGet]
        public FrayteShipperReceiver GetShipperDetail(int shipperId)
        {
            FrayteShipperReceiver shippersDetail = new FrayteShipperReceiver();

            shippersDetail = new ShipperRepository().GetShipperDetail(shipperId);

            return shippersDetail;
        }

        [HttpGet]
        public List<FrayteUser> GetShipperList()
        {
            List<FrayteUser> lstShippers = new List<FrayteUser>();

            lstShippers = new ShipperRepository().GetShipperList();

            return lstShippers;
        }

        [HttpGet]
        public List<FrayteUserModel> GetSearchShippers(string shipperName)
        {
            return new ShipperRepository().GetShipperList(shipperName);
        }

        //[HttpPost]
        //public IHttpActionResult SaveShipper(FrayteShipperReceiver shipperDetail)
        //{
        //    bool isNewShipper = shipperDetail.UserId == 0;
        //    FrayteResult result = new ShipperRepository().SaveShipper(shipperDetail);

        //    if (result != null)
        //    {
        //        if (isNewShipper)
        //        {
        //            FrayteUserRepository userRepository = new FrayteUserRepository();

        //            UserLogin userLogin = userRepository.SaveUserLogin((FrayteUser)shipperDetail);

        //            //To Dos: The password will be encrypted form, so here we just need to decrypte it.
        //            string decryptedPassword = userLogin.Password;
        //            userRepository.SendEmail_NewUser(new FrayteLoginUserLogin() { UserName = userLogin.UserName, Password = decryptedPassword });
        //        }
        //        return Ok(result);
        //    }
        //    else
        //    {
        //        return NotFound();
        //    }
        //}

        [HttpPost]
        public IHttpActionResult UploadShippers()
        {
            List<FrayteShipperReceiver> frayteShippers = new List<FrayteShipperReceiver>();

            var httpRequest = HttpContext.Current.Request;

            if (httpRequest.Files.Count > 0)
            {
                HttpFileCollection files = httpRequest.Files;

                HttpPostedFile file = files[0];

                if (!string.IsNullOrEmpty(file.FileName))
                {
                    string connString = "";
                    string filename = DateTime.Now.ToString("MM_dd_yyyy_hh_mm_ss_") + file.FileName;
                    string filepath = HttpContext.Current.Server.MapPath("~/UploadFiles/" + filename);

                    file.SaveAs(filepath);

                    connString = new ShipmentRepository().getExcelConnectionString(filename, filepath);

                    OleDbConnection oledbConn = new OleDbConnection(connString);
                    try
                    {
                        oledbConn.Open();
                        OleDbCommand cmd = new OleDbCommand("SELECT * FROM [Sheet1$]", oledbConn);
                        OleDbDataAdapter oleda = new OleDbDataAdapter();
                        oleda.SelectCommand = cmd;
                        DataSet ds = new DataSet();
                        oleda.Fill(ds, "Shippers");

                        var exceldata = ds.Tables[0];

                        if (exceldata != null && exceldata.Rows.Count > 0)
                        {
                            if (new ShipperRepository().CheckValidExcel(exceldata))
                            {
                                frayteShippers = new ShipperRepository().GetShippers(exceldata);

                                foreach (var shipper in frayteShippers)
                                {
                                    FrayteResult result = new ShipperRepository().SaveShipper(shipper);
                                }
                            }
                            else
                            {
                                return BadRequest("Excel file not valid");
                            }
                        }
                        oledbConn.Close();
                        if ((System.IO.File.Exists(filepath)))
                        {
                            System.IO.File.Delete(filepath);
                        }
                    }
                    catch
                    {
                    }
                    finally
                    {
                        oledbConn.Close();
                        if ((System.IO.File.Exists(filepath)))
                        {
                            System.IO.File.Delete(filepath);
                        }
                    }
                }
            }

            return Ok(frayteShippers);
        }

        [HttpGet]
        public IHttpActionResult DeleteShipper(int shipperId)
        {
            FrayteResult result = new FrayteResult();

            result = new ShipperRepository().DeleteShipper(shipperId);

            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        public FrayteAddress GetShipperMainAddress(int shipperId)
        {
            return new ShipperRepository().GetShipperMainAddress(shipperId);
        }

        [HttpGet]
        public List<FrayteAddress> GetShippeOtherAddresses(int shipperId)
        {
            return new ShipperRepository().GetShippeOtherAddresses(shipperId);
        }

        [HttpGet]
        public List<FrayteUserModel> GetShipperReceivers(int shipperId)
        {
            return new ShipperRepository().GetShipperReceivers(shipperId);
        }

        [HttpPost]
        public IHttpActionResult SaveShipperOtherAddress(FrayteAddress shipperOtherAddress)
        {
            FrayteResult result = new ShipperRepository().SaveShipperOtherAddress(shipperOtherAddress);

            if (result != null && result.Status)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
            
        }
    }
}
