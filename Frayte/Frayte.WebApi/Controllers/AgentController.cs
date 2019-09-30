using Frayte.Services.Business;
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
    public class AgentController : ApiController
    {
        [HttpGet]
        public FrayteAgent GetDestinatingAgent(int shipmentId)
        {
            FrayteAgent destinatingAgent = new FrayteAgent();
            int? agentId = new AgentRepository().GetDestinatingId(shipmentId);
            if (agentId > 0)
            {
           destinatingAgent = new AgentRepository().GetAgentDetail(agentId);

            }
            
            return destinatingAgent;
        }
        [HttpGet]
        public FrayteShipmentOriginatingAgentDetails GetOriginatingAgent(int shipmentId)
        {
            FrayteShipment shipment  = new FrayteShipment();
            FrayteShipmentOriginatingAgentDetails newAgent = new FrayteShipmentOriginatingAgentDetails();
            FrayteAgent originatingAgent = new FrayteAgent();
            int? agentId = new AgentRepository().GetOriginatingId(shipmentId);
            if (agentId > 0)
            {
                originatingAgent = new AgentRepository().GetAgentDetail(agentId);

            }
            if(originatingAgent!=null)
            {
                newAgent.DeliveryAddress = new FrayteAddress();
                newAgent.DeliveryAddress = originatingAgent.UserAddress;
            }

            shipment = new ShipmentRepository().GetShipmentDetail(shipmentId);
            if (shipment != null)
            {
                newAgent.ShipmentId = shipment.ShipmentId;
            }
            return newAgent;
        }

        [HttpGet]
        public List<FrayteUserModel> GetAgents()
        {
            List<FrayteUserModel> lstAgents = new List<FrayteUserModel>();

            lstAgents = new AgentRepository().GetAgents();

            return lstAgents;
        }

        [HttpGet]
        public List<FrayteAgentModel> GetAgents(int countryId)
        {
            List<FrayteAgentModel> lstAgents = new List<FrayteAgentModel>();

            lstAgents = new AgentRepository().GetAgents(countryId);
            return lstAgents;
        }

        [HttpGet]
        public List<FrayteUser> GetAgentList()
        {
            List<FrayteUser> lstAgents = new List<FrayteUser>();

            lstAgents = new AgentRepository().GetAgentList();

            return lstAgents;
        }

        [HttpGet]
        public FrayteAgent GetAgentDetail(int agentId)
        {
            FrayteAgent agent = new AgentRepository().GetAgentDetail(agentId);

            //return Ok(user);
            return agent;
        }

        [HttpPost]
        public IHttpActionResult SaveAgent(FrayteAgent agentDetail)
        {
            FrayteResult result = new AgentRepository().SaveAgent(agentDetail);

            if (result != null && result.Status)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }

            
        }

        [HttpPost]
        public IHttpActionResult UploadAgents()
        {
            List<FrayteAgent> frayteAgents = new List<FrayteAgent>();

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
                        oleda.Fill(ds, "Agents");

                        var exceldata = ds.Tables[0];

                        if (exceldata != null && exceldata.Rows.Count > 0)
                        {
                            if (new AgentRepository().CheckValidExcel(exceldata))
                            {
                                frayteAgents= new AgentRepository().GetAllAgents(exceldata);

                                foreach (var agent in frayteAgents)
                                {
                                    FrayteResult result = new AgentRepository().SaveAgent(agent);
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

            return Ok(frayteAgents);
        }
        [HttpGet]
        public FrayteResult DeleteAgent(int agentId)
        {
            return new AgentRepository().DeleteAgent(agentId);
        }

        
    }
}
