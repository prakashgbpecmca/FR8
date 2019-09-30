using Frayte.Services.Business;
using Frayte.Services.DataAccess;
using Frayte.Services.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Frayte.WebApi.Controllers
{
    [Authorize]
    public class ReceiverController : ApiController
    {
        [HttpGet]
        public List<FrayteUser> GetReceiverList()
        {
            List<FrayteUser> lstReceivers = new List<FrayteUser>();

            lstReceivers = new ReceiverRepository().GetReceiverList();

            return lstReceivers;
        }

        [HttpGet]
        public FrayteShipperReceiver GetReceiverDetail(int receiverId)
        {
            FrayteShipperReceiver receiverDetail = new FrayteShipperReceiver();

            receiverDetail = new ReceiverRepository().GetReceiverDetail(receiverId);

            return receiverDetail;
        }

        [HttpPost]
        public IHttpActionResult SaveReceiver(FrayteShipperReceiver receiverDetail)
        {
            FrayteResult result = new ReceiverRepository().SaveReceiver(receiverDetail);
            // Link Receiver To Shipper
            ReceiverShipper receiverShipper = new ReceiverShipper();
            receiverShipper.ReceiverId = receiverDetail.UserId;
            new ReceiverRepository().SaveReceiverShippers(receiverShipper);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPost]
        public IHttpActionResult SaveLoggedInReceiver(FrayteShipperReceiver receiverDetail)
        {
            FrayteResult result = new FrayteResult();
            result = new ReceiverRepository().SaveReceiver(receiverDetail);
            // Link Receiver To Shipper
            ReceiverShipper receiverShipper = new ReceiverShipper();
            receiverShipper.ReceiverId = receiverDetail.UserId;
            receiverShipper.ShipperId = receiverDetail.ShipperId;
            new ReceiverRepository().SaveReceiverShippers(receiverShipper);

            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPost]
        public IHttpActionResult UploadReceivers()
        {
            List<FrayteShipperReceiver> frayteReceivers = new List<FrayteShipperReceiver>();

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
                        oleda.Fill(ds, "Receivers");

                        var exceldata = ds.Tables[0];

                        if (exceldata != null && exceldata.Rows.Count > 0)
                        {
                            if (new ReceiverRepository().CheckValidExcel(exceldata))
                            {
                                frayteReceivers = new ReceiverRepository().GetReceivers(exceldata);

                                foreach (var receiver in frayteReceivers)
                                {
                                    FrayteResult result = new ReceiverRepository().SaveReceiver(receiver);
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

            return Ok(frayteReceivers);
        }

        [HttpGet]
        public IHttpActionResult DeleteReceiver(int receiverId)
        {
            FrayteResult result = new FrayteResult();

            result = new ReceiverRepository().DeleteReceiver(receiverId);

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
        public List<FrayteUserModel> GetAssignedShippers(int receiverId)
        {
            return new ReceiverRepository().GetAssignedShippers(receiverId);
        }

        [HttpPost]
        public IHttpActionResult SaveReceiverShippers(ReceiverShipper receiverShipper)
        {
            FrayteResult result = new ReceiverRepository().SaveReceiverShippers(receiverShipper);

            if (result.Status)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public IHttpActionResult RemoveReceiverShippers(ReceiverShipper receiverShipper)
        {
            FrayteResult result = new ReceiverRepository().RemoveReceiverShippers(receiverShipper);

            if (result.Status)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        public List<FrayteAddress> GetReceiverOtherAddresses(int receiverId)
        {
            return new ReceiverRepository().GetReceiverOtherAddresses(receiverId);
        }

        [HttpPost]
        public IHttpActionResult SaveReceiverOtherAddress(FrayteAddress receiverOtherAddress)
        {
            FrayteResult result = new ReceiverRepository().SaveReceiverOtherAddress(receiverOtherAddress);

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
